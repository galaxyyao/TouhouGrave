﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
	public class AttackModifier : SimpleBehavior<AttackModifier>
	{
		private bool m_modifierAdded = false;

		public Func<int, int> Expression
		{
			get; private set;
		}

		public AttackModifier(Func<int, int> expression)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			Expression = expression;
		}

		protected override void OnBind()
		{
			var warrior = Host.Behaviors.Get<Warrior>();
			if (warrior != null)
			{
				warrior.Attack.AddModifierToTail(Expression);
				m_modifierAdded = true;
			}
		}

		protected override void OnUnbind()
		{
			// TODO: better unbind if the Warrior behavior is unbound in advance
			var warrior = Host.Behaviors.Get<Warrior>();
			Debug.Assert((warrior != null) == m_modifierAdded);
			if (warrior != null)
			{
				warrior.Attack.RemoveModifier(Expression);
			}
		}
	}
}
