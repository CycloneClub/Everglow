namespace Everglow.Myth.LanternMoon.NPCs.FlamingDashCore;

public class DashCoreEffect : ModSystem
{
	public override void OnWorldLoad()
	{
		base.OnWorldLoad();
	}

	private float rDas = 0;

	public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
	{
		Color colorShine = FlamingDashCore.ColorShine;
		if (NPC.CountNPCS(ModContent.NPCType<FlamingDashCore>()) > 0 && FlamingDashCore.Shine > 0)
		{
			rDas = 1f;
		}
		else
		{
			if (rDas > 0)
			{
				rDas -= 0.01f;
			}
			else
			{
				rDas = 0;
			}
		}
		tileColor = new Color(2.2f * rDas * colorShine.R / 200f + tileColor.R / 255f * (1 - rDas), 2.2f * rDas * colorShine.G / 200f + tileColor.G / 255f * (1 - rDas), 2.2f * rDas * colorShine.B / 200f + tileColor.B / 255f * (1 - rDas));
		backgroundColor = new Color(2.2f * rDas * colorShine.R / 200f + tileColor.R / 255f * (1 - rDas), 2.2f * rDas * colorShine.G / 200f + tileColor.G / 255f * (1 - rDas), 2.2f * rDas * colorShine.B / 200f + tileColor.B / 255f * (1 - rDas));
		Main.ColorOfTheSkies = new Color(2.2f * rDas * colorShine.R / 200f + tileColor.R / 255f * (1 - rDas), 2.2f * rDas * colorShine.G / 200f + tileColor.G / 255f * (1 - rDas), 2.2f * rDas * colorShine.B / 200f + tileColor.B / 255f * (1 - rDas));
		base.ModifySunLightColor(ref tileColor, ref backgroundColor);
	}
}
