namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;

internal static class VampireMatBallRendering
{
	internal static bool IsVisible(Vector2 screenCenter, float scale, Vector2 starSize, float starScaleMultiplier, Matrix transform, int viewportWidth, int viewportHeight)
	{
		// The two ball frames are 90 x 90; the glow may extend farther on either axis.
		float halfWidth = MathF.Max(45f, starSize.X * starScaleMultiplier * 0.5f) * MathF.Abs(scale);
		float halfHeight = MathF.Max(45f, starSize.Y * starScaleMultiplier * 0.5f) * MathF.Abs(scale);
		Vector2 center = Vector2.Transform(screenCenter, transform);
		// Transform the bounds too, including zoom, camera flips and rotation.
		float extentX = halfWidth * MathF.Abs(transform.M11) + halfHeight * MathF.Abs(transform.M21);
		float extentY = halfWidth * MathF.Abs(transform.M12) + halfHeight * MathF.Abs(transform.M22);
		// Keep a pixel of slack for rasterization at the viewport boundary.
		return center.X + extentX >= -1f && center.X - extentX <= viewportWidth + 1f
			&& center.Y + extentY >= -1f && center.Y - extentY <= viewportHeight + 1f;
	}
}
