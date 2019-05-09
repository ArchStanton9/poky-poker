using System.Collections.Generic;
using System.Linq;
using PokyPoker.Domain.Exceptions;

namespace PokyPoker.Domain
{
    public class Round
    {
        private readonly Act[] acts;
        private readonly int playersCount;

        public Round(Act[] acts, int playersCount)
        {
            this.acts = acts;
            this.playersCount = playersCount;
            InTurn = acts.Length == 0 ? 0 : NextActor(acts.Last().Player);
        }

        public int InTurn { get; }
        public IReadOnlyCollection<Act> Acts => acts;
        public int Pot => Acts.Count == 0 ? 0 : Acts.Aggregate(0, (s, a) => s + a.Bet);
        public int MaxBet => GetMaxBet(acts);
        
        private static int GetMaxBet(IReadOnlyCollection<Act> acts) => acts.Count == 0
            ? 0
            : acts
                .GroupBy(a => a.Player)
                .Max(g => g.Sum(x => x.Bet));

        public Play LastPlay(int player) => LastPlay(acts, player);
        private static Play LastPlay(IEnumerable<Act> acts, int player) =>
            acts.Where(a => a.Player == player)
                .Select(a => a.Play)
                .LastOrDefault();


        public int PlayerBet(int player) => PlayerBet(acts, player);
        private static int PlayerBet(IEnumerable<Act> acts, int player) =>
            acts.Where(a => a.Player == player)
                .Aggregate(0, (s, a) => s + a.Bet);

        public bool IsActive(int player) => LastPlay(acts, player) != Play.Fold;

        public IEnumerable<int> ActivePlayers => Enumerable
            .Range(0, playersCount)
            .Where(IsActive);

        public bool HasWinner => ActivePlayers.Count() == 1;

        public static Round StartNew(int playersCount) =>
            new Round(new Act[0], playersCount);

        public bool IsComplete => InTurn < 0;

        public Round MakeAct(Play play, int bet = 0)
        {
            return MakeAct(InTurn, play, bet);
        }

        private Round MakeAct(int player, Play play, int bet)
        {
            if (player != InTurn)
                throw new GameOrderException($"Can not make the act. Current actor is {InTurn}");

            if (!ShouldAct(player))
                throw new GameLogicException("Current actor is not supposed to act.");

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

        public Play[] GetOptions() => GetOptions(InTurn);

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
                        : new[] {Play.Call, Play.Raise, Play.Fold};
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

        public bool HasSubPots => Enumerable
            .Range(0, playersCount)
            .Select(LastPlay)
            .Any(p => p == Play.AllIn);

        public int[] SubPots()
        {
            var allInBets = Enumerable
                .Range(0, playersCount)
                .Where(p => LastPlay(p) == Play.AllIn)
                .Select(PlayerBet)
                .OrderBy(b => b)
                .ToArray();

            if (allInBets.Length == 0)
                return new int[0];

            var bets = acts
                .GroupBy(a => a.Player, a => a.Bet)
                .ToDictionary(a => a.Key, a => a.Sum());

            var queue = new Queue<int>(playersCount);
            var maxAllInBet = 0;
            var pot = 0;
            foreach (var bet in allInBets)
            {
                var x = bet - maxAllInBet;
                maxAllInBet = x;
                foreach (var player in Enumerable.Range(0, playersCount))
                {
                    if (bets[player] <= x)
                    {
                        pot += bets[player];
                        bets[player] = 0;
                    }
                    else
                    {
                        bets[player] -= x;
                        pot += x;
                    }
                }

                queue.Enqueue(pot);
                pot = 0;
            }

            var remain = bets.Sum(b => b.Value);
            if (remain > 0)
                queue.Enqueue(remain);

            return queue.ToArray();
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
