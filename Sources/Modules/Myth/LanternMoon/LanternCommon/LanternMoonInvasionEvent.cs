using Everglow.Commons.DataStructures;
using Everglow.Commons.Mechanics.Events;
using Everglow.Myth.LanternMoon.NPCs;
using Everglow.Myth.LanternMoon.NPCs.LanternGhostKing;
using Terraria.GameContent;
using Terraria.Localization;

namespace Everglow.Myth.LanternMoon.LanternCommon;

public class LanternMoonInvasionEvent : ReplicaEvent
{
	public bool Boss15Started = false;

	public bool Boss15Ended = false;

	/// <summary>
	/// Point value for completing the current wave
	/// </summary>
	public int[] ScoreRequireOfWave = new int[40];

	/// <summary>
	/// What enemies will spawn in each wave, and chance.
	/// </summary>
	public List<(int Type, float Chance)>[] WaveEnemiesType = new List<(int, float)>[40];

	/// <summary>
	/// Total point account from the beginning.
	/// </summary>
	public float AccumulatedScore = 0;

	/// <summary>
	/// A Lantern Ghost King.
	/// </summary>
	public NPC Wave15Boss;

	public void Initialization()
	{
		Boss15Started = false;
		Boss15Ended = false;

		// Total: 40 Waves
		ScoreRequireOfWave[0] = 25;
		ScoreRequireOfWave[1] = 40;
		ScoreRequireOfWave[2] = 50;
		ScoreRequireOfWave[3] = 80;
		ScoreRequireOfWave[4] = 100;
		ScoreRequireOfWave[5] = 160;
		ScoreRequireOfWave[6] = 180;
		ScoreRequireOfWave[7] = 200;
		ScoreRequireOfWave[8] = 250;
		ScoreRequireOfWave[9] = 300;
		ScoreRequireOfWave[10] = 375;
		ScoreRequireOfWave[11] = 450;
		ScoreRequireOfWave[12] = 525;
		ScoreRequireOfWave[13] = 675;
		ScoreRequireOfWave[14] = 850;

		ScoreRequireOfWave[15] = 1025;
		ScoreRequireOfWave[16] = 1325;
		ScoreRequireOfWave[17] = 1550;
		ScoreRequireOfWave[18] = 2000;
		ScoreRequireOfWave[19] = 2250;
		ScoreRequireOfWave[20] = 2550;
		ScoreRequireOfWave[21] = 2750;
		ScoreRequireOfWave[22] = 3000;
		ScoreRequireOfWave[23] = 3300;
		ScoreRequireOfWave[24] = 4000;

		ScoreRequireOfWave[25] = 4500;
		ScoreRequireOfWave[26] = 5300;
		ScoreRequireOfWave[27] = 6100;
		ScoreRequireOfWave[28] = 6800;
		ScoreRequireOfWave[29] = 7500;
		ScoreRequireOfWave[30] = 9000;
		ScoreRequireOfWave[31] = 11500;
		ScoreRequireOfWave[32] = 12600;
		ScoreRequireOfWave[33] = 15000;
		ScoreRequireOfWave[34] = 20000;

		ScoreRequireOfWave[35] = 123500;
		ScoreRequireOfWave[36] = 167800;
		ScoreRequireOfWave[37] = 235500;
		ScoreRequireOfWave[38] = 341400;
		ScoreRequireOfWave[39] = 0;

		WaveEnemiesType[0] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f), (ModContent.NPCType<LittleRedPaperFigure>(), 1f) };
		WaveEnemiesType[1] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f), (ModContent.NPCType<LittleRedPaperFigure>(), 1f), (ModContent.NPCType<RedpaperGiant>(), 0.75f) };
		WaveEnemiesType[2] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f), (ModContent.NPCType<CylindricalLantern>(), 0.3f), (ModContent.NPCType<RedpaperGiant>(), 0.75f) };
		WaveEnemiesType[3] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f), (ModContent.NPCType<CylindricalLantern>(), 0.3f), (ModContent.NPCType<BloodLanternGhost>(), 0.25f), (ModContent.NPCType<LittleRedPaperFigure>(), 1f), (ModContent.NPCType<RedpaperGiant>(), 0.75f) };
		WaveEnemiesType[4] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f), (ModContent.NPCType<CylindricalLantern>(), 0.3f), (ModContent.NPCType<BombLantern>(), 0.3f), (ModContent.NPCType<GreenFlameLantern>(), 0.15f), (ModContent.NPCType<LittleRedPaperFigure>(), 1f), (ModContent.NPCType<RedpaperGiant>(), 0.75f) };
		WaveEnemiesType[5] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f), (ModContent.NPCType<CylindricalLantern>(), 0.3f), (ModContent.NPCType<BombLantern>(), 0.3f), (ModContent.NPCType<BloodLanternGhost>(), 0.25f), (ModContent.NPCType<LittleRedPaperFigure>(), 1f), (ModContent.NPCType<RedpaperGiant>(), 0.75f) };
		WaveEnemiesType[6] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f), (ModContent.NPCType<CylindricalLantern>(), 0.3f), (ModContent.NPCType<BombLantern>(), 0.3f), (ModContent.NPCType<LargeBloodLanternGhost>(), 0.2f), (ModContent.NPCType<BloodLanternGhost>(), 0.25f), (ModContent.NPCType<GreenFlameLantern>(), 0.15f), (ModContent.NPCType<LittleRedPaperFigure>(), 1f), (ModContent.NPCType<RedpaperGiant>(), 0.75f) };
		WaveEnemiesType[7] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f), (ModContent.NPCType<CylindricalLantern>(), 0.3f), (ModContent.NPCType<BombLantern>(), 0.3f), (ModContent.NPCType<LargeBloodLanternGhost>(), 0.2f), (ModContent.NPCType<BloodLanternGhost>(), 0.25f), (ModContent.NPCType<LittleRedPaperFigure>(), 1f), (ModContent.NPCType<RedpaperGiant>(), 0.75f) };
		WaveEnemiesType[8] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f), (ModContent.NPCType<BombLantern>(), 0.3f), (ModContent.NPCType<CylindricalLantern>(), 0.3f), (ModContent.NPCType<LargeBloodLanternGhost>(), 0.2f), (ModContent.NPCType<BloodLanternGhost>(), 0.25f), (ModContent.NPCType<GreenFlameLantern>(), 0.15f), (ModContent.NPCType<RedpaperGiant>(), 0.75f) };
		WaveEnemiesType[9] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f), (ModContent.NPCType<BombLantern>(), 0.3f), (ModContent.NPCType<CylindricalLantern>(), 0.3f), (ModContent.NPCType<LargeBloodLanternGhost>(), 0.2f), (ModContent.NPCType<BloodLanternGhost>(), 0.25f), (ModContent.NPCType<GreenFlameLantern>(), 0.15f), (ModContent.NPCType<LittleRedPaperFigure>(), 1f), (ModContent.NPCType<RedpaperGiant>(), 0.75f) };
		WaveEnemiesType[10] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f), (ModContent.NPCType<BombLantern>(), 0.3f), (ModContent.NPCType<CylindricalLantern>(), 0.3f), (ModContent.NPCType<LargeBloodLanternGhost>(), 0.2f), (ModContent.NPCType<BloodLanternGhost>(), 0.25f), (ModContent.NPCType<GreenFlameLantern>(), 0.15f), (ModContent.NPCType<WizardLantern>(), 0.15f), (ModContent.NPCType<LittleRedPaperFigure>(), 1f), (ModContent.NPCType<RedpaperGiant>(), 0.75f) };
		WaveEnemiesType[11] = new List<(int, float)>() { (ModContent.NPCType<BombLantern>(), 0.3f), (ModContent.NPCType<CylindricalLantern>(), 0.3f), (ModContent.NPCType<LargeBloodLanternGhost>(), 0.2f), (ModContent.NPCType<BloodLanternGhost>(), 0.25f), (ModContent.NPCType<GreenFlameLantern>(), 0.15f), (ModContent.NPCType<WizardLantern>(), 0.15f), (ModContent.NPCType<LittleRedPaperFigure>(), 1f), (ModContent.NPCType<RedpaperGiant>(), 0.75f) };
		WaveEnemiesType[12] = new List<(int, float)>() { (ModContent.NPCType<BombLantern>(), 0.3f), (ModContent.NPCType<CylindricalLantern>(), 0.3f), (ModContent.NPCType<LargeBloodLanternGhost>(), 0.2f), (ModContent.NPCType<BloodLanternGhost>(), 0.25f), (ModContent.NPCType<GreenFlameLantern>(), 0.15f), (ModContent.NPCType<WizardLantern>(), 0.15f), (ModContent.NPCType<RedpaperGiant>(), 0.75f) };
		WaveEnemiesType[13] = new List<(int, float)>() { (ModContent.NPCType<BombLantern>(), 0.3f), (ModContent.NPCType<CylindricalLantern>(), 0.3f), (ModContent.NPCType<LargeBloodLanternGhost>(), 0.2f), (ModContent.NPCType<BloodLanternGhost>(), 0.25f), (ModContent.NPCType<GreenFlameLantern>(), 0.15f), (ModContent.NPCType<WizardLantern>(), 0.15f) };
		WaveEnemiesType[14] = new List<(int, float)>() { (ModContent.NPCType<LanternGhostKing>(), 0) };
		WaveEnemiesType[15] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f) };
		WaveEnemiesType[16] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f) };
		WaveEnemiesType[17] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f) };
		WaveEnemiesType[18] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f) };
		WaveEnemiesType[19] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f) };
		WaveEnemiesType[20] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f) };
		WaveEnemiesType[21] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f) };
		WaveEnemiesType[22] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f) };
		WaveEnemiesType[23] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f) };
		WaveEnemiesType[24] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f) };
		WaveEnemiesType[25] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f) };
		WaveEnemiesType[26] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f) };
		WaveEnemiesType[27] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f) };
		WaveEnemiesType[28] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f) };
		WaveEnemiesType[29] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f) };
		WaveEnemiesType[30] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f) };
		WaveEnemiesType[31] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f) };
		WaveEnemiesType[32] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f) };
		WaveEnemiesType[33] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f) };
		WaveEnemiesType[34] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f) };
		WaveEnemiesType[35] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f) };
		WaveEnemiesType[36] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f) };
		WaveEnemiesType[37] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f) };
		WaveEnemiesType[38] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f) };
		WaveEnemiesType[39] = new List<(int, float)>() { (ModContent.NPCType<EvilLantern>(), 1f) };

		var messageColor = new Color(175, 75, 255);
		Color messageColor1 = Color.PaleGreen;
		Main.NewText(Language.GetTextValue("Lantern Moon is raising..."), messageColor1);
		Wave = 1;
		Main.NewText(Language.GetTextValue("Wave 1:" + GetWaveEnemiesMessage()), messageColor);
		ProgressMax = ScoreRequireOfWave[0];
		ProgressAlpha = 0f;
		AccumulatedScore = 0;
		Icon = ModAsset.LanternMoonIcon.Value;
	}

	public override bool CanActivate(params object[] args)
	{
		return !Main.dayTime && !Main.snowMoon && !Main.pumpkinMoon && base.CanActivate();
	}

	public override void OnActivate(params object[] args)
	{
		Initialization();
		base.OnActivate();
	}

	/// <summary>
	/// Disable vanilla music, use empty ogg instead.
	/// </summary>
	/// <returns></returns>
	public int SwitchMusic()
	{
		return MusicLoader.GetMusicSlot(ModAsset.SilentOGG_Mod);
	}

	public void StartMusic()
	{
		LanternMoonMusicManager musicSystem = ModContent.GetInstance<LanternMoonMusicManager>();
		musicSystem.Wave15StartTimer = 360000;
		for (int i = 0; i < musicSystem.CustomMusicCues.Count; i++)
		{
			musicSystem.FadeMusic(musicSystem.CustomMusicCues[i], 120);
		}
		if (Wave <= 14)
		{
			musicSystem.StartMusic(ModAsset.LanternMoonMusic_Pre15_Accompaniment_Head_Mod, 3, false);
			musicSystem.StartMusic(ModAsset.LanternMoonMusic_Pre15_Percussion_Head_Mod, 3, false);
		}
		else if (Wave == 15)
		{
			musicSystem.StartMusic(ModAsset.LanternMoonMusic_15_Accompaniment_Loop_Mod, 3, true);
			musicSystem.StartMusic(ModAsset.LanternMoonMusic_15_Melody_Loop_Mod, 3, true);
			musicSystem.StartMusic(ModAsset.LanternMoonMusic_15_Percussion_Loop_Mod, 3, true);
		}
	}

	public void StartWave15Music()
	{
		LanternMoonMusicManager musicSystem = ModContent.GetInstance<LanternMoonMusicManager>();
		musicSystem.Wave15StartTimer = 0;
		musicSystem.FadeAllTheCurrentMusic(120);
	}

	public void EndMusic()
	{
		LanternMoonMusicManager musicSystem = ModContent.GetInstance<LanternMoonMusicManager>();
		for (int i = 0; i < musicSystem.CustomMusicCues.Count; i++)
		{
			var inst = musicSystem.CustomMusicCues[i];
			var track = inst.Track;
			if (track != null)
			{
				inst.Fade = true;
				inst.FadeValue = -1f / 300;
				inst.Track = track;
			}
			musicSystem.CustomMusicCues[i] = inst;
		}
		musicSystem.Wave15StartTimer = 360000;
	}

	public bool ShouldReinitialize()
	{
		bool flag0 = Wave == 0 || ScoreRequireOfWave[0] == 0 || Icon == null;
		bool flag1 = Active;
		return flag0 && flag1;
	}

	public override void PostUpdateEverything()
	{
		LanternMoonMusicManager musicSystem = ModContent.GetInstance<LanternMoonMusicManager>();
		if (musicSystem is not null && musicSystem.CustomMusicCues.Count <= 0 && innerActive)
		{
			StartMusic();
		}
		if (Main.dayTime)
		{
			innerActive = false;
			EndMusic();
		}
		if (ShouldReinitialize())
		{
			Initialization();
		}
		Progress = WaveScore();
		if (Progress > ScoreRequireOfWave[Wave - 1])
		{
			NewWave();
		}
		if (Wave == 15 && Active)
		{
			UpdateWave15();
		}
	}

	public override void ModifyInvasionProgress(ref string text, ref Color c)
	{
		text = "Lantern Moon";
		base.ModifyInvasionProgress(ref text, ref c);
	}

	public string GetWaveEnemiesMessage()
	{
		if (WaveEnemiesType[Wave - 1] is not null)
		{
			string message = string.Empty;
			foreach (var npcTypeAndChance in WaveEnemiesType[Wave - 1])
			{
				int npcType = npcTypeAndChance.Type;
				string npcName = Lang.GetNPCNameValue(npcType);
				if (message != string.Empty)
				{
					message += ", ";
				}
				message += npcName;
			}
			return message;
		}
		return string.Empty;
	}

	public float WaveScore()
	{
		float score = AccumulatedScore - ScoreRequireOfWave.Take(Wave - 1).Sum();
		return score;
	}

	public void AddPoint(float value)
	{
		if (Wave == 15 || Wave == 25)
		{
			return;
		}
		float mulValue = 1;
		if (Main.expertMode)
		{
			mulValue = 2;
		}
		if (Main.masterMode)
		{
			mulValue = 2.5f;
		}
		AccumulatedScore += value * mulValue;
	}

	public void NewWave()
	{
		Main.NewText("Wave " + (Wave + 1) + ":" + GetWaveEnemiesMessage(), new Color(175, 75, 255));
		Wave++;
		ProgressMax = ScoreRequireOfWave[Wave - 1];
	}

	public void UpdateWave15()
	{
		if ((Wave15Boss == null || !Wave15Boss.active || Wave15Boss.type != ModContent.NPCType<LanternGhostKing>()) && innerActive)
		{
			int x0 = (int)(Main.screenPosition.X - Main.offScreenRange - 150);
			int x1 = (int)(Main.screenPosition.X + Main.screenWidth + Main.offScreenRange + 150);
			int y0 = (int)(Main.screenPosition.Y + Main.screenHeight * 0.5f);

			Wave15Boss = NPC.NewNPCDirect(NPC.GetSource_NaturalSpawn(), Main.rand.NextBool(2) ? x0 : x1, y0, ModContent.NPCType<LanternGhostKing>());
		}
		else
		{
			if (Wave15Boss.type != ModContent.NPCType<LanternGhostKing>() || !Wave15Boss.active)
			{
				foreach (var npc in Main.npc)
				{
					if (npc != null && npc.active && npc.type == ModContent.NPCType<LanternGhostKing>())
					{
						Wave15Boss = npc;
						break;
					}
				}
			}
			else
			{
				int waveRequire = ScoreRequireOfWave[Wave - 1];
				if (Wave15Boss.active)
				{
					AccumulatedScore = ScoreRequireOfWave.Take(14).Sum() + waveRequire - (int)(Wave15Boss.life / (float)Wave15Boss.lifeMax * waveRequire);
				}
			}
		}
	}

	public override void Draw(SpriteBatch sprite)
	{
		if (ShouldReinitialize())
		{
			Initialization();
		}
		DrawProgressBar(sprite);
	}

	public void DrawProgressBar(SpriteBatch spriteBatch)
	{
		if (innerActive)
		{
			UIBarFadeTimer = 160;
		}
		if (!Main.gamePaused && UIBarFadeTimer > 0)
		{
			UIBarFadeTimer--;
		}
		ProgressAlpha += UIBarFadeTimer > 0 ? 0.05f : -0.05f;
		ProgressAlpha = Math.Clamp(ProgressAlpha, 0, 1);
		if (ProgressAlpha <= 0f)
		{
			EventSystem.Deactivate(this);
			return;
		}
		using var scope = (SpriteBatchState.Immediate with { TransformMatrix = Main.UIScaleMatrix }).BeginScope(spriteBatch);
		float num = 0.5f + ProgressAlpha * 0.5f;
		string text = string.Empty;
		Color c = Color.White;

		ModifyInvasionProgress(ref text, ref c);
		Vector2 texturescale = new(25.6f / Icon.Width, 25.6f / Icon.Height);
		texturescale *= num;
		if (Wave > 0)
		{
			int num2 = (int)(200f * num);
			int num3 = (int)(45f * num);
			Vector2 vector = new Vector2(Main.screenWidth - 120, Main.screenHeight - 40);
			Rectangle r4 = new((int)vector.X - num2 / 2, (int)vector.Y - num3 / 2, num2, num3);
			Utils.DrawInvBG(spriteBatch, r4, new Color(63, 65, 151, 255) * 0.785f);
			string key = "Game.WaveMessage";
			object arg = ProgressMax != 0 ? (Progress / ProgressMax).ToString("##.##%") : Language.GetTextValue("Game.InvasionPoints", Progress);
			string text2 = Language.GetTextValue(key, Wave, arg);
			Texture2D value2 = TextureAssets.ColorBar.Value;
			float num4 = MathHelper.Clamp(Progress / ProgressMax, 0f, 1f);
			if (ProgressMax == 0)
			{
				num4 = 1f;
			}
			float num5 = 169f * num;
			float num6 = 8f * num;
			Vector2 vector2 = vector + Vector2.UnitY * num6 + Vector2.UnitX * 1f;
			Utils.DrawBorderString(spriteBatch, text2, vector2, Color.White * ProgressAlpha, num, 0.5f, 1f, -1);
			spriteBatch.Draw(value2, vector, null, Color.White * ProgressAlpha, 0f, new Vector2(value2.Width / 2, 0f), num, SpriteEffects.None, 0f);
			vector2 += Vector2.UnitX * (num4 - 0.5f) * num5;
			spriteBatch.Draw(TextureAssets.MagicPixel.Value, vector2, new Rectangle?(new Rectangle(0, 0, 1, 1)), new Color(255, 241, 51) * ProgressAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(num5 * num4, num6), SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureAssets.MagicPixel.Value, vector2, new Rectangle?(new Rectangle(0, 0, 1, 1)), new Color(255, 165, 0, 127) * ProgressAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(2f, num6), SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureAssets.MagicPixel.Value, vector2, new Rectangle?(new Rectangle(0, 0, 1, 1)), Color.Black * ProgressAlpha, 0f, new Vector2(0f, 0.5f), new Vector2(num5 * (1f - num4), num6), SpriteEffects.None, 0f);
		}
		else
		{
			int num7 = (int)(200f * num);
			int num8 = (int)(45f * num);
			Vector2 vector3 = new(Main.screenWidth - 120, Main.screenHeight - 40);
			Rectangle r4 = new((int)vector3.X - num7 / 2, (int)vector3.Y - num8 / 2, num7, num8);
			Utils.DrawInvBG(spriteBatch, r4, new Color(63, 65, 151, 255) * 0.785f);
			string text3 = ProgressMax != 0 ? (Progress / ProgressMax).ToString("##.##%") : Progress.ToString();
			text3 = Language.GetTextValue("Game.WaveCleared", text3);
			Texture2D value3 = TextureAssets.ColorBar.Value;
			if (ProgressMax != 0)
			{
				spriteBatch.Draw(value3, vector3, null, Color.White * ProgressAlpha, 0f, new Vector2(value3.Width / 2, 0f), num, SpriteEffects.None, 0f);
				float num9 = MathHelper.Clamp(Progress / ProgressMax, 0f, 1f);
				Vector2 vector4 = FontAssets.MouseText.Value.MeasureString(text3);
				float num10 = num;
				if (vector4.Y > 22f)
				{
					num10 *= 22f / vector4.Y;
				}
				float num11 = 169f * num;
				float num12 = 8f * num;
				Vector2 vector5 = vector3 + Vector2.UnitY * num12 + Vector2.UnitX * 1f;
				Utils.DrawBorderString(spriteBatch, text3, vector5 + new Vector2(0f, -4f), Color.White * ProgressAlpha, num10, 0.5f, 1f, -1);
				vector5 += Vector2.UnitX * (num9 - 0.5f) * num11;
				spriteBatch.Draw(TextureAssets.MagicPixel.Value, vector5, new Rectangle?(new Rectangle(0, 0, 1, 1)), new Color(255, 241, 51) * ProgressAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(num11 * num9, num12), SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureAssets.MagicPixel.Value, vector5, new Rectangle?(new Rectangle(0, 0, 1, 1)), new Color(255, 165, 0, 127) * ProgressAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(2f, num12), SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureAssets.MagicPixel.Value, vector5, new Rectangle?(new Rectangle(0, 0, 1, 1)), Color.Black * ProgressAlpha, 0f, new Vector2(0f, 0.5f), new Vector2(num11 * (1f - num9), num12), SpriteEffects.None, 0f);
			}
		}
		Vector2 value4 = FontAssets.MouseText.Value.MeasureString(text);
		float num13 = 120f;
		if (value4.X > 200f)
		{
			num13 += value4.X - 200f;
		}
		Rectangle r3 = Utils.CenteredRectangle(new Vector2(Main.screenWidth - num13, Main.screenHeight - 80), (value4 + new Vector2(Icon.Width + 12, 6f)) * num);
		Utils.DrawInvBG(spriteBatch, r3, c);
		spriteBatch.Draw(Icon, r3.Left() + Vector2.UnitX * num * 8f, null, Color.White * ProgressAlpha, 0f, new Vector2(0f, Icon.Height / 2), texturescale, SpriteEffects.None, 0f);
		Utils.DrawBorderString(spriteBatch, text, r3.Right() + Vector2.UnitX * num * -22f, Color.White * ProgressAlpha, num * 0.9f, 1f, 0.4f, -1);
	}
}
