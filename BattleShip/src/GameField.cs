namespace BattleShip
{
    public class GameField
    {
        private string _playerName;

        // корабль: 1
        // ореол корабля: 0
        // пустое пространство: -1
        // атакованная клетка: -2
        private int[,] _playerField;

        // выставляется true тогда, когда до конца отрабатывает метод arrangePlayerField()
        private bool _playerFieldArranged;

        public GameField(string playerName)
        {
            _playerName = playerName;

            _playerField = new int[10, 10];

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    _playerField[i, j] = -1;
        }

        public string GetPlayerName()
        {
            return _playerName;
        }

        public int[,] GetPlayerField()
        {
            return _playerField;
        }

        public bool IsPlayerFieldArranged()
        {
            return _playerFieldArranged;
        }
        public void ArrangePlayerField()
        {
            if (_playerFieldArranged)
            {
                Console.WriteLine($"Поле игрока {_playerName} уже готово");
                return;
            }

            Console.WriteLine($"Начнем расставлять корабли на поле {_playerName}. Другой игрок, не смотри!");

            string? userInput;

            Console.WriteLine("Введи координаты четырёхпалубного корабля (формат: x,y;x,y;x,y;x,y)");
            userInput = Console.ReadLine();
            while (!ArrangeShip(userInput, 4))
                userInput = Console.ReadLine();

            Console.WriteLine("Введи координаты первого трехпалубного корабля (формат: x,y;x,y;x,y)");
            userInput = Console.ReadLine();
            while (!ArrangeShip(userInput, 3))
                userInput = Console.ReadLine();

            Console.WriteLine("Введи координаты второго трехпалубного корабля (формат: x,y;x,y;x,y)");
            userInput = Console.ReadLine();
            while (!ArrangeShip(userInput, 3))
                userInput = Console.ReadLine();

            Console.WriteLine("Введи координаты первого двухпалубного корабля (формат: x,y;x,y)");
            userInput = Console.ReadLine();
            while (!ArrangeShip(userInput, 2))
                userInput = Console.ReadLine();

            Console.WriteLine("Введи координаты второго двухпалубного корабля (формат: x,y;x,y)");
            userInput = Console.ReadLine();
            while (!ArrangeShip(userInput, 2))
                userInput = Console.ReadLine();

            Console.WriteLine("Введи координаты третьего двухпалубного корабля (формат: x,y;x,y)");
            userInput = Console.ReadLine();
            while (!ArrangeShip(userInput, 2))
                userInput = Console.ReadLine();

            Console.WriteLine("Введи координаты первого однопалубного корабля (формат: x,y)");
            userInput = Console.ReadLine();
            while (!ArrangeShip(userInput, 1))
                userInput = Console.ReadLine();

            Console.WriteLine("Введи координаты второго однопалубного корабля (формат: x,y)");
            userInput = Console.ReadLine();
            while (!ArrangeShip(userInput, 1))
                userInput = Console.ReadLine();

            Console.WriteLine("Введи координаты третьего однопалубного корабля (формат: x,y)");
            userInput = Console.ReadLine();
            while (!ArrangeShip(userInput, 1))
                userInput = Console.ReadLine();

            Console.WriteLine("Введи координаты четвертого однопалубного корабля (формат: x,y)");
            userInput = Console.ReadLine();
            while (!ArrangeShip(userInput, 1))
                userInput = Console.ReadLine();

            _playerFieldArranged = true;
        }
        public void PrintField()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (_playerField[i, j] == 1)
                        Console.Write("\uD83D\uDEE5️");
                    else if (_playerField[i, j] == 0)
                        Console.Write("\uD83D\uDFE6");
                    else if (_playerField[i, j] == -2)
                        Console.Write("\uD83D\uDFE5");
                    else
                        Console.Write("⬜");
                }

                Console.WriteLine();
            }
        }

        private bool ArrangeShip(string userInput, int shipSize)
        {
            if (!Util.CheckCoordinates(userInput, shipSize))
                return false;

            int[][] shipCoordinates = Util.ParseShipCoordinates(userInput, shipSize);

            if (!Util.CheckShip(shipCoordinates, shipSize))
            {
                Console.WriteLine("Ваш корабль не валиден. " +
                        "Валидный корабль - это одна или несколько последовательно идущих клеток (по вертикали или горизонтали)");
                return false;
            }

            if (!ArrangementPossible(shipCoordinates, shipSize))
            {
                Console.WriteLine("Корабль должен занимать только свободное пространство на карте. " +
                        "Помимо этого, Вокруг корабля должна быть область шириной в одну клетку, " +
                        "в которой не может быть других кораблей (ореол корабля)");
                return false;
            }

            // all checks done, arrange ship on the map (with 1s and 0s)
            ArrangeShip(shipCoordinates, shipSize);

            return true;
        }
        private void ArrangeShip(int[][] shipCoordinates, int shipSize)
        {
            // arrange ship
            foreach (var shipCoordinate in shipCoordinates)
                _playerField[shipCoordinate[0], shipCoordinate[1]] = 1;

            // arrange aureole
            List<int[]> shipAureole = GetShipAureole(shipCoordinates, shipSize);

            foreach (var shipAureoleCoordinate in shipAureole)
                _playerField[shipAureoleCoordinate[0], shipAureoleCoordinate[1]] = 0;
        }

        private bool ArrangementPossible(int[][] shipCoordinates, int shipSize)
        {
            // check space for the ship itself
            foreach (var shipCoordinate in shipCoordinates)
            {
                if (_playerField[shipCoordinate[0], shipCoordinate[1]] == 1)
                    return false;
            }

            // check space for the ship's aureole
            List<int[]> shipAureole = GetShipAureole(shipCoordinates, shipSize);

            foreach (var shipAureoleCoordinate in shipAureole)
            {
                if (_playerField[shipAureoleCoordinate[0], shipAureoleCoordinate[1]] == 1)
                    return false;
            }

            return true;
        }
        private List<int[]> GetShipAureole(int[][] shipCoordinates, int shipSize)
        {
            List<int[]> shipAureole = new List<int[]>();

            // Determine the orientation of the ship (vertical or horizontal)
            bool vertical = Util.VerticalOrHorizontal(shipCoordinates);

            if (vertical)
            {
                // add right side
                if (shipCoordinates[0][1] + 1 <= 9)
                {
                    foreach (var shipCoordinate in shipCoordinates)
                        shipAureole.Add(new int[] { shipCoordinate[0], shipCoordinate[1] + 1 });

                    // add top right cell
                    if (shipCoordinates[0][0] - 1 >= 0)
                        shipAureole.Add(new int[] { shipCoordinates[0][0] - 1, shipCoordinates[0][1] + 1 });

                    // add bottom right cell
                    if (shipCoordinates[shipSize - 1][0] + 1 <= 9)
                        shipAureole.Add(new int[] { shipCoordinates[shipSize - 1][0] + 1, shipCoordinates[shipSize - 1][1] + 1 });
                }

                // add left side
                if (shipCoordinates[0][1] - 1 >= 0)
                {
                    foreach (var shipCoordinate in shipCoordinates)
                        shipAureole.Add(new int[] { shipCoordinate[0], shipCoordinate[1] - 1 });

                    // add top left cell
                    if (shipCoordinates[0][0] - 1 >= 0)
                        shipAureole.Add(new int[] { shipCoordinates[0][0] - 1, shipCoordinates[0][1] - 1 });

                    // add bottom left cell
                    if (shipCoordinates[shipSize - 1][0] + 1 <= 9)
                        shipAureole.Add(new int[] { shipCoordinates[shipSize - 1][0] + 1, shipCoordinates[shipSize - 1][1] - 1 });
                }

                // add top cell
                if (shipCoordinates[0][0] - 1 >= 0)
                    shipAureole.Add(new int[] { shipCoordinates[0][0] - 1, shipCoordinates[0][1] });

                // add bottom cell
                if (shipCoordinates[shipSize - 1][0] + 1 <= 9)
                    shipAureole.Add(new int[] { shipCoordinates[shipSize - 1][0] + 1, shipCoordinates[shipSize - 1][1] });
            }
            else
            {
                // add top side
                if (shipCoordinates[0][0] - 1 >= 0)
                {
                    foreach (var shipCoordinate in shipCoordinates)
                        shipAureole.Add(new int[] { shipCoordinate[0] - 1, shipCoordinate[1] });

                    // add top right cell
                    if (shipCoordinates[shipSize - 1][1] + 1 <= 9)
                        shipAureole.Add(new int[] { shipCoordinates[shipSize - 1][0] - 1, shipCoordinates[shipSize - 1][1] + 1 });

                    // add top left cell
                    if (shipCoordinates[0][1] - 1 >= 0)
                        shipAureole.Add(new int[] { shipCoordinates[0][0] - 1, shipCoordinates[0][1] - 1 });
                }

                // add bottom side
                if (shipCoordinates[0][0] + 1 <= 9)
                {
                    foreach (var shipCoordinate in shipCoordinates)
                        shipAureole.Add(new int[] { shipCoordinate[0] + 1, shipCoordinate[1] });

                    // add bottom left cell
                    if (shipCoordinates[0][1] - 1 >= 0)
                        shipAureole.Add(new int[] { shipCoordinates[0][0] + 1, shipCoordinates[0][1] - 1 });

                    // add bottom right cell
                    if (shipCoordinates[shipSize - 1][1] + 1 <= 9)
                        shipAureole.Add(new int[] { shipCoordinates[shipSize - 1][0] + 1, shipCoordinates[shipSize - 1][1] + 1 });
                }

                // add left cell
                if (shipCoordinates[0][1] - 1 >= 0)
                    shipAureole.Add(new int[] { shipCoordinates[0][0], shipCoordinates[0][1] - 1 });

                // add right cell
                if (shipCoordinates[shipSize - 1][1] + 1 <= 9)
                    shipAureole.Add(new int[] { shipCoordinates[shipSize - 1][0], shipCoordinates[shipSize - 1][1] + 1 });
            }

            return shipAureole;
        }

    }
}