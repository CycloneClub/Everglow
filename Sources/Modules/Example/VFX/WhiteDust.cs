using Everglow.Commons.VFX;
using Everglow.Commons.VFX.Pipelines;
using Everglow.Commons.VFX.Visuals;
using Terraria.GameContent;

namespace Everglow.Example.VFX;

[Pipeline(typeof(WCSPipeline), typeof(BloomPipeline))]
internal class WhiteDust : Particle
{
	public override void Draw()
	{
		Ins.Batch.BindTexture(TextureAssets.MagicPixel.Value).Draw(position, new Rectangle(0, 0, 16, 16), Color.White);
	}
}
