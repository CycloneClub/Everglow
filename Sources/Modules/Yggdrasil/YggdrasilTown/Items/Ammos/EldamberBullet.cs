using Terraria.ModLoader;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Ammos;

/// <summary>
/// 古珀子弹：枪弹类远程弹药。
/// 已确定配方：古珀×1 → 古珀子弹×100；制作站待定。
/// </summary>
public class EldamberBullet : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Ammo;

	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetDefaults()
	{
		Item.width = 10;
		Item.height = 10;
		Item.ammo = AmmoID.Bullet;
		Item.damage = 8;
		Item.DamageType = DamageClass.Ranged;
		Item.consumable = true;
		Item.maxStack = Item.CommonMaxStack;
		// TODO：子弹投射物与射速。

		Item.rare = ItemRarityID.Green;
		Item.value = Item.buyPrice(0, 0, 0, 0);
	}
}
