using Everglow.Yggdrasil.YggdrasilTown.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class JellyBallGrappleTests
{
	[TestInitialize]
	public void Initialize() => Program.SavePath = string.Empty;

	[TestMethod]
	public void ReleasingSideGrapplePreservesJumpAndNextFreeImpactStillBounces()
	{
		var player = new Player { whoAmI = Main.myPlayer };
		var jellyPlayer = (JellyBallSecretionPlayer)new JellyBallSecretionPlayer().NewInstance(player);
		var jelly = new JellyBallSecretion();
		var tileInstance = typeof(ContentInstance<JellyBallSecretion>).GetProperty("Instance")!;
		var rippleInstance = typeof(ContentInstance<JellyRipples>).GetProperty("Instance")!;
		var previousTile = tileInstance.GetValue(null);
		var previousRipples = rippleInstance.GetValue(null);
		bool wasDedicated = Main.dedServ;
		var tile = Main.tile[20, 20];
		var previousType = tile.TileType;
		var previousState = tile.Get<TileWallWireStateData>();
		try
		{
			Main.dedServ = false;
			tileInstance.SetValue(null, jelly);
			rippleInstance.SetValue(null, new JellyRipples());
			tile.Get<TileWallWireStateData>() = default;
			tile.TileType = jelly.Type;
			tile.HasTile = true;

			player.grappling[0] = 0;
			player.grapCount = 1;
			foreach (float expectedX in new[] { 0f, -9.9f })
			{
				player.position = new Vector2(320 - player.width, 320);
				jellyPlayer.PreUpdate();
				// Vanilla clears the hook on jump before PreUpdateMovement.
				player.grappling[0] = -1;
				player.grapCount = 0;
				player.velocity = new Vector2(11f, -5f);
				jellyPlayer.PreUpdateMovement();
				player.velocity.X = 0f;
				player.position += player.velocity;
				jellyPlayer.PostUpdate();
				Assert.AreEqual(expectedX, player.velocity.X, 0.001f);
				Assert.AreEqual(-5f, player.velocity.Y);
			}
		}
		finally
		{
			tile.Get<TileWallWireStateData>() = previousState;
			tile.TileType = previousType;
			tileInstance.SetValue(null, previousTile);
			rippleInstance.SetValue(null, previousRipples);
			Main.dedServ = wasDedicated;
		}
	}
}
