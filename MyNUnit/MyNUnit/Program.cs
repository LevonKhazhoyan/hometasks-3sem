using MyNUnit.MyNUnitImpl;

if (args.Length != 1)
{
    Console.WriteLine("Wrong number of arguments. Please, specify only path to directory with dlls");
    return;
}

var testSession = new MyNUnitClass();
testSession.Start(args[0]);
testSession.PrintReport();