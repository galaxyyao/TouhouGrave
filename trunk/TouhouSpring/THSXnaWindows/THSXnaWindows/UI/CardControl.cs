using System;
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

        public BaseCard Card
        {
            get; private set;
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

        public CardControl(BaseCard card, Style.CardControlStyle style)
        {
            if (card == null)
            {
                throw new ArgumentNullException("card");
            }
            else if (style == null)
            {
                throw new ArgumentNullException("style");
            }

            Addins = new List<Addin>();
            Card = card;
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
                layoutGizmo.Target.Dispatcher = BodyContainer;
                layoutGizmo.BindingProvider = this;
                m_designName = designName;
                m_cardDesignStyle = layoutGizmo;
            }
        }

        public void OnStyleInitialized()
        {
            MouseTracked = new MouseTracked(this);
            MouseTracked.Dispatcher = this;

            BodyContainer = Style.ChildIds["Body"].Target as TransformNode;
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
            return MouseTracked.Intersect(ray, Region, BodyContainer.TransformToGlobal.Invert());
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
                Addins.ForEach(addin => addin.Dispose());

                m_disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
