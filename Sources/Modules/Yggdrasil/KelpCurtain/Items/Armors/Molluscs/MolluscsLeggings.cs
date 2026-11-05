using Terraria.GameContent.Creative;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Armors.Molluscs;

[AutoloadEquip(EquipType.Legs)]
public class MolluscsLeggings : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;
		Item.value = Item.buyPrice(silver: 60);
		Item.rare = ItemRarityID.Green;
		Item.defense = 4;
	}

	override public void UpdateEquip(Player player)
	{
		player.moveSpeed += 0.08f; // +8% movement speed

		// +35% movement speed while wet
		player.GetModPlayer<KelpCurtainPlayer>().MolluscsLeggings = true;
	}
}
