using Everglow.Commons.Templates.Weapons.Whips;
using Everglow.Yggdrasil.KelpCurtain.Buffs;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.VFXs;
using Terraria.GameContent;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Summon;

public class RedAlgaeMagicWhip_Proj : WhipProjectile
{
	public override void SetDef()
	{
		WhipLength = 420;
		DustType = ModContent.DustType<YggdrasilCyatheaLeafDust>();
	}

	public override void GenerateDusts()
	{
		Player player = Main.player[Projectile.owner];
		float t = Projectile.ai[0] / TimeToFlyOut;
		if (t > 0.4f && t < 0.9f)
		{
			float times = 3 * player.meleeSpeed;
			if (times < 0)
			{
				times = 0;
			}
			if (WhipPointsForCollision.Count > 10)
			{
				for (int x = 0; x < times; x++)
				{
					int randSegment = WhipPointsForCollision.Count - 1;
					Vector2 spinningpoint = WhipPointsForCollision[randSegment] - WhipPointsForCollision[randSegment - 1];

					var redAlgaeDust = new RedAlgae_Spark();
					redAlgaeDust.Position = WhipPointsForCollision[randSegment];
					redAlgaeDust.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
					redAlgaeDust.Velocity = Vector2.Zero;
					redAlgaeDust.ai = new float[] { 0.9f };
					redAlgaeDust.MaxTime = 30;
					redAlgaeDust.Scale = Main.rand.NextFloat(1f, 2f);
					redAlgaeDust.Visible = true;
					redAlgaeDust.Active = true;
					Ins.VFXManager.Add(redAlgaeDust);
				}

				if (Main.rand.NextBool(2))
				{
					int randSegment = Main.rand.Next(WhipPointsForCollision.Count - 10, WhipPointsForCollision.Count);
					Vector2 spinningpoint = WhipPointsForCollision[randSegment] - WhipPointsForCollision[randSegment - 1];

					if (!Main.rand.NextBool(6))
					{
						var redAlgaeDust = new RedAlgae_Small_Dust();
						redAlgaeDust.Position = WhipPointsForCollision[randSegment];
						redAlgaeDust.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
						redAlgaeDust.Velocity = spinningpoint.RotatedBy((double)(player.direction * MathHelper.PiOver2), default);
						redAlgaeDust.ai = new float[] { 0.9f };
						redAlgaeDust.MaxTime = 60;
						redAlgaeDust.Scale = Main.rand.NextFloat(1f, 2f);
						redAlgaeDust.Visible = true;
						redAlgaeDust.Active = true;
						redAlgaeDust.Frame = Main.rand.Next(10);
						Ins.VFXManager.Add(redAlgaeDust);
					}
					else
					{
						var redAlgaeDust = new RedAlgaeDust();
						redAlgaeDust.Position = WhipPointsForCollision[randSegment];
						redAlgaeDust.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
						redAlgaeDust.Velocity = spinningpoint.RotatedBy((double)(player.direction * MathHelper.PiOver2), default) * 0.5f;
						redAlgaeDust.ai = new float[] { Main.rand.NextFloat(-0.1f, 0.1f) };
						redAlgaeDust.MaxTime = 60;
						redAlgaeDust.MaxScale = Main.rand.NextFloat(0.3f, 0.6f);
						redAlgaeDust.Visible = true;
						redAlgaeDust.Active = true;
						redAlgaeDust.Frame = Main.rand.Next(4);
						Ins.VFXManager.Add(redAlgaeDust);
					}
				}
			}
		}
	}

	public override void DrawWhip(float foreStep = 0)
	{
		Texture2D mainTexture = TextureAssets.Projectile[Projectile.type].Value;
		int frameHeight = mainTexture.Height / VerticalFrameCount;

		var list0 = new List<Vector2>();
		FillWhipControlPoints(list0, foreStep);
		for (int i = 0; i < list0.Count - 1; i++)
		{
			int frame = TileUtils.GetFixedRandomNumber_SingleSeed(i, 3) + 1;
			if (frame == 0 && i > 0)
			{
				frame = 1;
			}
			if (frame != 0 && i == 0)
			{
				frame = 0;
			}
			if (i == list0.Count - 2)
			{
				frame = 4;
			}
			var rectangle = new Rectangle(0, frameHeight * frame, mainTexture.Width, frameHeight);
			var origin = new Vector2(rectangle.Width / 2, 2f);
			Vector2 positionNow = list0[i];
			Vector2 positionAdd = list0[i + 1] - positionNow;
			float rotation = positionAdd.ToRotation() - MathHelper.PiOver2;
			Color color = Lighting.GetColor(positionNow.ToTileCoordinates());
			Color glowColor = new Color(1f, 1f, 1f, 0);
			if (foreStep != 0)
			{
				color *= (1 - foreStep / 3f) * 0.2f;
				glowColor *= (1 - foreStep / 3f) * 0.2f;
			}
			var scale = new Vector2(1f, (positionAdd.Length() + 2f) / rectangle.Height * 2f);
			Main.spriteBatch.Draw(mainTexture, list0[i] - Main.screenPosition, new Rectangle?(rectangle), color, rotation, origin, scale, SpriteEffects.None, 0f);
			if (frame == 4)
			{
				Main.spriteBatch.Draw(mainTexture, list0[i] - Main.screenPosition, new Rectangle?(rectangle), glowColor, rotation, origin, scale, SpriteEffects.None, 0f);
			}
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		int type = ModContent.BuffType<RedAlgae_FriendlyDebuff>();
		if (!target.HasBuff(type))
		{
			target.AddBuff(type, 900);
		}
	}
}
