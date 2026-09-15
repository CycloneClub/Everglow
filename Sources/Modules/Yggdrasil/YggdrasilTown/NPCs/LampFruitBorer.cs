using Everglow.Commons.Coroutines;
using Everglow.Commons.Mechanics.Miscs;
using Everglow.Commons.Templates.Enemies;
using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Everglow.Yggdrasil.YggdrasilTown.Items.PermanentBoosters;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs;

[NoGameModeScale]
public class LampFruitBorer : Caterpillar
{
	public override void SetStaticDefaults()
	{
		NPCSpawnManager.RegisterNPC(Type);
	}

	public override void SetDefaults()
	{
		base.SetDefaults();

		SegmentBehavioralSize = 10;
		SegmentHitBoxSize = 14;
		SegmentCount = 10;
		AnimationSpeed = 2;
		NPC.knockBackResist = -0.12f;
		NPC.lifeMax = 33;
		NPC.damage = 2;
		NPC.value = 0;
		if (Main.expertMode)
		{
			NPC.knockBackResist = -0.08f;
			NPC.lifeMax = 55;
			NPC.damage = 2;
		}
		if (Main.masterMode)
		{
			NPC.knockBackResist = -0.04f;
			NPC.lifeMax = 66;
			NPC.damage = 3;
		}
		DustType = -1;
	}

	public override Rectangle GetDrawFrame(int Style)
	{
		Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
		int height = texture.Height;
		if (Style == 0)
		{
			return new Rectangle(0, 0, 20, height);
		}
		if (Style == 1)
		{
			return new Rectangle(22, 0, 10, height);
		}
		if (Style == 2)
		{
			return new Rectangle(44, 0, 22, height);
		}
		return base.GetDrawFrame(Style);
	}

	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		YggdrasilTownBiome YggdrasilTownBiome = ModContent.GetInstance<YggdrasilTownBiome>();
		if (!YggdrasilTownBiome.IsBiomeActive(Main.LocalPlayer))
		{
			return 0f;
		}

		return 8f;
	}

	public override IEnumerator<ICoroutineInstruction> CrawlingII()
	{
		Crawl_2 = true;
		float tValue = 0;
		Vector2 toTail = GetNormalOfTiles(Segments[0].SelfPosition + NPC.Center).RotatedBy(-MathHelper.PiOver4 * Segments[0].SelfDirection);
		if (toTail == Vector2.zeroVector)
		{
			toTail = Vector2.Normalize(Segments[0].SelfPosition - Segments[Segments.Count - 1].SelfPosition);
		}

		// 从头部开始拉开虫体
		for (int t = 0; t < 60; t++)
		{
			// 被打破防了,掉下去,终止此协程
			if (Toughness <= 0)
			{
				Crawl_2 = false;
				yield break;
			}
			AnyAliveCoroutineTimer = 0; // 在所有可能活着的协程的执行里面都归零
			Segment head = Segments[0]; // 头
			head.SelfPosition += toTail * 2f * (SegmentCount * SegmentBehavioralSize / 300f);
			toTail = toTail.RotatedBy(-0.029f / (SegmentCount / 10f) * MathF.Sin(t / 60f * MathF.PI * head.SelfDirection));
			head.Normal = toTail.RotatedBy(MathHelper.PiOver2);
			tValue = (float)Utils.Lerp(tValue, SegmentBehavioralSize * SegmentCount / 5f, 0.03f);
			Segments[0] = head;

			for (int i = 1; i < Segments.Count; i++)
			{
				Segment segment = Segments[i];
				if (GetNormalOfTiles(segment.SelfPosition + NPC.Center) == Vector2.zeroVector)
				{
					segment.SelfPosition += toTail.RotatedBy(-MathHelper.PiOver2 * segment.SelfDirection) * tValue / 30f;
				}
				Vector2 v = Segments[i - 1].SelfPosition - segment.SelfPosition;
				if (v.Length() > SegmentBehavioralSize)
				{
					segment.SelfPosition = Segments[i - 1].SelfPosition - Vector2.Normalize(v) * (SegmentBehavioralSize - 2);
				}
				Vector2 direction = segment.SelfPosition - Segments[i - 1].SelfPosition;
				if (i != Segments.Count - 1)
				{
					direction = Segments[i + 1].SelfPosition - Segments[i - 1].SelfPosition;
				}
				direction = direction.RotatedBy(-MathHelper.PiOver2);
				segment.Normal = Vector2.Normalize(direction);
				Segments[i] = segment;
			}
			if (t > 10 && GetNormalOfTiles(Segments[0].SelfPosition + NPC.Center) != Vector2.zeroVector)
			{
				break;
			}
			yield return new SkipThisFrame();
		}

		// 头部寻找落点
		while (GetNormalOfTiles(Segments[0].SelfPosition + NPC.Center) == Vector2.zeroVector)
		{
			// 被打破防了,掉下去,终止此协程
			if (Toughness <= 0)
			{
				Crawl_2 = false;
				yield break;
			}
			AnyAliveCoroutineTimer = 0; // 在所有可能活着的协程的执行里面都归零
			Segment head = Segments[0]; // 头
			toTail += toTail.RotatedBy(-MathHelper.PiOver2 * head.SelfDirection) * 0.02f;
			toTail = Vector2.Normalize(toTail);
			head.SelfPosition += toTail * 2 * (SegmentCount / 10f);

			head.Normal = Vector2.Lerp(head.Normal, Segments[1].Normal, 0.2f);
			tValue = (float)Utils.Lerp(tValue, 120, 0.03f);
			Segments[0] = head;
			bool shouldFall = true;
			for (int i = 1; i < Segments.Count; i++)
			{
				Segment segment = Segments[i];
				Vector2 getNormal = GetNormalOfTiles(segment.SelfPosition + NPC.Center);
				if (getNormal == Vector2.zeroVector)
				{
					segment.SelfPosition += toTail.RotatedBy(-MathHelper.PiOver2 * segment.SelfDirection) * 2;
				}
				else
				{
					shouldFall = false;
				}
				Vector2 v = Segments[i - 1].SelfPosition - segment.SelfPosition;
				if (v.Length() > SegmentBehavioralSize)
				{
					segment.SelfPosition = Segments[i - 1].SelfPosition - Vector2.Normalize(v) * (SegmentBehavioralSize - 2);
				}
				Vector2 direction = segment.SelfPosition - Segments[i - 1].SelfPosition;

				direction = direction.RotatedBy(-MathHelper.PiOver2);
				segment.Normal = Vector2.Normalize(direction);
				Segments[i] = segment;
			}

			// 如果整条虫都被头带上天了,在重力作用下滑落
			if (shouldFall)
			{
				Crawl_2 = false;
				_caterpillarCoroutine.StartCoroutine(new Coroutine(Falling_HitByDamege()));
				yield break;
			}
			yield return new SkipThisFrame();
		}
		AdjustPosition();
		Crawl_2 = false;

		// 平坦程度
		float LineValue = 0;
		for (int i = 1; i < Segments.Count; i++)
		{
			LineValue += (Segments[i].Normal - Segments[i - 1].Normal).Length();
		}

		// 蠕虫只会在比较平坦的时候休息,1/10的几率休息5~50秒
		if (!Main.dayTime)
		{
			Vector2 direction = GetNormalOfTiles(Segments[Segments.Count / 2].SelfPosition + NPC.Center).RotatedBy(MathHelper.PiOver2 * Segments[Segments.Count / 2].SelfDirection);
			if (direction.X < -0.1f && NPC.Center.X < Main.screenPosition.X + Main.screenWidth / 2f - 400)
			{
				_caterpillarCoroutine.StartCoroutine(new Coroutine(CrawlingIII_Turn()));
				yield break;
			}
			if (direction.X > 0.1f && NPC.Center.X > Main.screenPosition.X + Main.screenWidth / 2f + 400)
			{
				_caterpillarCoroutine.StartCoroutine(new Coroutine(CrawlingIII_Turn()));
				yield break;
			}
		}
		if (Main.rand.NextBool(10) && LineValue < 1)
		{
			_caterpillarCoroutine.StartCoroutine(new Coroutine(Waiting(Main.rand.Next(300, 3000))));
			yield break;
		}
		if (!Crawl_1)
		{
			_caterpillarCoroutine.StartCoroutine(new Coroutine(Crawling()));
		}
	}

	public override bool PreKill()
	{
		for (int j = 0; j < Segments.Count; j++)
		{
			Vector2 v0 = new Vector2(0, Main.rand.NextFloat(0, 6f)).RotatedByRandom(MathHelper.TwoPi);
			int type = ModContent.Find<ModGore>("Everglow/LampFruitBorer_gore1").Type;
			if (j == 0)
			{
				type = ModContent.Find<ModGore>("Everglow/LampFruitBorer_gore0").Type;
			}
			if (j == Segments.Count - 1)
			{
				type = ModContent.Find<ModGore>("Everglow/LampFruitBorer_gore2").Type;
			}
			Gore.NewGore(NPC.GetSource_Death(), NPC.Center + Segments[j].SelfPosition, v0, type, NPC.scale);
		}
		return base.PreKill();
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		base.PreDraw(spriteBatch, screenPos, drawColor);
		Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
		Color glowColor = new Color(0.5f, 0.5f, 0.5f, 0f);
		for (int i = 1; i < Segments.Count; i++)
		{
			Vector2 drawPos = Segments[i].SelfPosition + NPC.Center - Main.screenPosition;
			Lighting.AddLight(Segments[i].SelfPosition + NPC.Center, new Vector3(0.6f, 0.4f, 0f));
			Rectangle drawRectangle = GetDrawFrame(Segments[i].Style);
			SpriteEffects spriteEffect = SpriteEffects.None;
			if (Segments[i].SelfDirection == -1)
			{
				spriteEffect = SpriteEffects.FlipVertically;
			}
			Vector2 origin = drawRectangle.Size() / 2f;
			spriteBatch.Draw(texture, drawPos, drawRectangle, glowColor, Segments[i].Normal.ToRotation() + MathHelper.PiOver2, origin, 1f, spriteEffect, 0);
			if (HitTimer > 0)
			{
				Color hitColor = new Color(glowColor.R / 255f, 0, 0, HitTimer / 30f);
				spriteBatch.Draw(texture, drawPos, drawRectangle, hitColor, Segments[i].Normal.ToRotation() + MathHelper.PiOver2, origin, 1f, spriteEffect, 0);
			}
		}
		for (int i = 0; i < 1; i++)
		{
			if (Segments.Count > 0)
			{
				Vector2 drawPos = Segments[i].SelfPosition + NPC.Center - Main.screenPosition;
				Rectangle drawRectangle = GetDrawFrame(Segments[i].Style);
				SpriteEffects spriteEffect = SpriteEffects.None;
				if (Segments[i].SelfDirection == -1)
				{
					spriteEffect = SpriteEffects.FlipVertically;
				}
				Vector2 origin = new Vector2(drawRectangle.Width, drawRectangle.Height / 2f);
				Vector2 offset = Segments[i].Normal.RotatedBy(MathHelper.PiOver2) * drawRectangle.Width / 3f;
				spriteBatch.Draw(texture, drawPos + offset, drawRectangle, glowColor, Segments[i].Normal.ToRotation() + MathHelper.PiOver2, origin, 1f, spriteEffect, 0);
				if (HitTimer > 0)
				{
					Color hitColor = new Color(glowColor.R / 255f, 0, 0, HitTimer / 30f);
					spriteBatch.Draw(texture, drawPos + offset, drawRectangle, hitColor, Segments[i].Normal.ToRotation() + MathHelper.PiOver2, origin, 1f, spriteEffect, 0);
				}
			}
		}
		return false;
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LampBorerHoney>(), 1, 1, 1));
	}
}
