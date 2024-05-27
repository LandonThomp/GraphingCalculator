using System;
using System.Collections;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace GraphingCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Bool to track button click state
        private bool _eqButtonClicked = false;
        // Bool to track Graph View button click state
        private bool _graphViewButtonClicked = false;
        // Bool to track Graph Settings button click state
        private bool _graphSettingsButtonClicked = false;
        // Bool to track System Settings button click state
        private bool _systemSettingsButtonClicked = false;

        // A queue for our user textbox
        private Queue<string> _userInputs = new Queue<string>();

        // Ints for button switcher
        int eqButtonInt = 1;
        int graphViewButtonInt = 2;
        int graphSettingsButtonInt = 3;
        int systemSettingsButtonInt = 4;

        public MainWindow()
        {
            InitializeComponent();
            // Our hidden comps for our UI
            EquationEditor.Visibility = Visibility.Collapsed;
            GraphView.Visibility = Visibility.Collapsed;
            GraphSettings.Visibility = Visibility.Collapsed;
            SystemSettings.Visibility = Visibility.Collapsed;

            textBorder1.Visibility = Visibility.Collapsed;
            textBorder2.Visibility = Visibility.Collapsed;
            textBorder3.Visibility = Visibility.Collapsed;
        }

        // Button state change helper method
        private void buttonChanger(int button)
        {
            if(button == eqButtonInt)
            {
                MainContent.Visibility = Visibility.Collapsed;
                GraphSettings.Visibility = Visibility.Collapsed;
                GraphView.Visibility = Visibility.Collapsed;
                SystemSettings.Visibility = Visibility.Collapsed;
                EquationEditor.Visibility = Visibility.Visible;

                _graphViewButtonClicked = false;
                _graphSettingsButtonClicked = false;
                _systemSettingsButtonClicked = false;
                // Graph View, Graph Settings, System Settings
                graphViewButton.Background = (System.Windows.Media.Brush)graphViewButton.FindResource("NormalColor");                
                grapSettingsButton.Background = (System.Windows.Media.Brush)grapSettingsButton.FindResource("NormalColor");
                systemSettingsButton.Background = (System.Windows.Media.Brush)systemSettingsButton.FindResource("NormalColor");
            }
            else if(button == graphViewButtonInt) 
            {
                MainContent.Visibility = Visibility.Collapsed;
                EquationEditor.Visibility = Visibility.Collapsed;
                SystemSettings.Visibility = Visibility.Collapsed;
                GraphSettings.Visibility = Visibility.Collapsed;
                GraphView.Visibility = Visibility.Visible;

                _eqButtonClicked = false;
                _systemSettingsButtonClicked = false;
                _graphSettingsButtonClicked = false;
                // Equation Editor, Graph Settings, System Settings
                eqEditorButton.Background = (System.Windows.Media.Brush)eqEditorButton.FindResource("NormalColor");                
                grapSettingsButton.Background = (System.Windows.Media.Brush)grapSettingsButton.FindResource("NormalColor");
                systemSettingsButton.Background = (System.Windows.Media.Brush)systemSettingsButton.FindResource("NormalColor");
            }
            else if(button == graphSettingsButtonInt) 
            {
                MainContent.Visibility = Visibility.Collapsed;
                EquationEditor.Visibility = Visibility.Collapsed;
                GraphView.Visibility = Visibility.Collapsed;
                SystemSettings.Visibility = Visibility.Collapsed;
                GraphSettings.Visibility = Visibility.Visible;

                _eqButtonClicked = false;
                _systemSettingsButtonClicked = false;
                _graphViewButtonClicked = false;
                // Equation Editor, System Settings, Graph View
                eqEditorButton.Background = (System.Windows.Media.Brush)eqEditorButton.FindResource("NormalColor");
                graphViewButton.Background = (System.Windows.Media.Brush)graphViewButton.FindResource("NormalColor");
                systemSettingsButton.Background = (System.Windows.Media.Brush)systemSettingsButton.FindResource("NormalColor");
            }
            else if(button == systemSettingsButtonInt)
            {
                MainContent.Visibility = Visibility.Collapsed;
                EquationEditor.Visibility = Visibility.Collapsed;
                GraphView.Visibility = Visibility.Collapsed;
                GraphSettings.Visibility = Visibility.Collapsed;
                SystemSettings.Visibility = Visibility.Visible;

                _eqButtonClicked = false;
                _graphSettingsButtonClicked = false;
                _graphViewButtonClicked = false;
                // Equation Editor, Graph Settings, Graph View
                eqEditorButton.Background = (System.Windows.Media.Brush)eqEditorButton.FindResource("NormalColor");
                grapSettingsButton.Background = (System.Windows.Media.Brush)grapSettingsButton.FindResource("NormalColor");
                graphViewButton.Background = (System.Windows.Media.Brush)graphViewButton.FindResource("NormalColor");
            }
        }

        private void eqEditorButton_Click(object sender, RoutedEventArgs e)
        {
            if(!_eqButtonClicked) 
            {
                buttonChanger(eqButtonInt);
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
        private void eqEditorButton_MouseEnter(object sender, MouseEventArgs e)
        {
            eqEditorButton.Background = (System.Windows.Media.Brush)eqEditorButton.FindResource("PressedColor");
        }
        private void eqEditorButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if(!_eqButtonClicked) 
            {
                eqEditorButton.Background = (System.Windows.Media.Brush)eqEditorButton.FindResource("NormalColor");
            }            
        }

        private void graphViewButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_graphViewButtonClicked)
            {
                // Switch other button states to off
                buttonChanger(graphViewButtonInt);
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
        // hovers over the equation editor button
        private void graphViewButton_MouseEnter(object sender, MouseEventArgs e)
        {
            graphViewButton.Background = (System.Windows.Media.Brush)graphViewButton.FindResource("PressedColor");
        }
        private void graphViewButton_MouseLeave(object obj, MouseEventArgs e)
        {
            if (!_graphViewButtonClicked)
            {
                graphViewButton.Background = (System.Windows.Media.Brush)graphViewButton.FindResource("NormalColor");
            }
        }
        
        private void graphSettingsButton_Click(Object sender, RoutedEventArgs e)
        {
            if(!_graphSettingsButtonClicked)
            {
                // Switch other button states off
                buttonChanger(graphSettingsButtonInt);
                _graphSettingsButtonClicked = true;
            }else
            {
                MainContent.Visibility = Visibility.Visible;
                GraphSettings.Visibility = Visibility.Collapsed;
                grapSettingsButton.Background = (System.Windows.Media.Brush)grapSettingsButton.FindResource("NormalColor");
                _graphSettingsButtonClicked = false;
            }
        }
        // These two methods handle the color changing when the mouse
        // hovers over the equation editor button
        private void graphSettingsButton_MouseEnter(object sender, MouseEventArgs e)
        {
            grapSettingsButton.Background = (System.Windows.Media.Brush)grapSettingsButton.FindResource("PressedColor");
        }
        private void graphSettingsButton_MouseLeave(Object sender, MouseEventArgs e)
        {
            if (!_graphSettingsButtonClicked)
            {
                grapSettingsButton.Background = (System.Windows.Media.Brush)grapSettingsButton.FindResource("NormalColor");
            }
        }

        private void systemSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if(!_systemSettingsButtonClicked)
            {
                buttonChanger(systemSettingsButtonInt);
                _systemSettingsButtonClicked = true;
            }else
            {
                MainContent.Visibility = Visibility.Visible;
                SystemSettings.Visibility = Visibility.Collapsed;
                systemSettingsButton.Background = (System.Windows.Media.Brush)systemSettingsButton.FindResource("NormalColor");
                _systemSettingsButtonClicked = false;
            }
        }
        // These two methods handle the color changing when the mouse
        // hovers over the equation editor button
        private void systemSettingsButton_MouseEnter(Object sender, MouseEventArgs e)
        {
            systemSettingsButton.Background = (System.Windows.Media.Brush)systemSettingsButton.FindResource("PressedColor");
        }
        private void systemSettingsButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!_systemSettingsButtonClicked)
            {
                systemSettingsButton.Background = (System.Windows.Media.Brush)systemSettingsButton.FindResource("NormalColor");
            }
        }

        private void enterButton_Click(object sender, RoutedEventArgs e)
        {
            // Our main user input string
            string userText = userInput.Text;

            if(!string.IsNullOrWhiteSpace(userText)) 
            {                     
                _userInputs.Enqueue(userText);

                // Remove oldest input if more than three
                if(_userInputs.Count > 3)
                {
                    _userInputs.Dequeue();
                }

                // Our inputsArray var for our queue
                var inputsArray = _userInputs.ToArray();
                // Update our text blocks                
                userTextBlock1.Text = inputsArray.Length > 0 ? inputsArray[^1] : string.Empty;
                if(userTextBlock1.Text != string.Empty)
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
    }
}