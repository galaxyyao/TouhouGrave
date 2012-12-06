﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
	/// <summary>
	/// The base class for all behaviors
	/// </summary>
	public abstract class BaseBehavior<T> : IInternalBehavior
        where T : BehaviorModel
	{
		/// <summary>
		/// The hosting card i.e. the card to which this behavior is bound
		/// </summary>
		public BaseCard Host
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

        public T Model
        {
            get; private set;
        }

        BehaviorModel IBehavior.Model
        {
            get { return Model; }
        }

        void IInternalBehavior.Initialize(BehaviorModel model, bool persistent)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            else if (!(model is T))
            {
                throw new ArgumentException(String.Format("Model must be of type {0}.", typeof(T).Name));
            }
            else if (Model != null)
            {
                throw new InvalidOperationException("Behavior is already initialized.");
            }

            Model = (T)model;
            Persistent = persistent;
            OnInitialize();
        }

        /// <summary>
        /// Called by BaseCard for binding this behavior to it.
        /// </summary>
        /// <param name="host">The hosting card</param>
        void IInternalBehavior.Bind(BaseCard host)
        {
            Debug.Assert(Host == null && host != null);
            Host = host;
        }

        /// <summary>
        /// Called by BaseCard for unbinding this behavior from it.
        /// </summary>
        void IInternalBehavior.Unbind()
        {
            Debug.Assert(Host != null);
            Host = null;
        }

        public virtual void OnMessage(string message, object[] args) { }

		protected bool IsOnBattlefield
		{
			get { return Host.Owner.CardsOnBattlefield.Contains(Host); }
		}

        protected virtual void OnInitialize() { }
	}
}
