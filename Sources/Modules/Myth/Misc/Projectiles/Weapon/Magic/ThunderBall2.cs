using Terraria.Localization;
using Terraria.ID;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Magic;

public class ThunderBall2 : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 12;
		Projectile.height = 12;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.hostile = false;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 30;
		Projectile.alpha = 0;
		Projectile.penetrate = 9;
		Projectile.scale = 1;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 60;
	}
	private int Tokill = -1;
	public override void AI()
	{
		Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(Main.rand.NextFloat(-10f / Projectile.timeLeft, 0f), Main.rand.NextFloat(0f, 10f / Projectile.timeLeft)));
		Player player = Main.player[Projectile.owner];
		if (Main.rand.NextBool(8))
		{
			Vector2 v = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-1.5f, 1.5f));
			int h = Projectile.NewProjectile(null, Projectile.Center - v, v, ModContent.ProjectileType<ThunderBall2>(), Projectile.damage / 2, Projectile.knockBack, player.whoAmI, player.GetCritChance(DamageClass.Magic) * 100 + 16);
			Main.projectile[h].timeLeft = Projectile.timeLeft;
		}
		for (int j = 0; j < 200; j++)
		{
			if ((Main.npc[j].Center - Projectile.Center).Length() < 25 && !Main.npc[j].dontTakeDamage && !Main.npc[j].friendly && Main.npc[j].active)
			{
				NPC.HitModifiers npchitmodifier = new NPC.HitModifiers();
				NPC.HitInfo hit = npchitmodifier.ToHitInfo(Projectile.damage * Main.rand.NextFloat(0.85f, 1.15f), Main.rand.NextFloat(100f) < player.GetTotalCritChance(Projectile.DamageType), 2);
				Main.npc[j].StrikeNPC(hit, true, true);
				NetMessage.SendStrikeNPC(Main.npc[j], hit);
				for (int θ = 0; θ < 8; θ++)
				{
					Vector2 v = new Vector2(0, Main.rand.Next(25, 75) / 50f).RotatedByRandom(Math.PI * 2);
					int num25 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemSapphire, v.X, v.Y, 150, default, 0.6f);
					Main.dust[num25].noGravity = false;
				}
			}
		}
	}
	private bool Nul = false;
	public override Color? GetAlpha(Color lightColor)
	{
		if (!Nul)
		{
			return new Color?(new Color(255, 255, 255, 0));
		}
		else
		{
			return new Color?(new Color(Tokill / 45f, Tokill / 45f, Tokill / 45f, 0));
		}
	}
	public override void PostDraw(Color lightColor)
	{
		Projectile.ai[0] = Tokill;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		var bars = new List<Vertex2D>();
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}
		}
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			float width = 18;
			if (Projectile.timeLeft > 30)
			{
				width = 18;
			}
			else
			{
				width = Projectile.timeLeft / 5f * 3;
			}
			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			if (normalDir.Length() < 0.2f)
			{
				normalDir = Projectile.velocity / Projectile.velocity.Length();
			}

			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));

			var factor = i / (float)Projectile.oldPos.Length;
			var color = new Color(0, 0.9f, 1f, 0);
			var w = MathHelper.Lerp(1f, 0.05f, factor);

			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width + new Vector2(6, 6) - Main.screenPosition, color, new Vector3((float)Math.Sqrt(factor), 1, w)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width + new Vector2(6, 6) - Main.screenPosition, color, new Vector3((float)Math.Sqrt(factor), 0, w)));
		}
		var triangleList = new List<Vertex2D>();
		if (bars.Count > 2)
		{
			triangleList.Add(bars[0]);
			Vector2 va = Projectile.velocity * 1.5f;
			if (Tokill <= 44 && Tokill > 0)
			{
				va = Projectile.velocity * 0.05f;
			}

			var vertex = new Vertex2D((bars[0].position + bars[1].position) * 0.5f + va, new Color(0, 0.9f, 1f, 0), new Vector3(0, 0.5f, 1));
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
			Texture2D t = ModContent.Request<Texture2D>("Everglow/Myth/UIImages/VisualTextures/FogTraceTheta2").Value;
			Main.graphics.GraphicsDevice.Textures[0] = t; // GlodenBloodScaleMirror
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		}
	}
}
