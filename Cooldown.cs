
namespace ProjectHook
{
    class Cooldown
    {
        public int Max;
        public int Count;

        public Cooldown(int max)
        {
            Max = max;
        }

        public bool IsOff()
        {
            return Count == 0;
        }

        public void Decrement()
        {
            if (Count > 0) Count--;
        }

        public void GoOnCooldown()
        {
            Count = Max;
        }

        public void Reset()
        {
            Count = 0;
        }
    }
}
