using Everglow.Commons.Utilities.BackgroundHelper;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;
using SubworldLibrary;

namespace Everglow.Yggdrasil.YggdrasilTown.Background;

public abstract class StoneCageOfChallengesBackgroundBase : BackgroundSlideBase
{
	public Vector2 WorldOffset;

	protected Vector2 Center => GetCenter();

	public static Vector2 GetCenter()
	{
		if (SubworldSystem.Current is not YggdrasilWorld yWorld)
		{
			return Vector2.Zero;
		}

		if (yWorld.StoneCageOfChallengesCenter == Vector2.zeroVector)
		{
			for (int x = 50; x < Main.maxTilesX - 50; x++)
			{
				for (int y = 50; y < Main.maxTilesY - 50; y++)
				{
					Tile tile = TileUtils.SafeGetTile(x, y);
					if (tile.TileType == ModContent.TileType<SquamousShellSeal>() && tile.TileFrameX == 180 && tile.TileFrameY == 162)
					{
						yWorld.StoneCageOfChallengesCenter = new Vector2(x, y - 40) * 16;
						break;
					}
				}
				if (yWorld.StoneCageOfChallengesCenter != Vector2.zeroVector)
				{
					break;
				}
			}

			if (yWorld.StoneCageOfChallengesCenter == Vector2.zeroVector)
			{
				yWorld.StoneCageOfChallengesCenter = new Vector2(Main.maxTilesX / 2, Main.maxTilesY - 1900) * 16;
			}
		}

		return yWorld.StoneCageOfChallengesCenter;
	}

	public override void SetDefaults()
	{
		base.SetDefaults();
		UseColorStyle = 2;
		Shader = Effects.XWrap_YClamp_Shader;
	}

	public override void Update()
	{
		WorldAnchor = Center + WorldOffset;
		base.Update();
	}

	public override bool CanActive()
	{
		Vector2 center = Center;
		Vector2 screenCenter = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
		return center != Vector2.Zero && Math.Abs(center.X - screenCenter.X) < 240 * 16 && Math.Abs(center.Y - screenCenter.Y) < 60 * 16;
	}
}
