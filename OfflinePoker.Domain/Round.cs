using System.Collections.Generic;
using System.Linq;
using OfflinePoker.Domain.Exceptions;

namespace OfflinePoker.Domain
{
    public class RoundActor
    {
        public int Bet;
        public Act LastAct;
        public Player Player;

        public RoundActor MakeAct(Act act, int bet) =>
            new RoundActor
            {
                Bet = Bet + bet,
                LastAct = act,
                Player = Player
            };

        public static RoundActor CreateFrom(Player player) =>
            new RoundActor {Bet = 0, LastAct = 0, Player = player};
    }

    public class RoundState
    {
        public int InTurn;
        public int Bet;
        public int Pot;
    }

    public class Round
    {
        private readonly RoundActor[] actors;
        private readonly BettingRules rules;
        private RoundState state;

        private Round(BettingRules rules, IEnumerable<RoundActor> actors, RoundState state)
        {
            this.rules = rules;
            this.state = state;
            this.actors = actors.ToArray();
        }

        public static Round CreateInitial(Player[] players, BettingRules rules)
        {
            var actors = new List<RoundActor>();
            var smallTaken = false;

            var i = players.Length - 1;
            for (; i >= 0; i--)
            {
                var player = players[i];
                if (!smallTaken)
                {
                    if (player.Stack < rules.SmallBlind)
                        continue;

                    actors.Add(RoundActor
                        .CreateFrom(player)
                        .MakeAct(Act.Bet, rules.SmallBlind)
                    );

                    smallTaken = true;
                    continue;
                }

                if (player.Stack < rules.BigBlind)
                    continue;

                actors.Add(RoundActor
                    .CreateFrom(player)
                    .MakeAct(Act.Bet, rules.BigBlind)
                );
            }

            actors.Reverse();
            if (actors.Count < 2)
                throw new GameLogicException("Can't start round. Not enough players.");

            actors.AddRange(players.Take(i).Select(RoundActor.CreateFrom));


            var state = new RoundState
            {
                Bet = rules.BigBlind,
                Pot = rules.BigBlind + rules.SmallBlind,
                InTurn = actors.Count == 2 ? 1 : 0
            };

            return new Round(rules, actors, state);
        }

        private RoundActor CurrentActor => actors[state.InTurn];
        public Player CurrentPlayer => CurrentActor.Player;

        public bool IsComplete => GetNextActorIndex(actors, state) < 0;

        public bool HasWinner => actors.Count(a => a.LastAct != Act.Fold) == 1;

        public Round StartNext()
        {
            if (HasWinner)
                throw new GameLogicException("Can't start next round. Winner already exists");

            var activeActors = actors.Where(a => a.LastAct != Act.Fold);
            return new Round(rules, activeActors, new RoundState());
        }

        public void MakeAct(Act act, int bet = 0)
        {
            MakeAct(CurrentActor, act, bet);
        }

        private void MakeAct(RoundActor actor, Act act, int bet)
        {
            if (actor != CurrentActor)
                throw new GameOrderException($"Can not make the act. Current actor is {CurrentActor}");

            if (!ShouldAct(actor, state))
                throw new GameLogicException("Current actor is not supposed to act.");

            var options = GetOptions(actor, state);
            if (!options.Contains(act))
                throw new GameLogicException($"Actor is not supposed to make '{act}'");
            
            state = MakeAct(state, actors, act, bet);
            actor.LastAct = act;
            actor.Bet += bet;
        }

        private static RoundState MakeAct(RoundState state, RoundActor[] actors, Act act, int bet)
        {
            var actor = actors[state.InTurn].MakeAct(act, bet);
            actors[state.InTurn] = actor;
            var currentBet = state.Bet;
            var pot = state.Pot;

            if (act == Act.Bet)
            {
                if (currentBet != 0)
                    throw new GameLogicException($"Can not make '{Act.Bet}'. Round is already open.");

                currentBet = bet;
                pot += bet;
            }

            if (act == Act.Raise)
            {
                if (bet < currentBet)
                    throw new GameLogicException($"Can not make '{Act.Raise}'. Wager is to low.");

                currentBet = bet;
                pot += bet;
            }

            if (act == Act.Call)
            {
                if (actor.Bet != state.Bet)
                    throw new GameLogicException("Call is not matching.");
                pot += bet;
            }

            return new RoundState
            {
                InTurn = GetNextActorIndex(actors, state),
                Pot = pot,
                Bet = currentBet
            };
        }

        private static int GetNextActorIndex(IReadOnlyList<RoundActor> actors, RoundState state)
        {
            var currentActor = state.InTurn;
            for (var i = currentActor + 1; i < actors.Count; i++)
            {
                if (ShouldAct(actors[i], state)) return i;
            }

            for (var i = 0; i < currentActor + 1; i++)
            {
                if (ShouldAct(actors[i], state)) return i;
            }

            return -1;
        }

        private static bool ShouldAct(RoundActor actor, RoundState state)
        {
            if (actor.LastAct == Act.Fold || actor.LastAct == Act.AllIn)
                return false;

            if (actor.LastAct == Act.None)
                return true;

            return actor.Bet < state.Bet;
        }

        private static Act[] GetOptions(RoundActor actor, RoundState state)
        {
            if (actor.LastAct != Act.Fold)
            {
                if (state.Bet == 0)
                    return new[] {Act.Bet, Act.Check};

                if (actor.Bet == state.Bet)
                    return new[] {Act.Check, Act.Raise};

                if (actor.Bet < state.Bet)
                    return new[] {Act.Fold, Act.Call, Act.Raise};
            }

            return new Act[0];
        }
    }
}
