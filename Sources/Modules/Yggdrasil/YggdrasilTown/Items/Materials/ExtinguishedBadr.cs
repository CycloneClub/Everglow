using Everglow.Commons.Utilities;
using Everglow.Yggdrasil.YggdrasilTown.Items.Pets;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Materials;

public sealed class ExtinguishedBadr : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Materials;

	public override string Texture => ModAsset.BrokenMiningLamp_Mod;

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 32;
		Item.maxStack = 1;
		Item.rare = ItemRarityID.Quest;
		Item.questItem = true;
	}

	public override void AddRecipes()
	{
		Recipe.Create(ModContent.ItemType<Badr>())
			.AddIngredient(Type)
			.AddTile<GiantFurnace>()
			.Register();
	}
}
