namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class TropicalSmoothieBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false;
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.GetModPlayer<TropicalSmoothiePlayer>().Enabled = true;
		// +50% increased magic damage
		player.GetDamage(DamageClass.Magic) *= 1.5f;
		// +50% increased magic critical strike chance
		player.GetCritChance(DamageClass.Magic) += 50;
	}

	public class TropicalSmoothiePlayer : ModPlayer
	{
		public bool Enabled { get; set; }

		public override void ResetEffects() => Enabled = false;

		public override ModPlayer Clone(Player newEntity)
		{
			var clone = (TropicalSmoothiePlayer)base.Clone(newEntity);
			clone.Enabled = false;
			return clone;
		}

		public override void PostUpdateEquips()
		{
			if (!Player.active || Player.dead || Player.outOfRange)
			{
				Enabled = false;
				return;
			}
			if (Enabled)
			{
				// At least 90% reduced mana cost
				Player.manaCost = Math.Min(Player.manaCost, 0.1f);
			}
		}
	}
}
