using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cercheck
{
    public static class MoveAnalyzer
    {
        public static Move GetBestMove(this List<Move> moves)
        {
            return moves.OrderByDescending(move => move.Weight).First();
        }
    }
}
