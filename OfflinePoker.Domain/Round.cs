using System.Collections.Generic;
using System.Linq;
using OfflinePoker.Domain.Exceptions;

namespace OfflinePoker.Domain
{
    public class Round
    {
        private readonly Act[] acts;
        private readonly int playersCount;

        private Round(Act[] acts, int playersCount)
        {
            this.acts = acts;
            this.playersCount = playersCount;
            InTurn = acts.Length == 0 ? 0 : NextActor(acts.Last().Player);
        }

        public int InTurn { get; }
        public IReadOnlyCollection<Act> Acts => acts;
        public int Pot => Acts.Count == 0 ? 0 : Acts.Aggregate(0, (s, a) => s + a.Bet);
        public int MaxBet => Acts.Count == 0 ? 0 : Acts.Max(a => a.Bet);

        public Play LastPlay(int player) => acts
            .Where(a => a.Player == player)
            .Select(a => a.Play)
            .LastOrDefault();

        public int PlayerBet(int player) => acts
            .Where(a => a.Player == player)
            .Aggregate(0, (s, a) => s + a.Bet);

        public bool IsActive(int player) => LastPlay(player) != Play.Fold;

        public IEnumerable<int> ActivePlayers => Enumerable
            .Range(0, playersCount)
            .Where(IsActive);

        public bool HasWinner => ActivePlayers.Count() == 1;

        public static Round CreateInitial(BettingRules rules, int playersCount)
        {
            if (playersCount < 2)
                throw new GameLogicException("Can't start round. Not enough players.");

            var acts = new[]
            {
                new Act(0, Play.Blind, rules.SmallBlind),
                new Act(1, Play.Blind, rules.BigBlind)
            };

            return new Round(acts, playersCount);
        }

        public static Round StartNew(int playersCount) =>
            new Round(new Act[0], playersCount);

        public bool IsComplete => InTurn < 0;

        public Round MakeAct(Play play, int bet = 0) => MakeAct(InTurn, play, bet);

        private Round MakeAct(int player, Play play, int bet)
        {
            if (player != InTurn)
                throw new GameOrderException($"Can not make the act. Current actor is {InTurn}");

            if (!ShouldAct(player))
                throw new GameLogicException("Current actor is not supposed to act.");

            var options = GetOptions(player);
            if (!options.Contains(play))
                throw new GameLogicException($"Actor is not supposed to make '{play}'");

            if (play == Play.Bet)
            {
                if (MaxBet != 0)
                    throw new GameLogicException($"Can not make '{Play.Bet}'. Round is already open.");
            }

            if (play == Play.Raise)
            {
                if (bet <= MaxBet)
                    throw new GameLogicException($"Can not make '{Play.Raise}'. Wager is to low.");
            }

            if (play == Play.Call)
            {
                var playerBet = PlayerBet(player);
                if (playerBet + bet != MaxBet)
                    throw new GameLogicException("Call is not matching.");
            }

            var act = new Act(player, play, bet);

            return new Round(acts.Append(act).ToArray(), playersCount);
        }

        public int NextActor(int lastActor)
        {
            if (lastActor < 0)
                return 0;

            for (var i = lastActor + 1; i < playersCount; i++)
            {
                if (ShouldAct(i)) return i;
            }

            for (var i = 0; i < lastActor + 1; i++)
            {
                if (ShouldAct(i)) return i;
            }

            return -1;
        }

        public bool ShouldAct(int player)
        {
            var lastPlay = LastPlay(player);
            switch (lastPlay)
            {
                case Play.Fold:
                case Play.AllIn:
                    return false;

                case Play.None:
                case Play.Blind:
                    return true;
                
                default:
                {
                    var bet = PlayerBet(player);
                    return bet < MaxBet;
                }
            }
        }

        public Play[] GetOptions(int player)
        {
            var lastPlay = LastPlay(player);
            if (lastPlay != Play.Fold)
            {
                var bet = PlayerBet(player);

                if (lastPlay == Play.Blind)
                {
                    return bet == MaxBet
                        ? new[] {Play.Check, Play.Raise}
                        : new[] {Play.Call, Play.Fold};
                }

                if (MaxBet == 0)
                    return new[] {Play.Bet, Play.Check};

                if (bet == MaxBet)
                    return new[] {Play.Check, Play.Raise};

                if (bet < MaxBet)
                    return new[] {Play.Fold, Play.Call, Play.Raise};
            }

            return new Play[0];
        }

        public RoundState GetPlayerState(int player) =>
            new RoundState
            {
                Bet = PlayerBet(player),
                IsActive = IsActive(player),
                IsCurrent = InTurn == player,
                LastPlay = LastPlay(player)
            };
    }
}
