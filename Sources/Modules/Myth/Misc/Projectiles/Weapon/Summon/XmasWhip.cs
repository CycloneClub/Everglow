using Everglow.Commons.Templates.Weapons.Whips;
using Everglow.Myth.Misc.Dusts;
using Terraria.GameContent;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Summon;

public class XmasWhip : WhipProjectile
{
	public override void SetDef()
	{
		SegmentCount = 14;
		WhipLength = 420;
		DustType = ModContent.DustType<PinePin>();
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return base.PreDraw(ref lightColor);
	}

	public override void DrawWhip(float foreStep = 0)
	{
		var list = new List<Vector2>();
		FillWhipControlPoints(list, foreStep);
		Texture2D mainTexture = TextureAssets.Projectile[Projectile.type].Value;
		int frameHeight = 30;
		for (int i = 0; i < list.Count - 1; i++)
		{
			int frame = 1;
			if (i == 0)
			{
				frame = 0;
			}
			Rectangle rectangle = new Rectangle(0, frameHeight * frame, 24, frameHeight);
			var origin = new Vector2(rectangle.Width / 2, 2f);
			Vector2 positionNow = list[i];
			Vector2 positionAdd = list[i + 1] - positionNow;
			float rotation = positionAdd.ToRotation() - MathHelper.PiOver2;
			Color color = Lighting.GetColor(positionNow.ToTileCoordinates());
			if (foreStep != 0)
			{
				color *= (1 - foreStep / 3f) * 0.2f;
			}
			var scale = new Vector2(1f, (positionAdd.Length() + 2f) / rectangle.Height * 2f);
			Main.spriteBatch.Draw(mainTexture, list[i] - Main.screenPosition, new Rectangle?(rectangle), color, rotation, origin, scale, SpriteEffects.None, 0f);
			Color light = new Color(0.66f, 0.66f, 0.66f, 0);
			if (foreStep != 0)
			{
				light *= (1 - foreStep / 3f) * 0.2f;
			}
			if (i == list.Count - 2)
			{
				Main.spriteBatch.Draw(mainTexture, list[i] + positionAdd * 1.5f - Main.screenPosition, new Rectangle(2, 116, 22, 22), color, rotation, origin, 1.2f, SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(mainTexture, list[i] + positionAdd * 1.5f - Main.screenPosition, new Rectangle(56, 116, 22, 22), light * 2f, rotation, origin, 1.2f, SpriteEffects.None, 0f);
			}
			if (i % 5 == 0)
			{
				Main.spriteBatch.Draw(mainTexture, list[i] - Main.screenPosition, new Rectangle(24, 10, 24, 20), color, rotation, origin, 1f, SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(mainTexture, list[i] - Main.screenPosition, new Rectangle(48, 10, 24, 20), light, rotation, origin, 1f, SpriteEffects.None, 0f);
			}
			if (i % 5 == 1)
			{
				Main.spriteBatch.Draw(mainTexture, list[i] - Main.screenPosition, new Rectangle(24, 32, 24, 20), color, rotation, origin, 1f, SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(mainTexture, list[i] - Main.screenPosition, new Rectangle(48, 32, 24, 20), light, rotation, origin, 1f, SpriteEffects.None, 0f);
			}
			if (i % 5 == 2)
			{
				Main.spriteBatch.Draw(mainTexture, list[i] - Main.screenPosition, new Rectangle(24, 54, 24, 20), color, rotation, origin, 1f, SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(mainTexture, list[i] - Main.screenPosition, new Rectangle(48, 54, 24, 20), light, rotation, origin, 1f, SpriteEffects.None, 0f);
			}
			if (i % 5 == 3)
			{
				Main.spriteBatch.Draw(mainTexture, list[i] - Main.screenPosition, new Rectangle(24, 76, 24, 20), color, rotation, origin, 1f, SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(mainTexture, list[i] - Main.screenPosition, new Rectangle(48, 76, 24, 20), light, rotation, origin, 1f, SpriteEffects.None, 0f);
			}
			if (i % 5 == 4)
			{
				Main.spriteBatch.Draw(mainTexture, list[i] - Main.screenPosition, new Rectangle(24, 98, 24, 20), color, rotation, origin, 1f, SpriteEffects.None, 0f);
			}
			if (i % 2 == 0)
			{
				Main.spriteBatch.Draw(mainTexture, list[i] + positionAdd * 1.5f - Main.screenPosition, new Rectangle(24, 108, 24, 10), color, rotation, origin, new Vector2(1.5f, 1f), SpriteEffects.None, 0f);
			}
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (Projectile.ai[1] == 0)
		{
			Projectile.ai[1] += 1;
			Player player = Main.player[Projectile.owner];
			for (int af = 0; af < 35; af++)
			{
				Vector2 v0 = new Vector2(0, Main.rand.NextFloat(9f, 10f)).RotatedByRandom(MathHelper.TwoPi);
				var d = Dust.NewDustDirect(target.Center - new Vector2(4), 0, 0, ModContent.DustType<PinePin>(), v0.X, v0.Y, Alpha: 0, Scale: Main.rand.NextFloat(0.3f, 1.1f));
				d.noGravity = true;
			}
			for (int z = 0; z < 30; z++)
			{
				float rot = Main.rand.NextFloat(6.283f);
				if (Main.rand.NextBool(2))
				{
					Vector2 v = new Vector2(0, Main.rand.NextFloat(6f, 8f)).RotatedBy(Math.PI * z / 15 + rot);
					Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), target.Center + v * 7, v, 336, Projectile.damage, 0.2f, player.whoAmI, player.GetCritChance(DamageClass.Summon), (int)(Projectile.damage * 0.3));
				}
			}
		}
	}

	public override void GenerateDusts()
	{
		if (WhipPointsForCollision.Count > 10)
		{
			Player player = Main.player[Projectile.owner];
			float t = Projectile.ai[0] / TimeToFlyOut;
			if (Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true) > 0.5f)
			{
				float times = (Main.rand.NextFloat(3) - 1) * player.meleeSpeed;
				if (times < 0)
				{
					times = 0;
				}

				for (int x = 0; x < times; x++)
				{
					int randSegment = Main.rand.Next(WhipPointsForCollision.Count - 10, WhipPointsForCollision.Count);
					Rectangle rectangle = Utils.CenteredRectangle(WhipPointsForCollision[randSegment], new Vector2(30f, 30f));
					var dust = Dust.NewDustDirect(rectangle.TopLeft(), rectangle.Width, rectangle.Height, DustType, 0f, 0f, 100, Color.White, 1f);
					dust.position = WhipPointsForCollision[randSegment];
					dust.fadeIn = 0.3f;
					Vector2 spinningpoint = WhipPointsForCollision[randSegment] - WhipPointsForCollision[randSegment - 1];
					dust.noGravity = true;
					dust.velocity *= 0.5f;
					dust.velocity += spinningpoint.RotatedBy((double)(player.direction * 1.5707964f), default);
					dust.velocity *= 0.5f;
				}
				Dust.NewDustDirect(WhipPointsForCollision[WhipPointsForCollision.Count - 1], 0, 0, DustID.GoldCoin);
			}
		}
	}
}
