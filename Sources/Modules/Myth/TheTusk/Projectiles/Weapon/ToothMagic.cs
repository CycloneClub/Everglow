using Everglow.Myth.TheTusk.VFXs;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.TheTusk.Projectiles.Weapon;

public class ToothMagic : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 18;
		Projectile.height = 18;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.ignoreWater = false;
		Projectile.tileCollide = true;
		Projectile.extraUpdates = 3;
		Projectile.timeLeft = 1500;
		Projectile.alpha = 0;
		Projectile.penetrate = 2;
		Projectile.scale = 1f;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 30;
		Projectile.DamageType = DamageClass.Magic;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 120;
	}

	internal int Aimnpc = -1;
	internal int TimeTokill = -1;

	public override void OnSpawn(IEntitySource source)
	{
		// Projectile.position.Y -= 15f;
	}

	public void GenerateVFX(int Frequency)
	{
		float mulVelocity = 3f;
		for (int g = 0; g < Frequency; g++)
		{
			Vector2 afterVelocity = Projectile.velocity;
			if (afterVelocity.Length() > 25)
			{
				afterVelocity = afterVelocity * 25 / afterVelocity.Length();
			}
			float mulWidth = 1f;
			if (afterVelocity.Length() < 10)
			{
				mulWidth = afterVelocity.Length() / 10f;
			}
			var blood = new BloodLiquidDust
			{
				velocity = afterVelocity * mulVelocity + Projectile.velocity.SafeNormalize(new Vector2(0, -1)),
				Active = true,
				Visible = true,
				position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) - Projectile.velocity * Main.rand.NextFloat(3f, 14f),
				maxTime = Main.rand.Next(18, 26),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0, Main.rand.NextFloat(18f, 28f) * mulWidth },
			};
			Ins.VFXManager.Add(blood);
		}
	}

	public void GenerateVFXWhileChasing(int Frequency)
	{
		float mulVelocity = 2f;
		for (int g = 0; g < Frequency; g++)
		{
			Vector2 afterVelocity = Projectile.velocity;
			if (afterVelocity.Length() > 25)
			{
				afterVelocity = afterVelocity * 25 / afterVelocity.Length();
			}
			float mulWidth = 1f;
			if (afterVelocity.Length() < 10)
			{
				mulWidth = afterVelocity.Length() / 10f;
			}
			var blood = new BloodLiquidDust
			{
				velocity = afterVelocity * mulVelocity + Projectile.velocity.SafeNormalize(new Vector2(0, -1)),
				Active = true,
				Visible = true,
				position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) - Projectile.velocity * Main.rand.NextFloat(3f, 14f),
				maxTime = Main.rand.Next(12, 14),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0, Main.rand.NextFloat(18f, 28f) * mulWidth },
			};
			Ins.VFXManager.Add(blood);
		}
	}

	public void GenerateVFXKill(int Frequency)
	{
		float mulVelocity = 1.5f;
		for (int g = 0; g < Frequency; g++)
		{
			Vector2 afterVelocity = new Vector2(0, 2f).RotatedBy(g / (float)Frequency * 2 * Math.PI);
			var blood = new BloodLiquidDust
			{
				velocity = afterVelocity * mulVelocity + Projectile.velocity.SafeNormalize(new Vector2(0, -1)),
				Active = true,
				Visible = true,
				position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) - Projectile.velocity * Main.rand.NextFloat(3f, 14f),
				maxTime = Main.rand.Next(42, 64),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0, Main.rand.NextFloat(6f, 12f) },
			};
			Ins.VFXManager.Add(blood);
		}
		mulVelocity = 1.1f;
		for (int g = 0; g < Frequency; g++)
		{
			Vector2 afterVelocity = new Vector2(0, 2f).RotatedBy(g / (float)Frequency * 2 * Math.PI);
			var blood = new BloodLiquidDust
			{
				velocity = afterVelocity * mulVelocity + Projectile.velocity.SafeNormalize(new Vector2(0, -1)),
				Active = true,
				Visible = true,
				position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) - Projectile.velocity * Main.rand.NextFloat(3f, 14f),
				maxTime = Main.rand.Next(42, 64),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0, Main.rand.NextFloat(6f, 12f) },
			};
			Ins.VFXManager.Add(blood);
		}
		mulVelocity = 0.8f;
		Frequency /= 3;
		for (int g = 0; g < Frequency; g++)
		{
			Vector2 afterVelocity = new Vector2(0, 2f).RotatedBy(g / (float)Frequency * 2 * Math.PI);
			var blood = new ThickBloodLiquidDust
			{
				velocity = afterVelocity * mulVelocity + Projectile.velocity.SafeNormalize(new Vector2(0, -1)),
				Active = true,
				Visible = true,
				position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) - Projectile.velocity * Main.rand.NextFloat(3f, 14f),
				maxTime = Main.rand.Next(42, 64),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0, Main.rand.NextFloat(12f, 20f) },
			};
			Ins.VFXManager.Add(blood);
		}
	}

	public void ProjHit()
	{
		SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot.WithVolumeScale(Projectile.ai[0]), Projectile.Center);

		// Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<NormalHit>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.velocity.Length(), Main.rand.NextFloat(6.283f));
		for (int x = 0; x < 40; x++)
		{
			int index = Dust.NewDust(Projectile.position - new Vector2(4), Projectile.width, Projectile.height, ModContent.DustType<Dusts.RedBlood>(), 0f, 0f, 0, default, Main.rand.NextFloat(0.3f, 0.7f));
			Main.dust[index].velocity = new Vector2(0, Main.rand.NextFloat(1.5f, 12f)).RotatedByRandom(6.283);
		}
		for (int x = 0; x < 80; x++)
		{
			int index = Dust.NewDust(Projectile.position - new Vector2(4), Projectile.width, Projectile.height, ModContent.DustType<Dusts.RedBlood>(), 0f, 0f, 0, default, Main.rand.NextFloat(0.5f, 0.7f));
			Main.dust[index].velocity = new Vector2(0, Main.rand.NextFloat(1.5f, 8f)).RotatedByRandom(6.283);
		}
		for (int x = 0; x < 10; x++)
		{
			int index = Dust.NewDust(Projectile.position - new Vector2(4), Projectile.width, Projectile.height, ModContent.DustType<Dusts.RedBlood>(), 0f, 0f, 0, default, Main.rand.NextFloat(0.9f, 1.1f));
			Main.dust[index].velocity = new Vector2(0, Main.rand.NextFloat(2.5f, 8f)).RotatedByRandom(6.283);
		}
		GenerateVFXKill(10);

		Projectile.friendly = false;
		TimeTokill = 120;
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];

		if (Projectile.timeLeft > 500)
		{
			Projectile.scale = 0.8f;
		}
		else
		{
			Projectile.velocity *= 0.996f;
			Projectile.scale = 0.8f * Projectile.timeLeft / 500f;

			Projectile.knockBack *= 0.9f;
			if (Projectile.timeLeft % 10 == 1)
			{
				if (Projectile.damage > 0)
				{
					Projectile.damage--;
				}
				else
				{
					Projectile.damage = 0;
					Projectile.friendly = false;
				}
			}
		}
		Projectile.rotation += 0.1f;
		Projectile.velocity.Y += 0.02f;

		if (TimeTokill >= 0 && TimeTokill <= 2)
		{
			Projectile.Kill();
		}

		if (TimeTokill <= 15 && TimeTokill > 0)
		{
			Projectile.velocity = Projectile.oldVelocity;
		}

		TimeTokill--;
		if (TimeTokill >= 0)
		{
			if (TimeTokill < 10)
			{
				Projectile.damage = 0;
				Projectile.friendly = false;
			}
			Projectile.velocity *= 0f;
		}
		if (TimeTokill <= 0)
		{
			for (int i = 1; i < 3; ++i)
			{
				var d = Dust.NewDustDirect(Projectile.position - Projectile.velocity * 4, Projectile.width, Projectile.height, DustID.Blood, Projectile.velocity.X * 2, Projectile.velocity.Y * 2, 100, default, Main.rand.NextFloat(0.85f, 1.85f) * Projectile.scale);
				d.noGravity = true;
			}
			if (Projectile.wet)
			{
				ProjHit();
			}
		}

		if (Aimnpc != -1 && TimeTokill <= 0)
		{
			NPC npc = Main.npc[Aimnpc];
			if (npc.active)
			{
				Vector2 v0 = npc.Center - Projectile.Center;
				Projectile.velocity += Vector2.Normalize(v0) * 0.5f;

				if (player.HeldItem.type == ItemID.RazorbladeTyphoon)
				{
					Projectile.velocity = Vector2.Normalize(Projectile.velocity) * player.HeldItem.shootSpeed;
				}
				else
				{
					Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 6f;
				}
			}
			GenerateVFXWhileChasing(1);
		}
		else
		{
			if (Projectile.timeLeft % 25 == 0)
			{
				GenerateVFX(1);
			}
		}
		if (Main.rand.NextBool(2))
		{
			float MinDis = 250;
			foreach (var target in Main.npc)
			{
				if (target.active && Main.rand.NextBool(2))
				{
					if (!target.dontTakeDamage && !target.friendly && target.CanBeChasedBy())
					{
						Vector2 ToTarget = target.Center - Projectile.Center;
						float dis = ToTarget.Length();
						if (dis < 250 && ToTarget != Vector2.Zero)
						{
							if (dis < MinDis)
							{
								MinDis = dis;
								Aimnpc = target.whoAmI;
							}
						}
					}
				}
			}
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D Light = ModAsset.BloodProj.Value;
		Texture2D Shade = ModAsset.BloodShade.Value;
		var c0 = new Color(93, 0, 21, 0);

		float width = 24;
		float MulByTimeLeft = 1f;
		if (Projectile.timeLeft < 500)
		{
			MulByTimeLeft = Projectile.timeLeft / 500f;
		}

		width *= MulByTimeLeft;
		c0 *= MulByTimeLeft;
		int TrueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			TrueL++;
		}

		DrawFlameTrail(TrueL, width, true, new Color(255, 255, 255, 40));

		if (TimeTokill <= 0)
		{
			Color c2 = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16);
			float mulColor = (c2.R + c2.G + c2.B) / 765f;
			var Frame = new Rectangle(0, (int)(Main.time / 100f % 7) * 32, 32, 32);
			Main.spriteBatch.Draw(Shade, Projectile.Center - Main.screenPosition, Frame, Color.White * MulByTimeLeft, Projectile.rotation, Frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(Light, Projectile.Center - Main.screenPosition, Frame, new Color(255, 0, 21, 0) * mulColor, Projectile.rotation, Frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		}
		DrawFlameTrail(TrueL, width, false, c0);

		return false;
	}

	private void DrawFlameTrail(int TrueL, float width, bool Shade = false, Color c0 = default(Color), float Mulfactor = 1.6f)
	{
		var bars = new List<Vertex2D>();
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			float mulColor = 1f;
			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
			if (i == 1)
			{
				mulColor = 0f;
			}

			if (i >= 2)
			{
				var normalDirII = Projectile.oldPos[i - 2] - Projectile.oldPos[i - 1];
				normalDirII = Vector2.Normalize(new Vector2(-normalDirII.Y, normalDirII.X));
				if (Vector2.Dot(normalDirII, normalDir) <= 0.965f)
				{
					mulColor = 0f;
				}
			}
			if (i < Projectile.oldPos.Length - 1)
			{
				var normalDirII = Projectile.oldPos[i] - Projectile.oldPos[i + 1];
				normalDirII = Vector2.Normalize(new Vector2(-normalDirII.Y, normalDirII.X));
				if (Vector2.Dot(normalDirII, normalDir) <= 0.965f)
				{
					mulColor = 0f;
				}
			}
			Vector2 DrawCenter = Projectile.oldPos[i] + new Vector2(9f);
			Color c2 = Lighting.GetColor((int)DrawCenter.X / 16, (int)DrawCenter.Y / 16);
			mulColor *= (c2.R + c2.G + c2.B) / 765f;
			var factor = i / (float)TrueL;
			float x0 = factor * Mulfactor - (float)(Main.time / 150d) + 100000;
			x0 %= 1f;
			bars.Add(new Vertex2D(DrawCenter + normalDir * -width * (1 - factor) - Main.screenPosition, c0 * mulColor, new Vector3(x0, 1, 0)));
			bars.Add(new Vertex2D(DrawCenter + normalDir * width * (1 - factor) - Main.screenPosition, c0 * mulColor, new Vector3(x0, 0, 0)));
			var factorII = factor;
			factor = (i + 1) / (float)TrueL;
			var x1 = factor * Mulfactor - (float)(Main.time / 150d) + 100000;
			x1 %= 1f;
			if (x0 > x1)
			{
				float DeltaValue = 1 - x0;
				var factorIII = factorII * x0 + factor * DeltaValue;
				bars.Add(new Vertex2D(DrawCenter + normalDir * -width * (1 - factorIII) - Main.screenPosition, c0 * mulColor, new Vector3(1, 1, 0)));
				bars.Add(new Vertex2D(DrawCenter + normalDir * width * (1 - factorIII) - Main.screenPosition, c0 * mulColor, new Vector3(1, 0, 0)));
				bars.Add(new Vertex2D(DrawCenter + normalDir * -width * (1 - factorIII) - Main.screenPosition, c0 * mulColor, new Vector3(0, 1, 0)));
				bars.Add(new Vertex2D(DrawCenter + normalDir * width * (1 - factorIII) - Main.screenPosition, c0 * mulColor, new Vector3(0, 0, 0)));
			}
		}
		Texture2D t = ModAsset.FogTrace.Value;
		if (Shade)
		{
			t = Commons.ModAsset.Trail_2_black_thick.Value;
		}

		Main.graphics.GraphicsDevice.Textures[0] = t;

		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		float width = 24;
		float MulByTimeLeft = 1f;
		if (Projectile.timeLeft < 500)
		{
			MulByTimeLeft = Projectile.timeLeft / 500f;
		}

		width *= MulByTimeLeft;
		int TrueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			TrueL++;
		}
		var bars = new List<Vertex2D>();
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			float mulColor = 1f;
			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
			if (i == 1)
			{
				mulColor = 0f;
			}

			if (i >= 2)
			{
				var normalDirII = Projectile.oldPos[i - 2] - Projectile.oldPos[i - 1];
				normalDirII = Vector2.Normalize(new Vector2(-normalDirII.Y, normalDirII.X));
				if (Vector2.Dot(normalDirII, normalDir) <= 0.965f)
				{
					mulColor = 0f;
				}
			}
			if (i < Projectile.oldPos.Length - 1)
			{
				var normalDirII = Projectile.oldPos[i] - Projectile.oldPos[i + 1];
				normalDirII = Vector2.Normalize(new Vector2(-normalDirII.Y, normalDirII.X));
				if (Vector2.Dot(normalDirII, normalDir) <= 0.965f)
				{
					mulColor = 0f;
				}
			}

			float k0 = (float)Math.Atan2(normalDir.Y, normalDir.X);
			k0 += 3.14f + 1.57f;
			if (k0 > 6.28f)
			{
				k0 -= 6.28f;
			}

			Color c0 = new Color(k0, 0.2f * mulColor, 0, 0) * MulByTimeLeft;

			var factor = i / (float)TrueL;
			float x0 = factor * 1.3f - (float)(Main.time / 150d) + 100000;
			x0 %= 1f;
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factor) + new Vector2(9f) - Main.screenPosition, c0, new Vector3(x0, 1, 0)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factor) + new Vector2(9f) - Main.screenPosition, c0, new Vector3(x0, 0, 0)));
			var factorII = factor;
			factor = (i + 1) / (float)TrueL;
			var x1 = factor * 1.3f - (float)(Main.time / 150d) + 100000;
			x1 %= 1f;
			if (x0 > x1)
			{
				float DeltaValue = 1 - x0;
				var factorIII = factorII * x0 + factor * DeltaValue;
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factorIII) + new Vector2(9f) - Main.screenPosition, c0, new Vector3(1, 1, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factorIII) + new Vector2(9f) - Main.screenPosition, c0, new Vector3(1, 0, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factorIII) + new Vector2(9f) - Main.screenPosition, c0, new Vector3(0, 1, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factorIII) + new Vector2(9f) - Main.screenPosition, c0, new Vector3(0, 0, 0)));
			}
		}
		Texture2D t = Commons.ModAsset.Trail_2.Value;

		if (bars.Count > 3)
		{
			spriteBatch.Draw(t, bars, PrimitiveType.TriangleStrip);
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		ProjHit();
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		ProjHit();
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		ProjHit();
		Projectile.tileCollide = false;
		return false;
	}
}
