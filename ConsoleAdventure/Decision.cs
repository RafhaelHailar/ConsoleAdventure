using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;
using SysAction = System.Action;
using System.Data.Common;
using System.Data;

/**
 * DecisionTree, creates the story feature. 
 * - it allows moving through decision node.
 * - it creates a way for us to have different outcomes for each nodes.
 */
public class DecisionTree
{
    // current decision
    private DecisionNode currentDecision;

    // decision progression.
    private Stack<DecisionNode> decisionProgression = new Stack<DecisionNode>();

    // component
    readonly Game game;

    public DecisionTree(Game game)
    {
        // connect other components
        this.game = game;

        // entrance door nodes.
        DecisionNode msc_toed = new DecisionNode("try opening entrance door", (state) =>
        {
            int triedOpeningEntranceDoor = (int)state["triedOpeningEntranceDoor"];
            if (triedOpeningEntranceDoor == 1)
            {
                return true;
            }
            return false;
        }, new InputMapping[]
        {
            new InputMapping(Input.InputState.MONOLOGUES, Game.MonologueKeys.ENTRANCE_DOOR_WAIL)
        });

        DecisionNode msc_dnoed = new DecisionNode("does not try opening entrance door", (state) =>
        {
            int triedOpeningEntranceDoor = (int)state["triedOpeningEntranceDoor"];
            if (triedOpeningEntranceDoor == 0)
            {
                return true;
            }
            return false;
        }, null, () =>
        {
            decisionProgression.Pop(); // remove this current node.
            decisionProgression.Pop(); // remove this current node.
            currentDecision = decisionProgression.Peek(); // replace by the parent node.
            Dictionary<string, object> updatedState = new Dictionary<string, object>()
            {
                { "triedOpeningEntranceDoor", -1 },
                { "currentLocation", Game.Location.GRAND_FROYER }
            };
            game.UpdateState(updatedState);
            game.action.ClearExecutionStack();
            game.input.TriggerChoicesDisplay();
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
        msc_ted.AddBranch("did not try opening the entrance door", msc_dnoed);

        UpdateCurrentDecision(start);
    }

    public string GetCurrentDecision()
    {
        return currentDecision.GetName();
    }

    private void UpdateCurrentDecision(DecisionNode decision)
    {
        currentDecision = decision;
        decisionProgression.Push(decision);
        ExecuteDecisionPlan();
        game.action.ExecutePlan();
    }

    public void ExecuteDecisionPlan()
    {
        currentDecision.Execute(game);
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

        public Func<Dictionary<string, object>, bool> Prerequisite { get; set; }

        // action that is outside of the decision plan.
        public SysAction ExternalAction { get; set; }

        public DecisionNode(string name, Func<Dictionary<string, object>, bool> prerequisite = null, InputMapping[] plan = null, SysAction externalAction = null)
        {
            this.name = name;
            this.Prerequisite = prerequisite ?? (state => false);
            this.plan = plan ?? new InputMapping[] { };
            this.ExternalAction = externalAction ?? DefaultExternalAction;
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

        public void Execute(Game game)
        {
            for (int i = plan.Length - 1; i >= 0; i--) {
                game.action.AddToStack(plan[i]);
            }

            ExternalAction();
        }

        // default external action
        public void DefaultExternalAction() { }
    }
}
