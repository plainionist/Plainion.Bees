using System;
using System.Globalization;
using System.Text;

namespace Plainion.WhiteRabbit.Presentation.Reports
{
    public class ReportBase
    {
        private readonly StringBuilder myGenerationEnvironment;

        protected ReportBase()
        {
            myGenerationEnvironment = new StringBuilder();
        }

        protected StringBuilder GenerationEnvironment => myGenerationEnvironment;

        public void Write(string textToAppend)
        {
            GenerationEnvironment.Append(textToAppend);
        }

        public void WriteLine(string textToAppend)
        {
            Write(textToAppend);
            GenerationEnvironment.AppendLine();
        }
    }
}
