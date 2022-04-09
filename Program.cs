using System;
using System.Text;
using System.Collections.Generic;

namespace WordSearchGenerator
{
    class Program
    {
        // DATA //
        // Constants
        public static readonly int MIN_DISPLAY_SPACE_SEPARATION = 2;
        public static readonly int EMPTY_LINES_BETWEEN_ROWS = 1;
        public static readonly string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static readonly char UNFILLED_SQUARE_CHAR = ' ';


        // MAIN //
        static void Main(string[] args)
        {
            // Gets initial input
            Console.WriteLine("---[Word Search Generator]---");
            Console.WriteLine("Enter the grid size:");

            int.TryParse(Console.ReadLine(), out int gridSize);

            // Generate base grid data
            char[,] grid = new char[gridSize, gridSize];

            for(int x = 0; x < grid.GetLength(0); x++)
            {
                for(int y = 0; y < grid.GetLength(1); y++)
                {
                    grid[x, y] = UNFILLED_SQUARE_CHAR;
                }
            }

            // Loop to get all words and incrementally add them to the grid until user finishes or runs out of space for more words
            string input = "";
            while (!input.Equals("$DONE"))
            {
                // Prints prompt
                Console.WriteLine(GetGridAsString(grid));
                for(int i = 0; i < grid.GetLength(0); i++)
                {
                    Console.Write("-");
                }

                Console.WriteLine();

                Console.WriteLine("Enter a word to add to the grid or \"$DONE\" to finish.");
                Console.WriteLine("Your word can be from 1 to " + Math.Min(grid.GetLength(0), grid.GetLength(1)) + " letters long.");

                // Gets input
                input = Console.ReadLine().Trim().ToUpper();

                if (input.Equals("$DONE"))
                {
                    break;
                }

                // Removes spaces
                int indexToEdit = 0;
                while(indexToEdit < input.Length)
                {
                    if(input[indexToEdit] == ' ')
                    {
                        input = input.Remove(indexToEdit, 1);
                        continue;
                    }

                    indexToEdit++;
                }

                // Attempts adding the word to the grid
                if (!AddWordToGrid(input, grid))
                {
                    Console.WriteLine("This word could not be added! It was either too long or it is impossible to add it.");
                    Console.WriteLine("Press ENTER to continue.");
                    Console.ReadLine();
                }
            }

            // Fills grid with random letters in the empty slots and outputs it.
            FillGrid(grid, ALPHABET);

            // Displays final grid
            Console.WriteLine("Your final generated word search:");
            Console.WriteLine(GetGridAsString(grid));

            // Close application
            Console.WriteLine("Press ENTER to quit.");
            Console.ReadLine();
        }       


        // Utility Functions //
        public static bool AddWordToGrid(string word, char[,] grid)
        {
            // Caches some basic info
            int wordLength = word.Length;
            int gridSize = grid.GetLength(0);

            // Creates a random generator
            Random rng = new Random();

            // Ensures word fits
            if(wordLength > gridSize)
            {
                return false;
            }

            // Gets the offset for the "border" in which the word can be placed
            int borderOffset = gridSize - wordLength + 1;

            // Stores an array of non-attempted positions
            List<(int, int)> remainingPositions = new List<(int, int)>();
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    if (x < borderOffset 
                        || x >= gridSize - borderOffset 
                        || y < borderOffset 
                        || y >= gridSize - borderOffset)
                    {
                        remainingPositions.Add((x, y));
                    }
                }
            }

            // Gets random positions within list until it is able to fully place the word, at which point return true.
            while(remainingPositions.Count > 0)
            {
                // Gets position
                int selectedPos = rng.Next(remainingPositions.Count);
                (int, int) position = remainingPositions[selectedPos];
                remainingPositions.RemoveAt(selectedPos);

                // Caches whether the position is valid
                bool validPos = false;

                // Attempts each possible direction using a nested for loop (-1, 0, 1 for both x and y directions)
                List<(int, int)> directions = new List<(int, int)>();
                for (int xDir = -1; xDir <= 1; xDir++)
                {
                    for (int yDir = -1; yDir <= 1; yDir++)
                    {
                        if (xDir == 0 && yDir == 0)
                        {
                            continue;
                        }
                    
                        directions.Add((xDir,yDir));
                    }
                }

                while(directions.Count > 0) 
                {
                    // Selects a direction
                    int selectedIndex = rng.Next(directions.Count);
                    (int, int) direction = directions[selectedIndex];
                    directions.RemoveAt(selectedIndex);

                    int xDir = direction.Item1;
                    int yDir = direction.Item2;

                    // Tries placing each letter
                    for (int offset = 0; offset < wordLength; offset++)
                    {
                        // If the letter at that index is the same or unfilled, continues
                        int x = position.Item1 + (xDir * offset);
                        int y = position.Item2 + (yDir * offset);

                        if (x < 0 || x >= gridSize || y < 0 || y >= gridSize)
                        {
                            break;
                        }

                        else if (grid[x, y] != word[offset] && grid[x, y] != UNFILLED_SQUARE_CHAR)
                        {
                            break;
                        }

                        // If this is the last letter, sets this position to valid
                        if (offset == wordLength - 1)
                        {
                            validPos = true;
                        }
                    }

                    // If the position is valid, places each letter and returns true
                    if (validPos)
                    {
                        for (int offset = 0; offset < wordLength; offset++)
                        {
                            // If the letter at that index is the same or unfilled, continues
                            int x = position.Item1 + (xDir * offset);
                            int y = position.Item2 + (yDir * offset);

                            grid[x, y] = word[offset];
                        }

                        return true;
                    }
                    
                }
            }

            // Returns false by default if could not place the word
            return false;
        }

        public static void FillGrid(char[,] grid, string allowedChars)
        {
            // Creates random generator
            Random rng = new Random();

            // Iterates through all characters in the grid
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for(int y = 0; y < grid.GetLength(1); y++)
                {
                    // If it is unfilled, fills it with random allowed character.
                    if(grid[x,y].Equals(UNFILLED_SQUARE_CHAR))
                    {
                        grid[x, y] = allowedChars[rng.Next(0, allowedChars.Length)];
                    }

                    // Otherwise, fills it with the value in the unfilled grid
                    else
                    {
                        grid[x, y] = grid[x, y];
                    }
                }
            }
        }

        public static string GetGridAsString(char[,] grid)
        {
            // Makes stringbuilder for more efficient string creation
            StringBuilder gridOutput = new StringBuilder();

            // Iterates through each row and column, adding the characters to the string.
            for(int x = 0; x < grid.GetLength(0); x++)
            {
                for(int y = 0; y < grid.GetLength(0); y++)
                {
                    gridOutput.Append(grid[x, y]);
                    gridOutput.Append(' ');
                }

                // Appends newline to proceed to next row
                gridOutput.Append('\n');
            }

            // Returns the generated string
            return gridOutput.ToString();
        }
    }
}
