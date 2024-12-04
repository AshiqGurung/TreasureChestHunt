using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TreasureHuntDetails
{
    enum Direction
    {
        North,
        East,
        South,
        West
    }
    public class Board
    {
        private int[,] board;
        private int boardSize;
        private int numTreasures;
        private int numPlayers;
        private int[] playerTreasures;
        private bool[] playerMines; 
        public Board(int boardSize, int numPlayers, int numTreasure)
        {
            board = new int[boardSize, boardSize];
            this.numPlayers = numPlayers;
            this.numTreasures = numTreasure;
            this.boardSize = boardSize;
            playerTreasures = new int[numPlayers];
            playerMines = new bool[numPlayers];
        }

        public void Start(string player1, string player2)
        {
            Initialize();
            DisplayBoard();

            bool continueGame = true;
            while (continueGame)
            {
                for (int i = 0; i < numPlayers; i++)
                {
                    int currentPlayer = i + 1;
                    string playerName = currentPlayer == 1 ? player1 : player2;
                    Console.WriteLine($"Player {playerName}'s (p{currentPlayer}) turn: Type 1 for mine or 2 for move");
                    int input = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine($"Player {playerName} type (1) north, (2) East (3) South (4) West. In which direction do you want to move/ place a Mine?");
                    int direction = Convert.ToInt32(Console.ReadLine());
                    if (input == 1)
                    {
                        bool isSuccesfullyPlaced = PlaceMine(currentPlayer, (Direction)(direction - 1));
                        if (isSuccesfullyPlaced)
                        {
                            DisplayBoard();
                        }
                    }
                    else
                    {
                       bool isSuccesfullMove =  MovePlayer(currentPlayer, (Direction)(direction - 1));

                        if (isSuccesfullMove)
                        {
                            DisplayBoard();
                        }
                    }
                    
                    if (playerTreasures[currentPlayer - 1] >= 3)
                    {
                        Console.WriteLine($"Player {playerName} wins!");
                        return;
                    }

                    if (playerMines[currentPlayer - 1])
                    {
                        playerName = currentPlayer == 1 ? player1 : player2;
                        Console.WriteLine($"Player {playerName} step on the mine.");
                        playerName = currentPlayer == 1 ? player2 : player1;
                        Console.WriteLine($"Player {playerName} won the game.");
                        Console.WriteLine();
                        return;
                    }
                }
            }
        }

        private void Initialize()
        {
            Random random = new Random();

            // Place players randomly on the board
            for (int i = 0; i < numPlayers; i++)
            {
                int x = random.Next(boardSize);
                int y = random.Next(boardSize);

                // Check if the cell is already occupied
                while (board[x, y] != 0)
                {
                    x = random.Next(boardSize);
                    y = random.Next(boardSize);
                }

                // Player number starts from 1
                board[x, y] = i + 1;
            }

            // Place treasures randomly on the board
            for (int i = 0; i < numTreasures; i++)
            {
                int x = random.Next(boardSize);
                int y = random.Next(boardSize);

                // Check if the cell is already occupied
                while (board[x, y] != 0)
                {
                    x = random.Next(boardSize);
                    y = random.Next(boardSize);
                }

                // Treasure

                board[x, y] = -1;
            }
        }

        public void DisplayBoard()
        {
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (board[i, j] > 0)
                    {
                        Console.Write($"P{board[i, j]} ");
                    }
                    else if (board[i, j] == 0)
                    {
                        Console.Write("-  ");
                    }
                    else if (board[i, j] == -1)
                    {
                        Console.Write("T ");
                    }
                    else
                    {
                        Console.Write("M ");
                    }
                }

                Console.WriteLine();
            }
        }

        private void CalculateNewPosition(int currentX, int currentY, Direction direction, out int newX, out int newY)
        {
            newX = currentX;
            newY = currentY;

            switch (direction)
            {
                case Direction.North:
                    newX--;
                    break;
                case Direction.East:
                    newY++;
                    break;
                case Direction.South:
                    newX++;
                    break;
                case Direction.West:
                    newY--;
                    break;
            }
        }

        private bool MovePlayer(int player, Direction direction)
        {
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (board[i, j] == player)
                    {
                        int newX, newY;

                        CalculateNewPosition(i, j, direction, out newX, out newY);

                        if (board[newX, newY] == -2)
                        {
                            playerMines[player - 1] = true;
                            return false;
                        }

                        if (!IsValidMove(newX, newY))
                        {
                            Console.WriteLine($"Player {player} cannot move {direction}. Out of bounds or position already occupied.");
                            Console.WriteLine();
                            return true;
                        }

                        if (IsPlayerOccupiedPosition(newX, newY))
                        {
                            Console.WriteLine($"Player {player} cannot move {direction} because other player is in that position");
                            Console.WriteLine();
                            return true;
                        }

                        if (IsContainTreasure(newX, newY))
                        {
                            collectTreasure(player, newX, newY);
                            MovePlayerPosition(player, newX, newY, i, j);
                            return true;
                        }
                        else
                        {
                            MovePlayerPosition(player, newX, newY, i, j);
                            Console.WriteLine($"Player {player} moved {direction}.");
                            Console.WriteLine();
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool PlaceMine(int player, Direction direction)
        {
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (board[i, j] == player)
                    {
                        int newX, newY;

                        CalculateNewPosition(i, j, direction, out newX, out newY);

                        if (!IsValidMove(newX, newY))
                        {
                            Console.WriteLine($"Player {player} cannot place mine on {direction}. Out of bounds or position already occupied.");
                            Console.WriteLine();
                            return false;
                        }

                        if (IsPlayerOccupiedPosition(newX, newY))
                        {
                            Console.WriteLine($"Player {player} cannot place mine on {direction} because other player is in that position");
                            Console.WriteLine();
                            return false;
                        }

                        if (IsContainTreasure(newX, newY))
                        {
                            Console.WriteLine($"Player {player} cannot place mine on {direction} because Treasure is in that Position.");
                            Console.WriteLine();
                            return false;
                        }

                        if (!CheckIfAlreadyMine(newX, newY))
                        {
                            //added mines to the board.
                            board[newX, newY] = -2;
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("There's already a mine in that position!");
                            return false; 
                        }
                    }
                }
            }

            return false;
        }

        private bool CheckIfAlreadyMine(int x, int y)
        {
            var result = board[x, y] == -2;
            return result;
        }

        private void MovePlayerPosition(int player, int newXPosition, int newYPosition, int oldXPosition, int oldYposition)   
        {
            //add the player to there new position.
            board[newXPosition, newYPosition] = player;
            // remove the player from there old Position.
            board[oldXPosition, oldYposition] = 0;
        }

        private bool IsValidMove(int x, int y)
        {
            bool validPosition = x >= 0 && x < boardSize && y >= 0 && y < boardSize;
            return validPosition;
        }

        private bool IsPlayerOccupiedPosition(int x, int y)
        {
            bool playerOccupiedPosition = board[x, y] == 1 || board[x, y] == 2;
            return playerOccupiedPosition;
        }

        private bool IsContainTreasure(int x, int y)
        {
            return board[x, y] == -1;
        }

        private void collectTreasure(int player, int x, int y)
        {
            //added treasure for the selected player.
            playerTreasures[player - 1]++;
            // Remove the treasure from the board
            board[x, y] = 0;
            Console.WriteLine($"Player {player} found a treasure! Total treasures: {playerTreasures[player - 1]}");
            Console.WriteLine();   
        }
    }
}
