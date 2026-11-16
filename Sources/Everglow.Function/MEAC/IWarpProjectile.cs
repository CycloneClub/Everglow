using Everglow.Commons.VFX;

namespace Everglow.Commons.MEAC;

/// <summary>
/// Warp in Polar Coordinate System.<br></br><br></br>
/// [R] for direction, from 0 to 255, corresponds 0 to 2Pi.<br></br>
/// An increase in the R will cause the distortion direction to rotate clockwise.<br></br><br></br>
/// [G] for the magnitude of distortion.
/// </summary>
public interface IWarpProjectile
{
	void DrawWarp(VFXBatch spriteBatch);
}

/// <summary>
/// Warp in Cartesian Coordinate System.<br></br><br></br>
/// [R] for X , from 0 to 255, corresponds LEFT to RIGHT. 127 to prevent distortion.<br></br><br></br>
/// [G] for Y , from 0 to 255, corresponds UP to DOWN. 127 to prevent distortion.<br></br><br></br>
/// [B]  for the magnitude of distortion.
/// </summary>
public interface IWarpProjectile_WarpStyle2
{
	void DrawWarp(VFXBatch spriteBatch);
}
