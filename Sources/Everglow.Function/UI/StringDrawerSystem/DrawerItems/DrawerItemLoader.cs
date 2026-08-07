using Everglow.Commons.UI.StringDrawerSystem.DrawerItems.TextDrawers;

namespace Everglow.Commons.UI.StringDrawerSystem.DrawerItems
{
	public class DrawerItemLoader
	{
		private static DrawerItemLoader instance;

		public static DrawerItemLoader Instance
		{
			get
			{
				instance ??= new DrawerItemLoader();
				return instance;
			}
		}

		private List<DrawerItem> drawers = new List<DrawerItem>();

		public DrawerItemLoader()
		{
			Load();
		}

		public void Load()
		{
			var cTypes = from c in GetType().Assembly.GetTypes()
						 where !c.IsAbstract && c.IsSubclassOf(typeof(DrawerItem))
						 select c;
			foreach (var ctype in cTypes)
			{
				drawers.Add((DrawerItem)Activator.CreateInstance(ctype));
			}
		}

		public DrawerItem GetDrawerItem(StringDrawer stringDrawer, string originalText, string drawerName, StringParameters stringParameters, int line)
		{
			foreach (DrawerItem drawer in drawers)
			{
				if (drawer.Permission(stringDrawer, originalText, drawerName, stringParameters))
				{
					var d = drawer.GetInstance(stringDrawer, originalText, drawerName, stringParameters);
					d.Line = line;
					return d;
				}
			}
			TextDrawer td = new TextDrawer();
			td.Init(stringDrawer, originalText, drawerName, stringParameters);
			td.Line = line;
			return td;
		}
	}
}
