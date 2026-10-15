namespace Everglow.Commons.UI.UIContainers.Sidebar.SidebarElements;

public class SlashAllTheItem : ISidebarElement
{
	public Texture2D Icon => ModAsset.SlashAllTheItem.Value;

	public string Tooltip => "清理背包";

	public void Invoke()
	{
		for (int i = 0; i < 50; i++)
		{
			var item = Main.LocalPlayer.inventory[i];
			if (!item.favorited)
			{
				item.SetDefaults();
			}
		}
		Main.NewText("已清除背包");
	}
}
