using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace INDSA_Sem_C
{
    public class BlockManager
    {
        private const int BinaryOption = 0;
        private const int InterpolationOption = 1;

        private const int BlockSize = 100;
        private const int NodeSize = 19;
        private FileStream _carrier;
        private BinaryReader _br;
        private BinaryWriter _bw;
        private List<MemoryBlock> _blocks;
        private MemoryBlock _workingblock;
        private string _filepath;
        private int _maxX;
        public List<int> VisitedBlocks;
        
        
        #region CONSTRUCTORS

        public BlockManager(string filepath)
       {
           _carrier = new FileStream(filepath, FileMode.OpenOrCreate);
            _filepath = filepath;
           // vytvorit referenci na soubor
           // inicializovat kontext BW a BR
           _bw = new BinaryWriter(_carrier);
           _br = new BinaryReader(_carrier);
           // incializovat bloky
            _blocks = new List<MemoryBlock>();
       }

        public BlockManager(string filepath, List<Node> unsortedItems )
       {
           _carrier = new FileStream(filepath, FileMode.OpenOrCreate);
           _filepath = filepath;
           // vytvorit referenci na soubor
           // inicializovat kontext BW a BR
           _bw = new BinaryWriter(_carrier);
           _br = new BinaryReader(_carrier);
           // incializovat bloky
           _blocks = new List<MemoryBlock>();
           Build(unsortedItems);
       }

        public BlockManager()
        {
            _filepath = "data.dat";
           _carrier =  new FileStream(_filepath,FileMode.OpenOrCreate);
           // vytvorit referenci na soubor
           // inicializovat kontext BW a BR
           _bw = new BinaryWriter(_carrier);
           _br = new BinaryReader(_carrier);
           // incializovat bloky
           _blocks = new List<MemoryBlock>();
       }

        #endregion

        public void Build(List<Node> unsortedItems )
        {
           List<Node> sortedItems = new List<Node>(unsortedItems);
           sortedItems.Sort(new CompositeKeyComparer());
           Rewind();
           int NodeCounter = 0;
      //     int BlockCounter = 0;
            // ziskame prvky
            // seradime prvky
            //rozkouskuji po velikosti bloku na jednotlive bloky
           foreach (Node sortedItem in sortedItems)
           {
               if (NodeCounter%BlockSize == 0)
               {
                   int from = _blocks.Count*BlockSize*NodeSize;
                   MemoryBlock mb = new MemoryBlock(_blocks.Count,from,NodeSize);
                   mb.Xkey = sortedItem.Location.X;
                   mb.Ykey = sortedItem.Location.Y;
                   _blocks.Add(mb); //ty pridame do seznamu bloku

               }
               DecomposeNode(sortedItem);

               if (sortedItems[sortedItems.Count-1].Equals(sortedItem))
               {

                   _maxX = sortedItem.Location.X;
               }
                   NodeCounter++;
              
           }
          //  _carrier.Flush();

           Rewind();
           _carrier.Close();
        }

        private void Rewind()
        {
            _carrier.Seek(0, SeekOrigin.Begin); 
        }

        private void DecomposeNode(Node sortedItem)
        {
            _bw.Seek(0, SeekOrigin.Current);
            _bw.Write(sortedItem.ID);
            _bw.Write(sortedItem.Location.X);
            _bw.Write(sortedItem.Location.Y);
        }

        public bool Remove(Point removing)
        {
            Node n;
            int index;
           if (InterpolationSearch(removing, out n, out index))
           {
            
               int position = _workingblock.PositionInBlock;
               n =   _workingblock.nodes[position]; //nalezený prvek na dané pozici
                  if (n.ID.Contains("NULL"))
                  {
                      return false;
                  }
               n.ID = "NULL";
               SaveMemoryBlock();
               return true;
           }
            return false;
        }

        #region SEARCH METHODS
        private bool BinarySearch(Point find, out Node result, out int MemBlock_index)
        {
            
            int foundBlock = BinaryFindBlock( find);
            // }
            if (foundBlock == -1)
            {
                //nenalezeno
                result = null;
                MemBlock_index = -1;
                return false;
            }
            LoadMemoryBlock(foundBlock);
            Node retval;
            _workingblock.FindInBlock(find, out retval);

            if (retval == null)
            {
                result = null;
                MemBlock_index = -1;
                return false;
            }

            result = retval;
            MemBlock_index = foundBlock;
            return true;
            
        }

        private int BinaryFindBlock(Point find)
        {
            VisitedBlocks = new List<int>();
            int middleindex = _blocks.Count / 2;
            int leftindex = 0;
            int rightindex = _blocks.Count - 1;
         
          

            while (true)
            {

                VisitedBlocks.Add(middleindex);

                if (find.X < _blocks[middleindex].Xkey)
                {
                    rightindex = middleindex;
                    middleindex = leftindex + (middleindex - leftindex)/2;
                    
                }

                if (find.X > _blocks[middleindex].Xkey)
                {
                    if (middleindex + 1 != _blocks.Count)
                    {
                        // existuje nasledujici a muzem na nej sahhnout
                        if (find.X < _blocks[middleindex + 1].Xkey) // je mensi nez prvni x nasledujiciho
                        {
                            //je v middleindex bloku
                            
                             return middleindex;
                            // nacist a projit
                        }
                        // jit do prave casti ;
                        leftindex = middleindex;
                        middleindex = leftindex + (rightindex - leftindex)/2;
                    }
                    else
                    {
                        return middleindex;
                        //nasledujici tu neni, jsme na konci a bud bude v tomhle bloku nebo zadnym

                    }

                }
                if (find.X == _blocks[middleindex].Xkey)
                {
                    if (find.Y < _blocks[middleindex].Ykey)
                    {
                        if (middleindex - 1 >= 0)//existuje predchozi blok
                        {
                            VisitedBlocks.Add(middleindex-1);
                            return middleindex - 1;
                        }
                        // mohl by byt v predchozim, ale zadny neni => nenalezen
                        return -1;
                    }
                    if (find.Y > _blocks[middleindex].Ykey)
                    {
                        // neni mensi nez pocatecni Y tohoto bloku ->je v tomto bloku
                        return middleindex;
                    }
                    // je rovnej = je to prvni prvek
                    return middleindex;
                }


            }// konec cyklu
        }

        private bool InterpolationSearch(Point find, out Node result, out int MemBlock_index)
        {

            int foundBlock = InterpolationFindBlock(find);
            // }
            if (foundBlock == -1)
            {
                //nenalezeno
                result = null;
                MemBlock_index = -1;
                return false;
            }
            //   VisitedBlocks.Add(foundBlock);
            LoadMemoryBlock(foundBlock);
            Node retval;
            _workingblock.FindInBlock(find, out retval);

            if (retval == null)
            {
                result = null;
                MemBlock_index = -1;
                return false;
            }

            result = retval;
            MemBlock_index = foundBlock;
            return true;
            // vyhledat v bloku
            //    throw new NotImplementedException("Coming soon");
          
        }

        private int InterpolationFindBlock(Point find)
        {
            VisitedBlocks = new List<int>();
            int lowest = _blocks[0].Xkey;
            int highest = _maxX;
            int est;
            int leftindex, rightindex;
            leftindex = 0;
            rightindex = _blocks.Count - 1;
            while (true)
            {

                est =Convert.ToInt32(leftindex + (rightindex - leftindex) * ((double)(find.X - lowest) / (highest - lowest)));
               VisitedBlocks.Add(est);

                if (leftindex == 0 && rightindex == 1)
                {
                    return est;
                }
              if (est == _blocks.Count - 1) //odhad je na posledni blok
              {
                    
                  if (find.X > _blocks[est].Xkey && find.X < _maxX)
                  {
                      return est;
                  }

                  // není na posledním odhadovaném bloku
              }else{  
                if (find.X >= _blocks[est].Xkey && find.X < _blocks[est + 1].Xkey)
                {
                    return est; // nalezeno
                }
                if (find.X < _blocks[est].Xkey)
                {
                    rightindex = est;
                    highest = _blocks[rightindex].Xkey;
                    // jdeme doleva
                }
                if (find.X > _blocks[est].Xkey && find.X > _blocks[est + 1].Xkey)
                {
                    // jdeme doprava
                    leftindex = est;
                    lowest = _blocks[leftindex].Xkey;

                }

              }
            }
            
        }
        public bool Search(int method, Point find, out Node result, out int MemBlock_index)
        {
            if (method == BinaryOption)
            {
                return BinarySearch(find, out result, out MemBlock_index);
            }
            if (method == InterpolationOption)
            {
                return InterpolationSearch(find, out result, out MemBlock_index);

            }
            throw new ArgumentException("Invalid search method - enter 0 for binary search or 1 for interpolation search");
        }
        #endregion

        private void LoadMemoryBlock(int index) //zmenit na private po otestovani
        {
            _carrier = new FileStream(_filepath,FileMode.Open);
            _br = new BinaryReader(_carrier);
            MemoryBlock memoryBlock = _blocks[index];
            _workingblock = memoryBlock;
            _workingblock.SetWorkingBlock(BlockSize,_br); //nastavime blok na working
            _carrier.Close();
        }
        
        private void SaveMemoryBlock()
        {
            _carrier = new FileStream(_filepath,FileMode.Open,FileAccess.Write); //otevrit k zapisu
            _bw = new BinaryWriter(_carrier);
            _carrier.Seek(_workingblock.from, SeekOrigin.Begin);

          /*  for (int i = 0; i < BlockSize; i++)
            {
                Node n = _workingblock.nodes[i];
                _bw.Write(n.ID);
                _bw.Write(n.Location.X);
                _bw.Write(n.Location.Y);
            }*/

            _bw.Write(ConvertToBytes());
            _workingblock.UnsetWorkingBlock();
            _workingblock = null;
          
            _carrier.Close();
        }

      
        private byte[] ConvertToBytes()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            for (int i = 0; i < BlockSize; i++)
            {
                Node n = _workingblock.nodes[i];
                bw.Write(n.ID);
                bw.Write(n.Location.X);
                bw.Write(n.Location.Y);
            }

            return ms.GetBuffer();


        }

    }
    public class MemoryBlock
    {
        private int index;
        public int from;
        public int Xkey;
        public int Ykey;
        public Node[] nodes;
        private  int _elementSize;
        public int PositionInBlock { get; private set; }

        public MemoryBlock(int index, int from, int elementSize)
        {
            this.from = from;
            this.index = index;
            _elementSize = elementSize;
        }

        public Point GetStartingPoint()
        {
            return new Point(Xkey,Ykey);
        }

        private void BytesToNodes(byte[] bytes, int Blocksize )
        {
            MemoryStream ms;
            BinaryReader br;
            int actualSize;   
                ms = new MemoryStream(bytes);
            br = new BinaryReader(ms);

            //nacist do objektu

            actualSize = bytes.Length / (_elementSize);

            try
            {
                for (int i = 0; i < actualSize; i++)
                {
                    Node n;
                    string ID;
                    int x;
                    int y;

                    ID = br.ReadString();
                    x = br.ReadInt32();
                    y = br.ReadInt32();
                    n = new Node(ID, new Point(x, y));
                    nodes[i] = n;

                }
            }
            catch (EndOfStreamException)
            {
                //prekroceni 
            }
            
        } 
        
        public bool FindInBlock(Point find, out Node result)
        {
           
            for (int i = 0; i < nodes.Length;i++ )
            {
                if (nodes[i].Location.X == find.X && nodes[i].Location.Y == find.Y)
                {
                    if (nodes[i].ID.Contains("NULL")) //prazdny prvek
                    {
                        result = null;
                        return false;
                    }
                    result = nodes[i];
                    PositionInBlock = i;
                    
                    return true;
                }
            }
            result = null;
            return false;
        }

        public void SetWorkingBlock(int size, BinaryReader binaryReader)
        {
            nodes = new Node[size];
            binaryReader.BaseStream.Seek(from, SeekOrigin.Begin);

            BytesToNodes(binaryReader.ReadBytes(size * _elementSize),size);  // nacti blok dat

        /*    for (int i = 0; i < size; i++) //to je spatne - nacist najednou 
            {
                Node n;
                string ID;
                int x;
                int y;
               
               ID = binaryReader.ReadString();
                x = binaryReader.ReadInt32();
                y = binaryReader.ReadInt32();
                n = new Node(ID,new Point(x,y));
                nodes[i] = n;

            }
         * */
        }

        public void UnsetWorkingBlock()
        {
            nodes = null;
        }
    }
}