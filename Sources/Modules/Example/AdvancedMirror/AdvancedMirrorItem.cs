using Everglow.Commons.Utilities;
using Terraria.Localization;

namespace Everglow.Example.AdvancedMirror;

public class AdvancedMirrorItem : ModItem
{
	internal const string TextKey = "Mods.Everglow.Examples.AdvancedMirror.";

	public override string LocalizationCategory => LocalizationUtils.Categories.Placeables;

	public override string Texture => $"Terraria/Images/Item_{ItemID.IceMirror}";

	public override LocalizedText DisplayName => Language.GetText(TextKey + "Name");

	public override LocalizedText Tooltip => Language.GetText(TextKey + "Tooltip");

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableWall(ModContent.WallType<AdvancedMirrorWall>());
		Item.width = 24;
		Item.height = 28;
	}

	public override void AddRecipes() => CreateRecipe(24).AddIngredient(ItemID.Glass, 6).AddTile(TileID.WorkBenches).Register();

	public override bool CanUseItem(Player player) => Main.dedServ || player.whoAmI != Main.myPlayer
		|| !ModContent.GetInstance<AdvancedMirrorConfigUI>().CapturesMouse;
}
