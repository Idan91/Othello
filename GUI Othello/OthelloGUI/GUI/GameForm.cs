using Ex02_Console_Othello;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Othello.GUI
{
     internal class GameForm : Form
     {
        public int m_GameSize = 6;
        public bool m_PlayerVSPlayer = false;
        private const int k_ClientIncrementSize = 55;
        private const int k_CellSize = 40;
        private const char k_YellowSymbol = 'O';
        private const char k_RedSymbol = 'X';
        private const int k_YellowPlayerNumber = 1;
        private const int k_RedPlayerNumber = 2;
        private Bitmap m_RedCoin = OthelloGUI.Properties.Resources.CoinRed;
        private Bitmap m_YellowCoin = OthelloGUI.Properties.Resources.CoinYellow;
        private bool m_FirstTurn = false;
        private int m_IndexRowClicked;
        private int m_IndexColClicked;
        private int m_YellowTotalWins = 0;
        private int m_RedTotalWins = 0;
        private readonly PictureBox[ , ] r_Board;
        private readonly GameSettingsForm r_GameSettingsForm;
        private readonly GameManager r_Game;

        public void OnCellsChanged()
        {
            drawBoard();
        }

        public GameForm()
        {
            r_GameSettingsForm = new GameSettingsForm(out m_GameSize, out m_PlayerVSPlayer);
            if (r_GameSettingsForm.OptionSelected)
            {
                r_Game = new GameManager(m_PlayerVSPlayer, m_GameSize);
                r_Game.AddCellChangeListener(this.OnCellsChanged); //subscribe as listener to event
                r_Board = new PictureBox[m_GameSize, m_GameSize];
                initialize();  
            }
        }

        private void initialize()
        {
            //Form
            this.Width = m_GameSize * (k_ClientIncrementSize - m_GameSize / 2);
            this.Height = m_GameSize * (k_ClientIncrementSize - m_GameSize / 2);
            this.Text = "Othello";
            this.CenterToScreen();
            this.BackColor = Color.WhiteSmoke;
            this.MaximumSize = new Size(this.Width , this.Height);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            //Board
            initializeBoard();
            this.ShowDialog();
        }

        private void initializeBoard()
        {
            for (int i = 0; i < m_GameSize; i++)
            {
                for (int j = 0; j < m_GameSize; j++)
                {
                    r_Board[i, j] = new PictureBox();
                    r_Board[i, j].Width = k_CellSize;
                    r_Board[i, j].Height = k_CellSize;
                    r_Board[i, j].BackColor = Color.FloralWhite;
                    r_Board[i, j].Left = j * r_Board[i, j].Width + (this.ClientSize.Width - (m_GameSize * k_CellSize)) / 2;
                    r_Board[i, j].Top = i * r_Board[i, j].Width + (this.ClientSize.Height - (m_GameSize * k_CellSize)) / 2;
                    r_Board[i, j].SizeMode = PictureBoxSizeMode.StretchImage;
                    r_Board[i, j].BorderStyle = BorderStyle.Fixed3D;
                    r_Board[i, j].Click += onClickCell;
                    r_Board[i, j].Enabled = false;
                    r_Board[i, j].Name = i.ToString() + "," + j.ToString(); // set names to recognize cell by indices when clicked
                    this.Controls.Add(r_Board[i, j]);
                }
            }
            firstBoardDrawing();
        }

        private void firstBoardDrawing()
        {
            for (int i = 0; i < m_GameSize; i++)
            {
                for (int j = 0; j < m_GameSize; j++)
                {
                    r_Board[i, j].Image = null;
                }
            }
            r_Board[m_GameSize / 2 - 1, m_GameSize / 2].Image = m_RedCoin;
            r_Board[m_GameSize / 2 - 1, m_GameSize / 2 - 1].Image = m_YellowCoin;
            r_Board[m_GameSize / 2, m_GameSize / 2 - 1].Image = m_RedCoin;
            r_Board[m_GameSize / 2, m_GameSize / 2].Image = m_YellowCoin;
            //optionalcells
            firstTurnOptionalCells();
            //form title
            this.Text = "Othello - Yellow's Turn";
        }

        private void firstTurnOptionalCells()
        {
            r_Board[m_GameSize / 2 - 2, m_GameSize / 2].BackColor = Color.LawnGreen;
            r_Board[m_GameSize / 2 - 1, m_GameSize / 2 + 1].BackColor = Color.LawnGreen;
            r_Board[m_GameSize / 2, m_GameSize / 2 - 2].BackColor = Color.LawnGreen;
            r_Board[m_GameSize / 2 + 1, m_GameSize / 2 - 1].BackColor = Color.LawnGreen;
            r_Board[m_GameSize / 2 - 2, m_GameSize / 2].Enabled = true;
            r_Board[m_GameSize / 2 - 1, m_GameSize / 2 + 1].Enabled = true;
            r_Board[m_GameSize / 2, m_GameSize / 2 - 2].Enabled = true;
            r_Board[m_GameSize / 2 + 1, m_GameSize / 2 - 1].Enabled = true;
        }

        private void onClickCell(object sender, EventArgs e)
        {
            setIndexClicked((sender as PictureBox));
            if (!r_Game.PlayerInputToBoard(m_IndexRowClicked, m_IndexColClicked))
            {
                criticalError();
            }
            if (!r_Game.GameIsRunning)
            {
                endGame();
            }
            else if (!m_PlayerVSPlayer && !m_FirstTurn)
            {
                cpuTurn();
            }
            changeTitle();
            setOptionalCells();
        }

        private void rePaintCellBackground()
        {
            for (int i = 0; i < m_GameSize; i++)
            {
                for (int j = 0; j < m_GameSize; j++)
                {
                    r_Board[i, j].BackColor = Color.FloralWhite;
                }
            }
        }

        private void cpuTurn()
        {
            while (r_Game.PlayerTurn == k_RedPlayerNumber)
            {
                r_Game.CPUTurn();
                drawBoard();
                if (!r_Game.GameIsRunning)
                {
                    endGame();
                }
            }
        }

        private void setIndexClicked(PictureBox i_BoxToSearch)
        {
            string[] cellCoordinates = i_BoxToSearch.Name.Split(',');
            m_IndexRowClicked = int.Parse(cellCoordinates[0]);
            m_IndexColClicked = int.Parse(cellCoordinates[1]);
        }
        

        private void criticalError()
        {
            MessageBox.Show("CRITICAL ERROR!");
        }

        private void changeTitle()
        {
            if (r_Game.PlayerSymbol == k_YellowSymbol)
            {
                this.Text = "Othello - Yellow's Turn";
            }
            else
            {
                this.Text = "Othello - Red's Turn";
            }
        }

        private void endGame()
        {
            string winner = "Yellow Won!!";
            int yellowPoints = r_Game.GetPoints(k_YellowPlayerNumber);
            int redPoints = r_Game.GetPoints(k_RedPlayerNumber);
            if(yellowPoints > redPoints)
            {
                m_YellowTotalWins++;
            }
            else if (yellowPoints < redPoints)
            {
                winner = "Red Won!!";
                m_RedTotalWins++;
            }
            else
            {
                winner = "It's a draw!";
                m_YellowTotalWins++;
                m_RedTotalWins++;
            }
            DialogResult userDialogAnswer = MessageBox.Show(winner + "(" + yellowPoints + "/" + redPoints + ") ("
                + m_YellowTotalWins + "/" + m_RedTotalWins + ")" + Environment.NewLine + "Would you like to play another round ?",
                      "Othello", MessageBoxButtons.YesNo);
            if (userDialogAnswer == DialogResult.Yes)
            {
                resetGame();
            }
            else
            {
                closeApplication();
            }
        }

        private void closeApplication()
        {
            this.Close();
        }

        private void resetGame()
        {
            r_Game.ResetGame();
            firstBoardDrawing();
            m_FirstTurn = true;
        }

        private void setOptionalCells()
        {
            rePaintCellBackground();
            if (!m_FirstTurn)
            {
                string[] splitOptionalIndices = r_Game.GetOptionalCell().ToString().Split('.');
                foreach (string currentOptionalCell in splitOptionalIndices)
                {
                    if (currentOptionalCell.Length > 0)
                    {
                        string[] splitRowCol = currentOptionalCell.Split(',');
                        int optionalRow = int.Parse(splitRowCol[0].ToString());
                        int optionalCol = int.Parse(splitRowCol[1].ToString());
                        r_Board[optionalRow, optionalCol].BackColor = Color.LawnGreen;
                        r_Board[optionalRow, optionalCol].Enabled = true;
                    }
                }
            }
            else
            {
                m_FirstTurn = false;
                firstTurnOptionalCells();
            }
        }

        private void drawBoard()
        {
            string boardStatus = r_Game.GetBoardStatus();
            for (int i = 0; i < m_GameSize; i++)
            {
                for (int j = 0; j < m_GameSize; j++)
                {
                    int index = i * m_GameSize + j;
                    if (boardStatus[index] == k_RedSymbol) 
                    {
                        r_Board[i, j].Image = m_RedCoin;
                    }
                    else if(boardStatus[index] == k_YellowSymbol)
                    {
                        r_Board[i, j].Image = m_YellowCoin;
                    }
                    else
                    {
                        r_Board[i, j].Image = null;
                    }
                    r_Board[i, j].Enabled = false;
                }
            }
        }
    }
}
