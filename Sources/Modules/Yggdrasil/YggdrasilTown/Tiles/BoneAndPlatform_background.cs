using Everglow.Commons.VFX.Scene;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

[Pipeline(typeof(WCSPipeline))]
public class BoneAndPlatform_background : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawBG;

	public override void OnSpawn()
	{
		Texture = ModAsset.BoneAndPlatform_background.Value;
	}
}
