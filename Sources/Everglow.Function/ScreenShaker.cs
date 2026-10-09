namespace Everglow.Commons;

public class ScreenShaker : ModPlayer
{
	public Vector2 FlyCamPosition = Vector2.Zero;
	public Vector2 FlyCamPosition2;
	public float DirFlyCamPosStrength = 1f; // ShakeStrength
	public int DirFlyCamPos = 0; // Shake
	public int MinaFlyCamPos = 0; // MinaShake
	public override void ModifyScreenPosition()
	{
		FlyCamPosition *= 0.25f;
		Main.screenPosition += FlyCamPosition;

		if (DirFlyCamPos > 0)
		{
			DirFlyCamPos -= 1;
			FlyCamPosition2 = new Vector2(Main.rand.NextFloat(-16 * DirFlyCamPosStrength, 16 * DirFlyCamPosStrength), Main.rand.Next(-16, 16));
			if (DirFlyCamPos == 1)
				FlyCamPosition2 = Vector2.Zero;
		}
		else
		{
			DirFlyCamPos = 0;
			DirFlyCamPosStrength = 1;
		}
		if (MinaFlyCamPos > 0)
		{
			MinaFlyCamPos -= 1;
			FlyCamPosition2 = new Vector2(Main.rand.NextFloat(-4 * DirFlyCamPosStrength, 4 * DirFlyCamPosStrength), Main.rand.NextFloat(-4 * DirFlyCamPosStrength, 4 * DirFlyCamPosStrength));
			if (MinaFlyCamPos == 1)
				FlyCamPosition2 = Vector2.Zero;
		}
		else
		{
			MinaFlyCamPos = 0;
		}
	}
}
