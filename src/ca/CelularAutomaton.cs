public class CelularAutomaton
{
    private readonly int gridSize_W;
    private readonly int gridSize_H;
    private readonly int number_Teams;
    private readonly int numberTeams;
    private int iteration_count = 0;
    private List<List<int>> CurrentState;
    private readonly Dictionary<int, ConsoleColor> colors = new Dictionary<int, ConsoleColor>
    {
        {-1, ConsoleColor.Gray},
        {0, ConsoleColor.Red},
        {1, ConsoleColor.Green},
        {2, ConsoleColor.Blue},
        {3, ConsoleColor.Yellow},
        {4, ConsoleColor.Magenta},
        {5, ConsoleColor.Cyan},
        {6, ConsoleColor.DarkRed},
        {7, ConsoleColor.DarkGreen},
        {8, ConsoleColor.DarkBlue},
        {9, ConsoleColor.DarkYellow},
        {10, ConsoleColor.DarkMagenta},
        {11, ConsoleColor.DarkCyan},
        {12, ConsoleColor.DarkGray},
        {13, ConsoleColor.White}
    };
    public CelularAutomaton( int gridSize_w, int gridSize_h, int number_teams, List<List<int>> initial_state)
    {
        gridSize_W = gridSize_w;
        gridSize_H = gridSize_h;
        numberTeams = number_teams;
        CurrentState = [];
        for (int i = 0; i < this.gridSize_W; i++)
        {
            CurrentState.Add([]);
            for(int j = 0; j < this.gridSize_H; j++)
            {
                CurrentState[i].Add(initial_state[i][j]);
            }
        }
    }

    public CelularAutomaton(int gridSize_w, int gridSize_h, int number_teams)
    {
        gridSize_W = gridSize_w;
        gridSize_H = gridSize_h;
        number_Teams = number_teams;
        CurrentState = CelularAutomaton.GenerateRandomStartUp(gridSize_W, gridSize_H, number_Teams);
    }

    public static List<List<int>> GenerateRandomStartUp( int gridSize_w, int gridSize_h, int numberTeams)
    {
        List<List<int>> randomStartUp = [];
        Random random = new Random();
        for (int i = 0; i < gridSize_w; i++)
        {
            randomStartUp.Add([]);
            for (int j = 0; j < gridSize_h; j++)
            {
                if (random.NextSingle() < 0.3)
                {
                    randomStartUp[i].Add(random.Next(0, numberTeams));
                }
                else
                {
                    randomStartUp[i].Add(-1);
                }
            }
        }
        return randomStartUp;
    }

    public void UpdateState()
    {
        this.iteration_count++;

        List<List<int>> new_state = [];
        for (int i = 0; i < this.gridSize_W; i++)
        {
            new_state.Add([]);
            for (int j = 0; j < this.gridSize_H; j++)
            {
                var neighboursStates = GetNeighboursStates(i, j);
                int current_state = this.CurrentState[i][j];
                int next_state = LocalUpdateState(current_state, neighboursStates);
                new_state[i].Add(next_state);
            }
        }
        CurrentState = new_state;
    }

    private int LocalUpdateState(int current_state, List<int> neighboursStates)
    {
        if (current_state == -1)
        {
            Dictionary<int, int> teamCount = new Dictionary<int, int>();
            foreach (int neighbourState in neighboursStates)
            {
                if (neighbourState != -1)
                {
                    if (teamCount.ContainsKey(neighbourState))
                    {
                        teamCount[neighbourState]++;
                    }
                    else
                    {
                        teamCount[neighbourState] = 1;
                    }
                }
            }

            int maxCount = 0;
            int mostCommonTeam = -1;
            foreach (KeyValuePair<int, int> pair in teamCount)
            {
                if (pair.Value > maxCount)
                {
                    maxCount = pair.Value;
                    mostCommonTeam = pair.Key;
                }
            }

            return mostCommonTeam;
        }
        else
        {
            int teamCount = 0;
            foreach (int neighbourState in neighboursStates)
            {
                if (neighbourState == current_state)
                {
                    teamCount++;
                }
            }

            if (teamCount >= 3)
            {
                return current_state;
            }
            else
            {
                List<int> availableTeams = new List<int>();
                foreach (int neighbourState in neighboursStates)
                {
                    if (neighbourState != -1 && !availableTeams.Contains(neighbourState))
                    {
                        availableTeams.Add(neighbourState);
                    }
                }

                Random random = new Random();
                if(availableTeams.Count == 0)
                {
                    return -1;
                }
                return availableTeams[random.Next(0, availableTeams.Count)];
            }
        }
    }

    private List<int> GetNeighboursStates(int i, int j)
    {
        List<int> neighbours = [];
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }
                int new_x = i + x;
                int new_y = j + y;
                if (new_x >= 0 && new_x < this.gridSize_W && new_y >= 0 && new_y < this.gridSize_H)
                {
                    neighbours.Add(this.CurrentState[new_x][new_y]);
                }
            }
        }
        return neighbours;
    }

    public int GetIterationCount()
    {
        return this.iteration_count;
    }

    public int GetCurrentState(int row, int column)
    {
        /*
        get current state of cells in the board.
        */
        return this.CurrentState[row][column];
    }

    public void Step_By_Step_Show()
    {
        while(true)
        {
            this.Show();
            this.UpdateState();
            Console.WriteLine("Press Enter to continue or type 'a' to stop the simulation.");
            string input = Console.ReadLine()!;
            if (input == "a")
            {
                break;
            }
        }
    }

    public void Show()
    {
        /*
        method for debugging directly from c# console the whole celular automata.
        */
        Console.WriteLine("Current State of the board:\n");
        for (int i = 0; i < this.gridSize_W; i++)
        {
            for (int j = 0; j < this.gridSize_H; j++)
            {
                var color = this.colors[this.CurrentState[i][j]];
                Console.ForegroundColor = color;
                Console.Write((this.CurrentState[i][j]).ToString().PadLeft(2));
                Console.ResetColor();
                Console.Write(" ");
            }
            Console.WriteLine();
        }
        Dictionary<int, int> colorCount = new Dictionary<int, int>();
        int totalValidCells = 0;

        // Count the number of cells for each color
        for (int i = 0; i < this.gridSize_W; i++)
        {
            for (int j = 0; j < this.gridSize_H; j++)
            {
                int currentState = this.CurrentState[i][j];
                if (currentState != -1)
                {
                    totalValidCells++;
                    if (colorCount.ContainsKey(currentState))
                    {
                        colorCount[currentState]++;
                    }
                    else
                    {
                        colorCount[currentState] = 1;
                    }
                }
            }
        }

        // Print the rectangle and percentage for each color
        foreach (KeyValuePair<int, int> pair in colorCount)
        {
            int team = pair.Key;
            int count = pair.Value;
            double percentage = (double)count / totalValidCells * 100;

            Console.BackgroundColor = this.colors[team];
            Console.Write($" {team} ");
            Console.ResetColor();
            Console.Write(" ");
            Console.ForegroundColor = this.colors[team];
            Console.Write($"{percentage:F1}%");
            Console.ResetColor();
            Console.Write("  ");
        }
        Console.WriteLine("");
        Console.WriteLine("End of Current State of the board.\n");
    } 
}