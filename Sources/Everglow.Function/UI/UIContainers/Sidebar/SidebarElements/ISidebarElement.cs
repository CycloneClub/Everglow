namespace Everglow.Commons.UI.UIContainers.Sidebar.SidebarElements;

public interface ISidebarElement
{
	/// <summary>
	/// 快捷栏元素的标识
	/// </summary>
	public Texture2D Icon { get; }

	/// <summary>
	/// 快捷栏元素的说明
	/// </summary>
	public string Tooltip { get => string.Empty; }

	/// <summary>
	/// 快捷栏元素被触发的事件
	/// </summary>
	public void Invoke();

	public bool IsVisible() => true;
}
