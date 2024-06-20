﻿using backend.communication.mqtt;
using backend.game.entities;

namespace backend.game
{
    internal class GameBoard
    {
        public GameBoard(RoboterAPI roboterAPI)
        {
            _field = new Player?[COLUMNS][];

            for (int i = 0; i < _field.Length; i++)
                _field[i] = new Player?[ROWS];
            _roboterAPI = roboterAPI;

            _roboterAPI.OnStonePlaced += OnStonePlacedOnRoboter;
            _roboterAPI.OnBoardReset += OnRoboterBoardReset;
        }
        ~GameBoard()
        {
            _roboterAPI.OnStonePlaced -= OnStonePlacedOnRoboter;
            _roboterAPI.OnBoardReset -= OnRoboterBoardReset;
        }

        public event Action<Player, Field>? OnStonePlaced;
        public event Action? OnBoardReset;

        public string[][] FieldAsIds
        {
            get
            {
                string[][] fieldAsIds = new string[_field.Length][];

                for (int i = 0; i < _field.Length; i++)
                {
                    fieldAsIds[i] = new string[_field[i].Length];

                    for (int j = 0; j < _field[i].Length; j++)
                    {
                        Player? player = _field[i][j];
                        fieldAsIds[i][j] = player == null ? "" : player.Id;
                    }
                }

                return fieldAsIds;
            }
        }
        public static int Columns => COLUMNS;
        public static int Rows => ROWS;
        public Field? PlacingField { get; private set; }

        public Player?[] this[int index]
        {
            get { return _field[index]; }
        }

        public bool PlaceStone(Player player, int column)
        {
            if (column < 0 || column >= COLUMNS)
                return false;

            Player?[] col = _field[column];
            if (col[col.Length - 1] != null)
                return false;

            for (int i = 0; i < col.Length; i++)
            {
                if (col[i] != null)
                    continue;

                col[i] = player;
                Field field = new(column, i);
                PlacingField = field;
                _roboterAPI.PlaceStone(player, field);
                return true;
            }

            return false;
        }
        public void Reset()
        {
            for (int i = 0; i < _field.Length; i++)
                for (int j = 0 ; j < _field[i].Length; j++)
                    _field[i][j] = null;

            _roboterAPI.ResetConnect4Board();
        }

        private void OnStonePlacedOnRoboter(Player player, Field field)
        {
            OnStonePlaced?.Invoke(player, field);
            PlacingField = null;
        }
        private void OnRoboterBoardReset()
        {
            OnBoardReset?.Invoke();
        }

        private readonly Player?[][] _field;
        private readonly RoboterAPI _roboterAPI;
        private const int ROWS = 6;
        private const int COLUMNS = 7;
    }
}
