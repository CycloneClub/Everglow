using Terraria.Enums;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;

public class TelescopicPlatformBeta : TelescopicPlatform
{
	public override void SetDefaults()
	{
		Item.DefaultToAccessory(80, 98);
		PillarCount = 21;
		MaxHeight = 720;
		MoveSpeed = 6;
		Texture2 = ModAsset.TelescopicPlatformBeta_Platform.Value;
		BodyDrawOffsetY = 8;
		BodyRect = new Rectangle(0, 0, 80, 40);
		PillarFrontRect = new Rectangle(0, 56, 40, 42);
		PillarBackRect = new Rectangle(40, 56, 40, 42);
		Texture_Glow = ModAsset.TelescopicPlatformBeta_Platform_Glow.Value;
		Item.SetShopValues(ItemRarityColor.LightRed4, Item.buyPrice(copper: 280000));
		base.SetDefaults();
	}

	public override void AddLight()
	{
		Lighting.AddLight(Main.LocalPlayer.Center + new Vector2(-30, 0), new Vector3(1f, 0.8f, 0.7f));
		Lighting.AddLight(Main.LocalPlayer.Center + new Vector2(30, 0), new Vector3(1f, 0.8f, 0.7f));
	}
}
