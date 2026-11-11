using Everglow.Commons.Coroutines;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs;
using SubworldLibrary;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

[AutoloadHead]
public class InnKeeper : TownNPC_LiveInYggdrasil
{
	public bool Attacking0 = false;

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 10;
	}

	public override void SetDefaults()
	{
		base.SetDefaults();
		StandFrame = new Rectangle(0, 0, 48, 56);
		SitFrame = new Rectangle(0, 448, 48, 56);
		NPC.frame = StandFrame;
		FrameHeight = 56;
		Attacking0 = false;
	}

	public override void TryAttack()
	{
		float minDis = 100;
		float minDis2 = 400;
		if (ThreatenTarget >= 0 && ThreatenTarget < 200)
		{
			NPC npc = Main.npc[ThreatenTarget];
			Vector2 distance = npc.Center - NPC.Center;
			for (int i = 0; i < 8; i++)
			{
				Vector2 checkPos = npc.Center;
				switch (i)
				{
					case 0:
						checkPos = npc.TopLeft;
						break;
					case 1:
						checkPos = npc.Top;
						break;
					case 2:
						checkPos = npc.TopRight;
						break;
					case 3:
						checkPos = npc.Left;
						break;
					case 4:
						checkPos = npc.Right;
						break;
					case 5:
						checkPos = npc.BottomLeft;
						break;
					case 6:
						checkPos = npc.Bottom;
						break;
					case 7:
						checkPos = npc.BottomRight;
						break;
				}
				if ((checkPos - NPC.Center).Length() < distance.Length())
				{
					distance = checkPos - NPC.Center;
				}
			}
			if (Main.rand.NextFloat(TotalThreaten) > 0.3f)
			{
				if (distance.Length() < minDis2)
				{
					BehaviorsCoroutines.Enqueue(new Coroutine(Attack2()));
					return;
				}
			}
			if (distance.Length() < minDis)
			{
				BehaviorsCoroutines.Enqueue(new Coroutine(Attack()));
				return;
			}
			else if (distance.Length() < minDis2)
			{
				BehaviorsCoroutines.Enqueue(new Coroutine(Attack2()));
				return;
			}
		}
	}

	public override void WalkFrame()
	{
		NPC.frame.Width = 48;
		NPC.frame.Height = FrameHeight;
		NPC.frameCounter += Math.Abs(NPC.velocity.X);
		if (NPC.frameCounter > 4)
		{
			NPC.frame.Y += FrameHeight;
			NPC.frameCounter = 0;
		}
		if (NPC.frame.Y > 8 * FrameHeight)
		{
			NPC.frame.Y = FrameHeight;
		}
	}

	public override void CheckInSuitableArea()
	{
		if (SubworldSystem.Current is not YggdrasilWorld)
		{
			return;
		}
		AnchorForBehaviorPos = YggdrasilTownCentralSystem.TownTopLeftWorldCoord.ToTileCoordinates() + new Point(358, 156);
		bool safe = false;
		var homePoint = AnchorForBehaviorPos;
		NPC.homeless = false;
		NPC.homeTileX = homePoint.X;
		NPC.homeTileY = homePoint.Y;

		var npcTilePos = NPC.Center.ToTileCoordinates();
		if (npcTilePos.X > AnchorForBehaviorPos.X - 100 && npcTilePos.X < AnchorForBehaviorPos.X + 100)
		{
			if (npcTilePos.Y > AnchorForBehaviorPos.Y - 20 && npcTilePos.Y < AnchorForBehaviorPos.Y + 20)
			{
				safe = true;
			}
		}
		if (!safe)
		{
			TeleportHome();
		}
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		if (!Attacking0 && !KnockOut)
		{
			Texture2D texMain = ModAsset.InnKeeper.Value;
			Vector2 drawPos = NPC.Center - screenPos + new Vector2(0, NPC.height - NPC.frame.Height) * 0.5f;
			Main.spriteBatch.Draw(texMain, drawPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
		}
		else if (Attacking0 && !KnockOut)
		{
			Texture2D texMain = ModAsset.InnKeeper_Attack.Value;
			Vector2 drawPos = NPC.Center - screenPos + new Vector2(0, -8);
			Main.spriteBatch.Draw(texMain, drawPos, null, drawColor, NPC.rotation, texMain.Size() * 0.5f, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
		}
		else
		{
			Texture2D texMain = ModAsset.Innkeeper_KnockOut.Value;
			Vector2 drawPos = NPC.Center - screenPos + new Vector2(0, -8);
			Main.spriteBatch.Draw(texMain, drawPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
		}
		return false;
	}

	public IEnumerator<ICoroutineInstruction> Attack()
	{
		bool safe = false;
		if (ThreatenTarget is >= 0 and < 200)
		{
			safe = true;
		}
		if (!safe)
		{
			EndAIPiece();
			yield break;
		}
		NPC target = Main.npc[ThreatenTarget];
		if (target.Center.X > NPC.Center.X)
		{
			NPC.direction = 1;
		}
		if (target.Center.X < NPC.Center.X)
		{
			NPC.direction = -1;
		}
		NPC.spriteDirection = NPC.direction;
		Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(-NPC.direction, 0), ModContent.ProjectileType<Georg_Hammer>(), 150, 4, Main.netMode == NetmodeID.SinglePlayer ? Main.myPlayer : Main.maxPlayers, NPC.whoAmI).npcProj = true;
		for (int t = 0; t < 60; t++)
		{
			NPC.velocity.X *= 0;
			Attacking0 = true;
			yield return new SkipThisFrame();
		}
		Attacking0 = false;
		EndAIPiece();
	}

	public IEnumerator<ICoroutineInstruction> Attack2()
	{
		bool safe = false;
		if (ThreatenTarget is >= 0 and < 200)
		{
			safe = true;
		}
		if (!safe)
		{
			EndAIPiece();
			yield break;
		}
		NPC target = Main.npc[ThreatenTarget];
		if (target.Center.X > NPC.Center.X)
		{
			NPC.direction = 1;
		}
		if (target.Center.X < NPC.Center.X)
		{
			NPC.direction = -1;
		}
		NPC.spriteDirection = NPC.direction;
		Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, (target.Center - NPC.Center).NormalizeSafe(), ModContent.ProjectileType<Georg_Hook>(), 100, 4, Main.netMode == NetmodeID.SinglePlayer ? Main.myPlayer : Main.maxPlayers, NPC.whoAmI).npcProj = true;
		for (int t = 0; t < 100; t++)
		{
			NPC.velocity.X *= 0;
			Attacking0 = true;
			yield return new SkipThisFrame();
		}
		Attacking0 = false;
		EndAIPiece();
	}

	public override void InitializeBossTags()
	{
		base.InitializeBossTags();
		var bossTags = new List<BossTag>
		{
			new BossTag("BurningHammer", 50, "Hammer become flaming hammer.") { IconType = 2 },
			new BossTag("ThunderHook", 50, "Hook attack with thunderbolt.") { IconType = 2 },
		};
		MyBossTags.AddRange(bossTags);
	}

	public bool BurningHammer = false;

	public bool ThunderHook = false;

	public override void ApplyBossTags()
	{
		base.ApplyBossTags();
		BurningHammer = false;
		ThunderHook = false;
		foreach (var tag in MyBossTags)
		{
			if (tag.Name == "ThunderHook" && tag.Enable)
			{
				ThunderHook = true;
			}
			if (tag.Name == "BurningHammer" && tag.Enable)
			{
				BurningHammer = true;
			}
		}
	}

	public override void BossAI()
	{
		base.BossAI();
		if (Fail)
		{
			NPC.velocity *= 0;
			NPC.dontTakeDamage = true;
			NPC.damage = 0;
			return;
		}
		if (KnockOut)
		{
			return;
		}
		bool safe = false;
		if (NPC.target >= 0)
		{
			safe = true;
		}
		if (!safe)
		{
			return;
		}
		Player player = Main.player[NPC.target];
		if (BehaviorsCoroutines.Count <= 0)
		{
			Idle = true;

			if (!BossToPlayerDistanceLowerThan(200))
			{
				if (player.Center.Y < NPC.Center.Y - 200 || player.Center.Y > NPC.Center.Y + 200)
				{
					BehaviorsCoroutines.Enqueue(new Coroutine(BossAttack_JumpAndHit()));
				}
				else
				{
					if (Main.rand.NextBool(2))
					{
						if (Main.rand.NextBool(2))
						{
							BehaviorsCoroutines.Enqueue(new Coroutine(BossAttack_FlyHammer()));
						}
						else
						{
							BehaviorsCoroutines.Enqueue(new Coroutine(BossAttack_JumpAndHit()));
						}
					}
					else
					{
						BehaviorsCoroutines.Enqueue(new Coroutine(BossWalk(60)));
					}
				}
			}
			else
			{
				BehaviorsCoroutines.Enqueue(new Coroutine(BossAttack_Hook()));
			}
		}
	}

	public IEnumerator<ICoroutineInstruction> BossAttack_Hook()
	{
		bool safe = false;
		if (NPC.target >= 0)
		{
			safe = true;
		}
		if (!safe)
		{
			EndAIPiece();
			yield break;
		}
		Player target = Main.player[NPC.target];
		if (target.Center.X > NPC.Center.X)
		{
			NPC.direction = 1;
		}
		if (target.Center.X < NPC.Center.X)
		{
			NPC.direction = -1;
		}
		NPC.spriteDirection = NPC.direction;
		Projectile p0 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, (target.Center - NPC.Center).NormalizeSafe(), ModContent.ProjectileType<Georg_Hook>(), (int)(NPC.damage * 1.5f), 4, Main.maxPlayers, NPC.whoAmI);
		p0.friendly = false;
		p0.hostile = true;
		for (int t = 0; t < 100 / AttackSpeed; t++)
		{
			NPC.velocity.X *= 0;
			Attacking0 = true;
			if (Resilience <= 0)
			{
				break;
			}
			yield return new SkipThisFrame();
		}
		Attacking0 = false;
		EndAIPiece();
	}

	public IEnumerator<ICoroutineInstruction> BossAttack_JumpAndHit()
	{
		NPC.noGravity = true;
		bool safe = false;
		if (NPC.target >= 0)
		{
			safe = true;
		}
		if (!safe)
		{
			EndAIPiece();
			yield break;
		}
		Player target = Main.player[NPC.target];
		var targetPos = target.Bottom;
		if (targetPos.X > NPC.Center.X)
		{
			NPC.direction = 1;
		}
		if (targetPos.X < NPC.Center.X)
		{
			NPC.direction = -1;
		}
		int fire = 0;
		if (BurningHammer)
		{
			fire = 1;
		}
		Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), targetPos, Vector2.zeroVector, ModContent.ProjectileType<Georg_Hammer_Target>(), 0, 0, Main.maxPlayers, fire);
		NPC.velocity *= 0;
		yield return new WaitForFrames(12);
		NPC.spriteDirection = NPC.direction;
		NPC.velocity.Y = -20 * AttackSpeed;

		// Ascend
		Projectile p0 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, (targetPos - NPC.Center).NormalizeSafe(), ModContent.ProjectileType<Georg_Hammer_JumpHit>(), NPC.damage, 4, Main.maxPlayers, NPC.whoAmI);
		float maxTimeAttack = (int)(30f / AttackSpeed);
		for (int t = 0; t < maxTimeAttack; t++)
		{
			float velX = (targetPos - NPC.Center).X / maxTimeAttack;
			NPC.velocity.X = velX;
			NPC.velocity.Y += 2 / 3f * AttackSpeed * AttackSpeed;
			if (Resilience <= 0)
			{
				break;
			}
			yield return new SkipThisFrame();
		}
		var startPos = NPC.Center;

		for (int i = 0; i < 500; i++)
		{
			if (!Collision.SolidCollision(targetPos + new Vector2(-10), 20, 20) && !TileUtils.PlatformCollision(targetPos))
			{
				targetPos.Y += 2;
			}
			else
			{
				break;
			}
		}

		// Descend
		float maxTime = (int)(6f / AttackSpeed);
		for (int t = 0; t < maxTime; t++)
		{
			float value = (MathF.Sin((t / maxTime - 0.5f) * MathHelper.Pi) + 1) * 0.5f;
			NPC.Center = Vector2.Lerp(startPos, targetPos, value);
			if (Resilience <= 0)
			{
				break;
			}
			yield return new SkipThisFrame();
		}
		NPC.Center = targetPos;
		for (int i = 0; i < 100; i++)
		{
			if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height) || TileUtils.PlatformCollision(NPC.position, NPC.width, NPC.height))
			{
				NPC.position.Y -= 2;
			}
			else
			{
				break;
			}
		}
		for (int t = 0; t < 30; t++)
		{
			NPC.velocity *= 0;
			yield return new SkipThisFrame();
		}
		NPC.noGravity = false;
		EndAIPiece();
	}

	public IEnumerator<ICoroutineInstruction> BossAttack_FlyHammer()
	{
		for (int t = 0; t <= 120; t++)
		{
			bool safe = false;
			if (NPC.target >= 0)
			{
				safe = true;
			}
			if (!safe)
			{
				EndAIPiece();
				yield break;
			}
			Player target = Main.player[NPC.target];
			if (target.Center.X > NPC.Center.X)
			{
				NPC.direction = 1;
			}
			if (target.Center.X < NPC.Center.X)
			{
				NPC.direction = -1;
			}
			NPC.spriteDirection = NPC.direction;
			float speed = 1.8f * BossMovingSpeed;
			WalkFrame();
			NPC.velocity.X = NPC.direction * speed;
			if (!Collision.SolidCollision(NPC.Bottom - new Vector2(20, 0), 40, 20))
			{
				NPC.velocity.Y += 0.5f;
			}
			if (t % (20 / AttackSpeed) == 0)
			{
				Projectile p0 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, (target.Center - NPC.Center).NormalizeSafe(), ModContent.ProjectileType<Georg_Fly_Hammer>(), NPC.damage, 4, Main.maxPlayers, NPC.whoAmI);
			}
			if (BossToPlayerDistanceLowerThan(100))
			{
				break;
			}
			if (Resilience <= 0)
			{
				break;
			}
			yield return new SkipThisFrame();
		}
		NPC.velocity.X *= 0;
		EndAIPiece();
	}

	public override void KnockOutFrame()
	{
		if (KnockOutTimer is >= 0 and < 5)
		{
			NPC.frame = new Rectangle(0, 0, 68, 60);
		}
		if (KnockOutTimer is >= 5 and < 10)
		{
			NPC.frame = new Rectangle(0, 60, 68, 60);
		}
		if (KnockOutTimer is >= 10 and < 15)
		{
			NPC.frame = new Rectangle(0, 120, 68, 60);
		}
		if (KnockOutTimer is >= 15)
		{
			NPC.frame = new Rectangle(0, 180, 68, 60);
		}
		base.KnockOutFrame();
	}

	public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		base.PostDraw(spriteBatch, screenPos, drawColor);
	}
}
