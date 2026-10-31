using Everglow.Myth.TheTusk.Projectiles;

namespace Everglow.Myth.TheTusk.Buffs;

public class TuskCursed : ModBuff
{
	/// <summary>
	/// Generate tusk around you.
	/// </summary>
	public override void SetStaticDefaults()
	{
		Main.buffNoSave[Type] = true;
		Main.buffNoTimeDisplay[Type] = true;
		Main.debuff[Type] = true;
	}

	public override void Update(Player player, ref int buffIndex)
	{
		if (player.active && !player.dead)
		{
			Projectile p0 = Projectile.NewProjectileDirect(player.GetSource_FromAI(), player.Center + new Vector2(0, Main.rand.NextFloat(30)).RotatedByRandom(MathHelper.TwoPi), Vector2.zeroVector, GetTusk_groundType(), GetDamage(18), 1, player.whoAmI);
			p0.timeLeft = Main.rand.Next(101, 110);
		}
	}

	public int GetTusk_groundType()
	{
		int type = ModContent.ProjectileType<Tusk_ground_little>();
		if (Main.rand.NextBool(3))
		{
			type = ModContent.ProjectileType<Tusk_ground>();
		}
		return type;
	}

	public int GetDamage(int origValue)
	{
		if (Main.expertMode)
		{
			return (int)(origValue * 1.6f);
		}
		if (Main.masterMode)
		{
			return (int)(origValue * 2.4f);
		}
		return origValue;
	}
}
