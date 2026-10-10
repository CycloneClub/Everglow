using Everglow.Commons.Utilities;

namespace Everglow.Commons.TileHelper;

/// <summary>
/// 可复用的镜面背景墙。Example 模块的镜面演示系统为此基类的墙绘制玩家倒影。
/// </summary>
public abstract class MirrorWall : ModWall
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Placeables;

	public override string Texture => ModAsset.White_Mod;

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		if (Main.dedServ)
		{
			return false;
		}
		Vector2 offset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
		Vector2 position = new Vector2(i * 16, j * 16) - Main.screenPosition + offset;
		Rectangle cell = new((int)position.X, (int)position.Y, 16, 16);
		Color light = Lighting.GetColor(i, j);
		Texture2D pixel = ModAsset.White.Value;
		spriteBatch.Draw(pixel, cell, light.MultiplyRGB(new Color(75, 95, 115)));
		Color edge = light.MultiplyRGB(new Color(175, 195, 210));
		if (!IsMirror(i - 1, j))
		{
			spriteBatch.Draw(pixel, new Rectangle(cell.X, cell.Y, 1, 16), edge);
		}
		if (!IsMirror(i + 1, j))
		{
			spriteBatch.Draw(pixel, new Rectangle(cell.Right - 1, cell.Y, 1, 16), edge);
		}
		if (!IsMirror(i, j - 1))
		{
			spriteBatch.Draw(pixel, new Rectangle(cell.X, cell.Y, 16, 1), edge);
		}
		if (!IsMirror(i, j + 1))
		{
			spriteBatch.Draw(pixel, new Rectangle(cell.X, cell.Bottom - 1, 16, 1), edge);
		}
		return false;
	}

	public static bool IsMirror(int i, int j) => WorldGen.InWorld(i, j, 0)
		&& WallLoader.GetWall(Main.tile[i, j].WallType) is MirrorWall;
}
