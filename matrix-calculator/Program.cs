using Exceptions;
using System;
using System.Collections.Generic;

namespace Exceptions
{
    public class IncompatibleSizeException : Exception
    {
        public IncompatibleSizeException() : base("matrix sizes are incompatible") { }
        public IncompatibleSizeException(string message) : base(message) { }
        public IncompatibleSizeException(string message, Exception inner) : base(message, inner) { }
    }
}

namespace task_1
{
    class Matrix
    {
        private int[,] matrix;

        public Matrix()
        {
            matrix = new int[0, 0];
        }

        public Matrix(int xSize, int ySize)
        {
            matrix = new int[xSize, ySize];
        }

        public void SetValue(int val, int x, int y)
        {
            this.matrix[x, y] = val;
        }

        public void Size(out int xSize, out int ySize)
        {
            xSize = matrix.GetLength(0);
            ySize = matrix.GetLength(1);
        }

        public int At(int x, int y)
        {
            return this.matrix[x, y];
        }

        public void FillRandom(int lowerBorder, int upperBorder)
        {
            Size(out int xSize, out int ySize);
            Random rnd = new Random();

            for (int line = 0; line < xSize; line++)
            {
                for (int column = 0; column < ySize; column++)
                {
                    this.matrix[line, column] = rnd.Next(lowerBorder, upperBorder);
                }
            }
        }

        public void Transpose()
        {
            this.Size(out int xSize, out int ySize);
            int[,] transposed = new int[ySize, xSize];

            for (int line = 0; line < xSize; line++)
            {
                for (int column = 0; column < ySize; column++)
                {
                    transposed[column, line] = this.matrix[line, column];
                }
            }

            this.matrix = transposed;
        }

        public static Matrix operator +(Matrix lho, Matrix rho)
        {
            lho.Size(out int xSizeLeft, out int ySizeLeft);
            rho.Size(out int xSizeRight, out int ySizeRight);

            if (xSizeLeft != xSizeRight)
            {
                throw new IncompatibleSizeException();
            }

            if (ySizeLeft != ySizeRight)
            {
                throw new IncompatibleSizeException();
            }

            Matrix result = new Matrix(xSizeLeft, ySizeLeft);

            for (int line = 0; line < xSizeLeft; line++)
            {
                for (int column = 0; column < ySizeLeft; column++)
                {
                    int newValue = lho.At(line, column) + rho.At(line, column);
                    result.SetValue(newValue, line, column);
                }
            }

            return result;
        }

        private static int Product(in Matrix lho, in Matrix rho, int x, int y)
        {
            lho.Size(out int xSizeLeft, out int ySizeLeft);
            int result = 0;

            for (int i = 0; i < ySizeLeft; i++)
            {
                result += lho.At(x, i) * rho.At(i, y);
            }

            return result;
        }

        public static Matrix operator *(Matrix lho, Matrix rho)
        {
            lho.Size(out int xSizeLeft, out int ySizeLeft);
            rho.Size(out int xSizeRight, out int ySizeRight);

            if (xSizeLeft != ySizeRight)
            {
                throw new IncompatibleSizeException();
            }

            Matrix result = new Matrix(xSizeLeft, ySizeRight);

            for (int x = 0; x < xSizeLeft; x++)
            {
                for (int y = 0; y < ySizeRight; y++)
                {
                    int val = Product(in lho, in rho, x, y);
                    result.SetValue(val, x, y);
                }
            }

            return result;
        }

        public override String ToString()
        {
            string result = "";
            for (int line = 0; line < this.matrix.GetLength(0); line++)
            {
                result += "(";
                for (int item = 0; item < this.matrix.GetLength(1); item++)
                {
                    result += matrix[line, item].ToString();
                    if (item < this.matrix.GetLength(1) - 1)
                    {
                        result += " ";
                    }
                }
                result += ")\n";
            }

            return result;
        }
    }

    class Program
    {
        private static Dictionary<string, Matrix> Data = new Dictionary<string, Matrix>();

        static bool IsNumeric(string[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (!IsNumeric(array[i]))
                {
                    return false;
                }
            }

            return true;
        }

        static bool IsNumeric(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (!char.IsNumber(str[i]))
                {
                    return false;
                }
            }

            return true;
        }

        static bool CreateMatrix(in string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("too few or too many args\n");
                return false;
            }

            string newName = args[1];

            bool isValidxSize = int.TryParse(args[2], out int xSize);
            bool isValidySize = int.TryParse(args[3], out int ySize);

            if (!(isValidxSize && isValidySize))
            {
                Console.WriteLine("invalid args\n");
                return false;
            }

            Matrix matrix = new Matrix(xSize, ySize);

            if (Data.ContainsKey(newName))
            {
                newName = Override(newName);
            }

            Data[newName] = matrix;
            Console.WriteLine($"matrix named {newName} ({xSize}x{ySize}) created\n");

            return true;
        }

        static bool Fill(string[] args)
        {
            string name = args[1];

            if (!Data.ContainsKey(name))
            {
                Console.WriteLine($"no such name {name}\n");
                return false;
            }

            if (args.Length == 2)
            {
                FillMatrix(name);
                Console.WriteLine($"matrix {name}:\n{Data[name]}");
                return true;
            }

            if ((args.Length > 2) && (args[2] != "random"))
            {
                Console.WriteLine($"no such command {args[2]}\n");
                return false;
            }

            if (args.Length == 3)
            {
                Data[name].FillRandom(1, 100);
                Console.WriteLine($"matrix {name}:\n{Data[name]}");
                return true;
            }
            else if (args.Length == 5)
            {
                bool isValidxSize = int.TryParse(args[3], out int lowerBorder);
                bool isValidySize = int.TryParse(args[4], out int upperBorder);

                if (!(isValidxSize && isValidySize))
                {
                    Console.WriteLine("invalid args\n");
                    return false;
                }

                Data[name].FillRandom(lowerBorder, upperBorder);
                Console.WriteLine($"matrix {name}:\n{Data[name]}");
                return true;
            }
            else
            {
                Console.WriteLine("invalid args amount\n");
                return false;
            }
        }

        static void FillMatrix(string name)
        {
            Data[name].Size(out int xSize, out int ySize);

            for (int line = 0; line < xSize;)
            {
                Console.Write($"{line}\t");
                string[] intLine = Console.ReadLine().Split();
                bool isValid = true;

                if (intLine.Length != ySize)
                {
                    Console.WriteLine("invalid args amount");
                    continue;
                }
                else
                {
                    line++;
                }

                for (int column = 0; column < ySize; column++)
                {
                    if (!int.TryParse(intLine[column], out int val))
                    {
                        Console.WriteLine($"{intLine[column]} is not a decimal value");
                        isValid = false;
                        break;
                    }

                    Data[name].SetValue(val, line - 1, column);
                }

                if (!isValid)
                {
                    continue;
                }
            }
        }

        static bool Sum(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("invalid args amount\n");
                return false;
            }

            string newName = args[1];
            string lhoName = args[2];
            string rhoName = args[3];
            Matrix lho = Data[lhoName];
            Matrix rho = Data[rhoName];
            lho.Size(out int xSizeLeft, out int ySizeLeft);
            rho.Size(out int xSizeRight, out int ySizeRight);

            if (xSizeLeft != xSizeRight)
            {
                Console.WriteLine($"matrix sizes are incompetible: {xSizeLeft} != {xSizeRight}");
                return false;
            }

            if (ySizeLeft != ySizeRight)
            {
                Console.WriteLine($"matrix sizes are incompetible: {ySizeLeft} != {ySizeRight}");
                return false;
            }

            if (Data.ContainsKey(newName))
            {
                newName = Override(newName);
            }

            Data[newName] = lho + rho;
            Console.WriteLine($"sum of {lhoName}, {rhoName} is stored in {newName}\n");
            return true;
        }

        static bool Mult(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("invalid args amount\n");
                return false;
            }

            string newName = args[1];
            string lhoName = args[2];
            string rhoName = args[3];
            Matrix lho = Data[lhoName];
            Matrix rho = Data[rhoName];
            lho.Size(out int xSizeLeft, out int ySizeLeft);
            rho.Size(out int xSizeRight, out int ySizeRight);

            if (xSizeLeft != ySizeRight)
            {
                Console.WriteLine($"matrix sizes are incompetible: {xSizeLeft} != {ySizeRight}");
                return false;
            }

            if (Data.ContainsKey(newName))
            {
                newName = Override(newName);
            }

            Data[newName] = lho * rho;
            Console.WriteLine($"product of {lhoName}, {rhoName} is stored in {newName} ({xSizeLeft}x{ySizeRight})\n");
            return true;
        }

        static bool Transpose(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("invalid args amount\n");
                return false;
            }

            string newName = args[1];
            string toTransposeName = args[2];

            if (Data.ContainsKey(newName))
            {
                newName = Override(newName);
            }

            Matrix toTranspose = Data[toTransposeName];
            Matrix Transposed = toTranspose;
            Transposed.Transpose();

            Data[newName] = Transposed;
            Console.WriteLine($"{toTransposeName} transposed is stored in {newName}\n");

            return true;
        }


        static string Override(string name)
        {
            while (true)
            {
                Console.WriteLine($"matrix with name {name} already exists\ndo you want to override it? (y/n)");
                string yesOrNo = Console.ReadLine();

                if (yesOrNo == "y")
                {
                    return name;
                }
                else if (yesOrNo == "n")
                {
                    while (true)
                    {
                        Console.Write("new name: ");
                        string newName = Console.ReadLine();

                        if (newName.Contains(" "))
                        {
                            Console.WriteLine("name can't contain spaces");
                            continue;
                        }

                        return newName;
                    }
                }
            }
        }

        static void Main()
        {
            while (true)
            {
                string[] args = Console.ReadLine().Split();

                if (args.Length < 2)
                {
                    Console.WriteLine("not enough args");
                    continue;
                }

                string command = args[0];

                if (command == "display")
                {
                    string name = args[1];

                    if (!Data.ContainsKey(name))
                    {
                        Console.WriteLine($"no such name {name}");
                        continue;
                    }

                    Console.WriteLine($"{name}:\n{Data[name]}");
                    continue;
                }
                else if (command == "new")
                {
                    CreateMatrix(in args);
                    continue;
                }
                else if (command == "fill")
                {
                    Fill(args);
                    continue;
                }
                else if (command == "sum")
                {
                    Sum(args);
                    continue;
                }
                else if (command == "mult")
                {
                    Mult(args);
                    continue;
                }
                else if (command == "determ")
                {
                    // calc determinant
                }
                else if (command == "invert")
                {
                    // display invert matrix
                }
                else if (command == "transpose")
                {
                    Transpose(args);
                    continue;
                }
                else if (command == "solve")
                {
                    // solve equation system
                }
                else
                {
                    Console.WriteLine($"no such command: {command}\n");
                    continue;
                }
            }
        }
    }
}
