using Everglow.Commons.DataStructures;
using Everglow.Commons.Mechanics.EliminateLight;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Ranged.Slingshots;

public class AmbiguousLine : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.friendly = true;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 60;
		Projectile.extraUpdates = 3;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 60;
	}

	public override void AI()
	{
		Projectile.scale = Projectile.timeLeft / 60f;
		if (Projectile.timeLeft <= 58)
		{
			Projectile.friendly = false;
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		foreach (var proj in Main.projectile)
		{
			if (proj.whoAmI > Projectile.whoAmI)
			{
				if (proj.ai[0] == Projectile.ai[0])
				{
					DrawShadowLine(proj.Center - Main.screenPosition, Projectile.Center - Main.screenPosition, Projectile.scale * 5);
				}
			}
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		EliminateLightManager.AddCircle(Projectile.Center, Projectile.timeLeft);
		return false;
	}

	public override void PostDraw(Color lightColor)
	{
		Texture2D shadow = ModAsset.CursedHit.Value;
		Texture2D blackHole = ModAsset.BlackHole_texture.Value;
		Texture2D blue = ModAsset.CorruptLight.Value;
		Main.spriteBatch.Draw(shadow, Projectile.Center - Main.screenPosition, null, Color.White, 0, shadow.Size() / 2f, 0.4f * Projectile.scale, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(blackHole, Projectile.Center - Main.screenPosition, null, Color.White, 0, blackHole.Size() / 2f, 0.06f * MathF.Sqrt(Projectile.scale), SpriteEffects.None, 0);
		Main.spriteBatch.Draw(blue, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0), 0, blue.Size() / 2f, 0.10f * Projectile.scale, SpriteEffects.None, 0);
	}

	public void DrawShadowLine(Vector2 StartPos, Vector2 EndPos, float width)
	{
		Color color = Color.White;
		Vector2 Width = Vector2.Normalize(StartPos - EndPos).RotatedBy(Math.PI / 2d) * width * 4;
		var vertex2Ds = new List<Vertex2D>();
		vertex2Ds.Add(new Vertex2D(StartPos + Width, color, new Vector3(0, 0, 0)));
		vertex2Ds.Add(new Vertex2D(StartPos - Width, color, new Vector3(0, 1, 0)));
		vertex2Ds.Add(new Vertex2D(EndPos + Width, color, new Vector3(1, 0, 0)));
		vertex2Ds.Add(new Vertex2D(EndPos - Width, color, new Vector3(1, 1, 0)));
		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.SlingshotTrailBlack.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertex2Ds.ToArray(), 0, vertex2Ds.Count - 2);

		color = new Color(255, 255, 255, 0);
		Width = Vector2.Normalize(StartPos - EndPos).RotatedBy(Math.PI / 2d) * width;
		vertex2Ds = new List<Vertex2D>();
		vertex2Ds.Add(new Vertex2D(StartPos + Width, color, new Vector3(0, 0, 0)));
		vertex2Ds.Add(new Vertex2D(StartPos - Width, color, new Vector3(0, 1, 0)));
		vertex2Ds.Add(new Vertex2D(EndPos + Width, color, new Vector3(1, 0, 0)));
		vertex2Ds.Add(new Vertex2D(EndPos - Width, color, new Vector3(1, 1, 0)));

		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.ShadowTrail.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertex2Ds.ToArray(), 0, vertex2Ds.Count - 2);

		color = Color.White;
		Width = Vector2.Normalize(StartPos - EndPos).RotatedBy(Math.PI / 2d) * width * 4;
		vertex2Ds = new List<Vertex2D>();
		vertex2Ds.Add(new Vertex2D(StartPos + Width, color, new Vector3(0, 0, 0)));
		vertex2Ds.Add(new Vertex2D(StartPos - Width, color, new Vector3(0, 1, 0)));
		vertex2Ds.Add(new Vertex2D(EndPos + Width, color, new Vector3(1, 0, 0)));
		vertex2Ds.Add(new Vertex2D(EndPos - Width, color, new Vector3(1, 1, 0)));
		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.ShadowTrailFlame.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertex2Ds.ToArray(), 0, vertex2Ds.Count - 2);
	}
}
