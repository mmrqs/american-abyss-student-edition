using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class AIHelper
{
    private TroopsManager troopsManager ;
    private FightingPopUpUI fightingPopUpUI;
    private AIRecapUI recap;
    
    public AIHelper(TroopsManager troopsManager, FightingPopUpUI fightingPopUpUI, AIRecapUI recap)
    {
        this.troopsManager = troopsManager;
        this.fightingPopUpUI = fightingPopUpUI;
        this.recap = recap;
        //List<Battalion> boardCopy = troopManager.Units.Select(x=> x.Clone()).ToList();
    }

    public void SimulatePlayer(List<Battalion> boardCopy, Character character, List<Character> characters)
    {
        AI ai = new AI();
        Action actionToPerform = ai.simulate(character, boardCopy, characters);
        bool won = false;
        if (actionToPerform.fightingZone.Item1 != Area.FOO)
            won = Fight(actionToPerform.fightingZone, character, actionToPerform.transitions);
        recap.BuildUI(actionToPerform, won);
    }

    private bool Fight((Area, Character, Area) figtingDetails, Character player, List<Transition> transitions)
    {
        System.Random rnd = new Random();
        int die = rnd.Next(1, 7);
        
        Character winner;
        if (die <= troopsManager.GetNumberOfUnitsInArea(figtingDetails.Item1, player))
        {
            //win
            winner = player;
            troopsManager.ReplaceBoard(transitions[0].board);
        }
        else
        {
            //loose
            winner = figtingDetails.Item2;
            troopsManager.ReplaceBoard(transitions[1].board);
        }

        fightingPopUpUI.BuildUI(player.Name.GetString(), 
            figtingDetails.Item2.Name.GetString(), die, 
            winner.Name.GetString(), player.NumberOfTroopsDestroyed);
        return winner.Name == player.Name;
    }
}
