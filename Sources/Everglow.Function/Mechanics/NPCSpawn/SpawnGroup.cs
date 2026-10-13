using Everglow.Commons.Mechanics.Events;

namespace Everglow.Commons.Mechanics.NPCSpawn;

internal sealed record SpawnGroupRule(SubworldSpawnRule Rule, Func<Vector2, bool> InRegion);

internal sealed record SpawnGroupContext(string WorldId, NPCSpawnInfo Info, SpawnGroupRule Entry);

/// <summary>原生自然生成首只 NPC 后，按表配置追加群体成员；没有独立时钟、预算或实体归属。</summary>
internal static class SpawnGroup
{
	public static void SpawnMembers(SpawnGroupContext context, NPC first)
	{
		if (!first.active)
		{
			return;
		}
		var rule = context.Entry.Rule;
		int count = Main.rand.Next(rule.MinGroupSize, rule.MaxGroupSize + 1);
		Vector2 anchor = first.Bottom;
		for (int member = 1; member < count; member++)
		{
			if (EventSystem.HasActiveSpawnOverride || ModContent.GetInstance<SubworldSpawnSystem>().FindActiveWorld()?.Id != context.WorldId ||
				!context.Info.Player.active || context.Info.Player.dead)
			{
				break;
			}
			Vector2 feet = anchor + MemberOffset(member, first.width, first.height);
			if (!CanPlace(context, feet, first.width, first.height))
			{
				continue;
			}
			// NewNPC 不调用 GlobalNPC.SpawnNPC，因此附加成员不会再触发扩群。
			int slot = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), (int)feet.X, (int)feet.Y,
				rule.Type, Target: context.Info.Player.whoAmI);
			if (slot >= Main.maxNPCs)
			{
				break;
			}
			Main.npc[slot].netUpdate = true;
			if (Main.netMode == NetmodeID.Server)
			{
				NetMessage.SendData(MessageID.SyncNPC, number: slot);
			}
		}
	}

	internal static Vector2 MemberOffset(int member, int width, int height)
	{
		int column = (member % 3) switch { 1 => -1, 2 => 1, _ => 0 };
		return new Vector2(column * (width + 10), -(member / 3) * (height + 10));
	}

	private static bool CanPlace(SpawnGroupContext context, Vector2 feet, int width, int height)
	{
		if (!context.Entry.InRegion(feet) ||
			Math.Abs(feet.X - context.Info.Player.Center.X) > NPC.spawnRangeX * 16 ||
			Math.Abs(feet.Y - context.Info.Player.Center.Y) > NPC.spawnRangeY * 16)
		{
			return false;
		}
		var body = new Rectangle((int)feet.X - width / 2, (int)feet.Y - height, width, height);
		int left = (int)MathF.Floor(body.Left / 16f);
		int right = (int)MathF.Floor((body.Right - 1) / 16f);
		int top = (int)MathF.Floor(body.Top / 16f);
		int bottom = (int)MathF.Floor((body.Bottom - 1) / 16f);
		if (!WorldGen.InWorld(left, top, 2) || !WorldGen.InWorld(right, bottom + 1, 2))
		{
			return false;
		}
		for (int x = left; x <= right; x++)
		{
			for (int y = top; y <= bottom; y++)
			{
				Tile tile = Main.tile[x, y];
				if (Main.wallHouse[tile.WallType] ||
					(tile.HasUnactuatedTile && Main.tileSolid[tile.TileType]) ||
					(context.Info.Water ? tile.LiquidType != LiquidID.Water || tile.LiquidAmount < 128 : tile.LiquidAmount > 0))
				{
					return false;
				}
			}
		}
		foreach (Player player in Main.ActivePlayers)
		{
			var safe = new Rectangle(
				(int)(player.Center.X - NPC.sWidth / 2f - NPC.safeRangeX),
				(int)(player.Center.Y - NPC.sHeight / 2f - NPC.safeRangeY),
				NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
			if (body.Intersects(safe))
			{
				return false;
			}
		}
		NPCSpawnInfo info = context.Info;
		info.SpawnTileX = (int)feet.X / 16;
		info.SpawnTileY = (int)feet.Y / 16;
		info.SpawnTileType = Main.tile[info.SpawnTileX, info.SpawnTileY].TileType;
		// 玩家事实沿用原生上下文；实际液体和区域已在新位置重新验证。
		return context.Entry.Rule.Condition?.Invoke(info) ?? true;
	}
}
