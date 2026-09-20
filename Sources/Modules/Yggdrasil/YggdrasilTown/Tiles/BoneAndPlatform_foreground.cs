using Everglow.Commons.VFX.Scene;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

[Pipeline(typeof(WCSPipeline))]
public class BoneAndPlatform_foreground : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawPlayers;

	public override void OnSpawn()
	{
		Texture = ModAsset.BoneAndPlatform_foreground.Value;
	}
}
