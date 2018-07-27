using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AbstractGamesCreationKit

{
    public partial class MainWindow : Form
    {
        //TODO test it -> piece moves list not working correctly - bug on add one and add all, lists are wrong, review whole thing
        //TODO directions of a move do not update when selected move changes - check if object is created 

        //TODO ! multiple moves in ZRF (add-partial, but how to handle move-type and priorities?)
        // maybe force the definition of move-types, change the priority option for move types and then change the multiple moves to use move types instead of moves
        //TODO jump over specific number of pieces not implemented
        //TODO drop option - adjacent to: only drop pieces if adjacent to on-board pieces, using direction and distance options. Maybe On-board piece dependant is a better name.
        //TODO count turns - use dummy pieces that are 'played' every turn/move, then use count-condition to check endgame
        //TODO - Promotion moves with more than one zone for each piece type
        //TODO - Multiple Moves -> add partial moves
        //TODO - Flip moves -> add copy?
        //TODO - for the Amazons ASG sniping problem: add-copy the arrows, multiple move: slide any + drop (depending on new position)
        
        //TODO eliminate invalid, but previously occupied positions in zones and board setup positions
        //TODO if the board dimensions change update the piece positions in order for them to be valid
        
        //TODO All Tooltips, using the groupboxes. Not working correctly
        //TODO BUG!!! TextBoxFocus focus leave throws 2 events when main tab index changes (causing 2 error messages in validate used names)
        //TODO BUG!!! TextBoxFocus focus leave doesn't work for empty strings and main tab index changes 
        
        //TODO - improve illegal operators regular expression
        //TODO - make move names not mandatory - generate names by all the movement options chosen
        //TODO - finish InvertPiecePositions
        //TODO - Change list boxes to multi select when possible (e.g. moves for pieces)
        //TODO - Game specific rules like castling and en passant
        
        public GameData NewGame = new GameData();
        private string _imagePath = string.Empty;
        const string PieceHelpMessage = @"write a short description of this piece type";
        const string PieceDescriptionMessage = @"insert a detailed description of this piece type and its abilities";

        /// <summary>
        /// Main Window
        /// </summary>

        public MainWindow()
        {
            InitializeComponent();
            ToolStripMenuItemNew_Click(null, null);
        }

        private void ButtonCreateZRFMouseClick(object sender, MouseEventArgs e)
        {
            //TODO check if all the necessary data is inserted
            
            NewGame.DefineBoardPositions();
            ZRFCreator code = new ZRFCreator(NewGame);

            code.GenerateZRF();

        }

        //Refreshes the Main Tab Control whenever the tab index changes
        private void TabControlMainSelectedIndexChanged(object sender, EventArgs e)
        {
            buttonBack.Enabled = tabControlMain.SelectedIndex >= 1;
            buttonNext.Enabled = tabControlMain.SelectedIndex < 5;
            
            switch (tabControlMain.SelectedIndex)
            {
                case 0: GameTabControl();  break;
                case 1: BoardTabControl(); break;
                case 2: MovesTabControl(); break;
                case 3: PieceTabControl(); break;
                case 4: SetupTabControl(); break;
                case 5: GoalsTabControl(); break;
            }
        }
        
        private void ButtonTabNavigation(object sender, MouseEventArgs e)
        {
            Button current = (Button)sender;
            tabControlMain.SelectTab(tabControlMain.SelectedIndex + (current.Name.Contains("Next") ? 1 : -1));
        }
        
        /// <summary>
        /// 'Game' Tab - responsible for defining basic game info
        /// Mandatory fields are Game Title, Player 1 Name and Player 2 Name (default values used: "untitled", "Player1", "Player2")
        /// Turn Options include Turn Passing (No (default), Yes, Only on stalemate)
        /// Optional fields are Game Description, History and Strategy
        /// The destination folder field allows the user to choose a folder where the game's files will be saved
        /// </summary>

        private void GameTabControl()
        {
            if (comboBoxGamePassTurn.SelectedIndex < 0) comboBoxGamePassTurn.SelectedIndex = 0;
            if (comboBoxBoardSymmetry.SelectedIndex < 0) comboBoxBoardSymmetry.SelectedIndex = 0;
        }

        private void TextBoxDestinationFolderMouseClick(object sender, MouseEventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() != DialogResult.OK) return;
            textBoxDestinationFolder.Text = folderBrowserDialog.SelectedPath;
        }

        private void TextBoxDestinationFolderTextChanged(object sender, EventArgs e)
        {
            NewGame.GamePath = textBoxDestinationFolder.Text + "\\";
        }

        private void ComboBoxGamePassTurnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxGamePassTurn.SelectedIndex < 0) return;
            NewGame.PassTurn = comboBoxGamePassTurn.SelectedIndex;
        }

        private void ComboBoxBoardSymmetrySelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxBoardSymmetry.SelectedIndex < 0) return;
            switch (comboBoxBoardSymmetry.SelectedIndex)
            {
                case 0: NewGame.BoardSymmetry = "All"; return;
                case 1: NewGame.BoardSymmetry = "None"; return;
                case 2: NewGame.BoardSymmetry = "Vertical"; return;
                case 3: NewGame.BoardSymmetry = "Horizontal"; return;
            }
        }

        /// <summary>
        /// 'Board' Tab
        /// </summary>

        private void BoardTabControl()
        {
            int rows = NewGame.BoardRowNumber, columns = NewGame.BoardColumnNumber;
            numericUpDownRows.Value = rows;
            numericUpDownColumns.Value = columns;
            //if (radioButtonBoardDrawingAutomatic.Checked) GenerateBoard();
            RadioButtonsBoardDrawing(null, null);
            comboBoxZonePlayersAffected.Items.Clear();
            IList<string> zonesPlayersAffected = new [] {"Both Players", NewGame.P1Name, NewGame.P2Name};
            foreach (var player in zonesPlayersAffected) comboBoxZonePlayersAffected.Items.Add(player);
            if (comboBoxZonePlayersAffected.SelectedIndex < 0) comboBoxZonePlayersAffected.SelectedIndex = 0;
            ListBoxZonesSelectedIndexChanged(null,null);
        }

        private void GenerateBoard()
        {
            NewGame.BoardRowNumber = (int)numericUpDownRows.Value;
            NewGame.BoardColumnNumber = (int)numericUpDownColumns.Value;
            NewGame.Color1 = panelColor1.BackColor;
            NewGame.Color2 = panelColor2.BackColor;
            pictureBoxBoard.Image = NewGame.CreateBoardBitmap(false);
        }

        private void RadioButtonsBoardDrawing(object sender, EventArgs e)
        {
            groupBoxColorChooser.Enabled = NewGame.AutomaticBoard = radioButtonBoardDrawingAutomatic.Checked;
            groupBoxBackgroundImage.Enabled = radioButtonBackgroundImage.Checked;
            pictureBoxBoard.Image = null;
            if(NewGame.AutomaticBoard) GenerateBoard();
            else if (!string.IsNullOrEmpty(_imagePath)) pictureBoxBoard.Load(_imagePath);
        }

        //Clicking on the Board Image will add or remove a position from the zone positions list
        private void PictureBoxBoardImageZonesMouseClick(object sender, MouseEventArgs e)
        {
            string boardPosition = FindPositionInBoard(e.X, e.Y); //Find the board position given a set of mouse coordinates

            List<string> tempPositions = listBoxZonePositions.Items.Cast<string>().ToList();

            if (tempPositions.Contains(boardPosition))
                tempPositions.Remove(boardPosition); //If the position is already selected for that zone, remove it
            else
                tempPositions.Add(boardPosition);    //If not, add the position to the temporary list of positions of the new zone
            
            listBoxZonePositions.Items.Clear();   //Refresh listBoxZonePositions to show the new list
            foreach(var position in tempPositions) listBoxZonePositions.Items.Add(position); 
        }

        private void ButtonZoneAddMouseClick(object sender, MouseEventArgs e)
        {
            //TODO position list is ugly
            if (!Program.CheckLegalName(textBoxZone_Name, "Zone name")) return;
            List<string> tempPositions = listBoxZonePositions.Items.Cast<string>().ToList();
            if (tempPositions.Count <= 0)
            {
                MessageBox.Show(@"There are no positions selected");
                return;
            }

            GameData.Zone newZone = new GameData.Zone(textBoxZone_Name.Text, comboBoxZonePlayersAffected.SelectedIndex, tempPositions);

            foreach (GameData.Zone oldZone in NewGame.ZoneList)
            {
                if (!oldZone.Name.Equals(newZone.Name)) continue;
                
                // TODO - Zones can have the same name if and only if the players affected are different
                if (oldZone.Name.Equals(newZone.Name) &&
                    ((oldZone.PlayersAffected == 1 && newZone.PlayersAffected == 2) ||
                    (oldZone.PlayersAffected == 2 && newZone.PlayersAffected == 1))) continue;
                
                string message = "Zone \"" + newZone.Name + "\" already exists. Overwrite?";
                const string caption = "Zone Overwriting";
                var result = MessageBox.Show(message, caption, MessageBoxButtons.OKCancel);
                if (result == DialogResult.Cancel) return;
                NewGame.ZoneList.Remove(oldZone);
                break;
            }
            
            NewGame.ZoneList.Add(newZone);
            buttonZoneRemove.Enabled = true;
            listBoxZones.Items.Clear();
            listBoxZonePositions.Items.Clear();

            foreach (var zone in NewGame.ZoneList) listBoxZones.Items.Add(zone.Name);
            listBoxZones.SelectedIndex = -1 ;
        }

        private void ListBoxZonePositionsMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listBoxZonePositions.SelectedIndex < 0) return;
            listBoxZonePositions.Items.Clear();
            List<string> tempPositions = listBoxZonePositions.Items.Cast<string>().ToList();
            tempPositions.Remove((string) listBoxZonePositions.SelectedItem);
            foreach (var position in tempPositions) listBoxZonePositions.Items.Add(position);
        }

        private void ButtonZoneRemoveMouseClick(object sender, MouseEventArgs e)
        {
            if (listBoxZones.SelectedIndex < 0)
                if (listBoxZones.Items.Count > 0) listBoxZones.SelectedIndex = 0;
                else return;

            //GameData.Zone target = NewGame.FindZone(listBoxZones.SelectedItem as string);
            GameData.Zone target = NewGame.FindZone(listBoxZones.SelectedIndex);
            NewGame.ZoneList.Remove(target);

            listBoxZones.Items.Clear();
            foreach (var zone in NewGame.ZoneList) listBoxZones.Items.Add(zone.Name);
            buttonZoneRemove.Enabled = NewGame.ZoneList.Count > 0;
            if (NewGame.ZoneList.Count > 0) return;
            textBoxZone_Name.Text = @"insert zone name here";
            comboBoxZonePlayersAffected.SelectedIndex = 0;
            listBoxZonePositions.Items.Clear();
        }

        private void ListBoxZonesSelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBoxZones.SelectedIndex < 0 || NewGame.ZoneList.Count <= 0)
            {
                textBoxZone_Name.Text = @"insert zone name here";
                listBoxZonePositions.Items.Clear();
                return;
            }
            //GameData.Zone selectedZone = NewGame.FindZone(listBoxZones.SelectedItem as string);
            GameData.Zone selectedZone = NewGame.FindZone(listBoxZones.SelectedIndex);
            if (selectedZone == null) return;
            textBoxZone_Name.Text = selectedZone.Name;
            comboBoxZonePlayersAffected.SelectedIndex = selectedZone.PlayersAffected;
            listBoxZonePositions.Items.Clear();
            foreach(var position in selectedZone.Positions)listBoxZonePositions.Items.Add(position);
        }

        /// <summary>
        /// 'Moves' tab
        /// </summary>
        
        private void MovesTabControl()
        {
            numericUpDownMoveDistanceUpTo.Maximum = numericUpDownMoveDistanceExactly.Maximum =
                        NewGame.BoardColumnNumber > NewGame.BoardRowNumber ? NewGame.BoardColumnNumber : NewGame.BoardRowNumber;
            numericUpDownJumpRadius.Maximum = NewGame.BoardColumnNumber + NewGame.BoardRowNumber;
            checkedListBoxMovesZones.Items.Clear();
            foreach (var zone in NewGame.ZoneNamesList()) checkedListBoxMovesZones.Items.Add(zone);
            if (comboBoxMovesZonesType.SelectedIndex == -1) comboBoxMovesZonesType.SelectedIndex = 0;
            if (comboBoxJumpOver.SelectedIndex == -1) comboBoxJumpOver.SelectedIndex = 0;
            
            
        }

        //Handles the enabling of options depending on the distance type selected
        private void RadioButtonDistanceCheckedChanged(object sender, EventArgs e)
        {
            RadioButton current = (RadioButton)sender;
            numericUpDownMoveDistanceUpTo.Enabled = current.Text.Equals("Up To");
            numericUpDownMoveDistanceExactly.Enabled = current.Text.Equals("Exactly");

            numericUpDownJumpRadius.Enabled = current.Text.Equals("Radius");
            comboBoxJumpOver.Enabled = groupBoxDirection.Enabled = !current.Text.Equals("Radius");

            if (current.Text.Equals("Radius"))
            {   comboBoxJumpOver.SelectedIndex = 0;
                radioButtonMovesJumpCaptureOnLanding.Checked = true;
                radioButtonMovesJumpCaptureDuringJump.Enabled = radioButtonMovesJumpCaptureBothMethods.Enabled = false;
            }
            else
            {
                radioButtonMovesJumpCaptureDuringJump.Enabled = radioButtonMovesJumpCaptureBothMethods.Enabled = true;
            }
        }

        //Handles the All Directions Button in the Directions option of the Moves tab
        private void ButtonDirectionAllClick(object sender, MouseEventArgs e)
        {
            checkBoxDirectionNW.Checked = !checkBoxDirectionNW.Checked;
            checkBoxDirectionN.Checked = !checkBoxDirectionN.Checked;
            checkBoxDirectionNE.Checked = !checkBoxDirectionNE.Checked;
            checkBoxDirectionW.Checked = !checkBoxDirectionW.Checked;
            checkBoxDirectionE.Checked = !checkBoxDirectionE.Checked;
            checkBoxDirectionSW.Checked = !checkBoxDirectionSW.Checked;
            checkBoxDirectionS.Checked = !checkBoxDirectionS.Checked;
            checkBoxDirectionSE.Checked = !checkBoxDirectionSE.Checked;
        }

        private void VerifyDropCapture()
        {
            if (radioButtonCapturingMandatory.Checked)
            {
                radioButtonDropEnemy.Checked = radioButtonDropEnemy.Enabled = true;
                radioButtonDropEmpty.Enabled = radioButtonDropAnywhere.Enabled = false;
            }
            if (radioButtonCapturingNotAllowed.Checked)
            {   radioButtonDropEmpty.Checked = radioButtonDropEmpty.Enabled = true;
                radioButtonDropEnemy.Enabled = radioButtonDropAnywhere.Enabled = false;
            }
            if (radioButtonCapturingAllowed.Checked)
                radioButtonDropEnemy.Enabled = radioButtonDropEmpty.Enabled = radioButtonDropAnywhere.Enabled = true;
        }
        
        private void RadioButtonMoveTypeCheckedChanged(object sender, EventArgs e)
        {
            groupBoxDrop.Enabled = radioButtonMoveDrop.Checked;
            groupBoxDirection.Enabled = groupBoxDistance.Enabled = !radioButtonMoveDrop.Checked;
            groupBoxMovesJump.Enabled = radioButtonDistanceRadius.Enabled = radioButtonMoveJump.Checked;
            radioButtonDistanceFurthest.Enabled = !radioButtonMoveJump.Checked;
            
            //groupBoxSwapWith.Enabled = radioButtonMoveSwap.Checked;

            //If jump is selected, the minimum move distances are set to 2
            numericUpDownMoveDistanceUpTo.Minimum = radioButtonMoveJump.Checked ? 2 : 1;
            numericUpDownMoveDistanceExactly.Minimum = radioButtonMoveJump.Checked ? 2 : 1;

            if (radioButtonMoveDrop.Checked) VerifyDropCapture();
        }

        private void RadioButtonCaptureCheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonMoveDrop.Checked) VerifyDropCapture();
            if (radioButtonMoveJump.Checked) groupBoxMovesJumpCapture.Enabled = !radioButtonCapturingNotAllowed.Checked;
        }

        private void ButtonAddMoveMouseClick(object sender, MouseEventArgs e)
        {
            String moveName = textBoxMove_Name.Text;
            
            //Verifies if the move name is valid
            if (!Program.CheckLegalName(textBoxMove_Name, "Move Name")) return;

            //Verifies if move name already exists (all moves must have diferent names)
            foreach (GameData.Move move in NewGame.MovesList)
            {
                if (!move.Name.Equals(moveName)) continue;
                MessageBox.Show(@"Move """ + moveName + @""" already exists");
                return;
            }

            int capture = 1;                                             //By default capturing is allowed:  Capture Value = 1
            if (radioButtonCapturingNotAllowed.Checked) capture = 0;     //If capture is not allowed:        Capture Value = 0
            else if (radioButtonCapturingMandatory.Checked) capture = 2; //If capture is mandatory:          Capture Value = 2

            int priority = (int)numericUpDownPriorityLevel.Value;

            List<GameData.Zone> moveZones = new List<GameData.Zone>();
            if(checkedListBoxMovesZones.CheckedIndices != null)
                foreach (GameData.Zone zone in NewGame.ZoneList) 
                    foreach (string itemChecked in checkedListBoxMovesZones.CheckedItems)
                        if (itemChecked.Equals(zone.Name)) moveZones.Add(zone);

            int zoneType = 0; // By default zone type is set to 0, meaning that no zones are affecting the move
            if (comboBoxMovesZonesType.SelectedIndex > 0)
                if (comboBoxMovesZonesType.SelectedItem.Equals("Inside"))
                    zoneType = 1;
                else if (comboBoxMovesZonesType.SelectedItem.Equals("Outside")) zoneType = 2;

            bool hasCheckedItems = false;
            for (int i = 0 ; i < checkedListBoxMovesZones.Items.Count; i++)
                if (checkedListBoxMovesZones.GetItemCheckState(i) == CheckState.Checked) 
                {
                    hasCheckedItems = true;
                    break;
                }
            if (!hasCheckedItems) zoneType = 0;

            if (radioButtonMoveDrop.Checked)
            {
                string dropType = "";
                if (radioButtonDropAnywhere.Checked) dropType = "anywhere";
                if (radioButtonDropEmpty.Checked) dropType = "empty";
                if (radioButtonDropEnemy.Checked) dropType = "enemy";
                NewGame.MovesList.Add(new GameData.Drop(moveName, "drop", priority, capture, moveZones, zoneType,
                                                        dropType));
            }
            else
            {
                List<string> directions = GetDirections();
                if (GetDirections().Count == 0 && !radioButtonDistanceRadius.Checked)
                {
                    MessageBox.Show(@"Choose at least one direction for the move");
                    return;
                }

                //Defines travel type information for the move (Any distance, exactly N spaces, up to N spaces or travel to the furthest space possible)
                string travelType = null;
                int distance = 0;
                if (radioButtonDistanceAny.Checked) travelType = "any";
                if (radioButtonDistanceFurthest.Checked) travelType = "furthest";
                if (radioButtonDistanceUpTo.Checked)
                {
                    travelType = "upto";
                    distance = (int)numericUpDownMoveDistanceUpTo.Value;
                }
                if (radioButtonDistanceExactly.Checked)
                {
                    travelType = "exactly";
                    distance = (int)numericUpDownMoveDistanceExactly.Value;
                }
                if (radioButtonDistanceRadius.Checked)
                {
                    travelType = "radius";
                    distance = (int)numericUpDownJumpRadius.Value;
                }

                if (radioButtonMoveSlide.Checked)
                {
                    NewGame.MovesList.Add(new GameData.Slide(moveName, "slide", priority, capture, moveZones, zoneType, directions, distance, travelType));
                    goto NewMove;
                }

                if (radioButtonMoveJump.Checked)
                {
                    int jumpOver = comboBoxJumpOver.SelectedIndex;
                    int jumpCapture = 0;
                    if (radioButtonMovesJumpCaptureOnLanding.Checked) jumpCapture = 0;
                    if (radioButtonMovesJumpCaptureDuringJump.Checked) jumpCapture = 1;
                    if (radioButtonMovesJumpCaptureBothMethods.Checked) jumpCapture = 2;

                    NewGame.MovesList.Add(new GameData.Jump(moveName, "jump", priority, capture, moveZones, zoneType, directions, distance, travelType, jumpOver, jumpCapture));
                    goto NewMove;
                }

                //if (radioButtonMoveSwap.Checked)  NewGame.MovesList.Add(new GameData.Swap(moveName, "swap", priority, capture, moveZones, zoneType, directions, distance, travelType));
            }

NewMove:    
            listBoxMovesList.Items.Clear();
            List<string> movesNames = new List<string>();
            foreach (GameData.Move move in NewGame.MovesList)
            {
                listBoxMovesList.Items.Add(move.Name); 
                movesNames.Add(move.Name);
            }
            
            buttonRemoveMove.Enabled = true;
            groupBoxMultipleMoves.Enabled = true;
            listBoxMovesList.SelectedIndex = listBoxMovesList.Items.Count - 1;
        }

        //Returns a string list with the selected directions for a move
        private List<string> GetDirections()
        {
            List<string> result = new List<string>();
            if (radioButtonDistanceRadius.Checked){
                result.Add("radius");
                return result;
            }

            if (checkBoxDirectionNW.Checked) result.Add("nw");
            if (checkBoxDirectionN.Checked)  result.Add("n");
            if (checkBoxDirectionNE.Checked) result.Add("ne");
            if (checkBoxDirectionW.Checked)  result.Add("w");
            if (checkBoxDirectionE.Checked)  result.Add("e");
            if (checkBoxDirectionSW.Checked) result.Add("sw");
            if (checkBoxDirectionS.Checked)  result.Add("s");
            if (checkBoxDirectionSE.Checked) result.Add("se");
            return result;
        }

        private void ButtonRemoveMoveMouseClick(object sender, MouseEventArgs e)
        {
            int selectedIndex = listBoxMovesList.SelectedIndex;
            if (selectedIndex < 0) return;

            NewGame.MovesList.RemoveAt(selectedIndex);

            listBoxMovesList.Items.Clear();

            foreach (GameData.Move move in NewGame.MovesList) listBoxMovesList.Items.Add(move.Name);

            buttonRemoveMove.Enabled = listBoxMovesList.Items.Count > 0;
        }

        private void ListBoxMovesListSelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = listBoxMovesList.SelectedIndex;
            if (selectedIndex < 0) return;
            buttonRemoveMove.Enabled = listBoxMovesList.SelectedIndex >= 0;
            
            checkBoxDirectionN.Checked = false;
            checkBoxDirectionE.Checked = false;
            checkBoxDirectionS.Checked = false;
            checkBoxDirectionW.Checked = false;
            checkBoxDirectionNE.Checked = false;
            checkBoxDirectionNW.Checked = false;
            checkBoxDirectionSE.Checked = false;
            checkBoxDirectionSW.Checked = false;
            
            //Show all information of the selected move
            GameData.Move move = NewGame.FindMove(listBoxMovesList.SelectedItem.ToString());
            textBoxMove_Name.Text = move.Name;
            numericUpDownPriorityLevel.Value = move.Priority;
            
            switch(move.Capture)
            {
                case 0: radioButtonCapturingNotAllowed.Checked = true;  break;
                case 1: radioButtonCapturingAllowed.Checked = true; break;
                case 2: radioButtonCapturingMandatory.Checked = true; break;
            }

            //reset checked zones
            for (int i = 0; i < checkedListBoxMovesZones.Items.Count; i++ )
                checkedListBoxMovesZones.SetItemChecked(i, false);

            //set checked zones
            foreach (var zone in move.MoveZones)
                for (int i = 0; i < checkedListBoxMovesZones.Items.Count; i++)
                {
                    if(checkedListBoxMovesZones.Items[i].ToString().Equals(zone.Name))
                    checkedListBoxMovesZones.SetItemChecked(i, true);
                }
            
            comboBoxMovesZonesType.SelectedIndex = move.ZoneType;
            
            switch (move.Type)
            {
                case "drop": 
                    radioButtonMoveDrop.Checked = true;
                    GameData.Drop drop = (GameData.Drop) move;
                    switch (drop.Type)
                    {
                        case "anywhere": radioButtonDropAnywhere.Checked = true; break;
                        case "empty": radioButtonDropEmpty.Checked = true; break;
                        case "enemy": radioButtonDropEnemy.Checked = true; break;
                    }
                    break;
                case "slide": 
                    radioButtonMoveSlide.Checked = true;
                    GameData.Slide slide = (GameData.Slide)move;
                    switch (slide.TravelType)
                    {
                        case "upto":
                            radioButtonDistanceUpTo.Checked = true;
                            numericUpDownMoveDistanceUpTo.Value = slide.Distance;
                            break;
                        case "any": 
                            radioButtonDistanceAny.Checked = true;
                            break;
                        case "furthest":
                            radioButtonDistanceFurthest.Checked = true;
                            break;
                        case "exactly": 
                            radioButtonDistanceExactly.Checked = true;
                            numericUpDownMoveDistanceExactly.Value = slide.Distance;
                            break;
                    }
                    
                    foreach (var direction in slide.Directions)
                    {
                        switch (direction)
                        {//TODO test this
                            case "n": checkBoxDirectionN.Checked = true; break;
                            case "e": checkBoxDirectionE.Checked = true; break;
                            case "s": checkBoxDirectionS.Checked = true; break;
                            case "w": checkBoxDirectionW.Checked = true; break;
                            case "ne": checkBoxDirectionNE.Checked = true; break;
                            case "nw": checkBoxDirectionNW.Checked = true; break;
                            case "se": checkBoxDirectionSE.Checked = true; break;
                            case "sw": checkBoxDirectionSW.Checked = true; break;
                        }
                    }
                    break;
                case "jump": 
                    radioButtonMoveJump.Checked = true;
                    GameData.Jump jump = (GameData.Jump) move;
                    switch (jump.TravelType)
                    {
                        case "upto":
                            radioButtonDistanceUpTo.Checked = true;
                            numericUpDownMoveDistanceUpTo.Value = jump.Distance;
                            break;
                        case "any":
                            radioButtonDistanceAny.Checked = true;
                            break;
                        case "furthest":
                            radioButtonDistanceFurthest.Checked = true;
                            break;
                        case "exactly":
                            radioButtonDistanceExactly.Checked = true;
                            numericUpDownMoveDistanceExactly.Value = jump.Distance;
                            break;
                        case "radius": 
                            radioButtonDistanceRadius.Checked = true;
                            numericUpDownJumpRadius.Value = jump.Distance;
                            break;
                    }
                    foreach (var direction in jump.Directions)
                    {
                        switch (direction)
                        {
                            case "n": checkBoxDirectionN.Checked = true; break;
                            case "e": checkBoxDirectionE.Checked = true; break;
                            case "s": checkBoxDirectionS.Checked = true; break;
                            case "w": checkBoxDirectionW.Checked = true; break;
                            case "ne": checkBoxDirectionNE.Checked = true; break;
                            case "nw": checkBoxDirectionNW.Checked = true; break;
                            case "se": checkBoxDirectionSE.Checked = true; break;
                            case "sw": checkBoxDirectionSW.Checked = true; break;
                        }
                    }
                    comboBoxJumpOver.SelectedIndex = jump.JumpOver;
                    switch (jump.JumpCapture)
                    {   case 0: radioButtonMovesJumpCaptureOnLanding.Checked = true; break;
                        case 1: radioButtonMovesJumpCaptureDuringJump.Checked = true; break;
                        case 2: radioButtonMovesJumpCaptureBothMethods.Checked = true; break;
                    }
                    break;
              }
        }

        private void ComboBoxMovesZonesOptionsSelectedIndexChanged(object sender, EventArgs e)
        {
            checkedListBoxMovesZones.Enabled = comboBoxMovesZonesType.SelectedIndex != 0;
            foreach (int index in checkedListBoxMovesZones.CheckedIndices)
                checkedListBoxMovesZones.SetItemChecked(index, false);
        }
        
        private void ComboBoxJumpOverSelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = comboBoxJumpOver.SelectedIndex;
            if (selectedIndex < 0) return;
            radioButtonMovesJumpCaptureDuringJump.Enabled = (selectedIndex != 2 && selectedIndex != 4);
            radioButtonMovesJumpCaptureBothMethods.Enabled = (selectedIndex != 2 && selectedIndex != 4);
            if (!radioButtonMovesJumpCaptureOnLanding.Checked && (selectedIndex == 2 || selectedIndex == 4))
                radioButtonMovesJumpCaptureOnLanding.Checked = true;
                                              
        }

        private void RefreshMultipleMovesList()
        {
            listBoxMovesMultipleMoves.Items.Clear();
            foreach (var multipleMove in NewGame.MultipleMoves)
                listBoxMovesMultipleMoves.Items.Add(multipleMove.Name);
        }

        private void ButtonMultipleMovesClick(object sender, EventArgs e)
        {
            FormMultipleMoves newForm = new FormMultipleMoves(NewGame);
            newForm.ShowDialog();
            newForm.Dispose();

            GameData.MultipleMove newMultipleMove = newForm.ReturnMultipleMove();
            if (!string.IsNullOrEmpty(newMultipleMove.Name)) NewGame.MultipleMoves.Add(newMultipleMove);
            RefreshMultipleMovesList();

        }

        private void ButtonMovesRemoveMultipleMoveClick(object sender, EventArgs e)
        {
            int selectedIndex = listBoxMovesMultipleMoves.SelectedIndex;
            if (selectedIndex < 0) return;
            NewGame.MultipleMoves.RemoveAt(selectedIndex);

            RefreshMultipleMovesList();
        }

        private void ListBoxMovesMultipleMovesSelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = listBoxMovesMultipleMoves.SelectedIndex;
            if (selectedIndex < 0) return;
            buttonMovesRemoveMultipleMove.Enabled = selectedIndex > 0;
        }

        /// <summary>
        /// 'Pieces' Tab
        /// </summary>

        private void PieceTabControl()
        {
            ResetPieceLists();
            buttonAddMoveToPiece.Enabled = listBoxAvailableMovesList.SelectedIndex >= 0;
            buttonAddAllMovesToPiece.Enabled = listBoxAvailableMovesList.Items.Count > 0;
        }

        private void ButtonAddPieceMouseClick(object sender, MouseEventArgs e)
        {
            //Verifies if the piece name is valid
            if (!Program.CheckLegalName(textBoxPiece_Name, "Piece Name")) return;
            //Verifies if name is null or has only white spaces
            if (string.IsNullOrWhiteSpace(textBoxPiece_Name.Text)){
                MessageBox.Show(@"Please insert a valid name for the new piece"); return;}
            //Verifies if piece name already exists (all pieces must have diferent names)
            if (NewGame.GetPieceNames().Contains(textBoxPiece_Name.Text)){
                MessageBox.Show(@"Piece """ + textBoxPiece_Name.Text + @""" already exists");return;}
            //Verifies if a move with the same name exists (pieces and moves must be distinct)
            if (NewGame.GetMovesNames().Contains(textBoxPiece_Name.Text)){
                MessageBox.Show(@"Moves and Pieces must have different names"); return;}
            //Verifies if there is at least one move selected for this piece
            if (listBoxPieceMovesList.Items.Count == 0){
                MessageBox.Show(@"Please choose at least one move for the new piece"); return;}
            //Verifies the piece's images selection
            if (pictureBoxPieceP1PieceImage.Image == null || pictureBoxPieceP2PieceImage.Image == null 
                || pictureBoxPieceP1PieceImage.Image == pictureBoxPieceP2PieceImage.Image){
                MessageBox.Show(@"Please choose two distinct images for this piece"); return;}

            string name = textBoxPiece_Name.Text;
            string pic1Path = pictureBoxPieceP1PieceImage.ImageLocation;
            string pic2Path = pictureBoxPieceP2PieceImage.ImageLocation;
            string help = !textBoxPiece_Help.Text.Equals(PieceHelpMessage) ? textBoxPiece_Help.Text : "";
            string description = !textBoxPiece_Description.Text.Equals(PieceDescriptionMessage) ? textBoxPiece_Description.Text : "";

            List<GameData.Move> moves = new List<GameData.Move>();
            List<GameData.MultipleMove> multiMoves = new List<GameData.MultipleMove>();
            List<string> positionsP1 = new List<string>(), positionsP2 = new List<string>();
            
            foreach (string move in listBoxPieceMovesList.Items)
            {
                if (NewGame.FindMove(move) != null) moves.Add(NewGame.FindMove(move));
                else
                    if (NewGame.FindMultipleMove(move) != null) multiMoves.Add(NewGame.FindMultipleMove(move));
            }

            GameData.Piece piece = new GameData.Piece(name, help, description, pic1Path, pic2Path, moves, multiMoves, positionsP1, positionsP2, 0, 0);
            NewGame.PieceList.Add(piece);
            
            buttonRemovePiece.Enabled = true;
            
            ResetPieceLists();
            
            textBoxPiece_Name.Text = @"insert piece name here";
            textBoxPiece_Help.Text = PieceHelpMessage;
            textBoxPiece_Description.Text = PieceDescriptionMessage;
            buttonAddPiece.Enabled = false;
        }

        private void ResetPieceLists()
        {
            //reset variables
            List<string> availableMovesNamesList = new List<string>();
            foreach (var move in NewGame.MovesList) availableMovesNamesList.Add(move.Name);
            foreach (var move in NewGame.MultipleMoves) availableMovesNamesList.Add(move.Name);
            
            // Refresh the lists
            listBoxAvailableMovesList.Items.Clear();
            listBoxPieceMovesList.Items.Clear();
            listBoxPiecesPieceList.Items.Clear();
            
            foreach (var pieceName in NewGame.GetPieceNames()) listBoxPiecesPieceList.Items.Add(pieceName);
            foreach (var moveName in availableMovesNamesList) listBoxAvailableMovesList.Items.Add(moveName);
        }

        // The Remove Piece button was clicked.
        private void ButtonRemovePieceMouseClick(object sender, MouseEventArgs e)
        {
            //remove selected piece, or the first on the list if none was selected
            if (listBoxPiecesPieceList.SelectedIndex < 0)
                if (listBoxPiecesPieceList.Items.Count > 0) listBoxPiecesPieceList.SelectedIndex = 0;
                else return;

            NewGame.PieceList.RemoveAt(listBoxPiecesPieceList.SelectedIndex);
            
            //refresh Piece List Box
            listBoxPiecesPieceList.Items.Clear();
            foreach (var piece in NewGame.GetPieceNames()) listBoxPiecesPieceList.Items.Add(piece);

            //if the number of pieces drops to zero, disable the remove button
            buttonRemovePiece.Enabled = NewGame.PieceList.Count > 0;
        }

        // Add one or all of the moves to the piece's move list and removes the moves added this way from the available moves list
        private void ButtonsAddMovesToPiece(object sender, MouseEventArgs e)
        {
            if (sender.Equals(buttonAddAllMovesToPiece))
            {
                foreach (var item in listBoxAvailableMovesList.Items)
                    listBoxPieceMovesList.Items.Add(item);
                listBoxAvailableMovesList.Items.Clear();
            }
            else
            {
                int selectedIndex = listBoxAvailableMovesList.SelectedIndex;
                if (selectedIndex < 0) return;

                var item = listBoxAvailableMovesList.SelectedItem;
                listBoxPieceMovesList.Items.Add(item);
                listBoxAvailableMovesList.Items.Remove(item);

                if (listBoxAvailableMovesList.SelectedIndex < 0 && listBoxAvailableMovesList.Items.Count > 0)
                    listBoxAvailableMovesList.SelectedIndex = 0;
            }

            buttonAddPiece.Enabled = (listBoxPieceMovesList.Items.Count > 0);
            buttonRemoveMoveFromPiece.Enabled = listBoxPieceMovesList.SelectedIndex >= 0;
            buttonAddAllMovesToPiece.Enabled = listBoxAvailableMovesList.Items.Count > 0;
            buttonAddMoveToPiece.Enabled = listBoxAvailableMovesList.SelectedIndex >= 0;
        }

        private void ButtonRemoveMoveFromPieceMouseClick(object sender, MouseEventArgs e)
        {
            int selectedIndex = listBoxPieceMovesList.SelectedIndex;
            if (selectedIndex < 0) return;

            var selectedItem = listBoxPieceMovesList.SelectedItem;

            listBoxAvailableMovesList.Items.Add(selectedItem);
            listBoxPieceMovesList.Items.Remove(selectedItem);

            if (listBoxPieceMovesList.Items.Count > 0) return;
            buttonAddPiece.Enabled = buttonRemoveMoveFromPiece.Enabled = false;
        }

        
        private void ListBoxPieceMovesListSelectedIndexChanged(object sender, EventArgs e){
            buttonRemoveMoveFromPiece.Enabled = listBoxPieceMovesList.SelectedIndex >= 0;}

        private void ListBoxPieceListSelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = listBoxPiecesPieceList.SelectedIndex;
            if (selectedIndex < 0)
            {
                buttonRemovePiece.Enabled = false;
                return;
            }
            buttonRemovePiece.Enabled = listBoxPiecesPieceList.SelectedIndex >= 0;
            GameData.Piece piece = NewGame.FindPiece(listBoxPiecesPieceList.Items[selectedIndex].ToString());
            textBoxPiece_Name.Text = piece.Name;
            textBoxPiece_Help.Text = !string.IsNullOrWhiteSpace(piece.Help) ? piece.Help : PieceHelpMessage;
            textBoxPiece_Description.Text = !string.IsNullOrWhiteSpace(piece.Description) ? piece.Description : PieceDescriptionMessage;
            
            listBoxPieceMovesList.Items.Clear();
            foreach (var move in piece.MovesList) listBoxPieceMovesList.Items.Add(move.Name);
            foreach (var move in piece.MultipleMovesList) listBoxPieceMovesList.Items.Add(move.Name);

            listBoxAvailableMovesList.Items.Clear();
            foreach (var move in NewGame.MovesList)
                if(!listBoxPieceMovesList.Items.Contains(move.Name))
                    listBoxAvailableMovesList.Items.Add(move.Name);
            foreach (var multipleMove in NewGame.MultipleMoves)
                if (!listBoxPieceMovesList.Items.Contains(multipleMove.Name))
                    listBoxAvailableMovesList.Items.Add(multipleMove.Name);

            pictureBoxPieceP1PieceImage.ImageLocation = piece.ImageP1Path;
            pictureBoxPieceP2PieceImage.ImageLocation = piece.ImageP2Path;

            buttonAddAllMovesToPiece.Enabled = listBoxAvailableMovesList.Items.Count > 0;
            buttonAddMoveToPiece.Enabled = listBoxAvailableMovesList.SelectedIndex >= 0;
            buttonRemoveMoveFromPiece.Enabled = listBoxAvailableMovesList.SelectedIndex >= 0;
        }

        private void ListBoxAvailableMovesListSelectedIndexChanged(object sender, EventArgs e)
        {
            buttonAddAllMovesToPiece.Enabled = listBoxAvailableMovesList.Items.Count > 0;
            buttonAddMoveToPiece.Enabled = listBoxAvailableMovesList.SelectedIndex >= 0;
        }

        /// <summary>
        /// Setup Tab
        /// </summary>

        private void SetupTabControl()
        {
            if (pictureBoxBoard.Image == null) GenerateBoard();
            pictureBoxBoardImageSetup.Image = pictureBoxBoard.Image;
            listBoxSetupPieceList.Items.Clear();
            foreach(var piece in NewGame.PieceList)listBoxSetupPieceList.Items.Add(piece.Name);
            comboBoxPromotionZone.Items.Clear();
            foreach(var zone in NewGame.ZoneNamesList())comboBoxPromotionZone.Items.Add(zone);
            groupBoxPromotion.Enabled = (NewGame.PieceList.Count > 1 && NewGame.ZoneList.Count > 0);

            List<string> pieceList = NewGame.PieceList.Select(piece => piece.Name).ToList();
            checkedListBoxPromotionPieceTypes.Items.Clear();
            foreach (var piece in pieceList) checkedListBoxPromotionPieceTypes.Items.Add(piece);
        }

        private void ListBoxSetupPieceListSelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = listBoxSetupPieceList.SelectedIndex;
            if (selectedIndex < 0){
                labelSelectedPieceName.Text = @"No piece selected";
                groupBoxPromotion.Enabled = false;
                pictureBoxSetupP1PieceImage.Image = null;
                pictureBoxSetupP2PieceImage.Image = null;
                return;
            }
            string selectedPieceName = NewGame.PieceList[selectedIndex].Name;
            labelSelectedPieceName.Text = selectedPieceName;

            pictureBoxSetupP1PieceImage.Image = new Bitmap(NewGame.PieceList[selectedIndex].ImageP1Path);
            pictureBoxSetupP2PieceImage.Image = new Bitmap(NewGame.PieceList[selectedIndex].ImageP2Path);
            
            numericUpDownOffBoardP1.Enabled = numericUpDownOffBoardP2.Enabled = true;
            numericUpDownOffBoardP1.Value = NewGame.PieceList[selectedIndex].OffBoardP1;
            numericUpDownOffBoardP2.Value = NewGame.PieceList[selectedIndex].OffBoardP2;

            buttonRemoveListBoxPositionsP1.Enabled = buttonRemoveListBoxPositionsP2.Enabled = true;

            int occupiedPositions = NewGame.PieceList.Aggregate(0, (current, piece) => current + piece.PositionsP1.Count + piece.PositionsP2.Count);

            int spaces = NewGame.BoardColumnNumber * NewGame.BoardRowNumber;
           
            if (occupiedPositions > spaces) MessageBox.Show(@"The board dimensions have changed");
            //TODO eliminate invalid, but previously occupied positions
            
            RefreshPiecePositionsLists(selectedIndex);

            GameData.Piece selectedPiece = NewGame.FindPiece(selectedPieceName);
            for (int i = 0; i < checkedListBoxPromotionPieceTypes.Items.Count; i++)
                checkedListBoxPromotionPieceTypes.SetItemChecked(i, false);
            if (selectedPiece.PiecePromotion != null)
            {
                groupBoxPromotion.Enabled = true;
                comboBoxPromotionZone.SelectedIndex = comboBoxPromotionZone.Items.IndexOf(selectedPiece.PiecePromotion.PromotionZone);
                for (int i = 0; i < checkedListBoxPromotionPieceTypes.Items.Count; i++)
                    foreach (var pieceType in selectedPiece.PiecePromotion.PieceTypes)
                    {
                        if (pieceType.Equals(checkedListBoxPromotionPieceTypes.Items[i]))
                            checkedListBoxPromotionPieceTypes.SetItemChecked(i, true);
                    }
                buttonPromotionAdd.Enabled = false;
                buttonPromotionRemove.Enabled = true;
            }
            else
            {
                if (checkedListBoxPromotionPieceTypes.CheckedIndices.Count > 0) buttonPromotionAdd.Enabled = true;
                buttonPromotionRemove.Enabled = false;
                groupBoxPromotion.Enabled = (NewGame.PieceList.Count > 1 && NewGame.ZoneList.Count > 0);
            }

        }
        
        private void RefreshPiecePositionsLists(int selectedIndex)
        {
            listBoxPiecePositionsP1.Items.Clear();
            listBoxPiecePositionsP2.Items.Clear();

            foreach (var position in NewGame.PieceList[selectedIndex].PositionsP1)
                listBoxPiecePositionsP1.Items.Add(position);
            foreach (var position in NewGame.PieceList[selectedIndex].PositionsP2)
                listBoxPiecePositionsP2.Items.Add(position);

            numericUpDownOnBoardP1.Value = NewGame.PieceList[selectedIndex].PositionsP1.Count;
            numericUpDownOnBoardP2.Value = NewGame.PieceList[selectedIndex].PositionsP2.Count;
            buttonRemoveListBoxPositionsP1.Enabled = NewGame.PieceList[selectedIndex].PositionsP1.Count > 0;
            buttonRemoveListBoxPositionsP2.Enabled = NewGame.PieceList[selectedIndex].PositionsP2.Count > 0;
        }

        private void PictureBoxBoardImageSetupMouseClick(object sender, MouseEventArgs e)
        {
            int selectedIndex = listBoxSetupPieceList.SelectedIndex;
            if (selectedIndex < 0)
            {
                MessageBox.Show(@"No piece selected");
                return;
            }

            string boardPosition = FindPositionInBoard(e.X, e.Y);

            int player = 0; // 0 - Both players; 1 - Player 1; 2 - Player 2
            if (radioButtonPlayer1Setup.Checked) player = 1;
            if (radioButtonPlayer2Setup.Checked) player = 2;

            AddPosition(player, boardPosition, selectedIndex);
            //switch (comboBoxSymmetry.SelectedIndex)
            //{
            //    case 1: AddPosition(player, InvertBoardPosition(boardPosition, "horizontal"), selectedIndex); break;
            //    case 2: AddPosition(player, InvertBoardPosition(boardPosition, "vertical"), selectedIndex); break;
            //    case 3: AddPosition(player, InvertBoardPosition(boardPosition, "left diagonal"), selectedIndex); break;
            //    case 4: AddPosition(player, InvertBoardPosition(boardPosition, "right diagonal"), selectedIndex); break;
            //}

            RefreshPiecePositionsLists(selectedIndex);
        }

    //    private string InvertBoardPosition(string boardPosition, string type)
    //    {
    //        string invertedBoardPosition = string.Empty;
    //        string[,] boardPositions = NewGame.BoardPositions;

    //        int i_index, j_index = 0;

    //        for (int i = 0; i < NewGame.BoardColumnNumber; i++)
    //            for (int j = 0; j < NewGame.BoardRowNumber; j++)
    //                if (boardPositions[i, j].Equals(boardPosition))
    //                {
    //                    i_index = i;
    //                    j_index = j;
    //                    goto Found;
    //                }
    //Found:
    //        switch (type)
    //        {//TODO! - but not that important
    //            case "horizontal":

    //                break;
    //            case "vertical":
    //                break;
    //            case "left diagonal":
    //                break;
    //            case "right diagonal":
    //                break;
    //        }
    //        return invertedBoardPosition;
    //    }

        private void AddPosition(int player, string boardPosition, int selectedIndex)
        {//TODO invert positions on player symmetry
            foreach (GameData.Piece piece in
                NewGame.PieceList.Where(piece => piece.PositionsP1.Contains(boardPosition) || piece.PositionsP2.Contains(boardPosition)))
            {
                MessageBox.Show(@"Position " + boardPosition + @" already occupied by the piece " + piece.Name);
                return;
            }

            switch (player)
            {
                case 1: NewGame.PieceList[selectedIndex].PositionsP1.Add(boardPosition);
                    numericUpDownOnBoardP1.Value = NewGame.PieceList[selectedIndex].PositionsP1.Count;
                    break;
                case 2: NewGame.PieceList[selectedIndex].PositionsP2.Add(boardPosition);
                    numericUpDownOnBoardP2.Value = NewGame.PieceList[selectedIndex].PositionsP2.Count;
                    break;
                case 0: //    invertedPosition = invertPosition(boardPosition);
                    //    NewGame.PieceList[selectedIndex].PositionsP1.Add(boardPosition);
                    //    numericUpDownOnBoardP1.Value = NewGame.PieceList[selectedIndex].PositionsP1.Count;
                    //    NewGame.PieceList[selectedIndex].PositionsP2.Add(invertedPosition);
                    //    numericUpDownOnBoardP2.Value = NewGame.PieceList[selectedIndex].PositionsP2.Count;
                    break;
            }
        }

        ////Draw the symmetry lines depending on the option selected
        //private void ComboBoxSymmetrySelectedIndexChanged(object sender, EventArgs e)
        //{
        //    lineShapeSymmetryHorizontal.Visible = lineShapeSymmetryVertical.Visible = lineShapeSymmetryLeftDiagonal.Visible
        //        = lineShapeSymmetryRightDiagonal.Visible = false;

        //    switch (comboBoxSymmetry.SelectedIndex)
        //    {
        //        case 1: lineShapeSymmetryHorizontal.Visible = true; break;
        //        case 2: lineShapeSymmetryVertical.Visible = true; break;
        //        case 3: lineShapeSymmetryLeftDiagonal.Visible = true; break;
        //        case 4: lineShapeSymmetryRightDiagonal.Visible = true; break;
        //    }
        //}

        //private void CheckBoxSymmetricBoardCheckedChanged(object sender, EventArgs e)
        //{
        //    bool enabler = checkBoxSymmetricBoard.Checked;
        //    if (enabler) radioButtonPlayer1Setup.Checked = radioButtonPlayer2Setup.Checked = false;
        //    radioButtonPlayer1Setup.Enabled = radioButtonPlayer2Setup.Enabled = !enabler;
        //}

        private void ButtonsRemoveListBoxPositionsMouseClick(object sender, MouseEventArgs e)
        {
            Button current = (Button)sender; //TODO bug - casting list to button not allowed
            int player = current.Name.Contains("1") ? 1 : 2;
            int selectedPositionIndex = player == 1 ? listBoxPiecePositionsP1.SelectedIndex : listBoxPiecePositionsP2.SelectedIndex;
            
            int selectedPieceIndex = listBoxSetupPieceList.SelectedIndex;
            if (selectedPieceIndex < 0) return;

            int occupiedPositions;
            switch (player)
            {
                case 1: occupiedPositions = NewGame.PieceList[selectedPieceIndex].PositionsP1.Count();
                        if (occupiedPositions <= 0) return;
                        if (selectedPositionIndex < 0) selectedPositionIndex = occupiedPositions - 1;
                        NewGame.PieceList[selectedPieceIndex].PositionsP1.RemoveAt(selectedPositionIndex);
                        numericUpDownOnBoardP1.Value = occupiedPositions - 1;
                        break;
                case 2: occupiedPositions = NewGame.PieceList[selectedPieceIndex].PositionsP2.Count();
                        if (occupiedPositions <= 0) return;
                        if (selectedPositionIndex < 0) selectedPositionIndex = occupiedPositions - 1;
                        NewGame.PieceList[selectedPieceIndex].PositionsP2.RemoveAt(selectedPositionIndex);
                        numericUpDownOnBoardP2.Value = occupiedPositions - 1;
                        break;
            }
            RefreshPiecePositionsLists(selectedPieceIndex);

        }

        private void ListBoxesPiecePositionsMouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListBox current = (ListBox)sender;
            ButtonsRemoveListBoxPositionsMouseClick(
                current.Name.Contains("1") ? buttonRemoveListBoxPositionsP1 : buttonRemoveListBoxPositionsP2, e);
        }

        private void ComboBoxPromotionZoneSelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedindex = comboBoxPromotionZone.SelectedIndex;
            if (selectedindex < 0 || listBoxSetupPieceList.SelectedIndex < 0) return;
            buttonPromotionAdd.Enabled = checkedListBoxPromotionPieceTypes.CheckedIndices.Count > 0;
            buttonPromotionRemove.Enabled = NewGame.PieceList[listBoxSetupPieceList.SelectedIndex].PiecePromotion != null;
        }

        private void ButtonPromotionAddMouseClick(object sender, MouseEventArgs e)
        {
            int selectedPieceIndex = listBoxSetupPieceList.SelectedIndex;
            if (selectedPieceIndex < 0) return;
            string selectedPiece = NewGame.PieceList[selectedPieceIndex].Name;

            //Overwrite Warning Message
            if (NewGame.PieceList[selectedPieceIndex].PiecePromotion != null)
            {
                string message = "Piece \"" + selectedPiece + "\" already has promoting capabilities. Overwrite?";
                const string caption = "Promotion Overwriting";
                var result = MessageBox.Show(message, caption, MessageBoxButtons.OKCancel);
                if (result == DialogResult.Cancel) return;
            }

            NewGame.PieceList[selectedPieceIndex].PiecePromotion = null;

            string promotionZone = comboBoxPromotionZone.SelectedItem.ToString();
            List<string> pieceTypes = new List<string>();
            foreach (var item in checkedListBoxPromotionPieceTypes.CheckedItems)
                pieceTypes.Add(item.ToString());

            NewGame.PieceList[selectedPieceIndex].PiecePromotion = new GameData.Promotion(promotionZone, pieceTypes);
            buttonPromotionRemove.Enabled = true;
        }

        private void ButtonPromotionRemoveMouseClick(object sender, MouseEventArgs e)
        {
            int selectedPieceIndex = listBoxSetupPieceList.SelectedIndex;
            if (selectedPieceIndex < 0) return;
            NewGame.PieceList[selectedPieceIndex].PiecePromotion = null;
            buttonPromotionRemove.Enabled = false;
            if (checkedListBoxPromotionPieceTypes.CheckedIndices.Count > 0)
                buttonPromotionAdd.Enabled = true;
        }

        /// <summary>
        /// Goals Tab
        /// </summary>

        private void GoalsTabControl()
        {
            checkedListBoxGoalPieceTypes.Items.Clear();
            checkedListBoxGoalZones.Items.Clear();
            comboBoxGoalPlayersAffected.Items.Clear();
            listBoxGoalDirections.Items.Clear();
            listBoxGoalList.Items.Clear();

            List<string> zoneNames = NewGame.ZoneNamesList();
            foreach (var zone in zoneNames) checkedListBoxGoalZones.Items.Add(zone);
            foreach (var piece in NewGame.PieceList) checkedListBoxGoalPieceTypes.Items.Add(piece.Name);    
            IList<string> goalsPlayersAffected = new[] {"Both Players", NewGame.P1Name, NewGame.P2Name };
            foreach(var gpa in goalsPlayersAffected) comboBoxGoalPlayersAffected.Items.Add(gpa);
            if (comboBoxGoalPlayersAffected.SelectedIndex < 0) comboBoxGoalPlayersAffected.SelectedIndex = 0;
            foreach (var goal in NewGame.GoalList) listBoxGoalList.Items.Add(goal.Name);

            if (checkedListBoxGoalPieceTypes.Items.Count == 1)
            {
                checkedListBoxGoalPieceTypes.SetItemChecked(0, true);
                checkedListBoxGoalPieceTypes.Enabled = false;
            }
            else checkedListBoxGoalPieceTypes.Enabled = true;

            radioButtonStalemate.Enabled = NewGame.PassTurn <= 0;
        }

        private void RadioButtonGoalTypeCheckedChanged(object sender, EventArgs e)
        {//TODO!!! All the goals
            
            RadioButton current = (RadioButton)sender;
            
            if (!current.Checked) return; // If the Goal Type selected didn't changed, return
            
            List<RadioButton> rbl = new List<RadioButton> //Creates a list of all the Goal Type radioButtons
            {   radioButtonOccupy, radioButtonClaim, radioButtonRace, radioButtonBreakthrough, radioButtonConnection,
                radioButtonPattern, radioButtonStalemate, radioButtonCheckmate, radioButtonCapture};
            
            foreach (RadioButton radioButton in rbl) // Checks to false all the Goal Types except the selected one
                if (radioButton != current) radioButton.Checked = false;

            string selectedGoalType = current.Name.Substring(11);

            //if (checkedListBoxGoalZones.Items.Count == 1)
            //    checkedListBoxGoalZones.SetItemChecked(0, true);            

            radioButtonGoalOnePieceCaptured.Enabled = checkedListBoxGoalPieceTypes.CheckedIndices.Count > 0 &&
                                                      radioButtonCapture.Checked;

            switch (selectedGoalType)
            {
                case "Occupy":
                    groupBoxGoalZones.Enabled = groupBoxGoalPieceTypesAffected.Enabled = true;
                    groupBoxGoalEndGameCondition.Enabled = groupBoxGoalDirections.Enabled = false;
                    break;
                case "Claim":
                    //TODO a combo box with all the claim goal options is necessary
                    //TODO verify spaces in the board 'owned' indirectly by the players, determined by rules of relative positioning
                    groupBoxGoalEndGameCondition.Enabled = groupBoxGoalZones.Enabled = false;
                    groupBoxGoalDirections.Enabled = groupBoxGoalPieceTypesAffected.Enabled = true;
                    break;
                case "Connection":
                    //TODO define which pieces need to be connected and how
                    groupBoxGoalEndGameCondition.Enabled = false;
                    groupBoxGoalZones.Enabled = groupBoxGoalPieceTypesAffected.Enabled = groupBoxGoalDirections.Enabled = true;
                    break;
                case "Pattern":
                    //TODO! directions list and code gen
                    groupBoxGoalEndGameCondition.Enabled = groupBoxGoalZones.Enabled = false;
                    groupBoxGoalPieceTypesAffected.Enabled = groupBoxGoalDirections.Enabled = true;
                    break;
                case "Race":
                    //TODO define goal positions for each player
                    groupBoxGoalPieceTypesAffected.Enabled = groupBoxGoalZones.Enabled = true;
                    groupBoxGoalDirections.Enabled = groupBoxGoalEndGameCondition.Enabled = false;
                    break;
                case "Breakthrough":
                    //TODO define who is the cat and who is the mouse, and goal positions for the mouse
                    groupBoxGoalPieceTypesAffected.Enabled = groupBoxGoalZones.Enabled = groupBoxGoalEndGameCondition.Enabled = true;
                    groupBoxGoalDirections.Enabled = false;
                    break;
                case "Checkmate":
                    groupBoxGoalPieceTypesAffected.Enabled = true;
                    groupBoxGoalDirections.Enabled = groupBoxGoalEndGameCondition.Enabled = groupBoxGoalZones.Enabled = false;
                    break;
                case "Stalemate":
                    groupBoxGoalDirections.Enabled = groupBoxGoalZones.Enabled = groupBoxGoalEndGameCondition.Enabled
                        = groupBoxGoalPieceTypesAffected.Enabled = false;
                    break;
                case "Capture": 
                    groupBoxGoalPieceTypesAffected.Enabled = true;
                    groupBoxGoalDirections.Enabled = groupBoxGoalEndGameCondition.Enabled = groupBoxGoalZones.Enabled = false;
                    break;
            }
            buttonAddGoal.Enabled = true;
        }

        private void ButtonAddGoalMouseClick(object sender, MouseEventArgs e)
        {
            List<RadioButton> rbl = new List<RadioButton> //Creates a list of all the Goal Type radioButtons
            {   radioButtonOccupy, radioButtonClaim, radioButtonRace, radioButtonBreakthrough, radioButtonConnection,
                radioButtonPattern, radioButtonStalemate, radioButtonCheckmate, radioButtonCapture};
            
            string type = string.Empty;
            foreach (RadioButton radioButton in rbl) // Checks which radio button is selected
                if (radioButton.Checked)
                {
                    type = radioButton.Name.Substring(11);
                    break;
                }

            if (!CheckValidGoal(type)) return;

            int playersAffected = comboBoxGoalPlayersAffected.SelectedIndex; // 0 - Both; 1 - Player1; 2 - Player2
            
            List<GameData.Piece> pieceTypes = new List<GameData.Piece>();
            if (!type.Equals("Stalemate")) // only Stalemate goals have no piece association
                for (int i = 0; i < checkedListBoxGoalPieceTypes.Items.Count; i++)
                    if (checkedListBoxGoalPieceTypes.GetItemChecked(i)) pieceTypes.Add(NewGame.PieceList[i]);
            
            List<GameData.Zone> goalZones = new List<GameData.Zone>();
            if(type.Equals("Race")||type.Equals("Breakthrough")||type.Equals("Occupy")) //Only these goals may have zone associations
                for (int i = 0; i < checkedListBoxGoalZones.Items.Count; i++)
                    if (checkedListBoxGoalZones.GetItemChecked(i)) goalZones.Add(NewGame.ZoneList[i]);
            
            string name = NewGame.NameGoal(type);
            
            int winDrawLoss = 0;
            if (radioButtonGoalLoss.Checked) winDrawLoss = 1;
            if (radioButtonGoalDraw.Checked) winDrawLoss = 2;

            //If only one piece needs to be captured value is -1, otherwise it's the number of remaining pieces on the board
            int value = radioButtonGoalOnePieceCaptured.Checked ? -1 : (int) numericUpDownGoalRemainingPiecesNumber.Value;

            List<string> directions = new List<string>();
            foreach (var direction in listBoxGoalDirections.Items) directions.Add(direction.ToString());
            
            GameData.Goal newGoal = new GameData.Goal(name, type, pieceTypes, value, goalZones, winDrawLoss, playersAffected, directions);
            
            string checkExists = NewGame.CheckExistingGoal(newGoal);
            if (!checkExists.Equals(""))
            {   
                MessageBox.Show(@"The goal " + checkExists + @" has the same attributes");
                return; 
            }
            
            NewGame.GoalList.Add(newGoal);

            listBoxGoalList.Items.Add(newGoal.Name);

            listBoxGoalDirections.Items.Clear();

            buttonGoalRemove.Enabled = true;
            buttonGoalMoveUp.Enabled = buttonGoalMoveDown.Enabled = listBoxGoalList.Items.Count > 1;
            listBoxGoalList.SelectedIndex = listBoxGoalList.Items.Count - 1;
            listBoxGoalDirections.Items.Clear();
        }
        
        private bool CheckValidGoal(string type)
        {
            int pieceTypes = checkedListBoxGoalPieceTypes.CheckedIndices.Count;
            int zones = checkedListBoxGoalZones.CheckedIndices.Count;
            int directions = listBoxGoalDirections.Items.Count;

            if ((type.Equals("Occupy") || type.Equals("Race") || type.Equals("Breakthrough")) && (zones == 0))
            {
                MessageBox.Show(@"'" + type + @"' goals must have at least one zone selected");
                return false;
            }
            
            if (!type.Equals("Stalemate") && pieceTypes == 0)
            {
                MessageBox.Show(@"'" + type + @"' goals must have at least one piece type selected");
                return false;
            }
            if ((type.Equals("Pattern")) && (directions == 0)) // TODO maybe claim as well
            {
                MessageBox.Show(@"'" + type + @"' goals must have at least one direction selected");
                return false;
            }
            //TODO chain types for connection and claim?
            return true;
        }

        private void ButtonGoalListBoxMouseClick(object sender, MouseEventArgs e)
        {
            int selectedIndex = listBoxGoalList.SelectedIndex;
            if (selectedIndex < 0) 
            {
                buttonGoalMoveUp.Enabled = buttonGoalMoveDown.Enabled = buttonGoalRemove.Enabled = false;
                return; // if no goal is selected
            }
            buttonGoalRemove.Enabled = true;
            buttonGoalMoveUp.Enabled = selectedIndex >= 1 && selectedIndex <= listBoxGoalList.Items.Count - 1;
            buttonGoalMoveDown.Enabled = selectedIndex >= 0 && selectedIndex <= listBoxGoalList.Items.Count - 2;

            List<GameData.Goal> existingGoals = new List<GameData.Goal>();
            existingGoals.InsertRange(0, NewGame.GoalList);

            List<GameData.Goal> newGoalList = new List<GameData.Goal>();
            List<GameData.Goal> goalsSelected = new List<GameData.Goal>();
            
            for(int i = 0; i < listBoxGoalList.Items.Count; i++)
                if(listBoxGoalList.SelectedIndices.Contains(i))
                    foreach (GameData.Goal goal in NewGame.GoalList)
                        if(listBoxGoalList.Items[i].Equals(goal.Name))
                            goalsSelected.Add(goal);
            
            Button current = (Button)sender;
            string buttonName = current.Name.Substring(10);
            bool moveUp = false;
            bool moveDown = false;
            switch (buttonName)
            {
                case "MoveUp":
                    newGoalList.InsertRange(0, existingGoals);
                    foreach (GameData.Goal t in goalsSelected)
                        for (int j = 0; j < newGoalList.Count; j++)
                        {
                            if (t != newGoalList[j] || j <= 0) continue; // bug here
                            GameData.Goal temp = newGoalList[j - 1];
                            newGoalList.Remove(temp);
                            newGoalList.Insert(j, temp);
                            break;
                        }
                    NewGame.GoalList.Clear();
                    NewGame.GoalList.InsertRange(0,newGoalList);
                    moveUp = true;
                    break;
                case "MoveDown":
                    newGoalList.InsertRange(0, existingGoals);
                    foreach (GameData.Goal t in goalsSelected)
                        for (int j = 0; j < newGoalList.Count; j++)
                        {
                            if (t != newGoalList[j] || j  >= newGoalList.Count) continue; // bug here
                            GameData.Goal temp = newGoalList[j + 1];  
                            newGoalList.Remove(temp);
                            newGoalList.Insert(j, temp);
                            break;
                        }
                    NewGame.GoalList.Clear();
                    NewGame.GoalList.InsertRange(0,newGoalList);
                    moveDown = true;
                    break;
                    
                case "Remove":
                    foreach(GameData.Goal goal in goalsSelected)
                        NewGame.GoalList.Remove(goal);
                    break;
            }

            listBoxGoalList.Items.Clear();
            foreach (var goal in NewGame.GoalList) listBoxGoalList.Items.Add(goal.Name);

            if (moveDown && selectedIndex + 1 <= listBoxGoalList.Items.Count - 1)
                listBoxGoalList.SelectedIndex = selectedIndex + 1;

            if (moveUp && selectedIndex - 1 >= 0)
                listBoxGoalList.SelectedIndex = selectedIndex - 1;
            if (listBoxGoalList.Items.Count == 0) buttonGoalRemove.Enabled = false;
            if (listBoxGoalList.SelectedIndex < 0)
            {
                buttonGoalMoveUp.Enabled = false;
                buttonGoalMoveDown.Enabled = false;
                buttonGoalRemove.Enabled = false;
            } 

        }

        private void RadioButtonsGoalEndGameConditionCheckedChanged(object sender, EventArgs e)
        {
            numericUpDownGoalRemainingPiecesNumber.Enabled = radioButtonGoalRemainingPieces.Checked;
        }


        private void CheckedListBoxGoalPieceTypes(object sender, EventArgs e)
        {
            radioButtonGoalOnePieceCaptured.Enabled = checkedListBoxGoalPieceTypes.CheckedIndices.Count > 0 &&
                                                      radioButtonCapture.Checked;
        }

        private void ButtonsGoalDirectionMouseClick(object sender, MouseEventArgs e)
        {
            Button current = (Button)sender;
            string direction = current.Name.Substring(19);
            listBoxGoalDirections.Items.Add(direction);
        }

        private void ButtonGoalDirectionsRemoveClick(object sender, EventArgs e)
        {
            int selectedIndex = listBoxGoalDirections.SelectedIndex;
            if (selectedIndex < 0) return;
            listBoxGoalDirections.Items.RemoveAt(selectedIndex);
            listBoxGoalDirections.SelectedIndex = listBoxGoalDirections.Items.Count - 1; // maybe don't go to the last one
        }

        private void ListBoxGoalDirectionsSelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = listBoxGoalDirections.SelectedIndex;
            buttonGoalDirectionsRemove.Enabled = selectedIndex >= 0;
            if (selectedIndex < 0) return;
        }

        private void ListBoxGoalListSelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = listBoxGoalList.SelectedIndex;
            if (selectedIndex < 0)
            {
                buttonGoalRemove.Enabled = buttonGoalMoveDown.Enabled = buttonGoalMoveUp.Enabled = false;
                return;
            }
            buttonGoalRemove.Enabled = true;
            buttonGoalMoveUp.Enabled = selectedIndex >= 1 && selectedIndex <= listBoxGoalList.Items.Count - 1;
            buttonGoalMoveDown.Enabled = selectedIndex >= 0 && selectedIndex <= listBoxGoalList.Items.Count - 2;
            
            string goalName = listBoxGoalList.Items[selectedIndex].ToString();
            GameData.Goal goal = NewGame.FindGoal(goalName);

            if (goal == null) return;
            switch (goal.Type)
            {
                case "Occupy": radioButtonOccupy.Checked = true; break;
                case "Claim": radioButtonClaim.Checked = true; break;
                case "Race": radioButtonRace.Checked = true; break;
                case "Breakthrough": radioButtonBreakthrough.Checked = true; break;
                case "Pattern": radioButtonPattern.Checked = true; break;
                case "Connection": radioButtonConnection.Checked = true; break;
                case "Stalemate": radioButtonStalemate.Checked = true; break;
                case "Capture": radioButtonCapture.Checked = true; break;
                case "Checkmate": radioButtonCheckmate.Checked = true; break;
            }

            for (int i = 0; i < checkedListBoxGoalZones.Items.Count; i++)
                checkedListBoxGoalZones.SetItemChecked(i, false);
            for (int i = 0; i < checkedListBoxGoalPieceTypes.Items.Count; i++)
                checkedListBoxGoalPieceTypes.SetItemChecked(i, false);

            if (goal.GoalZones.Count > 0)
            {
                foreach (var zone in goal.GoalZones)
                {
                    int index = checkedListBoxGoalZones.Items.IndexOf(zone.Name);
                    if (index >= 0) checkedListBoxGoalZones.SetItemChecked(index, true);
                }
            }

            if (goal.PieceTypesAffected.Count > 0)
            {
                foreach (var piece in goal.PieceTypesAffected)
                {
                    int index = checkedListBoxGoalPieceTypes.Items.IndexOf(piece.Name);
                    if (index >= 0) checkedListBoxGoalPieceTypes.SetItemChecked(index, true);
                }
            }

            listBoxGoalDirections.Items.Clear();
            foreach (var direction in goal.Directions)
                listBoxGoalDirections.Items.Add(direction);

            switch (goal.WinLossDraw)
            {
                case 0: radioButtonGoalWin.Checked = true; break;
                case 1: radioButtonGoalLoss.Checked = true; break;
                case 2: radioButtonGoalDraw.Checked = true; break;
            }

            comboBoxGoalPlayersAffected.SelectedIndex = goal.PlayersAffected;
        }

        /// <summary>
        /// Auxiliary methods
        /// </summary>

        //Updates mandatory game fields (like the names of game items), but only if valid text is inserted
        private void TextBoxFocusLeave(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string message = textBox.Name.Substring(7);
            message = message.Replace("_", @" ");

            if (!Program.CheckLegalName(textBox, message))
            {
                textBox.Focus();
                return;
            }
            
            if (!VerifyUsedName(textBox.Text, textBox)) return;
            
            switch (message)
            {
                case "Game Title":      NewGame.GameTitle = textBox.Text; break;
                case "Player 1 Name":   NewGame.P1Name = textBox.Text; break;
                case "Player 2 Name":   NewGame.P2Name = textBox.Text; break;
                case "Move Name":       buttonAddMove.Enabled = true; break;
                case "Piece Name":      buttonAddPiece.Enabled = listBoxPieceMovesList.Items.Count > 0;break;
            }
        }
        
        //Updates game optional fields, but only if valid text is inserted
        private void TextBoxOptionalChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string message = textBox.Name.Substring(7);
            if (!Program.CheckSomeIllegalOperators(textBox, message = message.Replace("_", @" "))) return;
            switch (message)
            {
                case "Game Description":    NewGame.GameDescription = textBox.Text; break;
                case "Game History":        NewGame.GameHistory = textBox.Text;     break;
                case "Game Strategy":       NewGame.GameStrategy = textBox.Text;    break;
                case "Piece Help":          /*_tempPiece.Help = textBox.Text;*/     break;
                case "Piece Description":   /*_tempPiece.Description = textBox.Text;*/break;
            }

        }

        //Handles all the NumericUpDown controls in the interface
        private void NumericUpDownValueChanged(object sender, EventArgs e)
        {
            NumericUpDown current = (NumericUpDown)sender;
            string type = "";

            if (current.Name.Contains("OffBoardP1")) type = "OffBoardP1";
            if (current.Name.Contains("OffBoardP2")) type = "OffBoardP2";
            if (current.Name.Contains("Rows")) type = "Rows";
            if (current.Name.Contains("Columns")) type = "Columns";
            
            int selectedIndex = 0;
            int value = (int)current.Value;

            if (type.Contains("OffBoard"))
            {
                selectedIndex = listBoxSetupPieceList.SelectedIndex;
                if (selectedIndex < 0) return;
            }

            switch (type)
            {
                case "OffBoardP1": NewGame.PieceList[selectedIndex].OffBoardP1 = value; return;
                case "OffBoardP2": NewGame.PieceList[selectedIndex].OffBoardP2 = value; return;
                case "Rows": NewGame.BoardRowNumber = value; break;
                case "Columns": NewGame.BoardColumnNumber = value; break;
            }

            if (NewGame.AutomaticBoard) GenerateBoard();
        }

        //This method returns the board position given a set of mouse coordinates
        private string FindPositionInBoard(int x, int y)
        {
            int width = pictureBoxBoardImageSetup.Size.Width;
            int height = pictureBoxBoardImageSetup.Size.Height;
            int rows = (int)numericUpDownRows.Value;
            int columns = (int)numericUpDownColumns.Value;
            int stepX = width / columns;
            int stepY = height / rows;

            string result = null;

            //TODO remove this if Define Board Positions in GameData works correctly
            string[,] boardPositions = new string[columns, rows];

            int counter = rows;
            char letter = 'a';

            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                    boardPositions[i, j] = letter.ToString() + counter--;
                letter++;
                counter = rows;
            }

            int xrem = x - x % stepX;
            int yrem = y - y % stepY;

            for (int i = 0; i < columns; i++)
                for (int j = 0; j < rows; j++)
                    if (xrem >= stepX * i && xrem < stepX * (i + 1) && yrem >= stepY * j && yrem < stepY * (j + 1))
                        result = boardPositions[i, j];
            return result;
        }

        private void TextBoxTextChanged(object sender, EventArgs e)
        {
            TextBox current = (TextBox)sender;
            if (!string.IsNullOrWhiteSpace(current.Text)) return;
            if (current.Name.Contains("MoveName")) buttonAddMove.Enabled = false;
            if (current.Name.Contains("PieceName")) buttonAddPiece.Enabled = false;
        }

        /// <summary>
        /// Mouse Events handlers
        /// </summary>

        //Handles all the color choosing panels of the interface
        private void PanelColorChooserClick(object sender, MouseEventArgs e)
        {
            if (colorDialogColorChooser.ShowDialog() != DialogResult.OK) return;
            Panel current = (Panel)sender;
            current.BackColor = colorDialogColorChooser.Color;

            if (current.Name.Equals("panelColor1")) NewGame.Color1 = current.BackColor;
            if (current.Name.Equals("panelColor2")) NewGame.Color2 = current.BackColor;
            GenerateBoard();
        }

        //Handles all image choosing options (Board and Pieces)
        private void ImageChooser(object sender, MouseEventArgs e)
        {
            if (openFileDialogImageChooser.ShowDialog() != DialogResult.OK) return;

            if (sender.Equals(textBoxBoardImage))
            {
                textBoxBoardImage.Text = openFileDialogImageChooser.FileName;
                pictureBoxBoard.Load(textBoxBoardImage.Text);
                NewGame.BoardImage = new Bitmap(textBoxBoardImage.Text);
                _imagePath = textBoxBoardImage.Text;
                return;
            }

            PictureBox current = (PictureBox)sender;
            if (current.Name.Equals("pictureBoxPieceP1PieceImage")) pictureBoxPieceP1PieceImage.Load(openFileDialogImageChooser.FileName);
            if (current.Name.Equals("pictureBoxPieceP2PieceImage")) pictureBoxPieceP2PieceImage.Load(openFileDialogImageChooser.FileName);
        }

        //Tooltips in the status bar for all Group Boxes
        private void GroupBoxMouseEnter(object sender, EventArgs e)
        {   //TODO all tooltips...
            string message;
            GroupBox current = (GroupBox)sender;
            switch (current.Text)
            {
                case "Title": message = "The title will be used for the .ZRF rules file and the containing folder";
                    break;
                case "Destination Folder": message = "All created files will be saved in the selected folder";
                    break;
                default: message = "Hover the mouse over something to view tooltip text";
                    break;
            }
            toolStripStatusLabelHelp.Text = message;
        }

        //This method forces the selection of all the text on the first time focus is given to a text box //TODO maybe remove, doesn't seem too user friendly
        private void TextBoxMouseClick(object sender, MouseEventArgs e)
        {
            TextBox text = (TextBox)sender;
            if (text.Text.Contains(" ") && !text.Text.Contains("\n")) text.SelectAll();
        }

        public bool VerifyUsedName(string name, TextBox textBox)
        {//TODO Fix the bug: when leaving focus by changing tabs the error message appears twice (why?)
            string type = textBox.Name.Substring(7);
            int index = -1;

            switch (type)
            {
                case "Game_Title": return true;
                case "Player_1_Name":
                case "Player_2_Name": index = 0; break;
                case "Zone_Name": index = 1; break;
                case "Move_Name": index = 2; break;
                case "Piece_Name": index = 3; break;
            }

            string foundIn = NewGame.FindUsedName(name);
            
            if (!type.Equals(foundIn) && !string.IsNullOrEmpty(foundIn))
            {

                tabControlMain.SelectedIndex = index;
                textBox.Focus();
                foundIn = foundIn.Replace("_", " ");
                MessageBox.Show(@"The name " + name + @" is already in use in " + foundIn);
                return false;
            }
            return true;
        }

        private void ToolStripMenuItemClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ToolStripMenuItemNew_Click(object sender, EventArgs e)
        {
            XmlManager start = new XmlManager();
            GameData newGame = start.LoadGame("c:\\zillions\\default.xml");
            LoadGameToInterface(newGame);
            StartInterface();
            tabControlMain.SelectedIndex = 0;
        }

        private void ToolStripMenuItemSave_Click(object sender, EventArgs e)
        {
            string path = "C:\\zillions\\" + NewGame.GameTitle + ".xml";
            
            XmlManager newXML = new XmlManager();
            if (!newXML.SaveGame(NewGame, path))
                MessageBox.Show(@"The file " + path + @" is in use by another process. Please close it and try again");
        }

        private void TextBoxBoardImageTextChanged(object sender, EventArgs e)
        {
            NewGame.BoardImagePath = textBoxBoardImage.Text;
        }

        private void ToolStripMenuItemOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;

            XmlManager newXml = new XmlManager();
            GameData newGame = newXml.LoadGame(openFileDialog.FileName);
            
            //Set interface options
            LoadGameToInterface(newGame);

            return;
        }

        private void LoadGameToInterface(GameData newGame)
        {
            //TODO board colors only work for specific named colors
            //Game Tab
            textBoxGame_Title.Text = newGame.GameTitle;
            textBoxDestinationFolder.Text = newGame.GamePath;

            if(!string.IsNullOrWhiteSpace(newGame.GameDescription))
                textBoxGame_Description.Text = newGame.GameDescription;

            if (!string.IsNullOrWhiteSpace(newGame.GameHistory))
                textBoxGame_Description.Text = newGame.GameHistory;

            if (!string.IsNullOrWhiteSpace(newGame.GameStrategy))
                textBoxGame_Description.Text = newGame.GameStrategy;

            textBoxPlayer_1_Name.Text = newGame.P1Name;
            textBoxPlayer_2_Name.Text = newGame.P2Name;
            
            switch (newGame.BoardSymmetry)
            {
                case "All": comboBoxBoardSymmetry.SelectedIndex = 0; break;
                case "None": comboBoxBoardSymmetry.SelectedIndex = 1; break;
                case "Vertical": comboBoxBoardSymmetry.SelectedIndex = 2; break;
                case "Horizontal": comboBoxBoardSymmetry.SelectedIndex = 3; break; 
            }

            comboBoxGamePassTurn.SelectedIndex = newGame.PassTurn;
            textBoxDestinationFolder.Text = newGame.GamePath;
            
            //Board Tab
            numericUpDownRows.Value = newGame.BoardRowNumber;
            numericUpDownColumns.Value = newGame.BoardColumnNumber;
            radioButtonBoardDrawingAutomatic.Checked = newGame.AutomaticBoard;
            radioButtonBackgroundImage.Checked = !newGame.AutomaticBoard;
            _imagePath = newGame.AutomaticBoard ? string.Empty : newGame.BoardImagePath;
            
            panelColor1.BackColor = newGame.Color1;
            panelColor2.BackColor = newGame.Color2;
            if (!string.IsNullOrWhiteSpace(newGame.BoardImagePath))textBoxBoardImage.Text = newGame.BoardImagePath;

            listBoxZones.Items.Clear();
            foreach (var zone in newGame.ZoneList) listBoxZones.Items.Add(zone.Name);
            
            comboBoxZonePlayersAffected.Items.Clear();
            
            //Moves Tab
            listBoxMovesList.Items.Clear();
            foreach (var move in newGame.MovesList) listBoxMovesList.Items.Add(move.Name);

            listBoxMovesMultipleMoves.Items.Clear();
            foreach (var multiMove in newGame.MultipleMoves) listBoxMovesMultipleMoves.Items.Add(multiMove.Name);

            numericUpDownMoveDistanceUpTo.Maximum =
                numericUpDownMoveDistanceExactly.Maximum =
                NewGame.BoardColumnNumber > NewGame.BoardRowNumber ? NewGame.BoardColumnNumber : NewGame.BoardRowNumber;
            numericUpDownJumpRadius.Maximum = NewGame.BoardRowNumber + NewGame.BoardColumnNumber;
            //Pieces Tab
            listBoxPiecesPieceList.Items.Clear();
            foreach (var piece in newGame.PieceList) listBoxPiecesPieceList.Items.Add(piece.Name);
            listBoxAvailableMovesList.Items.Clear();
            listBoxPieceMovesList.Items.Clear();
            pictureBoxPieceP1PieceImage.Image = null;
            pictureBoxPieceP2PieceImage.Image = null;

            //Setup Tab
            listBoxSetupPieceList.Items.Clear();
            foreach (var piece in newGame.PieceList) listBoxSetupPieceList.Items.Add(piece.Name);

            comboBoxPromotionZone.Items.Clear();
            foreach (var zone in NewGame.ZoneNamesList()) comboBoxPromotionZone.Items.Add(zone);
            pictureBoxSetupP1PieceImage.Image = null;
            pictureBoxSetupP2PieceImage.Image = null;

            //Goals Tab
            listBoxGoalList.Items.Clear();
            foreach (var goal in newGame.GoalList) listBoxGoalList.Items.Add(goal.Name);
            
            NewGame = new GameData();
            NewGame = newGame;
            GameTabControl();
            BoardTabControl();
            MovesTabControl();
            PieceTabControl();
            SetupTabControl();
            GoalsTabControl();
        }

        private void StartInterface()
        {
            //Game Tab
            textBoxGame_Title.Text = @"insert game title here";
            textBoxPlayer_1_Name.Text = @"insert player 1 name";
            textBoxPlayer_2_Name.Text = @"insert player 2 name";
            textBoxGame_Description.Text = @"write a short text explaining the game's rules and goals";
            textBoxGame_History.Text = @"add a some information by describing the game's history";
            textBoxGame_Strategy.Text = @"briefly explain the basic strategies and tactics that provide a good chance of winning the game";

            //BoardTab
            textBoxBoardImage.Text = @"choose an image for the board";
            textBoxZone_Name.Text = @"insert zone name here";

            //Moves Tab
            textBoxMove_Name.Text = @"insert move name here";

            //Piece Tab
            textBoxPiece_Name.Text = @"insert piece name here";
            textBoxPiece_Description.Text = PieceDescriptionMessage;
            textBoxPiece_Help.Text = PieceDescriptionMessage;
            
        }
    }
}
