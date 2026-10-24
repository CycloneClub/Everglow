using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.PermanentBoosters;

public class InsectManaEssence : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.PermanentBoosters;

	public override string Texture => Commons.ModAsset.White_Mod;

	public const int MaxUse = 6;
	public const int ManaPerUse = 5;

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ManaPerUse, MaxUse);

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 26;
		Item.scale = 0.1f;
		Item.useStyle = ItemUseStyleID.DrinkLiquid;
		Item.useTime = Item.useAnimation = 30;
		Item.UseSound = SoundID.Item4;
		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;
		Item.rare = ItemRarityID.Blue;
	}

	public override bool CanUseItem(Player player) => player.GetModPlayer<YggdrasilPlayer>().ConsumedInsectManaEssence < MaxUse;

	public override bool? UseItem(Player player)
	{
		if (player.whoAmI == Main.myPlayer)
		{
			return player.GetModPlayer<YggdrasilPlayer>().UseInsectManaEssence();
		}
		return true;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(ItemID.BottledWater);
		recipe.AddIngredient<CaterpillarJuice>(6);
		recipe.AddTile(TileID.Bottles);
		recipe.Register();
	}
}
