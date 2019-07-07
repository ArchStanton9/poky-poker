using System.Reactive;
using PokyPoker.Domain;
using ReactiveUI;

namespace PokyPoker.Desktop.TestApp.ViewModels
{
    public interface IPlayViewModel
    {
        Play Play { get; }

        string DisplayName { get; }

        ReactiveCommand<Unit, Unit> ActCommand { get; }
    }
}
