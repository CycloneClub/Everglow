using Everglow.SubSpace;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Everglow.Yggdrasil.YggdrasilTown.Kitchen.Tiles;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Miscs.PlayerArena;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;
using Everglow.Yggdrasil.YggdrasilTown.VFXs.RandomNPC;
using SubworldLibrary;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace Everglow.Yggdrasil.YggdrasilTown;

public class YggdrasilTownCentralSystem : ModSystem
{
	public static Rectangle TownArea => new Rectangle(TownTopLeftWorldCoord.ToTileCoordinates().X, TownTopLeftWorldCoord.ToTileCoordinates().Y, 706, 275);

	public static Vector2 TownTopLeftWorldCoord => YggdrasilTownBiome.BiomeCenter - new Vector2(257, 191) * 16;

	public float TownSurfaceWorldCoordY => TownTopLeftWorldCoord.Y + 1600;

	public static Vector2 TownPos(Vector2 position_Orig)
	{
		return position_Orig - TownTopLeftWorldCoord;
	}

	public static List<int> YggdrasilTownNPCList => new List<int>() { ModContent.NPCType<InnKeeper>(), ModContent.NPCType<TeahouseLady>(), ModContent.NPCType<Guard_of_YggdrasilTown>(), ModContent.NPCType<Fevens>() };

	public static List<int> CanteenNPCList => new List<int>() { ModContent.NPCType<CanteenMaid>(), ModContent.NPCType<Restauranteur>() };

	public static List<int> UnionNPCList => new List<int>() { ModContent.NPCType<Howard_Warden>() };

	/// <summary>
	/// X refer player.whoAmI; Y refer NPC.type; this will summon a NPC in corresponding type while enter the arena.
	/// </summary>
	public static Point FightingRequestPlayerNPCType = new Point(-1, -1);

	public static bool ResetedArena = true;

	public static int ArenaScore;

	public override void Load()
	{
		Ins.HookManager.AddHook(CodeLayer.PostDrawMapIcons, DrawMap_YggdrasilTown);
		base.Load();
	}

	private void DrawMap_YggdrasilTown(Vector2 mapTopLeft, Vector2 mapX2Y2AndOff, Rectangle? mapRect, float mapScale)
	{
		if (!SubworldSystem.IsActive<YggdrasilWorld>())
		{
			return;
		}
		Vector2 center = TownTopLeftWorldCoord + new Vector2(268, 162) * 16;
		Vector2 position = (center / 16f - mapTopLeft) * mapScale + mapX2Y2AndOff;
		var destination = new Rectangle((int)position.X - 1, (int)position.Y - 1, 2, 2);
		if (mapRect != null ? destination.Intersects(mapRect.Value) : true)
		{
			Texture2D tex = ModAsset.TownMap.Value;
			Main.spriteBatch.Draw(tex, position, null, Color.White * 0.5f, 0, tex.Size() * new Vector2(0.5f, 1f), mapScale * 0.5f, SpriteEffects.None, 0);
		}
	}

	public override void OnWorldLoad()
	{
		if (SubworldSystem.Current is YggdrasilWorld)
		{
			foreach (var type in YggdrasilTownNPCList)
			{
				CheckNPC(type);
			}
		}
		if (InCanteen_YggdrasilTown())
		{
			foreach (var type in CanteenNPCList)
			{
				CheckNPC(type);
			}
		}
		if (InUnion_YggdrasilTown())
		{
			foreach (var type in UnionNPCList)
			{
				CheckNPC(type);
			}
		}
		base.OnWorldLoad();
	}

	public override void PostUpdateNPCs()
	{
		if (Main.time == 0)
		{
			if (SubworldSystem.Current is YggdrasilWorld)
			{
				foreach (var type in YggdrasilTownNPCList)
				{
					CheckNPC(type);
				}
			}
			if (InCanteen_YggdrasilTown())
			{
				foreach (var type in CanteenNPCList)
				{
					CheckNPC(type);
				}
			}
			if (InUnion_YggdrasilTown())
			{
				foreach (var type in UnionNPCList)
				{
					CheckNPC(type);
				}
			}
		}
		if (InArena_YggdrasilTown() && !ResetedArena)
		{
			RoadSignPost_ToArenaVFX.BuildArenaGen();
			ResetedArena = true;
		}
		YggdrasilTownPersonManager.Update();
		base.PostUpdateNPCs();
	}

	public static void CheckNPC(int type)
	{
		if (type > 0)
		{
			if (NPC.CountNPCS(type) <= 0)
			{
				Point spawnPos = YggdrasilTownBiome.BiomeCenter.ToTileCoordinates();
				NPC.NewNPC(WorldGen.GetNPCSource_TileBreak(spawnPos.X, spawnPos.Y), spawnPos.X, spawnPos.Y, type);
			}
			else if (NPC.CountNPCS(type) >= 2)
			{
				foreach (NPC npc in Main.npc)
				{
					if (npc != null && npc.type == type && npc.active)
					{
						npc.active = false;
					}
					if (NPC.CountNPCS(type) <= 1)
					{
						break;
					}
				}
			}
		}
	}

	public static bool InYggdrasilTown(Vector2 worldCoordiante)
	{
		return InYggdrasilTown(worldCoordiante.ToTileCoordinates());
	}

	public static bool InYggdrasilTown(Point tileCoordiante)
	{
		if (InCanteen_YggdrasilTown() || InUnion_YggdrasilTown() || InPlayerRoom_YggdrasilTown() || InArena_YggdrasilTown())
		{
			return true;
		}
		if (SubworldSystem.Current is YggdrasilWorld)
		{
			return tileCoordiante.X >= TownArea.X && tileCoordiante.X <= TownArea.X + TownArea.Width && tileCoordiante.Y >= TownArea.Y && tileCoordiante.Y <= TownArea.Y + TownArea.Height;
		}
		return false;
	}

	public static bool InCanteen_YggdrasilTown()
	{
		if (SubworldSystem.Current is RoomWorld)
		{
			return TileUtils.SafeGetTile(20, 20).TileType == ModContent.TileType<CanteenCommandBlock>();
		}
		return false;
	}

	public static bool InUnion_YggdrasilTown()
	{
		if (SubworldSystem.Current is RoomWorld)
		{
			return TileUtils.SafeGetTile(20, 20).TileType == ModContent.TileType<UnionCommandBlock>();
		}
		return false;
	}

	public static bool InPlayerRoom_YggdrasilTown()
	{
		if (SubworldSystem.Current is RoomWorld)
		{
			return TileUtils.SafeGetTile(20, 20).TileType == ModContent.TileType<PlayerRoomCommandBlock>();
		}
		return false;
	}

	public static bool InArena_YggdrasilTown()
	{
		if (SubworldSystem.Current is RoomWorld)
		{
			return TileUtils.SafeGetTile(20, 20).TileType == ModContent.TileType<ArenaCommandBlock>();
		}
		return false;
	}

	public static bool InGravelStore_YggdrasilTown()
	{
		if (SubworldSystem.Current is RoomWorld)
		{
			return TileUtils.SafeGetTile(20, 20).TileType == ModContent.TileType<GravelStoreCommandBlock>();
		}
		return false;
	}

	public static bool InFurnace_YggdrasilTown()
	{
		if (Main.dedServ)
		{
			return false;
		}
		var tileCoordiante = Main.LocalPlayer.Center.ToTileCoordinates();
		if (SubworldSystem.Current is YggdrasilWorld)
		{
			return tileCoordiante.X >= TownArea.X && tileCoordiante.X <= TownArea.X + TownArea.Width && tileCoordiante.Y >= TownArea.Y + TownArea.Height / 2 && tileCoordiante.Y <= TownArea.Y + TownArea.Height;
		}
		return false;
	}

	public static void TryEnterArena()
	{
		if (FightingRequestPlayerNPCType.X >= 0 && FightingRequestPlayerNPCType.Y >= 0)
		{
			int i = 595;
			int j = 20674;
			for (int x = -8; x < 9; x++)
			{
				for (int y = -8; y < 9; y++)
				{
					Tile tile = TileUtils.SafeGetTile(i + x, j + y);
					if (tile.TileType == ModContent.TileType<RoadSignPost_ToArena>())
					{
						if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
						{
							i += x;
							j += y;
							x = 100;
							break;
						}
					}
				}
			}
			Point point = new Point(i, j);
			RoomManager.EnterNextLevelRoom(point, new Point(60, 144), RoadSignPost_ToArenaVFX.BuildArenaGen);
			ResetedArena = false;
		}
	}

	public static Vector2 GetTownCoord(Vector2 worldCoordiante)
	{
		return worldCoordiante - new Point(430, Main.maxTilesY - 400).ToWorldCoordinates();
	}
}

public class ArenaPlayer : ModPlayer
{
	public NPC TargetBoss = null;

	public bool StartFighting = false;

	public List<TownNPC_LiveInYggdrasil.BossTag> Tags = new List<TownNPC_LiveInYggdrasil.BossTag>();

	public float DamageReduce = 0f;

	public bool MouseInTagUIPanel = false;

	public int ShieldCooling = 0;

	public override void OnRespawn()
	{
		if (YggdrasilTownCentralSystem.InArena_YggdrasilTown())
		{
			ResetNPC();
		}
		base.OnRespawn();
	}

	public void ResetNPC()
	{
		TargetBoss = null;
		foreach (var npc in Main.npc)
		{
			if (npc != null && npc.active && npc.ModNPC is TownNPC_LiveInYggdrasil)
			{
				TargetBoss = npc;
				break;
			}
		}
		if (TargetBoss != null)
		{
			TownNPC_LiveInYggdrasil tNLIY = TargetBoss.ModNPC as TownNPC_LiveInYggdrasil;
			if (tNLIY != null)
			{
				Tags = tNLIY.MyBossTags;
				if (!tNLIY.StartedFight)
				{
					Player.statLife = Player.statLifeMax2;
				}
			}
		}
	}

	public override void PostUpdateEquips()
	{
		if (YggdrasilTownCentralSystem.InArena_YggdrasilTown())
		{
			DamageReduce = 1f;
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
			if (TargetBoss != null)
			{
				TownNPC_LiveInYggdrasil tNLIY = TargetBoss.ModNPC as TownNPC_LiveInYggdrasil;
				if (tNLIY != null)
				{
					Tags = tNLIY.MyBossTags;
					if (!tNLIY.StartedFight)
					{
						Player.statLife = Player.statLifeMax2;
					}
				}
			}
			if (Tags is not null)
			{
				int origLife = Player.statLifeMax;
				foreach (var tag in Tags)
				{
					if (tag.Name == "PlayerDamageReduce10" && tag.Enable)
					{
						DamageReduce -= 0.1f;
					}
					if (tag.Name == "PlayerDamageReduce20" && tag.Enable)
					{
						DamageReduce -= 0.2f;
					}
					if (tag.Name == "PlayerDamageReduce30" && tag.Enable)
					{
						DamageReduce -= 0.3f;
					}

					if (tag.Name == "PlayerDefenseReduce20" && tag.Enable)
					{
						Player.statDefense -= 20;
					}
					if (tag.Name == "PlayerDefenseReduce30" && tag.Enable)
					{
						Player.statDefense -= 30;
					}

					if (tag.Name == "PlayerLifeReduce25" && tag.Enable)
					{
						Player.statLifeMax2 -= (int)(origLife * 0.25f);
					}
					if (tag.Name == "PlayerLifeReduce50" && tag.Enable)
					{
						Player.statLifeMax2 -= (int)(origLife * 0.5f);
					}
				}
				Player.GetDamage(DamageClass.Generic) *= DamageReduce;
			}
			if (ShieldCooling <= 0 && Player.ownedProjectileCounts[ModContent.ProjectileType<PlayerDefence>()] <= 0)
			{
				ShieldCooling = 0;
				var tile = TileUtils.SafeGetTile((Player.Bottom + new Vector2(0, 16)).ToTileCoordinates());
				if (Player.controlDown && Player.velocity.Y <= 0.05f && Collision.SolidCollision(Player.BottomLeft, Player.width, 16) && tile.TileType == ModContent.TileType<ShieldTile>() && tile.TileFrameX == 0 && tile.TileFrameY == 0)
				{
					tile.TileFrameX = 342;
					tile.TileFrameY = 36;
					ShieldCooling = 60;
					Projectile.NewProjectileDirect(Player.GetSource_FromAI(), Player.Center, Vector2.zeroVector, ModContent.ProjectileType<PlayerDefence>(), 0, 0, Player.whoAmI);
				}
			}
			else
			{
				ShieldCooling--;
			}
		}
		base.PostUpdateEquips();
	}

	public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
	{
		if (YggdrasilTownCentralSystem.InArena_YggdrasilTown())
		{
			if (Tags is not null)
			{
				foreach (var tag in Tags)
				{
					if (tag.Name == "HalfHealthPotion" && tag.Enable)
					{
						healValue /= 2;
					}
					if (tag.Name == "BanHealthPotion" && tag.Enable)
					{
						healValue *= 0;

						// Main.NewText(item.healLife);
					}
				}
			}
		}

		base.GetHealLife(item, quickHeal, ref healValue);
	}

	public override bool CanUseItem(Item item)
	{
		if (YggdrasilTownCentralSystem.InArena_YggdrasilTown())
		{
			if (Tags is not null)
			{
				foreach (var tag in Tags)
				{
					if (tag.Name == "BanHealthPotion" && tag.Enable)
					{
						if (item.healLife > 0)
						{
							return false;
						}
					}
					if (tag.Name == "BanHealthPotion" && tag.Enable)
					{
						if (item.healLife > 0)
						{
							return false;
						}
					}
					if (tag.Name == "DisableCreate" && tag.Enable)
					{
						if (item.pick > 0)
						{
							return false;
						}
						if (item.createTile >= TileID.Dirt || item.createWall >= 0)
						{
							return false;
						}
					}
				}
			}
			if (MouseInTagUIPanel)
			{
				MouseInTagUIPanel = false;
				return false;
			}
		}
		return base.CanUseItem(item);
	}

	public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
	{
		if (YggdrasilTownCentralSystem.InArena_YggdrasilTown())
		{
			if (TargetBoss != null && TargetBoss.active)
			{
				TownNPC_LiveInYggdrasil tNLIY = TargetBoss.ModNPC as TownNPC_LiveInYggdrasil;
				if (tNLIY != null)
				{
					tNLIY.PopFailVFX();
				}
			}
		}
		base.Kill(damage, hitDirection, pvp, damageSource);
	}

	public bool Dodge;

	public override bool FreeDodge(Player.HurtInfo info)
	{
		if (Dodge)
		{
			Dodge = false;
			return true;
		}
		return base.FreeDodge(info);
	}

	public void PreHurt(ref Player.HurtInfo info)
	{
		if (YggdrasilTownCentralSystem.InArena_YggdrasilTown())
		{
			if (Player.ownedProjectileCounts[ModContent.ProjectileType<PlayerDefence>()] > 0)
			{
				Player.immuneTime = 60;
				foreach (var proj in Main.projectile)
				{
					if (proj != null && proj.active && proj.owner == Player.whoAmI && proj.type == ModContent.ProjectileType<PlayerDefence>())
					{
						proj.Kill();
						break;
					}
				}
				Dodge = true;
				Player.immune = true;
				Player.immuneTime = 30;
				Player.noKnockback = true;
				CombatText.NewText(new Rectangle((int)Player.Center.X - 10, (int)Player.Center.Y - 10, 20, 20), Color.White, "Block!");
			}
		}
	}

	public override void ModifyHurt(ref Player.HurtModifiers modifiers)
	{
		modifiers.ModifyHurtInfo += new Player.HurtModifiers.HurtInfoModifier(this.PreHurt);
		if (Dodge)
		{
			modifiers.DisableDust();
			modifiers.DisableSound();
		}
		base.ModifyHurt(ref modifiers);
	}
}
