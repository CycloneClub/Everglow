using Everglow.Yggdrasil.Common;
using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.PermanentBoosters;

public class AntiHeavenSicknessPill : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.PermanentBoosters;

	public const int AntiHeavenSicknessPillMax = 5;
	public static readonly Dictionary<int, int> LifeBonusTable = new()
	{
		{ 1, 20 },
		{ 2, 10 },
		{ 3, 5 },
		{ 4, 3 },
		{ 5, 2 },
	};

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(
		AntiHeavenSicknessPillMax,
		LifeBonusTable[1], LifeBonusTable[2], LifeBonusTable[3], LifeBonusTable[4], LifeBonusTable[5]);

	public override void SetDefaults()
	{
		Item.width = 18;
		Item.height = 18;

		Item.useStyle = ItemUseStyleID.DrinkLiquid;
		Item.useTime = Item.useAnimation = 30;
		Item.UseSound = SoundID.Item4;

		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;

		Item.rare = ItemRarityID.Green;
		Item.value = Item.buyPrice(platinum: 0, gold: 2);
	}

	public override bool? UseItem(Player player) => player.GetModPlayer<YggdrasilPlayer>().UseAntiHeavenSicknessPill();
}
