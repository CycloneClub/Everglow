using Everglow.Ocean.VFXs;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Ocean.Projectiles.Weapons;

public class TsunamiShark : ModProjectile
{
	public override string Texture => "Everglow/Ocean/Projectiles/Weapons/TsunamiShark/TsunamiShark_proj";

	public override void SetDefaults()
	{
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 100000;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Ranged;
	}

	private int useCount = 0;
	private int overridedamage;
	internal IEntitySource shootSource = null;

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

	private void Shoot()
	{
		Vector2 toMuzzle = new Vector2(15, -15);
		toMuzzle = toMuzzle.RotatedBy(Projectile.rotation);
		Player player = Main.player[Projectile.owner];
		Vector2 toMouse = Projectile.Center - player.MountedCenter;
		Vector2 velocity = Vector2.Normalize(toMouse) * 27;
		Item item = player.HeldItem;
		if (item.ModItem is Items.Weapons.TsunamiShark tsunamiS)
		{
			Vector2 random = new Vector2(0, Main.rand.NextFloat(1f)).RotatedByRandom(6.283);
			ScreenShaker Gsplayer = player.GetModPlayer<ScreenShaker>();
			Gsplayer.FlyCamPosition = new Vector2(0, 2).RotatedByRandom(6.283);
			SoundEngine.PlaySound(new SoundStyle("Everglow/Ocean/Sounds/WaterGun").WithVolumeScale(0.8f), Projectile.Center);

			Projectile p = Projectile.NewProjectileDirect(
				shootSource,
				Projectile.Center + toMuzzle + random,
				velocity,
				ModContent.ProjectileType<TsunamiShark_bullet>(),
				overridedamage == -1 ? item.damage : overridedamage,
				item.knockBack,
				player.whoAmI);
			p.CritChance = (int)(item.crit + player.GetCritChance(DamageClass.Generic));

			float rot = velocity.ToRotation();
			// TODO:子弹伤害校正，要求和被消耗的弹药种类挂钩
			Projectile.NewProjectile(
				shootSource,
				Projectile.Center + toMuzzle * 1.5f + velocity * 2.2f + random,
				Vector2.Zero,
				ModContent.ProjectileType<TsunamiShark_flame>(),
				overridedamage == -1 ? item.damage : overridedamage,
				item.knockBack,
				player.whoAmI,
				0.36f,
				rot);
		}
		useCount++;
		if (useCount == 12)
		{
			if (player.ownedProjectileCounts[ModContent.ProjectileType<TsunamiShark_missile>()] < 20)
			{
				Projectile.NewProjectileDirect(shootSource, Projectile.Center + toMuzzle, velocity.RotatedBy(-Main.rand.NextFloat(-0.2f, 0.4f) * player.direction) * 2.4f, ModContent.ProjectileType<TsunamiShark_missile>(), (int)((overridedamage == -1 ? item.damage : overridedamage) * 3.64f), item.knockBack, player.whoAmI, Main.rand.NextFloat(0.6f, 1.4f));
			}
			useCount = 0;
		}
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		var tsunamiS = player.HeldItem.ModItem as Items.Weapons.TsunamiShark;
		if (tsunamiS == null)
		{
			Projectile.Kill();
		}

		Vector2 toMouse = Main.MouseWorld - player.MountedCenter;
		toMouse = Vector2.Normalize(toMouse);
		if (player.controlUseItem)
		{
			Projectile.rotation = (float)(Math.Atan2(toMouse.Y, toMouse.X) + Math.PI * 0.25);
			Projectile.Center = player.MountedCenter + Vector2.Normalize(toMouse) * 6;
			if (Main.mouseRight)
			{
				if (tsunamiS.MarkedTarget != null)
				{
					if (Collision.CanHit(tsunamiS.MarkedTarget, Projectile))
					{
						Vector2 toTarget = tsunamiS.MarkedTarget.Center + tsunamiS.MarkedTarget.velocity - player.MountedCenter;
						toTarget = Vector2.Normalize(toTarget);
						Projectile.rotation = (float)(Math.Atan2(toTarget.Y, toTarget.X) + Math.PI * 0.25);
						Projectile.Center = player.MountedCenter + Vector2.Normalize(toTarget) * 6;
					}
				}
			}
			Projectile.velocity *= 0;
			if (Projectile.timeLeft % player.HeldItem.useTime == 0)
			{
				Shoot();
			}
		}
		else
		{
			Projectile.Kill();
		}
	}

	public void GenerateVFXKill(int Frequency)
	{
		float mulVelocity = 0.6f;
		for (int g = 0; g < Frequency; g++)
		{
			Vector2 afterVelocity = new Vector2(0, 2f).RotatedBy(g / (float)Frequency * 2 * Math.PI);
			var wave = new WaveSprayDust
			{
				velocity = afterVelocity * mulVelocity + Projectile.velocity.SafeNormalize(new Vector2(0, -1)),
				Active = true,
				Visible = true,
				position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) - Projectile.velocity * Main.rand.NextFloat(3f, 14f),
				maxTime = Main.rand.Next(21, 32),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0, Main.rand.NextFloat(6f, 12f) },
			};
			Ins.VFXManager.Add(wave);
		}
		mulVelocity = 0.4f;
		for (int g = 0; g < Frequency; g++)
		{
			Vector2 afterVelocity = new Vector2(0, 2f).RotatedBy(g / (float)Frequency * 2 * Math.PI);
			var wave = new WaveSprayDust
			{
				velocity = afterVelocity * mulVelocity + Projectile.velocity.SafeNormalize(new Vector2(0, -1)),
				Active = true,
				Visible = true,
				position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) - Projectile.velocity * Main.rand.NextFloat(3f, 14f),
				maxTime = Main.rand.Next(21, 32),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0, Main.rand.NextFloat(6f, 12f) },
			};
			Ins.VFXManager.Add(wave);
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}

	public override void PostDraw(Color lightColor)
	{
		Player player = Main.player[Projectile.owner];
		player.heldProj = Projectile.whoAmI;
		Vector2 toMouse = Projectile.Center - player.MountedCenter;
		if (player.controlUseItem)
		{
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (float)(Math.Atan2(toMouse.Y, toMouse.X) - Math.PI / 2d));
		}

		Texture2D texMain = ModAsset.TsunamiShark_proj.Value;
		Texture2D texMainGlow = ModAsset.TsunamiShark_proj_glow.Value;
		SpriteEffects se = SpriteEffects.None;
		Vector2 origin = new Vector2(texMain.Size().X * 0.3f, texMain.Size().Y * 0.6f);
		if (Projectile.Center.X < player.Center.X)
		{
			se = SpriteEffects.FlipVertically;
			player.direction = -1;
		}
		else
		{
			origin = new Vector2(texMain.Size().X * 0.3f, texMain.Size().Y * 0.4f);
			player.direction = 1;
		}
		Vector2 random = new Vector2(0, Main.rand.NextFloat(1)).RotatedByRandom(6.283);
		var offset = new Vector2(0, -5);
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition + offset - random, null, lightColor, Projectile.rotation - (float)(Math.PI * 0.25), origin, 1f, se, 0);
		Main.spriteBatch.Draw(texMainGlow, Projectile.Center - Main.screenPosition + offset - random, null, new Color(1f, 1f, 1f, 0), Projectile.rotation - (float)(Math.PI * 0.25), origin, 1f, se, 0);

		Texture2D texMark = ModAsset.TsunamiShark_mark.Value;
		var tsunamiS = player.HeldItem.ModItem as Items.Weapons.TsunamiShark;
		if (tsunamiS != null)
		{
			if (tsunamiS.MarkedTarget != null)
			{
				if (tsunamiS.MarkedTarget.active)
				{
					Main.spriteBatch.Draw(texMark, tsunamiS.MarkedTarget.Center - Main.screenPosition, null, new Color(105, 105, 105, 0), 0, texMark.Size() / 2f, 1f, SpriteEffects.None, 0);
				}
			}
		}
		else
		{
			Projectile.Kill();
		}
	}
}
