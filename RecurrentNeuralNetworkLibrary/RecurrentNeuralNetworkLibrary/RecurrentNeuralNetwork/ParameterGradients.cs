using Heuristics.Utilities.Matrices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heuristics.MetaHeuristics.NeuralNetworks.Recurrent_Neural_Network
{
    internal class ParameterGradients
    {
        public Matrix dCdV;
        public Matrix dCdU;
        public Matrix dCdW;

        public ParameterGradients(Matrix dcdv, Matrix dcdu, Matrix dcdw)
        {
            dCdV = dcdv;
            dCdU = dcdu;
            dCdW = dcdw;
        }
    }
}
