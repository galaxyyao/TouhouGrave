using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TouhouSpring.Agents
{
    class PlaybackAgent : LocalPlayerAgent
    {
        private StreamReader m_playingBack;

        public int RandomSeed
        {
            get; private set;
        }

        public PlaybackAgent(int pid)
            : base(pid)
        {
            m_playingBack = new StreamReader(new FileStream("record.txt", FileMode.Open, FileAccess.Read), Encoding.UTF8);
            RandomSeed = Int32.Parse(m_playingBack.ReadLine());
        }

        public override void OnTacticalPhase(Interactions.TacticalPhase io)
        {
            var respond = m_playingBack.ReadLine().Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            var verb = respond[0];
            var args = new int[respond.Length - 1];
            for (int i = 1; i < respond.Length; ++i)
            {
                args[i - 1] = Int32.Parse(respond[i]);
            }

            switch (verb)
            {
                case "pa":
                    io.RespondPass();
                    break;
                case "pl":
                    io.RespondPlay(io.PlayCardCandidates.First(c => c.Guid == args[0]));
                    break;
                case "ac":
                    io.RespondActivate(io.ActivateAssistCandidates.First(c => c.Guid == args[0]));
                    break;
                case "sa":
                    io.RespondSacrifice(io.SacrificeCandidates.First(c => c.Guid == args[0]));
                    break;
                case "re":
                    io.RespondRedeem(io.RedeemCandidates.First(c => c.Guid == args[0]));
                    break;
                case "ca":
                    io.RespondCast(io.CastSpellCandidates.First(c => c.Host.Guid == args[0] && c.Host.Behaviors[args[1]] == c));
                    break;
                case "atc":
                    io.RespondAttackCard(io.AttackerCandidates.First(c => c.Guid == args[0]), io.DefenderCandidates.First(c => c.Guid == args[1]));
                    break;
                case "atp":
                    io.RespondAttackPlayer(io.AttackerCandidates.First(c => c.Guid == args[0]), io.Game.Players[args[1]]);
                    break;
                default:
                    throw new NotSupportedException(String.Format("Unrecognized verb {0}", verb));
            }
        }

        public override void OnSelectCards(Interactions.SelectCards io)
        {
            var respond = m_playingBack.ReadLine().Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (respond[0] == "se")
            {
                CardInstance[] cards = new CardInstance[respond.Length - 1];
                for (int i = 1; i < respond.Length; ++i)
                {
                    cards[i - 1] = io.Candidates.First(c => c.Guid == Int32.Parse(respond[i]));
                }
                io.Respond(cards.ToIndexable());
            }
            else
            {
                throw new NotSupportedException(String.Format("Unrecognized verb {0}", respond[0]));
            }
        }

        public override void OnSelectNumber(Interactions.SelectNumber io)
        {
            var respond = m_playingBack.ReadLine().Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (respond[0] == "sn")
            {
                io.Respond(respond[1] == "null" ? (int?)null : Int32.Parse(respond[1]));
            }
            else
            {
                throw new NotSupportedException(String.Format("Unrecognized verb {0}", respond[0]));
            }
        }

        public override void OnSelectCardModel(Interactions.SelectCardModel io)
        {
            var respond = m_playingBack.ReadLine().Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (respond[0] == "sc")
            {
                io.Respond(respond[1] == "null" ? (ICardModel)null : io.Candidates[Int32.Parse(respond[1])]);
            }
            else
            {
                throw new NotSupportedException(String.Format("Unrecognized verb {0}", respond[0]));
            }
        }
    }
}
