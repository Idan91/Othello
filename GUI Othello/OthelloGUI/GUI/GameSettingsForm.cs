using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Othello.GUI
{
    internal class GameSettingsForm : Form
    {
        private Button m_BtnGameSize;
        private Button m_BtnVSCOM;
        private Button m_BtnVSPlayer;
        private int m_GameSize = 6;
        private bool m_PlayerVSPlayer = false;
        private bool m_OptionSelected = false;

        public GameSettingsForm(out int o_GameSize, out bool o_PlayerVSPlayer)
        {
            initialize();
            o_GameSize = m_GameSize;
            o_PlayerVSPlayer = m_PlayerVSPlayer;
        }

        public bool OptionSelected
        {
            get
            {
                return m_OptionSelected;
            }
        }

        private void initialize()
        {
            //Form
            this.Width = 450;
            this.Height = 200;
            this.Text = "Othello - Game Settings";
            this.CenterToScreen();
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            //Fonts
            Single defaultFormFontSize = 12F;
            string defualtFormFontName = "Arial";
            Font defualtFormFont = new Font(defualtFormFontName, defaultFormFontSize, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            //btnGameSize
            initializeBtnSize(defualtFormFont);
            //btnVSCOM
            initializeBtnVsCom(defualtFormFont);
            //btnVSCOM
            initializeBtnVsPlayer(defualtFormFont);
        }

        private void initializeBtnVsPlayer(Font i_TextFont)
        {
            m_BtnVSPlayer = new Button();
            m_BtnVSPlayer.Width = this.Width / 2 - 30;
            m_BtnVSPlayer.Height = 50;
            m_BtnVSPlayer.Left += m_BtnGameSize.Right - m_BtnVSPlayer.Width;
            m_BtnVSPlayer.Top += m_BtnGameSize.Bottom + 20;
            m_BtnVSPlayer.Text = "Play against your friend";
            m_BtnVSPlayer.Font = i_TextFont;
            m_BtnVSPlayer.Click += onClickStartGameVSPlater;
            this.Controls.Add(m_BtnVSPlayer);
            this.ShowDialog();
        }

        private void initializeBtnVsCom(Font i_TextFont)
        {
            m_BtnVSCOM = new Button();
            m_BtnVSCOM.Left += 20;
            m_BtnVSCOM.Top += m_BtnGameSize.Bottom + 20;
            m_BtnVSCOM.Width = this.Width / 2 - 30;
            m_BtnVSCOM.Height = 50;
            m_BtnVSCOM.Text = "Play against the computer";
            m_BtnVSCOM.Font = i_TextFont;
            m_BtnVSCOM.Click += onClickStartGameVSCOM;
            this.Controls.Add(m_BtnVSCOM);
        }

        private string getBoardSizeText()
        {
            return string.Format("Board Size: {0}x{1} (click to increase)", m_GameSize, m_GameSize);
        }

        private void initializeBtnSize(Font i_TextFont)
        {
            m_BtnGameSize = new Button();
            m_BtnGameSize.Left += 20;
            m_BtnGameSize.Top += 20;
            m_BtnGameSize.Width = this.Width - 50;
            m_BtnGameSize.Height = 50;
            m_BtnGameSize.Text = getBoardSizeText();
            m_BtnGameSize.Font = i_TextFont;
            m_BtnGameSize.Click += onClickIncreaseBoardSize;
            this.Controls.Add(m_BtnGameSize);
        }

        private void onClickStartGameVSCOM(object sender, EventArgs e)
        {
            m_OptionSelected = true;
            this.Close();
        }

        private void onClickStartGameVSPlater(object sender, EventArgs e)
        {
            m_PlayerVSPlayer = true;
            m_OptionSelected = true;
            this.Close();
        }

        private void onClickIncreaseBoardSize(object sender, EventArgs e)
        {
            if (m_GameSize < 12)
            {
                m_GameSize += 2;
            }
            else
            {
                m_GameSize = 6;
            }
            m_BtnGameSize.Text = getBoardSizeText();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // GameSettingsForm
            this.ClientSize = new System.Drawing.Size(1632, 829);
            this.Name = "GameSettingsForm";
            this.ResumeLayout(false);

        }
    }

}
