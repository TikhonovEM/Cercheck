using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cercheck
{
    public class MoveAnalyzer
    {
        public Game Game { get; set; }
        public List<Move> Moves { get; set; }
        public MoveAnalyzer(List<Move> moves, Game game)
        {
            Moves = moves;
            Game = game.FullCopy();
            
        }
        public Move GetBestMove()
        {
            var gamesAfterblackMovesLevel1 = new List<Game>();
            var gamesAfterwhiteMovesLevel1 = new List<Game>();
            var gamesAfterblackMovesLevel2 = new List<Game>();
            var gamesAfterwhiteMovesLevel2 = new List<Game>();
            // Первый ход черных
            foreach(var blackMoveLevel1 in Moves)
            {
                /*var gameAfterBlackMoveLevel1 = Game.FullCopy();
                gameAfterBlackMoveLevel1.MakeMove(blackMoveLevel1.StartPoint, blackMoveLevel1.DestinationPoint, false, false);
                whiteMovesLevel1.AddRange(gameAfterBlackMoveLevel1.GetAllPossibleMoves(true)); */
                var gameAfterBlackMoveLevel1 = Game.FullCopy();
                gameAfterBlackMoveLevel1.MakeMove(blackMoveLevel1.StartPoint, blackMoveLevel1.DestinationPoint, false, false);
                gameAfterBlackMoveLevel1.PreviousGameState = Game;
                gamesAfterblackMovesLevel1.Add(gameAfterBlackMoveLevel1);
            }
            // Первый ход белых
            foreach (var gameAfterblackMovesLevel1 in gamesAfterblackMovesLevel1)
            {
                var allpossiblemoves = gameAfterblackMovesLevel1.GetAllPossibleMoves(true);
                foreach(var move in allpossiblemoves)
                {
                    var gameAfterwhiteMovesLevel1 = gameAfterblackMovesLevel1.FullCopy();
                    gameAfterwhiteMovesLevel1.MakeMove(move.StartPoint, move.DestinationPoint, true, false);
                    gameAfterwhiteMovesLevel1.PreviousGameState = gameAfterblackMovesLevel1;
                    gamesAfterwhiteMovesLevel1.Add(gameAfterwhiteMovesLevel1);
                }
            }
            // Второй ход черных
            foreach (var gameAfterwhiteMovesLevel1 in gamesAfterwhiteMovesLevel1)
            {
                var allpossiblemoves = gameAfterwhiteMovesLevel1.GetAllPossibleMoves(false);
                foreach (var move in allpossiblemoves)
                {
                    var gameAfterblackMovesLevel2 = gameAfterwhiteMovesLevel1.FullCopy();
                    gameAfterblackMovesLevel2.MakeMove(move.StartPoint, move.DestinationPoint, false, false);
                    gameAfterblackMovesLevel2.PreviousGameState = gameAfterwhiteMovesLevel1;
                    gamesAfterblackMovesLevel2.Add(gameAfterblackMovesLevel2);
                }
            }
            // Второй ход белых
            foreach (var gameAfterblackMovesLevel2 in gamesAfterblackMovesLevel2)
            {
                var allpossiblemoves = gameAfterblackMovesLevel2.GetAllPossibleMoves(true);
                foreach (var move in allpossiblemoves)
                {
                    var gameAfterwhiteMovesLevel2 = gameAfterblackMovesLevel2.FullCopy();
                    gameAfterwhiteMovesLevel2.MakeMove(move.StartPoint, move.DestinationPoint, true, false);
                    gameAfterwhiteMovesLevel2.PreviousGameState = gameAfterblackMovesLevel2;
                    gamesAfterwhiteMovesLevel2.Add(gameAfterwhiteMovesLevel2);
                }
            }
            // Третий ход черных
            foreach (var gameAfterwhiteMovesLevel2 in gamesAfterwhiteMovesLevel2)
            {
                var allpossiblemoves = gameAfterwhiteMovesLevel2.GetAllPossibleMoves(false);
                foreach (var move in allpossiblemoves)
                {
                    move.Weight = CalculateWeight(move, gameAfterwhiteMovesLevel2);
                }
                gameAfterwhiteMovesLevel2.MaxWeight = allpossiblemoves.Count > 0 ? allpossiblemoves.Max(x => x.Weight) : -10.0;
            }
            foreach (var gameAfterblackMovesLevel2 in gamesAfterblackMovesLevel2)
            {
                gameAfterblackMovesLevel2.MaxWeight = gamesAfterwhiteMovesLevel2.Where(x => x.PreviousGameState == gameAfterblackMovesLevel2).Min(x => x.MaxWeight);
            }
            foreach (var gameAfterwhiteMovesLevel1 in gamesAfterwhiteMovesLevel1)
            {
                gameAfterwhiteMovesLevel1.MaxWeight = gamesAfterblackMovesLevel2.Where(x => x.PreviousGameState == gameAfterwhiteMovesLevel1).Max(x => x.MaxWeight);
            }
            foreach (var gameAfterblackMovesLevel1 in gamesAfterblackMovesLevel1)
            {
                gameAfterblackMovesLevel1.MaxWeight = gamesAfterwhiteMovesLevel1.Where(x => x.PreviousGameState == gameAfterblackMovesLevel1).Min(x => x.MaxWeight);                   
            }
            var bestGameState = gamesAfterblackMovesLevel1.OrderByDescending(x => x.MaxWeight).First();
            return bestGameState.LastMove;
        }

        private double GetAllPossibleHunters(Move move, Game game)
        {
            throw new Exception();
        }

        private double GetAllPossibleVictims(Move move, Game game)
        {
            var weight = 0.0;
            if ((game.BlackCheckers.Any(point => point.X == move.DestinationPoint.X - 1 && point.Y == move.DestinationPoint.Y - 1)) && 
                (move.DestinationPoint.X - 2 >= 0 && move.DestinationPoint.Y - 2 >= 0))
                if(game.Board.Rows[move.DestinationPoint.Y - 2].Cells[move.DestinationPoint.X - 2].Value == null)
                    weight++;
            if ((game.BlackCheckers.Any(point => point.X == move.DestinationPoint.X && point.Y == move.DestinationPoint.Y - 1)) &&
                (move.DestinationPoint.X >= 0 && move.DestinationPoint.Y - 2 >= 0))
                if (game.Board.Rows[move.DestinationPoint.Y - 2].Cells[move.DestinationPoint.X].Value == null)
                    weight++;
            if ((game.BlackCheckers.Any(point => point.X == move.DestinationPoint.X + 1 && point.Y == move.DestinationPoint.Y - 1)) &&
                (move.DestinationPoint.X + 2 <= 7 && move.DestinationPoint.Y - 2 >= 0))
                if (game.Board.Rows[move.DestinationPoint.Y - 2].Cells[move.DestinationPoint.X + 2].Value == null)
                    weight++;
            if ((game.BlackCheckers.Any(point => point.X == move.DestinationPoint.X - 1 && point.Y == move.DestinationPoint.Y)) &&
                (move.DestinationPoint.X - 2 >= 0 && move.DestinationPoint.Y >= 0))
                if (game.Board.Rows[move.DestinationPoint.Y].Cells[move.DestinationPoint.X - 2].Value == null)
                    weight++;
            if ((game.BlackCheckers.Any(point => point.X == move.DestinationPoint.X + 1 && point.Y == move.DestinationPoint.Y)) &&
                (move.DestinationPoint.X + 2 <= 7 && move.DestinationPoint.Y >= 0))
                if (game.Board.Rows[move.DestinationPoint.Y].Cells[move.DestinationPoint.X + 2].Value == null)
                    weight++;
            if ((game.BlackCheckers.Any(point => point.X == move.DestinationPoint.X - 1 && point.Y == move.DestinationPoint.Y + 1)) &&
                (move.DestinationPoint.X - 2 >= 0 && move.DestinationPoint.Y + 2 <= 7))
                if (game.Board.Rows[move.DestinationPoint.Y + 2].Cells[move.DestinationPoint.X - 2].Value == null)
                    weight++;
            if ((game.BlackCheckers.Any(point => point.X == move.DestinationPoint.X && point.Y == move.DestinationPoint.Y + 1)) &&
                (move.DestinationPoint.X >= 0 && move.DestinationPoint.Y + 2 <= 7))
                if (game.Board.Rows[move.DestinationPoint.Y + 2].Cells[move.DestinationPoint.X].Value == null)
                    weight++;
            if ((game.BlackCheckers.Any(point => point.X == move.DestinationPoint.X + 1 && point.Y == move.DestinationPoint.Y + 1)) &&
                (move.DestinationPoint.X + 2 <= 7 && move.DestinationPoint.Y + 2 <= 7))
                if (game.Board.Rows[move.DestinationPoint.Y + 2].Cells[move.DestinationPoint.X + 2].Value == null)
                    weight++;
            return weight * 5;
        }

        private double CalculateWeight(Move move, Game game)
        {
            var weight = 0.0;
            //weight += GetAllPossibleHunters(move, game);
            weight += GetAllPossibleVictims(move, game);
            return weight;
        }
    }
}
