using System.Reflection;
using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.Example.Projectiles;
using ReLogic.Graphics;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace Everglow.Example.Items;

public class ExampleStabbingSword_Config : ModPlayer
{
	public Projectile StabbingSword_Projectile = null;

	public Projectile StabbingSword_Stab_Projectile = null;

	public StabbingProjectile Stabbing_ModProj => StabbingSword_Projectile is null ? null : StabbingSword_Projectile.ModProjectile as StabbingProjectile;

	public StabbingProjectile_Stab Stabbing_ModProj_Stab => StabbingSword_Stab_Projectile is null ? null : StabbingSword_Stab_Projectile.ModProjectile as StabbingProjectile_Stab;

	public StabbingSwordItem Stabbing_ModItem => Player.HeldItem.ModItem is StabbingSwordItem ? Player.HeldItem.ModItem as StabbingSwordItem : null;

	//--------------Projectile Property-------------------------Projectile Property----------------Projectile Property----------------Projectile Property----------------Projectile Property----------------Projectile Property----

	/// <summary>
	/// Default ExtraUpdates | 默认额外刷新次数
	/// </summary>
	public const int NormalExtraUpdates = 20;

	/// <summary>
	/// Main color | 主要颜色
	/// </summary>
	public Color AttackColor = Color.White;

	/// <summary>
	/// Shadow intensity of first attack unit(<see cref="StabbingProjectile.LightAttackEffect"/>) | 首个攻击单元阴影强度
	/// </summary>
	public float CurrentColorFactor = 0.2f;

	/// <summary>
	/// Shadow intensity of old attack units(<see cref="StabbingProjectile.DarkAttackEffect"/>) | 旧攻击单元阴影强度
	/// </summary>
	public float OldColorFactor = 0.7f;

	/// <summary>
	/// Color(RGB) illumination coefficient of old attack units | 旧攻击单元RGB亮度系数
	/// </summary>
	public float OldLightColorValue = 1f;

	/// <summary>
	/// Amount of old attack units (Length of<see cref="StabbingProjectile.DarkAttackEffect"/>), default to 4; Warning : The projectile will keep active until old attack units run out | 最大旧攻击单元数，默认4; 警告：射弹会一直存在直到旧攻击单元耗尽
	/// </summary>
	public int MaxDarkAttackUnitCount = 4;

	/// <summary>
	/// Scale of old attack units will multiply this per update, no more than 1.0f | 旧攻击单元大小每次更新倍率, 不大于1.0f
	/// </summary>
	public float ScaleMultiplicative_Modifier = 1f;

	/// <summary>
	/// Light effect width coefficient of an attack unit， default to 1.0f | 攻击单元刀光宽度系数, 默认1.0f
	/// </summary>
	public float AttackEffectWidth = 0.4f;

	/// <summary>
	/// Shadow intensity of old attack units will multiply this per update, no more than 1.0f | 旧攻击单元阴影强度每次更新倍率, 不大于1.0f
	/// </summary>
	public float ShadeMultiplicative_Modifier = 0.64f;

	/// <summary>
	/// Color(RGB) illumination of old attack units will multiply this per update, no more than 1.0f | 旧攻击单元RGB色彩每次更新倍率, 不大于1.0f
	/// </summary>
	public float LightColorValueMultiplicative_Modifier = 0.4f;

	/// <summary>
	/// Light effect length coefficient of an attack unit， default to 1.0f (1.0 * 72 = 72) | 攻击单元刀光长度系数, 默认1.0f (1.0 * 72 = 72)
	/// </summary>
	public float AttackLength = 0.7f;

	/// <summary>
	/// Glow color, no affected by environment, default to <see cref="Color.Transparent"/> | 荧光颜色，不受环境影响，默认无色
	/// </summary>
	public Color GlowColor = Color.Transparent;

	/// <summary>
	/// Glow color(RGB) of old attack units will multiply this per update, no more than 1.0f | 旧攻击单元荧光颜色每次更新倍率, 不大于1.0f
	/// </summary>
	public float FadeGlowColorValue = 0f;

	//--------------Item Property-------------------------Item Property----------------Item Property----------------Item Property----------------Item Property----------------Item Property----

	/// <summary>
	/// The projectile type of powerful stab.
	/// </summary>
	public int PowerfulStabProj;

	/// <summary>
	/// The multiply zone of powerful stab, default to 400%(4f).
	/// </summary>
	public float PowerfulStabDamageFlat = 4f;

	/// <summary>
	/// Remaining cooling time.
	/// </summary>
	public int CurrentPowerfulStabCD;

	/// <summary>
	/// Cooling time of powerful stab.
	/// </summary>
	public int PowerfulStabCDMax = 30;

	/// <summary>
	/// Stamina deplete per unit of attack.
	/// </summary>
	public float StaminaCost = 1f;

	/// <summary>
	/// Stamina deplete per powerful stab, default to 45.
	/// </summary>
	public float PowerfulStabStaminaCost = 45f;

	//--------------Stab Projectile Property-------------------------Stab Projectile Property----------------Stab Projectile Property----------------Stab Projectile Property----------------Stab Projectile Property----------------Stab Projectile Property----

	/// <summary>
	/// Main color | 主要颜色
	/// </summary>
	public Color StabColor = new Color(80, 34, 5);

	/// <summary>
	/// Shadow intensity | 阴影强度
	/// </summary>
	public float StabShade = 0.4f;

	/// <summary>
	/// Width of stab effect | 特效宽度
	/// </summary>
	public float StabEffectWidth = 1f;

	/// <summary>
	/// Stab range coefficient, multiplied by 80 | 攻击距离系数, 乘以80
	/// </summary>
	public float StabDistance = 1f;

	//--------------Config Panel Property-------------------------Config Panel Property----------------Config Panel Property----------------Config Panel Property----------------Config Panel Property----------------Config Panel Property----
	public int DraggingIndex = -1;

	public Vector2 PanelPos = Vector2.zeroVector;

	private Vector2 oldPanelPos = Vector2.zeroVector;

	private Vector2 oldClickPos = Vector2.zeroVector;

	public override void UpdateEquips()
	{
		foreach (var proj in Main.projectile)
		{
			if (proj is not null && proj.active)
			{
				if (proj.type == ModContent.ProjectileType<ExampleStabbingSword_Pro>())
				{
					if (proj.owner == Player.whoAmI)
					{
						StabbingSword_Projectile = proj;
					}
				}
				if (proj.type == ModContent.ProjectileType<ExampleStabbingSword_Pro_Stab>())
				{
					if (proj.owner == Player.whoAmI)
					{
						StabbingSword_Stab_Projectile = proj;
					}
				}
			}
		}
	}

	public void Draw()
	{
		if (Player == null)
		{
			return;
		}
		if (Player.HeldItem.type != ModContent.ItemType<ExampleStabbingSword>())
		{
			return;
		}
		DrawStringStack("Projectile Configs", -1);
		DrawAndModify(0, 1, ref CurrentColorFactor, 0.2f, nameof(CurrentColorFactor), 0);
		DrawAndModify(0, 1, ref OldColorFactor, 0.7f, nameof(OldColorFactor), 1);
		DrawAndModify(0, 1, ref OldLightColorValue, 1f, nameof(OldLightColorValue), 2);
		DrawAndModify(0, 1, ref ScaleMultiplicative_Modifier, 1f, nameof(ScaleMultiplicative_Modifier), 3);
		DrawAndModify(0, 1, ref AttackEffectWidth, 0.4f, nameof(AttackEffectWidth), 4);
		DrawAndModify(0, 1, ref ShadeMultiplicative_Modifier, 0.64f, nameof(ShadeMultiplicative_Modifier), 5);
		DrawAndModify(0, 1, ref LightColorValueMultiplicative_Modifier, 0.4f, nameof(LightColorValueMultiplicative_Modifier), 6);
		DrawAndModify(0, 1, ref FadeGlowColorValue, 0f, nameof(FadeGlowColorValue), 7);
		DrawAndModify(0.5f, 4, ref AttackLength, 0.7f, nameof(AttackLength), 8);
		DrawAndModify_Int(1, 50, ref MaxDarkAttackUnitCount, 4, nameof(MaxDarkAttackUnitCount), 9);
		DrawAndModify_Color(ref AttackColor, Color.Gray, nameof(AttackColor), 10);
		DrawAndModify_Color(ref GlowColor, Color.Transparent, nameof(GlowColor), 14);

		DrawStringStack("Item Configs", 18);
		Item_DrawAndModify(1f, 10f, ref PowerfulStabDamageFlat, 4f, nameof(PowerfulStabDamageFlat), 19);
		Item_DrawAndModify_Int(6, 90, ref PowerfulStabCDMax, 30, nameof(PowerfulStabCDMax), 20);
		Item_DrawAndModify(0f, 2.5f, ref StaminaCost, 1f, nameof(StaminaCost), 21);
		Item_DrawAndModify(0f, 100f, ref PowerfulStabStaminaCost, 45f, nameof(PowerfulStabStaminaCost), 22);

		DrawStringStack("Stab Configs", 23);
		Stab_DrawAndModify(0, 1f, ref StabShade, 0.4f, nameof(StabShade), 24);
		Stab_DrawAndModify(0f, 2.5f, ref StabEffectWidth, 1f, nameof(StabEffectWidth), 25);
		Stab_DrawAndModify(0f, 3f, ref StabDistance, 1f, nameof(StabDistance), 26);
		Stab_DrawAndModify_Color(ref StabColor, new Color(80, 34, 5), nameof(StabColor), 27);
	}

	public void Stab_DrawAndModify(float minVallue, float maxValue, ref float currentValue, float defaultValue, string valueName, int order)
	{
		DrawSlideBar(minVallue, maxValue, ref currentValue, defaultValue, valueName, order);
		if (Stabbing_ModProj_Stab is not null)
		{
			FieldInfo fi = typeof(StabbingProjectile_Stab).GetField(valueName);
			if (fi != null)
			{
				fi.SetValue(Stabbing_ModProj_Stab, currentValue);
			}
		}
	}

	public void Stab_DrawAndModify_Color(ref Color currentValue, Color defaultValue, string valueName, int order)
	{
		byte colorR = currentValue.R;
		DrawSlideBar_Byte(0, 255, ref colorR, defaultValue.R, valueName + ".R", order);
		byte colorG = currentValue.G;
		DrawSlideBar_Byte(0, 255, ref colorG, defaultValue.G, valueName + ".G", order + 1);
		byte colorB = currentValue.B;
		DrawSlideBar_Byte(0, 255, ref colorB, defaultValue.B, valueName + ".B", order + 2);
		byte colorA = currentValue.A;
		DrawSlideBar_Byte(0, 255, ref colorA, defaultValue.A, valueName + ".A", order + 3);
		Color modifiedColor = new Color(colorR, colorG, colorB, colorA);
		if (currentValue != modifiedColor)
		{
			currentValue = modifiedColor;
			if (Stabbing_ModProj_Stab is not null)
			{
				FieldInfo fi = typeof(StabbingProjectile_Stab).GetField(valueName);
				if (fi != null)
				{
					fi.SetValue(Stabbing_ModProj_Stab, currentValue);
				}
			}
		}
		else
		{
			if (Stabbing_ModProj_Stab is not null)
			{
				FieldInfo fi = typeof(StabbingProjectile_Stab).GetField(valueName);
				if (fi != null && fi.GetValue(Stabbing_ModProj_Stab) is Color color)
				{
					currentValue = color;
				}
			}
		}
	}

	public void Item_DrawAndModify(float minVallue, float maxValue, ref float currentValue, float defaultValue, string valueName, int order)
	{
		DrawSlideBar(minVallue, maxValue, ref currentValue, defaultValue, valueName, order);
		if (Stabbing_ModItem is not null)
		{
			FieldInfo fi = typeof(StabbingSwordItem).GetField(valueName);
			if (fi != null)
			{
				fi.SetValue(Stabbing_ModItem, currentValue);
			}
		}
	}

	public void Item_DrawAndModify_Int(int minVallue, int maxValue, ref int currentValue, int defaultValue, string valueName, int order)
	{
		DrawSlideBar_Int(minVallue, maxValue, ref currentValue, defaultValue, valueName, order);
		if (Stabbing_ModItem is not null)
		{
			FieldInfo fi = typeof(StabbingSwordItem).GetField(valueName);
			if (fi != null)
			{
				fi.SetValue(Stabbing_ModItem, currentValue);
			}
		}
	}

	public void DrawAndModify(float minVallue, float maxValue, ref float currentValue, float defaultValue, string valueName, int order)
	{
		DrawSlideBar(minVallue, maxValue, ref currentValue, defaultValue, valueName, order);
		if (Stabbing_ModProj is not null)
		{
			FieldInfo fi = typeof(StabbingProjectile).GetField(valueName);
			if (fi != null)
			{
				fi.SetValue(Stabbing_ModProj, currentValue);
			}
		}
	}

	public void DrawAndModify_Int(int minVallue, int maxValue, ref int currentValue, int defaultValue, string valueName, int order)
	{
		DrawSlideBar_Int(minVallue, maxValue, ref currentValue, defaultValue, valueName, order);
		if (Stabbing_ModProj is not null)
		{
			FieldInfo fi = typeof(StabbingProjectile).GetField(valueName);
			if (fi != null)
			{
				fi.SetValue(Stabbing_ModProj, currentValue);
			}
		}
	}

	public void DrawAndModify_Byte(byte minVallue, byte maxValue, ref byte currentValue, byte defaultValue, string valueName, int order)
	{
		DrawSlideBar_Byte(minVallue, maxValue, ref currentValue, defaultValue, valueName, order);
	}

	public void DrawAndModify_Color(ref Color currentValue, Color defaultValue, string valueName, int order)
	{
		byte colorR = currentValue.R;
		DrawSlideBar_Byte(0, 255, ref colorR, defaultValue.R, valueName + ".R", order);
		byte colorG = currentValue.G;
		DrawSlideBar_Byte(0, 255, ref colorG, defaultValue.G, valueName + ".G", order + 1);
		byte colorB = currentValue.B;
		DrawSlideBar_Byte(0, 255, ref colorB, defaultValue.B, valueName + ".B", order + 2);
		byte colorA = currentValue.A;
		DrawSlideBar_Byte(0, 255, ref colorA, defaultValue.A, valueName + ".A", order + 3);
		Color modifiedColor = new Color(colorR, colorG, colorB, colorA);
		if (currentValue != modifiedColor)
		{
			currentValue = modifiedColor;
			if (Stabbing_ModProj is not null)
			{
				FieldInfo fi = typeof(StabbingProjectile).GetField(valueName);
				if (fi != null)
				{
					fi.SetValue(Stabbing_ModProj, currentValue);
				}
			}
		}
		else
		{
			if (Stabbing_ModProj is not null)
			{
				FieldInfo fi = typeof(StabbingProjectile).GetField(valueName);
				if (fi != null && fi.GetValue(Stabbing_ModProj) is Color color)
				{
					currentValue = color;
				}
			}
		}
	}

	/// <summary>
	/// process from 0~1.
	/// </summary>
	/// <param name="minVallue"></param>s
	/// <param name="maxValue"></param>
	/// <param name="process"></param>
	/// <param name="value"></param>
	public void AdjustValue(float minVallue, float maxValue, float process, out float value)
	{
		value = minVallue * (1 - process) + maxValue * process;
		if (StabbingSword_Projectile is not null)
		{
			StabbingSword_Projectile.Update(StabbingSword_Projectile.whoAmI);
		}
		if (StabbingSword_Stab_Projectile is not null)
		{
			StabbingSword_Stab_Projectile.Update(StabbingSword_Stab_Projectile.whoAmI);
		}
	}

	public void DrawSlideBar_Int(int minVallue, int maxValue, ref int currentValue, int defaultValue, string valueName, int order)
	{
		float midValue = currentValue;
		DrawSlideBar(minVallue, maxValue, ref midValue, defaultValue, valueName, order);
		currentValue = (int)midValue;
	}

	public void DrawSlideBar_Byte(byte minVallue, byte maxValue, ref byte currentValue, byte defaultValue, string valueName, int order)
	{
		float midValue = currentValue;
		DrawSlideBar(minVallue, maxValue, ref midValue, defaultValue, valueName, order);
		currentValue = (byte)midValue;
	}

	public void DrawSlideBar(float minVallue, float maxValue, ref float currentValue, float defaultValue, string valueName, int order)
	{
		Texture2D track = Commons.ModAsset.SlideTrack_black.Value;
		Texture2D block = Commons.ModAsset.SlideBlock.Value;
		Texture2D defaultCut = Commons.ModAsset.SlideDefaultCut.Value;
		Vector2 pos = Player.Center + new Vector2(-360, -120 + order * 16) + PanelPos;
		Vector2 mouseTransformed = Main.MouseScreen;

		// Panel
		Vector2 panelPos = pos - Main.screenPosition;
		Rectangle panelArea = new Rectangle((int)panelPos.X - 360, (int)panelPos.Y - 8, 520, 16);
		Rectangle panelAreaLeft = new Rectangle((int)panelPos.X - 360, (int)panelPos.Y - 8, 270, 16);
		Rectangle panelAreaRight = new Rectangle((int)panelPos.X - 360 + 470, (int)panelPos.Y - 8, 50, 16);
		Main.spriteBatch.Draw(Terraria.GameContent.TextureAssets.MagicPixel.Value, panelArea, new Color(0.05f, 0.05f, 0.05f, 0.4f));
		if (DraggingIndex == -1 && (panelAreaLeft.Contains(mouseTransformed.ToPoint()) || panelAreaRight.Contains(mouseTransformed.ToPoint())))
		{
			if (Main.mouseLeft && Main.mouseLeftRelease)
			{
				oldClickPos = mouseTransformed;
				oldPanelPos = PanelPos;
				DraggingIndex = -2;
			}
		}
		if (DraggingIndex == -2)
		{
			if (Main.mouseLeft)
			{
				PanelPos = oldPanelPos + mouseTransformed - oldClickPos;
			}
			else
			{
				DraggingIndex = -1;
			}
		}

		mouseTransformed += Main.screenPosition;

		// Slide track
		Main.spriteBatch.Draw(track, pos - Main.screenPosition, null, Color.White, 0, track.Size() * 0.5f, 1f, SpriteEffects.None, 0);

		// Calculate silde block position
		float middleValue = (maxValue + minVallue) / 2f;
		float valueRange = maxValue - minVallue;
		float radius = 75;
		float movePos = (currentValue - middleValue) / valueRange * radius * 2;
		Vector2 blockPos = pos + new Vector2(movePos, 0);
		Rectangle blockRec = new Rectangle((int)blockPos.X - 4, (int)blockPos.Y - 8, 8, 16);

		// Drag the slide block and modify the value
		if (DraggingIndex == -1 && Main.mouseLeft && blockRec.Contains(mouseTransformed.ToPoint()))
		{
			DraggingIndex = order;
		}
		if (DraggingIndex == order)
		{
			blockPos.X = Math.Clamp(mouseTransformed.X, pos.X - radius, pos.X + radius);
			float process = (blockPos.X - pos.X) / radius + 1;
			process *= 0.5f;
			AdjustValue(minVallue, maxValue, process, out currentValue);
			if (Main.mouseLeftRelease && !Main.mouseLeft)
			{
				DraggingIndex = -1;
			}
			Main.instance.MouseText(valueName + " : " + currentValue);
		}

		// Draw deafult value position
		float movePosDefault = (defaultValue - middleValue) / valueRange * radius * 2;
		Vector2 defaultCutPos = pos + new Vector2(movePosDefault, 0);
		Rectangle defaultCutkRec = new Rectangle((int)defaultCutPos.X - 4, (int)defaultCutPos.Y - 8, 8, 16);

		// default cut
		if (defaultCutkRec.Contains(mouseTransformed.ToPoint()))
		{
			Main.spriteBatch.Draw(defaultCut, defaultCutPos - Main.screenPosition, null, Color.Brown, 0, defaultCut.Size() * 0.5f, 1.2f, SpriteEffects.None, 0);
			Main.instance.MouseText(valueName + " default : " + defaultValue);
			if (Main.mouseLeft && Main.mouseLeftRelease)
			{
				currentValue = defaultValue;
			}
		}
		else
		{
			Main.spriteBatch.Draw(defaultCut, defaultCutPos - Main.screenPosition, null, Color.Brown * 0.75f, 0, defaultCut.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		}

		// silde block
		if (blockRec.Contains(mouseTransformed.ToPoint()))
		{
			Main.spriteBatch.Draw(block, blockPos - Main.screenPosition, null, Color.White, 0, block.Size() * 0.5f, 1.2f, SpriteEffects.None, 0);
			Main.instance.MouseText(valueName + " : " + currentValue);
		}
		else
		{
			Main.spriteBatch.Draw(block, blockPos - Main.screenPosition, null, Color.White * 0.75f, 0, block.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		}

		// Text beside silde track.
		Vector2 stringSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, valueName, Vector2.One);
		Main.spriteBatch.DrawString(FontAssets.MouseText.Value, valueName, pos - Main.screenPosition + new Vector2(-radius - 16, -8), Color.White, 0, new Vector2(stringSize.X, 0), 0.75f, SpriteEffects.None, 0);
		Main.spriteBatch.DrawString(FontAssets.MouseText.Value, currentValue.ToString(), pos - Main.screenPosition + new Vector2(radius + 16, -8), Color.White, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
	}

	public void DrawStringStack(string content, int order)
	{
		Vector2 pos = Player.Center + new Vector2(-360, -120 + order * 16) + PanelPos;
		Vector2 mouseTransformed = Main.MouseScreen;

		// Panel
		Vector2 panelPos = pos - Main.screenPosition;
		Rectangle panelArea = new Rectangle((int)panelPos.X - 360, (int)panelPos.Y - 8, 520, 16);
		Rectangle panelAreaLeft = new Rectangle((int)panelPos.X - 360, (int)panelPos.Y - 8, 270, 16);
		Rectangle panelAreaRight = new Rectangle((int)panelPos.X - 360 + 470, (int)panelPos.Y - 8, 50, 16);
		Main.spriteBatch.Draw(Terraria.GameContent.TextureAssets.MagicPixel.Value, panelArea, new Color(0.05f, 0.05f, 0.15f, 0.4f));

		panelArea.Y += panelArea.Height - 1;
		panelArea.Height = 1;
		Main.spriteBatch.Draw(Terraria.GameContent.TextureAssets.MagicPixel.Value, panelArea, new Color(0.75f, 0.75f, 0.75f, 0.4f));
		panelArea.Y -= 16;
		Main.spriteBatch.Draw(Terraria.GameContent.TextureAssets.MagicPixel.Value, panelArea, new Color(0.75f, 0.75f, 0.75f, 0.4f));
		if (DraggingIndex == -1 && (panelAreaLeft.Contains(mouseTransformed.ToPoint()) || panelAreaRight.Contains(mouseTransformed.ToPoint())))
		{
			if (Main.mouseLeft && Main.mouseLeftRelease)
			{
				oldClickPos = mouseTransformed;
				oldPanelPos = PanelPos;
				DraggingIndex = -2;
			}
		}
		if (DraggingIndex == -2)
		{
			if (Main.mouseLeft)
			{
				PanelPos = oldPanelPos + mouseTransformed - oldClickPos;
			}
			else
			{
				DraggingIndex = -1;
			}
		}

		// Text beside silde track.
		Vector2 stringSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, content, Vector2.One);
		Main.spriteBatch.DrawString(FontAssets.MouseText.Value, content, pos - Main.screenPosition + new Vector2(-150, -8), Color.White, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
	}
}
