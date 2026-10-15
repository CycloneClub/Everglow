using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.WorldBuilding;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs;

public class WhisperingGhost : ModNPC
{
	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 9;
		NPCSpawnManager.RegisterNPC(Type);
	}
	public override void SetDefaults()
	{

		NPC.width = 40;
		NPC.height = 40;
		NPC.lifeMax = 40;
		NPC.damage = 12;
		NPC.defense = 4;
		NPC.friendly = false;
		NPC.aiStyle = -1;
		NPC.knockBackResist = 0.15f;
		NPC.value = 60;
		NPC.HitSound = SoundID.NPCHit5;
		NPC.DeathSound = SoundID.NPCDeath5;
		NPC.noGravity = true;
		NPC.noTileCollide = false;
	}
	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		YggdrasilTownBiome YggdrasilTownBiome = ModContent.GetInstance<YggdrasilTownBiome>();
		if (!YggdrasilTownBiome.IsBiomeActive(Main.LocalPlayer))
			return 0f;
		return 3f;
	}
	public override void AI()
	{
		NPC.velocity = new Vector2(0, -1);
		float timeValue = (float)(Main.time * 0.035f);
		Vector2 toAim = Vector2.Zero;
		NPC.TargetClosest();
		Player target = Main.player[NPC.target];
		Vector2 aimTarget = target.Center;
		if (Collision.CanHit(NPC.Center, 1, 1, target.Center, 1, 1))
		{
			toAim = aimTarget - NPC.Center + new Vector2(0, MathF.Sin(timeValue + NPC.whoAmI) * 200);
			NPC.velocity = Vector2.Normalize(toAim) * NPC.velocity.Length() * 1.2f;
			if (NPC.velocity.Length() >= 2.5f)
			{
				NPC.velocity = Vector2.Normalize(NPC.velocity) * 2.5f;
			}
		}
		else
		{
			toAim = new Vector2(MathF.Cos(0.5f * (timeValue + NPC.whoAmI)), MathF.Sin(0.75f * (timeValue + NPC.whoAmI)));
			NPC.velocity = Vector2.Normalize(toAim) * NPC.velocity.Length() * 0.9f;
			if (NPC.velocity.Length() <= 0.75f)
			{
				NPC.velocity = Vector2.Normalize(NPC.velocity) * 0.75f;
			}
		}
		if (WorldUtils.Find(NPC.Center.ToTileCoordinates(), Searches.Chain(new Searches.Down(5), _cachedConditions_notNull, _cachedConditions_solid), out var _))
		{
			float length = NPC.velocity.Length();
			NPC.velocity += new Vector2(0, -0.5f);
			NPC.velocity = Vector2.Normalize(NPC.velocity) * length;
		}
		else
		{
			float length = NPC.velocity.Length();
			NPC.velocity += new Vector2(0, 0.125f);
			NPC.velocity = Vector2.Normalize(NPC.velocity) * length;
		}
		foreach (NPC npc in Main.npc)
		{
			if (npc != null && npc.active && npc != NPC)
			{
				if (npc.type == Type)
				{
					Vector2 v0 = NPC.Center - npc.Center;
					if (v0.Length() < 120)
					{
						NPC.velocity += Vector2.Normalize(v0) * 0.2f;
					}
				}
			}
		}
		NPC.spriteDirection = NPC.velocity.X > 0 ? 1 : -1;
		NPC.localAI[0] += 1;
		if (NPC.localAI[0] > 8)
		{
			NPC.localAI[0] = 0;
			Dust dust = Dust.NewDustDirect(NPC.Center - new Vector2(4) + new Vector2(Main.rand.NextFloat(-15, 15), 15), 0, 0, ModContent.DustType<WhisperingGhostGasSmalling>());
			dust.velocity = new Vector2(NPC.velocity.X, 3);
			dust.scale = Main.rand.NextFloat(2.2f, 3f);
			dust.rotation = Main.rand.NextFloat(0.4f, 0.8f);
			dust.alpha = Main.rand.Next(0, 55);
		}

	}
	private static Terraria.WorldBuilding.Conditions.NotNull _cachedConditions_notNull = new Terraria.WorldBuilding.Conditions.NotNull();
	private static Terraria.WorldBuilding.Conditions.IsSolid _cachedConditions_solid = new Terraria.WorldBuilding.Conditions.IsSolid();
	public override void FindFrame(int frameHeight)
	{
		frameHeight = 120;
		NPC.frameCounter++;
		if (NPC.frameCounter > 4)
		{
			NPC.frame.Y += frameHeight;
			NPC.frameCounter = 0;
		}
		if (NPC.frame.Y >= 1080)
		{
			NPC.frame.Y = 0;
		}
	}
	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		Texture2D texture = ModAsset.WhisperingGhost.Value;
		Texture2D glow = ModAsset.WhisperingGhost_glow.Value;
		SpriteEffects spriteEffect = SpriteEffects.None;
		if (NPC.spriteDirection == -1)
		{
			spriteEffect = SpriteEffects.FlipHorizontally;
		}
		spriteBatch.Draw(texture, NPC.Center - Main.screenPosition + new Vector2(0, 30), NPC.frame, drawColor, 0, NPC.frame.Size() * 0.5f, NPC.scale, spriteEffect, 0);
		spriteBatch.Draw(glow, NPC.Center - Main.screenPosition + new Vector2(0, 30), NPC.frame, new Color(1f, 1f, 1f, 0) * 0.6f, 0, NPC.frame.Size() * 0.5f, NPC.scale, spriteEffect, 0);
		return false;
	}
	public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
	{
		target.AddBuff(BuffID.Silenced, 180);
	}
	public override void OnSpawn(IEntitySource source)
	{
	}
	public override void HitEffect(NPC.HitInfo hit)
	{
		for (int x = 0; x < 3; x++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 4f)).RotatedByRandom(MathHelper.TwoPi);
			newVelocity.Y *= 2;
			Dust dust = Dust.NewDustDirect(NPC.Center - new Vector2(0, 20), 0, 0, ModContent.DustType<WhisperingGhostGas>());
			dust.velocity = newVelocity;
			dust.scale = Main.rand.NextFloat(1.2f, 3f);
			dust.rotation = Main.rand.NextFloat(0.4f, 0.8f);
			dust.alpha = Main.rand.Next(0, 55);
		}
		for (int x = 0; x < 2; x++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 4f)).RotatedByRandom(MathHelper.TwoPi);
			newVelocity.Y *= 2;
			Dust dust = Dust.NewDustDirect(NPC.Center - new Vector2(0, 20), 0, 0, ModContent.DustType<WhisperingGhostGas_hot>());
			dust.velocity = newVelocity;
			dust.scale = Main.rand.NextFloat(0.8f, 1.6f);
			dust.rotation = Main.rand.NextFloat(0.4f, 0.8f);
			dust.alpha = Main.rand.Next(0, 55);
		}
	}
	public override void OnKill()
	{
		for (int x = 0; x < 40; x++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 8f)).RotatedByRandom(MathHelper.TwoPi);
			newVelocity.Y *= 1.4f;
			Dust dust = Dust.NewDustDirect(NPC.Center - new Vector2(0, 20), 0, 0, ModContent.DustType<WhisperingGhostGas>());
			dust.velocity = newVelocity;
			dust.scale = Main.rand.NextFloat(1.2f, 3f);
			dust.rotation = Main.rand.NextFloat(0.4f, 0.8f);
			dust.alpha = Main.rand.Next(0, 55);
		}
		for (int x = 0; x < 20; x++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 8f)).RotatedByRandom(MathHelper.TwoPi);
			newVelocity.Y *= 1.4f;
			Dust dust = Dust.NewDustDirect(NPC.Center - new Vector2(0, 20), 0, 0, ModContent.DustType<WhisperingGhostGas_hot>());
			dust.velocity = newVelocity;
			dust.scale = Main.rand.NextFloat(0.8f, 1.6f);
			dust.rotation = Main.rand.NextFloat(0.4f, 0.8f);
			dust.alpha = Main.rand.Next(0, 55);
		}
	}
	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		npcLoot.Add(ItemDropRule.Common(ItemID.Megaphone, 50));
	}
}
