using AbstractGamesCreationKit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for ZRFCreatorTest and is intended
    ///to contain all ZRFCreatorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ZRFCreatorTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Players
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AbstractGamesCreationKit.exe")]
        public void PlayersTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ZRFCreator_Accessor target = new ZRFCreator_Accessor(param0); // TODO: Initialize to an appropriate value
            target.Players();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for WriteMoves
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AbstractGamesCreationKit.exe")]
        public void WriteMovesTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ZRFCreator_Accessor target = new ZRFCreator_Accessor(param0); // TODO: Initialize to an appropriate value
            GameData.Piece piece = null; // TODO: Initialize to an appropriate value
            target.WriteMoves(piece);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for WriteDrops
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AbstractGamesCreationKit.exe")]
        public void WriteDropsTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ZRFCreator_Accessor target = new ZRFCreator_Accessor(param0); // TODO: Initialize to an appropriate value
            GameData.Piece piece = null; // TODO: Initialize to an appropriate value
            target.WriteDrops(piece);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for TurnOrder
        ///</summary>
        [TestMethod()]
        public void TurnOrderTest()
        {
            object newGame = null; // TODO: Initialize to an appropriate value
            ZRFCreator target = new ZRFCreator(newGame); // TODO: Initialize to an appropriate value
            target.TurnOrder();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for StartRectangle
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AbstractGamesCreationKit.exe")]
        public void StartRectangleTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ZRFCreator_Accessor target = new ZRFCreator_Accessor(param0); // TODO: Initialize to an appropriate value
            target.StartRectangle();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for SaveImages
        ///</summary>
        [TestMethod()]
        public void SaveImagesTest()
        {
            object newGame = null; // TODO: Initialize to an appropriate value
            ZRFCreator target = new ZRFCreator(newGame); // TODO: Initialize to an appropriate value
            target.SaveImages();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Pieces
        ///</summary>
        [TestMethod()]
        public void PiecesTest()
        {
            object newGame = null; // TODO: Initialize to an appropriate value
            ZRFCreator target = new ZRFCreator(newGame); // TODO: Initialize to an appropriate value
            target.Pieces();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for MoveDefinitions
        ///</summary>
        [TestMethod()]
        public void MoveDefinitionsTest()
        {
            object newGame = null; // TODO: Initialize to an appropriate value
            ZRFCreator target = new ZRFCreator(newGame); // TODO: Initialize to an appropriate value
            target.MoveDefinitions();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Goals
        ///</summary>
        [TestMethod()]
        public void GoalsTest()
        {
            object newGame = null; // TODO: Initialize to an appropriate value
            ZRFCreator target = new ZRFCreator(newGame); // TODO: Initialize to an appropriate value
            target.Goals();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for GetStringCode
        ///</summary>
        [TestMethod()]
        public void GetStringCodeTest()
        {
            object newGame = null; // TODO: Initialize to an appropriate value
            ZRFCreator target = new ZRFCreator(newGame); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.GetStringCode();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCode
        ///</summary>
        [TestMethod()]
        public void GetCodeTest()
        {
            object newGame = null; // TODO: Initialize to an appropriate value
            ZRFCreator target = new ZRFCreator(newGame); // TODO: Initialize to an appropriate value
            string[] expected = null; // TODO: Initialize to an appropriate value
            string[] actual;
            actual = target.GetCode();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GenerateZRF
        ///</summary>
        [TestMethod()]
        public void GenerateZRFTest()
        {
            object newGame = null; // TODO: Initialize to an appropriate value
            ZRFCreator target = new ZRFCreator(newGame); // TODO: Initialize to an appropriate value
            target.GenerateZRF();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for GameTitle
        ///</summary>
        [TestMethod()]
        public void GameTitleTest()
        {
            object newGame = null; // TODO: Initialize to an appropriate value
            ZRFCreator target = new ZRFCreator(newGame); // TODO: Initialize to an appropriate value
            target.GameTitle();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for GameOptionalInformation
        ///</summary>
        [TestMethod()]
        public void GameOptionalInformationTest()
        {
            object newGame = null; // TODO: Initialize to an appropriate value
            ZRFCreator target = new ZRFCreator(newGame); // TODO: Initialize to an appropriate value
            target.GameOptionalInformation();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Game
        ///</summary>
        [TestMethod()]
        public void GameTest()
        {
            object newGame = null; // TODO: Initialize to an appropriate value
            ZRFCreator target = new ZRFCreator(newGame); // TODO: Initialize to an appropriate value
            target.Game();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for DiscardIllegalPositions
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AbstractGamesCreationKit.exe")]
        public void DiscardIllegalPositionsTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ZRFCreator_Accessor target = new ZRFCreator_Accessor(param0); // TODO: Initialize to an appropriate value
            string positions = string.Empty; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.DiscardIllegalPositions(positions);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Directions
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AbstractGamesCreationKit.exe")]
        public void DirectionsTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ZRFCreator_Accessor target = new ZRFCreator_Accessor(param0); // TODO: Initialize to an appropriate value
            target.Directions();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Dimensions
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AbstractGamesCreationKit.exe")]
        public void DimensionsTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ZRFCreator_Accessor target = new ZRFCreator_Accessor(param0); // TODO: Initialize to an appropriate value
            target.Dimensions();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for DimensionNames
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AbstractGamesCreationKit.exe")]
        public void DimensionNamesTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ZRFCreator_Accessor target = new ZRFCreator_Accessor(param0); // TODO: Initialize to an appropriate value
            string type = string.Empty; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.DimensionNames(type);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DefineZones
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AbstractGamesCreationKit.exe")]
        public void DefineZonesTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ZRFCreator_Accessor target = new ZRFCreator_Accessor(param0); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.DefineZones();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DefineSwap
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AbstractGamesCreationKit.exe")]
        public void DefineSwapTest()
        {
            GameData.Swap newSwap = null; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = ZRFCreator_Accessor.DefineSwap(newSwap);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DefineSlideUpTo
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AbstractGamesCreationKit.exe")]
        public void DefineSlideUpToTest()
        {
            GameData.Slide newSlide = null; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = ZRFCreator_Accessor.DefineSlideUpTo(newSlide);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DefineSlideFurthest
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AbstractGamesCreationKit.exe")]
        public void DefineSlideFurthestTest()
        {
            GameData.Slide newSlide = null; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = ZRFCreator_Accessor.DefineSlideFurthest(newSlide);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DefineSlideExactly
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AbstractGamesCreationKit.exe")]
        public void DefineSlideExactlyTest()
        {
            GameData.Slide newSlide = null; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = ZRFCreator_Accessor.DefineSlideExactly(newSlide);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DefineSlideAny
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AbstractGamesCreationKit.exe")]
        public void DefineSlideAnyTest()
        {
            GameData.Move newSlide = null; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = ZRFCreator_Accessor.DefineSlideAny(newSlide);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DefineSlide
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AbstractGamesCreationKit.exe")]
        public void DefineSlideTest()
        {
            GameData.Slide newSlide = null; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = ZRFCreator_Accessor.DefineSlide(newSlide);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DefineMove
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AbstractGamesCreationKit.exe")]
        public void DefineMoveTest()
        {
            GameData.Move move = null; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = ZRFCreator_Accessor.DefineMove(move);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DefineJump
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AbstractGamesCreationKit.exe")]
        public void DefineJumpTest()
        {
            GameData.Jump newJump = null; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = ZRFCreator_Accessor.DefineJump(newJump);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DefineGoalTypeStalemate
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AbstractGamesCreationKit.exe")]
        public void DefineGoalTypeStalemateTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ZRFCreator_Accessor target = new ZRFCreator_Accessor(param0); // TODO: Initialize to an appropriate value
            target.DefineGoalTypeStalemate();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for DefineGoalTypePattern
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AbstractGamesCreationKit.exe")]
        public void DefineGoalTypePatternTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ZRFCreator_Accessor target = new ZRFCreator_Accessor(param0); // TODO: Initialize to an appropriate value
            target.DefineGoalTypePattern();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for DefineGoalTypeOccupy
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AbstractGamesCreationKit.exe")]
        public void DefineGoalTypeOccupyTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ZRFCreator_Accessor target = new ZRFCreator_Accessor(param0); // TODO: Initialize to an appropriate value
            target.DefineGoalTypeOccupy();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for DefineGoalTypeMultiCapture
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AbstractGamesCreationKit.exe")]
        public void DefineGoalTypeMultiCaptureTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ZRFCreator_Accessor target = new ZRFCreator_Accessor(param0); // TODO: Initialize to an appropriate value
            target.DefineGoalTypeMultiCapture();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for DefineDrop
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AbstractGamesCreationKit.exe")]
        public void DefineDropTest()
        {
            GameData.Drop newDrop = null; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = ZRFCreator_Accessor.DefineDrop(newDrop);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for BoardSymmetry
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AbstractGamesCreationKit.exe")]
        public void BoardSymmetryTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ZRFCreator_Accessor target = new ZRFCreator_Accessor(param0); // TODO: Initialize to an appropriate value
            target.BoardSymmetry();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for BoardSetup
        ///</summary>
        [TestMethod()]
        public void BoardSetupTest()
        {
            object newGame = null; // TODO: Initialize to an appropriate value
            ZRFCreator target = new ZRFCreator(newGame); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.BoardSetup();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Board
        ///</summary>
        [TestMethod()]
        public void BoardTest()
        {
            object newGame = null; // TODO: Initialize to an appropriate value
            ZRFCreator target = new ZRFCreator(newGame); // TODO: Initialize to an appropriate value
            target.Board();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for AddTabs
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AbstractGamesCreationKit.exe")]
        public void AddTabsTest()
        {
            int tabCounter = 0; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = ZRFCreator_Accessor.AddTabs(tabCounter);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ZRFCreator Constructor
        ///</summary>
        [TestMethod()]
        public void ZRFCreatorConstructorTest()
        {
            object newGame = null; // TODO: Initialize to an appropriate value
            ZRFCreator target = new ZRFCreator(newGame);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
