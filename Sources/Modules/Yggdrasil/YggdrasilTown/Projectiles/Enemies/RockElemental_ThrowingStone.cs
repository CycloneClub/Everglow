using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Enemies;

public class RockElemental_ThrowingStone : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.EnemyProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 32;
		Projectile.height = 32;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.ignoreWater = false;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 1023;
		Projectile.scale = 0;
		Projectile.drawLayer = ProjectileDrawLayerID.OverPlayers;
		Projectile.usesOwnerLight = true;
	}

	public NPC MyOwner;
	public int PolymerizationTimer;
	public Vector2 OffestSuck;
	public bool ShotAway = false;

	public override void AI()
	{
		if (MyOwner == null || !MyOwner.active || MyOwner.type != ModContent.NPCType<RockElemental>())
		{
			if (Projectile.timeLeft > 60)
			{
				Projectile.timeLeft = 60;
			}
		}
		if (Projectile.timeLeft <= 60)
		{
			Projectile.Kill();
			return;
		}

		// 丢出去之后
		if (ShotAway)
		{
			Projectile.rotation += 0.1f;
			Projectile.velocity.Y += 0.163f;
			for (int j = 0; j < 3; j++)
			{
				var d = Dust.NewDustDirect(Projectile.position - Projectile.velocity * 2, Projectile.width, Projectile.height, ModContent.DustType<RockElemental_Energy_normal>());
				d.velocity = Projectile.velocity * 0.5f;
				d.scale = Main.rand.NextFloat(0.75f, 1.4f);
				d.noGravity = true;
			}
			return;
		}
		Player player = Main.player[MyOwner.target];
		if (PolymerizationTimer >= 0)
		{
			Projectile.rotation = MyOwner.rotation * 0.25f + Projectile.rotation * 0.75f;
			Projectile.Center = MyOwner.Center + new Vector2(-24, 36).RotatedBy(MyOwner.rotation);
			PolymerizationTimer++;
			if (PolymerizationTimer > 30)
			{
				Projectile.scale = (float)Utils.Lerp(Projectile.scale, 1f, 0.05f);
			}
			if (PolymerizationTimer > 120)
			{
				Projectile.scale = 1;
				PolymerizationTimer = -1;
				SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact.WithVolume(0.5f), Projectile.Center);
			}
			if (PolymerizationTimer < 80 && PolymerizationTimer > 5)
			{
				for (int g = 0; g < 3; g++)
				{
					Vector2 vel = new Vector2(0, Main.rand.NextFloat(2, 8)).RotatedByRandom(MathHelper.TwoPi);
					Vector2 addPos = new Vector2(0, Main.rand.NextFloat(132, 181)).RotatedBy(MyOwner.rotation + MathHelper.PiOver2 + Main.rand.NextFloat(-0.1f, 0.1f));
					float mulScale = Main.rand.NextFloat(2f, 8f);
					var current = new RockElemental_SuckingLine
					{
						Velocity = vel,
						Active = true,
						Visible = true,
						Position = Projectile.Center + addPos - vel * 15,
						MaxTime = Main.rand.Next(142, 184),
						Scale = mulScale,
						VFXOwner = Projectile,
						ai = new float[] { 0f, Main.rand.NextFloat(-0.05f, 0.05f) },
					};
					Ins.VFXManager.Add(current);
				}
				for (int g = 0; g < 2; g++)
				{
					Vector2 vel = new Vector2(0, Main.rand.NextFloat(2, 8)).RotatedByRandom(MathHelper.TwoPi);
					Vector2 addPos = new Vector2(0, Main.rand.NextFloat(132, 181)).RotatedBy(MyOwner.rotation + MathHelper.PiOver2 + Main.rand.NextFloat(-0.1f, 0.1f));
					var dust = Dust.NewDustDirect(Projectile.Center + addPos - vel * 15 - new Vector2(4), 0, 0, ModContent.DustType<RockElemental_Energy>());
					dust.customData = Projectile;
					dust.velocity = vel;
					dust.scale = 0.07f;
				}
				var rockOwner = MyOwner.ModNPC as RockElemental;
				if (rockOwner != null)
				{
					OffestSuck = rockOwner.SuckPoint - Projectile.Center + (rockOwner.SuckPoint - Projectile.Center).SafeNormalize(Vector2.zeroVector) * 8;
				}
			}

			// 震动
			if (PolymerizationTimer == 88)
			{
				ShakerManager.AddShaker(Projectile.Center, new Vector2(0, -1), 20, 10, 120);
				Projectile.width = 32;
				Projectile.height = 32;
			}

			// 吸起音效
			if (PolymerizationTimer == 90)
			{
				for (int g = 0; g < 30; g++)
				{
					Vector2 vel = new Vector2(0, Main.rand.NextFloat(8, 24)).RotatedByRandom(MathHelper.TwoPi);
					var dust = Dust.NewDustDirect(Projectile.Center + OffestSuck, 0, 0, ModContent.DustType<RockElemental_Energy>());
					dust.velocity = vel;
					dust.scale = Main.rand.NextFloat(0.06f, 0.1f);
				}
				SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot, Projectile.Center);
			}
			if (PolymerizationTimer > 90)
			{
				OffestSuck *= 0.85f;
			}

			// 吸力音效
			if (PolymerizationTimer == 1)
			{
				SoundEngine.PlaySound(new SoundStyle(ModAsset.RockElemental_Sucking_Mod).WithPitchOffset(0.5f).WithVolumeScale(0.5f), Projectile.Center);
			}
		}

		// 跟随岩石元素一起旋转
		else
		{
			Projectile.rotation = MyOwner.rotation;
			Vector2 newPos = MyOwner.Center + new Vector2(-24, 36).RotatedBy(MyOwner.rotation);
			Vector2 shootVelocity = (newPos - Projectile.Center).SafeNormalize(Vector2.zeroVector) * 14f * (280 - MyOwner.ai[0]) / 200f;
			if (MinDistanceToClosestTarget(shootVelocity) < 30 && MyOwner.ai[0] < 240)
			{
				ShotAway = true;
				Projectile.velocity = shootVelocity;
				MyOwner.velocity -= shootVelocity * 0.5f;
				if (MyOwner.ai[0] > 30)
				{
					MyOwner.ai[0] = 30;
				}
				Projectile.tileCollide = true;
			}
			// 小于2直接甩出去
			if (MyOwner.ai[0] < 2)
			{
				ShotAway = true;
				Projectile.velocity = shootVelocity;
				MyOwner.velocity -= shootVelocity * 0.5f;
				if (MyOwner.ai[0] > 30)
				{
					MyOwner.ai[0] = 30;
				}
				Projectile.tileCollide = true;
			}
			Projectile.Center = newPos;
		}
	}

	public override void PostAI()
	{
		if (PolymerizationTimer > 0 && PolymerizationTimer < 100)
		{
			Projectile.usesOwnerLight = true;
			Projectile.drawLayer = ProjectileDrawLayerID.BehindNPCsAndTiles;
		}
		else
		{
			Projectile.usesOwnerLight = true;
			Projectile.drawLayer = ProjectileDrawLayerID.OverPlayers;
		}
	}

	public override void OnSpawn(IEntitySource source)
	{
		PolymerizationTimer = 0;
		if (MyOwner == null)
		{
			foreach (var npc in Main.npc)
			{
				if (npc.type == ModContent.NPCType<RockElemental>())
				{
					if (npc.active && npc.life >= 0)
					{
						if ((npc.Center - Projectile.Center).Length() < 500)
						{
							MyOwner = npc;
							break;
						}
					}
				}
			}
		}
		if (MyOwner == null)
		{
			Projectile.Kill();
		}
		var rockOwner = MyOwner.ModNPC as RockElemental;
		if (rockOwner == null)
		{
			Projectile.Kill();
		}
		OffestSuck = rockOwner.SuckPoint - Projectile.Center;
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
	{
	}

	public override void OnKill(int timeLeft)
	{
		for (int x = 0; x < 16; x++)
		{
			var d = Dust.NewDustDirect(Projectile.position - new Vector2(4), Projectile.width, Projectile.height, ModContent.DustType<RockElemental_fragments>(), 0f, 0f, 0, default, 0.7f);
			d.velocity = new Vector2(0, Main.rand.NextFloat(7f, 16f)).RotatedByRandom(6.283);
		}
		for (int x = 0; x < 32; x++)
		{
			var d = Dust.NewDustDirect(Projectile.position - new Vector2(4), Projectile.width, Projectile.height, DustID.WitherLightning, 0f, 0f, 0, default, Main.rand.NextFloat(0.6f, 1.1f));
			d.velocity = new Vector2(0, Main.rand.NextFloat(2f, 11f)).RotatedByRandom(6.283);
		}
		GenerateSmog(8);
		SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);
		ShakerManager.AddShaker(Projectile.Center, new Vector2(0, -1), 20, 30, 120);
	}

	public void GenerateSmog(int Frequency)
	{
		for (int g = 0; g < Frequency / 2 + 1; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 4f)).RotatedByRandom(MathHelper.TwoPi);
			var somg = new RockSmogDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(37, 45),
				Scale = Main.rand.NextFloat(40f, 55f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		Texture2D texture = ModAsset.RockElemental_ThrowingStone.Value;
		Vector2 drawCenter = Projectile.Center - Main.screenPosition;
		lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
		if (PolymerizationTimer < 0)
		{
			Main.EntitySpriteDraw(texture, drawCenter, null, lightColor, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		}
		if (PolymerizationTimer >= 0)
		{
			Vector2 shake = new Vector2(0, Main.rand.NextFloat(5f)).RotatedByRandom(MathHelper.TwoPi);
			float silence = 1f;
			if (PolymerizationTimer > 90)
			{
				silence = (120 - PolymerizationTimer) / 30f;
			}
			shake *= silence;
			Vector2 drawCenter2 = drawCenter + OffestSuck + shake;
			for (int i = 0; i < 20 * silence; i++)
			{
				Main.EntitySpriteDraw(texture, drawCenter2 + new Vector2(0, 7).RotatedBy(i / 5d * MathHelper.TwoPi + Main.time * 0.08), null, new Color(0.7f, 0.2f, 1f, 0f), Projectile.rotation + i, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
			}
			Main.EntitySpriteDraw(texture, drawCenter2 + shake, null, lightColor, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		}
		return false;
	}

	/// <summary>
	/// 求当前发射速度3秒内,能飞到距离玩家的最短距离
	/// </summary>
	/// <param name="shootVelocity"></param>
	/// <returns></returns>
	public float MinDistanceToClosestTarget(Vector2 shootVelocity)
	{
		int index = Player.FindClosest(Projectile.Center, 0, 0);
		if (index < 0)
		{
			return float.NaN;
		}
		Player player = Main.player[Player.FindClosest(Projectile.Center, 0, 0)];
		float distance = (Projectile.Center - player.Center).Length();
		Vector2 trackPos = Projectile.Center;

		// 寻找3秒之内擦得最近的点位
		for (int time = 0; time < 60; time++)
		{
			trackPos += shootVelocity;
			shootVelocity.Y += 0.163f;
			float currentDistance = (trackPos - player.Center).Length();
			distance = currentDistance < distance ? currentDistance : distance;
		}
		return distance;
	}
}
