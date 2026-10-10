using Everglow.Commons.DataStructures;
using Everglow.Myth.LanternMoon.NPCs;

namespace Everglow.Myth.LanternMoon.Projectiles.PerWave15;

public class WizardLantern_Matrix_Thunder : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.EnemyProjectiles;

	public float Timer = 0;

	public NPC OwnerNPC;

	public struct LightningBolt()
	{
		public List<Vector3> Joint;
		public int Timer;
		public int MaxTime;
		public bool Active;
		public float[] ai;
	}

	public List<LightningBolt> LightningBolts = new List<LightningBolt>();

	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 180;
		Projectile.alpha = 0;
		Projectile.penetrate = -1;
		Projectile.scale = 0.1f;
		Projectile.hide = true;
	}

	public void UpdateLightningBolts()
	{
		if (Main.rand.NextBool(6) && Projectile.timeLeft > 20)
		{
			Vector2 pos = Vector2.zeroVector;
			Vector2 vel = new Vector2(0, 4).RotatedByRandom(MathHelper.TwoPi);
			AddLightningBolt(pos, vel, 1);
		}
		for (int k = LightningBolts.Count - 1; k >= 0; k--)
		{
			LightningBolt bolt = LightningBolts[k];
			if (bolt.Active)
			{
				bolt.Timer++;
				if (bolt.Timer > bolt.MaxTime)
				{
					bolt.Active = false;
				}
				for (int i = 0; i < bolt.Joint.Count; i++)
				{
					Vector2 pos = new Vector2(bolt.Joint[i].X, bolt.Joint[i].Y);
					if (i > 0)
					{
						pos += new Vector2(0, 0.25f).RotatedByRandom(MathHelper.TwoPi);
					}
					bolt.Joint[i] = new Vector3(pos, bolt.Joint[i].Z * 0.9f);
				}
				LightningBolts[k] = bolt;
			}
			else
			{
				LightningBolts.Remove(bolt);
			}
		}
	}

	public void AddLightningBolt(Vector2 position, Vector2 velocity, float size)
	{
		LightningBolt bolt = new LightningBolt();
		bolt.Joint = new List<Vector3>();
		bolt.Active = true;
		bolt.Timer = 0;
		bolt.MaxTime = 30;
		float totalRot = 0;
		for (int i = 0; i < 30; i++)
		{
			bolt.Joint.Add(new Vector3(position, size));
			size *= 0.96f;
			position += velocity;
			float rot = Main.rand.NextFloat(-0.3f, 0.3f);
			rot -= totalRot * 0.3f;
			velocity = velocity.RotatedBy(rot);
			totalRot += rot;
			if (Main.rand.NextBool(9) && size > 0.2f)
			{
				float rot2 = Main.rand.NextFloat(-0.7f, 0.7f);
				Vector2 vel2 = velocity.RotatedBy(-rot2);
				velocity = velocity.RotatedBy(rot2);
				size *= 0.5f;
				AddLightningBolt(position, vel2, size * 0.5f);
			}
		}
		LightningBolts.Add(bolt);
	}

	public override void AI()
	{
		Timer++;
		UpdateLightningBolts();
		if (OwnerNPC != null && OwnerNPC.active && OwnerNPC.type == ModContent.NPCType<WizardLantern>())
		{
			Projectile.Center = OwnerNPC.Center;
			if (Timer is 10 or 20)
			{
				for (int i = 0; i < 4; i++)
				{
					float addRot = MathHelper.PiOver4;
					if (Timer == 20)
					{
						addRot = 0;
					}
					Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, Timer * 1.4f + 80).RotatedBy(i / 4f * MathHelper.TwoPi + addRot), Vector2.zeroVector, ModContent.ProjectileType<ThunderSpell>(), 1, 0f, Main.myPlayer, i);
					p0.spriteDirection = -1;
					if (Timer == 10)
					{
						p0.spriteDirection = 1;
					}
					ThunderSpell tS = p0.ModProjectile as ThunderSpell;
					if (tS is not null)
					{
						tS.OwnerNPC = OwnerNPC;
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

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		behindNPCs.Add(index);
		base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		var drawPos = Projectile.Center;
		var drawColor = new Color(1f, 0.9f, 0.4f, 0);
		var drawColor2 = new Color(1f, 0.6f, 0.2f, 0);

		var drawColor3 = new Color(0.5f, 0.4f, 0.5f, 0);
		float disValue = 50;
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
			Vector2 radius_out = radius * 2f;
			float xCoord = i / 60f * 4;
			bars.Add(drawPos + radius, drawColor2 * colorFade, new Vector3(xCoord + Timer * 0.003f, 1, fade));
			bars.Add(drawPos + radius_out, drawColor * colorFade, new Vector3(xCoord + Timer * 0.003f, 0, fade));
			Lighting.AddLight(drawPos + radius * 1.5f, new Vector3(0.5f, 0.3f, 0.1f) * fade);
		}

		List<Vertex2D> bars_vortex = new List<Vertex2D>();
		for (int i = 0; i <= 60; i++)
		{
			Vector2 radius = new Vector2(0, disValue * 0.15f).RotatedBy(i / 60f * MathHelper.TwoPi + 0);
			Vector2 radius_out = radius * 8f;
			float xCoord = i / 60f;
			bars_vortex.Add(drawPos + radius, drawColor * colorFade, new Vector3(xCoord + Timer * 0.003f + 0.5f, 1, fade));
			bars_vortex.Add(drawPos + radius_out, drawColor3 * colorFade, new Vector3(xCoord + Timer * 0.003f, 0, fade));
		}
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		Effect effect0 = ModAsset.WizardLantern_Thunder_Matrix_Shader.Value;
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
		if (bars_vortex.Count > 0)
		{
			var texture = Commons.ModAsset.Noise_hiveCyber.Value;
			Main.graphics.GraphicsDevice.Textures[0] = texture;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_vortex.ToArray(), 0, bars_vortex.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		List<Vertex2D> bars_bolt = new List<Vertex2D>();
		foreach (var bolt in LightningBolts)
		{
			Color lightningColor = Color.Lerp(new Color(1f, 0.9f, 0.4f, 0), new Color(0.3f, 0.25f, 0.1f, 0), bolt.Timer / (float)bolt.MaxTime);
			for (int i = 0; i < bolt.Joint.Count - 1; i++)
			{
				Vector2 pos = new Vector2(bolt.Joint[i].X, bolt.Joint[i].Y);
				Vector2 posNext = new Vector2(bolt.Joint[i + 1].X, bolt.Joint[i + 1].Y);
				Vector2 dir = posNext - pos;
				dir = dir.NormalizeSafe();
				Vector2 width = dir.RotatedBy(MathHelper.PiOver2) * 8;
				float mulColor = 1;
				if (i == 0)
				{
					mulColor *= 0;
				}
				if (i == bolt.Joint.Count - 2)
				{
					mulColor *= 0;
				}
				bars_bolt.Add(drawPos + pos + width, lightningColor * mulColor, new Vector3(i / 30f, 0, bolt.Joint[i].Z));
				bars_bolt.Add(drawPos + pos - width, lightningColor * mulColor, new Vector3(i / 30f, 1, bolt.Joint[i].Z));
				if (i % 6 == 0)
				{
					Lighting.AddLight(drawPos + pos, new Vector3(1f, 0.9f, 0.4f) * fade * mulColor * bolt.Joint[i].Z * 1.5f);
				}
			}
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Effect effect1 = ModAsset.CylindricalLantern_explosion_lightningbolt.Value;
		effect1.Parameters["uTransform"].SetValue(model * projection);
		effect1.CurrentTechnique.Passes[0].Apply();
		if (bars_bolt.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_7.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_bolt.ToArray(), 0, bars_bolt.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}
}
