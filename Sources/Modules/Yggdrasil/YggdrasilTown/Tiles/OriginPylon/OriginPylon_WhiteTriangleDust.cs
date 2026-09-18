using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.Common.Projectiles;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using SubworldLibrary;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.OriginPylon;

[Pipeline(typeof(WCSPipeline))]
public class OriginalPylon_VFX : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawBG;

	public override void OnSpawn()
	{
		Texture = ModAsset.StoneBridge_fence.Value;
	}

	public override void Update()
	{
		//if (!SubworldSystem.IsActive<YggdrasilWorld>())
		//{
		//	Active = false;
		//}
		base.Update();
	}

	public override void Draw()
	{
		if (!Ins.VisualQuality.Low)
		{
			float i = Position.X / 16f;
			float j = Position.Y / 16f;
			var bars = new List<Vertex2D>();
			Color color0 = Color.White * 0.4f;
			color0.A = 0;
			bars = new List<Vertex2D>()
			{
				new Vertex2D(new Vector2(i - 11, j - 8.3f) * 16, color0, new Vector3(0, 0, 0)),
				new Vertex2D(new Vector2(i + 21, j - 8.3f) * 16, color0, new Vector3(1, 0, 0)),
				new Vertex2D(new Vector2(i - 11, j + 17) * 16, color0, new Vector3(0, 1, 0)),
				new Vertex2D(new Vector2(i + 21, j + 17) * 16, color0, new Vector3(1, 1, 0)),
			};
			Ins.Batch.Draw(ModAsset.OriginPylon_glow.Value, bars, PrimitiveType.TriangleStrip);

			float timeValue = (float)(Main.time * 0.0004f);
			var crack = new List<Vertex2D>();
			for (int k = 0; k < 6; k++)
			{
				Vector2 v0 = new Vector2(290, 0).RotatedBy(k);
				Vector2 v0Left = Vector2.Normalize(v0.RotatedBy(MathHelper.PiOver2)) * 60f;
				Vector2 v0Normal = Vector2.Normalize(v0) * 20;
				Vector2 pylonCenter = new Vector2(i + 5, j + 7) * 16 + v0;
				crack.Add(pylonCenter + v0Left + v0Normal, Color.Transparent, new Vector3(timeValue * 0.4f + k / 7f, 0.4f, 0));
				crack.Add(pylonCenter + v0Left - v0Normal, Color.Transparent, new Vector3(timeValue * 0.4f + k / 7f, 0.6f, 1));
				crack.Add(pylonCenter + v0Normal, new Color(155, 255, 0, 0), new Vector3(timeValue * 0.4f + 0.1f + k / 7f, 0.4f, 0));
				crack.Add(pylonCenter - v0Normal, new Color(155, 255, 0, 0), new Vector3(timeValue * 0.4f + 0.1f + k / 7f, 0.6f, 1));
				crack.Add(pylonCenter - v0Left + v0Normal, Color.Transparent, new Vector3(timeValue * 0.4f + 0.2f + k / 7f, 0.4f, 0));
				crack.Add(pylonCenter - v0Left - v0Normal, Color.Transparent, new Vector3(timeValue * 0.4f + 0.2f + k / 7f, 0.6f, 1));
			}
			Ins.Batch.End();
			Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.Default, SamplerState.PointWrap, RasterizerState.CullNone);
			Effect noise = ModAsset.PurpleCrack.Value;
			noise.CurrentTechnique.Passes["Test"].Apply();
			Ins.Batch.Draw(Commons.ModAsset.Noise_perlin.Value, crack, PrimitiveType.TriangleStrip);
			Ins.Batch.End();
			Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.Default, SamplerState.PointWrap, RasterizerState.CullNone);
			Effect effect = VFXManager.DefaultEffect.Value;
			effect.Parameters["uTransform"].SetValue(
				Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) *
				Main.GameViewMatrix.TransformationMatrix *
				Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1));
			effect.CurrentTechnique.Passes[0].Apply();

			var chain = new List<Vertex2D>();
			for (int k = 0; k < 6; k++)
			{
				Vector2 v0 = new Vector2(290, 0).RotatedBy(k);
				Vector2 v0Left = Vector2.Normalize(v0.RotatedBy(MathHelper.PiOver2)) * 4f;
				Vector2 v0Normal = -Vector2.Normalize(v0) * 12;
				Vector2 pylonCenter = new Vector2(i + 5, j + 7) * 16 + v0;
				chain.Add(pylonCenter + v0Left, Color.Transparent, new Vector3(0, 0, 0));
				chain.Add(pylonCenter - v0Left, Color.Transparent, new Vector3(0, 1, 0));
				for (int l = 1; l < 24; l++)
				{
					chain.Add(pylonCenter + v0Left + v0Normal * l, Color.White, new Vector3(l * 0.5f, 0, 0));
					chain.Add(pylonCenter - v0Left + v0Normal * l, Color.White, new Vector3(l * 0.5f, 1, 0));
				}
				chain.Add(pylonCenter + v0Left + v0Normal * 24, Color.Transparent, new Vector3(12, 0, 0));
				chain.Add(pylonCenter - v0Left + v0Normal * 24, Color.Transparent, new Vector3(12, 1, 0));
			}
			Ins.Batch.Draw(ModAsset.OriginPylon_chain.Value, chain, PrimitiveType.TriangleStrip);

			bars = new List<Vertex2D>()
			{
				new Vertex2D(new Vector2(i - 4, j + 9) * 16, color0 * 0.0f, new Vector3(0.1f, timeValue, 0)),
				new Vertex2D(new Vector2(i - 4, j - 10) * 16, color0 * 0.0f, new Vector3(0.1f, timeValue, 0)),
				new Vertex2D(new Vector2(i + 1, j + 9) * 16, color0 * 0.7f, new Vector3(0.2f, timeValue, 0)),
				new Vertex2D(new Vector2(i + 1, j - 28) * 16, color0 * 0.0f, new Vector3(0.2f, timeValue, 0)),
				new Vertex2D(new Vector2(i + 8, j + 9) * 16, color0 * 0.7f, new Vector3(0.4f, timeValue, 0)),
				new Vertex2D(new Vector2(i + 8, j - 28) * 16, color0 * 0.0f, new Vector3(0.4f, timeValue, 0)),
				new Vertex2D(new Vector2(i + 13, j + 9) * 16, color0 * 0.0f, new Vector3(0.5f, timeValue, 0)),
				new Vertex2D(new Vector2(i + 13, j - 10) * 16, color0 * 0.0f, new Vector3(0.5f, timeValue, 0)),
			};
			Ins.Batch.Draw(Commons.ModAsset.Noise_perlin.Value, bars, PrimitiveType.TriangleStrip);

			bars = new List<Vertex2D>()
			{
				new Vertex2D(new Vector2(i - 4, j + 9) * 16, color0 * 0.0f, new Vector3(0.1f, timeValue, 0)),
				new Vertex2D(new Vector2(i - 4, j + 10) * 16, color0 * 0.0f, new Vector3(0.1f, timeValue, 0)),
				new Vertex2D(new Vector2(i + 1, j + 9) * 16, color0 * 0.7f, new Vector3(0.2f, timeValue, 0)),
				new Vertex2D(new Vector2(i + 1, j + 28) * 16, color0 * 0.0f, new Vector3(0.2f, timeValue, 0)),
				new Vertex2D(new Vector2(i + 8, j + 9) * 16, color0 * 0.7f, new Vector3(0.4f, timeValue, 0)),
				new Vertex2D(new Vector2(i + 8, j + 28) * 16, color0 * 0.0f, new Vector3(0.4f, timeValue, 0)),
				new Vertex2D(new Vector2(i + 13, j + 9) * 16, color0 * 0.0f, new Vector3(0.5f, timeValue, 0)),
				new Vertex2D(new Vector2(i + 13, j + 10) * 16, color0 * 0.0f, new Vector3(0.5f, timeValue, 0)),
			};
			Ins.Batch.Draw(Commons.ModAsset.Noise_perlin.Value, bars, PrimitiveType.TriangleStrip);

			bars = new List<Vertex2D>()
			{
				new Vertex2D(new Vector2(i - 11, j - 8.3f) * 16, Color.White, new Vector3(0, 0, 0)),
				new Vertex2D(new Vector2(i + 21, j - 8.3f) * 16, Color.White, new Vector3(1, 0, 0)),
				new Vertex2D(new Vector2(i - 11, j + 17) * 16, Color.White, new Vector3(0, 1, 0)),
				new Vertex2D(new Vector2(i + 21, j + 17) * 16, Color.White, new Vector3(1, 1, 0)),
			};
			Ins.Batch.Draw(ModAsset.OriginPylon.Value, bars, PrimitiveType.TriangleStrip);
		}
	}
}
