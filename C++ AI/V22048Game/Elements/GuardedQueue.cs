using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V22048Game.Elements
{
    public class GuardedQueue<T> : Queue<T>
    {
        public delegate void ReleaseQueueItem(T value);

        public bool GuardIsOn { get; set; }

        private ReleaseQueueItem releaseItem;
        public ReleaseQueueItem ReleaseItem
        {
            get { return releaseItem; }
            set { releaseItem = value; }
        }

        public event EventHandler QueueEmptied;

        public GuardedQueue(ReleaseQueueItem releaseItem)
            : base()
        {
            this.ReleaseItem = releaseItem;
            //Initially, the queue is always ready
            GuardIsOn = false;
        }
        
        public new void Enqueue(T value)
        {
            if (this.Count == 0 && !this.GuardIsOn)
            {
                //Item can be immediately released if the guard is not on
                ReleaseItem(value);
                //Releasing an item causes the relevant updates further on
            }
            else
            {
                base.Enqueue(value);
            }
        }

        public new T Dequeue()
        {
            var value = base.Dequeue();
            //Any action which takes an element will block up whatever comes later
            this.GuardIsOn = true;
            if (this.Count == 0 && QueueEmptied != null)
                QueueEmptied(this, new EventArgs());
            return value;
        }

        public void Lock()
        {
            //If for any reason we need to artificially block the queue
            this.GuardIsOn = true;
        }

        public void Unlock()
        {
            //Unlocking means the next stage of the process wants to recieve an element:
            if (this.Count == 0)
            {
                //If there are no elements in the list, we can unguard the list
                this.GuardIsOn = false;
            }
            else
            {
                //There are elements in the list
                var item = this.Dequeue();
                //This provides the next element in the list, and re-locks the list
                this.GuardIsOn = true;
                ReleaseItem(item);
            }
        }
    }
}
