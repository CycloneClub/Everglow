using Everglow.Commons.UI.UIElements;

namespace Everglow.Commons.Mechanics.Mission.UI.UIElements;

/// <summary>
/// 任务信息板<see cref="MissionContainer"/>
/// </summary>
public class UIMissionBlock : UIBlock
{
	/// <summary>
	/// 0: 描述
	/// 1: 目标
	/// </summary>
	public int MissionBlockStyle = 0;

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
		DrawMissionPanel(sb, Info.TotalHitBox, Color.White);
		int x = Info.TotalHitBox.X;
		int y = Info.TotalHitBox.Y;
		int w = Info.TotalHitBox.Width;
		int h = Info.TotalHitBox.Height;
		Texture2D tex_side = ModAsset.MissionMessageBoard_bottom_side.Value;
		sb.Draw(tex_side, new Rectangle(x + (w - 177) / 2, y + h - 13, 177, 13), null, Color.White);
		Texture2D tex_icon = ModAsset.MissionDescription_Icon.Value;
		if(MissionBlockStyle == 1)
		{
			tex_icon = ModAsset.MissionObjective_Icon.Value;
			Texture2D divider = ModAsset.MissionDetailPanelDivider.Value;
			int halfWidth = Info.HitBox.Width / 2 - 20;
			sb.Draw(divider, new Rectangle(x + 10, y + h - 180, halfWidth, 15), new Rectangle(0, 0, halfWidth, 15), Color.White);
			sb.Draw(divider, new Rectangle(x + w - 35, y + h - 180, -halfWidth, 15), new Rectangle(0, 0, halfWidth, 15), Color.White);
		}
		sb.Draw(tex_icon, new Rectangle(x + 5, y + 6, 35, 35), null, Color.White);
	}

	public static void DrawMissionPanel(SpriteBatch sb, Rectangle hitbox, Color panelColor)
	{
		int x = hitbox.X;
		int y = hitbox.Y;
		int w = hitbox.Width;
		int h = hitbox.Height;
		Texture2D tex = ModAsset.MissionMessageBoard.Value;
		int sideListWidth = 40;
		int topRowHeight = 53;
		int bottomRowHeight = 62;
		int middleListWidth = 85;
		int middleRowHeight = 55;
		int leftList2Width = (w - 165) / 2;
		int rightList2Width = w - 165 - leftList2Width;
		int topRow2Height = (h - 170) / 2;
		int bottomRow2Height = h - 170 - topRow2Height;
		if (w < 165)
		{
			middleListWidth = w - 80;
			leftList2Width = 0;
			rightList2Width = 0;
		}
		if (h < 170)
		{
			middleRowHeight = h - 115;
			topRow2Height = 0;
			bottomRow2Height = 0;
		}

		// Top row
		sb.Draw(tex, new Rectangle(x, y, sideListWidth, topRowHeight), new Rectangle(0, 0, sideListWidth, topRowHeight), panelColor);
		sb.Draw(tex, new Rectangle(x + sideListWidth, y, leftList2Width, topRowHeight), new Rectangle(41, 0, 9, topRowHeight), panelColor);
		sb.Draw(tex, new Rectangle(x + sideListWidth + leftList2Width, y, middleListWidth, topRowHeight), new Rectangle(51, 0, middleListWidth, topRowHeight), panelColor);
		sb.Draw(tex, new Rectangle(x + sideListWidth + middleListWidth + leftList2Width, y, rightList2Width, topRowHeight), new Rectangle(137, 0, 9, topRowHeight), panelColor);
		sb.Draw(tex, new Rectangle(x + w - sideListWidth, y, sideListWidth, topRowHeight), new Rectangle(147, 0, sideListWidth, topRowHeight), panelColor);

		// Top2 row
		sb.Draw(tex, new Rectangle(x, y + topRowHeight, sideListWidth, topRow2Height), new Rectangle(0, 54, sideListWidth, 9), panelColor);
		sb.Draw(tex, new Rectangle(x + sideListWidth, y + topRowHeight, leftList2Width, topRow2Height), new Rectangle(41, 54, 9, 9), panelColor);
		sb.Draw(tex, new Rectangle(x + sideListWidth + leftList2Width, y + topRowHeight, middleListWidth, topRow2Height), new Rectangle(51, 54, middleListWidth, 9), panelColor);
		sb.Draw(tex, new Rectangle(x + sideListWidth + middleListWidth + leftList2Width, y + topRowHeight, rightList2Width, topRow2Height), new Rectangle(137, 54, 9, 9), panelColor);
		sb.Draw(tex, new Rectangle(x + w - sideListWidth, y + topRowHeight, sideListWidth, topRow2Height), new Rectangle(147, 54, sideListWidth, 9), panelColor);

		// Middle row
		sb.Draw(tex, new Rectangle(x, y + topRow2Height + topRowHeight, sideListWidth, middleRowHeight), new Rectangle(0, 64, sideListWidth, middleRowHeight), panelColor);
		sb.Draw(tex, new Rectangle(x + sideListWidth, y + topRow2Height + topRowHeight, leftList2Width, middleRowHeight), new Rectangle(41, 64, 9, middleRowHeight), panelColor);
		sb.Draw(tex, new Rectangle(x + sideListWidth + leftList2Width, y + topRow2Height + topRowHeight, middleListWidth, middleRowHeight), new Rectangle(51, 64, middleListWidth, middleRowHeight), panelColor);
		sb.Draw(tex, new Rectangle(x + sideListWidth + middleListWidth + leftList2Width, y + topRow2Height + topRowHeight, rightList2Width, middleRowHeight), new Rectangle(137, 64, 9, middleRowHeight), panelColor);
		sb.Draw(tex, new Rectangle(x + w - sideListWidth, y + topRow2Height + topRowHeight, sideListWidth, middleRowHeight), new Rectangle(147, 64, sideListWidth, middleRowHeight), panelColor);

		// Bottom2 row
		sb.Draw(tex, new Rectangle(x, y + h - bottomRowHeight - bottomRow2Height, sideListWidth, bottomRow2Height), new Rectangle(0, 120, sideListWidth, 10), panelColor);
		sb.Draw(tex, new Rectangle(x + sideListWidth, y + h - bottomRowHeight - bottomRow2Height, leftList2Width, bottomRow2Height), new Rectangle(41, 120, 9, 10), panelColor);
		sb.Draw(tex, new Rectangle(x + sideListWidth + leftList2Width, y + h - bottomRowHeight - bottomRow2Height, middleListWidth, bottomRow2Height), new Rectangle(51, 120, middleListWidth, 10), panelColor);
		sb.Draw(tex, new Rectangle(x + sideListWidth + middleListWidth + leftList2Width, y + h - bottomRowHeight - bottomRow2Height, rightList2Width, bottomRow2Height), new Rectangle(137, 120, 9, 10), panelColor);
		sb.Draw(tex, new Rectangle(x + w - sideListWidth, y + h - bottomRowHeight - bottomRow2Height, sideListWidth, bottomRow2Height), new Rectangle(147, 120, sideListWidth, 10), panelColor);

		// Bottom row
		sb.Draw(tex, new Rectangle(x, y + h - bottomRowHeight, sideListWidth, bottomRowHeight), new Rectangle(0, 131, sideListWidth, bottomRowHeight), panelColor);
		sb.Draw(tex, new Rectangle(x + sideListWidth, y + h - bottomRowHeight, leftList2Width, bottomRowHeight), new Rectangle(41, 131, 9, bottomRowHeight), panelColor);
		sb.Draw(tex, new Rectangle(x + sideListWidth + leftList2Width, y + h - bottomRowHeight, middleListWidth, bottomRowHeight), new Rectangle(51, 131, middleListWidth, bottomRowHeight), panelColor);
		sb.Draw(tex, new Rectangle(x + sideListWidth + middleListWidth + leftList2Width, y + h - bottomRowHeight, rightList2Width, bottomRowHeight), new Rectangle(137, 131, 9, bottomRowHeight), panelColor);
		sb.Draw(tex, new Rectangle(x + w - sideListWidth, y + h - bottomRowHeight, sideListWidth, bottomRowHeight), new Rectangle(147, 131, sideListWidth, bottomRowHeight), panelColor);
	}
}
