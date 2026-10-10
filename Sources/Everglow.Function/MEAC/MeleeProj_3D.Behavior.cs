using Everglow.Commons.MEAC.VFX;
using Everglow.Commons.Utilities;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Commons.MEAC;

public abstract partial class MeleeProj_3D : ModProjectile, IWarpProjectile_warpStyle2, IBloomProjectile
{
	public struct SlashEffect
	{
		public Queue<Vector3> SlashTrail;

		public List<Vector3> SlashTrail_Smoothed;

		public List<NPC> HasHitNPCs;

		public Queue<float> SlashFade;

		public int TrailMax;

		public int Direction;

		public bool Active;

		public float RotateSpeed;

		public float TrailFade;

		public float Timer;

		public float MaxTime;

		public float NextAttackTime;

		public Vector3 MainAxis;

		public Vector3 WeaponAxis;

		public Vector3 RotationAxis;
	}

	public List<SlashEffect> SlashEffects = new List<SlashEffect>();

	public int WeaponItemType;

	public int CurrentAttackType = 0;

	/// <summary>
	/// MainAxis will spin around this.<br/>
	/// Suggest to set this perpendicular to the MainAxis.
	/// </summary>
	public Vector3 RotatedAxis = new Vector3(0, -1, 0);

	public Vector3 SphericalCoordPos => MathUtils.CartesianToSpherical(MainAxis);

	public Vector3 MainAxisWithDepth => MainAxis + new Vector3(0, 0, CenterZ);

	public Vector3 MainAxis = new Vector3(150, 0, 0);

	public Vector3 WeaponAxis = new Vector3(150, 0, 0);

	public Vector3 MainAxisDirection => Vector3.Normalize(MainAxis);

	public Vector2 ScreenPositionOffset => Projectile.Center - Main.screenPosition;

	public float CenterZ = 1620;

	public float RadialDistance => SphericalCoordPos.X;

	public float PolarAngle => SphericalCoordPos.Y;

	public float AzimuthalAngle => SphericalCoordPos.Z;

	public float WeaponLength = 60;

	public float AttackTimer;

	public float MaxAttackTime = 60;

	public float RotateSpeed = 0;

	public float GetMeleeSpeed()
	{
		if (Owner is null || !Owner.active || Owner.dead)
		{
			return 1;
		}
		return Owner.GetAttackSpeed(DamageClass.Melee);
	}

	public override void OnSpawn(IEntitySource source)
	{
		EnableSphereCoordDraw = false;
	}

	public override void SetDefaults()
	{
		Projectile.drawLayer = ProjectileDrawLayerID.OverPlayers;
		Projectile.usesOwnerLight = true;
		Projectile.aiStyle = -1;
		Projectile.height = Projectile.width = 120;
		Projectile.penetrate = -1;
		Projectile.extraUpdates = 1;
		Projectile.localNPCHitCooldown = 1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.noEnchantmentVisuals = true;
		Projectile.DamageType = DamageClass.Melee;
		WeaponLength = 60;
		BaseMeleeSpeed = 1.4;
		BaseDecaySpeed = 0.93;
		SetCustomDefaults();
	}

	/// <summary>
	/// Suggest values: WeaponLength, BaseMeleeSpeed, BaseDecaySpeed, Projectile properties (width, height, etc).<br/>
	/// </summary>
	public virtual void SetCustomDefaults()
	{
	}

	public override void AI()
	{
		AttachWithOwner();
		CheckHeldItem();
		CenterZ = 1620;
		DevelopersAdjust();
		Update();
		HoldWeapon();
		ProduceWaterRipples(new Vector2(CurrentWeaponTipPosition().Length(), 30));
	}

	public virtual void Update()
	{
		float meleeSpeed = GetMeleeSpeed();
		TrailSlashEffects();
		RotateMainAxis(RotateSpeed * 0.35f, RotatedAxis, ref MainAxis);
		RotateSpeed *= SolveB(BaseMeleeSpeed * Owner.meleeSpeed);
		BindWeaponAxis();
		if (Owner.controlUseItem)
		{
			Visible = true;
			AttackTimer += meleeSpeed;
			if (SlashEffects.Count > 0)
			{
				SlashEffect minTimerEffect = SlashEffects.OrderBy(e => e.Timer).First();
				if (minTimerEffect.Timer >= minTimerEffect.NextAttackTime)
				{
					NewAttack();
				}
			}
			else
			{
				NewAttack();
			}
		}
		else
		{
			if (SlashEffects.Count <= 0)
			{
				Visible = false;
			}
		}
	}

	public float GetWeaponLength()
	{
		float outputLength = WeaponLength;
		if (Owner is not null)
		{
			if (Owner.HeldItem is not null)
			{
				outputLength *= Owner.GetAdjustedItemScale(Owner.HeldItem);
			}
		}
		return outputLength;
	}

	public virtual void NewAttack()
	{
		CurrentAttackType++;
		if (Main.MouseWorld.X > Owner.MountedCenter.X)
		{
			Owner.direction = 1;
		}
		else
		{
			Owner.direction = -1;
		}

		Vector2 mouseDir = Main.MouseWorld - Owner.MountedCenter;
		mouseDir = mouseDir.SafeNormalize(Vector2.zeroVector);
		AttackTimer = 0;
		float meleeSpeed = Owner.meleeSpeed;
		Attack_RotativeSwing(mouseDir, meleeSpeed);

		var ss = new SoundStyle(ModAsset.TrueMeleeSword_Mod);
		if (CurrentAttackType % 2 == 1)
		{
			ss = new SoundStyle(ModAsset.TrueMeleeSwordSwap_Mod);
		}
		SoundEngine.PlaySound(ss.WithPitchOffset(meleeSpeed - 1f + Main.rand.NextFloat(-0.15f, 0.15f)), Projectile.Center);
		float itemUseTime = Owner.HeldItem.useTime;
		float maxTime = itemUseTime / 0.4f;
		AddSlashEffect(maxTime, itemUseTime, (int)(maxTime + 1));
	}

	public virtual void Attack_RotativeSwing(Vector2 mouseDir, float meleeSpeed)
	{
		Vector2 mouseDir_90Deg = mouseDir.SafeNormalize(Vector2.zeroVector).RotatedBy(MathHelper.PiOver2);
		RotatedAxis = new Vector3(mouseDir_90Deg, Main.rand.NextFloat(0.2f, 1f));
		RotatedAxis = Vector3.Normalize(RotatedAxis);
		MainAxis = new Vector3(-mouseDir * (60 + GetWeaponLength()), 0);
		MathUtils.RotateToPerpendicular(RotatedAxis, ref MainAxis);
		RotateSpeed = (float)BaseMeleeSpeed * meleeSpeed * Owner.direction * Owner.gravDir;
	}

	public virtual void Attack_2_Side_Swing(Vector2 mouseDir, float meleeSpeed)
	{
		Vector2 mouseDir_2side = new Vector2(Math.Sign(mouseDir.X) * 30, 1f).NormalizeSafe();
		RotatedAxis = new Vector3(mouseDir_2side, 100f);
		RotatedAxis = Vector3.Normalize(RotatedAxis);
		MainAxis = new Vector3(-mouseDir_2side * (60 + GetWeaponLength()), 0);
		MathUtils.RotateToPerpendicular(RotatedAxis, ref MainAxis);
		RotateSpeed = (float)BaseMeleeSpeed * meleeSpeed * Owner.direction * Owner.gravDir * 0.75f;
	}

	public virtual void AddSlashEffect(float maxTime, float nextAttackTime, int trailMax)
	{
		SlashEffect sEffect = new SlashEffect() { };
		sEffect.Active = true;
		sEffect.Timer = 0;
		sEffect.MaxTime = maxTime;
		sEffect.TrailMax = trailMax;
		sEffect.NextAttackTime = nextAttackTime;
		sEffect.SlashFade = new Queue<float>();
		sEffect.SlashTrail = new Queue<Vector3>();
		sEffect.SlashTrail_Smoothed = new List<Vector3>();
		sEffect.HasHitNPCs = new List<NPC>();
		sEffect.TrailFade = 0;
		sEffect.MainAxis = MainAxis;
		sEffect.WeaponAxis = WeaponAxis;
		sEffect.RotateSpeed = RotateSpeed;
		sEffect.RotationAxis = RotatedAxis;
		sEffect.Direction = Owner.direction;
		SlashEffects.Add(sEffect);
	}

	public virtual void TrailSlashEffects()
	{
		float meleeSpeed = GetMeleeSpeed();
		for (int k = SlashEffects.Count - 1; k >= 0; k--)
		{
			SlashEffect sEffect = SlashEffects[k];
			sEffect.Timer += meleeSpeed;
			if (sEffect.Timer > sEffect.MaxTime)
			{
				sEffect.Active = false;
			}
			sEffect.TrailFade = Math.Abs(sEffect.RotateSpeed * 3.5f / (float)(BaseMeleeSpeed / 1.4) / meleeSpeed);
			RotateMainAxis(sEffect.RotateSpeed * 0.35f, sEffect.RotationAxis, ref sEffect.MainAxis);
			Vector3 oldWeaponAxis = sEffect.WeaponAxis;
			sEffect.WeaponAxis = sEffect.MainAxis + Vector3.Normalize(sEffect.MainAxis) * GetWeaponLength();
			if (sEffect.Timer >= 2 && sEffect.Timer <= MaxAttackTime - 4)
			{
				AddDust(oldWeaponAxis, oldWeaponAxis * 0.5f, sEffect.RotationAxis, sEffect.RotateSpeed, sEffect.TrailFade);
			}
			sEffect.RotateSpeed *= SolveB(BaseMeleeSpeed * meleeSpeed);
			if (sEffect.SlashFade is null)
			{
				sEffect.SlashFade = new Queue<float>();
			}
			if (sEffect.SlashTrail is null)
			{
				sEffect.SlashTrail = new Queue<Vector3>();
			}
			if (sEffect.SlashTrail_Smoothed is null)
			{
				sEffect.SlashTrail_Smoothed = new List<Vector3>();
			}
			if (sEffect.Timer < sEffect.MaxTime)
			{
				sEffect.SlashFade.Enqueue(sEffect.TrailFade);
				sEffect.SlashTrail.Enqueue(sEffect.WeaponAxis);
			}
			if (sEffect.SlashFade.Count > sEffect.TrailMax)
			{
				sEffect.SlashFade.Dequeue();
			}
			if (sEffect.SlashTrail.Count > sEffect.TrailMax)
			{
				sEffect.SlashTrail.Dequeue();
			}
			if (sEffect.SlashTrail.Count > 3)
			{
				sEffect.SlashTrail_Smoothed = GraphicsUtils.Smooth(sEffect.SlashTrail);
			}
			SlashEffects[k] = sEffect;
			if (!sEffect.Active)
			{
				SlashEffects.RemoveAt(k);
			}
		}
	}

	public virtual void AddDust(Vector3 oldAxisTip, Vector3 oldAxisTail, Vector3 rotationAxis, float rotationSpeed, float trailFade)
	{
		float maxCount = Math.Abs(rotationSpeed) * 100;
		float rotSpeed = rotationSpeed / maxCount;
		for (int i = 0; i < maxCount; i++)
		{
			if (Main.rand.NextBool(10))
			{
				float randValue = MathF.Sqrt(Main.rand.NextFloat());
				var melee_dust = new MeleeProj_3D_Dust()
				{
					Active = true,
					Visible = true,
					Position_Space = oldAxisTip * randValue + oldAxisTail * (1 - randValue),
					MaxTime = Main.rand.NextFloat(30, 60),
					Scale = Main.rand.NextFloat(0.1f, 0.8f) * (randValue + 1f) * trailFade * 0.37f,
					Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
					RotSpeed = rotationSpeed * 0.05f * Main.rand.NextFloat(0.8f, 1.2f),
					RotAxis = rotationAxis,
					ParentProj = this,
				};
				melee_dust.RegisterBehavior(CustomDustBehavior);
				melee_dust.RegisterDraw(CustomDustDraw);
				Ins.VFXManager.Add(melee_dust);
			}

			// Enchantment Effects
			EnchantmentDustEffect(oldAxisTip, oldAxisTail, rotationAxis, rotationSpeed, trailFade, Owner.meleeEnchant);
			if (Owner.magmaStone)
			{
				EnchantmentDustEffect(oldAxisTip, oldAxisTail, rotationAxis, rotationSpeed, trailFade, 3);
			}
			RotateMainAxis(rotSpeed, rotationAxis, ref oldAxisTip);
			RotateMainAxis(rotSpeed, rotationAxis, ref oldAxisTail);
		}
	}

	public virtual void EnchantmentDustEffect(Vector3 oldAxisTip, Vector3 oldAxisTail, Vector3 rotationAxis, float rotationSpeed, float trailFade, int type)
	{
		if (type <= 0)
		{
			return;
		}
		float maxCount = Math.Abs(rotationSpeed) * 100;
		float rotSpeed = rotationSpeed / maxCount;
		int freq = 40;
		float size = 1f;
		float rotSpeedMin = 0.2f;
		float rotSpeedMax = 0.4f;
		float spinSpeed = 0.01f;
		int maxTimeMin = 10;
		int maxTimeMax = 30;
		switch (type)
		{
			case 1:
				rotSpeedMin = -0.1f;
				rotSpeedMax = 0.1f;
				maxTimeMin = 40;
				maxTimeMax = 60;
				size = 1.2f;
				freq = 120;
				break;
			case 4:
				freq = 90;
				size = 0.5f;
				break;
			case 5:
				freq = 150;
				size = 0.75f;
				spinSpeed = 0.05f;
				break;
			case 6:
				freq = 50;
				size = 0.75f;
				break;
			case 7:
				rotSpeedMin = -0.1f;
				rotSpeedMax = 0.1f;
				freq = 100;
				maxTimeMin = 60;
				maxTimeMax = 120;
				break;
			case 8:
				rotSpeedMin = -0.05f;
				rotSpeedMax = 0.05f;
				maxTimeMin = 30;
				maxTimeMax = 40;
				size = 1.2f;
				freq = 120;
				break;
		}
		if (Main.rand.NextBool(freq))
		{
			float mulMaxTime = 1f;
			if (trailFade < 1f)
			{
				mulMaxTime = trailFade;
			}
			float randValue = MathF.Sqrt(Main.rand.NextFloat());
			var melee_dust = new MeleeProj_3D_Dust()
			{
				Active = true,
				Visible = true,
				Position_Space = oldAxisTip * randValue + oldAxisTail * (1 - randValue),
				MaxTime = Main.rand.NextFloat(maxTimeMin, maxTimeMax) * mulMaxTime,
				Scale = Main.rand.NextFloat(0.4f, 0.6f) * size * (randValue + 1f) * 1.6f,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				RotSpeed = rotationSpeed * spinSpeed * Main.rand.NextFloat(0.8f, 1.2f),
				RotAxis = rotationAxis,
				ParentProj = this,
				EnchantmentType = type,
				ai = new float[] { Main.rand.NextFloat(rotSpeedMin, rotSpeedMax), -2, Main.rand.Next(3), Main.rand.Next(4) },
			};
			melee_dust.RegisterBehavior(EnchantmentDustBehavior);
			melee_dust.RegisterDraw(EnchantmentDustDraw);
			Ins.VFXManager.Add(melee_dust);
		}
	}

	public virtual void CustomDustBehavior(MeleeProj_3D_Dust dust)
	{
		dust.RotSpeed *= 0.89f;
		dust.Scale *= 0.92f;
		if (dust.Scale < 0.01f)
		{
			dust.Active = false;
			return;
		}
	}

	public virtual void EnchantmentDustBehavior(MeleeProj_3D_Dust dust)
	{
		Vector3 wldPos3D = dust.Position_Space + new Vector3(0, 0, CenterZ);
		Vector2 wldPos = Project(wldPos3D, ProjectionMatrix()) + Projectile.Center;
		float lightMul = 1f;
		if (dust.MaxTime - dust.Timer < 10)
		{
			lightMul = (dust.MaxTime - dust.Timer) / 10f;
		}
		dust.Rotation += dust.ai[0];
		switch (dust.EnchantmentType)
		{
			case 1: // Venom
				break;
			case 2: // Cursed Flames
				Lighting.AddLight(wldPos, new Vector3(0.3f, 0.9f, 0.1f) * dust.Scale * lightMul);
				dust.Position_Space.Y += dust.ai[1];
				dust.ai[1] *= 0.9f;
				break;
			case 3: // Fire
				Lighting.AddLight(wldPos, new Vector3(1f, 0.5f, 0.15f) * dust.Scale * lightMul);
				dust.Position_Space.Y += dust.ai[1];
				dust.ai[1] *= 0.9f;
				break;
			case 4: // Gold
				Lighting.AddLight(wldPos, new Vector3(0.8f, 0.8f, 0.4f) * dust.Scale * lightMul);
				break;
			case 5: // Ichor
				Lighting.AddLight(wldPos, new Vector3(0.8f, 0.8f, 0.1f) * dust.Scale * lightMul * 2);
				break;
			case 6: // Nanites
				Lighting.AddLight(wldPos, new Vector3(0.6f, 0.7f, 0.8f) * dust.Scale * lightMul * 0.75f);
				break;
			case 7: // Party
				dust.Position_Space.Y += 1f;
				break;
			case 8: // Poison
				break;
		}
		if (dust.Scale < 0.01f)
		{
			dust.Active = false;
			return;
		}
	}

	/// <summary>
	/// Only for developers to adjust the behavior. Will not be called every tick, only when the projectile is spawned or when the attack is triggered. You can use it to adjust the MainAxis, RotatedAxis, RotateSpeed, etc. to make the attack look better. You can also use it to test new features without affecting the normal behavior of the projectile.
	/// </summary>
	public virtual void DevelopersAdjust()
	{
	}

	/// <summary>
	/// For developers only; allow weapon orientation to follow the mouse pointer(<see cref="MainAxis"/> maintaining length at 240);
	/// </summary>
	public void FollowMouse()
	{
		Vector3 toMouse = new Vector3(Main.MouseScreen - new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f, 0) - new Vector3(0, 0, CenterZ * 0.1f);
		MainAxis = Vector3.Normalize(toMouse) * 240f;
	}

	/// <summary>
	/// Rotate the weapon around the 3D axis pointing to the mouse.
	/// </summary>
	public void RotatedByMouseAxis()
	{
		float value = (MathF.Sin((float)Main.time * 0.13f) + 1) / 2f;
		value = MathF.Pow(value, 3) + 0.5f;
		Vector3 toMouse = new Vector3(Main.MouseScreen - new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f, 0) - new Vector3(0, 0, CenterZ * 0.1f);
		RotatedAxis = Vector3.Normalize(toMouse) * 120f;
		RotateMainAxis(value * 0.35f, RotatedAxis, ref MainAxis);
	}

	public virtual void CheckHeldItem()
	{
		if (Owner.HeldItem.type == WeaponItemType)
		{
			if (Projectile.timeLeft < 5)
			{
				Projectile.timeLeft = 5;
			}
		}
	}

	public virtual void AttachWithOwner()
	{
		if (Owner is null || !Owner.active || Owner.dead)
		{
			Projectile.Kill();
		}
		Projectile.Center = Owner.MountedCenter;
		Projectile.velocity *= 0;
	}

	public virtual void BindWeaponAxis()
	{
		WeaponAxis = MainAxis + MainAxisDirection * WeaponLength;
	}
}
