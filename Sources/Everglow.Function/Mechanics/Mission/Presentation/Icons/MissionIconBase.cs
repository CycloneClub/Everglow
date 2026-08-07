namespace Everglow.Commons.Mechanics.Mission.Presentation.Icons;

public abstract class MissionIconBase
{
	public virtual string Tooltip { get; }

	public void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle) => Draw(spriteBatch, destinationRectangle, Color.White);

	public void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, Color color) => Draw(spriteBatch, destinationRectangle, color, 1f);

	public abstract void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, Color color, float baseScale);

	public static float GetTextureScale(Rectangle destinationRect, Rectangle frameRect, float baseScale = 1f)
	{
		var scale = frameRect.Width > frameRect.Height
			? destinationRect.Width / (frameRect.Width * baseScale)
			: destinationRect.Height / (frameRect.Height * baseScale);
		return scale > 1f ? 1f : scale;
	}
}
