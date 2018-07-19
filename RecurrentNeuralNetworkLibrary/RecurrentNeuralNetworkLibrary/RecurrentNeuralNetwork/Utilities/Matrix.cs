using Heuristics.Utilities.Matrices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heuristics.Utilities.Matrices
{
    /// <summary>
    /// Represents a matrix
    /// </summary>
    public class Matrix
    {
        #region Properties

        /// <summary>
        /// Represents the height of the matrix
        /// </summary>
        public readonly int Height;

        /// <summary>
        /// The matrix values in memory as an array.
        /// </summary>
        internal double[,] InnerMatrix;

        /// <summary>
        /// Represents the width of the matrix
        /// </summary>
        public readonly int Width;

        #endregion Properties

        #region Constructor

        public Matrix(int height, int width)
        {
            InnerMatrix = new double[height, width];
            Height = height;
            Width = width;
        }

        #endregion Constructor

        #region Methods

        #region Operations

        /// <summary>
        /// Adds the passed in matrix to the current matrix
        /// </summary>
        /// <param name="i"></param>
        public void AddMatrix(Matrix i)
        {
            // Cannot add uneven matrices
            if (i.Height != Height || i.Width != Width)
            {
                throw new Exception("Cannot add uneven matrices!");
            }

            for (int p = 0; p < i.Height; p++)
            {
                for (int q = 0; q < i.Width; q++)
                {
                    InnerMatrix[p, q] = i.InnerMatrix[p, q] + InnerMatrix[p, q];
                }
            }
        }

        /// <summary>
        /// Adds the two passed in matrices, returns the result
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public static Matrix AddMatrix(Matrix i, Matrix j)
        {
            if(i.Height != j.Height || i.Width != j.Width)
            {
                throw new Exception("Cannot add uneven matrices!");
            }

            Matrix result = new Matrix(i.Height, i.Width);

            for(int p = 0; p < i.Height; p++)
            {
                for(int q = 0; q < i.Width; q++)
                {
                    result.InnerMatrix[p, q] = i.InnerMatrix[p, q] + j.InnerMatrix[p, q];
                }
            }

            return result;
        }

        /// <summary>
        /// Calculates the dot product of a matrix and vector
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public static Vector DotProduct(Matrix A, Vector B)
        {
            if (A.Width != B.Width)
            {
                throw new Exception("To perform dot product the first matrix must have a width equal to the seconds height!");
            }

            Vector result = new Vector(A.Height);

            double c;
            for (int i = 0; i < A.Width; i++)
            {
                c = 0;
                for(int j = 0; j < B.Width; j++)
                {
                    c = A.InnerMatrix[i, j] * B.InnerVector[j];
                }
                result.InnerVector[i] = c;
            }

            return result;
        }

        /// <summary>
        /// Calculates the dot product of a matrix and vector
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public static Vector DotProduct(Vector A, Matrix B)
        {
            if (A.Width != B.Width)
            {
                throw new Exception("To perform dot product the first matrix must have a width equal to the seconds height!");
            }

            Vector result = new Vector(B.Width);

            double c;
            for (int i = 0; i < A.Width; i++)
            {
                c = 0;
                for (int j = 0; j < B.Width; j++)
                {
                    c = A.InnerVector[i] * B.InnerMatrix[i, j];
                }
                result.InnerVector[i] = c;
            }

            return result;
        }

        /// <summary>
        /// Calculates the dot product of each row and column of each matrix into a new matrix.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public static Matrix MatrixMulitplication(Matrix A, Matrix B)
        {
            if (A.Width != B.Height)
            {
                throw new Exception("To perform dot product the first matrix must have a width equal to the seconds height!");
            }

            Matrix result = new Matrix(A.Height, B.Width);
            double c;

            // Use the tranpose so we can loop through row by row. Reduces the number of chunks loaded
            B = B.Transpose();
            for (int i = 0; i < A.Height; i++)
            {
                for (int j = 0; j < B.Height; j++)
                {
                    c = 0;
                    for (int k = 0; k < A.Width; k++)
                    {
                        c += A.InnerMatrix[i, k] * B.InnerMatrix[j, k];
                    }
                    result.InnerMatrix[i, j] = c;
                }
            }

            return result;
        }

        /// <summary>
        /// Performs the passed in operation on each value in the matrix individually.
        /// </summary>
        /// <param name="Operation"></param>
        public void PerformOperationAsDouble(Func<double, double> Operation)
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    InnerMatrix[i, j] = Operation(InnerMatrix[i, j]);
                }
            }
        }

        /// <summary>
        /// Performs the passed in operation on each value in the matrix individually.
        /// </summary>
        /// <param name="Operation"></param>
        public Matrix ReturnOperationAsDouble(Func<double, double> Operation)
        {
            Matrix newMatrix = new Matrix(Height, Width);
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    newMatrix.InnerMatrix[i, j] = Operation(InnerMatrix[i, j]);
                }
            }

            return newMatrix;
        }
        
        /// <summary>
        /// Subtracts matrix j from matrix i
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public static Matrix SubtractMatrices(Matrix i, Matrix j)
        {
            if (i.Height != j.Height || i.Width != j.Width)
            {
                return null;
            }

            Matrix result = new Matrix(i.Height, i.Width);

            for (int p = 0; p < i.Height; p++)
            {
                for (int q = 0; q < i.Width; q++)
                {
                    result.SetValue(p, q, i.GetValue(p, q) - j.GetValue(p, q));
                }
            }

            return result;
        }


        /// <summary>
        /// Initializes the matrix using Xavier Initialization
        /// </summary>
        /// <param name="NumberOfInputNeurons"></param>
        public void XavierInitialization(int NumberOfInputNeurons)
        {
            double Variance = 2 / ((double)NumberOfInputNeurons-1);
            double StandardDev = Math.Sqrt(Variance);
            double Value;

            double v1, v2;
            double w;
            Random r = new Random(Guid.NewGuid().GetHashCode());

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    v1 = r.NextDouble() - .5;
                    v2 = r.NextDouble() - .5;

                    w = v1 * v1 + v2 * v2;

                    Value = Math.Sqrt(-2 * Math.Log(w) / w) * StandardDev;

                    if(r.NextDouble() > .5)
                    {
                        Value *= v1;
                    }
                    else
                    {
                        Value *= v2;
                    }

                    SetValue(i, j, Value);
                }
            }
        }
        #endregion Operations

        #region Utilities


        /// <summary>
        /// Sets the row at the passed in index to the passed in rows values
        /// </summary>
        /// <param name="Row"></param>
        /// <param name="RowIndex"></param>
        public void AddRowToMatrix(double[] Row, int RowIndex)
        {
            if (Row.Length == Width)
            {
                for (int i = 0; i < Row.Length; i++)
                {
                    InnerMatrix[RowIndex, i] = Row[i];
                }
            }
        }

        /// <summary>
        /// Creates a copy of the current matrix that has allocated its own memory
        /// </summary>
        /// <returns></returns>
        public Matrix Copy()
        {
            Matrix NewMatrix = new Matrix(Height, Width);

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    NewMatrix.InnerMatrix[i, j] = InnerMatrix[i, j];
                }
            }

            return NewMatrix;
        }

        /// <summary>
        /// Aquires a column of matrix as a vector
        /// </summary>
        /// <param name="ColumnIndex"></param>
        /// <returns></returns>
        public Vector GetColumn(int ColumnIndex)
        {
            Vector column = new Vector(Height);
            for (int i = 0; i < Height; i++)
            {
                column.InnerVector[i] = InnerMatrix[i, ColumnIndex];
            }
            return column;
        }

        /// <summary>
        /// Aquires a row of a matrix as a vector
        /// </summary>
        /// <param name="RowIndex"></param>
        /// <returns></returns>
        public Vector GetRow(int RowIndex)
        {
            Vector row = new Vector(Width);
            for (int i = 0; i < Width; i++)
            {
                row.InnerVector[i] = InnerMatrix[RowIndex, i];
            }
            return row;
        }

        /// <summary>
        /// Gets the value of the cell
        /// </summary>
        /// <param name="Row"></param>
        /// <param name="Column"></param>
        /// <returns></returns>
        public double GetValue(int Row, int Column)
        {
            if (Height > Row && Width > Column)
            {
                return InnerMatrix[Row, Column];
            }

            throw new IndexOutOfRangeException("Matrix call is out of bounds!");
        }

        /// <summary>
        /// Sets the value of the column equal to the passed in values.
        /// </summary>
        /// <param name="ColumnValues"></param>
        /// <param name="ColumnNumber"></param>
        public void SetColumn(Matrix ColumnValues, int ColumnNumber)
        {
            if (ColumnValues.Width > 1)
            {
                throw new Exception("Passed in matrix must have a height of 1!");
            }

            if (ColumnValues.Height != Height)
            {
                throw new Exception("Passed in matrix must have same width as this matrix!");
            }

            for (int i = 0; i < ColumnValues.Height; i++)
            {
                SetValue(i, ColumnNumber, ColumnValues.GetValue(i, 0));
            }
        }

        /// <summary>
        /// Sets the values of the matrix to the values of the passed in array.
        /// </summary>
        /// <param name="matrix"></param>
        public void SetMatrixToTwoDimensionalArray(double[,] matrix)
        {
            if (matrix.GetLength(0) == Height && matrix.GetLength(1) == Width)
            {
                for (int row = 0; row < Height; row++)
                {
                    for (int column = 0; column < Width; column++)
                    {
                        InnerMatrix[row, column] = matrix[row, column];
                    }
                }
            }
        }

        /// <summary>
        /// Sets the value of the row equal to the passed in values.
        /// </summary>
        /// <param name="RowValues"></param>
        /// <param name="RowNumber"></param>
        public void SetRow(Vector RowValues, int RowNumber)
        {
            if (RowValues.Width != Width)
            {
                throw new Exception("Passed in matrix must have same width as this matrix!");
            }

            for (int i = 0; i < RowValues.Width; i++)
            {
                InnerMatrix[RowNumber, i] = RowValues.InnerVector[i];
            }
        }

        /// <summary>
        /// Sets the value of the cell to the passed in value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="Value"></param>
        public void SetValue(int row, int column, double Value)
        {
            if (Width > column && Height > row)
            {
                InnerMatrix[row, column] = Value;
            }
        }

        /// <summary>
        /// Returns the transpose of the matrix
        /// </summary>
        /// <returns></returns>
        public Matrix Transpose()
        {
            Matrix transpose = new Matrix(Width, Height);

            for (int row = 0; row < Height; row++)
            {
                for (int column = 0; column < Width; column++)
                {
                    transpose.InnerMatrix[column, row] = InnerMatrix[row, column];
                }
            }

            return transpose;
        }

        #endregion Utilities

        #endregion Methods
    }
}
