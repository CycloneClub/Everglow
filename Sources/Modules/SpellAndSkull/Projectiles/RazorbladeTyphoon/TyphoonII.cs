using Everglow.Commons.MEAC;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Terraria.DataStructures;

namespace Everglow.SpellAndSkull.Projectiles.RazorbladeTyphoon;

public class TyphoonII : ModProjectile, IWarpProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 68;
		Projectile.height = 68;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.ignoreWater = false;
		Projectile.tileCollide = true;
		Projectile.extraUpdates = 3;
		Projectile.timeLeft = 1500;
		Projectile.alpha = 0;
		Projectile.penetrate = 1080;
		Projectile.scale = 1f;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 30;
		Projectile.DamageType = DamageClass.Magic;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 120;
	}

	internal int Aimnpc = -1;

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.position.Y -= 15f;
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		Lighting.AddLight((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16), 0, 0.46f * Projectile.scale, 0.54f * Projectile.scale);
		if (Projectile.timeLeft > 500)
		{
			Projectile.scale = 1.2f + (float)Math.Sin(Main.timeForVisualEffects / 1.8f + Projectile.ai[0]) * 0.15f;
		}
		else
		{
			Projectile.velocity *= 0.996f;
			Projectile.scale = (1.2f + (float)Math.Sin(Main.timeForVisualEffects / 1.8f + Projectile.ai[0]) * 0.15f) * Projectile.timeLeft / 500f;

			Projectile.knockBack *= 0.9f;
			if (Projectile.timeLeft % 10 == 1)
			{
				if (Projectile.damage > 0)
				{
					Projectile.damage--;
				}
				else
				{
					Projectile.damage = 0;
					Projectile.friendly = false;
				}
			}
		}
		Projectile.rotation += 0.2f;

		for (int i = 1; i < 4; ++i)
		{
			var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.RazorbladeTyphoon, Projectile.velocity.X * 2, Projectile.velocity.Y * 2, 100, default, Main.rand.NextFloat(0.85f, 1.85f) * Projectile.scale);
			d.noGravity = true;
		}

		if (Aimnpc != -1)
		{
			NPC npc = Main.npc[Aimnpc];
			if (npc.active)
			{
				Vector2 v0 = npc.Center - Projectile.Center;
				Projectile.velocity += Vector2.Normalize(v0) * 0.5f;

				if (player.HeldItem.type == ItemID.RazorbladeTyphoon)
				{
					Projectile.velocity = Vector2.Normalize(Projectile.velocity) * player.HeldItem.shootSpeed;
				}
				else
				{
					Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 6f;
				}
			}
		}
		if (Main.rand.NextBool(2))
		{
			float MinDis = 250;
			foreach (var target in Main.npc)
			{
				if (target.active && Main.rand.NextBool(2))
				{
					if (!target.dontTakeDamage && !target.friendly && target.CanBeChasedBy())
					{
						if (target.velocity.Length() <= 0.001f)
						{
							continue;
						}

						Vector2 ToTarget = target.Center - Projectile.Center;
						float dis = ToTarget.Length();
						if (dis < 250 && ToTarget != Vector2.Zero)
						{
							float mess = target.width * target.height;
							mess = (float)Math.Sqrt(mess);
							Vector2 Addvel = Vector2.Normalize(ToTarget) / mess / (dis + 10) * 10000f * target.knockBackResist * Projectile.knockBack;
							if (!target.noGravity)
							{
								Addvel.Y *= 3f;
							}

							target.velocity -= Addvel;
							if (target.velocity.Length() > 10)
							{
								target.velocity *= 10 / target.velocity.Length();
							}

							if (dis < MinDis)
							{
								MinDis = dis;
								Aimnpc = target.whoAmI;
							}
						}
					}
				}
			}
			foreach (var target in Main.item)
			{
				if (target.active && Main.rand.NextBool(2))
				{
					Vector2 ToTarget = target.Center - Projectile.Center;
					float dis = ToTarget.Length();
					if (dis < 250 && ToTarget != Vector2.Zero)
					{
						if (dis < 45)
						{
							if ((target.type >= ItemID.CopperCoin && target.type <= ItemID.PlatinumCoin) || target.type == ItemID.Star || target.type == ItemID.Heart)
							{
								target.position = player.Center;
							}
						}
						float mess = target.width * target.height;
						mess = (float)Math.Sqrt(mess);
						Vector2 Addvel = Vector2.Normalize(ToTarget) / mess / (dis + 10) * 5000f * Projectile.knockBack;
						target.velocity -= Addvel;
						if (target.velocity.Length() > 10)
						{
							target.velocity *= 10 / target.velocity.Length();
						}
					}
				}
			}
			foreach (var target in Main.gore)
			{
				if (target.active && Main.rand.NextBool(2))
				{
					Vector2 ToTarget = target.position - Projectile.Center;
					float dis = ToTarget.Length();
					if (dis < 250 && ToTarget != Vector2.Zero)
					{
						float mess = target.Width * target.Height;
						mess = (float)Math.Sqrt(mess);
						Vector2 Addvel = Vector2.Normalize(ToTarget) / mess / (dis + 10) * 10000f * Projectile.knockBack;
						target.velocity -= Addvel;
						if (target.velocity.Length() > 10)
						{
							target.velocity *= 10 / target.velocity.Length();
						}

						target.timeLeft -= 24;
					}
				}
			}
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Texture2D Light = ModAsset.TyphoonII.Value;
		Texture2D Shade = ModAsset.TyphoonIIShade.Value;
		float k0 = 1f;
		var c0 = new Color(0, k0 * k0 * 0.3f + 0.6f, k0 * k0 * 0.1f + 0.9f, 0);

		float width = 96;
		float MulByTimeLeft = 1f;
		if (Projectile.timeLeft < 500)
		{
			MulByTimeLeft = Projectile.timeLeft / 500f;
		}

		width *= MulByTimeLeft;
		c0 *= MulByTimeLeft;
		int TrueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			TrueL++;
		}

		DrawFlameTrail(TrueL, width, true, Color.White);

		var Frame = new Rectangle(0, (int)(Main.timeForVisualEffects / 10f % 3) * 68, 68, 68);
		Main.spriteBatch.Draw(Shade, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Frame, Color.White * MulByTimeLeft, Projectile.rotation, Frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(Shade, Projectile.oldPos[4] + new Vector2(34f) - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Frame, Color.White * MulByTimeLeft * 0.85f, Projectile.rotation, Frame.Size() / 2f, Projectile.scale * 0.9f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(Shade, Projectile.oldPos[8] + new Vector2(34f) - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Frame, Color.White * MulByTimeLeft * 0.75f, Projectile.rotation, Frame.Size() / 2f, Projectile.scale * 0.85f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(Shade, Projectile.oldPos[12] + new Vector2(34f) - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Frame, Color.White * MulByTimeLeft * 0.5f, Projectile.rotation, Frame.Size() / 2f, Projectile.scale * 0.8f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(Shade, Projectile.oldPos[16] + new Vector2(34f) - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Frame, Color.White * MulByTimeLeft * 0.25f, Projectile.rotation, Frame.Size() / 2f, Projectile.scale * 0.75f, SpriteEffects.None, 0);

		DrawFlameTrail(TrueL, width, false, c0);

		Main.spriteBatch.Draw(Light, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Frame, c0, Projectile.rotation, Frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(Light, Projectile.oldPos[4] + new Vector2(34f) - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Frame, c0 * 0.85f, Projectile.rotation, Frame.Size() / 2f, Projectile.scale * 0.9f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(Light, Projectile.oldPos[8] + new Vector2(34f) - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Frame, c0 * 0.75f, Projectile.rotation, Frame.Size() / 2f, Projectile.scale * 0.85f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(Light, Projectile.oldPos[12] + new Vector2(34f) - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Frame, c0 * 0.5f, Projectile.rotation, Frame.Size() / 2f, Projectile.scale * 0.8f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(Light, Projectile.oldPos[16] + new Vector2(34f) - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), Frame, c0 * 0.25f, Projectile.rotation, Frame.Size() / 2f, Projectile.scale * 0.75f, SpriteEffects.None, 0);

		return false;
	}

	private void DrawFlameTrail(int TrueL, float width, bool Shade = false, Color c0 = default(Color), float Mulfactor = 1.6f)
	{
		var bars = new List<Vertex2D>();
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			float mulColor = 1f;
			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
			if (i == 1)
			{
				mulColor = 0f;
			}

			if (i >= 2)
			{
				var normalDirII = Projectile.oldPos[i - 2] - Projectile.oldPos[i - 1];
				normalDirII = Vector2.Normalize(new Vector2(-normalDirII.Y, normalDirII.X));
				if (Vector2.Dot(normalDirII, normalDir) <= 0.965f)
				{
					mulColor = 0f;
				}
			}
			if (i < Projectile.oldPos.Length - 1)
			{
				var normalDirII = Projectile.oldPos[i] - Projectile.oldPos[i + 1];
				normalDirII = Vector2.Normalize(new Vector2(-normalDirII.Y, normalDirII.X));
				if (Vector2.Dot(normalDirII, normalDir) <= 0.965f)
				{
					mulColor = 0f;
				}
			}
			var factor = i / (float)TrueL;
			float x0 = factor * Mulfactor - (float)(Main.timeForVisualEffects / 15d) + 100000;
			x0 %= 1f;
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factor) + new Vector2(34f) - Main.screenPosition, c0 * mulColor, new Vector3(x0, 1, 0)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factor) + new Vector2(34f) - Main.screenPosition, c0 * mulColor, new Vector3(x0, 0, 0)));
			var factorII = factor;
			factor = (i + 1) / (float)TrueL;
			var x1 = factor * Mulfactor - (float)(Main.timeForVisualEffects / 15d) + 100000;
			x1 %= 1f;
			if (x0 > x1)
			{
				float DeltaValue = 1 - x0;
				var factorIII = factorII * x0 + factor * DeltaValue;
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factorIII) + new Vector2(34f) - Main.screenPosition, c0 * mulColor, new Vector3(1, 1, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factorIII) + new Vector2(34f) - Main.screenPosition, c0 * mulColor, new Vector3(1, 0, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factorIII) + new Vector2(34f) - Main.screenPosition, c0 * mulColor, new Vector3(0, 1, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factorIII) + new Vector2(34f) - Main.screenPosition, c0 * mulColor, new Vector3(0, 0, 0)));
			}
		}
		Texture2D t = Commons.ModAsset.Trail_2_thick.Value;
		if (Shade)
		{
			t = ModAsset.FogTraceShade.Value;
		}

		Main.graphics.GraphicsDevice.Textures[0] = t;

		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		float width = 96;
		float MulByTimeLeft = 1f;
		if (Projectile.timeLeft < 500)
		{
			MulByTimeLeft = Projectile.timeLeft / 500f;
		}

		width *= MulByTimeLeft;
		int TrueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			TrueL++;
		}
		var bars = new List<Vertex2D>();
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			float mulColor = 1f;
			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
			if (i == 1)
			{
				mulColor = 0f;
			}

			if (i >= 2)
			{
				var normalDirII = Projectile.oldPos[i - 2] - Projectile.oldPos[i - 1];
				normalDirII = Vector2.Normalize(new Vector2(-normalDirII.Y, normalDirII.X));
				if (Vector2.Dot(normalDirII, normalDir) <= 0.965f)
				{
					mulColor = 0f;
				}
			}
			if (i < Projectile.oldPos.Length - 1)
			{
				var normalDirII = Projectile.oldPos[i] - Projectile.oldPos[i + 1];
				normalDirII = Vector2.Normalize(new Vector2(-normalDirII.Y, normalDirII.X));
				if (Vector2.Dot(normalDirII, normalDir) <= 0.965f)
				{
					mulColor = 0f;
				}
			}

			float k0 = (float)Math.Atan2(normalDir.Y, normalDir.X);
			k0 += 3.14f + 1.57f;
			if (k0 > 6.28f)
			{
				k0 -= 6.28f;
			}

			Color c0 = new Color(k0, 0.04f * mulColor, 0, 0) * MulByTimeLeft;

			var factor = i / (float)TrueL;
			float x0 = factor * 1.3f - (float)(Main.timeForVisualEffects / 15d) + 100000;
			x0 %= 1f;
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factor) + new Vector2(34f) - Main.screenPosition, c0, new Vector3(x0, 1, 0)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factor) + new Vector2(34f) - Main.screenPosition, c0, new Vector3(x0, 0, 0)));
			var factorII = factor;
			factor = (i + 1) / (float)TrueL;
			var x1 = factor * 1.3f - (float)(Main.timeForVisualEffects / 15d) + 100000;
			x1 %= 1f;
			if (x0 > x1)
			{
				float DeltaValue = 1 - x0;
				var factorIII = factorII * x0 + factor * DeltaValue;
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factorIII) + new Vector2(34f) - Main.screenPosition, c0, new Vector3(1, 1, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factorIII) + new Vector2(34f) - Main.screenPosition, c0, new Vector3(1, 0, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factorIII) + new Vector2(34f) - Main.screenPosition, c0, new Vector3(0, 1, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factorIII) + new Vector2(34f) - Main.screenPosition, c0, new Vector3(0, 0, 0)));
			}
		}
		Texture2D t = ModAsset.FogTraceLight.Value;

		if (bars.Count > 3)
		{
			spriteBatch.Draw(t, bars, PrimitiveType.TriangleStrip);
		}
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		if (Projectile.velocity.X != oldVelocity.X)
		{
			Projectile.velocity.X = -oldVelocity.X;
		}

		if (Projectile.velocity.Y != oldVelocity.Y)
		{
			Projectile.velocity.Y = -oldVelocity.Y;
		}

		Projectile.penetrate--;
		if (Projectile.penetrate < 0)
		{
			Projectile.Kill();
		}

		return false;
	}
}
