using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Everglow.Yggdrasil.Common.Fish;

/// <summary>
/// 生成一个可以被钓鱼的物品
/// </summary>
/// <param name="item">物品 id</param>
/// <param name="liquid">
/// 物品会在什么液体上生成，原版液体使用 <c>LiquidID</c>，ModLiquid 液体使用 <c>LiquidLoader.LiquidType</c>
/// </param>
/// <param name="chance">物品每帧的生成概率</param>
public class FishableItem(int item, int liquid, float chance)
{
	public int Item = item;
	public int Liquid = liquid;
	public float Chance = chance;
}
