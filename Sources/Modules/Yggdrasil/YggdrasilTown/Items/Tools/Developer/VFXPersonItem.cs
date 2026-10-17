using Everglow.Yggdrasil.YggdrasilTown.Travelers;
using Terraria.GameInput;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Tools.Developer;

public class VFXPersonItem : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.width = 16;
		Item.height = 16;
		Item.useTurn = true;
		Item.useAnimation = 15;
		Item.useTime = 15;
		Item.autoReuse = false;
		Item.useStyle = ItemUseStyleID.Swing;
	}

	public override bool CanUseItem(Player player) => CanControl(player);

	public override bool AltFunctionUse(Player player) => true;

	public override bool? UseItem(Player player)
	{
		if (CanControl(player))
		{
			if (player.altFunctionUse == 2)
			{
				TravelerControlPanel.Toggle();
			}
			else
			{
				TownTravelerSystem.ToggleMode(TravelerMode.NPC);
			}
		}
		return true;
	}

	public override void HoldItem(Player player)
	{
		if (CanControl(player) && PlayerInput.Triggers.JustPressed.MouseMiddle)
		{
			TownTravelerSystem.ToggleMode(TravelerMode.Actors);
		}
	}

	private static bool CanControl(Player player) => !Main.dedServ && player.whoAmI == Main.myPlayer
		&& !player.dead && !player.mouseInterface && !Main.playerInventory && !Main.blockInput && !Main.gamePaused
		&& !TravelerControlPanel.CapturesMouse;
}
