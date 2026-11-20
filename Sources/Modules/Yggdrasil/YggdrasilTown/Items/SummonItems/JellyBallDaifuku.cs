using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.KingJellyBall;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.SummonItems;

public class JellyBallDaifuku : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonItems;

	public override void SetDefaults()
	{
		Item.consumable = true;
		Item.width = 42;
		Item.height = 24;
		Item.value = 7500;
		Item.useTime = 12;
		Item.useAnimation = 12;
		Item.noMelee = true;
		Item.maxStack = Item.CommonMaxStack;
		Item.useTurn = true;
		Item.UseSound = SoundID.Roar;
		Item.useStyle = ItemUseStyleID.Swing;
	}

	public override bool? UseItem(Player player)
	{
		if (!CanUseItem(player))
		{
			return false;
		}
		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			var npc = NPC.NewNPCDirect(Item.GetNPCSource_FromThis(), player.Center + new Vector2(player.direction * 1800, 0), ModContent.NPCType<KingJellyBall>());
			npc.velocity.X = -player.direction * 12;
			npc.netUpdate = true;
		}
		return true;
	}

	public override bool CanUseItem(Player player)
	{
		return !NPC.AnyNPCs(ModContent.NPCType<KingJellyBall>());
	}

	public override void AddRecipes()
	{
		CreateRecipe(1)
			.AddIngredient(ModContent.ItemType<JellyBallCube>(), 60)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
