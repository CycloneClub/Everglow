using Everglow.Myth.TheFirefly.Projectiles;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Myth.TheFirefly.NPCs;

public class BlackStarFruit : ModNPC
{
	/// <summary>
	/// The time of the light from the entity. Used with <see cref="vectorRotationPosition"/> and <see cref="floatLightPosition"/>
	/// </summary>
	private float entityLightTime = 0;
	private Vector2[] vectorRotationPosition = new Vector2[12];
	private float[] floatLightPosition = new float[12];

	// public override string Texture => "Everglow/" + ModAsset.FruitBomb_Path; // Best way I found to avoid path errors, and FruitBomb is empty anyways
	public override void SetDefaults()
	{
		NPC.damage = 0;
		NPC.width = 40;
		NPC.height = 40;
		NPC.defense = 0;
		NPC.lifeMax = 1;
		NPC.knockBackResist = 0f;
		NPC.value = Item.buyPrice(0, 0, 0, 0);
		NPC.color = new Color(0, 0, 0, 0);
		NPC.alpha = 0;
		NPC.lavaImmune = true;
		NPC.noGravity = true;
		NPC.noTileCollide = true;
		NPC.behindTiles = true;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;
		NPC.aiStyle = -1;
	}

	public override void OnSpawn(IEntitySource source)
	{
		NPC.position.Y -= 35f;
		NPC.position.X += Main.rand.NextFloat(-16, 16);
		foreach (var npc in Main.npc)
		{
			if (npc.active && npc.type == Type)
			{
				if ((npc.Center - NPC.Center).Length() < 200)
				{
					NPC.active = false;
					return;
				}
			}
		}
	}

	public override float SpawnChance(NPC.Spawner spawner)
	{
		FireflyBiome fireflyBiome = ModContent.GetInstance<FireflyBiome>();
		if (!fireflyBiome.IsBiomeActive(Main.LocalPlayer))
		{
			return 0f;
		}

		return 0.5f;
	}

	public override void AI()
	{
		NPC.dontTakeDamage = false;
		NPC.velocity = new Vector2(0, (float)Math.Sin(Main.time / 50f + NPC.Center.X + NPC.whoAmI) * 0.25f);
		InitFruitLightEffect();
		Lighting.AddLight(NPC.Center, 0, 0.1f, 0.8f);
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<FruitBomb>(), 50, 3);
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		SpriteEffects effects = SpriteEffects.None;
		if (NPC.spriteDirection == 1)
		{
			effects = SpriteEffects.FlipHorizontally;
		}

		Texture2D fruit = ModAsset.BlackStarFruit.Value;
		Main.spriteBatch.Draw(fruit, NPC.Center - Main.screenPosition, null, new Color(10, 83, 110, 0), NPC.rotation, fruit.Size() / 2f, 0.025f * (float)(4 + Math.Sin(Main.time / 15d + NPC.position.X / 36d)), effects, 0f);
		return false;
	}

	public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		SpriteEffects effects = SpriteEffects.None;
		if (NPC.spriteDirection == 1)
		{
			effects = SpriteEffects.FlipHorizontally;
		}

		Texture2D lightShell = ModAsset.Lightball.Value;
		Texture2D lightFruit = ModAsset.BlackStarFruitLight.Value;
		var color = new Color(10, 83, 110, 0);
		Main.spriteBatch.Draw(lightShell, NPC.Center - Main.screenPosition, null, color, NPC.rotation, lightShell.Size() / 2f, 0.025f * (float)(4 + Math.Sin(Main.time / 15d + NPC.position.X / 36d)), effects, 0f);
		for (int j = 0; j < 12; j++)
		{
			Main.spriteBatch.Draw(lightFruit, NPC.Center - Main.screenPosition + vectorRotationPosition[j] * floatLightPosition[j] * entityLightTime, null, new Color(1 - floatLightPosition[j], 1 - floatLightPosition[j], 1 - floatLightPosition[j], 0), NPC.rotation, new Vector2(5, 5), 1.2f * entityLightTime * (1 - floatLightPosition[j]), effects, 0f);
		}
	}

	public override bool? CanBeHitByProjectile(Projectile projectile)
	{
		if (projectile.type == ModContent.ProjectileType<FruitBomb>())
		{
			return false;
		}
		return base.CanBeHitByProjectile(projectile);
	}

	private void InitFruitLightEffect()
	{
		if (entityLightTime < 1)
		{
			entityLightTime += 0.01f;
		}
		for (int j = 0; j < 12; j++)
		{
			floatLightPosition[j] *= 0.98f;
			if (entityLightTime <= 0.2f)
			{
				vectorRotationPosition[j] = new Vector2(0, 24).RotatedByRandom(3.1415926535 * 2);
				floatLightPosition[j] = Main.rand.NextFloat(0f, 1f);
			}
			if (floatLightPosition[j] < 0.07f)
			{
				floatLightPosition[j] = 1;
				vectorRotationPosition[j] = new Vector2(0, 20).RotatedByRandom(3.1415926535 * 2);
			}
		}
	}
}
