using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Microsoft.CSharp;
using Microsoft.Win32;
using System.Xml.Linq;
using System.Xml.Xsl;
using System.Reflection;
using WizardLibrary;

namespace WizardCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private XslCompiledTransform transform;

        public MainWindow()
        {
            InitializeComponent();

        }

        private void OnOpenXml(object sender, RoutedEventArgs e)
        {
            var d = new OpenFileDialog();
            d.Filter = "*XML Files|*.xml";
            if (d.ShowDialog().GetValueOrDefault())
            {
                try
                {
                    var transform = new XslCompiledTransform();
                    transform.Load(@"D:\Projects\WizardCreator\WizardCreator\StateMachine.xslt");

                    var doc = XDocument.Load(d.OpenFile());
                    Flatten(doc.Root);

                    var output = new MemoryStream();
                    transform.Transform(doc.CreateReader(), null, output);
                    output.Position = 0;
                    var reader = new StreamReader(output);
                    SourceBlock.Text = reader.ReadToEnd();
                    reader.Dispose();

                    CompileMenuItem.IsEnabled = true;
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }

        void Flatten(XElement element)
        {
            var states = element.Elements("state").ToList();
            if (states.Count == 0)
                return;

            foreach (var e in element.Elements())
            {
                if (e.Name == "state")
                    continue;

                foreach (var state in states)
                {
                    var existingElement = state.Elements(e.Name).FirstOrDefault(x => x.Attribute("name") != null && e.Attribute("name") != null && x.Attribute("name").Value == e.Attribute("name").Value);
                    if (existingElement == null)
                    {
                        state.Add(e);
                    }
                    else
                    {
                        var attributes = e.Attributes();
                        foreach (var attr in attributes)
                        {
                            if (existingElement.Attribute(attr.Name) == null)
                            {
                                existingElement.Add(attr);
                            }
                        }
                    }
                }
            }

            if (element.Parent != null)
            {
                foreach (var state in states)
                {
                    state.Remove();
                    element.Parent.Add(state);
                }
                element.Remove();
            }
            foreach (var state in states)
            {
                Flatten(state);
            }
        }

        private void OnCompile(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "Assembly File|*.dll";
            if (!dialog.ShowDialog().GetValueOrDefault())
                return;

            var code = SourceBlock.Text;


            var codeProvider = new CSharpCodeProvider();
            var options = new CompilerParameters();
            options.ReferencedAssemblies.Add("System.Core.dll");
            options.ReferencedAssemblies.Add("WizardLibrary.dll");
            options.OutputAssembly = dialog.FileName;
            options.GenerateExecutable = false;
            var compilerResults = codeProvider.CompileAssemblyFromSource(options, code);
            foreach (CompilerError error in compilerResults.Errors)
            {
                var icon = error.IsWarning ? MessageBoxImage.Warning : MessageBoxImage.Error;
                MessageBox.Show(error.ErrorText, "Error", MessageBoxButton.OK, icon);
            }
        }

        private void OnLoadAssembly(object sender, RoutedEventArgs e)
        {
            var d = new OpenFileDialog();
            d.Filter = "*Assembly Files|*.dll";
            if (!d.ShowDialog().GetValueOrDefault())
                return;

            var asm = Assembly.LoadFile(d.FileName);
            // Find the initial state
            var types = asm.GetExportedTypes();
            var first = types.FirstOrDefault(x => x.GetCustomAttributes(typeof (InitialStateAttribute), true).Length > 0);
            if (first == null)
            {
                MessageBox.Show("No initial state found.");
                return;
            }

            var wizard = new Wizard(new ReflectedState(Activator.CreateInstance(first)));
            wizard.Show();
            wizard.Activate();
        }
    }
}
