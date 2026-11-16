using Everglow.Commons.MEAC.VFX;
using Everglow.Commons.Utilities;
using Terraria.Enums;
using Terraria.GameContent.Shaders;

namespace Everglow.Commons.MEAC;

public abstract partial class MeleeProj_3D : ModProjectile, IWarpProjectile_WarpStyle2, IBloomProjectile
{
	public override bool? CanHitNPC(NPC target)
	{
		if (SlashEffects.Count > 0)
		{
			SlashEffect minTimerEffect = SlashEffects.OrderBy(e => e.Timer).First();
			if (minTimerEffect.HasHitNPCs.Contains(target))
			{
				return false;
			}
			else
			{
				return base.CanHitNPC(target);
			}
		}
		return false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (SlashEffects.Count > 0)
		{
			SlashEffect minTimerEffect = SlashEffects.OrderBy(e => e.Timer).First();
			minTimerEffect.HasHitNPCs.Add(target);
		}
		ScreenShake();
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		modifiers.HitDirectionOverride = target.Center.X > Main.player[Projectile.owner].Center.X ? 1 : -1;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		for (int k = 0; k < SlashEffects.Count; k++)
		{
			SlashEffect sEffect = SlashEffects[k];
			if (sEffect.SlashTrail_Smoothed is null || Math.Abs(sEffect.RotateSpeed) < 0.1f || sEffect.SlashTrail_Smoothed.Count < 3 || !sEffect.Active)
			{
				continue;
			}

			// Using a polygon of the last several vertices of the slash trail to detect collision, which can better fit the actual slash area and avoid missing targets when the slash is fast.
			int start = sEffect.SlashTrail_Smoothed.Count - 16;
			start = Math.Max(start, 1);
			for (int i = start; i < sEffect.SlashTrail_Smoothed.Count; i++)
			{
				List<Vector2> attackPolygon = [];
				Vector3 currentPos3D = sEffect.SlashTrail_Smoothed[i] + new Vector3(0, 0, CenterZ);
				Vector2 currentPos = Project(currentPos3D, ProjectionMatrix());
				Vector2 worldPos = Projectile.Center + currentPos;
				Vector3 oldPos3D = sEffect.SlashTrail_Smoothed[i - 1] + new Vector3(0, 0, CenterZ);
				Vector2 oldPos = Project(oldPos3D, ProjectionMatrix());
				Vector2 old_worldPos = Projectile.Center + oldPos;
				Vector3 oldPos3D_Inner = sEffect.SlashTrail_Smoothed[i] * 0.2f + new Vector3(0, 0, CenterZ);
				Vector2 oldPos_Inner = Project(oldPos3D_Inner, ProjectionMatrix());
				Vector2 worldPos_Inner = Projectile.Center + oldPos_Inner;
				Vector3 currentPos3D_Inner = sEffect.SlashTrail_Smoothed[i] * 0.2f + new Vector3(0, 0, CenterZ);
				Vector2 currentPos_Inner = Project(currentPos3D_Inner, ProjectionMatrix());
				Vector2 old_worldPos_Inner = Projectile.Center + currentPos_Inner;
				attackPolygon.Add(worldPos);
				attackPolygon.Add(worldPos_Inner);
				attackPolygon.Add(old_worldPos_Inner);
				attackPolygon.Add(old_worldPos);
				if (MathUtils.IntersectsPolygonAABB(attackPolygon, targetHitbox.TopLeft(), targetHitbox.BottomRight()))
				{
					float hitRot = (currentPos - oldPos).ToRotationSafe() + MathHelper.PiOver2;
					Vector2 pos = targetHitbox.Center();
					HitNPCVFX(hitRot, pos);
					return true;
				}
			}
		}
		return false;
	}

	public virtual void HitNPCVFX(float hitRotation, Vector2 hitPos)
	{
		var slash = new TrueMeleeHitSlash
		{
			Active = true,
			Visible = true,
			Position = hitPos,
			MaxTime = 30,
			Scale = 0.6f,
			Rotation = hitRotation,
			SelfLuminous = SelfLuminous,
			SlashColor = SlashColor,
		};
		Ins.VFXManager.Add(slash);
	}

	public virtual void ProduceWaterRipples(Vector2 beamDims)
	{
		if (!Main.dedServ)
		{
			var shaderData = (WaterShaderData)Terraria.Graphics.Effects.Filters.Scene["WaterDistortion"].GetShader();
			float waveSine = 1f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 20f);
			Vector2 ripplePos = Projectile.Center + new Vector2(beamDims.X * 0.5f, 0f).RotatedBy(CurrentWeaponTipPosition().ToRotation());
			Color waveData = new Color(0.5f, 0.1f * Math.Sign(waveSine) + 0.5f, 0f, 1f) * Math.Abs(waveSine);
			shaderData.QueueRipple(ripplePos, waveData, beamDims, RippleShape.Square, CurrentWeaponTipPosition().ToRotation());
		}
	}

	public override void CutTiles()
	{
		if (!Visible)
		{
			return;
		}
		for (int k = 0; k < SlashEffects.Count; k++)
		{
			SlashEffect sEffect = SlashEffects[k];
			if (sEffect.SlashTrail_Smoothed is null || Math.Abs(sEffect.RotateSpeed) < 0.1f || sEffect.SlashTrail_Smoothed.Count < 3 || !sEffect.Active)
			{
				continue;
			}

			// Using a polygon of the last several vertices of the slash trail to detect collision, which can better fit the actual slash area and avoid missing targets when the slash is fast.
			int start = sEffect.SlashTrail_Smoothed.Count - 16;
			start = Math.Max(start, 1);
			for (int i = start; i < sEffect.SlashTrail_Smoothed.Count; i++)
			{
				Vector3 currentPos3D = sEffect.SlashTrail_Smoothed[i] * 1.1f + new Vector3(0, 0, CenterZ);
				Vector2 currentPos = Project(currentPos3D, ProjectionMatrix());
				Vector2 worldPos = Projectile.Center + currentPos;
				Vector3 currentPos3D_Inner = sEffect.SlashTrail_Smoothed[i] * 0.2f + new Vector3(0, 0, CenterZ);
				Vector2 currentPos_Inner = Project(currentPos3D_Inner, ProjectionMatrix());
				Vector2 worldPos_Inner = Projectile.Center + currentPos_Inner;

				DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
				var cut = new Terraria.Utils.TileActionAttempt(DelegateMethods.CutTiles);
				Vector2 beamStartPos = Projectile.Center;
				Vector2 beamEndPos = beamStartPos + CurrentWeaponTipPosition();
				Utils.PlotTileLine(worldPos_Inner, worldPos, GetWeaponLength() * 0.5f, cut);
			}
		}
	}

	public virtual void ScreenShake()
	{
		if (MeleeProj_3D_Configs.ShouldMeleeWeaponScreenShake)
		{
			ShakerManager.AddShaker(Owner.MountedCenter + CurrentWeaponTipPosition(), new Vector2(0, -1).RotatedByRandom(MathHelper.TwoPi), 18, 0.8f, 16, 0.9f, 0.8f, 30);
		}
	}
}
