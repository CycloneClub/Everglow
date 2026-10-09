namespace Everglow.Minortopography.GiantPinetree.Items;
// TODO:翻译
// 任何攻击的暴击会召唤若干基础伤害为4的松针，算作魔法伤害
public class PearlOfCyan : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetDefaults()
	{
		Item.width = 42;
		Item.height = 42;
		Item.value = 5479;
		Item.accessory = true;
		Item.rare = ItemRarityID.Orange;
	}
	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.GetModPlayer<PearlOfCyanPlayer>().PearlOfCyanOpen = true;
	}
}
public class PearlOfCyanPlayer : ModPlayer
{
	internal bool PearlOfCyanOpen = false;
	public override void ResetEffects()
	{
		PearlOfCyanOpen = false;
	}
	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (PearlOfCyanOpen)
		{
			if (hit.Crit)
			{
				Item pearlOfCyan = null;
				foreach (Item item in Player.armor)
				{
					if (item.type == ModContent.ItemType<PearlOfCyan>())
					{
						pearlOfCyan = item;
					}
				}
				if (pearlOfCyan != null)
				{
					int count = Main.rand.Next(3, 6);
					for (int t = 0; t < count; t++)
					{
						Vector2 randomDirection = new Vector2(0, Main.rand.NextFloat(75f, 160f)).RotatedByRandom(6.283);
						if (!Collision.SolidCollision(target.Center + randomDirection, 0, 0))
						{
							Projectile.NewProjectile(Player.GetSource_Accessory(pearlOfCyan), target.Center + randomDirection, -randomDirection * 0.1f, ProjectileID.PineNeedleFriendly, (int)Player.GetDamage(DamageClass.Magic).ApplyTo(4), 0.1f, Player.whoAmI);
						}
					}
				}
			}
		}
	}
}
