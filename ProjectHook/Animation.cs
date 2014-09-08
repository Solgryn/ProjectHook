using System.Linq;

namespace ProjectHook
{
    public class Animation
    {
        public int Speed;
        public int[] Frames;

        public int Length
        {
            get { return Frames.Length; }
        }

        public Animation(int speed, int[] frames)
        {
            Speed = speed;
            Frames = frames;
        }

        public bool IsEqual(Animation other)
        {
            return Frames.SequenceEqual(other.Frames);
        }
    }
}
