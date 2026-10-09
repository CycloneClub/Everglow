namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords.Dusts
{
	public class StarShine_purple_withoutPlayer : ModDust
	{
		public override string Texture => "Everglow/EternalResolve/Items/Weapons/StabbingSwords/Dusts/StarShine_purple";

		public override void OnSpawn(Dust dust)
		{
			dust.frame = new Rectangle(0, Main.rand.Next(3) * 10, 10, 10);
			dust.color.A = (byte)Main.rand.Next(0, 140);
			dust.rotation = Main.rand.NextFromList(6.283f);
		}

		public override bool Update(Dust dust)
		{
			dust.position += dust.velocity;
			dust.scale = (1 - MathF.Sin(dust.color.A / 15f)) * dust.color.R / 100f;
			dust.color.R--;
			if (dust.color.R <= 1)
			{
				dust.color.R = 1;
			}
			dust.velocity *= 0.9f;
			dust.rotation += 0.09f;
			if (dust.color.A < 255)
			{
				dust.color.A += 5;
			}
			if (dust.color.A >= 250)
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
	}
}
