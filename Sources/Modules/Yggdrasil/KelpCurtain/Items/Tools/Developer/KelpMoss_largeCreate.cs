using Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;
using Everglow.Yggdrasil.KelpCurtain.Tiles.IsleOfBloom;
using Everglow.Yggdrasil.KelpCurtain.VFXs.VampireMat;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Tools.Developer;

public class KelpMoss_largeCreate : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<IslePeachTree_side>());
		Item.width = 16;
		Item.height = 16;
	}

	public override bool CanUseItem(Player player)
	{
		//if(player.itemTime == 0)
		//{
		//	var screenEffectVFX = new ScreenScaringEffect()
		//	{
		//		Active = true,
		//		Visible = true,
		//		Timer = 0,
		//		MaxTime = 120,
		//	};
		//	Ins.VFXManager.Add(screenEffectVFX);
		//}
		//player.itemTime = 30;
		Projectile.NewProjectileDirect(player.GetSource_FromAI(), Main.MouseWorld, Vector2.zeroVector, ModContent.ProjectileType<VampireMat_Attack_Proj_Absorb>(), 100, 1, player.whoAmI);
		return false;
	}
}
