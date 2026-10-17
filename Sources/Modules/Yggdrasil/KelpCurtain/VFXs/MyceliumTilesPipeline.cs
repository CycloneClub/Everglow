using Everglow.Commons.VFX.CommonVFXDusts;

namespace Everglow.Yggdrasil.KelpCurtain.VFXs;

public class MyceliumTilesPipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.MyceliumTilesShader;

		Ins.Batch.RegisterVertex<Vertex2DMycelium>(2048, 3072);
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_Spider_Net.Value);
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointClamp, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}

public struct Vertex2DMycelium : IVertexType
{
	private static VertexDeclaration _vertexDeclaration = new(new VertexElement[4]
	{
		new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
		new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
		new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0),
		new VertexElement(24, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 1),
	});

	public Vector2 position;
	public Color color;
	public Vector3 texCoord;
	public Vector3 texCoord2;

	public Vertex2DMycelium(Vector2 position, Color color, Vector3 texCoord, Vector3 texCoord2)
	{
		this.position = position;
		this.color = color;
		this.texCoord = texCoord;
		this.texCoord2 = texCoord2;
	}

	public override string ToString()
	{
		return $"[{position}, {color}, {texCoord}, {texCoord2}]";
	}

	public VertexDeclaration VertexDeclaration => _vertexDeclaration;
}
