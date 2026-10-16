using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	public class SwordfishBeak : StabbingSwordItem
	{
		// TODO:长喙剑鱼\n对于水里的敌人造成伤害下降40%,但是潮湿状态下攻击范围扩大50%,且体力消耗降低35%,对于干燥的敌人伤害提高40%
		public override void SetDefaults()
		{
			Item.damage = 14;
			Item.knockBack = 1.22f;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.sellPrice(0, 0, 50, 0);
			Item.shoot = ModContent.ProjectileType<SwordfishBeak_Pro>();
			PowerfulStabDamageFlat = 4f;
			StaminaCost = 0.75f;
			PowerfulStabProj = ModContent.ProjectileType<SwordfishBeak_Pro_Stab>();
			base.SetDefaults();
		}

		public override void UpdateVanitySet(Player player)
		{
			if (player.wet)
			{
				StaminaCost = 0.75f;
			}
			else
			{
				StaminaCost = 0.75f;
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Swordfish)
				.AddTile(TileID.Sawmill)
				.Register();
		}
	}
}
