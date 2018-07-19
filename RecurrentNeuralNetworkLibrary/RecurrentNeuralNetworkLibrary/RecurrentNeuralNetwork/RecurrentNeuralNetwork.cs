using Heuristics.Utilities.Matrices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heuristics.MetaHeuristics.NeuralNetworks.Recurrent_Neural_Network
{
    public class RecurrentNeuralNetwork
    {
        #region Properties

        /// <summary>
        /// Represents the width of the input vector
        /// </summary>
        private readonly int InputDimensions;

        /// <summary>
        /// Represents the width of the output vector
        /// </summary>
        private readonly int OutputDimensions;

        /// <summary>
        /// Represents the width and height of the hidden layer
        /// </summary>
        private readonly int HiddenLayerDimensions;

        /// <summary>
        /// Represents the maximum depth that should be traversed in any instance of the back propagation through time method
        /// </summary>
        private int Bppt_Truncate;

        /// <summary>
        /// Represents the U, V, and W weight matrices.
        /// </summary>
        private Matrix U, V, W;

        /// <summary>
        /// Represents the hidden states results of the last forward propagation method call
        /// </summary>
        private Matrix HiddenStates;

        /// <summary>
        /// Represents the values of the last hidden layer before they were run through the activation function
        /// </summary>
        private Matrix HiddenStates_NET;

        /// <summary>
        /// Represents hte output of the last forward propagation method call
        /// </summary>
        public Matrix Outputs;

        /// <summary>
        /// Represents the values of the last output matrix before they were run through the activation function.
        /// </summary>
        private Matrix Outputs_NET;

        public Matrix U_Matrix
        {
            get
            {
                return U.Copy();
            }
        }
        public Matrix V_Matrix
        {
            get
            {
                return V.Copy();
            }
        }
        public Matrix W_Matrix
        {
            get
            {
                return W.Copy();
            }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initializes the RNN's matrices. Initializes the weight matrices using Xavier Initialization.
        /// </summary>
        /// <param name="inputDimensions"></param>
        /// <param name="hiddenLayerDimensions"></param>
        /// <param name="outputDimensions"></param>
        public RecurrentNeuralNetwork(int inputDimensions, int hiddenLayerDimensions, int outputDimensions)
        {
            InputDimensions = inputDimensions;
            HiddenLayerDimensions = hiddenLayerDimensions;
            OutputDimensions = outputDimensions;

            W = new Matrix(HiddenLayerDimensions, OutputDimensions);
            V = new Matrix(InputDimensions, HiddenLayerDimensions);
            U = new Matrix(HiddenLayerDimensions, HiddenLayerDimensions);

            U.XavierInitialization(InputDimensions);
            V.XavierInitialization(HiddenLayerDimensions);
            W.XavierInitialization(HiddenLayerDimensions);
        }

        /// <summary>
        /// Initializes the RNN's matrices. The weight matrices are set to the passed in weight matrices.
        /// </summary>
        /// <param name="inputDimensions"></param>
        /// <param name="hiddenLayerDimensions"></param>
        /// <param name="outputDimensions"></param>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="w"></param>
        public RecurrentNeuralNetwork(int inputDimensions, int hiddenLayerDimensions, int outputDimensions, 
            Matrix u, Matrix v, Matrix w)
        {
            InputDimensions = inputDimensions;
            HiddenLayerDimensions = hiddenLayerDimensions;
            OutputDimensions = outputDimensions;

            U = u;
            V = v;
            W = w;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// The BPPT method used to calculate weight gradient changes given an input matrix and output matrix
        /// </summary>
        /// <param name="Inputs"></param>
        /// <param name="ExpectedOutputs"></param>
        /// <returns></returns>
        private ParameterGradients BackPropagationThroughTime(Matrix Inputs, Matrix ExpectedOutputs)
        {
            // Initialize parameters and run the forward propagation method
            int TimeSteps = Inputs.Height;
            ForwardPropagation(Inputs);

            Matrix dCdV = new Matrix(V.Height, V.Width);
            Matrix dCdU = new Matrix(U.Height, U.Width);
            Matrix dCdW = new Matrix(W.Height, W.Width);

            Matrix DeltaOutput = new Matrix(Outputs.Height, Outputs.Width);

            // Calculate delta output
            for (int i = 0; i < DeltaOutput.Height; i++)
            {
                for (int j = 0; j < DeltaOutput.Width; j++)
                {
                    DeltaOutput.InnerMatrix[i, j] = (ExpectedOutputs.InnerMatrix[i, j] - Outputs.InnerMatrix[i, j]) 
                        * Tanhderivation(Outputs_NET.InnerMatrix[i, j]);
                }
            }
            
            Vector DeltaT = new Vector(HiddenStates.Width);

            // Loop through each timestep, calculate the weight changes for that time, then run the BPPT algorithm for the remaining time steps
            for(int TimeStep = TimeSteps - 1; TimeStep >= 0; TimeStep--)
            {
                // Compute the changes to W
                dCdW.AddMatrix(Vector.OuterProduct(HiddenStates.GetRow(TimeStep), DeltaOutput.GetRow(TimeStep)));

                // Calculate delta T for this timestep
                DeltaT = Matrix.DotProduct(W, DeltaOutput.GetRow(TimeStep));
                for(int i = 0; i < DeltaT.Width; i++)
                {
                    DeltaT.InnerVector[i] *= Tanhderivation(HiddenStates_NET.InnerMatrix[TimeStep , i]);
                }

                // Calculate the depth the bppt algorithm should run
                int BPPT_STEPS;
                if(Bppt_Truncate == 0)
                {
                    BPPT_STEPS = 0;
                }
                else
                {
                    BPPT_STEPS = Math.Max(0, TimeStep - Bppt_Truncate);
                }

                // Go back a specific depth in time, step by step
                for (int BpptStep = TimeStep; BpptStep >= BPPT_STEPS; BpptStep--)
                {
                    // If you aren't add time step 0 calculate the changes to U
                    if (BpptStep > 0)
                    {
                        dCdU.AddMatrix(Vector.OuterProduct(DeltaT, HiddenStates.GetRow(BpptStep - 1)));
                    }
                    
                    // Calculate the changes to V
                    for(int i = 0; i < dCdV.Height; i++)
                    {
                        for(int j = 0; j < dCdV.Width; j++)
                        {
                            dCdV.InnerMatrix[i, j] += Inputs.InnerMatrix[BpptStep, i] * DeltaT.InnerVector[j];
                        }
                    }

                    // Calculate the new delta T
                    DeltaT = Matrix.DotProduct(U, DeltaT);
                    for (int i = 0; i < DeltaT.Width; i++)
                    {
                        if (BpptStep != 0)
                        {
                            DeltaT.InnerVector[i] *= Tanhderivation(HiddenStates_NET.InnerMatrix[BpptStep-1, i]);
                        }
                        else
                        {
                            DeltaT.InnerVector[i] *= Tanhderivation(0);
                        }
                    }
                }
            }


            return new ParameterGradients(dCdV, dCdU, dCdW);
        }
        
        /// <summary>
        /// Runs a single input Matrix through the recurrent neural network
        /// </summary>
        /// <param name="Inputs"></param>
        public void ForwardPropagation(Matrix Inputs)
        {
            // Initialize variables
            int TimeSteps = Inputs.Height;

            HiddenStates = new Matrix(TimeSteps, HiddenLayerDimensions);
            HiddenStates_NET = new Matrix(TimeSteps, HiddenLayerDimensions);

            Outputs = new Matrix(TimeSteps, OutputDimensions);
            Outputs_NET = new Matrix(TimeSteps, OutputDimensions);

            double temp;

            Matrix V_T = V.Transpose();

            // Calculate the hidden layer values for each time step
            for (int i = 0; i < TimeSteps; i++)
            {
                for (int j = 0; j < HiddenLayerDimensions; j++)
                {
                    temp = 0;
                    for (int k = 0; k < Inputs.Width; k++)
                    {
                        temp += Inputs.InnerMatrix[i, k] * V_T.InnerMatrix[j, k];
                    }

                    for (int k = 0; k < U.Width; k++)
                    {
                        if (i != 0)
                        {
                            temp += HiddenStates.InnerMatrix[i - 1, k] * U.InnerMatrix[j, k];
                        }
                    }

                    HiddenStates_NET.InnerMatrix[i, j] = temp;
                    temp = Tanh(temp);
                    HiddenStates.SetValue(i, j, temp);
                }
            }

            // Calculate the output values for each timestep
            for (int i = 0; i < TimeSteps; i++)
            {
                for (int j = 0; j < OutputDimensions; j++)
                {
                    temp = 0;
                    for (int k = 0; k < W.Width; k++)
                    {
                        temp += HiddenStates.InnerMatrix[i, k] * W.InnerMatrix[j, k];
                    }

                    Outputs_NET.InnerMatrix[i, j] = temp;
                    temp = Tanh(temp);
                    Outputs.SetValue(i, j, temp);
                }
            }
        }

        /// <summary>
        /// Represents the rectifier function.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private double Rectifier(double x)
        {
            double temp = Math.Log(1 + Math.Exp(x));

            return temp;
        }

        /// <summary>
        /// Represents the derivitive of the rectifier method
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private double RectifierDerivation(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }

        /// <summary>
        /// The tanh math function
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private double Tanh(double x)
        {
            return Math.Tanh(x);
        }

        /// <summary>
        /// The tanh derivation function
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private double Tanhderivation(double x)
        {
            return 1 - Math.Pow(Tanh(x), 2);
        }

        /// <summary>
        /// Trains the RNN using the passed in parameters
        /// </summary>
        /// <param name="InputsSeries"></param>
        /// <param name="ExpectedOutputs"></param>
        /// <param name="LearningRate"></param>
        /// <param name="NumberOfEpochs"></param>
        /// <param name="bppt_truncate"></param>
        public void Train(List<Matrix> InputsSeries, List<Matrix> ExpectedOutputs, double LearningRate, int NumberOfEpochs, int bppt_truncate = 0)
        {
            Bppt_Truncate = bppt_truncate;

            // Run through each epoch
            for(int epoch = 0; epoch < NumberOfEpochs; epoch++)
            {
                // and pass in each input/output pair to train with
                for(int i = 0; i < InputsSeries.Count; i++)
                {
                    TrainingStep(InputsSeries[i], ExpectedOutputs[i], LearningRate);
                }
            }
        }

        /// <summary>
        /// Trains the RNN using an individual input/output pair
        /// </summary>
        /// <param name="Inputs"></param>
        /// <param name="ExpectedOutputs"></param>
        /// <param name="LearningRate"></param>
        public void TrainingStep(Matrix Inputs, Matrix ExpectedOutputs, double LearningRate)
        {
            // Get the gradients from the BPPT method
            ParameterGradients gradients = BackPropagationThroughTime(Inputs, ExpectedOutputs);

            // Currently clipping gradients
            #region Clip gradients
            for(int i = 0; i < gradients.dCdU.Height; i++)
            {
                for(int j = 0; j < gradients.dCdU.Width; j++)
                {
                    double value = gradients.dCdU.InnerMatrix[i, j] * LearningRate;
                    if(value > 5)
                    {
                        value = 5;
                    }
                    else if(value < -5)
                    {
                        value = -5;
                    }

                    gradients.dCdU.InnerMatrix[i, j] = value;
                }
            }
            for (int i = 0; i < gradients.dCdV.Height; i++)
            {
                for (int j = 0; j < gradients.dCdV.Width; j++)
                {
                    double value = gradients.dCdV.InnerMatrix[i, j] * LearningRate;
                    if (value > 5)
                    {
                        value = 5;
                    }
                    else if (value < -5)
                    {
                        value = -5;
                    }

                    gradients.dCdV.InnerMatrix[i, j] = value;
                }
            }
            for (int i = 0; i < gradients.dCdW.Height; i++)
            {
                for (int j = 0; j < gradients.dCdW.Width; j++)
                {
                    double value = gradients.dCdW.InnerMatrix[i, j] * LearningRate;
                    if (value > 5)
                    {
                        value = 5;
                    }
                    else if (value < -5)
                    {
                        value = -5;
                    }

                    gradients.dCdW.InnerMatrix[i, j] = value;
                }
            }
            #endregion Clip Gradients

            // Set the matrices
            U.AddMatrix(gradients.dCdU);
            V.AddMatrix(gradients.dCdV);
            W.AddMatrix(gradients.dCdW);
        }



        #endregion Methods
    }
}
