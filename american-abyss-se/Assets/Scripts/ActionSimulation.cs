using UnityEngine;

public static class ActionSimulation
{
    public static Board MakeMove(Action action, Board board, Character adverser)
    {
        Board test = new Board(board.UsZone);
        
        /*Debug.Log("Entry : " + action.recruitingZone + action.movingZone + action.fightingZone);
        foreach (var z in board.UsZone)
        {
            Debug.Log(z.Key);
            foreach (var var in z.Value.Characters)
            {
                Debug.Log(var.Key + " " + var.Value);
            }
        }*/
        //Debug.Log("SMDGJLQMGJQLDGMQJDGMDL" + action.movingZone.Item3);
        if(action.recruitingZone != Area.FOO)
            test = Recruit(action.recruitingZone, test, action.character);
        if(action.movingZone.Item2 != Area.FOO)
            test = Move(action.movingZone.Item1, action.movingZone.Item2, action.movingZone.Item3, test, action.character);
        if(action.fightingZone.Item1 != Area.FOO)
            test = Fight(action.fightingZone.Item1, action.fightingZone.Item3, adverser, test, action.character);

        return test;
    }

    // recrute une unité sur une zone spécifique
    private static Board Recruit(Area area, Board board, Character character)
    {
        if(!board.UsZone[area].Characters.ContainsKey(character))
            board.UsZone[area].Characters.Add(character, 0);

        board.UsZone[area].Characters[character] += 1;
        return board;
    }
    
    // bouge une ou plusieurs unités
    private static Board Move(Area startingArea, Area endArea, int numberOfUnitsToMove, Board board, Character character)
    {
        //Debug.Log("Number of units to move in func: " + numberOfUnitsToMove);
        //Debug.Log("before : " + board.UsZone[startingArea].Characters[character]);
        board.UsZone[startingArea].Characters[character] -= numberOfUnitsToMove;
        
        //Debug.Log("After" + board.UsZone[startingArea].Characters[character]);
        if (board.UsZone[startingArea].Characters[character] <= 0)
            board.UsZone[startingArea].Characters.Remove(character);

        
        if(!board.UsZone[endArea].Characters.ContainsKey(character))
            board.UsZone[endArea].Characters.Add(character, 0);
            
        board.UsZone[endArea].Characters[character] += numberOfUnitsToMove;
        
        /*Debug.Log("BOARD MOVE: ");
        foreach (var z in board.UsZone)
        {
            Debug.Log(z.Key);
            foreach (var var in z.Value.Characters)
            {
                Debug.Log(var.Key + " " + var.Value);
            }
        }*/
        return board;
    }
    
    // combat une unité
    private static Board Fight(Area fightingZone, Area movingZone, Character looser, Board board, Character character)
    {
        /*Debug.Log("BOARD FIGHT: ");
        foreach (var z in board.UsZone)
        {
            Debug.Log(z.Key);
            foreach (var var in z.Value.Characters)
            {
                Debug.Log(var.Key + " " + var.Value);
            }
        }*/
        for (var i = 0; i < character.NumberOfTroopsDestroyed; i++)
            if (board.UsZone[fightingZone].Characters[looser] > 0)
                board.UsZone[fightingZone].Characters[looser] -= 1;

        int nb = board.UsZone[fightingZone].Characters[looser];
            
        if (nb > 0)
        {
            if (!board.UsZone[movingZone].Characters.ContainsKey(looser))
                board.UsZone[movingZone].Characters.Add(looser, 0);

            board.UsZone[movingZone].Characters[looser] += nb;
        }

        board.UsZone[fightingZone].Characters.Remove(looser);
        return board;
    }
}
