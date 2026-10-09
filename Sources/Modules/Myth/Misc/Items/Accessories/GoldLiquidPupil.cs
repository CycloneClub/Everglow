using Everglow.Myth.Misc.Projectiles.Accessory;
using Terraria;
using Terraria.Audio;

namespace Everglow.Myth.Misc.Items.Accessories;

[AutoloadEquip(EquipType.Neck)]
public class GoldLiquidPupil : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetDefaults()
	{
		Item.width = 58;
		Item.height = 62;
		Item.value = 5503;
		Item.accessory = true;
		Item.rare = ItemRarityID.Pink;
	}
	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		GoldLiquidPupilEquiper gLPE = player.GetModPlayer<GoldLiquidPupilEquiper>();
		gLPE.GoldLiquidPupilEnable = true;
	}
}
internal class GoldLiquidPupilEquiper : ModPlayer
{
	public bool GoldLiquidPupilEnable = false;
	public override void ResetEffects()
	{
		GoldLiquidPupilEnable = false;
	}
	public override void PostHurt(Player.HurtInfo info)
	{
		if (GoldLiquidPupilEnable)
		{
			if (Player.ownedProjectileCounts[ModContent.ProjectileType<IchorRing>()] <= 0)
			{
				Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<IchorRing>(), 6, 1.5f, Player.whoAmI);
				for (int i = 0; i < 12; i++)
				{
					GenerateDust();
				}
				SoundEngine.PlaySound(SoundID.Splash.WithPitchOffset(-0.2f), Player.Center);
			}
		}
	}
	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		if (GoldLiquidPupilEnable)
		{
			// modifiers.FinalDamage += target.defense * 0.175f;
		}
	}

	public override void ModifyHurt(ref Player.HurtModifiers modifiers)
	{
		if (modifiers.PvP)
		{
			var attacker = Main.player[modifiers.DamageSource.SourcePlayerIndex];
			if (attacker.GetModPlayer<GoldLiquidPupilEquiper>().GoldLiquidPupilEnable)
			{
				modifiers.FinalDamage += Player.statDefense * 0.175f;
			}
		}
	}
	private void GenerateDust()
	{
		Vector2 velocity = new Vector2(0, Main.rand.NextFloat(4.3f, 6f)).RotatedByRandom(6.283);
		var D = Dust.NewDustDirect(Player.Center - new Vector2(4)/*Dust的Size=8x8*/, 0, 0, DustID.Ichor, 0, 0, 150, default, Main.rand.NextFloat(0.4f, 1.1f));
		D.noGravity = true;
		D.velocity = velocity;
	}
}
