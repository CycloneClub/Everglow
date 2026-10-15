using Everglow.Commons.Mechanics.Quest.Presentation;
using Everglow.Commons.UI.UIElements;
using static Everglow.Commons.Mechanics.Quest.UI.QuestContainer;

namespace Everglow.Commons.Mechanics.Quest.UI.UIElements.QuestDetail;

public class UIQuestStarLevel : BaseElement
{
	// TODO: Add a Quest Star Level
	public int Stars = 1;

	private bool _mouseOver = false;

	public override void OnInitialization()
	{
		Events.OnMouseHover += e =>
		{
			if (Math.Abs(Main.MouseScreen.X - HitBox.Center.X) < Stars * 50 / 2f)
			{
				Instance.MouseText = TextDefinition.GetQuestLevelTooltip(Stars);
				_mouseOver = true;
			}
			else
			{
				_mouseOver = false;
			}
		};
		Events.OnMouseOut += e =>
		{
			_mouseOver = false;
		};
		base.OnInitialization();
	}

	public override void Draw(SpriteBatch sb)
	{
		for (int k = 0; k < Stars; k++)
		{
			Texture2D star = ModAsset.QuestLevelStar.Value;
			Vector2 pos = Info.HitBox.Center() + new Vector2((k - (Stars - 1) / 2f) * 50, 0);
			Rectangle frame = new Rectangle(0, 0, 21, 20);
			sb.Draw(star, pos, frame, Color.White, 0, frame.Size() * 0.5f, 2f, SpriteEffects.None, 0);
			if (_mouseOver)
			{
				frame.Y += 20;
			}
			sb.Draw(star, pos, frame, Color.White, 0, frame.Size() * 0.5f, 2f, SpriteEffects.None, 0);
		}
	}
}
