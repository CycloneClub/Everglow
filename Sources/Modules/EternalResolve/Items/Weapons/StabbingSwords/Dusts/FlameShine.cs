namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords.Dusts
{
	public class FlameShine : ModDust
	{
		public override bool Update(Dust dust)
		{
			dust.position += dust.velocity;
			dust.position += Main.player[dust.color.R].velocity;
			dust.scale *= 0.93f;
			if (dust.scale < 0.02f)
			{
				dust.active = false;
			}
			dust.velocity *= 0.9f;
			dust.rotation += 0.9f;
			if (dust.color.A < 255)
			{
				dust.color.A += 9;
			}
			else
			{
				dust.color.A = 255;
			}
			if (Collision.SolidCollision(dust.position, 0, 0))
			{
				dust.active = false;
			}
			Vector2 nextPosition = dust.position - dust.velocity * 1.5f;
			if (Main.tile[(int)(nextPosition.X / 16f), (int)(nextPosition.Y / 16f)].LiquidAmount > 0)
			{
				dust.active = false;
			}
			return false;
		}

		public override Color? GetAlpha(Dust dust, Color lightColor)
		{
			Color c0 = new Color(255, 255, 255, 0);
			return c0;
		}

		public override void OnSpawn(Dust dust)
		{
			dust.frame = new Rectangle(0, Main.rand.Next(3) * 10, 10, 10);
			dust.color.A = (byte)Main.rand.Next(0, 70);
			base.OnSpawn(dust);
		}
	}
}
