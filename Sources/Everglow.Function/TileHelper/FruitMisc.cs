using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Utilities;
namespace Everglow.Commons.TileHelper;

public class ShakeTreeTweak
{
	public abstract class FruitPickerTool : ModItem
	{
		public override bool CanUseItem(Player player)
		{
			// 标记命中树的坐标,掉落物产生后瞬间清除
			_shakeTreeCoord = new Point((int)(Main.MouseWorld.X / 16f), (int)(Main.MouseWorld.Y / 16f));
			if (Main.SmartCursorIsUsed)
			{
				_shakeTreeCoord = new Point(Main.SmartCursorX, Main.SmartCursorY);
			}
			return base.CanUseItem(player);
		}
		public override bool? UseItem(Player player)
		{
			return base.UseItem(player);
		}
	}

	private class ShakeTreeItem : GlobalItem
	{
		public override void OnSpawn(Item item, IEntitySource source)
		{
			if (_isShakingTree && source is EntitySource_ShakeTree)
			{
				_hasItemDropped = true;
			}
		}
	}

	private static bool _isShakingTree;
	private static bool _hasItemDropped; // 检测是否在摇树过程中有物品掉落
	public static Point _shakeTreeCoord;

	public static void Load()
	{
		On_WorldGen.ShakeTree += (orig, i, j) =>
		{
			if (_shakeTreeCoord != new Point(i, j))
			{
				orig(i, j);
			}
			_shakeTreeCoord = new Point(0, 0);
			_isShakingTree = true;
			_hasItemDropped = false;

			// 在orig前获取树是否被摇过，因为orig会修改WorldGen.treeShakeX,Y的值，标记为被摇过
			bool treeShaken = false;

			WorldGen.GetTreeBottom(i, j, out var x, out var y);
			for (int k = 0; k < WorldGen.numTreeShakes; k++)
			{
				if (WorldGen.treeShakeX[k] == x && WorldGen.treeShakeY[k] == y)
				{
					treeShaken = true;
					break;
				}
			}

			orig(i, j);

			_isShakingTree = false;

			if (WorldGen.numTreeShakes == WorldGen.maxTreeShakes || _hasItemDropped || treeShaken)
			{
				return;
			}

			TreeTypes treeType = WorldGen.GetTreeType(Main.tile[x, y].type);
			if (treeType == TreeTypes.None)
			{
				return;
			}

			y--;
			while (y > 10 && Main.tile[x, y].active() && TileID.Sets.IsShakeable[Main.tile[x, y].type])
			{
				y--;
			}

			y++;
			if (!WorldGen.IsTileALeafyTreeTop(x, y) || Collision.SolidTiles(x - 2, x + 2, y - 2, y + 2))
			{
				return;
			}

			int fruit = GetShakeTreeFruit(treeType);
			if (fruit > -1)
			{
				Item.NewItem(WorldGen.GetItemSource_FromTreeShake(x, y), x * 16, y * 16, 16, 16, fruit);
			}
		};
	}
	public static int GetShakeTreeFruit(TreeTypes treeType)
	{
		switch (treeType)
		{
			case TreeTypes.Forest:
				WeightedRandom<short> weightedRandom = new();
				weightedRandom.Add(ItemID.Apple);
				weightedRandom.Add(ItemID.Apricot);
				weightedRandom.Add(ItemID.Peach);
				weightedRandom.Add(ItemID.Grapefruit);
				weightedRandom.Add(ItemID.Lemon);
				return weightedRandom.Get();
			case TreeTypes.Snow:
				return (!WorldGen.genRand.NextBool(2)) ? ItemID.Plum : ItemID.Cherry;
			case TreeTypes.Jungle:
				return (!WorldGen.genRand.NextBool(2)) ? ItemID.Mango : ItemID.Pineapple;
			case TreeTypes.Palm or TreeTypes.PalmCorrupt or TreeTypes.PalmCrimson or TreeTypes.PalmHallowed:
				return (!WorldGen.genRand.NextBool(2)) ? ItemID.Coconut : ItemID.Banana;
			case TreeTypes.Corrupt or TreeTypes.PalmCorrupt:
				return (!WorldGen.genRand.NextBool(2)) ? ItemID.Elderberry : ItemID.BlackCurrant;
			case TreeTypes.Crimson or TreeTypes.PalmCrimson:
				return (!WorldGen.genRand.NextBool(2)) ? ItemID.BloodOrange : ItemID.Rambutan;
			case TreeTypes.Hallowed or TreeTypes.PalmHallowed:
				return (!WorldGen.genRand.NextBool(2)) ? ItemID.Dragonfruit : ItemID.Starfruit;
			case TreeTypes.Ash:
				return (!WorldGen.genRand.NextBool(2)) ? ItemID.SpicyPepper : ItemID.Pomegranate;
			default:
				return -1;
		}
	}
}
