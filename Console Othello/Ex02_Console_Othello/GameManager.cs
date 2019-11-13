
namespace Ex02_Console_Othello
{
    class GameManager
    {
        private int m_GameSize;
        private GameEngine m_Engine;
        private bool m_PlayerVSPlayer = false; //false for vs CPU, true for vs another human player 
        private int m_PlayerTurn = 1;
        private char m_CurrentPlayerSymbol = 'O';
        private char m_OpponentSymbol = 'X';
        private bool m_IsGameRunning = false;
        private bool v_AvailableMoves = false;

        private enum eWinners : byte
        {
            NoOne, // if user choses to exit
            Player1,
            Player2,
            Draw
        }

        public GameManager(bool i_PlayerVSPlayer, int i_GameSize)
        {
            m_GameSize = i_GameSize;
            m_PlayerVSPlayer = i_PlayerVSPlayer;
            m_Engine = new GameEngine(m_GameSize);
            m_IsGameRunning = true;
            v_AvailableMoves = true;
        }

        public string GetBoardStatus()
        {
            return m_Engine.GetBoard();
        }

        public void EndGame()
        {
            m_IsGameRunning = false;
        }

        public void ResetGame()
        {
            m_IsGameRunning = true;
            v_AvailableMoves = true;
            m_Engine.ResetBoard();
        }

        public int EndGameStatus()
        {
            int winnerStatus = (int)eWinners.NoOne;
            if (!v_AvailableMoves)
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
                return v_AvailableMoves;
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
            if (i_Player == 1)
            {
                playerPoints = m_Engine.Player1Points;
            }
            else
            {
                playerPoints = m_Engine.Player2Points;

            }
            return playerPoints;
        }

        public bool PlayerInputToBoard(string i_UserInput)
        {            
            bool v_ValidInput = true; // normally we initialize to false but here we want to avoid writing "= true" twice
            bool v_UserChoseToExit = (i_UserInput == "Q" || i_UserInput == "q");
            if (v_UserChoseToExit)
            {
                v_AvailableMoves = true; // to be sure that we quit by user choice
                EndGame();
            }
            else if (inputValidation(i_UserInput))
            {
                m_Engine.InsertToBoard(PlayerSymbol);
                changePlayerTurn();
                availableMoveCheck();  // set variables for next turn
            }
            else // Input is invalid
            {
                v_ValidInput = false;
            }
            return v_ValidInput;
        }

        public void CPUTurn()
        {
            m_Engine.CPUTurn();
            changePlayerTurn();
            availableMoveCheck();
        }

        private bool inputValidation(string i_StringToCheck)
        {
            return m_Engine.IsLegalMove(m_CurrentPlayerSymbol, i_StringToCheck);
        }

        private void availableMoveCheck()
        {
            if (!m_Engine.AvailableMovesForPlayer(PlayerSymbol))
            {
                v_AvailableMoves = false;
                if (!m_Engine.AvailableMovesForPlayer(m_OpponentSymbol)) // Both players have no legal moves
                {
                    EndGame();
                }
                else // Skip player turn
                {
                    changePlayerTurn();
                }
            }
            else 
            {
                v_AvailableMoves = true;
            }
        }

        private void changePlayerTurn()
        {

            if (m_PlayerTurn == 1)
            {
                m_PlayerTurn = 2;
                m_CurrentPlayerSymbol = 'X';
                m_OpponentSymbol = 'O';
            }
            else
            {
                m_PlayerTurn = 1;
                m_CurrentPlayerSymbol = 'O';
                m_OpponentSymbol = 'X';
            }
        }

    }
}
