using Everglow.Myth.Common;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;

namespace Everglow.Myth.TheFirefly.NPCs;

// 蜈蚣设定：起始时蜈蚣贴在物块上爬行。受到攻击后转换为钻地（蠕虫形态）

internal class CentipedeHead : FireWormHead
{
	public override int BodyType => ModContent.NPCType<CentipedeBody>();
	public override int TailType => ModContent.NPCType<CentipedeTail>();
	public float wormSpeed = 1.0f;
	public int checkHitWidth = 24;
	public Vector2 OldSpeedDirection = new Vector2(1.0f, 0.0f);

	public override void SetStaticDefaults()
	{
		var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
		{
			CustomTexturePath = "Everglow/Myth/TheFirefly/NPCs/FireflyCentipede_Bestiary",
			Position = new Vector2(40f, 24f),
			PortraitPositionXOverride = 0f,
			PortraitPositionYOverride = 12f,
		};
		NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
	}

	public override void SetDefaults()
	{
		// Head is 10 defence, body 20, tail 30.
		NPC.netAlways = true;
		NPC.noGravity = true;
		NPC.noTileCollide = false;
		NPC.behindTiles = true;
		NPC.scale = 0.9f;
		NPC.dontCountMe = true;

		NPC.damage = 26;
		NPC.width = 22;
		NPC.height = 22;
		NPC.defense = 36;
		NPC.lifeMax = 1200;
		NPC.knockBackResist = 0f;
		NPC.value = 300f;
		NPC.aiStyle = -1;
	}

	public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
	{
		// 我们可以使用AddRange，而不是多次调用Add，以便一次添加多个项目
		bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
			BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,
			BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

			new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Everglow.Bestiary.Centipede.Flavor")),
		});
	}
	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		FireflyBiome fireflyBiome = ModContent.GetInstance<FireflyBiome>();
		if (!fireflyBiome.IsBiomeActive(Main.LocalPlayer))
		{
			return 0f;
		}

		if (NPC.CountNPCS(ModContent.NPCType<Bosses.CorruptMoth>()) > 0)
			return 0;
		else if (NPC.CountNPCS(ModContent.NPCType<CentipedeHead>()) > 1)
		{
			return 0f;
		}
		else if (NPC.CountNPCS(ModContent.NPCType<CentipedeHead>()) > 0)
		{
			return 0.04f;
		}
		return 0.08f;
	}
	public override void Init()
	{
		MinSegmentLength = 24;
		MaxSegmentLength = 32;
		CommonWormInit(this);
	}

	internal static void CommonWormInit(FireWorm worm)
	{
		// 这两个属性处理蠕虫的运动
		worm.MoveSpeed = 10.5f;
		worm.Acceleration = 0.03f;
	}

	private int attackCounter;

	public override void SendExtraAI(BinaryWriter writer)
	{
		writer.Write(attackCounter);
	}

	public override void ReceiveExtraAI(BinaryReader reader)
	{
		attackCounter = reader.ReadInt32();
	}

	public override void AI()
	{
		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			Player target = Main.player[NPC.target];
			bool collision = HeadAI_CheckCollisionForDustSpawns();
			// 测量与目标的距离
			HeadAI_CheckTargetDistance(ref collision);
			HeadAI_Movement(collision);

			NPC.noGravity = true;
			NPC.noTileCollide = true;

			NPC.netUpdate = true;
		}
	}
	public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		Texture2D tex = ModAsset.CentipedeHead_Glow.Value;
		spriteBatch.Draw(tex, NPC.Center - Main.screenPosition + new Vector2(0, -28), null, new Color(255, 255, 255, 0), NPC.rotation, tex.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
	}
	private bool HeadAI_CheckCollisionForDustSpawns()
	{
		int minTilePosX = (int)(NPC.Left.X / 16) - 1;
		int maxTilePosX = (int)(NPC.Right.X / 16) + 2;
		int minTilePosY = (int)(NPC.Top.Y / 16) - 1;
		int maxTilePosY = (int)(NPC.Bottom.Y / 16) + 2;

		minTilePosX = Math.Clamp(minTilePosX, 0, Main.maxTilesX);
		minTilePosY = Math.Clamp(minTilePosY, 0, Main.maxTilesY);
		maxTilePosX = Math.Clamp(maxTilePosX, 0, Main.maxTilesX);
		maxTilePosY = Math.Clamp(maxTilePosY, 0, Main.maxTilesY);

		bool collision = false;

		// 这是对物体碰撞的初步检查。
		for (int i = minTilePosX; i < maxTilePosX; ++i)
		{
			for (int j = minTilePosY; j < maxTilePosY; ++j)
			{
				Tile tile = Main.tile[i, j];

				// 如果物体是实心的或被认为是一个平台，那么就有有效的碰撞。
				if (tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType] && tile.TileFrameY == 0))
				{
					Vector2 tileWorld = new Point16(i, j).ToWorldCoordinates(0, 0);

					if (NPC.Right.X > tileWorld.X && NPC.Left.X < tileWorld.X + 16 && NPC.Bottom.Y > tileWorld.Y && NPC.Top.Y < tileWorld.Y + 16)
					{
						// 发现碰撞
						collision = true;

						if (Main.rand.NextBool(100))
						{
							WorldGen.KillTile(i, j, fail: true, effectOnly: true, noItem: false);
						}
					}
				}
			}
		}

		return collision;
	}

	private void HeadAI_CheckTargetDistance(ref bool collision)
	{
		// 如果没有与tiles发生碰撞，我们会检查这个NPC和它的目标之间的距离是否过大，这样我们仍然可以触发 "碰撞"。
		if (!collision)
		{
			Rectangle hitbox = NPC.Hitbox;

			int maxDistance = MaxDistanceForUsingTileCollision;

			bool tooFar = true;

			for (int i = 0; i < Main.maxPlayers; i++)
			{
				Rectangle areaCheck;

				Player player = Main.player[i];

				if (ForcedTargetPosition is Vector2 target)
					areaCheck = new Rectangle((int)target.X - maxDistance, (int)target.Y - maxDistance, maxDistance * 2, maxDistance * 2);
				else if (player.active && !player.dead && !player.ghost)
				{
					areaCheck = new Rectangle((int)player.position.X - maxDistance, (int)player.position.Y - maxDistance, maxDistance * 2, maxDistance * 2);
				}
				else
				{
					continue;  // Not a valid player
				}

				if (hitbox.Intersects(areaCheck))
				{
					tooFar = false;
					break;
				}
			}

			if (tooFar)
			{
				collision = true;
			}
		}
	}

	private void HeadAI_Movement(bool collision)
	{
		// 移动速度决定了这个NPC可以移动的最大速度。
		float speed = MoveSpeed;
		// 这个NPC加速的速度。
		float acceleration = Acceleration;

		float targetXPos, targetYPos;

		Player playerTarget = Main.player[NPC.target];

		Vector2 forcedTarget = ForcedTargetPosition ?? playerTarget.Center;
		// 像这样使用一个ValueTuple，可以方便地分配多个值
		(targetXPos, targetYPos) = (forcedTarget.X, forcedTarget.Y);

		// Copy the value, since it will be clobbered later
		Vector2 npcCenter = NPC.Center;

		float targetRoundedPosX = (int)(targetXPos / 16f) * 16;
		float targetRoundedPosY = (int)(targetYPos / 16f) * 16;
		npcCenter.X = (int)(npcCenter.X / 16f) * 16;
		npcCenter.Y = (int)(npcCenter.Y / 16f) * 16;
		// dirX和dirY分别是当前蠕虫到目标的x和y距离
		float dirX = targetRoundedPosX - npcCenter.X;
		float dirY = targetRoundedPosY - npcCenter.Y;
		// 当前蠕虫到目标的直线距离
		float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);

		// 如果我们没有任何类型的碰撞，我们希望NPC向下并沿X轴减速。
		if (!collision && !CanFly)
			HeadAI_Movement_HandleFallingFromNoCollision(dirX, speed * NPC.localAI[0], acceleration);
		else
		{
			// 否则，我们要播放一些音频（soundDelay）并向我们的目标移动。
			HeadAI_Movement_PlayDigSounds(length);
			// 设置NPC的速度
			HeadAI_Movement_HandleMovement(dirX, dirY, length, speed, acceleration);
		}
		// 设置NPC的转向
		HeadAI_Movement_SetRotation(collision);
	}

	private void HeadAI_Movement_HandleFallingFromNoCollision(float dirX, float speed, float acceleration)
	{
		NPC.TargetClosest(true);
		// Constant gravity of 0.11 pixels/tick
		NPC.velocity.Y += 0.11f;

		// Ensure that the NPC does not fall too quickly
		if (NPC.velocity.Y > speed + 12.5)
		{
			NPC.velocity.Y = speed + 12.5f;
		}

		// 以下行为模仿了香草虫的运动
		if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.4f)
		{
			// 速度足够快，但不能太快
			if (NPC.velocity.X < 0.0f)
				NPC.velocity.X -= acceleration * 1.1f;
			else
			{
				NPC.velocity.X += acceleration * 1.1f;
			}
		}
		else if (NPC.velocity.Y == speed)
		{
			// NPC has reached terminal velocity
			if (NPC.velocity.X < dirX)
				NPC.velocity.X += acceleration;
			else if (NPC.velocity.X > dirX)
			{
				NPC.velocity.X -= acceleration;
			}
		}
		else if (NPC.velocity.Y > 4)
		{
			if (NPC.velocity.X < 0)
				NPC.velocity.X += acceleration * 0.9f;
			else
			{
				NPC.velocity.X -= acceleration * 0.9f;
			}
		}
	}

	private void HeadAI_Movement_PlayDigSounds(float length)
	{
		if (NPC.soundDelay == 0)
		{
			// NPC离目标地点越近，播放声音越快
			float num1 = length / 40f;

			if (num1 < 10)
			{
				num1 = 10f;
			}

			if (num1 > 20)
			{
				num1 = 20f;
			}

			NPC.soundDelay = (int)num1;
			Tile tile = Main.tile[(int)(NPC.Center.X / 16f), (int)(NPC.Center.Y / 16f)];
			if (tile.LiquidAmount > 15)
			{
				SoundEngine.PlaySound(SoundID.Waterfall, NPC.position);
				return;
			}

			SoundEngine.PlaySound(SoundID.WormDig, NPC.position);
		}
	}

	private void HeadAI_Movement_HandleMovement(float dirX, float dirY, float length, float speed, float acceleration)
	{
		float absDirX = Math.Abs(dirX);
		float absDirY = Math.Abs(dirY);
		float newSpeed = speed / length;  // 距离越近，速度越快
		dirX *= newSpeed;
		dirY *= newSpeed;
		// 蠕虫速度方向和目标方向有相同方向的
		if (NPC.velocity.X > 0 && dirX > 0 || NPC.velocity.X < 0 && dirX < 0 || NPC.velocity.Y > 0 && dirY > 0 || NPC.velocity.Y < 0 && dirY < 0)
		{
			// 该NPC正在向目标地点移动
			if (NPC.velocity.X < dirX)
				NPC.velocity.X += acceleration;
			else if (NPC.velocity.X > dirX)
			{
				NPC.velocity.X -= acceleration;
			}

			if (NPC.velocity.Y < dirY)
				NPC.velocity.Y += acceleration;
			else if (NPC.velocity.Y > dirY)
			{
				NPC.velocity.Y -= acceleration;
			}

			// 预定的Y-速度很小，而且NPC正在向左移动，目标在NPC的右边，反之亦然。
			if (Math.Abs(dirY) < speed * 0.2 && (NPC.velocity.X > 0 && dirX < 0 || NPC.velocity.X < 0 && dirX > 0))
			{
				if (NPC.velocity.Y > 0)
					NPC.velocity.Y += acceleration * 2f;
				else
				{
					NPC.velocity.Y -= acceleration * 2f;
				}
			}
			// 预定的X-速度很小，而且NPC正在向上/向下移动，目标在NPC的下方/上方。
			if (Math.Abs(dirX) < speed * 0.2 && (NPC.velocity.Y > 0 && dirY < 0 || NPC.velocity.Y < 0 && dirY > 0))
			{
				if (NPC.velocity.X > 0)
					NPC.velocity.X = NPC.velocity.X + acceleration * 2f;
				else
				{
					NPC.velocity.X = NPC.velocity.X - acceleration * 2f;
				}
			}
		}
		else if (absDirX > absDirY)
		{
			// X距离比Y距离大。 迫使沿X轴的运动更强烈
			if (NPC.velocity.X < dirX)
				NPC.velocity.X += acceleration * 1.1f;
			else if (NPC.velocity.X > dirX)
			{
				NPC.velocity.X -= acceleration * 1.1f;
			}

			if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.5)
			{
				if (NPC.velocity.Y > 0)
					NPC.velocity.Y += acceleration;
				else
				{
					NPC.velocity.Y -= acceleration;
				}
			}
		}
		else
		{
			if (NPC.velocity.Y < dirY)
				NPC.velocity.Y += acceleration * 1.1f;
			else if (NPC.velocity.Y > dirY)
			{
				NPC.velocity.Y -= acceleration * 1.1f;
			}

			if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.5)
			{
				if (NPC.velocity.X > 0)
					NPC.velocity.X += acceleration;
				else
				{
					NPC.velocity.X -= acceleration;
				}
			}
		}
	}

	private void HeadAI_Movement_SetRotation(bool collision)
	{
		// Set the correct rotation for this NPC.
		// 假设NPC的贴图是朝上的。 你可能需要修改这一行，以适当考虑到你的NPC的方向。
		NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

		// 一些netupdate的东西（多人游戏兼容性）。
		if (collision)
		{
			if (NPC.localAI[0] != 1)  // 碰撞检测同步
			{
				NPC.netUpdate = true;
			}

			NPC.localAI[0] = 1f;
		}
		else
		{
			if (NPC.localAI[0] != 0)
			{
				NPC.netUpdate = true;
			}

			NPC.localAI[0] = 0f;
		}

		// 如果NPC的速度发生变化，并且没有被玩家 "击中"，则强制进行网络更新。
		if ((NPC.velocity.X > 0 && NPC.oldVelocity.X < 0 || NPC.velocity.X < 0 && NPC.oldVelocity.X > 0 || NPC.velocity.Y > 0 && NPC.oldVelocity.Y < 0 || NPC.velocity.Y < 0 && NPC.oldVelocity.Y > 0) && !NPC.justHit)
		{
			NPC.netUpdate = true;
		}
	}
	public override void OnKill()
	{
		Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, Main.rand.Next(40)).RotatedByRandom(6.283),
					   new Vector2(0, Main.rand.Next(8)).RotatedByRandom(6.283) + new Vector2(-2, 2), ModContent.Find<ModGore>("Everglow/FireflyCentipedeBodyHead0").Type);
		Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, Main.rand.Next(40)).RotatedByRandom(6.283),
		   new Vector2(0, Main.rand.Next(8)).RotatedByRandom(6.283) + new Vector2(-2, 2), ModContent.Find<ModGore>("Everglow/FireflyCentipedeBodyHead1").Type);
		Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, Main.rand.Next(40)).RotatedByRandom(6.283),
		   new Vector2(0, Main.rand.Next(8)).RotatedByRandom(6.283) + new Vector2(-2, 2), ModContent.Find<ModGore>("Everglow/FireflyCentipedeBodyHead2").Type);
		for (int f = 0; f < 16; f++)
		{
			Vector2 v0 = new Vector2(0, Main.rand.NextFloat(9f)).RotatedByRandom(6.283);
			Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<Dusts.NavyBlood>(), v0.X, v0.Y, 0, default, Main.rand.NextFloat(0.85f, 2.75f));
		}
	}
	public override void HitEffect(NPC.HitInfo hit)
	{
		for (int f = 0; f < 8; f++)
		{
			Vector2 v0 = new Vector2(0, Main.rand.NextFloat(9f)).RotatedByRandom(6.283);
			Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<Dusts.NavyBlood>(), v0.X, v0.Y, 0, default, Main.rand.NextFloat(0.85f, 1.75f));
		}
	}
	public override bool CheckActive()
	{
		Player player = Main.player[NPC.target];
		return (player.Center - NPC.Center).Length() > 3500;
	}
}

internal class CentipedeBody : FireWormBody
{
	public override void SetStaticDefaults()
	{
		var value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
		{
			// 将此NPC从Bestiary中隐藏起来，对于你只想要一个条目的多部分NPC很有用。
			Hide = true,
		};
		NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
	}

	public override void SetDefaults()
	{
		NPC.netAlways = true;
		NPC.noGravity = true;
		NPC.noTileCollide = true;
		NPC.behindTiles = true;
		NPC.scale = 0.9f;
		NPC.dontCountMe = true;

		NPC.damage = 26;
		NPC.width = 24;
		NPC.height = 24;
		NPC.defense = 30;
		NPC.lifeMax = 1200;
		NPC.knockBackResist = 0f;
		NPC.value = 300f;
		NPC.aiStyle = -1;
	}

	public override void Init()
	{
		CentipedeHead.CommonWormInit(this);
	}
	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		float AddRot = (float)(Math.Sin(Main.timeForVisualEffects * 0.2 + NPC.ai[2] * 0.7) * 0.3f);
		Texture2D tex = ModAsset.CentipedeBody.Value;
		int FrameType = (int)NPC.ai[2] % 2;
		if (FrameType == 1 && (int)NPC.ai[2] % 4 == 1)
		{
			FrameType = 2;
		}

		if (FrameType == 1)
		{
			tex = ModAsset.CentipedeBody1.Value;
		}

		if (FrameType == 2)
		{
			tex = ModAsset.CentipedeBody2.Value;
		}

		spriteBatch.Draw(tex, NPC.Center - Main.screenPosition + new Vector2(0, -28), null, Lighting.GetColor((int)(NPC.Center.X / 16), (int)(NPC.Center.Y / 16)), NPC.rotation + AddRot, tex.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
		return false;
	}
	public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{

		float AddRot = (float)(Math.Sin(Main.timeForVisualEffects * 0.2 + NPC.ai[2] * 0.7) * 0.3f);
		int FrameType = (int)NPC.ai[2] % 2;
		if (FrameType == 1 && (int)NPC.ai[2] % 4 == 1)
		{
			FrameType = 2;
		}

		if (FrameType == 1)
		{
			Texture2D tex = ModAsset.CentipedeBody1_Glow.Value;
			spriteBatch.Draw(tex, NPC.Center - Main.screenPosition + new Vector2(0, -28), null, new Color(255, 255, 255, 0), NPC.rotation + AddRot, tex.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
		}
		if (FrameType == 2)
		{
			Texture2D tex = ModAsset.CentipedeBody2_Glow.Value;
			spriteBatch.Draw(tex, NPC.Center - Main.screenPosition + new Vector2(0, -28), null, new Color(255, 255, 255, 0), NPC.rotation + AddRot, tex.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
		}
	}
	public override void AI()
	{
		if (NPC.life <= 0)
		{
			int FrameType = (int)NPC.ai[2] % 2;
			if (FrameType == 1 && (int)NPC.ai[2] % 4 == 1)
			{
				FrameType = 2;
			}

			if (FrameType == 0)
			{
				if (Main.rand.NextBool(2))
				{
					Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, Main.rand.Next(40)).RotatedByRandom(6.283),
				new Vector2(0, Main.rand.Next(8)).RotatedByRandom(6.283) + new Vector2(-2, 2), ModContent.Find<ModGore>("Everglow/FireflyCentipedeBodyGore0").Type);
				}
				else
				{
					Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, Main.rand.Next(40)).RotatedByRandom(6.283),
						new Vector2(0, Main.rand.Next(8)).RotatedByRandom(6.283) + new Vector2(-2, 2), ModContent.Find<ModGore>("Everglow/FireflyCentipedeBodyGore1").Type);
					Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, Main.rand.Next(40)).RotatedByRandom(6.283),
						new Vector2(0, Main.rand.Next(8)).RotatedByRandom(6.283) + new Vector2(-2, 2), ModContent.Find<ModGore>("Everglow/FireflyCentipedeBodyGore2").Type);
				}
			}
			else
			{
				Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, Main.rand.Next(40)).RotatedByRandom(6.283),
					new Vector2(0, Main.rand.Next(8)).RotatedByRandom(6.283) + new Vector2(-2, 2), ModContent.Find<ModGore>("Everglow/FireflyCentipedeBodyHead1").Type);
			}
			if (FrameType == 1)
			{
				Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, Main.rand.Next(40)).RotatedByRandom(6.283),
				new Vector2(0, Main.rand.Next(8)).RotatedByRandom(6.283) + new Vector2(-2, 2), ModContent.Find<ModGore>("Everglow/FireflyCentipedeBodyGore3").Type);
			}
			if (FrameType == 2)
			{
				Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, Main.rand.Next(40)).RotatedByRandom(6.283),
				new Vector2(0, Main.rand.Next(8)).RotatedByRandom(6.283) + new Vector2(-2, 2), ModContent.Find<ModGore>("Everglow/FireflyCentipedeBodyGore4").Type);
			}
			for (int f = 0; f < 16; f++)
			{
				Vector2 v0 = new Vector2(0, Main.rand.NextFloat(9f)).RotatedByRandom(6.283);
				var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<Dusts.NavyBlood>(), v0.X, v0.Y, 0, default, Main.rand.NextFloat(0.85f, 2.75f));
			}
		}
	}
	public override void HitEffect(NPC.HitInfo hit)
	{
		for (int f = 0; f < 8; f++)
		{
			Vector2 v0 = new Vector2(0, Main.rand.NextFloat(9f)).RotatedByRandom(6.283);
			Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<Dusts.NavyBlood>(), v0.X, v0.Y, 0, default, Main.rand.NextFloat(0.85f, 1.75f));
		}
	}
	public override bool CheckActive()
	{
		Player player = Main.player[NPC.target];
		return (player.Center - NPC.Center).Length() > 3500;
	}
}

internal class CentipedeTail : FireWormTail
{
	[Obsolete]
	public override void SetStaticDefaults()
	{
		var value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
		{
			Hide = true,
		};
		NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
	}

	public override void SetDefaults()
	{
		NPC.netAlways = true;
		NPC.noGravity = true;
		NPC.noTileCollide = true;
		NPC.behindTiles = true;
		NPC.scale = 0.9f;
		NPC.dontCountMe = true;

		NPC.damage = 26;
		NPC.width = 24;
		NPC.height = 24;
		NPC.defense = 24;
		NPC.lifeMax = 1200;
		NPC.knockBackResist = 0f;
		NPC.value = 300f;
		NPC.aiStyle = -1;
	}

	public override void Init()
	{
		CentipedeHead.CommonWormInit(this);
	}
	public override void HitEffect(NPC.HitInfo hit)
	{
		for (int f = 0; f < 8; f++)
		{
			Vector2 v0 = new Vector2(0, Main.rand.NextFloat(9f)).RotatedByRandom(6.283);
			Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<Dusts.NavyBlood>(), v0.X, v0.Y, 0, default, Main.rand.NextFloat(0.85f, 1.75f));
		}
	}
	public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		Texture2D tex = ModAsset.CentipedeTail_Glow.Value;
		spriteBatch.Draw(tex, NPC.Center - Main.screenPosition + new Vector2(0, -28), null, new Color(255, 255, 255, 0), NPC.rotation, tex.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
	}

	public override void AI()
	{
		if (NPC.life <= 0)
		{
			Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, Main.rand.Next(40)).RotatedByRandom(6.283),
					   new Vector2(0, Main.rand.Next(8)).RotatedByRandom(6.283) + new Vector2(-2, 2), ModContent.Find<ModGore>("Everglow/FireflyCentipedeBodyTail").Type);
			Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, Main.rand.Next(40)).RotatedByRandom(6.283),
				 new Vector2(0, Main.rand.Next(8)).RotatedByRandom(6.283) + new Vector2(-2, 2), ModContent.Find<ModGore>("Everglow/FireflyCentipedeBodyHead1").Type);
			for (int f = 0; f < 16; f++)
			{
				Vector2 v0 = new Vector2(0, Main.rand.NextFloat(9f)).RotatedByRandom(6.283);
				var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<Dusts.NavyBlood>(), v0.X, v0.Y, 0, default, Main.rand.NextFloat(0.85f, 2.75f));

				// Dust d2 = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<Dusts.BlueParticleDark2StoppedByTile>(), v0.X, v0.Y, 0, default, Main.rand.NextFloat(1.65f, 3.75f));
				// d2.alpha = (int)(d2.scale * 50);
				// d2.rotation = Main.rand.NextFloat(0, 6.283f);
			}
		}
	}
	public override bool CheckActive()
	{
		Player player = Main.player[NPC.target];
		return (player.Center - NPC.Center).Length() > 3500;
	}
}
