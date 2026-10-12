using Everglow.Myth.TheTusk.Buffs;

namespace Everglow.Myth.TheTusk.Projectiles.Weapon;

internal class TuskSummon : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonProjectiles;

	public override void SetDefaults()
	{
		Projectile.tileCollide = false;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 6;
		Projectile.timeLeft = 720;
		Projectile.minionSlots = 1;
		Projectile.penetrate = -1;
		Projectile.aiStyle = -1;
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.DamageType = DamageClass.MagicSummonHybrid;
		Projectile.minion = true;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
	}

	internal int TargetWhoAmI = -1;
	internal int AttackCooling = 0;
	internal int TelePortCooling = 0;

	private void TelePortTo(Vector2 aim)
	{
		TelePortCooling = 60;
		Projectile.Center = aim;
	}

	private void MoveTo(Vector2 aim, float speedValue = 0.1f)
	{
		Vector2 v0 = aim - Projectile.velocity - Projectile.Center;
		float val = v0.Length();
		if (val > 100f)
		{
			val = 100f;
		}

		Projectile.velocity = (aim - Projectile.velocity - Projectile.Center).SafeNormalize(Vector2.Zero) * val * speedValue;
	}

	private void CheckKill()
	{
		Player player = Main.player[Projectile.owner];
		if (player.dead || !player.active)
		{
			player.ClearBuff(ModContent.BuffType<TuskStaff>());
			Projectile.Kill();
		}
		if (player.HasBuff(ModContent.BuffType<TuskStaff>()))
		{
			Projectile.timeLeft = 2;
		}
		else
		{
			Projectile.Kill();
		}
	}

	private void FindEnemies()
	{
		Player player = Main.player[Projectile.owner];
		Vector2 detectCenter = player.Center;
		if (Projectile.ai[0] > 5)
		{
			detectCenter = Projectile.Center;
		}

		float minDistance = 1600;
		int threatenEnemiesCount = 0;
		foreach (NPC npc in Main.npc)
		{
			if (npc.active)
			{
				if (npc.whoAmI == player.MinionAttackTargetNPC)
				{
					TargetWhoAmI = npc.whoAmI;
					return;
				}
				if (!npc.dontTakeDamage && !npc.friendly && npc.CanBeChasedBy() && Collision.CanHit(Projectile, npc))
				{
					if ((npc.Center - detectCenter).Length() < minDistance)
					{
						TargetWhoAmI = npc.whoAmI;
						minDistance = (npc.Center - detectCenter).Length();
					}

					float minDistanceII = (npc.Center - player.Center).Length();
					if (minDistanceII < 250)
					{
						threatenEnemiesCount++;
					}

					if (threatenEnemiesCount >= 3)
					{
						TargetWhoAmI = -2;
						return;
					}
				}
			}
		}
	}

	private void CheckTargetActive()
	{
		if (TargetWhoAmI > -1)
		{
			if (!Main.npc[TargetWhoAmI].active || Main.npc[TargetWhoAmI].dontTakeDamage)
			{
				TargetWhoAmI = -1;
			}
		}
	}

	private void Attack()
	{
		NPC target;
		if (TargetWhoAmI is >= 0 and <= 200)
		{
			target = Main.npc[TargetWhoAmI];
		}
		else
		{
			TargetWhoAmI = -1;
			return;
		}
		if (AttackCooling <= 0)
		{
			if ((Projectile.Center - target.Center).Length() < 170 + target.height)
			{
				Projectile.velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 25f;
				AttackCooling = 40;
			}
			else
			{
				MoveTo(target.Center + new Vector2(0, -target.height - 150).RotatedBy(Math.Sin(Main.time * 0.1f + Projectile.ai[0])));
				Projectile.rotation = Projectile.rotation * 0.95f + Projectile.velocity.X * 0.002f;
			}
		}
		else if (AttackCooling > 30)
		{
			Projectile.rotation = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X) - MathF.PI / 2f;
			if (Collision.SolidCollision(Projectile.Center + Projectile.velocity, 0, 0))
			{
				Projectile.velocity *= 0.1f;
				if (Projectile.velocity.Length() > 10f)
				{
					Collision.HitTiles(Projectile.Center + Projectile.velocity - new Vector2(8), Projectile.velocity, 16, 16);
				}
			}
		}
		else
		{
			MoveTo(target.Center + new Vector2(0, -target.height - 150).RotatedBy(Math.Sin(Main.time * 0.1f + Projectile.ai[0])));
			Projectile.rotation = Projectile.rotation * 0.85f + Projectile.velocity.X * 0.002f;
		}
		AttackCooling--;
	}

	private void AttackII()
	{
		Player player = Main.player[Projectile.owner];
		float radius = 60 + MathF.Sqrt(Projectile.ai[0]) * 10f;
		MoveTo(player.MountedCenter + new Vector2(0, radius).RotatedBy(Main.time / Math.Pow(radius, 1.5) * 600f + Projectile.ai[0]), 0.6f);
		Projectile.rotation = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X) - MathF.PI / 2f;
		AttackCooling = 0;
	}

	private void FlyToPlayer()
	{
		Player player = Main.player[Projectile.owner];
		float speed = 0.5f;
		Vector2 aim = player.MountedCenter + new Vector2((10 - Projectile.ai[0] * 30) * player.direction, -40 + MathF.Sin((float)Main.timeForVisualEffects * 0.04f - Projectile.ai[0]) * 35f);
		if ((Projectile.Center - aim).Length() > 500f)
		{
			speed = (Projectile.Center - player.Center).Length() / 1000f;
		}

		MoveTo(aim, speed);
		Projectile.rotation = Projectile.rotation * 0.95f + Projectile.velocity.X * 0.002f;
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		Projectile.hide = true;
		CheckKill();
		if (TelePortCooling > 0)
		{
			TelePortCooling--;
		}
		else
		{
			if ((player.Center - Projectile.Center).Length() > 2700f)
			{
				TelePortTo(player.MountedCenter + new Vector2((10 - Projectile.ai[0] * 30) * player.direction, -40 + MathF.Sin((float)Main.timeForVisualEffects * 0.04f - Projectile.ai[0]) * 35f));
			}
		}
		if (TargetWhoAmI <= -1)
		{
			if (TargetWhoAmI == -1)
			{
				FlyToPlayer();
			}

			if (TargetWhoAmI == -2)
			{
				AttackII();
			}

			FindEnemies();
		}
		else
		{
			CheckTargetActive();
			Attack();
		}
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		behindNPCsAndTiles.Add(index);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		float speed = Projectile.velocity.Length();
		if (speed > 10f)
		{
			float fade = Math.Min((speed - 10f) * 0.1f, 1f);
			var texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
			for (int i = 0; i < Projectile.oldPos.Length; i++)
			{
				Color colori = lightColor * (1f / i) * fade;
				Main.spriteBatch.Draw(texture, Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) / 2f - Main.screenPosition, null, colori, Projectile.oldRot[i], new Vector2(Projectile.width, Projectile.height) / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
			}
		}
		return base.PreDraw(ref lightColor);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(BuffID.Bleeding, 600);
	}
}
