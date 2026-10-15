using Everglow.Commons.Physics.MassSpringSystem;

namespace Everglow.Commons.TileHelper;

/// <summary>
/// A ModSystem that manages the update of hanging tiles in the world. It handles the physics simulation of hanging tiles using a mass-spring system and updates their positions accordingly. This system is responsible for ensuring that hanging tiles behave realistically when interacted with or affected by external forces.
/// </summary>
public class HangingTileUpdateSystem : ModSystem
{
	public static MassSpringContainer HangingTileMassSpringSystem = new MassSpringContainer();
	public static EulerSolver HangingTileEulerSolver = new EulerSolver(8);
	public static PBDSolver HangingTilePBDSolver = new PBDSolver(8);

	public static Dictionary<Point, float> WinchAdjustingPlayerTimers = new Dictionary<Point, float>();

	public override void PostUpdateEverything()
	{
		HangingTileMassSpringSystem = new MassSpringContainer();
		foreach (var HangingTile in TileLoader.tiles.OfType<HangingTile>())
		{
			foreach (var rope in HangingTile.RopesOfAllThisTileInTheWorld.Values)
			{
				HangingTileMassSpringSystem.AddMassSpringMesh(rope);
			}
		}
		HangingTileEulerSolver.Step(HangingTileMassSpringSystem, 1);
		foreach(var k in WinchAdjustingPlayerTimers.Keys.ToList())
		{
			WinchAdjustingPlayerTimers[k] -= 1;
			if (WinchAdjustingPlayerTimers[k] <= 0)
			{
				WinchAdjustingPlayerTimers.Remove(k);
			}
		}
	}

	public override void OnWorldLoad()
	{
		foreach (var hangingTile in TileLoader.tiles.OfType<HangingTile>())
		{
			hangingTile.RopesOfAllThisTileInTheWorld.Clear();
			hangingTile.WinchAdjustingPlayers.Clear();
			hangingTile.MouseOverWinchPlayers.Clear();
		}
		HangingTile.RopeGraspingPlayer.Clear();
		base.OnWorldLoad();
	}

	public override void OnWorldUnload()
	{
		foreach (var hangingTile in TileLoader.tiles.OfType<HangingTile>())
		{
			hangingTile.RopesOfAllThisTileInTheWorld.Clear();
			hangingTile.WinchAdjustingPlayers.Clear();
			hangingTile.MouseOverWinchPlayers.Clear();
		}
		HangingTile.RopeGraspingPlayer.Clear();
		base.OnWorldUnload();
	}
}
