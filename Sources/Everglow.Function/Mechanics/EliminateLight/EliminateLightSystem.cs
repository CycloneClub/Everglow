using Terraria.Graphics.Light;
using Terraria.Utilities;

namespace Everglow.Commons.Mechanics.EliminateLight;

public class EliminateLightSystem : ModSystem
{
	public override void Load()
	{
		On_TileLightScanner.ApplyWallLight += On_TileLightScanner_ApplyWallLight;
		On_TileLightScanner.ExportTo += On_TileLightScanner_ExportTo;
		On_Main.DrawBlack += Main_DrawBlack;
	}

	private void On_TileLightScanner_ApplyWallLight(On_TileLightScanner.orig_ApplyWallLight orig, TileLightScanner self, Tile tile, int x, int y, ref FastRandom localRandom, ref Vector3 lightColor)
	{
		// Call first to preserve wall light.
		EliminateLightManager.ApplyEliminateLight(x, y, ref lightColor);

		orig(self, tile, x, y, ref localRandom, ref lightColor);
	}

	private void On_TileLightScanner_ExportTo(On_TileLightScanner.orig_ExportTo orig, TileLightScanner self, Rectangle area, LightMap outputMap, TileLightScannerOptions options)
	{
		// Optimize data structure for light scanning.
		EliminateLightManager.RebuildSpatialIndex();
		orig(self, area, outputMap, options);
		EliminateLightManager.Clear();
	}

	/// <summary>
	/// TODO: Warning: This hook delete the BlackTile drawing, which may cause some visual bugs.
	/// </summary>
	/// <param name="orig"></param>
	/// <param name="self"></param>
	/// <param name="intoRenderTargets"></param>
	/// <param name="force"></param>
	private void Main_DrawBlack(On_Main.orig_DrawBlack orig, Main self, bool intoRenderTargets, bool force)
	{
		// orig(self, intoRenderTargets, force);
	}
}
