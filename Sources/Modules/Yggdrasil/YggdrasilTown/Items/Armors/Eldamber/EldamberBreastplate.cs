using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Terraria.ModLoader;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Eldamber;

/// <summary>
/// 古珀护甲：身体护甲。
/// 已确定配方：古珀×20＋球冻王晶体×5；在铁砧处制作。
/// </summary>
[AutoloadEquip(EquipType.Body)]
public class EldamberBreastplate : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 24;
		Item.maxStack = 1;
		Item.defense = 8;
		Item.rare = ItemRarityID.Blue;
		Item.value = Item.sellPrice(silver: 30);
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<YggdrasilAmber>(20);
		recipe.AddIngredient<CrystalNucleusOfJellyKing>(5);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}
}
