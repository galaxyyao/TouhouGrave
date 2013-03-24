using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Services
{
    interface IGameEvaluator
    {
        void Reevaluate(Game game);
    }

    class GameEvaluator<T> : IGameEvaluator
    {
        private Func<Game, T> m_evaluator;

        public T Value
        {
            get; private set;
        }

        public GameEvaluator(Func<Game, T> evaluator, T immediateValue)
        {
            m_evaluator = evaluator;
            Value = immediateValue;
        }

        void IGameEvaluator.Reevaluate(Game game)
        {
            Value = m_evaluator(game);
        }
    }

    class GameEvaluator : IGameEvaluator
    {
        private Action<Game> m_evaluator;

        public GameEvaluator(Action<Game> evaluator)
        {
            m_evaluator = evaluator;
        }

        void IGameEvaluator.Reevaluate(Game game)
        {
            m_evaluator(game);
        }
    }

    partial class GameManager
    {
        private List<WeakReference> m_evaluators = new List<WeakReference>();

        public GameEvaluator<T> CreateGameEvaluator<T>(Func<Game, T> evaluator, T immediateValue)
        {
            var ge = new GameEvaluator<T>(evaluator, immediateValue);
            RegisterGameEvaluator(new WeakReference(ge));
            return ge;
        }

        public GameEvaluator CreateGameEvaluator(Action<Game> evaluator)
        {
            var ge = new GameEvaluator(evaluator);
            RegisterGameEvaluator(new WeakReference(ge));
            return ge;
        }

        public void RefreshGameEvaluators()
        {
            for (int i = 0; i < m_evaluators.Count; ++i)
            {
                var wr = m_evaluators[i];
                if (wr != null && wr.IsAlive)
                {
                    (wr.Target as IGameEvaluator).Reevaluate(Game);
                }
                else
                {
                    m_evaluators[i] = null;
                }
            }
        }

        private void RegisterGameEvaluator(WeakReference wr)
        {
            for (int i = 0; i < m_evaluators.Count; ++i)
            {
                if (m_evaluators[i] == null)
                {
                    m_evaluators[i] = wr;
                    return;
                }
            }

            m_evaluators.Add(wr);
        }
    }
}
