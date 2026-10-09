namespace Everglow.Commons.Utilities;

public static class ProjectileUtils
{
	public static void TrackOldValue<T>(T[] array, T curValue)
	{
		for (int i = array.Length - 1; i > 0; i--)
		{
			array[i] = array[i - 1];
		}
		array[0] = curValue;
	}

	public static bool IsSafeInTheWorld(Projectile projectile)
	{
		return IsSafeInTheWorld(projectile.TopLeft) && IsSafeInTheWorld(projectile.TopRight) && IsSafeInTheWorld(projectile.BottomLeft) && IsSafeInTheWorld(projectile.BottomRight);
	}

	/// <summary>
	/// Find closest target by given position.
	/// </summary>
	/// <param name="fromWhere"></param>
	/// <param name="searchDistance">Exclusive maximum search distance in world pixels, measured to an NPC's center minus half its hitbox diagonal.</param>
	/// <returns></returns>
	public static int FindTarget(Vector2 fromWhere, float searchDistance)
	{
		int target = -1;
		float minDis = searchDistance;
		foreach (NPC npc in Main.ActiveNPCs)
		{
			if (npc.CanBeChasedBy() && !npc.dontTakeDamage && npc.life > 0 && !npc.friendly)
			{
				float dis = (npc.Center - fromWhere).Length() - npc.Hitbox.Size().Length() * 0.5f;
				if (dis < minDis)
				{
					minDis = dis;
					target = npc.whoAmI;
				}
			}
		}

		return target;
	}

	/// <summary>
	/// Check if target is active
	/// </summary>
	/// <returns>
	/// active: true | inactive: false
	/// </returns>
	public static bool MinionCheckTargetActive(int targetWhoAmI)
	{
		if (targetWhoAmI < 0 || targetWhoAmI >= Main.npc.Length)
		{
			return false;
		}

		NPC target = Main.npc[targetWhoAmI];
		if (!target.active || target.dontTakeDamage || !target.CanBeChasedBy() || target.friendly)
		{
			return false;
		}

		return true;
	}

	public static bool IsSafeInTheWorld(Vector2 position)
	{
		if (position.X <= 320 || position.X >= Main.maxTilesX * 16 - 320)
		{
			return false;
		}
		if (position.Y <= 320 || position.Y >= Main.maxTilesY * 16 - 320)
		{
			return false;
		}
		return true;
	}

	public abstract class StickNPCProjectile : ModProjectile
	{
		/// <summary>
		/// 目标敌人,目标敌人死亡的时候会改成-2
		/// </summary>
		public int StuckNPC = -1;

		/// <summary>
		/// 相对角度
		/// </summary>
		public float RelativeAngle = 0;

		/// <summary>
		/// 击中时的角度
		/// </summary>
		public float HitTargetAngle = 0;

		/// <summary>
		/// 相对位置
		/// </summary>
		public Vector2 RelativePos = Vector2.zeroVector;

		/// <summary>
		/// 击中时怪的大小
		/// </summary>
		public float HitTargetScale = 1f;

		public override void AI()
		{
			UpdateSticking();
		}

		public virtual void UpdateSticking()
		{
			if (StuckNPC >= 0 && StuckNPC < Main.maxNPCs)
			{
				NPC target = Main.npc[StuckNPC];
				if (target == null || !target.active)
				{
					StuckNPC = -2;
					return;
				}
				else
				{
					Projectile.rotation = target.rotation + RelativeAngle;
					float scaleRate = target.scale / HitTargetScale;
					Projectile.Center = target.Center + RelativePos.RotatedBy(target.rotation + RelativeAngle - HitTargetAngle) * scaleRate;
					Projectile.velocity = target.velocity;
				}
			}
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			RelativeAngle = Projectile.rotation - target.rotation;
			HitTargetAngle = Projectile.rotation;
			RelativePos = Projectile.Center - target.Center;
			HitTargetScale = target.scale;
			StuckNPC = target.whoAmI;
		}
	}
}
