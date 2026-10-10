using Everglow.Myth.LanternMoon.Buffs;
using Everglow.Myth.LanternMoon.Projectiles.DashCore;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;

namespace Everglow.Myth.LanternMoon.NPCs.FlamingDashCore;

// [AutoloadBossHead]
public class GreenCore : ModNPC
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Green Core");
		Main.npcFrameCount[NPC.type] = 4;
	}

	public override void SetDefaults()
	{
		NPC.damage = 180;
		NPC.lifeMax = 100;
		NPC.width = 18;
		NPC.height = 24;
		NPC.defense = 0;
		NPC.value = 0;
		NPC.aiStyle = -1;
		// NPC.boss = true;
		NPC.knockBackResist = 0f;
		NPC.dontTakeDamage = false;
		NPC.noGravity = true;
		NPC.noTileCollide = true;
		NPC.HitSound = SoundID.NPCHit3;
		NPC.scale = 2f;
	}

	public static int PauseCool = 120;

	public override Color? GetAlpha(Color drawColor)
	{
		return new Color(NPC.color.R, NPC.color.G, NPC.color.B, 150);
	}

	public override void FindFrame(int frameHeight)
	{
		NPC.frameCounter += 0.2f;
		NPC.frameCounter %= Main.npcFrameCount[NPC.type];
		int num = (int)NPC.frameCounter;
		NPC.frame.Y = num * frameHeight;
	}

	public override void AI()
	{
		NPC.localAI[0] += 1;
		if (NPC.localAI[0] <= 15)
		{
			sca = NPC.localAI[0] / 15f;
		}
		else
		{
			sca = 1;
		}
		NPC.color.R = (byte)(NPC.color.R * 0.94f + aimcolor.R * 0.06f);
		NPC.color.G = (byte)(NPC.color.G * 0.94f + aimcolor.G * 0.06f);
		NPC.color.B = (byte)(NPC.color.B * 0.94f + aimcolor.B * 0.06f);
		NPC.color.A = (byte)(NPC.color.A * 0.94f + aimcolor.A * 0.06f);
	}

	private float x = 0;
	private float sca = 0;
	private Color aimcolor = new Color(0, 255, 17);

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		x += 0.01f;
		float K = (float)(Math.Sin(x + Math.Sin(x) * 6) * (0.95 + Math.Sin(x + 0.24 + Math.Sin(x))) + 3) / 30f;
		float M = (float)(Math.Sin(x + Math.Tan(x) * 6) * (0.95 + Math.Cos(x + 0.24 + Math.Sin(x))) + 3) / 30f;
		spriteBatch.Draw(ModAsset.LightEffect.Value, NPC.Center - Main.screenPosition, null, new Color(NPC.color.R, NPC.color.G, NPC.color.B, 0) * 0.4f, 0, new Vector2(128f, 128f), K * 2.4f * sca, SpriteEffects.None, 0f);
		spriteBatch.Draw(ModAsset.LightEffect.Value, NPC.Center - Main.screenPosition, null, new Color(NPC.color.R, NPC.color.G, NPC.color.B, 0) * 0.4f, (float)(Math.PI * 0.5), new Vector2(128f, 128f), K * 2.4f * sca, SpriteEffects.None, 0f);
		spriteBatch.Draw(ModAsset.LightEffect.Value, NPC.Center - Main.screenPosition, null, new Color(NPC.color.R, NPC.color.G, NPC.color.B, 0) * 0.4f, (float)(Math.PI * 0.75), new Vector2(128f, 128f), M * 2.4f * sca, SpriteEffects.None, 0f);
		spriteBatch.Draw(ModAsset.LightEffect.Value, NPC.Center - Main.screenPosition, null, new Color(NPC.color.R, NPC.color.G, NPC.color.B, 0) * 0.4f, (float)(Math.PI * 0.25), new Vector2(128f, 128f), M * 2.4f * sca, SpriteEffects.None, 0f);
		spriteBatch.Draw(ModAsset.LightEffect.Value, NPC.Center - Main.screenPosition, null, new Color(NPC.color.R, NPC.color.G, NPC.color.B, 0) * 0.4f, x * 6f, new Vector2(128f, 128f), (M + K) * 2.4f * sca, SpriteEffects.None, 0f);
		spriteBatch.Draw(ModAsset.LightEffect.Value, NPC.Center - Main.screenPosition, null, new Color(NPC.color.R, NPC.color.G, NPC.color.B, 0) * 0.4f, -x * 6f, new Vector2(128f, 128f), (float)Math.Sqrt(M * M + K * K) * 2.4f * sca, SpriteEffects.None, 0f);
		return true;
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, NPC.Center);
		if (NPC.life <= 0)
		{
			for (int a = 0; a < 4; a++)
			{
				Vector2 vF = new Vector2(0, Main.rand.NextFloat(0, 4f)).RotatedByRandom(6.283);
				int f = Gore.NewGore(null, NPC.Center, vF, ModContent.Find<ModGore>("Everglow/CoreGore0").Type, 1f);
				Main.gore[f].GetAlpha(new Color(NPC.color.R, NPC.color.G, NPC.color.B, 150));
				vF = new Vector2(0, Main.rand.NextFloat(0, 4f)).RotatedByRandom(6.283);
				f = Gore.NewGore(null, NPC.Center, vF, ModContent.Find<ModGore>("Everglow/CoreGore1").Type, 1f);
				Main.gore[f].GetAlpha(new Color(NPC.color.R, NPC.color.G, NPC.color.B, 150));
				vF = new Vector2(0, Main.rand.NextFloat(0, 4f)).RotatedByRandom(6.283);
				f = Gore.NewGore(null, NPC.Center, vF, ModContent.Find<ModGore>("Everglow/CoreGore2").Type, 1f);
				Main.gore[f].GetAlpha(new Color(NPC.color.R, NPC.color.G, NPC.color.B, 150));
				vF = new Vector2(0, Main.rand.NextFloat(0, 4f)).RotatedByRandom(6.283);
				f = Gore.NewGore(null, NPC.Center, vF, ModContent.Find<ModGore>("Everglow/CoreGore3").Type, 1f);
				Main.gore[f].GetAlpha(new Color(NPC.color.R, NPC.color.G, NPC.color.B, 150));
				vF = new Vector2(0, Main.rand.NextFloat(0, 4f)).RotatedByRandom(6.283);
				f = Gore.NewGore(null, NPC.Center, vF, ModContent.Find<ModGore>("Everglow/CoreGore4").Type, 1f);
				Main.gore[f].GetAlpha(new Color(NPC.color.R, NPC.color.G, NPC.color.B, 150));
			}
			for (int d = 0; d < Main.player.Length; d++)
			{
				if (Main.player[d].active && !Main.player[d].dead)
				{
					Main.player[d].ClearBuff(ModContent.BuffType<RedImmune>());
					Main.player[d].ClearBuff(ModContent.BuffType<BlueImmune>());
					Main.player[d].ClearBuff(ModContent.BuffType<GreenImmune>());
					Main.player[d].ClearBuff(ModContent.BuffType<YellowImmune>());
					Main.player[d].ClearBuff(ModContent.BuffType<PinkImmune>());
					Main.player[d].ClearBuff(ModContent.BuffType<PurpleImmune>());
					Main.player[d].ClearBuff(ModContent.BuffType<BrownImmune>());
					Main.player[d].ClearBuff(ModContent.BuffType<WhiteImmune>());
					Main.player[d].AddBuff(ModContent.BuffType<GreenImmune>(), 1800);
					for (int q = 0; q < Main.projectile.Length; q++)
					{
						if (Main.projectile[q].type == ModContent.ProjectileType<ImmuneCircle>())
						{
							Main.projectile[q].Kill();
						}
					}
					Projectile.NewProjectile(NPC.GetSource_FromAI(), Main.player[d].Center, Vector2.Zero, ModContent.ProjectileType<ImmuneCircle>(), 0, 0, d, d);
				}
			}
		}
	}

	public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
	}

	private struct CustomVertexInfo : IVertexType
	{
		private static VertexDeclaration _vertexDeclaration = new VertexDeclaration(new VertexElement[3]
		{
			new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
			new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
			new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0),
		});

		public Vector2 Position;
		public Color Color;
		public Vector3 TexCoord;

		public CustomVertexInfo(Vector2 position, Color color, Vector3 texCoord)
		{
			Position = position;
			Color = color;
			TexCoord = texCoord;
		}

		public VertexDeclaration VertexDeclaration
		{
			get
			{
				return _vertexDeclaration;
			}
		}
	}
}
