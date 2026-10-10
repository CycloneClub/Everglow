using Everglow.EternalResolve.Buffs;
using Terraria.Localization;

namespace Everglow.EternalResolve.Items.Potions
{
	// TODO:翻译 能量饮料 NightElixir
	public class EnergyDrink : ModItem
	{
		public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Potions;

		public static LocalizedText RestoreLifeText { get; private set; }

		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 30;
		}

		public override void SetDefaults()
		{
			Item.width = 14;
			Item.height = 34;
			Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useAnimation = 17;
			Item.useTime = 17;
			Item.useTurn = true;
			Item.UseSound = SoundID.Item3;
			Item.maxStack = 9999;
			Item.consumable = true;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.buyPrice(gold: 1);
			Item.buffType = ModContent.BuffType<StaminaEnergyBuff>();
			Item.buffTime = 10800;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
		}

		public override void AddRecipes()
		{
			CreateRecipe().AddIngredient(ItemID.Bottle).AddRecipeGroup(RecipeGroupID.Fruit).AddTile(TileID.CookingPots).AddCondition(Condition.NearHoney).Register();
			CreateRecipe().AddIngredient(ItemID.Bottle).AddRecipeGroup(RecipeGroupID.Fruit).AddTile(TileID.CookingPots).AddIngredient(ItemID.HoneyBlock).Register();
			CreateRecipe().AddIngredient(ItemID.Bottle).AddRecipeGroup(RecipeGroupID.Fruit).AddTile(TileID.CookingPots).AddIngredient(ItemID.SugarPlum).Register();
			base.AddRecipes();
		}
	}
}
