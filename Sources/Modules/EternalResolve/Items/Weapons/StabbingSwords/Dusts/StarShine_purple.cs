namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords.Dusts
{
	public class StarShine_purple : ModDust
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
			if (Collision.SolidCollision(dust.position, 0, 0))
			{
				dust.active = false;
			}
			Vector2 nextPosition = dust.position - dust.velocity * 1.5f;
			if (Main.tile[(int)(nextPosition.X / 16f), (int)(nextPosition.Y / 16f)].LiquidAmount > 0)
			{
				dust.active = false;
			}
			Lighting.AddLight(dust.position, dust.scale * 0.04f, 0, dust.scale * 0.08f);
			return false;
		}

		public override Color? GetAlpha(Dust dust, Color lightColor)
		{
			Color c0 = new Color(235, 120, 195, 155);
			return c0;
		}

		public override void OnSpawn(Dust dust)
		{
			dust.frame = new Rectangle(0, Main.rand.Next(3) * 10, 10, 10);
			base.OnSpawn(dust);
		}
	}
}
