using Everglow.Yggdrasil.YggdrasilTown.Items.Fishing;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Buffs;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Potions;

public class RayPotion : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Potions;

	public override void SetStaticDefaults()
	{
		Item.ResearchUnlockCount = 20;
	}

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 26;
		Item.useStyle = ItemUseStyleID.DrinkLiquid;
		Item.useAnimation = Item.useTime = 15;
		Item.useTurn = true;
		Item.UseSound = SoundID.Item3;
		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;
		Item.rare = ItemRarityID.Blue;
		Item.value = Item.buyPrice(gold: 1);
		Item.buffType = ModContent.BuffType<RayBuff>();
		Item.buffTime = 21600;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<DimmyFish>(1);
		recipe.AddIngredient<TwilightCrystalExteactedLiquid>(2);
		recipe.AddIngredient(ItemID.BottledWater, 2);
		recipe.AddTile(TileID.Bottles);
		recipe.Register();
	}
}
