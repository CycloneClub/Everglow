using Terraria.Audio;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class StarDancer_smash : ClubProjSmash
{
	public override string Texture => ModAsset.StarDancer_Mod;

	public override void SetDef()
	{
		EnableReflection = true;
	}

	public override string TrailColorTex() => ModAsset.StarDancer_glow_Mod;

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (Main.rand.NextBool(7))
		{
			Vector2 v0 = new Vector2(Main.rand.NextFloat(1f, 1.2f), 0).RotatedByRandom(6.283);
			for (int i = 0; i < 5; i++)
			{
				Vector2 v1 = v0.RotatedBy(i / 2.5 * Math.PI);
				Vector2 v2 = v0.RotatedBy((i + 0.5) / 2.5 * Math.PI) * 3;
				Vector2 v3 = v0.RotatedBy((i + 1) / 2.5 * Math.PI);
				for (int j = 0; j < 15; j++)
				{
					Vector2 v4 = (v1 * j + v2 * (14 - j)) / 14f;
					Vector2 v5 = (v3 * j + v2 * (14 - j)) / 14f;
					Vector2 v6 = v2 * (14 - j) / 14f;
					var D = Dust.NewDustDirect(target.Center + v4 - new Vector2(4)/*Dust的Size=8x8*/, 0, 0, DustID.GoldCoin, 0, 0, 0, default, 1.5f);
					D.noGravity = true;
					D.velocity = v4;

					var D1 = Dust.NewDustDirect(target.Center + v5 - new Vector2(4)/*Dust的Size=8x8*/, 0, 0, DustID.GoldCoin, 0, 0, 0, default, 1.5f);
					D1.noGravity = true;
					D1.velocity = v5;

					var D2 = Dust.NewDustDirect(target.Center + v6 - new Vector2(4)/*Dust的Size=8x8*/, 0, 0, DustID.GoldCoin, 0, 0, 0, default, 1.3f);
					D2.noGravity = true;
					D2.velocity = v6;
				}
			}
			Vector2 v7 = new Vector2(0, -Main.rand.NextFloat(1000f, 1200f)).RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f));
			Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center + v7, -v7 / 40f, ProjectileID.FallingStar, (int)(Projectile.damage * 1.5f), Projectile.knockBack, Projectile.owner);
			SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact, target.Center);
		}
	}

	public override void Smash(int level = 0)
	{
		if (level == 0)
		{
			for (int i = 0; i < 12; i++)
			{
				Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, 40 * Player.gravDir), Vector2.zeroVector, ModContent.ProjectileType<StarDancer_starProj>(), Projectile.damage / 3, Projectile.knockBack * 0.2f, Projectile.owner);
				Vector2 addPos = new Vector2(0, Main.rand.NextFloat(-35, -120) * Player.gravDir).RotatedBy(Main.rand.NextFloat(-1.7f, 1.7f));
				p0.Center += addPos;
				p0.timeLeft = (int)(addPos.Length() * 0.2f + 4);
			}
		}
		if (level == 1)
		{
			for (int i = 0; i < 12; i++)
			{
				Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, 40 * Player.gravDir), Vector2.zeroVector, ModContent.ProjectileType<StarDancer_starProj>(), Projectile.damage / 3, Projectile.knockBack * 0.2f, Projectile.owner);
				Vector2 addPos = new Vector2(0, Main.rand.NextFloat(-35, -120) * Player.gravDir).RotatedBy(Main.rand.NextFloat(-1.7f, 1.7f));
				p0.Center += addPos;
				p0.timeLeft = (int)(addPos.Length() * 0.2f + 4 + Main.rand.Next(5));
			}
			for (int i = 0; i < 24; i++)
			{
				Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, 40 * Player.gravDir), Vector2.zeroVector, ModContent.ProjectileType<StarDancer_starProj2>(), Projectile.damage / 3, Projectile.knockBack * 0.2f, Projectile.owner);
				Vector2 addPos = new Vector2(0, Main.rand.NextFloat(-25, -260) * Player.gravDir).RotatedBy(Main.rand.NextFloat(-1.7f, 1.7f));
				p0.Center += addPos;
				p0.timeLeft = (int)(addPos.Length() * 0.26f + 10 + Main.rand.Next(5));
				p0.scale = Main.rand.NextFloat(0.5f, 2);
			}
		}
		base.Smash(level);
	}

	public override void AI()
	{
		base.AI();
		if (SmashTrailVecs.Count > 0)
		{
			int type = DustID.GoldCoin;
			for (float x = 0; x < Omega + 0.2 + Player.velocity.Length() / 40f; x += 0.05f)
			{
				Vector2 pos = (SmashTrailVecs.ToArray()[SmashTrailVecs.Count - 1] + SmashTrailVecs.ToArray()[SmashTrailVecs.Count - 1]) / 2f;
				float factor = Main.rand.NextFloat(0, 1f);
				if (SmashTrailVecs.Count > 1)
				{
					pos = SmashTrailVecs.ToArray()[SmashTrailVecs.Count - 1] * factor + SmashTrailVecs.ToArray()[SmashTrailVecs.Count - 2] * (1 - factor);
				}
				pos = (pos - Projectile.Center) * 0.9f + Projectile.Center - Player.velocity * factor;
				var d0 = Dust.NewDustDirect(pos - new Vector2(4)/*Dust的Size=8x8*/, 0, 0, type, 0, 0, 150, default, Main.rand.NextFloat(0.9f, 1.2f));
				d0.noGravity = true;
				d0.velocity = (SmashTrailVecs.ToArray()[SmashTrailVecs.Count - 1] - Projectile.Center).RotatedBy(MathHelper.PiOver2) / 150f;
			}
			for (float x = 0; x < Omega + 0.2 + Player.velocity.Length() / 40f; x += 0.10f)
			{
				Vector2 pos = (SmashTrailVecs.ToArray()[SmashTrailVecs.Count - 1] + SmashTrailVecs.ToArray()[SmashTrailVecs.Count - 1]) / 2f;
				float factor = Main.rand.NextFloat(0, 1f);
				if (SmashTrailVecs.Count > 1)
				{
					pos = SmashTrailVecs.ToArray()[SmashTrailVecs.Count - 1] * factor + SmashTrailVecs.ToArray()[SmashTrailVecs.Count - 2] * (1 - factor);
				}
				pos = (pos - Projectile.Center) * 0.5f + Projectile.Center - Player.velocity * factor;
				var d0 = Dust.NewDustDirect(pos - new Vector2(4)/*Dust的Size=8x8*/, 0, 0, type, 0, 0, 150, default, Main.rand.NextFloat(0.9f, 1.2f));
				d0.noGravity = true;
				d0.velocity = (SmashTrailVecs.ToArray()[SmashTrailVecs.Count - 1] - Projectile.Center).RotatedBy(MathHelper.PiOver2) / 150f;
			}
			for (float x = 0; x < Omega + 0.2 + Player.velocity.Length() / 40f; x += 0.15f)
			{
				Vector2 pos = (SmashTrailVecs.ToArray()[SmashTrailVecs.Count - 1] + SmashTrailVecs.ToArray()[SmashTrailVecs.Count - 1]) / 2f;
				float factor = Main.rand.NextFloat(0, 1f);
				if (SmashTrailVecs.Count > 1)
				{
					pos = SmashTrailVecs.ToArray()[SmashTrailVecs.Count - 1] * factor + SmashTrailVecs.ToArray()[SmashTrailVecs.Count - 2] * (1 - factor);
				}
				pos = (pos - Projectile.Center) * 0.2f + Projectile.Center - Player.velocity * factor;
				var d0 = Dust.NewDustDirect(pos - new Vector2(4)/*Dust的Size=8x8*/, 0, 0, type, 0, 0, 150, default, Main.rand.NextFloat(0.9f, 1.2f));
				d0.noGravity = true;
				d0.velocity = (SmashTrailVecs.ToArray()[SmashTrailVecs.Count - 1] - Projectile.Center).RotatedBy(MathHelper.PiOver2) / 150f;
			}
		}
	}

	public override void DrawSmashTrail(Color color)
	{
		if (!SmashTrailVecs.Smooth(out var smoothedTrail))
		{
			return;
		}

		var length = smoothedTrail.Count;
		var bars = new List<Vertex2D>();

		for (int i = 0; i < length; i++)
		{
			float factor = i / (length - 1f);
			Color c0 = new Color(1f * factor, 1f * factor * factor, 1f * factor * factor * factor, 0);
			if (i == 0)
			{
				c0 = Color.Transparent;
			}
			bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, c0, new Vector3(0, 1, 0f)));
			bars.Add(new Vertex2D(smoothedTrail[i] - Main.screenPosition, c0, new Vector3(1, 0, 0)));
		}

		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.StarDancer_glow.Value;
		Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		base.DrawSmashTrail(color);
	}
}
