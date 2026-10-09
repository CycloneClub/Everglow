using Everglow.Commons.Templates.Weapons.Slingshots;
using Everglow.Myth.Common;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Ranged.Slingshots;

public abstract class GemSlingshotProjectile : SlingshotProjectile
{
	/// <summary>
	/// 弦上的宝石贴图,从MythModule(不含)开始的路径
	/// </summary>
	internal string TexPath = "";
	public override void SetDef()
	{
		SlingshotLength = 8;
		SplitBranchDis = 10;
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
		Texture2D TexMain = MythContent.QuickTexture(TexPath);
		Main.spriteBatch.Draw(TexMain, Head1 * 0.67f + SlingshotStringTail * 0.33f, null, drawColor, DrawRot, TexMain.Size() / 2f, 1f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(TexMain, Head1 * 0.33f + SlingshotStringTail * 0.67f, null, drawColor, DrawRot, TexMain.Size() / 2f, 1f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(TexMain, SlingshotStringTail, null, drawColor, DrawRot, TexMain.Size() / 2f, 1f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(TexMain, Head2 * 0.33f + SlingshotStringTail * 0.67f, null, drawColor, DrawRot, TexMain.Size() / 2f, 1f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(TexMain, Head2 * 0.67f + SlingshotStringTail * 0.33f, null, drawColor, DrawRot, TexMain.Size() / 2f, 1f, SpriteEffects.None, 0);
	}
}
