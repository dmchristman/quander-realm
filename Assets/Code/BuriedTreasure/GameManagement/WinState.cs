
namespace BlackBox
{
    public class WinState
    {
        public bool levelWon = false;
        public int level;
        public int numNodes = 0;
        public int numCorrect = 0;
        public int livesRemaining = 0;
        public bool end = false;

        public WinState(int totalNodes, int correctFlags, bool levelWon, int level, int livesRemaining, bool end)
        {
            numNodes = totalNodes;
            numCorrect = correctFlags;
            this.livesRemaining = livesRemaining;
            this.levelWon = levelWon;
            this.level = level;
            this.end = end;
        }
    }
}
