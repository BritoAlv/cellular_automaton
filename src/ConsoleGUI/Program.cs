public class Program
{
    private static void Main(string[] args)
    {
        ConsoleGui console = new(10, 10, 3, 0.4);
        console.Step_By_Step_Show();
    }
}