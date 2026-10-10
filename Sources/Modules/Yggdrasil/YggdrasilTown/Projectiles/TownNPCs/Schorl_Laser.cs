using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.Dusts.TownNPCAttack;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;
using Microsoft.Xna.Framework;
using Terraria;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs;

public class Schorl_Laser : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public int Timer = 0;

	public Vector2 EndPos = Vector2.zeroVector;

	public NPC Owner = null;

	public NPC Target = null;

	public override void SetDefaults()
	{
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 60;
		Projectile.ArmorPenetration = 0;
		Projectile.friendly = true;
		Projectile.timeLeft = 180;
		Projectile.tileCollide = false;
		Projectile.penetrate = -1;
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
		Projectile.extraUpdates = 6;
		Timer = 0;
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
	}

	public override void AI()
	{
		Timer++;
		if (Timer > 60)
		{
			Projectile.extraUpdates = 0;
		}
		if (Timer == 60)
		{
			for (int i = 0; i < 30; i++)
			{
				var dust = Dust.NewDustDirect(Projectile.Center, 0, 0, ModContent.DustType<Schorl_GoldenDustNoDiffusion>());
				dust.position = Projectile.Center;
				dust.velocity = new Vector2(0, MathF.Sin(i / 30f * MathHelper.TwoPi * 5) + 3).RotatedBy(Projectile.velocity.ToRotationSafe() + i / 30f * MathHelper.TwoPi) * 0.6f;
			}
		}
		if (Owner == null)
		{
			foreach (var npc in Main.npc)
			{
				if (npc != null && npc.active && npc.type == ModContent.NPCType<TeahouseLady>())
				{
					if ((npc.Center - Projectile.Center).Length() < 50f)
					{
						Owner = npc;
						break;
					}
				}
			}
		}
		if (Owner != null)
		{
			Projectile.Center = Owner.Center + new Vector2(0, -4);
		}
		else
		{
			Projectile.active = false;
			return;
		}
		if (Target == null)
		{
			if (Projectile.ai[0] is >= 0 and < 200)
			{
				Target = Main.npc[(int)Projectile.ai[0]];
			}
		}
		if (Target != null && Target.active)
		{
			Projectile.velocity = (Target.Center - Projectile.Center).NormalizeSafe() * 6;
		}
		else if (Projectile.timeLeft > 10)
		{
			Projectile.timeLeft = 10;
		}
		Projectile.Center += Projectile.velocity * 2.2f;
		Vector2 checkPos = Projectile.Center;
		for (int i = 0; i < 500; i++)
		{
			checkPos += Projectile.velocity;
			if (Collision.SolidCollision(checkPos - new Vector2(8), 16, 16))
			{
				EndPos = checkPos;
				break;
			}
			EndPos = checkPos;
		}
		if (Projectile.timeLeft == 10)
		{
			float distance = (Projectile.Center - EndPos).Length() * 1f;
			for (int i = 0; i < distance; i++)
			{
				float value = i / distance;
				var dust = Dust.NewDustDirect(Projectile.Center, 0, 0, ModContent.DustType<Schorl_GoldenDust>());
				dust.position = Projectile.Center * value + EndPos * (1 - value);
				dust.velocity = new Vector2(0, MathF.Sin(i * 0.2f + (float)Main.time * 0.23f + Projectile.whoAmI * 0.36f)).RotatedBy(Projectile.velocity.ToRotationSafe()) * 0.4f;
			}
		}
		if (Main.rand.NextBool() && Timer >= 65 && Projectile.timeLeft > 10)
		{
			var dust = Dust.NewDustDirect(Projectile.Center, 0, 0, ModContent.DustType<Schorl_GoldenDust>());
			dust.position = EndPos;
			dust.velocity = new Vector2(0, MathF.Sin((float)Main.time * 0.23f + Projectile.whoAmI * 0.36f)).RotatedBy(Projectile.velocity.ToRotationSafe()) * 2f;
		}
		if (EndPos == Vector2.zeroVector)
		{
			Projectile.active = false;
			return;
		}
	}

	public override bool ShouldUpdatePosition() => false;

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		float point = 0;
		if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, EndPos, 4, ref point) && Timer > 70 && Projectile.timeLeft > 10)
		{
			return true;
		}
		return false;
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		var distance = (EndPos - Projectile.Center).Length() / 1500f;
		float size = 4;
		if (Timer < 60)
		{
			size = 0;
		}
		if (Timer is > 60 and < 70)
		{
			size = MathF.Sin((Timer - 63) / 10f * MathF.PI) * 5f;
		}
		if (Projectile.timeLeft < 20)
		{
			size = MathF.Pow(Projectile.timeLeft / 20f, 2) * 4f;
		}
		if (Projectile.timeLeft <= 10)
		{
			size = 0;
		}
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		var width = Projectile.velocity.NormalizeSafe().RotatedBy(MathHelper.PiOver2) * size;
		var step = Projectile.velocity.NormalizeSafe() * 6;
		var drawColor = new Color(0.7f, 0.8f, 0f, 0f);
		var timeValue = Timer * -0.02f;

		Effect effect = ModAsset.Schorl_Laser_Shader.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.CurrentTechnique.Passes[0].Apply();

		float startDistance = 15f;
		if ((EndPos - Projectile.Center).Length() < step.Length() * startDistance)
		{
			startDistance = (EndPos - Projectile.Center).Length() / step.Length() - 1;
		}

		float endDistance = 10f;
		if ((EndPos - Projectile.Center).Length() < step.Length() * (startDistance + endDistance))
		{
			endDistance = (EndPos - Projectile.Center).Length() / step.Length() - startDistance;
		}
		var bars = new List<Vertex2D>();
		for (int i = 0; i < startDistance; i++)
		{
			float zValue = MathF.Pow(i / startDistance, 0.5f);
			bars.Add(Projectile.Center + i * step + width, drawColor, new Vector3(timeValue, 0, zValue));
			bars.Add(Projectile.Center + i * step - width, drawColor, new Vector3(timeValue, 1f, zValue));
		}

		bars.Add(Projectile.Center + step * (startDistance + 1) + width, drawColor, new Vector3(timeValue, 0, 1));
		bars.Add(Projectile.Center + step * (startDistance + 1) - width, drawColor, new Vector3(timeValue, 1f, 1));

		bars.Add(EndPos - step * endDistance + width, drawColor, new Vector3(timeValue + distance, 0, 1));
		bars.Add(EndPos - step * endDistance - width, drawColor, new Vector3(timeValue + distance, 1f, 1));

		bars.Add(EndPos + width, drawColor * 0, new Vector3(timeValue + distance, 0, 1));
		bars.Add(EndPos - width, drawColor * 0, new Vector3(timeValue + distance, 1f, 1));
		if (bars.Count > 2)
		{
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_perlin.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
		bars = new List<Vertex2D>();
		width *= 1.5f;
		for (int i = 0; i < endDistance; i++)
		{
			float zValue = MathF.Pow(i / endDistance, 0.5f);
			bars.Add(EndPos - i * step + width, drawColor * (1 - zValue), new Vector3(timeValue, 0, zValue));
			bars.Add(EndPos - i * step - width, drawColor * (1 - zValue), new Vector3(timeValue, 1f, zValue));
		}
		if (bars.Count > 2)
		{
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_perlin.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		float r0 = 1;
		if (Timer is > 0 and < 30)
		{
			r0 = Timer / 30f;
		}
		if (Projectile.timeLeft < 20)
		{
			r0 = MathF.Pow(Projectile.timeLeft / 20f, 2);
		}
		if (Projectile.timeLeft <= 10)
		{
			r0 = 0;
		}
		bars = new List<Vertex2D>();
		for (int i = 0; i < 40; i++)
		{
			Vector2 radius = new Vector2(0, 30 + 6 * MathF.Sin(timeValue * 8.75f + Projectile.whoAmI) + MathF.Sin(i / 40f * 5 * MathHelper.TwoPi) * MathF.Sin(timeValue * 14f + Projectile.whoAmI) * 10).RotatedBy(i / 40f * MathHelper.TwoPi) * r0;
			float xValue = i / 40f * 5;
			bars.Add(Projectile.Center + radius - Main.screenPosition, drawColor, new Vector3(timeValue * 3 + xValue, 0, 0));
			bars.Add(Projectile.Center + radius * 0.6f - Main.screenPosition, drawColor, new Vector3(timeValue * 3 + xValue, 1f, 0));
		}
		if (bars.Count > 2)
		{
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_15.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
		bars = new List<Vertex2D>();
		for (int i = 0; i < 40; i++)
		{
			Vector2 radius = new Vector2(0, 15 + 4 * MathF.Sin(timeValue * 8.75f + Projectile.whoAmI + MathHelper.PiOver2) + MathF.Sin(i / 40f * 5 * MathHelper.TwoPi) * MathF.Sin(timeValue * 14f + Projectile.whoAmI) * 10).RotatedBy(i / 40f * MathHelper.TwoPi - timeValue * 3) * r0;
			float xValue = i / 40f * 5;
			bars.Add(Projectile.Center + radius - Main.screenPosition, drawColor, new Vector3(timeValue * 3 + xValue, 0, 0));
			bars.Add(Projectile.Center + radius * 0.6f - Main.screenPosition, drawColor, new Vector3(timeValue * 3 + xValue, 1f, 0));
		}
		if (bars.Count > 2)
		{
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_15.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		if (Timer > 50)
		{
			Texture2D slash = Commons.ModAsset.StarSlash.Value;
			float value = 1f;
			if (Projectile.timeLeft < 10f)
			{
				value *= Projectile.timeLeft / 10f;
			}
			if (Timer < 60)
			{
				value *= (Timer - 50) / 10f;
			}
			Main.spriteBatch.Draw(slash, Projectile.Center - Main.screenPosition, null, new Color(0.8f, 0.4f, 0.2f, 0) * value, MathHelper.Pi / 2f, slash.Size() * 0.5f, new Vector2(value * 3f, 1f + value * 0.4f) * 0.6f, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(slash, Projectile.Center - Main.screenPosition, null, new Color(0.8f, 0.5f, 0.2f, 0) * value, 0, slash.Size() * 0.5f, new Vector2(value * 2.4f, 0.75f + value * 0.3f) * 0.6f, SpriteEffects.None, 0);
		}
		return false;
	}
}
