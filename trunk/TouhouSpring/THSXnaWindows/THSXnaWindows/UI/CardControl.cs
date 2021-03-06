﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.UI
{
    partial class CardControl : TransformNode, IMouseTracked, IResourceContainer, IDisposable
    {
        private class CardDesignContainer : TransformNode, IResourceContainer, IDisposable
        {
            private ResourceContainer m_resourceContainer = new ResourceContainer();

            #region IResourceContainer implementation

            void IResourceContainer.Register(object resource)
            {
                m_resourceContainer.Register(resource);
            }

            void IResourceContainer.Release(object resource)
            {
                m_resourceContainer.Release(resource);
            }

            #endregion

            #region IDisposable implementation

            private bool m_disposed = false;

            public void Dispose()
            {
                if (!m_disposed)
                {
                    m_resourceContainer.ReleaseAll();
                    m_disposed = true;
                }
                GC.SuppressFinalize(this);
            }

            #endregion
        }

        private ResourceContainer m_resourceContainer = new ResourceContainer();
        private string m_designName;
        private Style.IStyleContainer m_cardDesignStyle;

        public int CardGuid
        {
            get; private set;
        }

        public Services.CardDataManager.ICardData CardData
        {
            // set in GameUI.UpdateCardControls() every frame
            get; internal set;
        }

        public bool IsCardDead
        {
            // set in GameUI.PutToGraveyard()
            get; internal set;
        }

        public Style.CardControlStyle Style
        {
            get; private set;
        }

        public TransformNode BodyContainer
        {
            get; private set;
        }

        public List<Addin> Addins
        {
            get; private set;
        }

        public T GetAddin<T>() where T : Addin
        {
            return Addins.FirstOrDefault(addin => addin is T) as T;
        }

        public string DesignName
        {
            get { return m_designName; }
        }

        public CardControl(int cardGuid, Style.CardControlStyle style)
        {
            if (style == null)
            {
                throw new ArgumentNullException("style");
            }

            Addins = new List<Addin>();
            CardGuid = cardGuid;
            Style = style;
            Style.RegisterBinding(this);

            Initialize_Render();
        }

        public void SetCardDesign(string designName)
        {
            if (designName == m_designName)
            {
                return;
            }

            if (m_cardDesignStyle != null)
            {
                m_cardDesignStyle.Target.Dispatcher = null;
                (m_cardDesignStyle.Target as CardDesignContainer).Dispose();
                m_cardDesignStyle = null;
            }

            if (designName != null)
            {
                var layoutGizmo = new Style.LayoutGizmo<CardDesignContainer>(Style, GameApp.Service<Services.Styler>().GetCardDesign(designName));
                layoutGizmo.Initialize();
                layoutGizmo.Target.Dispatcher = null;
                BodyContainer.Listeners.Insert(0, layoutGizmo.Target);
                layoutGizmo.BindingProvider = this;
                m_cardDesignStyle = layoutGizmo;
            }

            m_designName = designName;
        }

        public void OnStyleInitialized()
        {
            BodyContainer = Style.ChildIds["Body"].Target as TransformNode;

            MouseTracked = new MouseTracked(this);
            MouseTracked.Dispatcher = this;
        }

        public void SetParentAndKeepPosition(EventDispatcher dispatcher)
        {
            if (dispatcher == Dispatcher)
            {
                return;
            }
            else if (dispatcher == null || Dispatcher == null)
            {
                Dispatcher = dispatcher;
                return;
            }

            Transform *= GetTransformBetween(Dispatcher, dispatcher);
            Dispatcher = dispatcher;
        }

        public void Update(float deltaTime)
        {
            Style.Apply();
            if (m_cardDesignStyle != null)
            {
                m_cardDesignStyle.Apply();
            }
            Addins.ForEach(addin => addin.Update(deltaTime));
        }

        #region IMouseTracked implementation

        public Rectangle Region
        {
            get; set;
        }

        public MouseTracked MouseTracked
        {
            get; private set;
        }

        bool IMouseTracked.IntersectWith(Ray ray)
        {
            return MouseTracked.Intersect(ray, new Rectangle(0, -Region.Height / Region.Width, 1, Region.Height / Region.Width), TransformToGlobal.Invert());
        }

        void IMouseTracked.OnMouseEnter(MouseEventArgs e) { }
        void IMouseTracked.OnMouseLeave(MouseEventArgs e) { }
        void IMouseTracked.OnMouseMove(MouseEventArgs e) { }
        void IMouseTracked.OnMouseButton1Down(MouseEventArgs e) { }
        void IMouseTracked.OnMouseButton1Up(MouseEventArgs e) { }
        void IMouseTracked.OnMouseButton2Down(MouseEventArgs e) { }
        void IMouseTracked.OnMouseButton2Up(MouseEventArgs e) { }

        #endregion

        #region IResourceContainer implementation

        void IResourceContainer.Register(object resource)
        {
            m_resourceContainer.Register(resource);
        }

        void IResourceContainer.Release(object resource)
        {
            m_resourceContainer.Release(resource);
        }

        #endregion

        #region IDisposable implementation

        private bool m_disposed = false;

        public void Dispose()
        {
            if (!m_disposed)
            {
                Dispatcher = null;

                m_resourceContainer.ReleaseAll();
                (m_cardDesignStyle.Target as CardDesignContainer).Dispose();

                m_disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
