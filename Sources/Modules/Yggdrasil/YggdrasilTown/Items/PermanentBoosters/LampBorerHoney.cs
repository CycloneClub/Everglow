using Everglow.Yggdrasil.Common;
using Terraria;
using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.PermanentBoosters;

/// <summary>
/// "甘金蜜" Permanently increase life max by 1, at most use for 40 times.
/// </summary>
public class LampBorerHoney : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.PermanentBoosters;

	public static readonly int MaxUse = 40;
	public static readonly int LifePerHoney = 1;

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxUse, LifePerHoney);

	public override void SetStaticDefaults()
	{
		Item.ResearchUnlockCount = 12;
	}

	public override void SetDefaults()
	{
		Item.CloneDefaults(ItemID.LifeCrystal);
		Item.width = 30;
		Item.height = 26;
		Item.rare = ItemRarityID.Blue;
		Item.value = 15000;
		Item.maxStack = Item.CommonMaxStack;
		Item.useStyle = ItemUseStyleID.EatFood;
	}

	public override bool? UseItem(Player player) => player.GetModPlayer<YggdrasilPlayer>().UseLampBorerHoney();

	public override void Update(WorldItem worldItem, ref float gravity, ref float maxFallSpeed)
	{
		Lighting.AddLight(worldItem.Center, new Vector3(0.6f, 0.4f, 0));
		base.Update(worldItem, ref gravity, ref maxFallSpeed);
	}
}
