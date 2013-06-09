using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    static class CommandQueue
    {
        public static void PushTail(Commands.BaseCommand item, ref Commands.BaseCommand head, ref Commands.BaseCommand tail)
        {
            Debug.Assert(item.Next == null);

            if (head == null)
            {
                Debug.Assert(tail == null);
                head = tail = item;
            }
            else
            {
                Debug.Assert(tail.Next == null);
                tail.Next = item;
                tail = item;
            }
        }

        public static Commands.BaseCommand PopHead(ref Commands.BaseCommand head, ref Commands.BaseCommand tail)
        {
            if (head == null)
            {
                Debug.Assert(tail == null);
                return null;
            }
            else
            {
                var ret = head;
                head = head.Next;
                if (head == null)
                {
                    Debug.Assert(tail == ret);
                    tail = null;
                }
                return ret;
            }
        }
    }
}
