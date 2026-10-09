namespace Everglow.Myth.TheTusk.Projectiles.Weapon;

internal class SpineGun : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 36;
		Projectile.height = 36;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.penetrate = 2;
		Projectile.timeLeft = 90;
		Projectile.alpha = 255;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Ranged;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
	}

	private int Ran = -1;

	public override Color? GetAlpha(Color lightColor)
	{
		return new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0);
	}

	private bool Release = true;
	private Vector2 oldPo = Vector2.Zero;

	public override void AI()
	{
		Vector2 v0 = Main.MouseWorld - Main.player[Projectile.owner].MountedCenter;
		if (Main.mouseLeft && Release)
		{
			Projectile.ai[0] *= 0.9f;
			Projectile.ai[1] -= 1f;
			Projectile.rotation = (float)(Math.Atan2(v0.Y, v0.X) + Math.PI * 0.25);
			Projectile.Center = Main.player[Projectile.owner].MountedCenter + Vector2.Normalize(v0).RotatedBy(Projectile.ai[0] / 4d) * (8f - Projectile.ai[0] * 4);
			oldPo = Projectile.Center;
			Projectile.Center = oldPo;
			Projectile.velocity *= 0;
		}
		if (!Main.mouseLeft && Release)
		{
			if (Projectile.ai[1] > 0)
			{
				Projectile.ai[0] *= 0.9f;
				Projectile.ai[1] -= 1f;
				Projectile.Center = Main.player[Projectile.owner].MountedCenter + Vector2.Normalize(v0).RotatedBy(Projectile.ai[0] / 4d) * (8f - Projectile.ai[0] * 4);
			}
			else
			{
				Projectile.Kill();
			}
		}
		if (Ran == -1)
		{
			Ran = Main.rand.Next(9);
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}

	public override void PostDraw(Color lightColor)
	{
		if (!Release)
		{
			return;
		}

		Player player = Main.player[Projectile.owner];
		player.heldProj = Projectile.whoAmI;
		Vector2 v0 = Projectile.Center - player.MountedCenter;
		if (Main.mouseLeft)
		{
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (float)(Math.Atan2(v0.Y, v0.X) - Math.PI / 2d));
		}

		Texture2D TexMain = ModContent.Request<Texture2D>("Everglow/Myth/TheTusk/Items/Weapons/SpineGun").Value;
		Color drawColor = Lighting.GetColor((int)Projectile.Center.X / 16, (int)(Projectile.Center.Y / 16.0));
		SpriteEffects se = SpriteEffects.None;
		if (Projectile.Center.X < player.Center.X)
		{
			se = SpriteEffects.FlipVertically;
			player.direction = -1;
		}
		else
		{
			player.direction = 1;
		}
		Main.spriteBatch.Draw(TexMain, Projectile.Center - Main.screenPosition - new Vector2(0, 6), null, drawColor, Projectile.rotation - (float)(Math.PI * 0.25), new Vector2(TexMain.Width / 2f, TexMain.Height / 2f), 1f, se, 0);
	}

	private bool[] HasHit = new bool[200];

	private struct CustomVertexInfo : IVertexType
	{
		private static VertexDeclaration _vertexDeclaration = new VertexDeclaration(new VertexElement[3]
		{
			new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
			new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
			new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0),
		});

		public Vector2 Position;
		public Color Color;
		public Vector3 TexCoord;

		public CustomVertexInfo(Vector2 position, Color color, Vector3 texCoord)
		{
			Position = position;
			Color = color;
			TexCoord = texCoord;
		}

		public VertexDeclaration VertexDeclaration
		{
			get
			{
				return _vertexDeclaration;
			}
		}
	}
}
