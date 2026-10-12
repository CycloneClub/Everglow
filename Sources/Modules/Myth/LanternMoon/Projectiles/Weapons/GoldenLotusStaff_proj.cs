using Everglow.Commons.Templates.Weapons;
using Everglow.Myth.LanternMoon.VFX;
using Terraria.DataStructures;

namespace Everglow.Myth.LanternMoon.Projectiles.Weapons;

public class GoldenLotusStaff_proj : TrailingProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonProjectiles;

	public Player Owner;

	public int SplitCooling = 0;

	public int DashTimer = 0;

	public int NoTrailTimer = 0;

	public int CombineForm7Timer = 0;

	public bool SplitInto7 = false;

	public float SplitFade = 1f;

	public override void SetCustomDefaults()
	{
		Projectile.aiStyle = -1;
		Projectile.width = 16;
		Projectile.height = 16;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 36000000;
		Projectile.hostile = false;
		Projectile.friendly = true;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 10;
		Projectile.minion = true;
		Projectile.minionSlots = 1;
		TrailTexture = Commons.ModAsset.Trail_9.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_9_black.Value;
	}

	public override void OnSpawn(IEntitySource source)
	{
		SplitCooling = Main.rand.Next(120, 180);
		base.OnSpawn(source);
	}

	public override void Behaviors()
	{
		if (SplitCooling > 0)
		{
			SplitCooling--;
		}
		if (Timer % 6 == 0)
		{
			Projectile.frame++;
			Projectile.frame %= 4;
		}

		if (Owner is null)
		{
			if (Projectile.owner >= 0 && Projectile.owner < Main.player.Length)
			{
				Owner = Main.player[Projectile.owner];
			}
			else
			{
				DestroyEntity();
				return;
			}
		}
		if ((Owner.Center - Projectile.Center).Length() > 2000)
		{
			NoTrailTimer = TrailLength * 2;
			Projectile.Center = Owner.Center;
		}
		if (NoTrailTimer > 0)
		{
			NoTrailTimer--;
		}
		if (CombineForm7Timer > 0)
		{
			CombineForm7Timer--;
		}
		NPC target = Projectile.FindTargetWithinRange(300);
		if (target is null)
		{
			IdleMove();
			if (SplitInto7)
			{
				DashTimer = 0;

				// Conbine
				if (AllSubProjsReadyToConbine())
				{
					SplitInto7 = false;
					CombineForm7Timer = TrailLength;
					Projectile.friendly = true;
					SplitCooling = Main.rand.Next(360, 600);
				}
			}
		}
		else
		{
			if ((target.Center - Owner.Center).Length() > 1300)
			{
				return;
			}
			if (!SplitInto7)
			{
				if (ShouldSplit())
				{
					SplitAndHide();
				}
				else
				{
					ChaseTarget(target);
				}
			}
			else
			{
				IdleMove();
			}
		}
		if (SplitInto7)
		{
			if (SplitFade > 0)
			{
				SplitFade -= 0.1f;
			}
			else
			{
				SplitFade = 0f;
			}
			if (GetLowDefNPCCount() <= 0)
			{
				foreach (var proj in Main.projectile)
				{
					if (proj is not null && proj.active && proj.type == ModContent.ProjectileType<GoldenLotusStaff_subproj>())
					{
						GoldenLotusStaff_subproj gLSsp = proj.ModProjectile as GoldenLotusStaff_subproj;
						if (gLSsp is not null && gLSsp.ParentProj == Projectile)
						{
							if (gLSsp.Timer < gLSsp.MaxTime - 60)
							{
								gLSsp.Timer = gLSsp.MaxTime - 60;
							}
						}
					}
				}
			}
		}
		else
		{
			if (SplitFade < 1)
			{
				SplitFade += 0.1f;
			}
			else
			{
				SplitFade = 1f;
			}
		}
	}

	public bool ShouldSplit()
	{
		if (SplitCooling == 0)
		{
			if (GetLowDefNPCCount() >= 3 && (GetHighDefNPCCount() < 0 || (SameProjCount() > 0 && SameProjHasSplitCount() == 0)))
			{
				return true;
			}
		}
		return false;
	}

	public int SameProjCount()
	{
		int count = 0;
		foreach (var proj in Main.projectile)
		{
			if (proj is not null & proj.active && proj.owner == Projectile.owner && proj.type == Type)
			{
				if (proj != Projectile)
				{
					count++;
				}
			}
		}
		return count;
	}

	public int SameProjHasSplitCount()
	{
		int count = 0;
		foreach (var proj in Main.projectile)
		{
			if (proj is not null & proj.active && proj.owner == Projectile.owner && proj.type == Type)
			{
				if (proj != Projectile)
				{
					GoldenLotusStaff_proj gLSp = proj.ModProjectile as GoldenLotusStaff_proj;
					if (gLSp is not null)
					{
						if (gLSp.SplitInto7)
						{
							count++;
						}
					}
				}
			}
		}
		return count;
	}

	public int GetLowDefNPCCount()
	{
		int lowDefTargetCount = 0;
		foreach (var npc in Main.npc)
		{
			if (npc is not null && npc.active)
			{
				if (npc.CanBeChasedBy(Projectile))
				{
					if ((npc.Center - Projectile.Center).Length() < 800 && (npc.Center - Owner.Center).Length() < 1300)
					{
						if (npc.defense < 15)
						{
							lowDefTargetCount++;
						}
					}
				}
			}
		}
		return lowDefTargetCount;
	}

	public int GetHighDefNPCCount()
	{
		int highDefTargetCount = 0;
		foreach (var npc in Main.npc)
		{
			if (npc is not null && npc.active)
			{
				if (npc.CanBeChasedBy(Projectile))
				{
					if ((npc.Center - Projectile.Center).Length() < 800 && (npc.Center - Owner.Center).Length() < 1300)
					{
						if (npc.defense > 30)
						{
							highDefTargetCount++;
						}
					}
				}
			}
		}
		return highDefTargetCount;
	}

	public void SplitAndHide()
	{
		SplitCooling = 9999999;
		SplitInto7 = true;
		Projectile.friendly = false;
		for (int k = 0; k < 7; k++)
		{
			Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<GoldenLotusStaff_subproj>(), Projectile.damage / 3, Projectile.knockBack, Projectile.owner);
			GoldenLotusStaff_subproj gLSsp = p0.ModProjectile as GoldenLotusStaff_subproj;
			if (gLSsp is not null)
			{
				gLSsp.ParentProj = Projectile;
				gLSsp.MaxTime = 720;
				gLSsp.Timer = 0;
			}
		}
	}

	public void ChaseTarget(NPC target)
	{
		var targetPos = target.Center;
		Vector2 toNPCTarget = targetPos + target.velocity - Projectile.Center - Projectile.velocity;
		if (toNPCTarget.Length() > 300)
		{
			Vector2 targetVel = toNPCTarget.NormalizeSafe() * 15f;
			Projectile.velocity = targetVel * 0.05f + Projectile.velocity * 0.95f;
			DashTimer = 0;
		}
		else
		{
			DashTimer++;
			if (DashTimer == 1)
			{
				Projectile.velocity = toNPCTarget.NormalizeSafe() * 50f;
			}
			if (DashTimer < 45)
			{
				Vector2 targetVel = toNPCTarget.NormalizeSafe() * 1f;
				Projectile.velocity = targetVel * 0.1f + Projectile.velocity * 0.9f;
			}
			else
			{
				DashTimer = 0;
			}
		}
		Projectile.rotation = Projectile.velocity.ToRotationSafe();
	}

	public bool AllSubProjsReadyToConbine()
	{
		foreach (var proj in Main.projectile)
		{
			if (proj is not null && proj.active && proj.type == ModContent.ProjectileType<GoldenLotusStaff_subproj>())
			{
				GoldenLotusStaff_subproj gLSsp = proj.ModProjectile as GoldenLotusStaff_subproj;
				if (gLSsp is not null && gLSsp.ParentProj == Projectile)
				{
					if (gLSsp.Timer < gLSsp.MaxTime - 10)
					{
						return false;
					}
				}
			}
		}
		return true;
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

	public override void DestroyEntityEffect()
	{
		var star = new HitStarAndWave
		{
			Active = true,
			Visible = true,
			Position = Projectile.Center,
			Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
			MaxTime = Main.rand.Next(20, 30),
			Scale = Main.rand.NextFloat(1.2f, 1.6f),
		};
		Ins.VFXManager.Add(star);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (target.type != NPCID.TargetDummy && DashTimer > 0)
		{
			var star = new HitStarAndWave
			{
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				MaxTime = Main.rand.Next(20, 30),
				Scale = Main.rand.NextFloat(1.6f, 2f),
			};
			Ins.VFXManager.Add(star);
			for (int g = 0; g < 12; g++)
			{
				Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(23f, 40f)).RotatedByRandom(MathHelper.TwoPi);
				var spark = new HitEffectSpark
				{
					Velocity = newVelocity,
					Active = true,
					Visible = true,
					Position = Projectile.Center,
					MaxTime = Main.rand.Next(12, 16),
					DrawColor = new Color(0.8f, 0.8f, 0, 0),
					LightFlat = 0f,
					SpeedDecay = 0.9f,
					GravityAcc = 0.0f,
					SelfLight = false,
					Scale = Main.rand.NextFloat(20f, 30f),
				};
				Ins.VFXManager.Add(spark);
			}
		}
	}

	public override void DestroyEntity()
	{
		base.DestroyEntity();
	}

	public override void DrawSelf()
	{
		if (SplitInto7)
		{
			return;
		}
		Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
		Rectangle frame = new Rectangle(0, 26 * Projectile.frame, 52, 26);
		Color drawColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
		Texture2D bloom = ModAsset.GoldenLotusStaff_proj_bloom.Value;
		if (DashTimer > 0 && DashTimer < 24)
		{
			float value = 1 - DashTimer / 24f;
			Main.EntitySpriteDraw(bloom, Projectile.Center - Main.screenPosition, frame, new Color(1f, 1f, 1f, 0) * value, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		}
		Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, drawColor, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		if (CombineForm7Timer > 0)
		{
			Texture2D shape = ModAsset.GoldenLotusStaff_proj_shape.Value;
			Texture2D shape_black = ModAsset.GoldenLotusStaff_proj_shape_black.Value;
			float value = CombineForm7Timer / 30f;
			Main.EntitySpriteDraw(shape_black, Projectile.Center - Main.screenPosition, frame, Color.White * value, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
			if (CombineForm7Timer % 6 < 3)
			{
				Main.EntitySpriteDraw(shape, Projectile.Center - Main.screenPosition, frame, Color.Lerp(Color.White * value, drawColor, 0.5f), Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
			}
		}
	}

	public override Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue)
	{
		float x = factor + Projectile.whoAmI / 3.5f - timeValue * 0.5f;
		float y = 1;
		float z = widthValue;
		if (phase == 2)
		{
			y = 0;
		}
		if (phase % 2 == 1)
		{
			y = 0.5f;
		}
		return new Vector3(x, y, z);
	}

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0)
	{
		float teleportFade = 1;
		if (NoTrailTimer > 0)
		{
			teleportFade = TrailLength - NoTrailTimer;
			teleportFade /= 30f;
			teleportFade = Math.Clamp(teleportFade, 0, 1);
		}
		if (CombineForm7Timer > 0)
		{
			float value = TrailLength - CombineForm7Timer;
			value /= 30f;
			value = Math.Clamp(value, 0, 1);
			if (value < teleportFade)
			{
				teleportFade = value;
			}
		}
		if (style == 1)
		{
			Color goldenEnv = Lighting.GetColor(worldPos.ToTileCoordinates(), new Color(0.4f, 0.3f, 0.02f, 0));
			goldenEnv.A = 0;
			goldenEnv *= 1.4f;
			if (DashTimer > 0 && DashTimer < 24)
			{
				float value = 1 - DashTimer / 24f;
				goldenEnv = Color.Lerp(goldenEnv, new Color(1f, 0.9f, 0.6f, 0), value);
			}
			return Color.Lerp(goldenEnv, Color.Transparent, factor) * SplitFade * teleportFade;
		}
		if (style == 0)
		{
			return base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1) * 0.25f * SplitFade * teleportFade;
		}
		if (style == 2)
		{
			Color warpColor = base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);
			warpColor.B = (byte)(warpColor.B * SplitFade * teleportFade);
			if (SplitFade == 0)
			{
				warpColor *= 0;
			}
			return warpColor;
		}
		return base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);
	}
}
