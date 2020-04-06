using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Threading.Timer;

namespace Sudoku
{
    public partial class Sudoku : Form
    {
        
        int[][] sudoku = new int[][]
        {
        new int[] { 0, 0, 0, 8, 0, 1, 0, 0, 0 },
        new int[] { 0, 0, 0, 0, 0, 0, 0, 4, 3 },
        new int[] { 5, 0, 0, 0, 0, 0, 0, 0, 0 },
        new int[] { 0, 0, 0, 0, 7, 0, 8, 0, 0 },
        new int[] { 0, 0, 0, 0, 0, 0, 1, 0, 0 },
        new int[] { 0, 2, 0, 0, 3, 0, 0, 0, 0 },
        new int[] { 6, 0, 0, 0, 0, 0, 0, 7, 5 },
        new int[] { 0, 0, 3, 4, 0, 0, 0, 0, 0 },
        new int[] { 0, 0, 0, 2, 0, 0, 6, 0, 0 }
        };

        int[][] solution = new int[][]
        {
        new int[] { 2, 3, 7, 8, 4, 1, 5, 6, 9},
        new int[] { 1, 8, 6, 7, 9, 5, 2, 4, 3},
        new int[] { 5, 9, 4, 3, 2, 6, 7, 1, 8},
        new int[] { 3, 1, 5, 6, 7, 4, 8, 9, 2},
        new int[] { 4, 6, 9, 5, 8, 2, 1, 3, 7},
        new int[] { 7, 2, 8, 1, 3, 9, 4, 5, 6},
        new int[] { 6, 4, 2, 9, 1, 8, 3, 7, 5},
        new int[] { 8, 5, 3, 4, 6, 7, 9, 2, 1},
        new int[] { 9, 7, 1, 2 ,5 ,3 ,6 ,8 ,4}
        };

        public Sudoku()
        {
            InitializeComponent();
            ResizeForm();
            DisplaySudoku(sudoku);
            DisplayInstructions();
        }


        //numberframes graphics
        List<List<NumberFrame>> framesDisplayed = new List<List<NumberFrame>>();
        void DisplaySudoku(int[][] sudoku)
        {
            for (int r = 0; r < sudoku.Length; r++)
            {
                List<NumberFrame> row = new List<NumberFrame>();
                for (int c = 0; c < sudoku[r].Length; c++)
                {
                    //create numberframe
                    NumberFrame nf = GetNumberFrameTemplate(sudoku[r][c] == 0 ? "" : sudoku[r][c].ToString());
                    nf.Location = new Point((nf.Width * c), (nf.Height * r));
                    nf.Click += numberClick;
                    this.Controls.Add(nf);
                    row.Add(nf);
                    //create horizontal line
                    if (c == 0)
                        if (r == 2 || r == 5 || r == 8)
                        {
                            PictureBox line = new PictureBox();
                            line.BackColor = Color.Black;
                            line.Width = nf.Width * sudoku.Length;
                            line.Height = 4;
                            line.Location = new Point(0, (nf.Height * (r + 1)));
                            this.Controls.Add(line);
                        }
                    //create vertical line
                    if (r == 0)
                        if (c == 2 || c == 5)
                        {
                            PictureBox line = new PictureBox();
                            line.BackColor = Color.Black;
                            line.Width = 4;
                            line.Height = nf.Height * sudoku.Length;
                            line.Location = new Point((nf.Width * (c + 1)), 0);
                            this.Controls.Add(line);
                        }
                }
                    framesDisplayed.Add(row);
            }
        }
        static NumberFrame GetNumberFrameTemplate(string text)
        {
            NumberFrame nf = new NumberFrame();
            nf.Font = new Font("Consolas", 14);
            nf.BorderStyle = BorderStyle.FixedSingle;
            nf.Width = 40;
            nf.Height = 40;
            nf.Text = text;
            nf.TextAlign = ContentAlignment.MiddleCenter;

            return nf;
        }
       

        //messages graphics
        List<Label> messageTexts = new List<Label>();
        void DisplayInstructions()
        {
            Label line1 = GetTextLabelTemplate();
            line1.Text = "Try to solve it!";
            line1.Location = new Point(0, this.Height - (line1.Height * 4));
            messageTexts.Add(line1);
            this.Controls.Add(line1);

            Label line2 = GetTextLabelTemplate();
            line2.Text = "For algorithm solve: press space";
            line2.Location = new Point(0, line1.Location.Y + line2.Height);
            messageTexts.Add(line2);
            this.Controls.Add(line2);
        }
        Label GetTextLabelTemplate()
        {
            Label l = new Label();
            l.Font = new Font("Consolas", 12, FontStyle.Bold);
            l.Width = this.Width;
            l.BringToFront();
            return l;
        }
        void PrintSolvingMessage()
        {
            messageTexts[0].Text = "Solving...";
            messageTexts[1].Text = "";
            messageTexts[0].Refresh();
            messageTexts[1].Refresh();
        }
        void PrintSolvedMessage()
        {
            messageTexts[0].Text = "Solution found!";
            messageTexts[0].Refresh();
        }


        //displaying solving graphically
        void SolveSudokuLive(bool showGraphicsLive)
        {
            //clear the sudoku from player inputs
            for(int i = 0; i < framesDisplayed.Count;i++)
                for(int j = 0; j < framesDisplayed[i].Count; j++)
                {
                    if(sudoku[i][j] == 0)
                    {
                        var frame = framesDisplayed[i][j];
                        frame.Clear();
                        frame.AllowPlayerInput = false;
                        frame.Refresh();
                    }
                }

            PrintSolvingMessage();

            SolveSudoku(sudoku, showGraphicsLive);

            //show graphics after solved
            if (!showGraphicsLive)
                for (int i = 0; i < sudoku.Length; i++)
                    for (int j = 0; j < sudoku[i].Length; j++)
                        framesDisplayed[i][j].Text = sudoku[i][j].ToString();

            PrintSolvedMessage();

        }


        //solving logic
        private void SolveSudoku(int[][] grid, bool showGraphicsLive)
        {
            //if not solved, keep exploring choices
            if (!CheckIfSolved(grid))
            {
                //get the index of next zero
                int row = 0;
                int col = 0;
                bool indexFound = false;
                for (int r = 0; r < grid.Length; r++)
                {
                    for (int c = 0; c < grid[r].Length; c++)
                        if (grid[r][c] == 0)
                        {
                            indexFound = true;
                            row = r;
                            col = c;
                            break;
                        }
                    if (indexFound)
                        break;
                }

                for (int i = 1; i <= 9; i++)
                {
                    //make choice 
                    grid[row][col] = i;
                    if (showGraphicsLive)
                    {
                        framesDisplayed[row][col].Text = i.ToString();
                        framesDisplayed[row][col].Refresh();
                    }

                    if (IsValidChoice(row, col, grid))
                    {
                        SolveSudoku(grid, showGraphicsLive);
                        //stop exploring other branches if solution is found
                        if (CheckIfSolved(grid))
                            break;
                    }
                }
                //once done exploring undo the choice, unless sudoku is solved
                if (!CheckIfSolved(grid))
                {
                        grid[row][col] = 0;
                    if (showGraphicsLive)
                    {
                        framesDisplayed[row][col].Text = "";
                        framesDisplayed[row][col].Refresh(); 
                    }

                }
            }


        }
        static bool IsValidChoice(int row, int col, int[][] grid)
        {
            int choice = grid[row][col];
            bool valid = true;
            //check vertically, horizontally and in box

            //check horizontal
            for (int i = 0; i < grid[row].Length; i++)
                if (i != col)
                    if (grid[row][i] == choice)
                    {
                        valid = false;
                        break;
                    }

            if (valid)
                //check vertical
                for(int i = 0; i < grid.Length; i++)
                {
                    if(i != row)
                        if(grid[i][col] == choice)
                        {
                            valid = false;
                            break;
                        }
                }

            if (valid)
            {
                //check in box
                List<int> box = GetBox(grid, row, col);
                //check for duplicates of choice in box
                int choiceCounter = 0;
                foreach (int n in box)
                    if (n == choice)
                    {
                        choiceCounter++;
                        if (choiceCounter > 1)
                        {
                            valid = false;
                            break;
                        }
                    }    
            }
                
                return valid;
        }
        static List<int> GetBox(int[][] grid, int row, int col)
        {
            List<int> box = new List<int>();

            //get row range
            int fromRow = 0;
            int toRow = 0;
            if (row <= 2)
            {
                toRow = 2;
            }
            else if(row <= 5)
            {
                fromRow = 3;
                toRow = 5;
            }
            else
            {
                fromRow = 6;
                toRow = 8;
            }

            //get col range 
            int fromCol = 0;
            int toCol = 0;
            if (col <= 2)
                toCol = 2;
            else if(col <= 5)
            {
                fromCol = 3;
                toCol = 5;
            }
            else
            {
                fromCol = 6;
                toCol = 8;
            }

            for (int i = fromRow; i <= toRow; i++)
                for (int j = fromCol; j <= toCol; j++)
                    box.Add(grid[i][j]);

            return box;
        }
        static bool CheckIfSolved(int[][] grid)
        {
            //check for zeroes in grid
            bool solved = true;
            foreach(int[] ar in grid)
                foreach(int n in ar)
                    if(n == 0)
                    {
                        solved = false;
                        break;
                    }

            return solved;
        }


        //user inputs
        NumberFrame Selected = new NumberFrame();
        private void numberClick(object sender, EventArgs e)
        {
            
            var number = (NumberFrame)sender;
            if (number.AllowPlayerInput)
            {
                //reset 
                Selected.DeFocus();
                Selected = number;
                //new focus
                number.Focus();
            }
        }
        private void User_KeyDown(object sender, KeyEventArgs e)
        {
            //algorithm solve start
            if (e.KeyCode.ToString() == "Space")
            {
                DialogResult choice = MessageBox.Show("Do you want to watch the algorithm solve the sudoku?                   (warning, watching the algorithm slows the process down!)", "With or without graphics", MessageBoxButtons.YesNo);
                if (choice == DialogResult.Yes)
                    SolveSudokuLive(true);
                else
                    SolveSudokuLive(false);
                    
            }

            //player guess input
            else if (e.KeyCode.ToString() == "D1"
                || e.KeyCode.ToString() == "D2"
                || e.KeyCode.ToString() == "D3"
                || e.KeyCode.ToString() == "D4"
                || e.KeyCode.ToString() == "D5"
                || e.KeyCode.ToString() == "D6"
                || e.KeyCode.ToString() == "D7"
                || e.KeyCode.ToString() == "D8"
                || e.KeyCode.ToString() == "D9")
            {
                string guess = e.KeyCode.ToString().Substring(1);
                //get NumberFrame currently selected
                for (int i = 0; i < framesDisplayed.Count; i++)
                    for (int j = 0; j < framesDisplayed[i].Count; j++)
                        if (framesDisplayed[i][j].IsSelected)
                        {
                            var frame = framesDisplayed[i][j];
                            //check if right answer
                            if (solution[i][j].ToString() == guess)
                                frame.MarkRightAnswer(guess);
                            else
                                frame.MarkWrongAnswer();
                            break;
                        }
            }

        }


        void ResizeForm()
        {
            this.Width = GetNumberFrameTemplate("").Width * (sudoku.Length) + 21;
            this.Height = Width + 100;
        }

    }
}
