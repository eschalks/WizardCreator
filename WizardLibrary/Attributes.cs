using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WizardLibrary
{
    [AttributeUsage(AttributeTargets.Class)]   
    public class InitialStateAttribute : Attribute
    {    
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class EventAttribute : Attribute
    {
        
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class StatePropertyAttribute : Attribute
    {
        public bool IsVisible { get; set; }
    }

}
