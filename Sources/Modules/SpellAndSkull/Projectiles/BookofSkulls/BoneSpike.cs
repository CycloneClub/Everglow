using Everglow.Commons.MEAC;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Terraria.Audio;

namespace Everglow.SpellAndSkull.Projectiles.BookofSkulls;

public class BoneSpike : ModProjectile, IWarpProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 14;
		Projectile.height = 14;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.penetrate = 1;
		Projectile.timeLeft = 1800;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.tileCollide = false;
	}

	private bool shot = false;

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		if (!shot)
		{
			Vector2 AIM0 = player.Center + new Vector2(0, 12 * player.gravDir * (float)(0.2 + Math.Sin(Main.timeForVisualEffects / 18d) / 2d)) + new Vector2(-60 * player.direction, 30).RotatedBy((Projectile.ai[0] - 1) / 4.5 * Math.PI * player.direction) * Projectile.ai[1];
			if (player.itemTime > 0 && player.active)
			{
				AIM0 = player.Center + new Vector2(player.direction * -12, -24 * player.gravDir * (float)(0.2 + Math.Sin(Main.timeForVisualEffects / 18d) / 2d)) + new Vector2(-120 * player.direction, 60).RotatedBy((Projectile.ai[0] - 1) / 4.5 * Math.PI * player.direction) * Projectile.ai[1];
			}

			Projectile.Center = Projectile.Center * (-Projectile.ai[0] / 50f + 0.97f) + AIM0 * (Projectile.ai[0] / 50f + 0.03f);
			Projectile.rotation = Math.Clamp(Projectile.velocity.X / 21f, -1, 1);
			Projectile.velocity *= 0.9f;
			if (player.itemTime == 2 && Projectile.timeLeft <= 1750)
			{
				Projectile.friendly = true;
				Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * 45;
				Projectile.tileCollide = true;
				shot = true;
			}

			if (player.HeldItem.type != ItemID.BookofSkulls || Projectile.timeLeft <= 100)
			{
				Projectile.friendly = true;
				Projectile.velocity = new Vector2(0, 20);
				Projectile.tileCollide = true;
				shot = true;
			}
			Projectile.hide = true;
		}
		else
		{
			Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) - (float)(Math.PI * 0.5);
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(BuffID.Burning, 180);
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		overPlayers.Add(index);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D Spice = ModAsset.BoneSpike.Value;
		if (Projectile.timeLeft <= 1780)
		{
			Vector2 v0 = Projectile.Center;
			Color c0 = Lighting.GetColor((int)(v0.X / 16f), (int)(v0.Y / 16f));
			DrawTexLine(v0 + new Vector2(0, -31).RotatedBy(Projectile.rotation), v0 - new Vector2(0, -31).RotatedBy(Projectile.rotation), c0, 7, Spice);
		}
		return false;
	}

	public override void PostDraw(Color lightColor)
	{
		Texture2D Power = Commons.ModAsset.Trail_5.Value;
		float Pdark = 0f;
		float Pwidth = 1f;
		if (Projectile.timeLeft > 1780)
		{
			Pwidth = (1800 - Projectile.timeLeft) / 20f;
			Pdark = (1800 - Projectile.timeLeft) / 20f;
		}
		else if (Projectile.timeLeft > 1750)
		{
			Pdark = (Projectile.timeLeft - 1750) / 30f;
		}
		var c1 = new Color(1f * Pdark, 0.45f * Pdark * Pdark, 0f, 0f);
		DrawTexLineColor(Projectile.Center + new Vector2(0, -70).RotatedBy(Projectile.rotation), Projectile.Center, Color.Transparent, c1, 14f * Pwidth, Power);
		DrawTexLineColor(Projectile.Center, Projectile.Center + new Vector2(0, 70).RotatedBy(Projectile.rotation), c1, Color.Transparent, 14f * Pwidth, Power);
	}

	public void DrawPowerLine(Vector2 StartPos, Vector2 EndPos, Color c0, float Wid, Texture2D tex)
	{
		Vector2 Width = Vector2.Normalize(StartPos - EndPos).RotatedBy(Math.PI / 2d) * Wid;

		var vertex2Ds = new List<Vertex2D>();

		for (int x = 0; x < 3; x++)
		{
			float Value0 = (float)(Main.timeForVisualEffects / 291d + 20) % 1f;
			float Value1 = (float)(Main.timeForVisualEffects / 291d + 20.03) % 1f;
			if (Value1 < Value0)
			{
				float D0 = 1 - Value0;
				Vector2 Delta = EndPos - StartPos;
				vertex2Ds.Add(new Vertex2D(StartPos + Width + new Vector2(x / 3f).RotatedBy(x), c0, new Vector3(Value0, 0, 0)));
				vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 + Width + new Vector2(x / 3f).RotatedBy(x), c0, new Vector3(1, 0, 0)));
				vertex2Ds.Add(new Vertex2D(StartPos - Width + new Vector2(x / 3f).RotatedBy(x), c0, new Vector3(Value0, 1, 0)));

				vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 + Width + new Vector2(x / 3f).RotatedBy(x), c0, new Vector3(1, 0, 0)));
				vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 - Width + new Vector2(x / 3f).RotatedBy(x), c0, new Vector3(1, 1, 0)));
				vertex2Ds.Add(new Vertex2D(StartPos - Width + new Vector2(x / 3f).RotatedBy(x), c0, new Vector3(Value0, 1, 0)));

				vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 + Width + new Vector2(x / 3f).RotatedBy(x), c0, new Vector3(0, 0, 0)));
				vertex2Ds.Add(new Vertex2D(EndPos + Width + new Vector2(x / 3f).RotatedBy(x), c0, new Vector3(Value1, 0, 0)));
				vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 - Width + new Vector2(x / 3f).RotatedBy(x), c0, new Vector3(0, 1, 0)));

				vertex2Ds.Add(new Vertex2D(EndPos + Width + new Vector2(x / 3f).RotatedBy(x), c0, new Vector3(Value1, 0, 0)));
				vertex2Ds.Add(new Vertex2D(EndPos - Width + new Vector2(x / 3f).RotatedBy(x), c0, new Vector3(Value1, 1, 0)));
				vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 - Width + new Vector2(x / 3f).RotatedBy(x), c0, new Vector3(0, 1, 0)));

				continue;
			}
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

	public void DrawTexLineColor(VFXBatch spriteBatch, Vector2 StartPos, Vector2 EndPos, Color color1, Color color2, float Wid, Texture2D tex)
	{
		Vector2 Width = Vector2.Normalize(StartPos - EndPos).RotatedBy(Math.PI / 2d) * Wid;

		var vertex2Ds = new List<Vertex2D>();

		for (int x = 0; x < 3; x++)
		{
			float Value0 = (float)(Main.timeForVisualEffects / 91d + 20) % 1f;
			float Value1 = (float)(Main.timeForVisualEffects / 91d + 20.1) % 1f;

			if (Value1 < Value0)
			{
				float D0 = 1 - Value0;
				Vector2 Delta = EndPos - StartPos;
				vertex2Ds.Add(new Vertex2D(StartPos + Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(Value0, 0, 0)));
				vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 + Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(1, 0, 0)));
				vertex2Ds.Add(new Vertex2D(StartPos - Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(Value0, 1, 0)));

				vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 + Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(1, 0, 0)));
				vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 - Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(1, 1, 0)));
				vertex2Ds.Add(new Vertex2D(StartPos - Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(Value0, 1, 0)));

				vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 + Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(0, 0, 0)));
				vertex2Ds.Add(new Vertex2D(EndPos + Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(Value1, 0, 0)));
				vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 - Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(0, 1, 0)));

				vertex2Ds.Add(new Vertex2D(EndPos + Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(Value1, 0, 0)));
				vertex2Ds.Add(new Vertex2D(EndPos - Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(Value1, 1, 0)));
				vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 - Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(0, 1, 0)));

				continue;
			}

			vertex2Ds.Add(new Vertex2D(StartPos + Width + new Vector2(x / 3f).RotatedBy(x) - Main.screenPosition, color1, new Vector3(Value0, 0, 0)));
			vertex2Ds.Add(new Vertex2D(EndPos + Width + new Vector2(x / 3f).RotatedBy(x) - Main.screenPosition, color2, new Vector3(Value1, 0, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos - Width + new Vector2(x / 3f).RotatedBy(x) - Main.screenPosition, color1, new Vector3(Value0, 1, 0)));

			vertex2Ds.Add(new Vertex2D(EndPos + Width + new Vector2(x / 3f).RotatedBy(x) - Main.screenPosition, color2, new Vector3(Value1, 0, 0)));
			vertex2Ds.Add(new Vertex2D(EndPos - Width + new Vector2(x / 3f).RotatedBy(x) - Main.screenPosition, color2, new Vector3(Value1, 1, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos - Width + new Vector2(x / 3f).RotatedBy(x) - Main.screenPosition, color1, new Vector3(Value0, 1, 0)));
		}

		spriteBatch.Draw(tex, vertex2Ds, PrimitiveType.TriangleList);
	}

	public void DrawTexLineColor(Vector2 StartPos, Vector2 EndPos, Color color1, Color color2, float Wid, Texture2D tex)
	{
		Vector2 Width = Vector2.Normalize(StartPos - EndPos).RotatedBy(Math.PI / 2d) * Wid;

		var vertex2Ds = new List<Vertex2D>();

		for (int x = 0; x < 3; x++)
		{
			float Value0 = (float)(Main.timeForVisualEffects / 91d + 20) % 1f;
			float Value1 = (float)(Main.timeForVisualEffects / 91d + 20.1) % 1f;

			if (Value1 < Value0)
			{
				float D0 = 1 - Value0;
				Vector2 Delta = EndPos - StartPos;
				vertex2Ds.Add(new Vertex2D(StartPos + Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(Value0, 0, 0)));
				vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 + Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(1, 0, 0)));
				vertex2Ds.Add(new Vertex2D(StartPos - Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(Value0, 1, 0)));

				vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 + Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(1, 0, 0)));
				vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 - Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(1, 1, 0)));
				vertex2Ds.Add(new Vertex2D(StartPos - Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(Value0, 1, 0)));

				vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 + Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(0, 0, 0)));
				vertex2Ds.Add(new Vertex2D(EndPos + Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(Value1, 0, 0)));
				vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 - Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(0, 1, 0)));

				vertex2Ds.Add(new Vertex2D(EndPos + Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(Value1, 0, 0)));
				vertex2Ds.Add(new Vertex2D(EndPos - Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(Value1, 1, 0)));
				vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 - Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(0, 1, 0)));

				continue;
			}

			vertex2Ds.Add(new Vertex2D(StartPos + Width + new Vector2(x / 3f).RotatedBy(x) - Main.screenPosition, color1, new Vector3(Value0, 0, 0)));
			vertex2Ds.Add(new Vertex2D(EndPos + Width + new Vector2(x / 3f).RotatedBy(x) - Main.screenPosition, color2, new Vector3(Value1, 0, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos - Width + new Vector2(x / 3f).RotatedBy(x) - Main.screenPosition, color1, new Vector3(Value0, 1, 0)));

			vertex2Ds.Add(new Vertex2D(EndPos + Width + new Vector2(x / 3f).RotatedBy(x) - Main.screenPosition, color2, new Vector3(Value1, 0, 0)));
			vertex2Ds.Add(new Vertex2D(EndPos - Width + new Vector2(x / 3f).RotatedBy(x) - Main.screenPosition, color2, new Vector3(Value1, 1, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos - Width + new Vector2(x / 3f).RotatedBy(x) - Main.screenPosition, color1, new Vector3(Value0, 1, 0)));
		}

		Main.graphics.GraphicsDevice.Textures[0] = tex;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertex2Ds.ToArray(), 0, vertex2Ds.Count / 3);
	}

	public override void OnKill(int timeLeft)
	{
		SoundEngine.PlaySound(SoundID.NPCHit2, Projectile.Center);
		for (int f = 0; f < 20; f++)
		{
			Vector2 v0 = new Vector2(Main.rand.NextFloat(0f, 8f), 0).RotatedByRandom(6.283);
			var dust0 = Dust.NewDustDirect(Projectile.Center + Vector2.Normalize(Projectile.velocity) * 75, 0, 0, DustID.Torch, v0.X, v0.Y, 100, default, Main.rand.NextFloat(0.6f, 1.8f));
			dust0.velocity = v0 + Projectile.velocity * 0.2f;
			dust0.noGravity = true;
		}
		for (int f = 0; f < 20; f++)
		{
			Vector2 v0 = new Vector2(Main.rand.NextFloat(0f, 8f), 0).RotatedByRandom(6.283);
			var dust0 = Dust.NewDustDirect(Projectile.Center + Vector2.Normalize(Projectile.velocity) * 75, 0, 0, DustID.Bone, v0.X, v0.Y, 100, default, Main.rand.NextFloat(0.6f, 1.8f));
			dust0.velocity = v0 + Projectile.velocity * 0.2f;
			dust0.noGravity = true;
		}
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		Texture2D Power = Commons.ModAsset.Trail_5.Value;
		float Pdark = 0f;
		float Pwidth = 1f;
		if (Projectile.timeLeft > 1780)
		{
			Pwidth = (1800 - Projectile.timeLeft) / 20f;
			Pdark = (1800 - Projectile.timeLeft) / 20f;
		}
		else if (Projectile.timeLeft > 1750)
		{
			Pdark = (Projectile.timeLeft - 1750) / 30f;
		}
		var c1 = new Color(0.25f * Pdark, 0.011f * Pdark * Pdark, 0f, 0f);
		DrawTexLineColor(spriteBatch, Projectile.Center + new Vector2(0, -70).RotatedBy(Projectile.rotation), Projectile.Center, Color.Transparent, c1, 14f * Pwidth, Power);
		DrawTexLineColor(spriteBatch, Projectile.Center, Projectile.Center + new Vector2(0, 70).RotatedBy(Projectile.rotation), c1, Color.Transparent, 14f * Pwidth, Power);
		var c2 = new Color(Projectile.rotation / 6.283f, 0.03f, 0f, 0f);
		if (shot)
		{
			DrawTexLineColor(spriteBatch, Projectile.Center + new Vector2(0, -350).RotatedBy(Projectile.rotation), Projectile.Center, Color.Transparent, c2, 14f * Pwidth, Power);
			DrawTexLineColor(spriteBatch, Projectile.Center, Projectile.Center + new Vector2(0, 30).RotatedBy(Projectile.rotation), c2, Color.Transparent, 14f * Pwidth, Power);
		}
	}
}
