using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;
using ScottPlot;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GraphingCalculator
{
    public partial class MainWindow : Window
    {
        // This will hold the string the calculator client passes back
        CalculatorClient _client;

        private string _calcResult;

        // SystemSettings updater
        SystemSettings _systemSettings;
        private string _settingsFilePath;

        // Bool to track button click state
        private bool _eqButtonClicked = false;

        // Bool to track Graph View button click state
        private bool _graphViewButtonClicked = false;

        // Bool to track Graph Settings button click state
        private bool _graphSettingsButtonClicked = false;

        // Bool to track System Settings button click state
        private bool _systemSettingsButtonClicked = false;

        // Bool to track Symbols Menu button click state
        private bool _symbolsMenuButtonClicked = false;

        // Queues for our user textbox and calc response
        private Queue<string> _userInputs = new Queue<string>();
        private Queue<string> _calcResponses = new Queue<string>();

        // Ints for button switcher
        int _eqButtonInt = 1;
        int _graphViewButtonInt = 2;
        int _graphSettingsButtonInt = 3;
        int _systemSettingsButtonInt = 4;
        int _symbolsMenuButtonInt = 5;

        public MainWindow()
        {
            InitializeComponent();

            // starting our UI
            _settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
            LoadSysSettings();
            UpdateComboBoxSelection();
            UiStart();

            // Refresh Button Code
            RefreshButton.Content = Char.ConvertFromUtf32(81);
        }

        // This method starts all the UI at start
        private void UiStart()
        {
            // Starting our Backend Client
            _client = new CalculatorClient();
            // Start text input updating
            userInput.PreviewTextInput += userInput_PreviewTextInput;

            // Our hidden comps for our UI
            EquationEditor.Visibility = Visibility.Collapsed;
            GraphView.Visibility = Visibility.Collapsed;
            GraphSettings.Visibility = Visibility.Collapsed;
            SystemSettings.Visibility = Visibility.Collapsed;
            SymbolsMenu.Visibility = Visibility.Collapsed;

            textBorder1.Visibility = Visibility.Collapsed;
            textBorder2.Visibility = Visibility.Collapsed;
            textBorder3.Visibility = Visibility.Collapsed;
            calcTextBox1.Visibility = Visibility.Collapsed;
            calcTextBorder1.Visibility = Visibility.Collapsed;
            calcTextBox2.Visibility = Visibility.Collapsed;
            calcTextBorder2.Visibility = Visibility.Collapsed;
            calcTextBox3.Visibility = Visibility.Collapsed;
            calcTextBorder3.Visibility = Visibility.Collapsed;

            // read our settiings file
            // this only needs to happen at the start
            // of the programming
            LoadHistory();
            ReadGraphSettings();
        }

        // Button state change helper method
        private void ButtonChanger(int button)
        {
            if (button == _eqButtonInt)
            {
                MainContent.Visibility = Visibility.Collapsed;
                GraphSettings.Visibility = Visibility.Collapsed;
                GraphView.Visibility = Visibility.Collapsed;
                SystemSettings.Visibility = Visibility.Collapsed;
                SymbolsMenu.Visibility = Visibility.Collapsed;
                // EquationEditor = Visible and Color = pressed
                EquationEditor.Visibility = Visibility.Visible;
                eqEditorButton.Background = (System.Windows.Media.Brush)eqEditorButton.FindResource("PressedColor");

                _graphViewButtonClicked = false;
                _graphSettingsButtonClicked = false;
                _symbolsMenuButtonClicked = false;
                _systemSettingsButtonClicked = false;
                // Graph View, Graph Settings, System Settings
                SymbolsButton.Background = new BrushConverter().ConvertFromString("#70B493") as Brush;
                graphViewButton.Background = (System.Windows.Media.Brush)graphViewButton.FindResource("NormalColor");
                graphSettingsButton.Background =
                    (System.Windows.Media.Brush)graphSettingsButton.FindResource("NormalColor");
                systemSettingsButton.Background =
                    (System.Windows.Media.Brush)systemSettingsButton.FindResource("NormalColor");
                //UpdateGraphView();
            }
            else if (button == _graphViewButtonInt)
            {
                MainContent.Visibility = Visibility.Collapsed;
                EquationEditor.Visibility = Visibility.Collapsed;
                SystemSettings.Visibility = Visibility.Collapsed;
                GraphSettings.Visibility = Visibility.Collapsed;
                SymbolsMenu.Visibility = Visibility.Collapsed;
                // GraphView = Visible and Color = pressed
                GraphView.Visibility = Visibility.Visible;
                graphViewButton.Background = (System.Windows.Media.Brush)graphViewButton.FindResource("PressedColor");

                _eqButtonClicked = false;
                _systemSettingsButtonClicked = false;
                _graphSettingsButtonClicked = false;
                _symbolsMenuButtonClicked = false;
                // Equation Editor, Graph Settings, System Settings
                SymbolsButton.Background = new BrushConverter().ConvertFromString("#70B493") as Brush;
                eqEditorButton.Background = (System.Windows.Media.Brush)eqEditorButton.FindResource("NormalColor");
                graphSettingsButton.Background =
                    (System.Windows.Media.Brush)graphSettingsButton.FindResource("NormalColor");
                systemSettingsButton.Background =
                    (System.Windows.Media.Brush)systemSettingsButton.FindResource("NormalColor");

                // Refresh the graph
                //UpdateGraphView();
            }
            else if (button == _graphSettingsButtonInt)
            {
                MainContent.Visibility = Visibility.Collapsed;
                EquationEditor.Visibility = Visibility.Collapsed;
                GraphView.Visibility = Visibility.Collapsed;
                SystemSettings.Visibility = Visibility.Collapsed;
                SymbolsMenu.Visibility = Visibility.Collapsed;
                // GraphSettings = Visible and Color = pressed
                GraphSettings.Visibility = Visibility.Visible;
                graphSettingsButton.Background =
                    (System.Windows.Media.Brush)graphSettingsButton.FindResource("PressedColor");

                _eqButtonClicked = false;
                _systemSettingsButtonClicked = false;
                _graphViewButtonClicked = false;
                _symbolsMenuButtonClicked = false;
                // Equation Editor, System Settings, Graph View
                SymbolsButton.Background = new BrushConverter().ConvertFromString("#70B493") as Brush;
                eqEditorButton.Background = (System.Windows.Media.Brush)eqEditorButton.FindResource("NormalColor");
                graphViewButton.Background = (System.Windows.Media.Brush)graphViewButton.FindResource("NormalColor");
                systemSettingsButton.Background =
                    (System.Windows.Media.Brush)systemSettingsButton.FindResource("NormalColor");
                //UpdateGraphView();
            }
            else if (button == _systemSettingsButtonInt)
            {
                MainContent.Visibility = Visibility.Collapsed;
                EquationEditor.Visibility = Visibility.Collapsed;
                GraphView.Visibility = Visibility.Collapsed;
                SymbolsMenu.Visibility = Visibility.Collapsed;
                GraphSettings.Visibility = Visibility.Collapsed;
                // SystemSettings = Visible and Color = pressed
                SystemSettings.Visibility = Visibility.Visible;
                systemSettingsButton.Background =
                    (System.Windows.Media.Brush)systemSettingsButton.FindResource("PressedColor");

                _eqButtonClicked = false;
                _graphSettingsButtonClicked = false;
                _graphViewButtonClicked = false;
                _symbolsMenuButtonClicked = false;
                // Equation Editor, Graph Settings, Graph View
                SymbolsButton.Background = new BrushConverter().ConvertFromString("#70B493") as Brush;
                eqEditorButton.Background = (System.Windows.Media.Brush)eqEditorButton.FindResource("NormalColor");
                graphSettingsButton.Background =
                    (System.Windows.Media.Brush)graphSettingsButton.FindResource("NormalColor");
                graphViewButton.Background = (System.Windows.Media.Brush)graphViewButton.FindResource("NormalColor");
                //UpdateGraphView();
            }
            else if (button == _symbolsMenuButtonInt)
            {
                MainContent.Visibility = Visibility.Collapsed;
                GraphSettings.Visibility = Visibility.Collapsed;
                EquationEditor.Visibility = Visibility.Collapsed;
                GraphView.Visibility = Visibility.Collapsed;
                SystemSettings.Visibility = Visibility.Collapsed;

                SymbolsMenu.Visibility = Visibility.Visible;
                SymbolsButton.Background = new BrushConverter().ConvertFromString("#30B646") as Brush;

                _eqButtonClicked = false;
                _systemSettingsButtonClicked = false;
                _graphSettingsButtonClicked = false;
                _graphViewButtonClicked = false;
                // EqEditor, Graph Settings, Graph View, System Settings
                eqEditorButton.Background = (System.Windows.Media.Brush)eqEditorButton.FindResource("NormalColor");
                graphSettingsButton.Background =
                    (System.Windows.Media.Brush)graphSettingsButton.FindResource("NormalColor");
                graphViewButton.Background = (System.Windows.Media.Brush)graphViewButton.FindResource("NormalColor");
                systemSettingsButton.Background =
                    (System.Windows.Media.Brush)systemSettingsButton.FindResource("NormalColor");
            }
        }

        private void SymbolsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_symbolsMenuButtonClicked)
            {
                ButtonChanger(_symbolsMenuButtonInt);
                _symbolsMenuButtonClicked = true;
            }
            else
            {
                SymbolsMenu.Visibility = Visibility.Collapsed;
                MainContent.Visibility = Visibility.Visible;
                // Set our symbols menu clicked
                SymbolsButton.Background = new BrushConverter().ConvertFromString("#70B493") as Brush;
                _symbolsMenuButtonClicked = false;
            }
        }

        // These two methods handle the color changing when the mouse
        // hovers over the symbols button
        private void SymbolsButton_MouseEnter(object sender, MouseEventArgs e)
        {
            SymbolsButton.Background = new BrushConverter().ConvertFromString("#30B646") as Brush;
        }

        private void SymbolsButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!_symbolsMenuButtonClicked)
            {
                SymbolsButton.Background = new BrushConverter().ConvertFromString("#70B493") as Brush;
            }
        }

        private void EqEditorButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_eqButtonClicked)
            {
                ButtonChanger(_eqButtonInt);
                _eqButtonClicked = true;
            }
            else
            {
                MainContent.Visibility = Visibility.Visible;
                EquationEditor.Visibility = Visibility.Collapsed;
                // Grab the unpressed brush color
                eqEditorButton.Background = (System.Windows.Media.Brush)eqEditorButton.FindResource("NormalColor");
                _eqButtonClicked = false;
            }
        }

        // These two methods handle the color changing when the mouse
        // hovers over the equation editor button
        private void EqEditorButton_MouseEnter(object sender, MouseEventArgs e)
        {
            eqEditorButton.Background = (System.Windows.Media.Brush)eqEditorButton.FindResource("PressedColor");
        }

        private void EqEditorButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!_eqButtonClicked)
            {
                eqEditorButton.Background = (System.Windows.Media.Brush)eqEditorButton.FindResource("NormalColor");
            }
        }

        private void GraphViewButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_graphViewButtonClicked)
            {
                // Switch other button states to off
                ButtonChanger(_graphViewButtonInt);
                _graphViewButtonClicked = true;
            }
            else
            {
                MainContent.Visibility = Visibility.Visible;
                GraphView.Visibility = Visibility.Collapsed;
                graphViewButton.Background = (System.Windows.Media.Brush)graphViewButton.FindResource("NormalColor");
                _graphViewButtonClicked = false;
            }
        }

        // These two methods handle the color changing when the mouse
        // hovers over the graph view button
        private void GraphViewButton_MouseEnter(object sender, MouseEventArgs e)
        {
            graphViewButton.Background = (System.Windows.Media.Brush)graphViewButton.FindResource("PressedColor");
        }

        private void GraphViewButton_MouseLeave(object obj, MouseEventArgs e)
        {
            if (!_graphViewButtonClicked)
            {
                graphViewButton.Background = (System.Windows.Media.Brush)graphViewButton.FindResource("NormalColor");
            }
        }

        private void GraphSettingsButton_Click(Object sender, RoutedEventArgs e)
        {
            if (!_graphSettingsButtonClicked)
            {
                // Switch other button states off
                ButtonChanger(_graphSettingsButtonInt);
                _graphSettingsButtonClicked = true;
            }
            else
            {
                MainContent.Visibility = Visibility.Visible;
                GraphSettings.Visibility = Visibility.Collapsed;
                graphSettingsButton.Background =
                    (System.Windows.Media.Brush)graphSettingsButton.FindResource("NormalColor");
                _graphSettingsButtonClicked = false;
            }
        }

        // These two methods handle the color changing when the mouse
        // hovers over the graph settings button
        private void GraphSettingsButton_MouseEnter(object sender, MouseEventArgs e)
        {
            graphSettingsButton.Background =
                (System.Windows.Media.Brush)graphSettingsButton.FindResource("PressedColor");
        }

        private void GraphSettingsButton_MouseLeave(Object sender, MouseEventArgs e)
        {
            if (!_graphSettingsButtonClicked)
            {
                graphSettingsButton.Background =
                    (System.Windows.Media.Brush)graphSettingsButton.FindResource("NormalColor");
            }
        }

        private void SystemSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_systemSettingsButtonClicked)
            {
                ButtonChanger(_systemSettingsButtonInt);
                _systemSettingsButtonClicked = true;
            }
            else
            {
                MainContent.Visibility = Visibility.Visible;
                SystemSettings.Visibility = Visibility.Collapsed;
                systemSettingsButton.Background =
                    (System.Windows.Media.Brush)systemSettingsButton.FindResource("NormalColor");
                _systemSettingsButtonClicked = false;
            }
        }

        // These two methods handle the color changing when the mouse
        // hovers over the system settings button
        private void SystemSettingsButton_MouseEnter(Object sender, MouseEventArgs e)
        {
            systemSettingsButton.Background =
                (System.Windows.Media.Brush)systemSettingsButton.FindResource("PressedColor");
        }

        private void SystemSettingsButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!_systemSettingsButtonClicked)
            {
                systemSettingsButton.Background =
                    (System.Windows.Media.Brush)systemSettingsButton.FindResource("NormalColor");
            }
        }

        // User can hit enter key or click enter button
        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            TextBoxHelper();
        }

        private void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBoxHelper();
            }
        }

        // This method moves the user input into the display
        private async void TextBoxHelper()
        {
            // Our main user input string
            string userText = userInput.Text;

            // Send the user text to the calculator logic and
            // get the calcultor result
            string calculatorResult = await _client.CalculateAsync(userText);

            // Store the combined strings in our history database
            await _client.StoreStringsAsync(userText + "___" + calculatorResult);

            if (!string.IsNullOrWhiteSpace(calculatorResult))
            {
                // We want to update the response text blocks now
                _calcResponses.Enqueue(calculatorResult);

                // Remove the oldest item from the queue if the queue has more than 3 items
                if (_calcResponses.Count > 3)
                {
                    _calcResponses.Dequeue();
                }

                var responseArray = _calcResponses.ToArray();

                // Debug: Log the contents of the queue
                Console.WriteLine("Current Queue: " + string.Join(", ", responseArray));

                // Update the first text block and border
                calcTextBox1.Text = responseArray.Length > 0 ? responseArray[^1] : string.Empty;
                calcTextBox1.Visibility = calcTextBox1.Text != string.Empty ? Visibility.Visible : Visibility.Collapsed;
                calcTextBorder1.Visibility =
                    calcTextBox1.Text != string.Empty ? Visibility.Visible : Visibility.Collapsed;

                // Update the second text block and border
                calcTextBox2.Text = responseArray.Length > 1 ? responseArray[^2] : string.Empty;
                calcTextBox2.Visibility = calcTextBox2.Text != string.Empty ? Visibility.Visible : Visibility.Collapsed;
                calcTextBorder2.Visibility =
                    calcTextBox2.Text != string.Empty ? Visibility.Visible : Visibility.Collapsed;

                // Update the third text block and border
                calcTextBox3.Text = responseArray.Length > 2 ? responseArray[^3] : string.Empty;
                calcTextBox3.Visibility = calcTextBox3.Text != string.Empty ? Visibility.Visible : Visibility.Collapsed;
                calcTextBorder3.Visibility =
                    calcTextBox3.Text != string.Empty ? Visibility.Visible : Visibility.Collapsed;
            }

            if (!string.IsNullOrWhiteSpace(userText))
            {
                _userInputs.Enqueue(userText);

                // Remove oldest input if more than three
                if (_userInputs.Count > 3)
                {
                    _userInputs.Dequeue();
                }

                // Our inputsArray var for our queue
                var inputsArray = _userInputs.ToArray();
                // Update our text blocks                
                userTextBlock1.Text = inputsArray.Length > 0 ? inputsArray[^1] : string.Empty;
                if (userTextBlock1.Text != string.Empty)
                {
                    textBorder1.Visibility = Visibility.Visible;
                }

                userTextBlock2.Text = inputsArray.Length > 1 ? inputsArray[^2] : string.Empty;
                if (userTextBlock2.Text != string.Empty)
                {
                    textBorder2.Visibility = Visibility.Visible;
                }

                userTextBlock3.Text = inputsArray.Length > 2 ? inputsArray[^3] : string.Empty;
                if (userTextBlock3.Text != string.Empty)
                {
                    textBorder3.Visibility = Visibility.Visible;
                }

                // Clear our text box
                userInput.Clear();
            }
        }

        // Method to load the last three entries and update the text blocks
        private async Task LoadInitialEntriesAsync()
        {
            var (userInputs, calcResponses) = await _client.GetLastThreeEntriesAsync();
            if (userInputs != null && calcResponses != null)
            {
                foreach (var userInput in userInputs)
                {
                    _userInputs.Enqueue(userInput);
                }

                foreach (var calcResponse in calcResponses)
                {
                    _calcResponses.Enqueue(calcResponse);
                }

                UpdateTextBlocks();
            }
        }

        // Method to update the text blocks
        private void UpdateTextBlocks()
        {
            var responseArray = _calcResponses.ToArray();

            // Update the first text block and border
            calcTextBox1.Text = responseArray.Length > 0 ? responseArray[^1] : string.Empty;
            calcTextBox1.Visibility = calcTextBox1.Text != string.Empty ? Visibility.Visible : Visibility.Collapsed;
            calcTextBorder1.Visibility = calcTextBox1.Text != string.Empty ? Visibility.Visible : Visibility.Collapsed;

            // Update the second text block and border
            calcTextBox2.Text = responseArray.Length > 1 ? responseArray[^2] : string.Empty;
            calcTextBox2.Visibility = calcTextBox2.Text != string.Empty ? Visibility.Visible : Visibility.Collapsed;
            calcTextBorder2.Visibility = calcTextBox2.Text != string.Empty ? Visibility.Visible : Visibility.Collapsed;

            // Update the third text block and border
            calcTextBox3.Text = responseArray.Length > 2 ? responseArray[^3] : string.Empty;
            calcTextBox3.Visibility = calcTextBox3.Text != string.Empty ? Visibility.Visible : Visibility.Collapsed;
            calcTextBorder3.Visibility = calcTextBox3.Text != string.Empty ? Visibility.Visible : Visibility.Collapsed;

            var inputsArray = _userInputs.ToArray();

            // Update our text blocks
            userTextBlock1.Text = inputsArray.Length > 0 ? inputsArray[^1] : string.Empty;
            textBorder1.Visibility = userTextBlock1.Text != string.Empty ? Visibility.Visible : Visibility.Collapsed;

            userTextBlock2.Text = inputsArray.Length > 1 ? inputsArray[^2] : string.Empty;
            textBorder2.Visibility = userTextBlock2.Text != string.Empty ? Visibility.Visible : Visibility.Collapsed;

            userTextBlock3.Text = inputsArray.Length > 2 ? inputsArray[^3] : string.Empty;
            textBorder3.Visibility = userTextBlock3.Text != string.Empty ? Visibility.Visible : Visibility.Collapsed;
        }

        // Call loadHistory on first boot
        private async void LoadHistory()
        {
            await LoadInitialEntriesAsync();
        }

        // Our GraphSettings Window Methods        
        // our colors for the buttons
        readonly string _pressed = "#FF8FC7AA";

        readonly string _notPressed = "#FFF0F0F0";

        // our setting bools
        private bool _rectGc;
        private bool _coordOn;
        private bool _labelOn;
        private bool _exprOn;

        private bool _statOn;

        // our color ints 
        private int _gridColor;
        private int _gridBColor;

        private int _sysSetting;

        // our setting strings
        private string _xMin;
        private string _xMax;
        private string _yMin;
        private string _yMax;
        private string _xScale;
        private string _yScale;

        // this method updates the graph from the
        // graphSettings tab based on the GraphSettings.txt
        private ScottPlot.Plottables.Crosshair _myCrosshair;
        private bool _isMousePressed = false;
        private (double X, double Y) _lockedPosition;

        // This method helps convert strings to scottplot functions
        private static Func<double, double> CompileFunction(string functionString)
        {
            var options = ScriptOptions.Default
                .AddReferences(typeof(Math).Assembly)
                .AddImports("System");

            // Ensure the function string is a valid lambda expression
            string lambdaExpression = $"x => {functionString}";

            try
            {
                var script = CSharpScript.Create<Func<double, double>>(lambdaExpression, options);
                var compilation = script.GetCompilation();
                var diagnostics = compilation.GetDiagnostics();

                // Check for compilation errors
                if (diagnostics.Any(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error))
                {
                    var errors = diagnostics.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
                    var errorMessages = string.Join("\n", errors.Select(error => error.GetMessage()));
                    throw new InvalidOperationException($"Compilation errors: \n{errorMessages}");
                }

                var result = script.RunAsync().Result;
                return result.ReturnValue;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to compile function: {ex.Message}");
                throw;
            }
        }

        // METHOD: Converts user strings to valid math expressions
        // PRE: a string must be passed
        // Example: sinx -> Math.Sin(x)
        private static string PreprocessFunction(string func)
        {
            // Remove spaces
            func = func.Replace(" ", "");

            // Handle implicit multiplication (e.g., 2x -> 2*x, 5(x+2) -> 5*(x+2))
            func = Regex.Replace(func, @"(\d)(x)", "$1*x");
            func = Regex.Replace(func, @"(\))(\()", "$1*$2");
            func = Regex.Replace(func, @"(\d)(\()", "$1*(");
            func = Regex.Replace(func, @"(\))(\d)", "$1*$2");

            // Handle exponents (e.g., x^2 -> Math.Pow(x,2))
            func = Regex.Replace(func, @"(\w+)\^(\d+)", "Math.Pow($1,$2)");

            // Handle inverse trig functions before normal trig functions
            func = func.Replace("asinx", "Math.Asin(x");
            func = func.Replace("acosx", "Math.Acos(x");
            func = func.Replace("atanx", "Math.Atan(x");
            func = func.Replace("asin(x", "Math.Asin(x");
            func = func.Replace("acos(x", "Math.Acos(x");
            func = func.Replace("atan(x", "Math.Atan(x");

            // Handle normal trig functions
            func = func.Replace("sinx", "Math.Sin(x");
            func = func.Replace("cosx", "Math.Cos(x");
            func = func.Replace("tanx", "Math.Tan(x");
            func = func.Replace("sqrtx", "Math.Sqrt(x");
            func = func.Replace("logx", "Math.Log(x");
            func = func.Replace("expx", "Math.Exp(x");
            func = func.Replace("powx", "Math.Pow(x");

            // Ensure trigonometric functions and other common functions use Math namespace
            func = func.Replace("sin(x", "Math.Sin(x");
            func = func.Replace("cos(x", "Math.Cos(x");
            func = func.Replace("tan(x", "Math.Tan(x");
            func = func.Replace("sqrt(x", "Math.Sqrt(x");
            func = func.Replace("log(x", "Math.Log(x");
            func = func.Replace("exp(x", "Math.Exp(x");
            func = func.Replace("pow(x", "Math.Pow(x");

            func = func.Replace("Math.AMath.", "Math.A");
            func = func.Replace("AC", "Ac");
            func = func.Replace("AS", "As");
            func = func.Replace("AT", "At");
            // Add closing parenthesis if missing
            func = AddMissingParentheses(func);
            func = func.Replace("(x))", "(x)");

            return func;
        }

        private static string AddMissingParentheses(string func)
        {
            string[] functions =
            {
                "Math.Asin(", "Math.Acos(", "Math.Atan(", "Math.Sin(", "Math.Cos(", "Math.Tan(", "Math.Sqrt(",
                "Math.Log(", "Math.Exp(", "Math.Pow("
            };

            Stack<int> parenStack = new Stack<int>();
            HashSet<int> insertPositions = new HashSet<int>();

            for (int i = 0; i < func.Length; i++)
            {
                if (func[i] == '(')
                {
                    parenStack.Push(i);
                }
                else if (func[i] == ')')
                {
                    if (parenStack.Count > 0)
                    {
                        parenStack.Pop();
                    }
                    else
                    {
                        // Unbalanced closing parenthesis, ignore it
                        insertPositions.Add(i);
                    }
                }
            }

            // For each unclosed parenthesis, add a closing one at the end
            while (parenStack.Count > 0)
            {
                int pos = parenStack.Pop();
                insertPositions.Add(pos);
            }

            // Reconstruct the string with inserted parentheses
            StringBuilder sb = new StringBuilder(func);
            foreach (int pos in insertPositions.OrderByDescending(p => p))
            {
                sb.Insert(pos + 2, ")");
            }

            return sb.ToString();
        }


        private void UpdateGraphView()
        {
            // Update graph color and background color
            // from user settings
            UpdateGraphColor();
            // Clear old plot
            RectGC_Plot.Plot.Clear();
            // Add our x-axis and y-axis, and make them black
            var vl1 = RectGC_Plot.Plot.Add.VerticalLine(0);
            var hl1 = RectGC_Plot.Plot.Add.HorizontalLine(0);
            vl1.Color = ScottPlot.Colors.Black;
            hl1.Color = ScottPlot.Colors.Black;

            // Setting up Graph Crosshair
            _myCrosshair = RectGC_Plot.Plot.Add.Crosshair(0, 0);
            // Change cross hair to a circle and make invisible
            _myCrosshair.MarkerShape = MarkerShape.OpenCircle;
            _myCrosshair.IsVisible = false;
            // Hiding vertical/horizontal line on marker
            _myCrosshair.VerticalLine.LineWidth = 0;
            _myCrosshair.HorizontalLine.LineWidth = 0;
            // Updating our marker size
            _myCrosshair.MarkerSize = 5;

            // Subscribe to mouse events
            RectGC_Plot.MouseMove += RectGC_Plot_MouseMove;
            RectGC_Plot.MouseDown += RectGC_Plot_MouseDown;
            RectGC_Plot.MouseUp += RectGC_Plot_MouseUp;

            // Polar or Cart based on user choice
            if (_rectGc)
            {
                SetupCartesianPlot();
                if (_statOn)
                {
                    SetupUserScatterPlot();
                }
            }
            else
            {
                SetupPolarScatterPlot();
            }

            // Legen Setup and allowing user to choose to hide 
            RectGC_Plot.Plot.Legend.FontName = "Arial Rounded MT";
            RectGC_Plot.Plot.Legend.FontSize = 30;
            if (_exprOn)
            {
                RectGC_Plot.Plot.ShowLegend();
            }
            else
            {
                RectGC_Plot.Plot.HideLegend();
            }

            // user can hide axis labels, and it will update
            // depending on polar or cartesian
            if (_labelOn)
            {
                if (!_rectGc)
                {
                    RectGC_Plot.Plot.XLabel("r*cos(theta)");
                    RectGC_Plot.Plot.YLabel("r*sin(theta)");
                }
                else
                {
                    RectGC_Plot.Plot.XLabel("X");
                    RectGC_Plot.Plot.YLabel("Y");
                }
            }
            else
            {
                RectGC_Plot.Plot.XLabel("");
                RectGC_Plot.Plot.YLabel("");
            }

            // set the axis limits from user settings and refresh
            RectGC_Plot.Plot.Axes.SetLimits(int.Parse(_xMin), int.Parse(_xMax), int.Parse(_yMin), int.Parse(_yMax));
            RectGC_Plot.Refresh();
        }

        // Helper method that setups up the user provided data
        // in a scatter plot. If _scat = true then polar must
        // be off
        // Helper method that setups up the user provided data
        // in a scatter plot. If _scat = true then polar must be off
        double[] _userXs;
        double[] _userYs;

        private void SetupUserScatterPlot()
        {
            if (_xValues != "{}" && _yValues != "{}")
            {
                _userXs = ConvertToDoubleArray(_xValues.Replace("{", "").Replace("}", ""));
                _userYs = ConvertToDoubleArray(_yValues.Replace("{", "").Replace("}", ""));

                var sp1 = RectGC_Plot.Plot.Add.ScatterPoints(_userXs, _userYs);
                sp1.MarkerSize = 8;

                // Corrected the condition
                if (_userXs.Length >= 2 && _userYs.Length >= 2)
                {
                    // Making line of best fit
                    ScottPlot.Statistics.LinearRegression reg = new(_userXs, _userYs);
                    Coordinates pt1 = new(_userXs.First(), reg.GetValue(_userXs.First()));
                    Coordinates pt2 = new(_userXs.Last(), reg.GetValue(_userXs.Last()));

                    // Add the line
                    var line = RectGC_Plot.Plot.Add.Line(pt1, pt2);
                    line.MarkerSize = 0;
                    line.LineWidth = 3;
                    line.LinePattern = LinePattern.Dashed;

                    // Add formula to the legend
                    sp1.LegendText = reg.FormulaWithRSquared;
                }

                // Ensure the plot is refreshed
                RectGC_Plot.Refresh();
            }
        }

        // Converts {values} in string format to double[]
        private double[] ConvertToDoubleArray(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                Log.Error("Input string is null or empty.");
                return new double[0]; // Return an empty array or handle it appropriately
            }

            try
            {
                return input.Split(',')
                    .Select(str =>
                    {
                        if (string.IsNullOrWhiteSpace(str))
                        {
                            Log.Warning("Encountered empty string in input.");
                            return 0.0; // Return a default value or handle it appropriately
                        }

                        return double.Parse(str);
                    })
                    .ToArray();
            }
            catch (FormatException ex)
            {
                Log.Error(ex, "Error converting input string to double array.");
                throw; // Re-throw the exception or handle it appropriately
            }
        }


        // Helper method that setups up a scatter plot for our 
        // polar -> cartesian points
        private void SetupPolarScatterPlot()
        {
            // Change pointCount for more accurate plotting
            int pointCount = 1500;
            double[] thetas = Enumerable.Range(0, pointCount).Select(i => i * 2 * Math.PI / (pointCount - 1)).ToArray();
            // make sure each box isn't empty first
            if (_eq1Box != "")
            {
                double[] rs1 = ConvertToPolar(_eq1Box, thetas);
                var (xs, ys) = ConvertPolarToCartesian(thetas, rs1);
                var scatter1 = RectGC_Plot.Plot.Add.Scatter(xs, ys);
                scatter1.LineWidth = 2;
                scatter1.LegendText = LegendFormat(_eq1Box);
            }

            if (_eq2Box != "")
            {
                double[] rs2 = ConvertToPolar(_eq2Box, thetas);
                var (xs, ys) = ConvertPolarToCartesian(thetas, rs2);
                var scatter2 = RectGC_Plot.Plot.Add.Scatter(xs, ys);
                scatter2.LineWidth = 2;
                scatter2.LegendText = LegendFormat(_eq2Box);
            }

            if (_eq3Box != "")
            {
                double[] rs3 = ConvertToPolar(_eq3Box, thetas);
                var (xs, ys) = ConvertPolarToCartesian(thetas, rs3);
                var scatter3 = RectGC_Plot.Plot.Add.Scatter(xs, ys);
                scatter3.LineWidth = 2;
                scatter3.LegendText = LegendFormat(_eq3Box);
            }
        }

        // Two temp methods to format the graph legend
        // for testing
        private static string LegendFormat(string s)
        {
            s = s.Replace("*", "");
            return s;
        }

        // Helper method that converts polar coordinates to cartesian
        private (double[] xs, double[] ys) ConvertPolarToCartesian(double[] thetas, double[] rs)
        {
            double[] xs = new double[thetas.Length];
            double[] ys = new double[thetas.Length];
            for (int i = 0; i < thetas.Length; i++)
            {
                xs[i] = rs[i] * Math.Cos(thetas[i]);
                ys[i] = rs[i] * Math.Sin(thetas[i]);
            }

            return (xs, ys);
        }

        // Helper method that sets up our cartesian plot and and width of lines
        private void SetupCartesianPlot()
        {
            if (!string.IsNullOrEmpty(_eq1Box))
            {
                try
                {
                    string preprocessedFunction = PreprocessFunction(_eq1Box);
                    Func<double, double> my1Function = CompileFunction(preprocessedFunction);
                    var f1 = RectGC_Plot.Plot.Add.Function(my1Function);
                    f1.LineWidth = 3;
                    f1.LegendText = LegendFormat(_eq1Box);
                }
                catch (Exception)
                {
                    MessageBox.Show("Invalid function in Equation 1!", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    Equation1Box.Text = ""; // Clear the invalid equation
                }
            }

            if (!string.IsNullOrEmpty(_eq2Box))
            {
                try
                {
                    string preprocessedFunction = PreprocessFunction(_eq2Box);
                    Func<double, double> my2Function = CompileFunction(preprocessedFunction);
                    var f2 = RectGC_Plot.Plot.Add.Function(my2Function);
                    f2.LineWidth = 3;
                    f2.LegendText = LegendFormat(_eq2Box);
                }
                catch (Exception)
                {
                    MessageBox.Show("Invalid function in Equation 2!", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    Equation2Box.Text = ""; // Clear the invalid equation
                }
            }

            if (!string.IsNullOrEmpty(_eq3Box))
            {
                try
                {
                    string preprocessedFunction = PreprocessFunction(_eq3Box);
                    Func<double, double> my3Function = CompileFunction(preprocessedFunction);
                    var f3 = RectGC_Plot.Plot.Add.Function(my3Function);
                    f3.LineWidth = 3;
                    f3.LegendText = LegendFormat(_eq3Box);
                }
                catch (Exception)
                {
                    MessageBox.Show("Invalid function in Equation 3!", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    Equation3Box.Text = ""; // Clear the invalid equation
                }
            }
        }

        private static double[] ConvertToPolar(string functionString, double[] thetas)
        {
            Func<double, double> myFunction = CompileFunction(PreprocessFunction(functionString));
            double[] rs = new double[thetas.Length];
            for (int i = 0; i < thetas.Length; i++)
            {
                double theta = thetas[i];
                double x = Math.Cos(theta);
                double y = myFunction(x);
                rs[i] = Math.Sqrt(x * x + y * y);
            }

            return rs;
        }

        // Method that updates graph based on user selection 
        private void UpdateGraphColor()
        {
            //RectGC_Plot.Plot.
            if (0 == _gridColor)
            {
                RectGC_Plot.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#66A8DE").WithOpacity(1);
            }
            else if (1 == _gridColor)
            {
                RectGC_Plot.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#70B493").WithOpacity(1);
            }
            else if (2 == _gridColor)
            {
                RectGC_Plot.Plot.Grid.MajorLineColor = ScottPlot.Colors.White;
            }
            else if (3 == _gridColor)
            {
                RectGC_Plot.Plot.Grid.MajorLineColor = ScottPlot.Colors.Gray.WithOpacity(.8);
            }

            // Updating Background
            if (0 == _gridBColor)
            {
                RectGC_Plot.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#66A8DE").WithOpacity(0.8);
            }
            else if (1 == _gridBColor)
            {
                RectGC_Plot.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#70B493").WithOpacity(0.8);
            }
            else if (2 == _gridBColor)
            {
                RectGC_Plot.Plot.DataBackground.Color = ScottPlot.Colors.White;
            }
            else if (3 == _gridBColor)
            {
                RectGC_Plot.Plot.DataBackground.Color = ScottPlot.Colors.Gray.WithOpacity(0.8);
            }
        }

        // Methods to handle user mouse interaction in the graph
        private void RectGC_Plot_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isMousePressed)
            {
                UpdateCrosshairPosition(e);
            }
        }

        private void RectGC_Plot_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _isMousePressed = true;
                var mousePosition = e.GetPosition(RectGC_Plot);
                var pixel = new ScottPlot.Pixel(mousePosition.X * RectGC_Plot.DisplayScale,
                    mousePosition.Y * RectGC_Plot.DisplayScale);
                var coordinates = RectGC_Plot.Plot.GetCoordinates(pixel);
                _lockedPosition = (coordinates.X, coordinates.Y);
                UpdateCrosshairPosition(e);
            }
        }

        private void RectGC_Plot_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                _isMousePressed = false;
                UpdateCrosshairPosition(e);
            }
        }

        // Alternative cursor so user can see coordinates on our graph
        private void UpdateCrosshairPosition(MouseEventArgs e)
        {
            var mousePosition = e.GetPosition(RectGC_Plot);
            double mouseX, mouseY;

            if (_isMousePressed)
            {
                // Use the locked position
                mouseX = _lockedPosition.X;
                mouseY = _lockedPosition.Y;
            }
            else
            {
                // Update normally
                var pixel = new ScottPlot.Pixel(mousePosition.X * RectGC_Plot.DisplayScale,
                    mousePosition.Y * RectGC_Plot.DisplayScale);
                var coordinates = RectGC_Plot.Plot.GetCoordinates(pixel);
                mouseX = coordinates.X;
                mouseY = coordinates.Y;
            }

            // Update crosshair position
            _myCrosshair.IsVisible = true;
            _myCrosshair.X = mouseX;
            _myCrosshair.Y = mouseY;
            RectGC_Plot.Refresh();


            // Optionally update the plot title with coordinates
            if (_coordOn)
            {
                RectGC_Plot.Plot.Title($"X={mouseX:0.##}, Y={mouseY:0.##}", size: 15);
            }
            else
            {
                RectGC_Plot.Plot.Title("");
            }
        }


        // This method updates the graphSettings button colors
        // when users click
        private void ColorUpdate()
        {
            // updating RectGC color
            if (_rectGc)
            {
                RectGC.Background = new BrushConverter().ConvertFromString(_pressed) as Brush;
                PolarGC.Background = new BrushConverter().ConvertFromString(_notPressed) as Brush;
            }
            else
            {
                RectGC.Background = new BrushConverter().ConvertFromString(_notPressed) as Brush;
                PolarGC.Background = new BrushConverter().ConvertFromString(_pressed) as Brush;
            }

            // updating CoordOn
            if (_coordOn)
            {
                CoordOn.Background = new BrushConverter().ConvertFromString(_pressed) as Brush;
                CoordOff.Background = new BrushConverter().ConvertFromString(_notPressed) as Brush;
            }
            else
            {
                CoordOn.Background = new BrushConverter().ConvertFromString(_notPressed) as Brush;
                CoordOff.Background = new BrushConverter().ConvertFromString(_pressed) as Brush;
            }

            // updating LabelOn
            if (_labelOn)
            {
                LabelOn.Background = new BrushConverter().ConvertFromString(_pressed) as Brush;
                LabelOff.Background = new BrushConverter().ConvertFromString(_notPressed) as Brush;
            }
            else
            {
                LabelOn.Background = new BrushConverter().ConvertFromString(_notPressed) as Brush;
                LabelOff.Background = new BrushConverter().ConvertFromString(_pressed) as Brush;
            }

            // updating ExprOn
            if (_exprOn)
            {
                ExprOn.Background = new BrushConverter().ConvertFromString(_pressed) as Brush;
                ExprOff.Background = new BrushConverter().ConvertFromString(_notPressed) as Brush;
            }
            else
            {
                ExprOn.Background = new BrushConverter().ConvertFromString(_notPressed) as Brush;
                ExprOff.Background = new BrushConverter().ConvertFromString(_pressed) as Brush;
            }

            // updating StatOn
            if (_statOn)
            {
                StatOn.Background = new BrushConverter().ConvertFromString(_pressed) as Brush;
                StatOff.Background = new BrushConverter().ConvertFromString(_notPressed) as Brush;
            }
            else
            {
                StatOn.Background = new BrushConverter().ConvertFromString(_notPressed) as Brush;
                StatOff.Background = new BrushConverter().ConvertFromString(_pressed) as Brush;
            }
        }

        // This method updates the combo boxes from the
        // saved settings
        private void UpdateComboBoxes()
        {
            GridColorComboBox.SelectedIndex = _gridColor;
            GridBackgroundColorComboBox.SelectedIndex = _gridBColor;
            CalculateComboBox.SelectedIndex = _calcSel;
        }

        // this method reads the graph settings file and creates a new
        // file if the file is detected as missing
        // and updates the variables stored
        string _defaultSettings = @"RectGC=True
                                CoordOn=True
                                LabelOn=True
                                ExprOn=True
                                StatOn=True
                                GridColor=3
                                CalcSel=3
                                GridBackground=2
                                Xmin=-10
                                Xmax=10
                                Ymin=-10
                                Ymax=10
                                Xscale=1
                                Yscale=1
                                EQ1Box=
                                EQ2Box=
                                EQ3Box=
                                Xbox=
                                Ybox=
                                ";

        // our file path        
        string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GraphSettings.txt");

        private void ReadGraphSettings()
        {
            // Check if the file exists at the specified path
            if (!File.Exists(_filePath))
            {
                // If the file doesn't exist, create it and write the default settings
                File.WriteAllText(_filePath, _defaultSettings);
                Console.WriteLine($"{_filePath} created with default settings.");
            }
            else
            {
                Console.WriteLine($"{_filePath} already exists.");
            }

            foreach (var line in File.ReadLines(_filePath))
            {
                // Split the line into key and value
                var parts = line.Split('=');
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();

                    // Update the variables based on the key
                    switch (key)
                    {
                        case "RectGC":
                            _rectGc = bool.Parse(value);
                            break;
                        case "CoordOn":
                            _coordOn = bool.Parse(value);
                            break;
                        case "LabelOn":
                            _labelOn = bool.Parse(value);
                            break;
                        case "ExprOn":
                            _exprOn = bool.Parse(value);
                            break;
                        case "GridColor":
                            _gridColor = int.Parse(value);
                            break;
                        case "GridBackground":
                            _gridBColor = int.Parse(value);
                            break;
                        case "StatOn":
                            _statOn = bool.Parse(value);
                            break;
                        case "Xmin":
                            _xMin = value;
                            break;
                        case "Xmax":
                            _xMax = value;
                            break;
                        case "Ymin":
                            _yMin = value;
                            break;
                        case "Ymax":
                            _yMax = value;
                            break;
                        case "Xscale":
                            _xScale = value;
                            break;
                        case "Yscale":
                            _yScale = value;
                            break;
                        case "EQ1Box":
                            _eq1Box = value;
                            break;
                        case "EQ2Box":
                            _eq2Box = value;
                            break;
                        case "EQ3Box":
                            _eq3Box = value;
                            break;
                        case "Xbox":
                            _xValues = value;
                            break;
                        case "Ybox":
                            _yValues = value;
                            break;
                        case "CalcSel":
                            _calcSel = int.Parse(value);
                            break;
                    }
                }
            }

            // update settings
            ColorUpdate();
            UpdateComboBoxes();
            UpdateGraphSettingsTextBoxes();
            UpdateEqBoxes();
            UpdateGraphView();
        }

        // We want to write to the settings everytime we update a setting
        // then read the settings and update the buttons. This allows
        // us to have the settings saved on user exit
        private void WriteGraphSettings()
        {
            using (var writer = new StreamWriter(_filePath))
            {
                writer.WriteLine($"RectGC={_rectGc}");
                writer.WriteLine($"CoordOn={_coordOn}");
                writer.WriteLine($"LabelOn={_labelOn}");
                writer.WriteLine($"ExprOn={_exprOn}");
                writer.WriteLine($"StatOn={_statOn}");
                writer.WriteLine($"GridColor={_gridColor}");
                writer.WriteLine($"CalcSel={_calcSel}");
                writer.WriteLine($"GridBackground={_gridBColor}");
                writer.WriteLine($"Xmin={_xMin}");
                writer.WriteLine($"Xmax={_xMax}");
                writer.WriteLine($"Ymin={_yMin}");
                writer.WriteLine($"Ymax={_yMax}");
                writer.WriteLine($"Xscale={_xScale}");
                writer.WriteLine($"Yscale={_yScale}");
                writer.WriteLine($"EQ1Box={_eq1Box}");
                writer.WriteLine($"EQ2Box={_eq2Box}");
                writer.WriteLine($"EQ3Box={_eq3Box}");
                writer.WriteLine($"Xbox={_xValues}");
                writer.WriteLine($"Ybox={_yValues}");
            }

            ColorUpdate();
            UpdateEqBoxes();
            //updateGraphView();
        }

        // RectGC and PolarGC buttons
        private void RectGC_Click(object sender, RoutedEventArgs e)
        {
            if (!_rectGc)
            {
                _rectGc = !_rectGc;
                WriteGraphSettings();
            }
        }

        private void PolarGC_Click(object sender, RoutedEventArgs e)
        {
            if (_rectGc)
            {
                _rectGc = !_rectGc;
                _statOn = false;
                WriteGraphSettings();
            }
        }

        // CoordOn and CoordOff Buttons
        private void CoordOn_Click(object sender, RoutedEventArgs e)
        {
            if (!_coordOn)
            {
                _coordOn = !_coordOn;
                WriteGraphSettings();
            }
        }

        private void CoordOff_Click(object sender, RoutedEventArgs e)
        {
            if (_coordOn)
            {
                _coordOn = !_coordOn;
                WriteGraphSettings();
            }
        }

        // LabelOn and LabelOff Buttons
        private void LabelOn_Click(object sender, RoutedEventArgs e)
        {
            if (!_labelOn)
            {
                _labelOn = !_labelOn;
                WriteGraphSettings();
            }
        }

        private void LabelOff_Click(object sender, RoutedEventArgs e)
        {
            if (_labelOn)
            {
                _labelOn = !_labelOn;
                WriteGraphSettings();
            }
        }

        // ExprOn and ExprOff Buttons
        private void ExprOn_Click(object sender, RoutedEventArgs e)
        {
            if (!_exprOn)
            {
                _exprOn = !_exprOn;
                WriteGraphSettings();
            }
        }

        private void ExprOff_Click(object sender, RoutedEventArgs e)
        {
            if (_exprOn)
            {
                _exprOn = !_exprOn;
                WriteGraphSettings();
            }
        }

        // StatOn and StatOff Buttons
        private void StatOn_Click(object sender, RoutedEventArgs e)
        {
            if (!_statOn)
            {
                if (_rectGc)
                {
                    _statOn = !_statOn;
                    WriteGraphSettings();
                }
            }
        }

        private void StatOff_Click(object sender, RoutedEventArgs e)
        {
            if (_statOn)
            {
                _statOn = !_statOn;
                WriteGraphSettings();
            }
        }

        // This method checks if the user selects
        // a different grid color
        private void GridColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _gridColor = GridColorComboBox.SelectedIndex;
            WriteGraphSettings();
        }

        // This method checks if the user selects
        // a different grid background color
        private void GridBackgroundColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _gridBColor = GridBackgroundColorComboBox.SelectedIndex;
            WriteGraphSettings();
        }

        // This attempts to load our system settings from settings.json
        // if no file is found it creates a new settings.json with default
        // settings
        private void LoadSysSettings()
        {
            try
            {
                using (StreamReader reader = new StreamReader(_settingsFilePath))
                {
                    string json = reader.ReadToEnd();
                    _systemSettings = JsonConvert.DeserializeObject<SystemSettings>(json);
                }
            }
            catch (FileNotFoundException)
            {
                InitializeDefaultSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to read settings: {ex.Message}", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                InitializeDefaultSettings();
            }
        }

        // These are the default settings that will be saved
        // if no file is found -> will create a new file
        private void InitializeDefaultSettings()
        {
            _systemSettings = new SystemSettings
            {
                Decimals = 5,
                AngleUnit = "degrees",
                ScientificNotation = false,
                ExactFraction = false,
                AutoDecimals = false
            };
        }

        // Method attempts to save settings to settings.json
        // Catches any error
        private void SaveSysSettings()
        {
            try
            {
                string json = JsonConvert.SerializeObject(_systemSettings, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(_settingsFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save settings: {ex.Message}", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            UpdateComboBoxSelection();
        }

        // Updates all our combo boxes on the system settings page
        private void UpdateComboBoxSelection()
        {
            if (_systemSettings == null) return;

            if (_systemSettings.Decimals >= 0 && _systemSettings.Decimals <= 6)
            {
                SystemSettingsComboBox.SelectedIndex = _systemSettings.Decimals;
            }

            // Radians + Degrees Combo Box
            if (_systemSettings.AngleUnit == "degrees")
            {
                RadiansDegrees.SelectedIndex = 0;
            }
            else if (_systemSettings.AngleUnit == "radians")
            {
                RadiansDegrees.SelectedIndex = 1;
            }

            if (_systemSettings.ScientificNotation)
            {
                AnswerStyle.SelectedIndex = 3;
            }
            else if (_systemSettings.AutoDecimals)
            {
                AnswerStyle.SelectedIndex = 2;
            }
            else if (_systemSettings.ExactFraction)
            {
                AnswerStyle.SelectedIndex = 1;
            }
            else
            {
                AnswerStyle.SelectedIndex = 0;
            }
        }

        // Method lets the user control the calculator responding
        // in radians and degrees
        private void RadiansDegrees_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = RadiansDegrees.SelectedIndex;
            switch (selectedIndex)
            {
                case 0:
                    _systemSettings.AngleUnit = "degrees";
                    break;
                case 1:
                    _systemSettings.AngleUnit = "radians";
                    break;
            }

            SaveSysSettings();
        }

        // Method controls the style of the calculator response
        private void AnswerStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = AnswerStyle.SelectedIndex;
            switch (selectedIndex)
            {
                case 0:
                    _systemSettings.ExactFraction = false;
                    _systemSettings.AutoDecimals = false;
                    _systemSettings.ScientificNotation = false;
                    break;
                case 1:
                    _systemSettings.ExactFraction = true;
                    _systemSettings.AutoDecimals = false;
                    _systemSettings.ScientificNotation = false;
                    break;
                case 2:
                    _systemSettings.AutoDecimals = true;
                    _systemSettings.ScientificNotation = false;
                    _systemSettings.ExactFraction = false;
                    break;
                case 3:
                    _systemSettings.ScientificNotation = true;
                    _systemSettings.AutoDecimals = false;
                    _systemSettings.ExactFraction = false;
                    break;
            }

            SaveSysSettings();
        }

        // Method controls the system settings combo box
        private void SystemSettingsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_systemSettings == null)
            {
                _systemSettings = new SystemSettings();
            }

            int selectedIndex = SystemSettingsComboBox.SelectedIndex;
            switch (selectedIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    _systemSettings.Decimals = selectedIndex;
                    AnswerStyle.SelectedIndex = 0;
                    break;
                default:
                    break;
            }

            SaveSysSettings();
        }

        // this method updates the settings file when
        // the user changes graph settings
        private bool _isUpdating = false;

        private void GraphSettingsTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdating) return;
            _xMin = Xmin.Text;
            _xMax = Xmax.Text;
            _yMin = Ymin.Text;
            _yMax = Ymax.Text;
            _xScale = Xscale.Text;
            _yScale = Yscale.Text;
            WriteGraphSettings();
        }

        private void UpdateGraphSettingsTextBoxes()
        {
            _isUpdating = true;

            Xmin.Text = _xMin;
            Xmax.Text = _xMax;
            Ymin.Text = _yMin;
            Ymax.Text = _yMax;
            Xscale.Text = _xScale;
            Yscale.Text = _yScale;
            UserXBox.Text = _xValues.Replace("{", "").Replace("}", "");
            UserYBox.Text = _yValues.Replace("{", "").Replace("}", "");
            // flag is done
            _isUpdating = false;
        }

        // The following four buttons are for the calculate
        // column in graph settings        
        private void MinimumButton_Click(object sender, RoutedEventArgs e)
        {
            string equationS = "";
            bool isSelected = false;
            if (_calcSel == 0)
            {
                equationS = Equation1Box.Text;
                isSelected = !isSelected;
            }
            else if (_calcSel == 1)
            {
                equationS = Equation2Box.Text;
                isSelected = !isSelected;
            }
            else if (_calcSel == 2)
            {
                equationS = Equation3Box.Text;
                isSelected = !isSelected;
            }

            // Make sure selected equation is not empty  
            if (isSelected && equationS != "")
            {
                equationS = PreprocessFunction(equationS);
                var options = ScriptOptions.Default
                    .AddReferences(typeof(Math).Assembly)
                    .AddImports("System");

                // Ensure the function string is a valid lambda expression
                string lambdaExpression = $"x => {equationS}";

                var script = CSharpScript.Create<Func<double, double>>(lambdaExpression, options);
                var compilation = script.GetCompilation();
                var diagnostics = compilation.GetDiagnostics();

                var result = script.RunAsync().Result;
                Func<double, double> equation = result.ReturnValue;

                // Find the minimum value of the equation
                double xMin;
                double.TryParse(_xMin, out xMin);
                double xMax;
                double.TryParse(_xMax, out xMax);
                ;
                double step = 0.005;
                double minValue = double.MaxValue;
                double minX = xMin;

                for (double x = xMin; x <= xMax; x += step)
                {
                    double y = equation(x);
                    if (y < minValue)
                    {
                        minValue = y;
                        minX = x;
                    }
                }

                minValue = Math.Round(minValue, 2);
                // Add a horizontal line at the minimum value
                var hLine = RectGC_Plot.Plot.Add.HorizontalLine(minValue);
                if (minX == -0) minX = 0;
                hLine.IsVisible = true;
                hLine.Text = $"Min Value = {minValue}";
                hLine.LinePattern = LinePattern.DenselyDashed;

                // Refresh the plot
                RectGC_Plot.Refresh();
            }
        }

        private void MaximumButton_Click(object sender, RoutedEventArgs e)
        {
            string equationS = "";
            bool isSelected = false;
            if (_calcSel == 0)
            {
                equationS = Equation1Box.Text;
                isSelected = true;
            }
            else if (_calcSel == 1)
            {
                equationS = Equation2Box.Text;
                isSelected = true;
            }
            else if (_calcSel == 2)
            {
                equationS = Equation3Box.Text;
                isSelected = true;
            }

            // Make sure selected equation is not empty  
            if (isSelected && equationS != "")
            {
                equationS = PreprocessFunction(equationS);
                var options = ScriptOptions.Default
                    .AddReferences(typeof(Math).Assembly)
                    .AddImports("System");

                // Ensure the function string is a valid lambda expression
                string lambdaExpression = $"x => {equationS}";

                var script = CSharpScript.Create<Func<double, double>>(lambdaExpression, options);
                var result = script.RunAsync().Result;
                Func<double, double> equation = result.ReturnValue;

                // Find the maximum value of the equation
                double xMin;
                double.TryParse(_xMin, out xMin);
                double xMax;
                double.TryParse(_xMax, out xMax);
                double step = 0.001; // Increased step resolution
                double maxValue = double.MinValue;
                double maxX = xMin;

                for (double x = xMin; x <= xMax; x += step)
                {
                    double y = equation(x);
                    if (y > maxValue)
                    {
                        maxValue = y;
                        maxX = x;
                    }
                }

                maxValue = Math.Round(maxValue, 2);
                // Add a horizontal line at the maximum value
                var hLine = RectGC_Plot.Plot.Add.HorizontalLine(maxValue);
                if (maxX == -0) maxX = 0;
                hLine.IsVisible = true;
                hLine.Text = $"Max Value = {maxValue}";
                hLine.LinePattern = LinePattern.DenselyDashed;

                // Refresh the plot
                RectGC_Plot.Refresh();
            }
        }

        // This method calculates the intercepts of the chosen function
        // by using the bisection method
        private void InterceptsButton_Click(object sender, RoutedEventArgs e)
        {
            var equationS = "";
            var isSelected = false;
            switch (_calcSel)
            {
                case 0:
                    equationS = Equation1Box.Text;
                    isSelected = true;
                    break;
                case 1:
                    equationS = Equation2Box.Text;
                    isSelected = true;
                    break;
                case 2:
                    equationS = Equation3Box.Text;
                    isSelected = true;
                    break;
            }

            // Make sure selected equation is not empty  
            if (isSelected && equationS != "")
            {
                equationS = PreprocessFunction(equationS);
                var options = ScriptOptions.Default
                    .AddReferences(typeof(Math).Assembly)
                    .AddImports("System");

                // Ensure the function string is a valid lambda expression
                var lambdaExpression = $"x => {equationS}";

                var script = CSharpScript.Create<Func<double, double>>(lambdaExpression, options);
                var result = script.RunAsync().Result;
                var equation = result.ReturnValue;

                // Find the y-axis intercept
                var yIntercept = equation(0);
                var yLine = RectGC_Plot.Plot.Add.HorizontalLine(yIntercept);
                yLine.IsVisible = true;
                yLine.Text = $"y-Intercept = {Math.Round(yIntercept, 2)}";
                yLine.LinePattern = LinePattern.Solid;

                // Define the range and step size for finding roots
                double.TryParse(_xMin, out double xMin);
                double.TryParse(_xMax, out double xMax);
                var step = 0.001; // Smaller step size for more accurate evaluation

                var xIntercepts = new HashSet<double>();

                // Check for x-intercepts using small steps and identify sign changes
                for (var x = xMin; x < xMax; x += step)
                {
                    var y1 = equation(x);
                    var y2 = equation(x + step);

                    // Check for a sign change between y1 and y2
                    if (y1 * y2 <= 0)
                    {
                        // Refine root using Newton-Raphson method
                        var root = RootWNewtonRaphson(equation, x);
                        xIntercepts.Add(Math.Round(root, 6)); // Round to avoid floating-point precision issues
                    }
                }
                // Handle -0 and 0 specifically
                if (xIntercepts.Contains(-0.0) && xIntercepts.Contains(0.0))
                {
                    xIntercepts.Remove(-0);
                    xIntercepts.Add(0);
                }
                // Add lines for all x-intercepts found
                foreach (var xIntercept in xIntercepts.OrderBy(x => x)) // Ensure unique and sorted intercepts
                {
                    var xLine = RectGC_Plot.Plot.Add.VerticalLine(xIntercept);
                    xLine.IsVisible = true;
                    xLine.Text = $"x-Intercept = {Math.Round(xIntercept, 2)}";
                    xLine.LinePattern = LinePattern.Solid;
                }
                // Refresh the plot
                RectGC_Plot.Refresh();
            }
            else
            {
                // Display an error of bad selection to the user
                MessageBox.Show("Please select a valid equation.");
            }
        }
        // METHOD: finds the roots of a function using the NewtonRaphson method
        // PRE: A valid equation with an initial guess, tolerance, and the 
        // number of iterations allowed
        private double RootWNewtonRaphson(Func<double, double> equation, double initialGuess,
            double tolerance = 1e-6, int maxIterations = 100)
        {
            double x = initialGuess;
            for (int i = 0; i < maxIterations; i++)
            {
                double fx = equation(x);
                double dfx = (equation(x + tolerance) - fx) / tolerance; // Numerical derivative
                if (Math.Abs(fx) < tolerance)
                    return x;
                if (Math.Abs(dfx) < tolerance)
                    break; // Prevent division by zero or very small derivative
                x = x - fx / dfx;
            }
            return x;
        }


        // METHOD: Will display an integral of the user selected equation
        // PRE: There is an equation selected -> will display an error
        private async void IntegralButton_Click(object sender, RoutedEventArgs e)
        {
            // We want to display the integral of the currently selected function
            // We will pass the function to our python backend for integration
            var equationS = "";
            switch (_calcSel)
            {
                // Check which equation is selected
                case 4:
                    // We want to print a messageBox to show an invalid selection
                    MessageBox.Show("No equation selected!");
                    break;
                case 0:
                    // Assign our equation and break
                    equationS = Equation1Box.Text;
                    break;
                case 1:
                    // Assign our equation and break
                    equationS = Equation2Box.Text;
                    break;
                case 2:
                    // Assign our equation and break
                    equationS = Equation3Box.Text;
                    break;
            }

            // Make sure to not cause a formatting error
            if (_calcSel == 4) return;

            // Get the result from the logic
            var result = await _client.CalculateAsync("int(" + equationS + ")");
            // We need the line to be empty
            var line = RectGC_Plot.Plot.Add.Line(0, 0, 0, 0);
            line.LineWidth = 0;
            // Display the integral in our legend
            line.LegendText = "Integral: " + result;
            // Refresh our plot
            RectGC_Plot.Refresh();
        }

        // The methods below hold the logic behind symbols being
        // inserted into the text string at the user caret
        private void sqrButton_Click(object sender, RoutedEventArgs e)
        {
            InsertTextAtCaret("sqrt()");
        }

        // The next 16 methods hold the logic for the symbols
        // page, all insert a char into the user textbox
        private void piButton_Click(object sender, RoutedEventArgs e)
        {
            InsertTextAtCaret("pi");
        }

        private void derButton_Click(object sender, RoutedEventArgs e)
        {
            InsertTextAtCaret("d/dx()");
        }

        private void plusButton_Click(object sender, RoutedEventArgs e)
        {
            InsertTextAtCaret("+");
        }

        private void minButton_Click(object sender, RoutedEventArgs e)
        {
            InsertTextAtCaret("-");
        }

        private void multiButton_Click(object sender, RoutedEventArgs e)
        {
            InsertTextAtCaret("*");
        }

        private void divButton_Click(object sender, RoutedEventArgs e)
        {
            InsertTextAtCaret("/");
        }

        private void derivButton_Click(object sender, RoutedEventArgs e)
        {
            InsertTextAtCaret("d/dx");
        }

        private void integralButton_Click_1(object sender, RoutedEventArgs e)
        {
            InsertTextAtCaret("int()");
        }

        private void sinButton_Click(object sender, RoutedEventArgs e)
        {
            InsertTextAtCaret("sin()");
        }

        private void cosButton_Click(object sender, RoutedEventArgs e)
        {
            InsertTextAtCaret("cos()");
        }

        private void tanButton_Click(object sender, RoutedEventArgs e)
        {
            InsertTextAtCaret("tan()");
        }

        private void brackButton_Click(object sender, RoutedEventArgs e)
        {
            InsertTextAtCaret("{}");
        }

        private void invSinButton_Click(object sender, RoutedEventArgs e)
        {
            InsertTextAtCaret("asin()");
        }

        private void invCosButton_Click(object sender, RoutedEventArgs e)
        {
            InsertTextAtCaret("acos()");
        }

        private void invTanButton_Click(object sender, RoutedEventArgs e)
        {
            InsertTextAtCaret("atan()");
        }

        private void expButton_Click(object sender, RoutedEventArgs e)
        {
            InsertTextAtCaret("^()");
        }

        // Common method to insert text at the caret position
        private void InsertTextAtCaret(string text)
        {
            if (userInput != null)
            {
                // Save the current caret index
                int caretIndex = userInput.CaretIndex;

                // Check if there's selected text and replace it with the given text
                if (!string.IsNullOrEmpty(userInput.SelectedText))
                {
                    userInput.SelectedText = text;
                    caretIndex += text.Length - userInput.SelectedText.Length;
                }
                // If there's no selected text, insert the given text at the caret index
                else
                {
                    userInput.Text = userInput.Text.Insert(caretIndex, text);
                    caretIndex += text.Length; // Move caret index after the inserted text
                }

                // Restore the caret index
                userInput.CaretIndex = caretIndex;
            }
        }

        // The next 2 methods change the color of the insert 
        // button based on mouse entering/leaving
        private void InsertButton_MouseEnter(object sender, MouseEventArgs e)
        {
            InsertButton.Background = new BrushConverter().ConvertFromString("#30B646") as Brush;
        }

        private void InsertButton_MouseLeave(object sender, MouseEventArgs e)
        {
            InsertButton.Background = new BrushConverter().ConvertFromString("#70B493") as Brush;
        }

        // This method inserts the users equation if there
        // is an open spot avail.
        // Equation Box variables
        private string _eq1Box;
        private string _eq2Box;
        private string _eq3Box;

        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            if (Equation1Box.Text == "")
            {
                Equation1Box.Text = userInput.Text;
            }
            else if (Equation2Box.Text == "")
            {
                Equation2Box.Text = userInput.Text;
            }
            else if (Equation3Box.Text == "")
            {
                Equation3Box.Text = userInput.Text;
            }
        }

        // The next three methods update the saved settings 
        // based on the box text changing
        private void Equation3Box_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isChanging) return;
            _eq3Box = Equation3Box.Text;
            WriteGraphSettings();
        }

        private void Equation2Box_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isChanging) return;
            _eq2Box = Equation2Box.Text;
            WriteGraphSettings();
        }

        private void Equation1Box_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isChanging) return;
            _eq1Box = Equation1Box.Text;
            WriteGraphSettings();
        }

        // This method handles the initial load from saved settings,
        // sets a flag to make sure updating doesn't happen during
        // load
        private bool _isChanging = false;

        private void UpdateEqBoxes()
        {
            _isChanging = true;
            Equation1Box.Text = _eq1Box;
            Equation2Box.Text = _eq2Box;
            Equation3Box.Text = _eq3Box;
            _isChanging = false;
        }

        private void userInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            // List of functions to check
            string[] functions = { "sin", "sqrt", "cos", "tan", "atan", "asin", "acos", "int" };

            // Get the current text and caret position
            string text = textBox.Text;
            int caretIndex = textBox.CaretIndex;

            // Check if the user typed '(' or ')'
            if (e.Text == "(")
            {
                // Insert '(' at the current cursor position
                textBox.Text = textBox.Text.Insert(caretIndex, "(");
                // Insert ')' at the next position
                textBox.Text = textBox.Text.Insert(caretIndex + 1, ")");
                // Move the cursor inside the parentheses
                textBox.CaretIndex = caretIndex + 1;
                // Set the Handled property to true to prevent the default behavior
                e.Handled = true;
            }
            else if (e.Text == ")")
            {
                // Allow normal typing of ')'
                return;
            }
            else
            {
                // Check if the caret is immediately after a function name
                foreach (var function in functions)
                {
                    if (caretIndex >= function.Length &&
                        text.Substring(caretIndex - function.Length, function.Length) == function)
                    {
                        // If the next character isn't an opening parenthesis, wrap it in parentheses
                        if (e.Text != "(")
                        {
                            // Insert the character within parentheses
                            textBox.Text = textBox.Text.Insert(caretIndex, "(" + e.Text + ")");
                            // Update the caret position to be inside the parentheses
                            // Position after the opening parenthesis
                            // Set the Handled property to true to prevent the default behavior
                            textBox.CaretIndex = caretIndex + 2;
                            e.Handled = true;
                        }

                        break;
                    }
                }
            }
        }

        // Update our settings file with changed points
        private string _xValues;
        private string _yValues;

        private void UserXBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _xValues = "{" + UserXBox.Text + "}";
            WriteGraphSettings();
        }

        private void UserYBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _yValues = "{" + UserYBox.Text + "}";
            WriteGraphSettings();
        }

        // Lets the user choose which box buttons on graph settings with influence
        private int _calcSel;

        private void CalcComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _calcSel = CalculateComboBox.SelectedIndex;
            UpdateGraphView();
            WriteGraphSettings();
        }

        private bool _isUing = false; // Prevents re-entrance and recursive calls

        // METHOD: 
        // PRE:
        private void userInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Prevent re-entrance
            if (_isUing) return;
            // Set updating flag
            _isUing = true;
            // List of functions to check
            string[] functions = ["sin", "sqrt", "cos", "tan", "atan", "asin", "acos", "int"];
            // Get the current text and caret position
            var text = userInput.Text;
            var caretIndex = userInput.CaretIndex;

            // Check if user is deleting text
            var isDeleting = e.Changes.Any(change => change.RemovedLength > 0);
            // Update text only if not deleting
            if (!isDeleting)
            {
                foreach (var function in functions)
                {
                    // Check if the text contains the function without parentheses
                    int index = text.IndexOf(function);
                    while (index != -1)
                    {
                        // Ensure it's not already followed by "(" and the function is not part of a larger word
                        if (index + function.Length < text.Length &&
                            text[index + function.Length] != '(')
                        {
                            // Ensure the function is not part of a larger word
                            var isValidFunction = (index == 0 || !char.IsLetterOrDigit(text[index - 1])) &&
                                                  (index + function.Length == text.Length ||
                                                   !char.IsLetterOrDigit(text[index + function.Length]));

                            if (isValidFunction)
                            {
                                // Insert the parentheses
                                text = text.Insert(index + function.Length, "()");

                                // If the caret is after the insertion point, adjust the caret position
                                if (caretIndex > index + function.Length - 1)
                                {
                                    caretIndex += 2;
                                }
                            }
                        }

                        // Find the next occurrence of the function, ensuring the search index is within bounds
                        index = (index + function.Length + 2 <= text.Length)
                            ? text.IndexOf(function, index + function.Length + 2)
                            : -1;
                    }
                }
            }

            // Update the text and caret position
            userInput.Text = text;
            userInput.CaretIndex = caretIndex;

            // Reset updating flag
            _isUing = false;
        }

        // Refreshes the graph view when the user clicks the button
        private void RfButton_Click(object sender, EventArgs e)
        {
            UpdateGraphView();
        }

        // Changes the color when the mouse enters and leaves the button
        private void MouseEnter_RfButton(object sender, EventArgs e)
        {
            RefreshButton.Background = new BrushConverter().ConvertFromString("#30B646") as Brush;
        }

        private void MouseLeave_RfButton(Object sender, EventArgs e)
        {
            RefreshButton.Background = new BrushConverter().ConvertFromString("#70B493") as Brush;
        }
    }
}