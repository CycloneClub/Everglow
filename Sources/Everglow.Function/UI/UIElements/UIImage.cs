namespace Everglow.Commons.UI.UIElements;

public class UIImage : BaseElement
{
	/// <summary>
	/// 计算实际大小时的方式
	/// </summary>
	public enum CalculationStyle
	{
		/// <summary>
		/// 不计算
		/// </summary>
		None,

		/// <summary>
		/// 锁定宽高比，以宽为参考轴
		/// </summary>
		LockAspectRatioMainWidth,

		/// <summary>
		/// 锁定宽高比，以高为参考轴
		/// </summary>
		LockedAspectRatioMainHeight,
	}

	protected Texture2D texture;
	protected Color color;

	/// <summary>
	/// 计算实际大小时的方式
	/// </summary>
	public CalculationStyle Style { get; set; } = CalculationStyle.None;

	public Vector2 Origin { get; set; } = Vector2.Zero;

	public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;

	public float Rotation { get; set; } = 0f;

	public Rectangle? SourceRectangle { get; set; } = null;

	public Texture2D Texture { get => texture; set => texture = value; }

	public Color Color { get => color; set => color = value; }

	public UIImage(Texture2D texture, Color color)
	{
		this.color = color;

		if (texture != null)
		{
			this.texture = texture;
			Info.Width.Pixel = texture.Width;
			Info.Height.Pixel = texture.Height;
		}
	}

	protected override void DrawSelf(SpriteBatch sb)
	{
		base.DrawSelf(sb);

		if (texture != null)
		{
			sb.Draw(texture, Info.TotalHitBox, SourceRectangle, color, Rotation, Origin, SpriteEffects, 0f);
		}
	}

	public override void Calculation()
	{
		base.Calculation();
		if (Style == CalculationStyle.LockAspectRatioMainWidth)
		{
			float aspectRatio = (float)texture.Width / (float)texture.Height;
			Info.Height.Pixel = Info.Size.X / aspectRatio;
			base.Calculation();
		}
		else if (Style == CalculationStyle.LockedAspectRatioMainHeight)
		{
			float aspectRatio = (float)texture.Width / (float)texture.Height;
			Info.Width.Pixel = Info.Size.Y * aspectRatio;
			base.Calculation();
		}
	}
}
