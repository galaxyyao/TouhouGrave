﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    /// <summary>
    /// The base class for all behaviors
    /// </summary>
    public abstract class BaseBehavior<T> : IInternalBehavior
        where T : IBehaviorModel
    {
        private CardInstance m_host;

        /// <summary>
        /// The hosting card i.e. the card to which this behavior is bound
        /// </summary>
        public CardInstance Host
        {
            get { return IsStatic ? StaticBehaviorHost.Host : m_host; }
        }

        /// <summary>
        /// Persistent behavior is those created from card model.
        /// </summary>
        public bool Persistent
        {
            get; private set;
        }

        public bool IsStatic
        {
            get { return Persistent && Model.IsBehaviorStatic; }
        }

        protected T Model
        {
            get; private set;
        }

        protected Game Game
        {
            get { return Host != null && Host.Owner != null ? Host.Owner.Game : null; }
        }

        IBehaviorModel IBehavior.Model
        {
            get { return Model; }
        }

        CardInstance IInternalBehavior.RealHost
        {
            get { return m_host; }
            set { Debug.Assert(!IsStatic || value == null); m_host = value; }
        }

        void IInternalBehavior.Initialize(IBehaviorModel model, bool persistent)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            else if (!(model is T))
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Model must be of type {0}.", typeof(T).FullName));
            }
            else if (Model != null)
            {
                throw new InvalidOperationException("Behavior is already initialized.");
            }

            Model = (T)model;
            Persistent = persistent;
            OnInitialize();
        }

        void IInternalBehavior.TransferFrom(IBehavior original)
        {
            if (Model != null)
            {
                throw new InvalidOperationException("Already initialized.");
            }

            Model = (T)original.Model;
            Persistent = original.Persistent;
            OnTransferFrom(original);
        }

        void IInternalBehavior.ReceiveMessage(int messageId, object arg) { OnMessage(messageId, arg); }

        protected virtual void OnMessage(int messageId, object arg) { }
        protected virtual void OnInitialize() { }
        protected virtual void OnTransferFrom(IBehavior original) { }
    }

    internal static class StaticBehaviorHost
    {
        [ThreadStatic]
        public static CardInstance Host;
    }
}
