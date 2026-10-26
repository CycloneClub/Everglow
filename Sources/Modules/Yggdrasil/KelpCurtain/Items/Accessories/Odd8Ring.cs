using static Terraria.NPC;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Accessories;

public class Odd8Ring : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetDefaults()
	{
		Item.width = 22;
		Item.height = 22;
		Item.value = 1375;
		Item.accessory = true;
		Item.rare = ItemRarityID.Green;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		Odd8RingEquiper o8RE = player.GetModPlayer<Odd8RingEquiper>();
		o8RE.Odd8Enable = true;
	}
}

internal class Odd8RingEquiper : ModPlayer
{
	public bool Odd8Enable = false;

	public override void ResetEffects()
	{
		Odd8Enable = false;
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		if (Odd8Enable)
		{
			modifiers.ModifyHitInfo += (ref HitInfo info) =>
			{
				if (info.Damage < 8)
				{
					GoldStrike = 1;
					info.Damage = 8;
				}
			};
		}
	}

	public override void ModifyHurt(ref Player.HurtModifiers modifiers)
	{
		if (modifiers.PvP)
		{
			var attacker = Main.player[modifiers.DamageSource.SourcePlayerIndex].GetModPlayer<Odd8RingEquiper>();
			if (attacker.Odd8Enable)
			{
				// TODO 144
			}
		}
	}

	public static int GoldStrike = 0;

	public override void Load()
	{
		On_CombatText.NewText_Rectangle_Color_string_bool_bool += CombatText_NewText_Rectangle_Color_string_bool_bool;
	}

	private int CombatText_NewText_Rectangle_Color_string_bool_bool(On_CombatText.orig_NewText_Rectangle_Color_string_bool_bool orig, Rectangle location, Color color, string text, bool dramatic, bool dot)
	{
		if (GoldStrike > 0)
		{
			color = new Color(255, 235, 0);
			GoldStrike--;
		}
		return orig(location, color, text, dramatic, dot);
	}
}
