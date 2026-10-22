using Everglow.Commons.Utilities;
using Everglow.Yggdrasil.YggdrasilTown.Buffs;
using Everglow.Yggdrasil.YggdrasilTown.Items.Fishing;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Potions;

public class ScalePotion : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Potions;

	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetStaticDefaults()
	{
		Item.ResearchUnlockCount = 20;
	}

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 26;
		Item.scale = 0.1f;
		Item.useStyle = ItemUseStyleID.DrinkLiquid;
		Item.useTime = Item.useAnimation = 15;
		Item.useTurn = true;
		Item.UseSound = SoundID.Item3;
		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;
		Item.rare = ItemRarityID.Blue;
		Item.value = Item.buyPrice(gold: 1, silver: 23, copper: 36);
		Item.buffType = ModContent.BuffType<ScalePotionBuff>();
		Item.buffTime = 4 * 60 * 60;
	}

	public override void AddRecipes()
	{
		foreach (int ore in new[] { ItemID.IronOre, ItemID.LeadOre })
		{
			CreateRecipe()
				.AddIngredient(ItemID.BottledWater)
				.AddIngredient<Filefish>()
				.AddIngredient(ore)
				.Register();
		}
	}
}
