namespace Everglow.Commons.Mechanics.MirrorReflection.Core;

/// <summary>
/// 管理墙面材质注册，收集当前视口中的反光墙并按采样参数分组；不申请 GPU 资源或执行绘制。
/// </summary>
internal sealed class MirrorWallReflectionCollector
{
	private readonly Dictionary<int, Func<MirrorReflectionDefinition>> profiles = [];

	internal sealed class ReflectionGroup(MirrorReflectionDefinition definition)
	{
		public readonly MirrorReflectionDefinition Definition = definition;
		public readonly List<(Point Position, Color Tint)> Walls = [];
	}

	public MirrorWallReflectionCollector(IReadOnlyDictionary<int, MirrorReflectionDefinition> defaults)
	{
		foreach (var (wallType, profile) in defaults)
		{
			Register(wallType, profile);
		}
	}

	public void Register(int wallType, MirrorReflectionDefinition profile) => Register(wallType, () => profile);

	/// <summary>同类型注册替换旧值；动态材质每次收集只读取一次。</summary>
	public void Register(int wallType, Func<MirrorReflectionDefinition> provider)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(wallType);
		ArgumentNullException.ThrowIfNull(provider);
		profiles[wallType] = provider;
	}

	/// <summary>读取每类墙的定义一次，返回独立的参数与乘色快照。</summary>
	public Dictionary<int, (MirrorReflectionDefinition Definition, Color Tint)> ResolveMaterials()
	{
		Dictionary<int, (MirrorReflectionDefinition Definition, Color Tint)> materials = [];
		foreach (var (wallType, provider) in profiles)
		{
			MirrorReflectionDefinition profile = provider();
			float opacity = Math.Clamp(profile.Opacity, 0, 1);
			float strength = Math.Clamp(profile.Strength, 0, 1);
			Color tint = new(new Vector4(new Vector3(opacity * strength), opacity));
			if (tint.A != 0)
			{
				// 覆盖率和亮度已写入乘色；统一这两个字段，使相同采样定义能够共享倒影。
				var sampling = profile with
				{
					Opacity = 1,
					Strength = 1,
					Offset = new(Math.Clamp(profile.Offset.X, -100, 100), Math.Clamp(profile.Offset.Y, -30, 30)),
					ReflectionScale = Math.Clamp(profile.ReflectionScale, 0.5f, 1.5f),
					Blur = Math.Clamp(profile.Blur, 0, 4),
					Sigma = Math.Clamp(profile.Sigma, 0.25f, 3),
					NormalStrength = Math.Clamp(profile.NormalStrength, 0, 24),
					NormalPeriod = Math.Clamp(profile.NormalPeriod, 8, 256),
					Pitch = Math.Clamp(profile.Pitch, -60, 60),
					Yaw = Math.Clamp(profile.Yaw, -60, 60),
					Roll = Math.Clamp(profile.Roll, -90, 90),
					Distance = Math.Clamp(profile.Distance, 240, 1200),
				};
				materials.Add(wallType, (sampling, tint));
			}
		}
		return materials;
	}

	/// <summary>使用给定材质快照收集可见墙格；每次调用产生独立分组。</summary>
	public IReadOnlyList<ReflectionGroup> CollectVisibleWalls(IReadOnlyDictionary<int, (MirrorReflectionDefinition Definition, Color Tint)> materials)
	{
		Dictionary<MirrorReflectionDefinition, ReflectionGroup> parameterGroups = [];
		Dictionary<int, (ReflectionGroup Group, Color Tint)> wallMaterials = [];
		foreach (var (wallType, material) in materials)
		{
			if (!parameterGroups.TryGetValue(material.Definition, out ReflectionGroup group))
			{
				group = new ReflectionGroup(material.Definition);
				parameterGroups.Add(material.Definition, group);
			}
			// 每类墙只查找一次参数分组，逐格扫描时直接追加墙格。
			wallMaterials.Add(wallType, (group, material.Tint));
		}

		Matrix inverseView = Matrix.Invert(Main.GameViewMatrix.TransformationMatrix);
		Vector2 first = Vector2.Transform(Vector2.Zero, inverseView) + Main.screenPosition;
		Vector2 second = Vector2.Transform(new Vector2(Main.screenWidth, Main.screenHeight), inverseView) + Main.screenPosition;
		Vector2 topLeft = Vector2.Min(first, second);
		Vector2 bottomRight = Vector2.Max(first, second);
		int startX = Math.Max(0, (int)(topLeft.X / 16) - 1);
		int startY = Math.Max(0, (int)(topLeft.Y / 16) - 1);
		int endX = Math.Min(Main.maxTilesX - 1, (int)(bottomRight.X / 16) + 1);
		int endY = Math.Min(Main.maxTilesY - 1, (int)(bottomRight.Y / 16) + 1);
		for (int x = startX; x <= endX; x++)
		{
			for (int y = startY; y <= endY; y++)
			{
				Tile tile = Main.tile[x, y];
				if (tile.IsWallInvisible || !wallMaterials.TryGetValue(tile.WallType, out var wall))
				{
					continue;
				}
				// 不按 HasTile 丢弃整格；随后绘制的前景纹理负责逐像素遮挡。
				wall.Group.Walls.Add((new Point(x, y), wall.Tint));
			}
		}
		return parameterGroups.Values.Where(group => group.Walls.Count > 0).ToArray();
	}
}
