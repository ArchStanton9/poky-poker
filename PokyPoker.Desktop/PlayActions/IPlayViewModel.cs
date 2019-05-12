using System.Reactive;
using PokyPoker.Domain;
using ReactiveUI;

namespace PokyPoker.Desktop.PlayActions
{
    public interface IPlayViewModel
    {
        Play Play { get; }

        string DisplayName { get; }

        ReactiveCommand<Unit, Unit> ActCommand { get; }
    }
}
