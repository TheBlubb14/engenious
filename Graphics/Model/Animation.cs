using System.Collections.Generic;
using System.Linq;
namespace engenious.Graphics
{
    public class Animation
    {
        public float Time{ get;internal set; }

        public float MaxTime{ get; set; }

        public List<AnimationNode> Channels{ get; set; }
        public void Precalculate()
        {
            foreach(var channel in Channels)
            {
                for(int i=0;i<channel.Frames.Count;i++)
                    channel.Precalculate(i);
            }
        }
        public void Update(float elapsed)
        {
            Time = elapsed;
            Time %= MaxTime;
            foreach (var channel in Channels)
                channel.ApplyAnimation(Time, MaxTime);
        }
    }
}

