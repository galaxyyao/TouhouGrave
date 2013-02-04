using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Simulation
{
    class Context : BaseController
    {
        public class Branch
        {
            public List<Choice> ChoicePath;
            public Game Result;
        }

        private BaseSimulator m_simulator;
        private int m_depth;
        private List<Choice> m_currentChoicePath;
        private int m_currentChoicePriority;
        private List<List<Choice>> m_pendingChoices = new List<List<Choice>>();

        private List<Branch> m_branches = new List<Branch>();

        public Game RootGame
        {
            get; private set;
        }

        public IEnumerable<Branch> Branches
        {
            get { return m_branches; }
        }

        public int BranchCount
        {
            get { return m_branches.Count; }
        }

        public int CurrentChoicePriority
        {
            get
            {
                System.Diagnostics.Debug.Assert(!ChoiceMade);
                return m_currentChoicePriority;
            }
        }

        private bool ChoiceMade
        {
            get { return m_depth <= (m_currentChoicePath != null ? m_currentChoicePath.Count : 0); }
        }

        private Choice NextChoice
        {
            get { return m_currentChoicePath[m_depth - 1]; }
        }

        public Context(Game game, BaseSimulator simulator)
            : base(true)
        {
            if (game == null)
            {
                throw new ArgumentNullException("game");
            }
            else if (simulator == null)
            {
                throw new ArgumentNullException("simulator");
            }

            m_simulator = simulator;
            RootGame = game;
        }

        public void Start()
        {
            while (true)
            {
                m_depth = 0;
                m_currentChoicePriority = 0;
                TryMoveNextChoice();

                var game = RootGame.CloneWithController(this);
                game.RunTurn();

                m_branches.Add(new Branch
                {
                    ChoicePath = m_currentChoicePath,
                    Result = game
                });

                if (m_pendingChoices.Count == 0)
                {
                    break;
                }
            }
        }

        private void ForkChoice(Choice choice)
        {
            List<Choice> newChoicePath = new List<Choice>();
            if (m_currentChoicePath != null)
            {
                for (int i = 0; i < m_currentChoicePath.Count; ++i)
                {
                    newChoicePath.Add(m_currentChoicePath[i]);
                }
            }
            newChoicePath.Add(choice);
            m_pendingChoices.Insert(0, newChoicePath);
        }

        private void TryMoveNextChoice()
        {
            // discard the current choice path
            if (m_pendingChoices.Count > 0)
            {
                m_currentChoicePath = m_pendingChoices[0];
                m_pendingChoices.RemoveAt(0);
            }
            else
            {
                m_currentChoicePath = null;
            }
        }

        private void OnInteraction(Interactions.BaseInteraction io, IEnumerable<Choice> choices)
        {
            ++m_depth;

            if (!ChoiceMade)
            {
                foreach (var choice in choices)
                {
                    ForkChoice(choice);
                }

                TryMoveNextChoice();
            }

            if (ChoiceMade)
            {
                var nextChoice = NextChoice;
                nextChoice.Make(io);
                m_currentChoicePriority = Math.Max(nextChoice.Priority, m_currentChoicePriority);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        #region major interactions

        [Interactions.MessageHandler(typeof(Interactions.TacticalPhase))]
        private bool OnTacticalPhase(Interactions.TacticalPhase interactionObj)
        {
            OnInteraction(interactionObj, m_simulator.TacticalPhase(interactionObj, this));
            return false;
        }

        [Interactions.MessageHandler(typeof(Interactions.SelectCards))]
        private bool OnSelectCards(Interactions.SelectCards interactionObj)
        {
            OnInteraction(interactionObj, m_simulator.SelectCards(interactionObj, this));
            return false;
        }

        [Interactions.MessageHandler(typeof(Interactions.MessageBox))]
        private bool OnMessageBox(Interactions.MessageBox interactionObj)
        {
            OnInteraction(interactionObj, m_simulator.MessageBox(interactionObj, this));
            return false;
        }

        #endregion

        #region notifications

        [Interactions.MessageHandler(typeof(Interactions.NotifyCardEvent))]
        private bool OnNotified(Interactions.NotifyCardEvent interactionObj)
        {
            interactionObj.Respond();
            return false;
        }

        [Interactions.MessageHandler(typeof(Interactions.NotifyGameEvent))]
        private bool OnNotified(Interactions.NotifyGameEvent interactionObj)
        {
            interactionObj.Respond();
            return false;
        }

        [Interactions.MessageHandler(typeof(Interactions.NotifyPlayerEvent))]
        private bool OnNotified(Interactions.NotifyPlayerEvent interactionObj)
        {
            interactionObj.Respond();
            return false;
        }

        [Interactions.MessageHandler(typeof(Interactions.NotifySpellEvent))]
        private bool OnNotified(Interactions.NotifySpellEvent interactionObj)
        {
            interactionObj.Respond();
            return false;
        }

        #endregion
    }
}
