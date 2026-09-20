using Terraria.GameContent.Drawing;

namespace Everglow.Commons.TileHelper;

/// <summary>
/// 为物块提供一个每帧绘制60次的接口 <br/>
/// 注意：对solid物块不管用，这是给家具用的 <br/>
/// 相关: <see cref="TileFluentDrawManager"/>
/// </summary>
public interface ITileFluentlyDrawn
{
	/// <summary>
	/// 丝滑绘制
	/// </summary>
	/// <param name="screenPosition">屏幕坐标，这里不用考虑什么offRange了，绘制坐标直接pos-scrnPos就行</param>
	/// <param name="pos">物块坐标</param>
	/// <param name="spriteBatch">绘制用</param>
	/// <param name="tileDrawing">工具类，风速和摇晃有关的都得靠它</param>
	void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing);
}

public struct BasicDrawInfo
{
	public Vector2 DrawCenterPos;
	public SpriteBatch SpriteBatch;
	public TileDrawing TileDrawing;
}
