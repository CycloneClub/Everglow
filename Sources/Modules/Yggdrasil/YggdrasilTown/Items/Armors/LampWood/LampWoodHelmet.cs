using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Terraria.GameContent.Creative;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Armors.LampWood;

[AutoloadEquip(EquipType.Head)]
public class LampWoodHelmet : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	private const int PickSpeedBonus = 8;
	private const int ArmorSetPickSpeedBonus = 8;
	private const int ArmorSetLifeRegenBonus = 1;
	private const int ArmorSetDefenseBonus = 2;

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.width = 38;
		Item.height = 24;
		Item.value = Item.buyPrice(silver: 20);
		Item.rare = ItemRarityID.Green;
		Item.defense = 3;
	}

	public override void UpdateEquip(Player player)
	{
		player.pickSpeed -= PickSpeedBonus / 100f;
	}

	public override bool IsArmorSet(Item head, Item body, Item legs)
	{
		return body.type == ModContent.ItemType<LampWoodBreastplate>() && legs.type == ModContent.ItemType<LampWoodLeggings>();
	}

	public override void UpdateArmorSet(Player player)
	{
		player.statDefense += ArmorSetDefenseBonus;
		player.lifeRegen += ArmorSetLifeRegenBonus;
		player.pickSpeed -= ArmorSetPickSpeedBonus / 100f;
		Lighting.AddLight(player.Center, 0.5f, 0.5f, 0f);
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<LampWood_Wood>(25);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}
