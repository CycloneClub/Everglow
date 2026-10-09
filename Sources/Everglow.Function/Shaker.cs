namespace Everglow.Commons;

public class ShakerInfo
{
	internal Vector2 center;
	internal Vector2 dir;
	internal float strength;
	internal float period;
	internal float propagationSpeed;
	internal float ac_vibration;
	internal float ac_propagation;
	internal int tickTimer;
	internal int maxTick;
	internal int propagationDelayTimer;
	internal int maxPropagationTime;
	public virtual bool Update()
	{
		tickTimer++;
		if (strength * Math.Pow(ac_vibration, tickTimer) < 0.01f || (maxTick > 0 && tickTimer > maxTick))
		{
			propagationDelayTimer++;
			if (propagationDelayTimer >= maxPropagationTime)
			{
				return false;
			}
		}
		return true;
	}
	public virtual void ApplyTo(Vector2 pos, ref Vector2 effect)
	{
		int tracetime = (int)(Vector2.Distance(center, pos) / propagationSpeed);
		if (tickTimer < tracetime)
		{
			return;
		}
		int time = tickTimer - tracetime;
		if (maxTick != -1 && time > maxTick)
		{
			return;
		}
		float amplitude = (float)Math.Sin(period * time);
		Vector2 vdir = Vector2.Normalize(dir);
		float vStrength = strength * amplitude * (float)Math.Pow(ac_vibration, time);
		float attenuation = (float)Math.Pow(ac_propagation, tracetime);
		float attenuatedStrength = vStrength * attenuation;
		effect += attenuatedStrength * vdir;
	}
	// 根据标识头(FullName)调用解析
	public virtual ShakerInfo NetRecive(BinaryReader reader)
	{
		return new()
		{
			center = new(reader.ReadSingle(), reader.ReadSingle()),
			strength = reader.ReadSingle(),
			period = reader.ReadSingle(),
			propagationSpeed = reader.ReadSingle(),
			ac_vibration = reader.ReadSingle(),
			ac_propagation = reader.ReadSingle(),
			tickTimer = reader.ReadInt32(),
			maxTick = reader.ReadInt32(),
			propagationDelayTimer = reader.ReadInt32(),
			maxPropagationTime = reader.ReadInt32(),
		};
	}
	// 调用此方法前写入标识头，便于根据标识头(FullName)调用解析
	public virtual void NetSend(BinaryWriter writer)
	{
		writer.Write(center.X);
		writer.Write(center.Y);
		writer.Write(strength);
		writer.Write(period);
		writer.Write(propagationSpeed);
		writer.Write(ac_vibration);
		writer.Write(ac_propagation);
		writer.Write(tickTimer);
		writer.Write(maxTick);
		writer.Write(propagationSpeed);
		writer.Write(maxPropagationTime);
	}
}
public class UndirectedShakerInfo : ShakerInfo
{
	public static UndirectedShakerInfo Create(Vector2 center, float strength = 20, float period = 60, float speed = 30, float acv = 0.9f, float acp = 0.9f, int maxtick = 10)
	{
		if (period < 0)
		{
			throw new ArgumentException("不合法的物理参数.振动周期不可能为负数.");
		}
		if (speed <= 0)
		{
			throw new ArgumentException("不合法的物理参数.传播速度不可能为负数或0.这是一个标量速度!");
		}
		if (acv <= 0)
		{
			throw new ArgumentException("不合法的物理参数.强度衰减不可能为负数或0.这样的震动不可能存在!");
		}
		if (acp <= 0)
		{
			throw new ArgumentException("不合法的物理参数.传播衰减不可能为负数或0.这样的震动不可能存在!");
		}
		return new UndirectedShakerInfo()
		{
			center = center,
			strength = strength,
			period = period,
			propagationSpeed = speed,
			ac_vibration = acv,
			ac_propagation = acp,
			maxPropagationTime = (int)Math.Floor(1 / acp / speed) + 1,
			maxTick = maxtick,
		};
	}
	public override void ApplyTo(Vector2 pos, ref Vector2 effect)
	{
		int tracetime = (int)(Vector2.Distance(center, pos) / propagationSpeed);
		if (tickTimer < tracetime)
		{
			return;
		}
		int time = tickTimer - tracetime;
		if (maxTick != -1 && time > maxTick)
		{
			return;
		}
		float amplitude = (float)Math.Sin(period * time);
		Vector2 vdir = Vector2.Normalize(pos - center);
		float vStrength = strength * amplitude * (float)Math.Pow(ac_vibration, time);
		float attenuation = (float)Math.Pow(ac_propagation, tracetime);
		float attenuatedStrength = vStrength * attenuation;
		effect += attenuatedStrength * vdir;
	}
	public override ShakerInfo NetRecive(BinaryReader reader)
	{
		return new()
		{
			center = new(reader.ReadSingle(), reader.ReadSingle()),
			strength = reader.ReadSingle(),
			period = reader.ReadSingle(),
			propagationSpeed = reader.ReadSingle(),
			ac_vibration = reader.ReadSingle(),
			ac_propagation = reader.ReadSingle(),
			tickTimer = reader.ReadInt32(),
			maxTick = reader.ReadInt32(),
			propagationDelayTimer = reader.ReadInt32(),
			maxPropagationTime = reader.ReadInt32(),
		};
	}
}
public class ShakerManager : ModSystem
{
	private static List<ShakerInfo> shakers;
	private static List<ShakerInfo> waitremove;
	/// <summary>
	/// 关闭此震动系统的屏幕移动效果,每帧重置
	/// </summary>
	public static bool LockScreen { get; set; }
	public override void Load()
	{
		shakers = new();
		waitremove = new();
	}
	public override void Unload()
	{
		shakers.Clear();
		shakers = null;
		waitremove.Clear();
		waitremove = null;
	}
	public override void PostUpdateEverything() => Update();
	public override void ModifyScreenPosition()
	{
		if (LockScreen)
		{
			LockScreen = false;
			return;
		}
		Main.screenPosition += GetEffectOn(Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) / 2);
	}
	/// <summary>
	/// 加入一个默认有向振动源
	/// </summary>
	/// <param name="center">震动中心</param>
	/// <param name="dir">震动方向</param>
	/// <param name="strength">震动强度</param>
	/// <param name="period">震动周期</param>
	/// <param name="speed">传播速度</param>
	/// <param name="acv">强度衰减系数</param>
	/// <param name="acp">传播衰减系数</param>
	/// <param name="maxtick">最大持续帧数</param>
	/// <param name="needNetSync">是否需要联机同步</param>
	/// <exception cref="ArgumentException"></exception>
	public static void AddShaker(Vector2 center, Vector2 dir, float strength = 1, float period = 1, float speed = 1, float acv = 0.9f, float acp = 0.9f, int maxtick = -1, bool needNetSync = false)
	{
		if (period < 0)
		{
			throw new ArgumentException("不合法的物理参数.振动周期不可能为负数.");
		}
		if (speed <= 0)
		{
			throw new ArgumentException("不合法的物理参数.传播速度不可能为负数或0.这是一个标量速度!");
		}
		if (acv <= 0)
		{
			throw new ArgumentException("不合法的物理参数.强度衰减不可能为负数或0.这样的震动不可能存在!");
		}
		if (acp <= 0)
		{
			throw new ArgumentException("不合法的物理参数.传播衰减不可能为负数或0.这样的震动不可能存在!");
		}
		ShakerInfo info = new()
		{
			center = center,
			dir = dir,
			strength = strength,
			period = period,
			propagationSpeed = speed,
			ac_vibration = acv,
			ac_propagation = acp,
			maxPropagationTime = (int)Math.Floor(1 / acp / speed) + 1,
			maxTick = maxtick,
		};
		shakers.Add(info);
	}
	/// <summary>
	/// 加入一个自定义震动源
	/// <br>由联机同步得到的震动源务必关闭<see cref="ShakerInfo.NeedNetSync"/>以避免无限同步</br>
	/// </summary>
	/// <param name="info"></param>
	public static void AddShaker(ShakerInfo info)
	{
		shakers.Add(info);
	}
	/// <summary>
	/// 清空所有震动源
	/// </summary>
	public static void Clear() => shakers.Clear();
	private static void Update()
	{
		waitremove.Clear();
		foreach (ShakerInfo info in shakers)
		{
			if (!info.Update())
			{
				waitremove.Add(info);
			}
		}
		shakers.RemoveAll(waitremove.Contains);
	}
	/// <summary>
	/// 获取某一点受到该震动系统内所有震源的影响效果
	/// </summary>
	/// <param name="pos"></param>
	/// <returns></returns>
	public static Vector2 GetEffectOn(Vector2 pos)
	{
		Vector2 effect = Vector2.Zero;
		foreach (ShakerInfo info in shakers)
		{
			info.ApplyTo(pos, ref effect);
		}
		return effect;
	}
}
