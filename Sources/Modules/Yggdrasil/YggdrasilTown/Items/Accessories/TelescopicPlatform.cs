using Terraria.GameInput;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;

public abstract class TelescopicPlatform : ModItem
{
	/// <summary>
	/// 升降平台最多升高多高
	/// </summary>
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	/// <summary>
	/// 升降平台最多升高多高
	/// </summary>
	public int MaxHeight;

	/// <summary>
	/// 升降平台下面的支撑有多少节
	/// </summary>
	public int PillarCount;

	/// <summary>
	/// 平台的上升与下降速度
	/// </summary>
	public int MoveSpeed;

	// 分别为平台的区域，前支柱的区域，后支柱的区域，贴图上的支柱应该倾斜45度
	public Rectangle BodyRect;
	public Rectangle PillarFrontRect;
	public Rectangle PillarBackRect;
	public float BodyDrawOffsetY;

	public Texture2D Texture2;

	public Texture2D Texture_Glow;

	public override void SetDefaults()
	{
		Item.accessory = true;
	}

	public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
	{
		return
			(incomingItem.ModItem is not TelescopicPlatform && equippedItem.ModItem is TelescopicPlatform) ||
			(incomingItem.ModItem is TelescopicPlatform && equippedItem.ModItem is not TelescopicPlatform);
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		var modPlayer = player.GetModPlayer<TelescopePlatformPlayer>();
		if (modPlayer != null)
		{
			modPlayer.Platform = this;
		}
	}

	public virtual void AddLight()
	{
	}
}

public class TelescopePlatformPlayer : ModPlayer
{
	public TelescopicPlatform Platform;

	/// <summary>
	/// 当前升高到了多高的位置
	/// </summary>
	public int NowHeight = 0;

	public Vector2 PlatformBasePos;

	public Vector2 PlatformBaseBottom;
	public bool InPlatform = false;

	public int OldPlatformType;

	public override void ResetEffects()
	{
		if (InPlatform)
		{
			if (Platform != null)
			{
				NowHeight = Math.Clamp(NowHeight, 0, Platform.MaxHeight);
			}
			Player.gravity = 0;
			Player.noFallDmg = true;
		}
		Platform = null;
	}

	public override void PostUpdate()
	{
		if (Platform == null)
		{
			DeactivatePlatform();
			return;
		}
		if (InPlatform)
		{
			Player.gfxOffY = 0;
			Player.velocity *= 0;
		}

		// 这里不加24的话会有奇怪的 bug 发生，在非常靠近地面的时候角色会飞起来一段距离
		if (Player.velocity.Length() != 0 && InPlatform)
		{
			DeactivatePlatform();
			return;
		}
		if (NowHeight != 0)
		{
			Player.position = new Vector2(0, -NowHeight - 24) + PlatformBasePos;
		}
		else
		{
			DeactivatePlatform();
		}
		if (OldPlatformType != 0 && OldPlatformType != Platform.Type)
		{
			DeactivatePlatform();
		}
		OldPlatformType = Platform.Type;
	}

	public void ActivatePlatform()
	{
		// Visual effects
		if (!InPlatform)
		{
			PlayerSmoke();
			TelescopicPlatformVFX tPlatformVFX = new TelescopicPlatformVFX
			{
				Active = true,
				Visible = true,
				PlatformTexture = Platform.Texture2,
				FrontPillarFrame = Platform.PillarFrontRect,
				BackPillarFrame = Platform.PillarBackRect,
				PlatformFrame = Platform.BodyRect,
				CurrentHeight = NowHeight,
				PillarCount = Platform.PillarCount,
				BodyDrawOffsetY = (int)Platform.BodyDrawOffsetY,
			};
			if (Platform.Texture_Glow != null)
			{
				tPlatformVFX.PlatformTexture_Glow = Platform.Texture_Glow;
				tPlatformVFX.AddLight += Platform.AddLight;
			}
			Ins.VFXManager.Add(tPlatformVFX);
		}

		InPlatform = true;
		PlatformBaseBottom = Player.Bottom;
	}

	public void PlayerSmoke()
	{
		for (int i = 0; i < 16; i++)
		{
			int type;
			switch (Main.rand.Next(3))
			{
				case 0:
					type = GoreID.Smoke1;
					break;
				case 1:
					type = GoreID.Smoke2;
					break;
				case 2:
					type = GoreID.Smoke3;
					break;
				default:
					type = GoreID.ChimneySmoke1;
					break;
			}
			float scale = Main.rand.NextFloat(1f, 2f);
			Vector2 vel = new Vector2(MathF.Sqrt(Main.rand.NextFloat()), 0).RotatedByRandom(MathHelper.TwoPi);
			var gore = Gore.NewGorePerfect(Player.Center + vel * 30 - new Vector2(18), vel * 2f, type, scale);
			gore.timeLeft = Main.rand.Next(30, 60);
		}
	}

	public bool CanActivatePlatform()
	{
		// Check there is no gravitation buff or the triggersSet will conflict.
		// Check there is no mount.
		// Check player is stand in a flat ground.
		if (Player.HasBuff(BuffID.Gravitation) || Player.mount.Active)
		{
			return false;
		}
		return TileSafe(Player.Bottom);
	}

	public bool TileSafe(Vector2 position)
	{
		Vector2 checkTilePos = position;
		for (int i = -24; i <= 24; i += 16)
		{
			if (!Collision.IsWorldPointSolid(checkTilePos + new Vector2(i, 4), true))
			{
				return false;
			}
			if (Collision.IsWorldPointSolid(checkTilePos + new Vector2(i, -4)))
			{
				return false;
			}
		}
		return true;
	}

	public void DeactivatePlatform()
	{
		if (InPlatform)
		{
			PlayerSmoke();
			for (int j = 0; j <= NowHeight; j += 16)
			{
				int type;
				switch (Main.rand.Next(3))
				{
					case 0:
						type = GoreID.Smoke1;
						break;
					case 1:
						type = GoreID.Smoke2;
						break;
					case 2:
						type = GoreID.Smoke3;
						break;
					default:
						type = GoreID.ChimneySmoke1;
						break;
				}
				float scale = Main.rand.NextFloat(1f, 2f);
				Vector2 vel = new Vector2(MathF.Sqrt(Main.rand.NextFloat()), 0).RotatedByRandom(MathHelper.TwoPi);
				var gore = Gore.NewGorePerfect(Player.Center + new Vector2(0, j) + vel * 30 - new Vector2(18), vel * 2f, type, scale);
				gore.timeLeft = Main.rand.Next(30, 60);
			}
		}
		InPlatform = false;
		NowHeight = 0;
	}

	public override void ProcessTriggers(TriggersSet triggersSet)
	{
		if (triggersSet.Jump || triggersSet.QuickMount)
		{
			DeactivatePlatform();
			return;
		}
		if (Platform == null || Platform.MaxHeight == 0)
		{
			DeactivatePlatform();
			return;
		}
		if (InPlatform && !TileSafe(PlatformBaseBottom))
		{
			DeactivatePlatform();
			return;
		}
		int direction = 0;

		// 同时按下的时候要停住
		if (triggersSet.Up)
		{
			direction++;
		}
		if (triggersSet.Down)
		{
			direction--;
		}

		if (direction == 0)
		{
			return;
		}

		if (NowHeight == 0)
		{
			PlatformBasePos = Player.position;
		}

		if (!InPlatform && direction == 1)
		{
			if (CanActivatePlatform())
			{
				ActivatePlatform();
			}
		}

		if (InPlatform)
		{
			int move = Platform.MoveSpeed * direction;
			if (!Collision.SolidCollision(Player.position + new Vector2(0, -move), Player.Hitbox.Width, Player.Hitbox.Height - 16))
			{
				NowHeight = Math.Clamp(NowHeight + move, 0, Platform.MaxHeight);
			}
		}
	}
}
