using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Terraria.ModLoader;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.Eldamber;

/// <summary>
/// 古珀弓：远程武器，使用箭类弹药。
/// 已确定配方：古珀×10；在铁砧处制作。
/// </summary>
public class EldamberBow : ModItem
{
	// 临时使用已有白色纹理，待正式素材补齐后替换。
	public override string Texture => Commons.ModAsset.White_Mod;

	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	public override void SetDefaults()
	{
		Item.width = 32;
		Item.height = 32;
		Item.damage = 17;
		Item.useTime = Item.useAnimation = 27;
		Item.autoReuse = true;
		Item.useTurn = true;
		Item.maxStack = 1;
		Item.knockBack = 4f;
		Item.rare = ItemRarityID.Blue;
		Item.value = Item.sellPrice(silver: 20);
		Item.DamageType = DamageClass.Ranged;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.UseSound = SoundID.Item5;
		Item.useAmmo = AmmoID.Arrow;
		Item.shoot = ProjectileID.WoodenArrowFriendly;
		Item.shootSpeed = 6.5f;
		Item.noMelee = true;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<YggdrasilAmber>(10);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}
}
