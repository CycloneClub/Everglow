using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Eldamber;

/// <summary>
/// 古珀靴子：腿部护甲。
/// 已确定配方：古珀×16＋球冻王晶体×4；在铁砧处制作。
/// </summary>
[AutoloadEquip(EquipType.Legs)]
public class EldamberBoots : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(4);

	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 24;
		Item.maxStack = 1;
		Item.defense = 5;
		Item.rare = ItemRarityID.Blue;
		Item.value = Item.sellPrice(silver: 30);
	}

	public override void UpdateEquip(Player player)
	{
		player.GetCritChance(DamageClass.Generic) += 4f;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<YggdrasilAmber>(16);
		recipe.AddIngredient<CrystalNucleusOfJellyKing>(4);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}
}
