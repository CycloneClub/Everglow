using Everglow.Commons.Coroutines;
using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.VFXs.Arena;

[Pipeline(typeof(WCSPipeline))]
public class NPCBossTagsSystem : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PreDrawFilter;

	public NPC TargetBoss = null;

	public bool StartFighting = false;

	public bool AllTagsEnable = false;

	public List<TownNPC_LiveInYggdrasil.BossTag> Tags = new List<TownNPC_LiveInYggdrasil.BossTag>();

	public float Score = 0;

	public float VisualScore = 0;

	public int MaxTime = -1;

	public int RemainTime = -1;

	public Queue<int> OldHealthValue = new Queue<int>();

	/// <summary>
	/// Control visual animation.
	/// </summary>
	public CoroutineManager AnimationCoroutine = new CoroutineManager();

	public override void OnSpawn()
	{
		Texture = ModAsset.NPCBossTagsSystem.Value;
		MaxDiatanceOutOfScreen = 18000;
	}

	public override void Update()
	{
		MaxDiatanceOutOfScreen = 18000;
		AnimationCoroutine.Update();
		if (TargetBoss == null || !TargetBoss.active)
		{
			foreach (var npc in Main.npc)
			{
				if (npc != null && npc.active && npc.ModNPC is TownNPC_LiveInYggdrasil)
				{
					TargetBoss = npc;
					break;
				}
			}
		}
		if (TargetBoss == null)
		{
			return;
		}
		var tNLIY = TargetBoss.ModNPC as TownNPC_LiveInYggdrasil;
		if (tNLIY == null)
		{
			return;
		}
		OldHealthValue.Enqueue(TargetBoss.life);
		if (OldHealthValue.Count > 20)
		{
			OldHealthValue.Dequeue();
		}
		Tags = tNLIY.MyBossTags;
		int totalScore = 0;
		foreach (var tag in Tags)
		{
			if (tag.Enable)
			{
				totalScore += tag.Value;
			}
		}
		Score = totalScore;
		VisualScore = (float)Utils.Lerp(VisualScore, Score, 0.8f);
		YggdrasilTownCentralSystem.ArenaScore = (int)Score;
		if (StartFighting)
		{
			if (!tNLIY.StartedFight)
			{
				StartFighting = false;
			}
		}
		base.Update();
	}

	public IEnumerator<ICoroutineInstruction> EnableAllTags()
	{
		int maxTag = Tags.Count;
		for (int t = 0; t < maxTag; t++)
		{
			var tNLIY = TargetBoss.ModNPC as TownNPC_LiveInYggdrasil;
			if (tNLIY != null)
			{
				if (Tags[t].ConflictTags == null || Tags[t].ConflictTags.Count <= 0)
				{
					tNLIY.EnableBossTag(t);
				}
				else
				{
					int thisScore = Tags[t].Value;
					bool shouldAdd = true;
					foreach (var checkTag in Tags)
					{
						if (Tags[t].ConflictTags.Contains(checkTag.Name))
						{
							if (checkTag.Value > thisScore)
							{
								shouldAdd = false;
								break;
							}
						}
					}
					if (shouldAdd)
					{
						tNLIY.EnableBossTag(t);
					}
				}
			}
			yield return new SkipThisFrame();
		}
	}

	public IEnumerator<ICoroutineInstruction> DisableAllTags()
	{
		int maxTag = Tags.Count;
		for (int t = 0; t < maxTag; t++)
		{
			var tNLIY = TargetBoss.ModNPC as TownNPC_LiveInYggdrasil;
			if (tNLIY != null)
			{
				tNLIY.DisableBossTag(t);
			}
			yield return new SkipThisFrame();
		}
	}

	public override void Draw()
	{
		if (TargetBoss == null || !TargetBoss.active)
		{
			return;
		}

		int tagCount = 0;

		// tagCount should substract by conflict tags count.
		List<string> tagNames = new List<string>();
		for (int i = 0; i < Tags.Count; i++)
		{
			var tag = Tags[i];
			tagNames.Add(tag.Name);
			tagCount++;
			if (tag.ConflictTags != null && tag.ConflictTags.Count > 0)
			{
				// 1 tag can eliminate 1 count at most.
				foreach (var conf in tag.ConflictTags)
				{
					if (tagNames.Contains(conf))
					{
						tagCount--;
						break;
					}
				}
			}
		}
		int rowCount = (int)MathF.Sqrt(tagCount) + 1;
		var topLeftPos = Position + new Vector2(-rowCount * 80 / 2 + 40 + 16, -rowCount * 80 - 200 + 40 + 16);

		var tNLIY = TargetBoss.ModNPC as TownNPC_LiveInYggdrasil;
		if (tNLIY != null)
		{
			foreach (var tag in Tags)
			{
				if (!tNLIY.StartedFight)
				{
					if (tag.Enable && tag.Name == "RemoveTopPlatform")
					{
						DrawRemovingPlatformEffect(tag.Name);
					}
					if (tag.Enable && tag.Name == "RemoveBottomPlatform")
					{
						DrawRemovingPlatformEffect(tag.Name);
					}
					if (tag.Enable && tag.Name == "RemoveLeftPlatform")
					{
						DrawRemovingPlatformEffect(tag.Name);
					}
					if (tag.Enable && tag.Name == "RemoveRightPlatform")
					{
						DrawRemovingPlatformEffect(tag.Name);
					}
					if (tag.Enable && tag.Name == "180Seconds")
					{
						tNLIY.FailTime = 10800;
						tNLIY.BossTimer = 0;
						DrawTimeBar();
					}
					if (tag.Enable && tag.Name == "90Seconds")
					{
						tNLIY.FailTime = 5400;
						tNLIY.BossTimer = 0;
						DrawTimeBar();
					}
				}
				if (tag.Enable && tag.Name == "180Seconds")
				{
					DrawTimeBar();
				}
				if (tag.Enable && tag.Name == "90Seconds")
				{
					DrawTimeBar();
				}
			}
		}

		// Background Panel.
		var backGroundColor = new Color(10, 10, 10, 200);
		var backgroundPos = topLeftPos - new Vector2(40, 40);
		int backgroundWidth = rowCount * 80;
		int backgroundHeight = rowCount * 80 + 160;
		var blackBackground = new List<Vertex2D>()
		{
			 new Vertex2D(backgroundPos, backGroundColor, new Vector3(0, 0, 0)),
			 new Vertex2D(backgroundPos + new Vector2(backgroundWidth, 0), backGroundColor, new Vector3(0, 0, 0)),

			 new Vertex2D(backgroundPos + new Vector2(0, backgroundHeight), backGroundColor, new Vector3(0, 0, 0)),
			 new Vertex2D(backgroundPos + new Vector2(backgroundWidth, backgroundHeight), backGroundColor, new Vector3(0, 0, 0)),
		};
		Ins.Batch.Draw(Texture, blackBackground, PrimitiveType.TriangleStrip);

		if (StartFighting)
		{
			DrawHealthBar(backgroundWidth, backgroundHeight, backgroundPos);
			return;
		}
		if (MouseInArea(backgroundPos, new Vector2(backgroundWidth, backgroundHeight)))
		{
			if (Main.netMode != NetmodeID.Server)
			{
				ArenaPlayer aPlayer = Main.LocalPlayer.GetModPlayer<ArenaPlayer>();
				if (aPlayer != null)
				{
					aPlayer.MouseInTagUIPanel = true;
				}
			}
		}

		// Tag Icons.
		int drawPosIndex = 0;

		// drawAndRankIndex: X refer drawIndex; Y refer the rank of icons occupied same drawIndex.
		List<(TownNPC_LiveInYggdrasil.BossTag conflictTag, Point drawAndRankIndex)> conflictTags = new List<(TownNPC_LiveInYggdrasil.BossTag, Point)>();
		for (int i = 0; i < Tags.Count; i++)
		{
			int iconWidth = 38;
			var drawColor = new Color(50, 50, 50, 0);
			var tag = Tags[i];
			var drawPos = topLeftPos + new Vector2(drawPosIndex % rowCount * 80, (drawPosIndex - drawPosIndex % rowCount) / rowCount * 80);

			// Conflict tags
			bool isConflictTag = false;
			foreach (var item in conflictTags)
			{
				if (item.conflictTag.ConflictTags is not null && item.conflictTag.ConflictTags.Contains(tag.Name))
				{
					drawPosIndex--;
					drawPos = topLeftPos + new Vector2(item.drawAndRankIndex.X % rowCount * 80, (item.drawAndRankIndex.X - item.drawAndRankIndex.X % rowCount) / rowCount * 80);
					isConflictTag = true;
					int conflictCount = 1;
					if (tag.ConflictTags is not null && tag.ConflictTags.Count > 0)
					{
						conflictCount = tag.ConflictTags.Count + 1;
					}
					drawPos += new Vector2(19).RotatedBy(MathHelper.TwoPi / conflictCount * (item.drawAndRankIndex.Y + 1));
					break;
				}
			}
			if (isConflictTag)
			{
				(TownNPC_LiveInYggdrasil.BossTag conflictTag, Point drawAndRankIndex)[] oldConflictTags = conflictTags.ToArray();
				conflictTags.Clear();
				for (int j = 0; j < oldConflictTags.Length; j++)
				{
					var item = oldConflictTags[j];
					if (item.conflictTag.ConflictTags is not null && item.conflictTag.ConflictTags.Contains(tag.Name))
					{
						item.drawAndRankIndex.Y += 1;
						conflictTags.Add(item);
					}
					else
					{
						conflictTags.Add(item);
					}
				}
			}
			if (tag.ConflictTags is not null && tag.ConflictTags.Count > 0)
			{
				iconWidth = 34;
				if (!isConflictTag)
				{
					conflictTags.Add((tag, new Point(drawPosIndex, 0)));

					// Draw "or" tag
					Color orColor = drawColor * 0.5f;
					var orTag = new List<Vertex2D>()
					{
						 new Vertex2D(drawPos + new Vector2(-24, -16), orColor, new Vector3(294f / Texture.Width, 0, 0)),
						 new Vertex2D(drawPos + new Vector2(24, -16), orColor, new Vector3(358f / Texture.Width, 0, 0)),

						 new Vertex2D(drawPos + new Vector2(-24, 16), orColor, new Vector3(294f / Texture.Width, 38f / Texture.Height, 0)),
						 new Vertex2D(drawPos + new Vector2(24, 16), orColor, new Vector3(358f / Texture.Width, 38f / Texture.Height, 0)),
					};
					Ins.Batch.Draw(Texture, orTag, PrimitiveType.TriangleStrip);
					drawPos += new Vector2(19);
				}
			}

			int iType = tag.IconType;
			if (tag.Enable)
			{
				drawColor = new Color(160, 200, 250, 0);
			}
			if (MouseInArea(drawPos - new Vector2(iconWidth) / 2f, new Vector2(iconWidth)))
			{
				drawColor = Color.Lerp(drawColor, new Color(255, 220, 125, 0), 0.5f);
				if (Main.mouseLeft && Main.mouseLeftRelease)
				{
					if (tNLIY != null)
					{
						if (tag.Enable)
						{
							tNLIY.DisableBossTag(i);
						}
						else
						{
							tNLIY.EnableBossTag(i);
						}
					}
				}
				Main.instance.MouseText(tag.DisplayContents);
			}
			float coordX0 = iType % 6 * 36 / (float)Texture.Width;
			float coordY0 = (iType - iType % 6) / 6f * 36 / Texture.Height;
			float coordX1 = (iType % 6 * 36 + 38) / (float)Texture.Width;
			float coordY1 = ((iType - iType % 6) / 6f * 36 + 38) / Texture.Height;

			var bars = new List<Vertex2D>()
			{
				 new Vertex2D(drawPos + new Vector2(-iconWidth / 2f, -iconWidth / 2f), drawColor, new Vector3(coordX0, coordY0, 0)),
				 new Vertex2D(drawPos + new Vector2(iconWidth / 2f, -iconWidth / 2f), drawColor, new Vector3(coordX1, coordY0, 0)),

				 new Vertex2D(drawPos + new Vector2(-iconWidth / 2f, iconWidth / 2f), drawColor, new Vector3(coordX0, coordY1, 0)),
				 new Vertex2D(drawPos + new Vector2(iconWidth / 2f, iconWidth / 2f), drawColor, new Vector3(coordX1, coordY1, 0)),
			};
			Ins.Batch.Draw(Texture, bars, PrimitiveType.TriangleStrip);
			drawPosIndex++;
		}

		// Start Button
		var startButtonColor = new Color(50, 50, 50, 0);
		var startButtonPos = Position + new Vector2(-35 + 16, -80);
		if (MouseInArea(startButtonPos, new Vector2(70, 38)))
		{
			startButtonColor = Color.Lerp(startButtonColor, new Color(255, 220, 125, 0), 0.5f);
			if (CanStart())
			{
				Main.instance.MouseText("Start fighting");
				if (Main.mouseLeft && Main.mouseLeftRelease)
				{
					if (tNLIY != null)
					{
						StartFighting = true;
						tNLIY.StartFighting();
					}
				}
			}
			else
			{
				Main.instance.MouseText("Unable to start fighting, please check the tags again.", ItemRarityID.Red);
			}
		}
		var startButton = new List<Vertex2D>()
		{
			 new Vertex2D(startButtonPos, startButtonColor, new Vector3(222f / Texture.Width, 0, 0)),
			 new Vertex2D(startButtonPos + new Vector2(70, 0), startButtonColor, new Vector3(292f / Texture.Width, 0, 0)),

			 new Vertex2D(startButtonPos + new Vector2(0, 38), startButtonColor, new Vector3(222f / Texture.Width, 38f / Texture.Height, 0)),
			 new Vertex2D(startButtonPos + new Vector2(70, 38), startButtonColor, new Vector3(292f / Texture.Width, 38f / Texture.Height, 0)),
		};
		Ins.Batch.Draw(Texture, startButton, PrimitiveType.TriangleStrip);

		// All Enable Button
		var allEnableButtonColor = new Color(50, 50, 110, 0);
		var allEnableButtonPos = Position + new Vector2(-35 + 16 - 60, -80);
		if (MouseInArea(allEnableButtonPos, new Vector2(70, 38)))
		{
			allEnableButtonColor = Color.Lerp(allEnableButtonColor, new Color(255, 220, 125, 0), 0.5f);
			Main.instance.MouseText("Enable All Tags");
			if (Main.mouseLeft && Main.mouseLeftRelease)
			{
				AnimationCoroutine.StartCoroutine(new Coroutine(EnableAllTags()));
			}
		}
		var allEnableButton = new List<Vertex2D>()
		{
			 new Vertex2D(allEnableButtonPos, allEnableButtonColor, new Vector3(36f / Texture.Width, 180f / Texture.Height, 0)),
			 new Vertex2D(allEnableButtonPos + new Vector2(38, 0), allEnableButtonColor, new Vector3(74f / Texture.Width, 180f / Texture.Height, 0)),

			 new Vertex2D(allEnableButtonPos + new Vector2(0, 38), allEnableButtonColor, new Vector3(36f / Texture.Width, 218f / Texture.Height, 0)),
			 new Vertex2D(allEnableButtonPos + new Vector2(38, 38), allEnableButtonColor, new Vector3(74f / Texture.Width, 218f / Texture.Height, 0)),
		};
		Ins.Batch.Draw(Texture, allEnableButton, PrimitiveType.TriangleStrip);

		// All Disable Button
		var allDisableButtonColor = new Color(60, 5, 20, 0);
		var allDisableButtonPos = Position + new Vector2(-35 + 16 + 95, -80);
		if (MouseInArea(allDisableButtonPos, new Vector2(70, 38)))
		{
			allDisableButtonColor = Color.Lerp(allDisableButtonColor, new Color(255, 220, 125, 0), 0.5f);
			Main.instance.MouseText("Disable All Tags");
			if (Main.mouseLeft && Main.mouseLeftRelease)
			{
				AnimationCoroutine.StartCoroutine(new Coroutine(DisableAllTags()));
			}
		}
		var allDisableButton = new List<Vertex2D>()
		{
			 new Vertex2D(allDisableButtonPos, allDisableButtonColor, new Vector3(36f / Texture.Width, 180f / Texture.Height, 0)),
			 new Vertex2D(allDisableButtonPos + new Vector2(38, 0), allDisableButtonColor, new Vector3(74f / Texture.Width, 180f / Texture.Height, 0)),

			 new Vertex2D(allDisableButtonPos + new Vector2(0, 38), allDisableButtonColor, new Vector3(36f / Texture.Width, 218f / Texture.Height, 0)),
			 new Vertex2D(allDisableButtonPos + new Vector2(38, 38), allDisableButtonColor, new Vector3(74f / Texture.Width, 218f / Texture.Height, 0)),
		};
		Ins.Batch.Draw(Texture, allDisableButton, PrimitiveType.TriangleStrip);

		// Score Display
		var scoreColor = new Color(150, 150, 150, 0);
		var scorePos = Position + new Vector2(0, -110);
		int offSetX = 18;
		int vScore = (int)VisualScore;
		DrawNumber(vScore, scorePos + new Vector2(0 + offSetX, -40), 1f, scoreColor);
	}

	public void DrawNumber(int number, Vector2 centerPos, float size, Color color)
	{
		Vector2 numberSize = new Vector2(50, 90) * size;
		List<Vertex2D> scoreDraw = new List<Vertex2D>();
		if (number.ToString().Length == 1)
		{
			Vector2 numberTopLeft = new Vector2(2 + 52 * (number % 10), 254);
			Vector2 drawPos = centerPos;
			scoreDraw = new List<Vertex2D>();
			scoreDraw.Add(drawPos - numberSize * 0.5f, color, new Vector3(numberTopLeft / Texture.Size(), 0));
			scoreDraw.Add(drawPos + new Vector2(numberSize.X, -numberSize.Y) * 0.5f, color, new Vector3((numberTopLeft + new Vector2(numberSize.X, 0)) / Texture.Size(), 0));
			scoreDraw.Add(drawPos + new Vector2(-numberSize.X, numberSize.Y) * 0.5f, color, new Vector3((numberTopLeft + new Vector2(0, numberSize.Y)) / Texture.Size(), 0));
			scoreDraw.Add(drawPos + numberSize * 0.5f, color, new Vector3((numberTopLeft + numberSize) / Texture.Size(), 0));
			Ins.Batch.Draw(Texture, scoreDraw, PrimitiveType.TriangleStrip);
		}
		if (number.ToString().Length == 2)
		{
			int firstNumber = number % 10;
			int secondNumber = ((number - firstNumber) % 100) / 10;
			Vector2 numberTopLeft = new Vector2(2 + 52 * firstNumber, 254);
			Vector2 drawPos = centerPos + new Vector2(27 * size, 0);
			scoreDraw = new List<Vertex2D>();
			scoreDraw.Add(drawPos - numberSize * 0.5f, color, new Vector3(numberTopLeft / Texture.Size(), 0));
			scoreDraw.Add(drawPos + new Vector2(numberSize.X, -numberSize.Y) * 0.5f, color, new Vector3((numberTopLeft + new Vector2(numberSize.X, 0)) / Texture.Size(), 0));
			scoreDraw.Add(drawPos + new Vector2(-numberSize.X, numberSize.Y) * 0.5f, color, new Vector3((numberTopLeft + new Vector2(0, numberSize.Y)) / Texture.Size(), 0));
			scoreDraw.Add(drawPos + numberSize * 0.5f, color, new Vector3((numberTopLeft + numberSize) / Texture.Size(), 0));
			Ins.Batch.Draw(Texture, scoreDraw, PrimitiveType.TriangleStrip);

			numberTopLeft = new Vector2(2 + 52 * secondNumber, 254);
			drawPos = centerPos + new Vector2(-27 * size, 0);
			scoreDraw = new List<Vertex2D>();
			scoreDraw.Add(drawPos - numberSize * 0.5f, color, new Vector3(numberTopLeft / Texture.Size(), 0));
			scoreDraw.Add(drawPos + new Vector2(numberSize.X, -numberSize.Y) * 0.5f, color, new Vector3((numberTopLeft + new Vector2(numberSize.X, 0)) / Texture.Size(), 0));
			scoreDraw.Add(drawPos + new Vector2(-numberSize.X, numberSize.Y) * 0.5f, color, new Vector3((numberTopLeft + new Vector2(0, numberSize.Y)) / Texture.Size(), 0));
			scoreDraw.Add(drawPos + numberSize * 0.5f, color, new Vector3((numberTopLeft + numberSize) / Texture.Size(), 0));
			Ins.Batch.Draw(Texture, scoreDraw, PrimitiveType.TriangleStrip);
		}
		if (number.ToString().Length == 3)
		{
			int firstNumber = number % 10;
			int secondNumber = ((number - firstNumber) % 100) / 10;
			int thirdNumber = (number - secondNumber * 10 - firstNumber) / 100;
			Vector2 numberTopLeft = new Vector2(2 + 52 * firstNumber, 254);
			Vector2 drawPos = centerPos + new Vector2(54 * size, 0);
			scoreDraw = new List<Vertex2D>();
			scoreDraw.Add(drawPos - numberSize * 0.5f, color, new Vector3(numberTopLeft / Texture.Size(), 0));
			scoreDraw.Add(drawPos + new Vector2(numberSize.X, -numberSize.Y) * 0.5f, color, new Vector3((numberTopLeft + new Vector2(numberSize.X, 0)) / Texture.Size(), 0));
			scoreDraw.Add(drawPos + new Vector2(-numberSize.X, numberSize.Y) * 0.5f, color, new Vector3((numberTopLeft + new Vector2(0, numberSize.Y)) / Texture.Size(), 0));
			scoreDraw.Add(drawPos + numberSize * 0.5f, color, new Vector3((numberTopLeft + numberSize) / Texture.Size(), 0));
			Ins.Batch.Draw(Texture, scoreDraw, PrimitiveType.TriangleStrip);

			numberTopLeft = new Vector2(2 + 52 * secondNumber, 254);
			drawPos = centerPos + new Vector2(0, 0);
			scoreDraw = new List<Vertex2D>();
			scoreDraw.Add(drawPos - numberSize * 0.5f, color, new Vector3(numberTopLeft / Texture.Size(), 0));
			scoreDraw.Add(drawPos + new Vector2(numberSize.X, -numberSize.Y) * 0.5f, color, new Vector3((numberTopLeft + new Vector2(numberSize.X, 0)) / Texture.Size(), 0));
			scoreDraw.Add(drawPos + new Vector2(-numberSize.X, numberSize.Y) * 0.5f, color, new Vector3((numberTopLeft + new Vector2(0, numberSize.Y)) / Texture.Size(), 0));
			scoreDraw.Add(drawPos + numberSize * 0.5f, color, new Vector3((numberTopLeft + numberSize) / Texture.Size(), 0));
			Ins.Batch.Draw(Texture, scoreDraw, PrimitiveType.TriangleStrip);

			numberTopLeft = new Vector2(2 + 52 * thirdNumber, 254);
			drawPos = centerPos + new Vector2(-54 * size, 0);
			scoreDraw = new List<Vertex2D>();
			scoreDraw.Add(drawPos - numberSize * 0.5f, color, new Vector3(numberTopLeft / Texture.Size(), 0));
			scoreDraw.Add(drawPos + new Vector2(numberSize.X, -numberSize.Y) * 0.5f, color, new Vector3((numberTopLeft + new Vector2(numberSize.X, 0)) / Texture.Size(), 0));
			scoreDraw.Add(drawPos + new Vector2(-numberSize.X, numberSize.Y) * 0.5f, color, new Vector3((numberTopLeft + new Vector2(0, numberSize.Y)) / Texture.Size(), 0));
			scoreDraw.Add(drawPos + numberSize * 0.5f, color, new Vector3((numberTopLeft + numberSize) / Texture.Size(), 0));
			Ins.Batch.Draw(Texture, scoreDraw, PrimitiveType.TriangleStrip);
		}
	}

	public void DrawRemovingPlatformEffect(string type)
	{
		if (type == "RemoveBottomPlatform" || type == "RemoveTopPlatform")
		{
			float left = 0;
			float right = Main.maxTilesX * 16;
			float posY = 170 * 16;
			if (type == "RemoveBottomPlatform")
			{
				posY = 185 * 16;
			}
			float height = 16;
			float valueTime = (float)Main.timeForVisualEffects * 0.09f;
			valueTime = (MathF.Sin(valueTime) + 1) * 0.5f;
			Color drawColor = Color.Lerp(new Color(1f, 0, 0, 0), new Color(0, 0, 0, 0), valueTime);
			List<Vertex2D> removePlatform = new List<Vertex2D>();
			removePlatform.Add(new Vector2(left, posY), drawColor, new Vector3(0, 0, 0));
			removePlatform.Add(new Vector2(right, posY), drawColor, new Vector3(0, 0, 0));

			removePlatform.Add(new Vector2(left, posY + height), drawColor, new Vector3(0, 0, 0));
			removePlatform.Add(new Vector2(right, posY + height), drawColor, new Vector3(0, 0, 0));
			Ins.Batch.Draw(Texture, removePlatform, PrimitiveType.TriangleStrip);
		}

		if (type == "RemoveLeftPlatform" || type == "RemoveRightPlatform")
		{
			float left = 0;
			if (type == "RemoveRightPlatform")
			{
				left = Main.maxTilesX * 16 / 2;
			}
			float right = Main.maxTilesX * 16;
			if (type == "RemoveLeftPlatform")
			{
				right = Main.maxTilesX * 16 / 2;
			}
			float posY = 170 * 16;
			float height = 16;
			float valueTime = (float)Main.timeForVisualEffects * 0.09f;
			valueTime = (MathF.Sin(valueTime) + 1) * 0.5f;
			Color drawColor = Color.Lerp(new Color(1f, 0, 0, 0), new Color(0, 0, 0, 0), valueTime);
			List<Vertex2D> removePlatform = new List<Vertex2D>();
			removePlatform.Add(new Vector2(left, posY), drawColor, new Vector3(0, 0, 0));
			removePlatform.Add(new Vector2(right, posY), drawColor, new Vector3(0, 0, 0));

			removePlatform.Add(new Vector2(left, posY + height), drawColor, new Vector3(0, 0, 0));
			removePlatform.Add(new Vector2(right, posY + height), drawColor, new Vector3(0, 0, 0));
			Ins.Batch.Draw(Texture, removePlatform, PrimitiveType.TriangleStrip);

			posY = 185 * 16;
			removePlatform = new List<Vertex2D>();
			removePlatform.Add(new Vector2(left, posY), drawColor, new Vector3(0, 0, 0));
			removePlatform.Add(new Vector2(right, posY), drawColor, new Vector3(0, 0, 0));

			removePlatform.Add(new Vector2(left, posY + height), drawColor, new Vector3(0, 0, 0));
			removePlatform.Add(new Vector2(right, posY + height), drawColor, new Vector3(0, 0, 0));
			Ins.Batch.Draw(Texture, removePlatform, PrimitiveType.TriangleStrip);
		}
	}

	public void DrawHealthBar(int backgroundWidth, int backgroundHeight, Vector2 backgroundPos)
	{
		// Health Bar
		int gridSize = 40;
		float innerGridSize = 39;
		Vector2 innerOffset = new Vector2(gridSize - innerGridSize) * 0.5f;
		int oldHealth = TargetBoss.lifeMax;
		if (OldHealthValue.Count > 2)
		{
			oldHealth = OldHealthValue.ToArray()[0];
		}
		float lifeRatio = TargetBoss.life / (float)TargetBoss.lifeMax;
		float oldLifeRatio = oldHealth / (float)TargetBoss.lifeMax;
		int rank = 0;
		int maxRank = backgroundWidth * backgroundHeight / gridSize / gridSize;

		float deltaRatioValue = 1f / maxRank;
		var healthColor = Color.Lerp(new Color(250, 0, 0, 200), new Color(20, 250, 20, 200), lifeRatio);
		Color healthColorOld = Color.Lerp(new Color(250, 0, 0, 200), new Color(20, 250, 20, 200), oldLifeRatio) * 0.5f;
		for (int y = 0; y < backgroundHeight; y += gridSize)
		{
			for (int x = 0; x < backgroundWidth; x += gridSize)
			{
				rank++;
				float scaleFactor = 1f;
				float ratioValue = 1 - rank / (float)maxRank;

				// Old life
				if (oldLifeRatio < ratioValue - deltaRatioValue)
				{
					scaleFactor = 0;
				}
				if (oldLifeRatio > ratioValue - deltaRatioValue && oldLifeRatio < ratioValue)
				{
					scaleFactor = (oldLifeRatio - ratioValue + deltaRatioValue) / (float)deltaRatioValue;
				}
				var healthBarPos = backgroundPos + new Vector2(x, y) + new Vector2(innerGridSize) * 0.5f + innerOffset;
				var healthBar = new List<Vertex2D>()
					{
						 new Vertex2D(healthBarPos + new Vector2(-innerGridSize / 2f, -innerGridSize / 2f) * scaleFactor, healthColorOld, new Vector3(0, 0, 0)),
						 new Vertex2D(healthBarPos + new Vector2(innerGridSize / 2f, -innerGridSize / 2f) * scaleFactor, healthColorOld, new Vector3(0, 0, 0)),

						 new Vertex2D(healthBarPos + new Vector2(-innerGridSize / 2f, innerGridSize / 2f) * scaleFactor, healthColorOld, new Vector3(0, 0, 0)),
						 new Vertex2D(healthBarPos + new Vector2(innerGridSize / 2f, innerGridSize / 2f) * scaleFactor, healthColorOld, new Vector3(0, 0, 0)),
					};
				Ins.Batch.Draw(Texture, healthBar, PrimitiveType.TriangleStrip);

				scaleFactor = 1f;

				// Current life
				if (lifeRatio < ratioValue - deltaRatioValue)
				{
					scaleFactor = 0;
				}
				if (lifeRatio > ratioValue - deltaRatioValue && lifeRatio < ratioValue)
				{
					scaleFactor = (lifeRatio - ratioValue + deltaRatioValue) / (float)deltaRatioValue;
				}

				healthBar = new List<Vertex2D>()
				{
					new Vertex2D(healthBarPos + new Vector2(-innerGridSize / 2f, -innerGridSize / 2f) * scaleFactor, healthColor, new Vector3(0, 0, 0)),
					new Vertex2D(healthBarPos + new Vector2(innerGridSize / 2f, -innerGridSize / 2f) * scaleFactor, healthColor, new Vector3(0, 0, 0)),

					new Vertex2D(healthBarPos + new Vector2(-innerGridSize / 2f, innerGridSize / 2f) * scaleFactor, healthColor, new Vector3(0, 0, 0)),
					new Vertex2D(healthBarPos + new Vector2(innerGridSize / 2f, innerGridSize / 2f) * scaleFactor, healthColor, new Vector3(0, 0, 0)),
				};
				Ins.Batch.Draw(Texture, healthBar, PrimitiveType.TriangleStrip);
			}
		}
	}

	public void DrawTimeBar()
	{
		if (TargetBoss == null)
		{
			return;
		}
		var tNLIY = TargetBoss.ModNPC as TownNPC_LiveInYggdrasil;
		if (tNLIY == null)
		{
			return;
		}
		RemainTime = Math.Max(0, tNLIY.FailTime - tNLIY.BossTimer);
		var timePos = Main.screenPosition + new Vector2(Main.screenWidth / 2, 260);
		DrawNumber(RemainTime / 60, timePos, 1, new Color(1f, 1f, 1f, 1f));
	}

	public bool CanStart()
	{
		if (Main.netMode != NetmodeID.Server)
		{
			foreach (var tag in Tags)
			{
				if (tag.Enable && tag.Name == "3Inventory")
				{
					Player player = Main.LocalPlayer;
					int itemCount = 0;
					foreach (var item in player.inventory)
					{
						if (item.active)
						{
							itemCount++;
						}
					}
					if (itemCount > 3)
					{
						return false;
					}
				}
				if (tag.Enable && tag.Name == "4Accessories")
				{
					Player player = Main.LocalPlayer;
					int itemCount = 0;
					for (int i = 3; i < 9; i++)
					{
						Item item = player.armor[i];
						if (item.active)
						{
							itemCount++;
						}
					}
					if (itemCount > 4)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	public bool MouseInArea(Vector2 drawPos, Vector2 size)
	{
		Vector2 mousePos = Main.MouseWorld;
		if (mousePos.X >= drawPos.X && mousePos.X <= drawPos.X + size.X)
		{
			if (mousePos.Y >= drawPos.Y && mousePos.Y <= drawPos.Y + size.Y)
			{
				return true;
			}
		}
		return false;
	}
}
