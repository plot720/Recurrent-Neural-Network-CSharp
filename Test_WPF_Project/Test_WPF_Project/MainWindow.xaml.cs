
using DataPreperation.TextPreperation;
using DataPreperation.TextPreperation.WordMapping;
using Heuristics.MetaHeuristics.NeuralNetworks.Recurrent_Neural_Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Test_WPF_Project;

namespace Test_WPF_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties

        /// <summary>
        /// Size of the wordbank
        /// </summary>
        private int NumberOfRecordsToKeep = 2000;

        /// <summary>
        /// The number of batches to split the data into
        /// </summary>
        private int NumberOfBatches = 1;

        /// <summary>
        /// The percent of the data to use as training data
        /// </summary>
        private int PercentTrainingData = 50;

        private int NumberOfEpochs = 20;

        /// <summary>
        /// The recurrent neural network instance being used by the application
        /// </summary>
        RecurrentNeuralNetwork rnn;

        /// <summary>
        /// The training/test data being used by the application
        /// </summary>
        public List<SentenceIORecord> IORecords;

        public string CurrentDirectory;

        public string SavedStatesDirectory;

        #endregion Properties

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            // Initialize the memory
            IORecords = new List<SentenceIORecord>();

            CurrentDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            SavedStatesDirectory = CurrentDirectory + "//SavedWeightStates//";
        }

        #endregion Constructor

        #region Button Presses

        /// <summary>
        /// Tests a single record against the RNN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTestRandomRecord(object sender, RoutedEventArgs e)
        {
            TestRandomRecord();
        }

        /// <summary>
        /// Loads a specified saved NN state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            string RecordName = txtBoxRecordName.Text;
            LoadNNState(RecordName, NumberOfRecordsToKeep, 100, 5);
        }

        /// <summary>
        /// Loads IO record training/test data set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoadData_Click(object sender, RoutedEventArgs e)
        {
            IORecords = LoadIORecords();
        }

        /// <summary>
        /// Trains the RNN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            Train();
        }

        /// <summary>
        /// Tests the RNN against all current input/output pairs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTestAll_Click(object sender, RoutedEventArgs e)
        {
            TestAll();
        }
        #endregion Button Presses

        #region Methods

        /// <summary>
        /// Given the ID of the genre, returns the string of the Genre
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        private string GetGenreType(int Index)
        {

            switch (Index)
            {
                case 0:
                    return "Adventure";
                case 1:
                    return "Crime";
                case 2:
                    return "Horror";
                case 3:
                    return "Romance";
                case 4:
                    return "Sci-fi";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Returns all of the sentences from a book
        /// </summary>
        /// <param name="BookURL"></param>
        /// <returns></returns>
        private List<Sentence> GetSentenceFromBook(string BookURL)
        {
            string Content = File.ReadAllText(BookURL);

            List<Sentence> Sentences = TextPreperation.Tokenize(Content, 1000, 10, 3, TextSplitterModes.InOrder);

            return Sentences;
        }

        /// <summary>
        /// Loads the test/trainin data into the IO Records parameter
        /// </summary>
        /// <returns></returns>
        private List<SentenceIORecord> LoadIORecords()
        {
            // Book file URL's
            string Adventure1 =  CurrentDirectory + "\\Books\\Adventure\\Tarzan of the Apes.txt";
            string Adventure2 = CurrentDirectory + "\\Books\\Adventure\\The Lion of Petra.txt";
            string Adventure3 = CurrentDirectory + "\\Books\\Adventure\\The Scarlet Pimpernel.txt";
            string Crime1 = CurrentDirectory + "\\Books\\Crime Fiction\\Dead Men Tell No Tales.txt";
            string Crime2 = CurrentDirectory + "\\Books\\Crime Fiction\\Tales of Chinatown.txt";
            string Crime3 = CurrentDirectory + "\\Books\\Crime Fiction\\The Extraordinary Adventures of Arsene Lupin.txt";
            string Horror1 = CurrentDirectory + "\\Books\\Horror\\Ghost Stories of an Antiquary.txt";
            string Horror2 = CurrentDirectory + "\\Books\\Horror\\Metamorphosis.txt";
            string Horror3 = CurrentDirectory + "\\Books\\Horror\\The Wendigo.txt";
            string Romance1 = CurrentDirectory + "\\Books\\Romantic Fiction\\Only a Girl's Love.txt";
            string Romance2 = CurrentDirectory + "\\Books\\Romantic Fiction\\Star of India.txt";
            string Romance3 = CurrentDirectory + "\\Books\\Romantic Fiction\\Wastralls.txt";
            string Science1 = CurrentDirectory + "\\Books\\Science Fiction\\Astounding Stories of Super_Science.txt";
            string Science2 = CurrentDirectory + "\\Books\\Science Fiction\\The Lost World.txt";
            string Science3 = CurrentDirectory + "\\Books\\Science Fiction\\The Sky Is Falling.txt";

            List<Sentence> Sentences = new List<Sentence>();

            // Keep track of how many sentences were used for each genre
            int NumberOfAdventure, NumberOfCrime, NumberOfHorror, NumberOfRomance, NumberOfScience;

            // Get the sentences from each of the books for each genre
            Sentences.AddRange(GetSentenceFromBook(Adventure1));
            Sentences.AddRange(GetSentenceFromBook(Adventure2));
            Sentences.AddRange(GetSentenceFromBook(Adventure3));
            NumberOfAdventure = Sentences.Count;

            Sentences.AddRange(GetSentenceFromBook(Crime1));
            Sentences.AddRange(GetSentenceFromBook(Crime2));
            Sentences.AddRange(GetSentenceFromBook(Crime3));
            NumberOfCrime = Sentences.Count - NumberOfAdventure;

            Sentences.AddRange(GetSentenceFromBook(Horror1));
            Sentences.AddRange(GetSentenceFromBook(Horror2));
            Sentences.AddRange(GetSentenceFromBook(Horror3));
            NumberOfHorror = Sentences.Count - NumberOfAdventure;

            Sentences.AddRange(GetSentenceFromBook(Romance1));
            Sentences.AddRange(GetSentenceFromBook(Romance2));
            Sentences.AddRange(GetSentenceFromBook(Romance3));
            NumberOfRomance = Sentences.Count - NumberOfHorror;

            Sentences.AddRange(GetSentenceFromBook(Science1));
            Sentences.AddRange(GetSentenceFromBook(Science2));
            Sentences.AddRange(GetSentenceFromBook(Science3));
            NumberOfScience = Sentences.Count - NumberOfRomance;

            // Remove infrequent words
            Sentences = TextPreperation.RemoveInfrequentWords(Sentences, NumberOfRecordsToKeep, "UNKNOWN_TOKEN");

            // Add beggining and ending tokens
            //Sentences = TextPreperation.AddBegginingAndEndTokens(Sentences, "SENTENCE_START", "SENTENCE_END");

            // Map the word strings to integer values
            List<MappedSentence> MappedSentences = TextPreperation.MapSentences(Sentences);

            List<SentenceIORecord> IORecords = new List<SentenceIORecord>();

            int MappedSentenceNumber = 0;

            // For each mapped sentence create an input record
            foreach (MappedSentence s in MappedSentences)
            {
                MappedSentenceNumber++;
                SentenceIORecord temp = new SentenceIORecord();


                int GenreID;
                if (MappedSentenceNumber < NumberOfAdventure)
                {
                    GenreID = 0;
                }
                else if (MappedSentenceNumber < NumberOfCrime + NumberOfAdventure)
                {
                    GenreID = 1;
                }
                else if (MappedSentenceNumber < NumberOfHorror + NumberOfCrime + NumberOfAdventure)
                {
                    GenreID = 2;
                }
                else if (MappedSentenceNumber < NumberOfRomance + NumberOfHorror + NumberOfCrime + NumberOfAdventure)
                {
                    GenreID = 3;
                }
                else
                {
                    GenreID = 4;
                }

                temp.Output = GenreID;

                foreach (int x in s.IDs)
                {
                    temp.Inputs.Add(x);
                }

                IORecords.Add(temp);


            }

            return IORecords;
        }

        /// <summary>
        /// Loads weight matrices of a matrix using hte passed in record URL
        /// </summary>
        /// <param name="RecordName"></param>
        /// <param name="InputDimensions"></param>
        /// <param name="HiddenLayerDimensions"></param>
        /// <param name="OutputDimensions"></param>
        private void LoadNNState(string RecordName, int InputDimensions, int HiddenLayerDimensions, int OutputDimensions)
        {

            Heuristics.Utilities.Matrices.Matrix W = new Heuristics.Utilities.Matrices.Matrix(HiddenLayerDimensions, OutputDimensions);
            Heuristics.Utilities.Matrices.Matrix V = new Heuristics.Utilities.Matrices.Matrix(InputDimensions, HiddenLayerDimensions);
            Heuristics.Utilities.Matrices.Matrix U = new Heuristics.Utilities.Matrices.Matrix(HiddenLayerDimensions, HiddenLayerDimensions);

            string Content = File.ReadAllText(SavedStatesDirectory + RecordName);

            try
            {
                foreach (string line in Content.Split('\n'))
                {
                    List<string> chars = line.Split('_').ToList();
                    try
                    {
                        string Matrix = chars[0];
                        int i = Convert.ToInt32(chars[1]);
                        int j = Convert.ToInt32(chars[2]);
                        double Value = Convert.ToDouble(chars[3]);

                        switch (Matrix)
                        {
                            case "W":
                                W.SetValue(i, j, Value);
                                break;
                            case "U":
                                U.SetValue(i, j, Value);
                                break;
                            case "V":
                                V.SetValue(i, j, Value);
                                break;
                            default:
                                throw new Exception();
                        }
                    }
                    catch
                    {

                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("There was an error loading the file!\n" + e.Message);
            }

            rnn = new RecurrentNeuralNetwork(InputDimensions, HiddenLayerDimensions, OutputDimensions, U, V, W);
        }

        /// <summary>
        /// Saves the RNN weight matrices
        /// </summary>
        private void SaveNNStates()
        {
            Heuristics.Utilities.Matrices.Matrix U, V, W;
            U = rnn.U_Matrix;
            V = rnn.V_Matrix;
            W = rnn.W_Matrix;

            string Content = "";

            for (int i = 0; i < U.Height; i++)
            {
                for (int j = 0; j < U.Width; j++)
                {
                    Content += "U_" + i + "_" + j + "_" + U.GetValue(i, j).ToString() + "\r\n";
                }
            }
            for (int i = 0; i < W.Height; i++)
            {
                for (int j = 0; j < W.Width; j++)
                {
                    Content += "W_" + i + "_" + j + "_" + W.GetValue(i, j).ToString() + "\r\n";
                }
            }
            for (int i = 0; i < V.Height; i++)
            {
                for (int j = 0; j < V.Width; j++)
                {
                    Content += "V_" + i + "_" + j + "_" + V.GetValue(i, j).ToString() + "\r\n";
                }
            }


            string FileName = DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute.ToString() + ".txt";
            File.WriteAllText(SavedStatesDirectory + FileName, Content);
        }

        /// <summary>
        /// Shuffles a list of objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<T> ShuffleList<T>(List<T> list)
        {
            List<T> newList = new List<T>();
            Random r = new Random();

            while (list.Any())
            {
                int next = r.Next(list.Count);
                newList.Add(list[next]);
                list.RemoveAt(next);
            }

            return newList;
        }

        /// <summary>
        /// Splits the IO records into test and training data. An equal number of sentences from each genre will end up in the test and training data.
        /// </summary>
        /// <param name="IORecords"></param>
        /// <param name="PercentToSplit"></param>
        /// <returns></returns>
        private List<SentenceIORecord> SplitIORecords(List<SentenceIORecord> IORecords, double PercentToSplit)
        {
            List<SentenceIORecord> Temp = new List<SentenceIORecord>();
            List<SentenceIORecord> Temp2 = new List<SentenceIORecord>();

            int Amount = (int)(IORecords.Count * PercentToSplit / 5);

            List<SentenceIORecord> TempOfType;
            for (int i = 0; i < 5; i++)
            {
                TempOfType = IORecords.Where(x => x.Output == i).ToList();

                for (int j = 0; j < TempOfType.Count; j++)
                {
                    if (j < Amount)
                    {
                        Temp.Add(TempOfType[j]);
                    }
                    else
                    {
                        Temp2.Add(TempOfType[j]);
                    }
                }
            }

            Temp = ShuffleList(Temp);

            Temp.AddRange(ShuffleList(Temp2));

            return Temp;
        }

        /// <summary>
        /// Tests every record against the RNN
        /// </summary>
        private void TestAll()
        {
            int First = 0;
            int Second = 0;
            int Third = 0;
            int Fourth = 0;
            int Fifth = 0;

            int Total = 0;

            foreach (SentenceIORecord temp2 in IORecords)
            {
                Heuristics.Utilities.Matrices.Matrix TempInput = new Heuristics.Utilities.Matrices.Matrix(temp2.Inputs.Count, NumberOfRecordsToKeep);
                for (int i = 0; i < TempInput.Height; i++)
                {
                    int Value = temp2.Inputs[i];
                    for (int j = 0; j < TempInput.Width; j++)
                    {
                        if (j == Value)
                        {
                            TempInput.SetValue(i, j, 1);
                        }
                        else
                        {
                            TempInput.SetValue(i, j, 0);
                        }
                    }
                }
                rnn.ForwardPropagation(TempInput);

                #region Positive 1
                int IndexOfHighest = 5;
                int IndexOfSecondHighest = 5;
                int IndexOfThirdHighest = 5;
                int IndexOfFourthHighest = 5;
                int IndexOfFifthHighest = 5;

                double HighestValue = -1;
                double SecondHighestValue = -1;
                double ThirdHighestValue = -1;
                double FourthHighestValue = -1;

                for (int i = 0; i < rnn.Outputs.Width; i++)
                {
                    double Value = rnn.Outputs.GetValue(rnn.Outputs.Height - 1, i);
                    if (Value > HighestValue)
                    {
                        IndexOfFifthHighest = IndexOfFourthHighest;

                        IndexOfFourthHighest = IndexOfThirdHighest;
                        FourthHighestValue = ThirdHighestValue;

                        IndexOfThirdHighest = IndexOfSecondHighest;
                        ThirdHighestValue = SecondHighestValue;

                        IndexOfSecondHighest = IndexOfHighest;
                        SecondHighestValue = HighestValue;

                        IndexOfHighest = i;
                        HighestValue = Value;
                    }
                    else if (Value > SecondHighestValue)
                    {
                        IndexOfFifthHighest = IndexOfFourthHighest;

                        IndexOfFourthHighest = IndexOfThirdHighest;
                        FourthHighestValue = ThirdHighestValue;

                        IndexOfThirdHighest = IndexOfSecondHighest;
                        ThirdHighestValue = SecondHighestValue;

                        IndexOfSecondHighest = i;
                        SecondHighestValue = Value;
                    }
                    else if (Value > ThirdHighestValue)
                    {
                        IndexOfFifthHighest = IndexOfFourthHighest;

                        IndexOfFourthHighest = IndexOfThirdHighest;
                        FourthHighestValue = ThirdHighestValue;

                        IndexOfThirdHighest = i;
                        ThirdHighestValue = i;
                    }
                    else if (Value > FourthHighestValue)
                    {
                        IndexOfFifthHighest = IndexOfFourthHighest;

                        IndexOfFourthHighest = i;
                        FourthHighestValue = i;
                    }
                    else
                    {
                        IndexOfFifthHighest = i;
                    }
                }

                if (IndexOfHighest == temp2.Output)
                {
                    First++;
                }
                else if (IndexOfSecondHighest == temp2.Output)
                {
                    Second++;
                }
                else if (IndexOfThirdHighest == temp2.Output)
                {
                    Third++;
                }
                else if (IndexOfFourthHighest == temp2.Output)
                {
                    Fourth++;
                }
                else if (IndexOfFifthHighest == temp2.Output)
                {
                    Fifth++;
                }
                #endregion Positive 1


                Total++;
            }



            MessageBox.Show("First: " + First + "\nSecond: " + Second + "\nThird: " + Third
                + "\nFourth: " + Fourth + "\nFifth: " + Fifth + "\nTotal: " + Total);
        }

        /// <summary>
        /// Tests a random record against the RNN
        /// </summary>
        private void TestRandomRecord()
        {

            Random r = new Random();

            SentenceIORecord temp2 = IORecords[r.Next(0, IORecords.Count - 1)];
            Heuristics.Utilities.Matrices.Matrix TempInput = new Heuristics.Utilities.Matrices.Matrix(temp2.Inputs.Count, NumberOfRecordsToKeep);
            for (int i = 0; i < TempInput.Height; i++)
            {
                int Value = temp2.Inputs[i];
                for (int j = 0; j < TempInput.Width; j++)
                {
                    if (j == Value)
                    {
                        TempInput.SetValue(i, j, 1);
                    }
                    else
                    {
                        TempInput.SetValue(i, j, 0);
                    }
                }
            }
            rnn.ForwardPropagation(TempInput);

            int IndexOfHighest = 5;
            double HighestValue = 0;
            for (int i = 0; i < rnn.Outputs.Width; i++)
            {
                if (rnn.Outputs.GetValue(rnn.Outputs.Height - 1, i) > HighestValue)
                {
                    IndexOfHighest = i;
                    HighestValue = rnn.Outputs.GetValue(rnn.Outputs.Height - 1, i);
                }
            }

            txtBoxActual.Text = GetGenreType(IndexOfHighest);
            txtBoxExpected.Text = GetGenreType(temp2.Output);

        }

        /// <summary>
        /// Trains the RNN
        /// </summary>
        private void Train()
        {
            if (IORecords == null || !IORecords.Any())
            {
                return;
            }

            if (rnn == null)
            {
                rnn = new RecurrentNeuralNetwork(NumberOfRecordsToKeep, 100, 5);
            }

            IORecords = SplitIORecords(IORecords, PercentTrainingData * .01);

            int AmountPerBatch = (int)(IORecords.Count * (PercentTrainingData * .01) / NumberOfBatches);// / NumberOfBatches);

            int CurrentCounter = 0;

            int TotalTrainingData = NumberOfBatches * AmountPerBatch;

            while (CurrentCounter < TotalTrainingData)
            {
                List<Heuristics.Utilities.Matrices.Matrix> Inputs = new List<Heuristics.Utilities.Matrices.Matrix>();
                List<Heuristics.Utilities.Matrices.Matrix> Outputs = new List<Heuristics.Utilities.Matrices.Matrix>();

                for (int sample = 0; sample < AmountPerBatch; sample++)
                {
                    SentenceIORecord temp = IORecords[sample + CurrentCounter];

                    Heuristics.Utilities.Matrices.Matrix Input = new Heuristics.Utilities.Matrices.Matrix(temp.Inputs.Count, NumberOfRecordsToKeep);
                    Heuristics.Utilities.Matrices.Matrix Output = new Heuristics.Utilities.Matrices.Matrix(temp.Inputs.Count, 5);

                    for (int i = 0; i < Input.Height; i++)
                    {
                        int Value = temp.Inputs[i];
                        for (int j = 0; j < Input.Width; j++)
                        {
                            if (j == Value)
                            {
                                Input.SetValue(i, j, 1);
                            }
                            else
                            {
                                Input.SetValue(i, j, 0);
                            }
                        }

                        Value = temp.Output;
                        for (int j = 0; j < Output.Width; j++)
                        {
                            if (j == Value)
                            {
                                Output.SetValue(i, j, 1);
                            }
                            else
                            {
                                Output.SetValue(i, j, 0);
                            }
                        }
                    }

                    Inputs.Add(Input);
                    Outputs.Add(Output);
                }

                rnn.Train(Inputs, Outputs, .001, NumberOfEpochs, 0);

                CurrentCounter += AmountPerBatch;
            }


            SaveNNStates();
        }
        #endregion Methods

    }
}
