namespace Everglow.Commons.Mechanics.PlayerBonuses;

public partial class StatBonusPlayer
{
	public bool CactusBall;
	public bool GrubSoup;
	public bool MonsterLasagna;
	public bool Sashimi;
	public bool ShuckedOyster;
	public bool Mango;
	public int HungerSatietyLevel;
	public int PlagueLifeRegenLoss;
	public int CharredLifeRegenLoss;
	public int RedAlgaeLifeRegenLoss;
	public bool AuburnSelfReinforcing;
	public bool ShadowPotionInBrightLight;
	public bool FireflyInferno;
	public bool AcytaeaInferno;

	public override void UpdateBadLifeRegen()
	{
		// ====================================================================
		// Clear positive regen and its timer before applying losses.
		// ====================================================================
		bool foodPoison = GrubSoup || MonsterLasagna || Sashimi || ShuckedOyster;
		if (Player.lifeRegen > 0 && (CactusBall || foodPoison || HungerSatietyLevel < 0 || PlagueLifeRegenLoss > 0))
		{
			Player.lifeRegen = 0;
		}
		if (foodPoison || PlagueLifeRegenLoss > 0)
		{
			Player.lifeRegenTime = 0;
		}

		// ====================================================================
		// Life regen losses: accumulate all active contributions.
		// ====================================================================
		if (CactusBall)
		{
			Player.lifeRegen -= 5;
		}
		if (GrubSoup)
		{
			Player.lifeRegen -= Mango ? 2 : 4;
		}
		if (MonsterLasagna)
		{
			Player.lifeRegen -= Mango ? 3 : 6;
		}
		if (Sashimi)
		{
			Player.lifeRegen -= Mango ? 2 : 4;
		}
		if (ShuckedOyster)
		{
			Player.lifeRegen -= Mango ? 2 : 4;
		}
		if (HungerSatietyLevel == -2)
		{
			Player.lifeRegen -= (int)(Player.statLifeMax2 * 0.01f);
		}
		// Severe hunger uses native Player.starving damage after this hook.
		Player.lifeRegen -= PlagueLifeRegenLoss;
		Player.lifeRegen -= CharredLifeRegenLoss;
		Player.lifeRegen -= RedAlgaeLifeRegenLoss;
		if (AuburnSelfReinforcing)
		{
			Player.lifeRegen -= 2;
		}
		if (ShadowPotionInBrightLight)
		{
			Player.lifeRegen -= (int)(Player.statLifeMax2 * 0.01f);
		}

		// ====================================================================
		// Final restriction: apply Inferno after accumulating losses.
		// ====================================================================
		if (FireflyInferno || AcytaeaInferno)
		{
			Player.lifeRegen = Math.Min(Player.lifeRegen, -(int)(5 + Player.velocity.Length() * 8));
		}
	}
}
