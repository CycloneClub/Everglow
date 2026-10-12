using Everglow.Commons.Coroutines;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

[AutoloadHead]
public class Howard_Warden : TownNPC_LiveInYggdrasil
{
	public override string HeadTexture => ModAsset.Howard_Warden_Head_Mod;

	public bool Attacking = false;

	public bool CanDespawn = false;

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 13;
	}

	public override void SetDefaults()
	{
		StandFrame = new Rectangle(0, 0, 42, 60);
		SitFrame = new Rectangle(0, 600, 42, 60);
		NPC.frame = StandFrame;
		FrameHeight = 60;

		base.SetDefaults();
	}

	public override void CheckInSuitableArea()
	{
		if (!YggdrasilTownCentralSystem.InUnion_YggdrasilTown())
		{
			CanDespawn = true;
			return;
		}
		CanDespawn = false;
		AnchorForBehaviorPos = new Point(240, 173);
		bool safe = false;
		var homePoint = AnchorForBehaviorPos;
		NPC.homeless = false;
		NPC.homeTileX = homePoint.X;
		NPC.homeTileY = homePoint.Y + 2;

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

	public override bool NeedSaving() => CanDespawn;

	public override bool CheckActive() => CanDespawn;

	public override void TryAttack()
	{
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
			BehaviorsCoroutines.Enqueue(new Coroutine(Attack()));
		}
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
		var vel = (target.Center - NPC.Center).NormalizeSafe();
		Projectile myGun = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, vel, ModContent.ProjectileType<Howard_Shoot>(), 48, 4, Main.myPlayer, NPC.whoAmI);
		for (int t = 0; t < 180; t++)
		{
			if (myGun == null || !myGun.active || myGun.type != ModContent.ProjectileType<Howard_Shoot>())
			{
				break;
			}
			Howard_Shoot hS = myGun.ModProjectile as Howard_Shoot;
			if (hS != null && hS.Target != null)
			{
				target = hS.Target;
			}
			NPC.frame = new Rectangle(0, 720, 42, 60);
			Attacking = true;
			NPC.velocity.X *= 0;
			if (target != null && target.active)
			{
				if (target.Center.X > NPC.Center.X)
				{
					NPC.direction = 1;
				}
				if (target.Center.X < NPC.Center.X)
				{
					NPC.direction = -1;
				}
				NPC.spriteDirection = NPC.direction;
			}
			yield return new SkipThisFrame();
		}
		Attacking = false;
		EndAIPiece();
	}

	public override void WalkFrame()
	{
		NPC.frameCounter += Math.Abs(NPC.velocity.X * 1.3f);
		if (NPC.frameCounter > 4)
		{
			NPC.frame.Y += FrameHeight;
			NPC.frameCounter = 0;
		}
		if (NPC.frame.Y > 10 * FrameHeight)
		{
			NPC.frame.Y = FrameHeight;
		}
	}

	public override void CheckWalkBound()
	{
		if (!YggdrasilTownCentralSystem.InUnion_YggdrasilTown())
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

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		Texture2D texMain = ModAsset.Howard_Warden.Value;
		Vector2 drawPos = NPC.Center - screenPos + new Vector2(0, NPC.height - NPC.frame.Height) * 0.5f;
		Main.spriteBatch.Draw(texMain, drawPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
		return false;
	}
}
