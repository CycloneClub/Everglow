using Everglow.Myth.Common;
using Everglow.Commons.Templates.Weapons.Slingshots;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Ranged.Slingshots;

internal class JungleSlingshot : SlingshotProjectile
{
	public override void SetDef()
	{
		ShootProjType = ModContent.ProjectileType<GlowSporeBead>();
		SlingshotLength = 8;
		SplitBranchDis = 6;
	}
	public override void DrawString()
	{
		base.DrawString();
		Player player = Main.player[Projectile.owner];
		Color drawColor = Lighting.GetColor((int)(Projectile.Center.X / 16.0), (int)(Projectile.Center.Y / 16.0));
		float DrawRot = Projectile.rotation - MathF.PI / 4f;
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

		var newcolor = Color.Lerp(drawColor, new Color(255, 255, 255, 200), Power / 130f + 0.1f);
		Texture2D TexMain = ModAsset.GlowingSpore.Value;
		Texture2D TexMainII = ModAsset.GlowSporeBead.Value;
		Main.spriteBatch.Draw(TexMain, Head1 * 0.67f + SlingshotStringTail * 0.33f, null, newcolor, DrawRot, TexMain.Size() / 2f, 1f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(TexMainII, Head1 * 0.95f + SlingshotStringTail * 0.05f, null, newcolor, DrawRot, TexMainII.Size() / 2f, 0.85f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(TexMain, Head1 * 0.33f + SlingshotStringTail * 0.67f, null, newcolor, DrawRot, TexMain.Size() / 2f, 1f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(TexMainII, SlingshotStringTail, null, newcolor, DrawRot, TexMainII.Size() / 2f, 1f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(TexMain, Head2 * 0.33f + SlingshotStringTail * 0.67f, null, newcolor, DrawRot, TexMain.Size() / 2f, 1f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(TexMainII, Head2 * 0.95f + SlingshotStringTail * 0.05f, null, newcolor, DrawRot, TexMainII.Size() / 2f, 0.85f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(TexMain, Head2 * 0.67f + SlingshotStringTail * 0.33f, null, newcolor, DrawRot, TexMain.Size() / 2f, 1f, SpriteEffects.None, 0);
	}
}
