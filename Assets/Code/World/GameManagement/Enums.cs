
namespace Wrapper
{
    /// Dialogue System ///
    
    public enum Character
    {
        None,
        Molly,
        Tangle,
        Byte,
        Wolfie,
        Wane,
        Fran,
        Ken,
        Franken_Twins,
        Chef
    };

    public enum Expression
    {
        Neutral,
        Happy,
        Disappointed
    };

    /// <summary>
    /// Translates Step.Forward to +1 and Step.Backward to -1 | Added Skip for skipping all dialog
    /// </summary>
    public enum Step
    {
        Forward = 1,
        Backward = -1,
        Skip = 0
    };

    public enum Side
    {
        None,
        Left,
        Right
    };

    /// Save System ///
    
    public enum Game
    {
        BlackBox = 0,
        Circuits = 1,
        Labyrinth = 2,
        QueueBits = 3,
        Qupcakes = 4,
        Rewards = 5,
        None = 6
    };

    public enum LoginStatus
    {
        Success,
        FormatError,
        DatabaseError,
        ConnectionError,
        NonExistentUserError
    };

    /// Rewards ///
    
    public enum CardType
    {
        Character,
        Computer_Part,
        Visual,
        Concept
    };

    public enum DisplayType
    {
        Featured,
        InJournal,
        CardPopup
    };

    /// Audio ///
    public enum AudioType
    {
        Music,
        SFX
    }
}