using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	public class EternalNight : StabbingSwordItem
	{
		public override void SetDefaults()
		{
			Item.damage = 32;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.buyPrice(0, 20, 0, 0);
			Item.shoot = ModContent.ProjectileType<EternalNight_Pro>();
			PowerfulStabDamageFlat = 4f;
			PowerfulStabProj = ModContent.ProjectileType<EternalNight_Pro_Stab>();
			base.SetDefaults();
		}

		public override void AddRecipes()
		{
			int[] jungleSwords = [ModContent.ItemType<VegetationBayonet>(), ModContent.ItemType<BlossomThorn>()];
			int[] evilSwords = [ModContent.ItemType<RottenGoldBayonet>(), ModContent.ItemType<BloodGoldBayonet>()];
			foreach (int jungleSword in jungleSwords)
			{
				foreach (int evilSword in evilSwords)
				{
					CreateRecipe()
						.AddIngredient(jungleSword)
						.AddIngredient(evilSword)
						.AddIngredient<PrisonFireBayonet>()
						.AddIngredient<VertebralSpur>()
						.AddTile(TileID.DemonAltar)
						.Register();
				}
			}
		}
	}
}
