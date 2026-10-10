namespace Everglow.Myth.Perfixs;

/// <summary>
/// 功能尚未完善
/// </summary>
public class Accurate : ModPrefix
{
	public override PrefixCategory Category => PrefixCategory.Accessory;

	public override void ModifyValue(ref float valueMult)
	{
		valueMult = 0.1025f;
	}

	public override void Apply(Item item)
	{
		if (Main.netMode != NetmodeID.SinglePlayer)
		{
			foreach (WorldItem worldItem in Main.item)
			{
				if (worldItem.active && ReferenceEquals(worldItem.inner, item))
				{
					worldItem.FindOwner();
					break;
				}
			}
		}
	}

	public override float RollChance(Item item)
	{
		return 0f;
	}
}
