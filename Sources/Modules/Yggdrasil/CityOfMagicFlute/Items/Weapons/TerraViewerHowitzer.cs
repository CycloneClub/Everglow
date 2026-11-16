using Everglow.Yggdrasil.CityOfMagicFlute.Projectiles.Ranged;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.CityOfMagicFlute.Items.Weapons;

/// <summary>
/// 观世者榴弹——未来遗产
/// 思维具现化：左键使用速射机枪进行快速射击，每击中十次就会从屏幕外召唤一颗面板伤害90%的榴弹轰炸目标
/// 右键发射大范围的不会破坏图格的榴弹，爆炸时生成短时滞留的纳米云
/// 高爆榴弹需要三秒装填，榴弹轰炸每命中三次便会从天上降临高精度弹雨持续协同打击目标
/// 按住S+空格以使用冲击弹跳（小心榴弹破片误伤自己
/// "这玩意看起来不像现代的产物，而且我能感觉到这是我的思绪碎片之一..."
/// </summary>
public class TerraViewerHowitzer : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	public int ShootType = 0;
	public int RightClickCooling = 180;

	public override void SetDefaults()
	{
		Item.damage = 745;
		Item.width = 84;
		Item.height = 42;
		Item.value = 45500;
		Item.rare = ItemRarityID.Red;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.DamageType = DamageClass.Ranged;
		Item.noMelee = true;
		Item.knockBack = 5;
		Item.noUseGraphic = true;
		Item.crit = 4;
		Item.useAmmo = AmmoID.Bullet;
		Item.shootSpeed = 27;
		Item.shoot = ProjectileID.Bullet;
		Item.autoReuse = true;
		Item.useTime = 6;
		Item.useAnimation = 6;
	}

	public override bool AltFunctionUse(Player player) => true;

	public override bool CanUseItem(Player player)
	{
		if (player.altFunctionUse == 2)
		{
			Projectile.NewProjectile(player.GetSource_ItemUse(Item), Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<TerraViewerHowitzer_Proj>(), 0, Item.knockBack, player.whoAmI);
			return false;
		}
		return true;
	}

	public override void HoldItem(Player player) => player.ListenMouseWorld();

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		ShootType = type;
		if (player.ownedProjectileCounts[ModContent.ProjectileType<TerraViewerHowitzer_Proj>()] <= 0)
		{
			Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<TerraViewerHowitzer_Proj>(), damage, knockback, player.whoAmI);
		}

		return false;
	}

	public override void UpdateInventory(Player player)
	{
		if (RightClickCooling > 0)
		{
			RightClickCooling--;
		}
		else
		{
			RightClickCooling = 0;
		}
	}
}
