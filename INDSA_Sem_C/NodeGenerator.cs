using System;
using System.Collections.Generic;
using System.Drawing;

namespace INDSA_Sem_C
{
    public class NodeGenerator
    {
        public static void Generate(int amount, List<Node> nodes)
        {
            Random random = new Random();
            Node n;
            for (int i = 0; i < amount; i++)
            {
                string druhstring = null;
                int druh = random.Next(0, 3);
                switch (druh)
                {
                    case 0:
                        druhstring = "K";
                        break;
                    case 1:
                        druhstring = "Z";
                        break;
                    case 2:
                        druhstring = "O";
                        break;
                }
                n = new Node(druhstring + random.Next(0,amount),new Point(random.Next(0,amount/5),random.Next(0,amount/5)));
                nodes.Add(n);
            }
            
        }

        public static string  Analyze(List<Node> nodes)
        {
            throw new NotImplementedException("Coming soon");
        }
    }
   
}