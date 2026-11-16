using Terraria.UI;
using MonoMod.RuntimeDetour.HookGen;
using Terraria.GameContent.UI.Elements;
using ReLogic.Content;
using Everglow.Sources.Modules.ZYModule.Commons.Core.Enumerator;

namespace Everglow.Sources.Icons
{
    internal class AutoDynamicIconHook : ILoadable
    {
        MethodInfo HookTarget;
        public void Load(Mod mod)
        {
            //说明钩子已经挂上,直接跳过
            if (HookTarget is not null)
            {
                return;
            }
            //拿取上级程序集
            Assembly assembly = typeof(Terraria.ModLoader.UI.UICommon).Assembly;
            //准备容器
            Type[] types;
            try
            {
                //尝试将类型注入容器
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                //从异常中取出已经成功拿到的类型
                types = (from Type t in e.Types where t is not null select t).ToArray();
            }
            //筛选目标
            foreach (var type in types)
            {
                //找到目标
                if (type.Name == "UIModItem")
                {
                    //拿取目标类型的初始化方法句柄
                    HookTarget = type.GetMethod("OnInitialize", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                    break;
                }
            }
            //通过句柄挂钩,实际上应该进行一次HookTarget判空,但是我懒
            HookEndpointManager.Add(MethodBase.GetMethodFromHandle(HookTarget.MethodHandle), HookMethod);
        }
        public void Unload()
        {
            //说明没挂上,没必要卸载
            if (HookTarget is null)
            {
                return;
            } // TODO: Fix an object reference error on mod reload without introducing other bugs
            HookEndpointManager.Remove(MethodBase.GetMethodFromHandle(HookTarget.MethodHandle), HookMethod);
            HookTarget = null;
        }
        private void HookMethod(Action<UIElement> orig, UIElement self)
        {
            //运行原版方法,把基础内容加上
            orig(self);
            //拿取父对象里的mod的名字判定是不是自己
            string Name = (string)self.GetType().GetMethod("get_ModName", BindingFlags.Public | BindingFlags.Instance).Invoke(self, null);
            if (Name == Everglow.Instance.Name)
            {
                //移除原版的模组图标,实际上应该对反射进行一次判空,但是我懒
                self.RemoveChild((UIElement)self.GetType().GetField("_modIcon", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(self));
                //把图标做成动态UI放进去
                self.Append(new DynamicIconUI(ModContent.Request<Texture2D>("Everglow/Sources/Icons/Icon_Content", AssetRequestMode.ImmediateLoad).Value, new Rectangle(0, 0, 80, 80), 3));
                //把边框做成动态UI放进去
                var border = new DynamicIconUI(ModContent.Request<Texture2D>("Everglow/Sources/Icons/Icon_Framework", AssetRequestMode.ImmediateLoad).Value, new Rectangle(0, 0, 96, 96), 3);
                //边框比图标大16,所以需要向左上偏移(8,8)
                border.Left.Set(-8, 0);
                border.Top.Set(-8, 0);
                self.Append(border);
                //防止边框超出底板
                //self.OverflowHidden = true;
            }
        }
        public class DynamicIconUI : UIElement, IColorable
        {
            public Texture2D Texture;
            public Rectangle Frame;
            public Vector2 Size;
            public int Interval;
            public int Timer;
            public Color Color { get; set; } = Color.White;
            /// <summary>
            /// 动态图片的UI,只接受单列帧图,你也可以改下面的draw
            /// </summary>
            /// <param name="texture">要绘制的图片</param>
            /// <param name="frame">每帧的大小</param>
            /// <param name="size">适应性缩放大小限制</param>
            /// <param name="interval">刷新间隔</param>
            public DynamicIconUI(Texture2D texture, Rectangle frame, Vector2 size, int interval)
            {
                Texture = texture;
                Frame = frame;
                Size = size;
                Interval = interval;
            }
            //#region Use To Debug
            //public override string ToString()
            //{
            //    return $"[{nameof(Texture)}]:{{IsNull?{Texture is null},Name:{Texture.Name},Width:{Texture?.Width ?? 0},Height:{Texture?.Height ?? 0}}}\n" +
            //        $"[{nameof(Frame)}]:{Frame}\n" +
            //        $"[{nameof(Size)}]:{Size}\n" +
            //        $"[{nameof(Timer)}]:{Timer}" +
            //        $"[{nameof(Interval)}]:{Interval}\n" +
            //        $"[Scale]:{new Vector2(Size.X / Frame.Width, Size.Y / Frame.Height)}";
            //}
            //#endregion
            /// <summary>
            /// 动态图片的UI,只接受单列帧图,你也可以改下面的draw
            /// </summary>
            /// <param name="texture">要绘制的图片</param>
            /// <param name="frame">每帧的大小</param>
            /// <param name="interval">刷新间隔</param>
            public DynamicIconUI(Texture2D texture, Rectangle frame, int interval) : this(texture, frame, frame.Size(), interval)
            {

            }
            public override void OnInitialize()
            {
                //重置刷新间隔计时器
                Timer = 0;
            }
            protected override void DrawSelf(SpriteBatch spriteBatch)
            {
                //更新计时器
                Timer++;
                //如果该换帧了
                if (Timer % Interval == 0)
                {
                    //如果下一帧抵达贴图底部(超出图片界限)
                    if (Timer / Interval * Frame.Height >= Texture.Height)
                    {
                        //重置计时器
                        Timer = 0;
                    }
                    //重置帧区域
                    Frame = new Rectangle(0, Timer / Interval * Frame.Height, Frame.Width, Frame.Height);
                }
                //获取自适应缩放倍率
                Vector2 scale = new(Size.X / Frame.Width, Size.Y / Frame.Height);
                //画图
                spriteBatch.Draw(Texture,
                    GetDimensions().Position(),
                    Frame,
                    Color,
                    0,
                    Vector2.Zero,
                    scale,
                    SpriteEffects.None,
                    0);
                //如果你要切换其他的绘制图片的话,可以自己加图片索引
                //然后对Texture,Frame,Size,Interval重新赋值即可进行切换
            }
        }
    }
}
