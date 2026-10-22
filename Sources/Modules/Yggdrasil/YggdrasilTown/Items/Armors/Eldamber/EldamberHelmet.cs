using Terraria.ModLoader;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Eldamber;

/// <summary>
/// 古珀头盔：头部护甲。
/// 已确定配方：古珀×12＋球冻王晶体×3；制作站待定。
/// </summary>
[AutoloadEquip(EquipType.Head)]
public class EldamberHelmet : ModItem
{
	// 临时复用灯木护甲的物品及穿戴贴图，待古珀正式素材补齐后替换。
	public override string Texture => ModAsset.LampWoodHelmet_Mod;

	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	public override void SetDefaults()
	{
		Item.maxStack = 1;
		// TODO：防御、装备效果及物品尺寸。
	}
}
