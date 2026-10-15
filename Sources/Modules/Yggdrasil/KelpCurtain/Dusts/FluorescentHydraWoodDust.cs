namespace Everglow.Yggdrasil.KelpCurtain.Dusts;

public class FluorescentHydraWoodDust : ModDust
{
	public override bool Update(Dust dust)
	{
		dust.alpha = 150;
		return base.Update(dust);
	}
}
