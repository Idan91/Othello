using System;
using System.Text;

namespace Ex02_Console_Othello
{

    class GameEngine
    {
        private int m_PointBalancePlayer1 = 2; // Starts with 2 points
        private int m_PointBalancePlayer2 = 2; // Player 2 is human/CPU
        private char[,] m_Board;
        private int m_GameSize = 8; // defalut size
        private int m_RowToInsert;
        private int m_ColToInsert;
        private StringBuilder m_IndicesToTurnOver;
        private StringBuilder m_OptionalIndicesToTurnOver;
        private StringBuilder m_OptionalCells;
        private StringBuilder m_TakenCells;

        private char m_CurrentPlayerSymbol;
        private char m_CurrentOpponentSymbol;
        private Borders m_BordersFromTableEdge;
        private struct Borders
        {
            public int RowLow;
            public int RowHigh;
            public int ColLeft;
            public int ColRight;

            public void Initialize()
            {
                RowHigh = -1;
                RowLow = 1;
                ColLeft = -1;
                ColRight = 1;
            }
        }

        public GameEngine(int i_GameSize) //set default size
        {
            m_GameSize = i_GameSize;
            m_IndicesToTurnOver = new StringBuilder();
            m_OptionalIndicesToTurnOver = new StringBuilder();
            m_OptionalIndicesToTurnOver = new StringBuilder();
            m_OptionalCells = new StringBuilder();
            m_TakenCells = new StringBuilder();
            setupBoard();
        }

        public void InsertToBoard(char i_SymbolToInsert)
        {
            m_Board[m_RowToInsert, m_ColToInsert] = i_SymbolToInsert; // we set the row & col while validating
            m_TakenCells.Append(coordinatesToString(m_RowToInsert, m_ColToInsert));
            for (int i = 0; i < m_IndicesToTurnOver.Length; i += 2)
            {
                int row = int.Parse(m_IndicesToTurnOver[i].ToString());
                int col = int.Parse(m_IndicesToTurnOver[i + 1].ToString());
                m_Board[row, col] = i_SymbolToInsert;
            }
            calcPoints();
        }

        public bool IsLegalMove(char i_SymbolToInsert, string i_StringInputToCheck)
        {
            m_CurrentPlayerSymbol = i_SymbolToInsert;
            bool v_ValidCell = false;
            if (inputStringValidation(i_StringInputToCheck))
            {
                setTurnSettings();
                if (legalCell()) // return false if couldn't turn over any opponent cells
                {
                    v_ValidCell = true;
                }
            }
            return v_ValidCell;
        }

        public string GetBoard()
        {
            string flatBoard = null;
            foreach (char board in m_Board)
            {
                flatBoard += board.ToString();
            }
            return flatBoard;
        }

        public int Player1Points
        {
            get
            {
                return m_PointBalancePlayer1;
            }
        }

        public int Player2Points
        {
            get
            {
                return m_PointBalancePlayer2;
            }
        }

        public void ResetBoard()
        {
            setupBoard();
            m_PointBalancePlayer1 = 2;
            m_PointBalancePlayer2 = 2;
        }

        public bool AvailableMovesForPlayer(char i_PlayerSymbol)
        {
            bool v_AnyOptionalMove = false;
            if (optionalCells(i_PlayerSymbol).Length != 0)
            {
                v_AnyOptionalMove = true;
            }
            return v_AnyOptionalMove;
        }

        private bool inputStringValidation(string i_StringInputToCheck)
        {
            bool v_ValidCell = false;
            if (i_StringInputToCheck != null)
            {
                if (i_StringInputToCheck.Length == 2 && char.IsDigit(i_StringInputToCheck[1]))
                {
                    if (char.IsLower(i_StringInputToCheck[0]))
                    {
                        i_StringInputToCheck = char.ToUpper(i_StringInputToCheck[0]).ToString() +
                            i_StringInputToCheck[1].ToString();
                    }
                    bool v_VaildColLetter = (i_StringInputToCheck[0] >= 'A' &&
                i_StringInputToCheck[0] < 'A' + m_GameSize);
                    int digitToCheck = int.Parse(i_StringInputToCheck[1].ToString());
                    bool v_VaildRowNumber = (digitToCheck >= 1 && digitToCheck <= m_GameSize);
                    if (v_VaildColLetter)
                    {
                        if (v_VaildRowNumber)
                        {
                            stringToCoordinates(i_StringInputToCheck);
                            if (m_Board[m_RowToInsert, m_ColToInsert] == ' ') // the cell is free
                            {
                                v_ValidCell = true;
                            }
                        }
                    }
                }
            }
            return v_ValidCell;
        }

        private void setupBoard()
        {
            m_Board = new char[m_GameSize, m_GameSize];
            for (int i = 0; i < m_GameSize; i++)
            {
                for (int j = 0; j < m_GameSize; j++)
                {
                    m_Board[i, j] = ' ';
                }
            }
            m_Board[m_GameSize / 2, m_GameSize / 2] = 'O';
            m_Board[m_GameSize / 2, m_GameSize / 2 - 1] = 'X';
            m_Board[m_GameSize / 2 - 1, m_GameSize / 2] = 'X';
            m_Board[m_GameSize / 2 - 1, m_GameSize / 2 - 1] = 'O';
            m_TakenCells.Append(coordinatesToString(m_GameSize / 2, m_GameSize / 2));
            m_TakenCells.Append(coordinatesToString(m_GameSize / 2, m_GameSize / 2 - 1));
            m_TakenCells.Append(coordinatesToString(m_GameSize / 2 - 1, m_GameSize / 2));
            m_TakenCells.Append(coordinatesToString(m_GameSize / 2 - 1, m_GameSize / 2 - 1));

        }

        private void setTurnSettings()
        {
            m_IndicesToTurnOver.Clear();
            setPlayersSymbols();
            m_BordersFromTableEdge.Initialize();
            if (m_RowToInsert == 0)
            {
                m_BordersFromTableEdge.RowHigh = 0; // Automatically initialized to zero - for readability
            }
            else if (m_RowToInsert == m_GameSize - 1)
            {
                m_BordersFromTableEdge.RowLow = 0;
            }
            if (m_ColToInsert == 0)
            {
                m_BordersFromTableEdge.ColLeft = 0; // Automatically initialized to zero for readability
            }
            else if (m_ColToInsert == m_GameSize - 1)
            {
                m_BordersFromTableEdge.ColRight = 0;
            }
        }

        private void setPlayersSymbols()
        {
            if (m_CurrentPlayerSymbol == 'O')
            {
                m_CurrentOpponentSymbol = 'X';
            }
            else if (m_CurrentPlayerSymbol == 'X')
            {
                m_CurrentOpponentSymbol = 'O';
            }
        }

        private bool legalCell()
        {
            bool v_ValidCell = false;
            for (int i = m_BordersFromTableEdge.RowHigh; i <= m_BordersFromTableEdge.RowLow; i++)
            {
                for (int j = m_BordersFromTableEdge.ColLeft; j <= m_BordersFromTableEdge.ColRight; j++)
                {
                    bool v_InputCell = (i == 0 && j == 0);
                    if (!(v_InputCell))
                    {
                        int row = m_RowToInsert + i;
                        int col = m_ColToInsert + j;
                        if ((row >= 0 && row < m_GameSize) && (col >= 0 && col < m_GameSize))
                        {
                            if (m_Board[row, col] == m_CurrentOpponentSymbol) // potential direction
                            {
                                if (scanInDirection(row, col, i, j))
                                {
                                    v_ValidCell = true;
                                }
                            }
                        }
                    }
                }
            }
            return v_ValidCell;
        }

        private bool scanInDirection(int i_Row, int i_Col, int i_RowINC, int i_ColINC)
        {
            bool v_ValidCell = false;
            while (i_Row >= 0 && i_Row < m_GameSize && i_Col >= 0 && i_Col < m_GameSize && !v_ValidCell)
            {
                if (m_Board[i_Row, i_Col] == m_CurrentOpponentSymbol)
                {
                    m_OptionalIndicesToTurnOver.Append(i_Row);
                    m_OptionalIndicesToTurnOver.Append(i_Col);
                }
                else if (m_Board[i_Row, i_Col] == m_CurrentPlayerSymbol)
                {
                    v_ValidCell = true;
                    m_IndicesToTurnOver.Append(m_OptionalIndicesToTurnOver);
                }
                else if (m_Board[i_Row, i_Col] == ' ' || i_RowINC == 0 && i_ColINC == 0) // Empty cell OR nothing to increment by
                {
                    break;
                }
                i_Row += i_RowINC;
                i_Col += i_ColINC;
            }
            m_OptionalIndicesToTurnOver.Clear();
            return v_ValidCell;
        }

        private void stringToCoordinates(string i_InsertionCoordinateString) 
        {
            m_ColToInsert = i_InsertionCoordinateString[0] - 'A'; // A -> 0, B -> 1...@
            m_RowToInsert = int.Parse(i_InsertionCoordinateString[1].ToString()) - 1;
        }

        private string coordinatesToString(int i_RowToConvert, int i_ColToConvert)
        {
            int Col = 65 + int.Parse(i_ColToConvert.ToString());
            int Row = int.Parse(i_RowToConvert.ToString()) + 1;
            char cpuCol = Convert.ToChar(Col); // the coordinates of the random cell (COL)
            string cellString = (cpuCol.ToString() + Row.ToString()); // cell coordinates
            return cellString;
        }

        private void calcPoints()
        {
            if (m_IndicesToTurnOver.Length != 0)
            {
                int pointToChange = m_IndicesToTurnOver.Length / 2 + 1; // num of coordinates / 2 + one point for my spot
                if (m_CurrentPlayerSymbol == 'O')
                {
                    m_PointBalancePlayer1 += pointToChange;
                    m_PointBalancePlayer2 -= (pointToChange - 1);
                }
                else
                {
                    m_PointBalancePlayer1 -= (pointToChange - 1);
                    m_PointBalancePlayer2 += pointToChange;
                }
            }
        }

        private string optionalCells(char i_PlayerSymbol)
        {
            m_OptionalCells.Clear();
            for (int i = 0; i < m_GameSize; i++)
            {
                for (int j = 0; j < m_GameSize; j++)
                {
                    string potentialOptionalCell = coordinatesToString(i, j); // (row, col)
                    if (!m_TakenCells.ToString().Contains(potentialOptionalCell)) // place isnt taken yet
                    {
                        if (IsLegalMove(i_PlayerSymbol, potentialOptionalCell))
                        {
                            m_OptionalCells.Append(m_RowToInsert);
                            m_OptionalCells.Append(m_ColToInsert);
                        }
                    }
                }
            }
            return m_OptionalCells.ToString();
        }

        //CPU
        public void CPUTurn()
        {
            string optionalCells = m_OptionalCells.ToString();
            string cpuCell;
            cpuCell = AICPU.CPUCellPickAI(optionalCells, m_GameSize);
            //insert by AI
            if (cpuCell == null)
            {
                string maximumTurnOverCell = maxTurnOverCheck(optionalCells);
                if (maximumTurnOverCell.Length > 2) // more than one option
                {
                    cpuCell = cpuRandomCellPicker(maximumTurnOverCell);
                }
                else
                {
                    cpuCell = maximumTurnOverCell; // insert the cell with the maximum number of opponent cells to be turned over
                }
            }
            IsLegalMove('X', cpuCell); // used to set turn variables
            InsertToBoard('X');
        }

        private string maxTurnOverCheck(string i_IndicesToCheck)
        {
            string maximumTurnOverCell = null;
            int maxTurnOverCount = 0;
            for (int i = 0; i < i_IndicesToCheck.Length / 2; i++) 
            {
                int rowIndex = int.Parse(i_IndicesToCheck[i * 2].ToString());
                int colIndex = int.Parse(i_IndicesToCheck[i * 2 + 1].ToString());
                string potentialMaxTurnOverCell = coordinatesToString(rowIndex, colIndex);
                IsLegalMove('X', potentialMaxTurnOverCell);
                if(maxTurnOverCount < m_IndicesToTurnOver.Length)
                {
                    maxTurnOverCount = m_IndicesToTurnOver.Length;
                    maximumTurnOverCell = potentialMaxTurnOverCell;
                }
                else if (maxTurnOverCount == m_IndicesToTurnOver.Length) // allow randomization between all potential max turnover cells
                {
                    maximumTurnOverCell += potentialMaxTurnOverCell;
                }
            }
            return maximumTurnOverCell;
        }

        private string cpuRandomCellPicker(string i_OptionalCells)
        {
            Random rand = new Random();
            int randomIndex = rand.Next(i_OptionalCells.Length / 2); // generate random cell according to the number of potential indices
            randomIndex *= 2;
            string cpuCell = i_OptionalCells[randomIndex].ToString() +
                i_OptionalCells[randomIndex + 1].ToString();
            return cpuCell;
        }

        private class AICPU
        {
            private static string s_TopLeftEdge = "00";
            private static string s_TopRightEdge;
            private static string s_BottomLeftEdge;
            private static string s_BottomRightEdge;
            private static string s_TopLeftCell = "A1";
            private static string s_TopRightCell;
            private static string s_BottomLeftCell;
            private static string s_BottomRightCell;
            private static bool s_FirstTimeRuning = true; //set to true on first program runing to avoid setting variables more than once

            public static string CPUCellPickAI(string i_OptionalCells, int i_BoardSize)
            {
                if (s_FirstTimeRuning) 
                {
                    calcBoardEdge(i_BoardSize);
                    calcBoardCells(i_BoardSize);
                    s_FirstTimeRuning = false;
                }
                string AIstringInput = null;
                AIstringInput = validEdgeToInsert(i_OptionalCells); // return null if no edge is free
                return AIstringInput;
            }

            private static void calcBoardEdge(int i_BoardSize)
            {
                s_TopRightEdge = "0" + (i_BoardSize - 1).ToString();
                s_BottomLeftEdge = (i_BoardSize - 1).ToString() + "0";
                s_BottomRightEdge = (i_BoardSize - 1).ToString() + (i_BoardSize - 1).ToString();
            }

            private static void calcBoardCells(int i_BoardSize)
            {
                char maxSymbolCol = Convert.ToChar(65 + i_BoardSize - 1);
                s_TopRightCell = maxSymbolCol.ToString() + "1";
                s_BottomLeftCell = "A" + i_BoardSize .ToString();
                s_BottomRightCell = maxSymbolCol.ToString() + i_BoardSize.ToString();
            }

            private static string validEdgeToInsert(string i_OptionalCells)
            {
                string firstFreeEdgeCell = null;
                if (uniqueContains(i_OptionalCells, s_TopLeftEdge))
                {
                    firstFreeEdgeCell = s_TopLeftCell;
                }
                else if (uniqueContains(i_OptionalCells, s_TopRightEdge))
                {
                    firstFreeEdgeCell = s_TopRightCell;
                }
                else if (uniqueContains(i_OptionalCells, s_BottomLeftEdge))
                {
                    firstFreeEdgeCell = s_BottomLeftCell;
                }
                else if (uniqueContains(i_OptionalCells, s_BottomRightEdge))
                {
                    firstFreeEdgeCell = s_BottomRightCell;
                }
                return firstFreeEdgeCell;
            }

            private static bool uniqueContains(string i_StringToCheck, string i_Coordinates) //extended ad-hoc implementation of String.Contains
            {
                bool v_Contains = false;
                for (int i = 0; i < i_StringToCheck.Length / 2; i+=2) 
                {
                    if (i_Coordinates[0] == i_StringToCheck[i * 2])
                    {
                        if (i_Coordinates[1] == i_StringToCheck[i * 2 + 1])
                        {
                            v_Contains = true;
                        }
                    }
                }
                return v_Contains;
            }

        }
        
    }
}

