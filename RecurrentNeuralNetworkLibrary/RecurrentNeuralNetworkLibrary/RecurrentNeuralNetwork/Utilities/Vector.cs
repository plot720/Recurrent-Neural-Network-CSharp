using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heuristics.Utilities.Matrices
{
    public class Vector
    {
        #region Properties

        /// <summary>
        /// The matrix values in memory as an array.
        /// </summary>
        internal double[] InnerVector;

        /// <summary>
        /// Represents the width of the matrix
        /// </summary>
        public readonly int Width;

        #endregion Properties

        #region Constructor

        public Vector(int width)
        {
            InnerVector = new double[width];
            Width = width;
        }

        #endregion Constructor

        #region Methods

        #region Operations

        /// <summary>
        /// Adds two vectors together
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public static Vector AddVector(Vector i, Vector j)
        {
            if (i.Width != j.Width)
            {
                throw new Exception("Cannot add uneven vectors!");
            }

            Vector result = new Vector(i.Width);
            
            for (int q = 0; q < i.Width; q++)
            {
                result.InnerVector[q] = i.InnerVector[q] + j.InnerVector[q];
            }

            return result;
        }

        /// <summary>
        /// Calculates the dot product of two vectors
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public static double DotProduct(Vector A, Vector B)
        {
            if(A.Width != B.Width)
            {
                throw new Exception("Cannot perform dot product for differently sized vectors!");
            }
            Vector result = new Vector(B.Width);
            
            double c = 0;
            for (int k = 0; k < A.Width; k++)
            {
                c += A.InnerVector[k] * B.InnerVector[k];
            }
            
            return c;
        }
        
        /// <summary>
        /// Performs the passed in operation on each value in the vector individually.
        /// </summary>
        /// <param name="Operation"></param>
        public void PerformOperationAsDouble(Func<double, double> Operation)
        {
            for (int j = 0; j < Width; j++)
            {
                InnerVector[j] = Operation(InnerVector[j]);
            }
        }

        /// <summary>
        /// Computes the outer product matrix of two vectors
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static Matrix OuterProduct(Vector A, Vector B)
        {
            Matrix m = new Matrix(A.Width, B.Width);

            for(int i = 0; i < A.Width; i++)
            {
                for(int j = 0; j < B.Width; j++)
                {
                    m.InnerMatrix[i, j] = A.InnerVector[i] * B.InnerVector[j];
                }
            }

            return m;
        }

        /// <summary>
        /// Performs the passed in operation on each value in the vector individually.
        /// </summary>
        /// <param name="Operation"></param>
        public Vector ReturnOperationAsDouble(Func<double, double> Operation)
        {
            Vector newVector = new Vector(Width);
            
            for (int j = 0; j < Width; j++)
            {
                newVector.InnerVector[j] = Operation(InnerVector[j]);
            }

            return newVector;
        }

        /// <summary>
        /// Subtracts vector j from vector i
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public static Vector SubtractVectors(Vector i, Vector j)
        {
            if (i.Width != j.Width)
            {
                return null;
            }

            Vector result = new Vector(i.Width);
            
            for (int q = 0; q < i.Width; q++)
            {
                result.InnerVector[q] = i.InnerVector[q] - j.InnerVector[q];
            }

            return result;
        }

        /// <summary>
        /// Multiplies one vector by another vector
        /// </summary>
        /// <param name="A"></param>
        public void Multiply(Vector A)
        {
            if(A.Width != Width)
            {
                throw new Exception("Vectors must be same size to multiply!");
            }

            for(int i = 0; i < Width; i++)
            {
                InnerVector[i] *= A.InnerVector[i];
            }
        }

        #endregion Operations

        #region Utilities

        /// <summary>
        /// Gets the value of the cell
        /// </summary>
        /// <param name="Row"></param>
        /// <param name="Column"></param>
        /// <returns></returns>
        public double GetValue(int Column)
        {
            if (Width > Column)
            {
                return InnerVector[Column];
            }

            throw new IndexOutOfRangeException("Matrix call is out of bounds!");
        }

        /// <summary>
        /// Sets the value of the cell to the passed in value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="Value"></param>
        public void SetValue(int column, double Value)
        {
            if (Width > column)
            {
                InnerVector[column] = Value;
            }
        }

        /// <summary>
        /// Creates a copy of the vector that has allocated its own memory
        /// </summary>
        /// <returns></returns>
        public Vector Copy()
        {
            Vector NewVector = new Vector(Width);

            for (int j = 0; j < Width; j++)
            {
                NewVector.InnerVector[j] = InnerVector[j];
            }

            return NewVector;
        }

        #endregion Utilities

        #endregion Methods

    }
}
