using System.Reflection;
using System.Runtime.CompilerServices;
using Everglow.Yggdrasil;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.ID;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class YggdrasilTownBiomeTests
{
	private static readonly FieldInfo CurrentWorld = typeof(SubworldSystem).GetField("current", BindingFlags.Static | BindingFlags.NonPublic)!;
	private object? oldWorld;
	private Vector2 oldCenter;
	private Vector2 oldScreenPosition;
	private bool oldCheckedCenter;
	private bool oldDedServ;
	private int oldMode;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		oldWorld = CurrentWorld.GetValue(null);
		oldCenter = YggdrasilTownBiome.BiomeCenter;
		oldCheckedCenter = YggdrasilTownBiome.CheckedBiomeCenter;
		oldScreenPosition = Main.screenPosition;
		oldDedServ = Main.dedServ;
		oldMode = Main.netMode;
		CurrentWorld.SetValue(null, new YggdrasilWorld());
		YggdrasilTownBiome.BiomeCenter = new Vector2(8000, 320000);
		YggdrasilTownBiome.CheckedBiomeCenter = true;
	}

	[TestCleanup]
	public void Cleanup()
	{
		CurrentWorld.SetValue(null, oldWorld);
		YggdrasilTownBiome.BiomeCenter = oldCenter;
		YggdrasilTownBiome.CheckedBiomeCenter = oldCheckedCenter;
		Main.screenPosition = oldScreenPosition;
		Main.dedServ = oldDedServ;
		Main.netMode = oldMode;
	}

	[TestMethod]
	[DataRow(NetmodeID.SinglePlayer)]
	[DataRow(NetmodeID.MultiplayerClient)]
	[DataRow(NetmodeID.Server)]
	public void BiomeUsesEachPlayersCenterRegardlessOfCamera(int netMode)
	{
		Main.netMode = netMode;
		Main.dedServ = netMode == NetmodeID.Server;
		var biome = new YggdrasilTownBiome();
		var upperPlayer = (Player)RuntimeHelpers.GetUninitializedObject(typeof(Player));
		var lowerPlayer = (Player)RuntimeHelpers.GetUninitializedObject(typeof(Player));
		float boundary = YggdrasilTownBiome.BiomeCenter.Y - 18000;
		upperPlayer.Center = new Vector2(160, boundary - 1);
		lowerPlayer.Center = new Vector2(160, boundary + 1);
		foreach (float cameraY in new[] { 0f, boundary + 1000 })
		{
			Main.screenPosition = new Vector2(0, cameraY);
			Assert.IsFalse(biome.IsBiomeActive(upperPlayer));
			Assert.IsTrue(biome.IsBiomeActive(lowerPlayer));
			Assert.IsFalse(YggdrasilTownBiome.IsTownLayer(upperPlayer.Center));
			Assert.IsTrue(YggdrasilTownBiome.IsTownLayer(lowerPlayer.Center));
		}
		lowerPlayer.Center = new Vector2(160, boundary);
		Assert.IsFalse(biome.IsBiomeActive(lowerPlayer), "The existing strict lower-bound comparison is preserved.");
	}

	[TestMethod]
	public void MainWorldDoesNotUseTownLayerCoordinates()
	{
		CurrentWorld.SetValue(null, null);
		Main.screenPosition = new Vector2(0, 400000);
		var player = (Player)RuntimeHelpers.GetUninitializedObject(typeof(Player));
		player.Center = new Vector2(160, 400000);
		Assert.IsFalse(new YggdrasilTownBiome().IsBiomeActive(player));
		Assert.IsFalse(YggdrasilTownBiome.IsTownLayer(player.Center));
	}
}
