using FontStashSharp;

namespace Everglow.Commons.UI.StringDrawerSystem.DrawerItems.TextDrawers
{
	public class TypewriterText : TextDrawer
	{
		public float AnimationTime;
		public float CursorBlinkingInterval = 0f;
		public float CursorBlinkingTime;
		public bool EnableAnimation = false;
		public Color CursorColor = Color.White;
		public Vector2 CursorScale = Vector2.One;
		public float RepeatAnimationTime = -1;
		public float RepeatLinkAnimationTime = -1;

		public override void Init(StringDrawer stringDrawer, string originalText, string name, StringParameters stringParameters)
		{
			base.Init(stringDrawer, originalText, name, stringParameters);
			if (stringParameters == null)
				return;
			AnimationTime = stringParameters.GetFloat("AnimationTime",
				stringDrawer.DefaultParameters.GetFloat("AnimationTime", 0.2f)) * 60f;
			CursorBlinkingTime = stringParameters.GetFloat("CursorBlinkingTime",
				stringDrawer.DefaultParameters.GetFloat("CursorBlinkingTime", 0.2f)) * 60f;
			CursorBlinkingInterval = stringParameters.GetFloat("CursorBlinkingInterval",
				stringDrawer.DefaultParameters.GetFloat("CursorBlinkingInterval"));
			CursorColor = stringParameters.GetColor("CursorColor",
				stringDrawer.DefaultParameters.GetColor("CursorColor", Color.White));
			CursorScale = stringParameters.GetVector2("CursorScale",
				stringDrawer.DefaultParameters.GetVector2("CursorScale", Vector2.One));
			RepeatAnimationTime = stringParameters.GetFloat("RepeatAnimationTime",
				stringDrawer.DefaultParameters.GetFloat("RepeatAnimationTime", -1)) * 60f;
			RepeatLinkAnimationTime = stringParameters.GetFloat("RepeatLinkAnimationTime",
				stringDrawer.DefaultParameters.GetFloat("RepeatLinkAnimationTime", -1)) * 60f;
		}

		private int textIndex = 0;
		private float time = 0f;
		private float time1 = 0f;
		private float time2 = 0f;
		private float time3 = 0f;
		private float time4 = 0f;
		private bool head = false;

		public override void ResetAnimation()
		{
			base.ResetAnimation();
			head = false;
			EnableAnimation = false;
			time = 0f;
			time1 = 0f;
			time2 = 0f;
			textIndex = 0;
		}

		public override void StartAnimation()
		{
			EnableAnimation = true;
			time2 = CursorBlinkingTime;
		}

		public override TextDrawer Decomposition(string text, List<DrawerItem> drawerItems, int index)
		{
			TypewriterText typewriterText = (TypewriterText)base.Decomposition(text, drawerItems, index);
			typewriterText.AnimationTime = AnimationTime;
			typewriterText.CursorBlinkingTime = CursorBlinkingTime;
			typewriterText.CursorBlinkingInterval = CursorBlinkingInterval;
			typewriterText.CursorColor = CursorColor;
			return typewriterText;
		}

		public override void Draw(SpriteBatch sb)
		{
			if (RepeatLinkAnimationTime >= 0f)
			{
				if (time4 >= RepeatLinkAnimationTime)
				{
					time4 = 0f;
					ResetLinkAnimation();
				}
				time4 += 1f;
			}
			if (RepeatAnimationTime >= 0f)
			{
				if (time3 >= RepeatAnimationTime)
				{
					time3 = 0f;
					ResetAnimation();
					StartAnimation();
				}
				time3 += 1f;
			}
			if (!head && HeadLinkItems.Count == 0)
			{
				head = true;
				StartAnimation();
			}
			var pos = Position;
			var text = Text[..textIndex];
			sb.DrawString(Font, Text, pos + Offset, Color, Scale, Rotation,
				Origin, LayerDepth, CharacterSpacing, 0, TextStyle,
				FontSystemEffect, EffectAmount);
			pos.X += GetTextSize(text).X;
			if (!EnableAnimation || textIndex >= Text.Length)
				return;
			if (time1 >= CursorBlinkingInterval)
			{
				time1 = 0f;
				time2 = CursorBlinkingTime;
			}
			if (time2 > 0f)
			{
				time2 -= 1f;
				sb.DrawString(Font, "_", pos + Offset, CursorColor, CursorScale, Rotation,
					Origin, 0, CharacterSpacing, 0);
			}
			if (time >= AnimationTime)
			{
				time = 0f;
				textIndex++;
				if (textIndex >= Text.Length)
				{
					foreach (var i in TailLinkItems)
					{
						i.StartAnimation();
					}
					EnableAnimation = false;
				}
			}
			time += 1f;
			if (time2 <= 0f)
				time1 += 1f;
		}
	}
}
