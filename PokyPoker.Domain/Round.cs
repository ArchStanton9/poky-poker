using System;
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
        public Player[] Players { get; }
        private readonly Act[] acts;

        public Round(Act[] acts, Player[] players)
        {
            Players = players;
            this.acts = acts;
        }

        public IReadOnlyCollection<Act> Acts => acts;

        
        public int MaxBet => GetMaxBet(acts);
        
        private static int GetMaxBet(IReadOnlyCollection<Act> acts) => acts.Count == 0
            ? 0
            : acts
                .GroupBy(a => a.Player)
                .Max(g => g.Sum(x => x.Bet));


        [Pure]
        public Play LastPlay(Player player) => LastPlay(acts, player);

        [Pure]
        private static Play LastPlay(IEnumerable<Act> acts, Player player) =>
            acts.Where(a => a.Player == player)
                .Select(a => a.Play)
                .LastOrDefault();


        public int PlayerBet(Player player) => PlayerBet(acts, player);

        private static int PlayerBet(IEnumerable<Act> acts, Player player) =>
            acts.Where(a => a.Player == player)
                .Aggregate(0, (s, a) => s + a.Bet);

        public bool IsActive(Player player) => LastPlay(acts, player) != Play.Fold && LastPlay(acts, player) != Play.AllIn;

        public IEnumerable<Player> ActivePlayers => Players.Where(IsActive);

        public static Round StartNew(Player[] players) => new Round(new Act[0], players);

        public bool IsComplete => Players.Count(ShouldAct) == 0;

        private bool VerifyAllowedToAct(Player player)
        {
            var lastPlay = LastPlay(player);
            if (lastPlay == Play.Fold || lastPlay == Play.AllIn)
                throw new GameOrderException($"Player is not supposed to make act after '{lastPlay}'");

            var index = Array.IndexOf(Players, player);
            if (index < 0)
                return false;

            for (var i = index; i < Players.Length; i++)
            {
                var otherPlayer = Players[i];
                if (otherPlayer.Spot >= player.Spot)
                    break;

                if (ShouldAct(otherPlayer))
                    throw new GameOrderException($"Wrong acts sequence. Expected act from player #{otherPlayer}");
            }

            return true;
        }

        public Round MakeAct(Act act)
        {
            Debug.Assert(VerifyAllowedToAct(act.Player));

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

            var players = Players
                .Where(p => p.Spot != act.Player.Spot)
                .Append(act.Player)
                .ToArray();

            return new Round(acts.Append(act).ToArray(), players);
        }


        public bool ShouldAct(Player player)
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

        public Play[] GetOptions(Player player)
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
    }
}
