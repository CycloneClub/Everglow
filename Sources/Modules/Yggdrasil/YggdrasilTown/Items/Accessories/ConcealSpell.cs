using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;

/// <summary>
/// 静止时每帧充能1点，移动时0.1.
/// 能量达到MaxPower时获得"隐匿"，隐形且免疫下一次攻击（钛金）。
/// 受击后能量清零。
/// </summary>
public class ConcealSpell : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetDefaults()
	{
		Item.width = 32;
		Item.height = 30;
		Item.accessory = true;
		Item.rare = ItemRarityID.Blue;
		Item.value = 4865;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.GetModPlayer<ConcealSpellPlayer>().ConcealSpellEnable = true;
	}
}

public class ConcealSpellPlayer : ModPlayer
{
	public bool ConcealSpellEnable = false;
	public float Power = 0;
	public float MaxPower = 480;

	public override void ResetEffects()
	{
		ConcealSpellEnable = false;
	}

	public override void PostUpdate()
	{
		if (ConcealSpellEnable)
		{
			if (Player.velocity.Length() < 0.05f)
			{
				Power += 1;
			}
			else
			{
				Power += 0.1f;
			}
			if (Power > MaxPower)
			{
				Power = MaxPower;
			}
		}
		base.PostUpdate();
	}

	public override bool FreeDodge(Player.HurtInfo info)
	{
		if (ConcealSpellEnable)
		{
			if (Power >= MaxPower)
			{
				CombatText.NewText(Player.Hitbox, new Color(0.7f, 0.7f, 0.7f, 1f), "Miss");
				Player.immune = true;
				Player.immuneTime = 24;
				Player.noKnockback = true;
				Power = 0;
				return true;
			}
		}
		return base.FreeDodge(info);
	}

	public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
	{
		if (ConcealSpellEnable)
		{
			if (Power > MaxPower - 100)
			{
				float value = (MaxPower - Power) / 100f;
				r *= value;
				g *= value;
				b *= value;
				a *= value;
			}
		}
		base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
	}
}
