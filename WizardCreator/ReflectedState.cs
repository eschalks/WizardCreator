using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using WizardLibrary;
using System.Windows;
using System.IO;

namespace WizardCreator
{
    public class ReflectedState
    {
        public string Name;
        public List<MethodInfo> Events { get; private set; }
        public PropertyInfo[] Properties { get; private set; }
        public object Object { get; private set; }

        public ReflectedState(object obj)
        {
            Object = obj;
            Events = new List<MethodInfo>();
            var type = obj.GetType();
            Name = type.Name;

            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var method in methods)
            {
                if (method.GetCustomAttributes(typeof(EventAttribute), true).Length > 0 && method.GetParameters().Length == 0)
                {
                    Events.Add(method);
                }
            }

            Properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
        }
    }
}
