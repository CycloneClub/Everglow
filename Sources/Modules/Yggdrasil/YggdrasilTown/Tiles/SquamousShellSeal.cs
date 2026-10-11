using Everglow.Yggdrasil.Netcode;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.SquamousShell;
using Terraria.DataStructures;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class SquamousShellSeal : ModTile
{
	/// <summary>
	/// Respawn cooldown
	/// </summary>
	public int ReSpawnTimer = 0;

	/// <summary>
	/// Used for dissolve VFX
	/// </summary>
	public int DissolveTimer = 0;

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = false;

		Main.tileWaterDeath[Type] = false;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 10;
		TileObjectData.newTile.Width = 20;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
		};
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.addTile(Type);
		AddMapEntry(new Color(79, 76, 75));
	}

	public override bool CanExplode(int i, int j)
	{
		return false;
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = 0;
		base.NumDust(i, j, fail, ref num);
	}

	public override bool CanKillTile(int i, int j, ref bool blockDamaged)
	{
		return false;
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
		{
			Color lightColor = Lighting.GetColor(i + 10, j + 5);
			var zero = new Vector2(Main.offScreenRange);
			if (Main.drawToScreen)
			{
				zero = Vector2.Zero;
			}

			Texture2D deadS = ModAsset.DeadSquamousShell.Value;
			Texture2D frontSeal = ModAsset.SquamousShellSeal_front.Value;
			Texture2D backSeal = ModAsset.SquamousShellSeal_back.Value;
			Texture2D lightEffect = ModAsset.SquamousShellSeal.Value;
			Rectangle firstSealRect = new Rectangle(0, 0, 342, 152);
			if (ReSpawnTimer <= 0)
			{
				spriteBatch.Draw(backSeal, new Vector2(i, j) * 16 - Main.screenPosition + zero + new Vector2(8), firstSealRect, lightColor, 0, Vector2.zeroVector, 1f, SpriteEffects.None, 0);
				spriteBatch.Draw(lightEffect, new Vector2(i - 5, j) * 16 - Main.screenPosition + zero + new Vector2(8), null, new Color(1f, 1f, 1f, 0), 0, Vector2.zeroVector, 1f, SpriteEffects.None, 0);
				spriteBatch.Draw(deadS, new Vector2(i, j) * 16 - Main.screenPosition + zero + new Vector2(8), null, lightColor, 0, Vector2.zeroVector, 1f, SpriteEffects.None, 0);
				spriteBatch.Draw(frontSeal, new Vector2(i, j) * 16 - Main.screenPosition + zero + new Vector2(8), firstSealRect, lightColor, 0, Vector2.zeroVector, 1f, SpriteEffects.None, 0);
			}
			else if (ReSpawnTimer < 300)
			{
				Vector2 dive = new Vector2(0, ReSpawnTimer);
				spriteBatch.Draw(backSeal, new Vector2(i, j) * 16 - Main.screenPosition + zero + dive + new Vector2(8), firstSealRect, lightColor, 0, Vector2.zeroVector, 1f, SpriteEffects.None, 0);
				float colorValue = (150 - ReSpawnTimer) / 150f;
				colorValue = Math.Max(0, colorValue);
				spriteBatch.Draw(lightEffect, new Vector2(i - 5, j) * 16 - Main.screenPosition + zero + new Vector2(8), null, new Color(colorValue, colorValue, colorValue, 0), 0, Vector2.zeroVector, 1f, SpriteEffects.None, 0);
				spriteBatch.Draw(deadS, new Vector2(i, j) * 16 - Main.screenPosition + zero + dive + new Vector2(8), null, lightColor, 0, Vector2.zeroVector, 1f, SpriteEffects.None, 0);
				spriteBatch.Draw(frontSeal, new Vector2(i, j) * 16 - Main.screenPosition + zero + dive + new Vector2(8), firstSealRect, lightColor, 0, Vector2.zeroVector, 1f, SpriteEffects.None, 0);
			}
			if (NPC.CountNPCS(ModContent.NPCType<SquamousShell>()) == 0)
			{
				if (!Main.gamePaused)
				{
					if (ReSpawnTimer > 300)
					{
						ReSpawnTimer = 300;
					}
					ReSpawnTimer--;
				}
			}
			if (DissolveTimer > 0)
			{
				float colorValue = DissolveTimer / 60f;
				int frontValue = 60 - DissolveTimer;
				int backValue = 60 - DissolveTimer;
				Rectangle backSealRect = new Rectangle(0, backValue * 152, 342, 152);
				Rectangle frontSealRect = new Rectangle(0, frontValue * 152, 342, 152);
				if (backValue < 7)
				{
					spriteBatch.Draw(backSeal, new Vector2(i, j) * 16 - Main.screenPosition + zero + new Vector2(8), backSealRect, lightColor, 0, Vector2.zeroVector, 1f, SpriteEffects.None, 0);
				}
				spriteBatch.Draw(lightEffect, new Vector2(i - 5, j) * 16 - Main.screenPosition + zero + new Vector2(8), null, new Color(colorValue, colorValue, colorValue, 0), 0, Vector2.zeroVector, 1f, SpriteEffects.None, 0);
				if (DissolveTimer > 30)
				{
					spriteBatch.Draw(deadS, new Vector2(i, j) * 16 - Main.screenPosition + zero + new Vector2(8), null, lightColor, 0, Vector2.zeroVector, 1f, SpriteEffects.None, 0);
				}
				if (frontValue < 14)
				{
					spriteBatch.Draw(frontSeal, new Vector2(i, j) * 16 - Main.screenPosition + zero + new Vector2(8), frontSealRect, lightColor, 0, Vector2.zeroVector, 1f, SpriteEffects.None, 0);
				}
				if (!Main.gamePaused)
				{
					DissolveTimer--;
				}
			}
			else
			{
				DissolveTimer = 0;
			}
		}
		return false;
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		base.NearbyEffects(i, j, closer);
	}

	public override void MouseOver(int i, int j)
	{
		base.MouseOver(i, j);
	}

	public override bool RightClick(int i, int j)
	{
		if (ReSpawnTimer <= 0)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				ModIns.PacketResolver.Send(new SquamousShellSummonPacket { TileX = i, TileY = j });
			}
			else if (Main.netMode == NetmodeID.SinglePlayer)
			{
				ModContent.GetInstance<SquamousShellSeal>().TrySummon(i, j, Main.myPlayer);
			}
		}
		return true;
	}

	public bool TrySummon(int i, int j, int playerIndex)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient
			|| playerIndex < 0
			|| playerIndex >= Main.maxPlayers
			|| !WorldGen.InWorld(i, j))
		{
			return false;
		}

		Player player = Main.player[playerIndex];
		Tile clickedTile = Main.tile[i, j];
		if (!player.active || player.dead || !player.InInteractionRange(i, j, TileReachCheckSettings.Simple)
			|| !clickedTile.HasTile || clickedTile.TileType != Type || NPC.AnyNPCs(ModContent.NPCType<SquamousShell>()))
		{
			return false;
		}

		for (int x = -20; x <= 20; x++)
		{
			for (int y = -10; y <= 10; y++)
			{
				if (!WorldGen.InWorld(i + x, j + y))
				{
					continue;
				}
				Tile tile = Main.tile[i + x, j + y];
				if (tile.HasTile && tile.TileType == Type && tile.TileFrameX == 180 && tile.TileFrameY == 162)
				{
					int index = NPC.NewNPC(new EntitySource_TileInteraction(player, i, j), (i + x) * 16 + 18, (j + y) * 16 + 14, ModContent.NPCType<SquamousShell>());
					Main.npc[index].netUpdate = true;
					if (NetUtils.IsServer)
					{
						NetMessage.SendData(MessageID.SyncNPC, number: index);
					}

					ApplyPostSummonEffect();
					return true;
				}
			}
		}
		return false;
	}

	/// <summary>
	/// Set timers after summon, should be applyed on all ends.
	/// </summary>
	public void ApplyPostSummonEffect()
	{
		ReSpawnTimer = 360000;
		DissolveTimer = 60;
	}
}
