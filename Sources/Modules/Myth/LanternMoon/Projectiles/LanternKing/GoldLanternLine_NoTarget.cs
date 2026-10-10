using Everglow.Commons.DataStructures;
using Everglow.Commons.Templates.Weapons;
using Everglow.Myth.LanternMoon.NPCs.LanternGhostKing;
using Everglow.Myth.LanternMoon.VFX;

namespace Everglow.Myth.LanternMoon.Projectiles.LanternKing;

public class GoldLanternLine_NoTarget : ModProjectile
{
	public override string Texture => ModAsset.GoldLaser_Mod;

	public float LaserDirection = 0;

	public Vector2 EndPos = Vector2.zeroVector;

	public int Timer;

	public override void SetDefaults()
	{
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
		Projectile.aiStyle = -1;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = true;
		ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10240;
		Projectile.timeLeft = 300;
		Projectile.hostile = true;
		Projectile.friendly = false;
		Projectile.scale = 1;
	}

	public override void AI()
	{
		Timer++;
		Projectile.velocity *= 0.98f;
		LaserDirection = Projectile.ai[0];
		Lighting.AddLight(Projectile.Center, new Vector3(1f, 1f, 0) * Projectile.scale);
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if (Timer >= 100)
		{
			return CollisionUtils.Intersect(targetHitbox.Left(), targetHitbox.Right(), targetHitbox.Height, Projectile.Center, EndPos, 30);
		}
		return base.Colliding(projHitbox, targetHitbox);
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Texture2D star = Commons.ModAsset.StarSlash.Value;
		float width = 0.5f;
		if (Projectile.timeLeft < 200)
		{
			width = 2f;
		}
		if (Projectile.timeLeft < 150)
		{
			width *= Projectile.timeLeft / 150f;
		}
		width *= 1 + (MathF.Sin((float)Main.time * 0.23f + Projectile.whoAmI) + 0.5f) * 0.7f;
		float timeValue = MathF.Sin((float)Main.time * 0.07f + Projectile.whoAmI) * 0.5f + 0.5f;
		Color c0 = new Color(1f, 0.75f * timeValue, 0, 0) * timeValue;
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, c0, MathHelper.PiOver2, star.Size() / 2f, new Vector2(width / 1.5f, 0.5f), SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, c0, 0, star.Size() / 2f, new Vector2(width / 1.5f, 0.5f), SpriteEffects.None, 0);

		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, c0, MathHelper.PiOver2 + (float)Main.timeForVisualEffects * 0.04f, star.Size() / 2f, width / 10f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, c0, (float)Main.timeForVisualEffects * 0.04f, star.Size() / 2f, width / 10f, SpriteEffects.None, 0);

		float laserTime = 20;
		if (Timer >= 100)
		{
			width *= (100 + laserTime - Timer) / laserTime;
			if (width <= 0)
			{
				width = 0;
			}
		}

		Texture2D spot = Commons.ModAsset.LightPoint2.Value;
		Main.spriteBatch.Draw(spot, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 0.7f, 0), (float)Main.timeForVisualEffects * 0.04f, spot.Size() / 2f, width, SpriteEffects.None, 0);

		if (Timer > 50 && Timer < 100)
		{
			float rot = LaserDirection + MathHelper.PiOver2;
			Color c1 = Color.Lerp(new Color(0.3f, 0.05f, 0f, 0f), new Color(1f, 1f, 0.5f, 0f), (Timer - 50) / 50f);
			Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, c1, rot, star.Size() / 2f, new Vector2(width / 1.5f + 0.5f, 1f), SpriteEffects.None, 0);
		}
		if (Timer >= 100 && Timer <= 100 + laserTime)
		{
			var posCheck = Projectile.Center;
			var dir = new Vector2(1, 0).RotatedBy(LaserDirection) * 16;
			var value = (Timer - 100) / laserTime;
			var drawC = Color.Lerp(new Color(1f, 1f, 0.8f, 0f), new Color(0.4f, 0f, 0f, 0f), MathF.Pow(value, 0.5f));
			if (value > 0.3 && value < 0.7)
			{
				drawC = Color.Lerp(drawC, new Color(1f, 0.8f, 0.2f, 0f), value);
			}
			var widthRay = MathF.Sin(MathF.Pow(value, 0.5f) * MathF.PI);
			var ringCenter = Projectile.Center;
			var ringRadius = 20000f;
			foreach (var npc in Main.npc)
			{
				if (npc != null && npc.active && npc.type == ModContent.NPCType<LanternGhostKing>())
				{
					LanternGhostKing lKing = npc.ModNPC as LanternGhostKing;
					ringCenter = lKing.RingCenter;
					ringRadius = lKing.RingRadius;
				}
			}
			float lightValue = 0.5f / 255f;
			List<Vertex2D> bars = new List<Vertex2D>();
			for (int i = 0; i < 600; i++)
			{
				var drawPos = posCheck - Main.screenPosition;
				bars.Add(drawPos + dir.RotatedBy(MathHelper.PiOver2) * widthRay, drawC, new Vector3(i / 4f + value, 0, 0));
				bars.Add(drawPos - dir.RotatedBy(MathHelper.PiOver2) * widthRay, drawC, new Vector3(i / 4f + value, 1, 0));
				if (!Collision.SolidCollision(posCheck - new Vector2(4), 8, 8) && (posCheck - ringCenter).Length() < ringRadius)
				{
					posCheck += dir;
					Lighting.AddLight(posCheck, drawC.R * lightValue, drawC.G * lightValue, drawC.B * lightValue);
				}
				else
				{
					break;
				}
			}
			EndPos = posCheck;
			if (bars.Count > 2)
			{
				SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
				Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_1.Value;
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

				Main.spriteBatch.End();
				Main.spriteBatch.Begin(sBS);
			}
		}
		if (Timer == 100 && !Main.gamePaused)
		{
			for (int x = 0; x < 12; x++)
			{
				Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(4f, 8f)).RotatedByRandom(MathHelper.TwoPi);
				var spark = new GoldenLineStar()
				{
					Velocity = newVelocity,
					Active = true,
					Visible = true,
					Position = EndPos,
					RotateSpeed = 0,
					Rotation = 0,
					MaxTime = Main.rand.Next(20, 40),
					Scale = Main.rand.NextFloat(0.5f, 1f),
				};
				Ins.VFXManager.Add(spark);
			}
		}
		if (Timer == 100 + laserTime)
		{
			Projectile.Center = EndPos;
			Projectile.velocity = new Vector2(16, 0).RotatedBy(LaserDirection);
			Projectile.Kill();
		}
		return false;
	}

	public override void OnKill(int timeLeft)
	{
		if (Timer < 100)
		{
			for (int x = 0; x < 12; x++)
			{
				Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(4f, 8f)).RotatedByRandom(MathHelper.TwoPi);
				var spark = new GoldenLineStar()
				{
					Velocity = newVelocity,
					Active = true,
					Visible = true,
					Position = Projectile.Center,
					RotateSpeed = 0,
					Rotation = 0,
					MaxTime = Main.rand.Next(20, 40),
					Scale = Main.rand.NextFloat(0.5f, 1f),
				};
				Ins.VFXManager.Add(spark);
			}
		}
	}
}
