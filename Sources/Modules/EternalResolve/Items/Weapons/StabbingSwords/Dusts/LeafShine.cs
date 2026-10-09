namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords.Dusts
{
	public class LeafShine : ModDust
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
			return false;
		}

		public override Color? GetAlpha(Dust dust, Color lightColor)
		{
			lightColor.A = dust.color.A;
			return lightColor;
		}

		public override void OnSpawn(Dust dust)
		{
			dust.color.A = (byte)Main.rand.Next(40, 120);
			dust.frame = new Rectangle(0, Main.rand.Next(3) * 10, 10, 10);
			base.OnSpawn(dust);
		}
	}
}
