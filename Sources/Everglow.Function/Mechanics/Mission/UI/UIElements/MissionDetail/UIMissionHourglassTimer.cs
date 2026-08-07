using Everglow.Commons.UI.UIElements;
using Spine;

namespace Everglow.Commons.Mechanics.Mission.UI.UIElements;

/// <summary>
/// 沙漏计时器<see cref="MissionContainer"/>
/// </summary>
public class UIMissionHourglassTimer : UIBlock
{
	public float Timer;

	public float MaxTime;

	public bool OnSelect = false;

	public override void Draw(SpriteBatch sb)
	{
		// 声明光栅化状态，剔除状态为不剔除，开启剪切测试
		var overflowHiddenRasterizerState = new RasterizerState
		{
			CullMode = CullMode.None,
			ScissorTestEnable = true,
		};

		// 如果不隐藏UI部件
		if (!Info.IsHidden && IsVisible)
		{
			// 关闭画笔
			sb.End();

			// 启用画笔，传参：延迟绘制（纹理合批优化），alpha颜色混合模式，点采样，不启用深度模式，UI大小矩阵
			sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap,
				DepthStencilState.None, overflowHiddenRasterizerState, null, Main.UIScaleMatrix);

			// 绘制自己
			DrawSelf(sb);

			sb.End();

			sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap,
				DepthStencilState.None, overflowHiddenRasterizerState, null, Main.UIScaleMatrix);
			DrawReflection(sb);
		}

		// 设定gd是画笔绑定的图像设备
		var gd = sb.GraphicsDevice;

		// 储存绘制原剪切矩形
		var scissorRectangle = gd.ScissorRectangle;

		// 如果启用溢出隐藏
		if (Info.HiddenOverflow)
		{
			// 关闭画笔以便修改绘制参数
			sb.End();

			// 修改光栅化状态
			sb.GraphicsDevice.RasterizerState = overflowHiddenRasterizerState;

			// 修改GD剪切矩形为原剪切矩形与现剪切矩形的交集
			gd.ScissorRectangle = Rectangle.Intersect(gd.ScissorRectangle, TransformedHiddenOverflowRectangle);

			// 启用画笔，传参：延迟绘制（纹理合批优化），alpha颜色混合模式，点采样，不启用深度模式，UI大小矩阵
			sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap,
				DepthStencilState.None, overflowHiddenRasterizerState, null, Main.UIScaleMatrix);
		}

		// 绘制子元素
		DrawChildren(sb);

		// 如果启用溢出隐藏
		if (Info.HiddenOverflow)
		{
			// 关闭画笔
			sb.End();

			// 修改光栅化状态
			gd.RasterizerState = overflowHiddenRasterizerState;

			// 将剪切矩形换回原剪切矩形
			gd.ScissorRectangle = scissorRectangle;

			// 启用画笔
			sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
				SamplerState.PointWrap, DepthStencilState.None,
				overflowHiddenRasterizerState, null, Main.UIScaleMatrix);
		}
	}

	protected override void DrawSelf(SpriteBatch sb)
	{
		if (MaxTime <= 0)
		{
			return;
		}
		float duration = 1 - (float)(Main.time * 0.001f) % 1;
		int top_liquid_height = (int)(57 * duration);
		int bottom_liquid_height = 57 - top_liquid_height;

		Texture2D tex = ModAsset.TimeFunnel.Value;
		Vector2 pos = Info.TotalHitBox.Center();
		int width = tex.Width / 6;
		Rectangle frame_bound = new Rectangle(0, 0, width, tex.Height);
		if (OnSelect)
		{
			frame_bound = new Rectangle(310, 0, width, tex.Height);
		}
		Rectangle frame_liquid_top = new Rectangle(64, 1 + bottom_liquid_height, 58, top_liquid_height);
		Rectangle frame_liquid_bottom = new Rectangle(65, 57 + top_liquid_height, 56, bottom_liquid_height);
		Rectangle frame_liquid_surface_top = new Rectangle(126, 1 + bottom_liquid_height, 58, 2);
		Rectangle frame_liquid_surface_bottom = new Rectangle(126, 57 + top_liquid_height, 58, 2);
		Rectangle frame_liquid_post = new Rectangle(92, 43, 2, 2);
		Rectangle frame_shadow = new Rectangle(width * 3, 0, width, tex.Height);

		Color liquidColor = new Color(1f, 1f, 1f, 0.75f);
		sb.Draw(tex, pos, frame_bound, Color.White, 0f, frame_bound.Size() / 2f, 1f, SpriteEffects.None, 0f);
		sb.Draw(tex, pos, frame_shadow, Color.White, 0f, frame_bound.Size() / 2f, 1f, SpriteEffects.None, 0f);
		sb.Draw(tex, pos, frame_liquid_top, liquidColor, 0f, new Vector2(29, top_liquid_height), 1f, SpriteEffects.None, 0f);
		sb.Draw(tex, pos, frame_liquid_bottom, liquidColor, 0f, new Vector2(28, -57 + bottom_liquid_height), 1f, SpriteEffects.None, 0f);
		sb.Draw(tex, pos, frame_liquid_surface_top, Color.White, 0f, new Vector2(29, top_liquid_height), 1f, SpriteEffects.None, 0f);
		sb.Draw(tex, pos, frame_liquid_surface_bottom, Color.White, 0f, new Vector2(29, -57 + bottom_liquid_height), 1f, SpriteEffects.None, 0f);
		sb.Draw(tex, new Rectangle((int)pos.X - 1, (int)pos.Y, 2, 56 - bottom_liquid_height), frame_liquid_post, liquidColor);
	}

	private void DrawReflection(SpriteBatch sb)
	{
		Texture2D tex = ModAsset.TimeFunnel.Value;
		Vector2 pos = Info.TotalHitBox.Center();
		int width = tex.Width / 6;
		Rectangle frame_reflection = new Rectangle(width * 4, 0, width, tex.Height);
		sb.Draw(tex, pos, frame_reflection, Color.White, 0f, frame_reflection.Size() / 2f, 1f, SpriteEffects.None, 0f);
	}
}
