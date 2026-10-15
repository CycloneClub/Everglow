using Everglow.Commons.DeveloperContent.Items;
using Everglow.Commons.Enums;
using Everglow.Commons.TileHelper;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Everglow.Commons.VFX.Pipelines;
using static Everglow.Commons.Utilities.MathUtils;
using static Everglow.Commons.Utilities.TileUtils;

namespace Everglow.Commons.DeveloperContent.VFXs;

public class TileToolBoxSystem : ModSystem
{
	public override void OnWorldLoad()
	{
		TileToolBoxInterface.ClearHistory();
		base.OnWorldLoad();
	}

	public override void OnWorldUnload()
	{
		TileToolBoxInterface.ClearHistory();
		base.OnWorldUnload();
	}
}
