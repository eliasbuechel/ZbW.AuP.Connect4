using backend.game;

namespace backend.communication.DOTs
{
    internal class FieldDTO
    {
        public FieldDTO(Field field)
        {
            Column = field.Column;
            Row = field.Row;
        }

        public int Column { get; }
        public int Row { get; }
    }
}