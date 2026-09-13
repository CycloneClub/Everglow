using Everglow.Commons.Physics.MassSpringSystem;
using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.KelpCurtain.Items.Tools;
using Spine;
using Terraria.GameContent.Drawing;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.ForestRainVines;

public class ForestRainVineTile_Thick : HangingTile
{
	public override void InitHanging()
	{
		MaxWireStyle = 1;
		CanGrasp = true;

		RopeUnitMass = 3f;
		HangingItemMass = 250f;
		Elasticity = 200f;
		UnitLength = 16f;
	}

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		if (j >= 20)
		{
			Tile tileUp = Main.tile[i, j - 1];
			if (tileUp.Slope != SlopeType.Solid)
			{
				tileUp.Slope = SlopeType.Solid;
			}
		}
		return base.TileFrame(i, j, ref resetFrame, ref noBreak);
	}

	public override void DrawCable(Rope rope, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing, Color color = default)
	{
		if (!TileDrawing.IsVisible(Main.tile[pos]))
		{
			return;
		}

		var tile = Main.tile[pos];
		ushort type = tile.TileType;
		int paint = Main.tile[pos].TileColor;
		string originalPath = @Texture;
		string[] pathSegments = originalPath.Split("/");
		string trimmedTexturePath = Path.Combine(pathSegments.Skip(1).ToArray());
		Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(trimmedTexturePath, type, 1, paint, tileDrawing);
		tex ??= (Texture2D)ModContent.Request<Texture2D>(Texture);

		var masses = rope.Masses;
		for (int i = 0; i < masses.Length; i++)
		{
			Mass thisMass = masses[i];
			int totalPushTime = 80;
			float pushForcePerFrame = 1.26f;
			float windCycle = 0;
			if (tileDrawing.InAPlaceWithWind((int)((thisMass.Position.X - 8) / 16f), (int)((thisMass.Position.Y - 8) / 16f), 1, 1))
			{
				windCycle = tileDrawing.GetWindCycle((int)((thisMass.Position.X - 8) / 16f), (int)((thisMass.Position.Y - 8) / 16f), tileDrawing._sunflowerWindCounter);
			}

			float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex((int)((thisMass.Position.X - 8) / 16f), (int)((thisMass.Position.Y - 8) / 16f), 1, 1, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
			windCycle += highestWindGridPushComplex;
			if (!Main.gamePaused)
			{
				if (i < masses.Length - 1)
				{
					rope.ApplyForceSpecial(i, new Vector2(windCycle / 4.0f, 0.4f * thisMass.Value));
				}
				else
				{
					rope.ApplyForceSpecial(i, new Vector2(windCycle * 10.0f, 0.4f * thisMass.Value));
				}
			}

			// 支持发光涂料
			Color tileLight;
			if (color != default)
			{
				tileLight = color;
			}
			else
			{
				tileLight = Lighting.GetColor((int)((thisMass.Position.X - 8) / 16f), (int)((thisMass.Position.Y - 8) / 16f));
			}

			Vector2 toNextMass;
			if (i < masses.Length - 1)
			{
				Mass nextMass = masses[i + 1];
				toNextMass = nextMass.Position - thisMass.Position;
			}
			else
			{
				Mass passedMass = masses[i - 1];
				toNextMass = thisMass.Position - passedMass.Position;
			}
			Vector2 drawPos = thisMass.Position - Main.screenPosition;
			DrawRopeUnit(spriteBatch, tex, drawPos, pos, rope, i, toNextMass.ToRotation() - MathHelper.PiOver2, tileLight);
			if (HangingTileUpdateSystem.WinchAdjustingPlayerTimers.Keys.Contains(pos))
			{
				float value = HangingTileUpdateSystem.WinchAdjustingPlayerTimers[pos] / AdjustingVisualTimeMax;
				Color drawColor = Color.Lerp(Color.White, new Color(0.2f, 0.7f, 0.4f, 0), 1 - value) * value;
				DrawRopeUnit(spriteBatch, ModAsset.ForestRainVineTile_Thick_Shape.Value, drawPos, pos, rope, i, toNextMass.ToRotation() - MathHelper.PiOver2, drawColor);
			}
		}
	}

	public override void DrawWinch(int i, int j, SpriteBatch spriteBatch)
	{
		Color lightColor = Lighting.GetColor(i, j);
		var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}
		Texture2D tex = (Texture2D)ModContent.Request<Texture2D>(Texture);
		Rectangle frame = new Rectangle(0, 0, 50, 24);
		spriteBatch.Draw(tex, new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition + zero + new Vector2(0, -12), frame, lightColor, 0, new Vector2(frame.Width * 0.5f, 0), 1, SpriteEffects.None, 0);
		if (HangingTileUpdateSystem.WinchAdjustingPlayerTimers.Keys.Contains(new Point(i, j)))
		{
			tex = ModAsset.ForestRainVineTile_Thick_Shape.Value;
			float value = HangingTileUpdateSystem.WinchAdjustingPlayerTimers[new Point(i, j)] / AdjustingVisualTimeMax;
			Color drawColor = Color.Lerp(Color.White, new Color(0.2f, 0.7f, 0.4f, 0), 1 - value) * value;
			spriteBatch.Draw(tex, new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition + zero + new Vector2(0, -12), frame, drawColor, 0, new Vector2(frame.Width * 0.5f, 0), 1, SpriteEffects.None, 0);
		}
	}

	public override void DrawRopeUnit(SpriteBatch spriteBatch, Texture2D texture, Vector2 drawPos, Point tilePos, Rope rope, int index, float rotation, Color tileLight)
	{
		var masses = rope.Masses;
		Rectangle frame;
		Vector2 offset = new Vector2(0, 0);
		drawPos += offset;
		if (index <= masses.Length - 6)
		{
			if (!masses[Math.Min(index + 1, masses.Length - 1)].IsStatic)
			{
				frame = new Rectangle(0, 16 + TileUtils.GetFixedRandomNumber(index + 60, tilePos.X, 4) * 20, 50, 20);
				spriteBatch.Draw(texture, drawPos, frame, tileLight, rotation, frame.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			}
		}
		else
		{
			int contraryIndex = masses.Length - index;
			frame = new Rectangle(0, 100 + (6 - contraryIndex) * 20, 50, 20);
			spriteBatch.Draw(texture, drawPos, frame, tileLight, rotation, frame.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		}
	}

	public override void OnAdjustmentStart(Point fixPoint, Player player)
	{
		// 检查是否手持藤曼修理魔杖
		if (player.HeldItem?.type == ModContent.ItemType<VineRepairWand>())
		{
			// 直接调用法杖的开始调整方法
			(player.HeldItem.ModItem as VineRepairWand)?.StartAdjustment(player, fixPoint);
		}
	}

	public override void OnAdjustmentUpdate(Point fixPoint, Player player, int deltaLength)
	{
		// 检查是否手持藤曼修理魔杖
		if (player.HeldItem?.type == ModContent.ItemType<VineRepairWand>())
		{
			// 直接调用法杖的更新调整方法
			(player.HeldItem.ModItem as VineRepairWand)?.UpdateAdjustment(player, fixPoint, deltaLength);
		}
		else
		{
			// 如果没有手持法杖，结束调整
			OnAdjustmentEnd(fixPoint, player);
		}
		base.OnAdjustmentUpdate(fixPoint, player, deltaLength);
	}

	public override void OnAdjustmentEnd(Point fixPoint, Player player)
	{
		// 直接调用法杖的结束调整方法
		if (player.HeldItem?.type == ModContent.ItemType<VineRepairWand>())
		{
			(player.HeldItem.ModItem as VineRepairWand)?.EndAdjustment();
		}
		else
		{
			// 即使没有手持法杖，也尝试结束调整
			var wand = ModContent.GetInstance<VineRepairWand>();
			wand.EndAdjustment();
		}
	}

	/// <summary>
	/// 当手持藤曼修理魔杖时允许调整长度
	/// </summary>
	/// <param name="i"></param>
	/// <param name="j"></param>
	/// <returns></returns>
	public override bool IsCanRightClick(int i, int j)
	{
		Player player = Main.LocalPlayer;
		return player.HeldItem != null && player.HeldItem.type == ModContent.ItemType<VineRepairWand>();
	}

	#region 按钮重绘
	public override void DrawDefaultPanel(HangingTileAdjustingHelper hangingSystem, Player player, Color color, ref Queue<DrawStack> drawStacks)
	{
		drawStacks = new Queue<DrawStack>();

		// 背景改为淡绿色半透明（保持父类面板结构）
		// DrawBackgroundPanel(hangingSystem, new Color(0.8f, 1f, 0.8f, 0.5f), ref drawStacks);
		float maxCos = 0;
		int maxK = 0;
		Vector2 rotCenter = hangingSystem.FixPoint.ToWorldCoordinates();

		// 完全保留父类的旋转计算逻辑，不添加摆动偏移
		Vector2 cut = new Vector2(1, 0).RotatedBy(hangingSystem.HandleRotation + MathHelper.Pi);

		// 保留父类的校准尾迹计算逻辑
		for (int k = -10; k < 10; k++)
		{
			Vector2 cut2 = new Vector2(0, -1).RotatedBy(k / 20f * MathHelper.TwoPi);
			float cosValue = Vector2.Dot(cut, cut2);
			if (cosValue > maxCos)
			{
				maxCos = cosValue;
				maxK = k;
			}
		}

		// 绘制校准尾迹（仅替换为绿色主题）
		// 基础色改为绿色系，保留原有透明度逻辑
		Color baseGreen = new Color(0.3f, 0.8f, 0.4f); // 主绿色
		Color newDrawColor = Color.Lerp(baseGreen, color, 0.2f); // 轻微融合原色调
		Color outerRingColor = Color.Lerp(newDrawColor, new Color(0.3f, 0.24f, 0.16f), 0.6f);

		float nowFrameY = hangingSystem.StartFrameY60 / 60f + hangingSystem.HandleRotation * 2;

		// 警告色改为绿红色渐变（保持原有警告逻辑）
		if (nowFrameY < 5)
		{
			newDrawColor = Color.Lerp(newDrawColor, new Color(0.9f, 0.3f, 0.3f, 0.8f), (5 - nowFrameY) / 4f);
			outerRingColor = Color.Lerp(outerRingColor, new Color(0.9f, 0.2f, 0.2f, 0.8f), (5 - nowFrameY) / 4f);
		}
		if (nowFrameY > MaxCableLength - 5)
		{
			newDrawColor = Color.Lerp(newDrawColor, new Color(0.9f, 0.3f, 0.3f, 0.8f), (nowFrameY - (MaxCableLength - 5)) / 4f);
			outerRingColor = Color.Lerp(outerRingColor, new Color(0.9f, 0.2f, 0.2f, 0.8f), (nowFrameY - (MaxCableLength - 5)) / 4f);
		}

		// 完全保留父类的透明度计算逻辑
		float tK = 12f;
		if (hangingSystem.TimeToKill > 0)
		{
			tK = hangingSystem.TimeToKill;
		}
		float fade = Math.Min(hangingSystem.Timer, tK) / 12f;

		// 边界颜色替换为深绿色（保持父类绘制逻辑）
		DrawBound(hangingSystem, new Color(0.2f, 0.6f, 0.3f), player, ref drawStacks);

		// 方向环保持父类的稳定角度，仅替换为绿色
		DrawDirectionRing(hangingSystem, outerRingColor,
			hangingSystem.HandleRotation - MathHelper.PiOver2, // 移除所有摆动偏移，与父类一致
			ref drawStacks);

		// 绘制线条（保持父类的长度、粗细和角度逻辑，仅改颜色）
		for (int k = -10; k < 10; k++)
		{
			Vector2 cut2 = new Vector2(0, -1).RotatedBy(k / 20f * MathHelper.TwoPi);
			float cosValue = Vector2.Dot(cut, cut2);

			if (k == maxK)
			{
				// 保留黑色轮廓，确保与父类风格一致
				DrawLine_Black(
					hangingSystem,
					rotCenter + cut2 * (4 * hangingSystem.PanelRange - 12),
					rotCenter + cut2 * (4 * hangingSystem.PanelRange + 64),
					16, ref drawStacks);

				// 主线条替换为绿色
				DrawLine(
					rotCenter + cut2 * (4 * hangingSystem.PanelRange - 12),
					rotCenter + cut2 * (4 * hangingSystem.PanelRange + 64),
					16, newDrawColor * fade, 1f, ref drawStacks);
			}
			else
			{
				// 副线条替换为绿色
				DrawLine(
					rotCenter + cut2 * 4 * hangingSystem.PanelRange,
					rotCenter + cut2 * (4 * hangingSystem.PanelRange + 48),
					12, newDrawColor * fade, cosValue, ref drawStacks);
			}
		}

		// Draw the weapon which player holding.
		Texture2D tex = ModAsset.VineRepairWand.Value;
		Texture2D tex_glow = ModAsset.VineRepairWand_glow.Value;
		Texture2D tex_bloom = ModAsset.VineRepairWand_bloom.Value;
		Vector2 toCenter = rotCenter - player.Center;
		float rot = toCenter.ToRotationSafe() + MathHelper.PiOver4;
		if (toCenter.X > 1)
		{
			player.direction = 1;
		}
		if (toCenter.X < -1)
		{
			player.direction = -1;
		}
		Ins.Batch.Draw(tex, player.Center, null, Lighting.GetColor(player.Center.ToTileCoordinates()), rot, new Vector2(0, 34), 1f, SpriteEffects.None);
		Ins.Batch.Draw(tex_glow, player.Center, null, Color.White, rot, new Vector2(0, 34), 1f, SpriteEffects.None);
		Ins.Batch.Draw(tex_bloom, player.Center, null, new Color(1f, 1f, 1f, 0), rot, new Vector2(0, 80), 1f, SpriteEffects.None);
	}

	public override void DrawLine(Vector2 pos1, Vector2 pos2, float width, Color color, float highlight, ref Queue<DrawStack> drawStacks)
	{
		if (drawStacks == null)
		{
			drawStacks = new Queue<DrawStack>();
		}
		Vector2 normal = Utils.SafeNormalize(pos1 - pos2, Vector2.zeroVector).RotatedBy(MathHelper.PiOver2) * width / 2f;
		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(pos1 + normal, color, new Vector3(0, 0, 0)),
			new Vertex2D(pos2 + normal, color, new Vector3(1, 0, 0)),
			new Vertex2D(pos1 - normal, color, new Vector3(0, 1, 0)),

			new Vertex2D(pos1 - normal, color, new Vector3(0, 1, 0)),
			new Vertex2D(pos2 + normal, color, new Vector3(1, 0, 0)),
			new Vertex2D(pos2 - normal, color, new Vector3(1, 1, 0)),
		};
		drawStacks.Enqueue(new DrawStack(ModAsset.ForestRainVine_Cut.Value, bars, PrimitiveType.TriangleList));
		if (highlight > 0)
		{
			Color bloomColor = color * highlight;
			bloomColor.A = 0;
			bars = new List<Vertex2D>()
			{
				 new Vertex2D(pos1 + normal, bloomColor, new Vector3(0, 0, 0)),
				 new Vertex2D(pos2 + normal, bloomColor, new Vector3(1, 0, 0)),
				 new Vertex2D(pos1 - normal, bloomColor, new Vector3(0, 1, 0)),

				 new Vertex2D(pos1 - normal, bloomColor, new Vector3(0, 1, 0)),
				 new Vertex2D(pos2 + normal, bloomColor, new Vector3(1, 0, 0)),
				 new Vertex2D(pos2 - normal, bloomColor, new Vector3(1, 1, 0)),
			};
			drawStacks.Enqueue(new DrawStack(ModAsset.ForestRainVine_Cut_Bloom.Value, bars, PrimitiveType.TriangleList));
		}
	}

	public override void DrawLine_Black(HangingTileAdjustingHelper hangingSystem, Vector2 pos1, Vector2 pos2, float width, ref Queue<DrawStack> drawStacks)
	{
		if (drawStacks == null)
		{
			drawStacks = new Queue<DrawStack>();
		}
		Vector2 normal = Utils.SafeNormalize(pos1 - pos2, Vector2.zeroVector).RotatedBy(MathHelper.PiOver2) * width / 2f;
		Color bloomColor = Color.White;
		float tK = 12f;
		if (hangingSystem.TimeToKill > 0)
		{
			tK = hangingSystem.TimeToKill;
		}
		float fade = Math.Min(hangingSystem.Timer, tK) / 12f;
		bloomColor *= fade;
		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(pos1 + normal, bloomColor, new Vector3(0, 0, 0)),
			new Vertex2D(pos2 + normal, bloomColor, new Vector3(1, 0, 0)),
			new Vertex2D(pos1 - normal, bloomColor, new Vector3(0, 1, 0)),

			new Vertex2D(pos1 - normal, bloomColor, new Vector3(0, 1, 0)),
			new Vertex2D(pos2 + normal, bloomColor, new Vector3(1, 0, 0)),
			new Vertex2D(pos2 - normal, bloomColor, new Vector3(1, 1, 0)),
		};
		drawStacks.Enqueue(new DrawStack(ModAsset.ForestRainVine_Cut_Bloom_black.Value, bars, PrimitiveType.TriangleList));
	}

	public override void DrawDirectionRing(HangingTileAdjustingHelper hangingSystem, Color color, float rotation, ref Queue<DrawStack> drawStacks)
	{
		if (drawStacks == null)
		{
			drawStacks = new Queue<DrawStack>();
		}
		color.A = 0;
		Vector2 pos = hangingSystem.FixPoint.ToWorldCoordinates();
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int k = 0; k <= 60; k++)
		{
			bars.Add(pos + new Vector2(0, hangingSystem.PanelRange * 64).RotatedBy(k / 60f * MathHelper.TwoPi + rotation), Color.White, new Vector3(k / 20f, 0, 0));
			bars.Add(pos + new Vector2(0, hangingSystem.PanelRange * 0).RotatedBy(k / 60f * MathHelper.TwoPi + rotation), Color.White, new Vector3(k / 20f, 1, 0));
		}
		drawStacks.Enqueue(new DrawStack(ModAsset.ForestRainVine_Matrix_black.Value, bars, PrimitiveType.TriangleStrip));
		bars = new List<Vertex2D>();
		for (int k = 0; k <= 60; k++)
		{
			bars.Add(pos + new Vector2(0, hangingSystem.PanelRange * 64).RotatedBy(k / 60f * MathHelper.TwoPi + rotation), color, new Vector3(k / 20f, 0, 0));
			bars.Add(pos + new Vector2(0, hangingSystem.PanelRange * 0).RotatedBy(k / 60f * MathHelper.TwoPi + rotation), color, new Vector3(k / 20f, 1, 0));
		}
		drawStacks.Enqueue(new DrawStack(ModAsset.ForestRainVine_Matrix.Value, bars, PrimitiveType.TriangleStrip));
	}
	#endregion
}
