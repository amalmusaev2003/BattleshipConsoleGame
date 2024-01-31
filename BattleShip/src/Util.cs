namespace BattleShip
{
    public static class Util
    {

        public static int[] ParseCoordinate(string inputCoordinate)
        {
            string[] shipPartCoordinate = inputCoordinate.Split(',');

            return new int[] { int.Parse(shipPartCoordinate[0]), int.Parse(shipPartCoordinate[1]) };
        }


        public static int[][] ParseShipCoordinates(string shipCoordinates, int shipSize)
        {
            int[][] intCoordinates = new int[shipSize][];

            string[] shipParts = shipCoordinates.Split(';');

            for (int i = 0; i < shipParts.Length; i++)
                intCoordinates[i] = ParseCoordinate(shipParts[i]);

            return intCoordinates;
        }

        public static bool CheckCoordinate(string inputCoordinate)
        {
            string[] xy = inputCoordinate.Split(',');

            if (xy.Length != 2)
            {
                Console.WriteLine("В каждой из координат должно быть два значения, разделенных запятой");
                return false;
            }

            int x, y;
            if (!int.TryParse(xy[0], out x) || !int.TryParse(xy[1], out y))
            {
                Console.WriteLine("В качестве координат можно вводить только целые числа");
                return false;
            }

            if (x > 9 || x < 0 || y > 9 || y < 0)
            {
                Console.WriteLine("Координата может быть только в диапазоне 0...9");
                return false;
            }

            return true;
        }

        public static bool CheckCoordinates(string userInput, int correctNumberOfCoordinates)
        {
            string[] inputCoordinates = userInput.Split(';');

            if (inputCoordinates.Length != correctNumberOfCoordinates)
            {
                Console.WriteLine($"Недостаточное количество координат. Необходимо {correctNumberOfCoordinates}");
                return false;
            }

            foreach (string coordinate in inputCoordinates)
            {
                if (!CheckCoordinate(coordinate))
                    return false;
            }

            return true;
        }

        public static bool CheckShip(int[][] shipCoordinates, int shipSize)
        {
            if (shipSize == 1)
                return true;

            int[] onlyX = new int[shipSize];
            int[] onlyY = new int[shipSize];

            for (int i = 0; i < shipSize; i++)
            {
                onlyX[i] = shipCoordinates[i][0];
                onlyY[i] = shipCoordinates[i][1];
            }

            if (!AllValuesEqual(onlyX) && !AllValuesEqual(onlyY))
                return false;
            // проверка на одну возрастающую на единицу координату
            return AllValuesAscending(onlyX) || AllValuesAscending(onlyY);
        }

        private static bool AllValuesEqual(int[] array)
        {
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i] != array[0])
                    return false;
            }

            return true;
        }

        private static bool AllValuesAscending(int[] array)
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                if (array[i + 1] - array[i] != 1)
                    return false; // vertical
            }

            return true; // horizontal
        }

        public static bool VerticalOrHorizontal(int[][] shipCoordinates)
        {
            int[] onlyX = new int[shipCoordinates.Length];

            for (int i = 0; i < shipCoordinates.Length; i++)
                onlyX[i] = shipCoordinates[i][0];

            return AllValuesAscending(onlyX);
        }
    }
}