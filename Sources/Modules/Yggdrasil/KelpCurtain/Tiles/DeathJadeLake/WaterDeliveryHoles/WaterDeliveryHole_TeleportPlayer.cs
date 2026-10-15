using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake.WaterDeliveryHoles;

public class WaterDeliveryHole_TeleportPlayer : ModPlayer
{
	public bool Active = false;

	public Vector2 OldPos;

	public float Timer;

	public float MaxTeleportTime = 30;

	public override void ModifyScreenPosition()
	{
		if (Active)
		{
			if (Timer > 0)
			{
				Timer--;
				float value = 1f - Timer / MaxTeleportTime;
				Main.screenPosition = value.Lerp(OldPos, Main.screenPosition);
			}
			else
			{
				Timer = 0;
				Active = false;
			}
		}
		base.ModifyScreenPosition();
	}

	public static void GetDestinationAndTeleport(int i, int j)
	{
		Player player = Main.LocalPlayer;
		WaterDeliveryHole_TeleportPlayer modPlayer = player.GetModPlayer<WaterDeliveryHole_TeleportPlayer>();
		if (!modPlayer.Active)
		{
			Vector2 currentPos = new Vector2(i, j);
			int currentDir = -1;
			var currentTile = TileUtils.SafeGetTile(i, j);
			int currentStyle = TileObjectData.GetTileStyle(currentTile);
			TileObjectData currentObjectData = TileObjectData.GetTileData(currentTile.TileType, currentStyle);
			if (!TileAtOriginPos(currentTile, currentStyle, currentObjectData))
			{
				return;
			}
			Vector2 currentWorldPos = currentPos * 16 + new Vector2(8);
			if ((player.Center - currentWorldPos).Length() >= 32 || (player.Center - currentWorldPos).Length() < (player.Center + player.velocity - currentWorldPos).Length())
			{
				return;
			}

			// x, y, direction
			List<(int, int, int)> destinations = new List<(int, int, int)>();
			for (int x = -40; x <= 40; x += 1)
			{
				for (int y = -40; y <= 40; y += 1)
				{
					var tile = TileUtils.SafeGetTile(i + x, j + y);
					int style = TileObjectData.GetTileStyle(tile);
					if (style >= 0)
					{
						int targetType = tile.TileType;
						TileObjectData tileObjectData = TileObjectData.GetTileData(targetType, style);
						int dir = GetDirection(tile);
						if (x == 0 && y == 0)
						{
							currentDir = dir;
							continue;
						}
						if (TileAtOriginPos(tile, style, tileObjectData) && dir >= 0)
						{
							if (!destinations.Contains((tile.X(), tile.Y(), dir)))
							{
								destinations.Add((tile.X(), tile.Y(), dir));
							}
						}
					}
				}
			}
			var chosenDest = (-1, -1, -1);
			if (destinations.Count > 0)
			{
				Vector2 closestDest = new Vector2(i + 100, j + 100);
				float minDis = closestDest.Length() * 2;
				int bestDir = -1;
				float maxDirDis = 0;
				foreach (var dest in destinations)
				{
					Vector2 directionDis = new Vector2(1, 0).RotatedBy(dest.Item3 * MathHelper.PiOver4) - new Vector2(1, 0).RotatedBy(currentDir * MathHelper.PiOver4);
					if (directionDis.Length() > maxDirDis - 0.1f)
					{
						maxDirDis = directionDis.Length();
						bestDir = dest.Item3;
					}
				}
				foreach (var dest in destinations)
				{
					if (dest.Item3 != bestDir)
					{
						continue;
					}
					Vector2 destination = new Vector2(dest.Item1, dest.Item2);
					Vector2 toDes = destination - currentPos;
					float dis = toDes.Length();
					float cosValue = Vector2.Dot(toDes.SafeNormalize(Vector2.Zero), new Vector2(1, 0).RotatedBy(bestDir * MathHelper.PiOver4));
					if (cosValue < 0.8)
					{
						dis += 50;
					}
					if (dis < minDis)
					{
						minDis = dis;
						closestDest = destination;
						chosenDest = dest;
					}
				}
				if ((currentPos - closestDest).Length() < 40)
				{
					int destX = chosenDest.Item1;
					int destY = chosenDest.Item2;
					var destTile = TileUtils.SafeGetTile(destX, destY);
					var vfx = new WaterDeliveryHole_VFX_Release
					{
						Active = true,
						Visible = true,
						Position = new Point(destX, destY).ToWorldCoordinates(),
						OriginTilePos = new Point(destX, destY),
						OriginTileType = destTile.TileType,
						Direction = 1,
						Rotation = chosenDest.Item3 * MathHelper.PiOver4,
						MaxTime = 60,
					};
					Ins.VFXManager.Add(vfx);
					Teleport(player, closestDest * 16 + new Vector2(8), bestDir * MathHelper.PiOver4);
				}
			}
		}
	}

	public static int GetDirection(Tile tile)
	{
		int targetType = tile.TileType;
		int dir = -1;
		int style = TileObjectData.GetTileStyle(tile);
		if (targetType == ModContent.TileType<WaterDeliveryHole>())
		{
			if (style == 0)
			{
				dir = 6;
			}
			else
			{
				dir = 2;
			}
		}
		if (targetType == ModContent.TileType<WaterDeliveryHole_V>())
		{
			if (style == 0)
			{
				dir = 4;
			}
			else
			{
				dir = 0;
			}
		}
		if (targetType == ModContent.TileType<WaterDeliveryHole_BottomRight>())
		{
			dir = 1;
		}
		if (targetType == ModContent.TileType<WaterDeliveryHole_BottomLeft>())
		{
			dir = 3;
		}
		if (targetType == ModContent.TileType<WaterDeliveryHole_TopLeft>())
		{
			dir = 5;
		}
		if (targetType == ModContent.TileType<WaterDeliveryHole_TopRight>())
		{
			dir = 7;
		}
		return dir;
	}

	public static bool TileAtOriginPos(Tile tile, int style, TileObjectData tileObjectData)
	{
		return tile.TileFrameX - style * tileObjectData.Width * 18 == tileObjectData.Origin.X * 18 && tile.TileFrameY == tileObjectData.Origin.Y * 18;
	}

	public static void Teleport(Player player, Vector2 destination, float rotation)
	{
		WaterDeliveryHole_TeleportPlayer modPlayer = player.GetModPlayer<WaterDeliveryHole_TeleportPlayer>();
		modPlayer.Active = true;
		modPlayer.Timer = modPlayer.MaxTeleportTime;
		modPlayer.OldPos = Main.screenPosition;
		player.MountedCenter = destination;
		player.MountedCenter += new Vector2(20, 0).RotatedBy(rotation) + player.velocity;

		// player.velocity += new Vector2(-4, 0).RotatedBy(rotation);
	}
}
