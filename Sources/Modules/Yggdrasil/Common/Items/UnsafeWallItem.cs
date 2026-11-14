using Everglow.Commons.Utilities;

namespace Everglow.Yggdrasil.Common.Items;

/// <summary>危险墙物品共用放置设置与原生骷髅标记；安全物品和获取配方保持独立。</summary>
public abstract class UnsafeWallItem<TWall> : ModItem
	where TWall : ModWall
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Placeables;

	public override void SetStaticDefaults()
	{
		ItemID.Sets.DrawUnsafeIndicator[Type] = true;
	}

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableWall(ModContent.WallType<TWall>());
		Item.width = 24;
		Item.height = 24;
	}
}
