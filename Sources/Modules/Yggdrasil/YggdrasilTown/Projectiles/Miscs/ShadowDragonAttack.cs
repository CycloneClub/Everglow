using Everglow.Commons.CustomTiles;
using Everglow.Commons.Templates.Weapons;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.RoomScenes;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Miscs;

public class ShadowDragonAttack : TrailingProjectile
{
	public override void SetCustomDefaults()
	{
		TrailTexture = Commons.ModAsset.Empty.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_8_black.Value;
		Projectile.hostile = true;
		Projectile.friendly = false;
	}

	public override void Behaviors()
	{
		Projectile.rotation = Projectile.velocity.ToRotation();

		// Dust
		if (Main.rand.NextBool(6))
		{
			var dark = new DarkDragon_Scene_Dust()
			{
				Position = Projectile.Center,
				Velocity = Projectile.velocity * 0.2f,
				Fade = 1,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				Timer = 0,
				MaxTime = 90,
				MaxPosY = float.PositiveInfinity,
				Frame = new Rectangle(Main.rand.Next(4) * 10, 10 + Main.rand.Next(3) * 10, 10, 10),
				Scale = Main.rand.NextFloat(1.45f, 1.6f),
				Active = true,
				Visible = true,
				MoveLogic = 1,
			};
			Ins.VFXManager.Add(dark);
		}

		// Chase
		Player target = Main.LocalPlayer;
		float minLength = 600;
		foreach (var player in Main.player)
		{
			if (player is not null && player.active)
			{
				float distance = (player.Center - Projectile.Center).Length();
				if (distance < minLength)
				{
					minLength = distance;
					target = player;
				}
			}
		}
		Vector2 toTarget = target.Center - Projectile.Center - Projectile.velocity;
		if (toTarget.Length() > 600)
		{
			return;
		}
		toTarget = toTarget.NormalizeSafe();
		Projectile.velocity = toTarget * 0.02f + Projectile.velocity * 0.98f;
		Projectile.velocity = Projectile.velocity.NormalizeSafe() * 3;
	}

	public override void DestroyEntityEffect()
	{
		for (int k = 0; k < 30; k++)
		{
			var dark = new DarkDragon_Scene_Dust()
			{
				Position = Projectile.Center,
				Velocity = new Vector2(0, Main.rand.NextFloat(3, 3.5f)).RotatedByRandom(MathHelper.TwoPi),
				Fade = 1,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				Timer = 0,
				MaxTime = 90,
				MaxPosY = float.PositiveInfinity,
				Frame = new Rectangle(Main.rand.Next(4) * 10, 10 + Main.rand.Next(3) * 10, 10, 10),
				Scale = Main.rand.NextFloat(1.45f, 1.6f),
				Active = true,
				Visible = true,
				MoveLogic = 1,
			};
			Ins.VFXManager.Add(dark);
		}
	}
}
