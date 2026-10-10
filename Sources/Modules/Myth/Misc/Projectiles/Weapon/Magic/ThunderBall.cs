using Terraria;
using Terraria.Audio;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Magic;

public class ThunderBall : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 12;
		Projectile.height = 12;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = true;
		Projectile.timeLeft = 180;
		Projectile.alpha = 0;
		Projectile.penetrate = 4;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 60;
	}

	internal int Tokill = -1;
	internal bool[] HasBeenHit = new bool[200];
	internal int[] HasCool = new int[200];
	internal int[] coolingHit = new int[200];
	internal int TotalPower = 10;
	internal int addi = 0;
	private bool nul = false;
	private Vector2[] vdp = new Vector2[65];

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		Projectile.penetrate--;
		if (Projectile.velocity.X != oldVelocity.X)
		{
			Projectile.velocity.X = -oldVelocity.X;
		}

		if (Projectile.velocity.Y != oldVelocity.Y)
		{
			Projectile.velocity.Y = -oldVelocity.Y;
		}

		float a = Main.rand.NextFloat(0, 500.5f);
		Player player = Main.player[Projectile.owner];
		for (int y = 0; y < 3; y++)
		{
			Vector2 v = Projectile.velocity.RotatedBy(Math.PI * y / 1.5 + a) / Projectile.velocity.Length() * 15f;
			int h = Projectile.NewProjectile(null, Projectile.Center - v, v, ModContent.ProjectileType<ThunderBall2>(), Projectile.damage / 3, Projectile.knockBack, player.whoAmI, player.GetCritChance(DamageClass.Magic) * 100 + 16);
			Main.projectile[h].timeLeft = Main.rand.Next(4, 16);
		}
		for (int θ = 0; θ < 40; θ++)
		{
			Vector2 v = new Vector2(0, Main.rand.Next(25, 75) / 50f).RotatedByRandom(Math.PI * 2);
			int num25 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemSapphire, v.X, v.Y, 150, default, 0.6f);
			Main.dust[num25].noGravity = false;
		}
		return false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		float a = Main.rand.NextFloat(0, 500.5f);
		Player player = Main.player[Projectile.owner];
		for (int y = 0; y < 3; y++)
		{
			Vector2 v = Projectile.velocity.RotatedBy(Math.PI * y / 1.5 + a) / Projectile.velocity.Length() * 15f;
			int h = Projectile.NewProjectile(null, Projectile.Center, v, ModContent.ProjectileType<ThunderBall2>(), Projectile.damage / 3, Projectile.knockBack, player.whoAmI);
			Main.projectile[h].timeLeft = Main.rand.Next(4, 16);
		}
		for (int θ = 0; θ < 40; θ++)
		{
			Vector2 v = new Vector2(0, Main.rand.Next(25, 75) / 50f).RotatedByRandom(Math.PI * 2);
			int num25 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemSapphire, v.X, v.Y, 150, default, 0.6f);
			Main.dust[num25].noGravity = false;
		}
	}

	public override void AI()
	{
		addi += 1;
		if (addi % 60 == 1)
		{
			SoundEngine.PlaySound(new SoundStyle("Everglow/Myth/Sounds/ElectricCurrency"), Projectile.Center);
		}

		Projectile.velocity.Y += 0.15f;
		if (Projectile.timeLeft >= 1079)
		{
			for (int i = 0; i < 61; i++)
			{
				vdp[i] = new Vector2(0, Main.rand.NextFloat(0, 9f)).RotatedByRandom(Math.PI * 2d);
			}
		}
		if (TotalPower > 0)
		{
			for (int j = 0; j < 200; j++)
			{
				if (!HasBeenHit[j] && (Main.npc[j].Center - (Projectile.Center + Projectile.velocity * 10f)).Length() < 100 && !Main.npc[j].dontTakeDamage && !Main.npc[j].friendly && Main.npc[j].active)
				{
					SoundEngine.PlaySound(new SoundStyle("Everglow/Myth/Sounds/ElectricCurrency"), Projectile.Center);
					coolingHit[j] = 8;
					HasBeenHit[j] = true;
					TotalPower--;
					Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Main.npc[j].Center, Vector2.Zero, ModContent.ProjectileType<ThunderBallToNPC>(), Projectile.damage / 3, Projectile.knockBack, Projectile.owner, Projectile.whoAmI, j);
				}
				if (HasCool[j] == 0 && (Main.npc[j].Center - Projectile.Center).Length() < 70 && !Main.npc[j].dontTakeDamage && !Main.npc[j].friendly && Main.npc[j].active)
				{
					Player player = Main.player[Projectile.owner];
					NPC.HitModifiers npchitmodifier = new NPC.HitModifiers();
					NPC.HitInfo hit = npchitmodifier.ToHitInfo(Projectile.damage * Main.rand.NextFloat(0.85f, 1.15f), Main.rand.NextFloat(100f) < player.GetTotalCritChance(Projectile.DamageType), 2);
					Main.npc[j].StrikeNPC(hit, true, true);
					NetMessage.SendStrikeNPC(Main.npc[j], hit);
					HasCool[j] = 15;
				}
				if (HasCool[j] > 0)
				{
					HasCool[j]--;
				}

				if (HasBeenHit[j])
				{
					if (coolingHit[j] > 0)
					{
						coolingHit[j]--;
					}
					else
					{
						coolingHit[j] = 0;
						HasBeenHit[j] = false;
					}
				}
			}
		}

		if (Projectile.penetrate <= 1 && Projectile.penetrate > 0)
		{
			Projectile.velocity = Projectile.oldVelocity;
			Tokill = 45;
			Projectile.friendly = false;
			Projectile.damage = 0;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 200;
			nul = true;
		}
		for (int i = 0; i < 61; i++)
		{
			vdp[i] += new Vector2(0, Main.rand.NextFloat(0, 0.5f)).RotatedByRandom(Math.PI * 2d);
			if (vdp[i].Length() > 12)
			{
				vdp[i] = new Vector2(0, Main.rand.NextFloat(0, 5f)).RotatedByRandom(Math.PI * 2d);
			}
		}
		if (Tokill >= 0 && Tokill <= 2)
		{
			Projectile.Kill();
		}

		if (Tokill <= 44 && Tokill > 0)
		{
			Projectile.position = Projectile.oldPosition;
			Projectile.velocity = Projectile.oldVelocity;
		}
		if (Projectile.timeLeft > 60)
		{
			if (Tokill <= 0)
			{
				for (int i = 0; i < 4; i++)
				{
					Vector2 v = new Vector2(0, Main.rand.Next(25, 75) / 50f).RotatedByRandom(Math.PI * 2);
					int num25 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemSapphire, v.X, v.Y, 150, default, 0.6f);
					Main.dust[num25].noGravity = false;
				}
			}
		}
		else
		{
			for (int i = 0; i < 4; i++)
			{
				Vector2 v = new Vector2(0, Main.rand.Next(25, 75) / 50f).RotatedByRandom(Math.PI * 2);
				int num25 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemSapphire, v.X * Projectile.timeLeft / 60f, v.Y * Projectile.timeLeft / 60f, 150, default, 0.6f * Projectile.timeLeft / 60f);
				Main.dust[num25].noGravity = false;
			}
		}

		Tokill--;
	}

	public override Color? GetAlpha(Color lightColor)
	{
		if (!nul)
		{
			if (Projectile.timeLeft > 60f)
			{
				return new Color?(new Color(255, 255, 255, 0));
			}
			else
			{
				return new Color?(new Color(Projectile.timeLeft / 60f, Projectile.timeLeft / 60f, Projectile.timeLeft / 60f, 0));
			}
		}
		else
		{
			return new Color?(new Color(Tokill / 45f, Tokill / 45f, Tokill / 45f, 0));
		}
	}

	public override void PostDraw(Color lightColor)
	{
		var bars = new List<Vertex2D>();
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			int g = (i + 1080 - Projectile.timeLeft) % 60;
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			float width = 5;
			if (Projectile.timeLeft > 30)
			{
				width = 5;
			}
			else
			{
				width = Projectile.timeLeft / 6f;
			}
			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			if (normalDir.Length() < 0.2f)
			{
				normalDir = Projectile.velocity / Projectile.velocity.Length();
				for (int j = 1; j < Projectile.oldPos.Length; ++j)
				{
					i++;
					var normalDir2 = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
					if (normalDir2.Length() >= 0.2f)
					{
						break;
					}
				}
			}
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));

			var factor = i / (float)Projectile.oldPos.Length;
			var color = Color.Lerp(Color.White, Color.Transparent, factor);
			color.A = 0;
			var w = MathHelper.Lerp(1f, 0.05f, factor);

			if (i != 1)
			{
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width + new Vector2(6, 6) + vdp[g] - Main.screenPosition, color, new Vector3((float)Math.Sqrt(factor), 1, w)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width + new Vector2(6, 6) + vdp[g] - Main.screenPosition, color, new Vector3((float)Math.Sqrt(factor), 0, w)));
			}
			else
			{
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width + new Vector2(6, 6) - Main.screenPosition, color, new Vector3((float)Math.Sqrt(factor), 1, w)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width + new Vector2(6, 6) - Main.screenPosition, color, new Vector3((float)Math.Sqrt(factor), 0, w)));
			}
		}
		var triangleList = new List<Vertex2D>();
		if (bars.Count > 2)
		{
			triangleList.Add(bars[0]);
			var vertex = new Vertex2D((bars[0].position + bars[1].position) * 0.5f + Projectile.velocity / Projectile.velocity.Length() * 18f, Color.White, new Vector3(0, 0.5f, 1));
			triangleList.Add(bars[1]);
			triangleList.Add(vertex);
			for (int i = 0; i < bars.Count - 2; i += 2)
			{
				triangleList.Add(bars[i]);
				triangleList.Add(bars[i + 2]);
				triangleList.Add(bars[i + 1]);

				triangleList.Add(bars[i + 1]);
				triangleList.Add(bars[i + 2]);
				triangleList.Add(bars[i + 3]);
			}
			RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
			Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>("Everglow/Myth/UIImages/VisualTextures/ElecLine").Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);
			Main.graphics.GraphicsDevice.RasterizerState = originalState;
		}
	}
}
