using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;
using Terraria.DataStructures;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	public class BloodGoldBayonet : StabbingSwordItem
	{
		public override void SetDefaults()
		{
			Item.damage = 18;
			Item.knockBack = 1.61f;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(0, 1, 35, 0);
			Item.shoot = ModContent.ProjectileType<BloodGoldBayonet_Pro>();
			PowerfulStabDamageFlat = 4f;
			PowerfulStabProj = ModContent.ProjectileType<BloodGoldBayonet_Pro_Stab>();
			base.SetDefaults();
		}

		public override bool AltFunctionUse(Player player)
		{
			if (CurrentPowerfulStabCD > 0)
			{
				return false;
			}

			if (!player.GetModPlayer<StabbingSwordStaminaPlayer>().CheckStamina(StaminaCost * 45))
			{
				return false;
			}

			foreach (Projectile proj in Main.projectile)
			{
				if (proj.owner == player.whoAmI && proj.timeLeft > 1 && proj.type == PowerfulStabProj)
				{
					return false;
				}
			}
			if (player.statLife <= 15)
			{
				return false;
			}
			player.Hurt(PlayerDeathReason.ByPlayerItem(0, Item), 15, 0, false, true, false, 0);
			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe().
				AddIngredient(ItemID.CrimtaneBar, 10).
				AddIngredient(ItemID.TissueSample, 5).
				AddTile(TileID.Anvils).
				Register();
			base.AddRecipes();
		}
	}
}
