using Everglow.Commons.Enums;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;

namespace Everglow.SpellAndSkull.Projectiles.DemonScythe;

public class DemoSparkVFX : Visual
{
	public override void Update()
	{
		base.Update();
	}

	public override void Draw()
	{
		// DrawSpark(Color.White, Math.Min(Projectile.timeLeft / 8f, 20f), ModAsset.SparkDark.Value);
		// DrawSpark(new Color(131, 0, 255, 0), Math.Min(Projectile.timeLeft / 8f, 20f), ModAsset.SparkLight.Value);
	}

	public override CodeLayer DrawLayer => throw new NotImplementedException();

	private void DrawSpark(Color c0, float width, Texture2D tex)
	{
		var bars = new List<Vertex2D>();
	}
}
