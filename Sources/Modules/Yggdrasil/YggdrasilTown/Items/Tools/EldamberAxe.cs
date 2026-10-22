using Terraria.ModLoader;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Tools;

/// <summary>
/// 古珀斧：砍伐工具。
/// 已确定配方：古珀×10；制作站待定。
/// </summary>
public class EldamberAxe : ModItem
{
	// 临时使用已有白色纹理，待正式素材补齐后替换。
	public override string Texture => Commons.ModAsset.White_Mod;

	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Tools;

	public override void SetDefaults()
	{
		Item.DamageType = DamageClass.Melee;
		Item.useStyle = ItemUseStyleID.Swing;
		// TODO：斧力 Item.axe（显示百分比的 1/5）、使用速度、伤害及物品尺寸。
	}
}
