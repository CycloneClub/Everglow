namespace Everglow.Yggdrasil.YggdrasilTown.NPCs.KingJellyBall;

public class KingJellyBall_ReflectionPipeline : ScreenReflectionPipeline
{
	// PreDrawFilter captures the completed scene, including all players.
	protected override bool IncludeLocalPlayer => false;

	protected override Matrix CaptureTransform => Matrix.Identity;

	public override void BeginRender()
	{
		base.BeginRender();
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
	}

	protected override void ApplyReflectionEffect()
	{
		Effect reflection = ModAsset.KingJellyBall_SurfaceReflection.Value;
		var viewport = Main.graphics.GraphicsDevice.Viewport;
		var projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, 500);
		reflection.Parameters["uModel"].SetValue(Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)));
		reflection.Parameters["uViewProj"].SetValue(Main.GameViewMatrix.TransformationMatrix * projection);
		reflection.Parameters["uScreenTransform"].SetValue(Main.GameViewMatrix.TransformationMatrix);
		reflection.Parameters["uViewportSize"].SetValue(new Vector2(viewport.Width, viewport.Height));
		reflection.Parameters["uFresnelF0"].SetValue(new Vector3(0.17f));
		reflection.Parameters["uKs"].SetValue(Vector3.One);
		reflection.CurrentTechnique.Passes["Test"].Apply();
	}
}
