namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.PrimordialJadeWinged_Spear;

public class PrimordialJadeWinged_SpearShake : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 100;
		Projectile.height = 180;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 80;
		Projectile.alpha = 0;
		Projectile.penetrate = -1;
		Projectile.extraUpdates = 1;
	}

	public override void AI()
	{
		Projectile.hide = true;
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		behindNPCsAndTiles.Add(index);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Player player = Main.player[Projectile.owner];
		var Vx = new List<Vertex2D>();
		Vector2 Vbase = Projectile.Center - Main.screenPosition + new Vector2(0, 24 * player.gravDir);
		var v0 = new Vector2(0, -1);
		var v0T = new Vector2(1, 0);
		float length = Projectile.ai[0];
		v0 = v0 * length * Math.Clamp((80 - Projectile.timeLeft) / 24f, 0, 1f);
		v0T = v0T * 77.77f * Projectile.timeLeft / 120f;
		v0 = v0.RotatedBy(Projectile.rotation);
		v0T = v0T.RotatedBy(Projectile.rotation);

		var cr = new Color(0.0f, 0.17f, 0.17f, 0);
		float fadeK = Math.Clamp((Projectile.timeLeft - 10) / 24f, 0, 1f);

		Vx.Add(new Vertex2D(Vbase + v0 * 2, cr, new Vector3(1, 0, 0)));
		Vx.Add(new Vertex2D(Vbase + (v0 + v0T) * fadeK + v0 * 2 * (1 - fadeK), cr, new Vector3(1, fadeK, 0)));
		Vx.Add(new Vertex2D(Vbase + v0 * 2 * (1 - fadeK), cr, new Vector3(1 - fadeK, fadeK, 0)));

		Vx.Add(new Vertex2D(Vbase + v0 * 2, cr, new Vector3(1, 0, 0)));
		Vx.Add(new Vertex2D(Vbase + v0 * 2 * (1 - fadeK), cr, new Vector3(1 - fadeK, fadeK, 0)));
		Vx.Add(new Vertex2D(Vbase + (v0 - v0T) * fadeK + v0 * 2 * (1 - fadeK), cr, new Vector3(1 - fadeK, 0, 0)));

		Texture2D t = ModContent.Request<Texture2D>("Everglow/Myth/Misc/Projectiles/Weapon/Melee/PrimordialJadeWinged_Spear/PrimordialJadeWinged_SpearShake").Value;

		Main.graphics.GraphicsDevice.Textures[0] = t;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Vx.ToArray(), 0, Vx.Count / 3);
		return false;
	}
}
