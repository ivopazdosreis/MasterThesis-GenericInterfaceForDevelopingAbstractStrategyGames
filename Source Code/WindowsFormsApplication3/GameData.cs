using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AbstractGamesCreationKit
{
    /// <summary>
    /// This class is used to save all information regarding the new game, which will later be converted to a ZRF file
    /// </summary>
    public class GameData
    {
        /// <summary>
        /// Variable declarations for game data
        /// </summary>

        public string   GamePath          { get; set; }
        public string   GameTitle         { get; set; }
        public string   GameDescription   { get; set; }
        public string   GameHistory       { get; set; }
        public string   GameStrategy      { get; set; }
        public string   P1Name            { get; set; } // Unique
        public string   P2Name            { get; set; } // Unique
        public int      PassTurn          { get; set; }
        public bool     AutomaticBoard    { get; set; }
        public int      BoardRowNumber    { get; set; }
        public int      BoardColumnNumber { get; set; }
        public Color    Color1            { get; set; }
        public Color    Color2            { get; set; }
        public string   BoardSymmetry     { get; set; }

        public string BoardImagePath    { get; set; }
        public int    RowStep           { get; set; }
        public int    ColumnStep        { get; set; }
        public Bitmap BoardImage;
        public string[,] BoardPositions { get; set; }

        public List<Goal> GoalList = new List<Goal>();
        public List<Zone> ZoneList = new List<Zone>();
        public List<Move> MovesList = new List<Move>();
        public List<MultipleMove> MultipleMoves = new List<MultipleMove>();
        public List<Piece> PieceList = new List<Piece>();
        
        public Rectangle StartRectangle;
        

        public class Zone
        {
            public string Name { get; set; } // Can be the same for different zones if and only if the players affected are different
            public int PlayersAffected { get; set; } //0 - Both; 1 - P1; 2 - P2
            public List<string> Positions { get; set; }

            public Zone(string name, int playersAffected, List<string> positions)
            {
                Name = name;
                PlayersAffected = playersAffected;
                Positions = positions;
            }
            
            public Zone() { }
        }


        public class Goal
        {
            public string Name { get; set; } // for listing purposes
            public string Type { get; set; }
            public List<Piece> PieceTypesAffected;
            public int Value { get; set; } // can be number of captured pieces or other
            public List<Zone> GoalZones;
            public int WinLossDraw { get; set; } // 0 - win; 1- Loss; 2 - Draw
            public int PlayersAffected { get; set; } // 0 - both; 1 - player 1; 2 - player 2
            public List<string> Directions { get; set; }

            public Goal(string name, string type, List<Piece> pieceTypesAffected, int value, List<Zone> goalZones, int winLossDraw, int playersAffected, List<string> directions)
            {
                Name = name;
                Type = type;
                PieceTypesAffected = pieceTypesAffected;
                Value = value;
                GoalZones = goalZones;
                WinLossDraw = winLossDraw;
                PlayersAffected = playersAffected;
                Directions = directions;
            }

            public Goal(){}
        }
        
        public class Move
        {
            public string Name          { get; set;} // Unique for each move //TODO should be automatically generated
            public string Type          { get; set;} // Defines the move type (drop, slide, jump, etc)
            public int Priority         { get; set;} 
            public int Capture          { get; set;} // Values range from: 0 - Not Allowed, 1 - Allowed and 2 - Mandatory
            public List<Zone> MoveZones { get; set;} // List of the zones affecting this move 
            public int ZoneType         { get; set;} // Values range from: 0 - Indifferent, 1 - Inside and 2 - Outside (of zones in ZoneList)

            public Move(string name, string type, int priority, int capture, List<Zone> moveZones, int zoneType)
            {
                Name = name;
                Type = type;
                Priority = priority;
                Capture = capture; 
                MoveZones = moveZones;
                ZoneType = zoneType;
            }

        }

        public class Drop : Move
        {
            public string DropType { get; set; } // empty; enemy; anywhere
            public Drop(string name, string type, int priority, int capture, List<Zone> moveZones, int zoneType, string dropType)
                : base(name, type, priority, capture, moveZones, zoneType)
            {
                DropType = dropType;
            }
        }

        public class Slide : Move
        {
            public List<string> Directions { get; set; }
            public int Distance     { get; set; }
            public string TravelType { get; set; }
            public Slide(string name, string type, int priority, int capture, List<Zone> moveZones, int zoneType, List<string> directions, int distance, string travelType)
                : base(name, type, priority, capture, moveZones, zoneType)
            {
                Directions = directions;
                Distance = distance;
                TravelType = travelType;
            }
        }

        public class Jump : Move
        {
            public List<string> Directions  { get; set; }
            public int Distance             { get; set; }
            public string TravelType        { get; set; }
            public int JumpOver             { get; set; } // Jump Over: 0- anything; 1- enemy pieces; 2- friendly pieces; 3- Enemy or Empty Spaces; 4- Friendly or Empty spaces; 5-Enemy or Friendly Pieces
            public int JumpCapture          { get; set; } // 0 - Capture on landing; 1 - Capture during jump
            public Jump(string name, string type, int priority, int capture, List<Zone> moveZones, int zoneType, List<string> directions, int distance, string travelType, int jumpOver, int jumpCapture)
                : base(name, type, priority, capture, moveZones, zoneType)
            {
                Directions = directions;
                Distance = distance;
                TravelType = travelType;
                JumpOver = jumpOver;
                JumpCapture = jumpCapture;
            }
        }

        public class Swap : Move
        {
            public List<string> Directions { get; set; }
            public int Distance     { get; set; }
            public string TravelType { get; set; }
            public Swap(string name, string type, int priority, int capture, List<Zone> moveZones, int zoneType, List<string> directions, int distance, string travelType)
                : base(name, type, priority, capture, moveZones, zoneType)
            {
                Directions = directions;
                Distance = distance;
                TravelType = travelType;
            }
        }

        public List<string> GetMovesNames()
        {
            List<string> movesNames = new List<string>();
            foreach (Move move in MovesList)
                movesNames.Add(move.Name);
            return movesNames;
        }

        public class Piece
        {
            public string Name          { get; set; } // Unique for each Piece
            public string Help          { get; set; }
            public string Description   { get; set; }
            public string ImageP1Path   { get; set; }
            public string ImageP2Path   { get; set; }
            public int OffBoardP1       { get; set; }
            public int OffBoardP2       { get; set; }
            public List<string> PositionsP1 { get; set; }
            public List<string> PositionsP2 { get; set; }
            public List<Move> MovesList = new List<Move>();
            public List<MultipleMove> MultipleMovesList = new List<MultipleMove>();
            public Promotion PiecePromotion { get; set; } // Each piece can only have a promotion associated with it

            //bool atribbutes[]; //TODO
            //string notation;   // TODO

            public Piece(){}

            public Piece(string name, string help, string description, string imageP1Path, string imageP2Path, List<Move> movesList, List<MultipleMove> multiMoveList, List<string> positionsP1, List<string> positionsP2, int offBoardP1, int offBoardP2)
            {
                Name = name;
                Help = help;
                Description = description;
                ImageP1Path = imageP1Path;
                ImageP2Path = imageP2Path;
                MovesList = movesList;
                MultipleMovesList = multiMoveList;
                PositionsP1 = positionsP1;
                PositionsP2 = positionsP2;
                OffBoardP1 = offBoardP1;
                OffBoardP2 = offBoardP2;
            }
        }

        // A promotion consists of a promoting zone and a set of piece types that define what a piece can promote to
        public class Promotion
        {
            public string PromotionZone { get; set; }
            public List<string> PieceTypes { get; set; }

            public Promotion(string promotionZone, List<string> pieceTypes)
            {
                PromotionZone = promotionZone;
                PieceTypes = pieceTypes;
            }
        }

        public class MultipleMove
        {
            public string Name;
            public int Type; // 0 - Sequence of moves; 1 - Repeat a move
            public List<Move> MoveSequence = new List<Move>();

            public MultipleMove(string name, int type, List<Move> moveSequence)
            {
                Name = name;
                Type = type;
                MoveSequence = moveSequence;
            }

            public MultipleMove(){}
        }

        public Bitmap CreateBoardBitmap(bool final)
        {
            int squareSide = BoardRowNumber > 15 || BoardColumnNumber > 15 ? 15 : 25;

            if (final) squareSide = BoardRowNumber > BoardColumnNumber ? 400/BoardRowNumber : 400/BoardColumnNumber;

            int height = squareSide * BoardRowNumber;
            int width  = squareSide * BoardColumnNumber;

            BoardImage = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            
            bool colorflag = true;
            
            int[] m = new int[BoardRowNumber];
            int[] n = new int[BoardColumnNumber];
            for (int i = 1; i <= BoardRowNumber; i++) m[i-1] = i *squareSide;
            for (int i = 1; i <= BoardColumnNumber; i++) n[i-1] = i * squareSide;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    BoardImage.SetPixel(x, y, colorflag ? Color1 : Color2);

                    for (int i = 0; i < BoardColumnNumber; i++)
                    {
                        if (n[i] != x) continue;
                        colorflag = !colorflag;
                        i = BoardColumnNumber;
                    }

                    if (x == width - 1 && BoardColumnNumber%2 == 0)
                        colorflag = !colorflag;
                }

                for (int i = 0; i < BoardRowNumber; i++)
                {
                        if (m[i] != y) continue;
                        colorflag = !colorflag;
                        i = BoardRowNumber;
                }

            }
            DefineStartRectangle();
            return BoardImage;
        }

        public void DefineBoardPositions()
        {
            string[,] boardPositions = new string[BoardColumnNumber, BoardRowNumber];

            int counter = BoardRowNumber;
            char letter = 'a';

            for (int i = 0; i < BoardColumnNumber; i++)
            {
                for (int j = 0; j < BoardRowNumber; j++)
                    boardPositions[i, j] = letter.ToString() + counter--;
                letter++;
                counter = BoardRowNumber;
            }
            BoardPositions = boardPositions;
        }

        //Checks if a similar goal already exists, returns the existing goal if found, or an empty string if not
        public string CheckExistingGoal(Goal newGoal)
        {
            foreach (Goal goal in GoalList)
                if (goal.Type.Equals(newGoal.Type) && goal.PlayersAffected.Equals(newGoal.PlayersAffected)
                    && goal.Value.Equals(newGoal.Value) && goal.WinLossDraw.Equals(newGoal.WinLossDraw)
                    && Program.UnorderedEqual(newGoal.PieceTypesAffected, goal.PieceTypesAffected)
                    && Program.UnorderedEqual(newGoal.GoalZones, goal.GoalZones)
                    && Program.UnorderedEqual(newGoal.Directions, goal.Directions))
                    
                    return goal.Name;
            return "";
        }

        //Names each new goal in a successive and numeric fashion, according to its type
        public string NameGoal(string type)
        {
            int max = 0;
            foreach (Goal goal in GoalList)
            {
                if (goal.Type != type) continue;
                int newmax = Convert.ToInt16(goal.Name.Substring(type.Length));
                if (newmax >= max)
                    max = newmax + 1;
            }
            if (max == 0) max = 1;
            return type + max;
        }

        public class Rectangle
        {
            public int Left, Top, Right, Bottom;

            public Rectangle(int a, int b, int c, int d)
            {
                Left = a;
                Top = b;
                Right = c;
                Bottom = d;
            }
        }

        public void DefineStartRectangle()
        {
            RowStep = (int)(BoardImage.PhysicalDimension.Height / BoardRowNumber);
            ColumnStep = (int)(BoardImage.PhysicalDimension.Width / BoardColumnNumber);
            //MessageBox.Show("Height:" + BoardImage.PhysicalDimension.Height.Tostring() + "\nWidth:"+BoardImage.PhysicalDimension.Width.Tostring());
            StartRectangle = new Rectangle(0, 0, RowStep, ColumnStep);
        }

        public string FindUsedName(string name)
        {
            if (P1Name.Equals(name)) return "Player_1_Name";
            if (P2Name.Equals(name)) return "Player_2_Name";
            foreach (Move move in MovesList) if (move.Name.Equals(name)) return "Move_Name";
            foreach (Piece piece in PieceList) if (piece.Name.Equals(name)) return "Piece_Name";
            foreach (Zone zone in ZoneList) if (zone.Name.Equals(name)) return "Zone_Name";
            foreach (MultipleMove multipleMove in MultipleMoves) if (multipleMove.Name.Equals(name)) return "Multiple_Move_Name";
            return "";
        }

        public Move FindMove(string name)
        {
            foreach (Move move in MovesList) if (move.Name.Equals(name)) return move;
            return null;
        }

        public MultipleMove FindMultipleMove(string name)
        {
            foreach (MultipleMove move in MultipleMoves) if (move.Name.Equals(name)) return move;
            return null;
        }

        public Piece FindPiece(string name)
        {
            foreach (Piece piece in PieceList) if (piece.Name.Equals(name)) return piece;
            return null;
        }

        public List<string> GetPieceNames()
        {
            List<string> pieceNames = new List<string>();
            foreach (Piece piece in PieceList) pieceNames.Add(piece.Name);
            return pieceNames;
        }

        public List<string> GetMultipleMovesNames()
        {
            List<string> multipleMovesNames = new List<string>();
            foreach (MultipleMove multipleMove in MultipleMoves) multipleMovesNames.Add(multipleMove.Name);
            return multipleMovesNames;
        }

        public Zone FindZone(int index)
        {
            return ZoneList[index];
        }

        // Returns a list with the zone names (omits the repetition of zones with the same name but different players)
        public List<string> ZoneNamesList()
        {
            List<string> zoneNames = new List<string>();

            foreach (Zone zone in ZoneList)
                if (!zoneNames.Contains(zone.Name)) zoneNames.Add(zone.Name);

            return zoneNames;
        }

        public Goal FindGoal(string goalName)
        {
            foreach (var goal in GoalList)
                if (goal.Name.Equals(goalName)) return goal;
            return null;
        }
    }
}
