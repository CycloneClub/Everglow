using Everglow.Commons.DataStructures;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.PrimordialJadeWinged_Spear;

public class PrimordialJadeWinged_SpearSpice : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 100;
		Projectile.height = 100;
		Projectile.friendly = true;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 1;
		Projectile.penetrate = -1;
		Projectile.extraUpdates = 1;
		Projectile.localNPCHitCooldown = 30;
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		behindNPCsAndTiles.Add(index);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Player player = Main.player[Projectile.owner];
		var bars = new List<Vertex2D>();
		Vector2 drawRoot = Projectile.Center + new Vector2(0, 8 * player.gravDir);
		var v0 = new Vector2(0, -1);
		var v0T = new Vector2(1, 0);
		float length = Projectile.ai[0];
		v0 = v0 * length * Math.Clamp((130 - Projectile.timeLeft) / 24f, 0, 1f);
		v0T = v0T * 77.77f;
		v0 = v0.RotatedBy(Projectile.rotation);
		v0T = v0T.RotatedBy(Projectile.rotation);

		var cp = new Color(200, 200, 200, 200);
		float fadeG = Math.Clamp((Projectile.timeLeft - 10) / 24f + 0.12f, 0, 1f);

		bars.Add(new Vertex2D(drawRoot + v0 * 2, cp, new Vector3(1, 0, 0)));
		bars.Add(new Vertex2D(drawRoot + (v0 + v0T) * fadeG + v0 * 2 * (1 - fadeG), cp, new Vector3(1, fadeG, 0)));
		bars.Add(new Vertex2D(drawRoot + v0 * 2 * (1 - fadeG), cp, new Vector3(1 - fadeG, fadeG, 0)));

		bars.Add(new Vertex2D(drawRoot + v0 * 2, cp, new Vector3(1, 0, 0)));
		bars.Add(new Vertex2D(drawRoot + v0 * 2 * (1 - fadeG), cp, new Vector3(1 - fadeG, fadeG, 0)));
		bars.Add(new Vertex2D(drawRoot + (v0 - v0T) * fadeG + v0 * 2 * (1 - fadeG), cp, new Vector3(1 - fadeG, 0, 0)));

		Texture2D t = ModAsset.PrimordialJadeWinged_SpearSpice.Value;
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Effect dissolve = Commons.ModAsset.Dissolve.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
		float dissolveDuration = Projectile.timeLeft / 80f - 0.2f;
		if (Projectile.timeLeft > 80)
		{
			dissolveDuration = 1.2f;
		}
		dissolve.Parameters["uTransform"].SetValue(model * projection);
		dissolve.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_spiderNet.Value);
		dissolve.Parameters["duration"].SetValue(dissolveDuration);
		dissolve.Parameters["uDissolveColor"].SetValue(new Vector4(0, 0.9f, 0.9f, 1f));
		dissolve.Parameters["uNoiseSize"].SetValue(4f);
		dissolve.Parameters["uNoiseXY"].SetValue(new Vector2(Projectile.ai[1], Projectile.ai[2]));
		dissolve.CurrentTechnique.Passes[0].Apply();
		Main.graphics.GraphicsDevice.Textures[0] = t;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, bars.ToArray(), 0, bars.Count / 3);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		drawRoot -= Main.screenPosition;
		bars = new List<Vertex2D>();
		float heatGreenDuration = (Projectile.timeLeft - 90) / 30f;
		var cHeat = new Color(MathF.Pow(heatGreenDuration, 3), MathF.Pow(heatGreenDuration, 0.4f), MathF.Pow(heatGreenDuration, 0.6f), heatGreenDuration);
		bars.Add(new Vertex2D(drawRoot + v0 * 2, cHeat, new Vector3(1, 0, 0)));
		bars.Add(new Vertex2D(drawRoot + (v0 + v0T) * fadeG + v0 * 2 * (1 - fadeG), cHeat, new Vector3(1, fadeG, 0)));
		bars.Add(new Vertex2D(drawRoot + v0 * 2 * (1 - fadeG), cHeat, new Vector3(1 - fadeG, fadeG, 0)));

		bars.Add(new Vertex2D(drawRoot + v0 * 2, cHeat, new Vector3(1, 0, 0)));
		bars.Add(new Vertex2D(drawRoot + v0 * 2 * (1 - fadeG), cHeat, new Vector3(1 - fadeG, fadeG, 0)));
		bars.Add(new Vertex2D(drawRoot + (v0 - v0T) * fadeG + v0 * 2 * (1 - fadeG), cHeat, new Vector3(1 - fadeG, 0, 0)));

		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.PrimordialJadeWinged_SpearSpice_highlight.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, bars.ToArray(), 0, bars.Count / 3);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public override void AI()
	{
		if (Projectile.timeLeft <= 118)
		{
			Projectile.friendly = false;
		}
		Projectile.hide = true;
	}

	public static int CyanStrike = 0;

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		CyanStrike = 1;
		Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), target.Center, Vector2.Zero, ModContent.ProjectileType<XiaoHit>(), 0, 0, Projectile.owner, 0.45f);
	}

	public override void Load()
	{
		On_CombatText.NewText_Rectangle_Color_string_bool_bool += CombatText_NewText_Rectangle_Color_string_bool_bool;
	}

	private int CombatText_NewText_Rectangle_Color_string_bool_bool(On_CombatText.orig_NewText_Rectangle_Color_string_bool_bool orig, Rectangle location, Color color, string text, bool dramatic, bool dot)
	{
		if (CyanStrike > 0)
		{
			color = new Color(0, 255, 174);
			CyanStrike--;
		}
		return orig(location, color, text, dramatic, dot);
	}
}
