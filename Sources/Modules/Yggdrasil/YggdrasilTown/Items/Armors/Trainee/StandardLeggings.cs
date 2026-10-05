using Terraria.GameContent.Creative;
using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Trainee;

[AutoloadEquip(EquipType.Legs)]
public class StandardLeggings : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(3);

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 26;
		Item.value = 0;
		Item.rare = ItemRarityID.Green;
		Item.defense = 1;
	}

	public override void UpdateEquip(Player player)
	{
		player.jumpSpeedBoost += 0.05f;
		player.moveSpeed += 0.05f;
	}

	public override void AddRecipes()
	{
	}
}
