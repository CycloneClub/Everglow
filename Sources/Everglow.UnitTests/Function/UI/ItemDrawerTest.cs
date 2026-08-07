using Everglow.Commons.UI.StringDrawerSystem.DrawerItems.ImageDrawers;

namespace Everglow.UnitTests.Function.UI;

[TestClass]
public class ItemDrawerTest
{
	[TestMethod]
	public void Create_Should_Return_Correctly_When_ParamIsValid()
	{
		var str = ItemDrawer.Create(1);
		Assert.AreEqual("[ItemDrawer,Type='1']", str);

		str = ItemDrawer.Create(1, 2);
		Assert.AreEqual("[ItemDrawer,Type='1',Stack='2']", str);

		str = ItemDrawer.Create(1, 2, new Microsoft.Xna.Framework.Color(1, 1, 1));
		Assert.AreEqual("[ItemDrawer,Type='1',Stack='2',StackColor='1,1,1,255']", str);

		str = ItemDrawer.Create(1, 2, new Microsoft.Xna.Framework.Color(1, 1, 1), 30);
		Assert.AreEqual("[ItemDrawer,Type='1',Stack='2',StackColor='1,1,1,255',StackFontSize='30']", str);
	}
}
