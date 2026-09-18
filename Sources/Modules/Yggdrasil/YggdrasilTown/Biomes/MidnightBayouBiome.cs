using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.YggdrasilTown.Background;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using SubworldLibrary;

namespace Everglow.Yggdrasil.YggdrasilTown.Biomes;

public class MidnightBayouBiome : ModBiome
{
	public override int Music => YggdrasilContent.QuickMusic(ModAsset.NewYggdrasilTownBGM_Path);

	public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

	public override string BestiaryIcon => ModAsset.YggdrasilTownIcon_Mod;

	public override string BackgroundPath => ModAsset.MidnightBayou_MapBackground_Mod;

	public override string MapBackground => ModAsset.MidnightBayou_MapBackground_Mod;

	public override ModWaterStyle WaterStyle => ModContent.GetInstance<Water.YggdrasilTownWaterStyle>();

	public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => base.UndergroundBackgroundStyle;

	public override Color? BackgroundColor => base.BackgroundColor;

	public override void Load()
	{
		base.Load();
	}

	public override bool IsBiomeActive(Player player)
	{
		if (!SubworldSystem.IsActive<YggdrasilWorld>())
		{
			return false;
		}
		return (new Point(1395, Main.maxTilesY - 405).ToWorldCoordinates() - (Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f)).Length() < 5000;
	}

	public override void OnInBiome(Player player)
	{
		if (Main.maxRaining > 0)
		{
			Main.maxRaining = 0;
			Main.StopRain();
			Main.raining = false;
		}
		if (Main.slimeRain)
		{
			Main.StopSlimeRain();
		}
		Main.bloodMoon = false;
		if (Main.rand.NextBool(15))
		{
			Vector2 anchorPos = YggdrasilTownBackground.OriginPylonCenter;
			Texture2D tex = ModAsset.MidnightBayouBackgroundCloud_0.Value;
			switch (Main.rand.Next(4))
			{
				case 0:
					tex = ModAsset.MidnightBayouBackgroundCloud_0.Value;
					break;
				case 1:
					tex = ModAsset.MidnightBayouBackgroundCloud_1.Value;
					break;
				case 2:
					tex = ModAsset.MidnightBayouBackgroundCloud_2.Value;
					break;
				case 3:
					tex = ModAsset.MidnightBayouBackgroundCloud_3.Value;
					break;
			}

			var cloud = new MidnightBayouBackgroundCloud
			{
				Position = anchorPos + new Vector2(Main.rand.Next(-1000, 3000), Main.rand.NextFloat(850, 960)),
				Velocity = new Vector2(Main.windSpeedCurrent * 0.2f, 0),
				Fade = 0,
				AnchorPos = anchorPos,
				Scale = Main.rand.NextFloat(0.85f, 1.15f),
				MaxTime = Main.rand.NextFloat(300, 1500),
				CloudTexture = tex,
			};
			Ins.VFXManager.Add(cloud);
		}
		base.OnInBiome(player);
	}
}
