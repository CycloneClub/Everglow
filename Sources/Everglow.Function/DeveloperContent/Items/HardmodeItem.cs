using Everglow.Commons.FeatureFlags;

namespace Everglow.Commons.DeveloperContent.Items;

internal class HardmodeItem : ModItem
{
	// public override bool CloneNewInstances => true;
	public override string Texture => ModAsset.Wires_0_Mod;

	public override void SetDefaults()
	{
		Item.useStyle = ItemUseStyleID.HoldUp;
		Item.useTime = 40;
		Item.useAnimation = 40;
	}

	public override bool? UseItem(Player player)
	{
		if (!Main.hardMode)
		{
			Main.NewText("Activated Hardmode");
			Main.hardMode = true;
		}
		else
		{
			Main.NewText("Deactivated Hardmode");
			Main.hardMode = false;
		}

		// Ins.VFXManager.Add(new WhiteDust() { position = Main.MouseWorld });
		return true;
	}

	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		if (!Main.hardMode)
		{
			tooltips.Add(new TooltipLine(ModIns.Mod, "IsNotHardmode", "Not in Hardmode"));
		}
		else
		{
			tooltips.Add(new TooltipLine(ModIns.Mod, "IsHardmode", "In Hardmode"));
		}
	}

	public override void AddRecipes()
	{
		if (/*fireflyBiome.IsBiomeActive(Main.LocalPlayer) &&*/ EverglowConfig.DebugMode)
		{
			CreateRecipe().Register();
		}
	}
}
