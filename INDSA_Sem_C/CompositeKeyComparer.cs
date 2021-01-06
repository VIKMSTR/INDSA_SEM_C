using System.Collections.Generic;

namespace INDSA_Sem_C
{
    public class CompositeKeyComparer : IComparer<Node>
    {
        public int Compare(Node x, Node y)
        {
            if (x.Location.X < y.Location.X)
                return -1;
            if (x.Location.X > y.Location.X)
                return 1;

            if (x.Location.Y < y.Location.Y)
            {
                return -1;
            }
            if (x.Location.Y > y.Location.Y)
            {
                return 1;
            }
            return 0;
        }
    }
}