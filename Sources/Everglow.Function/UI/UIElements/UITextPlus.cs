using Everglow.Commons.DataStructures;
using Everglow.Commons.UI.StringDrawerSystem;
using Everglow.Commons.Utilities;

namespace Everglow.Commons.UI.UIElements;

/// <summary>
/// Everglow UI 的富文本组件，使用自定义的富文本渲染库<see cref="FontStashSharp"/>
/// </summary>
public class UITextPlus : BaseElement, IUIText
{
	private string text;

	/// <summary>
	/// 显示的文本内容
	/// <para/>注: 被赋值时自动调用<see cref="StringDrawer"/>的<see cref="StringDrawer.Init(string)"/>方法
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
			StringDrawer.Init(text);
		}
	}

	/// <summary>
	/// 文本绘制工具
	/// </summary>
	public StringDrawer StringDrawer { get; init; }

	[Obsolete("The Scale property has been deprecated, it will affect nothing", true)]
	public float Scale { get; set; }

	/// <summary>
	/// 是否自动修改Size为<see cref="StringDrawer"/>的Size
	/// </summary>
	public bool CalculateSize { get; set; } = true;

	public PositionStyle? CenterX { get; set; }

	public PositionStyle? CenterY { get; set; }

	public UITextPlus(string t)
	{
		StringDrawer = new StringDrawer();
		Text = t;

		Info.Width.Pixel = StringDrawer.Size.X;
		Info.Height.Pixel = StringDrawer.Size.Y;
	}

	public override void Calculation()
	{
		if (CalculateSize)
		{
			Info.Width.Pixel = StringDrawer.Size.X;
			Info.Height.Pixel = StringDrawer.Size.Y;
			Info.Width.Percent = 0f;
			Info.Height.Percent = 0f;
		}
		if (CenterX != null && CenterY != null)
		{
			var x = CenterX.Value;
			var y = CenterY.Value;
			Info.Left.Percent = x.Percent;
			Info.Top.Percent = y.Percent;
			Info.Left.Pixel = x.Pixel - Info.Width.Pixel / 2f;
			Info.Top.Pixel = y.Pixel - Info.Height.Pixel / 2f;
		}
		base.Calculation();
		StringDrawer.SetPosition(Info.Location);
	}

	protected override void DrawSelf(SpriteBatch sb)
	{
		base.DrawSelf(sb);
		StringDrawer.Draw(sb);
		//SpriteBatchState sBS = GraphicsUtils.GetState(sb).Value;
		//sb.End();
		//sb.Begin(SpriteSortMode.Immediate, sBS.BlendState, sBS.SamplerState, sBS.DepthStencilState, sBS.RasterizerState, null, sBS.TransformMatrix);
		//Effect effect = ModAsset.FontPerfectPixel.Value;
		//effect.CurrentTechnique.Passes[0].Apply();
		//StringDrawer.Draw(sb);
		//sb.End();
		//sb.Begin(sBS);
	}
}
