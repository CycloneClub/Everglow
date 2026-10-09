using System.Collections;
using System.Reflection;
using Everglow.Commons.Enums;
using Everglow.Commons.Interfaces;
using Everglow.Commons.Modules;
using Everglow.Commons.ObjectPool;
using ReLogic.Content;

namespace Everglow.Commons.VFX;

public class VFXManager : IVFXManager
{
	public static readonly CodeLayer[] drawLayers =
	{
		CodeLayer.PreDrawFilter,
		CodeLayer.PostDrawProjectiles,
		CodeLayer.PostDrawTiles,
		CodeLayer.PostDrawDusts,
		CodeLayer.PostDrawBG,
		CodeLayer.PostDrawPlayers,
		CodeLayer.PostDrawNPCs,
	};

	private const bool DefaultRt2DIndex = true;

	/// <summary>
	/// GraphicsDevice引用
	/// </summary>
	private GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;

	private Dictionary<(CodeLayer, PipelineIndex), IVisualCollection> lookup =
			new();

	/// <summary>
	/// Pipeline的实例
	/// </summary>
	private List<IPipeline> pipelineInstances = new();

	/// <summary>
	/// Pipeline的Type
	/// </summary>
	private List<Type> pipelineTypes = new();

	private Rt2DVisual renderingRt2D;

	/// <summary>
	/// 保存每一种Visual所需的Pipeline
	/// </summary>
	private List<PipelineIndex> requiredPipeline = new();

	/// <summary>
	/// 当前使用RenderTarget的Index
	/// </summary>
	private bool rt2DIndex = true;

	/// <summary>
	/// 用于Swap的RenderTarget
	/// </summary>
	private ResourceLocker<RenderTarget2D> tempRenderTarget;

	/// <summary> 用绘制层 + 第一个调用的绘制层作为Key来储存List<IVisual> </summary>
	private Dictionary<CodeLayer, List<IVisualCollection>> visuals =
		new();

	/// <summary>
	/// 保存每一种Visual的Type
	/// </summary>
	private Dictionary<Type, int> visualTypes = new()
	{
		[typeof(Rt2DVisual)] = int.MaxValue,
	};

	public VFXManager(IHookManager hookManager, IMainThreadContext mainThread)
	{
		foreach (var layer in drawLayers)
		{
			visuals[layer] = new();
			if (layer is CodeLayer.PostDrawNPCs or CodeLayer.PostDrawBG)
			{
				_ = Ins.HookManager.AddHook(layer, () =>
				{
					Main.spriteBatch.End();
					Draw(layer);
					Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
						SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone,
						null, Main.GameViewMatrix.TransformationMatrix);
				}, $"VFX {layer}");
			}
			else
			{
				_ = Ins.HookManager.AddHook(layer, () => Draw(layer), $"VFX {layer}");
			}
		}
		_ = hookManager.AddHook(CodeLayer.PostUpdateEverything, Update, "VFX Update");
		_ = hookManager.AddHook(CodeLayer.PreSaveAndQuit, Clear, "VFX Clear");
		mainThread.AddTask(() => tempRenderTarget = Ins.RenderTargetPool.GetRenderTarget2D());
		foreach (var visual in Ins.ModuleManager.CreateInstances<IVisual>())
		{
			Register(visual);
		}
	}

	private interface IVisualCollection : IEnumerable<IVisual>
	{
		int Count
		{
			get;
		}

		PipelineIndex Index
		{
			get;
		}

		void Add(IVisual visual);

		void Collect();
	}

	/// <summary>
	/// 包含uTransform，对s0进行采样的普通Shader
	/// </summary>
	public static Asset<Effect> DefaultEffect => ModAsset.Shader2D;

	/// <summary>
	/// 当前RenderTarget
	/// </summary>
	public RenderTarget2D CurrentRenderTarget
	{
		get; private set;
	}

	/// <summary>
	/// 当前的RenderTarget
	/// </summary>
	private RenderTarget2D TrCurrentTarget => rt2DIndex ? Main.screenTarget : Main.screenTargetSwap;

	/// <summary>
	/// 下一个RenderTarget
	/// </summary>
	private RenderTarget2D TrNextTarget => rt2DIndex ? Main.screenTargetSwap : Main.screenTarget;

	public static bool InScreen(Vector2 position, float exRange)
	{
		return Main.screenPosition.X - exRange < position.X && position.X < Main.screenPosition.X + Main.screenWidth + exRange
			&& Main.screenPosition.Y - exRange < position.Y && position.Y < Main.screenPosition.Y + Main.screenHeight + exRange;
	}

	/// <summary>
	/// </summary>
	/// <param name="visual"> </param>
	/// <param name="flag"> 为了避免重复的占位符 </param>
	public void Add(IVisual visual)
	{
		// 将Visual实例加到对应绘制层与第一个Pipeline的位置
		PipelineIndex index = requiredPipeline[visual.VisualType];
		if (lookup.TryGetValue((visual.DrawLayer, index), out var collection))
		{
			collection.Add(visual);
			return;
		}

		GetOrAddCollection(visual.DrawLayer, requiredPipeline[visual.VisualType]).Add(visual);
	}

	public void Clear()
	{
		foreach (var visuals in visuals.Values)
		{
			visuals.Clear();
		}
		lookup.Clear();
	}

	public void Dispose()
	{
		Ins.MainThread.AddTask(() =>
		{
			foreach (var pipeline in pipelineInstances)
			{
				pipeline.Unload();
			}
			tempRenderTarget?.Release();
			Ins.Batch.Dispose();
		});
		GC.SuppressFinalize(this);
	}

	public void Draw(CodeLayer layer)
	{
		var visuals = this.visuals[layer];
		int nextPipelineIndex = -1;
		foreach (var innerVisuals in visuals)
		{
			var pipelineIndex = innerVisuals.Index;
			var visibles = innerVisuals.Where(v => v.Visible && v.Active);
			if (!visibles.Any())
			{
				continue;
			}

			if (Ins.VisualQuality.High)
			{
				if (pipelineIndex.next != null && pipelineIndex.next.index != nextPipelineIndex)
				{
					nextPipelineIndex = pipelineIndex.next.index;
					var locker = Ins.RenderTargetPool.GetRenderTarget2D();
					SetRenderTarget(locker.Resource);
					renderingRt2D = new Rt2DVisual(locker);
					lookup[(layer, pipelineIndex.next)].Add(renderingRt2D);
				}
				else if (pipelineIndex.next == null && nextPipelineIndex != -1)
				{
					nextPipelineIndex = -1;
					SetRenderTarget(TrNextTarget);
					Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
					Main.spriteBatch.Draw(TrCurrentTarget, Vector2.Zero, Color.White);
					Main.spriteBatch.End();
					rt2DIndex = !rt2DIndex;
				}
			}

			pipelineInstances[pipelineIndex.index].Render(visibles);
		}

		if (Ins.VisualQuality.High && TrCurrentTarget != Main.screenTarget)
		{
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
			SetRenderTarget(Main.screenTarget);
			Main.spriteBatch.Draw(TrCurrentTarget, Vector2.Zero, Color.White);
			rt2DIndex = DefaultRt2DIndex;
			Main.spriteBatch.End();
		}
	}

	public void Collect()
	{
		foreach (var (layer, visuals) in visuals)
		{
			for (int i = 0; i < visuals.Count; i++)
			{
				visuals[i].Collect();

				if (visuals[i].Count == 0)
				{
					_ = lookup.Remove((layer, visuals[i].Index));
					visuals.RemoveAt(i--);
				}
			}

			var waitToAdd = new List<PipelineIndex>();
			for (int i = 0; i < visuals.Count; i++)
			{
				var next = visuals[i].Index.next;
				while (next != null)
				{
					waitToAdd.Add(next);
					next = next.next;
				}
			}

			foreach (var index in waitToAdd)
			{
				_ = GetOrAddCollection(layer, index);
			}
		}
	}

	/// <summary>
	/// 获得一种Pipeline的下标，若没有此Pipeline便创建此Pipeline
	/// </summary>
	/// <param name="pipelineType"> </param>
	/// <returns> </returns>
	public int GetOrCreatePipeline(Type pipelineType)
	{
		if (pipelineTypes.Contains(pipelineType))
		{
			return pipelineTypes.IndexOf(pipelineType);
		}

		pipelineTypes.Add(pipelineType);
		var pipeline = (IPipeline)Activator.CreateInstance(pipelineType);
		pipeline.Load();
		pipelineInstances.Add(pipeline);
		return pipelineTypes.Count - 1;
	}

	/// <summary>
	/// 获得Visual的Type
	/// </summary>
	/// <param name="visual"> </param>
	/// <returns> </returns>
	public int GetVisualType(IVisual visual)
	{
		return visualTypes[visual.GetType()];
	}

	public void ModifyPipeline<T>(params Type[] pipelines)
		where T : IVisual
	{
		// TODO 未进行测试
		requiredPipeline[visualTypes[typeof(T)]] = new PipelineIndex(pipelines.Select(i => GetOrCreatePipeline(i)));
	}

	/// <summary>
	/// 注册一个Visual
	/// </summary>
	/// <param name="visual"> </param>
	/// <exception cref="Exception"> 该Visual未绑定任何Pipeline </exception>
	public void Register(IVisual visual)
	{
		Type type = visual.GetType();
		visualTypes.Add(type, requiredPipeline.Count);
		if (type.IsDefined(typeof(PipelineAttribute)))
		{
			var pipelines = type.GetCustomAttribute<PipelineAttribute>().types;
			if (pipelines.Length == 0)
			{
				Debug.Fail("Not bind any pipeline");
				throw new Exception("Not bind any pipeline");
			}
			requiredPipeline.Add(new PipelineIndex(pipelines.Select(i => GetOrCreatePipeline(i))));
		}
		else
		{
			requiredPipeline.Add(null);
		}
	}

	public void SetRenderTarget(RenderTarget2D rt2D)
	{
		graphicsDevice.SetRenderTarget(rt2D);
		graphicsDevice.Clear(Color.Transparent);
		CurrentRenderTarget = rt2D;
	}

	public void SwapRenderTarget()
	{
		if (CurrentRenderTarget == TrCurrentTarget)
		{
			graphicsDevice.SetRenderTarget(TrNextTarget);
			graphicsDevice.Clear(Color.Transparent);
			CurrentRenderTarget = TrNextTarget;
			rt2DIndex = !rt2DIndex;
		}
		else
		{
			graphicsDevice.SetRenderTarget(tempRenderTarget.Resource);
			graphicsDevice.Clear(Color.Transparent);
			CurrentRenderTarget = tempRenderTarget.Resource;
			(renderingRt2D.locker, tempRenderTarget) = (tempRenderTarget, renderingRt2D.locker);
		}
	}

	public void Update()
	{
		foreach (var visuals in visuals.Values)
		{
			foreach (var list in visuals)
			{
				foreach (var visual in list)
				{
					if (visual.Active)
					{
						visual.Update();
					}
				}
			}
		}
	}

	public int VisualType<T>()
	{
		return visualTypes[typeof(T)];
	}

	private IVisualCollection GetOrAddCollection(CodeLayer layer, PipelineIndex index, bool first = true)
	{
		if (lookup.TryGetValue((layer, index), out var collection))
		{
			return collection;
		}

		collection = first ? new VisualCollection(index) : new SingleVisual(index);
		if (index.next == null)
		{
			visuals[layer].Add(collection);
			lookup[(layer, index)] = collection;
			return collection;
		}
		lookup[(layer, index)] = collection;

		// 保证next在index后面
		visuals[layer].Insert(
			Math.Min(
				visuals[layer].IndexOf(GetOrAddCollection(layer, index.next, false)),
				visuals[layer].FindIndex(v => v.Index.GetDepth() > index.GetDepth()) + 1), collection);
		return collection;
	}

	[ModuleHideType]
	internal class Rt2DVisual : Visual
	{
		public ResourceLocker<RenderTarget2D> locker;

		public Rt2DVisual(ResourceLocker<RenderTarget2D> locker)
		{
			this.locker = locker;
		}

		public override CodeLayer DrawLayer => throw new InvalidOperationException("Don't use this manually!");

		public override void Draw()
		{
			throw new NotImplementedException();
		}
	}

	private class SingleVisual : IVisualCollection
	{
		public PipelineIndex index;

		public IVisual visual;

		public SingleVisual(PipelineIndex index)
		{
			this.index = index;
		}

		public int Count => visual == null ? 0 : 1;

		public PipelineIndex Index => index;

		public void Add(IVisual visual)
		{
			this.visual = visual;
		}

		public void Collect()
		{
			if (!visual.Active)
			{
				visual = null;
			}
		}

		public IEnumerator<IVisual> GetEnumerator()
		{
			if (visual != null)
			{
				yield return visual;
			}

			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	private class VisualCollection : IVisualCollection
	{
		private const int FLUSH_COUNT = 50;

		/// <summary>
		/// 比较器
		/// </summary>
		private static VisualComparer compare = new();

		private PipelineIndex index;

		private SortedSet<IVisual> visuals;

		public VisualCollection(PipelineIndex index)
		{
			visuals = new SortedSet<IVisual>(compare);
			this.index = index;
		}

		public int Count => visuals.Count;

		public PipelineIndex Index => index;

		public void Add(IVisual visual)
		{
			if (visuals.Count % FLUSH_COUNT == 0)
			{
				Collect();
			}

			_ = visuals.Add(visual);
		}

		public void Collect()
		{
			int b = visuals.RemoveWhere(visual => !visual.Active);
		}

		public IEnumerator<IVisual> GetEnumerator()
		{
			return visuals.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private class VisualComparer : Comparer<IVisual>
		{
			/// <summary>
			/// 按照Type从小到大排序，允许重复
			/// </summary>
			/// <param name="x"> </param>
			/// <param name="y"> </param>
			/// <returns> </returns>
			public override int Compare(IVisual x, IVisual y)
			{
				if (x == y)
				{
					return 0;
				}

				var diff = x.VisualType - y.VisualType;
				return diff == 0 ? x.GetHashCode() - y.GetHashCode() : diff;
			}
		}
	}
}
