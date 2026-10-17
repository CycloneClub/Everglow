using System.Reflection;
using Everglow.Yggdrasil.YggdrasilTown.Items.Tools.Developer;
using Everglow.Yggdrasil.YggdrasilTown.Travelers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class TravelerPanelInputTests
{
	private readonly List<Action> restore = [];

	private void SetField(Type type, string name, object value)
	{
		var field = type.GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!;
		var old = field.GetValue(null);
		restore.Add(() => field.SetValue(null, old));
		field.SetValue(null, value);
	}

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		SetField(typeof(Main), "_uiScaleUsed", 1.25f);
		SetField(typeof(Main), "_uiScaleMatrix", Matrix.CreateScale(1.25f, 1.25f, 1));
		SetField(typeof(PlayerInput), "_originalScreenWidth", 1000);
		SetField(typeof(PlayerInput), "_originalScreenHeight", 750);
		SetField(typeof(Main), "screenWidth", 800);
		SetField(typeof(Main), "screenHeight", 600);
		SetField(typeof(Main), "mouseX", 448);
		SetField(typeof(Main), "mouseY", 204);
		SetField(typeof(PlayerInput), "MouseInfo", new MouseState(560, 255, 0, ButtonState.Pressed,
			ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released));
		SetField(typeof(Main), "mouseLeft", true);
		SetField(typeof(Main), "mouseLeftRelease", false);
		SetField(typeof(Main), "gameMenu", false);
		SetField(typeof(Main), "hasFocus", true);
		SetField(typeof(Main), "gamePaused", false);
		SetField(typeof(Main), "blockInput", false);
		SetField(typeof(Main), "playerInventory", false);
		SetField(typeof(Main), "dedServ", false);
		SetField(typeof(Main), "myPlayer", 0);
		var player = new Player();
		player.inventory[0] = new Item { type = 500, stack = 1 };
		SetField(typeof(Main), "player", new[] { player });
		var controller = (VFXPersonItem)new VFXPersonItem().NewInstance(new Item { type = 500 });
		var instance = typeof(ContentInstance<VFXPersonItem>).GetProperty(nameof(ContentInstance<VFXPersonItem>.Instance))!;
		var oldItem = instance.GetValue(null);
		restore.Add(() => instance.SetValue(null, oldItem));
		instance.SetValue(null, controller);
		var settings = typeof(TownTravelerSystem).GetProperty(nameof(TownTravelerSystem.Options))!;
		var oldSettings = settings.GetValue(null);
		restore.Add(() => settings.SetValue(null, oldSettings));
		settings.SetValue(null, new TravelerOptions());
		new TravelerControlPanel().OnWorldLoad();
	}

	[TestCleanup]
	public void Cleanup()
	{
		new TravelerControlPanel().OnWorldUnload();
		foreach (var action in restore.AsEnumerable().Reverse())
		{
			action();
		}
		restore.Clear();
	}

	[TestMethod]
	public void UpdateUIUsesUnscaledMouseAndItsOwnPressEdge()
	{
		new TravelerControlPanel().UpdateUI(new GameTime());
		Assert.AreEqual(30, TownTravelerSystem.Options.SpawnIntervalTicks, "The Busy button must work when Terraria has already scaled Main mouse coordinates and cleared the draw-phase release flag.");
		Assert.AreEqual(48, TownTravelerSystem.Options.PopulationLimit(TravelerMode.NPC));
	}

	[TestMethod]
	public void MouseCoordinatesStayTheSameAcrossTerrariaZoomPhases()
	{
		var mouse = typeof(TravelerControlPanel).GetProperty("Mouse", BindingFlags.Static | BindingFlags.NonPublic)!;
		var inUI = (Vector2)mouse.GetValue(null)!;
		Main.mouseX = 560;
		Main.mouseY = 255;
		Main.screenWidth = 1000;
		Main.screenHeight = 750;
		var unscaled = (Vector2)mouse.GetValue(null)!;
		Assert.AreEqual(inUI, unscaled);
		Assert.AreEqual(new Vector2(448, 204), unscaled);
	}

	[TestMethod]
	public void PanelPressCannotLeakIntoItemUseUntilReleasedOutside()
	{
		var panel = new TravelerControlPanel();
		var controller = new VFXPersonItem();
		panel.UpdateUI(new GameTime());
		Assert.IsFalse(controller.CanUseItem(Main.LocalPlayer));
		PlayerInput.MouseInfo = new MouseState(950, 700, 0, ButtonState.Pressed,
			ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);
		Main.LocalPlayer.mouseInterface = false;
		Assert.IsFalse(controller.CanUseItem(Main.LocalPlayer), "A panel press held outside must not toggle the world item.");
		Main.mouseLeft = false;
		panel.UpdateUI(new GameTime());
		Main.LocalPlayer.mouseInterface = false;
		Assert.IsTrue(controller.CanUseItem(Main.LocalPlayer), "Releasing outside must restore item interaction.");
	}

	[TestMethod]
	public void HiddenPanelDoesNotBlockWorldItemButOtherInterfacesStillDo()
	{
		var controller = new VFXPersonItem();
		Assert.IsFalse(controller.CanUseItem(Main.LocalPlayer));
		TravelerControlPanel.Toggle();
		Assert.IsTrue(controller.CanUseItem(Main.LocalPlayer));
		Main.LocalPlayer.mouseInterface = true;
		Assert.IsFalse(controller.CanUseItem(Main.LocalPlayer));
		Main.LocalPlayer.mouseInterface = false;
		Main.playerInventory = true;
		Assert.IsFalse(controller.CanUseItem(Main.LocalPlayer));
	}

	[TestMethod]
	public void HoldingTitleMovesPanelWithoutChangingPopulation()
	{
		var panel = new TravelerControlPanel();
		PlayerInput.MouseInfo = new MouseState(125, 160, 0, ButtonState.Pressed,
			ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);
		panel.UpdateUI(new GameTime());
		PlayerInput.MouseInfo = new MouseState(250, 285, 0, ButtonState.Pressed,
			ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);
		panel.UpdateUI(new GameTime());
		var position = (Vector2)typeof(TravelerControlPanel).GetField("position", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		Assert.AreEqual(new Vector2(124, 110), position, "The dragged panel must remain entirely inside the UI viewport.");
		Assert.AreEqual(24, TownTravelerSystem.Options.PopulationLimit(TravelerMode.NPC));
		Main.mouseLeft = false;
		panel.UpdateUI(new GameTime());
		Assert.AreEqual(-1, typeof(TravelerControlPanel).GetField("dragging", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null));
	}

	[TestMethod]
	[DataRow(-500, -500)]
	[DataRow(2000, -500)]
	[DataRow(-500, 2000)]
	[DataRow(2000, 2000)]
	public void HeldTitleStaysOnScreenBeforeTheNextDraw(int mouseX, int mouseY)
	{
		var panel = new TravelerControlPanel();
		PlayerInput.MouseInfo = new MouseState(125, 160, 0, ButtonState.Pressed,
			ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);
		panel.UpdateUI(new GameTime());
		PlayerInput.MouseInfo = new MouseState(mouseX, mouseY, 0, ButtonState.Pressed,
			ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);
		panel.UpdateUI(new GameTime());
		var position = (Vector2)typeof(TravelerControlPanel).GetField("position", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		Assert.AreEqual(new Vector2(mouseX < 0 ? 8 : 252, mouseY < 0 ? 8 : 110), position);
		Assert.IsTrue(TravelerControlPanel.CapturesMouse, "Dragging outside still owns this press until release.");
	}

	[TestMethod]
	public void ResizingWhileDraggingKeepsTheWholePanelVisible()
	{
		var panel = new TravelerControlPanel();
		PlayerInput.MouseInfo = new MouseState(125, 160, 0, ButtonState.Pressed,
			ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);
		panel.UpdateUI(new GameTime());
		SetField(typeof(PlayerInput), "_originalScreenWidth", 600);
		SetField(typeof(PlayerInput), "_originalScreenHeight", 400);
		PlayerInput.MouseInfo = new MouseState(590, 390, 0, ButtonState.Pressed,
			ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);
		panel.UpdateUI(new GameTime());
		var position = (Vector2)typeof(TravelerControlPanel).GetField("position", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		float scale = (float)typeof(TravelerControlPanel).GetProperty("FitScale", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		Assert.IsTrue(position.X >= 8 && position.Y >= 8);
		Assert.IsTrue((position.X + 540) * Main.UIScale * scale <= 600);
		Assert.IsTrue((position.Y + 482) * Main.UIScale * scale <= 400);
	}
}
