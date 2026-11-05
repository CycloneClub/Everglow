namespace Everglow.Commons.Mechanics.PlayerBonuses;

/// <summary>统一管理玩家加成状态与生命周期；具体计算按 tML 钩子拆分到 partial 文件。</summary>
public partial class StatBonusPlayer : ModPlayer
{
	public override void ResetEffects()
	{
		// ==================== PostUpdateEquips ====================
		OrangeCrown = false;
		FragmentsOfWreckage = false;
		WalnutClip = false;
		FireflyBorageBadgeAgainstMoth = false;
		YellowBellFlower = false;
		maxFallSpeedCap = float.PositiveInfinity;
		maxFallSpeedForSpecialBehavior = null;

		// ==================== PostUpdateMiscEffects ====================
		BloodTearCrystalCrown = false;
		WhiteCrown = false;
		CelesteStoneWaistPendant = false;
		DevilHeartSet = false;
		ThermalConductor = false;

		// ==================== PostHurt ====================
		OrangeIcecream = false;
		OrangeJuice = false;

		// ==================== UpdateBadLifeRegen ====================
		CactusBall = false;
		GrubSoup = false;
		MonsterLasagna = false;
		Sashimi = false;
		ShuckedOyster = false;
		Mango = false;
		HungerSatietyLevel = 0;
		PlagueLifeRegenLoss = 0;
		CharredLifeRegenLoss = 0;
		RedAlgaeLifeRegenLoss = 0;
		AuburnSelfReinforcing = false;
		ShadowPotionInBrightLight = false;
		FireflyInferno = false;
		AcytaeaInferno = false;
	}
}
