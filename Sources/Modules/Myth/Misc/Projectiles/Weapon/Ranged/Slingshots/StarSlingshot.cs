using Everglow.Myth.Common;
using Everglow.Commons.Templates.Weapons.Slingshots;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Ranged.Slingshots;

internal class StarSlingshot : SlingshotProjectile
{
	public override void SetDef()
	{
		ShootProjType = ModContent.ProjectileType<StarAmmo>();
		SlingshotLength = 10;
		SplitBranchDis = 6;
	}

	public override void DrawString()
	{
		Player player = Main.player[Projectile.owner];
		Color drawColor = Lighting.GetColor((int)(Projectile.Center.X / 16.0), (int)(Projectile.Center.Y / 16.0));
		float DrawRot;
		if (Projectile.Center.X < player.MountedCenter.X)
		{
			DrawRot = Projectile.rotation - MathF.PI / 4f;
		}
		else
		{
			DrawRot = Projectile.rotation - MathF.PI * 0.25f;
		}
		Vector2 HeadCenter = new Vector2(SlingshotLength, -SlingshotLength).RotatedBy(DrawRot);
		if (player.direction == -1)
		{
			HeadCenter = new Vector2(SlingshotLength, -SlingshotLength).RotatedBy(DrawRot + Math.PI / 2d);
		}

		HeadCenter += Projectile.Center - Main.screenPosition;
		Vector2 SlingshotStringHead = new Vector2(SlingshotLength, -SlingshotLength).RotatedBy(DrawRot) + Projectile.Center - Main.MouseWorld;
		Vector2 SlingshotStringTail = new Vector2(SlingshotLength, -SlingshotLength).RotatedBy(DrawRot) + Vector2.Normalize(SlingshotStringHead) * Power * 0.2625f;
		if (player.direction == -1)
		{
			SlingshotStringHead = new Vector2(SlingshotLength, -SlingshotLength).RotatedBy(DrawRot + Math.PI / 2d) + Projectile.Center - Main.MouseWorld;
			SlingshotStringTail = new Vector2(SlingshotLength, -SlingshotLength).RotatedBy(DrawRot + Math.PI / 2d) + Vector2.Normalize(SlingshotStringHead) * Power * 0.2625f;
		}
		SlingshotStringTail += Projectile.Center - Main.screenPosition;
		Vector2 Head1 = HeadCenter + HeadCenter.RotatedBy(Math.PI / 8 + DrawRot).SafeNormalize(Vector2.Zero) * SplitBranchDis;
		Vector2 Head2 = HeadCenter - HeadCenter.RotatedBy(Math.PI / 8 + DrawRot).SafeNormalize(Vector2.Zero) * SplitBranchDis;
		if (player.direction == -1)
		{
			Head1 = HeadCenter + HeadCenter.RotatedBy(Math.PI / 8 * 5 + DrawRot).SafeNormalize(Vector2.Zero) * SplitBranchDis;
			Head2 = HeadCenter - HeadCenter.RotatedBy(Math.PI / 8 * 5 + DrawRot).SafeNormalize(Vector2.Zero) * SplitBranchDis;
		}
		var Light = new Color(Power / 120f, Power / 260f, 0, 0);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		var dColor = Color.Lerp(drawColor, new Color(20, 20, 240, 40), Power / 120f);
		DrawTexLine(Head1, SlingshotStringTail, 1, dColor, Light, ModAsset.String.Value);
		DrawTexLine(Head2, SlingshotStringTail, 1, dColor, Light, ModAsset.String.Value);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		Texture2D star = ModAsset.SlingshotHitStar.Value;
		Main.spriteBatch.Draw(star, SlingshotStringTail, null, Light, 0, star.Size() / 2f, new Vector2(0.06f, 0.23f + MathF.Sin((float)(Main.timeForVisualEffects * 0.1)) * 0.2f) * Power / 120f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, SlingshotStringTail, null, Light, MathF.PI / 2, star.Size() / 2f, new Vector2(0.06f, 0.23f + MathF.Sin((float)(Main.timeForVisualEffects * 0.1)) * 0.2f) * Power / 120f, SpriteEffects.None, 0);

		Lighting.AddLight(SlingshotStringTail + Main.screenPosition, Light.R / 555f, Light.G / 555f, Light.B / 555f);
	}

	public void DrawTexLine(Vector2 StartPos, Vector2 EndPos, float width, Color color1, Color color2, Texture2D tex)
	{
		Vector2 Width = Vector2.Normalize(StartPos - EndPos).RotatedBy(Math.PI / 2d) * width;

		var vertex2Ds = new List<Vertex2D>();

		vertex2Ds.Add(new Vertex2D(StartPos + Width, color1, new Vector3(0, 0, 0)));
		vertex2Ds.Add(new Vertex2D(StartPos - Width, color1, new Vector3(0, 1, 0)));

		vertex2Ds.Add(new Vertex2D(EndPos + Width, color2, new Vector3(1, 0, 0)));
		vertex2Ds.Add(new Vertex2D(EndPos - Width, color2, new Vector3(1, 1, 0)));

		Main.graphics.GraphicsDevice.Textures[0] = tex;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertex2Ds.ToArray(), 0, vertex2Ds.Count - 2);
	}

	public override void PostDraw(Color lightColor)
	{
		Player player = Main.player[Projectile.owner];
		Texture2D TexMain = ModAsset.StarSlingsh_glow.Value;
		var drawColor = new Color(255, 255, 255, 0);
		SpriteEffects spriteEffect = SpriteEffects.None;
		float DrawRot = Projectile.rotation - MathF.PI / 4f;
		if (Projectile.Center.X < player.MountedCenter.X)
		{
			spriteEffect = SpriteEffects.FlipVertically;
		}

		Main.spriteBatch.Draw(TexMain, Projectile.Center - Main.screenPosition, null, drawColor, DrawRot, TexMain.Size() / 2f, 1f, spriteEffect, 0);
		base.PostDraw(lightColor);
	}
}
