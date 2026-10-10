using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class EyeOfAnabiosis_Projectile : ModProjectile
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

	public override void SetStaticDefaults()
	{
		Main.projFrames[Type] = 4;
	}

	public override void SetDefaults()
	{
		Projectile.width = 14;
		Projectile.height = 26;
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 240;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.penetrate = 1;
		Projectile.friendly = true;
		Projectile.drawLayer = ProjectileDrawLayerID.OverPlayers;
		Projectile.usesOwnerLight = true;
	}

	private bool HasTarget => TargetWhoAmI >= 0;

	/// <summary>
	/// Choose target closest to mouse while spawn.
	/// </summary>
	/// <param name="source"></param>
	public override void OnSpawn(IEntitySource source)
	{
		targetWhoAmI = FindTarget(Main.MouseWorld);
	}

	public override void AI()
	{
		if (Projectile.timeLeft % 8 == 0)
		{
			Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
		}

		if (HasTarget)
		{
			if (!Target.active || Target.friendly || !Target.CanBeChasedBy())
			{
				return;
			}

			var targetPos = HasTarget ? Main.npc[targetWhoAmI].Center : Vector2.Zero;
			var targetVel = Vector2.Normalize(targetPos - Projectile.Center) * 10f;
			Projectile.velocity = (targetVel + Projectile.velocity * 10) / 11f;
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

	public override void OnKill(int timeLeft)
	{
		for (int i = 0; i < 15; i++)
		{
			float size = Main.rand.NextFloat(0.3f, 0.96f);
			var anabiosisFlame = new AnabiosisFlameDust
			{
				Velocity = new Vector2(0, Main.rand.NextFloat(3, 4f)).RotatedByRandom(MathHelper.TwoPi),
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(24, 36),
				Scale = 25f * size,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				Frame = Main.rand.Next(3),
				ai = new float[] { Projectile.Center.X, Main.rand.NextFloat(-0.8f, 0.8f) },
			};
			Ins.VFXManager.Add(anabiosisFlame);
		}
		SoundEngine.PlaySound(SoundID.DD2_BetsysWrathImpact);
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		var texture = ModContent.Request<Texture2D>(Texture).Value;
		var frame = texture.Frame(horizontalFrames: Main.projFrames[Type], frameX: Projectile.frame);
		var origin = new Vector2(texture.Width / 8, texture.Height / 2);
		var textureGlow = ModAsset.EyeOfAnabiosis_Projectile_glow.Value;
		Main.spriteBatch.Draw(textureGlow, Projectile.Center - Main.screenPosition, frame, new Color(0.5f, 0.5f, 0.5f, 0.5f), 0, origin, 1, SpriteEffects.None, 0);
		float glow = MathF.Sin((float)Main.time * 0.3f + Projectile.whoAmI) + 1;
		glow *= 0.5f;
		Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, new Color(glow, glow, glow, 0), 0, origin, 1, SpriteEffects.None, 0);
		return false;
	}


}
