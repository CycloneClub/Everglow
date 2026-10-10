using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs.Fevens;

internal class Fevens_MagicArray : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public int NPCOwner;
	public int timer = 0;
	public Vector2 ringPos = Vector2.Zero;

	public override string Texture => Commons.ModAsset.Empty_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 28;
		Projectile.height = 28;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 100000;
		Projectile.tileCollide = false;
		base.SetDefaults();
	}

	public override bool? CanCutTiles()
	{
		return false;
	}

	public override void OnSpawn(IEntitySource source)
	{
		FindOwner();
	}

	public override void AI()
	{
		if (NPCOwner <= 0)
		{
			FindOwner();
			timer--;
			if (timer < 0)
			{
				Projectile.Kill();
			}
			return;
		}
		NPC npc = Main.npc[NPCOwner];
		if (!npc.active)
		{
			NPCOwner = -1;
		}
		Projectile.Center = Projectile.Center * 0.7f + (npc.Center + new Vector2(npc.spriteDirection * 2, -12 * (float)(0.2 + Math.Sin(Main.timeForVisualEffects / 18d) / 2d))) * 0.3f;
		Projectile.spriteDirection = npc.spriteDirection;
		Projectile.velocity *= 0;

		var fevens_Boss = npc.ModNPC as NPCs.TownNPCs.Fevens;
		if (fevens_Boss.Phase == 2)
		{
			timer--;
			if (timer < 0)
			{
				Projectile.Kill();
			}
		}
		else
		{
			if (timer < 30)
			{
				timer++;
			}
		}
	}

	public void FindOwner()
	{
		float maxDistance = 600;
		foreach (var npc in Main.npc)
		{
			if (npc != null && npc.active && npc.type == ModContent.NPCType<NPCs.TownNPCs.Fevens>())
			{
				float distance = (npc.Center - Projectile.Center).Length();
				if (distance < maxDistance)
				{
					maxDistance = distance;
					NPCOwner = npc.whoAmI;
				}
			}
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		var effect = ModAsset.Fevens_BossMagicArray.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_melting.Value);
		effect.Parameters["uHeatMap"].SetValue(ModAsset.HeatMap_Fevens_BossMagicArray.Value);
		Texture2D halo = Commons.ModAsset.Trail.Value;
		Main.graphics.graphicsDevice.Textures[0] = halo;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
		effect.CurrentTechnique.Passes[0].Apply();
		var toBottom = new Vector2(0, 40);
		int accurance = 40;
		var bars = new List<Vertex2D>();
		for (int x = 0; x < accurance; x++)
		{
			float pocession = 0.2f + (30 - timer) / 30f;
			Vector2 radius = toBottom.RotatedBy((MathF.PI - x / (float)accurance * MathHelper.TwoPi) * 0.75f + MathF.PI);
			float width = 120f;
			Vector2 normalizedRadious = radius / 40f * MathF.Sin(x / 40f * MathF.PI) * width;
			bars.Add(new Vertex2D(Projectile.Center + radius + normalizedRadious, new Color(x / 40f, 0.0f, pocession, 1.0f), new Vector3(x / 65f, 0, (float)Main.time * 0.012f)));
			bars.Add(new Vertex2D(Projectile.Center + radius, new Color(x / 40f, 0.5f, pocession, 1.0f), new Vector3(x / 65f, 0, (float)Main.time * 0.012f + 0.5f)));
		}
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		bars = new List<Vertex2D>();
		for (int x = 0; x < accurance; x++)
		{
			float pocession = 0.1f + (30 - timer) / 30f;
			Vector2 radius = toBottom.RotatedBy((MathF.PI - x / (float)accurance * MathHelper.TwoPi) * 0.75f + MathF.PI);
			float width = 10f;
			Vector2 normalizedRadious = radius / 40f * MathF.Sin(x / 40f * MathF.PI) * width;
			bars.Add(new Vertex2D(Projectile.Center + radius - normalizedRadious, new Color(x / 40f, 0.4f, pocession, 1.0f), new Vector3(x / 65f, 0, (float)Main.time * 0.012f + 0.6f)));
			bars.Add(new Vertex2D(Projectile.Center + radius, new Color(x / 40f, 0.5f, pocession, 1.0f), new Vector3(x / 65f, 0, (float)Main.time * 0.012f + 0.5f)));
		}
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		if (NPCOwner >= 0)
		{
			NPC npc = Main.npc[NPCOwner];
			bars = new List<Vertex2D>();
			Vector2 flamePos = npc.Hitbox.Left() + new Vector2(10, -4);
			Vector2 endPos = Projectile.Center + new Vector2(-45, -10);
			Vector2 flameVel = Vector2.Normalize(endPos - flamePos).RotatedBy(-0.9f + 0.1f * MathF.Sin((float)Main.time * 0.13f)) * 3f;
			for (int x = 0; x < 200; x++)
			{
				float pocession = 0.1f + (30 - timer) / 30f;
				float width = Math.Min(x * 2, 10);
				Vector2 normalizedRadious = Vector2.Normalize(flameVel.RotatedBy(MathHelper.PiOver2)) * width;
				bars.Add(new Vertex2D(flamePos + normalizedRadious, new Color(x / 40f, 0.2f, pocession, 1.0f), new Vector3(x / 150f, 0, (float)Main.time * 0.012f + 0.6f)));
				bars.Add(new Vertex2D(flamePos - normalizedRadious, new Color(x / 40f, 0.8f, pocession, 1.0f), new Vector3(x / 150f, 0, (float)Main.time * 0.012f + 0.57f)));
				flamePos += flameVel;
				if ((endPos - flamePos).Length() < 10)
				{
					break;
				}
				flameVel = Vector2.Normalize(endPos - flamePos - flameVel) * 0.1f + flameVel * 0.9f;
			}
			if (bars.Count >= 4)
			{
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			}

			bars = new List<Vertex2D>();
			flamePos = npc.Hitbox.Right() + new Vector2(-10, -4);
			endPos = Projectile.Center + new Vector2(45, -10);
			flameVel = Vector2.Normalize(endPos - flamePos).RotatedBy(0.9f - 0.1f * MathF.Sin((float)Main.time * 0.13f)) * 3f;
			for (int x = 0; x < 200; x++)
			{
				float pocession = 0.1f + (30 - timer) / 30f;
				float width = Math.Min(x * 2, 10);
				Vector2 normalizedRadious = Vector2.Normalize(flameVel.RotatedBy(MathHelper.PiOver2)) * width;
				bars.Add(new Vertex2D(flamePos + normalizedRadious, new Color(x / 40f, 0.2f, pocession, 1.0f), new Vector3(x / 150f, 0, (float)Main.time * 0.012f + 0.3f)));
				bars.Add(new Vertex2D(flamePos - normalizedRadious, new Color(x / 40f, 0.8f, pocession, 1.0f), new Vector3(x / 150f, 0, (float)Main.time * 0.012f + 0.37f)));
				flamePos += flameVel;
				if ((endPos - flamePos).Length() < 10)
				{
					break;
				}
				flameVel = Vector2.Normalize(endPos - flamePos - flameVel) * 0.1f + flameVel * 0.9f;
			}
			if (bars.Count >= 4)
			{
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			}
		}

		float colorValue = timer / 30f;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		Texture2D starBlack = Commons.ModAsset.StarSlash_black.Value;
		Main.spriteBatch.Draw(starBlack, Projectile.Center + new Vector2(-40, 0) - Main.screenPosition, new Rectangle(0, 0, starBlack.Width, starBlack.Height / 2), new Color(0.8f, 0.8f, 0.8f, 0.2f * colorValue), -MathHelper.PiOver2, new Vector2(starBlack.Width * 0.5f, starBlack.Height * 0.5f), new Vector2(0.5f, 1f), SpriteEffects.None, 0);
		Main.spriteBatch.Draw(starBlack, Projectile.Center + new Vector2(40, 0) - Main.screenPosition, new Rectangle(0, 0, starBlack.Width, starBlack.Height / 2), new Color(0.8f, 0.8f, 0.8f, 0.2f * colorValue), MathHelper.PiOver2, new Vector2(starBlack.Width * 0.5f, starBlack.Height * 0.5f), new Vector2(0.5f, 1f), SpriteEffects.None, 0);
		Texture2D star = Commons.ModAsset.StarSlash.Value;
		Main.spriteBatch.Draw(star, Projectile.Center + new Vector2(-40, 0) - Main.screenPosition, new Rectangle(0, 0, star.Width, star.Height / 2), new Color(1f * colorValue, 0, 0, 0), -MathHelper.PiOver2, new Vector2(star.Width * 0.5f, star.Height * 0.5f), new Vector2(0.5f, 1f), SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, Projectile.Center + new Vector2(40, 0) - Main.screenPosition, new Rectangle(0, 0, star.Width, star.Height / 2), new Color(1f * colorValue, 0, 0, 0), MathHelper.PiOver2, new Vector2(star.Width * 0.5f, star.Height * 0.5f), new Vector2(0.5f, 1f), SpriteEffects.None, 0);
		return false;
	}
}
