using Everglow.Commons.Templates.Weapons;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class BronzeLotusLamp_Blossom : TrailingProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	private const int SearchDistance = 600;

	private int targetWhoAmI = -1;

	private int TargetWhoAmI
	{
		get => targetWhoAmI;
		set => targetWhoAmI = value;
	}

	private NPC Target => Main.npc[TargetWhoAmI];

	private bool HasTarget => TargetWhoAmI >= 0;

	public override void SetCustomDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.friendly = true;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = true;
		Projectile.timeLeft = 2400;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 30;

		TrailLength = 21;
		TrailColor = new Color(0, 1f, 0.96f, 0f);
		TrailWidth = 20f;
		SelfLuminous = true;
		TrailTexture = Commons.ModAsset.Trail_10.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_10_black.Value;
		TrailShader = Commons.ModAsset.Trailing.Value;
	}

	public override void OnSpawn(IEntitySource source)
	{
		targetWhoAmI = FindTarget(Projectile.Center);
	}

	public override void Behaviors()
	{
		Projectile.velocity.Y += 0.5f;
		Projectile.velocity = Projectile.velocity.RotatedBy(Math.Sin(Main.time * 0.16 + Projectile.whoAmI) * 0.03f);
		Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 8;
		Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

		float size = Main.rand.NextFloat(0.1f, 0.96f);
		var lotusFlame = new CyanLotusFlameDust
		{
			Velocity = new Vector2(0, Main.rand.NextFloat(0.3f, 1f)).RotatedByRandom(MathHelper.TwoPi),
			Active = true,
			Visible = true,
			Position = Projectile.Center,
			MaxTime = Main.rand.Next(24, 36),
			Scale = 14f * size,
			Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
			Frame = Main.rand.Next(3),
			ai = new float[] { Main.rand.NextFloat(-0.8f, 0.8f) },
		};
		Ins.VFXManager.Add(lotusFlame);
		if (HasTarget)
		{
			if (!Target.active || Target.friendly || !Target.CanBeChasedBy())
			{
				targetWhoAmI = -1;
				return;
			}
			else
			{
				var targetPos = HasTarget ? Main.npc[targetWhoAmI].Center : Vector2.Zero;
				var targetVel = Vector2.Normalize(targetPos - Projectile.Center) * 10f;
				Projectile.velocity = (targetVel + Projectile.velocity * 10) / 11f;
			}
		}
		else
		{
			// If there is no target after spawned, search target by projectile center.
			targetWhoAmI = FindTarget(Projectile.Center);
		}
	}

	/// <summary>
	/// Find closest target by given position.
	/// </summary>
	/// <param name="fromWhere"></param>
	/// <returns></returns>
	public int FindTarget(Vector2 fromWhere)
	{
		int target = -1;
		float minDis = SearchDistance;
		foreach (NPC npc in Main.npc)
		{
			if (npc != null && npc.active)
			{
				if (npc.CanBeChasedBy() && !npc.dontTakeDamage && npc.life > 0)
				{
					float dis = (npc.Center - fromWhere).Length() - npc.Hitbox.Size().Length() * 0.5f;
					if (dis < minDis)
					{
						minDis = dis;
						target = npc.whoAmI;
					}
				}
			}
		}
		return target;
	}

	public override void DestroyEntityEffect()
	{
		int n = 6;
		Vector2 shootSpeed = new Vector2(0, Main.rand.NextFloat(4, 8)).RotatedByRandom(MathHelper.TwoPi);
		for (int i = 0; i < n; i++)
		{
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, shootSpeed.RotatedBy(i / 6d * MathHelper.TwoPi) * 0.5f, ModContent.ProjectileType<BronzeLotusLamp_SubBlossom>(), 0, 0, Projectile.owner, 0);
			for (int j = 0; j < 6; j++)
			{
				var lotusFlame = new CyanLotusFlameDust
				{
					Velocity = shootSpeed.RotatedBy((i + 0.5f) / 6d * MathHelper.TwoPi) * (j + 6) * 0.05f,
					Active = true,
					Visible = true,
					Position = Projectile.Center,
					MaxTime = Main.rand.Next(30, 34) + j * 8,
					Scale = 8f,
					Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
					Frame = Main.rand.Next(3),
					ai = new float[] { Main.rand.NextFloat(-0.8f, 0.8f) },
				};
				Ins.VFXManager.Add(lotusFlame);
			}
		}
		Projectile.height = Projectile.width = (int)(15 * shootSpeed.Length());
	}

	public override void DrawSelf()
	{
		Texture2D texture = ModAsset.BronzeLotusLamp_Blossom.Value;
		Vector2 drawCenter = Projectile.Center - Main.screenPosition;
		Main.EntitySpriteDraw(texture, drawCenter, null, new Color(1f, 1f, 1f, 0f), Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
	}

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0) => base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);

	public override Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue) => base.ModifyTrailTextureCoordinate(factor, timeValue, phase, widthValue);
}
