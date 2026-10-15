using Everglow.Commons.VFX.Scene;
using SubworldLibrary;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

[Pipeline(typeof(WCSPipeline))]
public class StoneBridge_Rightside_foreground : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawPlayers;
	public override void Update()
	{
		if (!SubworldSystem.IsActive<YggdrasilWorld>())
		{
			Active = false;
		}
		base.Update();
	}

	public override void OnSpawn()
	{
		Texture = ModAsset.StoneBridge_Rightside.Value;
	}
}
