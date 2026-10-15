using Terraria.DataStructures;

namespace Everglow.Commons.Mechanics.Quest.Presentation.Icons;

public class TextureQuestIcon : QuestIconBase
{
	private TextureQuestIcon()
	{
	}

	private DrawAnimation animation = null;
	private Texture2D texture;
	private string tooltip;

	public override string Tooltip => tooltip;

	public override void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, Color color, float baseScale)
	{
		var drawCenter = new Vector2(
			destinationRectangle.X + destinationRectangle.Width / 2,
			destinationRectangle.Y + destinationRectangle.Height / 2);

		var frameRect = new Rectangle(0, 0, texture.Width, texture.Height);
		Vector2 origin = texture.Size() / 2;
		if (animation != null)
		{
			frameRect = animation.GetFrame(texture);
			animation.Update();
			origin = new Vector2(frameRect.Width, frameRect.Height) / 2;
		}
		var scale = GetTextureScale(destinationRectangle, frameRect, baseScale) * baseScale;

		spriteBatch.Draw(texture, drawCenter, frameRect, color, 0, origin, scale, SpriteEffects.None, 0);
	}

	public static TextureQuestIcon Create(Texture2D texture, string tooltip = null, DrawAnimationVertical animation = null)
	{
		return new TextureQuestIcon()
		{
			texture = texture,
			tooltip = tooltip ?? string.Empty,
			animation = animation,
		};
	}
}
