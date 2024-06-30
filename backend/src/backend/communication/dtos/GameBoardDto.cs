using backend.game;

namespace backend.communication.dtos
{
    internal class GameBoardDto(Board board)
    {
        public string[][] Board { get; } = board.FieldAsIds;
    }
}