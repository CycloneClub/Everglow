using Everglow.Yggdrasil.Netcode;

namespace Everglow.Yggdrasil.KelpCurtain.Buffs;

public class RedAlgae_FriendlyDebuff_glocalNPC : GlobalNPC
{
	public override bool InstancePerEntity => true;

	private readonly RedAlgaeToxinState toxin = new();

	public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
	{
		if (projectile.ModProjectile is not IRedAlgaeToxinProjectile)
		{
			HandleProjectileHit(npc, projectile);
		}
	}

	public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
	{
		if (player.whoAmI != Main.myPlayer)
		{
			return;
		}
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			ModIns.PacketResolver.Send(new RedAlgaeToxinPacket(npc, -1, item.type, false, false));
			return;
		}
		ResolveContact(npc, player, false, false);
	}

	public static void HandleProjectileHit(NPC npc, Projectile projectile, bool applyOnly = false)
	{
		if (projectile.owner != Main.myPlayer)
		{
			return;
		}
		bool appliesToxin = projectile.ModProjectile is IRedAlgaeToxinProjectile;
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			ModIns.PacketResolver.Send(new RedAlgaeToxinPacket(npc, projectile.identity, projectile.type, true, applyOnly));
			return;
		}
		Player attacker = projectile.owner < Main.maxPlayers ? Main.player[projectile.owner] : null;
		ResolveContact(npc, attacker, appliesToxin, applyOnly);
	}

	public static void ResolveContact(NPC npc, Player attacker, bool appliesToxin, bool applyOnly)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient || !npc.active || npc.friendly || npc.dontTakeDamage)
		{
			return;
		}
		int buffType = ModContent.BuffType<RedAlgae_FriendlyDebuff>();
		int index = npc.FindBuffIndex(buffType);
		bool armorSet = attacker != null && attacker.active && attacker.GetModPlayer<KelpCurtainPlayer>().CrimsonMoonAlgaeSetBuff;
		var state = npc.GetGlobalNPC<RedAlgae_FriendlyDebuff_glocalNPC>().toxin;
		if (index < 0)
		{
			if (appliesToxin)
			{
				npc.AddBuff(buffType, state.Apply(armorSet));
				SyncBuffs(npc);
			}
			return;
		}
		if (applyOnly)
		{
			return;
		}

		int damage = state.Consume(npc.buffTime[index], armorSet);

		// Clear before striking so neither an early hit nor another hit can retain this application.
		npc.DelBuff(index);
		SyncBuffs(npc);
		if (damage <= 0)
		{
			return;
		}
		var detonation = new NPC.HitInfo { Damage = damage, Knockback = 0, HitDirection = 1, Crit = false };
		if (Main.netMode == NetmodeID.Server)
		{
			npc.StrikeNPC(detonation);
			NetMessage.SendStrikeNPC(npc, detonation);
		}
		else
		{
			npc.StrikeNPCWithCustomCombatText(detonation, new Color(0.7f, 0.1f, 0.4f), true);
		}
	}

	private static void SyncBuffs(NPC npc)
	{
		npc.netUpdate = true;
		if (Main.netMode == NetmodeID.Server)
		{
			NetMessage.SendData(MessageID.NPCBuffs, number: npc.whoAmI);
		}
	}
}
