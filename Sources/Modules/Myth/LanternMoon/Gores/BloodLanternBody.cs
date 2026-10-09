namespace Everglow.Myth.LanternMoon.Gores;

public class BloodLanternBody : ModGore
{
	public override void SetStaticDefaults()
	{
		GoreID.Sets.DisappearSpeed[ModContent.GoreType<BloodLanternBody>()] = 3;
	}

	public override Color? GetAlpha(Gore gore, Color lightColor)
	{
		return base.GetAlpha(gore, new Color(0, 0, 0, 0));
	}

	public override bool Update(Gore gore)
	{
		return base.Update(gore);
	}
}
