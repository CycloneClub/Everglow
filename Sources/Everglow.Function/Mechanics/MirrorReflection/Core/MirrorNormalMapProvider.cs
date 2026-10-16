namespace Everglow.Commons.Mechanics.MirrorReflection.Core;

public enum MirrorNormalPattern
{
	Wave,
	HorizontalWave,
	VerticalWave,
	Bulge,
	Pinch,
	Twist,
}

/// <summary>
/// 提供并缓存法线纹理；只负责图案生成和资源生命周期，不访问墙格或组织反射绘制。
/// </summary>
public sealed class MirrorNormalMapProvider : IDisposable
{
	private readonly Dictionary<MirrorNormalPattern, Texture2D> normalMaps = [];

	/// <summary>取得当前客户端的法线纹理。纹理由本提供者持有，调用方不得释放。</summary>
	public Texture2D GetTexture(MirrorNormalPattern pattern)
	{
		if (Main.dedServ)
		{
			throw new InvalidOperationException("Normal textures are only available on the client.");
		}
		if (normalMaps.TryGetValue(pattern, out Texture2D normalMap) && !normalMap.IsDisposed)
		{
			return normalMap;
		}
		const int size = 128;
		Color[] pixels = new Color[size * size];
		for (int y = 0; y < size; y++)
		{
			for (int x = 0; x < size; x++)
			{
				Vector3 normal = SampleNormal((x + 0.5f) / size, (y + 0.5f) / size, pattern);
				pixels[y * size + x] = new Color(normal * 0.5f + new Vector3(0.5f));
			}
		}
		normalMap = new Texture2D(Main.graphics.GraphicsDevice, size, size, false, SurfaceFormat.Color);
		normalMap.SetData(pixels);
		normalMaps[pattern] = normalMap;
		return normalMap;
	}

	/// <summary>可平铺双向曲面的单位法线，供运行时生成 RGB 法线贴图。</summary>
	public static Vector3 SampleNormal(float u, float v) => Vector3.Normalize(new Vector3(
		0.5f * MathF.Sin(MathHelper.TwoPi * u), 0.5f * MathF.Sin(MathHelper.TwoPi * v), 1));

	/// <summary>可平铺的哈哈镜法线；径向曲面在周期边界平滑归零，避免重复图案的接缝。</summary>
	public static Vector3 SampleNormal(float u, float v, MirrorNormalPattern pattern)
	{
		u -= MathF.Floor(u);
		v -= MathF.Floor(v);
		if (pattern == MirrorNormalPattern.Wave)
		{
			return SampleNormal(u, v);
		}
		if (pattern == MirrorNormalPattern.HorizontalWave)
		{
			return Vector3.Normalize(new Vector3(0.8f * MathF.Sin(MathHelper.TwoPi * u), 0, 1));
		}
		if (pattern == MirrorNormalPattern.VerticalWave)
		{
			return Vector3.Normalize(new Vector3(0, 0.8f * MathF.Sin(MathHelper.TwoPi * v), 1));
		}
		Vector2 radial = new(u * 2 - 1, v * 2 - 1);
		float window = (1 - radial.X * radial.X) * (1 - radial.Y * radial.Y);
		Vector2 slope = pattern switch
		{
			MirrorNormalPattern.Bulge => -radial,
			MirrorNormalPattern.Pinch => radial,
			MirrorNormalPattern.Twist => new Vector2(-radial.Y, radial.X),
			_ => throw new ArgumentOutOfRangeException(nameof(pattern)),
		};
		return Vector3.Normalize(new Vector3(slope * (1.5f * window * window), 1));
	}

	/// <summary>释放本提供者生成的纹理；图形资源在主线程释放。</summary>
	public void Dispose()
	{
		foreach (Texture2D texture in normalMaps.Values)
		{
			Main.QueueMainThreadAction(texture.Dispose);
		}
		normalMaps.Clear();
	}
}
