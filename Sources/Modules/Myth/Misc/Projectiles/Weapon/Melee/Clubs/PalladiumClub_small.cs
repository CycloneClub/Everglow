using Everglow.Commons.DataStructures;
using Terraria.DataStructures;
using Terraria.GameContent.Shaders;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class PalladiumClub_small : ModProjectile, IWarpProjectile
{
	/// <summary>
	/// 角速度
	/// </summary>
	internal float Omega = 0;
	/// <summary>
	/// 角加速度
	/// </summary>
	internal float Beta = 0.005f;
	/// <summary>
	/// 最大角速度(受近战攻速影响)
	/// </summary>
	internal float MaxOmega = 0.3f;
	/// <summary>
	/// 伤害半径
	/// </summary>
	internal float HitLength = 32f;
	/// <summary>
	/// 命中敌人后对于角速度的削减率(会根据敌人的击退抗性而再次降低)
	/// </summary>
	internal float StrikeOmegaDecrease = 0.9f;
	/// <summary>
	/// 命中敌人后最低剩余角速度(默认40%,即0.4)
	/// </summary>
	internal float MinStrikeOmegaDecrease = 0.4f;
	/// <summary>
	/// 内部参数，用来计算伤害
	/// </summary>
	internal int DamageStartValue = 0;
	/// <summary>
	/// 拖尾长度
	/// </summary>
	internal int trailLength = 10;
	/// <summary>
	/// 是否正在攻击
	/// </summary>
	internal bool isAttacking = false;
	/// <summary>
	/// 拖尾
	/// </summary>
	internal Queue<Vector2> trailVecs;
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

		trailVecs = new Queue<Vector2>(trailLength + 1);
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
	public float HealValue = 0;
	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		float power = Math.Max(StrikeOmegaDecrease - MathF.Pow(target.knockBackResist / 4f, 3), MinStrikeOmegaDecrease);
		if (target.type != NPCID.TargetDummy && HealValue < Projectile.ai[0] * 30)
		{
			HealValue++;
			CombatText.NewText(target.Hitbox, new Color(255, 125, 6, 255), "♥ + 1");
		}
		Omega *= power;
		modifiers.FinalDamage /= power;
		modifiers.Knockback *= Omega * 3;
	}
	public override void OnSpawn(IEntitySource source)
	{
		MaxOmega = Projectile.ai[0];
	}
	public override void AI()
	{
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
			Vector2 MouseToPlayer = Main.MouseWorld - player.MountedCenter;
			MouseToPlayer = Vector2.Normalize(MouseToPlayer) * 15f;
			Vector2 vT0 = Main.MouseWorld - player.MountedCenter;
			Projectile.Center = player.MountedCenter + MouseToPlayer;
			Projectile.spriteDirection = player.direction;
			if (Projectile.timeLeft == 550)
			{
				Projectile.velocity = vT0.SafeNormalize(Vector2.Zero) * 55;
				Projectile.friendly = true;
			}
		}
		if (Projectile.timeLeft < 550 && Projectile.timeLeft > 500)
		{
			Projectile.velocity *= 0.93f;
		}

		if (Projectile.timeLeft < 500)
		{
			Vector2 ProjToPlayer = player.MountedCenter - Projectile.Center;
			if (ProjToPlayer.Length() < 100 && Projectile.timeLeft > 20)
			{
				Projectile.timeLeft = 20;
			}

			ProjToPlayer = ProjToPlayer.SafeNormalize(Vector2.Zero) * 55;
			var value = Math.Max((Projectile.timeLeft - 400) / 100f, 0);
			Projectile.velocity = ProjToPlayer * (1 - value) + Projectile.velocity * value;
		}
		if (Projectile.timeLeft < 20)
		{
			Vector2 MouseToPlayer = Main.MouseWorld - player.MountedCenter;
			MouseToPlayer = Vector2.Normalize(MouseToPlayer) * 15f;
			Projectile.Center = player.MountedCenter + MouseToPlayer;
			Projectile.velocity *= 0;
		}
		Projectile.localNPCHitCooldown = (int)(MathF.PI / Math.Max(Omega, 0.157));
		// 这个受击冷却是个麻烦的问题
		// 旋转一周打两次，理论结果是Pi/Omega
		// 存在角加速过程
		// localNPCHitCooldown一旦命中就会开始计时，以当时的localNPCHitCooldown值倒计时。
		// 这个计时器还没归零，下一击已然命中。则这一击失效。
		// 如果设计极短，又会重复判断
		// 而且还考虑到怪物会动

		Projectile.rotation += Omega;
		if (Projectile.timeLeft > 120)
		{
			float MeleeSpeed = player.GetAttackSpeed(Projectile.DamageType);
			if (Omega < MeleeSpeed * MaxOmega)
			{
				Omega += Beta * MeleeSpeed * 4f;
			}
		}
		else
		{
			Omega *= 0.9f;
		}
		Vector2 HitRange = new Vector2(HitLength, HitLength * Projectile.spriteDirection).RotatedBy(Projectile.rotation) * Projectile.scale;
		trailVecs.Enqueue(HitRange);
		if (trailVecs.Count > trailLength)
		{
			trailVecs.Dequeue();
		}

		if (player.dead)
		{
			Projectile.Kill();
		}

		ProduceWaterRipples(new Vector2(HitLength * Projectile.scale));
	}
	public override bool PreDraw(ref Color lightColor)
	{
		SpriteEffects effects = SpriteEffects.None;
		if (Projectile.spriteDirection == 1)
		{
			effects = SpriteEffects.FlipHorizontally;
		}

		var texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
		lightColor.A = 150;
		Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor * Omega * 2, Projectile.rotation, texture.Size() / 2f, Projectile.scale, effects, 0f);
		for (int i = 0; i < 5; i++)
		{
			float alp = Omega / 0.4f;
			var color2 = new Color((int)(lightColor.R * (5 - i) / 5f * alp), (int)(lightColor.G * (5 - i) / 5f * alp), (int)(lightColor.B * (5 - i) / 5f * alp), 0);
			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, color2 * Omega * 2, Projectile.rotation - i * 0.75f * Omega, texture.Size() / 2f, Projectile.scale, effects, 0f);
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
		List<Vector2> SmoothTrailX = GraphicsUtils.CatmullRom(trailVecs.ToList()); // 平滑
		var SmoothTrail = new List<Vector2>();
		for (int x = 0; x < SmoothTrailX.Count - 1; x++)
		{
			SmoothTrail.Add(SmoothTrailX[x]);
		}
		if (trailVecs.Count != 0)
		{
			SmoothTrail.Add(trailVecs.ToArray()[trailVecs.Count - 1]);
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

		Effect MeleeTrail = Commons.ModAsset.MetalClubTrail.Value;
		MeleeTrail.Parameters["uTransform"].SetValue(model * projection);
		Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>(TrailShapeTex(), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

		MeleeTrail.Parameters["tex1"].SetValue((Texture2D)ModContent.Request<Texture2D>(Texture));

		var lightColor = Lighting.GetColor((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16)).ToVector4();
		lightColor.W = 0.7f * Omega;
		MeleeTrail.Parameters["Light"].SetValue(lightColor);
		MeleeTrail.CurrentTechnique.Passes["TrailByOrigTex"].Apply();

		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}
	public void DrawWarp(VFXBatch spriteBatch)
	{
		List<Vector2> SmoothTrailX = GraphicsUtils.CatmullRom(trailVecs.ToList()); // 平滑
		var SmoothTrail = new List<Vector2>();
		for (int x = 0; x < SmoothTrailX.Count - 1; x++)
		{
			SmoothTrail.Add(SmoothTrailX[x]);
		}
		if (trailVecs.Count != 0)
		{
			SmoothTrail.Add(trailVecs.ToArray()[trailVecs.Count - 1]);
		}

		int length = SmoothTrail.Count;
		if (length <= 3)
		{
			return;
		}

		float warpValue = Omega * 0.1f;
		Vector2[] trail = SmoothTrail.ToArray();
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
				var MidValue = (1 - dir) / (1 - dir + dir1);
				var MidPoint = MidValue * trail[i] + (1 - MidValue) * trail[i - 1];
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(0, warpValue, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition + MidPoint * Projectile.scale * 1.1f, new Color(0, warpValue, 0, 1), new Vector3(factor, 0, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(1, warpValue, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition + MidPoint * Projectile.scale * 1.1f, new Color(1, warpValue, 0, 1), new Vector3(factor, 0, 1)));
			}
			if (dir1 - dir > 0.5)
			{
				var MidValue = (1 - dir1) / (1 - dir1 + dir);
				var MidPoint = MidValue * trail[i - 1] + (1 - MidValue) * trail[i];
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(1, warpValue, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition + MidPoint * Projectile.scale * 1.1f, new Color(1, warpValue, 0, 1), new Vector3(factor, 0, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(0, warpValue, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition + MidPoint * Projectile.scale * 1.1f, new Color(0, warpValue, 0, 1), new Vector3(factor, 0, 1)));
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
				var MidValue = (1 - dir) / (1 - dir + dir1);
				var MidPoint = MidValue * trail[i] + (1 - MidValue) * trail[i - 1];
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(0, warpValue, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition - MidPoint * Projectile.scale * 1.1f, new Color(0, warpValue, 0, 1), new Vector3(factor, 0, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(1, warpValue, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition - MidPoint * Projectile.scale * 1.1f, new Color(1, warpValue, 0, 1), new Vector3(factor, 0, 1)));
			}
			if (dir1 - dir > 0.5)
			{
				var MidValue = (1 - dir1) / (1 - dir1 + dir);
				var MidPoint = MidValue * trail[i - 1] + (1 - MidValue) * trail[i];
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(1, warpValue, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition - MidPoint * Projectile.scale * 1.1f, new Color(1, warpValue, 0, 1), new Vector3(factor, 0, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(0, warpValue, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition - MidPoint * Projectile.scale * 1.1f, new Color(0, warpValue, 0, 1), new Vector3(factor, 0, 1)));
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
		float point = 0;
		Vector2 HitRange = new Vector2(HitLength, HitLength * Projectile.spriteDirection).RotatedBy(Projectile.rotation) * Projectile.scale;
		if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - HitRange, Projectile.Center + HitRange, 10 * HitLength / 32f * Omega / 0.3f, ref point) && Projectile.timeLeft < 550)
		{
			return true;
		}

		return false;
	}
	private void ProduceWaterRipples(Vector2 beamDims)
	{
		var shaderData = (WaterShaderData)Terraria.Graphics.Effects.Filters.Scene["WaterDistortion"].GetShader();
		float waveSine = 1f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 20f);
		Vector2 HitRange = new Vector2(HitLength, -HitLength).RotatedBy(Projectile.rotation) * Projectile.scale;
		Vector2 ripplePos = Projectile.Center + HitRange;
		Vector2 ripplePosII = Projectile.Center - HitRange;
		Color waveData = new Color(0.5f, 0.1f * Math.Sign(waveSine) + 0.5f, 0f, 1f) * Math.Abs(waveSine);
		shaderData.QueueRipple(ripplePos, waveData, beamDims, RippleShape.Square, Projectile.rotation + MathHelper.Pi / 2f);
		shaderData.QueueRipple(ripplePosII, waveData, beamDims, RippleShape.Square, Projectile.rotation + MathHelper.Pi / 2f);
	}
	public override void OnKill(int timeLeft)
	{
		Player player = Main.player[Projectile.owner];
		if (HealValue > 0 && (Projectile.Center - player.Center).Length() < 100)
		{
			player.Heal((int)HealValue);
		}
	}
}
