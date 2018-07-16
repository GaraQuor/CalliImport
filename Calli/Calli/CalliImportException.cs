using System;
using System.Reflection;

namespace Calli
{
    /// <summary>
    /// Calli import exception
    /// </summary>
    /// <remarks>
    /// By GaraQuor (.aka Trystan Delhaye) - 14/07/2018
    /// </remarks>
    public class CalliImportException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CalliImportException( FieldInfo _calliField, string _message) : base (_message + GetMessage(_calliField) )
        {

        }

        private static string GetMessage( FieldInfo _calliField)
        {
            return "\n   at " + _calliField.DeclaringType.FullName + "." + _calliField.Name + "()";
        }
    }
}
