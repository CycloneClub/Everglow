using Everglow.Commons.Mechanics.ElementalDebuff.Debuffs;
using Everglow.Commons.Utilities;

namespace Everglow.Example.ElementalDebuff;

public class PlayerPenetrationTest : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Examples;

	public override string Texture => Commons.ModAsset.Point_Mod;

	public override void SetDefaults()
	{
		Item.width = 32;
		Item.height = 32;

		Item.accessory = true;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.GetElementalPenetration().Generic += 0.1f;
		player.GetElementalPenetration(BurnDebuff.ID) += 0.05f;
	}
}
