namespace Ex02_Console_Othello
{
    /*
     To avoid overloading the Main class, the Launcher is class used
     for future-proofing in case additional actions would be taken before the
     game starts (leaderboards, opening files etc).*/
    class GameLauncher
    {
        GameConsoleUI m_UserInterface;
        public void StartGame()
        {
            m_UserInterface = new GameConsoleUI();
        }
    }
}
