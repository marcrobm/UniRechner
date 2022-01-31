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

            public static List<Matrix> rref(Matrix A)
        {
            List<Matrix> matsl = Elimination(A.Clone());
            Matrix M = matsl[matsl.Count - 1];
            List<Matrix> ret = new List<Matrix>();
            ret.Add(M);
            for (int i =Math.Min(A.m-1,A.n-1); i >= 0; i--)
            {
                Matrix c = ret[ret.Count - 1];
                //spalte normieren
                if (!c.data[i][i].Equals(KNG.zero()))
                {
                    Console.WriteLine("Normieren:" + i);
                    c = c.MulRow(i, KNG.mult_inv(c.data[i][i]));
                }
                ret.Add(c);
            }
            
             Matrix d = ret[ret.Count - 1];
            //einsetzen
            for(int SRC = d.m-1;SRC>0;SRC--){
                for (int DST = 0; DST < SRC; DST++)
                {
                    KNG Factor = KNG.add_inv(d.data[DST][SRC]);
                    Console.WriteLine("Einsetzen row:"+SRC+"in"+DST+"factor:"+Factor.ToString());
                    d = d.AddRow(DST,SRC,Factor);
                }
                ret.Add(d);
            }
           



            return (ret);
        }
        public static List<Matrix> Elimination(Matrix A)
        {
            List<Matrix> mats = new List<Matrix>();
            mats.Add(A.Clone());
            for (int i = 0; i < A.n && i < A.m; i++)
            {
                mats.Add(Schritt(mats[mats.Count - 1],i).Clone());
            }
            return (mats);
        }
        public static Matrix Schritt(Matrix In,int pos)
        {
            Matrix R = In.Clone();
            //find first line with non zero, push it to top       
            for (int i = pos; i < In.m; i++)
            {
                if(!R.data[i][pos].Equals(KNG.zero()))
                {
                    R = R.Clone().swapRow(i, pos);
                    break;
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
                Console.WriteLine("Added :" + factor.ToString() + "*" + pos + "to" + c);
                R = R.AddRow(c,pos,factor);
            }
            return (R);
        }
    }
}
