using Terraria.GameContent.Drawing;

namespace Everglow.Commons.TileHelper;

/// <summary>
/// 为物块提供一个每帧绘制60次的接口，这是管理类 <br/>
/// 注意：对solid物块不管用，这是给家具用的 <br/>
/// 相关: <see cref="ITileFluentlyDrawn"/>
/// </summary>
public class TileFluentDrawManager : ModSystem
{
	/// <summary>
	/// 在每次非solidLayer的常规物块绘制（即15times/tick的绘制）前清除 <br/>
	/// 即与原版 TileDrawing._specialsCount 清除时机相同 <br/>
	/// </summary>
	private static List<(ModTile, Point)> _fluentTiles = new List<(ModTile, Point)>();

	/// <summary>
	/// 应该作为添加丝滑绘制点的唯一方法，正常情况下两点不应该重叠（方法里面就不加检测了） <br/>
	/// 2024/5/5 在这里加重叠似乎已经是最有效的查重位置了
	/// 为保证正常工作，只应当在ModTile的PreDraw内调用，而且不应该给Solid物块调用
	/// </summary>
	public static void AddFluentPoint(ModTile modTile, int i, int j)
	{
		if (modTile is not ITileFluentlyDrawn)
		{
			Ins.Logger.Warn("TileFluentDrawManager.AddFluentPoint should only be called for ModTile inheriting ITileFluentlyDrawn");
			return;
		}
		if (_fluentTiles.Contains((modTile, new Point(i, j))))
		{
			return;
		}
		_fluentTiles.Add((modTile, new Point(i, j)));
	}

	public override void Load()
	{
		// 清除 cachedFluentTiles
		On_TileDrawing.ClearCachedTileDraws += (orig, self, solidLayer) =>
		{
			orig.Invoke(self, solidLayer);
			if (!solidLayer)
			{
				_fluentTiles.Clear();
			}
		};

		// 实际上插入到 TileDrawing.PostDrawTiles 的最后一个特殊绘制后，进行绘制
		On_TileDrawing.DrawReverseVines += (orig, self) =>
		{
			orig.Invoke(self);

			Vector2 unscaledPosition = Main.Camera.UnscaledPosition;
			foreach ((ModTile modTile, Point position) in _fluentTiles)
			{
				if (modTile is ITileFluentlyDrawn tileFluent && modTile is not null)
				{
					tileFluent.FluentDraw(unscaledPosition, position, Main.spriteBatch, self);
				}
			}
		};
	}
}
