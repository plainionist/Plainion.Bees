using System;
using System.Globalization;
using System.Text;

namespace Plainion.WhiteRabbit.Presentation.Reports
{
    public class ReportBase
    {
        private StringBuilder myGenerationEnvironmentField;
        private readonly IFormatProvider myFormatProvider = CultureInfo.InvariantCulture;
        private bool myEndsWithNewline;

        protected StringBuilder GenerationEnvironment
        {
            get
            {
                if ((myGenerationEnvironmentField == null))
                {
                    myGenerationEnvironmentField = new StringBuilder();
                }
                return myGenerationEnvironmentField;
            }
            set
            {
                myGenerationEnvironmentField = value;
            }
        }

        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((GenerationEnvironment.Length == 0) || myEndsWithNewline))
            {
                GenerationEnvironment.Append("");
                myEndsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                myEndsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if (("".Length == 0))
            {
                GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + ""));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (myEndsWithNewline)
            {
                GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - "".Length));
            }
            else
            {
                GenerationEnvironment.Append(textToAppend);
            }
        }

        public void WriteLine(string textToAppend)
        {
            Write(textToAppend);
            GenerationEnvironment.AppendLine();
            myEndsWithNewline = true;
        }

        /// <summary>
        /// This is called from the compile/run appdomain to convert objects within an expression block to a string
        /// </summary>
        public string ToStringWithCulture(object objectToConvert)
        {
            if ((objectToConvert == null))
            {
                throw new global::System.ArgumentNullException("objectToConvert");
            }

            var t = objectToConvert.GetType();
            var method = t.GetMethod("ToString", new Type[] { typeof(System.IFormatProvider) });
            if ((method == null))
            {
                return objectToConvert.ToString();
            }
            else
            {
                return ((string)(method.Invoke(objectToConvert, new object[] { myFormatProvider })));
            }
        }
    }
}
