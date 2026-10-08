using Everglow.Commons.Graphics;
using Everglow.Commons.VFX.CommonVFX_Rework;
using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.Myth.Misc.Projectiles.Accessory;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class IchorClub_smash : ClubProjSmash
{
	public override string Texture => ModAsset.IchorClub_Mod;

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		for (int x = 0; x < 2; x++)
		{
			Vector2 velocity = new Vector2(0, Main.rand.NextFloat(2f, 6f)).RotatedByRandom(6.283) - Projectile.velocity * 0.2f;
			var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), target.Center + velocity * -2, velocity, ModContent.ProjectileType<IchorCurrent>(), Projectile.damage / 3, Projectile.knockBack, Projectile.owner, 3f/*If ai[0] equal to 3, another ai will be execute*/);
			p.friendly = false;
			p.CritChance = Projectile.CritChance;
		}
		target.AddBuff(BuffID.Ichor, (int)(818 * Omega));
	}

	public override void AI()
	{
		base.AI();
		for (float x = 0; x < Omega + 0.6 + Player.velocity.Length() / 180f; x += 0.05f)
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

			if (Main.rand.NextBool(30))
			{
				var ichor = new IchorSplash
				{
					Velocity = vel,
					Active = true,
					Visible = true,
					Position = pos,
					MaxTime = Main.rand.Next(6, 32),
					Scale = Main.rand.NextFloat(6f, 12f),
					ai = new float[] { Main.rand.NextFloat(0.1f, 1f), rot * 0.01f, Main.rand.NextFloat(3.6f, 30f) },
				};

				Ins.VFXManager.Add(ichor);
			}
			if (Main.rand.NextBool(12))
			{
				int time = Main.rand.Next(10, 30);
				GradientColor color = new GradientColor();
				color.colorList.Add((Color.White, 0f));
				color.colorList.Add((Color.Yellow, 0.2f));
				color.colorList.Add((new Color(1f, 0.3f, 0f), 1f));

				var splash = new Splash
				{
					Position = Vector2.Lerp(pos, Projectile.Center, Main.rand.NextFloat(-0.2f, 0.5f)),
					Color = color,
					Gravity = 0.2f,
					Velocity = vel * 0.2f - new Vector2(0, 1f),
					Scale = Main.rand.NextFloat(0.1f, 0.4f),
					MaxTimeLeft = time,
					TimeLeft = time,
				};
				Ins.VFXManager.Add(splash);
			}
			if (Main.rand.NextBool(3))
			{
				for (int g = 0; g < 1; g++)
				{
					var spark = new IchorDrop
					{
						Velocity = vel.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * 0.5f,
						Active = true,
						Visible = true,
						Position = pos,
						MaxTime = Main.rand.Next(36, 75),
						Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(1f, 8.0f)),
						Rotation = Main.rand.NextFloat(6.283f),
						ai = new float[] { Main.rand.NextFloat(0.1f, 1f), rot * 0.1f },
					};
					Ins.VFXManager.Add(spark);
				}
			}
		}
	}

	public override void Smash(int level)
	{
		for (int i = 0; i < 10 + level * 5; i++)
		{
			var ichor = new IchorSplash
			{
				Velocity = new Vector2(Main.rand.NextFloat(-15, 15), Main.rand.NextFloat(-10, -5)) * 1.5f * (1 + level * 0.3f),
				Active = true,
				Visible = true,
				Position = Player.Bottom + new Vector2(0, 20),
				MaxTime = Main.rand.Next(15, 40),
				Scale = Main.rand.NextFloat(6f, 12f),
				ai = new float[] { Main.rand.NextFloat(0.1f, 1f), 0.01f, Main.rand.NextFloat(3.6f, 30f) },
			};

			Ins.VFXManager.Add(ichor);
		}

		for (int g = 0; g < 20; g++)
		{
			var spark = new IchorDrop
			{
				Velocity = new Vector2(Main.rand.NextFloat(-15, 15), Main.rand.NextFloat(-10, -5)),
				Active = true,
				Visible = true,
				Position = Player.Center,
				MaxTime = Main.rand.Next(36, 75),
				Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(1f, 8.0f)),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.1f, 1f), 0.2f },
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
			Color c0 = new Color(1f, 1.2f * factor, 0.0f, 0) * factor;
			if (i == 0)
			{
				c0 = Color.Transparent;
			}
			bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, c0, new Vector3(factor, 1.2f, 0f)));
			bars.Add(new Vertex2D(smoothedTrail[i] - Main.screenPosition, c0, new Vector3(factor, 0f, 0)));
		}

		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.IchorClub_Trail2.Value;
		Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
	}
}
