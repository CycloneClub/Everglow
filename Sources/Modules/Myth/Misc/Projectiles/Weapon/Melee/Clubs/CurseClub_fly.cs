using Everglow.Commons.Graphics;
using Everglow.Commons.VFX.CommonVFXDusts;
using Terraria.Audio;
using Terraria.GameContent.Shaders;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class CurseClub_fly : ModProjectile, IWarpProjectile
{
	/// <summary>
	/// 角速度
	/// </summary>
	public float Omega = 0;

	/// <summary>
	/// 角加速度
	/// </summary>
	public float Beta = 0.005f;

	/// <summary>
	/// 最大角速度(受近战攻速影响)
	/// </summary>
	public float MaxOmega = 0.5f;

	/// <summary>
	/// 伤害半径
	/// </summary>
	public float HitLength = 32f;

	/// <summary>
	/// 命中敌人后对于角速度的削减率(会根据敌人的击退抗性而再次降低)
	/// </summary>
	public float StrikeOmegaDecrease = 0.9f;

	/// <summary>
	/// 命中敌人后最低剩余角速度(默认40%,即0.4)
	/// </summary>
	public float MinStrikeOmegaDecrease = 0.4f;

	/// <summary>
	/// 内部参数，用来计算伤害
	/// </summary>
	public int DamageStartValue = 0;

	/// <summary>
	/// 拖尾长度
	/// </summary>
	public int TrailLength = 10;

	/// <summary>
	/// 是否正在攻击
	/// </summary>
	public bool IsAttacking = false;

	/// <summary>
	/// 拖尾
	/// </summary>
	public Queue<Vector2> TrailVecs;

	public override void SetDefaults()
	{
		Projectile.width = 80;
		Projectile.height = 80;
		Projectile.penetrate = -1;
		Projectile.localNPCHitCooldown = 1;
		Projectile.timeLeft = 580;

		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.tileCollide = false;

		Projectile.DamageType = DamageClass.Melee;

		TrailVecs = new Queue<Vector2>(TrailLength + 1);
	}

	private void GenerateVFX()
	{
		float mulVelocity = 0.3f + Omega;
		var v0 = new Vector2(1, 1);
		v0 *= Main.rand.NextFloat(Main.rand.NextFloat(HitLength * 0.75f, HitLength), HitLength);
		v0.X *= Projectile.spriteDirection;
		if (Main.rand.NextBool(2))
		{
			v0 *= -1;
		}

		v0 = v0.RotatedBy(Projectile.rotation + Main.rand.NextFloat(Omega));
		float speed = Math.Min(Omega * 0.15f, 0.061f) * 7.2f;
		var v1 = new Vector2(-v0.Y, v0.X) * speed;

		if (Main.rand.NextBool(2))
		{
			GradientColor color = new GradientColor();
			color.colorList.Add((new Color(1f, 1f, 0.1f), 0f));
			color.colorList.Add((new Color(0.1f, 0.6f, 0.1f), 0.3f));
			int time = Main.rand.Next(15, 35);
			var fire = new Flare()
			{
				Position = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 30, Main.rand.NextFloat(0.4f, 1.25f)),
				Velocity = Projectile.velocity * 0.5f,
				Gravity = -0.3f,
				Color = color,
				TimeLeft = time,
				MaxTimeLeft = time,
				Scale = Main.rand.NextFloat(0.3f, 0.6f),
			};
			Ins.VFXManager.Add(fire);
		}
		/*
		var cf = new CurseFlame_HighQualityDust
		{
			Velocity = v1 + Projectile.velocity * 0.3f,
			Active = true,
			Visible = true,
			Position = Projectile.Center + v0,
			MaxTime = Main.rand.Next(12, 30),
			ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Omega * 0.75f, Main.rand.NextFloat(3.6f, 30f) * mulVelocity },
		};
		Ins.VFXManager.Add(cf);*/
		for (int g = 0; g < 4; g++)
		{
			v0 = new Vector2(1, 1);
			v0 *= Main.rand.NextFloat(Main.rand.NextFloat(HitLength * 0.75f, HitLength), HitLength);
			v0.X *= Projectile.spriteDirection;
			if (Main.rand.NextBool(2))
			{
				v0 *= -1;
			}

			v0 = v0.RotatedBy(Projectile.rotation + Main.rand.NextFloat(Omega));
			Vector2 newVelocity = new Vector2(-v0.Y, v0.X) * speed * 0.2f;
			float v0Length = v0.Length();
			var spark = new CurseFlameSparkDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + v0,
				MaxTime = Main.rand.Next(37, Main.rand.Next(37, 185)),
				Scale = Main.rand.NextFloat(4f, 27.0f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Omega * 0.1f * v0Length / 14f, 15f },
			};
			Ins.VFXManager.Add(spark);
		}
	}

	private void GenerateDust()
	{
		var v0 = new Vector2(1, 1);
		v0 *= Main.rand.NextFloat(Main.rand.NextFloat(1, HitLength), HitLength);
		v0.X *= Projectile.spriteDirection;
		if (Main.rand.NextBool(2))
		{
			v0 *= -1;
		}

		v0 = v0.RotatedBy(Projectile.rotation);
		float speed = Math.Min(Omega * 0.5f, 0.221f);
		var d = Dust.NewDustDirect(Projectile.Center + v0 - new Vector2(4)/*Dust的Size=8x8*/, 0, 0, DustID.CursedTorch, -v0.Y * speed, v0.X * speed, 150, default, Main.rand.NextFloat(0.4f, 1.1f));
		d.noGravity = true;
		d.velocity = new Vector2(-v0.Y * speed, v0.X * speed);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		hit.HitDirection = target.Center.X > Main.player[Projectile.owner].Center.X ? 1 : -1;
		target.AddBuff(BuffID.CursedInferno, (int)(818 * Omega));
	}

	public BlendState TrailBlendState()
	{
		return BlendState.AlphaBlend;
	}

	public string TrailShapeTex()
	{
		return Commons.ModAsset.Melee_Mod;
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		float power = Math.Max(StrikeOmegaDecrease - MathF.Pow(target.knockBackResist / 4f, 3), MinStrikeOmegaDecrease);

		ScreenShaker gsPlayer = Main.player[Projectile.owner].GetModPlayer<ScreenShaker>();
		float shakeStrength = Omega * 0.04f;
		Omega *= power;
		modifiers.FinalDamage /= power;
		gsPlayer.FlyCamPosition = new Vector2(0, Math.Min(target.Hitbox.Width * target.Hitbox.Height / 12f * shakeStrength, 100)).RotatedByRandom(6.283);
		modifiers.Knockback *= Omega * 3;
	}

	public override void AI()
	{
		if (Omega > 0.1f)
		{
			for (float d = 0.1f; d < Omega; d += 0.1f)
			{
				GenerateDust();
				if (Main.rand.NextBool(3))
				{
					GenerateVFX();
				}
			}
		}
		else
		{
			GenerateDust();
			if (Main.rand.NextBool(3))
			{
				GenerateVFX();
			}
		}
		if (DamageStartValue == 0)
		{
			DamageStartValue = Projectile.damage;
			Projectile.damage = 0;
		}

		// 造成伤害等于原伤害*转速*3.334
		Projectile.damage = Math.Max((int)(DamageStartValue * Omega * 3.334), 1);

		Player player = Main.player[Projectile.owner];
		if (Projectile.timeLeft >= 550)
		{
			Vector2 mouseToPlayer = Main.MouseWorld - player.MountedCenter;
			mouseToPlayer = Vector2.Normalize(mouseToPlayer) * 15f;
			Vector2 vT0 = Main.MouseWorld - player.MountedCenter;
			Projectile.Center = player.MountedCenter + mouseToPlayer;
			Projectile.spriteDirection = player.direction;
			if (Projectile.timeLeft == 550)
			{
				Projectile.velocity = vT0.SafeNormalize(Vector2.Zero) * 15;
				Projectile.friendly = true;
				SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot.WithPitchOffset(-1f), Projectile.Center);
			}
		}
		if (Projectile.timeLeft < 550 && Projectile.timeLeft > 500)
		{
			Projectile.velocity *= 0.985f;
		}

		if (Projectile.timeLeft < 500)
		{
			Projectile.velocity *= 0.96f;
			if (Projectile.timeLeft > 20 && Omega < 0.1f)
			{
				Projectile.timeLeft = 20;
			}
		}
		Projectile.localNPCHitCooldown = (int)(MathF.PI / Math.Max(Omega, 0.157));

		Projectile.rotation += Omega;
		float meleeSpeed = player.GetAttackSpeed(Projectile.DamageType);
		if (Projectile.timeLeft > 550)
		{
			if (Omega < meleeSpeed * MaxOmega)
			{
				Omega += Beta * meleeSpeed * 4f;
			}
		}
		else
		{
			if (Projectile.timeLeft < 500)
			{
				Omega *= 0.98f;
			}
			else
			{
				if (Omega < meleeSpeed * MaxOmega + 0.2f)
				{
					Omega += Beta * meleeSpeed * 0.04f;
				}
			}
		}
		Vector2 hitRange = new Vector2(HitLength, HitLength * Projectile.spriteDirection).RotatedBy(Projectile.rotation) * Projectile.scale;
		TrailVecs.Enqueue(hitRange);
		if (TrailVecs.Count > TrailLength)
		{
			TrailVecs.Dequeue();
		}

		if (player.dead)
		{
			Projectile.Kill();
		}

		ProduceWaterRipples(new Vector2(HitLength * Projectile.scale));
		float distance = 200f;

		if (Target == -1)
		{
			foreach (var npc in Main.npc)
			{
				if (npc.active)
				{
					if (npc.CanBeChasedBy())
					{
						if (!npc.dontTakeDamage)
						{
							if (!npc.friendly)
							{
								if ((npc.Center - Projectile.Center).Length() < distance)
								{
									Target = npc.whoAmI;
									distance = (npc.Center - Projectile.Center).Length();
								}
							}
						}
					}
				}
			}
		}
		if (Target >= 0)
		{
			if (!Main.npc[Target].active)
			{
				Target = -1;
				return;
			}
			Vector2 addV = (Main.npc[Target].Center - Projectile.Center).SafeNormalize(Vector2.Zero);
			Projectile.velocity = addV * Projectile.velocity.Length() * 0.15f + Projectile.velocity * 0.9f;

			if (Projectile.timeLeft > 100)
			{
				if (Omega < MaxOmega)
				{
					Omega += Beta * 1f;
				}
			}
		}
	}

	public int Target = -1;

	public override bool PreDraw(ref Color lightColor)
	{
		SpriteEffects effects = SpriteEffects.None;
		if (Projectile.spriteDirection == 1)
		{
			effects = SpriteEffects.FlipHorizontally;
		}

		var texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
		float colorValue = Omega / 0.4f;
		var color = new Color(colorValue, colorValue, colorValue, colorValue);
		Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, texture.Size() / 2f, Projectile.scale, effects, 0f);
		for (int i = 0; i < 5; i++)
		{
			float alp = Omega / 0.4f + 2.5f;
			var color2 = new Color((int)((5 - i) / 5f * alp), (int)((5 - i) / 5f * alp), (int)((5 - i) / 5f * alp), 0);
			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, color2, Projectile.rotation - i * 0.75f * Omega, texture.Size() / 2f, Projectile.scale, effects, 0f);
		}
		DrawTrail();
		PostPreDraw();
		return false;
	}

	public void PostPreDraw()
	{
	}

	public void DrawTrail()
	{
		List<Vector2> smoothTrailX = GraphicsUtils.CatmullRom(TrailVecs.ToList()); // 平滑
		var smoothTrail = new List<Vector2>();
		for (int x = 0; x < smoothTrailX.Count - 1; x++)
		{
			smoothTrail.Add(smoothTrailX[x]);
		}
		if (TrailVecs.Count != 0)
		{
			smoothTrail.Add(TrailVecs.ToArray()[TrailVecs.Count - 1]);
		}

		int length = smoothTrail.Count;
		if (length <= 3)
		{
			return;
		}

		Vector2[] trail = smoothTrail.ToArray();
		var bars = new List<Vertex2D>();

		float fade = Omega * 2f + 0.2f;
		var color2 = new Color(Math.Min(fade * 0.6f, 0.6f), fade, fade * 0.01f, fade * 0.5f);
		if (Projectile.timeLeft < 20)
		{
			color2 *= Projectile.timeLeft / 20f;
		}

		for (int i = 0; i < length; i++)
		{
			float factor = i / (length - 1f);
			float w = 1 - Math.Abs((trail[i].X * 0.5f + trail[i].Y * 0.5f) / trail[i].Length());
			float w2 = MathF.Sqrt(TrailAlpha(factor));
			w *= w2 * w;
			bars.Add(new Vertex2D(Projectile.Center + trail[i] * 0.5f * Projectile.scale - Main.screenPosition, color2, new Vector3(factor, 1, 0f)));
			bars.Add(new Vertex2D(Projectile.Center + trail[i] * 1.0f * Projectile.scale - Main.screenPosition, color2, new Vector3(factor, 0, w)));
		}
		bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, Color.Transparent, new Vector3(0, 0, 0)));
		bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, Color.Transparent, new Vector3(0, 0, 0)));
		for (int i = 0; i < length; i++)
		{
			float factor = i / (length - 1f);
			float w = 1 - Math.Abs((trail[i].X * 0.5f + trail[i].Y * 0.5f) / trail[i].Length());
			float w2 = MathF.Sqrt(TrailAlpha(factor));
			w *= w2 * w;
			bars.Add(new Vertex2D(Projectile.Center - trail[i] * 0.5f * Projectile.scale - Main.screenPosition, color2, new Vector3(factor, 1, 0f)));
			bars.Add(new Vertex2D(Projectile.Center - trail[i] * 1.0f * Projectile.scale - Main.screenPosition, color2, new Vector3(factor, 0, w)));
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, TrailBlendState(), SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.CurseClub_trail.Value;

		var lightColor = Lighting.GetColor((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16)).ToVector4();
		lightColor.W = 0.7f * Omega;

		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		float warpvalue = Omega * 0.1f;
		List<Vector2> smoothTrailX = GraphicsUtils.CatmullRom(TrailVecs.ToList()); // 平滑
		var smoothTrail = new List<Vector2>();
		for (int x = 0; x < smoothTrailX.Count - 1; x++)
		{
			smoothTrail.Add(smoothTrailX[x]);
		}
		if (TrailVecs.Count != 0)
		{
			smoothTrail.Add(TrailVecs.ToArray()[TrailVecs.Count - 1]);
		}

		int length = smoothTrail.Count;
		if (length <= 3)
		{
			return;
		}

		Vector2[] trail = smoothTrail.ToArray();
		var bars = new List<Vertex2D>();
		for (int i = 0; i < length; i++)
		{
			float factor = i / (length - 1f);

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
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(0, warpvalue, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition + midPoint * Projectile.scale * 1.1f, new Color(0, warpvalue, 0, 1), new Vector3(factor, 0, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(1, warpvalue, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition + midPoint * Projectile.scale * 1.1f, new Color(1, warpvalue, 0, 1), new Vector3(factor, 0, 1)));
			}
			if (dir1 - dir > 0.5)
			{
				var midValue = (1 - dir1) / (1 - dir1 + dir);
				var midPoint = midValue * trail[i - 1] + (1 - midValue) * trail[i];
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(1, warpvalue, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition + midPoint * Projectile.scale * 1.1f, new Color(1, warpvalue, 0, 1), new Vector3(factor, 0, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(0, warpvalue, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition + midPoint * Projectile.scale * 1.1f, new Color(0, warpvalue, 0, 1), new Vector3(factor, 0, 1)));
			}

			bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(dir, warpvalue, 0, 1), new Vector3(factor, 1, 1)));
			bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition + trail[i] * Projectile.scale * 1.1f, new Color(dir, warpvalue, 0, 1), new Vector3(factor, 0, 1)));
		}
		bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, Color.Transparent, new Vector3(0, 0, 0)));
		bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, Color.Transparent, new Vector3(0, 0, 0)));
		for (int i = 0; i < length; i++)
		{
			float factor = i / (length - 1f);

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
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(0, warpvalue, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition - midPoint * Projectile.scale * 1.1f, new Color(0, warpvalue, 0, 1), new Vector3(factor, 0, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(1, warpvalue, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition - midPoint * Projectile.scale * 1.1f, new Color(1, warpvalue, 0, 1), new Vector3(factor, 0, 1)));
			}
			if (dir1 - dir > 0.5)
			{
				var midValue = (1 - dir1) / (1 - dir1 + dir);
				var midPoint = midValue * trail[i - 1] + (1 - midValue) * trail[i];
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(1, warpvalue, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition - midPoint * Projectile.scale * 1.1f, new Color(1, warpvalue, 0, 1), new Vector3(factor, 0, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(0, warpvalue, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition - midPoint * Projectile.scale * 1.1f, new Color(0, warpvalue, 0, 1), new Vector3(factor, 0, 1)));
			}

			bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(dir, warpvalue, 0, 1), new Vector3(factor, 1, 1)));
			bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition - trail[i] * Projectile.scale * 1.1f, new Color(dir, warpvalue, 0, 1), new Vector3(factor, 0, 1)));
		}

		spriteBatch.Draw(ModContent.Request<Texture2D>(Commons.ModAsset.Melee_Warp_Mod).Value, bars, PrimitiveType.TriangleStrip);
	}

	public float TrailAlpha(float factor)
	{
		float w;
		w = MathHelper.Lerp(0f, 1, factor);
		return w;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		float point = 0;
		Vector2 hitRange = new Vector2(HitLength, HitLength * Projectile.spriteDirection).RotatedBy(Projectile.rotation) * Projectile.scale;
		if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - hitRange, Projectile.Center + hitRange, 10 * HitLength / 32f * Omega / 0.3f, ref point) && Projectile.timeLeft < 550)
		{
			return true;
		}

		return false;
	}

	private void ProduceWaterRipples(Vector2 beamDims)
	{
		var shaderData = (WaterShaderData)Terraria.Graphics.Effects.Filters.Scene["WaterDistortion"].GetShader();
		float waveSine = 1f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 20f);
		Vector2 hitRange = new Vector2(HitLength, -HitLength).RotatedBy(Projectile.rotation) * Projectile.scale;
		Vector2 ripplePos = Projectile.Center + hitRange;
		Vector2 ripplePosII = Projectile.Center - hitRange;
		Color waveData = new Color(0.5f, 0.1f * Math.Sign(waveSine) + 0.5f, 0f, 1f) * Math.Abs(waveSine);
		shaderData.QueueRipple(ripplePos, waveData, beamDims, RippleShape.Square, Projectile.rotation + MathHelper.Pi / 2f);
		shaderData.QueueRipple(ripplePosII, waveData, beamDims, RippleShape.Square, Projectile.rotation + MathHelper.Pi / 2f);
	}
}
