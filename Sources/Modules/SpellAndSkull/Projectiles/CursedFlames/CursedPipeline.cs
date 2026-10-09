using Everglow.Commons.VFX;

namespace Everglow.SpellAndSkull.Projectiles.CursedFlames;

internal class CursedPipeline : PostPipeline
{
	public override void Render(RenderTarget2D rt2D)
	{
		var sb = Main.spriteBatch;
		sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
		effect.Value.Parameters["tex1"].SetValue(ModAsset.Cursed_Color.Value);
		effect.Value.Parameters["uTransform"].SetValue(
			Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1));
		effect.Value.CurrentTechnique.Passes[0].Apply();
		sb.Draw(rt2D, Vector2.Zero, Color.White);
		sb.End();
	}

	public override void Load()
	{
		effect = ModAsset.FlameColor;
	}
}
