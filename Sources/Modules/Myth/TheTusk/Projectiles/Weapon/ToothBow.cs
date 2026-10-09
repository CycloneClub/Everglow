using Everglow.Commons.Templates.Weapons;
using Terraria.Audio;

namespace Everglow.Myth.TheTusk.Projectiles.Weapon;

public class ToothBow : HandholdProjectile
{
	public override void SetDef()
	{
		Projectile.width = 64;
		Projectile.height = 64;
		Projectile.friendly = false;
		TextureRotation = 0;
		DepartLength = 20;
	}

	public override void AI()
	{
		base.AI();
	}

	public override void HeldProjectileAI()
	{
		Player player = Main.player[Projectile.owner];
		player.heldProj = Projectile.whoAmI;
		int dir = 1;
		if (Main.MouseWorld.X < player.MountedCenter.X)
		{
			dir = -1;
		}

		ArmRootPos = player.MountedCenter + new Vector2(-4 * dir, -2);
		float backValue = (player.itemTimeMax - player.itemTime) / (float)player.itemTimeMax;
		Player.CompositeArmStretchAmount compositeArmStretchAmount = Player.CompositeArmStretchAmount.Full;
		if (backValue < 0.5f)
		{
			if (backValue > 0.1f)
			{
				compositeArmStretchAmount = Player.CompositeArmStretchAmount.ThreeQuarters;
			}
			if (backValue > 0.25f)
			{
				compositeArmStretchAmount = Player.CompositeArmStretchAmount.Quarter;
			}
			if (backValue > 0.4f)
			{
				compositeArmStretchAmount = Player.CompositeArmStretchAmount.None;
			}
			player.SetCompositeArmFront(true, compositeArmStretchAmount, (Projectile.rotation - MathF.PI * 0.25f) * player.gravDir - MathF.PI * 0.5f);
		}
		else
		{
			compositeArmStretchAmount = Player.CompositeArmStretchAmount.None;
			if (backValue > 0.65f)
			{
				compositeArmStretchAmount = Player.CompositeArmStretchAmount.Quarter;
			}
			if (backValue > 0.8f)
			{
				compositeArmStretchAmount = Player.CompositeArmStretchAmount.ThreeQuarters;
			}
			if (backValue > 0.9f)
			{
				compositeArmStretchAmount = Player.CompositeArmStretchAmount.Full;
			}
			player.SetCompositeArmFront(true, compositeArmStretchAmount, (Projectile.rotation - MathF.PI * 0.25f) * player.gravDir + MathF.PI * 0.5f);
		}
		player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, (Projectile.rotation - MathF.PI * 0.25f) * player.gravDir - MathF.PI * 0.5f);
		Vector2 mouseToPlayer = Main.MouseWorld - ArmRootPos;
		mouseToPlayer = Vector2.Normalize(mouseToPlayer);
		if (player.controlUseItem)
		{
			float addRot = -MathF.Asin(Vector3.Cross(new Vector3(mouseToPlayer, 0), new Vector3(new Vector2(0, 1).RotatedBy(Projectile.rotation - Math.PI * 0.75), 0)).Z);
			addRot *= LerpFactorOfRotation;
			if (Math.Abs(addRot) > MaxRotationSpeed)
			{
				addRot *= MaxRotationSpeed / Math.Abs(addRot);
			}
			Projectile.rotation += addRot;
			Projectile.Center = ArmRootPos + new Vector2(0, 1).RotatedBy(Projectile.rotation - Math.PI * 0.75) * DepartLength;

			Projectile.timeLeft = player.itemTimeMax;
		}
		else
		{
			SoundEngine.PlaySound(SoundID.Item5, Projectile.Center);
			Projectile.NewProjectileDirect(Terraria.Entity.InheritSource(Projectile), Projectile.Center, new Vector2(0, 1).RotatedBy(Projectile.rotation - Math.PI * 0.75) * 30f, ModContent.ProjectileType<ToothBow_BloodArrow>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
			Projectile.Kill();
		}
		player.direction = dir;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		DrawBaseTexture(lightColor);

		return false;
	}

	public override void DrawBaseTexture(Color lightColor)
	{
		Player player = Main.player[Projectile.owner];
		var texMain = ModAsset.ToothBow_bow.Value;
		var texArrow = ModAsset.ToothBow_BloodArrow.Value;
		SpriteEffects se = SpriteEffects.None;
		if (player.direction == -1)
		{
			se = SpriteEffects.FlipVertically;
		}

		float rot = Projectile.rotation - (float)(Math.PI * 0.25) + TextureRotation * player.direction;
		float backValue = (player.itemTimeMax - player.itemTime) / (float)player.itemTimeMax * 20;

		Vector2 drawCenter = Projectile.Center - Main.screenPosition + DrawOffset;
		Main.spriteBatch.Draw(texArrow, drawCenter + new Vector2(5 - backValue, 0).RotatedBy(rot), null, lightColor, rot, texArrow.Size() / 2f, 0.75f, se, 0);
		Main.spriteBatch.Draw(texMain, drawCenter, null, lightColor, rot, texMain.Size() / 2f, 1f, se, 0);
		if (Main.MouseWorld.X < player.MountedCenter.X)
		{
			player.direction = -1;
		}
		else
		{
			player.direction = 1;
		}

		List<Vertex2D> bars = new List<Vertex2D>();
		bars.Add(drawCenter + (new Vector2(-23, -32) + new Vector2(-5, 0)).RotatedBy(rot), lightColor, new Vector3(0, 0, 0));
		bars.Add(drawCenter + (new Vector2(-23, -32) + new Vector2(5, 0)).RotatedBy(rot), lightColor, new Vector3(1, 0, 0));

		bars.Add(drawCenter + (new Vector2(-10 - backValue, 0) + new Vector2(-5, 0)).RotatedBy(rot), lightColor, new Vector3(0, 0.5f, 0));
		bars.Add(drawCenter + (new Vector2(-10 - backValue, 0) + new Vector2(5, 0)).RotatedBy(rot), lightColor, new Vector3(1, 0.5f, 0));

		bars.Add(drawCenter + (new Vector2(-23, 32) + new Vector2(-5, 0)).RotatedBy(rot), lightColor, new Vector3(0, 1, 0));
		bars.Add(drawCenter + (new Vector2(-23, 32) + new Vector2(5, 0)).RotatedBy(rot), lightColor, new Vector3(1, 1, 0));
		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.ToothBow_string.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}
}
