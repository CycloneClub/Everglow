namespace Everglow.Commons.DeveloperContent.Items;

internal class ScreenPositionPrintItem : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Tools;

	public override string Texture => ModAsset.Wires_0_Mod;

	public override void SetDefaults()
	{
		Item.useStyle = ItemUseStyleID.HoldUp;
		Item.useTime = 40;
		Item.useAnimation = 40;
	}

	public override bool? UseItem(Player player)
	{
		Main.NewText($"screenPosition (X:{Main.screenPosition.X}, Y:{Main.screenPosition.Y})");
		return true;
	}

	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		tooltips.Add(new TooltipLine(ModIns.Mod, "ScreenPos", string.Empty + Main.screenPosition));
	}
}
