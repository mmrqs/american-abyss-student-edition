using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Debug = UnityEngine.Debug;

public class Transition
{
    //public Board board;
    public readonly float proba;

    public Transition(float proba)
    {
        //board = _board;
        this.proba = proba;
    }
}

public class Action
{
    public Area recruitingZone;
    public (Area, Area, int) movingZone;
    // fighting / character / moving zone
    public (Area, Character, Area) fightingZone;
    public List<Transition> transitions;
    public Character character;

    public Action(Character character)
    {
        transitions = new List<Transition>()
        {
            new Transition(1)
        };
        this.character = character;
    }

    public Action(Action action)
    {
        transitions = new List<Transition>();
        
        foreach (var actionTransition in action.transitions)
            transitions.Add(new Transition(actionTransition.proba));
        
        character = action.character;
        movingZone = action.movingZone;
        fightingZone = action.fightingZone;
        recruitingZone = action.recruitingZone;
    }
}

public class AI
{
    private List<Area> AllAreas { get; } = new List<Area>() {Area.MIDWEST, Area.NORTHEAST, Area.NORTHWEST, Area.SOUTHEAST, Area.SOUTHWEST, Area.NORTH_CENTRAL, Area.SOUTH_CENTRAL};
    private List<Character> Characters { get; set; }
    private Character SimulatedPlayer { get; set; }

    private readonly Dictionary<Area, List<Area>> surroundings = new Dictionary<Area, List<Area>>()
    {
        {Area.MIDWEST, new List<Area>() {Area.NORTH_CENTRAL, Area.SOUTH_CENTRAL, Area.SOUTHEAST, Area.NORTHEAST}},
        {Area.NORTHEAST, new List<Area>() {Area.MIDWEST, Area.SOUTHEAST}},
        {Area.NORTHWEST, new List<Area>() {Area.NORTH_CENTRAL, Area.SOUTHWEST}},
        {Area.SOUTHEAST, new List<Area>() {Area.NORTHEAST, Area.MIDWEST, Area.SOUTH_CENTRAL}},
        {Area.SOUTHWEST, new List<Area>() {Area.NORTHWEST, Area.NORTH_CENTRAL, Area.SOUTH_CENTRAL}},
        {Area.NORTH_CENTRAL, new List<Area>() {Area.NORTHWEST, Area.SOUTHWEST, Area.SOUTH_CENTRAL, Area.MIDWEST}},
        {Area.SOUTH_CENTRAL, new List<Area>() {Area.SOUTHWEST, Area.NORTH_CENTRAL, Area.MIDWEST, Area.SOUTHEAST}}
    };
    
    private float GetCharacterEvaluation(Character character, Board board)
    {
        float percentage = 0;

        float nbUnits = GetTotalNumberOfUnitsInField(character, board);

        var controlledZones = GetNumberOfControlledZones(character, board);
        
        switch (character.Name)
        {
            case "Agent Yellow":
                percentage = nbUnits / GetTotalNumberOfUnitsInField(Characters.Find(c => c.Name == "Colonel Red"), board) * 100;
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

    private static int GetNumberOfControlledZones(Character character, Board board)
    {
        var count = 0;
        foreach (var zone in board.UsZone)
        {
            if (!zone.Value.Characters.ContainsKey(character)) continue;
            var maxValue = zone.Value.Characters[character];
            var isMaster = maxValue != 0;

            if (maxValue != 0)
                foreach (var player in 
                    zone.Value.Characters
                        .Where(player => player.Key != character)
                        .Where(player => player.Value >= maxValue))
                    isMaster = false;
                
            if (isMaster)
                count++;
        }
        return count;
    }

    private IEnumerable<Area> GetSurroundingZonesInPerimeter(int distance, Area startingZone)
    {
        var result = new HashSet<Area>(surroundings[startingZone]);

        for (var i = 0; i < distance - 1; i++)
            foreach (var area in result.Reverse())
                result.UnionWith(surroundings[area]);
        
        result.Remove(startingZone);
        return result;
    }

    private static List<Area> GetZonesWhereCharacterHasUnits(Character character, Board board)
    {
        return board.UsZone
            .Where(x => x.Value.Characters.ContainsKey(character) && x.Value.Characters[character] > 0)
            .Select(x => x.Key)
            .ToList();
    }

    private static int GetNumberOfUnitsInArea(Area area, Character character, Board board)
    {
        return board.UsZone[area].Characters.ContainsKey(character) ? board.UsZone[area].Characters[character] : 0;
    }
    private static float GetAttackProbabilityOfSuccess(Character character, Area area, Board board, int additionToZone)
    {
        return ((float)GetNumberOfUnitsInArea(area, character, board) + additionToZone) / (float)6;
    }
    private static int GetTotalNumberOfUnitsInField(Character character, Board board)
    {
        return board.UsZone
            .Where(x => x.Value.Characters.ContainsKey(character) && x.Value.Characters[character] > 0)
            .Select(x => x.Value.Characters[character])
            .Sum();
    }

    private static bool SimulationIfControlsArea(Character character, Board board, Area area, int nbOfUnitsChanging)
    {
        int maxValue;
        if (!board.UsZone[area].Characters.ContainsKey(character))
            maxValue = 1 + nbOfUnitsChanging;
        else
            maxValue = board.UsZone[area].Characters[character] + nbOfUnitsChanging;
        
        var isMaster = maxValue >= 0;
        
        foreach (var zone in board.UsZone[area].Characters.Where(zone => zone.Value >= maxValue && isMaster && zone.Key != character))
            isMaster = false;
        return isMaster;
    }
    
    private List<Action> GetAllPossibleMoves(Character player, Board board)
    {
        List<Action> result = new List<Action>();
        List<Action> recruitmentAction = new List<Action>();
        List<Action> temporaryList = new List<Action>();
        
        if (player.NumberOfUnits > GetTotalNumberOfUnitsInField(player, board))
        {
            
            // Recrutement, on ajoute toutes les zones où le personnage a une unité
            result = board.UsZone.Where(b => b.Value.Characters.ContainsKey(player) && b.Value.Characters[player] > 0)
                .Select(u =>
                {
                    var action = new Action(player) {recruitingZone = u.Key};
                    //action.Recruit(u.Key);
                    return action;
                }).ToList();
            
            // si le personnage n'a plus d'unités sur le board, on ajoute toutes les zones
            if (result.Count == 0)
            {
                result = AllAreas.Select(a => 
                { 
                    var action = new Action(player) {recruitingZone = a};
                    return action; 
                }).ToList();
            }
        }
        else
        {
            result.Add(new Action(player));
        }
        
        // cas où le joueur fait Recrutement -> Fight et zappe le mouvemeent
        result.ForEach(a =>
        {
            // trouver toutes les attaques possibles
            var zone = new List<KeyValuePair<Area, USZone>>();
            var zones2 = new List<(Area, Character)>();
            
            // récupère toutes les zones où il peut attaquer
            zone = board.UsZone
                .Where(b => b.Value.Characters.Values.Count > 1 && b.Value.Characters.ContainsKey(player) && b.Value.Characters[player] > 0)
                .ToList();
            
            if(zone.All(z => z.Key != a.recruitingZone) && a.recruitingZone != Area.FOO)
                zone.Add(new KeyValuePair<Area, USZone>(a.recruitingZone, new USZone(board.UsZone[a.recruitingZone].Characters)));

            // on converti ça en list de tuples
            foreach (var t in zone)
            {
                zones2.AddRange(t.Value.Characters
                    .Where(k => k.Key != player)
                    .Select(i => (t.Key, i.Key)));
            }
            
            // pour chacun on crée un état si la proba de succès est supérieure à 40%
            zones2.ForEach(z =>
            {
                recruitmentAction.AddRange(from area in 
                    GetSurroundingZonesInPerimeter(1, z.Item1) 
                    let proba = GetAttackProbabilityOfSuccess(a.character, z.Item1, board, a.recruitingZone == z.Item1 ? 1 : 0) 
                    where proba >= 0.40 select new Action(a) {fightingZone = {Item1 = z.Item1, Item3 = area, Item2 = z.Item2},
                        transitions = new List<Transition>{new Transition(proba), new Transition(1-proba)}});
            });
        });
        
        // Mouvement
        foreach (var a in result.Reverse<Action>())
        {
            List<Area> departureZones = GetZonesWhereCharacterHasUnits(player, board);
            if (departureZones.Count == 0)
                departureZones.Add(a.recruitingZone);
            
            departureZones.ForEach(z =>
            {
                float nbUnitsInArea = GetNumberOfUnitsInArea(z, player, board);
                if (z == a.recruitingZone)
                    nbUnitsInArea++;
                
                for(int i = 1; i <= nbUnitsInArea; i++)
                {
                    var addition = 0;
                    if (z == a.recruitingZone)
                        addition++;
                    // added for perfs issues : on bouge si on ne perd pas le contrôle de la zone d'où on vient
                    if (!SimulationIfControlsArea(player, board, z, -i + addition) && 
                        SimulationIfControlsArea(player, board, z, addition))
                        continue;

                    result.AddRange(GetSurroundingZonesInPerimeter(1, z)
                        .Select(area => new Action(a) {movingZone = {Item1 = z, Item3 = i, Item2 = area}}));
                }
            });
        }

        // Fighting
        
        result.ForEach(a => { 
            // trouver toutes les attaques possibles

            List<(Area, Character)> zones2 = new List<(Area, Character)>();

           Dictionary<Area, USZone> zone = board.UsZone
                .Where(b => b.Value.Characters.ContainsKey(a.character) && b.Value.Characters[a.character] > 0 &&
                            b.Value.Characters.Values.Count > 1)
                .ToDictionary(x => x.Key, x => x.Value);

           if(!zone.ContainsKey(a.recruitingZone) && a.recruitingZone != Area.FOO)
                zone.Add(a.recruitingZone, new USZone(board.UsZone[a.recruitingZone].Characters));
            
            if(!zone.ContainsKey(a.movingZone.Item2) && a.movingZone.Item2 != Area.FOO)
                zone.Add(a.movingZone.Item2, new USZone(board.UsZone[a.movingZone.Item2].Characters));

            var add = a.recruitingZone == a.movingZone.Item1 ? 1 : 0;
            
            if (zone.ContainsKey(a.movingZone.Item1) &&
                (GetNumberOfUnitsInArea(a.movingZone.Item1, a.character, board) + add) <= a.movingZone.Item3 
                && a.movingZone.Item1 != Area.FOO)
                zone.Remove(a.movingZone.Item1);
            
            
            foreach (var t in zone)
            {
                zones2.AddRange(t.Value.Characters
                    .Where(k => k.Key != player)
                    .Select(i => (t.Key, i.Key)));
            }

            zones2.ForEach(z =>
            {
                var surroundingsAreas = GetSurroundingZonesInPerimeter(1, z.Item1);

                foreach (var area in surroundingsAreas)
                {
                    var movements = 0;
                    if(a.recruitingZone == area)
                        movements++;
                    if (a.movingZone.Item1 == area)
                        movements -= a.movingZone.Item3;
                    if (a.movingZone.Item2 == area)
                        movements += a.movingZone.Item3;
                    
                    var proba = GetAttackProbabilityOfSuccess(a.character, z.Item1, board, movements);
                    if(proba >= 0.40)
                    {
                        var clone = new Action(a) {fightingZone = {Item1 = z.Item1, Item3 = area, Item2 = z.Item2},
                            transitions = new List<Transition>() {new Transition(proba), new Transition(1 - proba)}};
                        temporaryList.Add(clone);
                    }
                }
            });
        });
        result.AddRange(temporaryList);
        result.AddRange(recruitmentAction);
        
        result = result.OrderByDescending(o => o.fightingZone.Item2).ToList();       
        temporaryList = null;
        recruitmentAction = null;
        return result;
    }
    
    private float Maxn(Board board, int depth, Character player, int n)
    {
        float score = GetCharacterEvaluation(player, board);
        int nextN = (n + 1) % (Characters.Count);

        if (score >= 100 || score <= -100 || depth <= 0)
            return score;
        
        float best = -1000;

        foreach (Action move in GetAllPossibleMoves(player, board))
        {
            float actionValue = 0;
        
            var newBoard = (new Board(ActionSimulation.MakeMove(move, board, move.fightingZone.Item2).UsZone), 
                move.fightingZone.Item1 == Area.FOO ? null : new Board(ActionSimulation.MakeMove(move, board, move.character).UsZone));
            
            bool win = true;

            foreach (Transition transition in move.transitions)
            {
                var newDepth = depth;
                if (Characters[n % (Characters.Count)].Name == SimulatedPlayer.Name)
                    newDepth = depth - 1;
            
                var b = win ? newBoard.Item1 : newBoard.Item2;
                
                actionValue += Maxn(b, newDepth, Characters[(nextN) % (Characters.Count)], nextN) *
                               transition.proba;
                win = false;

            }

            best = Math.Max(best, actionValue);
        }
        return best;
    }

    private Action FindBestMove(Board board)
    {
        var player = SimulatedPlayer;
        var bestMove = new Action(player);
        float bestVal = -1000;
        
        foreach (var action in GetAllPossibleMoves(player, board))
        {
            // TODO do action
            var newBoard = (new Board(ActionSimulation.MakeMove(action, board, action.fightingZone.Item2).UsZone), 
                action.fightingZone.Item1 == Area.FOO ? null : new Board(ActionSimulation.MakeMove(action, board, action.character).UsZone));
         
            bool win = true;
            
            float actionValue = 0;
            int index = Characters.FindIndex(p => p.Name == SimulatedPlayer.Name);

            foreach (var transition in action.transitions)
            {
                var b = win ? newBoard.Item1 : newBoard.Item2;
                actionValue += Maxn(b, 2, Characters[(index + 1) % Characters.Count], (index + 1) % Characters.Count) * transition.proba;
                win = false;

            }
            if (!(actionValue > bestVal)) continue;
            bestMove = action;
            bestVal = actionValue;
        }

        return bestMove;
    }

    public void test(Character character, List<Battalion> board, List<Character> characters)
    {
        
        Board b = new Board();
        foreach (var battalion in board)
            b.UsZone[battalion.Area].Characters.Add(battalion.Character, battalion.NumberOfUnits);

        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        Characters = characters;
        SimulatedPlayer = character;

        Action action = FindBestMove(b);
        
        Debug.Log("Recruiting zone : " + action.recruitingZone);
        Debug.Log("Moving zone : " + action.movingZone);
        Debug.Log("Fighting zone : " + action.fightingZone);

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