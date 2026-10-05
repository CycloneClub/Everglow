using Everglow.Yggdrasil.KelpCurtain.Projectiles.Miscs.VineWand;
using Everglow.Yggdrasil.KelpCurtain.Tiles.ForestRainVines;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Tools;

/// <summary>
/// 控制森雨藤曼长度的魔杖 - 藤曼修理魔杖
/// </summary>
public class VineRepairWand : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.MagicWeapons;

	// 存储当前调整状态
	private AdjustmentData currentAdjustment;

	// 存储激活的能量线弹幕
	private int staticBeamIndex = -1;
	private List<int> movingOrbIndices = new List<int>();

	private class AdjustmentData
	{
		public Point FixPoint;
		public bool IsAdjusting;
		public int LastManaChange;
		public int PendingOrbs; // 待生成的能量球数量
	}

	public override void SetDefaults()
	{
		Item.width = 34;
		Item.height = 34;
		Item.damage = 1;
		Item.useTime = 12;
		Item.useAnimation = 12;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.autoReuse = false;
		Item.mana = 6;
		Item.value = 13320;
		Item.rare = ItemRarityID.Green;
		Item.noMelee = true;
		Item.tileBoost = 60;
	}

	public override void HoldItem(Player player)
	{
		// 如果正在调整，让法杖发光并保持使用状态
		if (currentAdjustment != null && currentAdjustment.IsAdjusting)
		{
			Lighting.AddLight(player.Center, new Vector3(0.2f, 0.8f, 0.2f));
			Item.useTime = 0;
			Item.useAnimation = 0;

			float rot = (player.Center - currentAdjustment.FixPoint.ToWorldCoordinates()).ToRotation();
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rot + MathHelper.PiOver2);

			// 更新静态能量线位置
			UpdateStaticBeam(player);

			// 生成待定的能量球
			GeneratePendingOrbs(player);

			// 清理已完成的能量球
			CleanupFinishedOrbs();
		}
		else
		{
			Item.useTime = 12;
			Item.useAnimation = 12;
		}
	}

	public override void UpdateInventory(Player player)
	{
		// 如果不再持有法杖，结束调整状态
		if (player.HeldItem?.type != ModContent.ItemType<VineRepairWand>())
		{
			EndAdjustment();
		}
	}

	/// <summary>
	/// 开始调整藤曼
	/// </summary>
	public bool StartAdjustment(Player player, Point fixPoint)
	{
		// 消耗初始10点蓝
		if (player.CheckMana(10, true))
		{
			currentAdjustment = new AdjustmentData
			{
				FixPoint = fixPoint,
				IsAdjusting = true,
				LastManaChange = 10,
				PendingOrbs = 0,
			};

			// 创建静态能量线
			CreateStaticBeam(player, fixPoint);

			return true;
		}
		return false;
	}

	/// <summary>
	/// 更新藤曼调整（长度变化）
	/// </summary>
	public void UpdateAdjustment(Player player, Point fixPoint, int deltaLength)
	{
		if (deltaLength == 0)
		{
			return;
		}

		if (currentAdjustment == null || !currentAdjustment.IsAdjusting || currentAdjustment.FixPoint != fixPoint)
		{
			return;
		}

		// 检查是否仍然手持法杖
		if (player.HeldItem?.type != ModContent.ItemType<VineRepairWand>())
		{
			EndAdjustment();
			return;
		}

		if (deltaLength > 0) // 调长，消耗蓝
		{
			if (player.statMana >= deltaLength)
			{
				player.statMana -= deltaLength;
				currentAdjustment.LastManaChange = deltaLength;

				// 每1单位长度发送一个能量点
				currentAdjustment.PendingOrbs += deltaLength;
			}
			else
			{
				// 蓝量不足，强制结束调整
				EndAdjustment();
			}
		}
		else if (deltaLength < 0) // 调短，恢复蓝
		{
			player.statMana = Math.Min(player.statManaMax2, player.statMana - deltaLength);
			currentAdjustment.LastManaChange = deltaLength;

			// 每1单位长度发送一个能量点
			currentAdjustment.PendingOrbs += Math.Abs(deltaLength);
		}

		// 限制待生成能量球数量不超过4个
		currentAdjustment.PendingOrbs = Math.Min(currentAdjustment.PendingOrbs, 4);
	}

	/// <summary>
	/// 结束藤曼调整
	/// </summary>
	public void EndAdjustment()
	{
		if (currentAdjustment != null && currentAdjustment.IsAdjusting)
		{
			// 清除所有能量线弹幕
			ClearAllBeams();

			currentAdjustment.IsAdjusting = false;
			currentAdjustment = null;
		}
	}

	/// <summary>
	/// 检查是否正在调整藤曼
	/// </summary>
	public bool IsAdjusting(Point fixPoint)
	{
		return currentAdjustment != null && currentAdjustment.IsAdjusting && currentAdjustment.FixPoint == fixPoint;
	}

	/// <summary>
	/// 生成待定的能量球
	/// </summary>
	private void GeneratePendingOrbs(Player player)
	{
		if (currentAdjustment == null || currentAdjustment.PendingOrbs <= 0)
		{
			return;
		}

		// 计算当前可以生成的能量球数量（不超过4个同时存在）
		int availableSlots = 4 - movingOrbIndices.Count;
		int orbsToGenerate = Math.Min(currentAdjustment.PendingOrbs, availableSlots);

		for (int i = 0; i < orbsToGenerate; i++)
		{
			CreateEnergyOrb(player, currentAdjustment.FixPoint);
			currentAdjustment.PendingOrbs--;
		}
	}

	/// <summary>
	/// 清理已完成或无效的能量球
	/// </summary>
	private void CleanupFinishedOrbs()
	{
		// 移除已完成或无效的能量球
		for (int i = movingOrbIndices.Count - 1; i >= 0; i--)
		{
			int index = movingOrbIndices[i];
			if (index < 0 || index >= Main.maxProjectiles || !Main.projectile[index].active ||
				Main.projectile[index].type != ModContent.ProjectileType<VineEnergyOrb>())
			{
				movingOrbIndices.RemoveAt(i);
			}
		}
	}

	private void CreateStaticBeam(Player player, Point targetPoint)
	{
		if (Main.myPlayer == player.whoAmI)
		{
			Vector2 startPos = player.Center;
			Vector2 endPos = targetPoint.ToWorldCoordinates();

			// 创建静态能量线
			int projectileIndex = Projectile.NewProjectile(
				player.GetSource_FromThis(),
				startPos,
				Vector2.Zero,
				ModContent.ProjectileType<VineEnergyBeam>(),
				0, 0, player.whoAmI,
				TileUtils.SafeGetTile(targetPoint).TileType == ModContent.TileType<ForestRainVineTile_Thin>() ? 0.5f : 1);

			// 设置弹幕属性
			Projectile projectile = Main.projectile[projectileIndex];
			if (projectile.ModProjectile is VineEnergyBeam beam)
			{
				beam.StartPosition = startPos;
				beam.EndPosition = endPos;
			}

			staticBeamIndex = projectileIndex;
		}
	}

	private void UpdateStaticBeam(Player player)
	{
		if (player.ownedProjectileCounts[ModContent.ProjectileType<VineEnergyBeam>()] <= 0)
		{
			CreateStaticBeam(player, currentAdjustment.FixPoint);
		}
	}

	private void CreateEnergyOrb(Player player, Point targetPoint)
	{
		if (Main.myPlayer == player.whoAmI && movingOrbIndices.Count < 4)
		{
			Vector2 startPos, endPos;

			// 根据调整方向决定能量球流向
			if (currentAdjustment.LastManaChange >= 0) // 调长，从法杖到tile
			{
				startPos = player.Center;
				endPos = targetPoint.ToWorldCoordinates();
			}
			else // 调短，从tile到法杖
			{
				startPos = targetPoint.ToWorldCoordinates();
				endPos = player.Center;
			}

			int projectileIndex = Projectile.NewProjectile(
				player.GetSource_FromThis(),
				startPos,
				Vector2.Zero,
				ModContent.ProjectileType<VineEnergyOrb>(),
				0, 0, player.whoAmI,
				TileUtils.SafeGetTile(targetPoint).TileType == ModContent.TileType<ForestRainVineTile_Thin>() ? 0.5f : 1);

			Projectile projectile = Main.projectile[projectileIndex];
			if (projectile.ModProjectile is VineEnergyOrb orb)
			{
				orb.StartPosition = startPos;
				orb.EndPosition = endPos;
				orb.Speed = 0.02f + Main.rand.NextFloat(0.01f); // 稍微随机化速度
				orb.Progress = 0f;
				if (endPos != player.Center)
				{
					orb.TileDestination = endPos;
				}
			}

			movingOrbIndices.Add(projectileIndex);
		}
	}

	private void ClearAllBeams()
	{
		// 清除静态能量线
		if (staticBeamIndex != -1 && Main.projectile[staticBeamIndex].active)
		{
			Main.projectile[staticBeamIndex].Kill();
			staticBeamIndex = -1;
		}

		// 清除所有移动能量球
		foreach (int index in movingOrbIndices)
		{
			if (index >= 0 && index < Main.maxProjectiles && Main.projectile[index].active)
			{
				Main.projectile[index].Kill();
			}
		}
		movingOrbIndices.Clear();
	}

	public override bool CanUseItem(Player player)
	{
		return false;
	}

	public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
	{
		if (currentAdjustment != null && currentAdjustment.IsAdjusting)
		{
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			Vector2 position = Item.position - Main.screenPosition + new Vector2(Item.width / 2, Item.height - texture.Height * 0.5f + 2f);

			Color glowColor = currentAdjustment.LastManaChange >= 0 ?
				Color.Lerp(Color.White, Color.LightGreen, (float)System.Math.Sin(Main.GlobalTimeWrappedHourly * 6f) * 0.3f + 0.7f) :
				Color.Lerp(Color.White, Color.YellowGreen, (float)System.Math.Sin(Main.GlobalTimeWrappedHourly * 6f) * 0.3f + 0.7f);

			spriteBatch.Draw(
				texture,
				position,
				null,
				glowColor,
				rotation,
				texture.Size() * 0.5f,
				scale,
				SpriteEffects.None,
				0f);
		}
	}
}
