using Everglow.Commons.DataStructures;
using Everglow.Commons.Enums;
using Everglow.Commons.Interfaces;

namespace Everglow.Commons.VFX.Pipelines;

/// <summary>
/// 屏幕空间反射Pipeline,以绘制的图像作为法线贴图反射屏幕前的信息
/// </summary>
public class ScreenReflectionPipeline : Pipeline
{
	private RenderTarget2D screenReflectionScreen; // 反射区域

	public override void Load()
	{
		Ins.MainThread.AddTask(() =>
		{
			AllocateRenderTarget(new Vector2(Main.screenWidth, Main.screenHeight));
		});
		Ins.HookManager.AddHook(CodeLayer.ResolutionChanged, (Vector2 size) =>
		{
			screenReflectionScreen?.Dispose();
			AllocateRenderTarget(size);
		}, "Realloc RenderTarget");
		effect = VFXManager.DefaultEffect;
	}

	private void AllocateRenderTarget(Vector2 size)
	{
		var gd = Main.instance.GraphicsDevice;
		screenReflectionScreen = new RenderTarget2D(gd, (int)size.X, (int)size.Y, false, gd.PresentationParameters.BackBufferFormat, DepthFormat.None);
	}

	public override void BeginRender()
	{
		var graphicsDevice = Main.graphics.GraphicsDevice;
		var spriteBatch = Main.spriteBatch;

		graphicsDevice.SetRenderTarget(Main.screenTargetSwap);

		// 保存原屏幕
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.Default,
			RasterizerState.CullNone);
		spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
		spriteBatch.End();
		List<Player> players = new List<Player>() { Main.LocalPlayer };

		// 外加玩家
		Main.PlayerRenderer.DrawPlayers(Main.Camera, players);

		graphicsDevice.SetRenderTarget(screenReflectionScreen);

		// 以另一种方法绘制保存下来的屏幕
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.Default,
			RasterizerState.CullNone, null, Matrix.Invert(Main.GameViewMatrix.TransformationMatrix));
		spriteBatch.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
		spriteBatch.End();

		graphicsDevice.SetRenderTarget(Main.screenTarget);

		// 绘制原屏幕
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.Default,
			RasterizerState.CullNone);
		spriteBatch.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
		spriteBatch.End();

		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None,
			RasterizerState.CullNone);

		Effect ef = Commons.ModAsset.ScreenReflection_NormalColor.Value;
		var viewport = Main.graphics.GraphicsDevice.Viewport;

		var projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, 500);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0));

		// ef.Parameters["uBaseColor"].SetValue(new Vector3(0.82f, 0.82f, 0.82f));
		ef.Parameters["uFresnelF0"].SetValue(new Vector3(0.17f));
		ef.Parameters["uKs"].SetValue(new Vector3(1.0f));
		ef.Parameters["uScreenDistanceMultipler"].SetValue(Vector2.One);
		ef.Parameters["uViewportSize"].SetValue(new Vector2(
			screenReflectionScreen.Width,
			screenReflectionScreen.Height));
		ef.Parameters["uModel"].SetValue(model);
		ef.Parameters["uMNormal"].SetValue(Matrix.Identity);
		ef.Parameters["uViewProj"].SetValue(Main.GameViewMatrix.TransformationMatrix * projection);

		ef.CurrentTechnique.Passes["Test"].Apply();
		Main.graphics.GraphicsDevice.Textures[0] = screenReflectionScreen;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
	}

	public override void Render(IEnumerable<IVisual> visuals)
	{
		BeginRender();
		foreach (var visual in visuals)
		{
			visual.Draw();
		}
		EndRender();
	}

	public override void EndRender()
	{
		var spriteBatch = Main.spriteBatch;
		spriteBatch.End();
	}

	/// <summary>
	/// 创建一个ABC逆时针组成的三角形面，并且生成其法线信息
	/// </summary>
	/// <param name="A"></param>
	/// <param name="B"></param>
	/// <param name="C"></param>
	/// <returns></returns>
	public static List<CrystalVertex> CreateFace(Vector3 A, Vector3 B, Vector3 C, out Vector3 N)
	{
		List<CrystalVertex> data = new List<CrystalVertex>();
		Vector3 Normal = Vector3.Normalize(Vector3.Cross(B - A, C - A));
		data.Add(new CrystalVertex(A, -Normal));
		data.Add(new CrystalVertex(B, -Normal));
		data.Add(new CrystalVertex(C, -Normal));
		N = -Normal;
		return data;
	}

	public struct CrystalVertex : IVertexType
	{
		private static VertexDeclaration _vertexDeclaration = new VertexDeclaration(new VertexElement[2]
		{
				new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
				new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
		});

		public Vector3 Position;
		public Vector3 Normal;

		public CrystalVertex(Vector3 position, Vector3 normal)
		{
			Position = position;
			Normal = normal;
		}

		public VertexDeclaration VertexDeclaration
		{
			get
			{
				return _vertexDeclaration;
			}
		}
	}

	public struct MirrorFaceVertex : IVertexType
	{
		private static VertexDeclaration _vertexDeclaration = new VertexDeclaration(new VertexElement[3]
		{
			new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
			new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0),
			new VertexElement(16, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0),
		});

		public Vector3 Position;
		public Color Color;
		public Vector3 Normal;

		public MirrorFaceVertex(Vector3 position, Color color, Vector3 normal)
		{
			Position = position;
			Color = color;
			Normal = normal;
		}

		public VertexDeclaration VertexDeclaration
		{
			get
			{
				return _vertexDeclaration;
			}
		}
	}
}
