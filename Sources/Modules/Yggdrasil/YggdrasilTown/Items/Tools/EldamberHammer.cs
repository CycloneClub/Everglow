using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Terraria.ModLoader;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Tools;

/// <summary>
/// 古珀锤：锤类工具。
/// 已确定配方：古珀×10；在铁砧处制作。
/// </summary>
public class EldamberHammer : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Tools;

	public override void SetDefaults()
	{
		Item.width = 32;
		Item.height = 32;
		Item.damage = 14;
		Item.DamageType = DamageClass.Melee;
		Item.useTime = Item.useAnimation = 26;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.UseSound = SoundID.Item1;
		Item.autoReuse = true;
		Item.useTurn = true;
		Item.maxStack = 1;
		Item.knockBack = 4f;
		Item.rare = ItemRarityID.Blue;
		Item.value = Item.sellPrice(silver: 20);
		Item.hammer = 55;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<YggdrasilAmber>(10);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}
}
