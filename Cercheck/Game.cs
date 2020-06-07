using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Cercheck
{
    public class Game
    {
        public DataGridView Board { get; set; }

        public List<Point> WhiteCheckers = new List<Point>();

        public List<Point> BlackCheckers = new List<Point>();

        private Bitmap WhiteChecker = new Bitmap(Image.FromFile("white_el.png"), 60, 60);
        private Bitmap BlackChecker = new Bitmap(Image.FromFile("black_el.png"), 60, 60);

        public Move LastMove { get; set; }

        public List<Move> CurrentPossibleMoves { get; set; }

        public double MaxWeight { get; set; }

        public Game PreviousGameState { get; set; }

        private Label WhiteCounter { get; set; }
        private Label BlackCounter { get; set; }

        private bool IsWhiteTurn { get; set; } = true;
        public bool IsGameOver { get; set; } = false;

        public Game FullCopy()
        {
            Game copy = (Game)this.MemberwiseClone();
            //this.WhiteCheckers.CopyTo(copy.WhiteCheckers.ToArray());
            copy.WhiteCheckers = new List<Point>();
            foreach (var point in WhiteCheckers)
                copy.WhiteCheckers.Add(new Point(point.X, point.Y));
            copy.BlackCheckers = new List<Point>();
            foreach (var point in BlackCheckers)
                copy.BlackCheckers.Add(new Point(point.X, point.Y));
            this.CurrentPossibleMoves.CopyTo(copy.CurrentPossibleMoves.ToArray());
            return copy;
        }

        public Game(DataGridView board, Label whiteCounter, Label blackCounter)
        {
            Board = board;
            WhiteCounter = whiteCounter;
            BlackCounter = blackCounter;
        }
        /// <summary>
        /// Перерисовывает игровое поле
        /// </summary>
        public void ArrangeCheckers()
        {
            for(var i = 0; i < 8; i++)
            {
                for(var j = 0; j < 8; j++)
                {
                    Board.Rows[i].Cells[j].Value = null;
                }
            }
            for(var i = 0; i < WhiteCheckers.Count; i++)
            {
                Board.Rows[WhiteCheckers[i].Y].Cells[WhiteCheckers[i].X].Value = WhiteChecker;
            }
            for (var i = 0; i < BlackCheckers.Count; i++)
            {
                Board.Rows[BlackCheckers[i].Y].Cells[BlackCheckers[i].X].Value = BlackChecker;
            }
            WhiteCounter.Text = WhiteCheckers.Count.ToString();
            BlackCounter.Text = BlackCheckers.Count.ToString();
            CheckForTheEnd();
        }
        /// <summary>
        /// Сделать ход
        /// </summary>
        /// <param name="startPoint">Текущая позиция шашки</param>
        /// <param name="destinationPoint">Место назначения</param>
        /// <param name="isWhiteMove">Чей ход</param>
        public void MakeMove(Point startPoint, Point destinationPoint, bool isWhiteMove, bool isNeedRefresh)
        {
            if(IsGameOver)
            {
                MessageBox.Show("Игра окончена!");
                return;
            }
            if (CheckPossibilityOfMove(startPoint, destinationPoint, isWhiteMove))
            {
                if (isWhiteMove)
                {
                    var checker = WhiteCheckers.Where(p => p.X == startPoint.X && p.Y == startPoint.Y).First();
                    WhiteCheckers[WhiteCheckers.IndexOf(checker)] = destinationPoint;
                    var felledCoords = new Point((startPoint.X + destinationPoint.X) / 2, (startPoint.Y + destinationPoint.Y) / 2);
                    var felledChecker = BlackCheckers.Where(p => p.X == felledCoords.X && p.Y == felledCoords.Y).First();
                    BlackCheckers.Remove(felledChecker);
                    IsWhiteTurn = false;
                }
                else
                {
                    var checker = BlackCheckers.Where(p => p.X == startPoint.X && p.Y == startPoint.Y).First();
                    BlackCheckers[BlackCheckers.IndexOf(checker)] = destinationPoint;
                    var felledCoords = new Point((startPoint.X + destinationPoint.X) / 2, (startPoint.Y + destinationPoint.Y) / 2);
                    var felledChecker = WhiteCheckers.Where(p => p.X == felledCoords.X && p.Y == felledCoords.Y).First();
                    WhiteCheckers.Remove(felledChecker);
                    IsWhiteTurn = true;
                }
                LastMove = new Move(startPoint, destinationPoint);
                if(isNeedRefresh)
                    ArrangeCheckers();
            }
        }
        /// <summary>
        /// Проверяет возможность хода указанной команды с указанными координатами
        /// </summary>
        /// <param name="startPount">Координаты шашки, которой делается ход</param>
        /// <param name="destinationPoint">Координаты места, куда двигается шашка</param>
        /// <param name="isWhiteMove">Чей ход</param>
        /// <returns>Возвращает true, если ход возможен</returns>
        private bool CheckPossibilityOfMove(Point startPoint, Point destinationPoint, bool isWhiteMove)
        {
            if (startPoint.X < 0 || startPoint.Y < 0 || destinationPoint.X < 0 || destinationPoint.Y < 0
                || startPoint.X > 7 || startPoint.Y > 7 || destinationPoint.X > 7 || destinationPoint.Y > 7)
                return false;
            if (isWhiteMove)
            {
                if (!WhiteCheckers.Any(point => point.X == startPoint.X && point.Y == startPoint.Y) || 
                    BlackCheckers.Any(point => point.X == startPoint.X && point.Y == startPoint.Y))
                {
                    MessageBox.Show("В указанном месте нет белой шашки");
                    return false;
                }
                if (WhiteCheckers.Any(point => point.X == destinationPoint.X && point.Y == destinationPoint.Y) ||
                    BlackCheckers.Any(point => point.X == destinationPoint.X && point.Y == destinationPoint.Y))
                {
                    MessageBox.Show("В указанном месте уже есть шашка");
                    return false;
                }
                var felledBlackChecker = new Point((startPoint.X + destinationPoint.X) / 2, (startPoint.Y + destinationPoint.Y) / 2);
                if(!BlackCheckers.Any(checker => checker.X == felledBlackChecker.X && checker.Y == felledBlackChecker.Y))
                {
                    MessageBox.Show("Рубка обязательна");
                    return false;
                }
            }
            else
            {
                if (WhiteCheckers.Any(point => point.X == startPoint.X && point.Y == startPoint.Y) ||
                    !BlackCheckers.Any(point => point.X == startPoint.X && point.Y == startPoint.Y))
                {
                    return false;
                }
                if (WhiteCheckers.Any(point => point.X == destinationPoint.X && point.Y == destinationPoint.Y) ||
                    BlackCheckers.Any(point => point.X == destinationPoint.X && point.Y == destinationPoint.Y))
                {
                    return false;
                }
                var felledWhiteChecker = new Point((startPoint.X + destinationPoint.X) / 2, (startPoint.Y + destinationPoint.Y) / 2);
                if (!WhiteCheckers.Any(checker => checker.X == felledWhiteChecker.X && checker.Y == felledWhiteChecker.Y))
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckPossibilityOfMoveNotInteractive(Point startPoint, Point destinationPoint, bool isWhiteMove)
        {
            if (startPoint.X < 0 || startPoint.Y < 0 || destinationPoint.X < 0 || destinationPoint.Y < 0 
                || startPoint.X > 7 || startPoint.Y > 7 || destinationPoint.X > 7 || destinationPoint.Y > 7)
                return false;
            if (isWhiteMove)
            {
                if (!WhiteCheckers.Any(point => point.X == startPoint.X && point.Y == startPoint.Y) ||
                    BlackCheckers.Any(point => point.X == startPoint.X && point.Y == startPoint.Y))
                {
                    return false;
                }
                if (WhiteCheckers.Any(point => point.X == destinationPoint.X && point.Y == destinationPoint.Y) ||
                    BlackCheckers.Any(point => point.X == destinationPoint.X && point.Y == destinationPoint.Y))
                {
                    return false;
                }
                var felledBlackChecker = new Point((startPoint.X + destinationPoint.X) / 2, (startPoint.Y + destinationPoint.Y) / 2);
                if (!BlackCheckers.Any(checker => checker.X == felledBlackChecker.X && checker.Y == felledBlackChecker.Y))
                {
                    return false;
                }
            }
            else
            {
                if (WhiteCheckers.Any(point => point.X == startPoint.X && point.Y == startPoint.Y) ||
                    !BlackCheckers.Any(point => point.X == startPoint.X && point.Y == startPoint.Y))
                {
                    return false;
                }
                if (WhiteCheckers.Any(point => point.X == destinationPoint.X && point.Y == destinationPoint.Y) ||
                    BlackCheckers.Any(point => point.X == destinationPoint.X && point.Y == destinationPoint.Y))
                {
                    return false;
                }
                var felledWhiteChecker = new Point((startPoint.X + destinationPoint.X) / 2, (startPoint.Y + destinationPoint.Y) / 2);
                if (!WhiteCheckers.Any(checker => checker.X == felledWhiteChecker.X && checker.Y == felledWhiteChecker.Y))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Проверяет игру на оконченность
        /// </summary>
        private void CheckForTheEnd()
        {
            if(WhiteCheckers.Count == 0)
            {
                MessageBox.Show("Игра окончена. Победили черные");
                IsGameOver = true;
                return;
            }
            if (BlackCheckers.Count == 0)
            {
                MessageBox.Show("Игра окончена. Победили белые");
                IsGameOver = true;
                return;
            }
            var whitePosMoves = GetAllPossibleMoves(true).Count;
            var blackPosMoves = GetAllPossibleMoves(false).Count;
            if (whitePosMoves == 0 && blackPosMoves == 0)
            {
                if (WhiteCheckers.Count > BlackCheckers.Count)
                {
                    MessageBox.Show("Игра окончена. У обоих сторон не осталось ходов. Победили белые, т.к. у черных меньше фигур");
                    IsGameOver = true;
                    return;
                }
                else if(WhiteCheckers.Count < BlackCheckers.Count)
                {
                    MessageBox.Show("Игра окончена. У обоих сторон не осталось ходов. Победили черные, т.к. у белых меньше фигур");
                    IsGameOver = true;
                    return;
                }
                else
                {
                    MessageBox.Show("Игра окончена. У обоих сторон не осталось ходов. Ничья.");
                    IsGameOver = true;
                    return;
                }
            }
        }

        public void DoAITurn()
        {
            if (!IsWhiteTurn && !IsGameOver)
            {
                Thread.Sleep(1000);
                var possibleMoves = GetAllPossibleMoves(false);
                var moveAnalyzer = new MoveAnalyzer(possibleMoves, this);
                var move = moveAnalyzer.GetBestMove();
                MakeMove(move.StartPoint, move.DestinationPoint, false, true);
            }
        }

        public List<Move> GetAllPossibleMoves(bool isWhiteMove)
        {
            var posmoves = new List<Move>();
            if(isWhiteMove)
            {
                foreach(var checker in WhiteCheckers)
                {
                    for(var i = 0; i < 5; i += 2)
                    {
                        var destinationPoint = new Point(checker.X - 2 + i, checker.Y - 2);
                        if (CheckPossibilityOfMoveNotInteractive(checker, destinationPoint, true))
                            posmoves.Add(new Move(checker, destinationPoint, LastMove));
                    }
                    var destinationPointLeft = new Point(checker.X - 2, checker.Y);
                    if (CheckPossibilityOfMoveNotInteractive(checker, destinationPointLeft, true))
                        posmoves.Add(new Move(checker, destinationPointLeft, LastMove));
                    var destinationPointRight = new Point(checker.X + 2, checker.Y);
                    if (CheckPossibilityOfMoveNotInteractive(checker, destinationPointRight, true))
                        posmoves.Add(new Move(checker, destinationPointRight, LastMove));
                    for (var i = 0; i < 5; i += 2)
                    {
                        var destinationPoint = new Point(checker.X - 2 + i, checker.Y + 2);
                        if (CheckPossibilityOfMoveNotInteractive(checker, destinationPoint, true))
                            posmoves.Add(new Move(checker, destinationPoint, LastMove));
                    }
                }
            }
            else
            {
                foreach (var checker in BlackCheckers)
                {
                    for (var i = 0; i < 5; i += 2)
                    {
                        var destinationPoint = new Point(checker.X - 2 + i, checker.Y - 2);
                        if (CheckPossibilityOfMoveNotInteractive(checker, destinationPoint, false))
                            posmoves.Add(new Move(checker, destinationPoint, LastMove));
                    }
                    var destinationPointLeft = new Point(checker.X - 2, checker.Y);
                    if (CheckPossibilityOfMoveNotInteractive(checker, destinationPointLeft, false))
                        posmoves.Add(new Move(checker, destinationPointLeft, LastMove));
                    var destinationPointRight = new Point(checker.X + 2, checker.Y);
                    if (CheckPossibilityOfMoveNotInteractive(checker, destinationPointRight, false))
                        posmoves.Add(new Move(checker, destinationPointRight, LastMove));
                    for (var i = 0; i < 5; i += 2)
                    {
                        var destinationPoint = new Point(checker.X - 2 + i, checker.Y + 2);
                        if (CheckPossibilityOfMoveNotInteractive(checker, destinationPoint, false))
                            posmoves.Add(new Move(checker, destinationPoint, LastMove));
                    }
                }
            }
            CurrentPossibleMoves = posmoves;
            return posmoves;
        }
    }
}
