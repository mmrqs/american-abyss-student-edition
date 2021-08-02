using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Unity.Collections.LowLevel.Unsafe;
using Debug = UnityEngine.Debug;

public class Transition
{
    public Board board;
    public float proba;

    public Transition(Board _board, float _proba)
    {
        board = _board;
        proba = _proba;
    }
}
public class Action
{
    public Area recruitingZone;
    public (Area, Area, int) movingZone;
    public (Area, Character) fightingZone;
    public List<Transition> transitions;
    public Character character;
    
    // recrute une unité sur une zone spécifique
    public void Recruit(Area area)
    {
        if(!transitions[0].board.UsZone[area].Characters.ContainsKey(character))
            transitions[0].board.UsZone[area].Characters.Add(character, 0);

        transitions[0].board.UsZone[area].Characters[character] += 1;
        recruitingZone = area;
    }
    
    // bouge une ou plusieurs unités
    public void Move(Area startingArea, Area endArea, int numberOfUnitsToMove)
    {
        transitions[0].board.UsZone[startingArea].Characters[character] -= numberOfUnitsToMove;
            
        if (transitions[0].board.UsZone[startingArea].Characters[character] <= 0)
            transitions[0].board.UsZone[startingArea].Characters.Remove(character);

        
        if(!transitions[0].board.UsZone[endArea].Characters.ContainsKey(character))
            transitions[0].board.UsZone[endArea].Characters.Add(character, 0);
            
        transitions[0].board.UsZone[endArea].Characters[character] += numberOfUnitsToMove;
    }
    
    // combat une unité
    public void Fight(float chanceToWin, Area movingZone)
    {
        Character adverser = fightingZone.Item2;
        Area zone = fightingZone.Item1;
        
        fightingZone = (zone, adverser);
        
        // créée un état gagnant et un état perdant
        List<Transition> newTransitions = new List<Transition>();
        
        foreach (Transition transition in transitions)
        {
            Transition winningTransition = new Transition(new Board(transition.board.UsZone), chanceToWin);
            Transition loosingTransition = new Transition(transition.board, 1 - chanceToWin);

            for (var i = 0; i < character.NumberOfTroopsDestroyed; i++)
                if (winningTransition.board.UsZone[zone].Characters[adverser] > 0)
                    winningTransition.board.UsZone[zone].Characters[adverser] -= 1;

            int nb = winningTransition.board.UsZone[zone].Characters[adverser];
            
            if (nb > 0)
            {
                if (!winningTransition.board.UsZone[movingZone].Characters.ContainsKey(adverser))
                    winningTransition.board.UsZone[movingZone].Characters.Add(adverser, 0);

                winningTransition.board.UsZone[movingZone].Characters[adverser] += nb;
            }

            winningTransition.board.UsZone[zone].Characters.Remove(adverser);
            
            // détruit unités
            if (loosingTransition.board.UsZone[zone].Characters[character] > 0)
                loosingTransition.board.UsZone[zone].Characters[character] -= 1;
         
            nb = loosingTransition.board.UsZone[zone].Characters[character];
            
            // s'il reste des unités on les bouge
            if (nb != 0)
            {
                if (!loosingTransition.board.UsZone[movingZone].Characters.ContainsKey(character))
                    loosingTransition.board.UsZone[movingZone].Characters.Add(character, 0);
                
                loosingTransition.board.UsZone[movingZone].Characters[character] += nb;
            }
            
            loosingTransition.board.UsZone[zone].Characters.Remove(character);
            
            newTransitions.Add(winningTransition);
            newTransitions.Add(loosingTransition);
        }

        transitions = newTransitions;
    }

    public Action(Board board, Character character)
    {
        transitions = new List<Transition>()
        {
            new Transition(board, 1)
        };
        this.character = character;
    }

    public Action(Action action)
    {
        transitions = new List<Transition>();
        
        foreach (var actionTransition in action.transitions)
            transitions.Add(new Transition(new Board(actionTransition.board.UsZone), actionTransition.proba));
        
        character = action.character;
        movingZone = action.movingZone;
        fightingZone = action.fightingZone;
        recruitingZone = action.recruitingZone;
    }

    public Board GetBoard()
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
    
    public List<(Character, float)> Evaluate(Board board, List<Character> characters)
    {
        return characters.Select(character => (character, GetCharacterEvaluation(character, board))).ToList();
    }
    private float GetCharacterEvaluation(Character character, Board board)
    {
        float percentage = 0;

        float nbUnits = GetTotalNumberOfUnitsInField(character, board);

        var controlledZones = GetNumberOfControlledZones(character, board);
        
        switch (character.Name)
        {
            case "Agent Yellow":
                percentage = nbUnits / GetTotalNumberOfUnitsInField(characters.Find(c => c.Name == "Colonel Red"), board) * 100;
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

    private int GetNumberOfControlledZones(Character character, Board board)
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

    private HashSet<Area> GetSurroundingZonesInPerimeter(int distance, Area startingZone)
    {
        var result = new HashSet<Area>(surroundings[startingZone]);

        for (var i = 0; i < distance - 1; i++)
            foreach (var area in result.Reverse())
                result.UnionWith(surroundings[area]);
        
        result.Remove(startingZone);
        return result;
    }

    private List<Area> GetZonesWhereCharacterHasUnits(Character character, Board board)
    {
        return board.UsZone
            .Where(x => x.Value.Characters.ContainsKey(character) && x.Value.Characters[character] > 0)
            .Select(x => x.Key)
            .ToList();
    }

    private int GetNumberOfUnitsInArea(Area area, Character character, Board board)
    {
        return board.UsZone[area].Characters.ContainsKey(character) ? board.UsZone[area].Characters[character] : 0;
    }
    private float GetAttackProbabilityOfSuccess(Character character, Area area, Board board)
    {
        return ((float)GetNumberOfUnitsInArea(area, character, board)) / (float)6;
    }
    private int GetTotalNumberOfUnitsInField(Character character, Board board)
    {
        return board.UsZone
            .Where(x => x.Value.Characters.ContainsKey(character) && x.Value.Characters[character] > 0)
            .Select(x => x.Value.Characters[character])
            .Sum();
    }

    private bool SimulationIfControlsArea(Character character, Board board, Area area, int NbOfUnitsChanging)
    {
        var maxValue = board.UsZone[area].Characters[character] + NbOfUnitsChanging;
        var isMaster = maxValue >= 0;
        
        foreach (var zone in board.UsZone[area].Characters)
            if(zone.Value >= maxValue && isMaster && zone.Key != character)
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
                    var action = new Action(new Board(board.UsZone), player);
                    action.Recruit(u.Key);
                    return action;
                }).ToList();
            
            // si le personnage n'a plus d'unités sur le board, on ajoute toutes les zones
            if (result.Count == 0)
            {
                result = AllAreas.Select(a => 
                { 
                    var action = new Action(new Board(board.UsZone), player);
                    action.Recruit(a);
                    return action; 
                }).ToList();
            }
        }
        else
        {
            result.Add(new Action(board, player));
        }
        
        // cas où le joueur fait Recrutement -> Fight et zappe le mouvemeent
        result.ForEach(a =>
        {
            // trouver toutes les attaques possibles
            var zone = new List<KeyValuePair<Area, USZone>>();
            var zones2 = new List<(Area, Character)>();
            
            // récupère toutes les zones où il peut attaquer
            zone = a.GetBoard().UsZone
                .Where(b => b.Value.Characters.Values.Count > 1 && b.Value.Characters.ContainsKey(player) && b.Value.Characters[player] > 0)
                .ToList();
            
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
                foreach (var area in GetSurroundingZonesInPerimeter(1, z.Item1))
                {
                    float proba = GetAttackProbabilityOfSuccess(a.character, z.Item1, a.GetBoard());

                    if (proba >= 0.40)
                    {
                        Action clone = new Action(a) {fightingZone = z};
                        clone.Fight(proba, area);
                        recruitmentAction.Add(clone);
                    }
                }
            });
        });
        
        // Mouvement
        foreach (var a in result.Reverse<Action>())
        {
            GetZonesWhereCharacterHasUnits(player, a.GetBoard()).ForEach(z =>
            {
                for(int i = 1; i <= GetNumberOfUnitsInArea(z, player, a.GetBoard()); i++)
                {
                    // added for perfs issues : on bouge si on ne perd pas le contrôle de la zone d'où on vient
                    if (!SimulationIfControlsArea(player, a.GetBoard(), z, -i) && 
                        SimulationIfControlsArea(player, a.GetBoard(), z, 0))
                        continue;
                    foreach (var clone in GetSurroundingZonesInPerimeter(1, z)
                        .Select(area => new Action(a) {movingZone = {Item1 = z, Item3 = i, Item2 = area}}))
                    {
                        clone.Move(clone.movingZone.Item1, clone.movingZone.Item2, clone.movingZone.Item3);
                        result.Add(clone);
                    }
                }
            });
        }

        // Fighting
        
        result.ForEach(a => { 
            // trouver toutes les attaques possibles
            
            List<(Area, Character)> zones2 = new List<(Area, Character)>();
            List<KeyValuePair<Area, USZone>> zone = new List<KeyValuePair<Area, USZone>>();
            zone = a.GetBoard().UsZone
                .Where(b => b.Value.Characters.ContainsKey(a.character) && b.Value.Characters[a.character] > 0 &&
                            b.Value.Characters.Values.Count > 1)
                .ToList();
            
            foreach (var t in zone)
            {
                zones2.AddRange(t.Value.Characters
                    .Where(k => k.Key != player)
                    .Select(i => (t.Key, i.Key)));
            }
            
            zones2.ForEach(z =>
            {
                HashSet<Area> surroundingsAreas = GetSurroundingZonesInPerimeter(1, z.Item1);

                foreach (var area in surroundingsAreas)
                {
                    float proba = GetAttackProbabilityOfSuccess(a.character, z.Item1, a.GetBoard());
                    if(proba >= 0.40)
                    {
                        var clone = new Action(a) {fightingZone = z};
                        clone.Fight(proba, area);
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

    private static int cnt = 0;
    private float Maxn(Board board, int depth, Character player, int n)
    {
        cnt++;
        float score = GetCharacterEvaluation(player, board);
        int nextN = (n + 1) % (Characters.Count);
        if (score >= 100 || score <= -100 || player.Name == SimulatedPlayer.Name)
            return score;
        
        float best = -1000;
        
        var newDepth = depth;
        newDepth = depth - 1;
        var possibilities = GetAllPossibleMoves(player, board);
        
        foreach (Action move in possibilities)
        {
            float actionValue = move.transitions
                .Sum(transition => Maxn(transition.board, newDepth, Characters[(nextN) % (Characters.Count)], nextN) * transition.proba);
            best = Math.Max(best, actionValue);
        }
        return best;
    }

    private Action FindBestMove(Board board, Character player)
    {
        var bestMove = new Action(new Board(), player);
        float bestVal = -1000;
        var values = GetAllPossibleMoves(player, board);
        foreach (var action in values)
        {
            int index = Characters.FindIndex(p => p.Name == SimulatedPlayer.Name);
            float actionValue = action.transitions.Sum(transition => Maxn(transition.board, 1, Characters[(index + 1) % Characters.Count], (index + 1) % Characters.Count) * transition.proba);

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
        
        Action action = FindBestMove(b, SimulatedPlayer);
        
        Debug.Log("Recruiting zone : " + action.recruitingZone);
        Debug.Log("Moving zone : " + action.movingZone);
        Debug.Log("Fighting zone : " + action.fightingZone);
        
        //Debug.Log(GetAllPossibleMoves(characters.Find(c => c.Name == " President Blue"), b).Count);
        
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