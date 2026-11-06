using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Rock;

[AutoloadEquip(EquipType.Legs)]
public class RockGreaves : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MoveSpeedBonus);

	private const int MoveSpeedBonus = 6;

	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 26;
		Item.value = Item.buyPrice(silver: 94);
		Item.rare = ItemRarityID.Green;
		Item.defense = 6;
	}

	public override void UpdateEquip(Player player)
	{
		player.moveSpeed += MoveSpeedBonus / 100f;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<TwilightCrystal>(18);
		recipe.AddIngredient<GiantBeetleAntennae>(9);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}
}
