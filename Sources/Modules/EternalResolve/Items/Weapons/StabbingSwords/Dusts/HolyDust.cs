namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords.Dusts
{
	public class HolyDust : ModDust
	{
		public override bool Update(Dust dust)
		{
			Lighting.AddLight(dust.position, dust.scale * 0.6f, dust.scale * 0.5f, 0);
			return true;
		}

		public override Color? GetAlpha(Dust dust, Color lightColor)
		{
			Color c0 = Color.White * 0.5f;
			c0.A = 50;
			return c0;
		}

		public override void OnSpawn(Dust dust)
		{
			dust.frame = new Rectangle(0, Main.rand.Next(3) * 10, 10, 10);
			base.OnSpawn(dust);
		}
	}
}
