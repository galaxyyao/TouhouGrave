using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Network
{
    public partial class Client
    {
        private List<RemoteCommand> _remoteCommandQueue = new List<RemoteCommand>();

        public class RemoteCommand
        {
            public enum RemoteActionEnum
            {
                PlayCard,
                ActivateAssist,
                CastSpell,
                Sacrifice,
                Redeem,
                AttackCard,
                AttackPlayer,
                Pass,
                SelectCards,
                Abort
            }

            public RemoteActionEnum RemoteAction
            {
                get;
                set;
            }

            public int ResultSubjectIndex
            {
                get;
                set;
            }

            public int[] ResultParameters
            {
                get;
                set;
            }
        }

        public void RemoteCommandEnqueue(RemoteCommand command)
        {
            _remoteCommandQueue.Add(command);
        }

        public RemoteCommand RemoteCommandDequeue()
        {
            var result = _remoteCommandQueue.FirstOrDefault();
            if (_remoteCommandQueue.Count > 0)
            {
                _remoteCommandQueue.RemoveAt(0);
            }
            return result;
        }

        public void RemoteCommandClearQueue()
        {
            _remoteCommandQueue.Clear();
        }
    }
}
