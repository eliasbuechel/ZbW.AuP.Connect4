using backend.game.entities;

namespace backend.communication.DOTs
{
    internal class FieldDTO : EntityDTO
    {
        public FieldDTO(Field field) : base(field)
        {
            Column = field.Column;
            Row = field.Row;
        }

        public int Column { get; }
        public int Row { get; }
    }
}