using Everglow.Myth.LanternMoon.Gores;
using Terraria.DataStructures;

namespace Everglow.Myth.LanternMoon.NPCs;

public class EvilLantern : LanternMoonNPC
{
	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 8;
	}

	public override void SetDefaults()
	{
		NPC.damage = 36;
		NPC.lifeMax = 104;
		NPC.npcSlots = 0.1f;
		NPC.width = 50;
		NPC.height = 36;
		NPC.defense = 6;
		NPC.value = 0;
		NPC.aiStyle = -1;
		NPC.knockBackResist = 0.9f;
		NPC.dontTakeDamage = false;
		NPC.noGravity = true;
		NPC.noTileCollide = true;
		NPC.HitSound = SoundID.NPCHit3;
		LanternMoonScore = 3f;
	}

	public override void OnSpawn(IEntitySource source)
	{
		NPC.ai[1] = Main.rand.Next(-120, 0);
		NPC.ai[2] = Main.rand.NextFloat(0.3f, 1800f);
	}

	public override void AI()
	{
		if (Main.dayTime)
		{
			NPC.velocity.Y += 1;
			float lightValue = MathF.Sin(NPC.ai[0] * 0.03f + NPC.whoAmI) * 0.5f + 0.5f;
			Lighting.AddLight(NPC.Center, new Vector3(1f, 0.7f, 0.6f) * lightValue);
			return;
		}
		NPC.TargetClosest(true);
		Player player = Main.player[NPC.target];

		NPC.rotation = NPC.velocity.X / 30f;
		NPC.ai[0] += 1;

		Vector2 v = player.Center + new Vector2((float)Math.Sin(NPC.ai[0] / 40f) * 500f, (float)Math.Sin((NPC.ai[0] + 200) / 40f) * 50f - 150) - NPC.Center;
		if (NPC.velocity.Length() < 9f)
		{
			NPC.velocity += v / v.Length() * 0.35f;
		}
		NPC.velocity *= 0.96f;
	}

	public override void FindFrame(int frameHeight)
	{
		int frameIndex = (int)(NPC.ai[0] / 6f + 1000) % 8;
		NPC.frame = new Rectangle(0, 106 * frameIndex, 62, 106);
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		if (NPC.life <= 0)
		{
			var gore0 = new BurningLanternGore
			{
				Active = true,
				Visible = true,
				Velocity = new Vector2(Main.rand.NextFloat(0, 6), 0).RotatedByRandom(6.283),
				NoGravity = false,
				Position = NPC.Center,
				Texture = ModAsset.EvilLanternGore1S.Value,
				DissolveAnimationTexture = ModAsset.EvilLanternGore1G.Value,
				NoDissolvePartTexture = ModAsset.EvilLanternGore1B.Value,
			};
			Ins.VFXManager.Add(gore0);
			var gore1 = new BurningLanternGore
			{
				Active = true,
				Visible = true,
				Velocity = new Vector2(Main.rand.NextFloat(0, 6), 0).RotatedByRandom(6.283),
				NoGravity = false,
				Position = NPC.Center,
				Texture = ModAsset.EvilLanternGore2S.Value,
				DissolveAnimationTexture = ModAsset.EvilLanternGore2G.Value,
				NoDissolvePartTexture = ModAsset.EvilLanternGore2B.Value,
			};
			Ins.VFXManager.Add(gore1);
			for (int f = 0; f < 2; f++)
			{
				var gore2 = new BurningLanternGore
				{
					Active = true,
					Visible = true,
					Velocity = new Vector2(Main.rand.NextFloat(0, 21), 0).RotatedByRandom(6.283),
					NoGravity = false,
					Position = NPC.Center,
					Texture = ModAsset.EvilLanternGore3S.Value,
					DissolveAnimationTexture = ModAsset.EvilLanternGore3G.Value,
				};
				Ins.VFXManager.Add(gore2);
				var gore3 = new BurningLanternGore
				{
					Active = true,
					Visible = true,
					Velocity = new Vector2(Main.rand.NextFloat(0, 21), 0).RotatedByRandom(6.283),
					NoGravity = false,
					Position = NPC.Center,
					Texture = ModAsset.EvilLanternGore4S.Value,
					DissolveAnimationTexture = ModAsset.EvilLanternGore4G.Value,
				};
				Ins.VFXManager.Add(gore3);
				var gore4 = new BurningLanternGore
				{
					Active = true,
					Visible = true,
					Velocity = new Vector2(Main.rand.NextFloat(0, 21), 0).RotatedByRandom(6.283),
					NoGravity = false,
					Position = NPC.Center,
					Texture = ModAsset.EvilLanternGore5S.Value,
					DissolveAnimationTexture = ModAsset.EvilLanternGore5G.Value,
				};
				Ins.VFXManager.Add(gore4);
				var gore5 = new BurningLanternGore
				{
					Active = true,
					Visible = true,
					Velocity = new Vector2(Main.rand.NextFloat(0, 21), 0).RotatedByRandom(6.283),
					NoGravity = false,
					Position = NPC.Center,
					Texture = ModAsset.EvilLanternGore6S.Value,
					DissolveAnimationTexture = ModAsset.EvilLanternGore6G.Value,
				};
				Ins.VFXManager.Add(gore5);
			}
			for (int f = 0; f < 8; f++)
			{
				Vector2 v3 = new Vector2(0, Main.rand.NextFloat(0, 12f)).RotatedByRandom(MathHelper.TwoPi);
				Dust d = Dust.NewDustDirect(NPC.Center - new Vector2(4, 4) - new Vector2(4, 4), 8, 8, ModContent.DustType<Dusts.Flame4>(), v3.X, v3.Y, 0, default, Main.rand.NextFloat(0.6f, 1.8f));
				d.noGravity = true;
				d.velocity = v3;
			}
		}
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		var texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
		SpriteEffects effects = SpriteEffects.None;
		if (NPC.spriteDirection == 1)
		{
			effects = SpriteEffects.FlipHorizontally;
		}

		Vector2 drawPos = NPC.Center - Main.screenPosition;
		spriteBatch.Draw(texture, drawPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() * 0.5f, 1f, effects, 0f);
		return false;
	}

	public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		Texture2D tg = ModAsset.EvilLantern_glow.Value;
		SpriteEffects effects = SpriteEffects.None;
		if (NPC.spriteDirection == 1)
		{
			effects = SpriteEffects.FlipHorizontally;
		}

		Vector2 drawPos = NPC.Center - Main.screenPosition;
		spriteBatch.Draw(tg, drawPos, NPC.frame, new Color(200, 200, 200, 0), NPC.rotation, NPC.frame.Size() * 0.5f, 1f, effects, 0f);
	}
}
