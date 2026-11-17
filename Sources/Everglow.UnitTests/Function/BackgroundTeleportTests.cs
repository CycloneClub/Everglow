using Everglow.Commons.Mechanics.CustomBackground;
using Microsoft.Xna.Framework;
using Terraria;

namespace Everglow.UnitTests.Function;

[TestClass]
[DoNotParallelize]
public class BackgroundTeleportTests
{
	private Player[] savedPlayers = null!;
	private int savedPlayerIndex;
	private bool savedMenu;
	private bool savedServer;
	private BackgroundSystem system = null!;
	private TestBackground slide = null!;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		savedPlayers = Main.player;
		savedPlayerIndex = Main.myPlayer;
		savedMenu = Main.gameMenu;
		savedServer = Main.dedServ;
		Main.player = [new Player { active = true, position = new Vector2(30000, 30000) }];
		Main.myPlayer = 0;
		Main.gameMenu = false;
		Main.dedServ = false;
		system = new BackgroundSystem();
		system.Load();
		slide = new TestBackground();
		system.AddBackgroundSlide(slide);
	}

	[TestCleanup]
	public void Cleanup()
	{
		Main.player = savedPlayers;
		Main.myPlayer = savedPlayerIndex;
		Main.gameMenu = savedMenu;
		Main.dedServ = savedServer;
	}

	[TestMethod]
	public void FirstSampleAndNormalMovementPreserveBackgrounds()
	{
		system.PreUpdateEntities();
		Assert.IsTrue(system.HasBgSlide(slide.UniqueName));
		for (int i = 0; i < 100; i++)
		{
			Main.LocalPlayer.position += new Vector2(20, -20);
			system.PreUpdateEntities();
		}
		Assert.IsTrue(system.HasBgSlide(slide.UniqueName));
	}

	[TestMethod]
	[DataRow(1024f, 0f, false)]
	[DataRow(1025f, 0f, true)]
	[DataRow(0f, -1025f, true)]
	[DataRow(800f, 800f, true)]
	public void TeleportClearsOldAnchorsAndAllowsImmediateRecreation(float x, float y, bool reset)
	{
		system.PreUpdateEntities();
		Main.LocalPlayer.position += new Vector2(x, y);
		system.PreUpdateEntities();
		Assert.AreEqual(!reset, system.HasBgSlide(slide.UniqueName));
		Assert.AreEqual(reset, system.AddBackgroundSlide(new TestBackground()));
		system.PreUpdateEntities();
		Assert.IsTrue(system.HasBgSlide(slide.UniqueName));
	}

	[TestMethod]
	public void WorldUnloadResetsPositionHistory()
	{
		system.PreUpdateEntities();
		system.OnWorldUnload();
		system.AddBackgroundSlide(new TestBackground());
		Main.LocalPlayer.position = Vector2.Zero;
		system.PreUpdateEntities();
		Assert.IsTrue(system.HasBgSlide(slide.UniqueName));
	}

	[TestMethod]
	public void DedicatedServerDoesNotAccessLocalPlayer()
	{
		Main.dedServ = true;
		Main.player = [];
		system.PreUpdateEntities();
		Assert.IsTrue(system.HasBgSlide(slide.UniqueName));
	}

	private sealed class TestBackground : BackgroundSlideBase
	{
		public override void SetDefaults()
		{
		}
	}
}
