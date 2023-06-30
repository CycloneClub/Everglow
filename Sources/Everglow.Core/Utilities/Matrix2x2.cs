using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Everglow.Commons.Utilities
{
	/// <summary>
	/// 2x2 矩阵类
	/// </summary>
	public class Matrix2x2
	{
		public static Matrix2x2 Identity = new Matrix2x2(
			new double[2, 2]
		{
			{ 1, 0}, { 0, 1 },
		});
		public static Matrix2x2 Zero = new Matrix2x2(
		   new double[2, 2]
	   {
			{ 0, 0 }, { 0, 0 },
	   });

		private double[,] _matrix;

		public Matrix2x2()
		{
			_matrix = new double[2, 2];
		}

		public Matrix2x2(double[,] initialMatrix)
		{
			if (initialMatrix.GetLength(0) != 2 || initialMatrix.GetLength(1) != 2)
				throw new ArgumentException("Initial matrix must be 2x2");

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

		public double Trace()
		{
			return _matrix[0, 0] + _matrix[1, 1];
		}

		public Matrix2x2 Inverse()
		{
			double det = Determinant();
			if (det == 0)
				throw new InvalidOperationException("Matrix is not invertible.");

			var result = new Matrix2x2
			{
				[0, 0] = _matrix[1, 1] / det,
				[0, 1] = -_matrix[0, 1] / det,
				[1, 0] = -_matrix[1, 0] / det,
				[1, 1] = _matrix[0, 0] / det
			};

			return result;
		}

		public Matrix2x2 Multiply(Matrix2x2 other)
		{
			var result = new Matrix2x2
			{
				[0, 0] = this[0, 0] * other[0, 0] + this[0, 1] * other[1, 0],
				[0, 1] = this[0, 0] * other[0, 1] + this[0, 1] * other[1, 1],
				[1, 0] = this[1, 0] * other[0, 0] + this[1, 1] * other[1, 0],
				[1, 1] = this[1, 0] * other[0, 1] + this[1, 1] * other[1, 1]
			};

			return result;
		}

		public Vector2 Multiply(Vector2 other)
		{
			return new Vector2((float)(this[0, 0] * other.X + this[0, 1] * other.Y),
				(float)(this[1, 0] * other.X + this[1, 1] * other.Y));
		}

		public double Determinant()
		{
			return _matrix[0, 0] * _matrix[1, 1] - _matrix[0, 1] * _matrix[1, 0];
		}

		public Matrix2x2 Adjoint()
		{
			return new Matrix2x2
			{
				[0, 0] = _matrix[1, 1],
				[0, 1] = -_matrix[0, 1],
				[1, 0] = -_matrix[1, 0],
				[1, 1] = _matrix[0, 0],
			};
		}

		public static Matrix2x2 operator *(Matrix2x2 a, double b)
		{
			var result = new Matrix2x2
			{
				[0, 0] = a[0, 0] * b,
				[0, 1] = a[0, 1] * b,
				[1, 0] = a[1, 0] * b,
				[1, 1] = a[1, 1] * b
			};

			return result;
		}

		public static Matrix2x2 operator *(double b, Matrix2x2 a)
		{
			var result = new Matrix2x2
			{
				[0, 0] = a[0, 0] * b,
				[0, 1] = a[0, 1] * b,
				[1, 0] = a[1, 0] * b,
				[1, 1] = a[1, 1] * b
			};

			return result;
		}

		public static Matrix2x2 operator *(Matrix2x2 a, Matrix2x2 b)
		{
			var result = new Matrix2x2
			{
				[0, 0] = a[0, 0] * b[0, 0] + a[0, 1] * b[1, 0],
				[0, 1] = a[0, 0] * b[0, 1] + a[0, 1] * b[1, 1],
				[1, 0] = a[1, 0] * b[0, 0] + a[1, 1] * b[1, 0],
				[1, 1] = a[1, 0] * b[0, 1] + a[1, 1] * b[1, 1]
			};

			return result;
		}

		public static Matrix2x2 operator +(Matrix2x2 a, Matrix2x2 b)
		{
			var result = new Matrix2x2
			{
				[0, 0] = a[0, 0] + b[0, 0],
				[0, 1] = a[0, 1] + b[0, 1],
				[1, 0] = a[1, 0] + b[1, 0],
				[1, 1] = a[1, 1] + b[1, 1]
			};

			return result;
		}

		public static Matrix2x2 operator -(Matrix2x2 a, Matrix2x2 b)
		{
			var result = new Matrix2x2
			{
				[0, 0] = a[0, 0] - b[0, 0],
				[0, 1] = a[0, 1] - b[0, 1],
				[1, 0] = a[1, 0] - b[1, 0],
				[1, 1] = a[1, 1] - b[1, 1]
			};

			return result;
		}

		public static Matrix2x2 operator /(Matrix2x2 a, Matrix2x2 b)
		{
			var result = new Matrix2x2
			{
				[0, 0] = a[0, 0] / b[0, 0],
				[0, 1] = a[0, 1] / b[0, 1],
				[1, 0] = a[1, 0] / b[1, 0],
				[1, 1] = a[1, 1] / b[1, 1]
			};

			return result;
		}

		public Matrix2x2 Transpose()
		{
			return new Matrix2x2
			{
				[0, 0] = _matrix[0, 0],
				[0, 1] = _matrix[1, 0],
				[1, 0] = _matrix[0, 1],
				[1, 1] = _matrix[1, 1],
			};
		}


		public static Matrix2x2 CreateRotationMatrix(float r)
		{
			return new Matrix2x2
			{
				[0, 0] = Math.Cos(r),
				[0, 1] = -Math.Sin(r),
				[1, 0] = Math.Sin(r),
				[1, 1] = Math.Cos(r),
			};
		}
	}
}
