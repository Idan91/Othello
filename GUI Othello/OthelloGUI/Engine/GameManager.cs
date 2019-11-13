
namespace Ex02_Console_Othello
{
    public class GameManager
    {
        private int m_GameSize;
        private readonly GameEngine r_Engine;
        private bool m_PlayerVSPlayer = false; //false for vs CPU, true for vs another human player 
        private const int k_PlayerOne = 1;
        private const int k_PlayerTwo = 2;
        private int m_PlayerTurn = k_PlayerOne;
        private const char k_PlayerOneSymbol = 'O';
        private const char k_PlayerTwoSymbol = 'X';
        private char m_CurrentPlayerSymbol = k_PlayerOneSymbol;
        private char m_OpponentSymbol = k_PlayerTwoSymbol;
        private bool m_IsGameRunning = false;
        private bool m_AvailableMoves = false;

        private enum eWinners : byte
        {
            NoOne, // if user chooses to exit
            Player1,
            Player2,
            Draw
        }

        public GameManager(bool i_PlayerVSPlayer, int i_GameSize)
        {
            m_GameSize = i_GameSize;
            m_PlayerVSPlayer = i_PlayerVSPlayer;
            r_Engine = new GameEngine(m_GameSize);
            m_IsGameRunning = true;
            m_AvailableMoves = true;
        }

        public void AddCellChangeListener(CellChangeEventHandler i_Method)
        {
            r_Engine.CellsChanged += i_Method;
        }

        public string GetBoardStatus()
        {
            return r_Engine.GetBoard();
        }

        public void EndGame()
        {
            m_IsGameRunning = false;
        }

        public void ResetGame()
        {
            m_IsGameRunning = true;
            m_AvailableMoves = true;
            m_PlayerTurn = k_PlayerOne;
            m_CurrentPlayerSymbol = k_PlayerOneSymbol;
            m_OpponentSymbol = k_PlayerTwoSymbol;
            r_Engine.ResetBoard();
        }

        public int EndGameStatus()
        {
            int winnerStatus = (int)eWinners.NoOne;
            if (!m_AvailableMoves)
            {
                if(GetPoints((int)eWinners.Player1) > GetPoints((int)eWinners.Player2))
                {
                    winnerStatus = (int)eWinners.Player1;
                }
                else if (GetPoints((int)eWinners.Player1) == GetPoints((int)eWinners.Player2))
                {
                    winnerStatus = (int)eWinners.Draw;
                }
                else
                {
                    winnerStatus = (int)eWinners.Player2;
                }
            }
            return winnerStatus;
        }

        public int BoardSize
        {
            get
            {
                return m_GameSize;
            }
            set
            {
                m_GameSize = value;
            }
        }

        public bool GameIsRunning
        {
            get
            {
                return m_IsGameRunning;
            }
        }

        public bool AvailableMoves
        {
            get
            {
                return m_AvailableMoves;
            }
        }

        public int PlayerTurn
        {
            get
            {
                return m_PlayerTurn;
            }
            set
            {
                m_PlayerTurn = value;
            }
        }

        public char PlayerSymbol
        {
            get
            {
                return m_CurrentPlayerSymbol;
            }
            set
            {
                m_CurrentPlayerSymbol = value;
            }
        }

        public int GetPoints(int i_Player)
        {
            int playerPoints = 0;
            if (i_Player == k_PlayerOne)
            {
                playerPoints = r_Engine.Player1Points;
            }
            else
            {
                playerPoints = r_Engine.Player2Points;

            }
            return playerPoints;
        }

        public bool PlayerInputToBoard(int i_RowToInsert, int i_ColToInsert)
        {            
            bool v_ValidInput = false;
            string userInput = r_Engine.CoordinatesToString(i_RowToInsert, i_ColToInsert);
            if (inputValidation(userInput))
            {
                r_Engine.InsertToBoard(PlayerSymbol);
                ChangePlayerTurn();
                availableMoveCheck();  // set variables for next turn
                v_ValidInput = true;
            }
            return v_ValidInput;
        }

        public void CPUTurn()
        {
            r_Engine.CPUTurn();
            ChangePlayerTurn();
            availableMoveCheck();
        }

        public string GetOptionalCell()
        {
            return r_Engine.GetOptionalCells;
        }

        private bool inputValidation(string i_StringToCheck)
        {
            return r_Engine.IsLegalMove(m_CurrentPlayerSymbol, i_StringToCheck);
        }

        private void availableMoveCheck()
        {
            if (!r_Engine.AvailableMovesForPlayer(PlayerSymbol))
            {
                m_AvailableMoves = false;
                if (!r_Engine.AvailableMovesForPlayer(m_OpponentSymbol)) // Both players have no legal moves
                {
                    EndGame();
                }
                else // Skip player turn
                {
                    ChangePlayerTurn();
                }
            }
            else
            {
                m_AvailableMoves = true;
            }
        }

        public void ChangePlayerTurn()
        {
            if (m_PlayerTurn == k_PlayerOne)
            {
                m_PlayerTurn = k_PlayerTwo;
                m_CurrentPlayerSymbol = k_PlayerTwoSymbol;
                m_OpponentSymbol = k_PlayerOneSymbol;
            }
            else
            {
                m_PlayerTurn = k_PlayerOne;
                m_CurrentPlayerSymbol = k_PlayerOneSymbol;
                m_OpponentSymbol = k_PlayerTwoSymbol;
            }
        }

    }
}
