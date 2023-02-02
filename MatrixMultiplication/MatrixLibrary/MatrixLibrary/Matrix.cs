namespace MatrixLibrary;

/// <summary>
/// Matrix realisation and function to work with it
/// </summary>
public class Matrix
{
    private readonly long[,] content;

    public int RowNumber { get; } 
    public int ColNumber { get; } 

    public long this[int i, int j]
    {
        get => content[i, j];
        set => content[i, j] = value;
    }

    public Matrix(int rows, int cols)
    {
        if (rows <= 0 || cols <= 0)
        {
            throw new ArgumentOutOfRangeException();
        }

        content = new long[rows, cols];
        RowNumber = rows;
        ColNumber = cols;
    }

    public Matrix(long[,] array)
    {
        content = array;
        RowNumber = array.GetLength(0);
        ColNumber = array.GetLength(1);
    }

    /// <summary>
    /// Matrix multiplication function
    /// </summary>
    /// <param name="this"> Left matrix </param>
    /// <param name="second"> Right matrix </param>
    /// <param name="strategy"> Strategy of multiplication </param>
    /// <returns> Matrix multiplication product</returns>
    public Matrix Multiply(Matrix second, IMatrixMultiplication strategy)
        => strategy.Multiply(this, second);

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var right = (Matrix)obj;

        if (ColNumber != right.ColNumber || RowNumber != right.RowNumber)
        {
            return false;
        }

        for (var i = 0; i < RowNumber; i++)
        {
            for (var j = 0; j < ColNumber; j++)
            {
                if (this[i, j] != right[i, j])
                {
                    return false;
                }
            }
        }

        return true;
    }

    public override int GetHashCode() 
        => HashCode.Combine(content, ColNumber, RowNumber);
}