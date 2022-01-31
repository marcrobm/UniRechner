using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinAlg
{
    class Utils
    {
        public static string StringArrToLatexMatrix(string[][] mat,string null_value)
        {
            String s = @"\pmatrix{";
            for (int i = 0; i < mat.Length; i++)
            {
                for (int j = 0; j < mat[i].Length; j++)
                {
                    if (mat[i][j] == null)
                    {
                        s += null_value + " & ";
                    }
                    else {
                        s += mat[i][j] + " & ";
                    }
                }
                s = s.Substring(0, s.Length - 3);// last remove &
                s += @"\\";
            }
            s = s.Substring(0, s.Length - 2); //remove last //
            s += "}";
            return (s);
        }
        public static void setCollumCount(ref string[][] mat,int count)
        {
            for(int i = 0; i < mat.Length;i++)
            {
                List<string> values = new List<string>();
                //add existing collom entriess
                for (int c = 0; c < mat[i].Count() && c<count; c++)
                {
                    values.Add(mat[i][c]);
                }
                //add new
                for (int c = mat[i].Count(); c < count; c++)
                {
                    values.Add(null);
                }
                mat[i] = values.ToArray();
            }
        }
        public static void setRowCount(ref string[][] mat, int count)
        {
                List<string[]> rows = new List<string[]>();
                //add existing
                for (int r = 0; r < mat.Count() && r < count; r++)
                {
                    rows.Add(mat[r]);
                }
                //add new
                for (int r = mat.Count(); r < count; r++)
                {
                    rows.Add(new string[mat[0].Count()]);
                }
                mat = rows.ToArray();
        }
    }
}
