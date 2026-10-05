namespace Everglow.Commons.VFX.Scene;

public class SceneVFXSystem : ModSystem
{
	private static Dictionary<(int, int), bool> _tilePointHasScene = [];

	public static Dictionary<(int X, int Y), bool> TilePointHasScene => _tilePointHasScene;

	private Vector2 lastCheckScreenPosition = default;
	private float maxUpdateDistance = 800;
	private float rescanTriggerDistance = 150;

	public override void PreUpdateDusts()
	{
		Vector2 deltaScreenPos = lastCheckScreenPosition - Main.screenPosition;
		if (deltaScreenPos.Length() > rescanTriggerDistance)
		{
			lastCheckScreenPosition = Main.screenPosition;
			int startX = (int)((Main.screenPosition.X - maxUpdateDistance) / 16f);
			startX = Math.Max(startX, 20);
			int endX = (int)((Main.screenPosition.X + Main.screenWidth + maxUpdateDistance) / 16f);
			endX = Math.Min(endX, Main.maxTilesX - 20);
			int startY = (int)((Main.screenPosition.Y - maxUpdateDistance) / 16f);
			startY = Math.Max(startY, 20);
			int endY = (int)((Main.screenPosition.Y + Main.screenHeight + maxUpdateDistance) / 16f);
			endY = Math.Min(endY, Main.maxTilesY - 20);
			for (int x = startX; x < endX; x++)
			{
				for (int y = startY; y < endY; y++)
				{
					Tile tile = Main.tile[x, y];
					if (tile != null && tile.HasTile)
					{
						if (TileLoader.GetTile(tile.TileType) is ISceneTile)
						{
							if (!TilePointHasScene.ContainsKey((x, y)) || !TilePointHasScene[(x, y)])
							{
								ISceneTile sceneTile = TileLoader.GetTile(tile.TileType) as ISceneTile;
								if (sceneTile != null)
								{
									sceneTile.AddScene(x, y);
									TilePointHasScene[(x, y)] = true;
								}
							}
						}
					}
				}
			}
		}
	}

	public override void OnWorldLoad()
	{
		_tilePointHasScene = [];
	}

	public override void OnWorldUnload()
	{
		_tilePointHasScene.Clear();
		_tilePointHasScene = null;
	}

	public class SceneTileBehavior : GlobalTile
	{
		public override void PlaceInWorld(int i, int j, int type, Item item)
		{
			Tile tile = Main.tile[i, j];
			ISceneTile sceneTile = TileLoader.GetTile(tile.TileType) as ISceneTile;
			if (sceneTile != null)
			{
				sceneTile.AddScene(i, j);
				SceneVFXSystem.TilePointHasScene[(i, j)] = true;
			}
		}
	}
}
