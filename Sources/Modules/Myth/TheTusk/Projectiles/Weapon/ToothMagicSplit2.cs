using Everglow.Myth.TheTusk;
using Terraria;
using Terraria.Localization;

namespace Everglow.Myth.TheTusk.Projectiles.Weapon;

public class ToothMagicSplit2 : ModProjectile
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Tooth Magic Ball");
	}
	public override void SetDefaults()
	{
		Projectile.width = 18;
		Projectile.height = 18;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.ignoreWater = true;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 120;
		Projectile.alpha = 0;
		Projectile.penetrate = 3;
		Projectile.scale = 1;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 45;
	}
	private int addi = 0;
	private int MaxAdd = -1;
	public override void AI()
	{
		Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
		/*int num91 = Dust.NewDust(new Vector2(Projectile.Center.X, Projectile.Center.Y) - new Vector2(12, 12) - Projectile.velocity, 16, 16, DustID.Blood, 0f, 0f, 100, default, Main.rand.NextFloat(1f, 2.6f));
            Main.dust[num91].noGravity = true;
            Main.dust[num91].velocity *= 0.5f;
            int num90 = Dust.NewDust(new Vector2(Projectile.Center.X, Projectile.Center.Y) - new Vector2(12, 12) - Projectile.velocity, 16, 16, 183, 0f, 0f, 100, default, Main.rand.NextFloat(1f, 2.6f));
            Main.dust[num90].noGravity = true;
            Main.dust[num90].velocity *= 0.5f;*/
		addi++;
		if (Tokill < 0)
		{
			float num2 = Projectile.Center.X;
			float num3 = Projectile.Center.Y;
			float num4 = 400f;
			bool flag = false;
			for (int j = 0; j < 200; j++)
			{
				if (Main.npc[j].CanBeChasedBy(Projectile, false) && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[j].Center, 1, 1))
				{
					float num5 = Main.npc[j].position.X + Main.npc[j].width / 2;
					float num6 = Main.npc[j].position.Y + Main.npc[j].height / 2;
					float num7 = Math.Abs(Projectile.position.X + Projectile.width / 2 - num5) + Math.Abs(Projectile.position.Y + Projectile.height / 2 - num6);
					if (num7 < num4)
					{
						num4 = num7;
						num2 = num5;
						num3 = num6;
						flag = true;
					}
				}
			}
			if (flag)
			{
				float num8 = 20f;
				var vector1 = new Vector2(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f);
				float num9 = num2 - vector1.X;
				float num10 = num3 - vector1.Y;
				float num11 = (float)Math.Sqrt((double)(num9 * num9 + num10 * num10));
				num11 = num8 / num11;
				num9 *= num11;
				num10 *= num11;
				Projectile.velocity.X = (Projectile.velocity.X * 20f + num9) / 21f;
				Projectile.velocity.Y = (Projectile.velocity.Y * 20f + num10) / 21f;
			}
		}
		if (Tokill >= 0 && Tokill <= 2)
		{
			Projectile.Kill();
		}

		if (Tokill > 0)
		{
			Tokill--;
		}

		if (Tokill <= 44 && Tokill > 0)
		{
			Projectile.position = Projectile.oldPosition;
			Projectile.velocity = Projectile.oldVelocity;
		}
		/*int r2 = Dust.NewDust(new Vector2(Projectile.Center.X, Projectile.Center.Y) - new Vector2(4, 4) + Projectile.velocity / Projectile.velocity.Length() * 12f, 0, 0, DustID.Blood, 0, 0, 0, default,3f);
            Main.dust[r2].noGravity = true;
            int r = Dust.NewDust(new Vector2(Projectile.Center.X, Projectile.Center.Y) - new Vector2(4, 4) + Projectile.velocity / Projectile.velocity.Length() * 12f, 0, 0, 183, 0, 0, 0, default, 4f);
            Main.dust[r].noGravity = true;*/
		if (MaxAdd == -1)
		{
			MaxAdd = Main.rand.Next(12, 27);
		}

		if (FirstVel == Vector2.Zero)
		{
			FirstVel = Vector2.Normalize(Projectile.velocity).RotatedBy(Main.rand.NextFloat(-1.5f, 1.5f)) * 0.9f;
		}

		if (addi < MaxAdd)
			Projectile.velocity += (float)(1 - Math.Cos(addi / 7.5d * Math.PI)) * FirstVel;
		else
		{
			if (Tokill < 0)
			{
				Projectile.tileCollide = true;
				Projectile.friendly = true;
			}
		}
		if (Projectile.damage <= 0 && Tokill <= 0)
		{
			Projectile.Kill();
		}

		if (Projectile.velocity.Length() > 7)
			Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.velocity.Length() / 30f * (Projectile.whoAmI % 2 - 0.5f)) * 0.96f;
		else
		{
			Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.velocity.Length() / 100f * (Projectile.whoAmI % 2 - 0.5f));
		}
	}
	private int Tokill = -1;
	private float wid = -1;
	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		Projectile.velocity = Projectile.oldVelocity;
		Tokill = 45; // 0.75s后消掉
		Projectile.friendly = false;
		Projectile.damage = 0;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.aiStyle = -1;
		return false;
	}
	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		Projectile.velocity = Projectile.oldVelocity;
		Tokill = 45; // 0.75s后消掉
		Projectile.friendly = false;
		Projectile.damage = 0;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.aiStyle = -1;
	}
	private Vector2 FirstVel = Vector2.Zero;
	private int TrueL = 1;
	private Vector2 ovel = Vector2.One;
	private float DelX = -1;
	private bool[] HasBeenHit = new bool[200];
	public override void PostDraw(Color lightColor)
	{
		if (DelX == -1)
		{
			DelX = Main.rand.NextFloat(1f, 40f);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		var bars = new List<Vertex2D>();
		float width = 2;
		if (Projectile.timeLeft < 45 && Tokill > 0)
		{
			width = Projectile.timeLeft / 22.5f;
		}

		TrueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			if (i == 1)
			{
				for (int j = 0; j < Projectile.oldPos.Length - 2; ++j)
				{
					if (Projectile.oldPos[i] == Projectile.oldPos[i - 1])
						i++;
					else
					{
						// i+=2;
						break;
					}
				}
			}
			TrueL++;
		}

		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			if (!Main.gamePaused)
			{
				if (addi == MaxAdd - 1)
				{
					for (int j = 0; j < 200; j++)
					{
						if (!HasBeenHit[j] && (Main.npc[j].Center - (Projectile.oldPos[i] + new Vector2(Projectile.width / 2f, Projectile.height / 2f))).Length() < 40 && !Main.npc[j].dontTakeDamage && !Main.npc[j].friendly)
						{
							HasBeenHit[j] = true;
							Player player = Main.player[Projectile.owner];
							NPC.HitModifiers npcHitM = new NPC.HitModifiers();
							NPC.HitInfo hit = npcHitM.ToHitInfo(Projectile.damage * Main.rand.NextFloat(0.85f, 1.15f), Main.rand.NextFloat(100f) < player.GetTotalCritChance(Projectile.DamageType), 2);
							Main.npc[j].StrikeNPC(hit, true, true);
							NetMessage.SendStrikeNPC(Main.npc[j], hit);
						}
					}
				}
			}
			if (i == 1)
			{
				for (int j = 0; j < Projectile.oldPos.Length - 2; ++j)
				{
					if (Projectile.oldPos[i] == Projectile.oldPos[i - 1])
						i++;
					else
					{
						// i+=2;
						break;
					}
				}
			}
			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));

			var factor = i / (float)TrueL;
			var w = MathHelper.Lerp(1f, 0.05f, factor);

			float CosWid = 1f; // 粗细
			if (TrueL - i < 25)
			{
				CosWid *= (float)(Math.Cos((25 - Math.Clamp(TrueL - i, 0, 25)) / 25d * Math.PI) + 1) / 2f;
			}

			if (wid == -1)
			{
				wid = Main.rand.NextFloat(1.0f, 2f);
			}

			float SinFx0 = 0; // 摆动函数
			float CosFx0 = 1 * CosWid * wid; // 求导简便计算透视投影
			if (Projectile.timeLeft < 30)
			{
				CosFx0 *= Projectile.timeLeft / 30f;
			}

			Vector2 P0 = Projectile.oldPos[i] + normalDir * SinFx0 + normalDir * width * CosFx0 + new Vector2(9, 9);
			Vector2 P1 = Projectile.oldPos[i] + normalDir * SinFx0 + normalDir * -width * CosFx0 + new Vector2(9, 9);
			Color c0 = Lighting.GetColor((int)(P0.X / 16f), (int)(P0.Y / 16f));
			Color c1 = Lighting.GetColor((int)(P1.X / 16f), (int)(P1.Y / 16f));
			bars.Add(new Vertex2D(P0 - Main.screenPosition, c0, new Vector3(factor, 1, w)));
			bars.Add(new Vertex2D(P1 - Main.screenPosition, c1, new Vector3(factor, 0, w)));
		}
		var Vx = new List<Vertex2D>();
		if (bars.Count > 2)
		{
			Vx.Add(bars[0]);
			if (Projectile.velocity.Length() > 0.05f)
			{
				ovel = Projectile.velocity;
			}

			Vector2 P2 = (bars[0].position + bars[1].position) * 0.5f + Vector2.Normalize(ovel) * 30;
			Color c2 = Lighting.GetColor((int)(P2.X / 16f), (int)(P2.Y / 16f));
			var vertex = new Vertex2D(P2, c2, new Vector3(0, 0.5f, 1));
			Vx.Add(bars[1]);
			Vx.Add(vertex);
			for (int i = 0; i < bars.Count - 2; i += 2)
			{
				Vx.Add(bars[i]);
				Vx.Add(bars[i + 2]);
				Vx.Add(bars[i + 1]);

				Vx.Add(bars[i + 1]);
				Vx.Add(bars[i + 2]);
				Vx.Add(bars[i + 3]);
			}
		}
		Texture2D t = ModContent.Request<Texture2D>("Everglow/Myth/UIImages/Tusk/DarkBloodRope").Value;
		Main.graphics.GraphicsDevice.Textures[0] = t; // GlodenBloodScaleMirror
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Vx.ToArray(), 0, Vx.Count / 3);
	}
}
