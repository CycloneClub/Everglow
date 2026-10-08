using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.Myth.Misc.Projectiles.Accessory;
using Terraria.Audio;

namespace Everglow.Myth.Misc.Items.Accessories;

public class CorruptEye : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetDefaults()
	{
		Item.width = 32;
		Item.height = 32;
		Item.value = 5000;
		Item.accessory = true;
		Item.rare = ItemRarityID.Pink;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.statDefense += 5;
		player.GetCritChance(DamageClass.Generic) += 4;
		player.buffImmune[39] = true;
		CorruptEyeEquiper cEE = player.GetModPlayer<CorruptEyeEquiper>();
		cEE.CorruptEyeEnable = true;
	}
}

internal class CorruptEyeEquiper : ModPlayer
{
	public bool CorruptEyeEnable = false;

	public override void ResetEffects()
	{
		CorruptEyeEnable = false;
	}

	public override void PostHurt(Player.HurtInfo info)
	{
		if (CorruptEyeEnable)
		{
			for (int i = 0; i < 5; i++)
			{
				Vector2 velocity = new Vector2(0, Main.rand.NextFloat(4.3f, 6f)).RotatedByRandom(6.283);
				var cursedFlame = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, velocity, ModContent.ProjectileType<CursedFlameBall>(), 60, 1.5f, Player.whoAmI);
				cursedFlame.timeLeft = Main.rand.Next(25, 45);
			}
			GenerateVFX(6, 1);
			SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot.WithPitchOffset(-0.2f), Player.Center);
		}
	}

	private void GenerateVFX(int frequency, float mulVelocity = 1f)
	{
		for (int g = 0; g < frequency * 3; g++)
		{
			var cf = new CurseFlameDust
			{
				Velocity = new Vector2(0, Main.rand.NextFloat(1.65f, 2.5f)).RotatedByRandom(6.283) * mulVelocity,
				Active = true,
				Visible = true,
				Position = Player.Center + new Vector2(Main.rand.NextFloat(-26f, 26f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(12, 66),
				Scale = 12f * mulVelocity,
				ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0.18f, 0.18f), Main.rand.NextFloat(1f, 2.2f) * mulVelocity },
			};
			Ins.VFXManager.Add(cf);
		}
		for (int g = 0; g < frequency; g++)
		{
			Vector2 vel = new Vector2(0, Main.rand.NextFloat(1.65f, 3.5f)).RotatedByRandom(6.283) * mulVelocity;
			var cf = new CurseFlameDust
			{
				Velocity = vel,
				Active = true,
				Visible = true,
				Position = Player.Center + vel * 3,
				MaxTime = Main.rand.Next(12, 70),
				Scale = 12f * mulVelocity,
				ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0.4f, 0.4f), Main.rand.NextFloat(2f, 3.2f) * mulVelocity },
			};
			Ins.VFXManager.Add(cf);
		}
		for (int g = 0; g < frequency * 3; g++)
		{
			Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(2f, 6f)).RotatedByRandom(MathHelper.TwoPi);
			var spark = new CurseFlameSparkDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) - Player.velocity + newVelocity * 3,
				MaxTime = Main.rand.Next(37, 145),
				Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(4f, 47.0f)),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.13f, 0.13f) },
			};
			Ins.VFXManager.Add(spark);
		}
	}
}
