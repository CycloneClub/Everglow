namespace Everglow.Commons.Utilities;

public class AdministratorMode : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public static bool Enabled = false;

	public override void SetDefaults()
	{
		Item.width = 16;
		Item.height = 16;
		Item.useTurn = true;
		Item.useAnimation = 4;
		Item.useTime = 4;
		Item.autoReuse = false;
		Item.useStyle = ItemUseStyleID.Swing;
	}

	public override bool CanUseItem(Player player)
	{
		Enabled = !Enabled;
		Main.NewText($"Administrator mode {(Enabled ? "enabled" : "disabled")}.", Enabled ? Color.Green : Color.Red);
		return true;
	}
}
