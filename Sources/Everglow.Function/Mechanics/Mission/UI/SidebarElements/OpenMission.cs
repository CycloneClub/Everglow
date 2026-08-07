using Everglow.Commons.Mechanics.Mission.UI;
using Everglow.Commons.UI.UIContainers.Sidebar.SidebarElements;

namespace Everglow.Commons.Mechanics.Mission.UI.SidebarElements;

internal class OpenMission : ISidebarElement
{
	public Texture2D Icon => ModAsset.OpenMission.Value;

	public string Tooltip => "打开任务面板";

	public bool Visible => true;

	public void Invoke()
	{
		if (MissionContainer.Instance.IsVisible)
		{
			MissionContainer.Instance.Close();
		}
		else
		{
			MissionContainer.Instance.Show();
		}
	}
}
