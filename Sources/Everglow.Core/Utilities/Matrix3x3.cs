using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Everglow.Commons.Utilities
{
	public class Matrix3x3
	{
		public static Matrix3x3 Identity = new Matrix3x3(new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1));
		private double[,] _matrix;

		public Matrix3x3()
		{
			_matrix = new double[3, 3];
		}
		public Matrix3x3(double[,] initialMatrix)
		{
			if (initialMatrix.GetLength(0) != 3 || initialMatrix.GetLength(1) != 3)
				throw new ArgumentException("Initial matrix must be 3x3");

			_matrix = initialMatrix;
		}

		public double this[int row, int col]
		{
			get
			{
				return _matrix[row, col];
			}
			set
			{
				_matrix[row, col] = value;
			}
		}

		public Matrix3x3(Vector3 col0, Vector3 col1, Vector3 col2)
		{
			_matrix = new double[3, 3];
			_matrix[0, 0] = col0.X;
			_matrix[1, 0] = col0.Y;
			_matrix[2, 0] = col0.Z;

			_matrix[0, 1] = col1.X;
			_matrix[1, 1] = col1.Y;
			_matrix[2, 1] = col1.Z;

			_matrix[0, 2] = col2.X;
			_matrix[1, 2] = col2.Y;
			_matrix[2, 2] = col2.Z;
		}

		public static Matrix3x3 operator *(Matrix3x3 a, double x)
		{
			double[,] data = new double[3, 3];
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					data[i, j] = a._matrix[i, j] * x;
				}
			}

			return new Matrix3x3(data);
		}

		public static Matrix3x3 operator -(Matrix3x3 a, Matrix3x3 b)
		{
			double[,] data = new double[3, 3];
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					data[i, j] = a._matrix[i, j] - b._matrix[i, j];
				}
			}

			return new Matrix3x3(data);
		}

		public static Matrix3x3 operator +(Matrix3x3 a, Matrix3x3 b)
		{
			double[,] data = new double[3, 3];
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					data[i, j] = a._matrix[i, j] + b._matrix[i, j];
				}
			}

			return new Matrix3x3(data);
		}

		public static Matrix3x3 operator /(Matrix3x3 a, Matrix3x3 b)
		{
			double[,] data = new double[3, 3];
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					data[i, j] = a._matrix[i, j] / b._matrix[i, j];
				}
			}

			return new Matrix3x3(data);
		}



		public Matrix3x3 Transpose()
		{
			double[,] m = new double[3, 3];
			m[0, 0] = _matrix[0, 0];
			m[0, 1] = _matrix[1, 0];
			m[0, 2] = _matrix[2, 0];

			m[1, 0] = _matrix[0, 1];
			m[1, 1] = _matrix[1, 1];
			m[1, 2] = _matrix[2, 1];

			m[2, 0] = _matrix[0, 2];
			m[2, 1] = _matrix[1, 2];
			m[2, 2] = _matrix[2, 2];
			return new Matrix3x3(m);
		}

		public Matrix3x3 Multiply(Matrix3x3 other)
		{
			double[,] result = new double[3, 3];

			result[0, 0] = _matrix[0, 0] * other._matrix[0, 0] + _matrix[0, 1] * other._matrix[1, 0] +
						   _matrix[0, 2] * other._matrix[2, 0];
			result[0, 1] = _matrix[0, 0] * other._matrix[0, 1] + _matrix[0, 1] * other._matrix[1, 1] +
						   _matrix[0, 2] * other._matrix[2, 1];
			result[0, 2] = _matrix[0, 0] * other._matrix[0, 2] + _matrix[0, 1] * other._matrix[1, 2] +
						   _matrix[0, 2] * other._matrix[2, 2];

			result[1, 0] = _matrix[1, 0] * other._matrix[0, 0] + _matrix[1, 1] * other._matrix[1, 0] +
						   _matrix[1, 2] * other._matrix[2, 0];
			result[1, 1] = _matrix[1, 0] * other._matrix[0, 1] + _matrix[1, 1] * other._matrix[1, 1] +
						   _matrix[1, 2] * other._matrix[2, 1];
			result[1, 2] = _matrix[1, 0] * other._matrix[0, 2] + _matrix[1, 1] * other._matrix[1, 2] +
						   _matrix[1, 2] * other._matrix[2, 2];

			result[2, 0] = _matrix[2, 0] * other._matrix[0, 0] + _matrix[2, 1] * other._matrix[1, 0] +
						   _matrix[2, 2] * other._matrix[2, 0];
			result[2, 1] = _matrix[2, 0] * other._matrix[0, 1] + _matrix[2, 1] * other._matrix[1, 1] +
						   _matrix[2, 2] * other._matrix[2, 1];
			result[2, 2] = _matrix[2, 0] * other._matrix[0, 2] + _matrix[2, 1] * other._matrix[1, 2] +
						   _matrix[2, 2] * other._matrix[2, 2];

			return new Matrix3x3(result);
		}

		public Vector3 Multiply(Vector3 v)
		{
			return new Vector3((float)(_matrix[0, 0] * v.X + _matrix[0, 1] * v.Y + _matrix[0, 2] * v.Z),
				(float)(_matrix[1, 0] * v.X + _matrix[1, 1] * v.Y + _matrix[1, 2] * v.Z),
				(float)(_matrix[2, 0] * v.X + _matrix[2, 1] * v.Y + _matrix[2, 2] * v.Z));
		}

		public double Determinant()
		{
			return _matrix[0, 0] * (_matrix[1, 1] * _matrix[2, 2] - _matrix[1, 2] * _matrix[2, 1]) -
				   _matrix[0, 1] * (_matrix[1, 0] * _matrix[2, 2] - _matrix[1, 2] * _matrix[2, 0]) +
				   _matrix[0, 2] * (_matrix[1, 0] * _matrix[2, 1] - _matrix[1, 1] * _matrix[2, 0]);
		}

		public Matrix3x3 Inverse()
		{
			double det = Determinant();
			if (Math.Abs(det) < 1e-7)
				throw new InvalidOperationException("Non-invertible matrix");

			double[,] result = new double[3, 3];

			result[0, 0] = (_matrix[1, 1] * _matrix[2, 2] - _matrix[1, 2] * _matrix[2, 1]) / det;
			result[0, 1] = (_matrix[0, 2] * _matrix[2, 1] - _matrix[0, 1] * _matrix[2, 2]) / det;
			result[0, 2] = (_matrix[0, 1] * _matrix[1, 2] - _matrix[0, 2] * _matrix[1, 1]) / det;
			result[1, 0] = (_matrix[1, 2] * _matrix[2, 0] - _matrix[1, 0] * _matrix[2, 2]) / det;
			result[1, 1] = (_matrix[0, 0] * _matrix[2, 2] - _matrix[0, 2] * _matrix[2, 0]) / det;
			result[1, 2] = (_matrix[0, 2] * _matrix[1, 0] - _matrix[0, 0] * _matrix[1, 2]) / det;
			result[2, 0] = (_matrix[1, 0] * _matrix[2, 1] - _matrix[1, 1] * _matrix[2, 0]) / det;
			result[2, 1] = (_matrix[0, 1] * _matrix[2, 0] - _matrix[0, 0] * _matrix[2, 1]) / det;
			result[2, 2] = (_matrix[0, 0] * _matrix[1, 1] - _matrix[0, 1] * _matrix[1, 0]) / det;

			return new Matrix3x3(result);
		}

		public double Trace()
		{
			return _matrix[0, 0] + _matrix[1, 1] + _matrix[2, 2];
		}

		public Vector3 Column(int i)
		{
			return new Vector3((float)_matrix[0, i], (float)_matrix[1, i], (float)_matrix[2, i]);
		}

		public bool HasNaN()
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					if (double.IsNaN(_matrix[i, j]))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
