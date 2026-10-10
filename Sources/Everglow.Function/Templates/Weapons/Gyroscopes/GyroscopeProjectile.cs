using Everglow.Commons.Templates.Weapons.Whips;
using Everglow.Commons.Utilities;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Commons.Templates.Weapons.Gyroscopes;

/// <summary>
/// Non-air summon projectile.
/// </summary>
public abstract class GyroscopeProjectile : ModProjectile
{
	public Player Owner => Main.player[Projectile.owner];

	public int EnemyTarget;

	public float MaxSearchRange = 450;

	public float MaxPower = 600;

	public float Power = 600;

	public float HitTargetPowerAddition = 30;

	public int SummonBuffType;

	public int FlyHitCooling = 0;

	public int FlyHitCoolingMax = 120;

	/// <summary>
	/// When take off for a targer strike, this set true. When touch down, turned false.
	/// </summary>
	public bool FlyingMode = false;

	/// <summary>
	/// Item1 : Projectile WhoAMI; Item2 : Cooling Timer
	/// </summary>
	public List<(int ProjectileWhoAmI, int CoolingTimer)> WhipCoolingsForProjectileWhoAmI = new List<(int, int)>();

	public override void SetDefaults()
	{
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 300;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.DamageType = DamageClass.Summon;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 12;
		Projectile.minionSlots = 1;
		Projectile.minion = true;
	}

	public virtual float MaxSpeed()
	{
		float maxSpeed = 10;
		if (Power < MaxPower * 4 / 6f)
		{
			maxSpeed = 7;
		}
		if (Power < MaxPower * 2 / 6f)
		{
			maxSpeed = 5;
		}
		if (Power < MaxPower * 1 / 6f)
		{
			maxSpeed = 0.5f;
		}
		return maxSpeed;
	}

	public override void OnSpawn(IEntitySource source)
	{
		EnemyTarget = -1;
		base.OnSpawn(source);
	}

	public override void AI()
	{
		CheckKill();
		if (Projectile.velocity.X > 1)
		{
			Projectile.spriteDirection = -1;
		}
		if (Projectile.velocity.X < -1)
		{
			Projectile.spriteDirection = 1;
		}
		FindFrame();

		if (EnemyTarget < 0)
		{
			EnemyTarget = FindEnemy();
		}
		if (EnemyTarget != -1)
		{
			AttackEnemies();
		}
		else
		{
			ApproachMyOwner();
		}
		CheckWhipHit();

		// Power = 600;
		Power -= 0.5f;
		if (Power < 1)
		{
			Projectile.Kill();
		}
		if (FlyHitCooling > 0)
		{
			FlyHitCooling--;
		}
		else
		{
			FlyHitCooling = 0;
		}
		CheckTooTar();
	}

	public virtual void CheckTooTar()
	{
		if ((Projectile.Center - Owner.Center).Length() > 2000 || Projectile.Center.Y > Owner.Center.Y + 800)
		{
			Projectile.Center = Owner.Center;
		}
	}

	/// <summary>
	/// When there are no enemies, try go back to player.
	/// </summary>
	public virtual void ApproachMyOwner()
	{
		Vector2 targetPos = FindTargetPosWhenNoEnemies();
		Vector2 toTarget = targetPos - Projectile.velocity - Projectile.Center;
		if (toTarget.Length() > 2400)
		{
			Projectile.active = false;
		}
		if (toTarget.Length() > 1)
		{
			toTarget = Vector2.Normalize(toTarget);
			Projectile.velocity.X += toTarget.X * 0.5f;
			if (Projectile.velocity.Length() > MaxSpeed())
			{
				Projectile.velocity = Vector2.Normalize(Projectile.velocity) * MaxSpeed();
			}
			if (!TileUtils.PlatformCollision(Projectile.Bottom))
			{
				Projectile.velocity.Y += 0.5f;
			}
			else
			{
				for (int i = 0; i < 100; i++)
				{
					if (TileUtils.PlatformCollision(Projectile.Bottom))
					{
						Projectile.position.Y -= 1;
						Power -= 0.2f;
					}
					else
					{
						break;
					}
				}
				BottomSpark(2);
			}
			if (Power > 400)
			{
				if (Collision.SolidCollision(Projectile.position + new Vector2(Projectile.velocity.X, 0), Projectile.width, Projectile.height))
				{
					Projectile.velocity.Y += -2;
				}
			}
		}
	}

	/// <summary>
	/// Choose a point close to player, oscillated by time and Projectile.WhoAmI.
	/// </summary>
	/// <returns></returns>
	public virtual Vector2 FindTargetPosWhenNoEnemies()
	{
		// A serrated function.
		Vector2 targetPos = Owner.Center + new Vector2(240 * MathF.Sin((float)Main.time * 0.04f + Projectile.whoAmI), 24);
		if (!TileUtils.PlatformCollision(targetPos))
		{
			int count = 0;
			while (!TileUtils.PlatformCollision(targetPos))
			{
				count++;
				targetPos.Y += 8;
				if (count > 40)
				{
					break;
				}
			}
		}
		if (TileUtils.PlatformCollision(targetPos))
		{
			int count = 0;
			while (TileUtils.PlatformCollision(targetPos))
			{
				count++;
				targetPos.Y -= 8;
				if (count > 40)
				{
					break;
				}
			}
		}
		return targetPos;
	}

	/// <summary>
	/// Attack.
	/// </summary>
	public virtual void AttackEnemies()
	{
		NPC npc = Main.npc[EnemyTarget];

		// if target is invalid, discard.
		if (npc == null || !npc.active || npc.life <= 0)
		{
			EnemyTarget = -1;
			return;
		}
		if ((npc.Center - Projectile.Center).Y < -300)
		{
			EnemyTarget = -1;
			return;
		}

		// oscillate over target.
		if (!FlyingMode)
		{
			Vector2 targetProjectToSurface = npc.Center + new Vector2(120 * MathF.Sin((float)Main.time * 0.05f + Projectile.whoAmI), 0);
			if (!TileUtils.PlatformCollision(targetProjectToSurface))
			{
				int count = 0;
				while (!TileUtils.PlatformCollision(targetProjectToSurface))
				{
					count++;
					targetProjectToSurface.Y += 8;
					if (count > 40)
					{
						break;
					}
				}
			}
			if (TileUtils.PlatformCollision(targetProjectToSurface))
			{
				int count = 0;
				while (TileUtils.PlatformCollision(targetProjectToSurface))
				{
					count++;
					targetProjectToSurface.Y -= 8;
					if (count > 40)
					{
						break;
					}
				}
			}
			Vector2 toTarget = targetProjectToSurface - Projectile.velocity - Projectile.Center;
			if (toTarget.Length() > 1)
			{
				toTarget = Vector2.Normalize(toTarget);
				Projectile.velocity.X += toTarget.X * 0.5f;
				if (Projectile.velocity.Length() > MaxSpeed())
				{
					Projectile.velocity = Vector2.Normalize(Projectile.velocity) * MaxSpeed();
				}
			}
			if (Power > 400)
			{
				if (Collision.SolidCollision(Projectile.position + new Vector2(toTarget.X * 16, 0), Projectile.width, Projectile.height))
				{
					Projectile.velocity.Y += -2;
				}
			}
			if (CanFlyHit(npc))
			{
				float timePredict = 8; // MathF.Sqrt(MathF.Abs(npc.Center.X - Projectile.Center.X)) * 2;
				Vector2 targetPos = npc.Center + npc.velocity * timePredict;

				// Main.NewText(timePredict);
				// Calculate a parabola.
				float distanceX = MathF.Abs(targetPos.X - Projectile.Center.X);
				float velX = MathF.Sqrt(distanceX) / 2;
				if (targetPos.X < Projectile.Center.X)
				{
					velX *= -1;
				}
				float velY = -2 * MathF.Abs(velX);
				float timeToHit = distanceX / MathF.Abs(velX);
				var takeOffVel = new Vector2(velX, velY);
				Projectile.velocity = takeOffVel;
				Projectile.position += Projectile.velocity;
				FlyingMode = true;
				FlyHitCooling = FlyHitCoolingMax;

				// Dust dust = Dust.NewDustDirect(targetPos, 0, 0, DustID.LastPrism);
				// dust.velocity *= 0;
				// dust.noGravity = true;
				// dust.scale = 3;
			}
		}

		if (!TileUtils.PlatformCollision(Projectile.position, Projectile.width, Projectile.height))
		{
			Projectile.velocity.Y += 0.5f;
		}
		else
		{
			FlyingMode = false;
			for (int i = 0; i < 100; i++)
			{
				if (TileUtils.PlatformCollision(Projectile.position, Projectile.width, Projectile.height))
				{
					Projectile.position.Y -= 1;
					Power -= 0.2f;
				}
				else
				{
					break;
				}
			}
			BottomSpark(2);
		}
	}

	public bool CanFlyHit(NPC target)
	{
		if (FlyHitCooling == 0)
		{
			if (TileUtils.PlatformCollision(Projectile.position, Projectile.width, Projectile.height + 16))
			{
				var checkPos = target.Center;
				float deltaY = checkPos.Y - Projectile.Center.Y;
				float deltaXOld = checkPos.X - (Projectile.Center.X - Projectile.velocity.X);
				float deltaX = checkPos.X - Projectile.Center.X;
				float deltaXNew = checkPos.X - (Projectile.Center.X + Projectile.velocity.X);
				float slope = Math.Abs(deltaY + Math.Abs(deltaX));
				float slopeOld = Math.Abs(deltaY + Math.Abs(deltaXOld));
				float slopeNew = Math.Abs(deltaY + Math.Abs(deltaXNew));

				// When the slope closest to 45 degree, take off.
				if (deltaY < -10 && deltaY > -200)
				{
					if (slope < slopeNew && slope < slopeOld)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	/// <summary>
	/// Generate spark VFX from bottom;
	/// </summary>
	/// <param name="count"></param>
	public virtual void BottomSpark(int count = 1)
	{
	}

	/// <summary>
	/// Whipping gyroscope can charge power.
	/// </summary>
	public virtual void CheckWhipHit()
	{
		foreach (var proj in Main.projectile)
		{
			if (proj != null && proj.active)
			{
				bool isAWhip = proj.aiStyle == ProjAIStyleID.Whip;
				if (!isAWhip)
				{
					if (proj.ModProjectile != null && proj.ModProjectile is WhipProjectile)
					{
						isAWhip = true;
					}
				}
				if (isAWhip)
				{
					int index = WhipCoolingsForProjectileWhoAmI.FindIndex(p => p.ProjectileWhoAmI == proj.whoAmI);
					if (index < 0)
					{
						if (proj.Colliding(proj.Hitbox, Projectile.Hitbox))
						{
							int whipValue = 150;
							CombatText.NewText(Projectile.Hitbox, new Color(1f, 1f, 1f, 1f), whipValue);
							Power += whipValue;
							if (Power > MaxPower)
							{
								Power = MaxPower;
							}
							WhipCoolingsForProjectileWhoAmI.Add((proj.whoAmI, 10));
							SoundEngine.PlaySound(SoundID.DrumTomHigh, Projectile.Center);
							WhipSpark(36);
							return;
						}
					}
				}
			}
		}
		for (int i = WhipCoolingsForProjectileWhoAmI.Count - 1; i > -1; i--)
		{
			if (WhipCoolingsForProjectileWhoAmI[i].CoolingTimer > 0)
			{
				WhipCoolingsForProjectileWhoAmI[i] = (WhipCoolingsForProjectileWhoAmI[i].ProjectileWhoAmI, WhipCoolingsForProjectileWhoAmI[i].CoolingTimer - 1);
			}
			else
			{
				WhipCoolingsForProjectileWhoAmI.RemoveAt(i);
			}
		}
	}

	/// <summary>
	/// Whipping VFX
	/// </summary>
	/// <param name="count"></param>
	public virtual void WhipSpark(int count = 20)
	{
	}

	/// <summary>
	/// Find a npc index which can be an enemy.
	/// </summary>
	/// <returns></returns>
	public int FindEnemy()
	{
		if (Owner.MinionAttackTargetNPC != -1)
		{
			return Owner.MinionAttackTargetNPC;
		}
		float minDetectionRange = MaxSearchRange;
		int targeWhoAmI = -1;
		foreach (NPC npc in Main.npc)
		{
			if (npc != null && npc.active && npc.life > 0 && !npc.friendly && npc.type != NPCID.TargetDummy && !npc.CountsAsACritter && npc.CanBeChasedBy() && !npc.dontTakeDamage)
			{
				float distance = (npc.Center - Owner.Center).Length();
				if (distance < MaxSearchRange)
				{
					if (Collision.CanHit(npc.Center - Vector2.One, 2, 2, Projectile.Center - Vector2.One, 2, 2) && (npc.Center - Projectile.Center).Y > -120)
					{
						if (npc.boss)
						{
							distance -= 600;
						}
						if (distance < minDetectionRange)
						{
							targeWhoAmI = npc.whoAmI;
							minDetectionRange = distance;
						}
					}
				}
			}
		}
		return targeWhoAmI;
	}

	/// <summary>
	/// Add power by 30;
	/// </summary>
	/// <param name="target"></param>
	/// <param name="hit"></param>
	/// <param name="damageDone"></param>
	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		Power += HitTargetPowerAddition;
		if (Power > MaxPower)
		{
			Power = MaxPower;
		}
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		if (Power > MaxPower * 5 / 6f)
		{
			modifiers.FinalDamage.Additive += 0.6f;
		}
		base.ModifyHitNPC(target, ref modifiers);
	}

	/// <summary>
	/// Get a unique order by Projectile.whoAmI.
	/// </summary>
	/// <returns></returns>
	public virtual int GetOrderFromOwner()
	{
		int count = 0;
		for (int i = 0; i < Main.projectile.Length; i++)
		{
			Projectile projectile = Main.projectile[i];
			if (projectile.active && projectile.owner == Owner.whoAmI)
			{
				if (projectile == Projectile)
				{
					return count;
				}
				if (projectile.type == Type)
				{
					count++;
				}
			}
		}
		return 0;
	}

	/// <summary>
	/// Low speed and high speed behave differently.
	/// </summary>
	public virtual void FindFrame()
	{
	}

	/// <summary>
	/// If rightclick to cancel the summon buff, remove projectile.
	/// </summary>
	public virtual void CheckKill()
	{
		Player player = Main.player[Projectile.owner];
		if (player.dead || !player.active)
		{
			player.ClearBuff(SummonBuffType);
			Projectile.Kill();
		}
		if (player.HasBuff(SummonBuffType))
		{
			Projectile.timeLeft = 2;
		}
		else
		{
			Projectile.Kill();
		}
	}

	/// <summary>
	/// Killing effect: some smoke.
	/// </summary>
	/// <param name="timeLeft"></param>
	public override void OnKill(int timeLeft)
	{
		if (Owner.ownedProjectileCounts[Type] <= 1)
		{
			Owner.ClearBuff(SummonBuffType);
		}
		for (int i = 0; i < 8; i++)
		{
			int type;
			switch (Main.rand.Next(3))
			{
				case 0:
					type = GoreID.ChimneySmoke1;
					break;
				case 1:
					type = GoreID.ChimneySmoke2;
					break;
				case 2:
					type = GoreID.ChimneySmoke3;
					break;
				default:
					type = GoreID.ChimneySmoke1;
					break;
			}

			var gore = Gore.NewGorePerfect(Projectile.Center + new Vector2(-20), new Vector2(0, Main.rand.NextFloat(1f)).RotatedByRandom(MathHelper.TwoPi), type);
			gore.timeLeft = Main.rand.Next(60, 120);
		}
		KillingSpark();
	}

	/// <summary>
	/// Killing VFX
	/// </summary>
	/// <param name="count"></param>
	public virtual void KillingSpark(int count = 20)
	{
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		return base.PreDraw(player, ref lightColor);
	}

	public virtual void DrawPowerBar()
	{
		float powerProgress = Power / MaxPower;
		var progressTexture = ModAsset.White.Value;
		var progressPosition = Projectile.Center - Main.screenPosition + Owner.gravDir * new Vector2(0, 36);

		var frameColor = new Color(0.05f, 0.05f, 0.08f, 0.9f);
		var frameColor2 = new Color(0.15f, 0.25f, 0.38f, 0.4f);
		Vector2 frameScale = new Vector2(2f, 0.4f) * 0.05f;
		Vector2 frameScale2 = new Vector2(1.8f, 0.2f) * 0.05f;

		Color lineColor = Color.White;
		Color lineColorInner = Color.White * 0.8f;
		lineColorInner.A = 255;
		Vector2 lineScaleOuter = new Vector2(2.2f * powerProgress + 0.2f, 0.7f) * 0.05f;
		Vector2 lineScale = new Vector2(2.2f * powerProgress, 0.5f) * 0.05f;
		Vector2 lineScale2 = new Vector2(2.2f * powerProgress - 0.2f, 0.3f) * 0.05f;
		var linePositionOffset = new Vector2(-2.2f * (1 - powerProgress) * progressTexture.Width * 0.025f, 0);

		Main.spriteBatch.Draw(progressTexture, progressPosition, null, frameColor, 0, progressTexture.Size() / 2, frameScale, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(progressTexture, progressPosition, null, frameColor2, 0, progressTexture.Size() / 2f, frameScale2, SpriteEffects.None, 0);

		Main.spriteBatch.Draw(progressTexture, progressPosition + linePositionOffset, null, frameColor, 0, progressTexture.Size() / 2, lineScaleOuter, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(progressTexture, progressPosition + linePositionOffset, null, lineColor, 0, progressTexture.Size() / 2, lineScale, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(progressTexture, progressPosition + linePositionOffset, null, lineColorInner, 0, progressTexture.Size() / 2, lineScale2, SpriteEffects.None, 0);
	}
}
