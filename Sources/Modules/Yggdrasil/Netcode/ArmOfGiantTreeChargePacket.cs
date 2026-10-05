using Everglow.Commons.Netcode.Abstracts;
using Everglow.Yggdrasil.KelpCurtain.Items.Weapons.UnderwaterTreasury;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Melee;

namespace Everglow.Yggdrasil.Netcode;

public class ArmOfGiantTreeChargePacket : IPacket
{
	private bool release;
	private bool alternate;
	private short slot;
	private int identity;
	private float aimAngle;

	public void Receive(BinaryReader reader, int whoAmI)
	{
		release = reader.ReadBoolean();
		alternate = reader.ReadBoolean();
		slot = reader.ReadInt16();
		identity = reader.ReadInt32();
		aimAngle = reader.ReadSingle();
	}

	public void Send(BinaryWriter writer)
	{
		writer.Write(release);
		writer.Write(alternate);
		writer.Write(slot);
		writer.Write(identity);
		writer.Write(aimAngle);
	}

	public static void Request(Player player, bool release, bool alternate, float angle, int identity = -1)
	{
		var packet = new ArmOfGiantTreeChargePacket { release = release, alternate = alternate, slot = (short)player.selectedItem, identity = identity, aimAngle = angle };
		if (Main.netMode == NetmodeID.SinglePlayer)
		{
			HandleRequest(packet, player.whoAmI);
		}
		else if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI == Main.myPlayer)
		{
			ModIns.PacketResolver.Send(packet, toClient: -1, ignoreClient: Main.myPlayer);
		}
	}

	private static void HandleRequest(ArmOfGiantTreeChargePacket packet, int whoAmI)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient || whoAmI < 0 || whoAmI >= Main.maxPlayers || !float.IsFinite(packet.aimAngle))
		{
			return;
		}
		Player player = Main.player[whoAmI];
		if (!player.active || player.dead || player.noItems || player.CCed || packet.slot < 0 || packet.slot >= player.inventory.Length
			|| player.selectedItem != packet.slot || player.HeldItem.type != ModContent.ItemType<ArmOfGiantTree>())
		{
			return;
		}
		int type = ModContent.ProjectileType<ArmOfGiantTreeHeld>();
		foreach (Projectile projectile in Main.projectile)
		{
			if (projectile.active && projectile.owner == whoAmI && projectile.type == type)
			{
				if (packet.release && projectile.identity == packet.identity)
				{
					((ArmOfGiantTreeHeld)projectile.ModProjectile).Release(packet.aimAngle);
				}
				return;
			}
		}
		if (packet.release)
		{
			return;
		}
		Item item = player.HeldItem;
		int index = Projectile.NewProjectile(player.GetSource_ItemUse(item), player.MountedCenter, packet.aimAngle.ToRotationVector2(), type,
			player.GetWeaponDamage(item), player.GetWeaponKnockback(item, item.knockBack), whoAmI,
			(float)(packet.alternate ? ArmOfGiantTreeAttackState.AttackPhase.NormalSwing : ArmOfGiantTreeAttackState.AttackPhase.Charging));
		if (index >= 0 && index < Main.maxProjectiles)
		{
			Main.projectile[index].netUpdate = true;
		}
	}

	[HandlePacket(typeof(ArmOfGiantTreeChargePacket))]
	public class ArmOfGiantTreeChargePacketHandler : IPacketHandler
	{
		public void Handle(IPacket packet, int whoAmI) => HandleRequest((ArmOfGiantTreeChargePacket)packet, whoAmI);
	}
}
