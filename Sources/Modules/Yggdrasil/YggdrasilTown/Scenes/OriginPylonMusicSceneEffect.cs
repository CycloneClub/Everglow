using Everglow.Yggdrasil.YggdrasilTown.Tiles.OriginPylon;
using SubworldLibrary;

namespace Everglow.Yggdrasil.YggdrasilTown.Scenes;

/// <summary>
/// The music controller for <see cref="OriginPylon"/>.
/// </summary>
public class OriginPylonMusicSceneEffect : ModSceneEffect
{
	public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

	public override int Music => MusicLoader.GetMusicSlot(ModIns.Mod, ModAsset.OriginPylonBGM_Path);

	public override bool IsSceneEffectActive(Player player)
	{
		if (!SubworldSystem.IsActive<YggdrasilWorld>())
		{
			return false;
		}

		if (!IsPlayerNearTile(player, ModContent.TileType<OriginPylon>(), 80))
		{
			return false;
		}
		return true;
	}

	private static bool IsPlayerNearTile(Player player, int tileType, int range)
	{
		// Get the player's tile position
		int playerTileX = (int)(player.Center.X / 16);
		int playerTileY = (int)(player.Center.Y / 16);

		// Check tiles in a square area around the player
		for (int x = playerTileX - range; x <= playerTileX + range; x++)
		{
			for (int y = playerTileY - range; y <= playerTileY + range; y++)
			{
				// Check if the tile exists and is the custom tile
				if (x >= 0 && y >= 0 && x < Main.maxTilesX && y < Main.maxTilesY)
				{
					Tile tile = Main.tile[x, y];
					if (tile.HasTile && tile.TileType == tileType)
					{
						return true;
					}
				}
			}
		}

		return false;
	}
}
