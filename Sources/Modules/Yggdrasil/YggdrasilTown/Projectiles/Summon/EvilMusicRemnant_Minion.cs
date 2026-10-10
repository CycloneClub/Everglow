using Everglow.Commons.Mechanics.ElementalDebuff.Debuffs;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Summon;

public class EvilMusicRemnant_Minion : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonProjectiles;

	public enum Minion_MainState
	{
		Spawn,
		Action,
	}

	public enum Minion_ActionState
	{
		Patrol,
		Chase,
		Attack,
		FlameAttack,
		Kill,
	}

	private const int TimeLeftMax = 300;
	private const float MaxDistanceToOwner = 1000f;
	private const int MaxTeleportCooldown = 60;
	private const int SpawnDuration = 120;

	private const int SearchDistance = 500;
	private const int KillTime = 30;
	private const int DashDistance = 200;
	private const int DashCooldown = 60;
	private const int FlameCooldown = 900;

	private int flameTime = -1;
	private int flameCooling = -1;

	private Vector2 dashStartPos;
	private Vector2 dashEndPos;
	private Vector2 flameTargetPos;
	private Vector2 killEndPos;

	private Queue<Vector2> oldEyeGlow = new Queue<Vector2>();

	public Minion_MainState MainState { get; set; } = Minion_MainState.Spawn;

	public Minion_ActionState ActionState { get; set; } = Minion_ActionState.Patrol;

	public Player Owner => Main.player[Projectile.owner];

	private int MinionIndex => (int)Projectile.ai[0];

	public int TargetWhoAmI
	{
		get => (int)Projectile.ai[1];
		set => Projectile.ai[1] = value;
	}

	private int Timer
	{
		get { return (int)Projectile.ai[2]; }
		set { Projectile.ai[2] = value; }
	}

	public bool ExplosionCommand = false;

	public int ExplosionTime = 120;

	public int TotalTime = 0;

	private int TeleportCooldown { get; set; }

	public NPC Target => TargetWhoAmI is >= 0 and < 200 ? Main.npc[TargetWhoAmI] : null;

	public float SpawnProgress => MathF.Min(1f, (TimeLeftMax - Projectile.timeLeft) / (float)SpawnDuration);

	public override void SetStaticDefaults()
	{
		Main.projPet[Projectile.type] = true;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
		ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
		ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
	}

	public override void SetDefaults()
	{
		Projectile.width = 50;
		Projectile.height = 50;

		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.tileCollide = false;
		Projectile.timeLeft = TimeLeftMax;

		Projectile.DamageType = DamageClass.Summon;
		Projectile.minion = true;
		Projectile.minionSlots = 0;

		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 30;

		Projectile.netImportant = true;

		TargetWhoAmI = -1;
		TotalTime = 0;
	}

	public override bool MinionContactDamage() => true;

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.velocity.Y = -0.5f;
	}

	public override void AI()
	{
		TotalTime++;
		UpdateLifeCycle();
		float skullRotation;
		if (Projectile.direction < 0)
		{
			skullRotation = Projectile.rotation - MathF.PI;
		}
		else
		{
			skullRotation = Projectile.rotation;
		}
		oldEyeGlow.Enqueue(Projectile.Center + new Vector2(12 * Projectile.direction, 3).RotatedBy(skullRotation));
		if (oldEyeGlow.Count > 15)
		{
			oldEyeGlow.Dequeue();
		}
		if (ExplosionCommand)
		{
			Projectile.minionSlots = 0;
			ActionState = Minion_ActionState.Kill;
			if (ExplosionTime >= 119)
			{
				if (Target != null && Target.active && !Target.friendly && !Target.dontTakeDamage)
				{
					killEndPos = Target.Center;
				}

				if (killEndPos == Vector2.zeroVector)
				{
					float closest = 20000;
					Vector2 minDisPos = Vector2.zeroVector;
					foreach (var proj in Main.projectile)
					{
						if (proj != null && proj.active)
						{
							if (proj.type == ModContent.ProjectileType<EvilMusicRemnant_Note_Mark>())
							{
								float distance = (proj.Center - Projectile.Center).Length();
								if (distance < closest)
								{
									closest = distance;
									minDisPos = proj.Center;
								}
							}
						}
					}
					killEndPos = minDisPos;
				}
				if (killEndPos == Vector2.zeroVector)
				{
					killEndPos = Owner.Center;
				}
			}
			ExplosionTime--;
			if (ExplosionTime > 100)
			{
				Projectile.rotation += (120 - ExplosionTime) / 40f;
			}
			else
			{
				Projectile.rotation += 0.5f;
			}
			if (ExplosionTime < 115)
			{
				var toTarget = (killEndPos - Projectile.Center - Projectile.velocity).NormalizeSafe() * 24;
				Projectile.velocity = Projectile.velocity * 0.9f + toTarget * 0.1f;

				// if(toTarget.X <= -23)
				// {
				// Main.NewText(killEndPos);
				// Main.NewText(Projectile.Center, Color.Teal);
				// }
				if (ExplosionTime <= 0 || (killEndPos - Projectile.Center).Length() < 30)
				{
					Projectile.Kill();
				}
			}
			return;
		}
		LimitDistanceFromOwner();

		if (MainState == Minion_MainState.Spawn)
		{
			GenerateSpawnMask();
			if (SpawnProgress > 0.6f)
			{
				Projectile.velocity = Vector2.Zero;
			}

			if (TimeLeftMax - Projectile.timeLeft > SpawnDuration)
			{
				MainState = Minion_MainState.Action;
				ActionState = Minion_ActionState.Patrol;
				TargetWhoAmI = -1;
			}
			flameTime = -1;
		}
		else if (MainState == Minion_MainState.Action)
		{
			if (!ExplosionCommand)
			{
				Projectile.minionSlots = 0;
			}
			else
			{
				Projectile.minionSlots = 0;
			}
			Action();
		}
		else
		{
			Projectile.minionSlots = 0;
			flameTime = -1;
		}
	}

	public void GetRotation()
	{
		if (!ExplosionCommand)
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
		}
	}

	public override void OnKill(int timeLeft)
	{
		Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<EvilMusicRemnant_Explosion>(), Projectile.damage * 4, Projectile.knockBack, Projectile.owner);
		for (int i = 0; i < 30; i++)
		{
			Dust.NewDust(Projectile.Center, 1, 1, DustID.Shadowflame, 0, 0);
		}
	}

	private void UpdateLifeCycle()
	{
		if (CheckOwnerActive())
		{
			Owner.AddBuff(ModContent.BuffType<Buffs.EvilMusicRemnant>(), 30);
			if (Projectile.timeLeft <= KillTime)
			{
				Projectile.timeLeft = KillTime;
			}
		}
	}

	/// <summary>
	/// Keep the distance between minion and owner within a certain amount
	/// </summary>
	private void LimitDistanceFromOwner()
	{
		if (TeleportCooldown > 0)
		{
			TeleportCooldown--;
		}
		else if (Projectile.Center.Distance(Owner.Center) > MaxDistanceToOwner)
		{
			TargetWhoAmI = -1;
			ActionState = Minion_ActionState.Patrol;

			// Teleport to
			TeleportCooldown = MaxTeleportCooldown;
			oldEyeGlow = new Queue<Vector2>();
			Projectile.position = Owner.MountedCenter + new Vector2((10 - MinionIndex * 30) * Owner.direction, -40 + MathF.Sin((float)Main.time * 0.04f - MinionIndex) * 35f);
		}
	}

	/// <summary>
	/// Check if owner is active
	/// </summary>
	/// <returns>
	/// active: true | inactive: false
	/// </returns>
	private bool CheckOwnerActive()
	{
		if (Owner.dead || Owner.active is false)
		{
			Owner.ClearBuff(ModContent.BuffType<Buffs.EvilMusicRemnant>());
			return false;
		}

		if (!Owner.HasBuff(ModContent.BuffType<Buffs.EvilMusicRemnant>()))
		{
			return false;
		}

		return true;
	}

	private void GenerateSpawnMask()
	{
		for (int i = 0; i < 4; i++)
		{
			float size = Main.rand.NextFloat(0.1f, 0.96f);
			var noteFlame = new EvilMusicRemnant_FlameDust
			{
				Velocity = new Vector2(0, Main.rand.NextFloat(0.3f, 1f)).RotatedByRandom(MathHelper.TwoPi) * 0.8f,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(24, 36) * 6 * (1 - SpawnProgress),
				Scale = 14f * size,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				Frame = Main.rand.Next(3),
				ai = [Main.rand.NextFloat(-0.8f, 0.8f)],
			};
			Ins.VFXManager.Add(noteFlame);
		}
	}

	private void Action()
	{
		GetRotation();
		flameCooling--;

		// If has target, check target active
		if (TargetWhoAmI >= 0 && !ProjectileUtils.MinionCheckTargetActive(TargetWhoAmI))
		{
			TargetWhoAmI = -1;
			ActionState = Minion_ActionState.Patrol;
			Projectile.netUpdate = true;
		}

		// If has no target, search target
		if (TargetWhoAmI < 0)
		{
			var targetWhoAmI = ProjectileUtils.FindTarget(Projectile.Center, SearchDistance);
			if (Owner.HasMinionAttackTargetNPC)
			{
				targetWhoAmI = Owner.MinionAttackTargetNPC;
			}
			if (targetWhoAmI >= 0)
			{
				TargetWhoAmI = targetWhoAmI;
				ActionState = Minion_ActionState.Chase;
				Projectile.netUpdate = true;
			}
			flameTime = -1;
		}

		// Switch action state
		if (ActionState == Minion_ActionState.Chase) // Phase: Chase
		{
			MoveTo(Target.Center);
			flameTime = -1;
			Timer++;
			Vector2 distanceToTarget = Target.Center - Projectile.Center;
			if (distanceToTarget.Length() <= DashDistance && Timer >= DashCooldown)
			{
				Timer = 0;
				dashStartPos = Projectile.Center;
				dashEndPos = Target.Center + distanceToTarget.NormalizeSafe() * (DashDistance - distanceToTarget.Length() + MinionIndex);
				ActionState = Minion_ActionState.Attack;
			}
			else if (distanceToTarget.Length() <= DashDistance * 2 && flameCooling <= 0)
			{
				Timer = 0;
				flameTargetPos = Target.Center;
				Projectile.velocity = (Target.Center - Projectile.Center).NormalizeSafe();
				ActionState = Minion_ActionState.FlameAttack;
			}
		}
		else if (ActionState == Minion_ActionState.Attack) // Phase: Attack
		{
			var dashProgress = Timer / 30f;
			var pos = dashStartPos + (dashEndPos - dashStartPos) * dashProgress;
			Projectile.velocity = pos - Projectile.Center;

			Timer++;
			if (Timer == 30)
			{
				Timer = 0;
				ActionState = Minion_ActionState.Chase;
			}
		}
		else if (ActionState == Minion_ActionState.FlameAttack) // Phase: Attack
		{
			flameTime = Timer;
			var flameProgress = Timer / 100f;
			if (Projectile.velocity.Length() > 0.001f)
			{
				Projectile.velocity *= 0.1f;
			}
			if (flameProgress is > 0.12f and < 0.9f)
			{
				if (Timer % 5 == 0)
				{
					float skullRotation;
					if (Projectile.direction < 0)
					{
						skullRotation = Projectile.rotation - MathF.PI;
					}
					else
					{
						skullRotation = Projectile.rotation;
					}
					Vector2 startCenter = Projectile.Center + new Vector2(6 * Projectile.direction, 20).RotatedBy(skullRotation);
					Vector2 targetCenter = flameTargetPos;
					Vector2 vel = (targetCenter - startCenter).NormalizeSafe() * 12f;
					Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), startCenter, vel, ModContent.ProjectileType<EvilMusicRemnant_Minion_Flame>(), (int)(Projectile.damage * 0.32f), Projectile.knockBack, Projectile.owner, 20);
				}
			}
			Timer++;
			if (Timer == 100)
			{
				flameTime = -1;
				Timer = 0;
				flameCooling = FlameCooldown;
				ActionState = Minion_ActionState.Chase;
			}
		}
		else // Phase: Patrol
		{
			Vector2 aim;
			const float NotMovingVelocity = 1E-05f;
			if (Owner.velocity.Length() > NotMovingVelocity) // Player is moving
			{
				aim = Owner.MountedCenter
					+ new Vector2(
						x: (10 - MinionIndex * 30 + MinionIndex * 10) * Owner.direction,
						y: -50 + MathF.Sin((float)Main.time * 0.04f - MinionIndex) * 35f);
			}
			else
			{
				aim = Owner.MountedCenter
					+ new Vector2(
						x: Owner.direction * (MathF.Cos((float)Main.time * 0.02f) * 60f - MinionIndex * 30),
						y: Owner.height + MathF.Sin((float)Main.time * 0.04f) * 30f);
			}
			MoveTo(aim);
		}
	}

	private void MoveTo(Vector2 aim)
	{
		Projectile.velocity *= 0.97f;

		Vector2 toAim = aim - Projectile.Center - Projectile.velocity;

		float timeValue = (float)(Main.time * 0.012f);
		Vector2 aimPosition = aim +
			new Vector2(
				80f * MathF.Sin(timeValue * 2f + Projectile.whoAmI) * Math.Clamp(Projectile.velocity.X, 1, MinionIndex + 1),
				(-10 + MinionIndex + 30f * MathF.Sin(timeValue * 0.18f + Projectile.whoAmI)) * 1)
			* Projectile.scale;
		if (toAim.Length() > 50)
		{
			Projectile.velocity += Vector2.Normalize(aimPosition - Projectile.Center - Projectile.velocity) * 0.18f * Projectile.scale;
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddElementalDebuffBuildUp(Main.player[Projectile.owner], NervousImpairmentDebuff.ID, 125);
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		var sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		var drawColor = lightColor * SpawnProgress * 0.7f;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(0, 0, 0)) * Main.GameViewMatrix.TransformationMatrix;
		Effect dissolve = Commons.ModAsset.DissolveWithLight.Value;
		float dissolveDuration = SpawnProgress;
		dissolve.Parameters["uTransform"].SetValue(model * projection);
		dissolve.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_perlin.Value);
		dissolve.Parameters["duration"].SetValue(dissolveDuration);
		dissolve.Parameters["uLightColor"].SetValue(drawColor.ToVector4());
		dissolve.Parameters["uDissolveColor"].SetValue(new Vector4(0f, 0f, 0f, 1f));
		dissolve.Parameters["uNoiseSize"].SetValue(2f);
		dissolve.Parameters["uNoiseXY"].SetValue(new Vector2(Projectile.ai[1], Projectile.ai[2]));
		dissolve.CurrentTechnique.Passes[0].Apply();

		var skullTexture = ModAsset.EvilMusicRemnant_Minion_Skull.Value;
		var chinTexture = ModAsset.EvilMusicRemnant_Minion_Chin.Value;
		var glowTexture = ModAsset.EvilMusicRemnant_Minion_Glow.Value;
		var scaleFactor = 1f;
		var scale = Projectile.scale * scaleFactor;

		float skullRotation;
		SpriteEffects skullSpriteEffect;
		if (Projectile.direction < 0)
		{
			skullRotation = Projectile.rotation - MathF.PI;
			skullSpriteEffect = SpriteEffects.FlipHorizontally;
		}
		else
		{
			skullRotation = Projectile.rotation;
			skullSpriteEffect = SpriteEffects.None;
		}

		// Skull
		var skullPosition = Projectile.Center - Main.screenPosition;
		var skullOrigin = skullTexture.Size() / 2;
		Main.spriteBatch.Draw(skullTexture, Projectile.Center - Main.screenPosition, null, drawColor, skullRotation, skullOrigin, scale, skullSpriteEffect, 0);

		// Chin
		var chinPositionOffset = new Vector2(-6 * Projectile.direction, 20).RotatedBy(skullRotation) * scaleFactor;
		var chinPosition = skullPosition + chinPositionOffset;
		var chinOrigin = new Vector2(0, 0);
		var chinRotation = skullRotation + 0.2f * MathF.Sin((float)Main.time * 0.6f + Projectile.whoAmI) * Projectile.direction;
		if (flameTime > 0)
		{
			chinRotation = skullRotation + (1 - flameTime / 100f) * Projectile.direction;
		}
		var chinSpriteEffect = SpriteEffects.None;
		if (Projectile.direction < 0)
		{
			chinOrigin = new Vector2(chinTexture.Width, 0);
			chinSpriteEffect = SpriteEffects.FlipHorizontally;
		}
		Main.spriteBatch.Draw(chinTexture, chinPosition, null, drawColor, chinRotation, chinOrigin, scale, chinSpriteEffect, 0);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);

		// Glow
		var glowPosition = skullPosition;
		var glowColor = new Color(1f, 1f, 1f, 0) * 0.6f * (MathF.Sin((float)Main.time * 0.06f + Projectile.whoAmI) + 2f) * dissolveDuration;
		Main.spriteBatch.Draw(glowTexture, glowPosition, null, glowColor, skullRotation, glowTexture.Size() / 2, scale, skullSpriteEffect, 0);
		DrawTrace(Main.spriteBatch);
		return false;
	}

	private void DrawTrace(SpriteBatch spriteBatch)
	{
		var sBS = Main.spriteBatch.GetState().Value;
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		Effect effect = Commons.ModAsset.Trailing.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.CurrentTechnique.Passes[0].Apply();
		var trailColor = new Color(44, 2, 185, 0);
		var oldGlowPos = oldEyeGlow.ToArray();
		for (int k = 0; k < 3; k++)
		{
			var bars = new List<Vertex2D>();
			var vectorWidth = new Vector2(30f, 0).RotatedBy(k / 3f * MathHelper.TwoPi);
			for (int i = 0; i < oldGlowPos.Count(); i++)
			{
				float width = 0f;
				if (oldGlowPos.Count() > 1)
				{
					width = i / (float)(oldGlowPos.Count() - 1);
				}
				bars.Add(oldGlowPos[i] + vectorWidth, trailColor, new Vector3(i / 40f, 0f, width));
				bars.Add(oldGlowPos[i], trailColor, new Vector3(i / 40f, 0.5f, width));
			}
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_8.Value;
			if (bars.Count > 3)
			{
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			}
		}

		spriteBatch.End();
		spriteBatch.Begin(sBS);
	}
}
