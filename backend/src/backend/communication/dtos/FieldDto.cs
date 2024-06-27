using backend.game.entities;

namespace backend.communication.dtos
{
    internal class FieldDto(Field field)
    {
        public int Column { get; } = field.Column;
        public int Row { get; } = field.Row;
    }
}