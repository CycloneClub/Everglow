using Everglow.Yggdrasil.YggdrasilTown.Items.Miscs;
using Terraria.GameContent;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

[AutoloadHead]
public class QuestCounter_Consultant : ModNPC
{
	public Point TilePos;

	public override void SetDefaults()
	{
		NPC.width = 38;
		NPC.height = 32;
		NPC.townNPC = true;
		NPC.defense = 100;
		NPC.lifeMax = 250;
		NPC.aiStyle = NPCAIStyleID.Passive;
		NPC.noTileCollide = true;
	}

	public override bool PreAI() => base.PreAI();

	public override void AI()
	{
		if (TilePos == default)
		{
			NPC.active = false;
		}
		NPC.Center = TilePos.ToWorldCoordinates() + new Vector2(170, 66);
		NPC.velocity *= 0;
		NPC.noTileCollide = true;
		NPC.noGravity = true;
		int playerIndex = Player.FindClosest(NPC.position, NPC.width, NPC.height);
		if (playerIndex >= 0)
		{
			Player player = Main.player[playerIndex];
			if (player.Center.X < NPC.Center.X)
			{
				NPC.spriteDirection = -1;
			}
			if (player.Center.X > NPC.Center.X)
			{
				NPC.spriteDirection = 1;
			}
		}
		base.AI();
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => base.PreDraw(spriteBatch, screenPos, drawColor);

	public override bool CanTownNPCSpawn(int numTownNPCs) => false;

	public override bool CanChat() => base.CanChat();

	public override void ChatBubblePosition(ref Vector2 position, ref SpriteEffects spriteEffects)
	{
		position.Y += 16;
		base.ChatBubblePosition(ref position, ref spriteEffects);
	}

	public override void EmoteBubblePosition(ref Vector2 position, ref SpriteEffects spriteEffects)
	{
		position.Y += 16;
		base.EmoteBubblePosition(ref position, ref spriteEffects);
	}

	public override string GetChat() => base.GetChat();

	public override void RegisterChatButtons(NPCInteractionList interactions)
	{
		interactions.InsertBefore(NPCInteractions.Shop("Yggdrasil Town Union Shop"), NPCInteractionDatabase.CloseButton);
	}

	public override void AddShops()
	{
		NPCShop shop = new NPCShop(Type, "Yggdrasil Town Union Shop");
		shop.Add<YggdrasilTownAccessCard>();
		shop.Add<YggdrasilPlayerRoomDoorKey>();
		shop.Register();
	}
}
