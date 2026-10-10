namespace Everglow.Yggdrasil.CorruptWormHive.Tiles;

public class BloodLightCrystalEntity : ModTileEntity
{
	public const float DISSOLVE_TIME = 1.5f; // 溶解时长（秒）
	public const float DISSOLVE_STEP = 1f / (DISSOLVE_TIME * 60); // 溶解速率（%/帧）

	private float dissolveProgress = 0; // 溶解进度; 0为未开始，1为完成

	public override void Update()
	{
		if (dissolveProgress > 0 && dissolveProgress <= 1)
		{
			dissolveProgress += DISSOLVE_STEP;

			if (Main.rand.NextBool(50))
			{
				BloodLightCrystal.SummonDust(Position.X, Position.Y);
			}

			// Main.NewText("Updated: [" + Position.X + "," + Position.Y + "]");

			if (dissolveProgress >= 1)
			{
				WorldGen.KillTile(Position.X, Position.Y, false, false, true);
				Kill(Position.X, Position.Y);
			}
		}
	}

	public override bool IsTileValidForEntity(int x, int y)
	{
		// Main.NewText("Validate: [" + x + "," + y+ "]");
		Tile tile = Main.tile[x, y];
		return tile.HasTile && tile.TileType == ModContent.TileType<BloodLightCrystal>();
	}

	public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			NetMessage.SendTileSquare(Main.myPlayer, i, j, 3);
			NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
			return -1;
		}
		// dissolveProgress = 0f;
		// Main.NewText("Placed");
		return Place(i, j);
	}

	public void StartDissolve()
	{
		if (dissolveProgress == 0)
		{
			// Main.NewText("6:[" + Position + "] start kill");
			dissolveProgress += DISSOLVE_STEP;
		}
	}

	public float GetDissolveProgress()
	{
		return dissolveProgress;
	}
}
