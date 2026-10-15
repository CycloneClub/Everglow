using Everglow.Commons.UI.UIElements;

namespace Everglow.Commons.Mechanics.Quest.UI.UIElements;

public class UIQuestBackground : UIBlock
{
	public override void Draw(SpriteBatch sb)
	{
		base.Draw(sb);
		Rectangle sourceRectangle = new Rectangle(0, 0, HitBox.Width, HitBox.Height);
		sb.Draw(ModAsset.Marble_Texture.Value, HitBox, sourceRectangle, new Color(1f, 1f, 1f, 1));
	}
}
