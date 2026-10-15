using FontStashSharp;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Everglow.Commons.UI.StringDrawerSystem.DrawerItems.TextDrawers
{
	public class DitherText : TextDrawer
	{
		public Vector2 Amplitude;
		public Vector2 SymExtraSize;

		protected override Vector2 GetTextSize(string text)
		{
			return base.GetTextSize(text) + new Vector2(SymExtraSize.X * 2 * text.Length, SymExtraSize.Y * 2);
		}

		public override void Init(StringDrawer stringDrawer, string originalText, string name, StringParameters stringParameters)
		{
			base.Init(stringDrawer, originalText, name, stringParameters);
			if (stringParameters == null)
				return;
			Amplitude = stringParameters.GetVector2("Amplitude",
				stringDrawer.DefaultParameters.GetVector2("Amplitude"));
			SymExtraSize = stringParameters.GetVector2("SymExtraSize",
				stringDrawer.DefaultParameters.GetVector2("SymExtraSize"));
		}

		public override void Draw(SpriteBatch sb)
		{
			var pos = Position;
			foreach (char c in Text)
			{
				var text = c.ToString();
				sb.DrawString(Font, Text, Position + Offset +
					new Vector2((Main.rand.NextFloat() - 0.5f) * Amplitude.X * 4f, (Main.rand.NextFloat() - 0.5f) * Amplitude.Y * 4f), Color, Scale, Rotation,
					Origin, LayerDepth, CharacterSpacing, 0, TextStyle,
					FontSystemEffect, EffectAmount);
				pos.X += GetTextSize(text).X;
			}
		}
	}
}
