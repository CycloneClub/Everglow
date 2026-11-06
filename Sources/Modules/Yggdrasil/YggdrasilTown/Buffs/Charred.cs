using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.Commons.Mechanics;
using Everglow.Yggdrasil.Common;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Buffs;

public class Charred : ModBuff
{
	public const float DefenseReduction = 0.11f;
	public const int LifeRegenReductionFromDot = 36;

	public override string Texture => Commons.ModAsset.BuffTemplate_Mod;

	public override void SetStaticDefaults()
	{
		Main.pvpBuff[Type] = true;
		Main.buffNoSave[Type] = true;
		Main.debuff[Type] = true;
	}

	public override void Update(NPC npc, ref int buffIndex)
	{
		npc.GetGlobalNPC<YggdrasilGlobalNPC>().CharredActive = true;
		npc.lifeRegen -= LifeRegenReductionFromDot;
		npc.SetLifeRegenExpectedLossPerSecond(LifeRegenReductionFromDot);

		if (Main.rand.NextBool(4))
		{
			Dust dust = Dust.NewDustDirect(npc.position - new Vector2(2), npc.width + 4, npc.height + 4, DustID.Torch, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color), 3f);
			dust.noGravity = true;
			dust.velocity *= 1.8f;
			dust.velocity.Y -= 0.5f;
		}
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.statDefense *= 1 - DefenseReduction;
		player.GetModPlayer<StatBonusPlayer>().CharredLifeRegenLoss = LifeRegenReductionFromDot;

		if (Main.rand.NextBool(4))
		{
			Dust dust = Dust.NewDustDirect(player.position - new Vector2(2), player.width + 4, player.height + 4, DustID.Torch, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default(Color), 3f);
			dust.noGravity = true;
			dust.velocity *= 1.8f;
			dust.velocity.Y -= 0.5f;
		}
	}

	public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
	{
		return true;
	}
}
