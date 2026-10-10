using Everglow.Commons.Utilities;
using Everglow.Example.Walls;
using Terraria.Localization;

namespace Everglow.Example.Items;

public class ExampleMirrorItem : ModItem
{
	internal const string TextKey = "Mods.Everglow.Common.MirrorDemo.";

	public override string LocalizationCategory => LocalizationUtils.Categories.Placeables;

	public override string Texture => $"Terraria/Images/Item_{ItemID.MagicMirror}";

	public override LocalizedText DisplayName => Language.GetText(TextKey + "Name");

	public override LocalizedText Tooltip => Language.GetText(TextKey + "Tooltip");

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableWall(ModContent.WallType<ExampleMirrorWall>());
		Item.width = 24;
		Item.height = 28;
		Item.value = 0;
	}

	public override void AddRecipes()
	{
		CreateRecipe(24)
			.AddIngredient(ItemID.Glass, 6)
			.AddTile(TileID.WorkBenches)
			.Register();
	}

	public override bool CanUseItem(Player player) => Main.dedServ || player.whoAmI != Main.myPlayer
		|| !ModContent.GetInstance<ExampleMirrorConfigUI>().CapturesMouse;
}
