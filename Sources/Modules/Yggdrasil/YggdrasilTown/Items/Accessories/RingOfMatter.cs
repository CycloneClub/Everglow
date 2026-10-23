namespace Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;

public class RingOfMatter : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	private const int MaxBonus = 8;

	private float Bonus { get; set; }

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 12;
		Item.accessory = true;
		Item.rare = ItemRarityID.Orange;
		Item.value = Item.buyPrice(gold: 10);
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		if (player.GetModPlayer<RingOfMatterPlayer>().HasNotEquipped)
		{
			int goldCoinNum = player.CountItem(ItemID.GoldCoin);
			int platinumCoinNum = player.CountItem(ItemID.PlatinumCoin);
			Bonus = (goldCoinNum + platinumCoinNum * 100) / 25;
			if (Bonus > MaxBonus)
			{
				Bonus = MaxBonus;
			}
		}

		player.GetDamage(DamageClass.Generic) += Bonus / 100;
		player.statDefense *= 1 + Bonus / 100;
		player.statLifeMax2 -= (int)(player.statLifeMax2 * (1.5f * Bonus / 100));

		player.GetModPlayer<RingOfMatterPlayer>().HasRingOfMatter = true;
	}
}

public class RingOfMatterPlayer : ModPlayer
{
	public bool HasNotEquipped { get; private set; } = false;

	public bool HasRingOfMatter { get; set; } = false;

	public override void ResetEffects()
	{
		HasNotEquipped = HasRingOfMatter ? false : true;
		HasRingOfMatter = false;
	}
}
