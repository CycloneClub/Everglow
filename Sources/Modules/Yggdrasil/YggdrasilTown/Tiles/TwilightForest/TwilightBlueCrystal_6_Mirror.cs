using Everglow.Commons.VFX.Scene;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest;

[Pipeline(typeof(ScreenReflectionPipeline))]
public class TwilightBlueCrystal_6_Mirror : Tile_MirrorFaceVFX
{
	public override void OnSpawn()
	{
		Texture = ModAsset.TwilightBlueCrystal_6_Mirror.Value;
	}
}
