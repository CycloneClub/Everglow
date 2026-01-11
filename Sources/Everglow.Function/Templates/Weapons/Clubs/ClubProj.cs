using Everglow.Commons.DataStructures;
using Everglow.Commons.MEAC;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Shaders;

namespace Everglow.Commons.Templates.Weapons.Clubs;

public abstract class ClubProj : ModProjectile, IWarpProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 80;
		Projectile.height = 80;
		Projectile.penetrate = -1;
		Projectile.localNPCHitCooldown = 1;

		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.tileCollide = false;

		Projectile.DamageType = DamageClass.Melee;
		SetCustomDefaults();
		TrailVecs = new Queue<Vector2>(TrailLength + 1);
	}

	public virtual void SetCustomDefaults()
	{
	}

	private bool enableReflection = false;

	/// <summary>
	/// Angular velocity
	/// </summary>
	public float Omega { get; private set; } = 0;

	/// <summary>
	/// Angular accleration
	/// </summary>
	public float Beta { get; protected set; } = 0.003f;

	/// <summary>
	/// Max angular velocity, affected by player.meleespeed. You should NOT modify this in most cases.
	/// </summary>
	public float MaxOmega { get; protected set; } = 0.3f;

	/// <summary>
	/// Damage radiu;
	/// </summary>
	public float HitLength { get; protected set; } = 32f;

	/// <summary>
	/// Warp magnitude
	/// </summary>
	public float WarpValue { get; protected set; } = 0.06f;

	/// <summary>
	/// The decrease amount of angular velocity while hit a target, associates with target.knownBackResist.
	/// </summary>
	public float StrikeOmegaDecrease { get; protected set; } = 0.9f;

	/// <summary>
	/// The minimun of the angular velocity flat when hitting a extemely-high-knownBackResist target.(Default to 0.4f, means that it will lost 60% of angular velocity when hitting a target.)
	/// </summary>
	public float MinStrikeOmegaDecrease { get; protected set; } = 0.4f;

	/// <summary>
	/// A timer, you can modify it for playing audios.
	/// </summary>
	public float AudioTimer { get; private set; } = 3.14159f;

	/// <summary>
	/// Actually an internal parameter for calculating damage.
	/// </summary>
	public int DamageStartValue { get; private set; } = 0;

	/// <summary>
	/// Trail length
	/// </summary>
	public int TrailLength { get; protected set; } = 10;

	/// <summary>
	/// Trail vectors
	/// </summary>
	public Queue<Vector2> TrailVecs { get; private set; }

	/// <summary>
	/// Represents whether the club projectile can reflecting. Default to <c>false</c>.
	/// <para/> Set this to <c>true</c> will also set <see cref="Beta"/> and <see cref="MaxOmega"/> to corresponding default values.
	/// </summary>
	public bool EnableReflection
	{
		get => enableReflection;
		protected set
		{
			enableReflection = value;
			if (value)
			{
				Beta = MaxOmega == 0.003f ? 0.0024f : Beta;
				MaxOmega = MaxOmega == 0.3f ? 0.27f : MaxOmega;
			}
		}
	}

	/// <summary>
	/// Reflection strength. Default to <c>4f</c>.
	/// </summary>
	public float ReflectionStrength { get; protected set; } = 4f;

	/// <summary>
	/// Reflection texture. Default to <see cref="string.Empty"/>.
	/// </summary>
	public string ReflectionTexture { get; protected set; } = string.Empty;

	public virtual BlendState TrailBlendState() => BlendState.NonPremultiplied;

	public virtual string TrailShapeTex() => ModAsset.Melee_Mod;

	/// <summary>
	/// Normal alpha value calculation of trail
	/// </summary>
	/// <param name="factor"></param>
	/// <returns></returns>
	public virtual float TrailAlpha(float factor) => MathHelper.Lerp(0f, 1, factor);

	/// <summary>
	/// Special alpha value calculation of trail
	/// </summary>
	/// <param name="trailVector"></param>
	/// <param name="factor"></param>
	/// <returns></returns>
	protected virtual float SpecialTrailAlpha(Vector2 trailVector, float factor)
	{
		float w = 1 - Math.Abs((trailVector.X * 0.5f + trailVector.Y * 0.5f) / trailVector.Length());
		float w2 = MathF.Sqrt(TrailAlpha(factor));
		w *= w2 * w;

		if (EnableReflection)
		{
			w *= ReflectionStrength;
		}

		return w;
	}

	public override void OnSpawn(IEntitySource source) => Omega = MaxOmega * 0.5f;

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		float power = Math.Max(StrikeOmegaDecrease - MathF.Pow(target.knockBackResist / 4f, 3), MinStrikeOmegaDecrease);
		modifiers.HitDirectionOverride = target.Center.X > Main.player[Projectile.owner].Center.X ? 1 : -1;
		ScreenShaker Gsplayer = Main.player[Projectile.owner].GetModPlayer<ScreenShaker>();
		float ShakeStrength = Omega;
		Omega *= power;
		modifiers.FinalDamage /= power;
		Gsplayer.FlyCamPosition = new Vector2(0, Math.Min(target.Hitbox.Width * target.Hitbox.Height / 12f * ShakeStrength, 100)).RotatedByRandom(MathHelper.TwoPi);
		modifiers.Knockback *= Omega * 3;
	}

	public virtual void UpdateSound()
	{
		AudioTimer -= Omega;
		if (AudioTimer <= 0)
		{
			SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing.WithPitchOffset(-1 + Omega * 3f).WithVolumeScale(1 - Omega), Projectile.Center);
			AudioTimer = MathF.PI;
		}
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
		Vector2 MouseToPlayer = Main.MouseWorld - player.MountedCenter;
		MouseToPlayer = Vector2.Normalize(MouseToPlayer) * 15f;
		Vector2 vT0 = Main.MouseWorld - player.MountedCenter;
		player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (float)(Math.Atan2(vT0.Y * player.gravDir, vT0.X) - Math.PI / 2d));
		Projectile.Center = player.MountedCenter + MouseToPlayer;
		Projectile.spriteDirection = player.direction;
		Projectile.localNPCHitCooldown = (int)(MathF.PI / Math.Max(Omega, 0.157));

		// 这个受击冷却是个麻烦的问题
		// 旋转一周打两次，理论结果是Pi/Omega
		// 存在角加速过程
		// localNPCHitCooldown一旦命中就会开始计时，以当时的localNPCHitCooldown值倒计时。
		// 这个计时器还没归零，下一击已然命中。则这一击失效。
		// 如果设计极短，又会重复判断
		// 而且还考虑到怪物会动
		Projectile.rotation += Omega;
		if (HasContinueUsing())
		{
			float MeleeSpeed = player.GetAttackSpeed(Projectile.DamageType) * (8f / player.HeldItem.useTime);
			Projectile.scale = player.HeldItem.scale * (player.meleeScaleGlove ? 1.2f : 1);
			if (Omega < MeleeSpeed * MaxOmega)
			{
				Omega += Beta * MeleeSpeed;
			}
			else
			{
				Omega *= 0.9f;
			}

			if (Projectile.timeLeft < 22)
			{
				Projectile.timeLeft = 22;
			}
		}
		else
		{
			Omega *= 0.9f;
			if (Projectile.timeLeft > 22)
			{
				Projectile.timeLeft = 22;
			}
		}
		Vector2 HitRange = new Vector2(HitLength, HitLength * Projectile.spriteDirection).RotatedBy(Projectile.rotation) * MathF.Sqrt(Projectile.scale);
		TrailVecs.Enqueue(HitRange);
		if (TrailVecs.Count > TrailLength)
		{
			TrailVecs.Dequeue();
		}

		if (player.dead)
		{
			Projectile.Kill();
		}

		player.heldProj = Projectile.whoAmI;
		UpdateSound();
		ProduceWaterRipples(new Vector2(HitLength * MathF.Sqrt(Projectile.scale)));
	}

	public bool HasContinueUsing()
	{
		Player player = Main.player[Projectile.owner];
		if (player.controlUseItem && player.active && !player.dead)
		{
			if (player.HeldItem.shoot == Projectile.type)
			{
				return true;
			}
		}
		return false;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		SpriteEffects effects = SpriteEffects.None;
		if (Projectile.spriteDirection == 1)
		{
			effects = SpriteEffects.FlipHorizontally;
		}

		var texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
		Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2f, Projectile.scale * Projectile.scale, effects, 0f);
		for (int i = 0; i < 5; i++)
		{
			float alp = Omega / 0.4f * 0.5f;
			var color2 = new Color((int)(lightColor.R * (5 - i) / 5f * alp), (int)(lightColor.G * (5 - i) / 5f * alp), (int)(lightColor.B * (5 - i) / 5f * alp), (int)(lightColor.A * (5 - i) / 5f * alp));
			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, color2, Projectile.rotation - i * 0.1f * Omega, texture.Size() / 2f, Projectile.scale * Projectile.scale, effects, 0f);
		}
		DrawTrail();
		PostPreDraw();
		return false;
	}

	public virtual void PostPreDraw()
	{
		if (EnableReflection)
		{
			var bars = CreateTrailVertices(useSpecialAplha: true);
			if (bars == null)
			{
				return;
			}

			var lightColor = Lighting.GetColor((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16)).ToVector4();
			lightColor.W = 0.7f * Omega;
			var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
			var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;

			Texture2D projTexture = ReflectionTexture != string.Empty
				? ModContent.Request<Texture2D>(ReflectionTexture).Value
				: (Texture2D)ModContent.Request<Texture2D>(Texture);

			SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, TrailBlendState(), SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone);

			Effect clubTrailEffect = ModAsset.MetalClubTrail.Value;
			clubTrailEffect.Parameters["uTransform"].SetValue(model * projection);
			clubTrailEffect.Parameters["tex1"].SetValue(projTexture);
			clubTrailEffect.Parameters["Light"].SetValue(lightColor);
			clubTrailEffect.CurrentTechnique.Passes["TrailByOrigTex"].Apply();

			Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>(TrailShapeTex(), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(sBS);
		}
	}

	protected List<Vertex2D> CreateTrailVertices(float paramA = 0.1f, float paramB = 0.1f, bool useSpecialAplha = true, Color? trailColor = null)
	{
		if (!TrailVecs.Smooth(out var smoothedTrail))
		{
			return null;
		}

		var color = trailColor ?? Color.White;
		var length = smoothedTrail.Count;
		var vertices = new List<Vertex2D>();
		for (int i = 0; i < length; i++)
		{
			float factor = i / (length - 1f);
			float w = useSpecialAplha ? SpecialTrailAlpha(smoothedTrail[i], factor) : TrailAlpha(factor);
			vertices.Add(new Vertex2D(Projectile.Center + smoothedTrail[i] * paramA * Projectile.scale, color, new Vector3(factor, 1, 0f)));
			vertices.Add(new Vertex2D(Projectile.Center + smoothedTrail[i] * Projectile.scale, color, new Vector3(factor, 0, w)));
		}
		vertices.Add(new Vertex2D(Projectile.Center, Color.Transparent, new Vector3(0, 0, 0)));
		vertices.Add(new Vertex2D(Projectile.Center, Color.Transparent, new Vector3(0, 0, 0)));
		for (int i = 0; i < length; i++)
		{
			float factor = i / (length - 1f);
			float w = useSpecialAplha ? SpecialTrailAlpha(smoothedTrail[i], factor) : TrailAlpha(factor);
			vertices.Add(new Vertex2D(Projectile.Center - smoothedTrail[i] * paramB * Projectile.scale, color, new Vector3(factor, 1, 0f)));
			vertices.Add(new Vertex2D(Projectile.Center - smoothedTrail[i] * Projectile.scale, color, new Vector3(factor, 0, w)));
		}

		return vertices;
	}

	public virtual void DrawTrail()
	{
		var bars = CreateTrailVertices();
		if (bars == null)
		{
			return;
		}

		var lightColor = Lighting.GetColor((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16)).ToVector4();
		lightColor.W = 0.7f * Omega;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;

		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, TrailBlendState(), SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone);

		Effect clubTrailEffect = ModAsset.ClubTrail.Value;
		clubTrailEffect.Parameters["uTransform"].SetValue(model * projection);
		clubTrailEffect.Parameters["tex0"].SetValue(ModAsset.Noise_flame_0.Value);
		clubTrailEffect.Parameters["tex1"].SetValue((Texture2D)ModContent.Request<Texture2D>(Texture));
		clubTrailEffect.Parameters["Light"].SetValue(lightColor);
		clubTrailEffect.CurrentTechnique.Passes["TrailByOrigTex"].Apply();

		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		if (!TrailVecs.Smooth(out var trail))
		{
			return;
		}
		float warpFactor = WarpValue;
		var length = trail.Count;
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
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(0, Omega * warpFactor, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition + MidPoint * Projectile.scale * 1.1f, new Color(0, Omega * warpFactor, 0, 1), new Vector3(factor, 0, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(1, Omega * warpFactor, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition + MidPoint * Projectile.scale * 1.1f, new Color(1, Omega * warpFactor, 0, 1), new Vector3(factor, 0, 1)));
			}
			if (dir1 - dir > 0.5)
			{
				var MidValue = (1 - dir1) / (1 - dir1 + dir);
				var MidPoint = MidValue * trail[i - 1] + (1 - MidValue) * trail[i];
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(1, Omega * warpFactor, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition + MidPoint * Projectile.scale * 1.1f, new Color(1, Omega * warpFactor, 0, 1), new Vector3(factor, 0, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(0, Omega * warpFactor, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition + MidPoint * Projectile.scale * 1.1f, new Color(0, Omega * warpFactor, 0, 1), new Vector3(factor, 0, 1)));
			}

			bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(dir, Omega * warpFactor, 0, 1), new Vector3(factor, 1, 1)));
			bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition + trail[i] * Projectile.scale * 1.1f, new Color(dir, Omega * warpFactor, 0, 1), new Vector3(factor, 0, 1)));
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
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(0, Omega * warpFactor, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition - MidPoint * Projectile.scale * 1.1f, new Color(0, Omega * warpFactor, 0, 1), new Vector3(factor, 0, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(1, Omega * warpFactor, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition - MidPoint * Projectile.scale * 1.1f, new Color(1, Omega * warpFactor, 0, 1), new Vector3(factor, 0, 1)));
			}
			if (dir1 - dir > 0.5)
			{
				var MidValue = (1 - dir1) / (1 - dir1 + dir);
				var MidPoint = MidValue * trail[i - 1] + (1 - MidValue) * trail[i];
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(1, Omega * warpFactor, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition - MidPoint * Projectile.scale * 1.1f, new Color(1, Omega * warpFactor, 0, 1), new Vector3(factor, 0, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(0, Omega * warpFactor, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition - MidPoint * Projectile.scale * 1.1f, new Color(0, Omega * warpFactor, 0, 1), new Vector3(factor, 0, 1)));
			}

			bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(dir, Omega * warpFactor, 0, 1), new Vector3(factor, 1, 1)));
			bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition - trail[i] * Projectile.scale * 1.1f, new Color(dir, Omega * warpFactor, 0, 1), new Vector3(factor, 0, 1)));
		}

		spriteBatch.Draw(ModAsset.Melee_Warp.Value, bars, PrimitiveType.TriangleStrip);
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		Vector2 HitRange = new Vector2(HitLength, HitLength * Projectile.spriteDirection).RotatedBy(Projectile.rotation) * MathF.Sqrt(Projectile.scale);
		return CollisionUtils.Intersect(targetHitbox.Left(), targetHitbox.Right(), targetHitbox.Height, Projectile.Center - HitRange, Projectile.Center + HitRange, 2 * HitLength / 32f * Omega / 0.3f);
	}

	private void ProduceWaterRipples(Vector2 beamDims)
	{
		var shaderData = (WaterShaderData)Terraria.Graphics.Effects.Filters.Scene["WaterDistortion"].GetShader();
		float waveSine = 1f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 20f);
		Vector2 HitRange = new Vector2(HitLength, -HitLength).RotatedBy(Projectile.rotation) * MathF.Sqrt(Projectile.scale);
		Vector2 ripplePos = Projectile.Center + HitRange;
		Vector2 ripplePosII = Projectile.Center - HitRange;
		Color waveData = new Color(0.5f, 0.1f * Math.Sign(waveSine) + 0.5f, 0f, 1f) * Math.Abs(waveSine);
		shaderData.QueueRipple(ripplePos, waveData, beamDims, RippleShape.Square, Projectile.rotation + MathHelper.Pi / 2f);
		shaderData.QueueRipple(ripplePosII, waveData, beamDims, RippleShape.Square, Projectile.rotation + MathHelper.Pi / 2f);
	}
}
