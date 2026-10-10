using Everglow.Myth.Common;
using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee;

internal class WorldHit : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 68;
		Projectile.height = 68;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 120;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 3;
	}

	public override void OnSpawn(IEntitySource source)
	{
		for (int f = 0; f < Projectile.ai[0] * 30f; f++)
		{
			var d = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 0, 0, DustID.GemEmerald, 0, 0, 0, default, Main.rand.NextFloat(2f));
			d.velocity = new Vector2(0, 25).RotatedByRandom(6.283) * Main.rand.NextFloat(1f);
			d.noGravity = true;
		}
	}

	public override void AI()
	{
		Projectile.velocity *= 0;
		for (int i = 0; i < 23; i++)
		{
			if (DrawLine[i, 0] == Vector2.Zero)
			{
				for (int j = 0; j < 18; j++)
				{
					DrawLine[i, j] = Projectile.Center;
				}
				DrawLineVelocity[i] = new Vector2(Main.rand.NextFloat(17f, 27f), 0).RotatedByRandom(6.283) * Projectile.ai[0];
			}
			DrawLine[i, 0] += DrawLineVelocity[i];
			DrawLineVelocity[i] *= 0.9f;
			for (int j = 17; j > 0; j--)// 记录每一组流星火位置
			{
				DrawLine[i, j] = DrawLine[i, j - 1];
			}
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		return false;
	}

	private Effect ef;
	internal float radius = 0;
	internal float FirstRo = 0;
	internal float SecondRo = 0;
	internal Vector2[,] DrawLine = new Vector2[23, 18];
	internal Vector2[] DrawLineVelocity = new Vector2[23];

	public override void PostDraw(Player player, Color lightColor)
	{
		if (FirstRo == 0)
		{
			FirstRo = Main.rand.NextFloat(0, 6.283f);
		}

		if (SecondRo == 0)
		{
			SecondRo = Main.rand.NextFloat(0, 6.283f);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
		var bars = new List<Vertex2D>();
		ef = MythContent.QuickEffect("Effects/FadeCurseGreen");
		float widx = Projectile.timeLeft / 120f;
		float widxM = 1f - widx;
		radius = (float)(Math.Sqrt(5 * widxM) * 60) * Projectile.ai[0];
		float width = widx * widx * 80f + 10;
		for (int i = 0; i < 51; ++i)
		{
			Vector2 vDp = new Vector2(0, radius).RotatedBy(i / 25d * Math.PI + FirstRo);
			var normalDir = Vector2.Normalize(vDp);

			var factor = i / 50f;
			var color = Color.Lime;
			var w = MathHelper.Lerp(1f, 0.05f, 0.5f);

			if (width < radius)
			{
				bars.Add(new Vertex2D(Projectile.Center, color, new Vector3(factor, 0, w)));
			}
			else
			{
				bars.Add(new Vertex2D(vDp + Projectile.Center + normalDir * width, color, new Vector3(factor, 1, w)));
			}
			bars.Add(new Vertex2D(vDp + Projectile.Center + normalDir * -width, color, new Vector3(factor, 0, w)));
		}

		if (bars.Count > 2)
		{
			RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
			var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
			var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.ZoomMatrix;

			ef.Parameters["tex0"].SetValue(ModAsset.heatmapLime.Value);
			ef.Parameters["uTransform"].SetValue(model * projection);
			ef.Parameters["alphaValue"].SetValue(0.1f);

			ef.CurrentTechnique.Passes[0].Apply();

			Main.graphics.GraphicsDevice.Textures[0] = ModAsset.EShootDark.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

			Main.graphics.GraphicsDevice.RasterizerState = originalState;
		}
		for (int i = 0; i < 23; i++)
		{
			var barsII = new List<Vertex2D>();

			for (int z = 1; z < 18; ++z)
			{
				float widthII = Math.Clamp((DrawLine[i, z] - DrawLine[i, z - 1]).Length(), 0, 20 * Projectile.ai[0]); // 宽度为距离(速度决定,上限20)
				if (z > 13)
				{
					widthII *= (18 - z) / 5f;
				}

				var normalDir = Vector2.Normalize(DrawLine[i, z] - DrawLine[i, z - 1]).RotatedBy(1.57);
				var factor = z / 18f;
				var w = MathHelper.Lerp(1f, 0.05f, 0.5f);
				barsII.Add(new Vertex2D(DrawLine[i, z] + normalDir * widthII, Color.White, new Vector3((float)Math.Sqrt(factor), 1, w)));
				barsII.Add(new Vertex2D(DrawLine[i, z] + normalDir * -widthII, Color.White, new Vector3((float)Math.Sqrt(factor), 0, w)));
			}
			if (barsII.Count > 2)
			{
				RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
				var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
				var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.ZoomMatrix;

				ef.Parameters["tex0"].SetValue(ModAsset.heatmapLime.Value);
				ef.Parameters["uTransform"].SetValue(model * projection);
				ef.Parameters["alphaValue"].SetValue(0.1f);
				ef.CurrentTechnique.Passes[0].Apply();

				Main.graphics.GraphicsDevice.Textures[0] = ModAsset.EShootDark.Value;
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, barsII.ToArray(), 0, barsII.Count - 2);

				Main.graphics.GraphicsDevice.RasterizerState = originalState;
			}
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
	}
}
