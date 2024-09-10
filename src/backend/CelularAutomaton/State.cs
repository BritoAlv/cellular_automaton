public struct State(int team, bool occupied, double force)
{
    public int Team = team; // -1, 0, 1, 2, ...
    public bool Ocuppied = occupied; // 0, 1
    public double Force = force; // 0 to 1
}
