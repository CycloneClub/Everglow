using Terraria.Enums;
using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;

public class TelescopicPlatformAlpha : TelescopicPlatform
{
	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxHeight / 16f, MoveSpeed * 60f / 16f);

	public override void SetDefaults()
	{
		Item.DefaultToAccessory(80, 98);
		PillarCount = 9;
		MaxHeight = 192;
		MoveSpeed = 3;
		Texture2 = ModAsset.TelescopicPlatformAlpha_Platform.Value;
		BodyDrawOffsetY = 8;
		BodyRect = new Rectangle(0, 0, 80, 38);
		PillarFrontRect = new Rectangle(0, 56, 40, 42);
		PillarBackRect = new Rectangle(40, 56, 40, 42);
		Item.SetShopValues(ItemRarityColor.Orange3, Item.buyPrice(copper: 30000));
		base.SetDefaults();
	}
}
