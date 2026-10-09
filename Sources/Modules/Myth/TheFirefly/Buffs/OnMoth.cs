using Everglow.Myth.Common;
using Everglow.Myth.TheFirefly.Projectiles;
using Terraria;

namespace Everglow.Myth.TheFirefly.Buffs;

public class OnMoth : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.debuff[Type] = true;
		Main.buffNoSave[Type] = true;
	}

	public override void Update(NPC npc, ref int buffindex)
	{
	}
}

public class MothBuffTarget : GlobalNPC
{
	public static int[] mothStack = new int[256]; // TODO: Have this increase. Currently stays at 0
	public override bool InstancePerEntity => true;

	public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
	{
		if (npc.HasBuff(ModContent.BuffType<OnMoth>()))
		{
			if (projectile.DamageType == DamageClass.Summon)
			{
				float velocityValue = MathF.Log(npc.velocity.Length() + 1) / 10f;
				if (!npc.collideX && !npc.collideY)
				{

				}
				modifiers.FinalDamage = modifiers.FinalDamage * (1 + mothStack[npc.whoAmI] * 0.1f + velocityValue);
			}
		}
		base.ModifyHitByProjectile(npc, projectile, ref modifiers);
	}

	public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		if (npc.HasBuff(ModContent.BuffType<OnMoth>()))
		{
			float colorValue = 1;

			int buffIndex = npc.FindBuffIndex(ModContent.BuffType<OnMoth>());
			if (npc.buffTime[buffIndex] < 72)
			{
				colorValue = npc.buffTime[buffIndex] / 60f - 0.2f;
			}
			if (npc.buffTime[buffIndex] <= 1)
			{
				mothStack[npc.whoAmI] = 0;
			}
			int index = mothStack[npc.whoAmI];
			Texture2D number = MythContent.QuickTexture("TheFirefly/Projectiles/GlowFanTex/" + index.ToString());
			Texture2D butterfly = ModAsset.BlueFly.Value;
			// Texture2D butterflyD = ModAsset.BlueFlyD.Value;

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Effect mothMarkEff = ModAsset.MothMark.Value;
			var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
			var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
			mothMarkEff.Parameters["uTransform"].SetValue(model * projection);
			mothMarkEff.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_spiderNet.Value);
			mothMarkEff.Parameters["duration"].SetValue(colorValue);
			mothMarkEff.CurrentTechnique.Passes[0].Apply();

			spriteBatch.Draw(butterfly, npc.Top - new Vector2(0, 30), null, new Color(1, 1, 1, 0), 0, butterfly.Size() * 0.5f, 0.75f, SpriteEffects.None, 0f);
			spriteBatch.Draw(number, npc.Top - new Vector2(0, 60), null, new Color(1, 1, 1, 0), 0, number.Size() * 0.5f, 1, SpriteEffects.None, 0f);

			// spriteBatch.Draw(butterfly, npc.Center - Main.screenPosition, null, new Color(1, 1, 1, 0), 0, butterfly.Size() * 0.5f, 0.5f, SpriteEffects.None, 0f);
			// spriteBatch.Draw(number, npc.Center- Main.screenPosition, null, new Color(1, 1, 1, 0), 0, number.Size() * 0.5f, 0.5f, SpriteEffects.None, 0f);

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		}
	}
}
