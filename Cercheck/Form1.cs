using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cercheck
{
    public partial class Form1 : Form
    {
        private Game Game { get; set; }
        private Point FirstClick { get; set; }
        private Point SecondClick { get; set; }

        private bool IsFirstClick = true;

        public Form1()
        {
            InitializeComponent();
            InitDataGrid();
        }

        public void InitDataGrid()
        {
            this.dataGridView1.Rows.Add(8);
            this.dataGridView1.DefaultCellStyle.SelectionBackColor = Color.Red;
            this.dataGridView1.DefaultCellStyle.NullValue = null;
            for (var q = 0; q < this.dataGridView1.Columns.Count; q++)
                this.dataGridView1.Columns[q].DefaultCellStyle.NullValue = null;
            var flag = false;
            for (var i = 0; i < this.dataGridView1.Rows.Count; i++)
            {
                this.dataGridView1.Rows[i].Height = 65;
                flag = !flag;
                for (var j = 0; j < this.dataGridView1.Columns.Count; j++)
                {                    
                    this.dataGridView1.Rows[i].Cells[j].Style.BackColor = flag ? Color.LightYellow : Color.SaddleBrown;
                    flag = !flag;
                }
            }
            Game = new Game(this.dataGridView1, this.label3, this.label4);
            var isWhite = true;
            for(var i = 2; i < 6; i++)
            {
                for(var j = 2; j < 6; j++)
                {
                    if (isWhite)
                        Game.WhiteCheckers.Add(new Point(j, i));
                    else
                        Game.BlackCheckers.Add(new Point(j, i));
                    isWhite = !isWhite;
                }
                isWhite = !isWhite;
            }
            Game.ArrangeCheckers();
            
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(IsFirstClick)
            {
                FirstClick = new Point(e.ColumnIndex, e.RowIndex);
            }
            else
            {
                SecondClick = new Point(e.ColumnIndex, e.RowIndex);
                Game.MakeMove(FirstClick, SecondClick, true);
                Game.DoAITurn();
            }
            IsFirstClick = !IsFirstClick;
        }


    }
}
