using Everglow.Commons.Mechanics.Quest.UI;
using Everglow.Commons.UI.UIContainers.Sidebar.SidebarElements;

namespace Everglow.Commons.Mechanics.Quest.UI.SidebarElements;

internal class OpenQuest : ISidebarElement
{
	public Texture2D Icon => ModAsset.OpenQuest.Value;

	public string Tooltip => "打开任务面板";

	public bool Visible => true;

	public void Invoke()
	{
		if (QuestContainer.Instance.IsVisible)
		{
			QuestContainer.Instance.Close();
		}
		else
		{
			QuestContainer.Instance.Show();
		}
	}
}
