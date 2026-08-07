namespace Everglow.Commons.Mechanics.Mission.Hooks;

public class MissionGlobalItem : GlobalItem
{
	/// <summary>
	/// This event is marked as obsoleted for reasons:
	/// <br/>1. The <see cref="GlobalItem.UseItem(Item, Player)"/> hook will be called every frame during item using.
	/// <br/>2. In vanilla code, <see cref="Player.itemTime"/> will be keep at 0 when item is being used.
	/// <br/>3. There're also multiple different behaviors of <see cref="Player.itemTime"/> controlled by TML.
	/// <br/>4. The above means no native symbol can be used to represent item is used once precisely.
	/// </summary>
	[Obsolete("This event is broken, don't use it.", true)]
	public static event Action<Item> PlayerSide_OnUseItemEvent; // TODO: Find a symbol representing item use.

	public static event Action<Item> PlayerSide_OnConsumeItemEvent;

	public static event Action<Item, Player> WorldSide_OnItemConsumed;

	public override bool? UseItem(Item item, Player player)
	{
		if (player.whoAmI == Main.myPlayer)
		{
			PlayerSide_OnUseItemEvent?.Invoke(item);
		}

		return null;
	}

	public override void OnConsumeItem(Item item, Player player)
	{
		PlayerSide_OnConsumeItemEvent?.Invoke(item);

		WorldSide_OnItemConsumed?.Invoke(item, player);
	}

	public override void OnConsumeAmmo(Item weapon, Item ammo, Player player)
	{
		if (player.whoAmI == Main.myPlayer)
		{
			PlayerSide_OnConsumeItemEvent?.Invoke(ammo);

			WorldSide_OnItemConsumed?.Invoke(ammo, player);
		}
	}
}
