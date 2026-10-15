using Everglow.Commons.UI.UIElements;

namespace Everglow.Commons.Mechanics.Quest.UI.UIElements.QuestDetail;

public class UIQuestDurationBar : BaseElement
{
	public float MaxDuration = 5;

	public float CurrentDuration = 0;

	public bool OnSelect = false;

	/// <summary>
	/// 0: 任务旗帜
	/// 1: 任务进度条
	/// </summary>
	public int BarStyle = 0;

	public override void Draw(SpriteBatch sb)
	{
		CurrentDuration = (int)((Main.time * 0.03f) % (MaxDuration + 1));
		Texture2D tex = ModAsset.QuestDurationFlag.Value;
		if (MaxDuration > 1)
		{
			for (int k = 0; k < MaxDuration; k++)
			{
				float y = Info.HitBox.Center().Y;
				float distance = Info.HitBox.Width / (MaxDuration - 1);
				var pos = new Vector2(Info.HitBox.X + distance * k, y);
				var frame = new Rectangle(25, 0, 25, 23);
				if (k < CurrentDuration)
				{
					frame = new Rectangle(0, 0, 25, 23);
				}
				if (MathF.Abs(distance) >= 30)
				{
					sb.Draw(tex, pos, frame, Color.White, 0, frame.Size() * 0.5f, 2f, SpriteEffects.None, 0);
					if (OnSelect)
					{
						frame = new Rectangle(50, 0, 25, 23);
						sb.Draw(tex, pos, frame, Color.White, 0, frame.Size() * 0.5f, 2f, SpriteEffects.None, 0);
					}
				}
				else
				{
					sb.Draw(tex, pos, frame, Color.White, 0, frame.Size() * 0.5f, 1f, SpriteEffects.None, 0);
					if (OnSelect)
					{
						frame = new Rectangle(50, 0, 25, 23);
						sb.Draw(tex, pos, frame, Color.White, 0, frame.Size() * 0.5f, 1f, SpriteEffects.None, 0);
					}
				}
			}
		}
	}
}
