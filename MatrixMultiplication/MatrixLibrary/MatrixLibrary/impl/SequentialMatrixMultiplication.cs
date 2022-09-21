namespace MatrixLibrary.impl;

/// <summary>
/// Sequential multiplication strategy.
/// </summary>
public class SequentialMatrixMultiplication : IMatrixMultiplication
{
    /// <summary>
    /// Single-threaded realisation of matrix multiplication.
    /// </summary>
    /// <param name="first"> First matrix to multiply.</param>
    /// <param name="second"> Second matrix to multiply.</param>
    /// <returns> Matrix multiplication product.</returns>
    /// <exception cref="ArgumentException"> Expected that columns of first matrix is equal to rows of second matrix.</exception>
    public Matrix Multiply(Matrix first, Matrix second)
    {
        var (rows1, cols1) = (first.RowNumber, first.ColNumber);
        var (rows2, cols2) = (second.RowNumber, second.ColNumber);

        if (cols1 != rows2)
        {
            throw new ArgumentException($"Invalid matrices sizes - first cols: {first.ColNumber}, " + 
                $"second lines: {second.RowNumber}");
        }

        var result = new Matrix(rows1, cols2);

        for (var row = 0; row < rows1; row++)
        {
            for (var col = 0; col < cols2; col++)
            {
                for (var i = 0; i < cols1; i++)
                {
                    result[row, col] += first[row, i] * second[i, col];
                }
            }
        }

        return result;
    }
}