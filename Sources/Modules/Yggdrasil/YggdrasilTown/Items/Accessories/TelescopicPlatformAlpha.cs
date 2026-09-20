using Terraria.Enums;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;

public class TelescopicPlatformAlpha : TelescopicPlatform
{
	public override void SetDefaults()
	{
		Item.DefaultToAccessory(80, 98);
		PillarCount = 9;
		MaxHeight = 240;
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
