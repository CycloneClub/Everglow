namespace Everglow.Commons.Templates.Weapons;

/// <summary>
/// If a custom rendering projectile don't use texture itself, inherit this one.
/// </summary>
public abstract class NoTextureProjectile : ModProjectile
{
	public override string Texture => ModAsset.Empty_Mod;
}
