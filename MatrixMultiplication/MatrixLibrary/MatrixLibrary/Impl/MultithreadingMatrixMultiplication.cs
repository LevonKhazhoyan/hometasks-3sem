namespace MatrixLibrary.Impl;

/// <summary>
/// Multi-threaded realisation of <see cref="IMatrixMultiplication"/> interface
/// </summary>
public class ParallelMatrixMatrixMultiplication : IMatrixMultiplication
{   
    /// <summary>
    /// Multi-threaded realisation of matrix multiplication.
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
            throw new ArgumentException($"Invalid matrices to multiply - first cols: {first.ColNumber}, " 
                + $"second rows: {second.RowNumber}"); 
        }

        var threads = new Thread[Math.Min(Environment.ProcessorCount, rows1)];
        var chunkSize = rows1 / threads.Length + 1;
        var result = new Matrix(rows1, cols2);

        for (var i = 0; i < threads.Length; i++)
        {
            var localI = i;
            threads[i] = new Thread(() =>
            {
                for (var row = localI * chunkSize; row < Math.Min((localI + 1) * chunkSize, rows1); row++)
                {
                    for (var col = 0; col < cols2; col++)
                    {
                        for (var j = 0; j < cols1; j++)
                        {
                            result[row, col] += first[row, j] * second[j, col];
                        }
                    }
                }
            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        return result;
    }
}