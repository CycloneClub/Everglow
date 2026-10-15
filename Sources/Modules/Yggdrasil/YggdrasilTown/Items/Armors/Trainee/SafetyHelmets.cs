using Terraria.GameContent.Creative;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Trainee;

[AutoloadEquip(EquipType.Head)]
public class SafetyHelmets : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
	}

	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 26;
		Item.value = 0;
		Item.rare = ItemRarityID.Green;
		Item.defense = 1;
	}

	public override bool IsArmorSet(Item head, Item body, Item legs)
	{
		return body.type == ModContent.ItemType<ConventionalEquipment>() && legs.type == ModContent.ItemType<StandardLeggings>();
	}

	public override void UpdateArmorSet(Player player)
	{
		player.statDefense += 2;
		player.lifeRegen += 1;
	}

	public override void UpdateEquip(Player player)
	{
		player.GetDamage(DamageClass.Generic) += 0.02f;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();

		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}
