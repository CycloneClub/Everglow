using Everglow.Myth.TheFirefly.Items.Materials;
using Terraria.GameContent.Creative;

namespace Everglow.Myth.TheFirefly.Items.Armors
{
	[AutoloadEquip(EquipType.Body)]
	public class FireflywoodBreastplate : ModItem
	{
		public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 28;
			Item.value = 6100;
			Item.rare = ItemRarityID.White;
			Item.defense = 6;
		}

		public override void UpdateEquip(Player player)
		{
			player.buffImmune[BuffID.ManaSickness] = true; // Was BuffID.OnFire
			player.statManaMax2 += 16;
			player.maxMinions++;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient<GlowWood>(50);
			recipe.AddIngredient<BlackStarShrub>(10);
			recipe.AddIngredient<GlowingPetal>(15);
			recipe.AddIngredient<FireflyMoss_Item>(5);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}
