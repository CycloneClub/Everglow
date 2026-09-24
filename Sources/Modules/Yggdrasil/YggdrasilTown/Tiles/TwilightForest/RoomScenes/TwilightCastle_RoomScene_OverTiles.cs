using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.WorldGeneration;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.RoomScenes;

[Pipeline(typeof(WCSPipeline))]
public class TwilightCastle_RoomScene_OverTiles : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawTiles;

	public Point Offset = new Point(0, 0);

	public bool? CachedFlipH;

	public Dictionary<Texture2D, StaticSceneMesh> MeshCache;

	/// <summary>
	/// Cull by the center of the 640×336 scene image instead of the top-left anchor tile, so the scene is not killed while still partly visible.
	/// </summary>
	public override Vector2 CullingCheckPos => Position + new Vector2(FlipH() ? -320 : 320, 168);

	/// <summary>
	/// Cached version of <see cref="FlipHorizontally(int, int)" />. The result depends only on static world geometry.
	/// </summary>
	public bool FlipH()
	{
		CachedFlipH ??= FlipHorizontally(OriginTilePos.X, OriginTilePos.Y);
		return CachedFlipH.Value;
	}

	/// <summary>
	/// Get a cached static mesh for the given texture, building it once per VFX instance.
	/// </summary>
	public StaticSceneMesh GetOrBuildMesh(Texture2D texture, int originI, int originJ, float colorFactors = 1)
	{
		MeshCache ??= new Dictionary<Texture2D, StaticSceneMesh>();
		if (!MeshCache.TryGetValue(texture, out StaticSceneMesh mesh))
		{
			mesh = new StaticSceneMesh();
			mesh.Build(originI, originJ, texture, FlipH(), colorFactors);
			MeshCache.Add(texture, mesh);
		}
		return mesh;
	}

	public bool FlipHorizontally(int i, int j)
	{
		int leftSolid = 0;
		int rightSolid = 0;
		for (int x = 1; x < 15; x++)
		{
			Tile tileLeft = TileUtils.SafeGetTile(i - x, j);
			Tile tileRight = TileUtils.SafeGetTile(i + x, j);
			if (tileLeft.HasTile && tileLeft.TileType == ModContent.TileType<GreenRelicBrick>())
			{
				leftSolid++;
			}
			if (tileRight.HasTile && tileRight.TileType == ModContent.TileType<GreenRelicBrick>())
			{
				rightSolid++;
			}
		}
		if (rightSolid > leftSolid)
		{
			return false;
		}
		return true;
	}

	/// <summary>
	/// Allow to make a custon draw for this vfx.
	/// </summary>
	public delegate void CustomDrawVFX(TwilightCastle_RoomScene_OverTiles overtileDraw);

	public event CustomDrawVFX CustomDraw;

	public override void OnSpawn()
	{
		MaxDiatanceOutOfScreen = Main.screenWidth / 2f;
	}

	public override void Draw()
	{
		CustomDraw?.Invoke(this);
	}

	public override void Kill()
	{
		UnregisterCustomDraw(CustomDraw);
		base.Kill();
	}

	public void RegisterCustomLogic(CustomDrawVFX customDraw)
	{
		CustomDraw += customDraw;
	}

	public void UnregisterCustomDraw(CustomDrawVFX customDraw)
	{
		CustomDraw -= customDraw;
	}
}
