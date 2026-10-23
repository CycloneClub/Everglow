using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Terraria.ModLoader;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Eldamber;

/// <summary>
/// 古珀头盔：头部护甲。
/// 已确定配方：古珀×12＋球冻王晶体×3；在铁砧处制作。
/// </summary>
[AutoloadEquip(EquipType.Head)]
public class EldamberHelmet : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 24;
		Item.maxStack = 1;
		Item.defense = 6;
		Item.rare = ItemRarityID.Blue;
		Item.value = Item.sellPrice(silver: 30);
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<YggdrasilAmber>(12);
		recipe.AddIngredient<CrystalNucleusOfJellyKing>(3);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}
}
