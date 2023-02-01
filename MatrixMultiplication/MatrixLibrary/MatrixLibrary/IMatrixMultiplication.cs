namespace MatrixLibrary;

/// <summary>
/// Matrix Multiplication interface 
/// </summary>
public interface IMatrixMultiplication
{
    /// <summary>
    /// Matrix multiplication function
    /// </summary>
    /// <param name="first">Left matrix </param>
    /// <param name="second">Second matrix </param>
    /// <returns>Matrix multiplication product</returns>
    Matrix Multiply(Matrix first, Matrix second);
}