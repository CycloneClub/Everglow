using Everglow.Yggdrasil.Common;
using Terraria.Localization;

namespace Everglow.Yggdrasil.KelpCurtain.Items.PermanentBoosters;

/// <summary>
/// "绿琉璃果" Permanently increase life max by 5, at most use for 10 times.
/// </summary>
public class JadeFruit : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.PermanentBoosters;

	public static readonly int MaxJadeFruits = 10;
	public static readonly int LifePerJadeFruit = 5;

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxJadeFruits, LifePerJadeFruit);

	public override void SetStaticDefaults()
	{
		Item.ResearchUnlockCount = 12;
	}

	public override void SetDefaults()
	{
		Item.CloneDefaults(ItemID.LifeCrystal);
		Item.width = 30;
		Item.height = 34;
		Item.rare = ItemRarityID.Blue;
		Item.value = 15000;
		Item.maxStack = Item.CommonMaxStack;
		Item.useStyle = ItemUseStyleID.EatFood;
	}

	public override bool? UseItem(Player player) => player.GetModPlayer<YggdrasilPlayer>().UseJadeGlazeFruit();
}
