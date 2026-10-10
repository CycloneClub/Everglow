namespace Everglow.Commons.TileHelper;

public static class MirrorWallGeometry
{
	/// <summary>在未缩放屏幕坐标中围绕玩家翻转，再应用相机；墙的尺寸不参与定位。</summary>
	public static Matrix CreatePlayerReflectionTransform(float centerX, Matrix view) =>
		Matrix.CreateTranslation(-centerX, 0, 0)
		* Matrix.CreateScale(-1, 1, 1)
		* Matrix.CreateTranslation(centerX, 0, 0)
		* view;

	public static bool TryGetReflectionSlice(Rectangle cell, Rectangle viewport, out Rectangle slice)
	{
		// 纹理在捕获时已镜像，这里只裁剪，不再以墙中心重新映射。
		slice = Rectangle.Intersect(cell, viewport);
		return slice.Width > 0 && slice.Height > 0;
	}
}
