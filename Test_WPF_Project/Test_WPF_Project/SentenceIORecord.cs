using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_WPF_Project
{
    public class SentenceIORecord
    {
        public List<int> Inputs { get; set; }
        public int Output { get; set; }

        public SentenceIORecord()
        {
            Inputs = new List<int>();
        }

        public SentenceIORecord(List<int> inputs, int output)
        {
            Inputs = inputs;
            Output = output;
        }
    }
}
