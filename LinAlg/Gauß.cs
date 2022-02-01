using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinAlg
{
    class Gauß
    {
        public static List<Matrix> symetrical(Matrix A)
        {
            Console.WriteLine("symetrical");
            List<Matrix> ret = new List<Matrix>();
            for(int i = 0; i < A.m; i++)
            {
                Console.WriteLine("current row:"+i);
                if (A.data[i][i] != KNG.zero()) {
                    Matrix mod = Matrix.ID(A.n);
                    for (int c = 1; c < A.n;c++)
                    {
                        Console.WriteLine("current coll:" + i);
                        mod.data[i][c] = KNG.add_inv(A.data[i][c] * KNG.mult_inv(A.data[i][i]));
                    }
                    ret.Add(mod.Clone());
                    ret.Add(mod.Trans().Clone());
                    A *= mod;
                    A *= mod.Trans();
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            ret.Add(A);
            return ret;
        }

            public static Matrix rref(Matrix A, out String latex)
        {
            Matrix M = Elimination(A.Clone(),out latex);
            latex += "\\\\Spalten normieren\\\\";
            // Spalten normieren
            String[] info = new string[M.m];
            latex += M.ToString();
            for (int i =Math.Min(A.m-1,A.n-1); i >= 0; i--)
            {
                if (!M.data[i][i].Equals(KNG.zero()))
                {
                    Console.WriteLine(" Normieren:" + i);
                    info[i] = M.data[i][i].ToString();
                    M = M.MulRow(i, KNG.mult_inv(M.data[i][i]));
                }
            }
            latex += Utils.StringArrToLatexMatrix(Utils.StringVecTransponiert(info.SubArray(0, info.Length)), "-", false) + "\\\\";
            latex += "\\\\Rueckwaerts einsetzen\\\\";
            latex += M.ToString();
            for (int SRC = M.m-1;SRC>0;SRC--){
                for (int DST = 0; DST < SRC; DST++)
                {
                    KNG Factor = KNG.add_inv(M.data[DST][SRC]);
                    info[DST] = "row:" + (SRC+1) + "in" + (DST+1) + "factor:" + Factor.ToString();
                    M = M.AddRow(DST, SRC, Factor);
                }
                latex += Utils.StringArrToLatexMatrix(Utils.StringVecTransponiert(info.SubArray(0, info.Length)), "-", false);
                if (SRC % 4 == (M.m - 1)%4+4 || SRC == 1)
                {
                    latex += "\\\\";
                }
                else
                {
                    latex += "\\Rightarrow ";
                }
                latex += M.ToString();
            }
            return (M);
        }
        public static Matrix Elimination(Matrix A,out string Latex)
        {
            A = A.Clone();
            Latex = A.ToString()+ " ";
            for (int i = 0; i < A.n && i < A.m; i++)
            {
                String[] info = new String[A.m+1];//Changes for each row,last entry is for misc info, like swaping rows
                A=Schritt(A,i,info);
                Latex += Utils.StringArrToLatexMatrix(Utils.StringVecTransponiert(info.SubArray(0,info.Length-1)), "-", false);
                Latex += info[info.Length-1];
                if (i % 4 == 4 || i==A.n-1)
                {
                    Latex += "\\\\";
                }
                else
                {
                    Latex += "\\Rightarrow ";
                }
                if (i < A.n - 1)
                {
                    Latex += A.ToString();
                }
            }
            return (A);
        }
        public static Matrix Schritt(Matrix In,int pos,String[] info)
        {
            Matrix R = In.Clone();
            //find first line with non zero, push it to top       
            if (R.data[pos][pos].Equals(KNG.zero()))
            {
                for (int i = pos; i < In.m; i++)
                {
                    if (!R.data[i][pos].Equals(KNG.zero()))
                    {
                        R = R.Clone().swapRow(i, pos);
                        info[R.m] = "Swapp:" + (i + 1) + "," + (pos + 1);
                        break;
                    }
                }
            }
            //Console.WriteLine("AMAT:"+R.ToString());
            //nothing to do
            if (R.data[pos][pos].Equals(KNG.zero()))
            {
                Console.WriteLine("nothing to do");
                return (R);
            }
            //Console.WriteLine("BMAT:" + R.ToString());
            //Eliminate collom
            for (int c = pos + 1; c < R.m; c++)
            {
                KNG factor = KNG.add_inv(R.data[c][pos])*KNG.mult_inv(R.data[pos][pos]);
                info[c] = factor.ToString() + "*" + (pos+1)+".Row";
                Console.WriteLine("Add:"+factor.ToString() + "*" + pos + "to" + c);
                R = R.AddRow(c,pos,factor);
            }
            return (R);
        }
    }
}
