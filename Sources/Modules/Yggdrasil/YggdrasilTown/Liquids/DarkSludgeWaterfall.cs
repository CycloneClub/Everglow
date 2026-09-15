using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModLiquidLib.ModLoader;

namespace Everglow.Yggdrasil.YggdrasilTown.Liquids;

public class DarkSludgeWaterfall : ModLiquidFall
{
	public override bool PlayWaterfallSounds()
	{
		return false;
	}

	public override float? Alpha(int x, int y, float Alpha, int maxSteps, int s, Tile tileCache)
	{
		return 0.7f;
	}
}
