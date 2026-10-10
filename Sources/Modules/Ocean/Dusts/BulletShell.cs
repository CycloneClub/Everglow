using Terraria.Audio;

namespace Everglow.Ocean.Dusts;

public class BulletShell : ModDust
{
	public override bool MidUpdate(Dust dust)
	{
		return true;
	}

	public override void OnSpawn(Dust dust)
	{
		dust.frame.Width = 10;
	}

	public override bool Update(Dust dust)
	{
		if (Collision.SolidCollision(dust.position, 8, 8))
		{
			if (dust.alpha == 0)
			{
				SoundEngine.PlaySound(SoundID.NPCHit4.WithPitchOffset(0.2f).WithVolumeScale(0.4f), dust.position);
			}

			dust.alpha += 5;
			dust.velocity *= 0;
		}
		else
		{
			dust.velocity.Y += 0.25f;
		}
		dust.position += dust.velocity;
		dust.rotation += dust.velocity.X;
		if (dust.alpha >= 250)
		{
			dust.active = false;
		}

		return false;
	}
}
