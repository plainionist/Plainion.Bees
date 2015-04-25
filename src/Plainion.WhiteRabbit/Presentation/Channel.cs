using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plainion.WhiteRabbit.Presentation
{
    public class Channel
    {
        public Action<TimeSpan> OnTimeElapsedChanged
        {
            get;
            set;
        }

        public Action<TimeSpan> OnJitterChanged
        {
            get;
            set;
        }

    }
}
