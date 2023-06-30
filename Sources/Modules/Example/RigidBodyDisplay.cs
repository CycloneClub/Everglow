using Everglow.Commons.Physics.PBEngine.Collision;
using Everglow.Commons.Physics.PBEngine.Collision.Colliders;
using Everglow.Commons.Physics.PBEngine.PlayGround.Constrain;
using Everglow.Commons.Physics.PBEngine.PlayGround.Contact;
using Everglow.Commons.Vertex;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;

namespace Everglow.Commons.Physics.PBEngine
{
    internal class RigidBodyDisplay 
    {
        private PhysicsSimulation _physicsSimulation;
        private RenderTarget2D _canvasTarget;
        private Asset<Effect> _renderShader;

        public static RigidBodyDisplay Instance;

        public RigidBodyDisplay()
        {
            Instance = this;
        }

        private const int CANVAS_SIZE = 1024;

        public void Update()
        {
            _physicsSimulation.Update(0.1f);
        }

        public void Draw(SpriteBatch sb)
        {
            GeometryUtils.Blit(Main.screenTarget, Main.screenTargetSwap, null, null);
            RenderPhysWorld();
            GeometryUtils.Blit(Main.screenTargetSwap, Main.screenTarget, null, null);
            sb.Begin();
            var start = Main.LocalPlayer.Center - new Vector2(CANVAS_SIZE / 2, CANVAS_SIZE / 2) - Main.screenPosition;
            sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)start.X - 2, (int)start.Y - 2, CANVAS_SIZE + 4, CANVAS_SIZE + 4), Color.White);
            sb.Draw(_canvasTarget, Main.LocalPlayer.Center - new Vector2(CANVAS_SIZE / 2, CANVAS_SIZE / 2) - Main.screenPosition,
                null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.FlipVertically, 0f);

            sb.DrawString(FontAssets.MouseText.Value, "[Profiler]", start, Color.Green);
            sb.DrawString(FontAssets.MouseText.Value, $"PreIntegration Time: {_physicsSimulation.MeasuredPreIntegrationTimeInMs} ms", start + new Vector2(0, 28), Color.Green);
            sb.DrawString(FontAssets.MouseText.Value, $"BroadPhase Time: {_physicsSimulation.MeasuredBroadPhaseTimeInMs} ms", start + new Vector2(0, 56), Color.Green);
            sb.DrawString(FontAssets.MouseText.Value, $"NarrowPhase Time: {_physicsSimulation.MeasuredNarrowPhaseTimeInMs} ms", start + new Vector2(0, 74), Color.Green);

            sb.End();
        }

        private void RenderPhysWorld()
        {
            List<Vertex2D> vertices = new List<Vertex2D>();

            foreach (var p in _physicsSimulation.GetCurrentWireFrames())
            {
                vertices.Add(new Vertex2D(p, Color.White, Vector3.Zero));
            }

            Main.graphics.GraphicsDevice.SetRenderTarget(_canvasTarget);
            Main.graphics.GraphicsDevice.Clear(Color.Black);
            _renderShader.Value.Parameters["uTransform"].SetValue(Matrix.CreateOrthographicOffCenter(0f, CANVAS_SIZE, CANVAS_SIZE, 0f, 0f, 1f));
            _renderShader.Value.CurrentTechnique.Passes[0].Apply();
            Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices.ToArray(), 0, vertices.Count / 2);
        }

        public void ReInitPhysWorld()
        {
            _physicsSimulation = JointPlayground.SpringTriganluar();

            //var staticPlane3 = new PhysicsObject(
            //    new AABBCollider(1, 40), null);
            //staticPlane3.Position = new Vector2(200, 300);
            //staticPlane3.Rotation = 0f;
            //_physicsSimulation.AddPhysicsObject(staticPlane3);

            _physicsSimulation.Initialize();
        }

        public void Initialize()
        {
            ReInitPhysWorld();
            Main.QueueMainThreadAction(() =>
            {
                _canvasTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, 1024, 1024);
            });
            _renderShader = ModContent.Request<Effect>("ChatGPT/Core/Physics/Shaders/Default");
        }
    }
}
