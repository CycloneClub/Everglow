using Everglow.Commons.VFX;

namespace Everglow.Yggdrasil;

public interface IOcclusionProjectile
{
	void DrawOcclusion(VFXBatch spriteBatch);

	void DrawEffect(VFXBatch spriteBatch);
}
