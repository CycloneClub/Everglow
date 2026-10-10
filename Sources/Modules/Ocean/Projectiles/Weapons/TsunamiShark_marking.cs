using Everglow.Commons.MEAC;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Ocean.Projectiles.Weapons;

public class TsunamiShark_marking : ModProjectile, IWarpProjectile
{
	public override string Texture => "Everglow/Ocean/Projectiles/Weapons/TsunamiShark/TsunamiShark_proj";

	public override void SetDefaults()
	{
		Projectile.width = 32;
		Projectile.height = 32;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 200;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 5;
	}

	internal List<NPC> PotentialTargets = new List<NPC>();
	internal Vector2 StartPos = Vector2.Zero;

	public override void OnSpawn(IEntitySource source)
	{
		Player player = Main.player[Projectile.owner];
		Vector2 toProj = Projectile.Center - player.MountedCenter;
		toProj = Vector2.Normalize(toProj);
		Projectile.rotation = (float)(Math.Atan2(toProj.Y, toProj.X) + Math.PI * 0.25);
		StartPos = player.MountedCenter + toProj * 80;

		SoundEngine.PlaySound(SoundID.ResearchComplete.WithVolume(0.2f), player.MountedCenter);
	}

	public override void AI()
	{
		float value = (200 - Projectile.timeLeft) / 200f;
		value = MathF.Sqrt(value) * 320f;
		foreach (NPC npc in Main.npc)
		{
			if (npc.active)
			{
				if (!npc.friendly && npc.life > 0 && npc.CanBeChasedBy() && !npc.dontTakeDamage)
				{
					if (npc.Distance(Main.MouseWorld) <= value)
					{
						if (!PotentialTargets.Contains(npc))
						{
							SoundEngine.PlaySound(25, npc.Center);
							PotentialTargets.Add(npc);
						}
					}
				}
			}
		}
		float valueLight = Projectile.timeLeft / 600f;
		for (int x = 0; x < value / 10f; x++)
		{
			Vector2 radious = new Vector2(0, value * 1.6f).RotatedBy(x * 10f / value * 2f * Math.PI);
			Lighting.AddLight(Projectile.Center + radious, 0, valueLight * valueLight, valueLight);
		}
	}

	public override void OnKill(int timeLeft)
	{
		if (PotentialTargets.Count > 0)
		{
			float maxWeight = -200;
			NPC target = null;
			foreach (NPC npc in PotentialTargets)
			{
				float weight = 200 - npc.Distance(Main.MouseWorld);
				if (IsRectangleInsertAVector2(npc.Hitbox, Main.MouseWorld))
				{
					weight += 100;
					weight += npc.life / (float)npc.lifeMax * 100;
					weight += npc.life * 0.05f;
					weight -= npc.defense;
				}
				if (npc.boss)
				{
					weight += 250;
				}
				if (weight > maxWeight)
				{
					maxWeight = weight;
					target = npc;
				}
			}
			if (target != null)
			{
				Player player = Main.player[Projectile.owner];
				var tsunamiS = player.HeldItem.ModItem as Items.Weapons.TsunamiShark;
				if (tsunamiS != null)
				{
					tsunamiS.MarkedTarget = target;
				}
			}
		}
		base.OnKill(timeLeft);
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		float value = (200 - Projectile.timeLeft) / 200f;
		value = MathF.Sqrt(value);
		float colorV = 0.9f * (1 - value);
		Texture2D t = ModAsset.HiveCyberNoise.Value;
		float width = 120;
		if (Projectile.timeLeft < 120)
		{
			width = Projectile.timeLeft;
		}

		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.Default, SamplerState.PointWrap, RasterizerState.CullNone);
		DrawTexRing_VFXBatch(Ins.Batch, value * 400, width, new Color(0, colorV * colorV * 1.6f, colorV * 12f, 0f), Projectile.Center - Main.screenPosition, t, Projectile.ai[1], (float)Main.time * 0.01f);
		Ins.Batch.End();

		return false;
	}

	public override void PostDraw(Player player, Color lightColor)
	{
		// 绘制特效
		player.heldProj = Projectile.whoAmI;
		Vector2 toMouse = Projectile.Center - player.MountedCenter;
		if (player.controlUseItem)
		{
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (float)(Math.Atan2(toMouse.Y, toMouse.X) - Math.PI / 2d));
		}

		float value = (200 - Projectile.timeLeft) / 200f;
		value = MathF.Sqrt(value);
		float colorV = 0.9f * (1 - value);
		Color color = new Color(0, colorV * colorV * 1.6f, colorV * 12f, 0f);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		DrawTexLine(StartPos, Projectile.Center, color, Projectile.timeLeft / 40f, ModAsset.Lightline.Value);
		Texture2D ball = ModAsset.LightBall.Value;
		Main.spriteBatch.Draw(ball, Projectile.Center - Main.screenPosition, null, color, 0, ball.Size() * 0.5f, Projectile.timeLeft / 120f, SpriteEffects.None, 0);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		// 绘制枪的部分
		if (Projectile.timeLeft > 150)
		{
			Texture2D texMain = ModAsset.TsunamiShark_proj.Value;
			SpriteEffects se = SpriteEffects.None;
			Vector2 origin = new Vector2(texMain.Size().X * 0.3f, texMain.Size().Y * 0.6f);
			if (Projectile.Center.X < player.Center.X)
			{
				se = SpriteEffects.FlipVertically;
				player.direction = -1;
			}
			else
			{
				origin = new Vector2(texMain.Size().X * 0.3f, texMain.Size().Y * 0.4f);
				player.direction = 1;
			}
			Vector2 random = new Vector2(0, Main.rand.NextFloat(1)).RotatedByRandom(6.283);
			var offset = new Vector2(0, -5);

			Main.spriteBatch.Draw(texMain, player.MountedCenter - Main.screenPosition + offset - random, null, lightColor, Projectile.rotation - (float)(Math.PI * 0.25), origin, 1f, se, 0);
		}

		// 绘制标记
		Texture2D texMark = ModAsset.TsunamiShark_mark.Value;
		foreach (NPC npc in PotentialTargets)
		{
			float valueNPC = (200 - Projectile.timeLeft) / 200f;
			valueNPC = MathF.Sqrt(valueNPC) * 320f;
			float colorValue = Math.Clamp((50 - Math.Abs(npc.Distance(Projectile.Center) - valueNPC)) / 50f, 0.2f, 1);

			Main.spriteBatch.Draw(texMark, npc.Center - Main.screenPosition, new Rectangle(0, 0, texMark.Width, texMark.Height), new Color(0, colorValue * colorValue, colorValue, 0), 0, texMark.Size() / 2f, 1f, SpriteEffects.None, 0);
		}
	}

	private bool IsRectangleInsertAVector2(Rectangle rectangle, Vector2 vector2)
	{
		return vector2.X > rectangle.Left && vector2.X < rectangle.Right && vector2.Y > rectangle.Top && vector2.Y < rectangle.Bottom;
	}

	public void DrawTexLine(Vector2 StartPos, Vector2 EndPos, Color c0, float Wid, Texture2D tex)
	{
		Vector2 Width = Vector2.Normalize(StartPos - EndPos).RotatedBy(Math.PI / 2d) * Wid;

		var vertex2Ds = new List<Vertex2D>();

		for (int x = 0; x < 3; x++)
		{
			vertex2Ds.Add(new Vertex2D(StartPos + Width + new Vector2(x / 3f).RotatedBy(x) - Main.screenPosition, c0, new Vector3(0, 0, 0)));
			vertex2Ds.Add(new Vertex2D(EndPos + Width + new Vector2(x / 3f).RotatedBy(x) - Main.screenPosition, c0, new Vector3(1, 0, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos - Width + new Vector2(x / 3f).RotatedBy(x) - Main.screenPosition, c0, new Vector3(0, 1, 0)));

			vertex2Ds.Add(new Vertex2D(EndPos + Width + new Vector2(x / 3f).RotatedBy(x) - Main.screenPosition, c0, new Vector3(1, 0, 0)));
			vertex2Ds.Add(new Vertex2D(EndPos - Width + new Vector2(x / 3f).RotatedBy(x) - Main.screenPosition, c0, new Vector3(1, 1, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos - Width + new Vector2(x / 3f).RotatedBy(x) - Main.screenPosition, c0, new Vector3(0, 1, 0)));
		}

		Main.graphics.GraphicsDevice.Textures[0] = tex;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertex2Ds.ToArray(), 0, vertex2Ds.Count / 3);
	}

	private static void DrawWarpTexCircle_VFXBatch(VFXBatch spriteBatch, float radious, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();

		for (int h = 0; h < radious / 2; h += 1)
		{
			float colorR = (h / radious * MathF.PI * 4 + (float)addRot + 1.57f) % (MathF.PI * 2f) / (MathF.PI * 2f);
			float color2R = ((h + 1) / radious * MathF.PI * 4 + (float)addRot + 1.57f) % (MathF.PI * 2f) / (MathF.PI * 2f);

			color = new Color(colorR, color.G / 255f, 0, 0);
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy(h / radious * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radious, 0.8f, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy(h / radious * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radious, 0.2f, 0)));
			if (Math.Abs(color2R - colorR) > 0.8f)
			{
				float midValue = (1f - colorR) / (float)(color2R + (1f - colorR));
				color.R = 255;
				circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy((h + midValue) / radious * Math.PI * 4 + addRot), color, new Vector3((h + midValue) * 2 / radious, 0.8f, 0)));
				circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy((h + midValue) / radious * Math.PI * 4 + addRot), color, new Vector3((h + midValue) * 2 / radious, 0.2f, 0)));
				color.R = 0;
				circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy((h + midValue) / radious * Math.PI * 4 + addRot), color, new Vector3((h + midValue) * 2 / radious, 0.8f, 0)));
				circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy((h + midValue) / radious * Math.PI * 4 + addRot), color, new Vector3((h + midValue) * 2 / radious, 0.2f, 0)));
			}
		}
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy(addRot), color, new Vector3(1, 0.8f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy(addRot), color, new Vector3(1, 0.2f, 0)));
		if (circle.Count > 2)
		{
			spriteBatch.Draw(tex, circle, PrimitiveType.TriangleStrip);
		}
	}

	private static void DrawTexRing_VFXBatch(VFXBatch spriteBatch, float radious, float width, Color color, Vector2 center, Texture2D tex, double rotation, float process)
	{
		var ring = new List<Vertex2D>();
		int precision = 60;
		for (int h = 0; h <= precision; h++)
		{
			float ratioOfInEx = 1f;
			float internalHalfWidth = width * ratioOfInEx;
			Vector2 radialVector = new Vector2(0, Math.Max(radious, 0)).RotatedBy(h / (float)precision * Math.PI * 2 + rotation);
			Vector2 radialHalfWidth = new Vector2(0, Math.Max(radious - internalHalfWidth, 0)).RotatedBy(h / (float)precision * Math.PI * 2 + rotation);
			float xProcession = h / (float)precision;
			float coordYProcession = (radious - internalHalfWidth) / radious;
			if (coordYProcession > 0)
			{
				coordYProcession = 0;
			}
			ring.Add(new Vertex2D(center + radialVector, color, new Vector3(xProcession, 0.9f + process, 0)));
			ring.Add(new Vertex2D(center + radialHalfWidth, Color.Transparent, new Vector3(xProcession, 0.6f - coordYProcession * 0.3f + process, 0)));
		}
		if (ring.Count > 2)
		{
			spriteBatch.Draw(tex, ring, PrimitiveType.TriangleStrip);
		}
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		float value = (200 - Projectile.timeLeft) / 200f;
		value = MathF.Sqrt(value);
		float colorV = 0.9f * (1 - value);
		Texture2D t = ModAsset.HiveCyberNoiseThicker.Value;
		float width = 120;
		if (Projectile.timeLeft < 120)
		{
			width = Projectile.timeLeft;
		}

		DrawWarpTexCircle_VFXBatch(spriteBatch, value * 400, width, new Color(colorV, colorV * 0.07f, colorV, 0f), Projectile.Center - Main.screenPosition, t);
	}
}
