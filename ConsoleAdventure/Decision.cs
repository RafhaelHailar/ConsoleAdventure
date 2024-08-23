using System.Collections.Generic;
using System.Collections;

public class Decision
{
    private DecisionNode currentDecision; 

    public Decision()
    {
        DecisionNode testPath = new DecisionNode("test path");
        DecisionNode start = new DecisionNode("start");

        start.AddBranch("go to test path", testPath);

        currentDecision = start;
    }

    class DecisionNode
    {
        private string name;
        private readonly Dictionary<string, DecisionNode> branches = new Dictionary<string, DecisionNode>();
        private readonly ArrayList choices = new ArrayList();

        public DecisionNode(string name)
        {
            this.name = name;
        }

        public void AddBranch(string name,DecisionNode node)
        {
            branches.Add(name, node);
            choices.Add(name);
        }

        public string[] GetChoices()
        {
            return (string[]) choices.ToArray(typeof(string));
        }

        public DecisionNode GetChoice(string name)
        {
            return branches[name];
        }
    }
}
