using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WizardLibrary;

namespace WizardCreator
{
    /// <summary>
    /// Interaction logic for Wizard.xaml
    /// </summary>
    public partial class Wizard : Window
    {
        private ReflectedState currentState;
        public Wizard(ReflectedState firstState)
        {
            InitializeComponent();
            currentState = firstState;
            CreateInterface();
        }

        void CreateInterface()
        {
            PropertyGrid.Children.Clear();
            PropertyGrid.RowDefinitions.Clear();
            EventsPanel.Children.Clear();

            foreach (var property in currentState.Properties)
            {
                var attrs = property.GetCustomAttributes(typeof (StatePropertyAttribute), true);
                if (attrs.Length == 0 || !((StatePropertyAttribute)attrs.First()).IsVisible)
                    continue;
                Control input = null;
                var currentValue = property.GetValue(currentState.Object, null);
                if (property.PropertyType == typeof(string))
                {
                    var textBox = new TextBox();
                    if (currentValue != null)
                        textBox.Text = (string) currentValue;
                    PropertyInfo property1 = property;
                    textBox.TextChanged += (sender, e) => property1.SetValue(currentState.Object, textBox.Text, null);
                    input = textBox;
                } else if (property.PropertyType == typeof(bool))
                {
                    var checkBox = new CheckBox();
                    if (currentValue != null)
                        checkBox.IsChecked = (bool) currentValue;
                    PropertyInfo property1 = property;
                    checkBox.Checked += (sender, e) => property1.SetValue(currentState.Object, true, null);
                    checkBox.Unchecked += (sender, e) => property1.SetValue(currentState.Object, false, null);
                    input = checkBox;
                }
                
                if (input != null)
                {
                    PropertyGrid.RowDefinitions.Add(new RowDefinition() {Height = new GridLength(20)});

                    var text = new TextBlock();
                    text.SetValue(Grid.ColumnProperty, 0);
                    text.SetValue(Grid.RowProperty, PropertyGrid.RowDefinitions.Count - 1);
                    text.Text = property.Name;
                    text.VerticalAlignment = VerticalAlignment.Center;
                    PropertyGrid.Children.Add(text);

                    input.SetValue(Grid.ColumnProperty, 1);
                    input.SetValue(Grid.RowProperty, PropertyGrid.RowDefinitions.Count - 1);
                    PropertyGrid.Children.Add(input);
                }
            }

            foreach (var evt in currentState.Events)
            {
                var button = new Button();
                button.Margin = new Thickness(4, 5, 4, 5);
                button.Content = evt.Name;
                button.Click += OnButtonClick;
                EventsPanel.Children.Add(button);
            }
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;
            var evt = currentState.Events.First(x => x.Name == button.Content.ToString());
            if (evt.ReturnType != typeof(void))
            {
                ReflectedState newState = null;
                try
                {
                    newState = new ReflectedState(evt.Invoke(currentState.Object, new object[0]));
                }
                catch (TargetInvocationException exception)
                {
                    if (!(exception.InnerException is GuardException))
                        throw exception.InnerException;
                    MessageBox.Show(exception.InnerException.Message, "Guard Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                // Copy over any properties both states have in common
                foreach (var property in newState.Properties)
                {
                    var oldProp =
                        currentState.Properties.FirstOrDefault(x => x.Name == property.Name && x.PropertyType == property.PropertyType);
                    if (oldProp != null)
                    {
                        property.SetValue(newState.Object, oldProp.GetValue(currentState.Object, null), null);
                    }
                }
                currentState = newState;
                CreateInterface();
            }
        }
    }
}
