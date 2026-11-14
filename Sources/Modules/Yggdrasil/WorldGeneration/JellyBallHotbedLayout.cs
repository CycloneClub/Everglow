namespace Everglow.Yggdrasil.WorldGeneration;

/// <summary>
/// 球冻温床的局部物块布局。只处理材料掩码，不访问 Main，便于离线预览和检查液体密封。
/// </summary>
internal sealed partial class JellyBallHotbedLayout
{
	public const byte Terrain = 0;
	public const byte Air = 1;
	public const byte Jelly = 2;
	public const byte Crystal = 3;
	public const byte Membrane = 4;
	public const byte Fresh = 5;
	public const byte Honey = 8;
	public const byte Protected = 9;
	public const byte CaveAir = 10;

	private static readonly (int X, int Y)[] SurfaceDirections = [(-1, 0), (1, 0), (0, -1), (0, 1)];

	private readonly byte[,] tiles;
	private readonly byte[,] originalTiles;
	private readonly bool[,] excavated;
	private readonly bool[,] neckHoles;
	private readonly bool[,] basinCells;
	private readonly int[] oldTips;
	private readonly int[] terminalBoundary;
	private readonly Random random;
	private readonly int width;
	private readonly int height;
	private readonly int transition;
	private readonly int neckStart;
	private readonly float depth;
	private readonly float center;
	private readonly float phase;
	private readonly List<Cell> cells = new();
	private readonly List<Cell> nucleusCandidates = new();
	private float[] fittedFloor;

	private readonly record struct Cell(float X, float Y, float RadiusX, float RadiusY, float Phase)
	{
		public float? SplashAngle { get; init; }

		public float? GrowthAngle { get; init; }
	}

	private JellyBallHotbedLayout(byte[,] source, int seed)
	{
		tiles = (byte[,])source.Clone();
		originalTiles = (byte[,])source.Clone();
		excavated = new bool[source.GetLength(0), source.GetLength(1)];
		random = new Random(seed);
		width = tiles.GetLength(0);
		height = tiles.GetLength(1);
		center = width * 0.5f;
		phase = random.NextSingle() * MathF.Tau;

		// 忽略最末端的零星孤点，实际球冻块的行宽决定衔接位置，而不是原生成循环的 bottomBound。
		int occupiedBottom = height / 2;
		for (int y = height / 3; y < height - 8; y++)
		{
			int count = 0;
			for (int x = 4; x < width - 4; x++)
			{
				if (source[x, y] == Jelly)
				{
					count++;
				}
			}
			if (count >= 8)
			{
				occupiedBottom = y;
			}
		}
		transition = Math.Clamp(occupiedBottom - 52, (int)(height * 0.46f), (int)(height * 0.67f));
		neckStart = occupiedBottom - 6;
		depth = height - 10 - transition;
		oldTips = new int[width];
		terminalBoundary = new int[width];
		Array.Fill(oldTips, -1);
		for (int x = 4; x < width - 4; x++)
		{
			for (int y = Math.Max(4, transition - 60); y <= Math.Min(height - 5, occupiedBottom + 8); y++)
			{
				if (source[x, y] == Jelly && (source[x - 1, y] == Jelly || source[x + 1, y] == Jelly))
				{
					oldTips[x] = y;
				}
			}
		}
		for (int x = 0; x < width; x++)
		{
			int nearest = Enumerable.Range(4, width - 8).Where(column => oldTips[column] >= 0)
				.OrderBy(column => Math.Abs(column - x)).FirstOrDefault(-1);
			terminalBoundary[x] = nearest < 0 ? neckStart : oldTips[nearest] - 6;
		}
		neckHoles = new bool[width, height];
		basinCells = new bool[width, height];
		// 小孔先占位，细胞完整地绕孔搭接，后续不能从细胞中挖掉像素。
		for (int y = neckStart + 4; y < Math.Min(height - 8, transition + depth * 0.43f); y += random.Next(9, 16))
		{
			float bend = center + 7 * MathF.Sin(y * 0.055f + phase) + 3 * MathF.Sin(y * 0.14f);
			int radius = random.Next(1, 3);
			for (int x = (int)(bend - radius); x <= bend + radius; x++)
			{
				for (int dy = -2; dy <= 2; dy++)
				{
					if ((x - bend) * (x - bend) / (radius * radius) + dy * dy / 4f < 1)
					{
						neckHoles[x, y + dy] = true;
					}
				}
			}
		}
	}

	public static byte[,] Generate(byte[,] source, int seed)
	{
		ArgumentNullException.ThrowIfNull(source);
		if (source.GetLength(0) < 160 || source.GetLength(1) < 240)
		{
			throw new ArgumentException("The hotbed needs at least 160 by 240 tiles.", nameof(source));
		}
		var layout = new JellyBallHotbedLayout(source, seed);
		layout.GrowCells();
		layout.FitPoolFloor();
		layout.CarveChamber();
		layout.BuildPool();
		layout.FillPackingPockets();
		layout.ScatterPeripheralCells();
		layout.FreshenSurfaces();
		layout.PlaceCrystals();
		return layout.tiles;
	}

	private float OuterTop(float x)
	{
		float u = MathF.Abs(x - center) / (width * 0.46f);
		return transition + depth * (0.03f + 0.55f * MathF.Pow(u, 0.72f) + 0.045f * MathF.Sin(u * MathF.PI));
	}

	private float OuterBottom(float x)
	{
		float u = MathF.Abs(x - center) / (width * 0.46f);
		return transition + depth * (0.96f - 0.33f * MathF.Pow(u, 0.78f) + 0.06f * MathF.Sin(u * MathF.PI));
	}

	private float PoolSurface => transition + depth * 0.64f;

	private float BasinFloor(float x)
	{
		float u = (x - center) / (width * 0.29f);
		// 大尺度下凹盆形承载水平液面；缓慢起伏跨越小缝隙，不逐格复制细胞轮廓。
		return PoolSurface - 5 + depth * 0.19f * (1 - MathF.Pow(MathF.Abs(u), 2.8f))
			+ 1.8f * MathF.Sin(x * 0.085f + phase) + 1.2f * MathF.Sin(x * 0.041f - phase);
	}

	private void FitPoolFloor()
	{
		// 直接寻找堆叠细胞的实际上表面；盆形公式只参与细胞中心排布。
		fittedFloor = new float[width];
		for (int x = 4; x < width - 4; x++)
		{
			int y = Math.Max(4, (int)PoolSurface - 25);
			while (y < height - 5 && !basinCells[x, y])
			{
				y++;
			}
			fittedFloor[x] = y;
		}
	}

	private float ChamberRoof(float x)
	{
		float u = MathF.Abs(x - center - 4 * MathF.Sin(x * 0.035f)) / (width * 0.29f);
		// 顶点靠近旧区末端，保留实体衔接；大细胞的腹部也不能压低中央高耸空腔。
		float capThickness = PoolSurface - neckStart < 36 ? 18 : 12;
		float apex = Math.Max(neckStart + capThickness, transition + depth * 0.22f);
		float shoulder = Math.Max(apex + 2, transition + depth * 0.62f);
		// 外侧三分之二缓升，约六成的高度变化集中在靠中心的三分之一。
		float profile = MathF.Log(1 + 12 * u) / MathF.Log(13);
		return apex + (shoulder - apex) * profile
			+ 2 * u * MathF.Sin(x * 0.11f + phase);
	}

	private void GrowCells()
	{
		// 新区域是完整洞穴，清除细胞间原生岩土；旧区中央仅允许末端衔接。
		for (int x = 6; x < width - 6; x++)
		{
			float tip = Math.Max(0, MathF.Abs(x - center) / (width * 0.45f) - 0.88f) * 9;
			float edgeNoise = 3 * MathF.Sin(x * 0.12f + phase) + 2 * MathF.Sin(x * 0.29f - phase);
			int top = Math.Max(transition, (int)(OuterTop(x) - 7 + edgeNoise + tip * tip * 15));
			int bottom = Math.Min(height - 6, (int)(OuterBottom(x) + 7 + edgeNoise - tip * tip * 15));
			for (int y = top; y <= bottom; y++)
			{
				if (tiles[x, y] is Terrain or Air)
				{
					Put(x, y, CaveAir);
				}
			}
		}
		// 侧壁的大细胞嵌入连续基底；上方过渡组织不再叠加带核的体积采样层。
		for (float y = transition + 8; y < height - 14; y += random.Next(25, 31))
		{
			for (float x = 18 + random.Next(18); x < width - 14; x += random.Next(26, 35))
			{
				float px = x + random.Next(-7, 8);
				float py = y + random.Next(-8, 9);
				if (py < OuterTop(px) || py > OuterBottom(px))
				{
					continue;
				}
				// 洞顶只保留下方单独布置的少数细胞，池底也只保留独立承托层。
				if (MathF.Abs(px - center) < width * 0.30f)
				{
					continue;
				}
				bool small = random.Next(6) == 0;
				float rx = small ? random.Next(7, 11) : random.Next(11, 19);
				float ry = small ? random.Next(9, 14) : random.Next(14, 24);
				if (py < PoolSurface - 15)
				{
					ry *= 1.25f;
				}
				AddCell(CreateBodyCell(px, py, rx, ry, random.NextSingle() * MathF.Tau));
			}
		}

		// 接口由搭接液滴构成近乎实心的组织，不是预先挖出的竖井。
		float neckEnd = Math.Min(height - 16, Math.Max(neckStart + 33, transition + depth * 0.43f + 8));
		for (float y = neckStart + 10; y < neckEnd; y += 16)
		{
			for (int side = -1; side <= 1; side++)
			{
				AddCell(new Cell(center + side * 8 + random.Next(-2, 3), y, random.Next(9, 13), random.Next(12, 17), random.NextSingle() * MathF.Tau), true, nucleus: false);
			}
		}

		// 育生池下增加相互搭接的承托细胞，避免膜成为悬在零星小滴上的薄吊床。
		for (int x = (int)(center - width * 0.28f); x < center + width * 0.28f; x += random.Next(22, 34))
		{
			float radiusY = random.Next(12, 28);
			float y = BasinFloor(x) + Math.Max(9, radiusY * 0.72f + random.Next(-2, 4));
			AddCell(CreateBodyCell(x + random.Next(-4, 5), y, random.Next(12, 22), radiusY, random.NextSingle() * MathF.Tau), true);
		}
		// 池口两侧由完整圆润支撑体承接膜，避免较疏的池底排列留下竖直膜沿。
		foreach (int side in new[] { -1, 1 })
		{
			AddCell(new Cell(center + side * width * 0.30f, PoolSurface + 8, 12, 14, 0), true, nucleus: false);
		}

		// 曲线只引导普通细胞的中心分布，轮廓由整颗细胞的并集决定。
		// 大小、间距和偏移均独立变化，允许完整细胞伸入曲线内侧。
		for (float x = center - width * 0.27f; x <= center + width * 0.29f; x += random.Next(42, 57))
		{
			AddCell(CreateBodyCell(x, ChamberRoof(x) - 20 + random.Next(-6, 7), random.Next(16, 23), random.Next(19, 27), random.NextSingle() * MathF.Tau), true);
		}
		PackCellWall();
		// 极端错位时先补整颗承托细胞，不能把找不到支撑误判成世界底边。
		for (int x = (int)(center - width * 0.29f); x <= center + width * 0.29f; x++)
		{
			bool supported = false;
			for (int y = Math.Max(4, (int)PoolSurface - 25); y < height - 5; y++)
			{
				supported |= basinCells[x, y];
			}
			if (!supported)
			{
				AddCell(new Cell(x, BasinFloor(x) + 14, 12, 16, random.NextSingle() * MathF.Tau), true, nucleus: false);
			}
		}
		ConnectOldTips();
		SealCellPacking();
		PackBasinContacts();
		GrowBasalBuds();
	}

	private void GrowBasalBuds()
	{
		// 参考图下沿有少量窄根圆鼓的凸起。沿实际组织边缘寻找搭接点，
		// 不再用另一圈中心线把它们整排埋入洞壁，也不裁切任何已有细胞。
		var budRandom = new Random(BitConverter.SingleToInt32Bits(phase) ^ 0x5B18);
		var anchors = Enumerable.Range((int)(width * 0.12f), (int)(width * 0.76f))
			.OrderBy(_ => budRandom.Next()).ToArray();
		var placed = new List<float>();
		foreach (int x in anchors)
		{
			if (placed.Count >= 3 || placed.Any(previous => MathF.Abs(previous - x) < width * 0.16f))
			{
				continue;
			}
			int edge = height - 5;
			while (edge > PoolSurface && !IsTissue(x, edge))
			{
				edge--;
			}
			if (edge <= PoolSurface || edge > height - 23)
			{
				continue;
			}
			float radius = budRandom.Next(7, 12);
			float angle = Math.Clamp((x - center) / width, -0.4f, 0.4f);
			for (int offset = 5; offset <= 17; offset++)
			{
				var cell = new Cell(x + MathF.Sin(angle) * offset, edge + offset, radius, radius * 1.25f, budRandom.NextSingle() * MathF.Tau)
				{
					SplashAngle = angle,
				};
				var footprint = CellFootprint(cell);
				if (footprint.Any(point => !CanPlaceCellPixel(point.X, point.Y, false)
					|| tiles[point.X, point.Y] is not (Terrain or Jelly)))
				{
					continue;
				}
				int overlap = footprint.Count(point => IsTissue(point.X, point.Y));
				if (overlap < footprint.Count * 0.035f || overlap > footprint.Count * 0.12f)
				{
					continue;
				}
				cells.Add(cell);
				nucleusCandidates.Add(cell);
				foreach (var point in footprint)
				{
					tiles[point.X, point.Y] = Jelly;
				}
				placed.Add(x);
				break;
			}
		}
	}

	private void PackBasinContacts()
	{
		// 只填池底相邻细胞之间的尖窄谷，最多向上补九格，保留大尺度丘谷和完整外露下沿。
		int left = (int)(center - width * 0.29f);
		int right = (int)(center + width * 0.29f);
		var floor = new int[width];
		for (int x = left; x <= right; x++)
		{
			int y = Math.Max(4, (int)PoolSurface - 25);
			while (y < height - 5 && !basinCells[x, y])
			{
				y++;
			}
			floor[x] = y;
		}
		for (int x = left; x <= right; x++)
		{
			int top = floor[x];
			for (int dx = -6; dx <= 6; dx++)
			{
				if (x + dx >= left && x + dx <= right)
				{
					top = Math.Min(top, floor[x + dx] + Math.Abs(dx));
				}
			}
			top = Math.Max(top, Math.Max(floor[x] - 9, (int)PoolSurface + 9));
			for (int y = top; y < floor[x]; y++)
			{
				if (tiles[x, y] is Terrain or Air or CaveAir)
				{
					Put(x, y, Jelly);
					basinCells[x, y] = true;
				}
			}
		}
	}

	private Cell CreateBodyCell(float x, float y, float radiusX, float radiusY, float cellPhase)
	{
		float side = Math.Clamp((x - center) / (width * 0.45f), -1, 1);
		return new Cell(x, y, radiusX, radiusY, cellPhase)
		{
			GrowthAngle = side * (y < PoolSurface ? 0.9f : 0.65f) + 0.25f * MathF.Sin(cellPhase),
		};
	}

	private void ScatterPeripheralCells()
	{
		// 外围装饰独立取随机数，不改变主体后续表面与细胞核的随机序列。
		var scatterRandom = new Random(BitConverter.SingleToInt32Bits(phase));
		// 洞壁先完整搭好，再沿外坡放置疏落的小群，避免补墙把外围小滴重新吞进主体。
		// 近壁较大，沿坡面外法线向外逐渐变小；各群错开，不排成等距的一圈。
		foreach (int side in new[] { -1, 1 })
		{
			for (int band = 0; band < 3; band++)
			{
				foreach (bool upper in new[] { true, false })
				{
					float x = center + side * width * (0.20f + band * 0.10f + scatterRandom.NextSingle() * 0.055f);
					float y = upper ? OuterTop(x) : OuterBottom(x);
					float slope = upper ? (OuterTop(x + 2) - OuterTop(x - 2)) / 4 : (OuterBottom(x + 2) - OuterBottom(x - 2)) / 4;
					float normalX = upper ? slope : -slope;
					float normalY = upper ? -1 : 1;
					float length = MathF.Sqrt(normalX * normalX + 1);
					normalX /= length;
					normalY /= length;
					float distance = 5;
					int count = scatterRandom.Next(2, 4);
					for (int drop = 0; drop < count; drop++)
					{
						float radius = (5.5f + scatterRandom.NextSingle() * 2) * MathF.Pow(0.68f, drop);
						float drift = (scatterRandom.NextSingle() - 0.5f) * 7;
						for (; distance < 60; distance += 2)
						{
							var cell = new Cell(x + normalX * distance - normalY * drift, y + normalY * distance + normalX * drift,
								radius, radius * (1.05f + scatterRandom.NextSingle() * 0.35f), scatterRandom.NextSingle() * MathF.Tau);
							var footprint = CellFootprint(cell);
							if (footprint.Any(point => !CanPlaceCellPixel(point.X, point.Y, false) || point.Y < neckStart - 12))
							{
								break;
							}
							if (footprint.Any(point => tiles[point.X, point.Y] is not (Terrain or Air)
								|| Enumerable.Range(-4, 9).Any(dx => Enumerable.Range(-4, 9).Any(dy => dx * dx + dy * dy <= 16
									&& tiles[point.X + dx, point.Y + dy] is Jelly or Fresh or Membrane or Honey))))
							{
								continue;
							}
							// 只放完整且与主体留有间距的细胞；到达世界边界就停止，不切边。
							AddCell(cell);
							distance += radius * 2 + scatterRandom.Next(5, 10);
							break;
						}
					}
				}
			}
		}
		ScatterOldLowerEdge();
	}

	private void ScatterOldLowerEdge()
	{
		// BuildJellyBallHotbed 的放射中心在局部图 (175, 120)。右侧预留二十格，所以比可写区域中心偏右十格。
		float originX = center + 10;
		const float originY = 120;
		var scatterRandom = new Random(BitConverter.SingleToInt32Bits(phase) ^ 0x4F1BBCDC);
		// 以正下方为中心的 120 度扇区；每个角度先找原地形的最外端，不能沿内部裂缝播种。
		for (int sector = 0; sector < 10; sector++)
		{
			float angle = (-54 + sector * 12 + scatterRandom.NextSingle() * 6 - 3) * MathF.PI / 180;
			float dx = MathF.Sin(angle);
			float dy = MathF.Cos(angle);
			float edge = -1;
			for (float distance = 30; distance < height; distance++)
			{
				int x = (int)MathF.Round(originX + dx * distance);
				int y = (int)MathF.Round(originY + dy * distance);
				if (x <= 4 || x >= width - 5 || y >= height - 5)
				{
					break;
				}
				if (originalTiles[x, y] == Jelly && SurfaceDirections.Any(step => originalTiles[x + step.X, y + step.Y] == Jelly))
				{
					edge = distance;
				}
			}
			if (edge < 0)
			{
				continue;
			}
			float offset = scatterRandom.Next(9, 16);
			int count = scatterRandom.Next(1, 3);
			for (int drop = 0; drop < count; drop++)
			{
				float radius = (4.5f + scatterRandom.NextSingle() * 2) * MathF.Pow(0.7f, drop);
				float stretch = 1.65f + scatterRandom.NextSingle() * 0.65f;
				float cellPhase = scatterRandom.NextSingle() * MathF.Tau;
				for (; offset <= 48; offset += 2)
				{
					var cell = new Cell(originX + dx * (edge + offset), originY + dy * (edge + offset), radius, radius * stretch, cellPhase)
					{
						SplashAngle = angle + 0.18f * MathF.Sin(cellPhase),
					};
					var footprint = CellFootprint(cell);
					if (footprint.Any(point => point.X <= 4 || point.X >= width - 5 || point.Y <= 4 || point.Y >= height - 5
						|| MathF.Abs(MathF.Atan2(point.X - originX, point.Y - originY)) > MathF.PI / 3))
					{
						break;
					}
					if (footprint.Any(point => tiles[point.X, point.Y] is not (Terrain or Air)
						|| Enumerable.Range(-4, 9).Any(mx => Enumerable.Range(-4, 9).Any(my => mx * mx + my * my <= 16
							&& tiles[point.X + mx, point.Y + my] is Jelly or Fresh or Membrane or Honey))))
					{
						continue;
					}
					// 独立装饰可以位于旧区下沿较高处；完整检查后放置，不受下方洞穴的颈部裁界限制。
					cells.Add(cell);
					nucleusCandidates.Add(cell);
					foreach (var point in footprint)
					{
						tiles[point.X, point.Y] = Jelly;
					}
					offset += radius * 2 + scatterRandom.Next(6, 11);
					break;
				}
			}
		}
	}

	private void ConnectOldTips()
	{
		// 仅沿中央通道两侧的真实末端搭接，外围保留旧区和下部细胞间的自然分离。
		for (int column = (int)(center - width * 0.14f); column <= center + width * 0.14f; column += 16)
		{
			int anchor = Enumerable.Range(Math.Max(4, column - 7), Math.Min(width - 5, column + 7) - Math.Max(4, column - 7) + 1)
				.Where(x => oldTips[x] >= 0).OrderBy(x => Math.Abs(x - column)).FirstOrDefault(-1);
			if (anchor < 0)
			{
				continue;
			}
			float end = Math.Max(neckStart + 10, OuterTop(column) + 8);
			float start = oldTips[anchor] + 6;
			int count = end - start <= 8 ? 0 : (int)MathF.Ceiling((end - start) / 28);
			for (int step = 0; step <= count; step++)
			{
				float t = count == 0 ? 0 : step / (float)count;
				float x = anchor + (column - anchor) * t + 2 * MathF.Sin(t * MathF.PI + phase);
				float y = start + Math.Max(0, end - start) * t;
				// 衔接旧区的是连续延伸组织，不把每一节搭接体都当作独立带核细胞。
				AddCell(new Cell(x, y, random.Next(11, 16), random.Next(18, 26), random.NextSingle() * MathF.Tau), true, nucleus: false, terminal: true);
			}
		}
	}

	private void SealCellPacking()
	{
		// 对实体并集做小范围闭运算，只增加搭接组织，不削去任何原有细胞。
		// 填平相邻圆润细胞间的三角缝，并把相隔数格的细胞接起来。
		const int radius = 2;
		var expanded = new bool[width, height];
		for (int x = 4; x < width - 4; x++)
		{
			for (int y = neckStart; y < height - 4; y++)
			{
				if (!IsTissue(x, y))
				{
					continue;
				}
				for (int dx = -radius; dx <= radius; dx++)
				{
					for (int dy = -radius; dy <= radius; dy++)
					{
						if (dx * dx + dy * dy <= radius * radius && x + dx >= 0 && x + dx < width && y + dy >= 0 && y + dy < height)
						{
							expanded[x + dx, y + dy] = true;
						}
					}
				}
			}
		}
		for (int x = radius + 1; x < width - radius - 1; x++)
		{
			for (int y = neckStart + radius; y < height - radius - 1; y++)
			{
				if (!excavated[x, y] || tiles[x, y] != CaveAir || neckHoles[x, y]
					|| (Math.Abs(x - center) < 14 && y < PoolSurface))
				{
					continue;
				}
				bool enclosed = true;
				bool basal = false;
				for (int dx = -radius; dx <= radius && enclosed; dx++)
				{
					for (int dy = -radius; dy <= radius; dy++)
					{
						if (dx * dx + dy * dy <= radius * radius)
						{
							enclosed &= expanded[x + dx, y + dy];
							basal |= basinCells[x + dx, y + dy];
						}
					}
				}
				if (enclosed)
				{
					Put(x, y, Jelly);
					basinCells[x, y] = basal;
				}
			}
		}
		FillPackingPockets();
	}

	private void FillPackingPockets()
	{
		// 较大的完全封闭夹层同样填实；保留主洞、外界和设计中的颈部小孔。
		var visited = new bool[width, height];
		for (int x = 1; x < width - 1; x++)
		{
			for (int y = neckStart; y < height - 1; y++)
			{
				if (visited[x, y] || !IsOpen(tiles[x, y]))
				{
					continue;
				}
				var region = new List<(int X, int Y)>();
				var queue = new Queue<(int X, int Y)>();
				queue.Enqueue((x, y));
				visited[x, y] = true;
				bool keep = false;
				while (queue.TryDequeue(out var point))
				{
					region.Add(point);
					keep |= !excavated[point.X, point.Y] || neckHoles[point.X, point.Y] || tiles[point.X, point.Y] == Honey
						|| (Math.Abs(point.X - center) < width * 0.25f
							&& point.Y > ChamberRoof(point.X) + 6 && point.Y < PoolSurface + 5);
					foreach (var direction in SurfaceDirections)
					{
						int px = point.X + direction.X;
						int py = point.Y + direction.Y;
						if (px >= 0 && px < width && py >= 0 && py < height && !visited[px, py] && IsOpen(tiles[px, py]))
						{
							visited[px, py] = true;
							queue.Enqueue((px, py));
						}
					}
				}
				if (!keep)
				{
					foreach (var point in region)
					{
						Put(point.X, point.Y, Jelly);
					}
				}
			}
		}
	}

	private void PackCellWall()
	{
		// 洞内空腔之外的开挖区域全部由互相搭接的细胞承托。
		// 包括朝向原木的一侧，不能仅沿洞内边缘铺一串悬空颗粒。
		for (int y = transition; y < height - 4; y++)
		{
			for (int x = 4; x < width - 4; x++)
			{
				if (!excavated[x, y] || tiles[x, y] is not (Air or CaveAir))
				{
					continue;
				}
				// 留出一整层细胞宽度，不用补缝小细胞把洞内轮廓抹成曲线。
				bool innerLayer = MathF.Abs(x - center) <= width * 0.29f
					&& y >= ChamberRoof(x) - 26 && y < BasinFloor(x) + 24;
				if (!innerLayer && !ReservedVoid(x, y))
				{
					AddCell(new Cell(x, y, random.Next(3, 7), random.Next(4, 8), random.NextSingle() * MathF.Tau), true, true);
				}
			}
		}
	}

	private bool ReservedVoid(int x, int y)
	{
		return neckHoles[x, y] || (MathF.Abs(x - center) <= width * 0.29f
			&& y >= (int)ChamberRoof(x) && y < BasinFloor(x) + 2);
	}

	private List<(int X, int Y)> CellFootprint(Cell cell)
	{
		if (cell.SplashAngle.HasValue)
		{
			return SplashCellFootprint(cell);
		}
		var footprint = new List<(int X, int Y)>();
		float reach = Math.Max(cell.RadiusX, cell.RadiusY) * 1.3f;
		float tilt = cell.GrowthAngle.HasValue ? -cell.GrowthAngle.Value : MathF.Sin(cell.Phase) * 0.55f;
		float cosine = MathF.Cos(tilt);
		float sine = MathF.Sin(tilt);
		for (int x = (int)MathF.Floor(cell.X - reach); x <= cell.X + reach; x++)
		{
			for (int y = (int)MathF.Floor(cell.Y - reach); y <= cell.Y + reach; y++)
			{
				float dy = ((y - cell.Y) * cosine - (x - cell.X) * sine) / cell.RadiusY;
				float dx = ((x - cell.X) * cosine + (y - cell.Y) * sine) / cell.RadiusX;
				// 完整的圆润卵形与液滴，仅保留低频的不对称。
				dx /= cell.GrowthAngle.HasValue ? 0.94f + 0.24f * dy + 0.08f * MathF.Sin(dy * 3 + cell.Phase) : 0.98f + 0.10f * dy;
				float angle = MathF.Atan2(dy, dx);
				float variation = cell.GrowthAngle.HasValue ? 2.6f : 1;
				float boundary = 1 + variation * (0.045f * MathF.Sin(angle * 2 + cell.Phase) + 0.025f * MathF.Cos(angle * 3 - cell.Phase));
				if (dx * dx + dy * dy < boundary * boundary)
				{
					footprint.Add((x, y));
				}
			}
		}
		return footprint;
	}

	private List<(int X, int Y)> SplashCellFootprint(Cell cell)
	{
		var footprint = new List<(int X, int Y)>();
		float directionX = MathF.Sin(cell.SplashAngle.Value);
		float directionY = MathF.Cos(cell.SplashAngle.Value);
		float reach = cell.RadiusY * 1.3f;
		for (int x = (int)MathF.Floor(cell.X - reach); x <= cell.X + reach; x++)
		{
			for (int y = (int)MathF.Floor(cell.Y - reach); y <= cell.Y + reach; y++)
			{
				float along = ((x - cell.X) * directionX + (y - cell.Y) * directionY) / cell.RadiusY;
				if (MathF.Abs(along) >= 1)
				{
					continue;
				}
				// 朝外圆鼓、朝内收尖；尾部轻微弯曲，保持完整液滴而非拉长的椭圆。
				float tail = Math.Max(0, -along);
				float bend = 0.28f * MathF.Cos(cell.Phase * 1.7f) * tail * tail;
				float across = ((x - cell.X) * directionY - (y - cell.Y) * directionX) / cell.RadiusX - bend;
				float taper = (0.85f + 0.65f * along) * (1 + 0.04f * MathF.Sin(cell.Phase + along * 3));
				if (across * across < (1 - along * along) * taper * taper)
				{
					footprint.Add((x, y));
				}
			}
		}
		return footprint;
	}

	private bool CanPlaceCellPixel(int x, int y, bool terminal)
	{
		return x > 3 && x < width - 4 && y >= Math.Max(4, transition - (terminal ? 60 : 42)) && y < height - 4
			&& (terminal ? tiles[x, y] == Jelly || y >= terminalBoundary[x] : !(MathF.Abs(x - center) < width * 0.16f && y < neckStart))
			&& tiles[x, y] != Protected && !neckHoles[x, y];
	}

	private void AddCell(Cell cell, bool connecting = false, bool wallCell = false, bool nucleus = true, bool terminal = false)
	{
		// 只约束中心，不能逐像素检测穹顶曲线，否则搭出来的边缘仍会被拟合平滑。
		if (MathF.Abs(cell.X - center) <= width * 0.29f
			&& cell.Y > ChamberRoof(cell.X) - 6 && cell.Y < BasinFloor(cell.X) + 8)
		{
			return;
		}
		if (!connecting && cells.Any(other => MathF.Pow((cell.X - other.X) / (cell.RadiusX + other.RadiusX), 2) + MathF.Pow((cell.Y - other.Y) / (cell.RadiusY + other.RadiusY), 2) < 0.40f))
		{
			return;
		}
		Cell original = cell;
		bool basal = cell.Y >= BasinFloor(cell.X) + 8;
		// 先挪动整颗细胞，再逐渐缩小；绝不逐像素裁切后硬塞进边界。
		for (int attempt = 0; attempt < 24; attempt++)
		{
			var footprint = CellFootprint(cell);
			var conflicts = footprint.Where(point => !CanPlaceCellPixel(point.X, point.Y, terminal)
				|| (basal && Math.Abs(point.X - center) < width * 0.25f && point.Y < PoolSurface + 9)).ToArray();
			if (conflicts.Length == 0 && footprint.Count > 0)
			{
				cells.Add(cell);
				if (!wallCell && nucleus)
				{
					nucleusCandidates.Add(cell);
				}
				foreach (var point in footprint)
				{
					tiles[point.X, point.Y] = Jelly;
					if (basal)
					{
						basinCells[point.X, point.Y] = true;
					}
				}
				return;
			}
			if (attempt % 4 == 3)
			{
				cell = original with { RadiusX = cell.RadiusX * 0.82f, RadiusY = cell.RadiusY * 0.82f };
				if (Math.Min(cell.RadiusX, cell.RadiusY) < (wallCell ? 2.5f : 4))
				{
					return;
				}
			}
			else
			{
				float dx = cell.X - conflicts.Average(point => (float)point.X);
				float dy = cell.Y - conflicts.Average(point => (float)point.Y);
				float length = MathF.Sqrt(dx * dx + dy * dy);
				if (length < 0.01f)
				{
					return;
				}
				cell = cell with { X = cell.X + dx / length * 2, Y = cell.Y + dy / length * 2 };
			}
		}
	}

	private void CarveChamber()
	{
		// 只移除预留空腔内的原生岩土，已摆放的细胞完整保留。
		for (int x = 4; x < width - 4; x++)
		{
			for (int y = Math.Max(4, neckStart); y < height - 4; y++)
			{
				if (ReservedVoid(x, y) && tiles[x, y] is Terrain or Air or CaveAir)
				{
					Put(x, y, CaveAir);
				}
			}
		}
	}

	private void BuildPool()
	{
		int left = (int)(center - width * 0.29f);
		int right = (int)(center + width * 0.29f);
		int surface = (int)PoolSurface;
		const int thickness = 4;
		// 到池底实体的等距层，而非每列向上铺相同格数；斜坡和凹角也保持约四格法向厚度。
		// 两侧膜沿作为同一层连续向上折回，高于液面五格，液体只填膜内侧。
		for (int x = left; x <= right; x++)
		{
			int bottom = (int)fittedFloor[x];
			for (int y = surface - 5 - thickness; y < bottom; y++)
			{
				if (IsTissue(x, y))
				{
					continue;
				}
				int edgeDistance = Math.Min(x - left + 1, right - x + 1);
				int edgeHeight = Math.Max(0, surface - 5 - y);
				float distanceSquared = edgeDistance * edgeDistance + edgeHeight * edgeHeight;
				for (int dx = -thickness; dx <= thickness; dx++)
				{
					int sx = x + dx;
					if (sx >= left && sx <= right)
					{
						float dy = Math.Max(0, fittedFloor[sx] - y);
						distanceSquared = Math.Min(distanceSquared, dx * dx + dy * dy);
					}
				}
				if (distanceSquared <= thickness * thickness)
				{
					Put(x, y, Membrane);
				}
			}
		}
		// 从池中央灌入连通内腔，不在膜外或另一侧的空隙中单独生成液体。
		var queue = new Queue<(int X, int Y)>();
		var visited = new bool[width, height];
		queue.Enqueue(((int)center, surface));
		visited[(int)center, surface] = true;
		while (queue.TryDequeue(out var point))
		{
			if (tiles[point.X, point.Y] is not (Air or CaveAir))
			{
				continue;
			}
			Put(point.X, point.Y, Honey);
			foreach (var direction in SurfaceDirections)
			{
				int x = point.X + direction.X;
				int y = point.Y + direction.Y;
				if (x > left && x < right && y >= surface && y < height - 4 && !visited[x, y])
				{
					visited[x, y] = true;
					queue.Enqueue((x, y));
				}
			}
		}
	}

	private void FreshenSurfaces()
	{
		var original = (byte[,])tiles.Clone();
		// 二维斑块决定浸润团的宽厚与凹凸，表面只是入口，不能把替换限制成一两格描边。
		var patch = new float[width, height];
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				float px = x + 4 * MathF.Sin(y * 0.08f + phase);
				float py = y + 3 * MathF.Sin(x * 0.11f - phase);
				patch[x, y] = FreshPatchNoise(px / 18, py / 18) * 0.8f + FreshPatchNoise(px / 7, py / 7) * 0.2f;
			}
		}
		GrowFreshPatches(original, patch, false, 0.59f);
		GrowFreshPatches(original, patch, true, 0.65f);
	}

	private float FreshPatchNoise(float x, float y)
	{
		int ix = (int)MathF.Floor(x);
		int iy = (int)MathF.Floor(y);
		float tx = x - ix;
		float ty = y - iy;
		tx = tx * tx * (3 - 2 * tx);
		ty = ty * ty * (3 - 2 * ty);
		float upper = FreshPatchHash(ix, iy) * (1 - tx) + FreshPatchHash(ix + 1, iy) * tx;
		float lower = FreshPatchHash(ix, iy + 1) * (1 - tx) + FreshPatchHash(ix + 1, iy + 1) * tx;
		return upper * (1 - ty) + lower * ty;
	}

	private float FreshPatchHash(int x, int y)
	{
		// 坐标决定斑块，不消耗细胞的随机序列；外围增加孤立细胞也不会重排主体斑块。
		uint hash = unchecked((uint)x * 0x9E3779B9u ^ (uint)y * 0x85EBCA6Bu ^ (uint)BitConverter.SingleToInt32Bits(phase));
		hash ^= hash >> 16;
		hash = unchecked(hash * 0x7FEB352Du);
		hash ^= hash >> 15;
		hash = unchecked(hash * 0x846CA68Bu);
		hash ^= hash >> 16;
		return (hash & 0xFFFFFF) / 16777215f;
	}

	private void GrowFreshPatches(byte[,] original, float[,] patch, bool membrane, float threshold)
	{
		const int maximumDepth = 11;
		var visited = new bool[width, height];
		var queue = new Queue<(int X, int Y, int Depth)>();
		for (int x = 4; x < width - 4; x++)
		{
			for (int y = 4; y < height - 4; y++)
			{
				if (original[x, y] != Jelly || patch[x, y] < threshold)
				{
					continue;
				}
				if (SurfaceDirections.Any(direction => membrane
					? original[x + direction.X, y + direction.Y] == Membrane
					: IsOpen(original[x + direction.X, y + direction.Y])))
				{
					visited[x, y] = true;
					queue.Enqueue((x, y, 0));
				}
			}
		}
		while (queue.TryDequeue(out var point))
		{
			if (point.Depth < maximumDepth)
			{
				foreach (var direction in SurfaceDirections)
				{
					int x = point.X + direction.X;
					int y = point.Y + direction.Y;
					int depth = point.Depth + 1;
					// 只沿相连球冻组织浸润，不能穿过原生土层、空洞或膜去染另一侧。
					if (x >= 4 && x < width - 4 && y >= 4 && y < height - 4 && !visited[x, y]
						&& original[x, y] == Jelly && patch[x, y] >= threshold + depth * 0.006f)
					{
						visited[x, y] = true;
						queue.Enqueue((x, y, depth));
					}
				}
			}
		}
		ApplyFreshPatch(original, visited, membrane);
	}

	private void ApplyFreshPatch(byte[,] original, bool[,] region, bool membrane)
	{
		// 只柔化组织内部的交界；朝向空腔和膜的表面保留团块主体。
		var edgeDistance = new byte[width, height];
		var queue = new Queue<(int X, int Y)>();
		for (int x = 4; x < width - 4; x++)
		{
			for (int y = 4; y < height - 4; y++)
			{
				if (region[x, y] && SurfaceDirections.Any(direction => original[x + direction.X, y + direction.Y] == Jelly
					&& !region[x + direction.X, y + direction.Y]))
				{
					edgeDistance[x, y] = 1;
					queue.Enqueue((x, y));
				}
			}
		}
		while (queue.TryDequeue(out var point))
		{
			if (edgeDistance[point.X, point.Y] >= 3)
			{
				continue;
			}
			foreach (var direction in SurfaceDirections)
			{
				int x = point.X + direction.X;
				int y = point.Y + direction.Y;
				if (region[x, y] && edgeDistance[x, y] == 0)
				{
					edgeDistance[x, y] = (byte)(edgeDistance[point.X, point.Y] + 1);
					queue.Enqueue((x, y));
				}
			}
		}
		var selected = new bool[width, height];
		for (int x = 4; x < width - 4; x++)
		{
			for (int y = 4; y < height - 4; y++)
			{
				float density = edgeDistance[x, y] switch { 1 => 0.45f, 2 => 0.72f, 3 => 0.9f, _ => 1 };
				selected[x, y] = region[x, y] && FreshPatchHash(x + 1129, y - 733) < density;
				if (selected[x, y] && SurfaceDirections.Any(direction => membrane
					? original[x + direction.X, y + direction.Y] == Membrane
					: IsOpen(original[x + direction.X, y + direction.Y])))
				{
					tiles[x, y] = Fresh;
					selected[x, y] = false;
					queue.Enqueue((x, y));
				}
			}
		}
		// 去掉羽化后孤悬在深层的像素，保持浸润由接触面连续进入组织。
		while (queue.TryDequeue(out var point))
		{
			foreach (var direction in SurfaceDirections)
			{
				int x = point.X + direction.X;
				int y = point.Y + direction.Y;
				if (selected[x, y])
				{
					tiles[x, y] = Fresh;
					selected[x, y] = false;
					queue.Enqueue((x, y));
				}
			}
		}
	}

	private void PlaceCrystals()
	{
		// 每个主要细胞各放一个核，辅助补墙和补支撑不产生核，不按距离删减主要细胞的核。
		var footprints = nucleusCandidates.Select(cell => CellFootprint(cell).ToHashSet()).ToArray();
		var coverage = new int[width, height];
		foreach (var footprint in footprints)
		{
			foreach (var point in footprint)
			{
				coverage[point.X, point.Y]++;
			}
		}
		for (int index = 0; index < nucleusCandidates.Count; index++)
		{
			Cell cell = nucleusCandidates[index];
			int cx = (int)cell.X;
			int cy = (int)(cell.Y + cell.RadiusY * 0.2f);
			if (cell.SplashAngle is float angle)
			{
				cx = (int)(cell.X + MathF.Sin(angle) * cell.RadiusY * 0.35f);
				cy = (int)(cell.Y + MathF.Cos(angle) * cell.RadiusY * 0.35f);
			}
			else if (cell.GrowthAngle is float growth)
			{
				float along = cell.RadiusY * 0.55f * MathF.Sin(cell.Phase * 3.1f + cell.X * 0.07f);
				float across = cell.RadiusX * 0.35f * MathF.Cos(cell.Phase * 2.1f);
				cx = (int)(cell.X + MathF.Sin(growth) * along + MathF.Cos(growth) * across);
				cy = (int)(cell.Y + MathF.Cos(growth) * along - MathF.Sin(growth) * across);
			}
			var body = footprints[index];
			// 重叠面积在所属主细胞之间分摊，避免同一块组织反复计入多个大核；每颗细胞仍留核。
			float area = body.Sum(point => 1f / coverage[point.X, point.Y]);
			float radius = Math.Clamp(MathF.Sqrt(area * 0.035f / (MathF.PI * 1.15f)), body.Count < 120 ? 1.2f : 1.55f, 3.6f);
			var positions = body.OrderBy(point => (point.X - cx) * (point.X - cx) + (point.Y - cy) * (point.Y - cy)).ToArray();
			bool placed = false;
			// 只有细胞本身太小时才缩小核。
			for (int attempt = 0; attempt <= 5 && !placed; attempt++)
			{
				float rx = radius * (1 - attempt * 0.2f);
				float ry = rx * 1.15f;
				int reach = (int)MathF.Ceiling(ry);
				foreach (var position in positions)
				{
					var footprint = new List<(int X, int Y)>();
					bool intact = true;
					for (int dx = -reach; dx <= reach; dx++)
					{
						for (int dy = -reach; dy <= reach; dy++)
						{
							if ((dx == 0 && dy == 0) || (rx > 0 && dx * dx / (rx * rx) + dy * dy / (ry * ry) <= 1))
							{
								var point = (X: position.X + dx, Y: position.Y + dy);
								intact &= body.Contains(point) && tiles[point.X, point.Y] is Jelly or Fresh;
								footprint.Add(point);
							}
						}
					}
					if (intact)
					{
						foreach (var point in footprint)
						{
							tiles[point.X, point.Y] = Crystal;
						}
						placed = true;
						break;
					}
				}
			}
		}
	}

	private bool IsTissue(int x, int y) => x > 3 && x < width - 4 && y > 3 && y < height - 4
		&& tiles[x, y] is Jelly or Fresh;

	private static bool IsOpen(byte material) => material is Air or CaveAir or Honey;

	private void Put(int x, int y, byte material)
	{
		if (MathF.Abs(x - center) < width * 0.16f && y < neckStart)
		{
			return;
		}
		if (x > 3 && x < width - 4 && y > 3 && y < height - 4 && tiles[x, y] != Protected)
		{
			tiles[x, y] = material;
			if (material == CaveAir)
			{
				excavated[x, y] = true;
			}
		}
	}
}
