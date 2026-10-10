using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Microsoft.Xna.Framework.Graphics;

namespace Everglow.Commons.MEAC;

/// <summary>
/// Postures, include player posture and weapon motivation.
/// </summary>
public abstract partial class MeleeProj_3D : ModProjectile, IWarpProjectile_warpStyle2, IBloomProjectile
{
	public Player Owner => DrawPlayer ?? Main.player[Projectile.owner];

	public void HoldWeapon()
	{
		if (Visible)
		{
			Vector3 currentPos3D = WeaponAxis + new Vector3(0, 0, CenterZ);
			Vector2 currentPos = Project(currentPos3D, ProjectionMatrix());
			if (Owner.gravDir == -1)
			{
				currentPos.Y *= -1;
			}
			Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, currentPos.ToRotation() - MathHelper.PiOver2);
		}
	}
}
