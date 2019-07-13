using System.Reactive.Disposables;
using PokyPoker.Desktop.ViewModels;
using ReactiveUI;

namespace PokyPoker.Desktop.Views
{
    /// <summary>
    /// Interaction logic for ChatView.xaml
    /// </summary>
    public partial class ChatView : ReactiveUserControl<ChatViewModel>
    {
        public ChatView()
        {
            InitializeComponent();

            this.WhenActivated(cleanUp =>
            {
                this.OneWayBind(ViewModel, vm => vm.Messages, v => v.MessageList.ItemsSource)
                    .DisposeWith(cleanUp);

                this.Bind(ViewModel, vm => vm.CurrentMessage, v => v.MessageTextBox.Text)
                    .DisposeWith(cleanUp);

                this.BindCommand(ViewModel, vm => vm.SendMessageCommand, v => v.SendButton,
                    vm => vm.CurrentMessage)
                    .DisposeWith(cleanUp);
            });
        }
    }
}
