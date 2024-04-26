using backend.game;

namespace backend.communication.DOTs
{
    internal class Connect4LineDTO : Connect4Line
    {
        public Connect4LineDTO(Connect4Line line)
        {
            Fields = new FieldDTO[line.Length];

            for (int i = 0; i < line.Length; i++)
            {
                Fields[i] = new FieldDTO(line[i]);
            }
        }

        public FieldDTO[] Fields { get; }
    }
}