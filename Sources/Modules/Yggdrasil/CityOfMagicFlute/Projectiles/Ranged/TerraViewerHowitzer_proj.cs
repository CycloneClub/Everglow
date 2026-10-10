using Everglow.Yggdrasil.CityOfMagicFlute.Items.Weapons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.CityOfMagicFlute.Projectiles.Ranged;

public class TerraViewerHowitzer_proj : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedProjectiles;

	public override string Texture => ModAsset.TerraViewerHowitzer_proj_Mod;

	private Vector2 OwnerMouseWorld => Main.player[Projectile.owner].MouseWorld();

	public override void PostAI()
	{
		bool heldByOwner = Projectile.owner >= 0 && Projectile.owner < Main.maxPlayers
			&& Main.player[Projectile.owner].heldProj == Projectile.whoAmI;
		Projectile.drawLayer = heldByOwner ? ProjectileDrawLayerID.HeldProj : ProjectileDrawLayerID.Default;
		Projectile.usesOwnerLight = heldByOwner;
	}

	public override void SetDefaults()
	{
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 1000000;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.drawLayer = ProjectileDrawLayerID.HeldProj;
		Projectile.usesOwnerLight = true;
	}

	public int useCount = 0;
	public int overridedamage;
	public IEntitySource shootSource = null;

	public override void OnSpawn(IEntitySource source)
	{
		shootSource = source;
		if (source is EntitySource_ItemUse_WithAmmo withammo && withammo.Entity is Player owner)
		{
			var modifer = owner.GetTotalDamage(withammo.Item.DamageType);
			modifer.CombineWith(owner.bulletDamage);
			CombinedHooks.ModifyWeaponDamage(owner, withammo.Item, ref modifer);
			overridedamage = Math.Max(
				1,
				(int)modifer.ApplyTo(withammo.Item.damage + ContentSamples.ItemsByType[withammo.AmmoItemIdUsed].damage));
		}
		else
		{
			overridedamage = -1;
		}
	}

	private void Shoot(float phi = 0)
	{
		var toMuzzle = new Vector2(-45, 43 + MathF.Sin((float)Main.time * 0.2f + phi) * 15);
		toMuzzle = toMuzzle.RotatedBy(Projectile.rotation);
		Player player = Main.player[Projectile.owner];
		Vector2 toMouse = Projectile.Center - player.MountedCenter;
		toMouse = Vector2.Normalize(toMouse);
		var offset = new Vector2(0, -25) + toMouse * 60f;
		Vector2 velocity = Vector2.Normalize(toMouse) * 27;
		Item item = player.HeldItem;
		if (item.ModItem is TerraViewerHowitzer)
		{
			Vector2 random = new Vector2(0, Main.rand.NextFloat(1f)).RotatedByRandom(6.283);
			ScreenShaker Gsplayer = player.GetModPlayer<ScreenShaker>();
			Gsplayer.FlyCamPosition = new Vector2(0, 2).RotatedByRandom(6.283);
			var tVH = item.ModItem as TerraViewerHowitzer;
			var p = Projectile.NewProjectileDirect(
				shootSource,
				Projectile.Center + offset + toMuzzle + random,
				velocity,
				tVH.ShootType,
				(int)((overridedamage == -1 ? item.damage : overridedamage) * 0.1f),
				item.knockBack,
				player.whoAmI);
			p.CritChance = (int)(item.crit + player.GetCritChance(DamageClass.Generic));

			float rot = velocity.ToRotation();

			// TODO:子弹伤害校正，要求和被消耗的弹药种类挂钩
			Projectile.NewProjectile(
				shootSource,
				Projectile.Center + offset + toMuzzle * 1.5f + velocity * 2.2f + random,
				Vector2.Zero,
				ModContent.ProjectileType<TerraViewerHowitzer_proj_flame>(),
				overridedamage == -1 ? item.damage : overridedamage,
				item.knockBack,
				player.whoAmI,
				0.36f,
				rot);
		}
		useCount++;
		if (useCount == 10)
		{
			Vector2 summonPos = Projectile.Center + new Vector2(Main.rand.NextFloat(-500, 500), -1500);
			Projectile.NewProjectile(
			shootSource,
			summonPos,
			Vector2.Normalize(OwnerMouseWorld - summonPos) * 60,
			ModContent.ProjectileType<TerraViewerHowitzer_grenade_fall>(),
			(int)((overridedamage == -1 ? item.damage : overridedamage) * 0.9f),
			item.knockBack,
			player.whoAmI,
			1);
			useCount = 0;
		}
	}

	public override void AI()
	{
		Main.player[Projectile.owner].heldProj = Projectile.whoAmI;
		Player player = Main.player[Projectile.owner];
		var tsunamiS = player.HeldItem.ModItem as TerraViewerHowitzer;
		if (tsunamiS == null)
		{
			Projectile.Kill();
			return;
		}
		int controlCount = 0;
		Vector2 toMouse = OwnerMouseWorld - player.MountedCenter;
		toMouse = Vector2.Normalize(toMouse);
		if (player.controlUseItem)
		{
			Projectile.rotation = (float)(Math.Atan2(toMouse.Y, toMouse.X) + Math.PI * 0.25);
			Projectile.Center = player.MountedCenter + Vector2.Normalize(toMouse) * 6;
			Projectile.velocity *= 0;
			if (Projectile.timeLeft % 3 == 0)
			{
				NextFramw();
			}
			if (Projectile.timeLeft % player.HeldItem.useTime == 0)
			{
				Shoot();
			}
			if (Projectile.timeLeft % player.HeldItem.useTime == player.HeldItem.useTime / 2)
			{
				Shoot(MathHelper.Pi);
			}
			controlCount++;
		}

		if (player.controlUseTile)
		{
			Projectile.rotation = (float)(Math.Atan2(toMouse.Y, toMouse.X) + Math.PI * 0.25);
			Projectile.Center = player.MountedCenter + Vector2.Normalize(toMouse) * 6;
			Projectile.velocity *= 0;
			if (Projectile.timeLeft % 3 == 0)
			{
				NextFramw();
			}
			if (tsunamiS.RightClickCooling <= 0)
			{
				tsunamiS.RightClickCooling = 180;
				var toMuzzle = new Vector2(5, -10);
				toMuzzle = toMuzzle.RotatedBy(Projectile.rotation);
				toMouse = Vector2.Normalize(toMouse);
				var offset = new Vector2(0, -25) + toMouse * 60f;
				Vector2 velocity = Vector2.Normalize(toMouse) * 60;
				Item item = player.HeldItem;
				if (item.ModItem is TerraViewerHowitzer)
				{
					var p = Projectile.NewProjectileDirect(
					shootSource,
					Projectile.Center + offset + toMuzzle,
					velocity,
					ModContent.ProjectileType<TerraViewerHowitzer_shoot>(),
					(int)((overridedamage == -1 ? item.damage : overridedamage) * 7.2f),
					item.knockBack,
					player.whoAmI,
					10);
					p.CritChance = (int)(item.crit + player.GetCritChance(DamageClass.Generic));
					float rot = velocity.ToRotation();

					// TODO:子弹伤害校正，要求和被消耗的弹药种类挂钩
					Projectile.NewProjectile(
					shootSource,
					Projectile.Center + offset + toMuzzle + velocity * 2.2f,
					Vector2.Zero,
					ModContent.ProjectileType<TerraViewerHowitzer_proj_flame_blue>(),
					overridedamage == -1 ? item.damage : overridedamage,
					item.knockBack,
					player.whoAmI,
					0.36f,
					rot);
				}
			}
			controlCount++;
		}
		if (controlCount == 0)
		{
			Projectile.Kill();
		}
	}

	public void NextFramw()
	{
		if (Projectile.frame < 9)
		{
			Projectile.frame++;
		}
		else
		{
			Projectile.frame = 4;
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		return false;
	}

	public override void PostDraw(Player player, Color lightColor)
	{
		player.heldProj = Projectile.whoAmI;
		Vector2 toMouse = Projectile.Center - player.MountedCenter;
		toMouse = toMouse.SafeNormalize(Vector2.zeroVector);
		if (player.controlUseItem)
		{
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (float)(Math.Atan2(toMouse.Y, toMouse.X) - Math.PI / 2d));
		}
		var tsunamiS = player.HeldItem.ModItem as TerraViewerHowitzer;
		if (tsunamiS == null)
		{
			Projectile.Kill();
			return;
		}

		Texture2D texMain = ModAsset.TerraViewerHowitzer_proj.Value;
		Texture2D texMainGlow = ModAsset.TerraViewerHowitzer_proj_glow.Value;
		Texture2D texPower = ModAsset.TerraViewerHowitzer_proj_power.Value;
		SpriteEffects se = SpriteEffects.None;
		var drawFrame = new Rectangle(0, Projectile.frame * 92, 242, 92);
		var origin = drawFrame.Size() * 0.5f;
		if (Projectile.Center.X < player.Center.X)
		{
			se = SpriteEffects.FlipVertically;
			player.direction = -1;
		}
		else
		{
			player.direction = 1;
		}
		Vector2 random = new Vector2(0, Main.rand.NextFloat(1)).RotatedByRandom(6.283);
		var offset = new Vector2(0, -25) + toMouse * 60f;

		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition + offset - random, drawFrame, lightColor, Projectile.rotation - (float)(Math.PI * 0.25), origin, 1f, se, 0);
		Main.spriteBatch.Draw(texMainGlow, Projectile.Center - Main.screenPosition + offset - random, drawFrame, new Color(1f, 1f, 1f, 0), Projectile.rotation - (float)(Math.PI * 0.25), origin, 1f, se, 0);
		if (tsunamiS.RightClickCooling < 160)
		{
			int framePower = (int)Math.Clamp((180 - tsunamiS.RightClickCooling) / 180f * 5, 0, 4);

			drawFrame = new Rectangle(0, framePower * 92, 242, 92);
			Main.spriteBatch.Draw(texPower, Projectile.Center - Main.screenPosition + offset - random, drawFrame, new Color(1f, 1f, 1f, 0), Projectile.rotation - (float)(Math.PI * 0.25), origin, 1f, se, 0);
		}
	}
}
