using Everglow.Yggdrasil.KelpCurtain.Buffs;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Summon;

public class Wither_Activated_Dog : ModProjectile
{
	/// <summary>
	/// 0: Stand; 1: Walk; 2: Run
	/// </summary>
	public int State = 0;

	public Player Owner => Main.player[Projectile.owner];

	public int EnemyTarget = -1;

	public float MaxSearchRange = 1600;

	/// <summary>
	/// Allow ignore gravity for a few seconds while attacking.
	/// </summary>
	public int AttackingFlyTimer = 0;

	/// <summary>
	/// Attacks Cooling.
	/// </summary>
	public int AttackingFlyCooling = 0;

	/// <summary>
	/// If jumped to hit a target, set true.
	/// </summary>
	public bool AttackSetOff = false;

	/// <summary>
	/// When there is no enemy, where should stay in.
	/// </summary>
	public Vector2 SitPosition = default;

	public Queue<Vector2> OldPositions = new Queue<Vector2>();

	public Queue<Vector2> OldDirectionsAnsOldFrames = new Queue<Vector2>();

	public int Timer = 0;

	public override void SetStaticDefaults()
	{
		Main.projFrames[Type] = 13;
		base.SetStaticDefaults();
	}

	public override void SetDefaults()
	{
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.width = 70;
		Projectile.height = 24;
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

	public override void AI()
	{
		Timer++;
		CheckKill();
		GravityBehavior();
		UpdateFrame();

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
		OldPositions.Enqueue(Projectile.Center);
		OldDirectionsAnsOldFrames.Enqueue(new Vector2(Projectile.spriteDirection, Projectile.frame));
		if (OldPositions.Count > 15)
		{
			OldPositions.Dequeue();
		}
		if (OldDirectionsAnsOldFrames.Count > 15)
		{
			OldDirectionsAnsOldFrames.Dequeue();
		}
	}

	public Vector2 FindTargetPosWhenNoEnemies()
	{
		for (int x = 0; x < 200; x++)
		{
			Vector2 ownerPos = Owner.Center + new Vector2((GetOrderFromOwner() + 3) * 48 * Owner.direction * -1 + x * (x % 2 - 0.5f) * 100, 0);
			Vector2 targetPos_Bottom = ownerPos;
			int safeCount = 0;
			for (int k = 0; k < 200; k++)
			{
				safeCount++;
				if (Collision.IsWorldPointSolid(targetPos_Bottom))
				{
					targetPos_Bottom.Y -= 4;
				}
				else
				{
					break;
				}
			}
			for (int k = 0; k < 200; k++)
			{
				safeCount++;
				if (!Collision.IsWorldPointSolid(targetPos_Bottom))
				{
					targetPos_Bottom.Y += 2;
				}
				else
				{
					break;
				}
			}
			if (safeCount <= 166)
			{
				return targetPos_Bottom;
			}
		}
		return Vector2.zeroVector;
	}

	/// <summary>
	/// Get a unique order by Projectile.whoAmI.
	/// </summary>
	/// <returns></returns>
	public int GetOrderFromOwner()
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
	/// When there are no enemies, try go back to player.
	/// </summary>
	public void ApproachMyOwner()
	{
		ResetAttackState();
		float maxSpeed = 5f;
		Vector2 targetPos = FindTargetPosWhenNoEnemies();
		if ((targetPos - SitPosition).Length() > 250 + MathF.Sin(Projectile.whoAmI) * 20)
		{
			SitPosition = targetPos;
		}
		Vector2 toTarget = SitPosition - Projectile.velocity - (Projectile.Bottom + new Vector2(0, 16));
		if (toTarget.Length() > 2400)
		{
			Projectile.Center = Owner.Center;
		}
		else if (toTarget.Length() > 300)
		{
			maxSpeed = 10f;
			var normalToTarget = Vector2.Normalize(toTarget);
			Projectile.velocity.X += normalToTarget.X * 0.5f;
			if (MathF.Abs(Projectile.velocity.X) > maxSpeed)
			{
				Projectile.velocity.X = Math.Sign(Projectile.velocity.X) * maxSpeed;
			}
			if (Collision.IsWorldPointSolid(Projectile.Bottom + new Vector2(0, 16)))
			{
				Projectile.velocity.Y -= 1.5f;
			}
		}
		else if (toTarget.Length() > 10)
		{
			var normalToTarget = Vector2.Normalize(toTarget);
			Projectile.velocity.X += normalToTarget.X * 0.5f;
			if (MathF.Abs(Projectile.velocity.X) > maxSpeed)
			{
				Projectile.velocity.X = Math.Sign(Projectile.velocity.X) * maxSpeed;
			}
			if (Collision.IsWorldPointSolid(Projectile.Bottom + new Vector2(0, 16)))
			{
				Projectile.velocity.Y -= 1f;
			}
		}
		else if (toTarget.Length() > 5)
		{
			Projectile.velocity *= 0.5f;
		}
		else
		{
			Projectile.velocity *= 0;
		}
		if (MathF.Abs(Projectile.velocity.X) > 5)
		{
			State = 2;
		}
		else if (MathF.Abs(Projectile.velocity.X) > 1)
		{
			State = 1;
		}
		else
		{
			State = 0;
		}

		if (Projectile.velocity.X < -1)
		{
			Projectile.spriteDirection = -1;
		}
		if (Projectile.velocity.X > 1)
		{
			Projectile.spriteDirection = 1;
		}
	}

	/// <summary>
	/// Attack.
	/// </summary>
	public void AttackEnemies()
	{
		NPC npc = Main.npc[EnemyTarget];

		// if target is invalid, discard.
		if (npc == null || !npc.active || npc.life <= 0)
		{
			EnemyTarget = -1;
			ResetAttackState();
			return;
		}
		if ((npc.Center - Projectile.Center).Y < -300)
		{
			EnemyTarget = -1;
			ResetAttackState();
			return;
		}

		if (AttackingFlyCooling <= 0)
		{
			AttackingFlyTimer++;
			if (AttackingFlyTimer >= 60)
			{
				ResetAttackState();
			}
			float maxSpeed = 10f;
			Vector2 targetPos = npc.Center;
			Vector2 toTarget = targetPos - Projectile.Center;
			if (toTarget.Length() < 20)
			{
				ResetAttackState();
			}
			if (toTarget.Length() <= 240 && !AttackSetOff)
			{
				AttackSetOff = true;
				Projectile.friendly = true;
				Projectile.velocity = toTarget.NormalizeSafe() * 24f;
			}
			if (!AttackSetOff)
			{
				toTarget = Vector2.Normalize(toTarget);
				Projectile.velocity.X += toTarget.X * 0.5f;
				if (MathF.Abs(Projectile.velocity.X) > maxSpeed)
				{
					Projectile.velocity.X = Math.Sign(Projectile.velocity.X) * maxSpeed;
				}
			}
			if (AttackSetOff)
			{
				float cosTheta = Vector2.Dot(Projectile.velocity.NormalizeSafe(), toTarget.NormalizeSafe());
				if (cosTheta <= 0)
				{
					ResetAttackState();
				}
			}
		}
		else
		{
			AttackSetOff = false;
			AttackingFlyTimer = 0;
			if (Projectile.velocity.Y < 0)
			{
				Projectile.velocity *= 0.8f;
			}
			else
			{
				Projectile.velocity.X *= 0.8f;
			}
			AttackingFlyCooling--;
			if (AttackingFlyCooling <= 0)
			{
				AttackingFlyCooling = 0;
			}
		}
		if (MathF.Abs(Projectile.velocity.X) > 10)
		{
			State = 2;
		}
		else if (MathF.Abs(Projectile.velocity.X) > 1)
		{
			State = 1;
		}
		else
		{
			State = 0;
		}

		if (Projectile.velocity.X < -1)
		{
			Projectile.spriteDirection = -1;
		}
		if (Projectile.velocity.X > 1)
		{
			Projectile.spriteDirection = 1;
		}
	}

	public void ResetAttackState()
	{
		Projectile.friendly = false;
		AttackingFlyCooling = Main.rand.Next(20, 50);
		AttackSetOff = false;
		AttackingFlyTimer = 0;
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
					if (Collision.CanHit(npc.Center - Vector2.One, 2, 2, Projectile.Center - Vector2.One, 2, 2))
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

	public void UpdateFrame()
	{
		switch (State)
		{
			case 0:
				Projectile.frame = 0;
				break;
			case 1:
				Projectile.frameCounter++;
				if (Projectile.frameCounter > 5)
				{
					Projectile.frame++;
					Projectile.frameCounter = 0;
					if (Projectile.frame >= 8)
					{
						Projectile.frame = 2;
					}
				}
				break;
			case 2:
				if (Projectile.frame < 8)
				{
					Projectile.frame = 8;
				}
				Projectile.frameCounter++;
				if (Projectile.frameCounter > 5)
				{
					Projectile.frame++;
					Projectile.frameCounter = 0;
					if (Projectile.frame >= 13)
					{
						Projectile.frame = 8;
					}
				}
				break;
		}
	}

	public void GravityBehavior()
	{
		if (AttackingFlyTimer > 0)
		{
			return;
		}
		if (!Collision.IsWorldPointSolid(Projectile.Center + new Vector2(0, 28)))
		{
			Projectile.velocity.Y += 0.3f;
			if (Projectile.velocity.Y > 20)
			{
				Projectile.velocity.Y = 20;
			}
		}
		else
		{
			Projectile.velocity.Y *= 0;
			for (int j = 0; j < 32; j++)
			{
				Projectile.position.Y -= 1;
				if (!Collision.IsWorldPointSolid(Projectile.Center + new Vector2(0, 28)))
				{
					Projectile.position.Y += 1;
					break;
				}
			}
		}
	}

	/// <summary>
	/// If rightclick to cancel the summon buff, remove projectile.
	/// </summary>
	public void CheckKill()
	{
		Player player = Main.player[Projectile.owner];
		if (player.dead || !player.active)
		{
			player.ClearBuff(ModContent.BuffType<Wither_Activated_Dog_Buff>());
			Projectile.Kill();
		}
		if (player.HasBuff(ModContent.BuffType<Wither_Activated_Dog_Buff>()))
		{
			Projectile.timeLeft = 2;
		}
		else
		{
			Projectile.Kill();
		}
	}

	public void FindFrame()
	{
		if (State == 0)
		{
			Projectile.frame = 0;
		}
	}

	public override void OnKill(int timeLeft)
	{
		if (Owner.ownedProjectileCounts[Type] <= 1)
		{
			Owner.ClearBuff(ModContent.BuffType<Wither_Activated_Dog_Buff>());
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Rectangle drawFrame = new Rectangle(0, Projectile.frame * 64, 94, 64);
		Texture2D mainTex = ModAsset.Wither_Activated_Dog.Value;
		Texture2D glowTex = ModAsset.Wither_Activated_Dog_glow.Value;
		Texture2D glowTex_c = ModAsset.Wither_Activated_Dog_glow_collar.Value;
		Texture2D bloomTex = ModAsset.Wither_Activated_Dog_bloom.Value;
		SpriteEffects spriteEffects = SpriteEffects.None;
		if (Projectile.spriteDirection == -1)
		{
			spriteEffects = SpriteEffects.FlipHorizontally;
		}
		lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
		Color glowColor = new Color(1f, 1f, 1f, 0);
		float count = OldPositions.Count;
		for (int t = 0; t < count; t++)
		{
			Rectangle trailFrame = new Rectangle(0, (int)(OldDirectionsAnsOldFrames.ToArray()[t].Y * 64), 94, 64);
			SpriteEffects trailSpriteEffects = SpriteEffects.None;
			if (OldDirectionsAnsOldFrames.ToArray()[t].X == -1)
			{
				trailSpriteEffects = SpriteEffects.FlipHorizontally;
			}
			Vector2 trailPos = OldPositions.ToArray()[t] - Main.screenPosition;
			float trailFade = t / count * 0.5f;
			Main.spriteBatch.Draw(mainTex, trailPos, trailFrame, lightColor * trailFade * 0.2f, Projectile.rotation, drawFrame.Size() * 0.5f, Projectile.scale, trailSpriteEffects, 0);
		}
		Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, drawFrame, lightColor, Projectile.rotation, drawFrame.Size() * 0.5f, Projectile.scale, spriteEffects, 0);
		Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, drawFrame, glowColor, Projectile.rotation, drawFrame.Size() * 0.5f, Projectile.scale, spriteEffects, 0);
		Main.spriteBatch.Draw(glowTex_c, Projectile.Center - Main.screenPosition, drawFrame, glowColor, Projectile.rotation, drawFrame.Size() * 0.5f, Projectile.scale, spriteEffects, 0);
		Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, drawFrame, glowColor, Projectile.rotation, drawFrame.Size() * 0.5f, Projectile.scale, spriteEffects, 0);

		for (int t = 0; t < count; t++)
		{
			Rectangle trailFrame = new Rectangle(0, (int)(OldDirectionsAnsOldFrames.ToArray()[t].Y * 64), 94, 64);
			SpriteEffects trailSpriteEffects = SpriteEffects.None;
			if (OldDirectionsAnsOldFrames.ToArray()[t].X == -1)
			{
				trailSpriteEffects = SpriteEffects.FlipHorizontally;
			}
			Vector2 trailPos = OldPositions.ToArray()[t] - Main.screenPosition;
			float trailFade = t / count * 0.5f;
			Main.spriteBatch.Draw(glowTex, trailPos, trailFrame, glowColor * trailFade, Projectile.rotation, drawFrame.Size() * 0.5f, Projectile.scale, trailSpriteEffects, 0);
		}

		Lighting.AddLight(Projectile.Center + new Vector2(36 * Projectile.spriteDirection, 0), new Vector3(0.1f, 0.6f, 0.2f));
		return false;
	}
}
