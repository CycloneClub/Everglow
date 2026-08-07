using Everglow.Commons.UI.UIContainers.Sidebar.SidebarElements;
using SubworldLibrary;

namespace Everglow.SubSpace;

public class GetOutOfTheRoom : ISidebarElement
{
	public Texture2D Icon => ModAsset.GetOutOfTheRoom.Value;

	public string Tooltip => "离开房间";

	public void Invoke() => RoomManager.ExitALevelOfRoom();

	public bool IsVisible() => SubworldSystem.IsActive<RoomWorld>();
}
