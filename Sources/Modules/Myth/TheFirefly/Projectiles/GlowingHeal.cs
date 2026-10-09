using Everglow.Myth.Common;
using Terraria.Localization;

namespace Everglow.Myth.TheFirefly.Projectiles;

public class GlowingHeal : ModNPC
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("");
	}

	public override void SetDefaults()
	{
		NPC.width = 10;
		NPC.height = 10;
		NPC.aiStyle = -1;
		NPC.friendly = false;
		NPC.damage = 0;
		NPC.behindTiles = true;
		NPC.aiStyle = -1;
		NPC.alpha = 0;
		NPC.lifeMax = 1;
		NPC.dontTakeDamage = true;
		NPC.dontCountMe = true;
		NPC.scale = 1f;
		NPC.noGravity = true;
		NPC.noTileCollide = true;
		NPCID.Sets.TrailingMode[NPC.type] = 0;
		NPCID.Sets.TrailCacheLength[NPC.type] = 40;
	}

	private bool start = false;
	private Vector2 cent;
	private Vector2 acc;
	private float ome = 0;
	private float kx = 1;
	private bool healed = false;

	public override void AI()
	{
		Player player = Main.player[NPC.target];
		NPC.TargetClosest(false);
		if (!start)
		{
			NPC.velocity = new Vector2(Main.rand.NextFloat(0, 10f), 0).RotatedByRandom(6.28);
			acc = new Vector2(Main.rand.NextFloat(0, 0.35f), 0).RotatedByRandom(6.28);

			ome = Main.rand.NextFloat(-0.16f, 0.16f);
			start = true;
		}
		cent = player.Center;
		Vector2 v0 = cent - NPC.Center;
		if (v0.Length() >= 32)
		{
			Vector2 v = cent - (NPC.Center + NPC.velocity * 30);
			Vector2 v2 = v / v.Length() * 0.05f * (float)(1 + Math.Log(v.Length() + 1));

			acc *= 0.95f;
			NPC.velocity += acc + v2;
			NPC.velocity = NPC.velocity.RotatedBy(ome);
			ome *= 0.96f;
			kx = 20 - v0.Length() / 12f;
			if (kx < 1)
			{
				kx = 1;
			}
		}
		else
		{
			if (!healed)
			{
				if (player.statLife < player.statLifeMax)
				{
					player.statLife += 15;
					CombatText.NewText(new Rectangle((int)player.Center.X - 10, (int)player.Center.Y - 10, 20, 20), new Color(100, 255, 100), 15);
				}
				else
				{
					// player.statLife = player.statLifeMax;
				}
				healed = true;
			}
			NPC.velocity *= 0.8f;
			kx--;
			if (kx <= 1)
			{
				NPC.active = false;
			}
		}
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		return false;
	}

	private Effect ef;

	public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
		var bars = new List<Vertex2D>();
		ef = MythContent.QuickEffect("Effects/Trail");
		Vector2 v = cent - NPC.Center;
		int width = (int)(kx / 2);
		for (int i = 1; i < NPC.oldPos.Length - 1; ++i)
		{
			if (NPC.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			var normalDir = NPC.oldPos[i - 1] - NPC.oldPos[i];
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));

			var factor = i / (float)NPC.oldPos.Length;
			var color = Color.Lerp(Color.White, Color.Red, factor);

			var w = MathHelper.Lerp(1f, 0.05f, factor);

			bars.Add(new Vertex2D(NPC.oldPos[i] + normalDir * width + new Vector2(4, 35), color, new Vector3((float)Math.Sqrt(factor), 1, w)));
			bars.Add(new Vertex2D(NPC.oldPos[i] + normalDir * -width + new Vector2(4, 35), color, new Vector3((float)Math.Sqrt(factor), 0, w)));
		}

		var triangleList = new List<Vertex2D>();

		if (bars.Count > 2)
		{
			triangleList.Add(bars[0]);
			var vertex = new Vertex2D((bars[0].position + bars[1].position) * 0.5f + Vector2.Normalize(NPC.velocity) * 5, Color.White, new Vector3(0, 0.5f, 1));
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
			var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
			var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.ZoomMatrix;
			ef.Parameters["uTransform"].SetValue(model * projection);
			ef.Parameters["uTime"].SetValue(0);
			Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>("Everglow/Myth/TheFirefly/NPCs/Bosses/heatmapBlueD").Value;
			Main.graphics.GraphicsDevice.Textures[1] = ModContent.Request<Texture2D>("Everglow/Myth/TheFirefly/NPCs/Bosses/MeteroD").Value;
			Main.graphics.GraphicsDevice.Textures[2] = ModContent.Request<Texture2D>("Everglow/Myth/TheFirefly/NPCs/Bosses/MeteroD").Value;
			Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
			Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
			ef.CurrentTechnique.Passes[0].Apply();
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);
			Main.graphics.GraphicsDevice.RasterizerState = originalState;
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		}
	}
}
