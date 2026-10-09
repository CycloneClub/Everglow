using Terraria.ModLoader.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI;
using Everglow.Food.Buffs;
using Everglow.Food.FoodUtilities;

namespace Everglow.Food;

public class FoodModPlayer : ModPlayer
{
	/// <summary>
	/// 玩家当前饱食度
	/// </summary>
	public int CurrentSatiety
	{
		get; set;
	}

	/// <summary>
	/// 玩家最大饱食度
	/// </summary>
	public int MaximumSatiety
	{
		get; set;
	}
	/// <summary>
	/// 玩家的饱食等级
	/// </summary>
	public int SatietyLevel { get; private set; }
	private int starvationCounter = 0;
	public int StarvationCounter
	{
		get
		{
			return starvationCounter;
		}
		private set
		{
			if (value < 0)
				starvationCounter = 0;
			else
			{
				starvationCounter = value;
			}
		}
	}
	/// <summary>
	/// 玩家当前渴觉状态
	/// </summary>
	public bool Thirstystate
	{
		get; set;
	}
	public FoodModPlayer()
	{
	}

	/// <summary>
	/// 如果能吃下，返回true，否则为false
	/// </summary>
	/// <param name="foodInfo"></param>
	/// <returns></returns>
	public bool CanEat(FoodInfo foodInfo)
	{
		if (CurrentSatiety + foodInfo.Satiety <= MaximumSatiety)
		{
			return true;
		}

		return false;
	}

	/// <summary>
	/// 如果能喝下，返回true，否则为false
	/// </summary>
	public bool CanDrink(DrinkInfo drinkInfo)
	{
		if (Thirstystate)
		{
			return true;
		}

		return false;
	}

	public bool CanText()
	{
		if (TextTimer <= 0)
		{
			return true;
		}

		return false;
	}
	/*
         
         
         */
	/// <summary>
	/// 以下为计时器
	/// </summary>
	public int SatietyLossTimer
	{
		get; private set;
	}// 饱食损失计时器
	public int ThirstyChangeTimer
	{
		get; private set;
	}// 口渴变化计时器
	public int TextTimer
	{
		get; set;
	}
	public override void PostUpdateMiscEffects()
	{
		Player.buffImmune[BuffID.WellFed] = true;
		Player.buffImmune[BuffID.WellFed2] = true;
		Player.buffImmune[BuffID.WellFed3] = true;
		Player.buffImmune[BuffID.Tipsy] = true;
		Player.buffImmune[BuffID.NeutralHunger] = true;
		Player.buffImmune[BuffID.Hunger] = true;
		Player.buffImmune[BuffID.Starving] = true;
		base.PostUpdateMiscEffects();
	}
	public override void PostUpdate()
	{
		FoodState();
		UpdateHungerEmote();
		base.PostUpdate();
	}
	public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
	{
		CurrentSatiety = 0;
		SatietyLevel = 0;
		Thirstystate = true;
		starvationCounter = 0;
		base.Kill(damage, hitDirection, pvp, damageSource);
	}
	public override void Initialize()
	{
		CurrentSatiety = 0;
		MaximumSatiety = 50;
		SatietyLevel = 0;
		SatietyLossTimer = 0;

		Thirstystate = true;
		ThirstyChangeTimer = 0;

		TextTimer = 0;

		base.Initialize();
	}
	public override void SaveData(TagCompound tag)
	{
		tag.Add("CurrentSatiety", CurrentSatiety);
		tag.Add("Thirstystate", Thirstystate);
		tag.Add("StarvationCounter", StarvationCounter);
		base.SaveData(tag);
	}

	public override void LoadData(TagCompound tag)
	{
		if (tag.ContainsKey("CurrentSatiety"))
		{
			CurrentSatiety = tag.GetInt("CurrentSatiety");
		}

		if (tag.ContainsKey("Thirstystate"))
		{
			Thirstystate = tag.GetBool("Thirstystate");
		}

		if (tag.ContainsKey("StarvationCounter"))
		{
			StarvationCounter = tag.GetInt("StarvationCounter");
		}

		base.LoadData(tag);
	}

	public void FoodState()
	{
		// 从吃食物后开始计时
		if (CurrentSatiety > 0)
		{
			SatietyLossTimer++;
			StarvationCounter = 0;
		}
		// 从喝饮料后开始计时
		if (!Thirstystate)
		{
			ThirstyChangeTimer++;
		}

		if (!CanText())
		{
			TextTimer--;
		}

		// 每三十秒减少一饱食度
		if (Player.GetModPlayer<FoodBuffModPlayer>().DurianBuff)
		{
			if (SatietyLossTimer >= FoodUtils.GetFrames(0, 0, 15, 0))
			{
				CurrentSatiety -= 1;
				SatietyLossTimer = 0;
			}
		}
		else
		{
			if (SatietyLossTimer >= FoodUtils.GetFrames(0, 0, 30, 0))
			{
				CurrentSatiety -= 1;
				SatietyLossTimer = 0;
			}
		}

		if (CurrentSatiety <= 0)
		{
			CurrentSatiety = 0;
			StarvationCounter++;

			#region Set satiety level
			if (StarvationCounter > FoodUtils.GetFrames(0, 15, 0, 0)) // starving
				SatietyLevel = -3;
			else if (StarvationCounter > FoodUtils.GetFrames(0, 10, 0, 0)) // hungry
			{
				SatietyLevel = -2;
			}
			else if (StarvationCounter > FoodUtils.GetFrames(0, 5, 0, 0)) // neutrual hungry
			{
				SatietyLevel = -1;
			}
			else // neutural
			{
				SatietyLevel = 0;
			}
			#endregion
		}
		else
		{
			StarvationCounter = 0;

			#region Set satiety level
			if (CurrentSatiety <= MaximumSatiety * 0.5f) // well fed
				SatietyLevel = 1;
			else if (CurrentSatiety > MaximumSatiety * 0.5f && CurrentSatiety <= MaximumSatiety * 0.75f) // plently satisfied
			{
				SatietyLevel = 2;
			}
			else // exquisitely stuffed
			{
				SatietyLevel = 3;
			}
			#endregion
		}
		// 每五分钟从口渴变得不口渴
		if (ThirstyChangeTimer >= FoodUtils.GetFrames(0, 5, 0, 0))
		{
			Thirstystate = true;
			ThirstyChangeTimer = 0;
		}
	}
	public override void PostUpdateBuffs()
	{
		#region Well fed life regen effect
		if (SatietyLevel > 0 || !Thirstystate)
		{
			Player.wellFed = true;
		}
		#endregion

		#region Give effects based on satiety level
		if (SatietyLevel == 1) // well fed
		{
			Player.statDefense += 1;
			Player.GetCritChance(DamageClass.Generic) += 0.01f;
			Player.GetDamage(DamageClass.Generic) += 0.02f;
			Player.GetAttackSpeed(DamageClass.Generic) += 0.02f;
			Player.GetKnockback(DamageClass.Summon) += 0.25f;
			Player.moveSpeed += 0.1f;
			Player.pickSpeed -= 0.1f;
		}
		else if (SatietyLevel == 2) // plently satisfied
		{
			Player.statDefense += 2;
			Player.GetCritChance(DamageClass.Generic) += 0.02f;
			Player.GetDamage(DamageClass.Generic) += 0.04f;
			Player.GetAttackSpeed(DamageClass.Generic) += 0.04f;
			Player.GetKnockback(DamageClass.Summon) += 0.5f;
			Player.moveSpeed += 0.05f;
			Player.pickSpeed -= 0.15f;
		}
		else if (SatietyLevel == 3) // exquisitely stuffed
		{
			SatietyLevel = 3;
			Player.statDefense += 4;
			Player.GetDamage(DamageClass.Generic) += 0.04f;
			Player.GetKnockback(DamageClass.Summon) += 0.75f;
			Player.pickSpeed -= 0.1f;
		}
		else if (SatietyLevel == -1) // neutrual hungry
		{
			if (Main.dontStarveWorld)
			{
				Player.statDefense -= 1;
				Player.GetDamage(DamageClass.Generic) -= 0.02f;
				Player.GetCritChance(DamageClass.Generic) -= 0.01f;
				Player.GetAttackSpeed(DamageClass.Generic) -= 0.02f;
				Player.GetKnockback(DamageClass.Summon) -= 0.3f;
				Player.moveSpeed -= 0.02f;
				Player.pickSpeed += 0.05f;
				if (Player.lifeRegen > 0)
				{
					Player.lifeRegen = 0;
				}
			}
		}
		else if (SatietyLevel == -2) // hungry
		{
			if (Main.dontStarveWorld)
			{
				Player.statDefense -= 3;
				Player.GetDamage(DamageClass.Generic) -= 0.06f;
				Player.GetCritChance(DamageClass.Generic) -= 0.03f;
				Player.GetAttackSpeed(DamageClass.Generic) -= 0.06f;
				Player.GetKnockback(DamageClass.Summon) -= 0.6f;
				Player.moveSpeed -= 0.04f;
				Player.pickSpeed += 0.1f;
				if (Player.lifeRegen > 0)
				{
					Player.lifeRegen = 0;
				}

				Player.lifeRegen -= (int)(Player.statLifeMax2 * 0.01f);
			}
		}
		else if (SatietyLevel == -3) // starving
		{
			if (Main.dontStarveWorld)
			{
				Player.statDefense -= 9;
				Player.GetDamage(DamageClass.Generic) -= 0.18f;
				Player.GetCritChance(DamageClass.Generic) -= 0.09f;
				Player.GetAttackSpeed(DamageClass.Generic) -= 0.18f;
				Player.GetKnockback(DamageClass.Summon) -= 0.9f;
				Player.moveSpeed -= 0.08f;
				Player.pickSpeed += 0.2f;
				if (Player.lifeRegen > 0)
				{
					Player.lifeRegen = 0;
				}

				Player.lifeRegen -= (int)(Player.statLifeMax2 * 0.02f);
				Player.starving = true;
			}
		}
		#endregion
		base.PostUpdateBuffs();
	}
	public void UpdateHungerEmote()
	{
		if (Main.dontStarveWorld)
		{
			if (StarvationCounter == FoodUtils.GetFrames(0, 15, 0, 0)) // starving
				EmoteBubble.MakeLocalPlayerEmote(148);
			else if (StarvationCounter == FoodUtils.GetFrames(0, 10, 0, 0)) // hungry
			{
				EmoteBubble.MakeLocalPlayerEmote(147);
			}
			else if (StarvationCounter == FoodUtils.GetFrames(0, 5, 0, 0)) // neutrual hungry
			{
				EmoteBubble.MakeLocalPlayerEmote(146);
			}
		}
	}
}
