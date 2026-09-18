using Terraria.Audio;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Melee;

/// <summary>
/// 机关长戟的挥舞弹幕，继承自MeleeProj
/// </summary>
public class MechanismHalberd_Proj : MeleeProj
{
	public override void SetDef()
	{
		maxAttackType = 2; // 三段攻击
		maxSlashTrailLength = 18; // 拖尾长度
		longHandle = true; // 长柄武器
		shaderType = Commons.MEAC.Enums.MeleeTrailShaderType.ArcBladeTransparentedByZ;
		autoEnd = false; // 不使用默认结束攻击逻辑
		canLongLeftClick = false; // 不需要左键蓄力
	}

	public override string TrailColorTex() => ModAsset.MechanismHalberd_Color_Mod;

	public override BlendState TrailBlendState() => BlendState.Additive;

	public override float TrailAlpha(float factor) => base.TrailAlpha(factor) * 1.5f;

	/// <summary>
	/// 攻击效果
	/// </summary>
	public override void Attack()
	{
		Player player = Main.player[Projectile.owner];
		TestPlayerDrawer Tplayer = player.GetModPlayer<TestPlayerDrawer>();
		Tplayer.HideLeg = true;

		useSlash = true;

		Vector2 vToMouse = Main.MouseWorld - player.Top;
		float AddHeadRotation = (float)Math.Atan2(vToMouse.Y, vToMouse.X) + (1 - player.direction) * 1.57f;
		if (player.gravDir == -1)
		{
			if (player.direction == -1)
			{
				if (AddHeadRotation >= 0.57f && AddHeadRotation < 2)
				{
					AddHeadRotation = 0.57f;
				}
			}
			else
			{
				if (AddHeadRotation <= -0.57f)
				{
					AddHeadRotation = -0.57f;
				}
			}
		}
		else
		{
			if (player.direction == -1)
			{
				if (AddHeadRotation >= 2 && AddHeadRotation < 5.71f)
				{
					AddHeadRotation = 5.71f;
				}
			}
			else
			{
				if (AddHeadRotation >= 0.57f)
				{
					AddHeadRotation = 0.57f;
				}
			}
		}

		// 上劈
		if (currantAttackType == 0)
		{
			LockPlayerDir(player);
			float rot = player.direction;

			if (timer == 8)
			{
				AttSound(new SoundStyle(Commons.ModAsset.TrueMeleeSwing_Mod));
			}
			if (timer < 20)
			{
				useSlash = false;

				float rot0 = -MathHelper.PiOver2 - rot * 1.2f;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(100, rot0, -0.8f, rot), 0.15f);
				Projectile.rotation = mainAxisDirection.ToRotation();
			}
			if (timer > 20 && timer < 40)
			{
				canHit = true;
				Lighting.AddLight(Projectile.Center + mainAxisDirection, 0.25f, 0.3f, 0.35f);

				Projectile.rotation -= Projectile.spriteDirection * 0.22f;

				mainAxisDirection = Vector2Elipse(120, Projectile.rotation, -0.8f, rot);
			}
			if (timer > 45)
			{
				NextAttackType();
			}

			if (canHit && Main.rand.NextBool(4))
			{
				Vector2 offset = mainAxisDirection * Main.rand.NextFloat(0.3f, 0.9f);
				Dust.NewDustPerfect(Projectile.Center + offset, DustID.Smoke, Main.rand.NextVector2Unit() * 1.5f, 0, new Color(180, 200, 220), 1.1f).noGravity = true;
			}
		}

		// 下劈
		else if (currantAttackType == 1)
		{
			LockPlayerDir(player);
			float rot = player.direction;

			if (timer == 8)
			{
				AttSound(new SoundStyle(Commons.ModAsset.TrueMeleeSwing_Mod));
			}
			if (timer < 18)
			{
				float rot0 = -MathHelper.PiOver2 + 0.8f * rot;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(60, rot0, -0.8f, rot), 0.15f);
				useSlash = false;
				Projectile.rotation = mainAxisDirection.ToRotation();
			}
			if (timer > 18 && timer < 35)
			{
				Lighting.AddLight(Projectile.Center + mainAxisDirection, 0.4f, 0.4f, 0.25f);
				canHit = true;

				Projectile.rotation += Projectile.spriteDirection * 0.3f;

				mainAxisDirection = Vector2Elipse(160, Projectile.rotation, -0.8f, rot);
			}
			if (timer > 45)
			{
				NextAttackType();
			}
		}

		// 横劈
		else if (currantAttackType == 2)
		{
			LockPlayerDir(player);
			float rot = player.direction;

			if (timer < 30)
			{
				if (timer == 6)
				{
					SoundEngine.PlaySound(SoundID.Item24 with { Volume = 2f }, Projectile.Center);
				}

				useSlash = false;
				canHit = false;

				float rot0 = -MathHelper.PiOver2 - 0.4f * rot;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(80, rot0, -1.3f, rot * 0.1f), 0.2f);
				Projectile.rotation = mainAxisDirection.ToRotation();
			}
			else if (timer == 30)
			{
				AttSound(new SoundStyle(Commons.ModAsset.TrueMeleePowerSwing_Mod) with { Volume = 1.4f });
			}
			else if (timer > 30 && timer < 45)
			{
				canHit = true;
				useSlash = true;

				Lighting.AddLight(Projectile.Center + mainAxisDirection, 0.2f, 0.4f, 0.8f);

				Projectile.rotation += Projectile.spriteDirection * 0.35f;
				mainAxisDirection = Vector2Elipse(180, Projectile.rotation, -1.3f, rot * 0.1f);

				if (timer % 5 == 0)
				{
					Lighting.AddLight(Projectile.Center + mainAxisDirection * 0.5f, 0.3f, 0.5f, 0.9f);
				}
			}
			else if (timer > 45)
			{
				End();
			}
		}
	}

	/// <summary>
	/// 击中敌人概率掉出刺球
	/// </summary>
	/// <param name="target"></param>
	/// <param name="hit"></param>
	/// <param name="damageDone"></param>
	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		SoundEngine.PlaySound(SoundID.Item74 with { Volume = 2f }, Projectile.Center);

		for (int i = 0; i < 8; i++)
		{
			float angle = MathHelper.TwoPi / 8f * i;
			Vector2 dir = angle.ToRotationVector2();

			Vector2 dustVel = dir * Main.rand.NextFloat(2.5f, 3.5f);
			Dust d = Dust.NewDustPerfect(
				target.Center,
				DustID.Torch,
				dustVel,
				100,
				new Color(Main.rand.Next(220, 255), Main.rand.Next(100, 160), 30),
				Main.rand.NextFloat(0.7f, 1f));

			d.noGravity = true;
			d.fadeIn = Main.rand.NextFloat(1.2f, 1.5f);
		}

		Lighting.AddLight(target.Center, 0.3f, 0.5f, 0.8f);
		if (Main.rand.NextBool(3))
		{
			Player player = Main.player[Projectile.owner];
			if (Main.myPlayer == player.whoAmI)
			{
				Vector2 dir = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);
				Vector2 spawnPos = target.Center + dir * (target.width * 0.6f + 20f);
				float angle = Main.rand.NextFloat() * MathHelper.TwoPi;
				Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
				Projectile.NewProjectile(
				Projectile.GetSource_FromThis(),
				spawnPos,
				velocity,
				ModContent.ProjectileType<MechanismSpike>(),
				Projectile.damage / 2, 1f, player.whoAmI);
				SoundEngine.PlaySound(SoundID.Item9 with { Volume = 0.6f }, Projectile.Center);
			}
		}
	}

	public override void End()
	{
		Player player = Main.player[Projectile.owner];
		TestPlayerDrawer Tplayer = player.GetModPlayer<TestPlayerDrawer>();
		player.legFrame = new Rectangle(0, 0, player.legFrame.Width, player.legFrame.Height);
		player.fullRotation = 0;
		player.legRotation = 0;
		Tplayer.HeadRotation = 0;
		Tplayer.HideLeg = false;
		player.legPosition = Vector2.Zero;
		Projectile.Kill();
		player.GetModPlayer<MEACPlayer>().isUsingMeleeProj = false;
	}

	public override void DrawTrail(Color color)
	{
		base.DrawTrail(color);
	}
}
