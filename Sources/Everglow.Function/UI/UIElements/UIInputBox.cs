using Microsoft.Xna.Framework.Input;

using ReLogic.Graphics;
using ReLogic.Localization.IME;
using ReLogic.OS;

namespace Everglow.Commons.UI.UIElements
{
	public delegate void TextChange(UIInputBox inputBox, string text);

	public class UIInputBox : UIPanel
	{
		private const string cursorSym = "|";
		public string Text { get => _text; set => _text = value; }

		public int Cursor
		{
			get
			{
				var texts = Text.Split('\n');
				int r = 0;
				for (int i = 0; i < _cursorPosition.Y; i++)
					r += texts[i].Length + 1;
				return r + _cursorPosition.X;
			}
			set
			{
				int l = value;
				if (l < 0)
				{
					_cursorPosition = Point.Zero;
					return;
				}
				var texts = Text.Split('\n');
				_cursorPosition.Y = 0;
				if (l > 0)
					foreach (var t in texts)
					{
						if (l > t.Length + 1)
						{
							l -= t.Length + 1;
							_cursorPosition.Y++;
						}
						else
							break;
					}
				_cursorPosition.X = l;
			}
		}

		public bool IsEnableIME
		{
			get => isEnableIME;
		}

		private readonly float symOffsetX;
		private string _text;

		public Point CursorPosition
		{
			get
			{
				return _cursorPosition;
			}
			set
			{
				var v = value;
				if (v.Y <= 0 && v.X < 0)
				{
					_cursorPosition = Point.Zero;
					return;
				}

				var texts = Text.Split('\n');
				while (v.X < 0 && v.Y - 1 >= 0)
				{
					v.X += texts[v.Y - 1].Length;
					v.Y--;
				}
				if (v.Y < 0)
					v.Y = 0;
				//while (v.Y < texts.Length && v.X > texts[v.Y].Length)
				//{
				//    v.X -= texts[v.Y].Length;
				//    v.Y++;
				//}
				if (v.Y >= texts.Length)
					v.Y = texts.Length - 1;
				if (v.Y < 0)
					v.Y = 0;
				if (v.X > texts[v.Y].Length)
					v.X = texts[v.Y].Length;
				if (v.X < 0)
					v.X = 0;
				_cursorPosition = v;
			}
		}

		public event TextChange OnTextChange;

		private Point _cursorPosition;
		private Color _color;
		private int timer;
		private Rectangle symHitBox;
		private Vector2 offset;
		private bool isEnableIME = false;
		private KeyCooldown up;
		private KeyCooldown down;
		private KeyCooldown left;
		private KeyCooldown right;
		private KeyCooldown enter;
		private DynamicSpriteFont _font;
		private float LineYHight = 0f;

		public UIInputBox(DynamicSpriteFont font, string text = "", Point cursorPosition = default(Point), Color color = default(Color), Vector2 symSizeOffice = default)
		{
			_text = text;
			_cursorPosition = cursorPosition;
			_color = color;
			_cursorPosition = Point.Zero;
			symHitBox = Rectangle.Empty;
			_font = font;
			var c = _font.MeasureString(cursorSym) + symSizeOffice;
			symHitBox.Width = (int)c.X;
			symHitBox.Height = (int)c.Y;
			symOffsetX = c.X / 2f;
			Info.Height.SetValue(30f, 0f);
			Info.HiddenOverflow = true;
			LineYHight = _font.MeasureString("啊").Y;
			offset = new Vector2(symOffsetX, 0f);
			CanDrag = false;
		}

		public override void OnInitialization()
		{
			base.OnInitialization();
			Info.SetMargin(2f);

			up = new KeyCooldown(() => Main.keyState.IsKeyDown(Keys.Up));
			down = new KeyCooldown(() => Main.keyState.IsKeyDown(Keys.Down));
			left = new KeyCooldown(() => Main.keyState.IsKeyDown(Keys.Left));
			right = new KeyCooldown(() => Main.keyState.IsKeyDown(Keys.Right));
			enter = new KeyCooldown(() => Main.keyState.IsKeyDown(Keys.Enter));

			timer = 14;
		}

		public override void LoadEvents()
		{
			base.LoadEvents();
			Events.OnMouseHover += element =>
			{
				if (!Main.mouseLeft)
					return;

				isEnableIME = true;

				Point cp = Point.Zero;

				if (string.IsNullOrEmpty(Text))
				{
					CursorPosition = cp;
					return;
				}

				var mousePos = Main.MouseScreen - Info.Location - offset + new Vector2(symOffsetX, 0f);
				var texts = Text.Split('\n');
				for (int i = 0; i < texts.Length; i++)
				{
					if (mousePos.Y > LineYHight)
						mousePos.Y -= LineYHight;
					else
						break;
					if (cp.Y < texts.Length - 1)
						cp.Y++;
				}
				var text = texts[cp.Y];
				if (mousePos.X >= _font.MeasureString(text).X)
				{
					cp.X = text.Length;
				}
				else
				{
					for (int i = 0; i < text.Length; i++)
					{
						if (mousePos.X <= _font.MeasureString(text.Substring(0, i + 1)).X)
						{
							cp.X = i;
							break;
						}
					}
				}
				CursorPosition = cp;
			};
		}

		public override void Update(GameTime gt)
		{
			if (Main.mouseLeft && !ContainsPoint(Main.MouseScreen) && isEnableIME)
				isEnableIME = false;

			up.Update();
			down.Update();
			left.Update();
			right.Update();
			enter.Update();
			base.Update(gt);
			if (isEnableIME)
				timer++;
			else
				timer = 14;
		}

		protected override void DrawChildren(SpriteBatch sb)
		{
			var texts = Text.Split('\n');
			float offsetY = 0f;
			string text;
			int j = timer % 14;
			float x;
			for (int i = 0; i < texts.Length; i++)
			{
				text = texts[i];
				if (isEnableIME && i == CursorPosition.Y && (j <= 8 && j >= 0))
				{
					x = _font.MeasureString(text.Substring(0, CursorPosition.X)).X;
					symHitBox.X = (int)(Info.Location.X + x - symOffsetX + offset.X);
					symHitBox.Y = (int)(Info.Location.Y + offsetY + offset.Y);
					if (!Info.HitBox.Contains(symHitBox))
					{
						float hitboxMaxX = 0f, symHitboxMaxX = 0f, hitboxMaxY = 0f, symHitboxMaxY = 0f;
						if (symHitBox.Height > Info.HitBox.Height || symHitBox.Width > Info.HitBox.Width)
						{
							hitboxMaxX = Info.HitBox.X + Info.HitBox.Width;
							symHitboxMaxX = symHitBox.X + Math.Min(Info.HitBox.Width, symHitBox.Width);
							hitboxMaxY = Info.HitBox.Y + Info.HitBox.Height;
							symHitboxMaxY = symHitBox.Y + Math.Min(Info.HitBox.Height, symHitBox.Height);
						}
						else
						{
							hitboxMaxX = Info.HitBox.X + Info.HitBox.Width;
							symHitboxMaxX = symHitBox.X + symHitBox.Width;
							hitboxMaxY = Info.HitBox.Y + Info.HitBox.Height;
							symHitboxMaxY = symHitBox.Y + symHitBox.Height;
						}
						if (hitboxMaxX < symHitboxMaxX)
							offset.X -= symHitboxMaxX - hitboxMaxX;
						if (hitboxMaxY < symHitboxMaxY)
							offset.Y -= symHitboxMaxY - hitboxMaxY;
						if (symHitBox.X < Info.HitBox.X)
							offset.X += Info.HitBox.X - symHitBox.X;
						if (symHitBox.Y < Info.HitBox.Y)
							offset.Y += Info.HitBox.Y - symHitBox.Y;
					}
					sb.DrawString(_font, cursorSym, Info.Location + new Vector2(x - symOffsetX, offsetY) + offset, _color);
				}
				sb.DrawString(_font, text, Info.Location + new Vector2(0f, offsetY) + offset, _color);
				offsetY += LineYHight;
			}
			base.DrawChildren(sb);

			if (isEnableIME)
			{
				Terraria.GameInput.PlayerInput.WritingText = true;
				Main.instance.HandleIME();
				var cp = Cursor;
				var remaining = _text.Substring(cp, _text.Length - cp);
				var crop = _text.Substring(0, cp);
				var input = Main.GetInputText(crop, true);
				var p = CursorPosition;
				_text = input + remaining;
				p.X += input.Length - crop.Length;
				if (input != crop)
					OnTextChange?.Invoke(this, _text);
				if (Platform.Get<IImeService>().CandidateCount == 0)
				{
					if (up.IsKeyDown())
					{
						p.Y--;
						up.ResetCoolDown();
					}
					if (down.IsKeyDown())
					{
						p.Y++;
						down.ResetCoolDown();
					}
					if (left.IsKeyDown())
					{
						if (!(p.Y <= 0 && p.X <= 0))
						{
							if (p.X == 0)
							{
								p.Y--;
								p.X = texts[p.Y].Length;
							}
							else
								p.X--;
						}
						left.ResetCoolDown();
					}
					if (right.IsKeyDown())
					{
						if (p.X + 1 > texts[p.Y].Length)
						{
							p.X = 0;
							p.Y++;
						}
						else
							p.X++;
						right.ResetCoolDown();
					}
					if (enter.IsKeyDown())
					{
						cp = Cursor;
						_text = _text.Insert(cp, "\n");
						p.X = 0;
						p.Y++;
						enter.ResetCoolDown();
					}
				}
				CursorPosition = p;
				Main.instance.DrawWindowsIMEPanel(Info.TotalLocation + Info.TotalSize, 0.5f);
			}
		}
	}
}
