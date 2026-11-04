using Everglow.Commons.Coroutines;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

[AutoloadHead]
public class CanteenMaid : TownNPC_LiveInYggdrasil
{
	public int AttackTimer = -1;

	public bool CanDespawn = false;

	public Vector2 AttackVelocity = Vector2.zeroVector;

	public override string HeadTexture => ModAsset.CanteenMaid_Head_Mod;

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 13;
	}

	public override void SetDefaults()
	{
		base.SetDefaults();
		StandFrame = new Rectangle(0, 0, 38, 52);
		SitFrame = new Rectangle(0, 676, 38, 52);
		NPC.frame = StandFrame;
		FrameHeight = 52;
	}

	public override void TryAttack()
	{
		int attackIndex = Main.rand.Next(4);
		for (int i = -1; i < attackIndex; i++)
		{
			if (CanAttack(attackIndex))
			{
				BehaviorsCoroutines.Enqueue(new Coroutine(Attack(attackIndex)));
				return;
			}
		}
	}

	public override void WalkFrame()
	{
		NPC.frame.Height = FrameHeight;
		NPC.frameCounter += Math.Abs(NPC.velocity.X);
		if (NPC.frameCounter > 4)
		{
			NPC.frame.Y += FrameHeight;
			NPC.frameCounter = 0;
		}
		if (NPC.frame.Y > 11 * FrameHeight)
		{
			NPC.frame.Y = FrameHeight;
		}
	}

	public override void CheckWalkBound()
	{
		if (!YggdrasilTownCentralSystem.InCanteen_YggdrasilTown())
		{
			return;
		}
		var npcTilePos = NPC.Center.ToTileCoordinates();
		if (npcTilePos.X < AnchorForBehaviorPos.X - 90)
		{
			NPC.direction = 1;
		}
		if (npcTilePos.X > AnchorForBehaviorPos.X + 90)
		{
			NPC.direction = -1;
		}
	}

	public bool CanAttack(int type)
	{
		AttackVelocity = Vector2.zeroVector;
		int size = 10;
		switch (type)
		{
			case 0:
				size = 28;
				break;
			case 1:
				size = 18;
				break;
			case 2:
				size = 16;
				break;
			case 3:
				size = 14;
				break;
			default:
				break;
		}
		if (ThreatenTarget is >= 0 and < 200)
		{
			AttackVelocity = GetShootVelocity(Main.npc[ThreatenTarget], new Vector2(size));
			if (AttackVelocity != Vector2.zeroVector)
			{
				return true;
			}
		}
		return false;
	}

	public Vector2 GetShootVelocity(NPC target, Vector2 projSize)
	{
		Vector2 bestVel = Vector2.zeroVector;
		float minDis = 1000;
		for (int r = -5; r < 12; r++)
		{
			int dir = 1;
			if (target.Center.X < NPC.Center.X)
			{
				dir = -1;
			}
			Vector2 shootPoint = NPC.Center + new Vector2(-8 * dir, -16);
			Vector2 distance = target.Center - shootPoint;
			float speed = distance.Length() / 16f;
			speed = Math.Clamp(speed, 6, 30);
			Vector2 vel = distance.NormalizeSafe() * speed;
			if (vel.X < 0)
			{
				vel = vel.RotatedBy(r / 6f);
			}
			else
			{
				vel = vel.RotatedBy(-r / 6f);
			}
			int maxStep = 40;
			var checkPos = shootPoint;
			var checkVel = vel;
			for (int t = 0; t < maxStep; t++)
			{
				switch (projSize.X)
				{
					// Watermelon
					case 28:
						if (checkVel.Y <= 21)
						{
							checkVel.Y += 0.6f;
						}
						checkVel *= 0.99f;
						break;

					// Plate
					case 18:
						if (checkVel.Y <= 14)
						{
							checkVel.Y += 0.4f;
						}
						checkVel *= 0.99f;
						break;

					// Fork
					case 16:
						if (checkVel.Y <= 14)
						{
							checkVel.Y += 0.3f;
						}
						checkVel *= 0.99f;
						break;

					// Apple
					case 14:
						if (checkVel.Y <= 12)
						{
							checkVel.Y += 0.6f;
						}
						checkVel *= 0.99f;
						break;
				}
				checkPos += checkVel;
				if (Rectangle.Intersect(target.Hitbox, new Rectangle((int)checkPos.X, (int)checkPos.Y, (int)projSize.X, (int)projSize.Y)) != Rectangle.emptyRectangle)
				{
					if ((checkPos - target.Center).Length() < minDis)
					{
						minDis = (checkPos - target.Center).Length();
						bestVel = vel;
					}
				}
				if (Collision.SolidCollision(checkPos, (int)projSize.X, (int)projSize.Y))
				{
					break;
				}
			}
		}
		return bestVel;
	}

	public IEnumerator<ICoroutineInstruction> Attack(int type)
	{
		bool safe = false;
		if (ThreatenTarget is >= 0 and < 200 && AttackVelocity != Vector2.zeroVector)
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
		for (int t = 0; t < 10; t++)
		{
			AttackTimer = t;
			NPC.frame = new Rectangle(0, 0, 38, FrameHeight);
			NPC.velocity.X *= 0;
			Vector2 shootPos = NPC.Center + new Vector2(-8 * NPC.direction, -16);
			if (t == 4)
			{
				switch (type)
				{
					case 0:
						Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), shootPos, AttackVelocity, ModContent.ProjectileType<Betty_Watermelon>(), 15, 4f, owner: Main.netMode == NetmodeID.SinglePlayer ? Main.myPlayer : Main.maxPlayers).npcProj = true;
						break;
					case 1:
						Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), shootPos, AttackVelocity, ModContent.ProjectileType<Betty_Plate>(), 6, 1f, owner: Main.netMode == NetmodeID.SinglePlayer ? Main.myPlayer : Main.maxPlayers).npcProj = true;
						break;
					case 2:
						Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), shootPos, AttackVelocity, ModContent.ProjectileType<Betty_Fork>(), 42, 0.05f, owner: Main.netMode == NetmodeID.SinglePlayer ? Main.myPlayer : Main.maxPlayers).npcProj = true;
						break;
					case 3:
						Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), shootPos, AttackVelocity, ModContent.ProjectileType<Betty_Apple>(), 4, 0.4f, owner: Main.netMode == NetmodeID.SinglePlayer ? Main.myPlayer : Main.maxPlayers).npcProj = true;
						break;
					default:
						break;
				}
			}
			yield return new SkipThisFrame();
		}
		AttackTimer = -1;
		EndAIPiece();
	}

	public override void CheckInSuitableArea()
	{
		if (!YggdrasilTownCentralSystem.InCanteen_YggdrasilTown())
		{
			CanDespawn = true;
			return;
		}
		CanDespawn = false;
		AnchorForBehaviorPos = new Point(146, 150);
		bool safe = false;
		var homePoint = AnchorForBehaviorPos;
		NPC.homeless = false;
		NPC.homeTileX = homePoint.X;
		NPC.homeTileY = homePoint.Y;

		var npcTilePos = NPC.Center.ToTileCoordinates();
		if (npcTilePos.X is > 100 and < 200)
		{
			if (npcTilePos.Y is > 100 and < 200)
			{
				safe = true;
			}
		}
		if (!safe)
		{
			NPC.Center = homePoint.ToWorldCoordinates() + new Vector2(0, 48);
		}
	}

	public override bool NeedSaving() => CanDespawn;

	public override bool CheckActive() => CanDespawn;

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		Texture2D texMain = ModAsset.CanteenMaid.Value;
		Vector2 drawPos = NPC.Center - screenPos + new Vector2(0, NPC.height - NPC.frame.Height + 4) * 0.5f;
		if (AttackTimer > 0)
		{
			var attackFrame = new Rectangle(0, 728, 38, 52);
			Main.spriteBatch.Draw(texMain, drawPos, attackFrame, drawColor, NPC.rotation, attackFrame.Size() * 0.5f, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
			var armFrame = new Rectangle(0, 782, 12, 20);
			if (NPC.spriteDirection == 1)
			{
				armFrame = new Rectangle(14, 782, 12, 20);
			}
			var armRot = NPC.rotation + AttackTimer * 0.2f * NPC.spriteDirection + MathF.PI;
			Main.spriteBatch.Draw(texMain, drawPos, armFrame, drawColor, armRot, new Vector2(6, 4), NPC.scale, SpriteEffects.None, 0);
		}
		else
		{
			Main.spriteBatch.Draw(texMain, drawPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
		}
		return false;
	}
}
