namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords.Dusts
{
	public class EnchantedDustMoveWithPlayer : ModDust
	{
		public override bool Update(Dust dust)
		{
			dust.position += Main.player[dust.color.R].velocity;
			if (dust.frame.X == 0)
			{
				Lighting.AddLight(dust.position, 0.71f * dust.scale, 0.66f * dust.scale, 0.09f * dust.scale);
			}
			if (dust.frame.X == 10)
			{
				Lighting.AddLight(dust.position, 0.70f * dust.scale, 0.07f * dust.scale, 0.67f * dust.scale);
			}
			if (dust.frame.X == 20)
			{
				Lighting.AddLight(dust.position, 0.04f * dust.scale, 0.37f * dust.scale, 0.67f * dust.scale);
			}
			return true;
		}

		public override Color? GetAlpha(Dust dust, Color lightColor)
		{
			Color c0 = Color.White;
			c0.A = 50;
			return c0;
		}

		public override void OnSpawn(Dust dust)
		{
			dust.frame = new Rectangle(Main.rand.Next(3) * 10, Main.rand.Next(3) * 10, 10, 10);
			base.OnSpawn(dust);
		}
	}
}
