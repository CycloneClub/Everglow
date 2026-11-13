namespace Everglow.Myth.Acytaea.Vanities;

[AutoloadEquip(EquipType.Head)]
public class Acytaea_Horn : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Vanity;

	public override void SetDefaults()
	{
		Item.width = 22;
		Item.height = 28;
		Item.rare = ItemRarityID.Expert;
		Item.value = 50000;
		Item.vanity = true;
		Item.maxStack = 1;
	}
}
