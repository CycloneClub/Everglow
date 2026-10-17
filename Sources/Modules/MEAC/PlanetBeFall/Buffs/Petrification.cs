namespace Everglow.MEAC.PlanetBeFall.Buffs;

public class Petrification : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.debuff[Type] = true;
		Main.pvpBuff[Type] = true;
		Main.buffNoSave[Type] = true;
	}

	public override void Update(NPC npc, ref int buffIndex)
	{
		base.Update(npc, ref buffIndex);
	}
}

public class PetrificationNPC : GlobalNPC
{
	public override void UpdateLifeRegen(NPC npc, ref int damage)
	{
	}

	public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		if (npc.HasBuff(ModContent.BuffType<Petrification>()))
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Effect frozenEff = ModAsset.PetrificationNPC.Value;
			frozenEff.Parameters["uImageSize"].SetValue(Main.npcFrameCount[npc.type]);
			frozenEff.Parameters["uHeatMap"].SetValue(ModAsset.Petrification_Color.Value);
			frozenEff.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_Crack_Dense.Value);
			frozenEff.Parameters["drawColor"].SetValue(drawColor.ToVector4());
			frozenEff.CurrentTechnique.Passes[0].Apply();
		}
		return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
	}

	public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		if (npc.HasBuff(ModContent.BuffType<Petrification>()))
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		}
		base.PostDraw(npc, spriteBatch, screenPos, drawColor);
	}

	public override void FindFrame(NPC npc, int frameHeight)
	{
		if (npc.HasBuff(ModContent.BuffType<Petrification>()))
		{
			npc.frameCounter = 0;
		}
		base.FindFrame(npc, frameHeight);
	}

	public override bool PreAI(NPC npc)
	{
		if (npc.HasBuff(ModContent.BuffType<Petrification>()))
		{
			npc.velocity *= 0;
			return false;
		}
		else
		{
			return base.PreAI(npc);
		}
	}

	public override void AI(NPC npc)
	{
		if (npc.HasBuff(ModContent.BuffType<Petrification>()))
		{
			npc.velocity *= 0;
			return;
		}
		else
		{
			base.AI(npc);
		}
	}
}
