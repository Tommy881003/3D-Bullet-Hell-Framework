namespace SpellBound.Core
{
    public class Stats
    {
        private int value;
        private int buffRatio;
        private int buffConst;

        public Stats(int value)
        {
            this.value = value;
            this.buffConst = 0;
            this.buffRatio = 0;
        }

        public void AddConst(int c)
        {
            this.buffConst += c;
        }

        public void AddRatio(int r)
        {
            this.buffRatio += r;
        }

        public int Value()
        {
            return this.value * (100 + this.buffRatio) / 100 + this.buffConst;
        }
    }
}

