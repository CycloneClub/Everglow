using ReLogic.Graphics;

namespace Everglow.Commons.UI.UIElements;

/// <summary>
/// Everglow UI 的富文本组件，使用原版(vanilla)字体库
/// </summary>
public class UIText : BaseElement, IUIText
{
	private string text;

	/// <summary>
	/// 显示的文本内容
	/// <para/>注: 被赋值时自动调用<see cref="Calculation"/>方法
	/// </summary>
	public string Text
	{
		get
		{
			return text;
		}

		set
		{
			text = value;
			Calculation();
		}
	}

	/// <summary>
	/// 字体贴图
	/// </summary>
	private DynamicSpriteFont font;

	/// <summary>
	/// 文本颜色
	/// </summary>
	public Color Color { get; set; }

	public float Scale { get; set; }

	/// <summary>
	/// 是否自动修改Size为<see cref="DynamicSpriteFont.MeasureString(string)"/>的计算结果
	/// </summary>
	public bool CalculateSize { get; set; } = true;

	public PositionStyle? CenterX { get; set; }

	public PositionStyle? CenterY { get; set; }

	public UIText(string t, DynamicSpriteFont spriteFont, float scale = 1f)
	{
		text = t;
		font = spriteFont;
		Scale = scale;
		Color = Color.White;

		Vector2 size = font.MeasureString(text) * Scale;
		Info.Width.Pixel = size.X;
		Info.Height.Pixel = size.Y;
		Info.Width.Percent = 0f;
		Info.Height.Percent = 0f;
	}

	public UIText(string t, DynamicSpriteFont spriteFont, Color textColor, float scale = 1f)
	{
		text = t;
		font = spriteFont;
		Scale = scale;
		Color = textColor;

		Vector2 size = font.MeasureString(text) * Scale;
		Info.Width.Pixel = size.X;
		Info.Height.Pixel = size.Y;
		Info.Width.Percent = 0f;
		Info.Height.Percent = 0f;
	}

	public override void Calculation()
	{
		if (CalculateSize)
		{
			Vector2 size = font.MeasureString(text) * Scale;
			Info.Width.Pixel = size.X;
			Info.Height.Pixel = size.Y;
			Info.Width.Percent = 0f;
			Info.Height.Percent = 0f;

			if (CenterX != null && CenterY != null)
			{
				var x = CenterX.Value;
				var y = CenterY.Value;
				Info.Left.Percent = x.Percent;
				Info.Top.Percent = y.Percent;
				Info.Left.Pixel = x.Pixel - Info.Width.Pixel / 2f;
				Info.Top.Pixel = y.Pixel - Info.Height.Pixel / 2f;
			}
		}
		base.Calculation();
	}

	protected override void DrawSelf(SpriteBatch sb)
	{
		base.DrawSelf(sb);
		sb.DrawString(font, text, Info.Location, Color, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
	}
}
