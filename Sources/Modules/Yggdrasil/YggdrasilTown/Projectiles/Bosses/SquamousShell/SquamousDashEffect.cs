using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Bosses.SquamousShell;

public class SquamousDashEffect : ModProjectile, IWarpProjectile_warpStyle2
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public int NPCOwner = -1;

	public override string Texture => Commons.ModAsset.Empty_Mod;

	public override void SetDefaults()
	{
		Projectile.timeLeft = 1200;
		Projectile.tileCollide = false;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.ignoreWater = true;
		Projectile.width = 100;
		Projectile.height = 100;
	}

	public override void OnSpawn(IEntitySource source)
	{
		GetOwner();
	}

	public override void AI()
	{
		GetOwner();
		if (NPCOwner >= 0 && NPCOwner < Main.npc.Length)
		{
			if (Main.npc[NPCOwner] == null || !Main.npc[NPCOwner].active)
			{
				Projectile.active = false;
				return;
			}
		}
		else
		{
			Projectile.active = false;
			return;
		}
		Projectile.velocity *= 0;
		NPC owner = Main.npc[NPCOwner];
		Projectile.Center = owner.Center;
		float speed = Math.Abs(owner.velocity.X);
		if (speed > 4)
		{
			for (int g = 0; g < 2; g++)
			{
				Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0.0f, 2f)).RotatedByRandom(MathHelper.TwoPi) + new Vector2(owner.velocity.X * 0.5f, 0);
				var spark = new Spark_MoonBladeDust
				{
					Velocity = newVelocity,
					Active = true,
					Visible = true,
					Position = Projectile.position + new Vector2(Main.rand.NextFloat(Projectile.width), Main.rand.NextFloat(Projectile.height)),
					MaxTime = Main.rand.Next(30, 45),
					Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(9f, 17.0f)),
					Rotation = Main.rand.NextFloat(6.283f),
					noGravity = true,
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
				};
				Ins.VFXManager.Add(spark);
			}
		}
	}

	public void GetOwner()
	{
		if (NPCOwner != -1)
		{
			return;
		}
		if (Projectile.ai[0] != -1)
		{
			NPCOwner = (int)Projectile.ai[0];
		}
		else
		{
			Projectile.active = false;
		}
		if (NPCOwner >= 0 && NPCOwner < Main.npc.Length)
		{
			if (Main.npc[NPCOwner] != null && Main.npc[NPCOwner].active)
			{
				return;
			}
			else
			{
				Projectile.active = false;
			}
		}
		else
		{
			Projectile.active = false;
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		if (NPCOwner >= 0 && NPCOwner < Main.npc.Length)
		{
			if (Main.npc[NPCOwner] == null || !Main.npc[NPCOwner].active)
			{
				Projectile.active = false;
				return false;
			}
		}
		else
		{
			Projectile.active = false;
			return false;
		}
		NPC owner = Main.npc[NPCOwner];
		float speed = Math.Abs(owner.velocity.X);
		float power = speed / 10f;
		power = Math.Clamp(power, 0, 1);
		float timeValue = -(float)Main.time * 0.03f;
		int npcDir = 1;
		if (owner.velocity.X < 0)
		{
			npcDir = -1;
		}
		Vector2 checkPos = Projectile.Center + new Vector2(npcDir * 160f, 76);
		Color drawColor = new Color(0.25f, 0.75f, 0.67f, 0) * power;
		Color drawColorDark = Color.White * power;
		var bars = new List<Vertex2D>();
		var barsDark = new List<Vertex2D>();
		for (int t = 0; t <= 40; t++)
		{
			float height = t / 40f;
			height = MathF.Pow(height, 0.35f);
			float fade = 1 - height;
			bars.Add(checkPos, drawColor * fade, new Vector3(t / 60f + timeValue, 0, height));
			bars.Add(checkPos + new Vector2(0, -250), drawColor * fade, new Vector3(t / 60f + timeValue, 1, height));

			barsDark.Add(checkPos, drawColorDark * fade, new Vector3(t / 60f + timeValue, 0, height));
			barsDark.Add(checkPos + new Vector2(0, -250), drawColorDark * fade, new Vector3(t / 60f + timeValue, 1, height));
			checkPos += new Vector2(-8 * npcDir, 0);
		}
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Effect distort = ModAsset.SquamousDashEffectFlow.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		distort.Parameters["uTransform"].SetValue(model * projection);
		distort.CurrentTechnique.Passes[0].Apply();

		if (barsDark.Count > 3)
		{
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_flame_0_black.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, barsDark.ToArray(), 0, barsDark.Count - 2);
		}
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_flame_0.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.EmptyCrystal.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		if (NPCOwner >= 0 && NPCOwner < Main.npc.Length)
		{
			if (Main.npc[NPCOwner] == null || !Main.npc[NPCOwner].active)
			{
				Projectile.active = false;
				return;
			}
		}
		else
		{
			Projectile.active = false;
			return;
		}
		NPC owner = Main.npc[NPCOwner];
		float speed = Math.Abs(owner.velocity.X);
		float power = speed / 10f;
		power = Math.Clamp(power, 0, 1);
		float timeValue = -(float)Main.time * 0.01f;
		int npcDir = 1;
		if (owner.velocity.X < 0)
		{
			npcDir = -1;
		}
		Vector2 checkPos = Projectile.Center + new Vector2(npcDir * 160f, 76);
		Color drawColor = new Color(1f, 1f, 1f, 1f) * power;
		var bars = new List<Vertex2D>();
		for (int t = 0; t <= 40; t++)
		{
			float height = t / 40f;
			height = MathF.Pow(height, 0.35f);
			float fade = 1 - height;
			fade *= 0.1f;
			bars.Add(checkPos, drawColor * fade, new Vector3(t / 60f + timeValue, 0, height));
			bars.Add(checkPos + new Vector2(0, -250), drawColor * fade, new Vector3(t / 60f + timeValue, 1, height));
			checkPos += new Vector2(-8 * npcDir, 0);
		}

		Effect distort = ModAsset.SquamousDashEffectFlow.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		distort.Parameters["uTransform"].SetValue(model * projection);
		distort.CurrentTechnique.Passes[0].Apply();

		if (bars.Count > 3)
		{
			spriteBatch.Draw(Commons.ModAsset.Noise_rgb.Value, bars, PrimitiveType.TriangleStrip);
		}
	}
}
