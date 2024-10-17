using System.Collections.Generic;
using System.Collections;
using System;

public class DecisionTree
{
    private DecisionNode currentDecision; 

    public DecisionTree()
    {
        DecisionNode testPath1 = new DecisionNode("test path 1");
        DecisionNode testPath2 = new DecisionNode("test path 2");
        DecisionNode testPath3 = new DecisionNode("test path 3", (state) =>
        {
            string location = (string) state["currentLocation"];
            if (location.Equals("study room")) return true;
            return false;
        });
        DecisionNode testPath3Path1 = new DecisionNode("test path 3 path 1", (state) =>
        {
            string location = (string)state["currentLocation"];
            if (location.Equals("center")) return true;
            return false;
        });
        DecisionNode start = new DecisionNode("start");

        // test 3 path branches
        testPath3.AddBranch("go to path 1 from path 3", testPath3Path1); // test path 3 path1

        // start path branches
        start.AddBranch("go to test path 1", testPath1); // test path 1
        start.AddBranch("go to test path 2", testPath2); // test path 2
        start.AddBranch("go to test path 3", testPath3); // test path 3

        currentDecision = start;
    }

    public string GetCurrentDecision()
    {
        return currentDecision.GetName();
    }

    private void UpdateCurrentDecision(DecisionNode decision)
    {
        currentDecision = decision;
    }

    public void Update(Dictionary<string, object> state)
    {
        string[] choices = currentDecision.GetChoices();
        for (int i = 0;i < choices.Length; i++)
        {
            DecisionNode choice = currentDecision.GetChoice(choices[i]);
            bool isMatch = choice.Prerequisite(state);
            if (isMatch)
            {
                UpdateCurrentDecision(choice);
            }
            //Console.WriteLine("choice: {0}: {1} ", choices[i], isMatch);
        }
    }

    class DecisionNode
    {
        private string name;
        private readonly Dictionary<string, DecisionNode> branches = new Dictionary<string, DecisionNode>();
        private readonly ArrayList choices = new ArrayList();
        public Func<Dictionary<string, object>, bool> Prerequisite { get; set; }

        public DecisionNode(string name, Func<Dictionary<string, object>, bool> prerequisite = null)
        {
            this.name = name;
            this.Prerequisite = prerequisite ?? (state => false);
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

        public string GetName()
        {
            return this.name;
        }
    }
}
