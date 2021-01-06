using System;
using System.Drawing;
using INDSA_Sem_A.Graph;

namespace INDSA_Sem_C
{
    public class Node
    {
        private string _id;
        private const int IdMaximumSize = 10; //celkova velikost slozek je 19B
        private NodeType _nodeType;
        public string ID
        {
            get { return _id; }
            set
            {
               
                var tmp = value.ToCharArray();
                Array.Resize(ref tmp, IdMaximumSize);
                _id = string.Join("", tmp);
            }
        }

        public Point Location { get;  set; }

        public Node(string ID, Point Location)
        {
            this.ID = ID;
            this.Location = Location;

            if (ID.Contains("K") || ID.Contains("k"))
            {
                _nodeType = NodeType.Crossroads;
            }
            if (ID.Contains("Z") || ID.Contains("z"))
            {
                _nodeType = NodeType.BusStop;
            }
            if (ID.Contains("O") || ID.Contains("o"))
            {
                _nodeType = NodeType.RestingPlace;
            }

        }

        public void SetEmpty()
        {
            ID = "NULL";

        }

        public override string ToString()
        {
          //  string id = ID;
            return ID + " [" + Location.X + " : " + Location.Y + "] ";
        }
    }
}