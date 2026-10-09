using Everglow.PlantAndFarm.Items.Accessories;

namespace Everglow.PlantAndFarm.Items.Materials;

public class PAFPlayer : ModPlayer
{
	public bool FlowerBrochure;
	public ThreeColorCrown ThreeColorCrown;

	public override void ResetEffects()
	{
		FlowerBrochure = false;
		ThreeColorCrown = null;
	}

	public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
	{
		ThreeColorCrown?.Trigger();
	}

	public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
	{
		ThreeColorCrown?.Trigger();
	}
}
