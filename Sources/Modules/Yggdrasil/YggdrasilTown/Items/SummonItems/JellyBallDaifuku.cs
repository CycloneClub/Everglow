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

	public override bool ConsumeItem(Player player)
	{
		return true;
	}

	public override bool? UseItem(Player player)
	{
		return base.UseItem(player);
	}

	public override bool CanUseItem(Player player)
	{
		int type = ModContent.NPCType<KingJellyBall>();
		if (NPC.CountNPCS(type) <= 0)
		{
			var npc = NPC.NewNPCDirect(Item.GetNPCSource_FromThis(), player.Center + new Vector2(player.direction * 1800, 0), type);
			npc.velocity.X = -player.direction * 12;
			Item.stack--;
			if (Item.stack <= 0)
			{
				Item.TurnToAir();
			}
			return true;
		}
		else
		{
			return false;
		}
	}

	public override void AddRecipes()
	{
		CreateRecipe(1)
			.AddIngredient(ModContent.ItemType<JellyBallCube>(), 60)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
