using Everglow.Commons.DataStructures;
using Everglow.Commons.Skeleton2D.Renderer;
using Everglow.Commons.Skeleton2D;
using Microsoft.Xna.Framework;
using Terraria.IO;
using Everglow.Commons.Coroutines;
using Everglow.Commons.Skeleton2D.Reader;
using Terraria.DataStructures;
using Terraria.Audio;
using Everglow.Commons.Skeleton2D.Renderer.DrawCommands;
using Microsoft.Xna.Framework.Graphics;
using Terraria.WorldBuilding;
using Spine;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Summon.FaelanternProj;

public class Faelanternbranch : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 256;
		Projectile.height = 148;
		Projectile.netImportant = true;
		Projectile.friendly = true;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 45;
		Projectile.penetrate = -1;
		Projectile.DamageType = DamageClass.Summon;
	}

	public void Suicide()
	{
		int tileX = (int)Projectile.Center.X / 16;
		int tileY = (int)Projectile.Center.Y / 16;
		if (Findtile(tileX + 3, tileY + 4) || Findtile(tileX + 4, tileY + 4) || Findtile(tileX + 5, tileY + 4) || Findtile(tileX + 6, tileY + 4))
		{
			Projectile.Kill();
		}
	}

	public bool Findtile(int tileX, int tileY)
	{
		return Main.tile[tileX, tileY] == null || !WorldGen.SolidTile2(tileX, tileY);
	}

	public override bool? CanDamage()
	{
		if (Projectile.timeLeft >= 35)
		{
			return false;
		}
		return true;
	}

	private int direction;

	public override void OnSpawn(IEntitySource source)
	{
		direction = (int)Projectile.ai[0];
	}

	public override void AI()
	{
		Projectile.velocity = Vector2.zeroVector;
		var size = new Vector2(Projectile.width * direction, Projectile.height);
		if (Projectile.timeLeft < 40 && Projectile.timeLeft > 35)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 5f)).RotatedByRandom(MathHelper.TwoPi);
			var somg = new RockSmogDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + size * 0.5f + new Vector2(Projectile.width * Main.rand.NextFloat(0f, -0.2f), Projectile.height * Main.rand.NextFloat(0f, -0.2f)),
				MaxTime = Main.rand.Next(25, 32),
				Scale = Main.rand.NextFloat(50f, 100f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = [Main.rand.NextFloat(0.0f, 0.93f), 0],
			};
			Ins.VFXManager.Add(somg);
		}
		if (Projectile.timeLeft <= 35 && Projectile.timeLeft >= 27)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 10f)).RotatedBy(size.ToRotation() + MathHelper.PiOver2 + Main.rand.NextFloat(-0.25f, 0.25f));
			var somg = new RockSmogDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + size * 0.5f - size.RotatedByRandom(0.25f) * Main.rand.NextFloat(0f, 0.75f),
				MaxTime = Main.rand.Next(25, 32),
				Scale = Main.rand.NextFloat(50f, 100f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = [Main.rand.NextFloat(0.0f, 0.93f), 0],
			};
			Ins.VFXManager.Add(somg);
		}
		Suicide();
		Lighting.AddLight(Projectile.Center, 0.1f, 0.5f, 0.5f);
		return;
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		var size = new Vector2(Projectile.width, Projectile.height);
		Vector2 pos = Projectile.position - Main.screenPosition;
		float factor = MathHelper.Clamp((45f - Projectile.timeLeft) / 10f, 0, 1);
		Main.spriteBatch.Draw(ModAsset.Faelanternbranchpit.Value, pos + (direction == 1 ? size : new Vector2(size.X, size.Y)), new Rectangle(0, 0, Projectile.width, Projectile.height), lightColor * factor, 0f, size, 1f, direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
		if (Projectile.timeLeft < 35f)
		{
			float scale = MathHelper.Clamp((35 - Projectile.timeLeft) / 5f, 0, 1);

			Main.spriteBatch.Draw(ModAsset.Faelanternbranch.Value, pos + (direction == 1 ? size : new Vector2(size.X * scale, size.Y)), new Rectangle(0, 0, Projectile.width, Projectile.height), lightColor, 0f, size, scale, direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
			Main.spriteBatch.Draw(ModAsset.Faelanternbranchglow.Value, pos + (direction == 1 ? size : new Vector2(size.X * scale, size.Y)), new Rectangle(0, 0, Projectile.width, Projectile.height), Color.White, 0f, size, scale, direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
		}
		return false;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		var size = new Vector2(Projectile.width, Projectile.height);
		float point = 0;
		Vector2 p1 = direction == 1 ? Projectile.position + size : Projectile.position + new Vector2(0, size.Y);
		Vector2 p2 = direction == 1 ? Projectile.position : Projectile.position + new Vector2(size.X, 0);
		if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), p1, p2, 50f, ref point))
		{
			return true;
		}

		return false;
	}
}
