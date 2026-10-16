using Everglow.Yggdrasil.Common.NPCSpawn;

namespace Everglow.Yggdrasil.KelpCurtain.NPCSpawn;

/// <summary>仅供水黾重定位使用；不调度或创建群体。</summary>
internal static class KelpCurtainNaturalSpawn
{
	public static bool CanPlace(Vector2 feet, int width, int height)
	{
		if (!YggdrasilSpawnRegistration.InKelpCurtain(feet))
		{
			return false;
		}
		var body = new Rectangle((int)feet.X - width / 2, (int)feet.Y - height, width, height);
		int left = (int)Math.Floor(body.Left / 16f);
		int right = (int)Math.Floor((body.Right - 1) / 16f);
		int top = (int)Math.Floor(body.Top / 16f);
		int bottom = (int)Math.Floor((body.Bottom - 1) / 16f);
		if (!WorldGen.InWorld(left, top, 2) || !WorldGen.InWorld(right, bottom, 2))
		{
			return false;
		}
		for (int x = left; x <= right; x++)
		{
			for (int y = top; y <= bottom; y++)
			{
				Tile tile = Main.tile[x, y];
				if (Main.wallHouse[tile.WallType] ||
					(tile.HasTile && !tile.IsActuated && Main.tileSolid[tile.TileType]) ||
					(tile.LiquidAmount > 0 && tile.LiquidType != LiquidID.Water))
				{
					return false;
				}
			}
		}
		// tML 验证的是移动前的点；新矩形仍须避开每名玩家的原生安全区。
		foreach (Player player in Main.ActivePlayers)
		{
			var safe = new Rectangle(
				(int)(player.Center.X - NPC.sWidth / 2f - NPC.safeRangeX),
				(int)(player.Center.Y - NPC.sHeight / 2f - NPC.safeRangeY),
				NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
			if (body.Intersects(safe))
			{
				return false;
			}
		}
		return true;
	}
}
