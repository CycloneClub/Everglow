using Everglow.Commons.Utilities;

namespace Everglow.Commons.MEAC;

public abstract partial class MeleeProj_3D : ModProjectile, IWarpProjectile_warpStyle2, IBloomProjectile
{
	// Rotation Speed Curve
	public double BaseMeleeSpeed = 1.4;
	public double BaseDecaySpeed = 0.93;

	/// <summary>
	/// Return a 2D position of the weapon tip projected from 3D space, which can be used for player arm rotation and slash effect drawing. The position is relative to the projectile center, so adding Projectile.Center will get the world position.
	/// </summary>
	/// <returns></returns>
	public Vector2 CurrentWeaponTipPosition()
	{
		Vector3 currentPos3D = WeaponAxis + new Vector3(0, 0, CenterZ);
		Vector2 currentPos = Project(currentPos3D, ProjectionMatrix());
		return currentPos;
	}

	public Matrix PerspectiveMatrix()
	{
		Vector2 lookat = Main.screenPosition + Main.ScreenSize.ToVector2() / 2;
		return
			Matrix.CreateTranslation(new Vector3(0, 0, 500f * 1 / MathF.Tan(MeleeProj_3D_Configs.AngleofFOV * 0.5f) - 1125f))
						* Matrix.CreateLookAt(
					new Vector3((Projectile.Center.X - lookat.X) / -1f, (Projectile.Center.Y - lookat.Y) / -1f, 500),
					new Vector3((Projectile.Center.X - lookat.X) / -1f, (Projectile.Center.Y - lookat.Y) / -1f, 0),
					new Vector3(0, 1, 0))
			;
	}

	public Vector2 Project(Vector3 point, Matrix ProjectionMatrix, [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = "")
	{

		if ((callerFilePath.Contains("MeleeProj_3D.Draw.cs") || callerFilePath.Contains("MeleeProj_3D.Posture.cs")) && MeleeProj_3D_Configs.IsMeleeWeaponProjectBindWithScreen_Draw)
		{
			return ProjectBindWithScreen(point, ProjectionMatrix) * 0.75f;
		}
		if (callerFilePath.Contains("MeleeProj_3D.Behavior.cs") && MeleeProj_3D_Configs.IsMeleeWeaponProjectBindWithScreen_Behavior)
		{
			return ProjectBindWithScreen(point, ProjectionMatrix) * 0.75f;
		}
		return ProjectBindWithProj(point, ProjectionMatrix);
	}

	public Vector2 ProjectBindWithProj(Vector3 point, Matrix ProjectionMatrix)
	{
		Vector4 homogenousPoint = Vector4.Transform(new Vector4(point, 1), ProjectionMatrix);

		int Size = Math.Min(Main.screenWidth, Main.screenHeight);

		float SizeCorrectionFlag = 1000f / Main.screenHeight * MeleeProj_3D_Configs.AngleofFOV / MathHelper.PiOver4; // 1000 is a randomly made-up magic number

		// Normalized Device Coordinates
		if (homogenousPoint.W != 0)
		{
			float xNDC = -homogenousPoint.X / homogenousPoint.W;
			float yNDC = -homogenousPoint.Y / homogenousPoint.W;

			float xScreen = xNDC * Main.screenWidth / 2f;
			float yScreen = yNDC * Main.screenHeight / 2f;

			return new Vector2(xScreen, yScreen) * SizeCorrectionFlag;
		}
		throw new InvalidOperationException("W component of the projected point is zero.");
	}

	public Vector2 ProjectBindWithScreen(Vector3 point, Matrix ProjectionMatrix)
	{
		Vector2 lookat = Main.screenPosition + Main.ScreenSize.ToVector2() / 2;
		Vector4 homogenousPoint = Vector4.Transform(new Vector4(point, 1), PerspectiveMatrix());
		homogenousPoint = Vector4.Transform(homogenousPoint, ProjectionMatrix);

		int Size = Math.Min(Main.screenWidth, Main.screenHeight);

		float SizeCorrectionFlag = 1000f / Main.screenHeight; // 750 is a randomly made-up magic number

		// Normalized Device Coordinates
		if (homogenousPoint.W != 0)
		{
			float xNDC = -homogenousPoint.X / homogenousPoint.W;
			float yNDC = -homogenousPoint.Y / homogenousPoint.W;

			float xScreen = xNDC * Main.screenWidth / 2f;
			float yScreen = yNDC * Main.screenHeight / 2f;

			return new Vector2(xScreen, yScreen) * SizeCorrectionFlag + (lookat - Projectile.Center);
		}
		throw new InvalidOperationException("W component of the projected point is zero.");
	}

	public List<Vector2> Project(List<Vector3> ring, Matrix ProjectionMatrix)
	{
		List<Vector2> projectedRing = new List<Vector2>();

		foreach (var point in ring)
		{
			projectedRing.Add(Project(point, ProjectionMatrix));
		}
		return projectedRing;
	}

	public List<Vector3> ToLongitudeRing(List<Vector2> ring, float longitude)
	{
		Matrix rotationMatrix = Matrix.CreateRotationY(longitude);

		List<Vector3> xozRing = new List<Vector3>(ring.Count);

		foreach (var point in ring)
		{
			Vector3 originalPoint = new Vector3(point, 0);
			Vector3 rotatedPoint = Vector3.Transform(originalPoint, rotationMatrix);
			rotatedPoint.Z += CenterZ;
			xozRing.Add(rotatedPoint);
		}
		return xozRing;
	}

	public List<Vector3> ToLatitudeRing(List<Vector2> ring, float latitude, float radius)
	{
		List<Vector3> newRing = new List<Vector3>(ring.Count);

		foreach (var point in ring)
		{
			Vector3 originalPoint = new Vector3(point.X, 0, point.Y);
			Vector3 resizedPoint = originalPoint * MathF.Cos(latitude);
			Vector3 latitudePoint = resizedPoint + new Vector3(0, MathF.Sin(latitude), 0) * radius;
			latitudePoint.Z += CenterZ;
			newRing.Add(latitudePoint);
		}
		return newRing;
	}

	public static void RotateMainAxis(float angle, Vector3 rotatedAxis, ref Vector3 mainAxis)
	{
		Quaternion rotation = Quaternion.CreateFromAxisAngle(Vector3.Normalize(rotatedAxis), angle);
		mainAxis = Vector3.Transform(mainAxis, rotation);
	}

	public float GetSizeZ(float coordZ)
	{
		return CenterZ / coordZ;
	}

	/// <summary>
	/// A const value, if change the BaseMeleeSpeed, BaseDecaySpeed or MaxAttackTime, this value should be recalculated. It represents the integral of the rotation speed curve over time, which is used to solve for the decay coefficient b when given a new melee speed a.
	/// </summary>
	private double FixedIntegral()
	{
		double maxTime = 60;
		if (SlashEffects.Count > 0)
		{
			SlashEffect minTimerEffect = SlashEffects.OrderBy(e => e.Timer).First();
			maxTime = minTimerEffect.MaxTime;
		}
		return (Math.Pow(BaseDecaySpeed, maxTime) - 1) / Math.Log(BaseDecaySpeed) * BaseMeleeSpeed;
	}

	/// <summary>
	/// Input a new meleeSpeed and output b value.
	/// </summary>
	public float SolveB(double newMeleeSpeed)
	{
		double maxTime = 60;
		if (SlashEffects.Count > 0)
		{
			SlashEffect minTimerEffect = SlashEffects.OrderBy(e => e.Timer).First();
			maxTime = minTimerEffect.MaxTime;
		}
		double a = newMeleeSpeed;
		double K = FixedIntegral();

		double t = maxTime * a / K;
		double arg = -t * Math.Exp(-t);
		double w = MathUtils.LambertW0(arg);
		double b = Math.Exp(-w / maxTime - a / K);

		return (float)b;
	}
}
