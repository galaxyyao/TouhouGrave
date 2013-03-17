﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TouhouSpring.Graphics;
using TouhouSpring.UI;

namespace TouhouSpring.Services
{
    [LifetimeDependency(typeof(ResourceManager))]
    [LifetimeDependency(typeof(Styler))]
    [LifetimeDependency(typeof(UIManager))]
    [UpdateDependency(typeof(UIManager))]
    partial class ConversationUI : GameService
    {
        private ConversationManager _convManager;

        public UI.TransformNode Root
        {
            get;
            private set;
        }

        public UI.Page ConversationUIPage
        {
            get;
            private set;
        }

        public override void Startup()
        {
            //Init Conversation Manager
            _convManager = new ConversationManager();
            _convManager.Scene = "init";

            Matrix toScreenSpace = Matrix.Identity;
            toScreenSpace.M11 = 2 / 1024.0f;
            toScreenSpace.M22 = 2 / 768.0f;
            toScreenSpace.M41 = -1;
            toScreenSpace.M42 = -1;

            var cam = new Camera
            {
                PostWorldMatrix = toScreenSpace,
                Position = Vector3.UnitZ,
                IsPerspective = false,
                ViewportWidth = 2,
                ViewportHeight = -2
            };
            cam.Dirty();

            Root = new UI.TransformNode
            {
                Transform = cam.WorldToProjectionMatrix,
                Dispatcher = GameApp.Service<UIManager>().Root
            };

            var pageStyle = new Style.PageStyle(GameApp.Service<Styler>().GetPageStyle("Conversation"));
            pageStyle.Initialize();
            ConversationUIPage = pageStyle.TypedTarget;

            ((Panel)pageStyle.ChildIds["ConversationPanel"].Target).MouseButton1Up += new EventHandler<MouseEventArgs>(ConversationUI_MouseButton1Up);
        }

        void ConversationUI_MouseButton1Up(object sender, MouseEventArgs e)
        {
            if (!_convManager.Next())
            {
                //Test, enter first battle
                // detach menu ui
                Root.Dispatcher = null;

               //startgame
            }
        }

        public void StartConversation()
        {
            ConversationUIPage.Dispatcher = Root;
        }

        public override void Update(float deltaTime)
        {
            if (ConversationUIPage.Dispatcher != null)
            {
                ConversationUIPage.Style.Apply();
            }
        }

        public string CurrentText { get { return _convManager.GetCurrentText(); } }
    }
}
