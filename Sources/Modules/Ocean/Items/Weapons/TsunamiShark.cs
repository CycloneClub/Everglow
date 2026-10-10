using Everglow.Myth.TheFirefly.Projectiles;
using Everglow.Ocean.Projectiles.Weapons;
using Terraria.DataStructures;

namespace Everglow.Ocean.Items.Weapons;

public class TsunamiShark : ModItem
{
	// 海啸银鲨
	// 子弹特效变成海蓝色，覆盖原有的子弹效果，但是保留子弹属性
	// 右键标记距离鼠标最近的怪物，如果有多个怪物的碰撞箱已经包含在鼠标内，优先标记Boss，其次是剩余血量多的怪物
	// 每20次攻击释放鲨鱼能量弹幕，可以穿墙，追踪被标记的敌人。击中被标记的目标或者穿透十次之后该弹幕爆炸造成范围伤害，目标在被击中前死亡则导致弹幕无法穿墙且在任意一次命中之后爆炸
	// 50%的概率不消耗弹药
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	// 海啸银鲨
	// 子弹特效变成海蓝色，覆盖原有的子弹效果，但是保留子弹属性
	// 右键标记距离鼠标最近的怪物，如果有多个怪物的碰撞箱已经包含在鼠标内，优先标记Boss，其次是剩余血量多的怪物
	// 每20次攻击释放鲨鱼能量弹幕，可以穿墙，追踪被标记的敌人。击中被标记的目标或者穿透十次之后该弹幕爆炸造成范围伤害，目标在被击中前死亡则导致弹幕无法穿墙且在任意一次命中之后爆炸
	// 50%的概率不消耗弹药
	public int ShootType = 0;

	public override void SetDefaults()
	{
		Item.damage = 88;
		Item.width = 86;
		Item.height = 46;
		Item.value = 45072;
		Item.rare = ItemRarityID.Yellow;
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

	internal NPC MarkedTarget = null;

	public override bool AltFunctionUse(Player player)
	{
		return true;
	}

	public override void UpdateInventory(Player player)
	{
		if (MarkedTarget != null)
		{
			if (!MarkedTarget.active || MarkedTarget.life <= 0 || MarkedTarget.dontTakeDamage || MarkedTarget.friendly || !MarkedTarget.CanBeChasedBy())
			{
				MarkedTarget = null;
			}
			if (player.HeldItem != Item)
			{
				MarkedTarget = null;
			}
		}
	}

	public override void HoldItem(Player player)
	{
		if (player.controlUseItem && player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Weapons.TsunamiShark>()] > 0)
		{
		}
	}

	public override bool CanUseItem(Player player)
	{
		if (player.altFunctionUse == 2)
		{
			Projectile.NewProjectile(player.GetSource_ItemUse(Item), Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<TsunamiShark_marking>(), 0, Item.knockBack, player.whoAmI);
			return false;
		}
		return true;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		ShootType = type;
		if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Weapons.TsunamiShark>()] <= 0)
		{
			Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Projectiles.Weapons.TsunamiShark>(), damage, knockback, player.whoAmI);
		}

		return false;
	}

	public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		Texture2D texMark = ModAsset.TsunamiShark_mark.Value;
		if (MarkedTarget != null)
		{
			if (MarkedTarget.active)
			{
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
				Main.spriteBatch.Draw(texMark, MarkedTarget.Center - Main.screenPosition, null, new Color(105, 105, 105, 0), 0, texMark.Size() / 2f, 1f, SpriteEffects.None, 0);
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
			}
		}
	}

	public override bool CanConsumeAmmo(Item ammo, Player player)
	{
		return Main.rand.NextBool(2);
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Megashark, 1)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}
}
