using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Gores;

public class CyatheaLeaf_small : ModGore
{
	public override string Texture => ModAsset.CyatheaLeaf_small_Mod;

	public override bool Update(Gore gore)
	{
		UpdateFrame(gore);
		UpdateMove(gore);
		return true;
	}
	private void UpdateFrame(Gore gore)
	{
		if (Math.Abs(gore.velocity.Y) < 0.1f)
		{
			gore.timeLeft -= 20;
			return;
		}
		gore.frameCounter++;
		if (gore.frameCounter > 4)
		{
			if (gore.frame < gore.numFrames - 1)
				gore.frame++;
			else
			{
				gore.frame = 0;
			}
			gore.frameCounter = 0;
		}
	}
	private void UpdateMove(Gore gore)
	{
		gore.velocity.Y -= 0.21f;
		gore.velocity.Y += 0.06f * gore.scale;
		gore.velocity.X += Main.windSpeedCurrent * 0.2f * Main.rand.NextFloat(0.85f, 1.15f) / gore.scale + MathF.Sin(gore.timeLeft * 0.02f * gore.scale) * 0.1f * gore.scale;
		gore.velocity *= 0.97f;
	}
	public override void OnSpawn(Gore gore, IEntitySource source)
	{
		gore.timeLeft = Main.rand.Next(350, 450);
		gore.drawOffset = new Vector2(5, 5);
		gore.frameCounter = (byte)Main.rand.Next(4);
		gore.numFrames = 9;
		gore.frame = (byte)Main.rand.Next(gore.frameCounter);
		base.OnSpawn(gore, source);
	}
}
