using Everglow.Ocean.Common;
using Terraria.DataStructures;

namespace Everglow.Ocean.Items.Weapons;

public class RampageShark : ModItem
{
	//暴走鲨
	//狂热度C：每秒获得2点，最高不超过16，有以下效果
	//武器精准度随着C升高而下降
	//有一定概率以散弹的形式同时打出很多子弹，概率为(C% + 33%暴击率)，其中暴击率部份只计算100%以内的部分
	//75%的概率不消耗弹药
	//停止使用时，狂热度未清零前会以每秒3点下降，此期间无法使用
	//击退力加算40%C;
	//C满值是攻速为200%
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	//暴走鲨
	//狂热度C：每秒获得2点，最高不超过16，有以下效果
	//武器精准度随着C升高而下降
	//有一定概率以散弹的形式同时打出很多子弹，概率为(C% + 33%暴击率)，其中暴击率部份只计算100%以内的部分
	//75%的概率不消耗弹药
	//停止使用时，狂热度未清零前会以每秒3点下降，此期间无法使用
	//击退力加算40%C;
	//C满值是攻速为200%
	public float CrazyValue = 0; //C
	public int ShootType = 0;
	public override void SetDefaults()
	{
		Item.damage = 88;
		Item.width = 72;
		Item.height = 34;
		Item.value = 60845;
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

	public override void UpdateInventory(Player player)
	{
		if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Weapons.RampageShark>()] <= 0 || player.HeldItem != Item)
		{
			if (CrazyValue > 0)
				CrazyValue -= 1 / 20f;
			else
			{
				CrazyValue = 0;
			}
		}
	}
	public override void HoldItem(Player player)
	{
		if (player.controlUseItem && player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Weapons.RampageShark>()] > 0)
		{
			if (CrazyValue < 16)
				CrazyValue += 1 / 30f;
			else
			{
				CrazyValue = 16;
			}
		}
	}
	public override bool CanUseItem(Player player)
	{
		if (player.controlUseItem)
			return true;
		else
		{
			return CrazyValue == 0;
		}
	}
	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		ShootType = type;
		if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Weapons.RampageShark>()] <= 0 && CrazyValue == 0)
			Projectile.NewProjectile(Item.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<Projectiles.Weapons.RampageShark>(), damage, knockback, player.whoAmI);
		if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Weapons.RampageShark>()] <= 0 && CrazyValue > 0)
			//TODO:翻译
			CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y - 40, player.width, player.height), Color.Orange, "Overheating, please wait for" + (CrazyValue / 3f).ToString() + "s");
		return false;
	}
	public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		Texture2D texMainG = OceanContent.QuickTexture("Projectiles/Weapons/RampageShark/RampageShark_glow");
		Texture2D TexEye = OceanContent.QuickTexture("Projectiles/Weapons/RampageShark/RampageShark_redEye");
		float glow = CrazyValue / 16f;
		spriteBatch.Draw(texMainG, position, frame, new Color(glow, glow * 0.2f, glow * 0.2f, 0), 0, origin, scale, SpriteEffects.None, 0);
		if (CrazyValue >= 15 && CrazyValue < 16)
		{
			float progress = CrazyValue - 15f;
			Main.spriteBatch.Draw(TexEye, position, frame, new Color(progress, progress, progress, progress), 0, origin, scale, SpriteEffects.None, 0);
		}
		else if (CrazyValue >= 16)
		{
			Main.spriteBatch.Draw(TexEye, position, frame, new Color(1f, 1f, 1f, 1f), 0, origin, scale, SpriteEffects.None, 0);
		}
	}
	public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
	{
		Texture2D texMainG = OceanContent.QuickTexture("Projectiles/Weapons/RampageShark/RampageShark_glow");
		Texture2D TexEye = OceanContent.QuickTexture("Projectiles/Weapons/RampageShark/RampageShark_redEye");
		float glow = CrazyValue / 16f;
		spriteBatch.Draw(texMainG, Item.Center - Main.screenPosition, null, new Color(glow, glow * 0.2f, glow * 0.2f, 0), rotation, texMainG.Size() / 2f, scale, SpriteEffects.None, rotation);
		if (CrazyValue >= 15 && CrazyValue < 16)
		{
			float progress = CrazyValue - 15f;
			Main.spriteBatch.Draw(TexEye, Item.Center - Main.screenPosition, null, new Color(progress, progress, progress, progress), rotation, texMainG.Size() / 2f, scale, SpriteEffects.None, 0);

		}
		else if (CrazyValue >= 16)
		{
			Main.spriteBatch.Draw(TexEye, Item.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 1f), rotation, texMainG.Size() / 2f, scale, SpriteEffects.None, 0);
		}
		if (!Main.gamePaused)
		{
			if (CrazyValue > 0)
				CrazyValue -= 1 / 20f;
			else
			{
				CrazyValue = 0;
			}
		}
	}

	public override bool CanConsumeAmmo(Item ammo, Player player)
	{
		return Main.rand.NextBool(4);
	}
	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Megashark, 1)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}
}
