using Everglow.Commons.VFX.Scene;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

[Pipeline(typeof(WCSPipeline))]
public class BoneAndPlatform_Foreground : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawPlayers;

	public override void OnSpawn()
	{
		Texture = ModAsset.BoneAndPlatform_Foreground.Value;
	}
}
