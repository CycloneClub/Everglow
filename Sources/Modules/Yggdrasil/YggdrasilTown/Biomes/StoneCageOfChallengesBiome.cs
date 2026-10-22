using Everglow.Commons.Utilities.BackgroundHelper;
using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.YggdrasilTown.Background;

namespace Everglow.Yggdrasil.YggdrasilTown.Biomes;

public class StoneCageOfChallenges : ModBiome
{
	public override int Music => YggdrasilContent.QuickMusic(ModAsset.NewYggdrasilTownBGM_Path);

	public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

	public override string BestiaryIcon => ModAsset.YggdrasilTownIcon_Mod;

	public override string BackgroundPath => ModAsset.YggdrasilTown_MapBackground_Mod;

	public override string MapBackground => ModAsset.YggdrasilTown_MapBackground_Mod;

	public override ModWaterStyle WaterStyle => ModContent.GetInstance<Water.YggdrasilTownWaterStyle>();

	public override Color? BackgroundColor => base.BackgroundColor;

	public override bool IsBiomeActive(Player player)
	{
		Vector2 center = StoneCageOfChallengesBackgroundBase.GetCenter();
		Vector2 screenCenter = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
		return center != Vector2.Zero && Math.Abs(center.X - screenCenter.X) < 240 * 16 && Math.Abs(center.Y - screenCenter.Y) < 60 * 16;
	}

	public override void OnInBiome(Player player)
	{
		BackgroundSystem bgSystem = ModContent.GetInstance<BackgroundSystem>();
		if (!bgSystem.HasBgSlide("Everglow.Yggdrasil.YggdrasilTown.Background.StoneCageOfChallengesSky"))
		{
			StoneCageOfChallengesSky sky = new StoneCageOfChallengesSky { WorldOffset = new Vector2(0, -2400) };
			bgSystem.AddBackgroundSlide(sky);

			StoneCageOfChallengesFar far = new StoneCageOfChallengesFar { WorldOffset = new Vector2(0, -9600) };
			bgSystem.AddBackgroundSlide(far);

			StoneCageOfChallengesMiddle middle = new StoneCageOfChallengesMiddle { WorldOffset = new Vector2(0, -5400) };
			bgSystem.AddBackgroundSlide(middle);

			StoneCageOfChallengesClose close = new StoneCageOfChallengesClose { WorldOffset = new Vector2(0, -2400) };
			bgSystem.AddBackgroundSlide(close);

			StoneCageOfChallengesClose2 close2 = new StoneCageOfChallengesClose2 { WorldOffset = new Vector2(0, -500) };
			bgSystem.AddBackgroundSlide(close2);
		}

		base.OnInBiome(player);
	}
}
