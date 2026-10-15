using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.Audio;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Summon;

public class ActivatedJellyGlandMinion : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonProjectiles;

	private enum State
	{
		Patrol,
		Chase,
		Attack,
	}

	private enum ChaseState
	{
		Dash,
		Rest,
	}

	private const int NoTarget = -1;
	private const int SearchDistance = 1200;
	private const float ExplodeStateDistance = 200f;
	private const int ExplosionDuration = 50;
	private const float ExplosionScaleFrequencyLimit = 0.4f;

	private int TargetWhoAmI { get; set; }

	private float Timer
	{
		get => Projectile.ai[0];
		set => Projectile.ai[0] = value;
	}

	private State MinionState { get; set; } = State.Patrol;

	private ChaseState MinionChaseState { get; set; } = ChaseState.Dash;

	private Vector3 BloomLightColor { get; set; } = new Vector3(0f, 0.5f, 1f);

	private Vector2 TargetCenter { get; set; } = Vector2.Zero;

	private float ExplosionScaleFrequency { get; set; } = 0.2f;

	private Player Owner => Main.player[Projectile.owner];

	private bool HasTarget => TargetWhoAmI != NoTarget;

	public override void SetStaticDefaults()
	{
		Main.projFrames[Projectile.type] = 5;
	}

	public override void SetDefaults()
	{
		Projectile.tileCollide = false;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 20;
		Projectile.timeLeft = 600;

		Projectile.tileCollide = true;
		Projectile.penetrate = -1;
		Projectile.aiStyle = -1;

		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.scale = 0.6f;

		Projectile.DamageType = DamageClass.Summon;
		Projectile.minion = true;
		Projectile.minionSlots = 0;

		ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;

		ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
		ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;

		Main.projPet[Projectile.type] = true;
	}

	public override bool? CanCutTiles() => true;

	public override bool MinionContactDamage() => false;

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => behindNPCsAndTiles.Add(index);

	public override void AI()
	{
		BaseMinionAI();

		GeneralBehavior_Bloom();

		switch (MinionState)
		{
			case State.Patrol:
				Patrol();
				break;
			case State.Chase:
				Chase();
				break;
			case State.Attack:
				Attack();
				break;
		}
	}

	#region AI

	private void BaseMinionAI()
	{
		// Update frame
		if (Main.time % 10 == 0)
		{
			Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
		}

		// Check owner
		if (Owner.dead || !Owner.active)
		{
			Projectile.Kill();
			return;
		}

		// Check and update target
		if (!HasTarget)
		{
			TargetWhoAmI = NoTarget;
		}
		else
		{
			NPC target = Main.npc[TargetWhoAmI];
			if (!target.active || target.dontTakeDamage || !target.CanBeChasedBy())
			{
				TargetWhoAmI = NoTarget;
			}
		}
	}

	private void SearchTarget()
	{
		var oldTarget = TargetWhoAmI;

		int target = -1;
		float minDis = SearchDistance;
		foreach (NPC npc in Main.npc)
		{
			if (npc != null && npc.active)
			{
				if (npc.CanBeChasedBy() && !npc.dontTakeDamage && npc.life > 0)
				{
					float dis = (npc.Center - Projectile.Center).Length() - npc.Hitbox.Size().Length() * 0.5f;
					if (dis < minDis)
					{
						minDis = dis;
						target = npc.whoAmI;
					}
				}
			}
		}
		TargetWhoAmI = target;

		if (oldTarget != TargetWhoAmI)
		{
			Projectile.netUpdate = true;
		}
	}

	private void GeneralBehavior_Bloom()
	{
		float glowStrength = 0.4f;
		if (HasTarget)
		{
			if (Projectile.frame == 1)
			{
				BloomLightColor = Vector3.Lerp(BloomLightColor, new Vector3(0.5f, 2f, 4f) * glowStrength, 0.3f);
			}
			else if (Projectile.frame == 2)
			{
				BloomLightColor = Vector3.Lerp(BloomLightColor, new Vector3(0f, 1f, 2f) * glowStrength, 0.3f);
			}
			else
			{
				BloomLightColor = Vector3.Lerp(BloomLightColor, new Vector3(0f, 0.3f, 0.6f) * glowStrength, 0.3f);
			}
		}
		else
		{
			BloomLightColor = Vector3.Lerp(BloomLightColor, new Vector3(0f, 0.5f, 1f) * glowStrength, 0.3f);
		}
		Lighting.AddLight(Projectile.Center, BloomLightColor);
	}

	public void Patrol()
	{
		Vector2 targetVel = new Vector2(MathF.Sin((float)Main.time * 0.03f + Projectile.whoAmI), MathF.Sin((float)Main.time * 0.06f + MathF.PI + Projectile.whoAmI)) * Projectile.scale;
		Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVel, 0.4f);
		Projectile.rotation = Math.Clamp(Projectile.velocity.X * 0.3f, -1f, 1f);

		SearchTarget();
		if (HasTarget)
		{
			MinionState = State.Chase;
		}
	}

	private void Chase()
	{
		if (HasTarget)
		{
			NPC target = Main.npc[TargetWhoAmI];
			TargetCenter = target.Center;
		}
		else
		{
			MinionState = State.Patrol;
			return;
		}
		Vector2 movement = TargetCenter - Projectile.Center;

		switch (MinionChaseState)
		{
			case ChaseState.Dash:
				var DashSpeed = 3f;
				Projectile.velocity = (movement + new Vector2(0, MathF.Sin((float)Main.time * 0.0005f + Projectile.whoAmI * 7 + 2.1f))).NormalizeSafe() * DashSpeed;
				Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

				const int DashDuration = 15;
				if (++Timer > DashDuration)
				{
					Timer = 0;
					MinionChaseState = ChaseState.Rest;
				}
				break;
			case ChaseState.Rest:
				Projectile.velocity *= 0.92f;
				if (Projectile.velocity.Length() <= 0.5f)
				{
					MinionChaseState = ChaseState.Dash;
				}
				break;
		}

		if (movement.Length() <= ExplodeStateDistance)
		{
			Timer = 0;
			MinionState = State.Attack;
		}
	}

	private void Attack()
	{
		// Decelerate until stop
		if (Projectile.velocity.Length() > 0.5f)
		{
			Projectile.velocity *= 0.92f;
		}
		else if (Projectile.velocity.Length() > 0)
		{
			Projectile.velocity = Vector2.Zero;
		}

		// Update target position if target is available
		if (HasTarget)
		{
			NPC target = Main.npc[TargetWhoAmI];
			TargetCenter = target.Center;
		}
		var direction = (TargetCenter - Projectile.Center).NormalizeSafe();

		// Do random stretching and squishing frequently before explosion
		ExplosionScaleFrequency += ExplosionScaleFrequency < ExplosionScaleFrequencyLimit ? 0.005f : 0f;
		Projectile.scale = 0.5f + 0.1f * MathF.Sin(Timer * 1.5f * ExplosionScaleFrequency);

		// Keep projectile active before explosion is happening
		Projectile.timeLeft += 2;
		if (++Timer > ExplosionDuration)
		{
			SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.Center);
			Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ActivatedJellyGlandExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
			Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, direction.NormalizeSafe() * 10f, ModContent.ProjectileType<ActivatedJellyGlandBeam>(), Projectile.damage, Projectile.knockBack, Projectile.owner, TargetWhoAmI);
			Projectile.Kill();
		}
	}

	#endregion

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		modifiers.FinalDamage *= 0.15f;

		base.ModifyHitNPC(target, ref modifiers);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		float glowStrength = HasTarget ? 1f : 0.4f;

		Texture2D textureMain = ModAsset.JellyBall.Value;
		Texture2D textureGlow = ModAsset.JellyBall_glow.Value;
		Texture2D textureBloom = ModAsset.JellyBall_bloom.Value;

		Color drawColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
		int frameWidth = textureMain.Width / 2;
		int frameHeight = textureMain.Height / 5;
		int frameX = HasTarget ? 0 : frameWidth;
		int frameY = frameHeight * (Projectile.frame % 5);

		var sourceRect = new Rectangle(frameX, frameY, frameWidth, frameHeight);

		int bloomFrameWidth = textureBloom.Width / 2;
		int bloomFrameHeight = textureBloom.Height / 5;
		int bloomFrameX = HasTarget ? 0 : bloomFrameWidth;
		int bloomFrameY = bloomFrameHeight * (Projectile.frame % 5);
		var bloomSourceRect = new Rectangle(bloomFrameX, bloomFrameY, bloomFrameWidth, bloomFrameHeight);

		Main.spriteBatch.Draw(textureMain, Projectile.Center - Main.screenPosition, sourceRect, Color.Lerp(drawColor * 0.7f, new Color(0.6f, 1f, 1f, 1f), 0.4f), Projectile.rotation, new Vector2(textureMain.Width / 4f, textureMain.Height / 10f), Projectile.scale, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(textureBloom, Projectile.Center - Main.screenPosition, bloomSourceRect, new Color(1f, 1f, 1f, 0f) * glowStrength, Projectile.rotation, new Vector2(textureBloom.Width / 4f, textureBloom.Height / 10f), Projectile.scale, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(textureGlow, Projectile.Center - Main.screenPosition, sourceRect, new Color(1f, 1f, 1f, 0f) * glowStrength, Projectile.rotation, new Vector2(textureGlow.Width / 4f, textureGlow.Height / 10f), Projectile.scale, SpriteEffects.None, 0);

		return false;
	}

	public override void OnKill(int timeLeft)
	{
		if (Projectile.timeLeft > 200)
		{
			for (int i = 0; i < 12; i++)
			{
				var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<JellyBallGel>());
				d.scale *= Main.rand.NextFloat(0.7f, 1.4f);
				d.velocity = new Vector2(Main.rand.NextFloat(2, 6f), 0).RotatedByRandom(MathHelper.TwoPi);
			}
			for (int i = 0; i < 4; i++)
			{
				var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<JellyBallSpark>());
				d.velocity = new Vector2(Main.rand.NextFloat(2, 6f), 0).RotatedByRandom(MathHelper.TwoPi);
			}
			for (int g = 0; g < 32; g++)
			{
				Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(2, 24)).RotatedByRandom(MathHelper.TwoPi);
				float mulScale = Main.rand.NextFloat(6f, 12f);
				var blood = new JellyBallGelDrop
				{
					Velocity = afterVelocity / mulScale,
					Active = true,
					Visible = true,
					Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
					MaxTime = Main.rand.Next(42, 84),
					Scale = mulScale,
					Rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
				};
				Ins.VFXManager.Add(blood);
			}
			for (int g = 0; g < 8; g++)
			{
				Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(2, 4)).RotatedByRandom(MathHelper.TwoPi);
				var blood = new JellyBallGelSplash
				{
					Velocity = afterVelocity,
					Active = true,
					Visible = true,
					Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) - afterVelocity,
					MaxTime = Main.rand.Next(32, 94),
					Scale = Main.rand.NextFloat(6f, 24f),
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.4f), 0 },
				};
				Ins.VFXManager.Add(blood);
			}
		}
		else
		{
			for (int i = 0; i < 4; i++)
			{
				var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<JellyBallGel>());
				d.scale *= Main.rand.NextFloat(0.6f, 1.3f);
				d.velocity = new Vector2(Main.rand.NextFloat(1, 3f), 0).RotatedByRandom(MathHelper.TwoPi);
			}
			for (int i = 0; i < 1; i++)
			{
				var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<JellyBallSpark>());
				d.velocity = new Vector2(Main.rand.NextFloat(2, 4f), 0).RotatedByRandom(MathHelper.TwoPi);
			}
		}
	}
}
