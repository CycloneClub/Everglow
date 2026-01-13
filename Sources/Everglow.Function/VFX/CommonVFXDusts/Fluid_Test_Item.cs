using System;
using Everglow.Commons.VFX.CommonVFXDusts.Fluid_Smoke;

namespace Everglow.Commons.VFX.CommonVFXDusts;

public class Fluid_Test_Item : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 16;
		Item.height = 16;
		Item.useTurn = true;
		Item.useAnimation = 4;
		Item.useTime = 4;
		Item.autoReuse = false;
		Item.useStyle = ItemUseStyleID.Swing;
	}

	public int VFXCount = 0;

	public override void HoldItem(Player player)
	{
		if(Main.mouseLeft && Main.mouseLeftRelease && VFXCount == 0)
		{
			VFXCount++;
			var brush = new BrushCanvas()
			{
				Active = true,
				Visible = true,
				Position = Main.MouseWorld,
			};
			Ins.VFXManager.Add(brush);
		}
		if (Main.mouseRight && Main.mouseRightRelease)
		{
			VFXCount = 0;
		}
	}
}