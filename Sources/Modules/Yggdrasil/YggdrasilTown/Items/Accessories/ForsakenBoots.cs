using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;

[AutoloadEquip(EquipType.Shoes)]
public class ForsakenBoots : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MoveSpeedBonus, 6.25f);

	public const int MoveSpeedBonus = 5;

	public override void SetDefaults()
	{
		Item.width = 22;
		Item.height = 22;

		Item.accessory = true;
		Item.rare = ItemRarityID.Blue;
		Item.value = Item.buyPrice(gold: 2, silver: 32, copper: 65);
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.moveSpeed += MoveSpeedBonus / 100f;

		// Enable running ability
		player.accRunSpeed = 6.25f;
	}
}
