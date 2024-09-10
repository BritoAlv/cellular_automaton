using System.Text;
using System.Text.Json;

public class ConsoleGui
{
    readonly int gridSize_H;
    readonly int gridSize_W;
    readonly int numberTeams;
    private GetStateApi board;
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

    public ConsoleGui(int gridSize_H, int gridSize_W, int numberTeams, double p)
    {
        this.gridSize_H = gridSize_H;
        this.gridSize_W = gridSize_W;
        this.numberTeams = numberTeams;
        board = SendStartQuery(gridSize_H, gridSize_W, numberTeams, p);
    }

    private static GetStateApi SendStartQuery(int gridSize_H, int gridSize_W, int numberTeams, double p)
    {
        PostStartApi to_send = new PostStartApi(gridSize_H, gridSize_W, numberTeams, p);

        using HttpClient client = new HttpClient();
        string json = JsonSerializer.Serialize(to_send);
        StringContent content = new(json, Encoding.UTF8, "application/json");
        HttpResponseMessage response = client.PostAsync("http://localhost:5251/Start", content).Result;
        response.EnsureSuccessStatusCode();
        string responseString = response.Content.ReadAsStringAsync().Result;
        GetStateApi currentState = JsonSerializer.Deserialize<GetStateApi>(responseString)!;
        return currentState;
    }

    public void Step_By_Step_Show()
    {
        while (true)
        {
            Show(board);
            board = SendUpdateState();
            Console.WriteLine("Press Enter to continue or type 'a' to stop the simulation.");
            string input = Console.ReadLine()!;
            if (input == "a")
            {
                break;
            }
        }
    }

    private GetStateApi SendUpdateState()
    {
        using HttpClient client = new HttpClient();
        HttpResponseMessage response = client.GetAsync("http://localhost:5251/UpdateCurrentState").Result;
        response.EnsureSuccessStatusCode();
        string responseString = response.Content.ReadAsStringAsync().Result;
        GetStateApi currentState = JsonSerializer.Deserialize<GetStateApi>(responseString)!;
        return currentState;
    }

    private void Show(GetStateApi current)
    {
        Console.WriteLine("Current State of the board:");
        for (int i = 0; i < gridSize_W; i++)
        {
            for (int j = 0; j < gridSize_H; j++)
            {
                Console.BackgroundColor = colors[current.Cells[i][j].Id];
                double to_print = current.Cells[i][j].Value;
                if (current.Cells[i][j].Id == -1)
                {
                    Console.Write("".PadLeft(4));
                }
                else if (current.Cells[i][j].Value == 0)
                {
                    Console.Write("X".PadLeft(4));
                }
                else
                {
                    string teamString = to_print.ToString("0.00").PadLeft(4); 
                    Console.Write(teamString);
                } 
                Console.ResetColor();
            }
            Console.WriteLine();
        }
        for(int i = 0; i < numberTeams; i++)
        {
            Console.BackgroundColor = colors[i];
            Console.Write($" {i+1}: ");
            Console.ResetColor();
            Console.Write(" ");
            Console.ForegroundColor = colors[i];
            Console.Write(Math.Round(current.Percents[i], 2));
            Console.ResetColor();
            Console.Write("  ");    
        }
        Console.WriteLine("");
        Console.WriteLine("End of Current State of the board.\n");
    }
}