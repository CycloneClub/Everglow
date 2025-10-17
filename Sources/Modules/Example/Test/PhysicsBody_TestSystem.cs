namespace Everglow.Example.Test;

/// <summary>
/// Devs only.
/// </summary>
public class PhysicsBody_TestSystem : ModItem
{
	public override void SetDefaults()
	{
		Item.useTime = 21;
		Item.useAnimation = 21;
	}

	public int soundID = 0;

	public override void HoldItem(Player player)
	{
		if (Main.mouseLeft && Main.mouseLeftRelease)
		{
		}
	}
}