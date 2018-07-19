# Recurrent-Neural-Network-CSharp
A C# library for processing recurrent neural networks.


Detailed Implementation
This project was implemented in Visual Studio 2017 as WPF desktop application and as two C# libraries. The WPF application handled the GUI as well as the initial data processing. The C# library contained the code for the recurrent neural network itself, as well as utility classes that help process the RNN. This project can be split into four main segments: The Matrix and Vector classes, the Recurrent Neural Network class, the Data Preparation class, and the Main Window class. The Matrix, Vector, and RNN classes are contained within the library while the Main Window class is contained in the WPF application. The Data Preparation class is contained in a separate library that was created to help prepare the data for the RNN.
The reason that the code for this project was split into different segments was to add to the reusability of the project. The Data Preparation class and the Matrix and Vector classes have no ties to any other classes in the project. This allows for these classes to be utilized individually in future projects without changing their code. The Recurrent Neural Network class has ties solely to the Matrix and Vector classes, so it can be implemented in any project containing these classes. The Main Window ties all these classes together, so it has strong links to each class. This approach makes the project very modular. 
While discussing the choices made when implementing this project, the choices will often be compared to the recurrent neural network implementation created by Denny Britz [3]. This implementation is likely the most popular implementation of a recurrent neural network on the internet currently. The implementation is written in Python and targets an NLP problem. The key difference between the two implementations is that this project is a classification problem, while Britz’s implementation targets word prediction. This means that Britz’s implementation needs an output at the end of each time step, while this projects implementation only needs an output at the last time step.

Matrix and Vector classes

The matrix and vector classes were the classes that handled most of the operations in this project. The matrix class represented a two-dimensional array, while the vector class represented a one-dimensional array. Each of these classes also held a variety of methods that allowed operations to be performed on the arrays. The reason that these classes were split into two different classes is because many operations can be only performed on one of the two classes. For example, the dot product operation can only be performed if one of the two objects are a vector. To make it easier to clarify when a matrix is a vector, it was separated and became its own class. 

Matrix Class

Properties and Constructors

The matrix class consisted of three properties: it’s height, width, and the two-dimensional array that stored its values. Its height and width were read only values that were initialized in the classes constructor. A height and width were required to be passed in so that the two-dimensional array could have its memory allocated before it was used in any operations. The height and width existed as a quick way to look up the size of one of the matrices. The two-dimensional array was set as an internal value so that it could only be accessed by objects in the RNN project.

Utility Methods

Utility methods for the Matrix class are methods that help in obtaining or setting data in the Matrix class. These methods do not perform any operations on the data however.
The first of these methods are the Get Value and Set Value methods. These values allow users outside of the RNN project to access and set values of the two-dimensional array. Because the array is internal only, these methods provide users with a way to set the value of the array. Each of the methods allow the user to specify what row and column the value should be at. If the row/column is out of bounds, an exception will be thrown. These methods were implemented to prevent memory leaks from occurring.
The next two utility methods to discuss are the Get Row and Get Column methods. An index value was passed into these functions to determine the row/column that would be returned. These methods returned a Vector that represented a single row or column of the current Matrix. Similarly, to the Get Value and Set Value functions, these functions were designed to prevent memory leaks. 
The third type of utility method is the Copy method. This method creates an exact clone of the current Matrix but allocates a new block of memory for it. This enables the user to create a second Matrix that has all the same values as the first but does not occupy the same space in memory as the first.
The final utility method is the Transpose method. This method returns a new Matrix that has the rows and columns of the current Matrix flipped. Each row becomes a column, and each column becomes a row. This method is extremely useful in performing Matrix operations.

Operation Methods

The operations implemented for this project were limited to operations that were used in this project. Although there is a large amount of functionality that could be added to the Matrix class in terms of operations it could compute, only the necessary methods were included in this project. Each operation method alters the value of the Matrix in some way or returns a new Matrix that was determined through operations on Matrix values. 
The first two operation methods were the Add Matrix and Subtract Matrix methods. These methods were both static methods to accepted two matrices as input. These matrices would then have their values added or subtracted if they were the same size. If they were different sizes, an error would be thrown. A new Matrix was then returned that held the results of the operation. The Add Matrix method had another non-static implementation created that added a passed in Matrix to the values of a local Matrix and stored the results in the local Matrix.
The next operation that was added was the Dot Product operation. This operation could only occur between a Vector and a Matrix and returned a new Vector as the result. The Vector could be specified as the first or second input because the dot product is different based on which parameter is on the left or right of the dot. This method could only be performed on a X sized Vector and an X-Y sized Matrix. The new Vector that was created would be of size Y. If the sizes weren’t compatible, an error would be thrown.
The next operation that was implemented was the Matrix Multiplication method. The Matrix Multiplication method was very similar to the Dot Product method, except two Matrices were used. The Matrixes had to be of size X-Y and Y-Z. A new Matrix would then be returned of size X-Z.
The last implemented operation for the Matrix class was the Xavier Initialization method. This method initialized the values of the Matrix to pseudo-random values based on the Xavier Algorithm. The Xavier Algorithm is an Algorithm designed to help reduce the effects of the vanishing gradient problem in recurrent neural networks [4]. This method was implemented to mitigate some of the key problems with recurrent neural networks (namely the vanishing gradient problem).

Vector Class

Properties and Constructor

The Vector class was very similar to the Matrix class. It had a one-dimensional inner array, as well as a read only width value. Each of the two properties was initialized in the constructor.

Utility Methods

The Vector class contained three different utility methods, all of which existed in the Matrix class also.
The first two utility methods were the Set Value and Get Value methods. These methods allowed users to pass in a certain column in which they wanted to get or set a value. If the column was out of bounds an error would be thrown. These two methods were implemented to fight memory leaks.
The last utility method for the Vector class was the Copy method. This method created a new Vector in memory that contained all the same values as the current Vector.

Operation Methods

The first two operation methods implemented for the Vector class were the Add Vector and Subtract Vector methods. These methods were both static methods that accepted two Vectors as input. If the Vectors were of the same size, their values would be added or subtracted from one another. If they weren’t the same size an error would be thrown. A new Vector was returned as output from the method that contained the results of the add/subtract operations.
The next operation that was implemented was the Outer Product method. This was a static method that accepted two Vectors as input and outputted a Matrix. The outputted Matrix would be of size X-Y were X was the size of the first Vector and Y was the size of the second Vector. This operation is used numerous times in neural networks.
The final operation added to the Vector class was the Multiply method. This method accepted a single Vector as input and multiplied its values with that of a local Vector. The results of the multiplication operation were then stored in the local Vector. If the sizes of the two Vectors were unequal an error would be thrown.

Subtle Optimizations

To keep the recurrent neural network running quickly many small optimization features were included in the implementations of the Matrix and Vector operations. The first of these optimizations was a memory access optimization in the Matrix class. In each of the methods in the Matrix class, if a Matrix is looped through it is always looped through row by row, rather than column by column. This optimization strategy reduces the overall number of memory access done by the program significantly. This is because rows are stored together in chunks in memory, and each time a value in a row is accessed, the whole chunk is loaded into the cache. By progressing row by row, we reduce the number of chunks that must be loaded into the cache because we are accessing all the values in the current chunk before moving on to a different chunk. If you progress column by column a new chunk is loaded each time a value is accessed. This optimization is very apparent in the Matrix Multiplication method. This optimization reduced the overall run time of the operation by 80% in performed trials of 1000x1000 size Matrices. 
The next optimization is very similar to the previous optimization but involves the Vector class. Vectors can only be of size 1-X rather than of size X-1. This is because, as previously stated, rows are cached together. By only allowing rows we reduce the overall number of chunks that must be loaded into the memory cache. 
The next optimization strategy was to use temporary variables to store the values being computed in looping operations. This strategy reduces the amount of memory writes and lookups that must occur by storing a value in a temporary variable. This strategy is very useful in summation operations on array values because the array values don’t have to be written to in every iteration of the summation. Rather a single temporary variable is used for quick reads and writes. 
Other than the three strategies mentioned above, the overall strategy of the project was to reduce the number of operations that had to be completed by as much as possible. This involved making sure each variable was declared in as high of a scope as needed (to avoid a variable being created in each instance of a loop, which would cause memory allocation load times).

Data Preparation

The purpose of the Data Preparation class was to create a library that could assist users in preparing text data to be run neural networks. The Data Preparation library has one main class, the Text Preparation class, and multiple other utility classes.

Text Preparation

Tokenize

The text preparation class is a static class that can be used to prepare data for use. The first method in the class is the Tokenize method. This method takes in a string of content and splits it into sentences and words. 
Two classes are used to support this method: the sentence and word methods. The word class contains a single string parameter that represents a word. The sentence class contains a list of words as well as a word count integer that returns the number of words in the sentence. 
The method has a variety of parameters that can be inputted into it. The method requires a string of content to be passed in and an integer value that represents the maximum number of sentences that should be returned. It also can accept multiple conditional parameters. The first two of these parameters are integer values that represent the maximum and minimum words that each sentence can contain. 
The next conditional parameter is the text splitter mode. This is an enum value that can be set to Random, In Order, and Equally Split. This value controls how sentences are removed from the content. The In Order mode means that sentences will be taken sequentially from the content. The Random mode means that sentences will be taken randomly from the content (there can be duplicate sentences using this mode). The Equally Split node takes sentences that are equidistantly apart from one another in the content. The distance is found by taking the length of the content and dividing it by the maximum number of sentences it can return.
The final conditional parameter is the case sensitive parameter. This parameter controls how words are taken from the content. If this value is set to false, all letters in each word are converted to lower case. This can help data processing because it ensures the two of the same words will use the same character; for example, The and the would be the same word. If this is set to false, each word is taken from the content as in. 
The purpose of this function is to generate the data that will be used by the RNN. Splitting the content into sentences and words also makes other data preparation steps much easier to process.
This tokenization implementation handles separating words similarly to other implementations. Special characters like semi colons and commas are treaded as their own words. If a word is split by an apostrophe the letters before the apostrophe are treated as one word while the apostrophe and the words after it are treated as their own word. This is done because these words tend to mean two separate words. All of these design choices fit in line with other implementation; for example Britz’s implementation works the same way [3]. 

Remove Infrequent Words

The next method that will be discussed is the Remove Frequent Words method. The purpose of this method is remove all infrequent words from the dataset. This is useful because it shrinks the size of the input matrix substantially. The input matrix’s size is set equal to the size of the word bank being used, so this has a very direct effect on the RNNs processing time.
The method accepts three parameters as input: a list of sentences, an integer value that represents the number of words to keep, and a blank word token string value. The list of sentences represents the sentences that should be modified, the number of words to keep represents the size of the word bank that should be used, and the blank word token represents a string value that the removed words should be replaced with. Removed words aren’t left as blanks because this would drastically change the size and meaning of the sentence. Instead words are kept as blank values to show that a word should be placed at this point in the sentence.
The method begins by getting the number of times each word is used in all the sentences. The top X words are then kept, while each other word is replaced by the blank word token. The modified list of sentences is then returned when the method completes.

Add Beginning and End Tokens

The third method implemented in the text preparation class was the Add Beginning and End Token method. This method accepted a list of sentences and appended a start and stop token to the begin and end of the sentence. The purpose of this method is to give each sentence a common starting and ending position. Because the RNN are trained using back propagation through time this gives a node before and after the actual last and first words so that an additional BPPT step can be taken. 
This method was implemented after view Britz’s implementation of an RNN [3]. After some initial tests however, it was found that this feature was not needed for this neural network. Because this project focuses solely on categorization, processing the default start and end tokens in the back propagation through time steps is merely encumber some. As shown in the Evaluation section of this report removing this step from the data preparation actually increased the speed and accuracy of the RNN. 

Mapping Words

The last major method contained in the Text Preparation class is the Map Sentences method. This method accepts a list of sentences as input. It then creates a word map based on the words in each sentence. A word map is an object that maps each word in the word bank to a certain integer number. This action is performed because the RNN will not be able to process string values, it needs numeric values. After creating the word map the method loops through each sentence and converts each word into its integer mapped value. A list of these converted sentences is then sent out of the method as the output.

Utility Methods

A few utility methods also exist in the Text Preparation class. The first of these methods is the Get Word Counts method. This method loops through a list of sentences and returns the number of times each word was used in the sentence. This number of words is returned as a list of Word Count class objects. The Word Count class contains a specific word and the number of times the sentences contained it.
The next utility function was Get All Words function. This function accepted a list of sentences as input and returned a list of each word in each of those sentences. This method does not return distinct values however, so you can get back multiple copies of each word if they are used multiple times.
The next utility method was the At End Of Sentence method. This method accepted a character as an input and was designed to tell if you if you were at the end of the sentence. If the method found that you were at the end of the sentence it would return true, otherwise it would return false. The end of the sentence was designed by the function as the character ‘.’, ‘!’, or ‘?’. If any of these characters were found the method would return true.
The last utility method was the Get Word Map method. This method would return a map of words given a list of sentences. This method was used by the Map Sentences method to generate the Word Map used to map each word in each sentence to a specific integer value. 

Recurrent Neural Network Class

The recurrent neural network class contains an instance of a recurrent neural network. It stores all the matrices used in training and processing the neural network so that they can be reused over many tests/inputs.

Properties and Constructor

The RNN class contains 14 different properties. The first three properties are read only properties that can only be set by the constructor. These properties are the Input Dimensions, the Output Dimensions, and the Hidden Layer Dimensions. The reason that these variables can only be set by the constructor is because if they change all the matrices stored in the neural network would also need to change. These changes would result in a completely new neural network. Since this is the same result as creating a new neural network, it was made so that these sizes can only be decided once.
The next three properties are the U, V, and W matrices. These matrices are private variables that are used to train the neural network, as well as process feed forward equations. These matrices represent the weight matrices of the RNN. For information regarding their purpose, refer to the Systems Specification section.
The next four properties are the Output Matrix, Hidden State Matrix, Output Net Matrix, and the Output Hidden State Net Matrix. The Hidden State Matrix contains the current values of the hidden state nodes in the RNN. These values are updated each time the Forward Propagation method is called. The Output Matrix contains the outputted values of the last run Forward Propagation method. The versions of these two parameters that have Net at the end of their names represent each of the matrices values before the activation function was run on the variables. These variables are stored because they would have to be found again during the back propagation through time algorithm if they weren’t stored.
Three additional properties were also created as Get functions for the U, V, and W matrices. These three matrices are all private and should not be edited, so the only a Get function was implemented for them. The Getter functions are named U_Matrix, V_Matrix, and W_Matrix and exist so that objects outside of the RNN class can see what values are in the matrices.
The final property in the RNN class is the Bppt_Truncate value. This value is set by the Training method of the RNN. It represents the depth that the back propagation through time, BPPT, algorithm should reach each time the BPPT is run. For instance, if a sentence had 8 words and you were at the first time-step (on word 8) and the Bppt Truncate values was set to 3, the BPPT algorithm would be run on words 8 through 8-3.
The RNN class also contains two constructors. Both constructors take in the Input Dimensions, Hidden Layer Dimensions, and the Output Dimensions and set their values. The second constructor has some additional functionality however, as it allows you to pass in values for V, U, and W. This functionality was implemented so that you could load previously run RNN into a new neural network. 
The other constructor initializes the values of V, U, and W using the Matrix classes Xavier Initialization method. This method initializes the values to pseudo random values that should mitigate the vanishing gradient problem RNN’s tend to have [4].

Public Methods

The RNN class has two public methods: The Forward Propagation method and the Train method.

Forward Propagation

The Forward Propagation method processes the outputs of the RNN for a single input matrix. The matrix can have any number of words but must have a consistent word bank size (the width of the matrix). The method begins by initializing new Output, Hidden State, Output Net, and Hidden State Net matrices based on the number of time steps the Input Matrix contains. The method then enters two for loops that calculate the values of the Hidden States, and then the values of the Output. These values are calculated using the equations described in the System Specification section. The Hidden State Net and Output Net matrices are also set to the Output and Hidden State values before they were run through the activation function. This is done simply to speed up the Back Propagation Through Time method, which will be discussed later.
The activation function used by the Forward Propagation method is the Tanh. This activation function was chosen because it always results in a value between -1 and 1. Because this is a classification problem we only need to find the Output node with the highest value, the exact value isn’t very important. Tanh also counters the exploding gradient problem because it limits the size of the Output values and the Hidden State values. 

Train Method

The Train Method in the RNN class is the method that trains the neural net using certain inputs and expected outputs. The method accepts 5 parameters as input: a list of input vectors, a list of expected output vectors, the learning rate, the number of epochs, and the bppt truncate value. The list of inputs and expected outputs represent the training data that should be used. Each input vector will be a sentence and each node will a word in that sentence. The expected output will contain the expected result of running each word through the Forward Propagation method. 
The learning rate represents the rate at which the RNN should be trained. This value is generally kept very small so that each sentence being run through the RNN doesn’t cause any drastic changes. By keeping this value small, changes in the RNN will slowly build up over tens of thousands of tests.
The number of epochs represents the number of times the RNN should train the data using the passed in input and output matrices. The data is processed multiple times to try and further refine the weight matrices to improve the accuracy of the RNN.
The last value that can be passed in is the BPPT truncate value. The BPPT stands for back propagation through time in this case. This value represents the maximum depth that each iteration of the back propagation through time procedure should go when training the data. For instance, if you had a sentence with 7 words and a BPPT truncate value of 3 and you were at time step 6 in the BPPT method, only the words from time step 6 to 6-3 would be used. This value is a conditional parameter that if kept at the default value does nothing. This value was added to speed up the RNN in cases where the depth was very large. 
The method contains a For loop that runs each input/output pair through a training step method a number of times equal to the epoch value. The training step method is a private method that will be discussed below.

Private Methods

The RNN class contains four private methods that are used to process the neural network. These methods are the Training Step method, the Back Propagation Through Time method, the Tanh method, and the Tanh derivation method. 

Training Step

The training step method accepts three parameters as its input: An Input vector, an Output vector, and the learning rate. The method begins by calling the Back Propagation Through Time method. This method returns the gradients for the U, V, and W matrices so that they can be used to train the RNN. After receiving the values from the BPPT method, each gradient is looped through to see if the gradient is very large. Any gradient that is higher than 5, or lower than -5, is capped out at 5 or -5. The purpose of these loops is to rescale the values whenever they go over a threshold to help mitigate the exploding gradient problem [3]. Once the values have been scaled down and multiplied by the learning rate they are added to the U, V, and W matrices to train the data.

Back Propagation Through Time

The purpose of the back propagation through time method is to compute the gradients for the weight matrices. The method accepts two parameters as input, the Input vector and the Expected Output vector. The first thing that the BPPT method does is run forward propagation using the passed in Input vector. This is done to set the Output Matrix and Hidden State Matrix to correct values for the passed in inputs. The general Back Propagation Through Time algorithm is then run on the data to find the gradient values. Detailed information on how the algorithm runs is explained in the System Specification section.
The algorithm begins by computing the Delta Output matrix. Once the Delta Output is calculated a loop is entered that processes each time step of the input (the width of the input). Each time step alters the values of the W, U, and V gradients based on the Hidden State values at that time step, as well as the input and Delta Output values. An inner loop also exists inside the time step loop that processes the backpropagation through time elements of the algorithm.

Tanh and Tanh Derivation

The Tanh and Tanh Derivation methods are used by the Back Propagation Through Time and Forward Propagation methods as activation methods. The Tanh method is used in the Forward Propagation method to scale every value between -1 and 1. The Tanh Derivation method is used by the BPPT method to calculate the weight gradients. 
The Tanh method accepts a double value as input and returns a new double that has been run through the Math.Tanh method.
The Tanh Derivation method accepts a double as input and returns a new double that represents the derivative of the Tanh method for that input.
Tanh was used as the activation method for this RNN because the neural network was working on a classification problem. In a classification problem you only need to see which value is most recommended by the RNN. Although the Tanh method does tend to scrunch numbers together towards its limits (-1 and 1) it is unlikely that this will be a large problem in a classification problem. Merely seeing which number is closest to 1 is what the neural network needs to do. 
The normal choice for a feed forward recurrent neural network is the rectifier method. This method was not chosen for this project because it greatly increases the chances of the exploding gradient problem from occurring because it allows for values passed 1 and is exponential in nature. Although the rectifier gives more distinct values the tanh method, these values are not needed in a classification problem. Overall the negatives of using the rectifier method seemed higher than the downside of using the tanh method.

Main Window

The class contains a constructor that loads the GUI and allocates memory for the list of IO records. The GUI for this project was created using WPF (Windows Presentation Framework). The Main Window object is what connects the Data Preparation classes to the Recurrent Neural Network classes. It handles preparing the data and running the data through the RNN. It also handles testing the data after the RNN has been trained.

Properties and Constructor

The Main Window contains eight properties that are used by the application. The first property is labeled Number Of Records To Keep. This property represents the size of the word bank that should be used for the RNN. This data also controls the number of words that will be kept from the content of the books. This value is used in the Text Preparation’s Remove Infrequent Words algorithm. 
The next property that the Main Window uses is a list of Sentence IO Records. A Sentence IO Record is a class object that contains a list integer that represent the inputs of an RNN value as well as the output for those inputs. Each Sentence IO Record is used to contain a single input record for the RNN. This list is used a property of the Main Window so that the data only must be loaded a single time.
The third property of the class is the Recurrent Neural Network class object itself. Because we want to be able to perform multiple tests on a single RNN it is kept inside the Main Window class itself. This way we don’t have to reload the RNN every time we want to use it for something. The RNN is normally only set a single time when you first open the program.
The next property used by the class is the Number of Batches property. This property controls how many batches the training data will be split into before its sent into the RNN to train the network. This property was set to a fixed value of 1 for this project, so all training data was sent in a single batch.
The fifth property is the Number of Epochs property. This property controller the number of epochs that would be used to train the RNN. This value was often changed while testing the RNN.
The sixth property was the Percent Training Data property. This property controlled what percentage of the data would be used as training data. This value was constantly changed while testing for analyzation purposes.
The last two properties were the Current Directory and Saved States Directory strings. These strings kept track of the directory the project was in, as well as the directory that should be used to save RNN records.
The constructor for this class initializes the GUI, allocates memory for the IO Records list, and then sets the Current Directory and Saved States Directory strings.

Main Methods

The Main Window class contains 6 main methods, as well as several utility methods.

Load IO Records

The Load IO Records method loads the data from the 15 books into the classes IO Records list. This is done by loading the content of each book, tokenizing it using the Data Preparation class, having its infrequent words removed using the Data Preparation class, mapping each sentence using a word map using the Data Preparation class, and then adding a new IO Record for each sentence returned by the word mapping algorithm.

Save NN State

The second method created was the Save NN State method. This method saves the weight matrices of the current RNN into a text file. The text file is saved at the Saved States Directory string file path. The name of the file is equivalent to the current month + the current day + the current hour + the current minute + .txt. Each value in the weight matrices is saved using the format (Matrix Name)_(row)_(column)_(value). This format allows easy looking up when the record is being loaded.

Load NN State

The next method used was the Load NN State method. This method loads a previously created RNN into memory. Only the weight matrices of the old RNN are loaded this way however, because the other values change after each forward propagation run anyways. The Load NN State algorithm accepts the record name as input. The record name is a string that represents the file name of the record to be loaded. This file should reside in the Saved States Directory. Once the matrices are loaded, a new RNN is created with the passed in weight values.

Test All

The Test All method runs each IO Record value through the Forward Propagation method of the RNN. It then compares the output of the RNN to the expected output. It then catalogues what rank the expected genre received in the RNN. The rank is how close it was to being the chosen value. Rank 1 means that the expected value was chosen, rank 5 means the expected value was the least likely to be picked. These numbers are then outputted in a text box for retrieval.

Test Random Record

The Test Random Record method runs a single random IO Record through the RNN and outputs its expected result and actual result into two different text boxes. This method exists as a quick way to look at how the RNN is doing.

Train

The last major method is the Train method. This method Trains the RNN using the data loaded into the IO Records list, and using the parameters manually set for the Main Window class. The records chosen as training data are randomly picked from each of the five genres, and the program tries to get as close to an equal split used as possible. The portions of the training data are run through the training algorithm of the RNN class a number of times equal to the number of batches, with the RNN training with the data a number of times equal to the Epoch parameter. Once the training has completed the weight matrices are saved using the Save NN States method. 

Utility Methods

Several utility methods are used in the Main Window class to assist the major methods in completing tasks.

Get Genre Type

This method takes in the Genre ID as the input parameter and returns the genre string that the ID represents.

Get Sentence From Book

This method takes in a book URL and returns all the sentence from that book. This method loads the content into a string, and then sends the content through the Data Preparation classes Tokenize method to split it into sentences. 

Shuffle List

The Shuffle List method shuffles a list of any type of object into a new equally sized list. This is done using the Random C# class to pick which value goes where in the new list.

Split IO Records

This method splits the IO records into training data and test data. It accepts the percentage to split into training data as a parameter. The list is then separated into two lists, the training and test data, and the method attempts to load an equal number of each genre into the training data. The training data is then shuffled, and the lists are then recombined at the end of the method and returned as one list.
