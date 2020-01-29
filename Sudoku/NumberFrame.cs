using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku
{
    class NumberFrame : Label
    {
        
        public bool IsSelected { private set; get; }
        public bool AllowPlayerInput = true;
        public NumberFrame()
        {
            this.BackColor = Color.White;
        }
        public void Focus()
        {
            IsSelected = true;
            this.BackColor = Color.LightGray;
        }
        public void DeFocus()
        {
            IsSelected = false;
            this.BackColor = Color.White;
        }
        public void MarkWrongAnswer()
        {
            this.BackColor = Color.Red;
            this.Text = "";
        }
        public void MarkRightAnswer(string answer)
        {
            this.Text = answer;
            this.BackColor = Color.LightGreen;
        }
        public void Clear()
        {
            this.Text = "";
            this.BackColor = Color.White;
        }

    }
}
