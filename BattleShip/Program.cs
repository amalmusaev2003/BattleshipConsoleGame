using BattleShip.models;

namespace BattleShip
{
    public class Program
    {
        // TODO
        // [1] Убрать эксепшн из конструктора Game
        // [2] Все строковые значения которые повторяются в коде вынести в константы
        // [3] Записывать ходы игрока в базу данных SQLite

        public static void Main(string[] args)
        {
            Console.WriteLine("Введите имя первого игрока: ");
            string? player1Name = Console.ReadLine();
            Console.WriteLine("Введите имя второго игрока: ");
            string? player2Name = Console.ReadLine();

            GameField gameField1 = new GameField(player1Name);

            // Какое-нибудь валидное игровое поле:
            // 4 - 0,0;0,1;0,2;0,3
            // 3(1) - 1,6;1,7;1,8
            // 3(2) - 3,2;4,2;5,2
            // 2(1) - 4,4;4,5
            // 2(2) - 7,2;7,3
            // 2(3) - 9,8;9,9
            // 1(1) - 2,0
            // 1(2) - 9,0
            // 1(3) - 7,6
            // 1(4) - 4,8

            gameField1.ArrangePlayerField();
            gameField1.PrintField();

            GameField gameField2 = new GameField(player2Name);
            gameField2.ArrangePlayerField();
            gameField2.PrintField();

            Game battleship = new Game(gameField1, gameField2);
            Game.checkIfArranged(gameField1, gameField2);
            battleship.Play();
            Game.seeStatistics();
        }
    }
}