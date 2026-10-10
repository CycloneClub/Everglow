using Everglow.Commons.MEAC;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Everglow.Food.Dusts;
using Terraria.Audio;

namespace Everglow.Food.Projectiles;

public class FryingPan : MeleeProj
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

	public override void SetDef()
	{
		Projectile.width = 60;
		Projectile.height = 20;

		maxAttackType = 0; // 循环攻击方式的总数

		maxSlashTrailLength = 10; // 拖尾的长度

		shaderType = Commons.MEAC.Enums.MeleeTrailShaderType.ArcBladeTransparentedByZ;

		drawScaleFactor = 1f;

		disFromPlayer = 0;

		Projectile.scale = 1f; // 总大小，有需要时可以使用

		Projectile.timeLeft = 3000;

		/*
             * 若要增加剑的宽度，需要增大scale并在Attack()函数中降低MainAxisDirection的长度
             */
	}

	// 一定程度上决定拖尾的亮度/不透明度
	public override float TrailAlpha(float factor)
	{
		if (currantAttackType == 0)
		{
			return base.TrailAlpha(factor) * 1.2f;
		}
		else
		{
			return base.TrailAlpha(factor) * 0.9f;
		}
	}

	public override string TrailShapeTex()
	{
		return Commons.ModAsset.Melee_Mod;
	}

	public override string TrailColorTex()
	{
		return "Everglow/Food/Images/PanColor";
	}

	// 拖尾的混合模式，通常使用NonPremultiplied（暗）或者Additive（亮）
	public override BlendState TrailBlendState()
	{
		return BlendState.NonPremultiplied;
	}

	public override void DrawSelf(SpriteBatch spriteBatch, Color lightColor, Vector4 diagonal = default, Vector2 drawScale = default, Texture2D glowTexture = null)
	{
		drawScale.Y = 1.3f;
		if (currantAttackType == 0)
		{
			drawScale = new Vector2(-0.4f, 1.4f);
		}
		base.DrawSelf(spriteBatch, lightColor, diagonal, drawScale, glowTexture);
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		// 调整各个攻击方式的伤害倍率等等
		// ScreenShaker Gsplayer = Main.player[Projectile.owner].GetModPlayer<ScreenShaker>();
		if (currantAttackType == 100)
		{
			modifiers.FinalDamage *= 1.85f;
			modifiers.Knockback *= 2;

			// Gsplayer.FlyCamPosition = new Vector2(0, Math.Min(target.Hitbox.Width * target.Hitbox.Height / 12, 150)).RotatedByRandom(6.283);
			ShakerManager.AddShaker(UndirectedShakerInfo.Create(target.Center, Math.Min(target.Hitbox.Width * target.Hitbox.Height / 24, 50)));
		}
	}

	// 攻击方式编辑
	internal bool state1 = false;
	internal bool state2 = false;
	internal bool state3 = false;

	public override void Attack()
	{
		useSlash = true;
		if (currantAttackType == 100)// 右键长按蓄力斩的写法。因为不在循环内，所以这个type数值可以随便写，由Item切换到这个CurrantAttackType
		{
			int chargeTime1 = 30;
			int chargeTime2 = 60;
			int chargeTime3 = 90;

			if (timer < 10000)// 蓄力中
			{
				useSlash = false;
				LockPlayerDir(Player);

				Projectile.ai[0] = GetAngToMouse(); // 获取往鼠标的方向
				float targetRot = -MathHelper.PiOver2 - Player.direction * 0.5f;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(77, targetRot, 0f, Projectile.ai[0], 1000), 0.2f);

				Projectile.rotation = mainAxisDirection.ToRotation();

				// 向内的粒子效果
				for (int x = 0; x < 2; x++)
				{
					Vector2 r = Main.rand.NextVector2Unit();
					float dis1 = MathHelper.Clamp(chargeTime1 - timer, 0, chargeTime1) * 4;
					float dis2 = MathHelper.Clamp(chargeTime2 - timer, 0, chargeTime2) * 4;
					float dis3 = MathHelper.Clamp(chargeTime3 - timer, 0, chargeTime3) * 4;
					Dust d1 = Dust.NewDustDirect(Projectile.Center + r * dis1, 10, 10, ModContent.DustType<BlackPanDust>(), 0, 0, 100, new Color(0, Projectile.owner, 20), 0.8f);
					d1.alpha = 255;

					Dust d2 = Dust.NewDustDirect(Projectile.Center + r * dis2, 10, 10, ModContent.DustType<BlackPanDust2>(), 0, 0, 100, new Color(0, Projectile.owner, 20), 2f);
					d2.alpha = 255;

					Dust d3 = Dust.NewDustDirect(Projectile.Center + r * dis3, 10, 10, ModContent.DustType<BlackPanDust3>(), 0, 0, 100, new Color(0, Projectile.owner, 20), 3f);
					d3.alpha = 255;
				}
			}
			SoundStyle sound = SoundID.Item4;
			sound.Volume *= 0.4f;

			if (timer == chargeTime1)// 蓄力完成时
			{
				// 播放音效。
				SoundEngine.PlaySound(sound, Projectile.Center);
			}

			if (timer == chargeTime2)
			{
				SoundEngine.PlaySound(sound, Projectile.Center);
			}

			if (timer == chargeTime3)
			{
				SoundEngine.PlaySound(sound, Projectile.Center);
			}

			if (!Player.controlUseTile && timer >= chargeTime1 && timer < 10000)// 松开右键，且蓄力已经完成时
			{
				// 进入攻击状态
				state1 = true;

				if (timer >= chargeTime2)
				{
					state2 = true;
				}

				if (timer >= chargeTime3)
				{
					state3 = true;
				}

				timer = 10000;
				SoundEngine.PlaySound(SoundID.Item1);
			}

			if (timer >= 10000)// 开始挥动攻击
			{
				canHit = true;

				if (state1 == true)
				{
					if (timer < 10015)
					{
						canHit = true;
						mainAxisDirection = Vector2Elipse(77, Projectile.rotation, 0f, Projectile.ai[0]);
						Projectile.rotation += Projectile.spriteDirection * 0.35f;
					}
					if (!state2)
					{
						if (timer >= 10020)
						{
							Projectile.friendly = false;
							state1 = false;
							End();
						}
					}
				}

				if (state2 == true)
				{
					if (timer > 10015 && timer < 10020)
					{
						useSlash = false;
					}

					if (timer == 10020)
					{
						SoundEngine.PlaySound(SoundID.Item1);
					}

					if (timer >= 10020 && timer < 10035)
					{
						useSlash = true;
						canHit = true;
						mainAxisDirection = Vector2Elipse(77, Projectile.rotation, 0f, Projectile.ai[0]);
						Projectile.rotation += Projectile.spriteDirection * -0.35f;
					}
					if (!state3)
					{
						if (timer >= 10040)
						{
							Projectile.friendly = false;
							state1 = false;
							state2 = false;
							End();
						}
					}
				}

				if (state3 == true)
				{
					if (timer > 10040 && timer < 10050)
					{
						useSlash = false;
					}

					if (timer == 10050)
					{
						SoundEngine.PlaySound(SoundID.Item1);
					}

					if (timer >= 10050 && timer < 10075)
					{
						useSlash = true;
						canHit = true;
						Projectile.rotation += Projectile.spriteDirection * 0.25f;
						mainAxisDirection = Vector2Elipse(120, Projectile.rotation, -1.2f, Projectile.ai[0]);
					}
					if (timer >= 10080)
					{
						Projectile.friendly = false;
						state1 = false;
						state2 = false;
						state3 = false;
						End();
					}
				}
			}
		}
		if (canHit)
		{
			// 攻击时的粒子之类的
		}
	}

	private void PlayerAnimation(Player player)
	{
		Vector2 vToMouse = Main.MouseWorld - player.Top;
		int overTimer = timer - 10000;
		float YDevideX = MathF.Abs(vToMouse.X) / (vToMouse.Length() + 0.001f);
		if (timer > 10000)
		{
			if (overTimer < 24)
			{
				float BodyRotation = (float)Math.Sin(overTimer / 40d * Math.PI) * 0.2f * player.direction * player.gravDir * YDevideX;
				player.fullRotation = BodyRotation;
				player.fullRotationOrigin = new Vector2(player.Hitbox.Width / 2f, player.gravDir == -1 ? 0 : player.Hitbox.Height);
				player.legRotation = -BodyRotation;
				player.legPosition = (new Vector2(player.Hitbox.Width / 2f, player.Hitbox.Height) - player.fullRotationOrigin).RotatedBy(-BodyRotation);
			}
			else if (overTimer < 50)
			{
				float BodyRotation = (float)Math.Sin((overTimer - 30) / 40d * Math.PI) * -0.2f * player.direction * player.gravDir * YDevideX;
				player.fullRotation = BodyRotation;
				player.fullRotationOrigin = new Vector2(player.Hitbox.Width / 2f, player.gravDir == -1 ? 0 : player.Hitbox.Height);
				player.legRotation = -BodyRotation;
				player.legPosition = (new Vector2(player.Hitbox.Width / 2f, player.Hitbox.Height) - player.fullRotationOrigin).RotatedBy(-BodyRotation);
			}
			else
			{
				float BodyRotation = (float)Math.Sin(overTimer / 40d * Math.PI) * -0.6f * player.direction * player.gravDir * YDevideX;
				player.fullRotation = BodyRotation;
				player.fullRotationOrigin = new Vector2(player.Hitbox.Width / 2f, player.gravDir == -1 ? 0 : player.Hitbox.Height);
				player.legRotation = -BodyRotation;
				player.legPosition = (new Vector2(player.Hitbox.Width / 2f, player.Hitbox.Height) - player.fullRotationOrigin).RotatedBy(-BodyRotation);
			}
		}
		else
		{
			float BodyRotation = (float)Math.Sin(-Math.Min(timer * 0.5f, 45) / 90d * Math.PI) * 0.3f * player.direction * player.gravDir * YDevideX;
			player.fullRotation = BodyRotation;
			player.fullRotationOrigin = new Vector2(player.Hitbox.Width / 2f, player.gravDir == -1 ? 0 : player.Hitbox.Height);
			player.legRotation = -BodyRotation;
			player.legPosition = (new Vector2(player.Hitbox.Width / 2f, player.Hitbox.Height) - player.fullRotationOrigin).RotatedBy(-BodyRotation);
		}
	}

	private void GenerateDust(Vector2 pos, int count = 60)
	{
		float strength = count / 40f;
		for (int i = 0; i < count; i++)
		{
			Dust d = Dust.NewDustDirect(pos, 10, 10, ModContent.DustType<PanHitSpark>(), 0, 0, 0, default, Main.rand.NextFloat(0.85f, 1.65f) * strength);
			d.velocity = new Vector2(0, Main.rand.NextFloat(0f, 14.7f * strength)).RotatedByRandom(6.283) * strength;
		}
	}

	internal float OldSoundRot = 0;

	public override void AI()
	{
		if (currantAttackType == 0)
		{
			Projectile.tileCollide = true;
			Projectile.ignoreWater = false;
			canHit = true;
			longHandle = true;
			Player player = Main.player[Projectile.owner];
			Projectile.Center += Projectile.velocity;
			useSlash = true;
			timer++;
			if (Projectile.rotation - OldSoundRot > 6.283)
			{
				OldSoundRot = Projectile.rotation;
				AttSound(SoundID.Item1);
			}

			// if (hittimes > 0)
			// {
			//    foreach (Projectile proj in Main.projectile)
			//    {
			//        if (proj.active && proj != Projectile && proj.GetGlobalProjectile<Canhitproj>().Canhit && proj.penetrate != -1)
			//        {
			//            if ((Projectile.Center - proj.Center).Length() <= (float)Math.Sqrt(proj.width ^ 2 + proj.height ^ 2) * proj.scale / 2 + 42 * Projectile.scale)
			//            {
			//                Vector2 v1 = proj.velocity;
			//                Vector2 v2 = Projectile.velocity;

			// float m1 = MathF.Pow(proj.width * proj.height, 1.5f) * proj.knockBack * proj.scale;
			//                float m2 = MathF.Pow(Projectile.width * Projectile.height, 1.5f) * Projectile.knockBack * Projectile.scale / 50;

			// Vector2 newvelocity1 = (v1 * (m1 - m2) + 2 * m2 * v2) / (m1 + m2);
			//                Vector2 newvelocity2 = (v2 * (m2 - m1) + 2 * m1 * v1) / (m1 + m2);
			//                Vector2 dustvelocity = newvelocity1 - v1;

			// if (newvelocity1.Length() <= v1.Length())
			//                {
			//                    proj.velocity = Vector2.Normalize(newvelocity1) * v1.Length();
			//                }
			//                else
			//                {
			//                    proj.velocity = newvelocity1;
			//                }
			//                Projectile.velocity = newvelocity2;//这里是质心动量守恒的弹性碰撞

			// Projectile.NewProjectile(null, Projectile.Center - (Vector2.Normalize(dustvelocity).RotatedBy(Math.PI / 4) * 32), Vector2.Normalize(dustvelocity) * 15, ProjectileID.Spark, Projectile.damage, Projectile.knockBack, player.whoAmI);

			// int dust1 = Dust.NewDust(Projectile.Center - (Vector2.Normalize(dustvelocity).RotatedBy(Math.PI / 4) * 32), 0, 0, ModContent.DustType<MothSmog>(), Vector2.Normalize(dustvelocity).X * 5, Vector2.Normalize(dustvelocity).Y * 10, 100, default, Main.rand.NextFloat(3.7f, 5.1f));
			//                Main.dust[dust1].alpha = (int)(Main.dust[dust1].scale * 50);
			//                Main.dust[dust1].rotation = Main.rand.NextFloat(0, 6.283f);

			// SoundEngine.PlaySound(SoundID.NPCHit4, Projectile.Center);
			//                proj.GetGlobalProjectile<Canhitproj>().Canhit = false;
			//                hittimes--;
			//                Projectile.timeLeft = 2980;
			//            }
			//        }
			//    }
			// }
			Projectile.rotation += 0.1f * Projectile.spriteDirection;
			mainAxisDirection = Projectile.rotation.ToRotationVector2() * 58;
			if (Projectile.timeLeft <= 2980)
			{
				float mulVelocity = Math.Min((2960 - Projectile.timeLeft) / 16f, 16f);
				Projectile.velocity = Projectile.velocity * 0.8f + Vector2.Normalize(player.Center - Projectile.velocity - Projectile.Center) * mulVelocity;

				Projectile.rotation += mulVelocity / 12f * Projectile.spriteDirection;
				mainAxisDirection = Projectile.rotation.ToRotationVector2() * 58;
				if ((player.Center - Projectile.Center).Length() < 32)
				{
					Projectile.timeLeft = 0;
				}
			}
			if (Projectile.timeLeft <= 2980 - 40)
			{
				Projectile.tileCollide = false;
			}

			if (useSlash)
			{
				slashTrail.Enqueue(mainAxisDirection);
				if (slashTrail.Count > maxSlashTrailLength)
				{
					slashTrail.Dequeue();
				}
			}
			else// 清空！
			{
				slashTrail.Clear();
			}
		}
		else
		{
			PlayerAnimation(Player);
			base.AI();
		}
	}

	public override void DrawWarp(VFXBatch spriteBatch)
	{
		List<Vector2> SmoothTrailX = GraphicsUtils.CatmullRom(slashTrail.ToList()); // 平滑
		var SmoothTrail = new List<Vector2>();
		for (int x = 0; x < SmoothTrailX.Count - 1; x++)
		{
			SmoothTrail.Add(SmoothTrailX[x]);
		}
		if (slashTrail.Count != 0)
		{
			SmoothTrail.Add(slashTrail.ToArray()[slashTrail.Count - 1]);
		}

		int length = SmoothTrail.Count;
		if (length <= 3)
		{
			return;
		}

		Vector2[] trail = SmoothTrail.ToArray();
		var bars = new List<Vertex2D>();
		for (int i = 0; i < length; i++)
		{
			float factor = i / (length - 1f);
			float w = 0.2f;
			float d = trail[i].ToRotation() + 3.14f + 1.57f;
			if (d > 6.28f)
			{
				d -= 6.28f;
			}

			float dir = d / MathHelper.TwoPi;

			float dir1 = dir;
			if (i > 0)
			{
				float d1 = trail[i - 1].ToRotation() + 3.14f + 1.57f;
				if (d1 > 6.28f)
				{
					d1 -= 6.28f;
				}

				dir1 = d1 / MathHelper.TwoPi;
			}

			if (dir - dir1 > 0.5)
			{
				var midValue = (1 - dir) / (1 - dir + dir1);
				var midPoint = midValue * trail[i] + (1 - midValue) * trail[i - 1];
				var oldFactor = (i - 1) / (length - 1f);
				var midFactor = midValue * factor + (1 - midValue) * oldFactor;
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(0, w, 0, 1), new Vector3(midFactor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition + midPoint * Projectile.scale * 1.1f, new Color(0, w, 0, 1), new Vector3(midFactor, 0, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(1, w, 0, 1), new Vector3(midFactor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition + midPoint * Projectile.scale * 1.1f, new Color(1, w, 0, 1), new Vector3(midFactor, 0, 1)));
			}
			if (dir1 - dir > 0.5)
			{
				var midValue = (1 - dir1) / (1 - dir1 + dir);
				var midPoint = midValue * trail[i - 1] + (1 - midValue) * trail[i];
				var oldFactor = (i - 1) / (length - 1f);
				var midFactor = midValue * oldFactor + (1 - midValue) * factor;
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(1, w, 0, 1), new Vector3(midFactor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition + midPoint * Projectile.scale * 1.1f, new Color(1, w, 0, 1), new Vector3(midFactor, 0, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(0, w, 0, 1), new Vector3(midFactor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition + midPoint * Projectile.scale * 1.1f, new Color(0, w, 0, 1), new Vector3(midFactor, 0, 1)));
			}
			bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(dir, w, 0, 1), new Vector3(factor, 1, w)));
			bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition + trail[i] * Projectile.scale * 1.1f, new Color(dir, w, 0, 1), new Vector3(factor, 0, w)));
		}

		spriteBatch.Draw(ModContent.Request<Texture2D>(Commons.ModAsset.Melee_Warp_Mod).Value, bars, PrimitiveType.TriangleStrip);
		return;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (currantAttackType == 0)
		{
			SoundEngine.PlaySound(new SoundStyle("Everglow/Food/Sounds/Pan1").WithVolumeScale(0.3f).WithPitchOffset(1f), Projectile.Center);
			if (Projectile.timeLeft > 2960)
			{
				Projectile.velocity.X = Projectile.velocity.X * -1f;
				Projectile.velocity.Y = Projectile.velocity.Y * -1f;
				Projectile.timeLeft = 2960;
			}
			GenerateDust(target.Center, 20);
		}
		else
		{
			GenerateDust(target.Center, 50);
			SoundEngine.PlaySound(new SoundStyle("Everglow/Food/Sounds/Pan1").WithVolumeScale(1), Projectile.Center);
		}

		return;
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		if (currantAttackType == 0)
		{
			SoundEngine.PlaySound(new SoundStyle("Everglow/Food/Sounds/Pan1").WithVolumeScale(0.3f).WithPitchOffset(1f), Projectile.Center);
			if (Projectile.timeLeft > 2960)
			{
				Projectile.velocity.X = Projectile.velocity.X * -1f;
				Projectile.velocity.Y = Projectile.velocity.Y * -1f;

				Projectile.timeLeft = 2960;
			}
			GenerateDust(target.Center, 20);
		}
		else
		{
			SoundEngine.PlaySound(new SoundStyle("Everglow/Food/Sounds/Pan1").WithVolumeScale(1), Projectile.Center);
			GenerateDust(target.Center, 50);
		}

		return;
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		SoundEngine.PlaySound(new SoundStyle("Everglow/Food/Sounds/Pan1").WithVolumeScale(0.3f).WithPitchOffset(1f), Projectile.Center);
		if (currantAttackType == 0)
		{
			if (Projectile.timeLeft > 2960)
			{
				if (Projectile.velocity.X != oldVelocity.X)
				{
					Projectile.velocity.X = -oldVelocity.X;
				}

				if (Projectile.velocity.Y != oldVelocity.Y)
				{
					Projectile.velocity.Y = -oldVelocity.Y;
				}

				Projectile.timeLeft = 2960;
			}
		}
		GenerateDust(Projectile.Center + Projectile.oldVelocity, 40);
		return false;
	}
}

public class Canhitproj : GlobalProjectile
{
	public override bool InstancePerEntity => true;

	public bool Canhit = true;
}
