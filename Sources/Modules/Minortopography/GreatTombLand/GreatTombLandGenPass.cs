using Everglow.Commons.TileHelper;
using Terraria.DataStructures;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Everglow.Minortopography.GreatTombLand;


/* Fork：Minortopography/GenPass
     * 
     * Everglow Dev Team
     * 
     * 后续需要解决的问题：
     * 1、箱子的东西随机化生成,应该和MapIO的WriteChest有关系，看看能不能@Override
     * 2、地形的集群序列化生成
     * 3、地块边缘的额外算法
     * 4、通过随机数，其他三个墓穴的构建完成
     *  
     * Ling Write 2023-03-06 16:40
     */
public class GreatTombLand : ModSystem
{
	private class GreatTombLandGenPass : GenPass
	{

		public GreatTombLandGenPass() : base("GreatTombLand", 500)
		{
		}


		//将东西写入WordGen里面并生效
		public override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			//Todo:翻译：生成森林的大墓地 HJSON
			//Main.statusText = Terraria.Localization.Language.GetTextValue("Mods.Everlow.Common.WorldSystem.BuildGreatTombLand");
			progress.Message = "正在生成丛林的阴森墓穴集群……";
			//构建墓地的主要方法
			BuildGreatTombLand();
		}
	}

	//MapIO
	public static void QuickBuild(int x, int y, string Path)
	{
		var mapIO = new MapIO(x, y);

		mapIO.Read(ModIns.Mod.GetFileStream("Minortopography/MapIO/" + Path));

		var it = mapIO.GetEnumerator();
		while (it.MoveNext())
		{
			WorldGen.SquareTileFrame(it.CurrentCoord.X, it.CurrentCoord.Y);
			WorldGen.SquareWallFrame(it.CurrentCoord.X, it.CurrentCoord.Y);
		}
	}

	public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight) => tasks.Add(new GreatTombLandGenPass());

	/// <summary>
	/// 生成森林墓穴
	/// </summary>
	public static void BuildGreatTombLand()
	{
		return;//TODO:保障Master安全,先封起来
			   //TODO 自适应尚未完成
		Point16 CenterPoint = RandomGreatTombLandGenPass();
		int X0 = CenterPoint.X;
		int Y0 = CenterPoint.Y - 90;
		float Width = 5;

		for (int i = (int)-Width; i <= (int)Width; i++)
		{

			if (i <= -Width + 7 || i >= Width - 7)
			{
				while (GetMergeToJungle(X0, Y0) <= 10)
				{
					X0 = Main.rand.Next(300, Main.maxTilesX - 600);
					Y0 = Main.rand.Next(500, Main.maxTilesY - 700);
				}
				switch (Main.rand.Next(6))
				{
					//默认区域
					case 0:
					default:
						QuickBuild(X0, Y0, "GreatTombLandDemo-1.mapio");
						break;
					//其他Roll
					//TODO 尚未完成
					case 1:
					case 2:
						QuickBuild(X0, Y0, "GreatTombLandDemo-1.mapio");
						break;
					case 3:
					case 4:
						QuickBuild(X0, Y0, "GreatTombLandDemo-1.mapio");
						break;
					case 5:
					case 6:
						QuickBuild(X0, Y0, "GreatTombLandDemo-1.mapio");
						break;
				}

			}
		}
	}

	private static int GetMergeToJungle(int PoX, int PoY)
	{
		int CrashCount = 0;
		ushort[] MustHaveTileType = new ushort[]
		{
			TileID.JungleGrass, //丛林草方块
                TileID.JunglePlants, //丛林草
                TileID.JungleVines, //丛林藤
                TileID.JunglePlants2, //高大丛林草
                TileID.PlantDetritus//丛林花
            };
		for (int x = -256; x < 257; x += 8)
		{
			for (int y = -128; y < 129; y += 8)
			{
				if (Array.Exists(MustHaveTileType, Ttype => Ttype == Main.tile[x + PoX, y + PoY].TileType))
					CrashCount++;
			}
		}
		return CrashCount;
	}

	public static Point16 RandomGreatTombLandGenPass()
	{
		//目标取点
		var AimPoint = new List<Point16>();


		int Jmin = Main.maxTilesY - 300;
		for (int i = 33; i < Main.maxTilesX - 34; i += 33)
		{
			for (int j = 12; j < Main.maxTilesY - 300; j += 6)
			{
				Tile tile = Main.tile[i, j];
				//获取的物块是丛林神庙墙 然后区域在正上方90地块。
				//TODO 可能含有其他地形冲突，需要后续解决
				if (tile.TileType == TileID.LihzahrdBrick)
				{
					AimPoint.Add(new Point16(i, j));
					if (j < Jmin)
						Jmin = Main.maxTilesY - 300;
					break;
				}
			}
		}
		var newAimPoint = new List<Point16>();
		foreach (Point16 point in AimPoint)
		{
			if (point.Y <= Jmin + 30)
				newAimPoint.Add(point);
		}
		return newAimPoint[Main.rand.Next(newAimPoint.Count)];
	}
}
