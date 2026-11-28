using Everglow.Commons.Enums;
using Everglow.Commons.Interfaces;

namespace Everglow.Commons.ObjectPool;

/// <summary>
/// 管理屏幕大小的 RenderTarget 的对象池，带有自动清理机制防止 GPU 内存泄漏。
/// </summary>
public class RenderTargetPool
{
	private const int MaxPoolSize = 20;

	private List<RenderTarget2D> renderTargetsPool;
	private LinkedList<int> renderTargetsFreeList;
	private GraphicsDevice graphicsDevice;
	private Dictionary<int, int> lastAccessFrame;
	private int frameCounter;

	public RenderTargetPool(IHookManager hookManager)
	{
		renderTargetsPool = new List<RenderTarget2D>();
		renderTargetsFreeList = new LinkedList<int>();
		lastAccessFrame = new Dictionary<int, int>();
		graphicsDevice = Ins.Device;
		frameCounter = 0;

		hookManager.AddHook(CodeLayer.ResolutionChanged, Main_OnResolutionChanged);
	}

	/// <summary>
	/// 屏幕大小变化时刷新 RT 的尺寸和清理过剩的 RT
	/// </summary>
	/// <param name="size"></param>
	private void Main_OnResolutionChanged(Vector2 size)
	{
		lock (this)
		{
			int poolSize = renderTargetsPool.Count;
			for (int i = 0; i < poolSize; i++)
			{
				if (renderTargetsPool[i] != null)
				{
					renderTargetsPool[i].Dispose();
					renderTargetsPool[i] = null;
				}
				renderTargetsPool[i] = new RenderTarget2D(
					graphicsDevice,
					(int)size.X,
					(int)size.Y);
			}
		}
	}

	/// <summary>
	/// 从对象池中获取1个 RT，并且返回句柄
	/// </summary>
	/// <returns></returns>
	public ResourceLocker<RenderTarget2D> GetRenderTarget2D()
	{
		int index = GetNextFreeIndexAndOccupy();
		return new ResourceLocker<RenderTarget2D>(renderTargetsPool[index], () =>
	   {
		   ReleaseResourceAt(index);
	   });
	}

	/// <summary>
	/// 从对象池中获取 size 那么多个 RT 对象，并且返回一个句柄
	/// </summary>
	/// <param name="size"></param>
	/// <returns></returns>
	public ResourceLocker<RenderTarget2D[]> GetRenderTarget2DArray(int size)
	{
		List<int> indices = new();
		List<RenderTarget2D> renderTargets = new();
		for (int i = 0; i < size; i++)
		{
			int index = GetNextFreeIndexAndOccupy();
			indices.Add(index);
			renderTargets.Add(renderTargetsPool[index]);
		}

		return new ResourceLocker<RenderTarget2D[]>(renderTargets.ToArray(), () =>
		{
			foreach (var i in indices)
			{
				ReleaseResourceAt(i);
			}
		});
	}

	/// <summary>
	/// 获取下一个空位的ID，并且占有它。如果池大小超过限制，会释放最久未使用的 RT。
	/// </summary>
	/// <returns></returns>
	private int GetNextFreeIndexAndOccupy()
	{
		lock (this)
		{
			frameCounter++;

			// 如果 freelist 有空位，直接复用
			if (renderTargetsFreeList.Count > 0)
			{
				int index = renderTargetsFreeList.First.Value;
				renderTargetsFreeList.RemoveFirst();
				lastAccessFrame[index] = frameCounter;
				return index;
			}

			// 如果池已满，释放最久未使用的 RT
			if (renderTargetsPool.Count >= MaxPoolSize)
			{
				// 在 freelist 中找到最久未使用的 RT 并删除它
				int lruIndex = -1;
				int lruFrame = int.MaxValue;

				foreach (var kvp in lastAccessFrame)
				{
					if (kvp.Value < lruFrame && renderTargetsFreeList.Contains(kvp.Key))
					{
						lruFrame = kvp.Value;
						lruIndex = kvp.Key;
					}
				}

				if (lruIndex >= 0)
				{
					// 从 freelist 中移除并释放该 RT
					renderTargetsFreeList.Remove(lruIndex);
					if (renderTargetsPool[lruIndex] != null)
					{
						renderTargetsPool[lruIndex].Dispose();
						renderTargetsPool[lruIndex] = null;
					}
					lastAccessFrame.Remove(lruIndex);

					// 复用该插槽
					renderTargetsPool[lruIndex] = new RenderTarget2D(
						graphicsDevice,
						graphicsDevice.Viewport.Width,
						graphicsDevice.Viewport.Height);
					lastAccessFrame[lruIndex] = frameCounter;
					return lruIndex;
				}
			}

			// 池未满，分配新的 RT
			int newIndex = renderTargetsPool.Count;
			renderTargetsPool.Add(new RenderTarget2D(
				graphicsDevice,
				graphicsDevice.Viewport.Width,
				graphicsDevice.Viewport.Height));
			lastAccessFrame[newIndex] = frameCounter;
			return newIndex;
		}
	}

	private void ReleaseResourceAt(int index)
	{
		lock (this)
		{
			renderTargetsFreeList.AddFirst(index);
		}
	}
}
