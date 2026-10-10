using Everglow.Myth.LanternMoon.VFX;
using Terraria.DataStructures;

namespace Everglow.Myth.LanternMoon.Projectiles.Weapons;

public class GoldenLotusStaff_subproj : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonProjectiles;

	public Player Owner;

	public Projectile ParentProj;

	public int MaxTime;

	public int DashTimer = 0;

	public int Timer;

	public Vector2 HitStart = Vector2.zeroVector;

	public Vector2 HitEnd = Vector2.zeroVector;

	public Queue<Vector3> OldPosAndRot = new Queue<Vector3>();

	public List<NPC> HasHitTargets = new List<NPC>();

	public override void SetDefaults()
	{
		Projectile.aiStyle = -1;
		Projectile.width = 16;
		Projectile.height = 16;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 6000;
		Projectile.hostile = false;
		Projectile.friendly = true;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 10;
		Timer = 0;
	}

	public override void OnSpawn(IEntitySource source)
	{
		base.OnSpawn(source);
	}

	public override void AI()
	{
		OldPosAndRot.Enqueue(new Vector3(Projectile.Center, Projectile.rotation));
		if (OldPosAndRot.Count > 6)
		{
			OldPosAndRot.Dequeue();
		}
		Timer++;
		if (Owner is null)
		{
			if (Projectile.owner >= 0 && Projectile.owner < Main.player.Length)
			{
				Owner = Main.player[Projectile.owner];
			}
			else
			{
				Projectile.Kill();
				return;
			}
		}
		if ((Owner.Center - Projectile.Center).Length() > 2000)
		{
			Projectile.Center = Owner.Center;
		}
		bool canChaseTarget = true;
		if (Timer < 60)
		{
			canChaseTarget = false;
			Projectile.velocity *= 0;
			float index = AllocateIndex();
			Projectile.rotation = index / 7f * MathHelper.TwoPi + MathF.Pow(Timer / 60f, 2) * 4;
		}
		if (Timer == 60)
		{
			canChaseTarget = false;
			Projectile.velocity = new Vector2(20, 0).RotatedBy(Projectile.rotation);
		}
		if (Timer is > 60 and < 120)
		{
			canChaseTarget = false;
			float rotValue = (120 - Timer) / 120f * 0.05f;
			Projectile.velocity = Projectile.velocity.RotatedBy(rotValue) * 0.96f;
			Projectile.rotation = Projectile.velocity.ToRotationSafe();
		}
		if (Timer > MaxTime - 120 && Timer < MaxTime)
		{
			DashTimer++;
			canChaseTarget = false;
			Vector2 targetPos = ParentProj.Center;
			Vector2 toTarget = targetPos - Projectile.Center - Projectile.velocity;
			float index = AllocateIndex();
			if (toTarget.Length() < 300)
			{
				toTarget = toTarget / 10f;
			}
			else
			{
				toTarget = toTarget.NormalizeSafe() * 30;
			}
			if (toTarget.Length() > 6 && Timer <= MaxTime - 10)
			{
				Projectile.velocity = Projectile.velocity * 0.8f + toTarget * 0.2f;
				Projectile.rotation = Projectile.velocity.ToRotationSafe();
			}
			else
			{
				Projectile.Center = ParentProj.Center;
				Projectile.velocity *= 0;
				Projectile.rotation = index / 7f * MathHelper.TwoPi + MathF.Pow(Timer / 60f, 2) * 20;
			}
		}
		if (Timer >= MaxTime)
		{
			var star = new FlashTrajectory
			{
				Active = true,
				Visible = true,
				Position = Projectile.Center - new Vector2(120, 0).RotatedBy(Projectile.rotation),
				Rotation = Projectile.rotation - MathHelper.PiOver2,
				MaxTime = Main.rand.Next(20, 30),
				Distance = 200,
			};
			Ins.VFXManager.Add(star);
			Projectile.Kill();
		}
		if (canChaseTarget)
		{
			NPC target = FindTarget();
			if (target is null)
			{
				IdleMove();
			}
			else
			{
				if ((target.Center - Owner.Center).Length() > 1300)
				{
					return;
				}
				ChaseTarget(target);
			}
		}
	}

	public NPC FindTarget()
	{
		float minValue = 100;
		NPC targetNPC = null;
		foreach (var npc in Main.npc)
		{
			if (npc is not null && npc.active)
			{
				if (npc.CanBeChasedBy(Projectile))
				{
					float distance = (npc.Center - Projectile.Center).Length();
					if (distance < 800 && (npc.Center - Owner.Center).Length() < 1300)
					{
						float value = 800 - distance - npc.defense * 50;
						if (value > minValue)
						{
							minValue = value;
							targetNPC = npc;
						}
					}
				}
			}
		}
		return targetNPC;
	}

	public void ChaseTarget(NPC target)
	{
		var targetPos = target.Center;
		Vector2 toNPCTarget = targetPos - Projectile.Center - Projectile.velocity;
		if (toNPCTarget.Length() > 100 + Projectile.whoAmI * 2)
		{
			Vector2 targetVel = toNPCTarget.NormalizeSafe() * 32f;
			Projectile.velocity = targetVel * 0.05f + Projectile.velocity * 0.95f;
		}
		else
		{
			DashTimer++;
			if (DashTimer == 1)
			{
				HasHitTargets = new List<NPC>();
				Vector2 toTarget2 = (targetPos - Projectile.Center) * 2;
				HitStart = Projectile.Center;
				HitEnd = Projectile.Center + toTarget2;
				var star = new FlashTrajectory
				{
					Active = true,
					Visible = true,
					Position = Projectile.Center - toTarget2 * 0.5f,
					Rotation = toTarget2.ToRotationSafe() - MathHelper.PiOver2,
					MaxTime = Main.rand.Next(20, 30),
					Distance = toTarget2.Length() * 2,
				};
				Ins.VFXManager.Add(star);
				Projectile.Center += toTarget2;
				Projectile.velocity = toTarget2.NormalizeSafe() * 12;
			}
			if (DashTimer < 15)
			{
				Vector2 targetVel = toNPCTarget.NormalizeSafe() * 1f;
				Projectile.velocity = targetVel * 0.2f + Projectile.velocity * 0.8f;
			}
			else
			{
				DashTimer = 0;
			}
		}
		Projectile.rotation = Projectile.velocity.ToRotationSafe();
	}

	public void IdleMove()
	{
		DashTimer = 0;
		Vector2 twistedRingTrajecory = new Vector2(240 * MathF.Sin(Timer * 0.05f + Projectile.whoAmI), 90 * MathF.Sin(Timer * 0.025f) - 160) + Owner.Center;
		Vector2 toTarget = twistedRingTrajecory - Projectile.Center - Projectile.velocity;
		if (toTarget.Length() > 12)
		{
			toTarget.Normalize();
			toTarget *= 12;
		}
		Projectile.velocity = toTarget * 0.05f + Projectile.velocity * 0.95f;
		Projectile.rotation = Projectile.velocity.ToRotationSafe();
	}

	public int AllocateIndex()
	{
		int index = 0;
		foreach (var proj in Main.projectile)
		{
			if (proj is not null && proj.active && proj.type == Type && proj.whoAmI < Projectile.whoAmI)
			{
				GoldenLotusStaff_subproj gLSsp = proj.ModProjectile as GoldenLotusStaff_subproj;
				if (gLSsp is not null && gLSsp.ParentProj == ParentProj)
				{
					index++;
				}
			}
		}
		return index;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		bool hasHit = false;
		foreach (var npc in HasHitTargets)
		{
			if (npc is not null && npc.active)
			{
				if (npc.Hitbox == targetHitbox)
				{
					hasHit = true;
					break;
				}
			}
		}
		if (DashTimer < 15 && HitStart != Vector2.zeroVector && HitEnd != Vector2.zeroVector && HasHitTargets.Count < 6 && !hasHit)
		{
			return CollisionUtils.Intersect(targetHitbox.Left(), targetHitbox.Right(), targetHitbox.Height, HitStart, HitEnd, 30);
		}
		return false;
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		modifiers.FinalDamage *= 3f / (HasHitTargets.Count + 3);
		if (HasHitTargets.Contains(target))
		{
			modifiers.FinalDamage *= 0;
			modifiers.Knockback *= 0;
			modifiers.HideCombatText();
		}
		base.ModifyHitNPC(target, ref modifiers);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		HasHitTargets.Add(target);
		if (damageDone > 1)
		{
			for (int g = 0; g < 8; g++)
			{
				Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(4f, 10f)).RotatedByRandom(MathHelper.TwoPi);
				var spark = new HitEffectSpark
				{
					Velocity = newVelocity,
					Active = true,
					Visible = true,
					Position = target.Center,
					MaxTime = Main.rand.Next(12, 16),
					DrawColor = new Color(0.8f, 0.8f, 0, 0),
					LightFlat = 0f,
					SpeedDecay = 0.9f,
					GravityAcc = 0.0f,
					SelfLight = false,
					Scale = Main.rand.NextFloat(10f, 20f),
				};
				Ins.VFXManager.Add(spark);
			}
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
		Color drawColor;
		Vector2 origin_main = new Vector2(4, tex.Height * 0.5f);
		for (int i = 0; i < OldPosAndRot.Count; i++)
		{
			Vector3 pos = OldPosAndRot.ToArray()[i];
			drawColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
			Main.EntitySpriteDraw(tex, new Vector2(pos.X, pos.Y) - Main.screenPosition, null, drawColor * (i / (float)OldPosAndRot.Count) * 0.25f, pos.Z, origin_main, Projectile.scale, SpriteEffects.None, 0);
		}
		drawColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
		Texture2D bloom = ModAsset.GoldenLotusStaff_subproj_bloom.Value;
		if (DashTimer > 0 && DashTimer < 12)
		{
			float value = 1 - DashTimer / 12f;
			Main.EntitySpriteDraw(bloom, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0) * value, Projectile.rotation, new Vector2(11, bloom.Height * 0.5f), Projectile.scale, SpriteEffects.None, 0);
		}
		float timeKill = 60;
		if (Timer > MaxTime - timeKill && Timer < MaxTime)
		{
			float value = Timer - MaxTime + timeKill;
			value /= timeKill;
			Main.EntitySpriteDraw(bloom, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0) * value, Projectile.rotation, new Vector2(11, bloom.Height * 0.5f), Projectile.scale, SpriteEffects.None, 0);
		}
		Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, origin_main, Projectile.scale, SpriteEffects.None, 0);

		if (Timer < 30)
		{
			Texture2D shape = ModAsset.GoldenLotusStaff_subproj_shape.Value;
			Texture2D shape_black = ModAsset.GoldenLotusStaff_subproj_shape_black.Value;
			float value = 1 - Timer / 30f;
			Main.EntitySpriteDraw(shape_black, Projectile.Center - Main.screenPosition, null, Color.White * value, Projectile.rotation, origin_main, Projectile.scale, SpriteEffects.None, 0);
			if (Timer % 6 < 3)
			{
				Main.EntitySpriteDraw(shape, Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.White * value, drawColor, 0.5f), Projectile.rotation, origin_main, Projectile.scale, SpriteEffects.None, 0);
			}
		}
		return false;
	}
}
