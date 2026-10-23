using Everglow.Commons.Mechanics.Cooldown;

namespace Everglow.Commons.Templates.Weapons.StabbingSwords;

public class StabbingSwordRecoveryCooldown : CooldownBase
{
	public static new string ID => "StabbingSwordRecoveryCooldown";

	public override string TypeID => ID;

	// The stamina player advances this only while idle and below full stamina.
	public override bool AutomaticTick => false;

	public override bool CanTickDown => true;

	public override bool PersistentCooldown => true;

	public override bool CooldownNoSave => true;

	public override bool IsVisible => false;

	public override Texture2D Texture => ModAsset.Empty.Value;
}
