public class ApiMethods
{
    public CelularAutomaton? CelularAutomata { get; set; }
    public void SetUpCa(int w, int h, int nteams, double p)
    {
        CelularAutomata = new(w, h, nteams, p);
    }

    public GetStateApi BuildResponse()
    {
        if (CelularAutomata is null)
        {
            return new GetStateApi([], []);
        }
        List<List<CellStateApi>> result = new();
        Dictionary<int, int> colorCount = [];
        for (int i = 0; i < CelularAutomata.numberTeams; i++)
        {
            colorCount.Add(i, 0);
        }
        int totalValidCells = 0;

        for (int i = 0; i < CelularAutomata.gridSize_W; i++)
        {
            result.Add([]);
            for (int j = 0; j < CelularAutomata.gridSize_H; j++)
            {
                var state = CelularAutomata.GetCurrentState(i, j);
                if (state.Team == -1)
                {
                    result[i].Add(new CellStateApi(0, -1));
                }
                else if (!state.Ocuppied)
                {
                    result[i].Add(new CellStateApi(0, state.Team));
                }
                else
                {
                    result[i].Add(new CellStateApi(state.Force, state.Team));
                }
                int team = state.Team;
                if (team != -1 && state.Ocuppied == true)
                {
                    totalValidCells++;
                    colorCount[team]++;
                }
            }
        }
        List<double> percents = new();
        foreach (KeyValuePair<int, int> pair in colorCount)
        {
            int team = pair.Key;
            int count = pair.Value;
            double percentage = (double)count / totalValidCells * 100;
            percents.Add(percentage);
        }
        return new GetStateApi(result, percents);
    }

    public GetStateApi handleUpdateRequest()
    {
        if (CelularAutomata is null)
        {
            return new GetStateApi([], []);
        }
        CelularAutomata.UpdateState();
        return BuildResponse();
    }

    public GetStateApi HandleStartRequest(PostStartApi postStartApi)
    {
        CelularAutomata = new CelularAutomaton(postStartApi.w, postStartApi.h, postStartApi.numberTeams, postStartApi.p);
        return BuildResponse();
    }
}