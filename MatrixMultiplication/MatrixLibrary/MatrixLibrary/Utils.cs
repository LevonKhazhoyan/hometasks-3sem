using System.Text;

namespace MatrixLibrary;

/// <summary>
/// Provides functions to I/O generate Matrix e.t.c. 
/// </summary>
public static class Utils
{
    /// <summary>
    /// Prints matrix to specified path.
    /// </summary>
    /// <param name="path">Path to file.</param>
    /// <param name="array">Matrix to print.</param>
    public static void Print(string path, Matrix array)
    {
        var rows = array.RowNumber;
        var cols = array.ColNumber;
        var text = new StringBuilder();
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                text = text.Append(array[i, j] + " ");
            }

            text = text.Replace(" ", "\n", text.Length - 1, 1);
        }

        File.WriteAllText(path, text.ToString());
    }

    /// <summary>
    /// Reads matrix from specified file.
    /// </summary>
    /// <param name="path">Path to file.</param>
    /// <returns>read matrix.</returns>
    public static Matrix Read(string path)
    {
        var lines = File.ReadAllLines(path);
        var (rows, cols) = (lines.Length, lines[0].Split(' ').Length);
        var matrix = new Matrix(rows, cols);
        for (var i = 0; i < rows; i++)
        {
            var separated = lines[i].Split(' ');
            for (var j = 0; j < cols; ++j)
            {
                matrix[i, j] = Convert.ToInt64(separated[j]);
            }
        }

        return matrix;
    }

    /// <summary>
    /// Generates random matrix.
    /// </summary>
    /// <param name="rows">Numbers of rows in generated matrix.</param>
    /// <param name="cols">Numbers of cols in generated matrix.</param>
    /// <returns> Matrix with given sizes.</returns>
    /// <exception cref="ArgumentOutOfRangeException">rows or cols below 1.</exception>
    public static Matrix GenerateMatrix(int rows, int cols)
    {
        if (rows <= 0 || cols <= 0)
        {
            throw new ArgumentOutOfRangeException($"rows: {rows}, cols: {cols}");
        }

        var random = new Random();
        var output = new Matrix(rows, cols);

        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                output[i, j] = random.Next();
            }
        }

        return output;
    }
}