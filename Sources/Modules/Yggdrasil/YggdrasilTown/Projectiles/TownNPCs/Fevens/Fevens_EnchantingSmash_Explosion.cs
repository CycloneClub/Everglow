using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs.Fevens;

public class Fevens_EnchantingSmash_Explosion : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public override string Texture => Commons.ModAsset.StabbingProjectile_Mod;

	public Vector2 OriginalPos = Vector2.zeroVector;

	public override void OnSpawn(IEntitySource source)
	{
		OriginalPos = new Vector2(Projectile.ai[0], Projectile.ai[1]);
		ShakerManager.AddShaker(Projectile.Center, Vector2.One.RotatedByRandom(MathHelper.Pi), 210, 6f, 200, 0.9f, 0.8f, 600);
		for (int g = 0; g < 72; g++)
		{
			Vector2 newVelocity = new Vector2(0, -Main.rand.NextFloat(55f, 220f)).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi));
			Vector2 pos = Projectile.Center - newVelocity * 1;
			var somg = new Fevens_EnchantingSmash_Explosion_Smog
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = pos,
				MaxTime = Main.rand.Next(55, 148),
				Scale = Main.rand.NextFloat(10f, 120f),
				ai = new float[] { 0, 0 },
			};
			Ins.VFXManager.Add(somg);
		}
		for (int g = 0; g < 360; g++)
		{
			Vector2 newVelocity = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat(1f)) * 40).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi));
			Vector2 pos = Projectile.Center - newVelocity * 1;
			var somg = new Fevens_PurpleFlameDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = pos,
				MaxTime = Main.rand.Next(55, 188),
				Scale = Main.rand.NextFloat(120f, 240f),
				ai = new float[] { Main.rand.NextFloat(1f), Main.rand.NextFloat(-0.15f, 0.15f) },
			};
			Ins.VFXManager.Add(somg);
		}
		for (int g = 0; g < 360; g++)
		{
			Vector2 newVelocity = new Vector2(0, -Main.rand.NextFloat(15f, 70f)).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi));
			Vector2 pos = Projectile.Center - newVelocity * 1;
			var somg = new Fevens_PurpleSparkDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = pos,
				MaxTime = Main.rand.Next(55, 288),
				Scale = Main.rand.NextFloat(10f, 20f),
				ai = new float[] { 0, 0 },
			};
			Ins.VFXManager.Add(somg);
		}
		float maxValue = (OriginalPos - Projectile.Center).Length() / 4f;
		for (int t = 0; t < maxValue; t++)
		{
			Vector2 newVelocity = Vector2.Normalize(Projectile.Center - OriginalPos) * 18f;
			var smog = new Fevens_ArrowTrail
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Vector2.Lerp(OriginalPos, Projectile.Center, t / maxValue) + new Vector2(Main.rand.NextFloat(-20f, 20f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(17, 25),
				Scale = Main.rand.NextFloat(1f, 6f),
				Rotation = newVelocity.ToRotation(),

				ai = new float[] { Main.rand.NextFloat(0f, 1f), Main.rand.NextFloat(0f, 1f), Main.rand.NextFloat(-0.005f, 0.005f) },
			};
			Ins.VFXManager.Add(smog);
		}
	}

	public override void SetDefaults()
	{
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.aiStyle = -1;
		Projectile.hostile = true;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 60;
		ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 180000;
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if ((targetHitbox.Center() - projHitbox.Center()).Length() < 750 && Projectile.timeLeft > 55)
		{
			return true;
		}
		return false;
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info) => base.OnHitPlayer(target, info);

	public override void AI()
	{
		Lighting.AddLight(Projectile.Center, new Vector3(1f, 0.3f, 2f) * Projectile.timeLeft);

		foreach (Player player in Main.player)
		{
			if (CollisionUtils.Intersect(player.Hitbox.Left(), player.Hitbox.Right(), player.Hitbox.Height, OriginalPos, Projectile.Center, 60) && Projectile.timeLeft > 56)
			{
				player.Center = Projectile.Center;
			}
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		var drawColor = Color.Transparent;
		float timeValue = MathF.Max(0, (Projectile.timeLeft - 50f) / 10f);
		if (Projectile.timeLeft > 50)
		{
			drawColor = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(0.4f, 0.1f, 0.7f, 0f), 1 - timeValue);
		}
		else if (Projectile.timeLeft > 40)
		{
			timeValue = MathF.Max(0, (Projectile.timeLeft - 40f) / 10f);
			drawColor = Color.Lerp(new Color(0.4f, 0.1f, 0.7f, 0f), new Color(0f, 0f, 0f, 0f), 1 - timeValue);
		}
		float distanceValue = 1600 - (Projectile.Center - (Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f)).Length();
		distanceValue = Math.Max(0, distanceValue);
		distanceValue /= 1600f;
		drawColor *= distanceValue;
		var whiteScreen = new List<Vertex2D>();
		whiteScreen.Add(new Vector2(-200), drawColor, new Vector3(0.5f, 0.5f, 0));
		whiteScreen.Add(new Vector2(200 + Main.screenWidth, -200), drawColor, new Vector3(0.5f, 0.5f, 0));

		whiteScreen.Add(new Vector2(-200, 200 + Main.screenHeight), drawColor, new Vector3(0.5f, 0.5f, 0));
		whiteScreen.Add(new Vector2(200 + Main.screenWidth, 200 + Main.screenHeight), drawColor, new Vector3(0.5f, 0.5f, 0));

		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Point.Value;
		if (whiteScreen.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, whiteScreen.ToArray(), 0, whiteScreen.Count - 2);
		}

		for (int t = 0; t < 4; t++)
		{
			Vector2 move = new Vector2(25, 0).RotatedBy(t / 4f * MathHelper.TwoPi);
			if (t == 0)
			{
				move *= 0;
				move += Vector2.Normalize(OriginalPos - Projectile.Center) * 300;
			}
			float value0 = (60 - Projectile.timeLeft) / 60f;
			float value1 = MathF.Pow(value0, 0.5f);
			float width = (1 - MathF.Cos(value1 * 2f * MathF.PI)) * 18f;
			var normalizedVelocity = new Vector2(0, 1);
			Vector2 normalize = normalizedVelocity.RotatedBy(Math.PI / 2d) * width;
			Color shadow = Color.White * 0.4f;
			float endCoordY = 0.5f;
			var bars = new List<Vertex2D>
			{
				new Vertex2D(OriginalPos + move + normalize - Main.screenPosition, shadow, new Vector3(0, 0, 0)),
				new Vertex2D(OriginalPos + move - normalize - Main.screenPosition, shadow, new Vector3(0, 1, 0)),
				new Vertex2D(Projectile.Center + move + normalize - Main.screenPosition, shadow, new Vector3(endCoordY, 0, 0)),
				new Vertex2D(Projectile.Center + move - normalize - Main.screenPosition, shadow, new Vector3(endCoordY, 1, 0)),
			};
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Star2_black.Value;
			if (bars.Count > 3)
			{
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			}
			Color light = Color.Lerp(new Color(1f, 1f, 1f, 0), new Color(0f, 0.3f, 1f, 0), value0 * 3) * width;
			if (value0 > 0.5f)
			{
				light = Color.Lerp(light, new Color(0f, 0.02f, 0.2f, 0), value0 * 3) * width;
			}
			light *= width / 10f;
			normalize *= width * width / 120f;
			if (t > 0)
			{
				normalize *= 0.4f;
				light = Color.Lerp(light, new Color(0f, 0.02f, 0.2f, 0), 0.5f);
			}
			bars = new List<Vertex2D>()
			{
				new Vertex2D(OriginalPos + move + normalize - Main.screenPosition, light, new Vector3(0, 0, 0)),
				new Vertex2D(OriginalPos + move - normalize - Main.screenPosition, light, new Vector3(0, 1, 0)),
				new Vertex2D(Projectile.Center + move + normalize - Main.screenPosition, light, new Vector3(endCoordY, 0, 0)),
				new Vertex2D(Projectile.Center + move - normalize - Main.screenPosition, light, new Vector3(endCoordY, 1, 0)),
			};
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Textures_Star.Value;
			if (bars.Count > 3)
			{
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			}
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}
}
