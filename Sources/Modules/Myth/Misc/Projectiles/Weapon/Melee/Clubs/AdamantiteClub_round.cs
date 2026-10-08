using Everglow.Commons.DataStructures;
using Everglow.Commons.VFX.CommonVFXDusts;
using Terraria.GameContent.Shaders;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class AdamantiteClub_round : ModProjectile, IWarpProjectile
{
	public override string Texture => "Everglow/" + ModAsset.AdamantiteClub_Path;

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
		Projectile.width = 100;
		Projectile.height = 100;
		Projectile.penetrate = -1;

		Projectile.timeLeft = 580;

		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.localNPCHitCooldown = 60;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 1;

		Projectile.DamageType = DamageClass.Melee;
		TrailVecs = new Queue<Vector2>(TrailLength + 1);
		StrikeOmegaDecrease = 0.99f;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		hit.HitDirection = target.Center.X > Main.player[Projectile.owner].Center.X ? 1 : -1;
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
		float power = Math.Max(StrikeOmegaDecrease - MathF.Pow(target.knockBackResist / 4f, 6), MinStrikeOmegaDecrease);
		Omega *= power;
		modifiers.FinalDamage /= power;
		modifiers.Knockback *= Omega * 3;
	}

	public override void AI()
	{
		if (DamageStartValue == 0)
		{
			DamageStartValue = Projectile.damage;
			Projectile.damage = 0;
			int count = 0;
			while (GetHitTimes(Projectile.Center) == 0)
			{
				count++;
				Projectile.Center += new Vector2(0, 10);
				if (count > 30)
				{
					break;
				}
			}
		}

		// 造成伤害等于原伤害*转速*3.334
		Projectile.damage = Math.Max((int)(DamageStartValue * Omega * 3.334), 1);

		Player player = Main.player[Projectile.owner];

		Projectile.rotation += Omega;
		float meleeSpeed = player.GetAttackSpeed(Projectile.DamageType);
		if (Projectile.timeLeft > 570)
		{
			Projectile.velocity *= 0.2f;
			if (Omega < meleeSpeed * MaxOmega)
			{
				Omega += Beta * meleeSpeed * 12f;
			}
		}
		else
		{
			if (Projectile.timeLeft == 570)
			{
				Projectile.friendly = true;
				Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 20f;
			}
			UpdateMovement();
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
		if (Projectile.timeLeft < 450)
		{
			Projectile.friendly = false;
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

		if (Projectile.timeLeft is > 450 and < 570)
		{
			Vector2 v2 = new Vector2(0, Main.rand.NextFloat(46f, 92f)).RotatedByRandom(Math.PI * 2) * 0.25f;
			Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, v2, ModContent.ProjectileType<AdamantiteClub_round_slash>(), Projectile.damage / 2, 0, player.whoAmI, Main.rand.NextFloat(-0.05f, 0.05f));
			p0.timeLeft = Main.rand.Next(118, 126);
			AdamantiteClub_round_slash aCRS = p0.ModProjectile as AdamantiteClub_round_slash;
			if (aCRS != null)
			{
				aCRS.StickProjectile = Projectile;
			}
			GenerateSpark((int)Projectile.velocity.Length() * 2);
		}
	}

	public int MoveTimer = 0;
	public Vector2 NormalToTiles;

	public void UpdateMovement()
	{
		int totalHit = 0;
		if (MoveTimer == 0)
		{
		}
		NormalToTiles = Vector2.zeroVector;
		for (int i = 0; i < 32; i++)
		{
			Vector2 checkPoint = new Vector2(50, 0).RotatedBy(i / 32f * MathHelper.TwoPi);
			if (TileUtils.PlatformCollision(checkPoint + Projectile.Center))
			{
				totalHit++;
				NormalToTiles -= checkPoint;
			}
			else
			{
				NormalToTiles += checkPoint;
			}
		}
		for (int i = 0; i < 24; i++)
		{
			Vector2 checkPoint = new Vector2(36, 0).RotatedBy(i / 24f * MathHelper.TwoPi);
			if (TileUtils.PlatformCollision(checkPoint + Projectile.Center))
			{
				totalHit += 3;
				NormalToTiles -= checkPoint * 2;
			}
			else
			{
				NormalToTiles += checkPoint * 2;
			}
		}
		float velocityLength = 20;
		if (Projectile.timeLeft < 550)
		{
			velocityLength = Math.Max(0, (Projectile.timeLeft - 450) / 5f);
		}
		velocityLength *= Projectile.ai[0];
		if (NormalToTiles.Length() > 0.1f)
		{
			NormalToTiles = Vector2.Normalize(NormalToTiles);
			float angle0 = Vector2.Dot(NormalToTiles.RotatedBy(MathHelper.PiOver2), Projectile.velocity) / Projectile.velocity.Length();
			float angle1 = Vector2.Dot(NormalToTiles.RotatedBy(-MathHelper.PiOver2), Projectile.velocity) / Projectile.velocity.Length();
			Projectile.velocity = angle0 > angle1
				? NormalToTiles.RotatedBy(MathHelper.PiOver2) * velocityLength
				: NormalToTiles.RotatedBy(-MathHelper.PiOver2) * velocityLength;
		}
		else
		{
			Projectile.velocity += new Vector2(0, 1);
		}
		int maxTo12Hit = 12;
		float adjustValue = 0;
		for (int i = -8; i <= 8; i++)
		{
			float value = i / 10f;
			Vector2 checkVel = Projectile.velocity.RotatedBy(value);
			int newHitTimes = Math.Abs(GetHitTimes(Projectile.Center + checkVel) - 12);
			if (newHitTimes < maxTo12Hit)
			{
				maxTo12Hit = newHitTimes;
				adjustValue = value;
			}
		}
		Projectile.velocity = Projectile.velocity.RotatedBy(adjustValue);
	}

	public int GetHitTimes(Vector2 checkPoint)
	{
		int totalHit = 0;
		for (int i = 0; i < 32; i++)
		{
			if (TileUtils.PlatformCollision(checkPoint + new Vector2(50, 0).RotatedBy(i / 32f * MathHelper.TwoPi)))
			{
				totalHit++;
			}
		}
		for (int i = 0; i < 24; i++)
		{
			if (TileUtils.PlatformCollision(checkPoint + new Vector2(36, 0).RotatedBy(i / 24f * MathHelper.TwoPi)))
			{
				totalHit += 3;
			}
		}
		return totalHit;
	}

	public void GenerateSpark(int frequency)
	{
		float mulVelocity = Omega * 10;
		for (int g = 0; g < frequency; g++)
		{
			Vector2 newVelocity = NormalToTiles.RotatedBy(MathHelper.PiOver2 * (Main.rand.NextBool(2) ? 1 : -1)) * 2.6f * mulVelocity * Main.rand.NextFloat(0.1f, 2.0f);
			var spark = new FireSpark_MetalStabDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center - NormalToTiles * 26f,
				MaxTime = Main.rand.Next(27, 35),
				Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(4.1f, 27.0f)),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.01f, 0.01f) },
			};
			Ins.VFXManager.Add(spark);
		}
	}

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
		Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, texture.Size() / 2f, Projectile.scale, effects, 0f);
		for (int i = 0; i < 5; i++)
		{
			float alp = Omega / 0.1f;
			var color2 = new Color((float)((5 - i) / 5f * alp), (float)((5 - i) / 5f * alp * Omega), (float)((5 - i) / 5f * alp * Omega), 0);
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color2, Projectile.rotation - i * 0.75f * Omega, texture.Size() / 2f, Projectile.scale, effects, 0f);
		}
		DrawTrail();
		PostPreDraw();
		return false;
	}

	public void PostPreDraw()
	{
		float colorValue = Omega / 0.1f * (MathF.Sin((float)Main.time * 0.15f + Projectile.whoAmI * 4.32f) * 0.4f + 0.3f);
		var starDark = Commons.ModAsset.StarSlash_black.Value;
		var star = Commons.ModAsset.StarSlash.Value;
		Vector2 size = new Vector2(colorValue * 0.14f, 1f) * Projectile.scale * 1.4f;
		Color redSlash = new Color(colorValue, 0.002f * colorValue * colorValue, 0.002f * colorValue * colorValue, 0);

		Main.EntitySpriteDraw(starDark, Projectile.Center - Main.screenPosition - NormalToTiles * 40, null, Color.White * colorValue, NormalToTiles.ToRotation(), starDark.Size() / 2f, size, SpriteEffects.None, 0f);
		Main.EntitySpriteDraw(star, Projectile.Center - Main.screenPosition - NormalToTiles * 40, null, redSlash * colorValue, NormalToTiles.ToRotation(), star.Size() / 2f, size, SpriteEffects.None, 0f);
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

		for (int i = 0; i < length; i++)
		{
			float factor = i / (length - 1f);
			float w = 1 - Math.Abs((trail[i].X * 0.5f + trail[i].Y * 0.5f) / trail[i].Length());
			float w2 = MathF.Sqrt(TrailAlpha(factor));
			w *= w2 * w;
			bars.Add(new Vertex2D(Projectile.Center + trail[i] * 0.1f * Projectile.scale, Color.White, new Vector3(factor, 1, 0f)));
			bars.Add(new Vertex2D(Projectile.Center + trail[i] * Projectile.scale, Color.White, new Vector3(factor, 0, w * 1f)));
		}
		bars.Add(new Vertex2D(Projectile.Center, Color.Transparent, new Vector3(0, 0, 0)));
		bars.Add(new Vertex2D(Projectile.Center, Color.Transparent, new Vector3(0, 0, 0)));
		for (int i = 0; i < length; i++)
		{
			float factor = i / (length - 1f);
			float w = 1 - Math.Abs((trail[i].X * 0.5f + trail[i].Y * 0.5f) / trail[i].Length());
			float w2 = MathF.Sqrt(TrailAlpha(factor));
			w *= w2 * w;
			bars.Add(new Vertex2D(Projectile.Center - trail[i] * 0.1f * Projectile.scale, Color.White, new Vector3(factor, 1, 0f)));
			bars.Add(new Vertex2D(Projectile.Center - trail[i] * Projectile.scale, Color.White, new Vector3(factor, 0, w * 1f)));
		}
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, TrailBlendState(), SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone);
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;

		Effect meleeTrail = Commons.ModAsset.MetalClubTrail.Value;
		meleeTrail.Parameters["uTransform"].SetValue(model * projection);
		Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>(TrailShapeTex(), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

		meleeTrail.Parameters["tex1"].SetValue((Texture2D)ModContent.Request<Texture2D>(Texture));

		var lightColor = Lighting.GetColor((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16)).ToVector4();
		lightColor.W = 4f * Omega;
		meleeTrail.Parameters["Light"].SetValue(lightColor);
		meleeTrail.CurrentTechnique.Passes["TrailByOrigTex"].Apply();

		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}

	public void DrawWarp(VFXBatch spriteBatch)
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
		float warpValue = Omega * 0.1f;

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
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(0, warpValue, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition + midPoint * Projectile.scale * 1.1f, new Color(0, warpValue, 0, 1), new Vector3(factor, 0, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(1, warpValue, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition + midPoint * Projectile.scale * 1.1f, new Color(1, warpValue, 0, 1), new Vector3(factor, 0, 1)));
			}
			if (dir1 - dir > 0.5)
			{
				var midValue = (1 - dir1) / (1 - dir1 + dir);
				var midPoint = midValue * trail[i - 1] + (1 - midValue) * trail[i];
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(1, warpValue, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition + midPoint * Projectile.scale * 1.1f, new Color(1, warpValue, 0, 1), new Vector3(factor, 0, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(0, warpValue, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition + midPoint * Projectile.scale * 1.1f, new Color(0, warpValue, 0, 1), new Vector3(factor, 0, 1)));
			}

			bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(dir, warpValue, 0, 1), new Vector3(factor, 1, 1)));
			bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition + trail[i] * Projectile.scale * 1.1f, new Color(dir, warpValue, 0, 1), new Vector3(factor, 0, 1)));
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
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(0, warpValue, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition - midPoint * Projectile.scale * 1.1f, new Color(0, warpValue, 0, 1), new Vector3(factor, 0, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(1, warpValue, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition - midPoint * Projectile.scale * 1.1f, new Color(1, warpValue, 0, 1), new Vector3(factor, 0, 1)));
			}
			if (dir1 - dir > 0.5)
			{
				var midValue = (1 - dir1) / (1 - dir1 + dir);
				var midPoint = midValue * trail[i - 1] + (1 - midValue) * trail[i];
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(1, warpValue, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition - midPoint * Projectile.scale * 1.1f, new Color(1, warpValue, 0, 1), new Vector3(factor, 0, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(0, warpValue, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition - midPoint * Projectile.scale * 1.1f, new Color(0, warpValue, 0, 1), new Vector3(factor, 0, 1)));
			}

			bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(dir, warpValue, 0, 1), new Vector3(factor, 1, 1)));
			bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition - trail[i] * Projectile.scale * 1.1f, new Color(dir, warpValue, 0, 1), new Vector3(factor, 0, 1)));
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
		return base.Colliding(projHitbox, targetHitbox);
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
