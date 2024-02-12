using System;
using BattleShip.models;

namespace BattleShip
{
    public class Game
    {
        // поле первого игрока
        private GameField _player1Field;
        // поле второго игрока
        private GameField _player2Field;

        // true, пока игра идет. false, когда игра заканчивается
        private bool _gameIsOn;

        // кто сейчас ходит (true - player1, false - player2)
        private bool _isPlayer1;

        private int _player1ShipCount;
        private int _player2ShipCount;

        private int _player1Id;
        private int _player2Id;

        public static void checkIfArranged(GameField player1Field, GameField player2Field) {
            if (!player1Field.IsPlayerFieldArranged() || !player2Field.IsPlayerFieldArranged())
            {
                Console.WriteLine("Создание игры остановлено. Корабли на обоих полях должны быть расставлены.");
            }
        }

        public Game(GameField player1Field, GameField player2Field)
        {
            _player1Field = player1Field;
            _player2Field = player2Field;

            _player1ShipCount = 10;
            _player2ShipCount = 10;

            using (var context = new ApplicationContext())
            {
                var player1 = new PlayerEntity { Name = _player1Field.GetPlayerName() };
                var player2 = new PlayerEntity { Name = _player2Field.GetPlayerName() };

                context.Players.Add(player1);
                context.Players.Add(player2);

                context.SaveChanges();

                _player1Id = player1.Id;
                _player2Id = player2.Id;
            }

            _gameIsOn = true;

            // who makes the first move
            _isPlayer1 = new Random().NextDouble() >= 0.5;
        }

        public void Play()
        {
            while (_gameIsOn)
            {
                Console.WriteLine((_isPlayer1 ? _player1Field.GetPlayerName() : _player2Field.GetPlayerName()) + ", ваш ход!");

                string userInput = Console.ReadLine();

                while (!MakeMove(_isPlayer1, userInput))
                    Console.WriteLine("Ваш ход невалиден. Повторите ход.");
            }
        }

        // Возвращает true, если userInput - валидный ход. Возвращает false, если userInput - невалидный ход
        // Первый аргумент true если это ход первого игрока, false - если это ход второго игрока
        private bool MakeMove(bool isPlayer1, string userInput)
        {
            if (!Util.CheckCoordinate(userInput))
                return false;

            int[] coordinate = Util.ParseCoordinate(userInput);

            if (isPlayer1)
                Hit(_player2Field.GetPlayerField(), coordinate);
            else
                Hit(_player1Field.GetPlayerField(), coordinate);

            if (_player1ShipCount == 0)
            {
                Console.WriteLine(_player2Field.GetPlayerName() + " победил! Игра заканчивается");
                _gameIsOn = false;
            }

            if (_player2ShipCount == 0)
            {
                Console.WriteLine(_player1Field.GetPlayerName() + " победил! Игра заканчивается");
                _gameIsOn = false;
            }
            return true;
        }
        // Производит удар по ячейке
        // Выводит сообщение либо "Мимо!", либо "Попадание", либо "Утопил".
        // В случае потопления декрементирует количество кораблей на плаву
        // Переводит право на ход другому игроку, если удар был "Мимо!"
        private void Hit(int[,] playerField, int[] hitCoordinate)
        {
            if (playerField[hitCoordinate[0], hitCoordinate[1]] == 1)
            {
                playerField[hitCoordinate[0], hitCoordinate[1]] = -2;
                using (var context = new ApplicationContext())
                {
                    context.Moves.Add(new MoveEntity
                    {
                        PlayerId = _isPlayer1 ? _player1Id : _player2Id,
                        Row = hitCoordinate[0],
                        Column = hitCoordinate[1],
                        Hit = true,
                        Time = DateTime.Now
                    });
                    context.SaveChanges();
                }

                if (ShipSank(playerField, hitCoordinate))
                {
                    Console.WriteLine("Утопил!");

                    if (_isPlayer1)
                        _player2ShipCount--;
                    else
                        _player1ShipCount--;
                }
                else
                {
                    Console.WriteLine("Попадание!");
                }
            }
            else
            {
                Console.WriteLine("Мимо!");
                using (var context = new ApplicationContext())
                {
                    context.Moves.Add(new MoveEntity
                    {
                        PlayerId = _isPlayer1 ? _player1Id : _player2Id,
                        Row = hitCoordinate[0],
                        Column = hitCoordinate[1],
                        Hit = false,
                        Time = DateTime.Now
                    });
                    context.SaveChanges();
                }
                _isPlayer1 = !_isPlayer1; // переход хода
            }
        }

        // true - если удар утопил корабль
        // false - если удар не утопил корабль
        private bool ShipSank(int[,] playerField, int[] hitCoordinate)
        {
            // идем вверх - вниз и вправо-влево пока не упремся в ореол.
            // проверяем, есть ли 1
            int x = hitCoordinate[0];
            int y = hitCoordinate[1];

            while (x >= 0 && playerField[x, y] != 0)
            {
                if (playerField[x, y] == 1)
                    return false;
                x -= 1;
            }

            x = hitCoordinate[0];
            while (x < playerField.GetLength(0) && playerField[x, y] != 0)
            {
                if (playerField[x, y] == 1)
                    return false;
                x += 1;
            }
            x = hitCoordinate[0];
            while (y >= 0 && playerField[x, y] != 0)
            {
                if (playerField[x, y] == 1)
                    return false;
                y -= 1;
            }

            y = hitCoordinate[1];
            while (y < playerField.GetLength(1) && playerField[x, y] != 0)
            {
                if (playerField[x, y] == 1)
                    return false;
                y += 1;
            }

            return true;
        }
        public static void seeStatistics() {
            using(var context = new ApplicationContext()){
                var hitCounts = context.Moves
                .Where(m => m.Hit)
                .GroupBy(m => m.PlayerId)
                .Select(g => new { PlayerId = g.Key, HitCount = g.Count() })
                .ToList();

                foreach(var hitCount in hitCounts) {
                    Console.WriteLine($"Игрок {hitCount.PlayerId} сделал {hitCount.HitCount} попаданий.");
                }
            }
        }
    }
}