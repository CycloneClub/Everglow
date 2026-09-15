using Everglow.Commons.Templates.Weapons.Gyroscopes;
using Everglow.Yggdrasil.KelpCurtain.Buffs;
using Everglow.Yggdrasil.KelpCurtain.VFXs;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Summon;

/// <summary>
/// Non-air summon projectile.
/// </summary>
public class RedAlgaeMinionGyroscope_Proj : GyroscopeProjectile
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		SummonBuffType = ModContent.BuffType<RedAlgaeMinionGyroscopeBuff>();
	}

	/// <summary>
	/// Generate spark VFX from bottom;
	/// </summary>
	/// <param name="count"></param>
	public override void BottomSpark(int count = 1)
	{
		for (int i = 0; i < count; ++i)
		{
			if (Main.rand.NextBool(2))
			{
				var redAlgaeDust = new RedAlgae_Small_Dust();
				redAlgaeDust.Position = Projectile.Bottom;
				redAlgaeDust.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
				redAlgaeDust.Velocity = new Vector2(0, Main.rand.NextFloat(0.2f)).RotatedByRandom(MathHelper.TwoPi);
				redAlgaeDust.ai = new float[] { 0.99f };
				redAlgaeDust.MaxTime = 30;
				redAlgaeDust.Scale = Main.rand.NextFloat(0.7f, 2f);
				redAlgaeDust.Visible = true;
				redAlgaeDust.Active = true;
				Ins.VFXManager.Add(redAlgaeDust);
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
		base.FindFrame();
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		int type = ModContent.BuffType<RedAlgae_FriendlyDebuff>();
		if (!target.HasBuff(type))
		{
			target.AddBuff(type, 900);
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
			player.ClearBuff(SummonBuffType);
			Projectile.Kill();
		}
		if (player.HasBuff(SummonBuffType))
		{
			Projectile.timeLeft = 2;
		}
		else
		{
			Projectile.Kill();
		}
	}

	public override void KillingSpark(int count = 20) => base.KillingSpark(count);

	public override void WhipSpark(int count = 20)
	{
		for (int k = 0; k < 18; k++)
		{
			var redAlgaeDust = new RedAlgae_Spark();
			redAlgaeDust.Position = Projectile.Center;
			redAlgaeDust.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
			redAlgaeDust.Velocity = new Vector2(0, 6).RotatedBy(MathHelper.TwoPi * k / 18f);
			redAlgaeDust.ai = new float[] { 0.9f };
			redAlgaeDust.MaxTime = 50;
			redAlgaeDust.Scale = Main.rand.NextFloat(2.7f, 5f);
			redAlgaeDust.Visible = true;
			redAlgaeDust.Active = true;
			Ins.VFXManager.Add(redAlgaeDust);
		}
		for (int k = 0; k < 12; k++)
		{
			var redAlgaeDust = new RedAlgae_Spark();
			redAlgaeDust.Position = Projectile.Center;
			redAlgaeDust.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
			redAlgaeDust.Velocity = new Vector2(0, 3).RotatedBy(MathHelper.TwoPi * k / 12f);
			redAlgaeDust.ai = new float[] { 0.9f };
			redAlgaeDust.MaxTime = 50;
			redAlgaeDust.Scale = Main.rand.NextFloat(2.7f, 5f);
			redAlgaeDust.Visible = true;
			redAlgaeDust.Active = true;
			Ins.VFXManager.Add(redAlgaeDust);
		}
		for (int k = 0; k < 20; k++)
		{
			var redAlgaeDust = new RedAlgae_Small_Dust();
			redAlgaeDust.Position = Projectile.Center;
			redAlgaeDust.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
			redAlgaeDust.Velocity = new Vector2(0, Main.rand.NextFloat(8f, 24f)).RotatedByRandom(MathHelper.TwoPi);
			redAlgaeDust.ai = new float[] { 0.9f };
			redAlgaeDust.Frame = Main.rand.Next(10);
			redAlgaeDust.MaxTime = 60;
			redAlgaeDust.Scale = Main.rand.NextFloat(1.7f, 2f);
			redAlgaeDust.Visible = true;
			redAlgaeDust.Active = true;
			Ins.VFXManager.Add(redAlgaeDust);
		}
		var gasRing = new RedAlgae_GasRing();
		gasRing.Position = Projectile.Center;
		gasRing.MaxTime = 60;
		gasRing.Scale = 10f;
		gasRing.Visible = true;
		gasRing.Active = true;
		Ins.VFXManager.Add(gasRing);
		int type = ModContent.BuffType<RedAlgae_FriendlyDebuff>();
		foreach (var npc in Main.npc)
		{
			if (npc is not null && npc.active)
			{
				if (!npc.friendly && !npc.dontTakeDamage && MathUtils.IntersectsCircleAABB(Projectile.Center, 180, npc.position, npc.position + npc.Size))
				{
					if (!npc.HasBuff(type))
					{
						npc.AddBuff(type, 900);
					}
				}
			}
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D texture = ModAsset.RedAlgaeMinionGyroscope_Proj.Value;
		Texture2D textureBloom = ModAsset.RedAlgaeMinionGyroscope_bloom.Value;
		Texture2D textureGlow = ModAsset.RedAlgaeMinionGyroscope_Proj_glow.Value;
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
		Main.spriteBatch.Draw(textureGlow, Projectile.Bottom - Main.screenPosition, frame, new Color(1f, 1f, 1f, 0), Projectile.rotation, new Vector2(frame.Width * 0.5f, frame.Height), Projectile.scale, SpriteEffects.None, 0);
		return false;
	}
}
