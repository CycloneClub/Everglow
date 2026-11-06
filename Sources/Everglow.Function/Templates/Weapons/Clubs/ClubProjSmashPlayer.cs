using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.Commons.Utilities;

namespace Everglow.Commons.Templates.Weapons.Clubs;

public class ClubProjSmashPlayer : ModPlayer
{
	public override void UpdateEquips()
	{
		if (ClubProjSmash.OwnSmashClubPlayers.Contains(Player.whoAmI))
		{
			// 10000 px/tick maximum fall speed during the club smash action.
			Player.AddStat(StatBonus.MaxFallSpeedForSpecialBehavior, 10000f);
		}
	}
}
