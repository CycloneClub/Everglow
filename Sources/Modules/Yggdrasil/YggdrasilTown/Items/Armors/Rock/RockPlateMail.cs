using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Rock;

[AutoloadEquip(EquipType.Body)]
public class RockPlateMail : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	public const int MeleeDamageBonus = 5;

	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 26;
		Item.value = Item.buyPrice(gold: 1, silver: 7);
		Item.rare = ItemRarityID.Green;
		Item.defense = 8;
	}

	public override void UpdateEquip(Player player)
	{
		player.GetDamage(DamageClass.Melee) += MeleeDamageBonus / 100f;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<TwilightCrystal>(24);
		recipe.AddIngredient<GiantBeetleAntennae>(12);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}
}
