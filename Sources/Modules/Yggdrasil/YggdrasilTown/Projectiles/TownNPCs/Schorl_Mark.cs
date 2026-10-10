using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;
using Microsoft.Xna.Framework;
using Terraria;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs;

public class Schorl_Mark : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public int Timer;

	public NPC Owner = null;

	public NPC Target = null;

	public override void SetDefaults()
	{
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 60;
		Projectile.ArmorPenetration = 0;
		Projectile.friendly = true;
		Projectile.timeLeft = 140;
		Projectile.tileCollide = false;
		Projectile.penetrate = -1;
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
	}

	public override void AI()
	{
		if (Owner == null)
		{
			foreach (var npc in Main.npc)
			{
				if (npc != null && npc.active && npc.type == ModContent.NPCType<TeahouseLady>())
				{
					if ((npc.Center - Projectile.Center).Length() < 50000f)
					{
						Owner = npc;
						break;
					}
				}
			}
		}
		if (Owner == null)
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
		Timer++;
		Projectile.velocity *= 0;
		if (Target != null && Target.active)
		{
			Projectile.Center = Target.Center;
		}
		else
		{
			Projectile.active = false;
			return;
		}
	}

	public override bool ShouldUpdatePosition() => false;

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		return base.Colliding(projHitbox, targetHitbox);
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		if (Target == null)
		{
			return false;
		}
		if (Owner == null)
		{
			return false;
		}
		Texture2D tex = ModAsset.Schorl_Mark.Value;
		Main.EntitySpriteDraw(tex, Target.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0), Target.rotation, tex.Size() * 0.5f, Target.scale, SpriteEffects.None, 0);

		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		// draw concentrated energy flow
		Effect effect = ModAsset.TeleportToYggdrasilFlowEffect.Value;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.CurrentTechnique.Passes[0].Apply();
		Vector2 drawCenter = Owner.Center + new Vector2(18 * Owner.spriteDirection, 0);

		// inner timer.
		float timeValue = (float)Main.time * 0.04f;
		float decrease = Projectile.timeLeft % 20 / 20f;
		float addTimeValue = MathF.Pow(0.75f - decrease, 3) * 10f;
		decrease = MathF.Pow(decrease, 3);
		decrease = MathF.Sin(decrease * MathHelper.Pi) * 7f;

		for (int r = 0; r < 6; r++)
		{
			var flows = new List<Vertex2D>();
			for (int i = 0; i <= 20; i++)
			{
				Vector2 thisJoint = new Vector2(0, i * 10).RotatedBy(GetFlowEffectRotation(i, r));
				Vector2 nextJoint = new Vector2(0, (i + 1) * 10).RotatedBy(GetFlowEffectRotation(i + 1, r));
				Vector2 normalWidth = Vector2.Normalize(nextJoint - thisJoint).RotatedBy(MathHelper.PiOver2);
				float vertexWidth = 40 + MathF.Sin(r) * 10;
				normalWidth *= vertexWidth;
				float drawWidth = MathF.Sin(Math.Min(i / 10f, 0.5f) * MathF.PI);
				float fade = 1;
				if (i > decrease)
				{
					fade *= Math.Clamp((decrease + 6 - i) / 6f, 0, 1);
				}

				var drawColor = new Color(1f, 1f, 1f, 1);
				flows.Add(drawCenter + thisJoint - normalWidth, drawColor * fade, new Vector3(i * 0.07f + timeValue + r * 0.2f + addTimeValue, 0, drawWidth));
				flows.Add(drawCenter + thisJoint + normalWidth, drawColor * fade, new Vector3(i * 0.07f + timeValue + r * 0.2f + addTimeValue, 1, drawWidth));
			}
			Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_2_black_thick.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, flows.ToArray(), 0, flows.Count - 2);
		}
		for (int r = 0; r < 6; r++)
		{
			var flows = new List<Vertex2D>();
			for (int i = 0; i <= 20; i++)
			{
				Vector2 thisJoint = new Vector2(0, i * 6).RotatedBy(GetFlowEffectRotation(i * 0.5f, r) + MathHelper.Pi / 6f);
				Vector2 nextJoint = new Vector2(0, (i + 1) * 6).RotatedBy(GetFlowEffectRotation(i * 0.5f + 0.5f, r) + MathHelper.Pi / 6f);
				Vector2 normalWidth = Vector2.Normalize(nextJoint - thisJoint).RotatedBy(MathHelper.PiOver2);
				float vertexWidth = 34 + MathF.Sin(r) * 5;
				normalWidth *= vertexWidth;
				float drawWidth = MathF.Sin(Math.Min(i / 4f, 0.5f) * MathF.PI);
				float fade = 1;
				if (i > decrease)
				{
					fade *= Math.Clamp((decrease + 6 - i) / 6f, 0, 1);
				}

				var drawColor = new Color(1f, 1f, 1f, 1);
				flows.Add(drawCenter + thisJoint - normalWidth, drawColor * fade, new Vector3(i * 0.035f + timeValue + r * 0.2f + addTimeValue, 0, drawWidth));
				flows.Add(drawCenter + thisJoint + normalWidth, drawColor * fade, new Vector3(i * 0.035f + timeValue + r * 0.2f + addTimeValue, 1, drawWidth));
			}
			Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_3_black.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, flows.ToArray(), 0, flows.Count - 2);
		}

		for (int r = 0; r < 6; r++)
		{
			var flows = new List<Vertex2D>();
			for (int i = 0; i <= 20; i++)
			{
				Vector2 thisJoint = new Vector2(0, i * 10).RotatedBy(GetFlowEffectRotation(i, r));
				Vector2 nextJoint = new Vector2(0, (i + 1) * 10).RotatedBy(GetFlowEffectRotation(i + 1, r));
				Vector2 normalWidth = Vector2.Normalize(nextJoint - thisJoint).RotatedBy(MathHelper.PiOver2);
				float vertexWidth = 40 + MathF.Sin(r) * 10;
				normalWidth *= vertexWidth;
				float drawWidth = MathF.Sin(Math.Min(i / 10f, 0.5f) * MathF.PI);
				float fade = 1;
				if (i > decrease)
				{
					fade *= Math.Clamp((decrease + 6 - i) / 6f, 0, 1);
				}

				var drawColor = new Color(0.7f, 0.5f, 0.0f, 0);
				flows.Add(drawCenter + thisJoint - normalWidth, drawColor * fade, new Vector3(i * 0.07f + timeValue + r * 0.2f + addTimeValue, 0, drawWidth));
				flows.Add(drawCenter + thisJoint + normalWidth, drawColor * fade, new Vector3(i * 0.07f + timeValue + r * 0.2f + addTimeValue, 1, drawWidth));
			}
			Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_2_thick.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, flows.ToArray(), 0, flows.Count - 2);
		}
		for (int r = 0; r < 6; r++)
		{
			var flows = new List<Vertex2D>();
			for (int i = 0; i <= 20; i++)
			{
				Vector2 thisJoint = new Vector2(0, i * 6).RotatedBy(GetFlowEffectRotation(i * 0.5f, r) + MathHelper.Pi / 6f);
				Vector2 nextJoint = new Vector2(0, (i + 1) * 6).RotatedBy(GetFlowEffectRotation(i * 0.5f + 0.5f, r) + MathHelper.Pi / 6f);
				Vector2 normalWidth = Vector2.Normalize(nextJoint - thisJoint).RotatedBy(MathHelper.PiOver2);
				float vertexWidth = 34 + MathF.Sin(r) * 5;
				normalWidth *= vertexWidth;
				float drawWidth = MathF.Sin(Math.Min(i / 4f, 0.5f) * MathF.PI);
				float fade = 1;
				if (i > decrease)
				{
					fade *= Math.Clamp((decrease + 6 - i) / 6f, 0, 1);
				}

				var drawColor = new Color(0.8f, 0.8f, 0.0f, 0);
				flows.Add(drawCenter + thisJoint - normalWidth, drawColor * fade, new Vector3(i * 0.035f + timeValue + r * 0.2f + addTimeValue, 0, drawWidth));
				flows.Add(drawCenter + thisJoint + normalWidth, drawColor * fade, new Vector3(i * 0.035f + timeValue + r * 0.2f + addTimeValue, 1, drawWidth));
			}
			Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_3.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, flows.ToArray(), 0, flows.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		decrease = Projectile.timeLeft % 20 / 20f;

		if (decrease < 0.5f)
		{
			Texture2D slash = Commons.ModAsset.StarSlash.Value;
			float value = MathF.Cos((decrease - 0.25f) / 0.5f * MathHelper.Pi);
			float mainRot = (Timer - Timer % 20f) / 20f;
			Main.spriteBatch.Draw(slash, Owner.Center + new Vector2(18 * Owner.spriteDirection, 0) - Main.screenPosition, null, new Color(0.8f, 0.8f, 0.0f, 0), mainRot, slash.Size() * 0.5f, new Vector2(value, 1f + value * 1.3f) * 0.6f, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(slash, Owner.Center + new Vector2(18 * Owner.spriteDirection, 0) - Main.screenPosition, null, new Color(0.8f, 0.8f, 0.0f, 0), mainRot + MathHelper.Pi / 3f, slash.Size() * 0.5f, new Vector2(value * 0.5f, 0.75f + value * 0.3f) * 0.6f, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(slash, Owner.Center + new Vector2(18 * Owner.spriteDirection, 0) - Main.screenPosition, null, new Color(0.8f, 0.8f, 0.0f, 0), mainRot - MathHelper.Pi / 3f, slash.Size() * 0.5f, new Vector2(value * 0.5f, 0.75f + value * 0.3f) * 0.6f, SpriteEffects.None, 0);
		}

		return false;
	}

	private float GetFlowEffectRotation(float i, float r)
	{
		float timeValue = (float)Main.time * 0.04f;
		timeValue *= 0.5f;

		return r / 6f * MathHelper.TwoPi + MathF.Sin(i * 0.07f + r * 0.03f + timeValue + Projectile.whoAmI) * 1.4f + MathF.Sin(i * 0.12f + r + timeValue) * 0.2f;
	}
}
