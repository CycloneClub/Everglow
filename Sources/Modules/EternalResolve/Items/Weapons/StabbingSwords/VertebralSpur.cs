using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	public class VertebralSpur : StabbingSwordItem
	{
		public override void SetDefaults()
		{
			Item.damage = 16;
			Item.knockBack = 1.37f;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(0, 0, 90, 0);
			Item.shoot = ModContent.ProjectileType<VertebralSpur_Pro>();
			PowerfulStabDamageFlat = 4f;
			PowerfulStabProj = ModContent.ProjectileType<VertebralSpur_Pro_Stab>();
			base.SetDefaults();
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Bone, 60)
				.AddIngredient(ItemID.Cobweb, 20)
				.AddCondition(Condition.DownedSkeletron)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}
