using System;
using System.Text;

namespace Ex02_Console_Othello
{
    class GameConsoleUI
    {
        private GameManager m_Game;
        private string m_Player1Nickname;
        private string m_Player2Nickname;
        private string m_ColsTitle;
        private string m_LineSeparator;
        private bool m_PlayerVSPlayer = false;

        public GameConsoleUI()
        {
            setUpGameInfo();
            runGame();
        }

        /* Should both players have no move to make or should the user chose to quit the game,
        'GameIsRunning' will be set to false inside GameManager class*/
        private void runGame()
        {
            while (this.m_Game.GameIsRunning) 
            {
                clearAndDraw();
                userTurnMsg();
                userTurnInput();
            }
            exitGame();
        }
       
        private void exitGame()
        {
            Console.WriteLine(string.Format(@"
GAME FINISHED.
{0}",
endGameMsg()));
            bool v_ValidInput = false;
            string playerInput = "";
            while (!v_ValidInput)
            {
                Console.Write("Would you like to play another round? (Y / N): ");
                playerInput = Console.ReadLine();
                if (playerInput == "y" || playerInput == "Y")
                {
                    v_ValidInput = true;
                    m_Game.ResetGame();
                    runGame();
                }
                else if (playerInput == "n" || playerInput == "N")
                {
                    Console.WriteLine("Thank you for playing. Goodbye!" + Environment.NewLine);
                    v_ValidInput = true;             
                }
                else
                {
                    Console.WriteLine("Invalid input!");
                }
            }
        }

        private string endGameMsg()
        {
            clearAndDraw();
            StringBuilder endGameMsg = new StringBuilder();
            endGameMsg.AppendLine();
            int endGameStatus = m_Game.EndGameStatus();
            if (endGameStatus == 1)
            {
                endGameMsg.Append(m_Player1Nickname);
            }
            else if (endGameStatus == 2)
            {
                endGameMsg.Append(m_Player2Nickname);
            }
            else if (endGameStatus == 3) // there is an option for no winner if the user choses to exit
            {
                endGameMsg.Append("IT'S A DRAW.");
            }
            if (endGameStatus != 0)
            {
                endGameMsg.Append(" IS THE WINNER.");
                endGameMsg.AppendLine();
                endGameMsg.Append(Environment.NewLine + m_Player1Nickname + " POINTS: " + m_Game.GetPoints(1));
                endGameMsg.AppendLine();
                endGameMsg.Append(m_Player2Nickname + " POINTS: " + m_Game.GetPoints(2) + Environment.NewLine);
            }
            return endGameMsg.ToString();
        }

        private void clearAndDraw()
        {
            Ex02.ConsoleUtils.Screen.Clear();
            drawBoard();
        }

        private void drawBoard()
        {
            string boardStatus = m_Game.GetBoardStatus();
            Console.WriteLine();
            Console.WriteLine(m_ColsTitle);
            for (int i = 0; i < m_Game.BoardSize; i++)
            {
                Console.WriteLine(m_LineSeparator);
                Console.Write((i + 1).ToString());
                for (int j = 0; j < m_Game.BoardSize; j++)
                {
                    Console.Write(" | " + boardStatus[i * m_Game.BoardSize + j]);
                }
                Console.Write(" | ");
                Console.WriteLine();
            }
            Console.WriteLine(m_LineSeparator);
        }

        private void setUISettings()
        {
            //set cols, title and LineSeparator
            char letterRunner = 'A';
            m_LineSeparator = "  =";
            StringBuilder boardTitleBuilder = new StringBuilder(" ");
            for (int i = 0; i < m_Game.BoardSize; i++)
            {
                boardTitleBuilder.Append("   " + letterRunner.ToString());
                letterRunner++; // advance letter value for printing (A->B->C...)
                m_LineSeparator += "====";
            }
            m_ColsTitle = boardTitleBuilder.ToString();
        }

        private void setUpGameInfo()
        {
            Console.WriteLine("Welcome to Othello!" + Environment.NewLine);
            userSettingsInput();
            boardSettingsInput();
            setUISettings();
        }

        private void userSettingsInput()
        {
            bool v_ValidInput = false;
            string userChoiceInput;
            Console.Write("Enter Player 1 nickname: ");
            m_Player1Nickname = Console.ReadLine();
            while (!v_ValidInput)
            {
                Console.Write("Would you like to play VS CPU? (Y / N): ");
                userChoiceInput = Console.ReadLine();
                if (userChoiceInput == "y" || userChoiceInput == "Y") // play vs CPU
                {
                    v_ValidInput = true;
                    m_Player2Nickname = "CPU";
                }
                else if (userChoiceInput == "n" || userChoiceInput == "N") // play vs player
                {
                    Console.Write("Enter Player 2 nickname: ");
                    m_Player2Nickname = Console.ReadLine();
                    v_ValidInput = true;
                    m_PlayerVSPlayer = true;
                }
                else 
                {
                    Console.WriteLine("Invalid input!");
                }
            }
        }

        private void boardSettingsInput()
        {
            bool v_ValidInput = false;
            string userChoiceInput;
            int gameSize = 8;
            Console.WriteLine(Environment.NewLine + "Which board size would you like to play in?");
            while (!v_ValidInput)
            {
                Console.WriteLine("Enter 1 for (6X6)");
                Console.WriteLine("Enter 2 for (8X8)");
                Console.Write("Your Choice? : ");
                userChoiceInput = Console.ReadLine();
                if (userChoiceInput != "1" && userChoiceInput != "2")
                {
                    Console.WriteLine("Inavlid input!");
                }
                else if (userChoiceInput == "1") // 6x6 board
                {
                    v_ValidInput = true;
                    gameSize = 6;
                }
                else if (userChoiceInput == "2") // 8x8 board
                {
                    v_ValidInput = true;
                    gameSize = 8;
                }
            }
            m_Game = new GameManager(m_PlayerVSPlayer, gameSize);
        }

        private void userTurnInput()
        {
            if (m_PlayerVSPlayer == true || m_Game.PlayerTurn == 1) // skip CPU turn
            {
                bool v_ValidInput = false;
                while (!v_ValidInput)
                {
                    Console.Write("Enter Cell: ");
                    if (m_Game.PlayerInputToBoard(Console.ReadLine()))
                    {
                        v_ValidInput = true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input!");
                    }
                }
            }
            else
            {
                m_Game.CPUTurn();
            }
        }

        private void userTurnMsg()
        {
            string playerName = m_Player1Nickname;
            string opponentName = m_Player2Nickname;
            if (m_Game.PlayerTurn == 2)
            {
                playerName = m_Player2Nickname;
                opponentName = m_Player1Nickname;
            }
            if (!m_Game.AvailableMoves)
            {
                Console.WriteLine(opponentName + " has no moves to make! Turn skipped!");
            }
            if (m_Game.GameIsRunning)
            {
                Console.WriteLine(Environment.NewLine + m_Player1Nickname + ": " + m_Game.GetPoints(1));
                Console.WriteLine(m_Player2Nickname + ": " + m_Game.GetPoints(2));
                Console.WriteLine(Environment.NewLine + "It's " + playerName + "'s turn (" + m_Game.PlayerSymbol + ")");
            }
        }

    }
}