using Everglow.Commons.Utilities;

namespace Everglow.Commons.Templates.Weapons.StabbingSwords;

public class StabbingSwordStaminaPlayer : ModPlayer
{
	public const int RecoveryCooldownDefault = 45;
	public const int MaxStaminaDefault = 100;

	private float stamina = MaxStaminaDefault;
	private float drawAlpha = 0f;
	private int drawAlphaCD = 0;

	/// <summary>
	/// Initialize the original recovery delay after cooldown registration is ready.
	/// </summary>
	private bool recoveryCooldownInitialized;

	private int MaxStamina => MaxStaminaDefault + ExtraStamina;

	/// <summary>
	/// Extra stamina, usually provided by buffs or equipments.
	/// </summary>
	public int ExtraStamina = 0;

	/// <summary>
	/// Is recovery stamina.
	/// </summary>
	public bool StaminaRecovering = false;

	/// <summary>
	/// The speed of stamina recovering.
	/// </summary>
	public float StaminaRecoverySpeed => (MaxStamina + ExtraStamina) / 100f * StaminaRecoveryBonus;

	/// <summary>
	/// The final mutiplier of <see cref="StaminaRecoverySpeed"/>, default to 1.
	/// <br/>eg: <c>StaminaRecoverySpeed += 0.05f</c> provide 5% increased recovery speed.
	/// </summary>
	public float StaminaRecoveryBonus = 1f;

	/// <summary>
	/// The mutiplier of stamina depletion, default to 1.
	/// <br/>Use mutiplication to apply each effect to avoid depletion be cut to 0.
	/// <br/>eg: <c>StaminaDepletionBonus *= 0.75f</c> provide 25% decreased depletion.
	/// </summary>
	public float StaminaDepletionBonus = 1f;

	public override void ResetEffects()
	{
		StaminaRecoveryBonus = 1f;
		StaminaDepletionBonus = 1f;
		ExtraStamina = 0;
	}

	/// <summary>
	/// 检测并扣除耐力值
	/// </summary>
	/// <param name="amount"></param>
	/// <param name="pay"></param>
	/// <returns></returns>
	public bool CheckStamina(float amount, bool pay = true)
	{
		if (StaminaRecovering)
		{
			return false;
		}

		amount *= StaminaDepletionBonus;
		if (stamina > amount)
		{
			if (pay)
			{
				stamina -= amount;
			}
		}
		else
		{
			stamina = 0;
		}
		return true;
	}

	public override void PostUpdate()
	{
		if (!recoveryCooldownInitialized)
		{
			Player.AddCooldown(StabbingSwordRecoveryCooldown.ID, RecoveryCooldownDefault, overwrite: false);
			recoveryCooldownInitialized = true;
		}

		// Start Recovery Process
		if (stamina <= 0 && !StaminaRecovering)
		{
			stamina = 0;
			StaminaRecovering = true;
		}
		if (StaminaRecovering && !Player.HasCooldown<StabbingSwordRecoveryCooldown>())
		{
			stamina += StaminaRecoverySpeed;
			if (stamina > MaxStamina)
			{
				StaminaRecovering = false;
				stamina = MaxStamina;
			}
		}
		if (Player.itemAnimation == 0)
		{
			if (stamina < MaxStamina)
			{
				if (!Player.HasCooldown<StabbingSwordRecoveryCooldown>())
				{
					stamina += StaminaRecoverySpeed / 2f;
				}
				else
				{
					Player.TickCooldown(StabbingSwordRecoveryCooldown.ID);
				}
			}
			else
			{
				stamina = MaxStamina;
			}
		}
		else
		{
			if (Player.GetCooldown(StabbingSwordRecoveryCooldown.ID)?.timeLeft != RecoveryCooldownDefault)
			{
				Player.AddCooldown(StabbingSwordRecoveryCooldown.ID, RecoveryCooldownDefault);
			}
		}

		if (stamina != MaxStamina)
		{
			drawAlpha = MathHelper.Lerp(drawAlpha, 1f, 0.2f);
			drawAlphaCD = 0;
		}
		else
		{
			if (drawAlphaCD > 0)
			{
				drawAlphaCD--;
			}
			else
			{
				drawAlpha = MathHelper.Lerp(drawAlpha, 0f, 0.08f);
			}
		}
	}

	public void DrawStamina(Vector2 center)
	{
		if (drawAlpha <= 0.001)
		{
			return;
		}

		Texture2D tex_dark = ModAsset.StabbingSwordStamina_black.Value;
		Texture2D tex = ModAsset.StabbingSwordStamina.Value;
		Texture2D tex_bloom = ModAsset.StabbingSwordStamina_bloom.Value;
		SpriteEffects spriteEffects = SpriteEffects.None;
		if (Player.gravDir == -1)
		{
			spriteEffects = SpriteEffects.FlipVertically;
		}
		float value = stamina / MaxStamina;
		Color color = Main.hslToRgb(value * 0.36f - 0.15f, 1f, 0.75f);
		color.A = 0;
		float alpha = drawAlpha * (StaminaRecovering ? 0.4f : 0.8f);
		int height = (int)(tex.Height * value);
		int height2 = height + 2;
		if (height <= 1)
		{
			height2 = 0;
		}
		if (height2 > tex.Height)
		{
			height2 = tex.Height;
		}
		Vector2 drawPos = center - Main.screenPosition;
		Rectangle frame = new Rectangle(0, tex.Height - height, tex.Width, height);
		Rectangle frame2 = new Rectangle(0, tex.Height - height2, tex.Width, height2);
		Main.spriteBatch.Draw(tex_dark, drawPos, null, Color.White * alpha, 0, new Vector2(tex.Width * 0.5f, tex.Height), 1f, spriteEffects, 0);
		if (Player.gravDir == 1)
		{
			Main.spriteBatch.Draw(tex, drawPos + new Vector2(0, tex.Height - height2), frame2, color * alpha * 3, 0, new Vector2(tex.Width * 0.5f, tex.Height), 1f, spriteEffects, 0);
			Main.spriteBatch.Draw(tex_dark, drawPos + new Vector2(0, tex.Height - height), frame, Color.White * alpha, 0, new Vector2(tex.Width * 0.5f, tex.Height), 1f, spriteEffects, 0);
			Main.spriteBatch.Draw(tex, drawPos + new Vector2(0, tex.Height - height), frame, color * alpha * 0.7f, 0, new Vector2(tex.Width * 0.5f, tex.Height), 1f, spriteEffects, 0);
		}
		else
		{
			Main.spriteBatch.Draw(tex, drawPos + new Vector2(0, tex.Height - tex.Height), frame2, color * alpha * 3, 0, new Vector2(tex.Width * 0.5f, tex.Height), 1f, spriteEffects, 0);
			Main.spriteBatch.Draw(tex_dark, drawPos + new Vector2(0, tex.Height - tex.Height), frame, Color.White * alpha, 0, new Vector2(tex.Width * 0.5f, tex.Height), 1f, spriteEffects, 0);
			Main.spriteBatch.Draw(tex, drawPos + new Vector2(0, tex.Height - tex.Height), frame, color * alpha * 0.7f, 0, new Vector2(tex.Width * 0.5f, tex.Height), 1f, spriteEffects, 0);
		}
		Main.spriteBatch.Draw(tex_bloom, drawPos + new Vector2(0, 45), null, color * alpha * 0.4f, 0, new Vector2(tex_bloom.Width * 0.5f, tex_bloom.Height), 1f, spriteEffects, 0);
		if (stamina == MaxStamina)
		{
			Main.spriteBatch.Draw(tex_bloom, drawPos + new Vector2(0, 45), null, color * alpha * 0.6f, 0, new Vector2(tex_bloom.Width * 0.5f, tex_bloom.Height), 1f, spriteEffects, 0);
		}
		Vector2 topLeft = drawPos - new Vector2(tex.Width * 0.5f, tex.Height);
		Rectangle drawArea = new Rectangle((int)topLeft.X, (int)topLeft.Y, tex.Width, tex.Height);
		Vector2 mouseScreen = Vector2.Transform(Main.MouseScreen, Matrix.Invert(Main.GameViewMatrix.TransformationMatrix));
		if (drawArea.Contains((int)mouseScreen.X, (int)mouseScreen.Y))
		{
			Main.instance.MouseText("Stamina: " + (int)stamina + "/" + MaxStamina, ItemRarityID.White);
		}
	}
}
