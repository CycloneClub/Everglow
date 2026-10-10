namespace Everglow.EternalResolve.Bosses.Projectiles
{
	public class Rapier_Slash : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.aiStyle = -1;
			Projectile.timeLeft = 90;
			Projectile.tileCollide = false;
			Projectile.scale = 2;
			alpha = 0;
		}

		private float alpha = 0;

		public override bool? CanDamage()
		{
			return false;
		}

		public override void AI()
		{
			if (Projectile.timeLeft > 60)
			{
				alpha = MathHelper.Lerp(alpha, 1f, 0.1f);
			}
			else
			{
				alpha = MathHelper.Lerp(alpha, 0f, 0.1f);
			}
			if (Projectile.timeLeft <= 30)
			{
				if (Projectile.timeLeft % 4 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
				{
					Vector2 pos = Projectile.Center + Main.rand.NextVector2Unit() * 120;
					Vector2 vel = pos.DirectionTo(Projectile.Center).RotatedByRandom(0.4f) * 0.7f;
					Main.npc[(int)Projectile.ai[0]].Center = pos;
					Main.npc[(int)Projectile.ai[0]].velocity = vel * 15;
					Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, vel, ModContent.ProjectileType<CrutchRapier_Stab_Hostile>(), Projectile.damage, 0, Projectile.owner, Projectile.ai[0], 2f);
				}
			}
		}

		public override bool PreDraw(Player player, ref Color lightColor)
		{
			Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
			float factor = (float)Math.Sin(Main.timeForVisualEffects * 0.2f) / 2 + 0.5f;
			Color color = Color.Lerp(new Color(1f, 1f, 0.5f, 0f), new Color(1f, 0.5f, 0.5f, 0f), factor);
			Color c = color * alpha;
			Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, c, 0, tex.Size() / 2, Projectile.scale, 0, 0);
			Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, c, 0, tex.Size() / 2, Projectile.scale, SpriteEffects.FlipHorizontally, 0);

			return false;
		}
	}
}
