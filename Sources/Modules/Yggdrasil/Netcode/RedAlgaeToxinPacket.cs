using Everglow.Commons.Netcode.Abstracts;
using Everglow.Yggdrasil.KelpCurtain.Buffs;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Summon;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Magic;

namespace Everglow.Yggdrasil.Netcode;

public class RedAlgaeToxinPacket : IPacket
{
	private int npcIndex;
	private int npcType;
	private int sourceIdentity;
	private int sourceType;
	private bool projectileHit;
	private bool applyOnly;

	public RedAlgaeToxinPacket()
	{
	}

	public RedAlgaeToxinPacket(NPC npc, int identity, int type, bool projectile, bool onlyApply)
	{
		npcIndex = npc.whoAmI;
		npcType = npc.netID;
		sourceIdentity = identity;
		sourceType = type;
		projectileHit = projectile;
		applyOnly = onlyApply;
	}

	public void Receive(BinaryReader reader, int whoAmI)
	{
		npcIndex = reader.ReadInt32();
		npcType = reader.ReadInt32();
		sourceIdentity = reader.ReadInt32();
		sourceType = reader.ReadInt32();
		projectileHit = reader.ReadBoolean();
		applyOnly = reader.ReadBoolean();
	}

	public void Send(BinaryWriter writer)
	{
		writer.Write(npcIndex);
		writer.Write(npcType);
		writer.Write(sourceIdentity);
		writer.Write(sourceType);
		writer.Write(projectileHit);
		writer.Write(applyOnly);
	}

	[HandlePacket(typeof(RedAlgaeToxinPacket))]
	public class Handler : IPacketHandler
	{
		public void Handle(IPacket packet, int whoAmI)
		{
			if (Main.netMode != NetmodeID.Server || whoAmI < 0 || whoAmI >= Main.maxPlayers)
			{
				return;
			}
			var data = (RedAlgaeToxinPacket)packet;
			Player player = Main.player[whoAmI];
			if (!player.active || player.dead || data.npcIndex < 0 || data.npcIndex >= Main.maxNPCs)
			{
				return;
			}
			NPC npc = Main.npc[data.npcIndex];
			if (!npc.active || npc.netID != data.npcType || npc.friendly || npc.dontTakeDamage)
			{
				return;
			}
			bool appliesToxin = false;
			if (data.projectileHit)
			{
				Projectile source = null;
				foreach (var candidate in Main.projectile)
				{
					if (candidate.active && candidate.owner == whoAmI && candidate.identity == data.sourceIdentity && candidate.type == data.sourceType)
					{
						source = candidate;
						break;
					}
				}
				if (source == null || !source.friendly || source.damage <= 0)
				{
					return;
				}
				appliesToxin = source.ModProjectile is IRedAlgaeToxinProjectile;
				if (data.applyOnly)
				{
					if (source.ModProjectile is not RedAlgaeMinionGyroscope_Proj
						|| !MathUtils.IntersectsCircleAABB(source.Center, 180, npc.position, npc.position + npc.Size))
					{
						return;
					}
				}
				else
				{
					// Allow a small positional margin for the owner's hit arriving after a movement update.
					var hitbox = source.Hitbox;
					int margin = source.ModProjectile is RedAlgaeMagicSpellBook_proj book ? (int)book.MaxRange + 64
						: ProjectileID.Sets.IsAWhip[source.type] ? 480 : 96;
					hitbox.Inflate(margin, margin);
					if (!source.Colliding(source.Hitbox, npc.Hitbox) && !hitbox.Intersects(npc.Hitbox))
					{
						return;
					}
				}
			}
			else if (data.applyOnly || player.HeldItem.type != data.sourceType || player.HeldItem.damage <= 0
				|| player.HeldItem.noMelee || player.itemAnimation <= 0 || Vector2.DistanceSquared(player.Center, npc.Center) > 300 * 300)
			{
				return;
			}
			RedAlgae_FriendlyDebuff_glocalNPC.ResolveContact(npc, player, appliesToxin, data.applyOnly);
		}
	}
}
