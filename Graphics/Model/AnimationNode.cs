using System;
using System.Linq;
using System.Collections.Generic;

namespace engenious.Graphics
{
    public class AnimationNode
    {
        private bool _sorted = false;

        public Node Node{ get; set; }

        public AnimationNode Parent{get;set;}

        public List<AnimationFrame> Frames{ get; set; }

        public void Sort()
        {
            if (_sorted)
                return;
            _sorted = true;
            Frames = Frames.OrderBy(f => f.Frame).ToList();
        }
        internal AnimationTransform getTransform(float frameTime)
        {
            Sort();


            var frame = Frames.First(x => x.Frame == frameTime);
            if (frame.Calculated || Parent == null)
                return frame.Transform;

            frame.Transform = Parent.getTransform(frameTime) * frame.Transform;
            frame.Calculated = true;

            return frame.Transform;

        }
        public void Precalculate(int frame)
        {
            Sort();

            getTransform(Frames[frame].Frame);
        }
        public bool Repeat{get;set;}=true;

        public void ApplyAnimation(float time, float maxTime)
        {
            
            Sort();
            int frameIndex = Math.Max(Frames.FindIndex(f => f.Frame >= time)-1,0);
            AnimationFrame frame = Frames[frameIndex];
            AnimationFrame nextFrame=null;
            if (Repeat){
                nextFrame = Frames[(frameIndex + 1) % Frames.Count];
            }
            else if(frameIndex < Frames.Count-1)
                nextFrame = Frames[frameIndex + 1];
            else
                return;
            
            float diff = time-frame.Frame;
            float frameTime = nextFrame.Frame - frame.Frame;

            /*if (diff == 0)
            {
                Node.LocalTransform = frame.Transform.ToMatrix();
            }
            else if (diff > 0)*/
            float percent=0;//diff / frameTime;

            {
                //Matrix m1 = frame.Transform.ToMatrix();
                //Matrix m2 = nextFrame.Transform.ToMatrix();
                //Node.LocalTransform = Matrix.Lerp(m1,m2,percent);
                Node.LocalTransform =
                    frame
                        .Transform.ToMatrix(); //AnimationTransform.Lerp(frame.Transform, nextFrame.Transform, percent).ToMatrix();
            }

            if (Node.Name.Contains("$"))
            {
                //Console.WriteLine(frameIndex + " - " + percent);
            }
        }
    }
}

