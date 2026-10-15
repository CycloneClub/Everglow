using Terraria.GameContent;

namespace Everglow.Commons.UI.UIElements
{
	public class UIVerticalScrollbar : BaseElement, IScrollbar
	{
		private const int TOP_HEIGHT = 0;
		private const int BUTTOM_HEIGHT = 0;

		public Texture2D UIScrollbarTexture { get; protected set; }

		public Texture2D UIScrollbarInnerTexture { get; protected set; }

		protected Color _innerColor = Color.White;
		protected Vector2 _innerScale = new Vector2(0.75f);
		protected float _wheelValue;
		protected float _waitToWheelValue;
		private float alpha = 0f;
		public bool AlwaysOnLight = false;

		public virtual float Scale => Info.TotalHitBox.Width / (float)UIScrollbarTexture.Width;

		public virtual float TopMin => 10f * Scale;

		public virtual float TopMax => 10f * Scale;

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

		public UIVerticalScrollbar(float wheelValue = 0f)
		{
			Info.Width = new PositionStyle(20f, 0f);
			Info.Left = new PositionStyle(-20f, 1f);
			Info.Height = new PositionStyle(-20f, 1f);
			Info.Top = new PositionStyle(10f, 0f);
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
			float height = Info.TotalSize.Y - TopMax - TopMin - innerHeight;
			if (_isMouseDown)
			{
				WheelValue = (Main.mouseY -
					Info.TotalLocation.Y - TopMin - innerHeight / 2f) / height;
			}

			if (_wheelValue != _waitToWheelValue)
			{
				_wheelValue += (_waitToWheelValue - _wheelValue) / 4f;
				Calculation();
			}
		}

		protected override void DrawSelf(SpriteBatch sb)
		{
			int cTopHeight = (int)(TOP_HEIGHT * Scale), cButtomHeight = (int)(BUTTOM_HEIGHT * Scale);

			sb.Draw(UIScrollbarTexture, new Rectangle(
				Info.TotalHitBox.X,
				Info.TotalHitBox.Y, Info.TotalHitBox.Width, cTopHeight),
				new Rectangle(0, 0, UIScrollbarTexture.Width, TOP_HEIGHT),
				Color.White * MathHelper.SmoothStep(0, 1, alpha));

			sb.Draw(UIScrollbarTexture, new Rectangle(
				Info.TotalHitBox.X,
				Info.TotalHitBox.Y + cTopHeight, Info.TotalHitBox.Width, Info.TotalHitBox.Height - cTopHeight - cButtomHeight),
				new Rectangle(0, TOP_HEIGHT, UIScrollbarTexture.Width,
				UIScrollbarTexture.Height - TOP_HEIGHT - BUTTOM_HEIGHT),
				Color.White * MathHelper.SmoothStep(0, 1, alpha));

			sb.Draw(UIScrollbarTexture, new Rectangle(
				Info.TotalHitBox.X,
				Info.TotalHitBox.Y - cButtomHeight + Info.HitBox.Height, Info.TotalHitBox.Width, cButtomHeight),
				new Rectangle(0, UIScrollbarTexture.Height - BUTTOM_HEIGHT,
				UIScrollbarTexture.Width, BUTTOM_HEIGHT),
				Color.White * MathHelper.SmoothStep(0, 1, alpha));

			var innerHeight = UIScrollbarInnerTexture.Height * _innerScale.Y;
			float height = Info.TotalSize.Y - TopMax - TopMin - innerHeight;
			var top = TopMin + height * _wheelValue;
			sb.Draw(
				UIScrollbarInnerTexture,
				new Vector2(
					Info.TotalLocation.X - (UIScrollbarInnerTexture.Width * _innerScale.X - Info.TotalSize.X) / 2f,
					Info.TotalLocation.Y + top), null, _innerColor, 0f,
				Vector2.Zero, _innerScale, SpriteEffects.None, 0f);
		}
	}
}
