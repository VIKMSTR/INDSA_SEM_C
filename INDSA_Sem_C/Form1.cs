using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace INDSA_Sem_C
{
    public partial class Form1 : Form
    {
        private List<Node> nodes;
        private BlockManager bm;
        public int count;
        public const int ItemsToLoad = 50;
        public Form1()
        {
            InitializeComponent();
            nodes = new List<Node>();
            listView1.Columns.Add("ID");
            listView1.Columns.Add("Location");
            listView2.Columns.Add("Index bloku");
            listView2.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            count = 0;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox1.Invalidate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Semestrální práce C \n(c) Viktor Krejčíř, 2013");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            nodes.Clear();
            NodeGenerator.Generate(Convert.ToInt32(numericUpDown3.Value), nodes);
            textBox1.AppendText("Vygenerováno " + nodes.Count + " křižovatek. \n" );
            bm = new BlockManager("data.dat",nodes);
            AddItems();
        }

        private void AddItems()
        {
           int NodesTotal = nodes.Count;
     /*       foreach (Node n in nodes)
            {
                
                ListViewItem lvi = new ListViewItem();
                lvi.Text = n.ID;
                lvi.SubItems.Add(n.Location.ToString());
                listView1.Items.Add(lvi);

                //   listBox1.Items.Add(n);
            }*/
    if(count*ItemsToLoad < nodes.Count){
            for (int i = count*ItemsToLoad; i < count*ItemsToLoad + ItemsToLoad; i++)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = nodes[i].ID;
                lvi.SubItems.Add(nodes[i].Location.ToString());
                listView1.Items.Add(lvi);
            }
            count++;
    }
                listView1.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView1.Refresh();
        }

        private void BinarySearchButton_Click(object sender, EventArgs e)
        {
            Node n;
            int BlockIndex;
         //   bm.VisitedBlocks.Clear();
            if (bm.Search(0, new Point(Convert.ToInt32(numericUpDown1.Value), Convert.ToInt32(numericUpDown2.Value)), out n,
                          out BlockIndex))
            {
                List<int> visited = bm.VisitedBlocks;
                textBox1.AppendText("Křižovatka nalezena ! " + "\n");
                textBox1.AppendText(n.ID + "[" + n.Location.X + ", "+ n.Location.Y + "] " );
          //      textBox1.AppendText("Bloky:  ");
               /* foreach (int i in visited)
                {
                    textBox1.AppendText(i + ", ");
                }
                textBox1.AppendText("\n");*/
         //      bm.VisitedBlocks.Add(BlockIndex);
            }
            else
            {
                textBox1.AppendText("Křižovatka nenalezena :( \n");
            }
            FillVisitedBlocksList();
        }

        private void FillVisitedBlocksList()
        {
            listView2.Items.Clear();
            foreach (int block in bm.VisitedBlocks)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = block.ToString();
                listView2.Items.Add(lvi);
            }
        }

        private void InterpSearchButton_Click(object sender, EventArgs e)
        {
            Node n;
            int BlockIndex;
           // bm.VisitedBlocks.Clear();
            if (bm.Search(1, new Point(Convert.ToInt32(numericUpDown1.Value), Convert.ToInt32(numericUpDown2.Value)), out n,
                          out BlockIndex))
            {
                List<int> visited = bm.VisitedBlocks;
                textBox1.AppendText("Křižovatka nalezena !" + "\n" +
                                    n.ID + ", " + n.Location.ToString() + "\n");
             /*   foreach (int i in bm.VisitedBlocks)
                {
                    textBox1.AppendText(i + ", ");
                }
                textBox1.AppendText("\n");*/
                
            }
            else
            {
                textBox1.AppendText("Křižovatka nenalezena :( \n");
            }
            FillVisitedBlocksList();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (nodes != null)
            {
                AddItems();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (bm.Remove(new Point(Convert.ToInt32(numericUpDown1.Value), Convert.ToInt32(numericUpDown2.Value))))
            {
                textBox1.AppendText("Křižovatka odstraněna \n");
            }
            else
            {
                textBox1.AppendText("Křižovatka NEBYLA odstraněna \n");
            }
            
        }
    }
}
