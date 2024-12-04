
using TreasureHuntDetails;

Console.WriteLine("Please, Enter the 1st Player name?");
string player1 = Console.ReadLine();
Console.WriteLine("Please, Enter the 2nd Player name?");
string player2 = Console.ReadLine();


if (player1 != null && player2 != null)
{
    Console.WriteLine("Welcome to the Treasure Hunt Game!");
    Console.WriteLine("In this game, your goal is to collect treasures while avoiding mines.");
    Console.WriteLine("Each player can move on the board or place a mine.");
    Console.WriteLine("The first player to collect 3 treasures chest wins.");
    Console.WriteLine("Or The first player to steps on the mine loses the game.");
    Console.WriteLine("Let's begin the game!");
    Console.WriteLine();


    Board board = new Board(5, 2, 5);
    board.Start(player1, player2);
}
