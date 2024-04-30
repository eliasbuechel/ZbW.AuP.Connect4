using System.Diagnostics;

namespace backend.game
{
    internal class Connect4Line
    {
        public Connect4Line()
        {
            for (int i = 0; i < _line.Length; i++)
            {
                _line[i] = new Field(-1, -1);
            }
        }

        public Field this[int index]
        {
            get
            {
                Debug.Assert(index >= 0 && index < _line.Length);
                return _line[index];
            }
        }
        public int Length => _line.Length;

        private readonly Field[] _line = new Field[4];
    }
}
