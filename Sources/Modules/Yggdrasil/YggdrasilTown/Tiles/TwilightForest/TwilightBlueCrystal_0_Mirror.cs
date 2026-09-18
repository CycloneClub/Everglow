using Everglow.Commons.VFX.Scene;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest;

[Pipeline(typeof(ScreenReflectionPipeline))]
public class TwilightBlueCrystal_0_Mirror : Tile_MirrorFaceVFX
{
	public override void OnSpawn()
	{
		Texture = ModAsset.TwilightBlueCrystal_0_Mirror.Value;
	}
}
