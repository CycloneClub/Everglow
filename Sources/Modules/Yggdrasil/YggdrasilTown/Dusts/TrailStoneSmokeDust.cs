using Everglow.Yggdrasil.YggdrasilTown.VFXs;

namespace Everglow.Yggdrasil.YggdrasilTown.Dusts;

public class TrailStoneSmokeDust : ModDust
{
	public override bool Update(Dust dust)
	{
		Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 0.04f)).RotatedByRandom(MathHelper.TwoPi);
		for (int y = 0; y < 5; y++)
		{
			var somg = new RockSmogDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = dust.position + new Vector2(4) + dust.velocity * Main.rand.NextFloat(1f),
				MaxTime = Main.rand.Next(37, 145) * dust.scale,
				Scale = Main.rand.NextFloat(7f, 15f) * dust.scale,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
		dust.scale *= 0.94f;
		dust.velocity.Y += 0.1f;
		dust.position += dust.velocity;
		if (Collision.SolidCollision(dust.position + new Vector2(dust.velocity.X, 0), 0, 0))
		{
			dust.velocity.X *= -0.4f;
			dust.scale *= 0.5f;
		}
		if (Collision.SolidCollision(dust.position + new Vector2(0, dust.velocity.Y), 0, 0))
		{
			dust.velocity.Y *= -0.4f;
			dust.scale *= 0.5f;
		}
		if (dust.scale < 0.01f)
		{
			dust.active = false;
		}
		return false;
	}
}
