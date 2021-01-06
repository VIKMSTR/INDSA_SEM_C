using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using INDSA_Sem_C;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            List<Node> nodes = new List<Node>();
            NodeGenerator.Generate(5000, nodes);

            BlockManager bm = new BlockManager();
            bm.Build(nodes); // vystav strukturu
            bm.LoadMemoryBlock(49); //nacti memory block 5
       //     bm.TESTING_METHOD_REMOVE_AFTER_TESTING();

           // TODO: hledani (Dodelat binar, interpolacni)
           // TODO: odstranovani - kontanta jako prazdne misto ?
           // TODO: rebuild pri pridavani ?
            
            int a = 0;
            bm.SaveMemoryBlock();
            
            bm.LoadMemoryBlock(5);

            Node nn = null;
            int MemBlock = -1;

            bm.Search(1, new Point(597, 100), out nn, out MemBlock);

            int searchx = 0;
           searchx =  Convert.ToInt32(Console.ReadLine());
            bm.Search(1, new Point(searchx, 100), out nn, out MemBlock);
            bm.Search(0, new Point(900, 100),out nn,out MemBlock);

        }
    }
}
