using Everglow.Commons.VFX.Scene;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.RoomScenes;

public class GenerateSplash : ModDust
{
	public override string Texture => Commons.ModAsset.Empty_Mod;

	public override void OnSpawn(Dust dust)
	{
	}

	public override bool Update(Dust dust)
	{
		if (dust.customData is Visual)
		{
			Ins.VFXManager.Add(dust.customData as Visual);
		}
		dust.active = false;
		return base.Update(dust);
	}
}
