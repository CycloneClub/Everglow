using Everglow.Commons.Graphics;
using Everglow.Commons.VFX.CommonVFXDusts;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class CurseClub_smash : ClubProjSmash
{
	public override string Texture => ModAsset.CurseClub_Mod;

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(BuffID.CursedInferno, (int)(818 * Omega));
	}

	public bool smashed = false;

	public override void AI()
	{
		base.AI();
		for (float x = 0; x < Omega + 0.2 + Player.velocity.Length() / 100f; x += 0.12f)
		{
			Vector2 pos = (SmashTrailVecs.ToArray()[SmashTrailVecs.Count - 1] + SmashTrailVecs.ToArray()[SmashTrailVecs.Count - 1]) / 2f;
			float factor = Main.rand.NextFloat(0, 1f);
			if (SmashTrailVecs.Count > 1)
			{
				pos = SmashTrailVecs.ToArray()[SmashTrailVecs.Count - 1] * factor + SmashTrailVecs.ToArray()[SmashTrailVecs.Count - 2] * (1 - factor);
			}
			pos = (pos - Projectile.Center) * 0.9f + Projectile.Center - Player.velocity * factor;
			Vector2 vel = Vector2.zeroVector;
			if (SmashTrailVecs.Count > 1)
			{
				vel = SmashTrailVecs.ToArray()[SmashTrailVecs.Count - 1] - SmashTrailVecs.ToArray()[SmashTrailVecs.Count - 2];
			}
			if (SmashTrailVecs.Count > 2)
			{
				vel = (SmashTrailVecs.ToArray()[SmashTrailVecs.Count - 1] - SmashTrailVecs.ToArray()[SmashTrailVecs.Count - 2]) * factor + (SmashTrailVecs.ToArray()[SmashTrailVecs.Count - 2] - SmashTrailVecs.ToArray()[SmashTrailVecs.Count - 3]) * (1 - factor);
			}
			vel += Player.velocity;
			vel *= Main.rand.NextFloat(0.1f, 0.3f);
			float rot = 0;
			if (SmashTrailVecs.Count > 1)
			{
				rot = (SmashTrailVecs.ToArray()[SmashTrailVecs.Count - 1] - Projectile.Center).ToRotation() - (SmashTrailVecs.ToArray()[SmashTrailVecs.Count - 2] - Projectile.Center).ToRotation();
			}

			if (Main.rand.NextBool(5))
			{
				GradientColor color = new GradientColor();
				color.colorList.Add((new Color(1f, 1f, 0.1f), 0f));
				color.colorList.Add((new Color(0.1f, 0.6f, 0.1f), 0.3f));
				int time = Main.rand.Next(15, 35);
				var fire = new Flare()
				{
					Position = Vector2.Lerp(Player.Center, pos, Main.rand.NextFloat(0.4f, 1.25f)),
					Velocity = vel * 0.2f,
					color = color,
					timeleft = time,
					maxTimeleft = time,
					Scale = Main.rand.NextFloat(0.3f, 0.6f) * (smashed ? 1 : 0.5f),
				};
				Ins.VFXManager.Add(fire);
			}
			/*
			for (int g = 0; g < 3; g++)
			{
				var fire = new CurseFlame_HighQualityDust
				{
					Velocity = vel * 0.3f,
					Active = true,
					Visible = true,
					Position = pos + vel * g,
					MaxTime = Main.rand.Next(16, 35),

					ai = new float[] { Main.rand.NextFloat(0.1f, 1f), rot * 0.1f, Main.rand.NextFloat(3.6f, 30f) },
				};
				Ins.VFXManager.Add(fire);
			}*/

			for (int g = 0; g < 2; g++)
			{
				var spark = new CurseFlameSparkDust
				{
					Velocity = vel.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)),
					Active = true,
					Visible = true,
					Position = pos,
					MaxTime = Main.rand.Next(6, Main.rand.Next(6, 75)),
					Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(4f, 47.0f)),
					Rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { Main.rand.NextFloat(0.1f, 1f), rot * 0.1f },
				};
				Ins.VFXManager.Add(spark);
			}
		}
	}

	public override void Smash(int level)
	{
		smashed = true;
		for (int t = 0; t < 5; t++)
		{
			Vector2 vel = new Vector2(Main.rand.NextFloat(-15, 15), Main.rand.NextFloat(-10, -5)) * 1.5f * (1 + level * 0.3f);
			var fire = new CurseFlame_HighQualityDust
			{
				Velocity = vel,
				Active = true,
				Visible = true,
				Position = Player.Bottom - vel * 3 + Main.rand.NextVector2Circular(30, 30),
				MaxTime = Main.rand.Next(10, 25),

				ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(3.6f, 30f) },
			};
			Ins.VFXManager.Add(fire);
		}
		for (int g = 0; g < 20; g++)
		{
			Vector2 vel = new Vector2(0, -20).RotatedBy(Main.rand.NextFloatDirection());
			var spark = new CurseFlameSparkDust
			{
				Velocity = vel.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)),
				Active = true,
				Visible = true,
				Position = Player.Bottom - vel * 3,
				MaxTime = Main.rand.Next(6, Main.rand.Next(6, 405)),
				Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(4f, 47.0f)),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0.1f, 0.1f) },
			};
			Ins.VFXManager.Add(spark);
		}

		base.Smash(level);
	}

	public override void DrawSmashTrail(Color color)
	{
		base.DrawSmashTrail(color);

		if (!SmashTrailVecs.Smooth(out var smoothedTrail))
		{
			return;
		}

		var length = smoothedTrail.Count;
		var bars = new List<Vertex2D>();
		for (int i = 0; i < length; i++)
		{
			float factor = i / (length - 1f);
			Color c0 = new Color(1f * factor * factor, 1f * factor, 1f * factor * factor, 0);
			if (i == 0)
			{
				c0 = Color.Transparent;
			}
			bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, c0, new Vector3(0, 1, 0f)));
			bars.Add(new Vertex2D(smoothedTrail[i] - Main.screenPosition, c0, new Vector3(1, 0, 0)));
		}

		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.CurseClub_glow.Value;
		Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
	}
}
