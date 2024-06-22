using backend.game.entities;

namespace backend.communication.DOTs
{
    internal class FieldDTO(Field field)
    {
        public int Column { get; } = field.Column;
        public int Row { get; } = field.Row;
    }
}