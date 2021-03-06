﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading;
using THSNetwork = TouhouSpring.Network;

namespace TouhouSpring.Services
{
    [LifetimeDependency(typeof(CardDatabase))]
    [LifetimeDependency(typeof(ResourceManager))]
    [LifetimeDependency(typeof(Styler))]
    [LifetimeDependency(typeof(UIManager))]
    [UpdateDependency(typeof(UIManager))]
    [LifetimeDependency(typeof(Graphics.SwfRenderer))]
    [LifetimeDependency(typeof(CurrentProfile))]
    partial class MenuUI : GameService
    {
        private Dictionary<string, MenuPage> m_pages = new Dictionary<string, MenuPage>();
        private bool m_isFirstLoaded = true;

        GameStartupParameters param = new GameStartupParameters();

        public UI.TransformNode Root
        {
            get;
            private set;
        }

        public override void Startup()
        {
            #region Initialize Page
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

            LoadPage("MainMenu");
            LoadPage("FreeMode");
            LoadPage("Network");
            LoadPage("Quit");

            #endregion

            if (m_isFirstLoaded)
            {
                #region Main Menu
                m_pages["MainMenu"].MenuClicked += (id, item) =>
                {
                    if (id == "freemode")
                    {
                        CurrentPage = m_pages["FreeMode"];
                    }
                    //else if (id == "storymode")
                    //{
                    //    //Test Conversation UI
                    //    CurrentPage = null;
                    //    Root.Dispatcher = null;
                    //    GameApp.Service<GameManager>().EnterConversation();
                    //}
                    else if (id == "makedeck")
                    {
                        CurrentPage = null;
                        Root.Dispatcher = null;
                        GameApp.Service<GameManager>().EnterDeckUI();
                    }
                    else if (id == "quit")
                    {
                        CurrentPage = m_pages["Quit"];
                    }
                };
                #endregion

                #region FreeMode Menu

                PrepareGameStartupParam();

                m_pages["FreeMode"].MenuClicked += (id, item) =>
                {
                    if (id == "vsai" || id == "hotseat")
                    {
                        CurrentPage = null;
                        // detach menu ui
                        Root.Dispatcher = null;

                        Agents.BaseAgent agent0;
                        Agents.BaseAgent agent1;

                        switch (id)
                        {
                            case "vsai":
                                if (GameApp.Instance.GetCommandLineArgValue("playback") != null)
                                {
                                    var pbAgent = new Agents.PlaybackAgent(0);
                                    param.Seed = pbAgent.RandomSeed;
                                    agent0 = pbAgent;
                                }
                                else if (GameApp.Instance.GetCommandLineArgValue("record") != null)
                                {
                                    agent0 = new Agents.LocalPlayerAgent(0, param.Seed);
                                }
                                else
                                {
                                    agent0 = new Agents.LocalPlayerAgent(0);
                                }
                                agent1 = new Agents.AIAgent(1);
                                break;
                            case "hotseat":
                                agent0 = new Agents.LocalPlayerAgent(0);
                                agent1 = new Agents.LocalPlayerAgent(1);
                                break;
                            default:
                                throw new InvalidOperationException("Invalid menu item");
                        }

                        GameApp.Service<GameManager>().StartGame(param, new Agents.BaseAgent[] { agent0, agent1 });
                    }
                    else if (id == "vsnetwork")
                    {
                        //CurrentPage = m_pages["Network"];

                        CurrentPage = null;
                        Root.Dispatcher = null;
                        GameApp.Service<GameManager>().EnterNetworkUI();
                    }
                    else if (id == "back")
                    {
                        CurrentPage = m_pages["MainMenu"];
                    }
                };
                #endregion

                #region Network Menu
                m_pages["Network"].MenuClicked += (id, item) =>
                {
                    if (id == "backtofreemode")
                    {
                        //CurrentPage = m_pages["FreeMode"];
                    }
                };
                #endregion

                #region Quit Menu
                m_pages["Quit"].MenuClicked += (id, item) =>
                {
                    if (id == "quit")
                    {
                        GameApp.Instance.Exit();
                    }
                    else if (id == "back")
                    {
                        CurrentPage = m_pages["MainMenu"];
                    }
                };
                #endregion

                //m_isFirstLoaded = false;
            }

            CurrentPage = m_pages["MainMenu"];

            var curve = GameApp.Service<ResourceManager>().Acquire<Curve>("Curves/CardMove");
            var font = new Graphics.TextRenderer.FontDescriptor("Microsoft YaHei", 16);
            var fmtOptions = new Graphics.TextRenderer.FormatOptions(font);
            var candFmtOptions = new Graphics.TextRenderer.FormatOptions(new Graphics.TextRenderer.FontDescriptor("Microsoft YaHei", 11));

            var txtBox = new UI.TextBox(250, 30, fmtOptions, candFmtOptions)
            {
                ForeColor = Color.Black,
                SelectionBackColor = new Color(0, 0, 0, 0.75f),
                SlidingCurve = curve,
                Transform = MatrixHelper.Translate(50, 200),
                Dispatcher = Root
            };

            txtBox = new UI.TextBox(250, 30, fmtOptions, candFmtOptions)
            {
                ForeColor = Color.Black,
                SelectionBackColor = new Color(0, 0, 0, 0.75f),
                SlidingCurve = curve,
                Transform = MatrixHelper.Translate(50, 250),
                Dispatcher = Root
            };

            //m_testAnimation = new Graphics.SwfInstance("germs")
            //{
            //    IsPlaying = true,
            //    TimeFactor = 2.0f,
            //    Transform = MatrixHelper.RotateZ(MathHelper.Pi / 4) * MatrixHelper.Translate(256f, 256f)
            //};

            //m_testAnimation2 = new Graphics.SwfInstance("28835")
            //{
            //    IsPlaying = true,
            //    Transform = MatrixHelper.Translate(768f, 0f)
            //};
        }

        private void PrepareGameStartupParam()
        {
            var cardDb = GameApp.Service<CardDatabase>();
            int deck1Id = Settings.Instance.MyAppSettings.Profiles.CurrentProfile.Deck1Id;
            int deck2Id = Settings.Instance.MyAppSettings.Profiles.CurrentProfile.Deck2Id;

            Deck deck1 = (from deck in Settings.Instance.MyAppSettings.Profiles.CurrentProfile.Decks.MyDecks
                          where deck.Id == deck1Id
                          select deck).FirstOrDefault();
            foreach (var cardModelId in deck1.DeckCardIdList.Model)
            {
                deck1.Add(cardDb.GetModel(cardModelId));
            }
            foreach (var assistModelId in deck1.DeckAssistIdList.Model)
            {
                deck1.Assists.Add(cardDb.GetModel(assistModelId));
            }

            Deck deck2 = (from deck in Settings.Instance.MyAppSettings.Profiles.CurrentProfile.Decks.MyDecks
                          where deck.Id == deck2Id
                          select deck).FirstOrDefault();
            foreach (var cardModelId in deck2.DeckCardIdList.Model)
            {
                deck2.Add(cardDb.GetModel(cardModelId));
            }
            foreach (var assistModelId in deck2.DeckAssistIdList.Model)
            {
                deck2.Assists.Add(cardDb.GetModel(assistModelId));
            }

            param = new GameStartupParameters();
            param.PlayerDecks.Add(deck1);
            param.PlayerDecks.Add(deck2);
            param.PlayerIds.Add("真凉");
            param.PlayerIds.Add("爱衣");
        }

        private void LoadPage(string id)
        {
            var pageStyle = new Style.PageStyle(GameApp.Service<Styler>().GetPageStyle(id));
            pageStyle.Initialize();
            if (!m_pages.ContainsKey(id))
                m_pages.Add(id, new MenuPage(pageStyle.TypedTarget));
        }

        public override void Update(float deltaTime)
        {
            foreach (var page in m_pages.Values)
            {
                if (page.Page.Dispatcher != null)
                {
                    page.Page.Style.Apply();
                    page.Update(deltaTime);
                }
            }

            //m_testAnimation.Update(deltaTime);
            //m_testAnimation2.Update(deltaTime);
        }

        public override void Render()
        {
            //GameApp.Service<Graphics.SwfRenderer>().Render(m_testAnimation);
            //GameApp.Service<Graphics.SwfRenderer>().Render(m_testAnimation2);
        }
    }
}
