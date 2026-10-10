using Everglow.Commons.DataStructures;
using Everglow.Myth.LanternMoon.NPCs;

namespace Everglow.Myth.LanternMoon.Projectiles.PerWave15;

public class LargeBloodLanternGhost_Matrix_Summon : ModProjectile
{
	public float Timer = 0;

	public NPC OwnerNPC;

	public override void SetDefaults()
	{
		Projectile.drawLayer = ProjectileDrawLayerID.BehindNPCs;
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 120;
		Projectile.alpha = 0;
		Projectile.penetrate = -1;
		Projectile.scale = 1f;
	}

	public override void AI()
	{
		Timer++;
		if (OwnerNPC != null && OwnerNPC.active && OwnerNPC.type == ModContent.NPCType<LargeBloodLanternGhost>())
		{
			Projectile.Center = OwnerNPC.Center;
			if (Timer == 30)
			{
				for (int i = 0; i < 5; i++)
				{
					Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, -116).RotatedBy(i / 5f * MathHelper.TwoPi), Vector2.zeroVector, ModContent.ProjectileType<LargeBloodLanternGhost_Minion>(), 20, 0f, Main.myPlayer, i);
					LargeBloodLanternGhost_Minion lBLGM = p0.ModProjectile as LargeBloodLanternGhost_Minion;
					if (lBLGM is not null)
					{
						lBLGM.OwnerNPC = OwnerNPC;
					}
				}
			}
		}
		else
		{
			if (Projectile.timeLeft > 60)
			{
				Projectile.timeLeft = 60;
				Timer = 60;
			}
		}
	}


	public override bool PreDraw(Player player, ref Color lightColor)
	{
		var drawPos = Projectile.Center;
		var drawColor = new Color(1f, 0.05f, 0.1f, 0);
		float disValue = 14;
		float fade = 1f;
		if (Projectile.timeLeft < 60)
		{
			fade *= Projectile.timeLeft / 60f;
		}
		float colorFade = 0;
		if (Timer < 30)
		{
			colorFade = Timer / 30f;
		}
		else
		{
			colorFade = 1f;
		}
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int i = 0; i <= 60; i++)
		{
			Vector2 radius = new Vector2(0, disValue).RotatedBy(i / 60f * MathHelper.TwoPi + 0);
			Vector2 radius_out = radius * 10f;
			float xCoord = i / 60f * 5;
			bars.Add(drawPos + radius, drawColor * colorFade, new Vector3(xCoord, 1, fade));
			bars.Add(drawPos + radius_out, drawColor * colorFade, new Vector3(xCoord, 0, fade));
			Lighting.AddLight(drawPos + radius * 1.5f, new Vector3(1f, 0f, 0.1f) * fade);
		}
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		Effect effect0 = ModAsset.LargeBloodLanternGhost_Matrix_Summon_Shader.Value;
		effect0.Parameters["uTransform"].SetValue(model * projection);
		effect0.Parameters["size1"].SetValue(Vector2.One);
		effect0.CurrentTechnique.Passes[0].Apply();

		if (bars.Count > 0)
		{
			var texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
			Main.graphics.GraphicsDevice.Textures[1] = Commons.ModAsset.Noise_perlin.Value;
			Main.graphics.GraphicsDevice.Textures[0] = texture;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}
}
