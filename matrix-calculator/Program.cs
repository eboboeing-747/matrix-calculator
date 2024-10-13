﻿using Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

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
    class CofactorMult
    {
        public int size;
        public double multiplier;
        public double[,] matrix;

        public CofactorMult()
        {
            this.size = 0;
            this.multiplier = 0;
            this.matrix = new double[0, 0];
        }

        public CofactorMult(double multiplier, double[,] matrix)
        {
            int xSize = matrix.GetLength(0);
            int ySize = matrix.GetLength(1);

            if (xSize != ySize)
            {
                throw new IncompatibleSizeException();
            }

            this.size = xSize;
            this.multiplier = multiplier;
            this.matrix = matrix;
        }

        public override String ToString()
        {
            string result = this.multiplier.ToString() + "\n";

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

    class Matrix
    {
        private double[,] matrix;

        public Matrix()
        {
            matrix = new double[0, 0];
        }

        public Matrix(int xSize, int ySize)
        {
            matrix = new double[xSize, ySize];
        }

        public void SetValue(double val, int x, int y)
        {
            this.matrix[x, y] = val;
        }

        public void Size(out int xSize, out int ySize)
        {
            xSize = matrix.GetLength(0);
            ySize = matrix.GetLength(1);
        }

        public double At(int x, int y)
        {
            return this.matrix[x, y];
        }

        public void FillRandom(double lowerBorder, double upperBorder)
        {
            Size(out int xSize, out int ySize);
            Random rnd = new Random();

            for (int line = 0; line < xSize; line++)
            {
                for (int column = 0; column < ySize; column++)
                {
                    double newVal = rnd.NextDouble() * (upperBorder - lowerBorder) + lowerBorder;
                    this.matrix[line, column] = newVal;
                }
            }
        }

        public void Transpose()
        {
            this.Size(out int xSize, out int ySize);
            double[,] transposed = new double[ySize, xSize];

            for (int line = 0; line < xSize; line++)
            {
                for (int column = 0; column < ySize; column++)
                {
                    transposed[column, line] = this.matrix[line, column];
                }
            }

            this.matrix = transposed;
        }

        private double Daterminant2x2(CofactorMult[] cofMult)
        {
            int size = cofMult[0].size;

            if (size != 2)
            {
                throw new IncompatibleSizeException();
            }

            double cofactor = 0;

            for (int i = 0; i < cofMult.Length; i++)
            {
                CofactorMult mat = cofMult[i];
                double temp = mat.matrix[0, 0] * mat.matrix[1, 1] - mat.matrix[0, 1] * mat.matrix[1, 0];
                temp *= mat.multiplier;
                cofactor += temp;
            }

            return cofactor;
        }

        static double[,] SubMatrix(double[,] mat, int knockoutCol)
        {
            if (mat.GetUpperBound(0) != mat.GetUpperBound(1))
            {
                throw new IncompatibleSizeException();
            }

            int ubound = mat.GetUpperBound(0);
            double[,] m = new double[ubound, ubound];

            int mCol = 0;
            int mRow = 0;

            for (int row = 1; row <= ubound; row++)
            {
                mCol = 0;
                for (int col = 0; col <= ubound; col++)
                {
                    if (col == knockoutCol)
                    {
                        continue;
                    }
                    else
                    {
                        m[mCol, mRow] = mat[col, row];
                        mCol += 1;
                    }
                }
                mRow += 1;
            };

            return m;
        }

        private static CofactorMult[] Cofactor(CofactorMult cofactor)
        {
            double[,] matrix = cofactor.matrix;
            double majorMult = cofactor.multiplier;
            CofactorMult[] result = new CofactorMult[cofactor.size];

            for (int line = 0; line < cofactor.size; line++)
            {
                double[,] subMatrix = SubMatrix(cofactor.matrix, line);
                double mult = matrix[line, 0];

                if ((line + 2) % 2 != 0)
                {
                    mult = -mult;
                }

                result[line] = new CofactorMult(mult * majorMult, subMatrix);
            }

            return result;
        }

        private static CofactorMult[] Cofactor(CofactorMult[] cofMult)
        {
            int matrixSize = cofMult[0].size;
            CofactorMult[] result = new CofactorMult[cofMult.Length * matrixSize];

            for (int i = 0; i < cofMult.Length; i++)
            {
                double majorMult = cofMult[i].multiplier;

                CofactorMult[] temp = Cofactor(cofMult[i]);

                for (int copy = 0; copy < matrixSize; copy++)
                {
                    double mult = cofMult[i].matrix[copy, 0];

                    if ((copy + 2) % 2 != 0)
                    {
                        mult = -mult;
                    }

                    result[i * matrixSize + copy] = temp[copy];
                }
            }

            return result;
        }

        public double Determinant()
        {
            this.Size(out int xSize, out int ySize);

            if (xSize != ySize)
            {
                throw new IncompatibleSizeException();
            }

            if (xSize == 1)
            {
                return this.matrix[0, 0];
            }

            if (xSize == 2)
            {
                return this.matrix[0, 0] * this.matrix[1, 1] - this.matrix[1, 0] * this.matrix[0, 1];
            }

            CofactorMult[] cofMult = new CofactorMult[xSize];

            for (int col = 0; col < xSize; col++)
            {
                double[,] subMatrix = SubMatrix(this.matrix, col);
                double mult = this.matrix[col, 0];

                if ((col + 2) % 2 != 0)
                {
                    mult = -mult;
                }

                cofMult[col] = new CofactorMult(mult, subMatrix);
            }

            while (true)
            {
                if (cofMult[0].size == 2)
                {
                    return Daterminant2x2(cofMult);
                }
                else
                {
                    cofMult = Cofactor(cofMult);
                }
            }
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
                    double newValue = lho.At(line, column) + rho.At(line, column);
                    result.SetValue(newValue, line, column);
                }
            }

            return result;
        }

        private static double Product(in Matrix lho, in Matrix rho, int x, int y)
        {
            lho.Size(out int xSizeLeft, out int ySizeLeft);
            double result = 0;

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
                    double val = Product(in lho, in rho, x, y);
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

        //static bool IsNumeric(string str)
        //{
        //    for (int i = 0; i < str.Length; i++)
        //    {
        //        if (!char.IsNumber(str[i]))
        //        {
        //            return false;
        //        }
        //    }

        //    return true;
        //}

        public static bool IsNumeric(string valueToTest)
        {
            if (double.TryParse(valueToTest, out double d) && !Double.IsNaN(d) && !Double.IsInfinity(d))
            {
                return true;
            }

            return false;
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
                Data[name].FillRandom(1.0f, 100.0f);
                Console.WriteLine($"matrix {name}:\n{Data[name]}");
                return true;
            }
            else if (args.Length == 5)
            {
                bool isValidLowerBorder = double.TryParse(args[3], out double lowerBorder);
                bool isValidUpperBorder = double.TryParse(args[4], out double upperBorder);

                if (!(isValidLowerBorder && isValidUpperBorder))
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
                string[] doubleLine = Console.ReadLine().Split();
                bool isValid = true;

                if (doubleLine.Length != ySize)
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
                    if (!double.TryParse(doubleLine[column], out double val))
                    {
                        Console.WriteLine($"{doubleLine[column]} is not a decimal value");
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
                Console.WriteLine($"matrix sizes are incompetible: {xSizeLeft} != {xSizeRight}\n");
                return false;
            }

            if (ySizeLeft != ySizeRight)
            {
                Console.WriteLine($"matrix sizes are incompetible: {ySizeLeft} != {ySizeRight}\n");
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
                Console.WriteLine($"matrix sizes are incompetible: {xSizeLeft} != {ySizeRight}\n");
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

        static bool Determinant(string name)
        {
            if (!Data.ContainsKey(name))
            {
                Console.WriteLine($"no such name: {name}\n");
                return false;
            }

            Matrix matrix = Data[name];
            matrix.Size(out int xSize, out int ySize);

            if (xSize != ySize)
            {
                Console.WriteLine($"illegal matix size: ({xSize}x{ySize})\n");
                return false;
            }

            double determinant = matrix.Determinant();
            Console.WriteLine($"determinant of {name}: {determinant}\n");

            return true;
        }

        static void Display(string argument)
        {
            if (argument == "names")
            {
                string[] names = Data.Keys.ToArray();
                for (int i = 0; i < names.Length; i++)
                {
                    Console.Write($"{names[i]} ");
                }
                Console.WriteLine("\n");
                return;
            }

            if (!Data.ContainsKey(argument))
            {
                Console.WriteLine($"no such name {argument}");
                return;
            }

            Console.WriteLine($"{argument}:\n{Data[argument]}");
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

                if (args.Length == 0)
                {
                    continue;
                }
                else if (args.Length < 2)
                {
                    Console.WriteLine("not enough args");
                    continue;
                }

                string command = args[0];

                if ((command == "display") || (command == "view"))
                {
                    Display(args[1]);
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
                else if ((command == "determinant") || (command == "det"))
                {
                    Determinant(args[1]);
                    continue;
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
