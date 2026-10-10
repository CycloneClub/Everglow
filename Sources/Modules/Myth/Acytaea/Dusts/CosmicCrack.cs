namespace Everglow.Myth.Acytaea.Dusts;

[Pipeline(typeof(NPPipeline), typeof(AcytaeaPipeline))]
public class CosmicCrack : Particle
{
	public float rotation;

	public override void OnSpawn()
	{
		scale = 1;
		rotation = Main.rand.NextFloat() * MathHelper.TwoPi;
	}

	public override void Update()
	{
		scale -= 0.03f;
		velocity *= 0.99f;
		if (scale <= 0)
		{
			Active = false;
		}
	}

	public override void Draw()
	{
		var texture = ModAsset.CosmicCrack.Value;
		Ins.Batch.BindTexture(texture).Draw(position, null, Color.White, rotation, texture.Size() / 2, scale, SpriteEffects.None);
	}
}
