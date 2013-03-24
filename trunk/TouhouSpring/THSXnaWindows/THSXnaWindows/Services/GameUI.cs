 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.Services
{
    [LifetimeDependency(typeof(ResourceManager))]
    [LifetimeDependency(typeof(Styler))]
    [LifetimeDependency(typeof(UIManager))]
    [UpdateDependency(typeof(UIManager))]
    [RenderDependency(typeof(Graphics.Scene))]
    partial class GameUI : GameService
    {
        public UI.Page InGameUIPage
        {
            get; private set;
        }

        public Camera WorldCamera
        {
            get; private set;
        }

        public Camera UICamera
        {
            get; private set;
        }

        public Game Game
        {
            get { return GameApp.Service<GameManager>().Game; }
        }

        public IUIState UIState
        {
            get; private set;
        }

        public override void Startup()
        {
            Matrix toScreenSpace = Matrix.Identity;
            toScreenSpace.M11 = 2 / 1024.0f;
            toScreenSpace.M22 = 2 / 768.0f;
            toScreenSpace.M41 = -1;
            toScreenSpace.M42 = -1;

            float fov = MathUtils.PI / 4;
            float nearPlaneHeight = 2 * 0.1f * (float)Math.Tan(fov * 0.5f);
            WorldCamera = new Camera
            {
                Position = new Vector3(0, -1.2f, 2.2f),
                LookAt = new Vector3(0, -0.2f, 0.0f),
                Up = Vector3.UnitZ,
                IsPerspective = true,
                ViewportWidth = nearPlaneHeight * 1.3333f,
                ViewportHeight = nearPlaneHeight
            };
            WorldCamera.Dirty();

            UICamera = new Camera
            {
                PostWorldMatrix = toScreenSpace,
                Position = Vector3.UnitZ,
                IsPerspective = false,
                ViewportWidth = 2,
                ViewportHeight = -2
            };
            UICamera.Dirty();

            InitializeContextButton();
            CreateBindingEvaluator();

            var pageStyle = new Style.PageStyle(GameApp.Service<Styler>().GetPageStyle("InGame"));
            pageStyle.Initialize();
            pageStyle.BindingProvider = this;
            InGameUIPage = pageStyle.TypedTarget;
        }

        public override void Shutdown()
        {
            UnregisterAllCards();
            InGameUIPage.DisposeResources();
            DestroyContextButton();
        }

        public override void Update(float deltaTime)
        {
            if (Game != null)
            {
                InGameUIPage.Style.Apply();
                UpdateCardZones();
                UpdateCardControls(deltaTime);
                UpdatePiles(deltaTime);
                UpdateZoomInCard(deltaTime);
            }
        }

        public void EnterState(IUIState uiState, Interactions.BaseInteraction io)
        {
            if (uiState == null)
            {
                throw new ArgumentNullException("uiState");
            }
            else if (UIState != null)
            {
                throw new InvalidOperationException("Can't enter a state before leaving the previous state.");
            }

            UIState = uiState;
            uiState.OnEnter(io);
        }

        public void LeaveState()
        {
            if (UIState == null)
            {
                throw new InvalidOperationException("No state is entered.");
            }

            UIState.OnLeave();
            RemoveAllContextButtons();
            UIState = null;
        }

        public bool ShallPlayerBeRevealed(Player player)
        {
            var pid = Game.ActingPlayer != null ? Game.ActingPlayer.Index : -1;
            bool actingPlayerIsLocalPlayer = pid != -1 &&
                (Game.Controller as XnaUIController).Agents[pid] is Agents.LocalPlayerAgent;
            return actingPlayerIsLocalPlayer ? (player == Game.ActingPlayer) : (player != Game.ActingPlayer);
        }

        internal void GameStarted()
        {
            InGameUIPage.Dispatcher = GameApp.Service<UIManager>().Root;
            InGameUIPage.Style.Apply();

            InitializeCardZones();
            InitializePiles();

            foreach (var player in Game.Players)
            {
                player.Assists.ForEach(card => RegisterCard(card));
                if (player.Hero != null)
                {
                    RegisterCard(player.Hero);
                }
            }
        }
    }
}
