using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.Example.Projectiles;

namespace Everglow.Example.Items;

public class ExampleStabbingSword : StabbingSwordItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Examples;

	public override void SetCustomDefault()
	{
		StaminaCost += 0.3f;
		Item.damage = 3;
		Item.knockBack = 1f;
		Item.rare = ItemRarityID.White;
		Item.value = Item.sellPrice(0, 0, 12, 0);
		Item.shoot = ModContent.ProjectileType<ExampleStabbingSword_Pro>();
		PowerfulStabDamageFlat = 4f;
		PowerfulStabProj = ModContent.ProjectileType<ExampleStabbingSword_Pro_Stab>();
	}
}
