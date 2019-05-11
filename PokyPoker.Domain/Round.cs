using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.Contracts;
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
        }

        public IReadOnlyCollection<Act> Acts => acts;
        public int Pot => Acts.Count == 0 ? 0 : Acts.Aggregate(0, (s, a) => s + a.Bet);
        public int MaxBet => GetMaxBet(acts);
        
        private static int GetMaxBet(IReadOnlyCollection<Act> acts) => acts.Count == 0
            ? 0
            : acts
                .GroupBy(a => a.Player)
                .Max(g => g.Sum(x => x.Bet));


        [Pure]
        public Play LastPlay(int player) => LastPlay(acts, player);
        
        [Pure]
        private static Play LastPlay(IEnumerable<Act> acts, int player) =>
            acts.Where(a => a.Player == player)
                .Select(a => a.Play)
                .LastOrDefault();


        public int PlayerBet(int player) => PlayerBet(acts, player);
        private static int PlayerBet(IEnumerable<Act> acts, int player) =>
            acts.Where(a => a.Player == player)
                .Aggregate(0, (s, a) => s + a.Bet);

        public bool IsActive(int player) => LastPlay(acts, player) != Play.Fold && LastPlay(acts, player) != Play.AllIn;

        public IEnumerable<int> ActivePlayers => Players.Where(IsActive);

        public bool HasWinner => ActivePlayers.Count() == 1;

        public static Round StartNew(int playersCount) =>
            new Round(new Act[0], playersCount);

        public bool IsComplete => Players.Count() == playersCount && Players.Count(ShouldAct) == 0;

        private IEnumerable<int> Players => acts.Select(a => a.Player).Distinct();

        private bool IsAllowedToAct(int player)
        {
            var players = Players.ToImmutableSortedSet();
            if (players.Contains(player))
            {
                var lastPlay = LastPlay(player);
                if (lastPlay == Play.Fold || lastPlay == Play.AllIn)
                    throw new GameOrderException($"Player is not supposed to make act after '{lastPlay}'");

                foreach (var otherPlayer in players)
                {
                    if (otherPlayer >= player)
                        break;

                    if (ShouldAct(otherPlayer))
                        throw new GameOrderException($"Wrong acts sequence. Expected act from player #{otherPlayer}");
                }
            }
            else
            {
                if (players.Count == playersCount)
                    throw new GameStateException("Can't make act. Players limit for round is exceed");
            }

            return true;
        }


        public Round MakeAct(Act act)
        {
            Debug.Assert(IsAllowedToAct(act.Player));

            switch (act.Play)
            {
                case Play.Bet when MaxBet != 0:
                    throw new GameLogicException($"Can not make '{Play.Bet}'. Round is already open.");

                case Play.Raise when act.Bet <= MaxBet:
                    throw new GameLogicException($"Can not make '{Play.Raise}'. Wager is to low.");

                case Play.Call:
                {
                    var playerBet = PlayerBet(act.Player);
                    if (playerBet + act.Bet != MaxBet)
                        throw new GameLogicException("Call is not matching.");
                    break;
                }
            }

            return new Round(acts.Append(act).ToArray(), playersCount);
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

        public bool HasSubPots => Players
            .Select(LastPlay)
            .Any(p => p == Play.AllIn);

        public int[] SubPots()
        {
            var allInBets = Players
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
                foreach (var player in Players)
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
    }
}
