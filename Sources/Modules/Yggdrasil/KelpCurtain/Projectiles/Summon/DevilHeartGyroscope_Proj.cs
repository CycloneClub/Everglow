using Everglow.Commons.Templates.Weapons.Gyroscopes;
using Everglow.Commons.Templates.Weapons.Whips;
using Everglow.Yggdrasil.KelpCurtain.Buffs;
using Everglow.Yggdrasil.KelpCurtain.VFXs;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Summon;

/// <summary>
/// Non-air summon projectile.
/// </summary>
public class DevilHeartGyroscope_Proj : GyroscopeProjectile
{
	public int BloodPower = 0;

	public override void SetDefaults()
	{
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 300;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.DamageType = DamageClass.Summon;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 12;
		Projectile.minionSlots = 1;
		Projectile.minion = true;
		BloodPower = 0;
		SummonBuffType = ModContent.BuffType<DevilHeartGyroscopeBuff>();
	}

	/// <summary>
	/// Generate spark VFX from bottom;
	/// </summary>
	/// <param name="count"></param>
	public override void BottomSpark(int count = 1)
	{
		float mulScale = 0.8f;
		if (BloodPower >= 120)
		{
			mulScale = 1f;
		}
		for (int i = 0; i < count; ++i)
		{
			Vector2 vel = new Vector2(0, Main.rand.NextFloat(0.6f, 1.4f)).RotatedByRandom(MathHelper.TwoPi) + Projectile.velocity - new Vector2(0, 2);
			var dust = new DevilHeart_Spark_gyroscope
			{
				Velocity = vel,
				Active = true,
				Visible = true,
				Position = Projectile.Bottom,
				MaxTime = Main.rand.Next(60, 90) * mulScale,
				Scale = Main.rand.NextFloat(3f, 5f) * mulScale,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(4.0f, 10.93f) },
			};
			Ins.VFXManager.Add(dust);
		}
	}

	/// <summary>
	/// Whipping gyroscope can charge power.
	/// </summary>
	public override void CheckWhipHit()
	{
		foreach (var proj in Main.projectile)
		{
			if (proj != null && proj.active)
			{
				bool isAWhip = proj.aiStyle == ProjAIStyleID.Whip;
				if (!isAWhip)
				{
					if (proj.ModProjectile != null && proj.ModProjectile is WhipProjectile)
					{
						isAWhip = true;
					}
				}
				if (isAWhip)
				{
					int index = WhipCoolingsForProjectileWhoAmI.FindIndex(p => p.ProjectileWhoAmI == proj.whoAmI);
					if (index < 0)
					{
						if (proj.Colliding(proj.Hitbox, Projectile.Hitbox))
						{
							int whipValue = 150;
							CombatText.NewText(Projectile.Hitbox, new Color(1f, 1f, 1f, 1f), whipValue);
							Power += whipValue;
							if (Power > MaxPower)
							{
								Power = MaxPower;
							}
							WhipCoolingsForProjectileWhoAmI.Add((proj.whoAmI, 10));
							KillingSpark(36);
							if (BloodPower >= 120)
							{
								HitSpark();
								BloodPower = 0;
								Item.NewItem(Projectile.GetSource_FromAI(), Projectile.Hitbox, new Item(ItemID.Heart));
								Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<DevilHeartGyroscope_Proj_Hit>(), 0, 0, Projectile.owner);
							}
							return;
						}
					}
				}
			}
		}
		for (int i = WhipCoolingsForProjectileWhoAmI.Count - 1; i > -1; i--)
		{
			if (WhipCoolingsForProjectileWhoAmI[i].CoolingTimer > 0)
			{
				WhipCoolingsForProjectileWhoAmI[i] = (WhipCoolingsForProjectileWhoAmI[i].ProjectileWhoAmI, WhipCoolingsForProjectileWhoAmI[i].CoolingTimer - 1);
			}
			else
			{
				WhipCoolingsForProjectileWhoAmI.RemoveAt(i);
			}
		}
	}

	/// <summary>
	/// Low speed and high speed behave differently.
	/// </summary>
	public override void FindFrame()
	{
		Projectile.frameCounter += Math.Clamp((int)Power, 300, 600);
		if (Power < 400)
		{
			if (Projectile.frameCounter > 1200)
			{
				Projectile.frame++;
				Projectile.frameCounter = 0;
			}
			if (Projectile.frame >= 4)
			{
				Projectile.frame = 0;
			}
		}
		else
		{
			if (Projectile.frameCounter > 1200)
			{
				Projectile.frame++;
				Projectile.frameCounter = 0;
			}
			if (Projectile.frame >= 7)
			{
				Projectile.frame = 4;
			}
		}
		if (Power > 100)
		{
			Projectile.rotation = Math.Clamp(Projectile.velocity.X * 0.05f, -0.8f, 0.8f);
		}
		else
		{
			float targetRot = (1.5f - Power / 100f) * MathF.Sin((float)Main.time * 0.24f + Projectile.whoAmI);
			Projectile.rotation = targetRot * 0.1f + Projectile.rotation * 0.9f;
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		BloodPower += hit.Damage;
		base.OnHitNPC(target, hit, damageDone);
	}

	/// <summary>
	/// If rightclick to cancel the summon buff, remove projectile.
	/// </summary>
	public override void CheckKill()
	{
		Player player = Main.player[Projectile.owner];
		if (player.dead || !player.active)
		{
			player.ClearBuff(ModContent.BuffType<DevilHeartGyroscopeBuff>());
			Projectile.Kill();
		}
		if (player.HasBuff(ModContent.BuffType<DevilHeartGyroscopeBuff>()))
		{
			Projectile.timeLeft = 2;
		}
		else
		{
			Projectile.Kill();
		}
	}

	/// <summary>
	/// Killing effect: some smoke.
	/// </summary>
	/// <param name="timeLeft"></param>
	public override void OnKill(int timeLeft)
	{
		if (Owner.ownedProjectileCounts[Type] <= 1)
		{
			Owner.ClearBuff(ModContent.BuffType<DevilHeartGyroscopeBuff>());
		}
		for (int i = 0; i < 8; i++)
		{
			int type;
			switch (Main.rand.Next(3))
			{
				case 0:
					type = GoreID.ChimneySmoke1;
					break;
				case 1:
					type = GoreID.ChimneySmoke2;
					break;
				case 2:
					type = GoreID.ChimneySmoke3;
					break;
				default:
					type = GoreID.ChimneySmoke1;
					break;
			}

			var gore = Gore.NewGorePerfect(Projectile.Center + new Vector2(-20), new Vector2(0, Main.rand.NextFloat(1f)).RotatedByRandom(MathHelper.TwoPi), type);
			gore.timeLeft = Main.rand.Next(60, 120);
		}
		KillingSpark();
	}

	public override void KillingSpark(int count = 20)
	{
		if (BloodPower >= 120)
		{
			count += 27;
		}
		for (int i = 0; i < count; ++i)
		{
			Vector2 vel = new Vector2(0, Main.rand.NextFloat(6.6f, 9.6f)).RotatedByRandom(MathHelper.TwoPi);
			if (BloodPower >= 120)
			{
				vel *= 2.4f;
			}
			var dust = new DevilHeart_Spark_gyroscope
			{
				Velocity = vel,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(60, 90),
				Scale = Main.rand.NextFloat(3f, 5f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(4.0f, 10.93f) },
			};
			Ins.VFXManager.Add(dust);
		}
	}

	public void HitSpark()
	{
		for (int i = 0; i < 6; i++)
		{
			float rotSpeed = 0;
			Vector2 vel = new Vector2(0, Main.rand.NextFloat(-6, -4)).RotatedByRandom(0.9);
			var dustVFX = new Heart_VFX
			{
				Omega = rotSpeed,
				beta = -rotSpeed * 0.05f,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				Velocity = vel,
				MaxTime = vel.Length() * 12,
				Scale = 9f,
				DustColor = Color.Lerp(Color.Red, Color.White, (vel.Length() - 3) / 2f),
				ai = new float[] { Main.rand.NextFloat(1f, 8f) },
			};
			Ins.VFXManager.Add(dustVFX);
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Texture2D texture = ModAsset.DevilHeartGyroscope_Proj.Value;
		Texture2D textureBloom = ModAsset.DevilHeartGyroscope_bloom.Value;
		Texture2D textureGlow = ModAsset.DevilHeartGyroscope_Proj_glow.Value;
		var frame = new Rectangle(0, 32 * Projectile.frame, 32, 32);
		if (Projectile.frame >= 4)
		{
			frame = new Rectangle(32, 32 * (Projectile.frame - 4), 32, 32);
		}
		int whipCooling = 0;
		foreach (var pC in WhipCoolingsForProjectileWhoAmI)
		{
			if (pC.CoolingTimer > whipCooling)
			{
				whipCooling = pC.CoolingTimer;
			}
		}
		if (whipCooling > 0)
		{
			float value = whipCooling / 10f;
			Main.spriteBatch.Draw(textureBloom, Projectile.Bottom - Main.screenPosition, null, new Color(value, value, value, 0), Projectile.rotation, new Vector2(64, 80), Projectile.scale, SpriteEffects.None, 0);
		}
		Main.spriteBatch.Draw(texture, Projectile.Bottom - Main.screenPosition, frame, lightColor, Projectile.rotation, new Vector2(frame.Width * 0.5f, frame.Height), Projectile.scale, SpriteEffects.None, 0);
		Vector2 randomVec = new Vector2(0, Main.rand.NextFloat(2f)).RotatedByRandom(MathHelper.TwoPi);
		if (Power > 500)
		{
			Main.spriteBatch.Draw(texture, Projectile.Bottom - Main.screenPosition + randomVec, frame, lightColor * 0.5f, Projectile.rotation, new Vector2(frame.Width * 0.5f, frame.Height), Projectile.scale, SpriteEffects.None, 0);
		}
		if (Owner != null)
		{
			var gyroscopePlayer = Owner.GetModPlayer<GyroscopePlayer>();
			if (gyroscopePlayer != null && gyroscopePlayer.EnablePowerBarUI)
			{
				DrawPowerBar();
			}
		}
		if (BloodPower >= 120)
		{
			BloodPower = 120;
			Main.spriteBatch.Draw(textureGlow, Projectile.Bottom - Main.screenPosition, frame, new Color(1f, 1f, 1f, 0), Projectile.rotation, new Vector2(frame.Width * 0.5f, frame.Height), Projectile.scale, SpriteEffects.None, 0);
			if (Power > 500)
			{
				Main.spriteBatch.Draw(textureGlow, Projectile.Bottom - Main.screenPosition + randomVec, frame, new Color(1f, 1f, 1f, 0) * 0.5f, Projectile.rotation, new Vector2(frame.Width * 0.5f, frame.Height), Projectile.scale, SpriteEffects.None, 0);
			}
		}
		return false;
	}
}
