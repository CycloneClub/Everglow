namespace Everglow.Commons.TileHelper;

public static class MirrorWallGeometry
{
	/// <summary>围绕脚底缩放和翻转，再施加世界像素偏移，最后应用相机缩放。</summary>
	public static Matrix CreatePlayerReflectionTransform(Vector2 anchor, Vector2 offset, float scale, bool flipHorizontally, Matrix view) =>
		Matrix.CreateTranslation(-anchor.X, -anchor.Y, 0)
		* Matrix.CreateScale(flipHorizontally ? -scale : scale, scale, 1)
		* Matrix.CreateTranslation(anchor.X + offset.X, anchor.Y + offset.Y, 0)
		* view;

	/// <summary>在未缩放屏幕坐标中围绕玩家翻转，再应用相机；墙的尺寸不参与定位。</summary>
	public static Matrix CreatePlayerReflectionTransform(float centerX, Matrix view) =>
		CreatePlayerReflectionTransform(new Vector2(centerX, 0), Vector2.Zero, 1, true, view);

	public static bool TryGetReflectionSlice(Rectangle cell, Rectangle viewport, out Rectangle slice)
	{
		// 纹理在捕获时已镜像，这里只裁剪，不再以墙中心重新映射。
		slice = Rectangle.Intersect(cell, viewport);
		return slice.Width > 0 && slice.Height > 0;
	}
}
