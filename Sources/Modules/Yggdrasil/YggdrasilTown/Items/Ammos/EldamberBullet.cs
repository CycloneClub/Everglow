using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Terraria.ModLoader;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Ammos;

/// <summary>
/// 古珀子弹：枪弹类远程弹药。
/// 已确定配方：古珀×1 → 古珀子弹×100；在铁砧处制作。
/// </summary>
public class EldamberBullet : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Ammo;

	public override void SetDefaults()
	{
		Item.width = 10;
		Item.height = 10;
		Item.ammo = AmmoID.Bullet;
		Item.damage = 10;
		Item.DamageType = DamageClass.Ranged;
		Item.consumable = true;
		Item.maxStack = Item.CommonMaxStack;
		Item.shoot = ProjectileID.Bullet;
		Item.shootSpeed = 4f;
		Item.knockBack = 2f;
		Item.rare = ItemRarityID.Green;
		Item.value = Item.sellPrice(copper: 1);
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe(100);
		recipe.AddIngredient<YggdrasilAmber>(1);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}
}
