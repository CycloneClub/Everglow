using Everglow.Commons.VFX.Scene;
using SubworldLibrary;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

[Pipeline(typeof(WCSPipeline))]
public class StoneBridge_fence_left_side : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawBG;

	public override void OnSpawn()
	{
		Texture = ModAsset.StoneBridge_fence_left_side.Value;
	}

	public override void Update()
	{
		if (!SubworldSystem.IsActive<YggdrasilWorld>())
		{
			Active = false;
		}
		base.Update();
	}

	public override void Draw()
	{
		base.Draw();
	}
}
