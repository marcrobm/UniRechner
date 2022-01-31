using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinAlg
{
    class Matrix
    {
        public KNG[][] data;
        public int m;
        public int n;
        public Matrix(int m1, int n1)
        {
            m = m1;
            n = n1;
            data = new KNG[m][];
            for(int i = 0; i < m; i++)
            {
                data[i] = new KNG[n];
            }
            for (int i = 0; i < m; i++)
            {
                for (int c = 0; c < n; c++)
                {
                    data[i][c] = KNG.zero();
                }
            }
        }

        public static List<String> BracketParse(String s)
        {
            List<String> strs = new List<String>();
            int i = 0;
            while (i < s.Length)
            {
                int end = CloseIndex(s, i);
                String str = s.Substring(i+1, end - i-1);
                strs.Add(str);
                Console.WriteLine(str);
                i = end + 1;
            }
            return (strs);
        }
        public static int CloseIndex(String s, int open)
        {
            int level = 0;
            for (int i = open; i < s.Length; i++)
            {
                if (s[i] == '{')
                {
                    level++;
                }
                if (s[i] == '}')
                {
                    level--;
                }
                if (level == 0 && s[i] == '}')
                {
                    return (i);
                }
              
            }
            return -1;
        }
        public static Matrix FromInput(String description)
        {
            List<String> rows = BracketParse(description);
            int n = rows[0].Split(',').Length;
            int m = rows.Count;
            Matrix M = new Matrix(m,n);
            for (int r = 0; r < m; r++)
            {
                String[] t = rows[r].Split(',');
                for (int c = 0; c < n; c++)
                {
                    KNG val = new KNG(t[c]);
                    M.data[r][c] = val;
                }
            }
            return (M);
        }
        public String ToString()
        {
            String s = @"\pmatrix{";
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    s += data[i][j].ToString() + " & ";
                }
                s = s.Substring(0, s.Length - 3);// last remove &
                s += @"\\";
            }
            s = s.Substring(0, s.Length - 2); //remove last //
            s += "}";
            return (s);
        }    
        public static Matrix operator *(Matrix c1, Matrix c2)
           {      
            Matrix result = new Matrix(c1.m, c2.n);
            for (int i = 0; i < result.m; i++)
            {
                for (int k = 0; k < result.n; k++)
                {
                    KNG sum = new KNG();
                    for (int l = 0; l < c2.m; l++) {
                        sum += c1.data[i][l] * c2.data[l][k];
                    }
                    result.data[i][k] = sum;
                }          
            }
            return (result);
        }
        public static Matrix operator +(Matrix c1, Matrix c2)
        {
            Matrix result = new Matrix(c1.m, c2.n);
            for (int i = 0; i < result.m; i++)
            {
                for (int k = 0; k < result.n; k++)
                {
                    result.data[i][k] = c1.data[i][k] + c2.data[i][k];
                }
            }
            return (result);
        }
        public Matrix swapRow(int a,int b)
        {
            Matrix R = this.Clone();
            KNG[] tmp = (KNG[])R.data[a].Clone();
            R.data[a] = R.data[b];
            R.data[b] = tmp;
            return (R);
        }
        public Matrix AddRow(int row,int toAdd,KNG factor)
        {
            Matrix R = this.Clone();
            for(int i = 0; i < n; i++)
            {
                R.data[row][i] += data[toAdd][i]*factor;
            }
            return (R);
        }
        public Matrix MulRow(int row, KNG factor)
        {
            Matrix R = this.Clone();
            for (int i = 0; i < n; i++)
            {
                R.data[row][i] *= factor;
            }
            return (R);
        }
        public Matrix Clone()
        {
            Matrix M = new Matrix(m, n);
            for(int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    M.data[i][j] = data[i][j].Clone();
                }
            }           
            return M;
        }
        public static Matrix ID(int n)
        {
            Matrix ret = new Matrix(n, n);
            for(int i = 0; i < n; i++)
            {
                ret.data[i][i] = KNG.one();
            }
            return (ret);
        }
        public Matrix Trans()
        {
            Matrix t = new Matrix(n, m);
            for(int i = 0; i < n; i++)
            {
                for (int c = 0; c < m; c++)
                {
                    t.data[i][c] = data[c][i];
                }
            }
            return t;
        }
        public void Load(Matrix src,int m=0, int n=0)
        {
            for (int i = 0; i < src.m; i++)
            {
                for (int j = 0; j < src.n; j++)
                {
                    this.data[i][j] = src.data[i+m][j+n].Clone();
                }
            }
        }
        public Matrix collumsFrom(int nc)
        {
            Matrix ret = this.Clone();
            for (int r = 0; r < this.data.Length; r++)
            {
                var prev = new List<KNG>(this.data[r]);
                prev.RemoveRange(0,nc);
                ret.data[r] = prev.ToArray();
                ret.n = nc;
            }
            return (ret);
        }
        public Matrix Append(Matrix toAppend)
        {
            Matrix ret = this.Clone();
            for(int r = 0; r < this.data.Length; r++)
            {
                var prev = new List<KNG>(this.data[r]);
                var other = new List<KNG>(toAppend.data[r]);
                prev.AddRange(other);
                ret.data[r] = prev.ToArray();
                ret.n = this.n + toAppend.n;
            }
            return (ret);
        }
        public List<Matrix> Inverse()
        {
            Matrix tmp = this.Clone();
            tmp = tmp.Append(Matrix.ID(tmp.n));
            var result = Gauß.rref(tmp);
            result.Add(result[result.Count - 1].collumsFrom(tmp.n / 2));
            return (result);
        }
        public KNG Norm1(out int used_index)
        {
            //Maximale Spaltensumme
            return Maximum(colSumAbs,this.n-1,out used_index);
        }
        public KNG NormInfty(out int used_index)
        {
            //Maximale Spaltensumme
            return Maximum(rowSumAbs, this.n-1, out used_index);
        }
        public KNG Maximum(Func<int, KNG> f, int max, out int used_index)
        {
            if (max == 0)
            {
                used_index = 0;
                return f(0);
            }
            else
            {
                var omax = Maximum(f, max - 1,out used_index);
                if (f(max) > omax)
                {
                    used_index = max;
                    return f(max);
                }
                else
                {
                    used_index = max-1;
                    return omax;
                }
            }
        }
        public KNG rowSumAbs(int r)
        {
            KNG ret = KNG.zero();
            for (int i = 0; i < n;i++)
            {
                ret += KNG.abs(this.data[r][i]);
            }
            return ret;
        }
        public KNG colSumAbs(int r)
        {
            KNG ret = KNG.zero();
            for (int i = 0; i < m; i++)
            {
                ret += KNG.abs(this.data[i][r]);
            }
            return ret;
        }
    }
}
