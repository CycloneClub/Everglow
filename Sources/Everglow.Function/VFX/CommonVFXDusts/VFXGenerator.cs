namespace Everglow.Commons.VFX.CommonVFXDusts;

internal class VFXGenerator : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 30;
		Item.height = 20;
		Item.useTurn = true;
		Item.useAnimation = 4;
		Item.useTime = 4;
		Item.autoReuse = false;
		Item.useStyle = ItemUseStyleID.Swing;
	}

	public override bool CanUseItem(Player player)
	{
		float mulVelocity = 1f;
		int type = Main.rand.Next(14);
		switch (type)
		{
			case 0: // 冰雾
				for (int g = 0; g < 20; g++)
				{
					Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(0f, 4f)).RotatedByRandom(MathHelper.TwoPi);
					var smog = new IceSmogDust
					{
						Velocity = newVelocity,
						Active = true,
						Visible = true,
						Position = player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + newVelocity * 16,
						MaxTime = Main.rand.Next(237, 345),
						Scale = Main.rand.NextFloat(320f, 435f),
						Rotation = Main.rand.NextFloat(6.283f),
						ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
					};
					Ins.VFXManager.Add(smog);
				}
				break;
			case 1:
				for (int g = 0; g < 80; g++)
				{
					Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(0f, 12f)).RotatedByRandom(MathHelper.TwoPi);
					var smog = new IceParticleDust
					{
						Velocity = newVelocity,
						Active = true,
						Visible = true,
						Position = player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
						MaxTime = Main.rand.Next(237, 345),
						Scale = Main.rand.NextFloat(12.20f, 32.35f),
						Rotation = Main.rand.NextFloat(6.283f),
						ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
					};
					Ins.VFXManager.Add(smog);
				}
				break; // 冰粒
			case 2: // 雪花
				for (int g = 0; g < 380; g++)
				{
					Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(2f, 9.6f)).RotatedByRandom(MathHelper.TwoPi);
					var smog = new SnowPieceDust
					{
						Velocity = newVelocity + player.velocity * Main.rand.NextFloat(0f, 0.03f),
						Active = true,
						Visible = true,
						Coord0 = new Vector2(Main.rand.NextFloat(0.1f, 0.2f), 0).RotatedByRandom(6.283),
						Coord1 = new Vector2(Main.rand.NextFloat(0.1f, 0.2f), 0).RotatedByRandom(6.283),
						Position = player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + player.velocity * Main.rand.NextFloat(-3f, 2f),
						MaxTime = Main.rand.Next(147, 285),
						Scale = Main.rand.NextFloat(2f, 12f),
						Rotation = Main.rand.NextFloat(6.283f),
						Rotation2 = Main.rand.NextFloat(6.283f),
						Omega = Main.rand.NextFloat(-10f, 10f),
						Phi = Main.rand.NextFloat(6.283f),
						ai = new float[] { Main.rand.NextFloat(0f, 1f), Main.rand.NextFloat(0f, 1f), Main.rand.NextFloat(-0.005f, 0.005f) },
					};
					Ins.VFXManager.Add(smog);
				}
				break;
			case 3: // 黑烟
				for (int g = 0; g < 40; g++)
				{
					Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(2f, 4f)).RotatedByRandom(MathHelper.TwoPi);
					var smog = new FireSmogDust
					{
						Velocity = newVelocity,
						Active = true,
						Visible = true,
						Position = player.Center + new Vector2(Main.rand.NextFloat(0, 12), 0).RotatedByRandom(6.283) + newVelocity * 3,
						MaxTime = Main.rand.Next(37, 45),
						Scale = Main.rand.NextFloat(20f, 35f),
						Rotation = Main.rand.NextFloat(6.283f),
						ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
					};
					Ins.VFXManager.Add(smog);
				}
				break;
			case 4: // 火焰
				for (int g = 0; g < 40; g++)
				{
					Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(0f, 7f)).RotatedByRandom(MathHelper.TwoPi);
					var fire = new FireDust
					{
						Velocity = newVelocity,
						Active = true,
						Visible = true,
						Position = player.Center + new Vector2(Main.rand.NextFloat(0, 6f), 0).RotatedByRandom(6.283) + newVelocity * 3,
						MaxTime = Main.rand.Next(16, 45),
						Scale = Main.rand.NextFloat(20f, 60f),
						Rotation = Main.rand.NextFloat(6.283f),
						ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
					};
					Ins.VFXManager.Add(fire);
				}
				break;
			case 5: // 火花
				for (int g = 0; g < 720; g++)
				{
					Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(2f, 6f)).RotatedByRandom(MathHelper.TwoPi);
					var spark = new FireSparkDust
					{
						Velocity = newVelocity,
						Active = true,
						Visible = true,
						Position = player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) - player.velocity + newVelocity * 3,
						MaxTime = Main.rand.Next(137, 245),
						Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(0.1f, 17.0f)),
						Rotation = Main.rand.NextFloat(6.283f),
						ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.13f, 0.13f) },
					};
					Ins.VFXManager.Add(spark);
				}
				break;
			case 6: // 诅咒焰
				for (int g = 0; g < 40; g++)
				{
					Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(0f, 7f)).RotatedByRandom(MathHelper.TwoPi);
					var fire = new CurseFlameDust
					{
						Velocity = newVelocity,
						Active = true,
						Visible = true,
						Position = player.Center + new Vector2(Main.rand.NextFloat(0, 6f), 0).RotatedByRandom(6.283) + newVelocity * 3,
						MaxTime = Main.rand.Next(16, 45),
						Scale = Main.rand.NextFloat(20f, 60f),
						Rotation = Main.rand.NextFloat(6.283f),
						ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0, 1f },
					};
					Ins.VFXManager.Add(fire);
				}
				break;
			case 7: // 诅咒火花
				for (int g = 0; g < 500; g++)
				{
					Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(2f, 6f)).RotatedByRandom(MathHelper.TwoPi);
					var spark = new CurseFlameSparkDust
					{
						Velocity = newVelocity,
						Active = true,
						Visible = true,
						Position = player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) - player.velocity + newVelocity * 3,
						MaxTime = Main.rand.Next(137, 245),
						Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(4f, 47.0f)),
						Rotation = Main.rand.NextFloat(6.283f),
						ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.13f, 0.13f) },
					};
					Ins.VFXManager.Add(spark);
				}
				break;
			case 8: // 水气
				for (int g = 0; g < 10; g++)
				{
					Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(0f, 4f)).RotatedByRandom(MathHelper.TwoPi);
					var smog = new VaporDust
					{
						Velocity = newVelocity,
						Active = true,
						Visible = true,
						Position = player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + newVelocity * 16,
						MaxTime = Main.rand.Next(104, 220),
						Scale = Main.rand.NextFloat(320f, 435f),
						Rotation = Main.rand.NextFloat(6.283f),
						ai = new float[] { Main.rand.NextFloat(-0.05f, -0.01f), 0 },
					};
					Ins.VFXManager.Add(smog);
				}
				break;
			case 9: // 丛林孢子
				for (int g = 0; g < 200; g++)
				{
					Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(2f, 26f)).RotatedByRandom(MathHelper.TwoPi);
					var spark = new JungleSporeDust
					{
						Velocity = newVelocity,
						Active = true,
						Visible = true,
						Position = player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) - player.velocity + newVelocity * 3,
						MaxTime = Main.rand.Next(137, 245),
						Scale = Main.rand.NextFloat(12f, Main.rand.NextFloat(12f, 37.0f)),
						Rotation = Main.rand.NextFloat(6.283f),
						ai = new float[] { Main.rand.NextFloat(0.0f, Main.rand.NextFloat(0.5f, 1.0f)), Main.rand.NextFloat(-0.03f, 0.03f) },
					};
					Ins.VFXManager.Add(spark);
				}
				break;
			case 10: // 血
				for (int g = 0; g < 100; g++)
				{
					Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(80f)).RotatedByRandom(MathHelper.TwoPi);
					float mulScale = Main.rand.NextFloat(6f, 25f);
					var blood = new BloodDrop
					{
						Velocity = afterVelocity * mulVelocity / mulScale,
						Active = true,
						Visible = true,
						Position = player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) - player.velocity * Main.rand.NextFloat(3f, 14f),
						MaxTime = Main.rand.Next(82, 164),
						Scale = mulScale,
						Rotation = Main.rand.NextFloat(6.283f),
						ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
					};
					Ins.VFXManager.Add(blood);
				}
				for (int g = 0; g < 40; g++)
				{
					Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(5f)).RotatedByRandom(MathHelper.TwoPi);
					var blood = new BloodSplash
					{
						Velocity = afterVelocity * mulVelocity,
						Active = true,
						Visible = true,
						Position = player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) - player.velocity * Main.rand.NextFloat(3f, 14f),
						MaxTime = Main.rand.Next(42, 78),
						Scale = Main.rand.NextFloat(6f, 18f),
						ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0, Main.rand.NextFloat(20.0f, 40.0f) },
					};
					Ins.VFXManager.Add(blood);
				}
				break;
			case 11: // 灵液
				for (int g = 0; g < 60; g++)
				{
					Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(40f)).RotatedByRandom(MathHelper.TwoPi);
					float mulScale = Main.rand.NextFloat(6f, 14f);
					var blood = new IchorDrop
					{
						Velocity = afterVelocity * mulVelocity / mulScale,
						Active = true,
						Visible = true,
						Position = player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) - player.velocity * Main.rand.NextFloat(3f, 14f),
						MaxTime = Main.rand.Next(82, 164),
						Scale = mulScale,
						Rotation = Main.rand.NextFloat(6.283f),
						ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
					};
					Ins.VFXManager.Add(blood);
				}
				for (int g = 0; g < 30; g++)
				{
					Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(3f)).RotatedByRandom(MathHelper.TwoPi);
					var blood = new IchorSplash
					{
						Velocity = afterVelocity * mulVelocity,
						Active = true,
						Visible = true,
						Position = player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) - player.velocity * Main.rand.NextFloat(3f, 14f),
						MaxTime = Main.rand.Next(42, 164),
						Scale = Main.rand.NextFloat(6f, 12f),
						ai = new float[] { Main.rand.NextFloat(0.0f, 0.4f), 0 },
					};
					Ins.VFXManager.Add(blood);
				}
				break;
			case 12: // 电流
				for (int g = 0; g < 40; g++)
				{
					float size = Main.rand.NextFloat(8f, Main.rand.NextFloat(20f, 40f));
					Vector2 afterVelocity = new Vector2(0, size * 1.3f).RotatedByRandom(MathHelper.TwoPi);
					var electric = new ElectricCurrent
					{
						Velocity = afterVelocity * mulVelocity,
						Active = true,
						Visible = true,
						Position = player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) - player.velocity * Main.rand.NextFloat(3f, 14f),
						MaxTime = size * size / 16f,
						Scale = size,
						ai = new float[] { Main.rand.NextFloat(0.0f, 0.6f), size / 2, Main.rand.NextFloat(0.8f, 1.2f) },
					};
					Ins.VFXManager.Add(electric);
				}
				for (int g = 0; g < 120; g++)
				{
					float size = Main.rand.NextFloat(8f, Main.rand.NextFloat(8f, 18f));
					Vector2 afterVelocity = new Vector2(0, size * 1.3f).RotatedByRandom(MathHelper.TwoPi);
					var electric = new ElectricCurrent
					{
						Velocity = afterVelocity * mulVelocity,
						Active = true,
						Visible = true,
						Position = player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) - player.velocity * Main.rand.NextFloat(3f, 14f),
						MaxTime = size * size / 4f,
						Scale = size,
						ai = new float[] { Main.rand.NextFloat(0.0f, 0.6f), size, Main.rand.NextFloat(1.3f, Main.rand.NextFloat(1.3f, 4f)) },
					};
					Ins.VFXManager.Add(electric);
				}
				break;
			case 13: // 分叉闪电
				{
					var lightning = new BranchedLightning(100f, 9f, Main.MouseWorld, Main.rand.NextVector2Unit().ToRotation(), 300f, (float)(Math.PI / 5));
					Ins.VFXManager.Add(lightning);
				}
				break;
		}

		return true;
	}

	public override bool? UseItem(Player player)
	{
		return base.UseItem(player);
	}
}
