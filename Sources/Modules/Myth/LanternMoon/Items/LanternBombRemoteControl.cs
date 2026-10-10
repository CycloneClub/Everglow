using Everglow.Myth.LanternMoon.Gores;
using Everglow.Myth.LanternMoon.Projectiles.LanternKing;
using Everglow.Myth.LanternMoon.Projectiles.PerWave15;
using Everglow.Myth.LanternMoon.VFX;
using Terraria.DataStructures;

namespace Everglow.Myth.LanternMoon.Items;

public class LanternBombRemoteControl : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicWeapons;

	public override void SetDefaults()
	{
		Item.DamageType = DamageClass.Magic;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.rare = ItemRarityID.White;
		Item.noUseGraphic = true;
		Item.autoReuse = false;
		Item.useTurn = true;
		Item.mana = 7;
		Item.width = 20;
		Item.height = 38;
		Item.useAnimation = 10;
		Item.useTime = 10;
		Item.value = 10000;
		Item.shoot = ProjectileID.WoodenArrowFriendly;
		Item.shootSpeed = 10;
		Item.damage = 9999;
		Item.knockBack = 15;
	}

	public override bool AltFunctionUse(Player player)
	{
		return true;
	}

	public Vector2 RandomVector2(float maxLength, float minLength = 0)
	{
		if (maxLength <= minLength)
		{
			maxLength = minLength + 0.001f;
		}
		return new Vector2(Main.rand.NextFloat(minLength, maxLength), 0).RotatedByRandom(MathHelper.TwoPi);
	}

	public void BrakeGoldenShieldEffect()
	{
		for (int g = 0; g < 150; g++)
		{
			float value = MathF.Pow(Main.rand.NextFloat(), 0.3f);
			Vector2 offsetPos = new Vector2(0, -value * 135).RotatedByRandom(2.6f);
			offsetPos.Y *= 85f / 135f;
			Vector2 newVelocity = offsetPos / 2.4f;
			var sparkFlame = new LanternGoldenShieldFragiles
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Main.MouseWorld + offsetPos,
				RotateSpeed = Main.rand.NextFloat(-0.3f, 0.3f),
				Rotate2Speed = Main.rand.NextFloat(-0.5f, 0.5f),
				VelocityRotateSpeed = Main.rand.NextFloat(0.15f, 0.45f) * (g % 2 - 0.5f) * 0.2f,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				Rotation2 = Main.rand.NextFloat(MathHelper.TwoPi),
				MaxTime = Main.rand.Next(155, 200),
				Scale = Main.rand.NextFloat(0.6f, 1.5f),
				Frame = Main.rand.Next(0, 4),
			};
			Ins.VFXManager.Add(sparkFlame);
		}
		for (int x = 0; x < 72; x++)
		{
			float value = MathF.Pow(Main.rand.NextFloat(), 0.3f);
			Vector2 offsetPos = new Vector2(0, -value * 135).RotatedByRandom(2.6f);
			offsetPos.Y *= 85f / 135f;
			Vector2 newVelocity = offsetPos / 2f;
			var spark = new LanternGoldenShieldStar()
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Main.MouseWorld + offsetPos,
				RotateSpeed = 0,
				Rotation = 0,
				MaxTime = Main.rand.Next(50, 100),
				Scale = Main.rand.NextFloat(0.5f, 1f),
			};
			Ins.VFXManager.Add(spark);
		}
	}

	public void KillGreenLanternEffect(Vector2 pos)
	{
		for (int g = 0; g < 12; g++)
		{
			Vector2 vel = new Vector2(MathF.Sqrt(Main.rand.NextFloat()) * 8f, 0).RotatedByRandom(MathHelper.TwoPi);
			string texturePath = ModAsset.GreenFlameLantern_Gore_0_Mod;
			if (texturePath is not null)
			{
				texturePath = texturePath.Remove(texturePath.Length - 1, 1);
				texturePath += g;
			}
			var gore = new NormalGore
			{
				Velocity = vel,
				Position = pos + vel * 6,
				Texture = ModContent.Request<Texture2D>(texturePath).Value,
				RotateSpeed = Main.rand.NextFloat(-0.2f, 0.2f),
				Scale = Main.rand.NextFloat(0.8f, 1.2f),
				MaxTime = Main.rand.Next(300, 340),
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
			};
			Ins.VFXManager.Add(gore);
		}

		for (int g = 0; g < 20; g++)
		{
			float value = MathF.Pow(Main.rand.NextFloat(), 0.3f);
			Vector2 offsetPos = new Vector2(0, -value * 15).RotatedByRandom(MathHelper.TwoPi);
			offsetPos.Y *= 85f / 135f;
			Vector2 newVelocity = offsetPos / 2.4f;
			offsetPos *= 6;
			var sparkFlame = new GreenLanternFragment
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = pos + offsetPos,
				RotateSpeed = Main.rand.NextFloat(-0.3f, 0.3f),
				Rotate2Speed = Main.rand.NextFloat(-0.5f, 0.5f),
				VelocityRotateSpeed = Main.rand.NextFloat(0.15f, 0.45f) * (g % 2 - 0.5f) * 0.2f,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				Rotation2 = Main.rand.NextFloat(MathHelper.TwoPi),
				MaxTime = Main.rand.Next(105, 150),
				Scale = Main.rand.NextFloat(0.6f, 1f),
				Frame = Main.rand.Next(0, 4),
				Gravity = true,
			};
			Ins.VFXManager.Add(sparkFlame);
		}
		for (int x = 0; x < 20; x++)
		{
			float value = MathF.Pow(Main.rand.NextFloat(), 0.3f);
			Vector2 offsetPos = new Vector2(0, -value * 25).RotatedByRandom(MathHelper.TwoPi);
			offsetPos.Y *= 85f / 135f;
			Vector2 newVelocity = offsetPos / 2f;
			var spark = new GreenLanternRedStar
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = pos + offsetPos,
				RotateSpeed = 0,
				Rotation = 0,
				MaxTime = Main.rand.Next(80, 160),
				Scale = Main.rand.NextFloat(0.5f, 1f),
				Gravity = true,
			};
			Ins.VFXManager.Add(spark);
		}
		for (int x = 0; x < 15; x++)
		{
			float value = MathF.Pow(Main.rand.NextFloat(), 0.3f);
			Vector2 offsetPos = new Vector2(0, -value * 12).RotatedByRandom(MathHelper.TwoPi);
			Vector2 newVelocity = offsetPos / 2f;
			var spark = new GreenLanternCyanStar
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = pos + new Vector2(0, -30) + offsetPos,
				RotateSpeed = 0,
				Rotation = 0,
				MaxTime = Main.rand.Next(50, 100),
				Scale = Main.rand.NextFloat(0.5f, 1f),
			};
			Ins.VFXManager.Add(spark);
		}
		for (int u = 0; u < 15; u++)
		{
			float sqrtSpeed = MathF.Sqrt(Main.rand.NextFloat(1f));
			Vector2 newVelocity = new Vector2(0, sqrtSpeed * 16f).RotatedByRandom(MathHelper.TwoPi);
			var somg = new LanternFlameDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = pos + new Vector2(Main.rand.NextFloat(30), 0).RotatedByRandom(MathHelper.TwoPi),
				MaxTime = Main.rand.Next(30, 45),
				Scale = Main.rand.NextFloat(50f, 120f),
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				RotateSpeed = Main.rand.NextFloat(-0.8f, 0.8f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
		for (int u = 0; u < 8; u++)
		{
			float sqrtSpeed = MathF.Sqrt(Main.rand.NextFloat(1f));
			Vector2 newVelocity = new Vector2(0, sqrtSpeed * 8f).RotatedByRandom(MathHelper.TwoPi);
			var somg = new GreenLanternFlame
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = pos + new Vector2(0, -30) + new Vector2(Main.rand.NextFloat(30), 0).RotatedByRandom(MathHelper.TwoPi),
				MaxTime = Main.rand.Next(30, 45),
				Scale = Main.rand.NextFloat(50f, 70f),
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				RotateSpeed = Main.rand.NextFloat(-0.8f, 0.8f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
		for (int i = 0; i < 8; i++)
		{
			Vector2 vel = new Vector2(0, -5).RotatedBy(i / 8f * MathHelper.TwoPi);
			if (i % 2 == 1)
			{
				vel *= 0.65f;
			}
			Projectile.NewProjectileDirect(Main.LocalPlayer.GetSource_ItemUse(Item), pos, vel, ModContent.ProjectileType<GreenFlameSharpCrystal>(), 20, 1, Main.myPlayer);
		}
		Main.slimeRain = false;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		Projectile.NewProjectileDirect(source, Main.MouseWorld, Vector2.zeroVector, ModContent.ProjectileType<KillLanternMoonMobs>(), 20, 1, Main.myPlayer);
		// KillGreenLanternEffect(Main.MouseWorld);
		return false;
	}
}
