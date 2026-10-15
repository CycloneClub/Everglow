using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Miscs;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.RoomScenes;

public class DarkDragon_Scene_DustManager : TwilightCastle_RoomScene_OverTiles
{
	public int Timer = 0;

	public override void Update()
	{
		base.Update();
		Timer++;
		int direction = 1;
		if (FlipHorizontally(OriginTilePos.X, OriginTilePos.Y))
		{
			direction = -1;
		}
		Vector2 p0 = new Vector2(21 * direction, 26);
		Vector2 p1 = new Vector2(396 * direction, 26);
		if (Main.rand.NextBool(6))
		{
			float value = Main.rand.NextFloat();
			Vector2 pos = p0 * value + p1 * (1 - value) + Position;
			Dust dust = Dust.NewDustPerfect(Position, ModContent.DustType<GenerateSplash>(), Vector2.zeroVector);
			var dark = new DarkDragon_Scene_Dust()
			{
				Position = pos,
				Velocity = new Vector2(Main.rand.NextFloat(2, 2.5f) * direction, Main.rand.NextFloat(1, 1.5f)),
				Fade = 1,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				Timer = 0,
				MaxTime = 300,
				MaxPosY = OriginTilePos.Y * 16 + 312,
				Frame = new Rectangle(Main.rand.Next(4) * 10, 10 + Main.rand.Next(3) * 10, 10, 10),
				Scale = Main.rand.NextFloat(1.45f, 3.6f),
				Active = true,
				Visible = true,
			};
			dust.customData = dark;
		}
		if (Timer % 150 == 0)
		{
			float value = Main.rand.NextFloat();
			Vector2 pos = p0 * value + p1 * (1 - value) + Position;
			Projectile.NewProjectileDirect(WorldGen.GetProjectileSource_PlayerOrWires(OriginTilePos.X, OriginTilePos.Y, false, Main.LocalPlayer), pos, new Vector2(2 * direction, 2), ModContent.ProjectileType<ShadowDragonAttack>(), 30, 2, Main.myPlayer);
		}
	}
}
