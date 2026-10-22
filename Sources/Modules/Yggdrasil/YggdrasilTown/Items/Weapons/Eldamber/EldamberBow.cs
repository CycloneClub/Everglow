using Terraria.ModLoader;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.Eldamber;

/// <summary>
/// 古珀弓：远程武器，使用箭类弹药。
/// 已确定配方：古珀×10；制作站待定。
/// </summary>
public class EldamberBow : ModItem
{
	// 临时使用已有白色纹理，待正式素材补齐后替换。
	public override string Texture => Commons.ModAsset.White_Mod;

	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	public override void SetDefaults()
	{
		Item.DamageType = DamageClass.Ranged;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.useAmmo = AmmoID.Arrow;
		Item.shoot = ProjectileID.WoodenArrowFriendly;
		Item.noMelee = true;
		// TODO：伤害、攻速、射速、击退及物品尺寸。
	}
}
