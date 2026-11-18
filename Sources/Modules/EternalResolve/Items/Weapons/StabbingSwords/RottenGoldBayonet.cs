using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	public class RottenGoldBayonet : StabbingSwordItem
	{
		internal int specialDelay = 0;

		public override void SetDefaults()
		{
			Item.damage = 13;
			Item.knockBack = 1.52f;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(0, 1, 35, 0);
			Item.shoot = ModContent.ProjectileType<RottenGoldBayonet_Pro>();
			PowerfulStabDamageFlat = 4f;
			PowerfulStabProj = ModContent.ProjectileType<RottenGoldBayonet_Pro_Stab>();
			base.SetDefaults();
		}

		public override void AddRecipes()
		{
			CreateRecipe().
				AddIngredient(ItemID.DemoniteBar, 10).
				AddIngredient(ItemID.ShadowScale, 5).
				AddTile(TileID.Anvils).
				Register();
			base.AddRecipes();
		}

		public override bool AltFunctionUse(Player player)
		{
			return NPC.downedBoss1 && base.AltFunctionUse(player);
		}

		public override void UpdateInventory(Player player)
		{
			if (specialDelay > 0)
			{
				specialDelay--;
			}
			base.UpdateInventory(player);
		}
	}
}
