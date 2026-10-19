namespace Everglow.Commons.Mechanics.MirrorReflection.Core;

/// <summary>
/// 定义镜面反射的覆盖率、亮度、采样和投影参数；不持有纹理或执行绘制。
/// </summary>
/// <param name="Opacity">
/// 倒影的不透明度，范围为 0 到 1。0 表示完全透明，1 表示使用倒影纹理本身的完整覆盖率。
/// 此值不改变原墙纹理的不透明度。
/// </param>
/// <param name="Strength">
/// 倒影的 RGB 亮度系数，范围为 0 到 1，默认值为 1。此值不影响 Alpha；
/// 设为 0 会保留黑色倒影的覆盖率，关闭倒影应将 <paramref name="Opacity"/> 设为 0。
/// </param>
/// <remarks>
/// 构造和初始化时不校验或截断数值；调用方应提供有限浮点数和有效枚举值。
/// 墙面收集阶段限制数值范围，并将覆盖率和亮度转换为绘制颜色；二者不参与倒影采样分组。
/// 客户端配置只控制反射开关和滤波精度。使用 <see langword="default"/> 创建的结构体覆盖率为 0，不产生倒影。
/// </remarks>
public readonly record struct MirrorReflectionDefinition(float Opacity, float Strength = 1)
{
	/// <summary>
	/// 获取或初始化倒影的世界像素偏移，正值向右、向下。
	/// </summary>
	/// <value>X 范围为 -100 到 100，Y 范围为 -30 到 30；默认值为 (12, 0)。</value>
	public Vector2 Offset { get; init; } = new(12, 0);

	/// <summary>
	/// 获取或初始化倒影在水平和垂直方向上的统一缩放系数。
	/// </summary>
	/// <value>
	/// 范围为 0.5 到 1.5，1 表示原始大小。
	/// </value>
	/// <remarks>
	/// 通用反射系统以本地玩家脚底为缩放基准，不以连续墙面的中心为基准。
	/// </remarks>
	public float ReflectionScale { get; init; } = 1;

	/// <summary>
	/// 获取或初始化是否围绕反射基准进行左右翻转。
	/// </summary>
	/// <value>
	/// <see langword="true"/> 表示左右翻转，<see langword="false"/> 表示保持方向。
	/// </value>
	public bool FlipHorizontally { get; init; }

	/// <summary>
	/// 获取或初始化高画质下高斯模糊的基准采样间距，单位为世界像素。
	/// </summary>
	/// <value>
	/// 范围为 0 到 4，0 表示跳过模糊。
	/// </value>
	/// <remarks>
	/// 此值控制采样间距，不表示采样点数量或核的标准差。
	/// 低、中、高画质分别执行每方向三点、五点、九点采样；较低画质会增大实际采样间距，
	/// 各档位单方向最远采样位置距中心均为此值的四倍。
	/// </remarks>
	public float Blur { get; init; } = 0.5f;

	/// <summary>
	/// 获取或初始化高斯采样核的标准差，用于控制各采样点的权重分布。
	/// </summary>
	/// <value>
	/// 范围为 0.25 到 3，以高画质的基准采样点索引间距为单位。
	/// </value>
	/// <remarks>
	/// 值越大，远离中心的采样点权重越高。实际采样位置由 <see cref="Blur"/> 和画质档位共同决定。
	/// </remarks>
	public float Sigma { get; init; } = 1.5f;

	/// <summary>
	/// 获取或初始化用于生成法线贴图的曲面类型，例如波纹、鼓起、凹陷或扭转。
	/// </summary>
	/// <value>
	/// 有效的 <see cref="MirrorNormalPattern"/> 枚举值。
	/// </value>
	public MirrorNormalPattern NormalPattern { get; init; }

	/// <summary>
	/// 获取或初始化法线扭曲的位移系数，单位为世界像素。
	/// </summary>
	/// <value>
	/// 范围为 0 到 24，0 表示不产生法线扭曲。
	/// </value>
	/// <remarks>
	/// 最终位移还取决于法线斜率和镜面角度，此值不是最大位移距离。
	/// </remarks>
	public float NormalStrength { get; init; }

	/// <summary>
	/// 获取或初始化法线图案在世界坐标中的平铺周期，单位为世界像素。
	/// </summary>
	/// <value>
	/// 范围为 8 到 256，允许材质使用小于演示面板下限的细密图案。
	/// </value>
	/// <remarks>
	/// 数值越大，单个曲面图案覆盖的区域越大；图案位置随世界坐标固定。
	/// </remarks>
	public float NormalPeriod { get; init; } = 96;

	/// <summary>
	/// 获取或初始化反射投影绕 X 轴旋转的俯仰角，单位为度。
	/// </summary>
	/// <value>
	/// 范围为 -60 到 60。
	/// </value>
	/// <remarks>
	/// 角度作用于倒影投影及法线位移方向，墙面网格保持原位。
	/// </remarks>
	public float Pitch { get; init; }

	/// <summary>
	/// 获取或初始化反射投影绕 Y 轴旋转的偏航角，单位为度。
	/// </summary>
	/// <value>
	/// 范围为 -60 到 60。
	/// </value>
	/// <remarks>
	/// 角度作用于倒影投影及法线位移方向，墙面网格保持原位。
	/// </remarks>
	public float Yaw { get; init; }

	/// <summary>
	/// 获取或初始化反射投影绕 Z 轴旋转的滚转角，单位为度。
	/// </summary>
	/// <value>
	/// 范围为 -90 到 90。
	/// </value>
	/// <remarks>
	/// 角度作用于倒影投影及法线位移方向，墙面网格保持原位。
	/// </remarks>
	public float Roll { get; init; }

	/// <summary>
	/// 获取或初始化反射透视投影使用的虚拟观察距离，单位为世界像素。
	/// </summary>
	/// <value>
	/// 范围为 240 到 1200。
	/// </value>
	/// <remarks>
	/// 倒影倾斜时，较小的距离产生更强的透视变化；此值不表示玩家与墙之间的实际距离。
	/// </remarks>
	public float Distance { get; init; } = 480;

	/// <summary>获取按 X、Y、Z 轴排列的旋转角，单位为弧度。</summary>
	public Vector3 Angles => new(MathHelper.ToRadians(Pitch), MathHelper.ToRadians(Yaw), MathHelper.ToRadians(Roll));
}
