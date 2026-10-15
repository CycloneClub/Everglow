using Everglow.Commons.Graphics;
using Everglow.Commons.VFX;

namespace Everglow.Commons.VFX.CommonVFXDusts;

public class FogVFX : MEACVFX
{
	public override void SetDefault()
	{
		MaxTimeLeft = 60;
		TexPath = ModAsset.FBM_Mod;
		Alpha = 0;
	}

	public bool Subtract = false;

	public override void AI()
	{
		if (Ai0 == 0)
		{
			Velocity *= 0.95f;
			if (Ai1 == 0)
			{
				Ai1 = Main.rand.NextFloatDirection() * 0.05f;
			}
			Rotation += Ai1;
			if (TimeLeft > 15)
			{
				MathHelper.Lerp(Alpha, 1f, 0.06f);
			}
			else
			{
				MathHelper.Lerp(Alpha, 0f, 0.06f);
			}
		}
		if (Ai0 == 1)
		{
			Velocity *= 0.95f;
			if (Ai1 == 0)
			{
				Ai1 = Main.rand.NextFloatDirection() * 0.05f;
			}
			Rotation += Ai1;
			if (TimeLeft > MaxTimeLeft - 20)
			{
				MathHelper.Lerp(Alpha, 1f, 0.1f);
			}
			else
			{
				MathHelper.Lerp(Alpha, 0f, 0.015f);
			}
		}
	}

	public override void Draw()
	{
		// Main.NewText("Draw", Color.Red);
		// base.Draw();
		// substract = false;
		if (Subtract)
		{
			Ins.Batch.End();
			Ins.Batch.Begin(CustomBlendStates.Subtract);
		}

		Ins.Batch.Draw(Texture, Center - Main.screenPosition, null, DrawColor * Alpha, Rotation, Texture.Size() / 2, Scale, SpriteEffects.None);
		/*
        Vector2 Position = Center - Main.screenPosition;
        Color color = drawColor;

        Vertex2D[] bars = new Vertex2D[]
        {
            new Vertex2D(Position, color, new(0,0,0)),
            new Vertex2D(Position+new Vector2(100,0), color,  new(1,0,0)),
            new Vertex2D(Position+new Vector2(0,100), color,  new(0,1,0)),
            new Vertex2D(Position+new Vector2(100,100), color,  new(1,1,0))
        };
        Ins.Batch.BindTexture<Vertex2D>(Texture).Draw(bars, PrimitiveType.TriangleStrip);*/

		if (Subtract)
		{
			Ins.Batch.End();
			Ins.Batch.Begin(BlendState.AlphaBlend);
		}
	}

	private struct VFX2D : IVertexType
	{
		public Color Color;

		public Vector2 Position;

		public Vector2 TexCoord;

		public VFX2D(Vector2 position, Color color, Vector2 texCoord)
		{
			this.Position = position;
			this.Color = color;
			this.TexCoord = texCoord;
		}

		public VertexDeclaration VertexDeclaration => new(
											new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
											new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
											new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0));
	}
}
