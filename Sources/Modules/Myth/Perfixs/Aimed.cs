namespace Everglow.Myth.Perfixs;

/// <summary>
/// 功能尚未完善
/// </summary>
public class Aimed : ModPrefix
{
	public override PrefixCategory Category => PrefixCategory.Accessory;

	public override void ModifyValue(ref float valueMult)
	{
		valueMult = 0.3225f;
	}

	public override void Apply(Item item)
	{
		item.FindOwner(item.whoAmI);
		base.Apply(item);
	}

	public override float RollChance(Item item)
	{
		return 0f;
	}
}
