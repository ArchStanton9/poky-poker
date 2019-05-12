using System.Reactive;
using PokyPoker.Domain;
using ReactiveUI;

namespace PokyPoker.Desktop.PlayActions
{
    public class PlayViewModel : ReactiveObject, IPlayViewModel
    {
        public PlayViewModel(Play play, GameModel model)
        {
            Play = play;
            ActCommand = ReactiveCommand.Create(() => model.MakeAct(play));
        }

        public Play Play { get; }
        public string DisplayName => Play.ToString();
        public ReactiveCommand<Unit, Unit> ActCommand { get; }
    }
}
