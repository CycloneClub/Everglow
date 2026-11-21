using Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;
using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.JellyBall;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Items.Mounts;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.BossDrops;

public class KingJellyBallTreasureBag : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TreasureBags;

	public override void SetDefaults()
	{
		Item.maxStack = 999;
		Item.consumable = true;
		Item.width = 32;
		Item.height = 32;
		Item.rare = ItemRarityID.Cyan;
		Item.expert = true;
	}

	public override bool CanRightClick()
	{
		return true;
	}

	public override void ModifyItemLoot(ItemLoot itemLoot)
	{
		itemLoot.Add(ItemDropRule.OneFromOptionsNotScalingWithLuck(1, ModContent.ItemType<JellyBallInhibitor>(), ModContent.ItemType<MysteriousTablet>(), ModContent.ItemType<JellyBallBathingCap>()));
		itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<CrystalNucleusOfJellyKing>(), 1, 4, 6));
		itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<JellyBallCube>(), 1, 24, 40));
		itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<JellyBubbleItem>(), 2));
		itemLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 1, 15, 15));
	}
}
