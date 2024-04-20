//using backend.game;
//using Microsoft.EntityFrameworkCore.Metadata.Conventions;

//namespace backendTests.game
//{
//    [TestFixture]
//    internal class GameTests
//    {
//        [Test]
//        public void PlayMove_BaseOperation_NoExceptionGetsThrown()
//        {
//            // arrange
//            Player player1 = new Player();
//            Player player2 = new Player();
//            Game game = new Game(player1, player2);

//            // act - assert
//            Assert.DoesNotThrow(() => game.PlayMove(player1, 1));
//        }

//        [Test]
//        public void PlayMove_TwiceTheSamePlayer_ReturnFalse()
//        {
//            // arrange
//            Player player1 = new Player();
//            Player player2 = new Player();
//            Game game = new Game(player1, player2);

//            // act - assert
//            game.PlayMove(player1, 1);
//            Assert.Throws<WrongPlayerPlayingMoveException>(() => game.PlayMove(player1, 1));
//        }

//        [TestCase(-1, true)]
//        [TestCase(0, false)]
//        [TestCase(5, false)]
//        [TestCase(6, true)]
//        public void PlayMove_AcceptOnlyValidColumn_ExpectedResult(int col, bool expectException)
//        {
//            // arrange
//            Player player1 = new Player();
//            Player player2 = new Player();
//            Game game = new Game(player1, player2);

//            // act - assert
//            if (expectException)
//                Assert.Throws<MoveNotPossibleException>(() => game.PlayMove(player1, col));
//            else
//                Assert.DoesNotThrow(() => game.PlayMove(player1, col));
//        }

//        [Test]
//        public void PlayMove_NotInGamePlayerMakesMove_ThrowsException()
//        {
//            // arrange
//            Player player1 = new Player();
//            Player player2 = new Player();
//            Player player3 = new Player();
//            Game game = new Game(player1, player2);

//            // act - assert
//            Assert.Throws<WrongPlayerPlayingMoveException>(() => game.PlayMove(player3, 0));
//        }

//        [Test]
//        public void OnMovePlayed_PlayerMakesAMove_EventGetsEmitted()
//        {
//            // arrange
//            Player expectedPlayer = new Player();
//            Player player2 = new Player();
//            Game game = new Game(expectedPlayer, player2);
//            const int expectedRow = 3;

//            bool called = false;
//            Player? resultingPlayer = null;
//            int resultingRow = int.MinValue;

//            // act
//            game.OnMovePlayed += (player, row) =>
//            {
//                called = true;
//                resultingPlayer = player;
//                resultingRow = row;
//            };
//            game.PlayMove(expectedPlayer, expectedRow);

//            // assert
//            Assert.True(called);
//            Assert.That(resultingPlayer, Is.EqualTo(expectedPlayer));
//            Assert.That(resultingRow, Is.EqualTo(expectedRow));
//        }

//        [Test]
//        public void OnMovePlayd_PlayerMakesMoveInFullColumn_ExeptionGetsThrown()
//        {
//            // arrange
//            Player player1 = new Player();
//            Player player2 = new Player();
//            Game game = new Game(player1, player2);
//            const int expectedColumn = 0;

//            for (int i = 0; i < 3; i++)
//            {
//                game.PlayMove(player1, expectedColumn);
//                game.PlayMove(player2, expectedColumn);
//            }
//            game.PlayMove(player1, expectedColumn);

//            // act
//            Assert.Throws<MoveNotPossibleException>(() =>  game.PlayMove(player2, expectedColumn));
//        }

//        [Test]
//        public void On4Connected_PlayerConnects4InRow_EventGetsEmitted()
//        {
//            // arrange
//            Player player1 = new Player();
//            Player player2 = new Player();
//            Game game = new Game(player1, player2);

//            bool onConnect4 = false;
//            Connect4Line? connect4Line = null;

//            // act
//            game.OnConnect4 += (line) =>
//            {
//                onConnect4 = true;
//                connect4Line = line;
//            };

//            for (int i = 0; i < 3; i++)
//            {
//                game.PlayMove(player1, i);
//                game.PlayMove(player2, 5);
//            }

//            Assert.False(onConnect4);
//            game.PlayMove(player1, 3);

//            // assert
//            Assert.True(onConnect4);
//            Assert.That(connect4Line, Is.Not.Null);
//            Assert.That(connect4Line[0].Column, Is.EqualTo(0));
//            Assert.That(connect4Line[0].Row, Is.EqualTo(0));
//            Assert.That(connect4Line[1].Column, Is.EqualTo(1));
//            Assert.That(connect4Line[1].Row, Is.EqualTo(0));
//            Assert.That(connect4Line[2].Column, Is.EqualTo(2));
//            Assert.That(connect4Line[2].Row, Is.EqualTo(0));
//            Assert.That(connect4Line[3].Column, Is.EqualTo(3));
//            Assert.That(connect4Line[3].Row, Is.EqualTo(0));
//        }

//        [Test]
//        public void On4Connected_PlayerConnects4InColumn_EventGetsEmitted()
//        {
//            // arrange
//            Player player1 = new Player();
//            Player player2 = new Player();
//            Game game = new Game(player1, player2);
//            const int colPlayer1 = 0;
//            const int colPlayer2 = 1;

//            bool onConnect4 = false;
//            Connect4Line? connect4Line = null;

//            // act
//            game.OnConnect4 += (line) =>
//            {
//                onConnect4 = true;
//                connect4Line = line;
//            };

//            for (int i = 0; i < 3; i++)
//            {
//                game.PlayMove(player1, colPlayer1);
//                game.PlayMove(player2, colPlayer2);
//            }

//            Assert.False(onConnect4);
//            game.PlayMove(player1, colPlayer1);

//            // assert
//            Assert.True(onConnect4);
//            Assert.That(connect4Line, Is.Not.Null);
//            Assert.That(connect4Line[0].Column, Is.EqualTo(0));
//            Assert.That(connect4Line[0].Row, Is.EqualTo(0));
//            Assert.That(connect4Line[1].Column, Is.EqualTo(0));
//            Assert.That(connect4Line[1].Row, Is.EqualTo(1));
//            Assert.That(connect4Line[2].Column, Is.EqualTo(0));
//            Assert.That(connect4Line[2].Row, Is.EqualTo(2));
//            Assert.That(connect4Line[3].Column, Is.EqualTo(0));
//            Assert.That(connect4Line[3].Row, Is.EqualTo(3));
//        }

//        [Test]
//        public void On4Connected_PlayerConnects4Diagonal_EventGetsEmitted()
//        {
//            // arrange
//            Player player1 = new Player();
//            Player player2 = new Player();
//            Game game = new Game(player1, player2);

//            bool onConnect4 = false;
//            Connect4Line? connect4Line = null;

//            // act
//            game.OnConnect4 += (line) =>
//            {
//                onConnect4 = true;
//                connect4Line = line;
//            };

//            game.PlayMove(player1, 0);
//            game.PlayMove(player2, 1);
//            game.PlayMove(player1, 1);
//            game.PlayMove(player2, 2);
//            game.PlayMove(player1, 2);
//            game.PlayMove(player2, 3);
//            game.PlayMove(player1, 2);
//            game.PlayMove(player2, 3);
//            game.PlayMove(player1, 3);
//            game.PlayMove(player2, 5);

//            Assert.False(onConnect4);
//            game.PlayMove(player1, 3);

//            // assert
//            Assert.True(onConnect4);
//            Assert.That(connect4Line, Is.Not.Null);
//            Assert.That(connect4Line[0].Column, Is.EqualTo(0));
//            Assert.That(connect4Line[0].Row, Is.EqualTo(0));
//            Assert.That(connect4Line[1].Column, Is.EqualTo(1));
//            Assert.That(connect4Line[1].Row, Is.EqualTo(1));
//            Assert.That(connect4Line[2].Column, Is.EqualTo(2));
//            Assert.That(connect4Line[2].Row, Is.EqualTo(2));
//            Assert.That(connect4Line[3].Column, Is.EqualTo(3));
//            Assert.That(connect4Line[3].Row, Is.EqualTo(3));
//        }
//    }
//}
