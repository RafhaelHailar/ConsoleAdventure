using System.Collections.Generic;
using System;
using System.Linq;

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

        // entrance door nodes.
        DecisionNode msc_toed = new DecisionNode("try opening entrance door", (state) =>
        {
            bool triedOpeningEntranceDoor = (bool)state["triedOpeningEntranceDoor"];
            return triedOpeningEntranceDoor;
        }, new InputMapping[]
        {
            new InputMapping(Input.InputState.MONOLOGUES, Game.MonologueKeys.ENTRANCE_DOOR_WAIL)
        });

        // start wander stair case nodes.
        DecisionNode msc_ted = new DecisionNode("grand foyer check entrance door", (state) =>
        {
            Game.Location currentLocation = (Game.Location)state["currentLocation"];
            if (currentLocation.Equals(Game.Location.ENTRANCE))
            {
                return true;
            }
            return false;
        }, new InputMapping[]
        {
            new InputMapping(Input.InputState.MONOLOGUES, Game.MonologueKeys.TRY_ENTRANCE_DOOR),
            new InputMapping(Input.InputState.CHOOSING, Game.ChoicesKeys.OPEN_ENTRANCE_DOOR),
        });

        // start path nodes.
        DecisionNode start_msc = new DecisionNode("wander on the main stair case", (state) =>
        {
            Game.Location currentLocation = (Game.Location)state["currentLocation"];
            if (currentLocation.Equals(Game.Location.MAIN_STAIRCASE))
            {
                return true;
            }
            return false;
        }, new InputMapping[]
        {
            new InputMapping(Input.InputState.MONOLOGUES, Game.MonologueKeys.MAIN_STAIRCASE_FIRST_ENTER)
        });

        DecisionNode start_n = new DecisionNode("investigate nursery", (state) =>
        {
            Game.Location currentLocation = (Game.Location)state["currentLocation"];
            if (currentLocation.Equals(Game.Location.NURSERY))
            {
                return true;
            }
            return false;
        }, new InputMapping[] {
               new InputMapping(Input.InputState.MONOLOGUES, Game.MonologueKeys.NURSERY_FIRST_ENTER)
        });

        // start node
        DecisionNode start = new DecisionNode("start", null, new InputMapping[] {
               new InputMapping(Input.InputState.MONOLOGUES, Game.MonologueKeys.INTRO),
               new InputMapping(Input.InputState.MONOLOGUES, Game.MonologueKeys.CRYING)
        });


        // start path branches
        start.AddBranch("start to nursery", start_n);
        start.AddBranch("start to main stair case", start_msc);

        // start wander stair case branches.
        start_msc.AddBranch("main stair case to check entrance door", msc_ted);

        // entrance door branches.
        msc_ted.AddBranch("try opening the entrance door", msc_toed);

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
        game.action.ExecutePlan();
    }

    public void ExecuteDecisionPlan()
    {
        currentDecision.ExecutePlan(game);
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
              break;
           }
        }
    }

    class DecisionNode
    {
        private readonly string name;
        private readonly Dictionary<string, DecisionNode> branches = new Dictionary<string, DecisionNode>();
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
        }

        public string[] GetChoices()
        {
            return branches.Keys.ToArray();
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
            for (int i = plan.Length - 1; i >= 0; i--) {
                game.action.AddToStack(plan[i]);
            }
        }

        public Input.InputState GetCurrentPlanInputState()
        {
            return plan[currentPlanIndex].State;
        }
    }
}
