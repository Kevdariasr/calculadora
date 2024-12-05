using System.IO;

namespace calculadora
{
    public partial class MainPage : ContentPage
    {
        private string _currentEntry = "0";
        private string _operation = "";
        private double _firstNumber;
        private bool _isNewEntry = true;

        private List<string> _history = new List<string>();

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnClearButtonClicked(object sender, EventArgs e)
        {
            _currentEntry = "0";
            _operation = "";
            _firstNumber = 0;
            _isNewEntry = true;
            DisplayLabel.Text = _currentEntry;
        }

        private void OnNumberButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var number = button.Text;

            if (_isNewEntry)
            {
                _currentEntry = number;
                _isNewEntry = false;
            }
            else
            {
                _currentEntry += number;
            }

            DisplayLabel.Text = _currentEntry;
        }

        private void OnOperatorButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            _operation = button.Text;
            _firstNumber = double.Parse(_currentEntry);
            _isNewEntry = true;
        }

        private void OnDecimalButtonClicked(object sender, EventArgs e)
        {
            if (!_currentEntry.Contains("."))
            {
                _currentEntry += ".";
                DisplayLabel.Text = _currentEntry;
            }
        }

        private void OnPlusMinusButtonClicked(object sender, EventArgs e)
        {
            if (_currentEntry.StartsWith("-"))
            {
                _currentEntry = _currentEntry.Substring(1);
            }
            else
            {
                _currentEntry = "-" + _currentEntry;
            }
            DisplayLabel.Text = _currentEntry;
        }

        private void OnEqualButtonClicked(object sender, EventArgs e)
        {
            double secondNumber = double.Parse(_currentEntry);
            double result = 0;

            switch (_operation)
            {
                case "+":
                    result = _firstNumber + secondNumber;
                    break;
                case "-":
                    result = _firstNumber - secondNumber;
                    break;
                case "×":
                    result = _firstNumber * secondNumber;
                    break;
                case "÷":
                    if (secondNumber != 0)
                    {
                        result = _firstNumber / secondNumber;
                    }
                    else
                    {
                        DisplayLabel.Text = "Error";
                        return;
                    }
                    break;
                case "%":
                    result = _firstNumber % secondNumber;
                    break;
            }

            _history.Add($"{_firstNumber} {_operation} {secondNumber} = {result}");
            _currentEntry = result.ToString();
            DisplayLabel.Text = _currentEntry;
            _isNewEntry = true;
        }

        private void CalcularRaizClicked(object sender, EventArgs e)
        {
            // Intentamos convertir _currentEntry a un número
            if (double.TryParse(_currentEntry, out double number))
            {
                // Verificamos si el número es positivo, ya que la raíz cuadrada de un número negativo no es válida en números reales
                if (number >= 0)
                {
                    double result = Math.Sqrt(number);
                    _history.Add($"√{number} = {result}");
                    _currentEntry = result.ToString();
                    DisplayLabel.Text = $"Resultado: {result}";
                }
                else
                {
                    DisplayLabel.Text = "Número no válido (negativo)";
                }
            }
            else
            {
                DisplayLabel.Text = "Ingrese datos válidos";
            }
            // Indica que el próximo número que ingrese será una nueva entrada
            _isNewEntry = true;
        }

        private void OnCalcularPotenciaClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_operation))
            {
                // Si el operador está vacío, significa que este es el primer número (base)
                _firstNumber = double.Parse(_currentEntry);
                _operation = "^";
                _isNewEntry = true;
            }
            else if (_operation == "^")
            {
                // Este es el segundo número (exponente)
                if (double.TryParse(_currentEntry, out double exponent))
                {
                    double result = Math.Pow(_firstNumber, exponent);
                    _history.Add($"{_firstNumber} ^ {exponent} = {result}");
                    _currentEntry = result.ToString();
                    DisplayLabel.Text = $"Resultado: {result}";
                    _operation = "";
                    _isNewEntry = true;
                }
                else
                {
                    DisplayLabel.Text = "Ingrese datos válidos para el exponente";
                }
            }
        }

        private async void OnHistoryButtonClicked(object sender, EventArgs e)
        {
            string historyText = string.Join("\n", _history);
            bool deleteHistory = await DisplayAlert("Historial", historyText, "Borrar Memoria", "Cerrar");

            if (deleteHistory)
            {
                _history.Clear();
                await DisplayAlert("Historial", "Memoria borrada", "Cerrar");
            }
        }

        private async void OnDownloadHistoryClicked(object sender, EventArgs e)
        {
            if (_history.Count == 0)
            {
                await DisplayAlert("Error", "No hay operaciones en la memoria", "Cerrar");
                return;
            }

            string historyText = string.Join("\n", _history);
            string fileName = "historial_calculadora.txt";

            try
            {
                #if ANDROID
                var downloadPath = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath, fileName);

                // Guarda el archivo en la carpeta de Descargas pública de Android
                File.WriteAllText(downloadPath, historyText);
                await DisplayAlert("Éxito", $"Archivo descargado en Descargas: {downloadPath}", "Cerrar");
                #else
                await DisplayAlert("Error", "Descarga solo disponible en Android", "Cerrar");
            #endif
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo crear el archivo: {ex.Message}", "Cerrar");
            }
        }
    }
}
