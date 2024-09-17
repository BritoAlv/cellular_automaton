public class CelularAutomaton
{
    public readonly int gridSize_W;
    public readonly int gridSize_H;
    public readonly int numberTeams;
    private int iteration_count = 0;
    private readonly double p;
    private List<List<State>> CurrentState;
    private readonly Random random = new();
    const double _maxForce = 1;

    public CelularAutomaton(int gridSize_w, int gridSize_h, int number_teams, List<List<State>> initial_state, double obstacle_rate = 0.6)
    {
        gridSize_W = gridSize_w;
        gridSize_H = gridSize_h;
        numberTeams = number_teams;
        CurrentState = [];
        p = obstacle_rate;
        for (int i = 0; i < gridSize_W; i++)
        {
            CurrentState.Add([]);
            for (int j = 0; j < gridSize_H; j++)
            {
                CurrentState[i].Add(initial_state[i][j]);
            }
        }
    }

    public CelularAutomaton(int gridSize_w, int gridSize_h, int number_teams, double obstacle_rate)
    {
        gridSize_W = gridSize_w;
        gridSize_H = gridSize_h;
        numberTeams = number_teams;
        p = obstacle_rate;
        CurrentState = GenerateRandomStartUp();
    }

    public List<List<State>> GenerateRandomStartUp()
    {
        List<List<State>> randomStartUp = [];
        Random random = new();
        for (int i = 0; i < gridSize_W; i++)
        {
            randomStartUp.Add([]);
            for (int j = 0; j < gridSize_H; j++)
            {
                if (random.NextSingle() < p)
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
        iteration_count++;

        List<List<State>> new_state = [];
        for (int i = 0; i < gridSize_W; i++)
        {
            new_state.Add([]);
            for (int j = 0; j < gridSize_H; j++)
            {
                var neighboursStates = GetNeighboursStates(i, j);
                var current_state = CurrentState[i][j];
                var next_state = LocalUpdateState(current_state, neighboursStates);
                new_state[i].Add(next_state);
            }
        }
        CurrentState = new_state;
    }

    private State LocalUpdateState(State current_state, List<State> neighboursStates)
    {
        if (current_state.Team == -1)
        {
            /*
                If obstacle keep being obstacle.
            */
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
            /*
            all neighbours cells are land or unoccopied.
            */
            return current_state;
        }
        /*
        set strongers the teams close to MaxForce by a constant.
        */
        List<int> stronger = Enumerable.Range(0, numberTeams).Where(x => Math.Abs(get_average(x) - maxForce) <= 0.2 && teams_forces[x].Item2 > 0).ToList();
        /*
        the cell will be taken by a random team
        */
        var random_team = stronger[random.Next() % stronger.Count];
        if (!current_state.Ocuppied)
        {
            /*
            take the color of one of the strongest team, 
            */
            if (stronger.Count == 1)
            {
                return new State(random_team, true, maxForce + (1 - maxForce) / (1 + random.Next() % 10));
            }
            else
            {
                return new State(random_team, true, maxForce / (1 + random.Next() % 10));
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
                        maxForce -= teams_forces.Where(x => x.Item2 > 0).Select(x => x.Item1 / (double)x.Item2).Skip(1).First();
                    }
                    else
                    {
                        maxForce += (1 - maxForce) * random.NextDouble();
                    }
                }
                if (maxForce == 0)
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
        int distance = 2;
        for (int x = -distance; x <= distance; x++)
        {
            for (int y = -distance; y <= distance; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }
                int new_x = i + x;
                int new_y = j + y;
                if (new_x >= 0 && new_x < gridSize_W && new_y >= 0 && new_y < gridSize_H)
                {
                    neighbours.Add(CurrentState[new_x][new_y]);
                }
            }
        }
        return neighbours;
    }

    public int GetIterationCount()
    {
        return iteration_count;
    }

    public State GetCurrentState(int row, int column)
    {
        /*
        get current state of cells in the board.
        */
        return CurrentState[row][column];
    }
}
