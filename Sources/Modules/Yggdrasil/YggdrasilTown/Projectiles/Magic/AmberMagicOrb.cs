using Everglow.Commons.Templates.Weapons;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class AmberMagicOrb : HandholdProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetDef()
	{
		DepartLength = 60;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 20;
		TextureRotation = 5 / 18f * MathHelper.Pi;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 5;
		Projectile.ArmorPenetration = 45;
		base.SetDef();
	}

	public override void OnSpawn(IEntitySource source)
	{
		Player player = Main.player[Projectile.owner];
		Vector2 mouseToPlayer = Main.MouseWorld - player.Center;
		mouseToPlayer = Vector2.Normalize(mouseToPlayer);
		Projectile.rotation = mouseToPlayer.ToRotation() + MathHelper.PiOver4;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		base.OnHitNPC(target, hit, damageDone);
	}

	public override void AI()
	{
		HeldProjectileAI();
	}

	public float Power = 0f;

	public override void HeldProjectileAI()
	{
		Player player = Main.player[Projectile.owner];
		if (player.HeldItem.type != ModContent.ItemType<Items.Weapons.AmberMagicOrb>())
		{
			Projectile.Kill();
		}
		player.heldProj = Projectile.whoAmI;
		ArmRootPos = player.MountedCenter + new Vector2(-4 * player.direction, -2);

		Vector2 mouseToPlayer = Main.MouseWorld - ArmRootPos;
		mouseToPlayer = Vector2.Normalize(mouseToPlayer);
		Projectile.rotation = mouseToPlayer.ToRotation() + MathHelper.PiOver4;

		player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (Projectile.rotation - MathF.PI * 0.25f) * player.gravDir - MathF.PI * 0.5f);
		DepartLength = Power * player.itemAnimation / player.itemAnimationMax;

		Projectile.Center = ArmRootPos + new Vector2(0, 1).RotatedBy(Projectile.rotation - Math.PI * 0.75) * DepartLength;
		if (player.controlUseItem)
		{
			Projectile.timeLeft = 10;
			Power = (float)Utils.Lerp(Power, 60, 0.5f);
		}
		else
		{
			Power = (float)Utils.Lerp(Power, 0, 0.5f);
		}
		if (Projectile.Center.X < ArmRootPos.X)
		{
			player.direction = -1;
		}
		else
		{
			player.direction = 1;
		}
	}

	public void ShootProj(Vector2 velocity)
	{
		for (int x = 0; x < 3; x++)
		{
			Vector2 v0 = velocity.RotatedBy(Main.rand.NextFloat(-0.35f, 0.35f)) * Main.rand.NextFloat(0.75f, 1.25f);
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, v0, ModContent.ProjectileType<AmberBall>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Player player = Main.player[Projectile.owner];
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		var texGlow = ModAsset.AmberMagicOrb_glow.Value;
		SpriteEffects se = SpriteEffects.None;
		if (player.direction == -1)
		{
			se = SpriteEffects.FlipVertically;
		}

		float rot = Projectile.rotation - (float)(Math.PI * 0.25) + TextureRotation * player.direction;
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition + DrawOffset, null, lightColor, rot + (float)Main.time * 0.25f, texMain.Size() / 2f, 1f, se, 0);
		Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition + DrawOffset, null, new Color(1f, 1f, 1f, 0), rot + (float)Main.time * 0.25f, texGlow.Size() / 2f, 1f, se, 0);

		var bars = new List<Vertex2D>();
		for (int t = 0; t < 3; t++)
		{
			Vector2 v0 = Projectile.Center + new Vector2(0, -5 * player.direction).RotatedBy((float)Main.time * 0.25f + 1.5f);
			Vector2 v1 = new Vector2(0, -2).RotatedBy(t / 3d * MathHelper.TwoPi + Projectile.rotation);
			var drawC = new Color(1f, 0.5f, 0f, 0);
			for (int i = 0; i < 150; i++)
			{
				float factor = i / 85f;
				float timeValue = (float)Main.time * 0.006f;

				Vector2 drawPos = v0;
				Vector2 toPlayer = ArmRootPos - v0 - v1;
				float mulColor = 1f;
				if (toPlayer.Length() < 30)
				{
					mulColor = (toPlayer.Length() - 8f) / 30f;
					mulColor = MathF.Max(0, mulColor);
				}
				float width = MathF.Sin(Math.Min(i / 25f, 0.5f) * MathHelper.Pi);
				Vector2 v2 = Vector2.Normalize(v1) * 6;
				if (i == 0)
				{
					bars.Add(new Vertex2D(drawPos + v2.RotatedBy(MathHelper.PiOver2) * 1.3f, drawC * 0, new Vector3(-factor * 2 + timeValue + t / 3f, 0.8f, width)));
					bars.Add(new Vertex2D(drawPos - v2.RotatedBy(MathHelper.PiOver2) * 1.3f, drawC * 0, new Vector3(-factor * 2 + timeValue + t / 3f, 0.2f, width)));
				}
				bars.Add(new Vertex2D(drawPos + v2.RotatedBy(MathHelper.PiOver2) * 1.3f, drawC * mulColor, new Vector3(-factor * 2 + timeValue + t / 3f, 0.8f, width)));
				bars.Add(new Vertex2D(drawPos - v2.RotatedBy(MathHelper.PiOver2) * 1.3f, drawC * mulColor, new Vector3(-factor * 2 + timeValue + t / 3f, 0.2f, width)));
				v0 += v1;
				if (toPlayer.Length() < 8)
				{
					bars.Add(new Vertex2D(drawPos + v2.RotatedBy(MathHelper.PiOver2) * 1.3f, drawC * 0, new Vector3(-factor * 2 + timeValue + t / 3f, 0.8f, width)));
					bars.Add(new Vertex2D(drawPos - v2.RotatedBy(MathHelper.PiOver2) * 1.3f, drawC * 0, new Vector3(-factor * 2 + timeValue + t / 3f, 0.2f, width)));
					break;
				}
				else
				{
					v1 *= 0.95f;
					v1 += Vector2.Normalize(toPlayer) * 0.5f;
					v1 = Vector2.Normalize(v1) * 2;
				}
			}
		}

		Effect effect = Commons.ModAsset.Trailing.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.CurrentTechnique.Passes[0].Apply();
		Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_2_thick.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		return false;
	}
}
