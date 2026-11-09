using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Enemies;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs;

public class EucaLeafy : ModNPC
{
	public float FlyingAngularSpeed = 0.025f;
	public float FlyingMaxSpeed = 3.2f;
	public float FlyingTargetHeight = 90f;

	public int DashCooling = 0;

	public int DashTimer = 0;

	private float _flyingWidth;
	private float _flyingHeight;
	private float _flyingPhase;
	private bool _flyingParametersInitialized;

	public int TooDenseTimer = 0;
	public int TimeToEmitGas = 0;
	public int EmitCharge = 0;
	public bool Flying = false;
	public bool Escape = false;

	/// <summary>
	/// 0: front, 1:attach at left side; 2: attach at right side
	/// </summary>
	public int AttachStyle = 0;

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 3;
	}

	public override void SetDefaults()
	{
		NPC.width = 30;
		NPC.height = 30;
		NPC.lifeMax = 75;
		NPC.damage = 18;
		NPC.defense = 8;
		NPC.aiStyle = -1;
		NPC.knockBackResist = 0.5f;
		NPC.value = 100;
		NPC.friendly = false;
		NPC.noGravity = true;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;
		AIType = NPCID.WalkingAntlion;
	}

	public override void OnSpawn(IEntitySource source)
	{
		InitializeFlyingParameters();
		DashCooling = Main.rand.Next(60, 250);
		TimeToEmitGas = Main.rand.Next(360, 1800);
		FlyingAngularSpeed = Main.rand.NextFloat(0.02f, 0.03f);
		FlyingMaxSpeed = Main.rand.NextFloat(2.8f, 4.5f);
		NPC.scale = Main.rand.NextFloat(0.85f, 1.15f);
		float minDis = 5;
		Vector2 attachPoint = NPC.Center;
		for (int i = -3; i <= 3; i++)
		{
			for (int j = -3; j <= 3; j++)
			{
				var tile = TileUtils.SafeGetTile(NPC.Center.ToTileCoordinates() + new Point(i, j));
				var tileUp = TileUtils.SafeGetTile(NPC.Center.ToTileCoordinates() + new Point(i, j - 1));
				var tileDown = TileUtils.SafeGetTile(NPC.Center.ToTileCoordinates() + new Point(i, j + 1));
				if (TileID.Sets.IsATreeTrunk[tile.TileType] && TileID.Sets.IsATreeTrunk[tileUp.TileType] && TileID.Sets.IsATreeTrunk[tileDown.TileType])
				{
					Vector2 toNPC = new Vector2(i, j);
					if (toNPC.Length() < minDis)
					{
						minDis = toNPC.Length();
						attachPoint = (NPC.Center.ToTileCoordinates() + new Point(i, j)).ToWorldCoordinates();
					}
				}
			}
		}
		if (minDis >= 4.5f)
		{
			Flying = true;
		}
		else
		{
			NPC.Center = attachPoint;
			switch (Main.rand.Next(3))
			{
				case 1:
					NPC.Center += new Vector2(-12f, 0);
					AttachStyle = 1;
					break;
				case 2:
					NPC.Center += new Vector2(12f, 0);
					AttachStyle = 2;
					break;
			}
		}
	}

	public override void FindFrame(int frameHeight)
	{
		if (Flying)
		{
			NPC.frameCounter++;
			NPC.frame.X = 0;
			NPC.frame.Width = 48;
			NPC.frame.Height = 48;
		}
		if (NPC.frameCounter > 4)
		{
			NPC.frame.Y += frameHeight;
			if (NPC.frame.Y >= 3 * frameHeight)
			{
				NPC.frame.Y = 0;
			}
		}
	}

	public override void AI()
	{
		if (!Flying)
		{
			bool tooDense = false;
			foreach (var npc in Main.npc)
			{
				if (npc.type == Type && npc != NPC && npc.active)
				{
					Vector2 toNPC = NPC.Center - npc.Center;
					if (Math.Abs(toNPC.X) < 4 && Math.Abs(toNPC.Y) < 24)
					{
						NPC.Center += new Vector2(0, Math.Sign(toNPC.Y));
						tooDense = true;
					}
				}
			}
			if (tooDense)
			{
				TooDenseTimer++;
			}
			else
			{
				TooDenseTimer = 0;
			}
			if (TooDenseTimer > 120)
			{
				Flying = true;
			}
		}
		else if(!Escape)
		{
			FlyingAI();
		}
		else
		{
			NPC.velocity *= 0.98f;
			NPC.velocity.Y -= 0.1f;
		}
	}

	public void FlyingAI()
	{
		NPC.TargetClosest();
		if (NPC.target < 0 || !Main.player[NPC.target].active || Main.player[NPC.target].dead)
		{
			NPC.velocity *= 0.98f;
			NPC.velocity.Y -= 0.1f;
			return;
		}
		Player targetPlayer = Main.player[NPC.target];

		TimeToEmitGas--;
		if (TimeToEmitGas <= 0)
		{
			EmitCharge++;
			if (EmitCharge >= 60)
			{
				EmitGasAI();
				return;
			}
		}

		// Fly in a lemniscate pattern around the player, and occasionally dash towards the player
		if (DashTimer <= 0)
		{
			float time = NPC.ai[0]++;
			float parameter = _flyingPhase + time * FlyingAngularSpeed;
			float sin = MathF.Sin(parameter);
			float cos = MathF.Cos(parameter);
			float denominator = 1f + sin * sin;
			Vector2 lemniscateOffset = new Vector2(
				_flyingWidth * cos / denominator,
				_flyingHeight * 2f * sin * cos / denominator);
			Vector2 targetPosition = targetPlayer.Center + new Vector2(0f, -FlyingTargetHeight) + lemniscateOffset;
			Vector2 toTarget = targetPosition - NPC.Center;

			NPC.velocity = toTarget.LengthSquared() > 1f ? toTarget.SafeNormalize(Vector2.Zero) * FlyingMaxSpeed : Vector2.Zero;
			if (DashCooling <= 0)
			{
				DashTimer = 60;
				DashCooling = Main.rand.Next(60, 250);
				Vector2 toDashTarget = targetPlayer.Center - NPC.Center;
				toDashTarget = toDashTarget.NormalizeSafe();
				NPC.velocity = toDashTarget * 10f;
			}
			else
			{
				DashCooling--;
			}
		}
		else
		{
			DashTimer--;
			if (DashTimer < 30)
			{
				NPC.velocity *= 0.9f;
			}
			else if (NPC.collideX || NPC.collideY)
			{
				DashTimer = 5;
				NPC.velocity *= 0f;
			}
		}
		NPC.spriteDirection = NPC.velocity.X > 0f ? 1 : -1;
		NPC.rotation = NPC.velocity.X * 0.04f;
	}

	public void EmitGasAI()
	{
		NPC.TargetClosest();
		if (NPC.target < 0 || !Main.player[NPC.target].active || Main.player[NPC.target].dead)
		{
			Escape = true;
			return;
		}
		Player targetPlayer = Main.player[NPC.target];
		Vector2 toTarget = targetPlayer.Center - NPC.Center;
		if (toTarget.Length() < 100f)
		{
			toTarget = toTarget.NormalizeSafe();
			Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + toTarget * 30, toTarget * 2f, ModContent.ProjectileType<EucaLeafyGas>(), 24, 0);
			EmitCharge = -1;
			Escape = true;
			return;
		}
		toTarget = toTarget.NormalizeSafe();
		NPC.velocity = toTarget * FlyingMaxSpeed;
		NPC.spriteDirection = NPC.velocity.X > 0f ? 1 : -1;
		NPC.rotation = NPC.velocity.X * 0.04f;
	}

	private void InitializeFlyingParameters()
	{
		if (_flyingParametersInitialized)
		{
			return;
		}
		_flyingWidth = 175f + GetWhoAmIVariation(1) * 100f;
		_flyingHeight = 75f + GetWhoAmIVariation(2) * 50f;
		_flyingPhase = GetWhoAmIVariation(3) * MathHelper.TwoPi;
		_flyingParametersInitialized = true;
	}

	private float GetWhoAmIVariation(int salt)
	{
		float value = MathF.Sin((NPC.whoAmI + 1) * (12.9898f + salt * 78.233f));
		return (value + 1f) * 0.5f;
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		if (!Flying)
		{
			Flying = true;
		}
		base.HitEffect(hit);
	}

	public override void OnKill()
	{
		for (int i = 0; i < 5; i++)
		{
			Vector2 v0 = new Vector2(0, Main.rand.NextFloat(0, 6f)).RotatedByRandom(MathHelper.TwoPi);
			int type = ModContent.Find<ModGore>("Everglow/EucaLeafy_Gore_" + i).Type;
			Gore.NewGore(NPC.GetSource_Death(), NPC.Center, v0, type, NPC.scale);
		}
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		Texture2D tex = ModAsset.EucaLeafy_Stop.Value;
		if (!Flying)
		{
			Rectangle frame = new Rectangle(26, 0, 26, 42);
			SpriteEffects spEffect = SpriteEffects.None;
			if (AttachStyle == 1 || AttachStyle == 2)
			{
				frame = new Rectangle(0, 0, 26, 42);
			}
			if (AttachStyle == 2)
			{
				spEffect = SpriteEffects.FlipHorizontally;
			}
			spriteBatch.Draw(tex, NPC.Center + new Vector2(0, NPC.gfxOffY) - screenPos, frame, drawColor, NPC.rotation, frame.Size() * 0.5f, NPC.scale, spEffect, 0);
		}
		else
		{
			tex = ModAsset.EucaLeafy.Value;
			Rectangle frame = NPC.frame;
			if (EmitCharge > 0)
			{
				frame.X = 48;
			}
			if (EmitCharge >= 48)
			{
				frame.X = 96;
			}
			SpriteEffects spEffect = SpriteEffects.None;
			if (NPC.spriteDirection == -1)
			{
				spEffect = SpriteEffects.FlipHorizontally;
			}
			spriteBatch.Draw(tex, NPC.Center + new Vector2(0, NPC.gfxOffY) - screenPos, frame, drawColor, NPC.rotation, frame.Size() * 0.5f, NPC.scale, spEffect, 0);
			if (EmitCharge > 0)
			{
				Texture2D glow = ModAsset.EucaLeafy_glow.Value;
				spriteBatch.Draw(glow, NPC.Center + new Vector2(0, NPC.gfxOffY) - screenPos, frame, new Color(1f, 1f, 1f, 0), NPC.rotation, frame.Size() * 0.5f, NPC.scale, spEffect, 0);
			}
		}

		return false;
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
	}
}
