namespace Everglow.Myth.LanternMoon.VFX;

[Pipeline(typeof(WCSPipeline))]
public class LanternExplosionDustGroup : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawNPCs;

	public Vector2 Position;
	public float Timer;
	public float MaxTime;

	public struct MovingDust()
	{
		public Vector2 Position;
		public Vector2 Velocity;
		public float Scale;
		public float Rotation;
		public float RotationSpeed;
	}

	public List<MovingDust> Dusts = new List<MovingDust>();

	public void UpdateDust()
	{
		int count = 360;
		if (Dusts.Count <= 0)
		{
			for (int i = 0; i < count; i++)
			{
				MovingDust dust = new MovingDust();
				dust.Position = Position;
				dust.Velocity = new Vector2(0, 10).RotatedBy(i / (float)count * MathHelper.TwoPi);
				dust.Scale = 1;
				dust.Rotation = 0;
				dust.RotationSpeed = 0;
				Dusts.Add(dust);
			}
		}
		for (int i = 0; i < Dusts.Count; i++)
		{
			MovingDust dust = Dusts[i];
			dust.Position += dust.Velocity;
			dust.Velocity = dust.Velocity.RotatedBy(dust.RotationSpeed);
			dust.Velocity *= Math.Clamp(0.99f - MathF.Abs(dust.RotationSpeed * 0.02f), 0, 1);
			dust.RotationSpeed *= 0.95f;
			Dusts[i] = dust;
		}
	}

	public float GetRandomNoise(Vector2 pos)
	{
		float x = pos.X;
		float y = pos.Y;
		float number = 0;
		float timeValue = (float)Main.timeForVisualEffects * 0.03f;
		for (int i = 0; i < 10; i++)
		{
			number += MathF.Sin((x - timeValue) * MathF.Pow(1.5f, i)) * MathF.Pow(2, -i);
			number += MathF.Sin((y + timeValue) * MathF.Pow(1.5f, i)) * MathF.Pow(2, -i);
		}
		return number;
	}

	public override void Update()
	{
		UpdateDust();
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
	}

	public override void Draw()
	{
		Texture2D tex = ModAsset.LanternExplosionDustGroup.Value;

		if (Dusts.Count <= 0)
		{
			Ins.Batch.Draw(tex, Vector2.zeroVector, null, Color.Transparent, 0, Vector2.zeroVector, 0, SpriteEffects.None);
		}
		else
		{
			for (int i = 0; i < Dusts.Count; i++)
			{
				Ins.Batch.Draw(tex, Dusts[i].Position, null, Color.White, Dusts[i].Rotation, Vector2.zeroVector, Dusts[i].Scale, SpriteEffects.None);
			}
		}
	}
}
