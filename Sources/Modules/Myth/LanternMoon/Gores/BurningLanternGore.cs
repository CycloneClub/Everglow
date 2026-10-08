namespace Everglow.Myth.LanternMoon.Gores;

[Pipeline(typeof(DissolveAndNoDissolvePipeline))]
public class BurningLanternGore : DissolveGore
{
	public override void OnSpawn()
	{
		MaxTime = 200;
		base.OnSpawn();
	}

	public override void Update()
	{
		base.Update();

		float alpha2 = (Timer - 100) / (MaxTime - 100f);
		alpha2 = Math.Clamp(alpha2, 0.0f, 1.0f);
		alpha2 = MathF.Sin(alpha2 * MathHelper.Pi);
		Lighting.AddLight(Position, new Vector3(1f, 0.5f, 0) * alpha2 * Width / 60f);
	}
}
