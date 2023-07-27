namespace SlotMaker
{
    /** 
        https://gist.github.com/michaelbartnett/5652076
    */
    public static class Tuple
    {
        public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
        {
            return new Tuple<T1, T2>(item1, item2);
        }

        public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3)
        {
            return new Tuple<T1, T2, T3>(item1, item2, item3);
        }

        public static Tuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            return new Tuple<T1, T2, T3, T4>(item1, item2, item3, item4);
        }
    }

    public sealed class Tuple<T1, T2>
    {
        private readonly T1 item1;
        private readonly T2 item2;

        public T1 Item1 
        {
            get { return item1; }
        }

        public T2 Item2 
        {
            get { return item2; }
        }

        public Tuple(T1 item1, T2 item2)
        {
            this.item1 = item1;
            this.item2 = item2;   
        }
    }

    public sealed class Tuple<T1, T2, T3>
    {
        private readonly T1 item1;
        private readonly T2 item2;
        private readonly T3 item3;

        public T1 Item1 
        {
            get { return item1; }
        }

        public T2 Item2 
        {
            get { return item2; }
        }

        public T3 Item3 
        {
            get { return item3; }
        }

        public Tuple(T1 item1, T2 item2, T3 item3)
        {
            this.item1 = item1;
            this.item2 = item2;   
            this.item3 = item3;
        }
    }

    public sealed class Tuple<T1, T2, T3, T4>
    {
        private readonly T1 item1;
        private readonly T2 item2;
        private readonly T3 item3;
        private readonly T4 item4;

        public T1 Item1 
        {
            get { return item1; }
        }

        public T2 Item2 
        {
            get { return item2; }
        }

        public T3 Item3 
        {
            get { return item3; }
        }

        public T4 Item4 
        {
            get { return item4; }
        }

        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            this.item1 = item1;
            this.item2 = item2;   
            this.item3 = item3;
            this.item4 = item4;
        }
    }
}