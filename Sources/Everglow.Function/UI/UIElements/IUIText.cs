namespace Everglow.Commons.UI.UIElements;

public interface IUIText
{
	/// <summary>
	/// 是否启用组件特殊尺寸计算方式
	/// </summary>
	bool CalculateSize { get; set; }

	/// <summary>
	/// 组件中心位置的X坐标
	/// </summary>
	BaseElement.PositionStyle? CenterX { get; set; }

	/// <summary>
	/// 组件中心位置的Y坐标
	/// </summary>
	BaseElement.PositionStyle? CenterY { get; set; }

	/// <summary>
	/// 绘制时的缩放大小，不改变部件碰撞箱，不改变绘制中心
	/// </summary>
	float Scale { get; set; }

	/// <summary>
	/// 显示的文本内容
	/// </summary>
	string Text { get; set; }

	/// <summary>
	/// 计算组件的位置
	/// </summary>
	void Calculation();
}
