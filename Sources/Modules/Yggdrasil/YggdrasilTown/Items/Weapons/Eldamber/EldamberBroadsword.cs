using Terraria.ModLoader;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.Eldamber;

/// <summary>
/// 古珀阔剑：挥砍型近战武器。
/// 已确定配方：古珀×12；制作站待定。
/// </summary>
public class EldamberBroadsword : ModItem
{
	// 临时使用已有白色纹理，待正式素材补齐后替换。
	public override string Texture => Commons.ModAsset.White_Mod;

	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

	public override void SetDefaults()
	{
		Item.DamageType = DamageClass.Melee;
		Item.useStyle = ItemUseStyleID.Swing;
		// TODO：伤害、攻速、击退及物品尺寸。
	}
}
