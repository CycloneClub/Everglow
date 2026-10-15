using FontStashSharp;
using Microsoft.Xna.Framework.Input;
using ReLogic.Localization.IME;
using ReLogic.OS;
using Terraria.GameContent;

namespace Everglow.Commons.UI.UIElements
{
	public class UIOneLineInputBox : BaseElement
	{
		public const string SYM = "|";

		public string Text
		{
			get => _text;
			set => _text = value;
		}

		public readonly SpriteFontBase Font;
		public bool PersetSym;
		public Vector2 SymSize;
		public Color TextColor;

		public int Cursor
		{
			get => _imeEnable ? _cursor : 0;
			set
			{
				if (_imeEnable)
					_cursor = value;
			}
		}

		public bool IsIMEEnable => _imeEnable;
		private bool _imeEnable = false;
		public Vector2 ElementSize;
		private float _timer = 0f;
		private bool _hideCursor = true;
		private Vector2 _textDrawOffset;
		private int _cursor;
		private string _text;

		public event Action<UIOneLineInputBox, string> OnTextChange;

		public event Action<UIOneLineInputBox, bool> OnIMEChange;

		public Vector2 OffsetThreshold = new Vector2(10f, 10f);

		private KeyCooldown left;
		private KeyCooldown right;
		private KeyCooldown enter;

		public UIOneLineInputBox(SpriteFontBase font, string text = "")
		{
			Text = text;
			Font = font;
			TextColor = Color.White;

			PersetSym = false;
			SymSize = new Vector2(MathHelper.Max(1f, font.LineHeight / 7f), font.LineHeight);

			left = new KeyCooldown(() => Main.keyState.IsKeyDown(Keys.Left));
			right = new KeyCooldown(() => Main.keyState.IsKeyDown(Keys.Right));
			enter = new KeyCooldown(() => Main.keyState.IsKeyDown(Keys.Enter));
		}

		public UIOneLineInputBox(SpriteFontBase font, string text = "", Color color = default, Vector2 symSizeOffset = default)
		{
			Text = text;
			Font = font;
			TextColor = color;

			PersetSym = false;
			var size = font.MeasureString(SYM);
			SymSize = size + symSizeOffset;

			left = new KeyCooldown(() => Main.keyState.IsKeyDown(Keys.Left));
			right = new KeyCooldown(() => Main.keyState.IsKeyDown(Keys.Right));
			enter = new KeyCooldown(() => Main.keyState.IsKeyDown(Keys.Enter));
		}

		public override void LoadEvents()
		{
			base.LoadEvents();
			Events.OnLeftClick += Events_OnLeftClick;
		}

		private void Events_OnLeftClick(BaseElement baseElement)
		{
			EnableIME();
			if (Text.Length == 0)
				return;
			float touchLength = Main.mouseX - Info.TotalLocation.X;
			if (touchLength < Font.MeasureString(Text[0].ToString()).X / 2f)
			{
				Cursor = 0;
				return;
			}
			if (touchLength >= Font.MeasureString(Text[..(Text.Length - 1)]).X + Font.MeasureString(Text[Text.Length - 1].ToString()).X / 2f)
			{
				Cursor = Text.Length;
				return;
			}

			int endIndex = Text.Length - 2;
			int middle;
			int startIndex = 0;
			while (true)
			{
				middle = (startIndex + endIndex) / 2;
				float nowTextLength = Font.MeasureString(Text[..middle]).X + Font.MeasureString(Text[middle + 1].ToString()).X / 2f;
				if (touchLength == nowTextLength)
				{
					Cursor = middle;
					return;
				}
				else if (touchLength > nowTextLength)
					startIndex = middle + 1;
				else
					endIndex = middle - 1;
				if (startIndex > endIndex)
				{
					Cursor = startIndex;
					return;
				}
			}
		}

		public void EnableIME()
		{
			if (_imeEnable)
				return;
			_imeEnable = true;
			_hideCursor = false;
			_timer = 0f;
			OnIMEChange?.Invoke(this, true);
		}

		public void DisableIME()
		{
			if (!_imeEnable)
				return;
			_imeEnable = false;
			_hideCursor = true;
			_timer = 0f;
			Terraria.GameInput.PlayerInput.WritingText = false;
			OnIMEChange?.Invoke(this, false);
		}

		public override void Update(GameTime gt)
		{
			base.Update(gt);

			if (_imeEnable && Main.mouseLeft && !GetElementsContainsPoint(Main.MouseScreen.ToPoint()).Contains(this))
			{
				DisableIME();
			}

			if (_imeEnable)
			{
				_timer += 1f;
				if (_timer > 60f / 4f)
				{
					_timer = 0f;
					_hideCursor = !_hideCursor;
				}
			}

			ElementSize.X = Font.MeasureString(Text).X + (IsIMEEnable ? SymSize.X : 0f);
			ElementSize.Y = Font.LineHeight;

			left.Update();
			right.Update();
			enter.Update();
		}

		public override void Calculation()
		{
			ElementSize.X = Font.MeasureString(Text).X + (IsIMEEnable ? SymSize.X : 0f);
			ElementSize.Y = Font.LineHeight;
			base.Calculation();
		}

		protected override void DrawChildren(SpriteBatch sb)
		{
			Vector2 offset = _textDrawOffset;
			Vector2 fontSize = Vector2.Zero;
			string lastHalf = string.Empty;
			string nextHalf = string.Empty;
			if (!string.IsNullOrEmpty(Text))
			{
				lastHalf = Text[..Cursor];
				if (!string.IsNullOrEmpty(lastHalf))
				{
					fontSize = Font.MeasureString(lastHalf);
					offset.Y = (Font.LineHeight - fontSize.Y) / 2f;
					sb.DrawString(Font, lastHalf, Info.TotalLocation + offset, TextColor);
					offset.X += fontSize.X;
				}
			}

			if (IsIMEEnable)
			{
				offset.Y = (Font.LineHeight - SymSize.Y) / 2f;

				float variation = offset.X - _textDrawOffset.X;
				if (!Info.HiddenOverflow)
					_textDrawOffset = Vector2.Zero;
				else if (offset.X + SymSize.X + OffsetThreshold.X > Info.Size.X)
				{
					_textDrawOffset.X -= offset.X + SymSize.X + OffsetThreshold.X - Info.Size.X;
				}
				else if (offset.X - Math.Min(variation, OffsetThreshold.Y) < 0)
				{
					_textDrawOffset.X -= offset.X - Math.Min(variation, OffsetThreshold.Y);
				}

				if (!_hideCursor)
				{
					if (PersetSym)
						sb.DrawString(Font, SYM, Info.TotalLocation + offset, TextColor);
					else
						sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)(Info.TotalLocation.X + offset.X),
							(int)(Info.TotalLocation.Y + offset.Y), (int)SymSize.X, (int)SymSize.Y), TextColor);
				}
				offset.X += SymSize.X;
			}

			if (!string.IsNullOrEmpty(Text))
			{
				nextHalf = Text[Cursor..];
				fontSize = Font.MeasureString(nextHalf);
				offset.Y = (Font.LineHeight - fontSize.Y) / 2f;
				sb.DrawString(Font, nextHalf, Info.TotalLocation + offset, TextColor);
				offset.X += fontSize.X;
			}
			base.DrawChildren(sb);

			if (_imeEnable)
			{
				Terraria.GameInput.PlayerInput.WritingText = true;
				Main.instance.HandleIME();
				var input = Main.GetInputText(lastHalf, false);
				Text = input + nextHalf;
				Cursor += input.Length - lastHalf.Length;
				if (input != lastHalf)
					OnTextChange?.Invoke(this, Text);

				if (Platform.Get<IImeService>().CandidateCount == 0)
				{
					if (left.IsKeyDown())
					{
						if (Cursor > 0)
							Cursor--;
						left.ResetCoolDown();
					}
					if (right.IsKeyDown())
					{
						if (Cursor < Text.Length)
							Cursor++;
						right.ResetCoolDown();
					}
					if (enter.IsKeyDown())
					{
						DisableIME();
						enter.ResetCoolDown();
					}
				}
			}
		}
	}
}
