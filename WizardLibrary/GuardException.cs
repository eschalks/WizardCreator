using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WizardLibrary
{
    public class GuardException : Exception
    {
        public GuardException(string message) : base(message)
        {
            
        }
    }
}
