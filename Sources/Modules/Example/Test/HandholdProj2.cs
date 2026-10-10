using Everglow.Commons.Templates.Weapons;

namespace Everglow.Example.Test;

public class HandholdProj2 : HandholdProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MiscsProjectiles;

	public override void SetDef()
	{
		TextureRotation = 1.204f;
		base.SetDef();
	}
}
