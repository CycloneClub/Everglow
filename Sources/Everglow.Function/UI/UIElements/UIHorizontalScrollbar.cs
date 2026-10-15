using Terraria.GameContent;

namespace Everglow.Commons.UI.UIElements
{
	public class UIHorizontalScrollbar : BaseElement, IScrollbar
	{
		private const int LEFT_HEIGHT = 1;
		private const int RIGHT_HEIGHT = 1;

		public Texture2D UIScrollbarTexture { get; protected set; }

		public Texture2D UIScrollbarInnerTexture { get; protected set; }

		protected Color _innerColor = Color.White;
		protected Vector2 _innerScale = new Vector2(0.75f);
		protected float _wheelValue;
		protected float _waitToWheelValue;
		private float alpha = 0f;
		public bool AlwaysOnLight = false;

		public virtual float Scale => Info.TotalHitBox.Width / (float)UIScrollbarTexture.Width;

		public virtual float LeftMin => 10f * Scale;

		public virtual float LeftMax => 10f * Scale;

		public virtual Vector2 InnerScale => _innerScale;

		public BaseElement BindElement;

		protected bool _isMouseDown = false;

		public float WheelValue
		{
			get
			{
				return _wheelValue;
			}

			set
			{
				_waitToWheelValue = value;
				if (_waitToWheelValue > 1f)
				{
					_waitToWheelValue = 1f;
				}

				if (_waitToWheelValue < 0f)
				{
					_waitToWheelValue = 0f;
				}
			}
		}

		public float WheelValueMult
		{
			get => _wheelValueMult;
			set => _wheelValueMult = value;
		}

		private float _wheelValueMult = 1f;

		public UIHorizontalScrollbar(float wheelValue = 0f)
		{
			Info.Height = new PositionStyle(20f, 0f);
			Info.Top = new PositionStyle(-20f, 1f);
			Info.Width = new PositionStyle(-20f, 1f);
			Info.Left = new PositionStyle(10f, 0f);
			UIScrollbarTexture = TextureAssets.MagicPixel.Value;
			UIScrollbarInnerTexture = TextureAssets.MagicPixel.Value;
			WheelValue = wheelValue;
		}

		public override void LoadEvents()
		{
			Events.OnLeftDown += e => _isMouseDown = true;
			Events.OnLeftUp += e => _isMouseDown = false;
		}

		public override void Update(GameTime gt)
		{
			base.Update(gt);

			bool isMouseHover = ContainsPoint(Main.MouseScreen);

			if (AlwaysOnLight)
			{
				alpha = 1f;
			}
			else
			{
				if ((isMouseHover || _isMouseDown) && alpha < 1f)
				{
					alpha += 0.01f;
				}

				if (!(isMouseHover || _isMouseDown) && alpha > 0f)
				{
					alpha -= 0.01f;
				}
			}

			_innerColor = Color.White * alpha;

			var innerHeight = UIScrollbarInnerTexture.Height * _innerScale.Y;
			float height = Info.TotalSize.Y - LeftMax - LeftMin - innerHeight;
			if (_isMouseDown)
			{
				WheelValue = (Main.mouseY -
					Info.TotalLocation.Y - LeftMin - innerHeight / 2f) / height;
			}

			if (_wheelValue != _waitToWheelValue)
			{
				_wheelValue += (_waitToWheelValue - _wheelValue) / 4f;
				Calculation();
			}
		}

		protected override void DrawSelf(SpriteBatch sb)
		{
			//float scale = Info.HitBox.Height / (float)UIScrollbarInnerTexture.Height;
			//int ct = (int)(12 * scale);
			//sb.Draw(UIScrollbarInnerTexture, new Rectangle(
			//	Info.HitBox.X,
			//	Info.HitBox.Y, ct, Info.HitBox.Height),
			//	new Rectangle(0, 0, 12, UIScrollbarInnerTexture.Height), Color.White * alpha);

			//sb.Draw(UIScrollbarInnerTexture, new Rectangle(
			//	Info.HitBox.X + ct,
			//	Info.HitBox.Y, Info.HitBox.Width - ct * 2, Info.HitBox.Height),
			//	new Rectangle(12, 0, UIScrollbarInnerTexture.Width - 24, UIScrollbarInnerTexture.Height), Color.White * alpha);

			//sb.Draw(UIScrollbarInnerTexture, new Rectangle(
			//	Info.HitBox.X - ct + Info.HitBox.Width,
			//	Info.HitBox.Y, ct, Info.HitBox.Height),
			//	new Rectangle(UIScrollbarInnerTexture.Width - 12, 0, 12, UIScrollbarInnerTexture.Height), Color.White * alpha);
		}
	}
}
