using System.Collections.Generic;

namespace Wrapper
{
    public class DialogueSequence
    {
        public List<Dialogue> Nodes { get => nodes; }
        private List<Dialogue> nodes;

        private int currentLineNumber = 0;

        public DialogueSequence(Dialogue node)
        {
            Events.SortSequences += SortByLineNumber;
            nodes = new List<Dialogue> { node };
        }

        ~DialogueSequence()
        {
            Events.SortSequences -= SortByLineNumber;
        }

        public void Add(Dialogue node)
        {
            nodes.Add(node);
        }

        public Dialogue GetFirst()
        {
            currentLineNumber = 0;
            Events.TogglePreviousButton?.Invoke(false); //disable if first line
            return nodes[currentLineNumber];
        }

        public Dialogue GetLine(int step)
        {
            if (SteppingOutOfBounds(step))
                return null;

            UpdateLineNumber(step);
            return nodes[currentLineNumber];
        }

        private void UpdateLineNumber(int step)
        {
            currentLineNumber += step;
            Events.TogglePreviousButton?.Invoke(currentLineNumber != 0); //disable if first line
            Events.SwitchNextButton?.Invoke(currentLineNumber == nodes.Count - 1); // switch to "dismiss" button if last line
        }

        private bool SteppingOutOfBounds(int step)
        {
            bool result = false;

            if (step < 0 && currentLineNumber == 0)
                result = true;
            else if (step > 0 && currentLineNumber == nodes.Count - 1)
                result = true;

            return result;
        }

        private void SortByLineNumber()
        {
            nodes.Sort((x, y) => x.num.CompareTo(y.num));
        }
    }
}
