using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Twilight;

[AutoloadEquip(EquipType.Head)]
public class TwilightWoodHelmet : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	public const int DamageBonus = 7;
	public const int CritChanceBonus = 4;

	public const int ArmorSetDamageBonus = 3;
	public const int ArmorSetCritChanceBonus = 3;
	public const int ArmorSetSaveAmmoChance = 15;

	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 26;
		Item.value = Item.buyPrice(silver: 37, copper: 50);
		Item.rare = ItemRarityID.White;
		Item.defense = 2;
	}

	public override void UpdateEquip(Player player)
	{
		player.GetDamage(DamageClass.Ranged) += DamageBonus / 100f;
		player.GetCritChance(DamageClass.Ranged) += CritChanceBonus;
	}

	public override bool IsArmorSet(Item head, Item body, Item legs)
	{
		return body.type == ModContent.ItemType<TwilightWoodBreastplate>() &&
			legs.type == ModContent.ItemType<TwilightWoodLeggings>();
	}

	public override void UpdateArmorSet(Player player)
	{
		player.GetDamage(DamageClass.Ranged) += ArmorSetDamageBonus / 100f;
		player.GetCritChance(DamageClass.Ranged) += ArmorSetCritChanceBonus;
		player.GetModPlayer<EverglowPlayer>().ammoCost *= 1 - ArmorSetSaveAmmoChance / 100f;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<TwilightEucalyptusWood_Item>(20);
		recipe.AddIngredient<TwilightCrystal>(8);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}
