using Everglow.Commons.Templates.Weapons;

namespace Everglow.Example.Test;

public class HandholdProj2 : HandheldProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Examples;

	public override void SetDef()
	{
		TextureRotation = 1.204f;
	}
}
