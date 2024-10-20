using System.Collections.Generic;
using System.Collections;
using System;

/**
 * DecisionTree, creates the story feature. 
 * - it allows moving through decision node.
 * - it creates a way for us to have different outcomes for each nodes.
 */
public class DecisionTree
{
    private DecisionNode currentDecision;
    readonly Game game;

    public DecisionTree(Game game)
    {
        // connect other components
        this.game = game;


        // start path nodes.
        DecisionNode start_cc = new DecisionNode("start center check", (state) =>
        {
            Game.Location currentLocation = (Game.Location) state["currentLocation"];
            if (currentLocation.Equals(Game.Location.MAIN_STAIRCASE))
            {
                return true;
            }
            return false;
        }, new InputMapping[] {
               new InputMapping(Input.InputState.MONOLOGUES, Game.MonologueKeys.INTHECENTER),
               new InputMapping(Input.InputState.CHOOSING, Game.ChoicesKeys.CHANGEPLACE)
        });

        // start node
        DecisionNode start = new DecisionNode("start", null, new InputMapping[] {
               new InputMapping(Input.InputState.MONOLOGUES, Game.MonologueKeys.INTRO),
               new InputMapping(Input.InputState.CHOOSING, Game.ChoicesKeys.CHANGEPLACE)
        });


        // start path branches
        start.AddBranch("go to family history room", start_cc);


        currentDecision = start;
    }

    public string GetCurrentDecision()
    {
        return currentDecision.GetName();
    }

    private void UpdateCurrentDecision(DecisionNode decision)
    {
        currentDecision = decision;
        ExecuteDecisionPlan();
    }

    public void ExecuteDecisionPlan()
    {
        currentDecision.ExecutePlan(game);
        currentDecision.NextPlan();
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
        private readonly string name;
        private readonly Dictionary<string, DecisionNode> branches = new Dictionary<string, DecisionNode>();
        private readonly ArrayList choices = new ArrayList();
        private readonly InputMapping[] plan;
        private int currentPlanIndex = 0;
        public Func<Dictionary<string, object>, bool> Prerequisite { get; set; }

        public DecisionNode(string name, Func<Dictionary<string, object>, bool> prerequisite = null, InputMapping[] plan = null)
        {
            this.name = name;
            this.Prerequisite = prerequisite ?? (state => false);
            this.plan = plan ?? new InputMapping[] { };
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

        public void ExecutePlan(Game game)
        {
            if (plan.Length <= currentPlanIndex) return;

            InputMapping currentPlan =  plan[currentPlanIndex];

            switch (currentPlan.State)
            {
                case Input.InputState.MONOLOGUES:
                    game.input.SetState(Input.InputState.MONOLOGUES);
                    game.display.DisplayText(Game.GetMonologue((Game.MonologueKeys) currentPlan.Key));
                    break;
                case Input.InputState.CHOOSING:
                    game.input.SetState(Input.InputState.CHOOSING);
                    game.SetCurrentChoices((Game.ChoicesKeys) currentPlan.Key);
                    break;
                default:
                    throw new Exception("Given State Is Invalid!");
            }
        }

        public Input.InputState GetCurrentPlanInputState()
        {
            return plan[currentPlanIndex].State;
        }

        public void NextPlan()
        {
            currentPlanIndex++;
        }
    }
}
