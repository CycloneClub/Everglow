using System.IO;
using Everglow.Yggdrasil.KelpCurtain.Items.Weapons;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Ranged;

public class TendonGreatbowOrigin : GlobalProjectile
{
	public override bool InstancePerEntity => true;

	public bool FromTendonGreatbow { get; set; }

	public override void OnSpawn(Projectile projectile, IEntitySource source)
	{
		FromTendonGreatbow = (source is IEntitySource_WithStatsFromItem itemSource && itemSource.Item.ModItem is TendonGreatbow)
			|| (source is EntitySource_Parent { Entity: Projectile parent } && parent.GetGlobalProjectile<TendonGreatbowOrigin>().FromTendonGreatbow);
	}

	public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
	{
		if (FromTendonGreatbow && target.boss)
		{
			modifiers.FinalDamage *= 1.1f;
		}
	}

	public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter) => bitWriter.WriteBit(FromTendonGreatbow);

	public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader) => FromTendonGreatbow = bitReader.ReadBit();
}
