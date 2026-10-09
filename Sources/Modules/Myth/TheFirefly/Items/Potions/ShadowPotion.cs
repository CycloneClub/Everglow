using Everglow.Myth.TheFirefly.Buffs;
using Everglow.Myth.TheFirefly.Items.Materials;
using Terraria.Localization;

namespace Everglow.Myth.TheFirefly.Items.Potions
{
	// TODO:翻译 幽夜药剂 NightElixir
	public class ShadowPotion : ModItem
	{
		public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Potions;

		public static LocalizedText RestoreLifeText { get; private set; }

		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 30;
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 26;
			Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useAnimation = 17;
			Item.useTime = 17;
			Item.useTurn = true;
			Item.UseSound = SoundID.Item3;
			Item.maxStack = 9999;
			Item.consumable = true;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.buyPrice(gold: 1);
			Item.buffType = ModContent.BuffType<ShadowPotionBuff>();
			Item.buffTime = 14400;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			// TODO: Use actual tooltips
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<GlowingPetal>(), 1);
			recipe.AddIngredient(ModContent.ItemType<GlowingFirefly>(), 1);
			recipe.AddIngredient(ItemID.Moonglow, 1);
			recipe.AddIngredient(ItemID.BottledWater, 1);
			recipe.AddTile(TileID.AlchemyTable);
			recipe.Register();
		}
	}
}
