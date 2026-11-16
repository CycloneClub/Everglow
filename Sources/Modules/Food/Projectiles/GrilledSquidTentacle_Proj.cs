using Everglow.Commons.Templates.Weapons.Whips;

namespace Everglow.Food.Projectiles;

public class GrilledSquidTentacle_Proj : WhipProjectile
{
	public override void SetDef()
	{
		DustType = ModContent.DustType<Dusts.GrillJuice>();
		WhipLength = 180;
	}
}
