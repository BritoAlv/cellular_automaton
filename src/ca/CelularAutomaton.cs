public struct State(int team, bool occupied, double force)
{
    public int Team = team; // -1, 0, 1, 2, ...
    public bool Ocuppied = occupied; // 0, 1
    public double Force = force; // 0 to 1
}

public class CelularAutomaton
{
    private readonly int gridSize_W;
    private readonly int gridSize_H;
    private readonly int numberTeams;
    private int iteration_count = 0;
    private List<List<State>> CurrentState;
    const double _maxForce = 1;
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
    public CelularAutomaton(int gridSize_w, int gridSize_h, int number_teams, List<List<State>> initial_state)
    {
        gridSize_W = gridSize_w;
        gridSize_H = gridSize_h;
        numberTeams = number_teams;
        CurrentState = [];
        for (int i = 0; i < this.gridSize_W; i++)
        {
            CurrentState.Add([]);
            for (int j = 0; j < this.gridSize_H; j++)
            {
                CurrentState[i].Add(initial_state[i][j]);
            }
        }
    }

    public CelularAutomaton(int gridSize_w, int gridSize_h, int number_teams)
    {
        gridSize_W = gridSize_w;
        gridSize_H = gridSize_h;
        numberTeams = number_teams;
        CurrentState = CelularAutomaton.GenerateRandomStartUp(gridSize_W, gridSize_H, numberTeams);
    }

    public static List<List<State>> GenerateRandomStartUp(int gridSize_w, int gridSize_h, int numberTeams)
    {
        List<List<State>> randomStartUp = [];
        Random random = new();
        for (int i = 0; i < gridSize_w; i++)
        {
            randomStartUp.Add([]);
            for (int j = 0; j < gridSize_h; j++)
            {
                if (random.NextSingle() < 0.6)
                {
                    randomStartUp[i].Add(new State(random.Next(0, numberTeams), random.Next(0, 2) == 1, random.NextDouble()));
                }
                else
                {
                    randomStartUp[i].Add(new State(-1, false, 0));
                }
            }
        }
        return randomStartUp;
    }

    public void UpdateState()
    {
        this.iteration_count++;

        List<List<State>> new_state = [];
        for (int i = 0; i < this.gridSize_W; i++)
        {
            new_state.Add([]);
            for (int j = 0; j < this.gridSize_H; j++)
            {
                var neighboursStates = GetNeighboursStates(i, j);
                var current_state = this.CurrentState[i][j];
                var next_state = LocalUpdateState(current_state, neighboursStates);
                new_state[i].Add(next_state);
            }
        }
        CurrentState = new_state;
    }

    private State LocalUpdateState(State current_state, List<State> neighboursStates)
    {
        var random = new Random();
        /*
        If obstacle keep being obstacle.
        */
        if (current_state.Team == -1)
        {
            return current_state;
        }
        List<Tuple<double, int>> teams_forces = [];
        for (int i = 0; i < numberTeams; i++)
        {
            teams_forces.Add(new Tuple<double, int>(0, 0));
        }
        foreach (State neighbour in neighboursStates)
        {
            if (neighbour.Team != -1 && neighbour.Ocuppied)
            {
                double force = teams_forces[neighbour.Team].Item1;
                int ocurr = teams_forces[neighbour.Team].Item2;
                teams_forces[neighbour.Team] = new Tuple<double, int>(force + neighbour.Force, ocurr + 1);
            }
        }

        if (current_state.Ocuppied)
        {
            double force = teams_forces[current_state.Team].Item1;
            int ocurr = teams_forces[current_state.Team].Item2;
            teams_forces[current_state.Team] = new Tuple<double, int>(force + current_state.Force, ocurr + 1);
        }

        double get_average(int index)
        {
            if (teams_forces[index].Item2 == 0)
            {
                return 0;
            }
            return teams_forces[index].Item1 / (double)teams_forces[index].Item2;
        }

        double maxForce = Enumerable.Range(0, numberTeams).Select(x => get_average(x)).Max();
        if (maxForce == 0)
        {
            return current_state;
        }
        List<int> stronger = Enumerable.Range(0, numberTeams).Where(x => Math.Abs(get_average(x) - maxForce) <= 0.2 && teams_forces[x].Item2 > 0).ToList();
        var random_team = stronger[random.Next() % stronger.Count];
        if (current_state.Ocuppied == false)
        {
            /*
            take the color of one of the strongest team, 
            */
            if(stronger.Count == 1)
            {
                return new State(random_team, true, maxForce + (1 - maxForce) /  (1 + random.Next() % 10 ));
            }
            else
            {
                return new State(random_team, true, maxForce / (1 + random.Next() % 10 ));
            }
        }
        else
        {
            /*
            if you are alive and your team is strongest, then keep alive but with a random life because it's a battle.
            */
            if (stronger.Contains(current_state.Team))
            {
                if (stronger.Count > 1)
                {
                    maxForce = random.NextDouble() * (maxForce);
                }
                else
                {
                    if (teams_forces.Where(x => x.Item2 > 0).Count() > 1)
                    {
                        maxForce -= teams_forces.Where(x => x.Item2 > 0 ).Select(x => x.Item1 / (double)x.Item2 ).Skip(1).First();
                    }
                    else
                    {
                        maxForce += (1 - maxForce) * random.NextDouble();
                    }
                }
                if(maxForce == 0)
                {
                    return new State(random_team, false, 0);
                }
                else
                {
                    return new State(random_team, true, maxForce);
                }
            }
            /*
            become dead with 0 force.
            */
            else
            {
                return new State(current_state.Team, false, 0);
            }
        }
    }

    private List<State> GetNeighboursStates(int i, int j)
    {
        List<State> neighbours = [];
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

    public State GetCurrentState(int row, int column)
    {
        /*
        get current state of cells in the board.
        */
        return this.CurrentState[row][column];
    }

    public void Step_By_Step_Show()
    {
        while (true)
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
        Console.WriteLine("Current State of the board:");
        for (int i = 0; i < this.gridSize_W; i++)
        {
            for (int j = 0; j < this.gridSize_H; j++)
            {
                var team = this.CurrentState[i][j].Team;
                var lifeness = this.CurrentState[i][j].Ocuppied;
                var color = this.colors[this.CurrentState[i][j].Team];
                Console.BackgroundColor = color;
                if (team == -1)
                {
                    Console.Write($"    ".PadLeft(4));
                }
                else if (!lifeness)
                {
                    Console.Write(" X ".PadLeft(4));
                }
                else
                {
                    Console.Write($"{team}".PadLeft(4));
                }
                Console.ResetColor();
            }
            Console.WriteLine();
        }
        Dictionary<int, int> colorCount = [];
        for (int i = 0; i < this.numberTeams; i++)
        {
            colorCount.Add(i, 0);
        }
        int totalValidCells = 0;

        // Count the number of cells for each color
        for (int i = 0; i < this.gridSize_W; i++)
        {
            for (int j = 0; j < this.gridSize_H; j++)
            {
                State currentState = this.CurrentState[i][j];
                int team = currentState.Team;
                if (team != -1 && currentState.Ocuppied == true)
                {
                    totalValidCells++;
                    colorCount[team]++;
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