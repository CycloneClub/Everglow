using Everglow.Commons.CustomTiles;
using Everglow.Commons.CustomTiles.Core;
using Everglow.Commons.Mechanics.Miscs;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Banners;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Enemies;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs;

[NoGameModeScale]
public class LightBulb : ModNPC
{
	public Vector2 RootPos = Vector2.zeroVector;

	public Vector2 RootToward = new Vector2(0, -1);

	private Point rootTilePosition = new Point(-1, -1);

	private ushort rootTileType;

	private BoxEntity rootBoxEntity;

	private Vector2 rootBoxOffset;

	private string rootBoxEntityTypeName = string.Empty;

	private bool rootIsDynamic;

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 12;
	}

	public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
	{
		bestiaryEntry.Info.Add(ModContent.GetInstance<LampWoodForest>().ModBiomeBestiaryInfoElement);
		bestiaryEntry.Info.Add(BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime);
		bestiaryEntry.Info.Add(new FlavorTextBestiaryInfoElement(LocalizationUtils.GetHabitatLocalizationKey("LampWoodDay")));
	}

	public override void SetDefaults()
	{
		Banner = Type;
		BannerItem = ModContent.ItemType<LightBulbBanner>();
		NPC.width = 42;
		NPC.height = 42;
		NPC.lifeMax = Main.masterMode ? 90 : Main.expertMode ? 65 : 40;
		NPC.damage = Main.masterMode ? 16 : Main.expertMode ? 12 : 8;
		NPC.defense = 2;
		NPC.friendly = false;
		NPC.aiStyle = -1;
		NPC.knockBackResist = 0;
		NPC.noGravity = false;
		NPC.noTileCollide = false;
		NPC.value = 100;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
	{
		target.AddBuff(BuffID.Poisoned, 600);
	}

	public override void FindFrame(int frameHeight)
	{
		switch (state)
		{
			case (int)NPCState.Sleep:
				{
					NPC.frame.Y = (int)(NPC.frameCounter / 8 % 4 + 8) * frameHeight;
					break;
				}
			case (int)NPCState.Charge:
				{
					NPC.frame.Y = (int)(NPC.frameCounter / 8 % 4 + 8) * frameHeight;
					break;
				}
			case (int)NPCState.Attack:
				{
					NPC.frame.Y = (int)(NPC.frameCounter / 3) * frameHeight;
					break;
				}
			case (int)NPCState.Cooldown:
				{
					NPC.frame.Y = (int)(NPC.frameCounter / 8 % 4 + 8) * frameHeight;
					break;
				}
		}
	}

	public override void OnSpawn(IEntitySource source)
	{
		state = (int)NPCState.Sleep;
		TryAttachToNearbyTile();
	}

	public override void SendExtraAI(System.IO.BinaryWriter writer)
	{
		writer.Write(RootPos.X);
		writer.Write(RootPos.Y);
		writer.Write(RootToward.X);
		writer.Write(RootToward.Y);
		writer.Write(rootTilePosition.X);
		writer.Write(rootTilePosition.Y);
		writer.Write(rootTileType);
		writer.Write(rootIsDynamic);
		writer.Write(rootBoxOffset.X);
		writer.Write(rootBoxOffset.Y);
		writer.Write(rootBoxEntityTypeName ?? string.Empty);
	}

	public override void ReceiveExtraAI(System.IO.BinaryReader reader)
	{
		RootPos = new Vector2(reader.ReadSingle(), reader.ReadSingle());
		RootToward = new Vector2(reader.ReadSingle(), reader.ReadSingle());
		rootTilePosition = new Point(reader.ReadInt32(), reader.ReadInt32());
		rootTileType = reader.ReadUInt16();
		rootIsDynamic = reader.ReadBoolean();
		rootBoxOffset = new Vector2(reader.ReadSingle(), reader.ReadSingle());
		rootBoxEntityTypeName = reader.ReadString();
		if (rootIsDynamic)
		{
			rootBoxEntity = FindAttachedBoxEntity();
		}
	}

	private BoxEntity FindAttachedBoxEntity()
	{
		if (!ColliderManager.Enable)
		{
			return null;
		}

		BoxEntity nearestEntity = null;
		float nearestDistance = float.MaxValue;
		foreach (BoxEntity entity in ColliderManager.Instance.OfType<BoxEntity>())
		{
			if (!entity.Active || entity.GetType().FullName != rootBoxEntityTypeName)
			{
				continue;
			}

			Vector2 candidateRoot = entity.Position + rootBoxOffset;
			float distance = Vector2.DistanceSquared(candidateRoot, RootPos);
			if (distance < nearestDistance)
			{
				nearestDistance = distance;
				nearestEntity = entity;
			}
		}
		return nearestEntity;
	}

	private static bool IsSolidTileAt(Point tilePosition)
	{
		if (!WorldGen.InWorld(tilePosition.X, tilePosition.Y, 1))
		{
			return false;
		}

		Tile tile = TileUtils.SafeGetTile(tilePosition);
		return tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType];
	}

	private bool TryAttachToNearbyTile()
	{
		if (RootPos != Vector2.zeroVector)
		{
			return true;
		}

		const int searchRadius = 12;
		Point center = NPC.Center.ToTileCoordinates();
		Vector2 nearestRoot = Vector2.zeroVector;
		Vector2 nearestNormal = Vector2.Zero;
		Point nearestTile = new Point(-1, -1);
		BoxEntity nearestBoxEntity = null;
		float nearestDistance = float.MaxValue;

		void Consider(Point tilePosition, Vector2 root, Vector2 normal)
		{
			if (!WorldGen.InWorld(tilePosition.X, tilePosition.Y, 1))
			{
				return;
			}
			float distance = Vector2.DistanceSquared(NPC.Center, root);
			if (distance < nearestDistance)
			{
				nearestDistance = distance;
				nearestRoot = root;
				nearestNormal = normal;
				nearestTile = tilePosition;
				nearestBoxEntity = null;
			}
		}

		void ConsiderBoxEntity(BoxEntity entity, Vector2 root, Vector2 normal)
		{
			float distance = Vector2.DistanceSquared(NPC.Center, root);
			if (distance > searchRadius * 16f * searchRadius * 16f || distance >= nearestDistance)
			{
				return;
			}

			nearestDistance = distance;
			nearestRoot = root;
			nearestNormal = normal;
			nearestTile = new Point(-1, -1);
			nearestBoxEntity = entity;
		}

		for (int x = center.X - searchRadius; x <= center.X + searchRadius; x++)
		{
			for (int y = center.Y - searchRadius; y <= center.Y + searchRadius; y++)
			{
				Point tilePosition = new Point(x, y);
				if (!IsSolidTileAt(tilePosition))
				{
					continue;
				}

				Tile tile = TileUtils.SafeGetTile(x, y);
				Vector2 tileCenter = new Vector2(x * 16 + 8, y * 16 + 8);
				if (tile.Slope == SlopeType.Solid)
				{
					Vector2 normal = new Vector2(0, -1);
					if (tile.IsHalfBlock)
					{
						if (!IsSolidTileAt(new Point(x, y - 1)))
						{
							Consider(tilePosition, new Vector2(x * 16 + 8, y * 16 + 8), normal);
						}
						continue;
					}

					Vector2[] normals =
					{
						new Vector2(0, -1),
						new Vector2(1, 0),
						new Vector2(0, 1),
						new Vector2(-1, 0),
					};
					foreach (Vector2 candidateNormal in normals)
					{
						Point neighbor = tilePosition + candidateNormal.ToPoint();
						if (!IsSolidTileAt(neighbor))
						{
							Consider(tilePosition, tileCenter + candidateNormal * 8f, candidateNormal);
						}
					}
					continue;
				}

				Vector2 slopeNormal = tile.Slope switch
				{
					SlopeType.SlopeDownLeft => new Vector2(-1, -1).NormalizeSafe(),
					SlopeType.SlopeDownRight => new Vector2(1, -1).NormalizeSafe(),
					SlopeType.SlopeUpLeft => new Vector2(-1, 1).NormalizeSafe(),
					SlopeType.SlopeUpRight => new Vector2(1, 1).NormalizeSafe(),
					_ => Vector2.UnitY,
				};
				if (!IsSolidTileAt(tilePosition + slopeNormal.ToPoint()))
				{
					Consider(tilePosition, tileCenter + slopeNormal * 8f, slopeNormal);
				}
			}
		}

		if (ColliderManager.Enable)
		{
			foreach (BoxEntity entity in ColliderManager.Instance.OfType<BoxEntity>())
			{
				if (!entity.Active || entity.Size.X <= 0 || entity.Size.Y <= 0)
				{
					continue;
				}

				var box = entity.Box;
				ConsiderBoxEntity(
					entity,
					new Vector2(MathHelper.Clamp(NPC.Center.X, box.Left, box.Right), box.Top), -Vector2.UnitY);
				ConsiderBoxEntity(
					entity,
					new Vector2(box.Right, MathHelper.Clamp(NPC.Center.Y, box.Top, box.Bottom)), Vector2.UnitX);
				ConsiderBoxEntity(
					entity,
					new Vector2(MathHelper.Clamp(NPC.Center.X, box.Left, box.Right), box.Bottom), Vector2.UnitY);
				ConsiderBoxEntity(
					entity,
					new Vector2(box.Left, MathHelper.Clamp(NPC.Center.Y, box.Top, box.Bottom)), -Vector2.UnitX);
			}
		}

		if (nearestTile.X < 0 && nearestBoxEntity is null)
		{
			return false;
		}

		RootPos = nearestRoot;
		RootToward = nearestNormal;
		rootIsDynamic = nearestBoxEntity is not null;
		rootBoxEntity = nearestBoxEntity;
		if (rootIsDynamic)
		{
			rootBoxOffset = RootPos - rootBoxEntity.Position;
			rootBoxEntityTypeName = rootBoxEntity.GetType().FullName;
			rootTilePosition = new Point(-1, -1);
		}
		else
		{
			rootTilePosition = nearestTile;
			rootTileType = TileUtils.SafeGetTile(rootTilePosition).TileType;
			rootBoxOffset = Vector2.Zero;
			rootBoxEntityTypeName = string.Empty;
		}
		NPC.Center = RootPos + RootToward * (NPC.height * 0.5f + 1f);
		NPC.velocity = Vector2.Zero;
		NPC.noGravity = true;
		NPC.netUpdate = true;
		return true;
	}

	private int state;

	private enum NPCState
	{
		Sleep,
		Charge,
		Attack,
		Cooldown,
	}

	public override void AI()
	{
		Lighting.AddLight(NPC.Center, 0.6f, 0.6f, 0);
		float timeValue = (float)Main.time * 0.024f + TileUtils.GetFixedRandomNumber(NPC.whoAmI, (int)NPC.position.X) / 1024f * MathHelper.TwoPi;
		if (NPC.frame.Y == 270)
		{
			Lighting.AddLight(NPC.Center, 2.6f, 2.6f, 0.6f);
		}
		if (RootPos != Vector2.zeroVector)
		{
			if (rootIsDynamic)
			{
				if (rootBoxEntity is null || !rootBoxEntity.Active)
				{
					rootBoxEntity = FindAttachedBoxEntity();
				}
				if (rootBoxEntity is null || !rootBoxEntity.Active)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						NPC.StrikeInstantKill();
						NPC.netUpdate = true;
					}
					return;
				}

				Vector2 nextRootPos = rootBoxEntity.Position + rootBoxOffset;
				Vector2 movement = nextRootPos - RootPos;
				RootPos = nextRootPos;
				NPC.position += movement;
				if (movement != Vector2.Zero && Main.netMode == NetmodeID.Server && Main.GameUpdateCount % 10UL == (ulong)(NPC.whoAmI % 10))
				{
					NPC.netUpdate = true;
				}
			}
			else if (!IsSolidTileAt(rootTilePosition) || TileUtils.SafeGetTile(rootTilePosition).TileType != rootTileType)
			{
				NPC.StrikeInstantKill();
				NPC.netUpdate = true;
				return;
			}
		}
		else
		{
			TryAttachToNearbyTile();
		}
		NPC.TargetClosest();
		if (!NPC.HasValidTarget)
		{
			return;
		}
		Player player = Main.player[NPC.target];
		// Main.NewText(state);
		switch (state)
		{
			case (int)NPCState.Sleep:
				{
					NPC.frameCounter++;
					NPC.ai[0] = 0;
					if (player.Distance(NPC.Center) <= 550)
					{
						Vector2 npcToward = new Vector2(0, -1).RotatedBy(NPC.rotation);
						Vector2 toPlayer = (player.Center - NPC.Center).NormalizeSafe();
						if (RootPos != Vector2.zeroVector)
						{
							NPC.velocity += toPlayer * 0.3f;
							NPC.velocity *= MathF.Pow(1 - 1 / 450f, (NPC.Center - RootPos).Length());
						}
						float deltaRotation = MathF.Asin(Vector3.Cross(new Vector3(npcToward, 0), new Vector3(toPlayer, 0)).Z);
						NPC.rotation += deltaRotation * 0.1f;
						if (player.Distance(NPC.Center) >= 250)
						{
							state = (int)NPCState.Charge;
							NPC.ai[0] = 0;
							NPC.frameCounter = 0;
						}
					}
					else
					{
						Wandering();
					}
					break;
				}
			case (int)NPCState.Charge:
				{
					NPC.frameCounter++;
					NPC.ai[0]++;
					Vector2 npcToward = new Vector2(0, -1).RotatedBy(NPC.rotation);
					Vector2 toPlayer = (player.Center - NPC.Center).NormalizeSafe();
					float deltaRotation = MathF.Asin(Vector3.Cross(new Vector3(npcToward, 0), new Vector3(toPlayer, 0)).Z);
					NPC.rotation += deltaRotation * 0.1f;
					if (RootPos != Vector2.zeroVector)
					{
						if ((player.Center - NPC.Center).Length() < 500)
						{
							Vector2 target = player.Center;
							Vector2 toTarget = (target - NPC.Center - NPC.velocity).NormalizeSafe() * 12.5f;
							NPC.velocity = toTarget * 0.25f + NPC.velocity * 0.75f;
							NPC.velocity *= MathF.Pow(1 - 1 / 450f, (NPC.Center - RootPos).Length());
						}
						else
						{
							Wandering();
						}
					}
					if (player.Distance(NPC.Center) <= 150 || NPC.ai[0] > 120)
					{
						state = (int)NPCState.Attack;
						NPC.frameCounter = 0;
						NPC.ai[0] = 0;
					}
					break;
				}
			case (int)NPCState.Attack:
				{
					if (NPC.ai[0] == 9)
					{
						for (int i = 0; i < 6; i++)
						{
							Vector2 v = new Vector2(5f, 0);
							v = v.RotatedBy(-Math.PI * i / 5 + NPC.rotation).RotatedByRandom(Math.PI / 10);
							if (v.Y > 0)
							{
								v.Y = -v.Y;
							}
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, -40).RotatedBy(NPC.rotation), v, ModContent.ProjectileType<LightSeed>(), 4, 0, Owner: -1);
						}
						for (int x = 0; x < 12; x++)
						{
							Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 4f)).RotatedByRandom(MathHelper.TwoPi) + new Vector2(0, -3f).RotatedBy(NPC.rotation);
							Dust dust = Dust.NewDustDirect(NPC.Center + new Vector2(0, -40).RotatedBy(NPC.rotation), 0, 0, ModContent.DustType<LampWood_Dust_Fluorescent_Appear>());
							dust.velocity = newVelocity;
							dust.scale = Main.rand.NextFloat(1.2f, 2f);
							dust.rotation = Main.rand.NextFloat(0.4f, 0.8f);
						}
						SoundEngine.PlaySound(SoundID.NPCDeath3.WithPitchOffset(1), NPC.Center - new Vector2(0, 40));
						NPC.velocity += new Vector2(0, 10).RotatedBy(NPC.rotation);
					}
					NPC.ai[0]++;
					NPC.frameCounter++;
					NPC.TargetClosest();
					NPC.velocity *= 0.9f;
					if (NPC.ai[0] >= 36)
					{
						state = (int)NPCState.Cooldown;
					}
					break;
				}
			case (int)NPCState.Cooldown:
				{
					NPC.frameCounter++;
					NPC.ai[0]++;
					Wandering();
					if ((NPC.HasValidTarget && Main.player[NPC.target].Distance(NPC.Center) > 750) || NPC.ai[0] >= 160)
					{
						state = (int)NPCState.Charge;
						NPC.ai[0] = 0;
					}
					break;
				}
		}
		// RestrictFromRootPos();
	}

	public void Wandering()
	{
		float timeValue = (float)Main.time * 0.024f + NPC.whoAmI;
		float value0 = MathF.Sin(NPC.whoAmI) * 30f;
		float value1 = MathF.Sin(NPC.whoAmI + 1.5f) * 40f;
		if (RootPos != Vector2.zeroVector)
		{
			Vector2 target = RootPos + new Vector2(MathF.Sin(timeValue), MathF.Sin(timeValue * 2)) * new Vector2(120 + value0, 60) + new Vector2(0, -150 + value1);
			Vector2 toTarget = (target - NPC.Center - NPC.velocity).NormalizeSafe() * 2.5f;
			NPC.velocity = toTarget * 0.25f + NPC.velocity * 0.75f;
			NPC.rotation = NPC.velocity.X * 0.05f;
		}
	}

	public void RestrictFromRootPos()
	{
		if (RootPos != Vector2.zeroVector)
		{
			Vector2 toRoot = RootPos - NPC.Center;
			float length = toRoot.Length();
			if (toRoot.Length() > 160)
			{
				NPC.velocity -= toRoot.NormalizeSafe() * (length - 160) * 0.2f;
			}
		}
	}

	public override void OnKill()
	{
		for (int i = 0; i < 5; i++)
		{
			Vector2 v0 = new Vector2(0, Main.rand.NextFloat(0, 6f)).RotatedByRandom(MathHelper.TwoPi);
			int type = ModContent.Find<ModGore>("Everglow/GlowingBlossom_Gore" + Main.rand.Next(4)).Type;
			Gore.NewGore(NPC.GetSource_Death(), NPC.Center, v0, type, NPC.scale);
		}
		for (int i = 0; i < 8; i++)
		{
			Vector2 v0 = new Vector2(0, Main.rand.NextFloat(0, 6f)).RotatedByRandom(MathHelper.TwoPi);
			int type = ModContent.Find<ModGore>("Everglow/GlowingBlossom_Gore" + Main.rand.Next(4, 7)).Type;
			Gore.NewGore(NPC.GetSource_Death(), NPC.Center, v0, type, NPC.scale);
		}

		Vector2 stemStart = RootPos;
		Vector2 stemEnd = NPC.Bottom;
		Vector2 stemStartTangent = new Vector2(0, -1);
		Vector2 stemEndTangent = new Vector2(0, -1).RotatedBy(NPC.rotation);
		float handleLength = MathHelper.Clamp(stemStart.Distance(stemEnd) * 0.35f, 24f, 100f);
		Vector2 controlPoint1 = stemStart + stemStartTangent * handleLength;
		Vector2 controlPoint2 = stemEnd - stemEndTangent * handleLength;
		int segmentCount = Math.Clamp((int)MathF.Ceiling(stemStart.Distance(stemEnd) / 10f), 2, 160);

		for (int k = 0; k <= segmentCount; k++)
		{
			float t = k / (float)segmentCount;
			float inverseT = 1f - t;
			Vector2 stemPos = inverseT * inverseT * inverseT * stemStart
				+ 3f * inverseT * inverseT * t * controlPoint1
				+ 3f * inverseT * t * t * controlPoint2
				+ t * t * t * stemEnd;
			if (k % 2 == 1)
			{
				Vector2 v0 = new Vector2(0, Main.rand.NextFloat(0, 6f)).RotatedByRandom(MathHelper.TwoPi);
				int type = ModContent.Find<ModGore>("Everglow/GlowingBlossom_Gore7").Type;
				Gore.NewGore(NPC.GetSource_Death(), stemPos, v0, type, NPC.scale);
			}
		}
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<UnstablePollen>(), 1, 1, 2));
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LightBulbOvule>(), 24, 1));
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		Texture2D tex = ModAsset.LightBulb.Value;
		if (RootPos != Vector2.zeroVector)
		{
			Texture2D root = ModAsset.LightBulb_Root.Value;
			spriteBatch.Draw(root, RootPos - Main.screenPosition, null, drawColor, RootToward.ToRotation() + MathHelper.PiOver2, root.Size() * new Vector2(0.5f, 1f), NPC.scale, SpriteEffects.None, 0);
			Texture2D stem = ModAsset.LightBulb_Stem.Value;
			Vector2 stemStart = RootPos;
			Vector2 stemEnd = NPC.Bottom;
			Vector2 stemStartTangent = RootToward;
			Vector2 stemEndTangent = new Vector2(0, -1).RotatedBy(NPC.rotation);
			float handleLength = MathHelper.Clamp(stemStart.Distance(stemEnd) * 0.35f, 24f, 100f);
			Vector2 controlPoint1 = stemStart + stemStartTangent * handleLength;
			Vector2 controlPoint2 = stemEnd - stemEndTangent * handleLength;
			int segmentCount = Math.Clamp((int)MathF.Ceiling(stemStart.Distance(stemEnd) / 10f), 2, 160);

			for (int k = 0; k <= segmentCount; k++)
			{
				float t = k / (float)segmentCount;
				float inverseT = 1f - t;
				Vector2 stemPos = inverseT * inverseT * inverseT * stemStart
					+ 3f * inverseT * inverseT * t * controlPoint1
					+ 3f * inverseT * t * t * controlPoint2
					+ t * t * t * stemEnd;
				Vector2 stemTangent = 3f * inverseT * inverseT * (controlPoint1 - stemStart)
					+ 6f * inverseT * t * (controlPoint2 - controlPoint1)
					+ 3f * t * t * (stemEnd - controlPoint2);
				Rectangle frame = new Rectangle(0, (k % 2) * 19, 16, 19);
				spriteBatch.Draw(stem, stemPos - Main.screenPosition, frame, drawColor, stemTangent.ToRotationSafe() + MathHelper.PiOver2, frame.Size() * new Vector2(0.5f, 1f), NPC.scale, SpriteEffects.None, 0);
			}
		}
		spriteBatch.Draw(tex, NPC.Bottom - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() * new Vector2(0.5f, 1f), NPC.scale, SpriteEffects.None, 0);
		return false;
	}

	public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		Texture2D glow = ModAsset.LightBulb_Glow.Value;
		spriteBatch.Draw(glow, NPC.Bottom - Main.screenPosition, NPC.frame, new Color(1f, 1f, 1f, 0) * 0.5f, NPC.rotation, NPC.frame.Size() * new Vector2(0.5f, 1f), NPC.scale, SpriteEffects.None, 0);
		base.PostDraw(spriteBatch, screenPos, drawColor);
	}
}
