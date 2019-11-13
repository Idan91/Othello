using System;
using System.Text;

namespace Ex02_Console_Othello
{

    public delegate void CellChangeEventHandler();

    internal class GameEngine
    {
        private int m_PointBalancePlayer1 = 2; // Starts with 2 points
        private int m_PointBalancePlayer2 = 2; // Player 2 is human/CPU
        private char[,] r_Board;
        private int m_GameSize; // defalut size
        private int m_RowToInsert;
        private int m_ColToInsert;
        private const char k_PlayerOneSymbol = 'O';
        private const char k_PlayerTwoSymbol = 'X';
        private StringBuilder m_IndicesToTurnOver;
        private StringBuilder m_OptionalIndicesToTurnOver;
        private StringBuilder m_OptionalCells;
        private StringBuilder m_TakenCells;

        public event CellChangeEventHandler CellsChanged;

        protected virtual void OnCellsChanged()
        {
            if(CellsChanged != null)
            {
                CellsChanged.Invoke();
            }
        }

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
            r_Board[m_RowToInsert, m_ColToInsert] = i_SymbolToInsert; // we set the row & col while validating
            m_TakenCells.Append(CoordinatesToString(m_RowToInsert, m_ColToInsert));
            m_TakenCells.Append(',');
            string[] splitIndices = m_IndicesToTurnOver.ToString().Split('.');
            foreach (string currentOptionalCell in splitIndices)
            {
                if (currentOptionalCell.Length > 0)
                {
                    string[] splitRowCol = currentOptionalCell.Split(',');
                    int rowIndex = int.Parse(splitRowCol[0].ToString());
                    int colIndex = int.Parse(splitRowCol[1].ToString());
                    r_Board[rowIndex, colIndex] = i_SymbolToInsert;
                }
            }
            OnCellsChanged(); //informing cell change
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
            if (m_IndicesToTurnOver.Length > 0)
            {
                if (m_IndicesToTurnOver[m_IndicesToTurnOver.Length - 1] == '.')
                {
                    m_IndicesToTurnOver.Remove(m_IndicesToTurnOver.Length - 1, 1); // delete the last '.'
                }
            }
            return v_ValidCell;
        }

        public string GetBoard()
        {
            string flatBoard = string.Empty;
            foreach (char item in r_Board)
            {
                flatBoard += item.ToString();
            }
            return flatBoard;
        }

        public string GetOptionalCells
        {
            get
            {
                return m_OptionalCells.ToString();
            }
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
            m_IndicesToTurnOver.Clear();
            m_OptionalIndicesToTurnOver.Clear();
            m_OptionalIndicesToTurnOver.Clear();
            m_OptionalCells.Clear();
            m_TakenCells.Clear();
            setupBoard();
            m_RowToInsert = 0;
            m_ColToInsert = 0;
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
                if (i_StringInputToCheck.Length >= 2 && i_StringInputToCheck.Length <= 3)
                {
                    bool v_VaildColLetter = (i_StringInputToCheck[0] >= 'A' &&
                i_StringInputToCheck[0] < 'A' + m_GameSize);
                    if (v_VaildColLetter)
                    {
                        bool v_VaildRowNumber = false;
                        if (i_StringInputToCheck.Length == 2)
                        {
                            v_VaildRowNumber = (int.Parse(i_StringInputToCheck[1].ToString()) >= 1 &&
                                int.Parse(i_StringInputToCheck[1].ToString()) <= m_GameSize);
                        }
                        else // i_StringInputToCheck.Length = 3
                        {
                            int rowNumber = int.Parse(i_StringInputToCheck[1].ToString()) * 10 +
                                int.Parse(i_StringInputToCheck[2].ToString());
                            v_VaildRowNumber = (rowNumber >= 1 && rowNumber <= m_GameSize);
                        }
                        if (v_VaildRowNumber)
                        {
                            stringToCoordinates(i_StringInputToCheck);
                            if (r_Board[m_RowToInsert, m_ColToInsert] == ' ') // the cell is free
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
            r_Board = new char[m_GameSize, m_GameSize];
            for (int i = 0; i < m_GameSize; i++)
            {
                for (int j = 0; j < m_GameSize; j++)
                {
                    r_Board[i, j] = ' ';
                }
            }
            r_Board[m_GameSize / 2, m_GameSize / 2] = k_PlayerOneSymbol;
            r_Board[m_GameSize / 2, m_GameSize / 2 - 1] = k_PlayerTwoSymbol;
            r_Board[m_GameSize / 2 - 1, m_GameSize / 2] = k_PlayerTwoSymbol;
            r_Board[m_GameSize / 2 - 1, m_GameSize / 2 - 1] = k_PlayerOneSymbol;
            m_TakenCells.Append(CoordinatesToString(m_GameSize / 2, m_GameSize / 2));
            m_TakenCells.Append(CoordinatesToString(m_GameSize / 2, m_GameSize / 2 - 1));
            m_TakenCells.Append(CoordinatesToString(m_GameSize / 2 - 1, m_GameSize / 2));
            m_TakenCells.Append(CoordinatesToString(m_GameSize / 2 - 1, m_GameSize / 2 - 1));
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
            if (m_CurrentPlayerSymbol == k_PlayerOneSymbol)
            {
                m_CurrentOpponentSymbol = k_PlayerTwoSymbol;
            }
            else if (m_CurrentPlayerSymbol == k_PlayerTwoSymbol)
            {
                m_CurrentOpponentSymbol = k_PlayerOneSymbol;
            }
        }

        private bool legalCell()
        {
            bool v_ValidCell = false;
            for (int i = m_BordersFromTableEdge.RowHigh; i <= m_BordersFromTableEdge.RowLow; i++)
            {
                for (int j = m_BordersFromTableEdge.ColLeft; j <= m_BordersFromTableEdge.ColRight; j++)
                {
                    bool v_InputCell = (i == 0 && j == 0); // if they are both '0' this is my cell
                    if (!v_InputCell)
                    {
                        int row = m_RowToInsert + i;
                        int col = m_ColToInsert + j;
                        if ((row >= 0 && row < m_GameSize) && (col >= 0 && col < m_GameSize))
                        {
                            if (r_Board[row, col] == m_CurrentOpponentSymbol) // potential direction
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
                if (r_Board[i_Row, i_Col] == m_CurrentOpponentSymbol)
                {
                    m_OptionalIndicesToTurnOver.Append(i_Row);
                    m_OptionalIndicesToTurnOver.Append(',');
                    m_OptionalIndicesToTurnOver.Append(i_Col);
                    m_OptionalIndicesToTurnOver.Append('.');
                }
                else if (r_Board[i_Row, i_Col] == m_CurrentPlayerSymbol)
                {
                    v_ValidCell = true;
                    m_IndicesToTurnOver.Append(m_OptionalIndicesToTurnOver);
                }
                else if (r_Board[i_Row, i_Col] == ' ' || i_RowINC == 0 && i_ColINC == 0) // Empty cell OR nothing to increment by
                {
                    break;
                }
                i_Row += i_RowINC;
                i_Col += i_ColINC;
            }
            m_OptionalIndicesToTurnOver.Clear();
            return v_ValidCell;
        }

        private void stringToCoordinates(string i_InsertCoordinateString) 
        {
            m_ColToInsert = i_InsertCoordinateString[0] - 'A'; // A -> 0, B -> 1...@
            if (i_InsertCoordinateString.Length == 3) // if row bigger then 9
            {
                string tmpRowString = i_InsertCoordinateString[1].ToString() + i_InsertCoordinateString[2].ToString();
                m_RowToInsert = int.Parse(tmpRowString) - 1;
            }
            else
            {
                m_RowToInsert = int.Parse(i_InsertCoordinateString[1].ToString()) - 1;
            }
        }

        public string CoordinatesToString(int i_RowToConvert, int i_ColToConvert)
        {
            int colToConvert = 65 + int.Parse(i_ColToConvert.ToString());
            int newRow = int.Parse(i_RowToConvert.ToString()) + 1;
            char newCol = Convert.ToChar(colToConvert); // the coordinates of the random cell (COL)
            string cellString = (newCol.ToString() + newRow.ToString()); // cell coordinates
            return cellString;
        }

        private void calcPoints()
        {
            if (m_IndicesToTurnOver.Length != 0)
            {
                string[] cellsThatChanged = m_IndicesToTurnOver.ToString().Split('.');
                int pointToChange = cellsThatChanged.Length + 1; // num of coordinates + one point for my cell
                if (m_CurrentPlayerSymbol == k_PlayerOneSymbol)
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
                    string potentialOptionalCell = CoordinatesToString(i, j); // (row, col)
                    StringBuilder checkForTakenCell = new StringBuilder();
                    checkForTakenCell.Append(potentialOptionalCell);
                    checkForTakenCell.Append(',');
                    if (!m_TakenCells.ToString().Contains(checkForTakenCell.ToString())) // cell hasn't been taken yet
                    {
                        if (IsLegalMove(i_PlayerSymbol, potentialOptionalCell))
                        {
                            m_OptionalCells.Append(m_RowToInsert);
                            m_OptionalCells.Append(",");
                            m_OptionalCells.Append(m_ColToInsert);
                            m_OptionalCells.Append(".");
                        }
                    }
                }
            }
            if (m_OptionalCells.Length > 0)
            {
                if (m_OptionalCells[m_OptionalCells.Length - 1] == '.')
                {
                    m_OptionalCells.Remove(m_OptionalCells.Length - 1, 1); // delete the last '.'
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
                if (maximumTurnOverCell != null)
                {
                    if (maximumTurnOverCell.Length > 2) // more than one option
                    {
                        cpuCell = cpuRandomCellPicker(maximumTurnOverCell);
                    }
                    else
                    {
                        cpuCell = maximumTurnOverCell; // insert the cell with the maximum number of opponent cells to be turned over
                    }
                }
            }
            if (cpuCell != null)
            {
                IsLegalMove(k_PlayerTwoSymbol, cpuCell);// used to set turn variables
                InsertToBoard(k_PlayerTwoSymbol);
            }
        }

        private string maxTurnOverCheck(string i_IndicesToCheck)
        {
            int maxTurnOverCount = 0;
            StringBuilder maximumTurnOverCell = new StringBuilder();
            if (i_IndicesToCheck.Length > 0)
            {
                string[] splitIndices = i_IndicesToCheck.ToString().Split('.');
                foreach (string currentOptionalCell in splitIndices)
                {
                    string[] splitRowCol = currentOptionalCell.Split(',');
                    int rowIndex = int.Parse(splitRowCol[0].ToString());
                    int colIndex = int.Parse(splitRowCol[1].ToString());
                    string potentialMaxTurnOverCell = CoordinatesToString(rowIndex, colIndex);
                    IsLegalMove(k_PlayerTwoSymbol, potentialMaxTurnOverCell); // no need to chek if returns true because we choose cells only from the optional COM cells
                    string[] optionalCells = m_IndicesToTurnOver.ToString().Split('.');
                    if (maxTurnOverCount < optionalCells.Length)
                    {
                        maxTurnOverCount = optionalCells.Length;
                        maximumTurnOverCell.Clear();
                        maximumTurnOverCell.Append(potentialMaxTurnOverCell);
                    }
                    else if (maxTurnOverCount == optionalCells.Length) // allow randomization between all potential max turnover cells
                    {
                        maximumTurnOverCell.Append(potentialMaxTurnOverCell);
                        maximumTurnOverCell.Append(',');
                    }
                }
                if (maximumTurnOverCell.Length > 0)
                {
                    if (maximumTurnOverCell[maximumTurnOverCell.Length - 1] == ',')
                    {
                        maximumTurnOverCell.Remove(maximumTurnOverCell.Length - 1, 1);
                    }
                }
            }
            return maximumTurnOverCell.ToString();
        }

        private string cpuRandomCellPicker(string i_OptionalCells)
        {
            string[] splitOptionalCells = i_OptionalCells.Split(',');
            Random rand = new Random();
            int cpuRandomIndex = rand.Next(splitOptionalCells.Length); // generate random cell according to the number of potential indices
            return splitOptionalCells[cpuRandomIndex];
        }

        private class AICPU
        {
            private static string s_TopLeftEdge = "0,0";
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
                s_TopRightEdge = "0" + "," + (i_BoardSize - 1).ToString();
                s_BottomLeftEdge = (i_BoardSize - 1).ToString() + "," + "0";
                s_BottomRightEdge = (i_BoardSize - 1).ToString() + "," +(i_BoardSize - 1).ToString();
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
                string[] splitIndexes = i_StringToCheck.Split('.');
                for (int i = 0; i < splitIndexes.Length - 1; i++)
                {
                    if (i_Coordinates == splitIndexes[i])
                    {
                        v_Contains = true;
                    }
                }
                    return v_Contains;
            }
        }
    }
}

