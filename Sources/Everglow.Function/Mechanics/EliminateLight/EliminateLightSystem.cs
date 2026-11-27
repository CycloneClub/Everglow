using Mono.Cecil.Cil;
using MonoMod.Cil;
using SubworldLibrary;
using Terraria.Graphics.Light;
using Terraria.Utilities;

namespace Everglow.Commons.Mechanics.EliminateLight;

public class EliminateLightSystem : ModSystem
{
	public override void Load()
	{
		On_TileLightScanner.ApplyWallLight += On_TileLightScanner_ApplyWallLight;
		On_TileLightScanner.ExportTo += On_TileLightScanner_ExportTo;
		IL_Main.DrawBlack += PatchDrawBlack;
	}

	private void On_TileLightScanner_ApplyWallLight(On_TileLightScanner.orig_ApplyWallLight orig, TileLightScanner self, Tile tile, int x, int y, ref FastRandom localRandom, ref Vector3 lightColor)
	{
		// Call first to preserve wall light.
		EliminateLightManager.ApplyEliminateLight(x, y, ref lightColor);
		// lightColor = Vector3.Max(lightColor, new Vector3(0.01f));

		orig(self, tile, x, y, ref localRandom, ref lightColor);
	}

	private void On_TileLightScanner_ExportTo(On_TileLightScanner.orig_ExportTo orig, TileLightScanner self, Rectangle area, LightMap outputMap, TileLightScannerOptions options)
	{
		// Optimize data structure for light scanning.
		EliminateLightManager.RebuildSpatialIndex();
		orig(self, area, outputMap, options);
		EliminateLightManager.Clear();
	}

	private static void PatchDrawBlack(ILContext il)
	{
		var cursor = new ILCursor(il);
		var subworld = new VariableDefinition(il.Module.TypeSystem.Boolean);
		il.Body.Variables.Add(subworld);

		// Reuse the forced viewport in subworlds. Only the original depth clipping reads force.
		cursor.GotoNext(MoveType.After, i => i.MatchLdarg(1), i => i.MatchBrtrue(out _));
		cursor.Index--;
		cursor.EmitDelegate<Func<bool>>(static () => SubworldSystem.Current != null);
		cursor.Emit(OpCodes.Dup);
		cursor.Emit(OpCodes.Stloc, subworld);
		cursor.Emit(OpCodes.Or);

		int showInvisible = -1;
		int tile = -1;
		int column = -1;
		cursor.GotoNext(MoveType.After, i => i.MatchCall<Main>(nameof(Main.ShouldShowInvisibleWalls)), i => i.MatchStloc(out showInvisible));
		// Use the ordinary brightness threshold at every subworld depth.
		cursor.GotoNext(MoveType.After, i => i.MatchCall<Main>("get_UnderworldLayer"));
		cursor.GotoNext(i => i.MatchStloc(out _));
		cursor.Emit(OpCodes.Ldloc, subworld);
		cursor.Emit(OpCodes.Ldc_I4_0);
		cursor.Emit(OpCodes.Ceq);
		cursor.Emit(OpCodes.And);

		cursor.GotoNext(i => i.MatchStloc(out tile), i => i.MatchLdloc(out column), i => i.MatchLdloc(out _), i => i.MatchCall<Lighting>(nameof(Lighting.Brightness)));
		cursor.Index++;
		var tileCheck = cursor.Next;

		// Use the same surface rule for submerged tiles when drawing to a render target.
		cursor.GotoNext(MoveType.After, i => i.MatchLdsfld<Main>(nameof(Main.worldSurface)));
		cursor.Emit(OpCodes.Ldloc, subworld);
		cursor.EmitDelegate<Func<double, bool, double>>(static (surface, inSubworld) => inSubworld ? double.MaxValue : surface);

		cursor.GotoNext(i => i.MatchLdloc(column), i => i.MatchLdloc(out _), i => i.MatchSub(), i => i.MatchLdcI4(0), i => i.MatchBle(out _));
		var flushRun = cursor.Next;

		cursor.Goto(tileCheck);
		cursor.EmitLdloc(tile);
		cursor.EmitLdloc(showInvisible);
		cursor.EmitDelegate<Func<Tile, bool, bool>>(CoversBackground);
		cursor.Emit(OpCodes.Brfalse, flushRun);
	}

	private static bool CoversBackground(Tile tile, bool showInvisible)
	{
		int wall = tile.WallType;
		if (wall != WallID.None && !Main.wallLight[wall] && !WallID.Sets.Transparent[wall]
			&& !WallID.Sets.AllowsWind[wall] && (showInvisible || !tile.IsWallInvisible))
		{
			return true;
		}
		return WorldGen.SolidTile(tile) && Main.tileBlockLight[tile.TileType]
			&& !TileID.Sets.DrawsWalls[tile.TileType] && (showInvisible || !tile.IsTileInvisible);
	}
}
