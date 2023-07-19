namespace BlackBox
{
    public enum Dir
    {
        None,
        Left,
        Bot,
        Right,
        Top
    };

    public enum Marker
    {
        None,
        Detour,
        Miss,
        Hit,
        Reflect
    };

    public enum CellType
    {
        Node,
        EdgeNode,
        Nav
    };

    public enum GridSize
    {
        Small,
        Medium,
        Large
    };
}
