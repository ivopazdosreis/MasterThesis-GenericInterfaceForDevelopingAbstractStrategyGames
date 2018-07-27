using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;

namespace AbstractGamesCreationKit
{
    class XmlManager
    {
        public GameData LoadGame(string path)
        {
            //open an XMLgameData file and return a game to the interface
            XmlTextReader textReader = new XmlTextReader(path);
            GameData game = new GameData();

            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNodeList zoneNodes = doc.GetElementsByTagName("zone");
            XmlNodeList moveNodes = doc.GetElementsByTagName("move");
            XmlNodeList pieceNodes = doc.GetElementsByTagName("piece");
            XmlNodeList goalNodes = doc.GetElementsByTagName("goal");
            XmlNodeList color1Node = doc.GetElementsByTagName("Color1");
            XmlNodeList color2Node = doc.GetElementsByTagName("Color2");

            while(textReader.Read())
            {
                
                switch(textReader.Name)
                {
                    case "GameTitle": game.GameTitle = (string) textReader.ReadElementContentAs(typeof(string),null); break;
                    case "GamePath": game.GamePath = (string)textReader.ReadElementContentAs(typeof(string), null); break;
                    case "GameDescription": game.GameDescription = (string)textReader.ReadElementContentAs(typeof(string), null); break;
                    case "GameHistory": game.GameHistory = (string)textReader.ReadElementContentAs(typeof(string), null); break;
                    case "GameStrategy": game.GameStrategy = (string)textReader.ReadElementContentAs(typeof(string), null); break;
                    case "P1Name": game.P1Name = (string)textReader.ReadElementContentAs(typeof(string), null); break;
                    case "P2Name": game.P2Name = (string)textReader.ReadElementContentAs(typeof(string), null); break;
                    case "Color1": game.Color1 = LoadColor(color1Node); break;
                    case "Color2": game.Color2 = LoadColor(color2Node); break;
                    case "AutomaticBoard": string flag = (string)textReader.ReadElementContentAs(typeof(string), null);
                        game.AutomaticBoard = flag.Equals("true");
                        break;
                    case "BoardRowNumber": game.BoardRowNumber = (int) textReader.ReadElementContentAs(typeof(int), null); break;
                    case "BoardColumnNumber": game.BoardColumnNumber = (int)textReader.ReadElementContentAs(typeof(int), null); break;
                    case "PassTurn": game.PassTurn = (int)textReader.ReadElementContentAs(typeof(int), null); break;
                    case "BoardImagePath": game.BoardImagePath = (string) textReader.ReadElementContentAs(typeof(string), null); break;
                    case "BoardSymmetry": game.BoardSymmetry = (string)textReader.ReadElementContentAs(typeof(string), null); break;
                    case "zone": game.ZoneList = LoadZones(zoneNodes); break;
                    case "move": game.MovesList = LoadMoves(moveNodes, game); break;
                    case "piece": game.PieceList = LoadPieces(pieceNodes, game); break;
                    case "goal": game.GoalList = LoadGoals(goalNodes, game); break;
                }
            }
            
            return game;
        }

        private static Color LoadColor(XmlNodeList colorNode)
        {
            XmlElement cNode = (XmlElement) colorNode.Item(0);
            byte r = Convert.ToByte(cNode.GetElementsByTagName("r")[0].InnerText);
            byte g = Convert.ToByte(cNode.GetElementsByTagName("g")[0].InnerText);
            byte b = Convert.ToByte(cNode.GetElementsByTagName("b")[0].InnerText);
            byte a = Convert.ToByte(cNode.GetElementsByTagName("a")[0].InnerText);
            return Color.FromArgb(a,r,g,b);
        }

        private static List<GameData.Goal> LoadGoals(XmlNodeList goalNodes, GameData game)
        {
            List<GameData.Goal> goals = new List<GameData.Goal>();
            
            foreach (XmlNode node in goalNodes)
            {
                XmlElement goalNode = (XmlElement)node;
                
                List<string> pieceTypesNames = new List<string>();
                List<GameData.Piece> pieceTypes = new List<GameData.Piece>();
                List<string> zonesNames = new List<string>();
                List<GameData.Zone> zones = new List<GameData.Zone>();
                List<string> directions = new List<string>();

                string goalName = goalNode.GetElementsByTagName("goalName")[0].InnerText;
                string goalType = goalNode.GetElementsByTagName("goalType")[0].InnerText;
                int value = Convert.ToInt32(goalNode.GetElementsByTagName("Value")[0].InnerText);
                int winDrawLoss = Convert.ToInt32(goalNode.GetElementsByTagName("WinDrawLoss")[0].InnerText);
                int playersAffected = Convert.ToInt32(goalNode.GetElementsByTagName("goalPlayersAffected")[0].InnerText);

                XmlNodeList pieceTypesNodes = goalNode.GetElementsByTagName("goalPieceType");
                if (pieceTypesNodes.Count > 0)
                    foreach (XmlNode pieceTypeNode in pieceTypesNodes)
                    {
                        XmlElement pNode = (XmlElement)pieceTypeNode;
                        pieceTypesNames.Add(pNode.InnerText);
                    }

                foreach (var piece in pieceTypesNames)
                    pieceTypes.Add(game.FindPiece(piece));
                

                XmlNodeList zoneNodes = goalNode.GetElementsByTagName("goalZones");
                if (zoneNodes.Count > 0)
                    foreach (XmlNode zoneNode in zoneNodes)
                    {
                        XmlElement zNode = (XmlElement)zoneNode;
                        zonesNames.Add(zNode.InnerText);
                    }

                foreach (var zone in zonesNames)
                    zones.Add(game.FindZone(zonesNames.IndexOf(zone))); //zone order in the XML file must be preserved!

                XmlNodeList directionNodes = goalNode.GetElementsByTagName("goalDirection");
                if (directionNodes.Count > 0)
                    foreach (XmlNode directionNode in directionNodes)
                    {
                        XmlElement dNode = (XmlElement)directionNode;
                        directions.Add(dNode.InnerText);
                    }

                GameData.Goal goal = new GameData.Goal(goalName,goalType,pieceTypes,value,zones,winDrawLoss,playersAffected, directions);
                goals.Add(goal);
            }
            return goals;

        }

        private static List<GameData.Piece> LoadPieces(XmlNodeList pieceNodes, GameData game)
        {
            List<GameData.Piece> pieces = new List<GameData.Piece>();

            foreach (XmlNode node in pieceNodes)
            {
                XmlElement pieceNode = (XmlElement)node;

                string name = pieceNode.GetElementsByTagName("pieceName")[0].InnerText;

                string description = "";
                XmlNode descriptionNode = pieceNode.GetElementsByTagName("pieceDescription")[0]; //bug can be null
                if(descriptionNode!=null) description = descriptionNode.InnerText;

                string help = "";
                XmlNode helpNode = pieceNode.GetElementsByTagName("pieceHelp")[0]; //bug can be null
                if (helpNode != null) help = helpNode.InnerText;

                string imageP1Path = pieceNode.GetElementsByTagName("ImageP1Path")[0].InnerText;
                string imageP2Path = pieceNode.GetElementsByTagName("ImageP2Path")[0].InnerText;
                int offBoardP1 = Convert.ToInt32(pieceNode.GetElementsByTagName("OffBoardP1")[0].InnerText);
                int offBoardP2 = Convert.ToInt32(pieceNode.GetElementsByTagName("OffBoardP2")[0].InnerText);
                List<string> positionsP1 = new List<string>();
                List<string> positionsP2 = new List<string>();
                
                XmlNode positionsP1Node = pieceNode.GetElementsByTagName("positionsP1")[0];
                if (positionsP1Node!=null)
                    foreach (XmlNode positionNode in positionsP1Node)
                    {
                        XmlElement pNode = (XmlElement)positionNode;
                        positionsP1.Add(pNode.InnerText);
                    }

                XmlNode positionsP2Node = pieceNode.GetElementsByTagName("positionsP2")[0];
                if (positionsP2Node != null)
                    foreach (XmlNode positionNode in positionsP2Node)
                    {
                        XmlElement pNode = (XmlElement)positionNode;
                        positionsP2.Add(pNode.InnerText);
                    }

                List<string> pieceMovesNames = new List<string>();
                List<GameData.Move> pieceMoves = new List<GameData.Move>();
                
                XmlNode movesNode = pieceNode.GetElementsByTagName("pieceMoves")[0];
                if(movesNode != null)
                    foreach (XmlNode moveNode in movesNode)
                        pieceMovesNames.Add(moveNode.InnerText);
                
                foreach (var move in pieceMovesNames)
                    pieceMoves.Add(game.FindMove(move));

                List<string> pieceMultiMovesNames = new List<string>();
                List<GameData.MultipleMove> pieceMultiMoves = new List<GameData.MultipleMove>();

                XmlNode multiMovesNode = pieceNode.GetElementsByTagName("pieceMultipleMoves")[0];
                if(multiMovesNode != null)
                    foreach (XmlNode multiMoveNode in multiMovesNode)
                        pieceMultiMovesNames.Add(multiMoveNode.InnerText);
                
                foreach (var multiMove in pieceMultiMovesNames)
                    pieceMultiMoves.Add(game.FindMultipleMove(multiMove));
                

                string promotionZone = "";
                XmlNode promotionZoneNode = pieceNode.GetElementsByTagName("PromotionZone")[0];
                if (promotionZoneNode != null) promotionZone = promotionZoneNode.InnerText;

                List<string> pieceTypes = new List<string>();
                
                
                XmlNode promotionNode = pieceNode.GetElementsByTagName("piecePromotionTypes")[0];
                if (promotionNode != null)
                    foreach (XmlNode piecePromotionTypeNode in promotionNode)
                    {
                        XmlElement pNode = (XmlElement)piecePromotionTypeNode;
                        pieceTypes.Add(pNode.InnerText);
                    }

                GameData.Promotion piecePromotion = new GameData.Promotion(promotionZone, pieceTypes);
                if (string.IsNullOrEmpty(promotionZone))
                    piecePromotion = null;

                GameData.Piece piece = new GameData.Piece(name, help, description, imageP1Path, imageP2Path, pieceMoves, pieceMultiMoves, positionsP1, positionsP2, offBoardP1, offBoardP2);
                if(piecePromotion!= null) piece.PiecePromotion = piecePromotion;
                pieces.Add(piece);
            }

            return pieces;
            //Test me!!
        }

        private static List<GameData.Move> LoadMoves(XmlNodeList moveNodes, GameData game)
        {
            List<GameData.Move> movesList = new List<GameData.Move>();

            foreach (XmlNode node in moveNodes)
            {
                XmlElement moveNode = (XmlElement)node;

                string name = moveNode.GetElementsByTagName("moveName")[0].InnerText;
                int priority = Convert.ToInt32(moveNode.GetElementsByTagName("Priority")[0].InnerText);
                string type = moveNode.GetElementsByTagName("moveType")[0].InnerText;
                int capture = Convert.ToInt32(moveNode.GetElementsByTagName("Capture")[0].InnerText);
                int zoneType = Convert.ToInt32(moveNode.GetElementsByTagName("moveZoneType")[0].InnerText);
                
                List<string> zoneNameList = new List<string>();
                List<GameData.Zone> zoneList = new List<GameData.Zone>();

                XmlNode zonesNode = moveNode.GetElementsByTagName("moveZones")[0];
                if(zonesNode != null)
                    foreach (XmlNode zoneNode in zonesNode)
                    {
                        string zoneName = zoneNode.Value;
                        zoneNameList.Add(zoneName);
                    }

                foreach (var zone in zoneNameList)
                    zoneList.Add(game.FindZone(zoneNameList.IndexOf(zone))); //zone order in the XML file must be preserved!
                
                GameData.Move move = new GameData.Move(name,type,priority,capture,zoneList,zoneType);
                switch(type)
                {
                    case "drop":
                        string dropType = moveNode.GetElementsByTagName("DropType")[0].InnerText;
                        GameData.Drop drop = new GameData.Drop(move.Name, move.Type, move.Priority, move.Capture, move.MoveZones, move.ZoneType, dropType);
                        movesList.Add(drop);
                        break;
                    case "slide":
                        string slideTravelType = moveNode.GetElementsByTagName("TravelType")[0].InnerText;
                        int slideDistance = Convert.ToInt32(moveNode.GetElementsByTagName("Distance")[0].InnerText);
                        List<string> slideDirections = new List<string>();
                        XmlNode slideDirectionsNode = moveNode.GetElementsByTagName("directions")[0];
                        foreach (XmlNode direction in slideDirectionsNode)
                        {
                            XmlElement dNode = (XmlElement) direction;
                            slideDirections.Add(dNode.InnerText);
                        }

                        GameData.Slide slide = new GameData.Slide(move.Name, move.Type, move.Priority, move.Capture, move.MoveZones, move.ZoneType, slideDirections, slideDistance, slideTravelType);
                        movesList.Add(slide);
                        break;
                    case "jump":
                        string jumpTravelType = moveNode.GetElementsByTagName("TravelType")[0].InnerText;
                        int jumpDistance = Convert.ToInt32(moveNode.GetElementsByTagName("Distance")[0].InnerText);
                        List<string> jumpDirections = new List<string>();
                        XmlNode jumpDirectionsNode = moveNode.GetElementsByTagName("directions")[0];
                        foreach (XmlNode direction in jumpDirectionsNode)
                        {
                            XmlElement dNode = (XmlElement)direction;
                            jumpDirections.Add(dNode.InnerText);
                        }
                        int jumpOver = Convert.ToInt32(moveNode.GetElementsByTagName("JumpOver")[0].InnerText);
                        int jumpCapture = Convert.ToInt32(moveNode.GetElementsByTagName("JumpCapture")[0].InnerText);
                        GameData.Jump jump = new GameData.Jump(move.Name, move.Type, move.Priority, move.Capture, move.MoveZones, move.ZoneType, jumpDirections, jumpDistance, jumpTravelType, jumpOver, jumpCapture);
                        movesList.Add(jump);
                        break;
                }
                
            }
            
            return movesList;
        }

        private static List<GameData.Zone> LoadZones(XmlNodeList zoneNodes)
        {
            List<GameData.Zone> zones = new List<GameData.Zone>();

            foreach (XmlNode node in zoneNodes)
            {
                string name = "";
                int playersAffected = 0;
                XmlElement zoneNode = (XmlElement) node;

                XmlNodeList zNodes = zoneNode.GetElementsByTagName("zoneName");
                if (zNodes.Count > 0) name = zoneNode.GetElementsByTagName("zoneName")[0].InnerText;


                zNodes = zoneNode.GetElementsByTagName("zonePlayersAffected");
                if (zNodes.Count > 0) 
                    playersAffected = Convert.ToInt32(zoneNode.GetElementsByTagName("zonePlayersAffected")[0].InnerText);

                List<string> positions = new List<string>();
                
                XmlNode positionsNode = zoneNode.GetElementsByTagName("zonePositions")[0];
                foreach (XmlNode positionNode in positionsNode)
                {
                    XmlElement pNode = (XmlElement) positionNode;
                    positions.Add(pNode.InnerText);
                }

                GameData.Zone zone = new GameData.Zone(name, playersAffected, positions);
                zones.Add(zone);
            }

            return zones;
        }
        
        //create an XML version of the game
        public bool SaveGame(GameData game, string path)
        {
            // Create an XmlWriterSettings object with the correct options. 
            XmlWriterSettings settings = new XmlWriterSettings
                                                {
                                                    Indent = true,
                                                    IndentChars = ("\t"),
                                                    OmitXmlDeclaration = true,
                                                    NewLineChars = Environment.NewLine
                                                };

            // Create the XmlWriter object and write some content.
            XmlWriter writer = XmlWriter.Create(path, settings); //bug cannot open file if file already open

            //return false if a problem occurs creating the new file
            if (writer == null) return false;

            // Opens the document
            writer.WriteStartDocument();

            //Write element 'game'
            writer.WriteStartElement("game");

            // Write game elements
            writer.WriteElementString("GameTitle", game.GameTitle);
            writer.WriteElementString("GamePath", game.GamePath);
            writer.WriteElementString("P1Name", game.P1Name);
            writer.WriteElementString("P2Name", game.P2Name);

            writer.WriteElementString("AutomaticBoard", game.AutomaticBoard ? "true" : "false");
            writer.WriteElementString("BoardRowNumber", game.BoardRowNumber.ToString());
            writer.WriteElementString("BoardColumnNumber", game.BoardColumnNumber.ToString());
            writer.WriteElementString("PassTurn", game.PassTurn.ToString());
            writer.WriteElementString("BoardSymmetry", game.BoardSymmetry);

            //Colors
            byte r = game.Color1.R, g = game.Color1.G, b = game.Color1.B, a = game.Color1.A;

            writer.WriteStartElement("Color1");
            writer.WriteElementString("r", r.ToString());
            writer.WriteElementString("g", g.ToString());
            writer.WriteElementString("b", b.ToString());
            writer.WriteElementString("a", a.ToString());
            writer.WriteEndElement();

            r = game.Color2.R; g = game.Color2.G; b = game.Color2.B; a = game.Color2.A;

            writer.WriteStartElement("Color2");
            writer.WriteElementString("r", r.ToString());
            writer.WriteElementString("g", g.ToString());
            writer.WriteElementString("b", b.ToString());
            writer.WriteElementString("a", a.ToString());
            writer.WriteEndElement();
            
            if (!string.IsNullOrWhiteSpace(game.GameDescription))
                writer.WriteElementString("GameDescription", game.GameDescription);

            if (!string.IsNullOrWhiteSpace(game.GameHistory))
                writer.WriteElementString("GameHistory", game.GameHistory);

            if (!string.IsNullOrWhiteSpace(game.GameStrategy))
                writer.WriteElementString("GameStrategy", game.GameStrategy);

            if (!string.IsNullOrWhiteSpace(game.BoardImagePath))
                writer.WriteElementString("BoardImagePath", game.BoardImagePath);


            //write zone nodes
            foreach (GameData.Zone zone in game.ZoneList)
            {
                writer.WriteStartElement("zone");
                writer.WriteElementString("zoneName", zone.Name);
                writer.WriteElementString("zonePlayersAffected", zone.PlayersAffected.ToString());

                writer.WriteStartElement("zonePositions");
                foreach (string position in zone.Positions)
                    writer.WriteElementString("zonePosition", position);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }

            //write move nodes
            foreach (GameData.Move move in game.MovesList)
            {
                writer.WriteStartElement("move");
                writer.WriteElementString("moveName", move.Name);
                writer.WriteElementString("Priority", move.Priority.ToString());
                writer.WriteElementString("Capture", move.Capture.ToString());
                writer.WriteElementString("moveType", move.Type);
                writer.WriteElementString("moveZoneType", move.ZoneType.ToString());

                if (move.MoveZones != null)
                    if (move.MoveZones.Count > 0)
                    {
                        writer.WriteStartElement("moveZones");
                        foreach (GameData.Zone zone in move.MoveZones)
                            writer.WriteElementString("moveZone", zone.Name);
                        writer.WriteEndElement();
                    }

                switch (move.Type)
                {
                    case "drop":
                        GameData.Drop drop = (GameData.Drop) move;
                        writer.WriteElementString("DropType", drop.DropType);
                        break;
                    case "slide":
                        GameData.Slide slide = (GameData.Slide) move;
                        writer.WriteElementString("TravelType", slide.TravelType);
                        writer.WriteElementString("Distance", slide.Distance.ToString());
                        writer.WriteStartElement("directions");
                        foreach (string direction in slide.Directions)
                            writer.WriteElementString("direction", direction);
                        writer.WriteEndElement();
                        break;
                    case "jump":
                        GameData.Jump jump = (GameData.Jump) move;
                        writer.WriteElementString("TravelType", jump.TravelType);
                        writer.WriteElementString("Distance", jump.Distance.ToString());
                        writer.WriteElementString("JumpOver", jump.JumpOver.ToString());
                        writer.WriteElementString("JumpCapture", jump.JumpCapture.ToString());
                        writer.WriteStartElement("directions");
                        foreach (string direction in jump.Directions)
                            writer.WriteElementString("direction", direction);
                        writer.WriteEndElement();
                        break;
                }
                writer.WriteEndElement();
            }

            //write Piece nodes
            foreach (var piece in game.PieceList)
            {
                writer.WriteStartElement("piece");
                writer.WriteElementString("pieceName", piece.Name);

                if (!string.IsNullOrWhiteSpace(piece.Help))
                    writer.WriteElementString("Help", piece.Help);
                if (!string.IsNullOrWhiteSpace(piece.Description))
                    writer.WriteElementString("Description", piece.Description);
                writer.WriteElementString("ImageP1Path", piece.ImageP1Path);
                writer.WriteElementString("ImageP2Path", piece.ImageP2Path);
                writer.WriteElementString("OffBoardP1", piece.OffBoardP1.ToString());
                writer.WriteElementString("OffBoardP2", piece.OffBoardP2.ToString());

                if(piece.PositionsP1 != null)
                    if (piece.PositionsP1.Count > 0)
                    {
                        writer.WriteStartElement("positionsP1");
                        foreach (var position in piece.PositionsP1)
                            writer.WriteElementString("positionP1", position);
                        writer.WriteEndElement();
                    }

                if (piece.PositionsP1 != null)
                    if (piece.PositionsP2.Count > 0)
                    {
                        writer.WriteStartElement("positionsP2");
                        foreach (var position in piece.PositionsP2)
                            writer.WriteElementString("positionP2", position);
                        writer.WriteEndElement();
                    }

                if(piece.MovesList.Count!=0)
                    if (piece.MovesList.Count > 0)
                    {
                        writer.WriteStartElement("pieceMoves");
                        foreach (var move in piece.MovesList)
                            writer.WriteElementString("pieceMove", move.Name);
                        writer.WriteEndElement();
                    }

                if (piece.MultipleMovesList.Count != 0)
                    if (piece.MultipleMovesList.Count > 0)
                    {
                        writer.WriteStartElement("pieceMultipleMoves");
                        foreach (var multiMove in piece.MultipleMovesList)
                            writer.WriteElementString("pieceMultiMove", multiMove.Name);
                        writer.WriteEndElement();
                    }

                if (piece.PiecePromotion != null)
                {
                    writer.WriteStartElement("piecePromotion");
                    writer.WriteElementString("PromotionZone", piece.PiecePromotion.PromotionZone);
                    writer.WriteStartElement("piecePromotionTypes");
                    foreach (var pieceType in piece.PiecePromotion.PieceTypes)
                        writer.WriteElementString("promotionPieceType", pieceType);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

            //write Goal nodes
            foreach (var goal in game.GoalList)
            {
                writer.WriteStartElement("goal");
                writer.WriteElementString("goalName", goal.Name);
                writer.WriteElementString("goalType", goal.Type);
                writer.WriteElementString("Value", goal.Value.ToString());
                writer.WriteElementString("WinDrawLoss", goal.WinLossDraw.ToString());
                writer.WriteElementString("goalPlayersAffected", goal.PlayersAffected.ToString());

                if (goal.PieceTypesAffected.Count > 0)
                    foreach (var pieceType in goal.PieceTypesAffected)
                        writer.WriteElementString("goalPieceType", pieceType.Name);

                if (goal.GoalZones.Count > 0)
                    foreach (var zone in goal.GoalZones)
                        writer.WriteElementString("goalZone", zone.Name);
                    
                if (goal.Directions.Count > 0)
                    foreach (var direction in goal.Directions)
                        writer.WriteElementString("goalDirection", direction);
                    
                writer.WriteEndElement();
            }

            //write multiple moves??

            //Ends the 'game' element
            writer.WriteEndElement();

            // Ends the document.
            writer.Flush();
            writer.WriteEndDocument();

            // close writer
            writer.Close();

            return true;

            // Write comments
            //writer.WriteComment("This file was generated using Abstract Games Creation Kit");
        }
        
    }
}
