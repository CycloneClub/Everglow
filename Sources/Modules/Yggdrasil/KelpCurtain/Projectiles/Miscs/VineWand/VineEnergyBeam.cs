using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.KelpCurtain.Items.Tools;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Miscs.VineWand;

public class VineEnergyBeam : ModProjectile
{
	public Vector2 StartPosition;
	public Vector2 EndPosition;

	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 0;
	}

	public override void SetDefaults()
	{
		Projectile.width = 2;
		Projectile.height = 2;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 2000000;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.alpha = 100;
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		if (player.active)
		{
			StartPosition = player.Center;
		}
		if (player.HeldItem is not null && player.HeldItem.ModItem is not null && player.HeldItem.ModItem is VineRepairWand vWand)
		{
			if (!vWand.IsAdjusting(EndPosition.ToTileCoordinates()))
			{
				ReadyToKill();
			}
			else
			{
				if (Projectile.timeLeft < 2000000)
				{
					Projectile.timeLeft = 3000000;
				}
			}
		}
		else
		{
			ReadyToKill();
		}

		Projectile.Center = (StartPosition + EndPosition) / 2f;
		Projectile.rotation = (EndPosition - StartPosition).ToRotation();
	}

	public void ReadyToKill()
	{
		if (Projectile.timeLeft > 10)
		{
			Projectile.timeLeft = 10;
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Vector2 start = StartPosition - Main.screenPosition;
		Vector2 end = EndPosition - Main.screenPosition;

		Vector2 direction = end - start;
		start += direction.NormalizeSafe() * 36;
		float length = direction.Length();
		direction = direction.NormalizeSafe();
		float fade = 1;
		if (Projectile.timeLeft < 10)
		{
			fade = Projectile.timeLeft / 10f;
		}
		Vector2 thick = direction.RotatedBy(MathHelper.PiOver2) * 20f * Projectile.ai[0];
		float time = -(float)Main.time * 0.03f;
		Color[] flowColors = new Color[]
		{
			new Color(187, 224, 163, 0),
			new Color(64, 186, 82, 0),
			new Color(51, 147, 64, 0),
			new Color(64, 104, 82, 0),
		};

		List<Vertex2D> bars_dark = [];
		List<Vertex2D> bars = [];

		bars_dark.Add(start + thick, Color.White * fade, new Vector3(time, 0, 0));
		bars_dark.Add(start - thick, Color.White * fade, new Vector3(time, 1, 0));
		bars.Add(start + thick, new Color(187, 224, 163, 0) * fade, new Vector3(time, 0, 0));
		bars.Add(start - thick, new Color(187, 224, 163, 0) * fade, new Vector3(time, 1, 0));

		Vector2 dustPosOld = start;
		int index = 0;
		for (int k = 0; k < length - 45; k += 45 + TileUtils.GetFixedRandomNumber(Projectile.whoAmI, k, 45))
		{
			index++;
			float deltaValue = index % 2 - 0.5f;
			var color = flowColors[Math.Min(index, 3)];
			Vector2 wave = thick * MathF.Sin(k * 0.07f + time) * 1.2f * MathF.Sin(k / length * MathHelper.Pi);

			bars_dark.Add(start + direction * k + thick + wave, Color.White * fade, new Vector3(time + deltaValue, 0, 0));
			bars_dark.Add(start + direction * k - thick + wave, Color.White * fade, new Vector3(time + deltaValue, 1, 0));
			bars.Add(start + direction * k + thick + wave, color * fade, new Vector3(time + deltaValue, 0, 0));
			bars.Add(start + direction * k - thick + wave, color * fade, new Vector3(time + deltaValue, 1, 0));

			// 生成粒子效果 - 使用更深的颜色
			if (Main.rand.NextBool((int)(12 / Projectile.ai[0])) && !Main.gamePaused)
			{
				Vector2 randomPos = Vector2.Lerp(start + direction * k + wave, dustPosOld, Main.rand.NextFloat()) + Main.screenPosition;

				// 使用更深的绿色粒子
				Color[] dustColors = new Color[]
				{
					new Color(20, 80, 20),     // 深绿
					new Color(30, 100, 30),    // 中深绿
					new Color(40, 120, 40),    // 中绿
					new Color(60, 140, 60),    // 浅深绿
					new Color(80, 160, 80),     // 浅绿
				};

				Color dustColor = dustColors[Main.rand.Next(dustColors.Length)];
				Dust dust = Dust.NewDustPerfect(randomPos, DustID.TerraBlade, Vector2.Zero, 0, dustColor, 0.8f);
				dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
				dust.noGravity = true;

				if (k + 45 + TileUtils.GetFixedRandomNumber(Projectile.whoAmI, k, 45) >= length - 45)
				{
					randomPos = Vector2.Lerp(end, start + direction * k + wave, Main.rand.NextFloat()) + Main.screenPosition;
					dustColor = dustColors[Main.rand.Next(dustColors.Length)];
					Dust dust2 = Dust.NewDustPerfect(randomPos, DustID.TerraBlade, Vector2.Zero, 0, dustColor, 0.8f);
					dust2.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
					dust2.noGravity = true;
				}
			}

			dustPosOld = start + direction * k + wave;
		}

		bars_dark.Add(end + thick, Color.White * fade, new Vector3(time + length / 500f, 0, 0));
		bars_dark.Add(end - thick, Color.White * fade, new Vector3(time + length / 500f, 1, 0));
		bars.Add(end + thick, new Color(64, 104, 82, 0) * fade, new Vector3(time + length / 500f, 0, 0));
		bars.Add(end - thick, new Color(64, 104, 82, 0) * fade, new Vector3(time + length / 500f, 1, 0));

		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		Texture2D flow_dark = Commons.ModAsset.Trail_9_black.Value;
		Main.graphics.graphicsDevice.Textures[0] = flow_dark;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_dark.ToArray(), 0, bars_dark.Count - 2);

		Texture2D flow = Commons.ModAsset.Trail_9.Value;
		Main.graphics.graphicsDevice.Textures[0] = flow;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}
}
