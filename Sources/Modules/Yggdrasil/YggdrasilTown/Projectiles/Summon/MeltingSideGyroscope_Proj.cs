using Everglow.Commons.Templates.Weapons.Gyroscopes;
using Everglow.Commons.Templates.Weapons.Whips;
using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.Yggdrasil.YggdrasilTown.Buffs;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Microsoft.Xna.Framework;
using Terraria;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Summon;

/// <summary>
/// Non-air summon projectile.
/// </summary>
public class MeltingSideGyroscope_Proj : GyroscopeProjectile
{
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
		SummonBuffType = ModContent.BuffType<MeltingSideGyroscopeBuff>();
	}

	/// <summary>
	/// Generate spark VFX from bottom;
	/// </summary>
	/// <param name="count"></param>
	public override void BottomSpark(int count = 1)
	{
		for (int i = 0; i < count; ++i)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 2f)).RotatedByRandom(MathHelper.TwoPi) - new Vector2(Projectile.velocity.X, 0);
			var spark = new FireSparkDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Bottom,
				MaxTime = Main.rand.Next(7, 20),
				Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(4f, 27.0f)),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.03f, 0.03f) },
			};
			Ins.VFXManager.Add(spark);
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
			if (Projectile.frameCounter > 2400)
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

	/// <summary>
	/// If rightclick to cancel the summon buff, remove projectile.
	/// </summary>
	public override void CheckKill()
	{
		Player player = Main.player[Projectile.owner];
		if (player.dead || !player.active)
		{
			player.ClearBuff(ModContent.BuffType<MeltingSideGyroscopeBuff>());
			Projectile.Kill();
		}
		if (player.HasBuff(ModContent.BuffType<MeltingSideGyroscopeBuff>()))
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
			Owner.ClearBuff(ModContent.BuffType<MeltingSideGyroscopeBuff>());
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

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(BuffID.Burning, 120);
		base.OnHitNPC(target, hit, damageDone);
	}

	public override void KillingSpark(int count = 20)
	{
		for (int i = 0; i < count; ++i)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 14f)).RotatedByRandom(MathHelper.TwoPi);
			var spark = new FireSparkDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(7, 45),
				Scale = Main.rand.NextFloat(2f, Main.rand.NextFloat(4f, 27.0f)),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.03f, 0.03f) },
			};
			Ins.VFXManager.Add(spark);
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		Texture2D texture = ModAsset.MeltingSideGyroscope_Proj.Value;
		Texture2D textureGlow = ModAsset.MeltingSideGyroscope_Proj_glow.Value;
		Texture2D textureBloom = ModAsset.MeltingSideGyroscope_Proj_bloom.Value;
		var frame = new Rectangle(0, 24 * Projectile.frame, 28, 24);
		if (Projectile.frame >= 4)
		{
			frame = new Rectangle(28, 24 * (Projectile.frame - 4), 28, 24);
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
			Main.spriteBatch.Draw(textureBloom, Projectile.Bottom - Main.screenPosition, null, new Color(value, value, value, 0), Projectile.rotation, new Vector2(64, 78), Projectile.scale, SpriteEffects.None, 0);
		}
		Main.spriteBatch.Draw(texture, Projectile.Bottom - Main.screenPosition, frame, lightColor, Projectile.rotation, new Vector2(frame.Width * 0.5f, frame.Height), Projectile.scale, SpriteEffects.None, 0);
		if (Power > 500)
		{
			Main.spriteBatch.Draw(texture, Projectile.Bottom - Main.screenPosition + new Vector2(0, Main.rand.NextFloat(2f)).RotatedByRandom(MathHelper.TwoPi), frame, lightColor * 0.5f, Projectile.rotation, new Vector2(frame.Width * 0.5f, frame.Height), Projectile.scale, SpriteEffects.None, 0);
		}
		var glowColor = new Color(1f, 0.7f, 0.3f, 0) * 0.6f;
		Main.spriteBatch.Draw(textureGlow, Projectile.Bottom - Main.screenPosition, frame, glowColor, Projectile.rotation, new Vector2(frame.Width * 0.5f, frame.Height), Projectile.scale, SpriteEffects.None, 0);
		if (Power > 500)
		{
			Main.spriteBatch.Draw(textureGlow, Projectile.Bottom - Main.screenPosition + new Vector2(0, Main.rand.NextFloat(2f)).RotatedByRandom(MathHelper.TwoPi), frame, glowColor * 0.5f, Projectile.rotation, new Vector2(frame.Width * 0.5f, frame.Height), Projectile.scale, SpriteEffects.None, 0);
		}

		if (Owner != null)
		{
			var gyroscopePlayer = Owner.GetModPlayer<GyroscopePlayer>();
			if (gyroscopePlayer != null && gyroscopePlayer.EnablePowerBarUI)
			{
				DrawPowerBar();
			}
		}
		return false;
	}
}
