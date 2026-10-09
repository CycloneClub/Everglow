namespace Everglow.SpellAndSkull.Projectiles.CrystalStorm;

internal class Storm : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 28;
		Projectile.height = 280;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 600;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.tileCollide = false;
	}
	public void GenerateVFX(int Frequency)
	{
		float mulVelocity = 1f;
		for (int g = 0; g < Frequency; g++)
		{
			float k2 = Main.rand.NextFloat(0f, 1f);
			float k3 = k2 * k2 * k2 * k2;
			var v2 = new Vector2(Main.rand.NextFloat(-150f, 150f) / (k3 * 10f + 1f), -k3 * 200 + 10);
			var cw = new CrystalWindVFX
			{
				velocity = Projectile.velocity * Main.rand.NextFloat(0.65f, 2.5f) * mulVelocity + Projectile.velocity.SafeNormalize(new Vector2(0, -1)),
				Active = true,
				Visible = true,
				position = Projectile.Center + v2,
				maxTime = Math.Min(120, Intensity / 2),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Intensity / 1400f * Main.rand.NextFloat(0.85f, 1.15f), Projectile.whoAmI, 0 },
			};
			Ins.VFXManager.Add(cw);
		}
	}
	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		Projectile.velocity *= 0;
		if (Projectile.timeLeft > 550)
			Intensity += 9;
		else
		{
			Intensity -= 5;
			if (Intensity <= 0)
				Projectile.Kill();
		}
		for (int j = 0; j < 4; j++)
		{
			Vector2 v0 = Vector2.Zero;
			float k0 = Main.rand.NextFloat(0f, 1f);
			float k1 = k0 * k0 * k0 * k0;
			var v1 = new Vector2(Main.rand.NextFloat(-150f, 150f) / (k1 * 10f + 1f), -k1 * 200 + 10);
			if (Collision.SolidCollision(Projectile.Center + v1, 1, 1))
				continue;
			var dust0 = Dust.NewDustDirect(Projectile.Center + v1, 0, 0, ModContent.DustType<Dusts.CrystalAppearStoppedByTileInAStorm>(), v0.X, v0.Y, 100, default, Main.rand.NextFloat(0.3f, 1.6f) * Math.Min(Intensity, 300) / 450f);
			dust0.noGravity = true;
			dust0.color.B = (byte)(v1.Length() / 2f);
			dust0.color.A = (byte)(Intensity / 2);
			dust0.dustIndex = Projectile.whoAmI;
		}

		for (int j = 0; j < 4; j++)
		{
			float k2 = Main.rand.NextFloat(0f, 1f);
			float k3 = k2 * k2 * k2 * k2;
			var v2 = new Vector2(Main.rand.NextFloat(-150f, 150f) / (k3 * 10f + 1f), -k3 * 200 + 10);
			var p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + v2, Vector2.Zero, ModContent.ProjectileType<CrystalWind>(), 0, 0, Projectile.owner, Projectile.whoAmI, Intensity / 1400f * Main.rand.NextFloat(0.85f, 1.15f));
			p0.timeLeft = Math.Min(120, Intensity / 2);
			p0.rotation = Main.rand.NextFloat(6.283f);
		}

		// GenerateVFX(4);
		if (Main.rand.NextBool(10))
		{
			foreach (var target in Main.npc)
			{
				if (target.active && Main.rand.NextBool(2))
				{
					if (!target.dontTakeDamage && !target.friendly && target.CanBeChasedBy() && target.knockBackResist > 0)
					{
						if (target.velocity.Length() <= 0.001f)
							continue;
						Vector2 ToTarget = target.Center - (Projectile.Center - new Vector2(0, 150));
						float dis = ToTarget.Length();
						if (dis < 800 && ToTarget != Vector2.Zero)
						{
							float mess = target.width * target.height;
							mess = (float)Math.Sqrt(mess);
							Vector2 Addvel = Vector2.Normalize(ToTarget) / mess / (dis + 10) * 100f * target.knockBackResist * Intensity;
							if (!target.noGravity)
								Addvel.Y *= 3f;
							target.velocity -= Addvel;
							if (target.velocity.Length() > 10)
								target.velocity *= 10 / target.velocity.Length();
						}
					}
				}
			}
			foreach (var target in Main.item)
			{
				if (target.active && Main.rand.NextBool(2))
				{
					Vector2 ToTarget = target.Center - (Projectile.Center - new Vector2(0, 50));
					float dis = ToTarget.Length();
					if (dis < 800 && ToTarget != Vector2.Zero)
					{
						if (dis < 45)
						{
							if (target.type is >= ItemID.CopperCoin and <= ItemID.PlatinumCoin or ItemID.Star or ItemID.Heart)
								target.position = player.Center;
						}
						float mess = target.width * target.height;
						mess = (float)Math.Sqrt(mess);
						Vector2 Addvel = Vector2.Normalize(ToTarget) / mess / (dis + 10) * 50f * Intensity;
						target.velocity -= Addvel;
						if (target.velocity.Length() > 10)
							target.velocity *= 10 / target.velocity.Length();
					}
				}
			}
			foreach (var target in Main.gore)
			{
				if (target.active && Main.rand.NextBool(2))
				{
					Vector2 ToTarget = target.position - (Projectile.Center - new Vector2(0, 50));
					float dis = ToTarget.Length();
					if (dis < 800 && ToTarget != Vector2.Zero)
					{
						float mess = target.Width * target.Height;
						mess = (float)Math.Sqrt(mess);
						Vector2 Addvel = Vector2.Normalize(ToTarget) / mess / (dis + 10) * 100f * Intensity;
						target.velocity -= Addvel;
						if (target.velocity.Length() > 10)
							target.velocity *= 10 / target.velocity.Length();
						target.timeLeft -= 24;
					}
				}
			}
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}

	internal int Intensity = 0;
	internal Vector2 ringPos = Vector2.Zero;
}
