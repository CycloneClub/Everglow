using Everglow.Commons.Utilities;

namespace Everglow.Yggdrasil.YggdrasilTown.Travelers;

/// <summary>Experimental non-resident NPC. Its appearance is supplied by the shared traveler view.</summary>
public class TravelerNPC : ModNPC
{
	internal TravelerJourney Journey { get; set; }

	public override string LocalizationCategory => LocalizationUtils.Categories.TownNPCs;

	public override string Texture => ModAsset.InnKeeper_Mod;

	public override void SetStaticDefaults()
	{
		NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers { Hide = true });
	}

	public override void SetDefaults()
	{
		NPC.width = 16;
		NPC.height = 48;
		NPC.aiStyle = -1;
		NPC.lifeMax = 250;
		NPC.damage = 0;
		NPC.friendly = true;
		NPC.dontTakeDamage = true;
		NPC.noGravity = true;
		NPC.npcSlots = 0;
		NPC.dontCountMe = true;
		NPC.knockBackResist = 0;
	}

	public override void AI()
	{
		if (!TownTravelerSystem.Enabled || Journey == null || !Journey.Active)
		{
			NPC.active = false;
			return;
		}
		NPC.timeLeft = 600;
		Journey.Update();
		NPC.noTileCollide = Journey.IsAttached;
		NPC.direction = Journey.Direction;
	}

	public override bool CheckActive() => false;

	public override bool CanChat() => false;

	public override bool? CanFallThroughPlatforms() => Journey?.FallThroughPlatforms == true;

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => false;
}
