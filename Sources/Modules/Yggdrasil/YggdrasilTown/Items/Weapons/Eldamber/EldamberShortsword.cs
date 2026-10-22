using Terraria.ModLoader;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.Eldamber;

/// <summary>
/// 古珀短剑：刺击型近战武器。
/// 已确定配方：古珀×8；制作站待定。
/// </summary>
public class EldamberShortsword : ModItem
{
	// 临时使用已有白色纹理，待正式素材补齐后替换。
	public override string Texture => Commons.ModAsset.White_Mod;

	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

	public override void SetDefaults()
	{
		Item.DamageType = DamageClass.Melee;
		Item.useStyle = ItemUseStyleID.Rapier;
		// TODO：刺击投射物及配套使用设置，伤害、攻速、击退和物品尺寸。
	}
}
