using Mysqlx.Expr;

namespace backend.game
{
    internal class Connect4Line
    {
        public Connect4Line()
        {
            for (int i = 0; i < _line.Length; i++)
            {
                _line[i] = new Field();
            }
        }

        public Field this[int index]
        {
            get
            {
                return _line[index];
            }
        }

        private readonly Field[] _line = new Field[4];
    }
}
