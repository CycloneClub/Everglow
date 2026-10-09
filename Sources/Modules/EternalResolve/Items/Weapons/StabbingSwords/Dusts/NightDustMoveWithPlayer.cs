namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords.Dusts
{
	public class NightDustMoveWithPlayer : ModDust
	{
		public override bool Update(Dust dust)
		{
			dust.position += Main.player[dust.color.R].velocity;
			if (dust.frame.Y == 0)
			{
				Lighting.AddLight(dust.position, dust.scale * 0.1f, 0, dust.scale * 0.4f);
			}
			return true;
		}

		public override Color? GetAlpha(Dust dust, Color lightColor)
		{
			Color c0 = Color.White * 0.5f;
			c0.A = 150;
			if (dust.frame.Y == 0)
			{
				c0.A = 40;
			}
			return c0;
		}

		public override void OnSpawn(Dust dust)
		{
			dust.frame = new Rectangle(0, Main.rand.Next(3) * 10, 10, 10);
			base.OnSpawn(dust);
		}
	}
}
