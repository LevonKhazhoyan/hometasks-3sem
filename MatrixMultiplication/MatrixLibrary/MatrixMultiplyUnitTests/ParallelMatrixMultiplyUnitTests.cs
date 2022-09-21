using MatrixLibrary;
using MatrixLibrary.impl;

namespace MatrixMultiplyUnitTests;

using System;
using System.IO;
using NUnit.Framework;

public class Tests
{
    private static IMatrixMultiplication[] _strategies =
    {
        new ParallelMatrixMatrixMultiplication(),
        new SequentialMatrixMultiplication()
    };

    [TestCaseSource(nameof(_strategies))]
    public void IdentityMatrixTest(IMatrixMultiplication strategy)
    {
        var testMatrix = new Matrix(new long[,]
        {
            {1, 2},
            {3, 4}
        });

        var identity = new Matrix(new long[,]
        {
            {1, 0},
            {0, 1}
        });

        var expectation = new Matrix(new long[,]
        {
            {1, 2},
            {3, 4}
        });
        
        Assert.That(expectation, Is.EqualTo(testMatrix.Multiply(identity, strategy)));
    }

    [TestCaseSource(nameof(_strategies))]
    public void ZeroTest(IMatrixMultiplication strategy)
    {
        var zeroMatrix = new Matrix(new long[,]
        {
            {0, 0},
            {0, 0}
        });

        var identity = new Matrix(new long[,]
        {
            {1, 0},
            {0, 1}
        });

        Assert.That(zeroMatrix, Is.EqualTo(zeroMatrix.Multiply(identity, strategy))); 
    }

    [TestCaseSource(nameof(_strategies))]
    public void NumberTest(IMatrixMultiplication strategy)
    {
        var singleElementMatrix1 = new Matrix(new long[,]
        {
            { 1 }
        });

        var singleElementMatrix2 = new Matrix(new long[,]
        {
            { 10 }
        });

        var result = new Matrix(new long[,]
        {
            { 10 }
        });

        Assert.That(result, Is.EqualTo(singleElementMatrix1.Multiply(singleElementMatrix2, strategy))); 
    }

    [Test]
    public void ThrowTest()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Matrix(0, -1));
    }

    [Test]
    public void WorkWithFileTest()
    {
        var random = new Random();
        for (var i = 0; i < 100; i++)
        {
            var rows = random.Next(1, 100);
            var cols = random.Next(1, 100);
            var matrix = Utils.GenerateMatrix(rows, cols);
            var path = Path.GetTempPath() + Guid.NewGuid() + ".txt";
            Utils.Print(path, matrix);
            var readMatrix = Utils.Read(path);
            Assert.That(matrix, Is.EqualTo(readMatrix));
        }
    }
}