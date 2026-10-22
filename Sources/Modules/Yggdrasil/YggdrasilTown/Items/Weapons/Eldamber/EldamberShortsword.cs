using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Terraria.ModLoader;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.Eldamber;

/// <summary>
/// 古珀短剑：刺击型近战武器。
/// 已确定配方：古珀×8；在铁砧处制作。
/// </summary>
public class EldamberShortsword : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

	public override void SetDefaults()
	{
		Item.width = 32;
		Item.height = 32;
		Item.damage = 14;
		Item.DamageType = DamageClass.Melee;
		Item.useTime = Item.useAnimation = 12;
		Item.UseSound = SoundID.Item1;
		Item.autoReuse = true;
		Item.useTurn = true;
		Item.maxStack = 1;
		Item.knockBack = 4f;
		Item.rare = ItemRarityID.Blue;
		Item.value = Item.sellPrice(silver: 20);
		Item.useStyle = ItemUseStyleID.Rapier;
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.shoot = ProjectileID.GoldShortswordStab;
		Item.shootSpeed = 2.1f;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<YggdrasilAmber>(8);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}
}
