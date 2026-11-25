using System.Text.Json;
using Everglow.Commons.Mechanics.CustomBackground;
using Everglow.Commons.Mechanics.MirrorReflection;

namespace Everglow.Yggdrasil.YggdrasilTown.Background;

[Autoload(Side = ModSide.Client)]
public class TownBackgroundReflections : ModSystem
{
	// Former wall cells that coincide with visible window panes or metalwork on the surface town facade.
	// Each named material has rows of local Y, inclusive left X and exclusive right X.
	private WallRun[] runs;
	private readonly List<(Rectangle Bounds, int WallType)> windowRegions = [];

	private readonly record struct WallRun(int Y, int Left, int Right, int WallType);

	public override void PostSetupContent()
	{
		var materials = JsonSerializer.Deserialize<Dictionary<string, int[][]>>(Mod.GetFileBytes(ModAsset.TownReflectiveWalls_Path));
		List<WallRun> resolved = [];
		foreach (var (name, rows) in materials)
		{
			// tML registers vanilla field names and modded ModName/WallName keys in this lookup.
			int wallType = WallID.Search.GetId(name);
			foreach (int[] row in rows)
			{
				resolved.Add(new WallRun(row[0], row[1], row[2], wallType));
			}
		}
		runs = resolved.OrderBy(run => run.Y).ThenBy(run => run.Left).ToArray();
		// Local pixel rectangles from the lighted window atlas, excluding pixels covered by the wall runs.
		var windows = JsonSerializer.Deserialize<Dictionary<string, int[][]>>(Mod.GetFileBytes(ModAsset.TownWindowReflections_Path));
		foreach (var (name, rectangles) in windows)
		{
			int wallType = WallID.Search.GetId(name);
			foreach (int[] rect in rectangles)
			{
				windowRegions.Add((new Rectangle(rect[0], rect[1], rect[2], rect[3]), wallType));
			}
		}
		var reflections = ModContent.GetInstance<MirrorReflectionSystem>();
		reflections.CollectBackgroundWalls += Collect;
		reflections.CollectBackgroundWallRegions += CollectWindows;
	}

	private void CollectWindows(Rectangle visiblePixels, Action<Rectangle, int> add)
	{
		if (!YggdrasilWorld.InYggdrasil)
		{
			return;
		}
		Rectangle town = YggdrasilTownCentralSystem.TownArea;
		Rectangle bounds = new(town.X * 16, town.Y * 16, town.Width * 16, town.Height * 16);
		if (!visiblePixels.Intersects(bounds) || !ModContent.GetInstance<BackgroundSystem>().HasBgSlide(typeof(YggdrasilTown_Construct).FullName))
		{
			return;
		}
		foreach (var (local, material) in windowRegions)
		{
			Rectangle pane = local;
			pane.Offset(bounds.X, bounds.Y);
			if (visiblePixels.Intersects(pane))
			{
				add(pane, material);
			}
		}
	}

	private void Collect(Rectangle visible, Action<Point, int> add)
	{
		if (!YggdrasilWorld.InYggdrasil)
		{
			return;
		}

		Rectangle town = YggdrasilTownCentralSystem.TownArea;
		if (!town.Intersects(visible) || !ModContent.GetInstance<BackgroundSystem>().HasBgSlide(typeof(YggdrasilTown_Construct).FullName))
		{
			return;
		}

		visible.Offset(-town.X, -town.Y);
		foreach (WallRun run in runs)
		{
			int y = run.Y;
			if (y >= visible.Bottom)
			{
				break;
			}
			if (y >= visible.Top)
			{
				int end = Math.Min(run.Right, visible.Right);
				for (int x = Math.Max(run.Left, visible.Left); x < end; x++)
				{
					add(new Point(town.X + x, town.Y + y), run.WallType);
				}
			}
		}
	}
}
