//TODO add Move Up and Move down buttons to reorder move sequences

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AbstractGamesCreationKit
{
    public partial class FormMultipleMoves : Form
    {
        public GameData NewGame = new GameData();

        public string MultipleMoveName = string.Empty;
        public int Type; // 0 - Sequence of moves; 1 Repeat selected move
        public List<GameData.Move> MoveSequence = new List<GameData.Move>();
        public GameData.MultipleMove MultipleMove = new GameData.MultipleMove();

        public FormMultipleMoves(GameData newGame)
        {
            InitializeComponent();
            SetGame(newGame);
            MultipleMovesStart();
        }

        public void SetGame(GameData newGame)
        {
            NewGame = newGame;
        }

        private void MultipleMovesStart()
        {
            listBoxMovesList.DataSource = null;
            listBoxMovesList.DataSource = NewGame.GetMovesNames();
            radioButtonTypeSequence.Checked = true;
            ListBoxMovesListSelectedIndexChanged(null,null);
            Type = 0;
        }

        private void ButtonAddMoveClick(object sender, EventArgs e)
        {
            if (listBoxMovesList.SelectedIndex < 0 || radioButtonTypeRepeat.Checked) return;
            MoveSequence.Add(NewGame.MovesList[listBoxMovesList.SelectedIndex]);
            
            List<string> multipleMoveSequenceNames = new List<string>();
            foreach(GameData.Move move in MoveSequence) multipleMoveSequenceNames.Add(move.Name);
            
            listBoxNewMultipleMove.DataSource = null;
            listBoxNewMultipleMove.DataSource = multipleMoveSequenceNames;
            buttonRemoveMove.Enabled = true;
            if(MoveSequence.Count>1) buttonAddMultipleMove.Enabled = true;
        }

        private void ButtonRemoveMoveClick(object sender, EventArgs e)
        {
            int selectedIndex = listBoxNewMultipleMove.SelectedIndex;
            if( selectedIndex< 0) return;
            MoveSequence.RemoveAt(selectedIndex);

            List<string> multipleMoveSequenceNames = new List<string>();
            foreach (GameData.Move move in MoveSequence) multipleMoveSequenceNames.Add(move.Name);

            listBoxNewMultipleMove.DataSource = null;
            listBoxNewMultipleMove.DataSource = multipleMoveSequenceNames;
            if (MoveSequence.Count < 2) 
                buttonAddMultipleMove.Enabled = buttonRemoveMove.Enabled = false;
        }

        private void ListBoxMovesListSelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = listBoxMovesList.SelectedIndex;
            if (selectedIndex < 0) return;
            if (radioButtonTypeSequence.Checked)
            {
                buttonAddMove.Enabled = selectedIndex >= 0;
                return;
            }
            if(radioButtonTypeRepeat.Checked) buttonAddMultipleMove.Enabled = true;
        }

        private void RadioButtonTypeCheckedChanged(object sender, EventArgs e)
        {
            Type = radioButtonTypeSequence.Checked ? 0 : 1;
            groupBoxMoveSequence.Enabled = radioButtonTypeSequence.Checked;

            if (radioButtonTypeSequence.Checked && MoveSequence.Count < 2)
            {
                buttonAddMultipleMove.Enabled = buttonRemoveMove.Enabled = false;
                return;
            }
            if (radioButtonTypeRepeat.Checked) buttonAddMove.Enabled = false;
            if (radioButtonTypeRepeat.Checked && listBoxMovesList.SelectedIndex >= 0)
                buttonAddMultipleMove.Enabled = true;
        }

        private void ButtonCancelClick(object sender, EventArgs e)
        {
            Close();
        }

        private void ButtonAddMultipleMoveClick(object sender, EventArgs e)
        {
            String newName = textBoxMultiple_Move_Name.Text;

            //Verifies if the move name is valid
            if (!Program.CheckLegalName(textBoxMultiple_Move_Name, "Multiple Move Name")) return;

            //Verifies if move name already exists (all moves must have diferent names)
            List<string> multipleMovesNames = new List<string>();
            foreach (var multipleMove in NewGame.MultipleMoves) multipleMovesNames.Add(multipleMove.Name);

            if (multipleMovesNames.Contains(newName))
            {
                MessageBox.Show(@"Multiple Move """ + newName + @""" already exists");
                return;
            }

            MultipleMoveName = newName;
            switch(Type)
            {
                case 0: // Move sequence
                    
                    break;
                case 1: // Repeat selected Move
                    MoveSequence.Clear();
                    MoveSequence.Add(NewGame.MovesList[listBoxMovesList.SelectedIndex]);
                    break;
            }
            SetMultipleMove();

            Close();
        }

        public void SetMultipleMove()
        {
            GameData.MultipleMove newMultipleMove = new GameData.MultipleMove(MultipleMoveName, Type, MoveSequence);
            MultipleMove = newMultipleMove;
        }

        public GameData.MultipleMove ReturnMultipleMove()
        {
            return MultipleMove;
        }

        //Updates mandatory game fields (like the names of game items), but only if valid text is inserted
        private void TextBoxFocusLeave(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string message = textBox.Name.Substring(7);
            message = message.Replace("_", @" ");
            string name = textBox.Text;

            if (!Program.CheckLegalName(textBox, message))
            {
                textBox.Focus();
                return;
            }

            string foundIn = NewGame.FindUsedName(name);

            if (!foundIn.Equals("Multiple_Move_Name") && !string.IsNullOrEmpty(foundIn))
            {
                textBox.Focus();
                foundIn = foundIn.Replace("_", " ");
                MessageBox.Show(@"The name " + name + @" is already in use in " + foundIn);
                return;
            }
        }

        //This method forces the selection of all the text on the first time focus is given to a text box //TODO maybe remove, doesn't seem too user friendly
        private void TextBoxMouseClick(object sender, MouseEventArgs e)
        {
            TextBox text = (TextBox)sender;
            if (text.Text.Contains(" ") && !text.Text.Contains("\n")) text.SelectAll();
        }
    }
}
