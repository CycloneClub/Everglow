using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;
using Terraria.DataStructures;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	public class BloodGoldBayonet : StabbingSwordItem
	{
		// TODO:翻译
		// 命中敌人后有1/25的概率吸血,吸血量为造成伤害的30%
		// 命中的敌人未死之前,你的生命回复+2
		// 据不完全统计，多数吸血鬼并不会吸取史莱姆汁
		// 重击：以自身损失15血为代价,对被命中的敌人施加持续3s的嗜血符印,血金刺剑命中有符印的敌人造成回血
		public override void SetDefaults()
		{
			Item.damage = 11;
			Item.knockBack = 1.61f;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(0, 1, 2, 0);
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
