using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Summon;

public class EvilMusicRemnant_Projectile : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonProjectiles;

	public const int SearchDistance = 200;
	public Vector2 ClickCenter;
	public int MaxTime = 600;

	public int TargetWhoAmI
	{
		get => (int)Projectile.ai[0];
		set => Projectile.ai[0] = value;
	}

	public override void SetDefaults()
	{
		Projectile.width = 26;
		Projectile.height = 26;

		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.timeLeft = MaxTime;
		Projectile.DamageType = DamageClass.Summon;
		Projectile.drawLayer = ProjectileDrawLayerID.BehindProjectiles;
	}

	public override void OnSpawn(IEntitySource source)
	{
		// Select random texture
		Projectile.timeLeft += Main.rand.Next(-30, 30);
		ClickCenter = Main.MouseWorld;
		var num = Main.rand.Next(6);
		switch (num)
		{
			case 1:
				ProjectileTexture = ModAsset.EvilMusicRemnant_Projectile2_Mod;
				break;
			case 2:
				ProjectileTexture = ModAsset.EvilMusicRemnant_Projectile3_Mod;
				break;
			case 3:
				ProjectileTexture = ModAsset.EvilMusicRemnant_Projectile4_Mod;
				break;
			case 4:
				ProjectileTexture = ModAsset.EvilMusicRemnant_Projectile5_Mod;
				break;
			case 5:
				ProjectileTexture = ModAsset.EvilMusicRemnant_Projectile6_Mod;
				break;
			default:
				ProjectileTexture = ModAsset.EvilMusicRemnant_Projectile1_Mod;
				break;
		}

		TargetWhoAmI = -1;
	}

	public bool BehideProj = true;

	private string ProjectileTexture { get; set; } = ModAsset.EvilMusicRemnant_Projectile1_Mod;

	public override string Texture => ProjectileTexture;

	public override void AI()
	{
		var owner = Main.player[Projectile.owner];
		CheckClickCenter();
		if (TargetWhoAmI < 0)
		{
			TargetWhoAmI = ProjectileUtils.FindTarget(Projectile.Center, SearchDistance);
			if (TargetWhoAmI >= 0)
			{
				Projectile.netUpdate = true;
			}
			else if (Projectile.timeLeft < MaxTime - 30)
			{
				if (ClickCenter != Vector2.zeroVector)
				{
					var ellipses = new Vector2(0, 80).RotatedBy(Projectile.whoAmI + Main.time * 0.04f);
					ellipses.Y *= 0.15f;
					var targetPos = ClickCenter + ellipses;
					var directionToTarget = (targetPos - Projectile.Center - Projectile.velocity).NormalizeSafe();
					Projectile.velocity = 0.05f.Lerp(Projectile.velocity, directionToTarget * 6);
					if (Projectile.velocity.X < 0)
					{
						BehideProj = false;
					}
					else
					{
						BehideProj = true;
					}
					Projectile.usesOwnerLight = true;
					if (Projectile.timeLeft < 240 && Projectile.timeLeft > 60 && owner.ownedProjectileCounts[Type] <= 30)
					{
						Projectile.timeLeft = Main.rand.Next(240, 270);
					}
				}
				else
				{
					Projectile.usesOwnerLight = false;
				}
			}
		}
		else
		{
			if (!ProjectileUtils.MinionCheckTargetActive(TargetWhoAmI))
			{
				TargetWhoAmI = -1;
				return;
			}

			var target = Main.npc[TargetWhoAmI];
			var directionToTarget = (target.Center - Projectile.Center - Projectile.velocity).NormalizeSafe();
			Projectile.velocity = 0.05f.Lerp(Projectile.velocity, directionToTarget * 6);
		}
	}

	public override void PostAI()
	{
		if (BehideProj)
		{
			Projectile.drawLayer = ProjectileDrawLayerID.BehindProjectiles;
		}
		else
		{
			Projectile.drawLayer = ProjectileDrawLayerID.OverPlayers;
		}
	}

	public void CheckClickCenter()
	{
		float closest = 20000;
		Vector2 minDisPos = Vector2.zeroVector;
		foreach (var proj in Main.projectile)
		{
			if (proj != null && proj.active)
			{
				if (proj.type == ModContent.ProjectileType<EvilMusicRemnant_Note_Mark>())
				{
					float distance = (proj.Center - Projectile.Center).Length();
					if (distance < closest)
					{
						closest = distance;
						minDisPos = proj.Center;
					}
				}
			}
		}
		ClickCenter = minDisPos;
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		float fade = 1f;
		if (Projectile.timeLeft < 60f)
		{
			fade = Projectile.timeLeft / 60f;
		}
		var texture = ModContent.Request<Texture2D>(ProjectileTexture).Value;
		float tremorValue = MathF.Sin(Projectile.timeLeft * 0.1f + Projectile.whoAmI) * 0.3f + 1f;
		for (int i = 0; i < 6; i++)
		{
			var termorPos = new Vector2(2, 0).RotatedBy(i / 6f * MathHelper.TwoPi);
			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + termorPos, null, new Color(153, 110, 255, 255) * 0.45f * fade, 0, texture.Size() / 2, tremorValue, SpriteEffects.None, 0);
		}
		Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, new Color(74, 23, 124, 255) * fade, 0, texture.Size() / 2, tremorValue, SpriteEffects.None, 0);
		return false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (!target.active && !target.friendly && !target.townNPC)
		{
			SummonMinion();
		}
	}

	public void SummonMinion()
	{
		var owner = Main.player[Projectile.owner];
		var minionProjType = ModContent.ProjectileType<EvilMusicRemnant_Minion>();
		for (int x = 0; x < owner.ownedProjectileCounts[minionProjType]; x++)
		{
			float ownerRealMinions = owner.slotsMinions;
			foreach (Projectile proj in Main.projectile)
			{
				if (proj != null && proj.active)
				{
					if (proj.type == minionProjType && proj.minionSlots == 0)
					{
						var eMRM = proj.ModProjectile as EvilMusicRemnant_Minion;
						if (eMRM != null && !eMRM.ExplosionCommand)
						{
							ownerRealMinions++;
						}
					}
				}
			}
			if (owner.maxMinions <= ownerRealMinions)
			{
				if (owner.ownedProjectileCounts[minionProjType] <= 0)
				{
					return;
				}
				else
				{
					var queryMinions = Main.projectile.Where(x => x.type == minionProjType && x.active);
					int maxTotalTime = 0;
					if (queryMinions.Any())
					{
						EvilMusicRemnant_Minion eMRM_Kill = default;
						foreach (Projectile proj in queryMinions)
						{
							EvilMusicRemnant_Minion eMRM;
							eMRM = proj.ModProjectile as EvilMusicRemnant_Minion;
							if (eMRM != null && !eMRM.ExplosionCommand)
							{
								if (eMRM.TotalTime > maxTotalTime)
								{
									maxTotalTime = eMRM.TotalTime;
									eMRM_Kill = eMRM;
								}
							}
						}
						if (eMRM_Kill != null && eMRM_Kill != default)
						{
							eMRM_Kill.ExplosionCommand = true;
						}
					}
				}
			}
		}

		var index = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, minionProjType, Projectile.damage / 2, Projectile.knockBack, Projectile.owner, owner.ownedProjectileCounts[minionProjType] + 1);
		owner.AddBuff(ModContent.BuffType<Buffs.EvilMusicRemnant>(), 30);
	}

	public override void OnKill(int timeLeft)
	{
		for (int i = 0; i < 4; i++)
		{
			float size = Main.rand.NextFloat(0.1f, 0.96f);
			var noteFlame = new EvilMusicRemnant_FlameDust
			{
				Velocity = new Vector2(0, Main.rand.NextFloat(0.3f, 1f)).RotatedByRandom(MathHelper.TwoPi) * 0.8f,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(54, 86),
				Scale = 14f * size,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				Frame = Main.rand.Next(3),
				ai = [Main.rand.NextFloat(-0.8f, 0.8f)],
			};
			Ins.VFXManager.Add(noteFlame);
		}
	}
}
