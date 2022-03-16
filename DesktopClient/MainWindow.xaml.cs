using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CurrencyRpcTestClient;
using Grpc.Core;

namespace DesktopClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Regex Regex = new("(\\d{1,3} ?){1,3},?\\d*");
        private readonly Currency.CurrencyClient _currencyClient;

        public MainWindow(Currency.CurrencyClient currencyClient)
        {
            _currencyClient = currencyClient;
            InitializeComponent();
        }

        private void UIElement_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);
            e.Handled = !IsTextAllowed(fullText);
        }

        private static bool IsTextAllowed(string text)
        {
            var match = Regex.Match(text);
            return match.Success && match.Value.Length == text.Length;
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Input.Text))
            {
                Output.Text = "Please fill the input field.";
            }

            try
            {
                var response = await _currencyClient.ConvertAsync(
                    new ConvertRequest
                    {
                        NumberInput = Input.Text
                    });
                Output.Text = response.ConvertedOutput;
            }
            catch (RpcException)
            {
                Output.Text = "Server is unavailable. Please check and try again.";
            }
        }
    }
}