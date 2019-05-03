﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using DynamicData;
using OfflinePoker.Domain;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OfflinePoker.Desktop
{
    public class MainViewModel : ReactiveObject
    {
        private readonly SourceList<Play> optionsSource = new SourceList<Play>();

        public MainViewModel()
        {
            optionsSource.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out options)
                .Subscribe();

            var deck = Deck.CreateStandardDeck();
            var players = new[]
            {
                new Player("p1", deck.Take(2), true, 4000),
                new Player("p2", deck.Take(2), true, 5000),
                new Player("p3", deck.Take(2), true, 2000),
            };

            Game = Game.StartNew(BettingRules.Standard, players, deck.Take(5));
            Players = new ObservableCollection<PlayerViewModel>(
                players.Select(p => new PlayerViewModel(p, this)));

            MakePlayCommand = ReactiveCommand
                .Create<Play>(p =>
                {
                    Game = Game.MakeAct(p, Bet);
                });

            this.WhenAnyValue(v => v.Game)
                .Subscribe(g => Bet = 0);

            this.WhenAnyValue(v => v.Game)
                .Subscribe(g =>
                {
                    optionsSource.Clear();
                    optionsSource.AddRange(g.CurrentRound.GetOptions(g.CurrentRound.InTurn));
                });
        }

        [Reactive]
        public Game Game { get; set; }

        [Reactive]
        public string CurrentPlayer { get; set; }

        [Reactive]
        public int Bet { get; set; }

        public ReadOnlyObservableCollection<Play> Options => options;
        private readonly ReadOnlyObservableCollection<Play> options;

        public ObservableCollection<PlayerViewModel> Players { get; }

        public ReactiveCommand<Play, Unit> MakePlayCommand { get; set; }
    }
}
