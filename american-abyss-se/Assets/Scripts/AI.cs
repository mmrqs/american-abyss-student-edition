using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Action
{
    public Area recruitingZone;
    public (Area, Area, int) movingZone;
    public (Area, Character) fightingZone;
    private List<Battalion> board;
    public List<Battalion> Board => board;
    public Character character;
    public float chanceToOccur;

    public void Recruit(Area area)
    {
        if(!board.Exists(t => t.Character.Name == character.Name && t.Area == area))
            board.Add(new Battalion(character, area));
        board.First(t => t.Character.Name == character.Name && t.Area == area).NumberOfUnits += 1;
        
        recruitingZone = area;
    }

    public void Move(Area startingArea, Area endArea, int numberOfUnitsToMove)
    {

        board.Find(u => u.Character.Name == character.Name && u.Area == startingArea).NumberOfUnits -= numberOfUnitsToMove;

        if(board.Find(u => u.Character.Name == character.Name && u.Area == startingArea).NumberOfUnits == 0) 
            board.Remove(board.Find(b => b.Character.Name == character.Name && b.Area == startingArea));

        if (!board.Exists(b => b.Character.Name == character.Name && b.Area == endArea))
            board.Add(new Battalion(character, endArea));

        board.Find(u => u.Character.Name == character.Name && u.Area == endArea).NumberOfUnits += numberOfUnitsToMove;
        
    }

    public void Fight(bool win, float percentage, Area movingZone)
    {
        Character adverser = fightingZone.Item2;
        Area zone = fightingZone.Item1;
        chanceToOccur = percentage;
        fightingZone = (zone, adverser);
        if (win)
        {
            for (var i = 0; i < character.NumberOfTroopsDestroyed; i++)
                if (board.Find(u => u.Character.Name == adverser.Name && u.Area == zone).NumberOfUnits > 0)
                    board.Find(u => u.Character.Name == adverser.Name && u.Area == zone).NumberOfUnits -= 1;
            int nb = board.Find(b => b.Character.Name == adverser.Name && b.Area == zone).NumberOfUnits;
            if (nb > 0)
            {
                if (!board.Exists(b => b.Character.Name == adverser.Name && b.Area == movingZone))
                    board.Add(new Battalion(adverser, movingZone));

                board.Find(u => u.Character.Name == adverser.Name && u.Area == movingZone).NumberOfUnits += nb;
            }
            board.Remove(board.Find(b => b.Character.Name == adverser.Name && b.Area == zone));
        }
        else
        {
            for(var j = 0; j < adverser.NumberOfTroopsDestroyed; j++)
                if(board.Find(u => u.Character.Name == character.Name && u.Area == zone).NumberOfUnits > 0)
                    board.Find(u => u.Character.Name == character.Name && u.Area == zone).NumberOfUnits -= 1;
            
            int nb = board.Find(b => b.Character.Name == character.Name && b.Area == zone).NumberOfUnits;
            
            if (nb != 0)
            {
                if(!board.Exists(b => b.Character.Name == character.Name && b.Area == movingZone))
                    board.Add(new Battalion(character, movingZone));
                board.Find(u => u.Character.Name == character.Name && u.Area == movingZone).NumberOfUnits += nb;
            }
            
            board.Remove(board.Find(b => b.Character.Name == character.Name && b.Area == zone));
        }
    }

    public Action(List<Battalion> board, Character character)
    {
        this.board = board;
        this.character = character;
    }

    public Action(Action action)
    {
        this.board = action.board.Select(x=> x.Clone()).ToList();
        this.character = action.character;
        this.movingZone = action.movingZone;
        this.fightingZone = action.fightingZone;
        this.recruitingZone = action.recruitingZone;
    }
}

public class AI
{
    private List<Area> allAreas = new List<Area>() {Area.MIDWEST, Area.NORTHEAST, Area.NORTHWEST, Area.SOUTHEAST, Area.SOUTHWEST, Area.NORTH_CENTRAL, Area.SOUTH_CENTRAL};
    public List<Area> AllAreas
    {
        get => allAreas;
    }

    private List<Character> characters;
    public List<Character> Characters
    {
        get => characters;
        set => characters = value;
    }

    private Character simulatedPlayer;

    public Character SimulatedPlayer
    {
        get => simulatedPlayer;
        set => simulatedPlayer = value;
    }

    public Dictionary<Area, List<Area>> surroundings = new Dictionary<Area, List<Area>>()
    {
        {Area.MIDWEST, new List<Area>() {Area.NORTH_CENTRAL, Area.SOUTH_CENTRAL, Area.SOUTHEAST, Area.NORTHEAST}},
        {Area.NORTHEAST, new List<Area>() {Area.MIDWEST, Area.SOUTHEAST}},
        {Area.NORTHWEST, new List<Area>() {Area.NORTH_CENTRAL, Area.SOUTHWEST}},
        {Area.SOUTHEAST, new List<Area>() {Area.NORTHEAST, Area.MIDWEST, Area.SOUTH_CENTRAL}},
        {Area.SOUTHWEST, new List<Area>() {Area.NORTHWEST, Area.NORTH_CENTRAL, Area.SOUTH_CENTRAL}},
        {Area.NORTH_CENTRAL, new List<Area>() {Area.NORTHWEST, Area.SOUTHWEST, Area.SOUTH_CENTRAL, Area.MIDWEST}},
        {Area.SOUTH_CENTRAL, new List<Area>() {Area.SOUTHWEST, Area.NORTH_CENTRAL, Area.MIDWEST, Area.SOUTHEAST}}
    };
    
    public List<(Character, float)> Evaluate(List<Battalion> board, List<Character> characters)
    {
        return characters.Select(character => (character, GetCharacterEvaluation(character, board))).ToList();
    }
    private float GetCharacterEvaluation(Character character, List<Battalion> board)
    {
        float percentage = 0;
        
        float nbUnits = board.Where(b => b.Character.Name == character.Name)
            .Sum(b => b.NumberOfUnits);

        var controlledZones = GetZoneMasters(board).FindAll(b => b.Character.Name.Equals(character.Name)).Count;
        
        switch (character.Name)
        {
            case "Agent Yellow":
                percentage = nbUnits / board
                    .Where(b => b.Character.Name == "Colonel Red")
                    .Sum(b => b.NumberOfUnits) * 100;
                break;
            
            case "Colonel Red":
                percentage = ((controlledZones + nbUnits) 
                              / (float)character.NbOfTerritoriesToControl) * 100;
                break;
            
            case "Dr. Green":
                percentage = ((15 - character.AmountOfMoneyToHave) / 30) * 100 + ((controlledZones +
                    nbUnits) / (float)character.NbOfTerritoriesToControl) * 100 / 2;
                break;
            case " President Blue":
                percentage = (controlledZones / (float)character.NbOfTerritoriesToControl) * 100;
                break;
        }
        
        return percentage;
    }
    public List<Battalion> GetZoneMasters(List<Battalion> units)
    {
        var result = new List<Battalion>();
        foreach (var battalion in units)
        {
            Battalion concurrent = result.Find(b => battalion.Area == b.Area);
            
            if(concurrent == null && battalion.NumberOfUnits > 0)
                result.Add(battalion);
            else if (concurrent != null && concurrent.NumberOfUnits < battalion.NumberOfUnits)
            {
                result.Remove(concurrent);
                result.Add(battalion);
            }
            else if(concurrent != null && concurrent.NumberOfUnits == battalion.NumberOfUnits)
                result.Remove(concurrent);
        }
        return result;
    }
    public HashSet<Area> GetSurroundingZonesInPerimeter(int distance, Area startingZone)
    {
        HashSet<Area> result = new HashSet<Area>(surroundings[startingZone]);

        for (int i = 0; i < distance - 1; i++)
            foreach (Area area in result.Reverse())
                result.UnionWith(surroundings[area]);
        
        result.Remove(startingZone);
        return result;
    }
    public List<Area> GetZonesWhereCharacterHasUnits(Character character, List<Battalion> board)
    {
        List<Area> areas = board.Where(b => b.Character.Name == character.Name)
            .Select(u => u.Area)
            .ToList();
        return areas;
    }
    public int GetNumberOfUnitsInArea(Area area, Character character, List<Battalion> board)
    {
        return board.Find(b => b.Character.Name == character.Name && b.Area == area).NumberOfUnits;
    }

    public float GetAttackProbabilityOfSuccess(Character character, Area area, List<Battalion> board)
    {
        return ((float)GetNumberOfUnitsInArea(area, character, board)) / (float)6;
    }
    public int GetTotalNumberOfUnitsInField(Character character, List<Battalion> board)
    {
        return board
            .Where(b => b.Character.Name == character.Name)
            .Sum(b => b.NumberOfUnits); 
    }

    private List<Action> GetAllPossibleMoves(Character player, List<Battalion> board)
    {
        List<Action> result = new List<Action>();
        List<Action> recruitmentAction = new List<Action>();

        if (player.NumberOfUnits > GetTotalNumberOfUnitsInField(player, board))
        {
            // Recruitment
            recruitmentAction = board.Where(b => b.Character.Name == player.Name)
                .Select(u => u.Area)
                .Select(u =>
                {
                    var action = new Action(board.Select(x=> x.Clone()).ToList(), player);
                    action.Recruit(u);
                    return action;
                })
                .ToList();

            if (GetTotalNumberOfUnitsInField(player, board) == 0)
            {
                recruitmentAction = AllAreas.Select(a => 
                { 
                    var action = new Action(board.Select(x=> x.Clone()).ToList(), player);
                    action.Recruit(a);
                    return action; 
                }).ToList();
            }
        }
        else
        {
            recruitmentAction.Add(new Action(board, player));
            result.Add(new Action(board, player));
        }

        // Moving
        List<Action> movingAction1 = new List<Action>();
        recruitmentAction.ForEach(a =>
        {
            result.Add(a);
            
            GetZonesWhereCharacterHasUnits(player, a.Board).ForEach(z =>
            {
                
                for(int i = 1; i < GetNumberOfUnitsInArea(z, player, a.Board); i++)
                {
                    Action clone = new Action(a);
                    clone.movingZone.Item1 = z;
                    clone.movingZone.Item3 = i;
                    movingAction1.Add(clone);
                }
                
            });
        });
        
        List<Action> movingAction2 = new List<Action>();
        movingAction1.ForEach(a =>
        {
            foreach (Area area in GetSurroundingZonesInPerimeter(1, a.movingZone.Item1))
            {
                Action clone = new Action(a);
                clone.movingZone.Item2 = area;
                
                clone.Move(clone.movingZone.Item1, clone.movingZone.Item2, clone.movingZone.Item3);
                movingAction2.Add(clone);
            }
        });
        
        // Fighting

        movingAction2.ForEach(a => { 
            result.Add(a);
            // trouver toutes les attaques possibles
            List<Area> zones = a.Board.Where(b => b.Character.Name == a.character.Name)
                .Select(u => u.Area)
                .ToList();

            List<(Area, Character)> zones2 = a.Board
                .Where(b => b.Character.Name != a.character.Name && zones.Contains(b.Area))
                .Select(p => (p.Area, p.Character))
                .ToList();
            
            // pour chaque attaque créer un état s'il gagne + s'il perd
            zones2.ForEach(z =>
            {
                // Le cas où il gagne
                foreach (var area in GetSurroundingZonesInPerimeter(1, z.Item1))
                {
                    Action cloneWinning = new Action(a);
                    cloneWinning.fightingZone = z;
                    cloneWinning.Fight(true, GetAttackProbabilityOfSuccess(a.character, z.Item1, a.Board), area);
                    result.Add(cloneWinning);
                    
                    
                    Action cloneLosing = new Action(a);
                    cloneLosing.fightingZone = z;
                    cloneLosing.Fight(false, 1 - GetAttackProbabilityOfSuccess(a.character, z.Item1, a.Board), area);
                    result.Add(cloneLosing);
                }
            });
        });
        
        recruitmentAction.ForEach(a =>
        {
            result.Add(a);
            // trouver toutes les attaques possibles
            List<Area> zones = a.Board.Where(b => b.Character.Name == a.character.Name)
                .Select(u => u.Area)
                .ToList();

            List<(Area, Character)> zones2 = a.Board
                .Where(b => b.Character.Name != a.character.Name && zones.Contains(b.Area))
                .Select(p => (p.Area, p.Character))
                .ToList();
            
            // pour chaque attaque créer un état s'il gagne + s'il perd
            zones2.ForEach(z =>
            {
                // Le cas où il gagne
                foreach (var area in GetSurroundingZonesInPerimeter(1, z.Item1))
                {
                    Action cloneWinning = new Action(a);
                    cloneWinning.fightingZone = z;
                    cloneWinning.Fight(true, GetAttackProbabilityOfSuccess(a.character, z.Item1, a.Board), area);
                    result.Add(cloneWinning);
                }
                
                // Le cas où il perd
                foreach (var area in GetSurroundingZonesInPerimeter(1, z.Item1))
                {
                    Action cloneLosing = new Action(a);
                    cloneLosing.fightingZone = z;
                    cloneLosing.Fight(false, 1 - GetAttackProbabilityOfSuccess(a.character, z.Item1, a.Board), area);
                    result.Add(cloneLosing);
                }
            });
        });
        return result;
    }

    private float Maxn(List<Battalion> board, int depth, Character player, int n)
    {
        float score = GetCharacterEvaluation(player, board);

        if (score >= 100 || score <= -100 || depth == 0)
            return score;
        
        float best = -1000;

        foreach (Action move in GetAllPossibleMoves(player, board))
        {
            var newDepth = depth;
            if (Characters[n % Characters.Count].Name == SimulatedPlayer.Name)
                newDepth = depth - 1;

            best = Math.Max(best, Maxn(move.Board, newDepth , Characters[(n + 1) % Characters.Count], n + 1));
        }
        return best;
    }

    private Action FindBestMove(List<Battalion> board, Character player)
    {
        var bestMove = new Action(new List<Battalion>(), player);
        float bestVal = -1000;
        
        foreach (var move in GetAllPossibleMoves(player, board))
        {
            var moveVal = Maxn(move.Board, 1, Characters[0], 0);
            if (moveVal > bestVal)
            {
                bestMove = move;
                bestVal = moveVal;
            }
        }

        return bestMove;
    }

    public void test(Character character, List<Battalion> board, List<Character> characters)
    {
        Characters = characters;
        SimulatedPlayer = character;
        
        Action action = FindBestMove(board, character);
        
        Debug.Log("Recruiting zone : " + action.recruitingZone);
        Debug.Log("Moving zone : " + action.movingZone);
        Debug.Log("Fighting zone : " + action.fightingZone);
    }
}
