using Everglow.Commons.Templates.Weapons.Yoyos;
using Terraria.Audio;
using Terraria.GameContent;

namespace Everglow.Myth.LanternMoon.Projectiles.Weapons;

public class LanternYoyoProjectile : YoyoProjectile
{
	public Vector2 GapCenter = Vector2.zeroVector;

	public float Timer = 0;

	public override void SetCustomDefaults()
	{
		ProjectileID.Sets.YoyosMaximumRange[Type] = 340;
	}

	public override void AI()
	{
		base.AI();
		Timer++;
		Player player = Main.player[Projectile.owner];
		if (Timer % 20 == 0 && ProjectileOwnFireYoyoCount() < 5)
		{
			SoundStyle sound;
			switch (ProjectileOwnFireYoyoCount() % 3)
			{
				case 0:
					sound = new SoundStyle(ModAsset.LanternYoyo_FireBallSpawn0_Mod);
					break;
				case 1:
					sound = new SoundStyle(ModAsset.LanternYoyo_FireBallSpawn1_Mod);
					break;
				case 2:
					sound = new SoundStyle(ModAsset.LanternYoyo_FireBallSpawn2_Mod);
					break;
				default:
					sound = new SoundStyle(ModAsset.LanternYoyo_FireBallSpawn0_Mod);
					break;
			}

			SoundEngine.PlaySound(sound.WithVolume(Main.rand.NextFloat(0.4f, 0.6f)), GapCenter);
			Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), GapCenter, Vector2.zeroVector, ModContent.ProjectileType<LanternYoyo_FireYoyo>(), (int)(Projectile.damage * 1.6f), 2f, Projectile.owner);
			LanternYoyo_FireYoyo lYfY = p0.ModProjectile as LanternYoyo_FireYoyo;
			if (lYfY is not null)
			{
				lYfY.MainProjYoyo = Projectile;
			}
		}
		Lighting.AddLight(Projectile.Center, new Vector3(1.2f, 0f, 0));
	}

	public int ProjectileOwnFireYoyoCount()
	{
		int count = 0;
		foreach (var proj in Main.projectile)
		{
			if (proj is not null && proj.active && proj.type == ModContent.ProjectileType<LanternYoyo_FireYoyo>())
			{
				LanternYoyo_FireYoyo lYfY = proj.ModProjectile as LanternYoyo_FireYoyo;
				if (lYfY is not null && lYfY.MainProjYoyo == Projectile)
				{
					count++;
				}
			}
		}
		return count;
	}

	public override void DrawYoyo_String(Vector2 playerHeldPos = default)
	{
		base.DrawYoyo_String(playerHeldPos);
	}

	public override void DrawYoyo_String_Attachments(Vector2 drawPos, float rotation, Color color, float length, float index, float stringUnitCount)
	{
		base.DrawYoyo_String_Attachments(drawPos, rotation, color, length, index, stringUnitCount);
	}

	public override void DrawYoyo_String_Pieces(Player player, float unitLength, float finalLength, float finalRot)
	{
		Texture2D tex = TextureAssets.FishingLine.Value;
		int stringColor = player.stringColor;
		Vector2 gapStart = player.Center;
		Vector2 gapEnd = player.Center;
		for (int i = 0; i < YoyoStringPos.Count; i++)
		{
			int toEnd = YoyoStringPos.Count - i - 1;
			if (toEnd is < 6 and > 2)
			{
				continue;
			}
			float drawLength = unitLength;
			float rotation = finalRot;
			if (toEnd > 0)
			{
				rotation = (YoyoStringPos[i + 1] - YoyoStringPos[i]).ToRotation() - MathHelper.PiOver2;
			}
			else
			{
				drawLength = finalLength;
			}
			Vector2 drawPos = YoyoStringPos[i] + tex.Size() * 0.5f - new Vector2(6, 0);
			if (toEnd == 6)
			{
				drawLength += finalLength + 1;
				gapStart = drawPos + new Vector2(0, 1).RotatedBy(rotation) * drawLength;
			}
			if (toEnd == 2)
			{
				drawLength -= finalLength - 1;
				drawPos += new Vector2(0, 1).RotatedBy(rotation) * finalLength;
				gapEnd = drawPos;
			}
			Color color = ModifyYoyoStringColor_VanillaRender(stringColor, drawPos, i, YoyoStringPos.Count);
			drawPos -= Main.screenPosition;
			Main.spriteBatch.Draw(tex, drawPos, new Rectangle(0, 0, tex.Width, (int)drawLength), color, rotation, new Vector2(tex.Width * 0.5f, 0f), 1f, SpriteEffects.None, 0f);
			DrawYoyo_String_Attachments(drawPos, rotation, color, drawLength, i, YoyoStringPos.Count);
		}
		Vector2 gapCenter = (gapStart + gapEnd) * 0.5f;
		GapCenter = gapCenter;
		Texture2D star = Commons.ModAsset.StarSlashGray.Value;
		Texture2D spot = Commons.ModAsset.LightPoint2.Value;
		Main.spriteBatch.Draw(star, gapCenter - Main.screenPosition, null, new Color(0.5f, 0.15f, 0, 0), 0, star.Size() * 0.5f, new Vector2(0.25f), SpriteEffects.None, 0f);
		Main.spriteBatch.Draw(star, gapCenter - Main.screenPosition, null, new Color(0.5f, 0.15f, 0, 0), MathHelper.PiOver2, star.Size() * 0.5f, new Vector2(0.25f, 0.4f), SpriteEffects.None, 0f);
		Main.spriteBatch.Draw(spot, gapCenter - Main.screenPosition, null, new Color(0.5f, 0.15f, 0, 0), MathHelper.PiOver2, spot.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
		if (ProjectileOwnFireYoyoCount() < 5)
		{
			float releaseTimer = Math.Max(0, Timer % 20 - 6);
			float releaseScale = releaseTimer / 13f;
			Color drawColor = Color.Lerp(new Color(1f, 0.48f, 0.1f, 0), new Color(1f, 1f, 0.7f, 0), MathF.Sin((float)Main.time * 0.08f + Projectile.whoAmI) * 0.5f + 0.5f);
			Lighting.AddLight(gapCenter, new Vector3(drawColor.R, drawColor.G * 0.7f, drawColor.B * 0.5f) / 300f);
			Texture2D fireYoyo = ModAsset.LanternYoyo_FireYoyo.Value;
			Main.EntitySpriteDraw(spot, gapCenter - Main.screenPosition, null, drawColor, MathHelper.PiOver2, spot.Size() * 0.5f, releaseScale * 2f, SpriteEffects.None, 0f);
			Main.EntitySpriteDraw(fireYoyo, gapCenter - Main.screenPosition, null, drawColor, Projectile.rotation, tex.Size() * 0.5f, releaseScale, SpriteEffects.None, 0);
		}

		Lighting.AddLight(gapCenter, new Vector3(0.75f, 0.23f, 0));
		DrawLanternLine(player, gapStart, gapEnd, 6, 2);
	}

	public void DrawLanternLine(Player player, Vector2 start, Vector2 end, float indexStart, float indexEnd)
	{
		float count = 10;
		float lineCount = 5f;
		for (int j = 0; j < lineCount; j++)
		{
			for (int k = 0; k < count; k++)
			{
				float value = k / count;
				float valueNext = (k + 1) / count;
				float phase = (j + value) / lineCount * MathHelper.TwoPi + (float)Main.time * 0.03f;
				float phaseNext = (j + valueNext) / lineCount * MathHelper.TwoPi + (float)Main.time * 0.03f;
				float v_dis = (MathF.Sin(value * MathHelper.TwoPi - MathHelper.PiOver2) + 1) * 0.5f * 12 * MathF.Sin(phase);
				float v_disNext = (MathF.Sin(valueNext * MathHelper.TwoPi - MathHelper.PiOver2) + 1) * 0.5f * 12 * MathF.Sin(phaseNext);
				Vector2 pos0 = start * (1 - value) + end * value;
				Vector2 pos1 = start * (1 - valueNext) + end * valueNext;
				Vector2 dir = pos1 - pos0;
				pos0 += dir.SafeNormalize(Vector2.zeroVector).RotatedBy(MathHelper.PiOver2) * v_dis;
				pos1 += dir.SafeNormalize(Vector2.zeroVector).RotatedBy(MathHelper.PiOver2) * v_disNext;
				float indexValue = indexStart * (1 - value) + indexEnd * value;
				DrawLineBetween(player, pos0, pos1, indexValue);
			}
		}
	}

	public void DrawLineBetween(Player player, Vector2 pos0, Vector2 pos1, float index = 0)
	{
		Texture2D tex = TextureAssets.FishingLine.Value;
		Vector2 dir = pos1 - pos0;
		float drawLength = dir.Length();
		float rotation = (-dir).ToRotation() + MathHelper.PiOver2;
		pos0 -= dir.NormalizeSafe() * 0.5f;
		int stringColor = player.stringColor;
		Color color = ModifyYoyoStringColor_VanillaRender(stringColor, pos0, index, YoyoStringPos.Count);
		pos0 -= Main.screenPosition;
		Main.spriteBatch.Draw(tex, pos0, new Rectangle(0, 0, tex.Width, (int)(drawLength + 1)), color, rotation, new Vector2(tex.Width * 0.5f, 0f), 1f, SpriteEffects.None, 0f);
	}
}
