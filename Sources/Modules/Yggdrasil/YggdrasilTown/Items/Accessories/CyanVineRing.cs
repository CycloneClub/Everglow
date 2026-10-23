using Everglow.Yggdrasil.YggdrasilTown.Buffs;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;

public class CyanVineRing : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public const int BuffDuration = 600;

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 12;
		Item.accessory = true;
		Item.rare = ItemRarityID.Green;
		Item.value = Item.buyPrice(silver: 88);
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		// 1. 3% Crit chance.
		player.GetCritChance(DamageClass.Generic) += 3;
		player.GetModPlayer<CyanVineRingPlayer>().HasCyanVineRing = true;
	}
}

public class CyanVineRingPlayer : ModPlayer
{
	public bool HasCyanVineRing = false;
	public int HurtTimer = 0;

	public override void ResetEffects()
	{
		HasCyanVineRing = false;
		if (HurtTimer > 0)
		{
			HurtTimer--;
		}
		else
		{
			HurtTimer = 0;
		}
	}

	public override void PostHurt(Player.HurtInfo info)
	{
		if (HasCyanVineRing)
		{
			HurtTimer = 90;
		}
	}

	public override void OnHitAnything(float x, float y, Entity victim)
	{
		if (HurtTimer > 0)
		{
			Player.AddBuff(ModContent.BuffType<CyanVineRingCrit>(), CyanVineRing.BuffDuration);
		}
	}
}
