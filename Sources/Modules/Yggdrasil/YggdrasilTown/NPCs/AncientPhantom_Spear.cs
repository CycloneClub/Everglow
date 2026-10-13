using Everglow.Commons.Templates.Enemies;
using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Everglow.Yggdrasil.YggdrasilTown.Dusts.TwilightForest;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Enemies;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs;

public class AncientPhantom_Spear : Fighter
{
	public float HurtValue = 0f;

	public float SpearAttackTimer = 0f;

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 14;
		NPCSpawnManager.RegisterNPC(Type);
	}

	public override void SetDefaults()
	{
		NPC.width = 30;
		NPC.height = 40;
		NPC.lifeMax = 110;
		NPC.damage = 55;
		NPC.defense = 10;
		NPC.friendly = false;
		NPC.knockBackResist = 0.5f;
		NPC.value = 100;
		NPC.HitSound = SoundID.Item53;
		NPC.DeathSound = SoundID.Shatter;
		NPC.alpha = 100;

		MaxSpeedX = 3f;
	}

	public override void FindFrame(int frameHeight)
	{
		if (NPC.velocity.Y < 0f)
		{
			NPC.frameCounter = 0;
		}

		NPC.frameCounter += 1;
		int frameNumber = (int)(NPC.frameCounter / 3);
		if (frameNumber >= Main.npcFrameCount[NPC.type] - 5)
		{
			NPC.frameCounter = 0;
			frameNumber = 0;
		}
		if (!NPC.collideY)
		{
			frameNumber = Main.npcFrameCount[NPC.type] - 4;
		}
		if (SpearAttackTimer > 0)
		{
			frameNumber = Main.npcFrameCount[NPC.type] - 1;
		}
		if (SpearAttackTimer >= 50 && SpearAttackTimer < 55)
		{
			frameNumber = Main.npcFrameCount[NPC.type] - 2;
			Lighting.AddLight(NPC.Center + new Vector2(30 * NPC.direction, 0), new Vector3(0.6f, 0.8f, 1f));
		}
		if (SpearAttackTimer >= 55)
		{
			frameNumber = Main.npcFrameCount[NPC.type] - 3;
			Lighting.AddLight(NPC.Center + new Vector2(30 * NPC.direction, 0), Vector3.Lerp(new Vector3(0.0f, 0.0f, 0.1f), new Vector3(0.2f, 0.6f, 0.7f), (SpearAttackTimer - 55) / 5f));
		}
		NPC.frame.Y = frameNumber * frameHeight;
		if (HurtValue > 0)
		{
			HurtValue -= 2f;
		}
		else
		{
			HurtValue = 0f;
		}
	}

	public override void AI()
	{
		if (SpearAttackTimer > 0)
		{
			SpearAttackTimer--;
			if (SpearAttackTimer == 45 && NPC.target >= 0)
			{
				var target = Main.player[NPC.target];
				Vector2 distance = (target.Center - NPC.Center).NormalizeSafe().RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f));
				var proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, 8), distance, ModContent.ProjectileType<AncientPhantom_Spear_Spear>(), NPC.damage, 2, Main.myPlayer, NPC.whoAmI);
				AncientPhantom_Spear_Spear aPSS = proj.ModProjectile as AncientPhantom_Spear_Spear;
				if (aPSS is not null)
				{
					aPSS.OwnerEnemy = NPC;
				}
				proj.rotation = distance.ToRotationSafe();
			}
			if (SpearAttackTimer > 50)
			{
				NPC.velocity *= 0.1f;
			}
			if (SpearAttackTimer == 30)
			{
				NPC.velocity += new Vector2(-NPC.direction * 4, 0);
			}
		}
		else
		{
			SpearAttackTimer = 0;
			base.AI();
		}
		Lighting.AddLight(NPC.Bottom, new Vector3(0, 0.24f, 0.48f));
	}

	public override void Fighters_Accelerate()
	{
		if (NPC.velocity.Y == 0 && NPC.target >= 0)
		{
			Player player = Main.player[NPC.target];
			if ((player.Center - NPC.Center).Length() is > 90 and < 100)
			{
				if (Main.rand.NextBool(8))
				{
					SpearAttackTimer = 60;
				}
			}
		}
		base.Fighters_Accelerate();
	}

	public override void Fighters_Jump(int pushTileX, int pushTileY, bool canJump)
	{
		Player player = Main.player[NPC.target];
		if ((NPC.velocity.X < 0f && NPC.spriteDirection == -1) || (NPC.velocity.X > 0f && NPC.spriteDirection == 1))
		{
			// Jump over obstacles.
			if (NPC.height >= 32 && Main.tile[pushTileX, pushTileY - 2].HasUnactuatedTile && Main.tileSolid[Main.tile[pushTileX, pushTileY - 2].TileType])
			{
				if (Main.tile[pushTileX, pushTileY - 3].HasUnactuatedTile && Main.tileSolid[Main.tile[pushTileX, pushTileY - 3].TileType])
				{
					NPC.velocity.Y = -8f;
					NPC.netUpdate = true;
				}
				else
				{
					NPC.velocity.Y = -7f;
					NPC.netUpdate = true;
				}
			}
			else if (Main.tile[pushTileX, pushTileY - 1].HasUnactuatedTile && Main.tileSolid[Main.tile[pushTileX, pushTileY - 1].TileType])
			{
				NPC.velocity.Y = -6f;
				NPC.netUpdate = true;
			}
			else if (NPC.position.Y + NPC.height - pushTileY * 16 > 20f && Main.tile[pushTileX, pushTileY].HasUnactuatedTile && !Main.tile[pushTileX, pushTileY].TopSlope && Main.tileSolid[Main.tile[pushTileX, pushTileY].TileType])
			{
				NPC.velocity.Y = -5f;
				NPC.netUpdate = true;
			}
			else if (NPC.directionY < 0 && (!Main.tile[pushTileX, pushTileY + 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[pushTileX, pushTileY + 1].TileType]) && (!Main.tile[pushTileX + NPC.direction, pushTileY + 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[pushTileX + NPC.direction, pushTileY + 1].TileType]))
			{
				NPC.velocity.Y = -8f;
				NPC.velocity.X *= 1.5f;
				NPC.netUpdate = true;
			}

			// Jump when starting to accumulate aggro. "I found you! Look at me!"
			if (NPC.velocity.Y == 0f && canJump && NPC.ai[3] == 1f)
			{
				NPC.velocity.Y = -5f;
			}

			// Jump and attack target.
			if (NPC.velocity.Y == 0f && Main.expertMode && player.Bottom.Y < NPC.Top.Y && Math.Abs(NPC.Center.X - player.Center.X) < player.width * 3 && Collision.CanHit(NPC, player))
			{
				if (SpearAttackTimer <= 0)
				{
					if (Main.rand.NextBool(4))
					{
						int maxTileOverHead = 6;
						if (player.Bottom.Y > NPC.Top.Y - maxTileOverHead * 16)
						{
							NPC.velocity.Y = -7.9f;
						}
						else
						{
							int centerTileX = (int)(NPC.Center.X / 16f);
							int bottomTileY = (int)(NPC.Bottom.Y / 16f) - 1;
							for (int y = bottomTileY; y > bottomTileY - maxTileOverHead; y--)
							{
								if (Main.tile[centerTileX, y].HasUnactuatedTile && TileID.Sets.Platforms[Main.tile[centerTileX, y].TileType])
								{
									NPC.velocity.Y = -7.9f;
									break;
								}
							}
						}
					}
					else
					{
						SpearAttackTimer = 60;
					}
				}
			}
		}
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		HurtValue = 60f;
		base.HitEffect(hit);
	}

	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		TwilightForsetAndRelic twlightBiome = ModContent.GetInstance<TwilightForsetAndRelic>();
		return !twlightBiome.IsBiomeActive(Main.LocalPlayer) ? 0f : 3f;
	}

	public override void OnKill()
	{
		for (int h = 0; h < 40; h++)
		{
			Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<TwilightCrystalDust>());
			dust.velocity = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat()) * 6).RotatedByRandom(MathHelper.TwoPi);
		}
		for (int h = 0; h < 16; h++)
		{
			Vector2 velocity = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat()) * 6).RotatedByRandom(MathHelper.TwoPi);
			var dustVFX = new TwilightCrystalVFXDust
			{
				Velocity = velocity,
				Active = true,
				Visible = true,
				Position = NPC.Center + velocity * 6,
				MaxTime = Main.rand.Next(24, 42),
				Scale = Main.rand.NextFloat(3, 12),
				Rotation = velocity.ToRotation() - MathHelper.PiOver4,
				ai = new float[] { 0, 0, 0 },
			};
			Ins.VFXManager.Add(dustVFX);
		}
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		return true;
	}

	public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		SpriteEffects flip = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
		float fade = (255 - NPC.alpha) / 255f;
		Texture2D body_glow = ModAsset.AncientPhantom_Spear_glow.Value;
		spriteBatch.Draw(body_glow, NPC.Center + new Vector2(0, NPC.gfxOffY - 7) - screenPos, NPC.frame, Color.White, 0, NPC.frame.Size() * 0.5f, NPC.scale, flip, 0);

		Texture2D body_Shape = ModAsset.AncientPhantom_Spear_Shape.Value;
		Color hurtColor = Color.Lerp(Color.Blue, Color.White, HurtValue / 60f);
		hurtColor *= HurtValue / 60f;

		spriteBatch.Draw(body_Shape, NPC.Center + new Vector2(0, NPC.gfxOffY - 7) - screenPos, NPC.frame, hurtColor, 0, NPC.frame.Size() * 0.5f, NPC.scale, flip, 0);
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
	}
}
