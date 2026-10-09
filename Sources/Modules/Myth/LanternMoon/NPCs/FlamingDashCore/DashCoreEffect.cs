namespace Everglow.Myth.LanternMoon.NPCs.FlamingDashCore;

public class DashCoreEffect : ModSystem
{
	public override void OnWorldLoad()
	{
		base.OnWorldLoad();
	}
	private float RDas = 0;
	public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
	{
		Color colorShine = FlamingDashCore.ColorShine;
		if (NPC.CountNPCS(ModContent.NPCType<FlamingDashCore>()) > 0 && FlamingDashCore.Shine > 0)
			RDas = 1f;
		else
		{
			if (RDas > 0)
				RDas -= 0.01f;
			else
			{
				RDas = 0;
			}
		}
		tileColor = new Color(2.2f * RDas * colorShine.R / 200f + tileColor.R / 255f * (1 - RDas), 2.2f * RDas * colorShine.G / 200f + tileColor.G / 255f * (1 - RDas), 2.2f * RDas * colorShine.B / 200f + tileColor.B / 255f * (1 - RDas));
		backgroundColor = new Color(2.2f * RDas * colorShine.R / 200f + tileColor.R / 255f * (1 - RDas), 2.2f * RDas * colorShine.G / 200f + tileColor.G / 255f * (1 - RDas), 2.2f * RDas * colorShine.B / 200f + tileColor.B / 255f * (1 - RDas));
		Main.ColorOfTheSkies = new Color(2.2f * RDas * colorShine.R / 200f + tileColor.R / 255f * (1 - RDas), 2.2f * RDas * colorShine.G / 200f + tileColor.G / 255f * (1 - RDas), 2.2f * RDas * colorShine.B / 200f + tileColor.B / 255f * (1 - RDas));
		base.ModifySunLightColor(ref tileColor, ref backgroundColor);
	}
}
