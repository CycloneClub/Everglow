using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Events;

/// <summary>
/// 可复数启用的事件
/// <br>仅作示例,不加载,发布时请移除</br>
/// </summary>
internal class PluralEvent : ModEvent
{
	public override bool IsLoadingEnabled(Mod mod) => false;

	private string defName = string.Empty;

	public override string DefName => base.DefName + "." + defName;

	/// <summary>
	/// 启动一个可复数启用的事件
	/// </summary>
	/// <param name="args"></param>
	/// <returns></returns>
	public static bool Start(params object[] args)
	{
		// 创建一个可复数启用的事件，赋予其独特的defName
		// 当事件保存存档时如果DefName与FullName不一致,EventManager会额外为其创立数据保存
		// 在加载存档时将通过FullName获取原型的复制,再将保存的DefName和数据传入
		return EventSystem.Activate(new PluralEvent() { defName = Guid.NewGuid().ToString() }, args);
	}

	// 额外的,因为此事件特殊性,无法等待自动保存
	// 如果想要长期保存该事件，需要如下重写，使其始终不关闭
	// 极其不建议这种做法，可复用事件理应为临时性事件，本身不应该保存
	public override bool CanDeactivate(params object[] args)
	{
		return false;
	}

	public override void LoadWorldData(string defName, TagCompound tag)
	{
		this.defName = defName;
	}

	/// <summary>
	/// 重新加载时交付一个空defName的实例便于<see cref="LoadWorldData(string, TagCompound)"/>读取
	/// </summary>
	/// <returns></returns>
	public override ModEvent Clone()
	{
		return new PluralEvent();
	}
}
