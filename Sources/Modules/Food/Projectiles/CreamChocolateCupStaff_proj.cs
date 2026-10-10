using Everglow.Food.Items.Weapons;
using Terraria.DataStructures;

namespace Everglow.Food.Projectiles;

public class CreamChocolateCupStaff_proj : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 36;
		Projectile.height = 36;
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 360000;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Magic;
	}

	public float WeaponShake = 0;

	public override void OnSpawn(IEntitySource source)
	{
		Player player = Main.player[Projectile.owner];
		Vector2 mouseToPlayer = Main.MouseWorld - player.MountedCenter;
		mouseToPlayer = Vector2.Normalize(mouseToPlayer);
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, mouseToPlayer * 16f, ModContent.ProjectileType<CreamChocolateCup_ChocolateBars>(), Projectile.damage * 2, 0.4f, player.whoAmI);
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		player.heldProj = Projectile.whoAmI;

		Vector2 mouseToPlayer = Main.MouseWorld - player.MountedCenter;
		mouseToPlayer = Vector2.Normalize(mouseToPlayer);
		if (player.controlUseItem)
		{
			Projectile.rotation = (float)(Math.Atan2(mouseToPlayer.Y, mouseToPlayer.X) + Math.PI * 0.25);
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (float)(Math.Atan2(mouseToPlayer.Y, mouseToPlayer.X * player.gravDir) + Math.PI * 0.25) - MathF.PI * 0.75f + (player.gravDir == -1 ? MathHelper.Pi : 0));
			Projectile.Center = player.MountedCenter + Vector2.Normalize(mouseToPlayer).RotatedBy(WeaponShake / 0.8d) * (24f - WeaponShake * 8) + new Vector2(0, 0);
			Projectile.velocity *= 0;
			if (player.itemTime == 0)
			{
				if (player.ItemCheck_PayMana(player.HeldItem, true))
				{
					player.ItemCheck_ApplyManaRegenDelay(player.HeldItem);
					player.itemTime = player.itemTimeMax;
					Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, mouseToPlayer * 16f, ModContent.ProjectileType<CreamChocolateCup_ChocolateBars>(), Projectile.damage * 3, 0.4f, player.whoAmI);
				}
				else
				{
					Projectile.Kill();
				}
			}
		}
		if (!player.controlUseItem)
		{
			Projectile.Kill();
		}
		if (Projectile.Center.X < player.MountedCenter.X)
		{
			player.direction = -1;
		}
		else
		{
			player.direction = 1;
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

		var texMain = ModAsset.CreamChocolateCupStaff_cup.Value;

		Color drawColor = Lighting.GetColor((int)Projectile.Center.X / 16, (int)(Projectile.Center.Y / 16.0));
		SpriteEffects se = SpriteEffects.None;
		if (player.direction == -1)
		{
			se = SpriteEffects.FlipVertically;
		}

		float rot0 = Projectile.rotation - (float)(Math.PI * 0.25) + MathF.PI * 0.25f * player.direction;

		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition, null, drawColor, rot0, texMain.Size() / 2f, 1f, se, 0);
	}
}

public class CreamChocolateCupStaff_proj_rightClick : ModProjectile
{
	public override string Texture => "Everglow/Food/Projectiles/CreamChocolateCupStaff_proj";

	public override void SetDefaults()
	{
		Projectile.width = 36;
		Projectile.height = 36;
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 11;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Magic;
	}

	public float WeaponShake = 0;
	public Projectile CreamFlow;

	public override void OnSpawn(IEntitySource source)
	{
		Player player = Main.player[Projectile.owner];
		CreamFlow = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), player.Center, Vector2.zeroVector, ModContent.ProjectileType<CreamChocolateCup_CreamFlow>(), Projectile.damage, 0.4f, player.whoAmI);
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		player.heldProj = Projectile.whoAmI;
		player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathF.PI * 0.75f);

		Vector2 mouseToPlayer = Main.MouseWorld - player.MountedCenter;
		mouseToPlayer = Vector2.Normalize(mouseToPlayer);
		if (player.controlUseTile)
		{
			Projectile.rotation = (float)(Math.Atan2(mouseToPlayer.Y, mouseToPlayer.X) + Math.PI * 0.25);
			Projectile.Center = player.MountedCenter + Vector2.Normalize(mouseToPlayer).RotatedBy(WeaponShake / 0.8d) * (24f - WeaponShake * 8) + new Vector2(0, 0);
			Projectile.velocity *= 0;
			player.itemAnimation = 1;
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (float)(Math.Atan2(mouseToPlayer.Y, mouseToPlayer.X * player.gravDir) + Math.PI * 0.25) - MathF.PI * 0.75f + (player.gravDir == -1 ? MathHelper.Pi : 0));
			if (CreamFlow == null || CreamFlow.active == false || CreamFlow.type != ModContent.ProjectileType<CreamChocolateCup_CreamFlow>())
			{
				CreamFlow = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), player.Center, Vector2.zeroVector, ModContent.ProjectileType<CreamChocolateCup_CreamFlow>(), Projectile.damage, 0.4f, player.whoAmI);
			}
			else
			{
				CreamChocolateCup_CreamFlow cCCCF = CreamFlow.ModProjectile as CreamChocolateCup_CreamFlow;
				if (cCCCF != null)
				{
					cCCCF.Joints.Add(Vector2.zeroVector);
					cCCCF.JointVelocity.Add(Vector2.Normalize(mouseToPlayer) * 16f);
				}
				CreamFlow.Center = Projectile.Center - Vector2.Normalize(mouseToPlayer) * 16f;
				CreamFlow.timeLeft = 150;
			}
		}
		if (!player.controlUseTile)
		{
			Projectile.Kill();
		}
		if (Projectile.Center.X < player.MountedCenter.X)
		{
			player.direction = -1;
		}
		else
		{
			player.direction = 1;
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

		var texMain = ModAsset.CreamChocolateCupStaff_cupFront.Value;

		Color drawColor = Lighting.GetColor((int)Projectile.Center.X / 16, (int)(Projectile.Center.Y / 16.0));
		SpriteEffects se = SpriteEffects.None;
		if (player.direction == -1)
		{
			se = SpriteEffects.FlipVertically;
		}

		float rot0 = Projectile.rotation - (float)(Math.PI * 0.25) + MathF.PI * 0.25f * player.direction;

		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition, null, drawColor, rot0, texMain.Size() / 2f, 1f, se, 0);
	}
}

public class CreamChocolateCupStaff_proj_held : ModProjectile
{
	public override string Texture => "Everglow/Food/Projectiles/CreamChocolateCupStaff_proj";

	public override void SetDefaults()
	{
		Projectile.width = 36;
		Projectile.height = 36;
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 360000;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Magic;
	}

	public float Cooling = 0;

	public override void OnSpawn(IEntitySource source)
	{
		Cooling = 60;
	}

	public override void AI()
	{
		if (Cooling > 0)
		{
			Cooling--;
		}
		Player player = Main.player[Projectile.owner];
		player.heldProj = Projectile.whoAmI;
		player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, 0);

		Vector2 mouseToPlayer = new Vector2(50 * player.direction, -10 * player.gravDir);
		mouseToPlayer = Vector2.Normalize(mouseToPlayer);
		Projectile.rotation = (float)(Math.Atan2(mouseToPlayer.Y, mouseToPlayer.X) + Math.PI * 0.25) + MathF.Sin(player.bodyFrame.Y * 0.01f) * 0.15f;
		Projectile.Center = player.MountedCenter + new Vector2(0, 10 * player.gravDir);
		Projectile.velocity *= 0;
		if (player.ownedProjectileCounts[ModContent.ProjectileType<CreamChocolateCupStaff_proj_rightClick>()] + player.ownedProjectileCounts[ModContent.ProjectileType<CreamChocolateCupStaff_proj>()] > 0)
		{
			Projectile.Kill();
		}
		if (player.HeldItem.type != ModContent.ItemType<CreamChocolateCupStaff>())
		{
			Projectile.Kill();
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
		Vector2 v0 = Projectile.Center - player.MountedCenter;

		if (player.controlUseTile)
		{
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (float)(Math.Atan2(v0.Y, v0.X) - Math.PI / 2d));
		}

		var texMain = ModAsset.CreamChocolateCupStaff_cupFront.Value;

		Color drawColor = Lighting.GetColor((int)Projectile.Center.X / 16, (int)(Projectile.Center.Y / 16.0));
		SpriteEffects se = SpriteEffects.None;
		if (player.direction == -1)
		{
			se = SpriteEffects.FlipVertically;
		}

		float rot0 = Projectile.rotation - (float)(Math.PI * 0.25) + MathF.PI * 0.25f * player.direction;

		Texture2D cream = ModAsset.CreamChocolateCupStaff_cream.Value;
		Texture2D bar = ModAsset.CreamChocolateCupStaff_chocolateBar.Value;
		Texture2D stick = ModAsset.CreamChocolateCupStaff_chocolateStick.Value;
		Vector2 normal = new Vector2(1, -1 * player.direction).RotatedBy(rot0);
		Vector2 normalB = new Vector2(1, -1 * player.direction).RotatedBy(rot0 - 0.5f * player.direction);
		Vector2 normalS = new Vector2(1, -1 * player.direction).RotatedBy(rot0 + 0.5f * player.direction);

		Main.spriteBatch.Draw(ModAsset.CreamChocolateCupStaff_cup.Value, Projectile.Center - Main.screenPosition, null, drawColor, rot0, new Vector2(10, se == SpriteEffects.FlipVertically ? 10 : 44), 1f, se, 0);
		float value = (60 - Cooling) / 60f;
		value = MathF.Pow(value, 0.5f);
		value = (float)Utils.Lerp(15, 25, value);
		Main.spriteBatch.Draw(bar, Projectile.Center - Main.screenPosition + normalB * value + new Vector2(8 * player.direction, 4), null, drawColor, rot0, bar.Size() / 2f, 1f, se, 0);
		Main.spriteBatch.Draw(cream, Projectile.Center - Main.screenPosition + normal * value + new Vector2(0, -2), null, drawColor, rot0, cream.Size() / 2f, 1f, se, 0);
		Main.spriteBatch.Draw(stick, Projectile.Center - Main.screenPosition + normalS * value + new Vector2(8 * player.direction, -8), null, drawColor, rot0, stick.Size() / 2f, 1f, se, 0);

		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition, null, drawColor, rot0, new Vector2(10, se == SpriteEffects.FlipVertically ? 10 : 44), 1f, se, 0);
	}
}
