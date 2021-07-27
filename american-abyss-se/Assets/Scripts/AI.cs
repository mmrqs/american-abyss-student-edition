using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Debug = UnityEngine.Debug;

public class Transition
{
    public List<Battalion> board;
    public float proba;

    public Transition(List<Battalion> _board, float _proba)
    {
        this.board = _board;
        this.proba = _proba;
    }
}
public class Action
{
    public Area recruitingZone;
    public (Area, Area, int) movingZone;
    public (Area, Character) fightingZone;
    public List<Transition> transitions;
    public Character character;
    public void Recruit(Area area)
    {
        foreach (Transition transition in transitions)
        {
            if(!transition.board.Exists(t => t.Character.Name == character.Name && t.Area == area))
                transition.board.Add(new Battalion(character, area));
            transition.board.First(t => t.Character.Name == character.Name && t.Area == area).NumberOfUnits += 1;
            recruitingZone = area;
        }
    }

    public void Move(Area startingArea, Area endArea, int numberOfUnitsToMove)
    {
        foreach (Transition transition in transitions)
        {
            transition.board.Find(u => u.Character.Name == character.Name && u.Area == startingArea).NumberOfUnits -= numberOfUnitsToMove;

            if(transition.board.Find(u => u.Character.Name == character.Name && u.Area == startingArea).NumberOfUnits == 0) 
                transition.board.Remove(transition.board.Find(b => b.Character.Name == character.Name && b.Area == startingArea));

            if (!transition.board.Exists(b => b.Character.Name == character.Name && b.Area == endArea))
                transition.board.Add(new Battalion(character, endArea));

            transition.board.Find(u => u.Character.Name == character.Name && u.Area == endArea).NumberOfUnits += numberOfUnitsToMove;
        }
    }

    public void Fight(float chanceToWin, Area movingZone)
    {
        Character adverser = fightingZone.Item2;
        Area zone = fightingZone.Item1;
        
        //chanceToOccur = percentage;
        fightingZone = (zone, adverser);
        
        List<Transition> newTransitions = new List<Transition>();
        
        foreach (Transition transition in this.transitions)
        {
            
            Transition winningTransition = new Transition(transition.board.Select(x => x.Clone()).ToList(), chanceToWin);
            
            for (var i = 0; i < character.NumberOfTroopsDestroyed; i++)
                if (winningTransition.board.Find(u => u.Character.Name == adverser.Name && u.Area == zone).NumberOfUnits > 0)
                    winningTransition.board.Find(u => u.Character.Name == adverser.Name && u.Area == zone).NumberOfUnits -= 1;
            
            int nb = winningTransition.board.Find(b => b.Character.Name == adverser.Name && b.Area == zone).NumberOfUnits;
            if (nb > 0)
            {
                if (!winningTransition.board.Exists(b => b.Character.Name == adverser.Name && b.Area == movingZone))
                    winningTransition.board.Add(new Battalion(adverser, movingZone));

                winningTransition.board.Find(u => u.Character.Name == adverser.Name && u.Area == movingZone).NumberOfUnits += nb;
            }
            winningTransition.board.Remove(winningTransition.board.Find(b => b.Character.Name == adverser.Name && b.Area == zone));

            Transition loosingTransition = new Transition(transition.board, 1 - chanceToWin);

            for(var j = 0; j < adverser.NumberOfTroopsDestroyed; j++)
                if(loosingTransition.board.Find(u => u.Character.Name == character.Name && u.Area == zone).NumberOfUnits > 0)
                    loosingTransition.board.Find(u => u.Character.Name == character.Name && u.Area == zone).NumberOfUnits -= 1;
            
            nb = loosingTransition.board.Find(b => b.Character.Name == character.Name && b.Area == zone).NumberOfUnits;
            
            if (nb != 0)
            {
                if(!loosingTransition.board.Exists(b => b.Character.Name == character.Name && b.Area == movingZone))
                    loosingTransition.board.Add(new Battalion(character, movingZone));
                loosingTransition.board.Find(u => u.Character.Name == character.Name && u.Area == movingZone).NumberOfUnits += nb;
            }
            
            loosingTransition.board.Remove(loosingTransition.board.Find(b => b.Character.Name == character.Name && b.Area == zone));   
            
            newTransitions.Add(winningTransition);
            newTransitions.Add(loosingTransition);
        }

        this.transitions = newTransitions;
    }

    public Action(List<Battalion> board, Character character)
    {
        this.transitions = new List<Transition>()
        {
            new Transition(board, 1)
        };
        this.character = character;
    }

    public Action(Action action)
    {
        this.transitions = new List<Transition>();
        foreach (var actionTransition in action.transitions)
        {
            this.transitions.Add(new Transition(actionTransition.board.Select(x=> x.Clone()).ToList(), actionTransition.proba));
        }
        this.character = action.character;
        this.movingZone = action.movingZone;
        this.fightingZone = action.fightingZone;
        this.recruitingZone = action.recruitingZone;
    }

    public List<Battalion> GetBoard()
    {
        if(transitions.Count != 1)
            throw new Exception("More than one transition");
        return transitions[0].board;
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
            
            GetZonesWhereCharacterHasUnits(player, a.GetBoard()).ForEach(z =>
            {
                
                for(int i = 1; i < GetNumberOfUnitsInArea(z, player, a.GetBoard()); i++)
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
            List<Area> zones = a.GetBoard().Where(b => b.Character.Name == a.character.Name)
                .Select(u => u.Area)
                .ToList();

            List<(Area, Character)> zones2 = a.GetBoard()
                .Where(b => b.Character.Name != a.character.Name && zones.Contains(b.Area))
                .Select(p => (p.Area, p.Character))
                .ToList();
            
            // pour chaque attaque créer un état s'il gagne + s'il perd
            zones2.ForEach(z =>
            {
                // Le cas où il gagne
                foreach (var area in GetSurroundingZonesInPerimeter(1, z.Item1))
                {
                    Action clone = new Action(a);
                    clone.fightingZone = z;
                    clone.Fight(GetAttackProbabilityOfSuccess(a.character, z.Item1, a.GetBoard()), area);
                    result.Add(clone);
                }
            });
        });
        
        recruitmentAction.ForEach(a =>
        {
            result.Add(a);
            // trouver toutes les attaques possibles
            List<Area> zones = a.GetBoard().Where(b => b.Character.Name == a.character.Name)
                .Select(u => u.Area)
                .ToList();

            List<(Area, Character)> zones2 = a.GetBoard()
                .Where(b => b.Character.Name != a.character.Name && zones.Contains(b.Area))
                .Select(p => (p.Area, p.Character))
                .ToList();
            
            // pour chaque attaque créer un état s'il gagne + s'il perd
            zones2.ForEach(z =>
            {
                foreach (var area in GetSurroundingZonesInPerimeter(1, z.Item1))
                {
                    Action clone = new Action(a);
                    clone.fightingZone = z;
                    clone.Fight(GetAttackProbabilityOfSuccess(a.character, z.Item1, a.GetBoard()), area);
                    result.Add(clone);
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
            float actionValue = 0;
            foreach (Transition transition in move.transitions)
            {
                var newDepth = depth;
                if (Characters[n % Characters.Count].Name == SimulatedPlayer.Name)
                    newDepth = depth - 1;
                actionValue += Maxn(transition.board, newDepth, Characters[(n + 1) % Characters.Count], n + 1) *
                               transition.proba;
            }
            best = Math.Max(best, actionValue);

        }
        return best;
    }

    private Action FindBestMove(List<Battalion> board, Character player)
    {
        var bestMove = new Action(new List<Battalion>(), player);
        float bestVal = -1000;
        
        foreach (var action in GetAllPossibleMoves(player, board))
        {
            float actionValue = 0;
            foreach (Transition transition in action.transitions)
            {
                actionValue += Maxn(transition.board, 1, Characters[0], 0) * transition.proba;
            }
            if (actionValue > bestVal)
            {
                bestMove = action;
                bestVal = actionValue;
            }
        }

        return bestMove;
    }

    public void test(Character character, List<Battalion> board, List<Character> characters)
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        Characters = characters;
        SimulatedPlayer = character;
        
        //Action action = FindBestMove(board, character);
        
        /*Debug.Log("Recruiting zone : " + action.recruitingZone);
        Debug.Log("Moving zone : " + action.movingZone);
        Debug.Log("Fighting zone : " + action.fightingZone);*/
        
        Debug.Log(GetAllPossibleMoves(character, board).Count);
        
        stopWatch.Stop();
        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = stopWatch.Elapsed;

        // Format and display the TimeSpan value.
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        Debug.Log("RunTime " + elapsedTime);
    }
}
