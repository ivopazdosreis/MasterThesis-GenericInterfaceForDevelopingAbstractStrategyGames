using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.IO;
namespace AbstractGamesCreationKit
{
    public class ZRFCreator
    {
        private readonly GameData _newGame;
        private int _tab = 1;

        //TODO swaps
        //TODO can swaps move type have no direction associated?
        //TODO create <move-type> if there are different priorities
        
        //TODO make this optional or with more complex symmetries (diagonal only, mirror only, etc)
        //TODO enable more directions? (cylindrical boards, wormholes, spherical, etc)
        //todo -> Change DimensionNames() method for more dimensions or in the case of having user defined dimension names (in this case add the new directions to the list of invalid names)
        //TODO - allow more specialized turn orders
        //TODO advanced move types (trampling, pushing, pulling, stacking, sniping, teleporting, etc)
        //TODO advanced capturing: sandwich capture, etc
        //TODO more complex slides, jumps and swaps
        //TODO Do not generate unused moves - notify users?
        //TODO variant

        public ZRFCreator(object newGame)
        {
            _newGame = (GameData) newGame;
        }

        //Main code generation method, calls all other methods. It's organized as a typical ZRF file
        private string Game()
        {   //TODO Unimplemented ZRF Features:
            //TODO      sounds (capture, change, click,draw,drop,loss,move,opening,release,win)(.wav) & music (.mid)
            //TODO      default, option)
            
            string result = string.Empty;

            //Promotions
            result += PromotionDefinitions();

            //Moves Tab
            result += MoveDefinitions();

            //Game Tab
            result += "(game\n";
            result += GameOptions();
            result += GameTitle();
            result += GameOptionalInformation();
            result += Players();
            result += TurnOrder();
            result += MovePriorities();

            //Board Tab
            result += Board();

            //Pieces Tab
            result += Pieces(); //TODO - must verify that there is at least one piece

            //Setup tab
            result += BoardSetup();

            //Goals Tab
            result += Goals();
            result += ")\n";

            return result;
        }

        private string MovePriorities()
        {
            string result = string.Empty;
            int maxPriority = 0;
            List<int> priorities = new List<int>();
            foreach (var move in _newGame.MovesList)
            {
                if (priorities.Contains(move.Priority)) continue;
                priorities.Add(move.Priority);
                if (move.Priority > maxPriority) maxPriority = move.Priority;
            }

            result += AddTabs(_tab) + "(move-priorities";
            for(int i = 0; i <= maxPriority; i++)
            {
                if(!priorities.Contains(i)) continue;
                result += " priority-" + i;
            }
            return result + ")\n";
        }

        private string GameOptions()
        {
            string result = string.Empty;
            
            //Turn Passing
            switch (_newGame.PassTurn)
            {
                case 0: break; // No turn passing is the default Zillions option
                case 1:
                    result += AddTabs(1) + "(option \"pass turn\" true)\n"; break;
                case 2: 
                    result += AddTabs(1) + "(option \"pass turn\" forced)\n"; break;
            }
            return result;
        }

        //Defines Promotion 'add' movements to the pieces that have promoting capabilities
        private string PromotionDefinitions()
        {
            string result = string.Empty;
         
            List<GameData.Piece> promotingPieces = new List<GameData.Piece>();
            foreach (GameData.Piece piece in _newGame.PieceList)
                if (piece.PiecePromotion != null) promotingPieces.Add(piece);

            if (promotingPieces.Count == 0) return result;

            foreach (GameData.Piece promotingPiece in promotingPieces)
            {
                string zone = promotingPiece.PiecePromotion.PromotionZone;
                string pieceTypes = string.Empty;
                foreach (string pieceType in promotingPiece.PiecePromotion.PieceTypes)
                    pieceTypes += " " + pieceType;

                result += "(define " + promotingPiece.Name + "-add\n";
                result += AddTabs(1) + "(if (not-in-zone? " + zone + ")\n";
                result += AddTabs(2) + "add\n";
                result += AddTabs(1) + "else\n";
                result += AddTabs(2) + "(add" + pieceTypes + ")\n";
                result += AddTabs(1) + ")\n";
                result += ")\n";
            }

            return result;
        }

        /// <summary>
        /// Game Tab related methods
        /// </summary>
        private string GameTitle()
        {
            return AddTabs(_tab) + "(title \"" + _newGame.GameTitle + "\")\n";
        }

        private string GameOptionalInformation()
        {
            string result = string.Empty;

            string description = "";
            if (!string.IsNullOrWhiteSpace(_newGame.GameDescription)) 
                description = _newGame.GameDescription.Replace("\n", "\\");
            string history = "";
            if (!string.IsNullOrWhiteSpace(_newGame.GameHistory))
                history = _newGame.GameHistory.Replace("\n", "\\");
            string strategy = "";
            if (!string.IsNullOrWhiteSpace(_newGame.GameStrategy))
                strategy = _newGame.GameStrategy.Replace("\n", "\\");
            if (_newGame.GameDescription != string.Empty)
                result += AddTabs(_tab) + "(description \"" + description + "\")\n";
            if (_newGame.GameHistory != string.Empty)
                result += AddTabs(_tab) + "(history \"" + history + "\")\n";
            if (_newGame.GameStrategy != string.Empty)
                result += AddTabs(_tab) + "(strategy \"" + strategy + "\")\n";
            return result;
        }

        private string Players()
        {
            return AddTabs(_tab) + "(players " + _newGame.P1Name + " " + _newGame.P2Name + ")\n";
        }

        /// <summary>
        /// Board Tab related methods
        /// </summary>
        private string Board()
        {
            //TODO Unimplemented ZRF Features: dummy positions, kill-positions, links, unlink, symmetry
            string result = string.Empty;
            
            result += AddTabs(_tab++) + "(board\n";
            result += AddTabs(_tab) + "(image \"" + _newGame.BoardImagePath + "\")\n";
            result += AddTabs(_tab++) + "(grid\n";
            result += StartRectangle();
            result += Dimensions();
            result += Directions();
            result += AddTabs(--_tab) + ")\n";
            result += BoardSymmetry();
            result += DefineZones();
            result += AddTabs(--_tab) + ")\n";
            return result;
        }

        private string TurnOrder()
        {//TODO - allow more specialized turn orders
            return AddTabs(_tab) + "(turn-order " + _newGame.P1Name + " " + _newGame.P2Name + ")\n";
        }

        //Return a string with all the zones defined
        private string DefineZones()
        {
            string result = string.Empty;
            if (_newGame.ZoneList.Count <= 0) return result;    // If there are no zones return ""
            
            foreach (GameData.Zone zone in _newGame.ZoneList)
            {
                string positions = string.Empty;                // A zone is a group of one or more positions in the board
                foreach (string position in zone.Positions) positions += " " + position;
                positions = DiscardIllegalPositions(positions); // It is necessary to discard illegal positions
                if(positions.Length <= 1) continue;             // In case the zone was only a group of illegal positions

                string players = string.Empty; // Zones can affect one or both players
                switch (zone.PlayersAffected)
                {
                    case 0: players = _newGame.P1Name + " " + _newGame.P2Name; break;
                    case 1: players = _newGame.P1Name; break;
                    case 2: players = _newGame.P2Name; break;
                }

                result += AddTabs(_tab++) + "(zone\n";
                result += AddTabs(_tab) + "(name " + zone.Name + ")\n";
                result += AddTabs(_tab) + "(players " + players + ")\n";          
                result += AddTabs(_tab) + "(positions " + positions + ")\n";
                result += AddTabs(--_tab) + ")\n";
            }
            return result;
        }

        //Return a string of valid on-board positions (separated by ' '), discarding illegal ones (the board dimension may have changed)
        private string DiscardIllegalPositions(string positions)
        {
            string result = string.Empty;
            //Fill a Columns x Rows matrix with the names of the board positions (letters for columns, numbers for rows)
            int columns = _newGame.BoardColumnNumber;
            int rows = _newGame.BoardRowNumber;

            string[,] boardPositions = _newGame.BoardPositions; //TODO to test
            
            positions = positions.Substring(1); //To remove the first char: ' '
            //If a position doesn't exist in the board, it is discarded)
            for (int i = 0; i < columns; i++)
            {   for (int j = 0; j < rows; j++)
                {
                    if(positions.Length <= 0) break;
                    int index = positions.IndexOf(boardPositions[i, j]);
                    if(index < 0) continue;
                    result += boardPositions[i, j] + " ";
                    positions = positions.Replace(boardPositions[i, j], "");
                }
                if (positions.Length <= 0) break;
            }
            return result.Substring(0, result.Length - 1); // To remove the final ' '
        }

        private string StartRectangle()
        {
            _newGame.DefineStartRectangle();
            string positions = _newGame.StartRectangle.Left + " " + _newGame.StartRectangle.Top + " " + _newGame.StartRectangle.Right + " " + _newGame.StartRectangle.Bottom;
            return AddTabs(_tab) + "(start-rectangle " + positions + ") ; top-left position\n";
        }

        private string Dimensions()
        {
            string result = string.Empty;
            result += AddTabs(_tab++) + "(dimensions ; " + _newGame.BoardColumnNumber + "x" + _newGame.BoardRowNumber + "\n";
            result += AddTabs(_tab) + DimensionNames("columns") + " (" + _newGame.ColumnStep + " 0)) ; columns\n";
            result += AddTabs(_tab) + DimensionNames("rows") + " (0 " + _newGame.RowStep + ")) ; rows\n";
            result += AddTabs(--_tab) + ")\n";
            return result;
        }

        private string Directions()
        {//TODO enable more directions? (cylindrical boards, wormholes, spherical, etc)
            return AddTabs(_tab) + "(directions (nw -1 -1) (n 0 -1) (ne 1 -1) (e 1 0) (se 1 1) (s 0 1) (sw -1 1) (w -1 0))\n";
        }

        private string BoardSymmetry()
        {
            //TODO make this optional or with more complex symmetries (diagonal only, mirror only, etc) 
            switch (_newGame.BoardSymmetry)
            {
                case "All": return AddTabs(_tab) + "(symmetry " + _newGame.P2Name + " (n s) (s n) (e w) (w e) (nw se) (se nw) (ne sw) (sw ne))\n";
                case "None":    return null;
                case "Vertical": return AddTabs(_tab) + "(symmetry " + _newGame.P2Name + " (n s) (s n))\n";
                case "Horizontal": return AddTabs(_tab) + "(symmetry " + _newGame.P2Name + " (e w) (w e))\n";
            }
            return null;
        }

        //Defines the names for the specified dimension (must receive "rows" or "columns")
        //Each cell will be named in the style [Column Letter][Row Number]. Example: a1,a2,aN
        private string DimensionNames(string type)
        {//todo -> change this method for more dimensions or in the case of having user defined dimension names
            string result = @"(""";
            int counter = 0;
            char letter = 'a';
            bool isRow = false;

            if (type != "rows" && type != "columns") return null;

            if (type == "rows")
            {
                isRow = true;
                counter = _newGame.BoardRowNumber;
            }

            if (type == "columns") counter = _newGame.BoardColumnNumber;

            while (counter-- > 0)
            {
                result += isRow ? (counter + 1).ToString() : (letter++).ToString();
                if (counter > 0) result += "/";
            }
            return result + @"""";
        }

        /// <summary>
        /// Moves Tab related methods
        /// </summary>
        private string MoveDefinitions ()
        {
            string result = string.Empty;
            int i = 0;
            IEnumerator myEnumerator = _newGame.GetMovesNames().GetEnumerator();

            string addType = "add";
            while (myEnumerator.MoveNext())
                result += "(define " + myEnumerator.Current + " " + DefineMove(_newGame.MovesList.ElementAt(i++), addType) + ")\n";

            foreach (GameData.Piece piece in _newGame.PieceList)
            {
                if (piece.PiecePromotion != null)
                {
                    addType = "(" + piece.Name + "-add)";
                    foreach (GameData.Move move in piece.MovesList)
                        result += "(define " + piece.Name + "-" + move.Name + " " + DefineMove(move, addType) + ")\n";
                }
                foreach (GameData.MultipleMove multiMove in piece.MultipleMovesList)
                {
                    switch (multiMove.Type)
                    {
                        case 0: // the multiple move is a sequence of moves
                            break;
                        case 1: // one move is repeated
                            {
                                addType = "(add-partial " + multiMove.Name + ")";
                                GameData.Move move = multiMove.MoveSequence[0];
                                result += "(define " + move.Name + "-partial " + DefineMove(move, addType) + ")\n";
                            }
                            break;
                    }
                }
            }
            return result;
        }

        private string DefineMove(GameData.Move newMove, string addType)
        {
            string result = string.Empty;
            switch (newMove.Type)
            {
                case "drop":    GameData.Drop newDrop = (GameData.Drop)newMove;
                                result = DefineDrop(newDrop, addType);
                                break;
                case "slide":   GameData.Slide newSlide = (GameData.Slide)newMove;
                                result = DefineSlide(newSlide, addType);
                                break;
                case "jump":    GameData.Jump newJump = (GameData.Jump)newMove;
                                result = DefineJump(newJump, addType);
                                break;
                //case "swap": GameData.Swap newSwap = (GameData.Swap)newMove;
                //                result = DefineSwap(newSwap, addType);
                //                break;
            }
            return result;
        }

        //Returns the zone restriction verifications for a move
        public string DefineZones(GameData.Move move)
        {
            string result = string.Empty;
            if (move.MoveZones.Count() != 0)
            {
                string zoneType = string.Empty;
                switch (move.ZoneType)
                {
                    case 1: zoneType = "in-zone? "; break; // if inside the zone
                    case 2: zoneType = "not-in-zone? "; break; // if outside the zone
                }
                foreach (GameData.Zone zone in move.MoveZones)
                    result += "(verify (" + zoneType + zone.Name + ")) "; // each zone verification is added to the move
            }
            return result;
        }

        //Defines the various macros for each drop type
        private string DefineDrop(GameData.Drop newDrop, string addType)
        {
            string dropType = string.Empty;
            switch (newDrop.DropType)
            {
                case "empty": dropType = "(verify empty?) " + addType; break;
                case "enemy": dropType = "(verify enemy?)  " + addType; break;
                case "anywhere": dropType = addType; break;
            }
            return "(" + DefineZones(newDrop) + dropType + ")";
        }

        private string DefineSlide(GameData.Slide newSlide, string addType)
        {   
            //disregards direction, making a macro of a generic move in any direction
            
            //Slides available:
            //             | Non-Capt | Can Capt | Must Capt           
            //    Any      |    X     |    X     |    X      
            //    Up to N  |    X     |    X     |    X    
            //    Exactly N|    X     |    X     |    X    
            //    Furthest |    X     |    X     |    X    

            switch (newSlide.TravelType)
            {
                case "any":         return DefineSlideAny(newSlide, addType);
                case "furthest":    return DefineSlideFurthest(newSlide, addType);
                case "upto":        return DefineSlideUpTo(newSlide, addType);
                case "exactly":     return DefineSlideExactly(newSlide, addType);
            }
            return null;
        }

        // Defines Slide any number of spaces in any direction
        private string DefineSlideAny(GameData.Move newSlide, string addType)
        {
            string result = "(" + DefineZones(newSlide) + "$1 (while empty? ";
            switch (newSlide.Capture)
            {
                case 0: return result + addType + " $1))"; // Capturing not allowed (Non-capturing Queen)
                case 1: return result + addType + " $1) (verify not-friend?) " + addType + ")"; // Capturing allowed (Regular Queen)
                case 2: return result + " $1) (verify enemy?) " + addType + ")";// Capturing mandatory (Queen that only moves if capturing)
            }
            return null;
        }

        //Defines the Slides that move only to the furthest possible space in any direction
        private string DefineSlideFurthest(GameData.Slide newSlide, string addType)
        {
            string result = "(" + DefineZones(newSlide) + "(while ";
            switch (newSlide.Capture)
            {
                case 0: return result + "(empty? $1) $1) " + addType + ")"; // Capturing not allowed (Non-capturing Queen that slides to the furthest empty space)
                case 1: return result + "(and (empty? $1) (on-board? $1)) $1) (if (enemy? $1) $1 " + addType + " else " + addType + "))"; // Capturing allowed (Regular Queen that only moves to the furthest space, empty or enemy occupied)
                case 2: return result + "(empty? $1) $1) $1 (verify enemy?) " + addType + ")"; // Capturing mandatory (Queen that only moves to the furthest space if it's enemy occupied)
            }
            return null;
        }

        //Define Slide "Up To" n spaces (distance limited movements)
        private string DefineSlideUpTo(GameData.Slide newSlide, string addType)
        {
            string zoneCheck = DefineZones(newSlide);
            // Capturing not allowed (non-capturing Queen slide with max distance limit)
            if (newSlide.Capture == 0)
            {
                string result = string.Empty;
                for (int i = 0; i < newSlide.Distance; i++)
                    result += "$1 (verify empty?) " + addType + " ";
                return "(" + zoneCheck + result + ")";
            }

            // Capturing allowed (Regular Queen slide but with a max distance limit)
            if (newSlide.Capture == 1)
            {
                string[] result = new string[newSlide.Distance];

                result[0] = "(" + zoneCheck + "$1 (verify not-friend?) " + addType + ")";
                for (int i = 1; i < newSlide.Distance; i++)
                {
                    string aux = "(" + zoneCheck;
                    for (int j = i; j < newSlide.Distance; j++)
                        aux += "$1 (verify empty?)";
                    result[i] = aux + "$1 (verify not-friend?) " + addType + ")";
                }
                return result.Aggregate("", (current, t) => current + t);
            }

            // Capturing Mandatory (Queen that only moves if capturing and if within the distance permitted)
            if (newSlide.Capture == 2)
            {
                string[] result = new string[newSlide.Distance];
                result[0] = "(" + zoneCheck + "$1 (verify enemy?) " + addType + ")";
                for (int i = 1; i < newSlide.Distance; i++)
                {
                    string aux = "(" + zoneCheck;
                    for (int j = 1; j < newSlide.Distance; j++)
                        aux += "$1 (verify empty?)";
                    result[i] = aux + "$1 (verify enemy?) " + addType + ")";
                }
                return result.Aggregate("", (current, t) => current + t);
            }
            return null;
        }

        //Define Slides that move only the exact number of spaces in any direction
        private string DefineSlideExactly(GameData.Slide newSlide, string addType)
        {
            string result = "(" + DefineZones(newSlide);
            for (int i = 1; i < newSlide.Distance; i++)
                result += "$1 (verify empty?) ";
            switch (newSlide.Capture)
            {
                case 0: return result + "$1 (verify empty?) " + addType + ")";      // Capturing not allowed (Non-capturing Queen that moves exactly N spaces)
                case 1: return result + "$1 (verify not-friend?) " + addType + ")"; // Capturing allowed (Regular Queen that moves exactly N spaces)
                case 2: return result + "$1 (verify enemy?) " + addType + ")";      // Capturing mandatory (Queen that only moves if capturing and exactly N spaces)
            }
            return null;
        }

        private string DefineJump(GameData.Jump newJump, string addType)
        {
            //Jumps available:
            //             | Non-Capt | Can Capt | Must Capt           
            //    Any      |    X     |    X     |    X      
            //    Furthest |    -     |    -     |    -    
            //    Up to N  |    X     |    X     |    X
            //    Exactly N|    X     |    X     |    X
            //    Radius   |    X     |    X     |    X    

            switch (newJump.TravelType)
            {
                case "any":         return DefineJumpAny(newJump, addType);
                case "furthest":    return DefineJumpFurthest(newJump, addType);
                case "upto":        return DefineJumpUpTo(newJump, addType);
                case "exactly":     return DefineJumpExactly(newJump, addType);
                case "radius":      return DefineJumpRadius(newJump, addType);
            }
            return null;
        }

        private string DefineJumpAny(GameData.Jump newJump, string addType)
        {
            string result = string.Empty;
            int max = _newGame.BoardColumnNumber;
            if (max < _newGame.BoardRowNumber) max = _newGame.BoardRowNumber;

            for (int i = 2; i <= max; i++)
            {
                GameData.Jump partialJump = new GameData.Jump("", "", newJump.Priority, newJump.Capture, newJump.MoveZones, newJump.ZoneType, newJump.Directions, i, "", newJump.JumpOver, newJump.JumpCapture);
                result += DefineJumpExactly(partialJump, addType);
            }
            return result;
        }

        private string DefineJumpFurthest(GameData.Jump newJump, string addType)
        {//TODO - this may be hard to accomplish. maybe leave it for later (it's use is dubious at best)
            string result = string.Empty;
            return result;
        }

        private string DefineJumpUpTo(GameData.Jump newJump, string addType)
        {
            string result = string.Empty;
            for (int i = 2; i <= newJump.Distance; i++)
            {
                GameData.Jump partialJump = new GameData.Jump("", "",newJump.Priority,newJump.Capture,newJump.MoveZones, newJump.ZoneType,newJump.Directions,i,"",newJump.JumpOver,newJump.JumpCapture);
                result += DefineJumpExactly(partialJump, addType);
            }
            return result;
        }

        private string DefineJumpExactly(GameData.Jump newJump, string addType)
        {
            string result = string.Empty;
            
            string landingTarget = "empty?";
            string duringTarget = "";
            
            switch (newJump.Capture) // 0 - Not Allowed; 1 - Allowed; 2 - Mandatory
            {
                case 1:
                    switch (newJump.JumpCapture) // 0 - Capture on landing; 1 - Capture during jump; 2 - capture during jump and on landing
                    {   case 0: landingTarget = "not-friend?"; break;
                        case 1: duringTarget = "(if enemy? capture) "; break;
                        case 2: landingTarget = "not-friend?"; duringTarget = "(if enemy? capture) "; break;
                    }
                    break;
                case 2:
                    switch (newJump.JumpCapture)
                    {
                        case 0: landingTarget = "enemy?"; break;
                        case 1: duringTarget = "(verify enemy?) capture "; break;
                        case 2: landingTarget = "enemy?"; duringTarget = "(verify enemy?) capture "; break;
                    }
                    break;
            }
            
            string overCheck = string.Empty;
            switch (newJump.JumpOver)        // 0 - anything; 1- enemy pieces; 2- friendly pieces; 3- Enemy or Empty Spaces; 4- Friendly or Empty spaces; 5-Enemy or Friendly Pieces
            {
                case 0: overCheck = " "; break;
                case 1: overCheck = " (verify enemy?) "; break;
                case 2: overCheck = " (verify friend?) "; break;
                case 3: overCheck = " (verify not-friend?) "; break;
                case 4: overCheck = " (verify not-enemy?) "; break;
                case 5: overCheck = " (verify not-empty?) "; break;
            }

            //special case: mandatory capture, both capturing methods, over friends and enemies
            if (newJump.Capture == 2 && newJump.JumpCapture == 2 && newJump.JumpOver == 5) duringTarget = "(if enemy? capture) ";

            //special case: mandatory capture, jump capturing method, over friends and enemies
            if (newJump.Capture == 2 && newJump.JumpCapture == 1 && newJump.JumpOver == 5)
            {
                duringTarget = "(if enemy? capture (set-flag one-captured true)) ";
                landingTarget = "empty?) (verify (flag? one-captured)";
                result += "(set-flag one-captured false) ";
            }
            
            for (int i = 1; i < newJump.Distance; i++) 
                result += "$1" + overCheck + duringTarget;
            result += "$1 (verify " + landingTarget + ") ";

            return "(" + DefineZones(newJump) + result + addType + ")";
        }

        private string DefineJumpRadius(GameData.Jump newJump, string addType)
        {
            string result = string.Empty;
            int maxDistance = newJump.Distance;
            string landingTarget = "empty?";
            switch (newJump.Capture)
            {
                case 1: landingTarget = "not-friend?"; break;
                case 2: landingTarget = "enemy?"; break;
            }

            for (int i = 1; i < maxDistance; i++ )
            {
                result += "(" + DefineZones(newJump);
                int rowDistance = i;
                int colDistance = maxDistance - i;
                for (int j = 0; j < rowDistance; j++) result += "$1 ";
                for (int j = 0; j < colDistance; j++) result += "$2 ";
                result += "(verify "+ landingTarget + ")" + addType + ")";
            }
            
            return result;
        }

        //private string DefineSwap(GameData.Swap newSwap, string addType)
        //{
        //    string result = string.Empty;
        //    return result;
        //    throw new NotImplementedException(); ;
        //    //Swap places with piece to the North
        //    //(n cascade (verify not-empty?) from s add)
        //}

        /// <summary>
        /// 'Pieces' Tab related methods
        /// </summary>
        private string Pieces()
        {
            //TODO Unimplemented ZRF Features: attribute, dummy, notation, open
            string result = string.Empty;
            foreach (GameData.Piece piece in _newGame.PieceList)
            {
                result += AddTabs(_tab++) + "(piece\n";
                result += AddTabs(_tab) + "(name " + piece.Name + ")\n";

                if (!string.IsNullOrWhiteSpace(piece.Help))
                    result += AddTabs(_tab) + "(help \"" + piece.Name + ": " + piece.Help + "\")\n";
                if (!string.IsNullOrWhiteSpace(piece.Description))
                    result += AddTabs(_tab) + "(description \"" + piece.Name + "\\ " + piece.Description.Replace("\n", "\\") + "\")\n";

                result += AddTabs(_tab++) + @"(image " + _newGame.P1Name + " \"" + piece.ImageP1Path + "\"\n\t   ";
                result += AddTabs(--_tab) + _newGame.P2Name + " \"" + piece.ImageP2Path + "\")\n";

                result += WriteDrops(piece);
                result += WriteMoves(piece);
                result += AddTabs(--_tab) + ")\n"; // finish piece statement
            }
            return result;
        }

        //Adds the Drops block statement to the piece if there are drop moves available, updating zoning requirements when necessary
        private string WriteDrops(GameData.Piece piece)
        {
            string result = string.Empty;
            bool hasDrops = false;
            int priority = -1;

            List<GameData.Move> dropMoves = new List<GameData.Move>();
            foreach (GameData.Move move in piece.MovesList.Where(move => move.Type.Equals("drop"))) dropMoves.Add(move);
            
            foreach (GameData.Move move in dropMoves.OrderBy(move => move.Priority))
            {
                if (!hasDrops)
                {
                    hasDrops = true;
                    result += AddTabs(_tab++) + "(drops \n"; // initialize drops statement
                }

                GameData.Drop drop = (GameData.Drop) move;

                if(drop.Priority > priority) 
                {
                    priority = drop.Priority;
                    result += AddTabs(_tab) + "(move-type priority-" + priority + ")\n";
                }

                string dropName = drop.Name;
                if(piece.PiecePromotion != null) dropName = piece.Name + "-" + drop.Name;

                result += AddTabs(_tab) + "(" + dropName + ")\n";
                
            }

            if(hasDrops) result += AddTabs(--_tab) + ")\n"; // finish drops statement
            return result;
        }

        //Adds the Moves block statement to the piece if there are moves available (moves different than drops)
        private string WriteMoves(GameData.Piece piece) // TODO
        {
            string result = string.Empty;
            bool hasMoves = false;

            int priority = -1;

            List<GameData.Move> otherMoves = new List<GameData.Move>();
            foreach (GameData.Move move in piece.MovesList.Where(move => move.Type.Equals("slide")
                || move.Type.Equals("jump") || move.Type.Equals("swap"))) otherMoves.Add(move);

            foreach (GameData.Move move in otherMoves.OrderBy(move => move.Priority))
            {
                if (hasMoves == false)
                {
                    hasMoves = true;
                    result += AddTabs(_tab++) + "(moves \n"; // initialize moves statement
                }

                if (move.Priority > priority)
                {
                    priority = move.Priority;
                    result += AddTabs(_tab) + "(move-type priority-" + priority + ")\n";
                }

                //foreach (GameData.MultipleMove multiMove in piece.MultipleMovesList)
                //{
                //    foreach (GameData.Move tempMove in multiMove.MoveSequence)
                //    {
                //        if(move.Name.Equals(tempMove.Name))
                //            result += AddTabs(_tab) + "(move-type " + multiMove.Name + ")\n";
                //    }
                //}

                string moveName = move.Name;
                if (piece.PiecePromotion != null) moveName = piece.Name + "-" + move.Name;

                switch (move.Type)
                {
                    case "slide":
                        GameData.Slide slide = (GameData.Slide) move;
                        foreach (string direction in slide.Directions) //macro call for each direction
                            result += AddTabs(_tab) + "(" + moveName + " " + direction + ") \n";
                        break;
                    case "jump":
                        GameData.Jump jump = (GameData.Jump) move;
                        if (!jump.Directions.Contains("radius"))
                        {
                            foreach (string direction in jump.Directions)
                            {
                                //macro call for each direction
                                result += AddTabs(_tab) + "(" + moveName + " " + direction + ") \n";
                            }
                        }
                        else
                        {
                            result += AddTabs(_tab) + "(" + moveName + " " + "n e" + ") \n";
                            result += AddTabs(_tab) + "(" + moveName + " " + "s e" + ") \n";
                            result += AddTabs(_tab) + "(" + moveName + " " + "s w" + ") \n";
                            result += AddTabs(_tab) + "(" + moveName + " " + "n w" + ") \n";
                        }
                        break;
                    //case "swap": //TODO can swaps have no direction?
                    //    GameData.Swap swap = (GameData.Swap)move;
                    //    foreach (string direction in swap.Directions) //macro call for each direction
                    //        result += AddTabs(_tab) + "(" + moveName + " " + direction + ") \n";
                    //    break;
                }
            }

            if (hasMoves) result += AddTabs(--_tab) + ")\n"; // finalize moves statement
            return result;
        }

        /// <summary>
        /// 'Setup Tab' related methods
        /// </summary>
        
        private static bool CheckPlayer1Pieces(GameData.Piece piece)
        {
            return piece.OffBoardP1 > 0 || piece.PositionsP1.Count > 0;
        }
        private static bool CheckPlayer2Pieces(GameData.Piece piece)
        {
            return piece.OffBoardP2 > 0 || piece.PositionsP2.Count > 0;
        }

        private string BoardSetup()
        {
            if (!_newGame.PieceList.Exists(CheckPlayer1Pieces) && !_newGame.PieceList.Exists(CheckPlayer2Pieces))
                return null;

            string result = AddTabs(_tab++) + "(board-setup\n";
            
            //Player 1
            if (!_newGame.PieceList.Exists(CheckPlayer1Pieces)) goto Player2;
            
            result += AddTabs(_tab++) + "(" + _newGame.P1Name + "\n";
            foreach (GameData.Piece piece in
                _newGame.PieceList.Where(piece => piece.OffBoardP1 > 0 || piece.PositionsP1.Count > 0))
            {
                if (piece.OffBoardP1 > 0)
                    result += AddTabs(_tab) + "(" + piece.Name + " off " + piece.OffBoardP1 + ")\n";
                if (piece.PositionsP1.Count <= 0) continue;
                result += AddTabs(_tab) + "(" + piece.Name;
                result = piece.PositionsP1.Aggregate(result, (current, position) => current + (" " + position));
                result +=")\n";
            }
            result += AddTabs(--_tab) + ")\n";

 Player2:   //Player 2
            if (!_newGame.PieceList.Exists(CheckPlayer2Pieces)) return null;
            result += AddTabs(_tab++) + "(" + _newGame.P2Name + "\n";
            foreach (GameData.Piece piece in
                _newGame.PieceList.Where(piece => piece.OffBoardP2 > 0 || piece.PositionsP2.Count > 0))
            {
                if (piece.OffBoardP2 > 0)
                    result += AddTabs(_tab) + "(" + piece.Name + " off " + piece.OffBoardP2 + ")\n";
                if (piece.PositionsP2.Count <= 0) continue;
                result += AddTabs(_tab) + "(" + piece.Name;
                result = piece.PositionsP2.Aggregate(result, (current, position) => current + (" " + position));
                result += ")\n";
            }
            result += AddTabs(--_tab) + ")\n";

            return result + "\n" + AddTabs(--_tab) + ")\n";
        }

        /// <summary>
        /// 'Goals Tab' related methods
        /// </summary>
        /// 
        private string Goals()
        {   //TODO      goals: count-condition, draw-condition, loss-condition, win-condition
            //TODO Usable Goals ZRF features: stalemated, repetition, captured, checkmated, absolute-config, relative-config, pieces-remaining, total-piece-count
            string result = string.Empty;
            foreach (GameData.Goal goal in _newGame.GoalList)
            {
                switch (goal.Type)
                {
                    case "Occupy":  result += DefineGoalTypeOccupy(goal); break;
                    case "Claim":   break;
                    case "Race":    break;
                    case "Breakthrough":    break;
                    case "Connection":      break;
                    case "Pattern":     result += DefineGoalTypePattern(goal); break;
                    case "Checkmate":   result += DefineGoalTypeCheckmate(goal); break;
                    case "Stalemate":   result += DefineGoalTypeStalemate(goal); break;
                    case "Capture":     result += DefineGoalTypeCapture(goal); break;
                }
            }
            return result;
        }

        private string GoalPlayersAffected(int playersAffected)
        {
            switch (playersAffected)
            {
                case 0: return _newGame.P1Name + " " + _newGame.P2Name;
                case 1: return _newGame.P1Name;
                case 2: return _newGame.P2Name;
            }
            return null;
        }

        private static string GoalEndGameResult(int winDrawLoss)
        {
            switch (winDrawLoss)
            {
                case 0: return "win-condition";
                case 1: return "loss-condition";
                case 2: return "draw-condition";
            }
            return null;
        }

        private string DefineGoalTypeCheckmate(GameData.Goal goal)
        {
            string result = string.Empty;
            string endGameResult = GoalEndGameResult(goal.WinLossDraw);
            string players = GoalPlayersAffected(goal.PlayersAffected);

            result += AddTabs(_tab) + "(" + endGameResult + "(" + players + ") (checkmated";
            foreach (GameData.Piece piece in goal.PieceTypesAffected)
                result += " " + piece.Name;
            return result + "))\n";
        }

        private string DefineGoalTypeStalemate(GameData.Goal goal)
        {//TODO - won't work with pass turn option on on Zillions
            string players = GoalPlayersAffected(goal.PlayersAffected);
            string endGameResult = GoalEndGameResult(goal.WinLossDraw);
            // by default Zillions has the draw stalemate rule active
            return (AddTabs(_tab) + "(" + endGameResult + " (" + players + ") stalemated)\n");
        }

        private string DefineGoalTypeCapture(GameData.Goal goal)
        {
            string result = string.Empty;
            string players = GoalPlayersAffected(goal.PlayersAffected);
            string endGameResult = GoalEndGameResult(goal.WinLossDraw);

            string start = AddTabs(_tab) + "(" + endGameResult + " (" + players + ")";

            if (goal.Value == -1)
            {
                result = start + "(captured";
                foreach (GameData.Piece piece in goal.PieceTypesAffected)
                    result += " " + piece.Name;
                result += "))\n";
            }
            else
            {
                switch (goal.PieceTypesAffected.Count)
                {
                    case 0:
                        result = start + " (pieces-remaining " + goal.Value + "))\n";
                        break;
                    case 1:
                        foreach (GameData.Piece piece in goal.PieceTypesAffected)
                            result += start + " (pieces-remaining " + goal.Value + " " + piece.Name + "))\n";
                        break;
                    default:
                        result += start + "\n" + AddTabs(++_tab) + "(and \n";
                        _tab++;
                        foreach (GameData.Piece piece in goal.PieceTypesAffected)
                            result += AddTabs(_tab) + " (pieces-remaining " + goal.Value + " " + piece.Name + ")\n";
                        result += AddTabs(--_tab) + ")\n" + AddTabs(--_tab) + ")\n";
                        break;
                }
            }
            return result;
        }

        private string DefineGoalTypeOccupy(GameData.Goal goal)
        {
            string playersAffected = GoalPlayersAffected(goal.PlayersAffected);
            string endGameResult = GoalEndGameResult(goal.WinLossDraw);
            string checkOccupants = string.Empty; // any one of the piece types activates the goal
            string positionArgs = string.Empty; // the occupant must be inside all the chosen zones

            foreach (var pieceType in goal.PieceTypesAffected) checkOccupants += pieceType.Name + " ";
            foreach (var zone in goal.GoalZones) positionArgs += zone.Name + " ";

            return AddTabs(_tab) + "(" + endGameResult + "(" + playersAffected + ") (absolute-config " +
                      checkOccupants + "(" + positionArgs + ")))\n";
        }

        
        private string DefineGoalTypePattern(GameData.Goal goal)
        {
            string result = string.Empty;
            string playersAffected = GoalPlayersAffected(goal.PlayersAffected);
            string endGameResult = GoalEndGameResult(goal.WinLossDraw);
            List<string> directions = goal.Directions;
            foreach (var piece in goal.PieceTypesAffected)
            {
                result += AddTabs(_tab) + "(" + endGameResult + "(" + playersAffected + ") (relative-config ";
                foreach (var direction in directions) result += piece.Name + " " + direction.ToLower() + " ";
                result += piece.Name;
                result += "))\n";
            }
            return result;
        }
        
        /// <summary>
        /// Saves the board and piece images in the specified game folder
        /// </summary>
        private void SaveImages()
        {   //Define game folder
            string gamepath = _newGame.GamePath + _newGame.GameTitle + "\\";
            
            //Creates the board image if the automatic board image option is selected
            if (_newGame.AutomaticBoard) _newGame.BoardImage = _newGame.CreateBoardBitmap(true);
            
            //Saves the board image with the name MxNboard.bmp, where 'M' and 'N' are the number of columns and rows
            _newGame.BoardImagePath = gamepath + _newGame.BoardColumnNumber + "x" + _newGame.BoardRowNumber + "board.bmp";
            _newGame.BoardImage.Save(_newGame.BoardImagePath, ImageFormat.Bmp);

            foreach(GameData.Piece piece in _newGame.PieceList)
            {
                //The name of a piece image is [Piece Name]_[Player].bmp
                string newImageP1Path = gamepath + piece.Name + "_" + _newGame.P1Name + ".bmp";
                string newImageP2Path = gamepath + piece.Name + "_" + _newGame.P2Name + ".bmp";
                                
                //For each piece it is necessary to resize the image in order for it to fit in each cell of the board
                Bitmap imageP1 = new Bitmap(piece.ImageP1Path);
                imageP1 = new Bitmap(imageP1, _newGame.ColumnStep, _newGame.RowStep);
                Bitmap imageP2 = new Bitmap(piece.ImageP2Path);
                imageP2 = new Bitmap(imageP2, _newGame.ColumnStep, _newGame.RowStep);
                
                //piece.ImageP1.Save(piece.ImageP1Path, ImageFormat.Bmp);
                //piece.ImageP2.Save(piece.ImageP2Path, ImageFormat.Bmp);

                //SaveBmp(newImageP1Path, imageP1, 1);
                //SaveBmp(newImageP2Path, imageP2, 1);
                try
                {
                    imageP1.Save(newImageP1Path, ImageFormat.Bmp);
                    imageP2.Save(newImageP2Path, ImageFormat.Bmp);
                }
                catch (Exception)
                {
                    
                }
                piece.ImageP1Path = newImageP1Path;
                piece.ImageP2Path = newImageP2Path;

            }
        }
        
        public static void SaveBmp (string path, Image img, int quality)
        {
            EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, quality);

            EncoderParameters encoderParams= new EncoderParameters(1);

            encoderParams.Param[0] = qualityParam;

            MemoryStream mss = new MemoryStream();

            FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            img.Save(mss, ImageFormat.Bmp);
            byte[] matriz = mss.ToArray();
            fs.Write(matriz, 0, matriz.Length);

            mss.Close();
            fs.Close();
        }

        /// <summary>
        /// Creates a folder and a sub folder with the new game's title, then generate the ZRF file with the code
        /// </summary>
        public void GenerateZRF()
        {
            //TODO -> warning - if file exists then overwrite
            
            // Specify a "currently active folder"
            string activeDir = _newGame.GamePath;

            //Create a new subfolder under the current active folder
            string newPath = Path.Combine(activeDir, _newGame.GameTitle);
            Directory.CreateDirectory(newPath);

            // Create a new file name
            string newFileName = _newGame.GameTitle;
            Directory.CreateDirectory(newPath);
            
            // Combine the new file name with the path
            newPath = Path.Combine(newPath, newFileName);

            //Saves the board image in the folder of the game and updates pixel data for code generation
            //It also creates a copy of the images of each piece in the same folder
            SaveImages();

            //Code generation
            string code = Game();
            
            // Write the code as a string array into the .ZRF file
            File.WriteAllLines(newPath + ".ZRF", StringToArray(code, "\n"));
         }

        /// <summary>
        /// These methods are used for indentation of the generated ZRF code
        /// </summary>
        
        // Adds a tab to the code
        private static string AddTabs(int tabCounter)
        {
            string result = string.Empty;
            while (tabCounter-- > 0) result += "\t";
            return result;
        }
       
        // Code handling methods (get and convert to string for debugging)

        public string[] StringToArray(string input, string separator)
        {
            string[] stringList = input.Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
           return stringList;
        }
        ////Returns a string with the entire generated code, useful for debugging
        //public string GetStringCode()
        //{
        //    return GetCode().Aggregate("", (current, t) => current + t);
        //}

        ////Returns a string array with the code. Each line of code is a string in the array. (disregards tabs and line breaks)
        //private string[] GetCode()
        //{
        //    string[] result = new string[Code.Count];
        //    int i = 0;
        //    IEnumerator myEnumerator = Code.GetEnumerator();
        //    while (myEnumerator.MoveNext())
        //        result[i++] = myEnumerator.Current.ToString();// +"\n";
        //    return result;
        //}
    }
}
