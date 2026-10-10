using Everglow.Commons.VFX.CommonVFXDusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Legacies;

public class HandheldCircularSaw_proj : ModProjectile
{
	public override string LocalizationCategory => LocalizationUtils.Categories.MagicProjectiles;

	public override void PostAI()
	{
		bool heldByOwner = Projectile.owner >= 0 && Projectile.owner < Main.maxPlayers
			&& Main.player[Projectile.owner].heldProj == Projectile.whoAmI;
		Projectile.drawLayer = heldByOwner ? ProjectileDrawLayerID.HeldProj : ProjectileDrawLayerID.Default;
		Projectile.usesOwnerLight = heldByOwner;
	}

	public override void SetDefaults()
	{
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.friendly = true;
		Projectile.ignoreWater = true;
		Projectile.drawLayer = ProjectileDrawLayerID.HeldProj;
		Projectile.usesOwnerLight = true;
	}

	public override void OnSpawn(IEntitySource source)
	{
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathF.PI * 0.75f);

		Vector2 armRootPos = player.MountedCenter + new Vector2(-4 * player.direction, -2);
		player.heldProj = Projectile.whoAmI;
		Vector2 mouseToPlayer = Main.MouseWorld - player.MountedCenter;
		mouseToPlayer = Vector2.Normalize(mouseToPlayer);
		if (player.controlUseItem)
		{
			float aimRot = -MathF.Asin(Vector3.Cross(new Vector3(mouseToPlayer, 0), new Vector3(new Vector2(0, 1).RotatedBy(Projectile.rotation - Math.PI * 0.75), 0)).Z);
			Projectile.rotation += aimRot * 0.05f;
			Projectile.Center = armRootPos + new Vector2(0, 1).RotatedBy(Projectile.rotation - Math.PI * 0.75) * 50f;
			Projectile.velocity *= 0;
			if (player.itemTime == 0)
			{
				player.itemTime = player.itemTimeMax;
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
		GenerateOrangeSpark((int)FlameValue);
		FlameValue *= 0.92f;
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		return false;
	}

	public float FlameValue = 0;

	public override void PostDraw(Player player, Color lightColor)
	{
		player.heldProj = Projectile.whoAmI;
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		var texSaw = ModAsset.HandheldCircularSaw_saw.Value;
		Color drawColor = Lighting.GetColor((int)Projectile.Center.X / 16, (int)(Projectile.Center.Y / 16.0));
		SpriteEffects se = SpriteEffects.None;
		if (player.direction == -1)
		{
			se = SpriteEffects.FlipVertically;
		}

		float rot0 = Projectile.rotation - (float)(Math.PI * 0.25) + MathF.PI * 0.25f * player.direction;
		Vector2 projToPlayer = new Vector2(0, 1).RotatedBy(Projectile.rotation - Math.PI * 0.75);

		Main.spriteBatch.Draw(texSaw, Projectile.Center - Main.screenPosition + projToPlayer * 0, new Rectangle(0, 0, 34, 34), drawColor, (float)Main.time * 0.75f, new Vector2(17), 1f, se, 0);
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition - projToPlayer * 13, null, drawColor, rot0, texMain.Size() / 2f, 1f, se, 0);
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		return base.Colliding(projHitbox, targetHitbox);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		FlameValue += 1f;
		base.OnHitNPC(target, hit, damageDone);
	}

	public void GenerateOrangeSpark(int times)
	{
		Player player = Main.player[Projectile.owner];
		Vector2 projToPlayer = new Vector2(0, 1).RotatedBy(Projectile.rotation - Math.PI * 0.75);
		Vector2 projToPlayerDown = new Vector2(-1, 0).RotatedBy(Projectile.rotation - Math.PI * 0.75);
		float addAngle = Main.rand.NextFloat(-0.8f, 0.8f);
		for (int a = 0; a < times; a++)
		{
			Vector2 newVelocity = projToPlayer.RotatedBy(addAngle) * -1f * FlameValue;
			var spark = new FireSparkDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + projToPlayer * 3 + projToPlayerDown.RotatedBy(addAngle) * 15 * player.direction,
				MaxTime = Main.rand.Next(7, 45),
				Scale = Main.rand.NextFloat(1f, Main.rand.NextFloat(4f, 7.0f)),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.03f, 0.08f) * player.direction },
			};
			Ins.VFXManager.Add(spark);
		}
	}
}
