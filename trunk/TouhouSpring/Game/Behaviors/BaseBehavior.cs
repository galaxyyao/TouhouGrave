using System;
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
        /// <summary>
        /// The hosting card i.e. the card to which this behavior is bound
        /// </summary>
        public CardInstance Host
        {
            get; private set;
        }

        /// <summary>
        /// Persistent behavior is those created from card model.
        /// </summary>
        public bool Persistent
        {
            get; private set;
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

        /// <summary>
        /// Called by CardInstance for binding this behavior to it.
        /// </summary>
        /// <param name="host">The hosting card</param>
        void IInternalBehavior.Bind(CardInstance host)
        {
            Debug.Assert(Host == null && host != null);
            Host = host;
        }

        /// <summary>
        /// Called by CardInstance for unbinding this behavior from it.
        /// </summary>
        void IInternalBehavior.Unbind()
        {
            Debug.Assert(Host != null);
            Host = null;
        }

        void IInternalBehavior.ReceiveMessage(string message, object[] args) { OnMessage(message, args); }

        protected virtual void OnMessage(string message, object[] args) { }
        protected virtual void OnInitialize() { }
        protected virtual void OnTransferFrom(IBehavior original) { }
    }
}
