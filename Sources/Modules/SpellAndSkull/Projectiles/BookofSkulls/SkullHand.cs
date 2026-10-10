using Everglow.Commons.Vertex;
using Everglow.SpellAndSkull.Common;
using Terraria;
using Terraria.Audio;

namespace Everglow.SpellAndSkull.Projectiles.BookofSkulls;

internal class SkullHand : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 100;
		Projectile.height = 100;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 600;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 4;
	}

	private Vector2 direction = new Vector2(0, -1);
	private Vector2 armPos;
	private Vector2[,] fingerPos = new Vector2[4, 4];
	private Vector2[] thumbPos = new Vector2[3];
	private float[] finRot1 = new float[5];
	private float[] finRot2 = new float[5];
	private float[] finRot3 = new float[4];
	private float[] finLength1 = new float[5];
	private float[] finLength2 = new float[5];
	private float[] finLength3 = new float[4];
	private int dir = 1;

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		if (Projectile.timeLeft == 600)
		{
			Vector2 TotalVector = Vector2.Zero; // 合向量
			int TCount = 0;
			for (int a = 0; a < 12; a++)
			{
				Vector2 v0 = new Vector2(10, 0).RotatedBy(a / 6d * Math.PI);
				if (Collision.SolidCollision(Projectile.Center + v0, 1, 1))
				{
					TotalVector -= v0;
					TCount++;
				}
				else
				{
					TotalVector += v0;
				}
			}
			for (int a = 0; a < 24; a++)
			{
				Vector2 v0 = new Vector2(20, 0).RotatedBy(a / 12d * Math.PI);
				if (Collision.SolidCollision(Projectile.Center + v0, 1, 1))
				{
					TotalVector -= v0 * 0.5f;
					TCount++;
				}
				else
				{
					TotalVector += v0 * 0.5f;
				}
			}
			direction = TotalVector.SafeNormalize(new Vector2(0, -1));
			Projectile.velocity = direction * 0.15f;
			Projectile.Center -= Projectile.velocity * 600;
			finLength1[0] = 66;
			finLength1[1] = 66;
			finLength1[2] = 66;
			finLength1[3] = 60;
			finLength1[4] = 66;

			finLength2[0] = 45;
			finLength2[1] = 45;
			finLength2[2] = 42f;
			finLength2[3] = 40;
			finLength2[4] = 38f;

			finLength3[0] = 28f;
			finLength3[1] = 32f;
			finLength3[2] = 27f;
			finLength3[3] = 26f;

			finRot1[0] = 0.18f;
			finRot1[1] = -0.14f;
			finRot1[2] = -0.4f;
			finRot1[3] = -0.76f;
			finRot1[4] = 0.78f;

			finRot2[0] = 0f;
			finRot2[1] = -0.012f;
			finRot2[2] = -0.04f;
			finRot2[3] = -0.048f;
			finRot2[4] = -0.48f;

			finRot3[0] = 0f;
			finRot3[1] = 0f;
			finRot3[2] = 0.08f;
			finRot3[3] = 0.16f;
			if (Main.rand.NextBool(2))
			{
				dir = -1;
			}

			SoundEngine.PlaySound(new SoundStyle("Everglow/SpellAndSkull/Sounds/MothHitCocoon"), Projectile.Center);
		}
		if (Projectile.timeLeft > 300)
		{
			finRot1[0] = UpdateSelf(finRot1[0], 0.36f);
			finRot1[1] = UpdateSelf(finRot1[1], -0.28f);
			finRot1[2] = UpdateSelf(finRot1[2], -0.8f);
			finRot1[3] = UpdateSelf(finRot1[3], -1.22f);
			finRot1[4] = UpdateSelf(finRot1[4], 1.56f);

			finRot2[0] = UpdateSelf(finRot2[0], 0.26f);
			finRot2[1] = UpdateSelf(finRot2[1], -0.18f);
			finRot2[2] = UpdateSelf(finRot2[2], -0.8f);
			finRot2[3] = UpdateSelf(finRot2[3], -1.38f);
			finRot2[4] = UpdateSelf(finRot2[4], 1.26f);

			finRot3[0] = UpdateSelf(finRot3[0], 0.26f);
			finRot3[1] = UpdateSelf(finRot3[1], -0.18f);
			finRot3[2] = UpdateSelf(finRot3[2], -0.6f);
			finRot3[3] = UpdateSelf(finRot3[3], -0.9f);
		}
		else if (Projectile.timeLeft > 200)
		{
			finRot1[0] = UpdateSelf(finRot1[0], 0.26f, 0.9f);
			finRot1[1] = UpdateSelf(finRot1[1], -0.16f, 0.9f);
			finRot1[2] = UpdateSelf(finRot1[2], -0.54f, 0.9f);
			finRot1[3] = UpdateSelf(finRot1[3], -0.75f, 0.9f);
			finRot1[4] = UpdateSelf(finRot1[4], 0.7f, 0.9f);

			finRot2[0] = UpdateSelf(finRot2[0], 2.2f, 0.9f);
			finRot2[1] = UpdateSelf(finRot2[1], 1.9f, 0.9f);
			finRot2[2] = UpdateSelf(finRot2[2], 1.8f, 0.9f);
			finRot2[3] = UpdateSelf(finRot2[3], 1.7f, 0.9f);
			finRot2[4] = UpdateSelf(finRot2[4], -0.7f, 0.9f);

			finRot3[0] = UpdateSelf(finRot3[0], 5f, 0.9f);
			finRot3[1] = UpdateSelf(finRot3[1], 5f, 0.9f);
			finRot3[2] = UpdateSelf(finRot3[2], 4f, 0.9f);
			finRot3[3] = UpdateSelf(finRot3[3], 3f, 0.9f);
		}
		float Ang = (float)(Math.Atan2(direction.Y, direction.X) - Math.PI * 1.5);

		armPos = Projectile.Center;
		thumbPos[0] = armPos;

		for (int x = 0; x < 4; x++)
		{
			fingerPos[x, 0] = armPos;
			fingerPos[x, 1] = fingerPos[x, 0] + new Vector2(0, -finLength1[x]).RotatedBy(finRot1[x] * dir + Ang);
			fingerPos[x, 2] = fingerPos[x, 1] + new Vector2(0, -finLength2[x]).RotatedBy(finRot2[x] * dir + Ang);
			fingerPos[x, 3] = fingerPos[x, 2] + new Vector2(0, -finLength3[x]).RotatedBy(finRot3[x] * dir + Ang);
		}
		thumbPos[0] = armPos;
		thumbPos[1] = thumbPos[0] + new Vector2(0, -finLength1[4]).RotatedBy(finRot1[4] * dir + Ang);
		thumbPos[2] = thumbPos[1] + new Vector2(0, -finLength2[4]).RotatedBy(finRot2[4] * dir + Ang);

		if (Projectile.timeLeft > 535)
		{
			Projectile.velocity *= 1.04f;
		}
		else if (Projectile.timeLeft > 300)
		{
			Projectile.velocity *= 0.97f;
		}
		else if (Projectile.timeLeft > 280)
		{
			ScreenShaker Ss = player.GetModPlayer<ScreenShaker>();
			float Strength = Projectile.timeLeft - 280;
			Strength /= 20f;
			Strength *= Strength * 200f;
			Ss.FlyCamPosition = new Vector2(0, Strength).RotatedByRandom(6.283);
			Projectile.friendly = true;
			Projectile.velocity *= 1.55f;
		}
		else if (Projectile.timeLeft > 240)
		{
			Projectile.velocity *= 0.6f;
		}
		else if (Projectile.timeLeft == 209)
		{
			Projectile.velocity = Vector2.Normalize(Projectile.velocity) * -0.1f;
		}
		else if (Projectile.timeLeft is < 209 and > 30)
		{
			Projectile.velocity *= 1.02f;
		}
		else if (Projectile.timeLeft < 30)
		{
			Projectile.Kill();
		}

		if (Projectile.timeLeft == 282)
		{
			for (int f = 0; f < 80; f++)
			{
				Vector2 v0 = new Vector2(Main.rand.NextFloat(3f, 5f), 0).RotatedByRandom(6.283);
				var dust0 = Dust.NewDustDirect(Projectile.Center + Vector2.Normalize(Projectile.velocity) * 75, 0, 0, DustID.Torch, v0.X, v0.Y, 100, default, Main.rand.NextFloat(0.6f, 1.8f));
				dust0.velocity = v0;
			}
			SoundEngine.PlaySound(SoundID.Item89, Projectile.Center);
		}
	}

	private float UpdateSelf(float sel, float aim, float value = 0.99f)
	{
		value = Math.Clamp(value, 0, 1);
		return sel * value + aim * (1 - value);
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		behindNPCsAndTiles.Add(index);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(BuffID.Burning, 180);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Projectile.hide = true;
		Texture2D bone = ModAsset.SkullHand.Value;
		Texture2D Power = Commons.ModAsset.Trail_5.Value;
		Vector2 v0 = Projectile.Center + direction * 60;
		Color c0 = Lighting.GetColor((int)(v0.X / 16f), (int)(v0.Y / 16f));

		float Pwidth = 0f;
		float Pdark = 0f;
		if (Projectile.timeLeft is > 280 and < 330)
		{
			Pwidth = (330 - Projectile.timeLeft) / 33f;
			Pdark = (330 - Projectile.timeLeft) / 50f;
		}
		else if (Projectile.timeLeft < 280)
		{
			Pwidth = 1f;
			Pdark = (Projectile.timeLeft - 20) / 260f;
		}
		var c1 = new Color(1f * Pdark, 0.45f * Pdark * Pdark, 0f, 0f);
		DrawPowerLine(armPos - direction * 80f, armPos, c1, 32f * Pwidth, Power);
		for (int x = 3; x >= 0; x--)
		{
			for (int y = 0; y < 3; y++)
			{
				DrawPowerLine(fingerPos[x, y], fingerPos[x, y + 1], c1, 18f * Pwidth, Power);
			}
		}
		for (int y = 0; y < 2; y++)
		{
			DrawPowerLine(thumbPos[y], thumbPos[y + 1], c1, 20f * Pwidth, Power);
		}

		DrawTexLine(armPos - direction * 80f, armPos, c0, 12f, bone);
		for (int x = 3; x >= 0; x--)
		{
			for (int y = 0; y < 3; y++)
			{
				DrawTexLine(fingerPos[x, y], fingerPos[x, y + 1], c0, 6f, bone);
			}
		}
		for (int y = 0; y < 2; y++)
		{
			DrawTexLine(thumbPos[y], thumbPos[y + 1], c0, 8f, bone);
		}
		return false;
	}

	public void DrawTexLine(Vector2 StartPos, Vector2 EndPos, Color c0, float Wid, Texture2D tex)
	{
		Vector2 Width = Vector2.Normalize(StartPos - EndPos).RotatedBy(Math.PI / 2d) * Wid;

		var vertex2Ds = new List<Vertex2D>();

		for (int x = 0; x < 3; x++)
		{
			vertex2Ds.Add(new Vertex2D(StartPos + Width + new Vector2(x / 3f).RotatedBy(x) - Main.screenPosition, c0, new Vector3(0, 0, 0)));
			vertex2Ds.Add(new Vertex2D(EndPos + Width + new Vector2(x / 3f).RotatedBy(x) - Main.screenPosition, c0, new Vector3(0, 1, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos - Width + new Vector2(x / 3f).RotatedBy(x) - Main.screenPosition, c0, new Vector3(1, 0, 0)));

			vertex2Ds.Add(new Vertex2D(EndPos + Width + new Vector2(x / 3f).RotatedBy(x) - Main.screenPosition, c0, new Vector3(0, 1, 0)));
			vertex2Ds.Add(new Vertex2D(EndPos - Width + new Vector2(x / 3f).RotatedBy(x) - Main.screenPosition, c0, new Vector3(1, 1, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos - Width + new Vector2(x / 3f).RotatedBy(x) - Main.screenPosition, c0, new Vector3(1, 0, 0)));
		}

		Main.graphics.GraphicsDevice.Textures[0] = tex;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertex2Ds.ToArray(), 0, vertex2Ds.Count / 3);
	}

	public void DrawPowerLine(Vector2 StartPos, Vector2 EndPos, Color c0, float Wid, Texture2D tex)
	{
		Vector2 Width = Vector2.Normalize(StartPos - EndPos).RotatedBy(Math.PI / 2d) * Wid;

		var vertex2Ds = new List<Vertex2D>();

		for (int x = 0; x < 3; x++)
		{
			float Value0 = (float)(Main.timeForVisualEffects / 291d + 200) % 1f;
			float Value1 = (float)(Main.timeForVisualEffects / 291d + 200.05) % 1f;
			vertex2Ds.Add(new Vertex2D(StartPos + Width + new Vector2(x / 3f).RotatedBy(x) - Main.screenPosition, c0, new Vector3(Value0, 0, 0)));
			vertex2Ds.Add(new Vertex2D(EndPos + Width + new Vector2(x / 3f).RotatedBy(x) - Main.screenPosition, c0, new Vector3(Value1, 0, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos - Width + new Vector2(x / 3f).RotatedBy(x) - Main.screenPosition, c0, new Vector3(Value0, 1, 0)));

			vertex2Ds.Add(new Vertex2D(EndPos + Width + new Vector2(x / 3f).RotatedBy(x) - Main.screenPosition, c0, new Vector3(Value1, 0, 0)));
			vertex2Ds.Add(new Vertex2D(EndPos - Width + new Vector2(x / 3f).RotatedBy(x) - Main.screenPosition, c0, new Vector3(Value1, 1, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos - Width + new Vector2(x / 3f).RotatedBy(x) - Main.screenPosition, c0, new Vector3(Value0, 1, 0)));
		}

		Main.graphics.GraphicsDevice.Textures[0] = tex;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertex2Ds.ToArray(), 0, vertex2Ds.Count / 3);
	}
}
