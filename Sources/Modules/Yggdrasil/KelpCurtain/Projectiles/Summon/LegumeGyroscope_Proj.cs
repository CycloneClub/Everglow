using Everglow.Commons.Templates.Weapons.Gyroscopes;
using Everglow.Commons.Templates.Weapons.Whips;
using Everglow.Yggdrasil.KelpCurtain.Buffs;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Microsoft.Xna.Framework;
using Terraria;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Summon;

/// <summary>
/// Non-air summon projectile.
/// </summary>
public class LegumeGyroscope_Proj : GyroscopeProjectile
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
		SummonBuffType = ModContent.BuffType<LegumeGyroscopeBuff>();
	}

	/// <summary>
	/// Generate spark VFX from bottom;
	/// </summary>
	/// <param name="count"></param>
	public override void BottomSpark(int count = 1)
	{
		for (int i = 0; i < count; ++i)
		{
			Dust.NewDustDirect(Projectile.Bottom, 0, 0, ModContent.DustType<LegumeGyroscopeEffectDust>());
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
							int randCount = Main.rand.Next(1, 4);
							for (int i = 0; i < randCount; i++)
							{
								Vector2 vel = Projectile.velocity * 0.3f + new Vector2(0, -Main.rand.NextFloat(12f, 17f)).RotatedByRandom(0.6f);
								Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel, ModContent.ProjectileType<Legume_Proj>(), Projectile.damage / 2, 0.5f, Projectile.owner);
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
			if (Projectile.frame >= 6)
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
			if (Projectile.frame >= 9)
			{
				Projectile.frame = 6;
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
			player.ClearBuff(ModContent.BuffType<LegumeGyroscopeBuff>());
			Projectile.Kill();
		}
		if (player.HasBuff(ModContent.BuffType<LegumeGyroscopeBuff>()))
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
			Owner.ClearBuff(ModContent.BuffType<LegumeGyroscopeBuff>());
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
		for (int i = 0; i < count; ++i)
		{
			Dust.NewDustDirect(Projectile.Bottom, 0, 0, ModContent.DustType<LegumeGyroscopeEffectDust>());
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		Texture2D texture = ModAsset.LegumeGyroscope_Proj.Value;
		Texture2D textureBloom = ModAsset.LegumeGyroscope_bloom.Value;
		var frame = new Rectangle(0, 28 * Projectile.frame, 38, 28);
		if (Projectile.frame >= 6)
		{
			frame = new Rectangle(38, 28 * (Projectile.frame - 6), 38, 28);
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
