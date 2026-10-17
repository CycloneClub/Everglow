namespace Everglow.Yggdrasil.CorruptWormHive.VFXs;

public class DevilPipeline : PostPipeline
{
	public override void Render(RenderTarget2D rt2D)
	{
		var sb = Main.spriteBatch;
		sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
		effect.Value.Parameters["tex1"].SetValue(ModAsset.Death_Sickle_Color.Value);
		effect.Value.Parameters["uTransform"].SetValue(
			Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1));
		effect.Value.CurrentTechnique.Passes[0].Apply();
		sb.Draw(rt2D, Vector2.Zero, Color.White);
		sb.End();
	}

	public override void Load()
	{
		effect = ModContent.Request<Effect>(ModAsset.FlameColor_Mod);
	}
}
