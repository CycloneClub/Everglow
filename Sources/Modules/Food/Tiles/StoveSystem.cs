using Everglow.Food.UI;
using Terraria.UI;

namespace Everglow.Food.Tiles;

public class StoveSystem : ModSystem
{
	public StoveUIManager StoveSystemUI;

	public bool Active = true;

	public override void Load()
	{
		StoveSystemUI = new StoveUIManager("StoveSystemInterface", InterfaceScaleType.UI);
	}

	public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
	{
		if (!Active)
		{
			return;
		}
		int mouseItemIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
		if (mouseItemIndex != -1)
		{
			layers.Insert(mouseItemIndex, StoveSystemUI);
		}
	}

	public delegate void StoveSystemDraw();
}
