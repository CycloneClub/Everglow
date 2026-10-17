namespace Everglow.Yggdrasil.YggdrasilTown.Travelers;

public class TownTravelerCommand : ModCommand
{
	public override CommandType Type => CommandType.Chat;

	public override string Command => "towncrowd";

	public override string Usage => "/towncrowd status|spawn";

	public override string Description => "Inspect town travelers; use VFXPersonItem to toggle modes / 查询旅客；使用旅客控制器切换模式";

	public override void Action(CommandCaller caller, string input, string[] args)
	{
		string action = args.Length == 1 ? args[0].ToLowerInvariant() : "status";
		if (!TravelerRules.CanRun(Main.netMode, YggdrasilWorld.InYggdrasil))
		{
			caller.Reply("Enter Yggdrasil in single-player / 请在单人模式进入天穹树。", Color.Orange);
			return;
		}
		switch (action)
		{
			case "spawn":
				caller.Reply(TownTravelerSystem.TrySpawn(true) ? "Traveler created / 已生成旅客。" : "No safe route or capacity; wait for landmark scan / 暂无安全路线或名额，请等待地标扫描。", Color.LightSkyBlue);
				break;
			case "status":
				break;
			default:
				caller.Reply(Usage + "; VFXPersonItem: left click toggles NPC, middle click toggles actors / 旅客控制器：左键开关 NPC，中键开关轻量模式。");
				return;
		}
		caller.Reply(TownTravelerSystem.Status, Color.LightSkyBlue);
	}
}
