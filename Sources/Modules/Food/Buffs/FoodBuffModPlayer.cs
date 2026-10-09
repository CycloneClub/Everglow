using Terraria.DataStructures;
using Terraria.Audio;
using Terraria.Localization;
using Everglow.Food.Buffs.VanillaFoodBuffs;

namespace Everglow.Food.Buffs;

public class FoodBuffModPlayer : ModPlayer
{
	public float WingTimeModifier;

	public bool BananaBuff;
	public bool BananaDaiquiriBuff;
	public bool BananaSplitBuff;
	public bool DragonfruitBuff;
	public bool GoldenDelightBuff;
	public bool SmoothieofDarknessBuff;
	public bool GrubSoupBuff;
	public bool MonsterLasagnaBuff;
	public bool SashimiBuff;
	public bool ShuckedOysterBuff;
	public bool MangoBuff;
	public bool StarfruitBuff;
	public bool NachosBuff;
	public bool RoastedBirdBuff;
	public bool RoastedDuckBuff;
	public bool BloodyMoscatoBuff;
	public int BloodyMoscatoHealCount;
	public bool FriedEggBuff;
	public bool CherryBuff;
	public bool PiercoldWindBuff;
	public bool PurpleHooterBuff;
	public bool QuinceMarryBuff;
	public bool RedWineBuff;
	public bool SunriseBuff;
	public bool TricolourBuff;
	public bool BlueHawaiiBuff;
	public bool DreamYearningBuff;
	public bool KiwiJuiceBuff;
	public bool KiwiFruitBuff;
	public bool KiwiIceCreamBuff;
	public bool MangosteenBuff;
	public bool DurianBuff;
	public bool StinkyTofuBuff;
	public bool StrawberryBuff;
	public bool StrawberryIcecreamBuff;
	public bool CaramelPuddingBuff;
	public bool CantaloupeJellyBuff;
	public bool GreenStormBuff;

	public static float CriticalDamage;
	public static float AddCritDamage;

	public override void UpdateDead()
	{
		WingTimeModifier = 1f;

		BananaBuff = false;
		BananaDaiquiriBuff = false;
		BananaSplitBuff = false;
		DragonfruitBuff = false;
		GoldenDelightBuff = false;
		SmoothieofDarknessBuff = false;
		GrubSoupBuff = false;
		MonsterLasagnaBuff = false;
		SashimiBuff = false;
		ShuckedOysterBuff = false;
		MangoBuff = false;
		StarfruitBuff = false;
		NachosBuff = false;
		RoastedBirdBuff = false;
		RoastedDuckBuff = false;
		BloodyMoscatoBuff = false;
		BloodyMoscatoHealCount = 0;
		FriedEggBuff = false;
		CherryBuff = false;
		PiercoldWindBuff = false;
		PurpleHooterBuff = false;
		QuinceMarryBuff = false;
		RedWineBuff = false;
		SunriseBuff = false;
		TricolourBuff = false;
		BlueHawaiiBuff = false;
		DreamYearningBuff = false;
		KiwiJuiceBuff = false;
		KiwiFruitBuff = false;
		KiwiIceCreamBuff = false;
		MangosteenBuff = false;
		DurianBuff = false;
		StinkyTofuBuff = false;
		StrawberryBuff = false;
		StrawberryIcecreamBuff = false;
		CaramelPuddingBuff = false;
		CantaloupeJellyBuff = false;
		GreenStormBuff = false;

		CriticalDamage = 1f;
		AddCritDamage = 0;
	}
	public override void ResetEffects()
	{
		WingTimeModifier = 1f;

		BananaBuff = false;
		BananaDaiquiriBuff = false;
		BananaSplitBuff = false;
		DragonfruitBuff = false;
		GoldenDelightBuff = false;
		SmoothieofDarknessBuff = false;
		GrubSoupBuff = false;
		MonsterLasagnaBuff = false;
		SashimiBuff = false;
		ShuckedOysterBuff = false;
		MangoBuff = false;
		StarfruitBuff = false;
		NachosBuff = false;
		RoastedBirdBuff = false;
		RoastedDuckBuff = false;
		BloodyMoscatoBuff = false;
		BloodyMoscatoHealCount = Math.Max(--BloodyMoscatoHealCount, 0);
		FriedEggBuff = false;
		CherryBuff = false;
		PiercoldWindBuff = false;
		PurpleHooterBuff = false;
		QuinceMarryBuff = false;
		RedWineBuff = false;
		SunriseBuff = false;
		TricolourBuff = false;
		BlueHawaiiBuff = false;
		DreamYearningBuff = false;
		KiwiJuiceBuff = false;
		KiwiFruitBuff = false;
		KiwiIceCreamBuff = false;
		MangosteenBuff = false;
		DurianBuff = false;
		StinkyTofuBuff = false;
		StrawberryBuff = false;
		StrawberryIcecreamBuff = false;
		CaramelPuddingBuff = false;
		CantaloupeJellyBuff = false;
		GreenStormBuff = false;

		CriticalDamage = 1f;
		AddCritDamage = 0;
	}
	public override void PostUpdateBuffs()
	{
		if (StinkyTofuBuff)
		{
			foreach (NPC target in Main.npc)
			{
				if (!target.friendly && Main.rand.NextBool(100) && Player.WithinRange(target.Center, 300))
				{
					target.AddBuff(BuffID.Confused, 600);
				}
			}
		}
		if (RoastedBirdBuff)
		{
			Player.wingTimeMax = (int)(Player.wingTimeMax * WingTimeModifier);
		}

		if (RoastedDuckBuff)
		{
			Player.wingTimeMax = (int)(Player.wingTimeMax * WingTimeModifier);
		}

		var mp = Player.GetModPlayer<EverglowPlayer>();
		if (BananaBuff)
		{
			mp.ammoCost *= 1 - 0.05f;
		}
		if (BananaDaiquiriBuff)
		{
			mp.ammoCost *= 0;
		}
		if (BananaSplitBuff)
		{
			mp.ammoCost *= 1 - 0.05f;
		}
	}

	public override void PostUpdate()
	{
		CriticalDamage += AddCritDamage;
		base.PostUpdate();
	}

	public override void ModifyHurt(ref Player.HurtModifiers modifiers)
	{
		if (Player.whoAmI == Main.myPlayer && SmoothieofDarknessBuff && Main.rand.NextBool(2))
		{
			Player.NinjaDodge();
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (DragonfruitBuff)
		{
			target.AddBuff(BuffID.Oiled, 600);
			target.AddBuff(BuffID.OnFire, 600);
		}
		if (BloodyMoscatoBuff && BloodyMoscatoHealCount <= (Main.hardMode ? 150 : 75))
		{
			Player.HealEffect(2, true);
			Player.statLife += 2;
			BloodyMoscatoHealCount += 2;
		}
	}
	public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
	{
		if (CherryBuff)
		{
			SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode);
			// ScreenShaker Gsplayer = Player.GetModPlayer<ScreenShaker>();
			// Gsplayer.FlyCamPosition = new Vector2(0, 150).RotatedByRandom(6.283);
			ShakerManager.AddShaker(UndirectedShakerInfo.Create(Player.Center, 150));

			float k1 = Math.Clamp(Player.velocity.Length(), 1, 3);
			float k2 = Math.Clamp(Player.velocity.Length(), 6, 10);
			float k0 = 1f / 4 * k2;

			foreach (NPC target in Main.npc)
			{
				float Dis = (target.Center - Player.Center).Length();

				if (Dis < 250)
				{
					if (!target.dontTakeDamage && !target.friendly && target.active)
					{
						target.AddBuff(ModContent.BuffType<CherryBuff>(), 1800);
						Player.ApplyDamageToNPC(target, Math.Max(Player.HeldItem.damage * 4, 120), Math.Max(Player.HeldItem.knockBack * 4, 24), 0, Main.rand.NextBool(22, 33));
					}
				}
			}
			CombatText.NewText(Player.Hitbox, Color.HotPink, Language.GetTextValue("Mods.Everglow.Common.FoodSystem.Khan"));
			Player.ClearBuff(ModContent.BuffType<CherryBuff>());
		}
	}

	public override void UpdateBadLifeRegen()
	{
		if (GrubSoupBuff)
		{
			if (Player.lifeRegen > 0)
			{
				Player.lifeRegen = 0;
			}

			Player.lifeRegenTime = 0;
			Player.lifeRegen -= MangoBuff ? 2 : 4;
		}
		if (MonsterLasagnaBuff)
		{
			if (Player.lifeRegen > 0)
			{
				Player.lifeRegen = 0;
			}

			Player.lifeRegenTime = 0;
			Player.lifeRegen -= MangoBuff ? 3 : 6;
		}
		if (SashimiBuff)
		{
			if (Player.lifeRegen > 0)
			{
				Player.lifeRegen = 0;
			}

			Player.lifeRegenTime = 0;
			Player.lifeRegen -= MangoBuff ? 2 : 4;
		}
		if (ShuckedOysterBuff)
		{
			if (Player.lifeRegen > 0)
			{
				Player.lifeRegen = 0;
			}

			Player.lifeRegenTime = 0;
			Player.lifeRegen -= MangoBuff ? 2 : 4;
		}
	}
}
