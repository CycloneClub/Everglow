using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs;

public class DarkGlimmeringRods : ModNPC
{
	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 8;
		NPCSpawnManager.RegisterNPC(Type);
	}
	public override void SetDefaults()
	{
		NPC.width = 40;
		NPC.height = 40;
		NPC.lifeMax = 188;
		NPC.damage = 22;
		NPC.defense = 8;
		NPC.friendly = false;
		NPC.aiStyle = -1;
		NPC.knockBackResist = 0.2f;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;
		NPC.noGravity = true;
	}
	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		YggdrasilTownBiome YggdrasilTownBiome = ModContent.GetInstance<YggdrasilTownBiome>();
		if (!YggdrasilTownBiome.IsBiomeActive(Main.LocalPlayer))
			return 0f;
		return 3f;
	}
	public int BodyLength = 20;
	public Vector2 TargetPos = Vector2.zeroVector;
	public override void OnSpawn(IEntitySource source)
	{
		NPC.scale = Main.rand.NextFloat(0.85f, 1.15f);
		NPC.lifeMax = (int)(NPC.lifeMax * NPC.scale);
		NPC.life = NPC.lifeMax;
		BodyLength = Main.rand.Next(14, 26);
		TargetPos = NPC.Center + new Vector2(Main.rand.Next(-210, 210), Main.rand.Next(-210, -50));
	}
	public override void AI()
	{
		NPC.frameCounter++;
		float timeValue = (float)(Main.time * 0.014f);
		NPC.TargetClosest();
		Player player = Main.player[NPC.target];
		if (NPC.life < NPC.lifeMax)
		{
			TargetPos = player.Center;
		}
		NPC.velocity *= 0.97f;
		NPC.rotation = MathF.Log(MathF.Abs(NPC.velocity.X) + 1) * 0.2f * NPC.direction * 0.05f + NPC.rotation * 0.95f;
		Vector2 aimTarget = TargetPos + new Vector2(210f * MathF.Sin(timeValue * 2 + NPC.whoAmI) * NPC.scale, -50f + 30f * MathF.Sin(timeValue * 0.15f + NPC.whoAmI));
		Vector2 toAim = aimTarget - NPC.Center - NPC.velocity;
		if (toAim.Length() > 50)
		{
			NPC.velocity += Vector2.Normalize(toAim) * 0.15f * NPC.scale;
		}

	}
	public override void OnKill()
	{
		for (int f = 0; f < 2; f++)
		{
			Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), NPC.velocity + new Vector2(0, Main.rand.NextFloat(0f, 3f)).RotatedByRandom(MathHelper.TwoPi), ModContent.Find<ModGore>("Everglow/DarkGlimmeringRods_gore0").Type, NPC.scale);
			Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), NPC.velocity + new Vector2(0, Main.rand.NextFloat(0f, 3f)).RotatedByRandom(MathHelper.TwoPi), ModContent.Find<ModGore>("Everglow/DarkGlimmeringRods_gore1").Type, NPC.scale);
			Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), NPC.velocity + new Vector2(0, Main.rand.NextFloat(0f, 3f)).RotatedByRandom(MathHelper.TwoPi), ModContent.Find<ModGore>("Everglow/DarkGlimmeringRods_gore2").Type, NPC.scale);
		}
	}
	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AuburnRodSkeleton>(), 24, 1));
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RodWing>(), 1, 3, 6));
	}
	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		SpriteBatchState sBS = GraphicsUtils.GetState(spriteBatch).Value;
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Texture2D mainTex = (Texture2D)ModContent.Request<Texture2D>(Texture);
		float timeValue = (float)(NPC.frameCounter);
		List<Vertex2D> bars = new List<Vertex2D>();
		Vector2 drawCenter = NPC.Center - Main.screenPosition + new Vector2(BodyLength * 5f * 0.75f, 0).RotatedBy(NPC.rotation) * NPC.scale;
		for (int i = 0; i < BodyLength; i++)
		{
			float jointIndex = i / (float)BodyLength;
			int frameY = (int)(timeValue + i) % Main.npcFrameCount[NPC.type];
			float frameYValue = frameY / (float)Main.npcFrameCount[NPC.type];
			float deltaYValue = 1 / (float)Main.npcFrameCount[NPC.type];
			float jointScale = 0.5f + 0.5f * MathF.Sin(jointIndex * MathHelper.Pi);
			bars.Add(drawCenter + new Vector2(-20, -20).RotatedBy(NPC.rotation) * jointScale * NPC.scale, drawColor, new Vector3(0, frameYValue, 0));
			bars.Add(drawCenter + new Vector2(-20, 20).RotatedBy(NPC.rotation) * jointScale * NPC.scale, drawColor, new Vector3(0, frameYValue + deltaYValue, 0));
			bars.Add(drawCenter + new Vector2(20, -20).RotatedBy(NPC.rotation) * jointScale * NPC.scale, drawColor, new Vector3(1, frameYValue, 0));

			bars.Add(drawCenter + new Vector2(20, -20).RotatedBy(NPC.rotation) * jointScale * NPC.scale, drawColor, new Vector3(1, frameYValue, 0));
			bars.Add(drawCenter + new Vector2(-20, 20).RotatedBy(NPC.rotation) * jointScale * NPC.scale, drawColor, new Vector3(0, frameYValue + deltaYValue, 0));
			bars.Add(drawCenter + new Vector2(20, 20).RotatedBy(NPC.rotation) * jointScale * NPC.scale, drawColor, new Vector3(1, frameYValue + deltaYValue, 0));
			drawCenter -= new Vector2(10, 0).RotatedBy(NPC.rotation) * jointScale * NPC.scale;
		}
		Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
		Main.graphics.GraphicsDevice.Textures[0] = mainTex;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		if (bars.Count > 3)
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, bars.ToArray(), 0, bars.Count / 3);
		spriteBatch.End();
		spriteBatch.Begin(sBS);
		return false;
	}
}
