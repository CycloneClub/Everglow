using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Everglow.Commons.Mechanics.MirrorReflection;

/// <summary>
/// 模糊反射的采样精度档位；较低档位减少采样次数，保留材质指定的采样范围。
/// </summary>
public enum MirrorReflectionQuality
{
	/// <summary>
	/// 低开销滤波。
	/// </summary>
	Low,

	/// <summary>
	/// 平衡滤波精度与开销。
	/// </summary>
	Medium,

	/// <summary>
	/// 使用完整的九点高斯滤波。
	/// </summary>
	High,
}

/// <summary>
/// 客户端反射画质设置；墙面的形状、位置、颜色和反射强度由各自材质定义。
/// </summary>
public class MirrorReflectionConfig : ModConfig
{
	public override ConfigScope Mode => ConfigScope.ClientSide;

	/// <summary>
	/// 获取或设置是否启用反射；关闭时跳过场景捕获和倒影绘制。
	/// </summary>
	[DefaultValue(true)]
	public bool Enabled = true;

	/// <summary>
	/// 获取或设置模糊反射的滤波精度，不覆盖墙面材质参数。
	/// </summary>
	[DefaultValue(MirrorReflectionQuality.High)]
	[DrawTicks]
	public MirrorReflectionQuality Quality = MirrorReflectionQuality.High;

	/// <summary>
	/// 获取每个方向的模糊采样数量；未知配置值回退到当前默认的高画质。
	/// </summary>
	/// <returns>低、中、高画质分别对应 3、5、9 个采样点。</returns>
	public int GetBlurSampleCount() => Quality switch
	{
		MirrorReflectionQuality.Low => 3,
		MirrorReflectionQuality.Medium => 5,
		_ => 9,
	};
}
