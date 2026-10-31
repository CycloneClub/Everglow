using Everglow.Commons.Mechanics.Cooldown;

namespace Everglow.Commons.TileHelper;

public class HangingTileSwitchCooldown : CooldownBase
{
	public static new string ID => "HangingTileSwitchCooldown";

	public override string TypeID => ID;

	// Keep the switch boundary immediately before the player's grasp check.
	public override bool AutomaticTick => false;

	public override bool CanTickDown => true;

	public override bool PersistentCooldown => true;

	public override bool CooldownNoSave => true;

	public override bool IsVisible => false;

	public override Texture2D Texture => ModAsset.Empty.Value;
}
