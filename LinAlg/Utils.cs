using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinAlg
{
    public static class Extensions
    {
        public static T[] SubArray<T>(this T[] array, int offset, int length)
        {
            return new ArraySegment<T>(array, offset, length)
                        .ToArray();
        }
        public static int[] AddComponentWise<T>(this int[] array, int toAdd)
        {
            int[] ret = new int[array.Length];
            for(int i = 0; i < array.Length; i++)
            {
                ret[i] = array[i];
                ret[i] += toAdd;
            }
            return ret;
        }
        public static string toLatexVector<T>(this T[] array)
        {
            String s = @"\pmatrix{";
            for (int i = 0; i < array.Length; i++)
            {
                s += array[i].ToString() + @"\\";
            }
            s = s.Substring(0, s.Length - 2);
            s += "}";
            return (s);
        }
    }
    class Utils
    {
        public static string StringArrToLatexMatrix(string[][] mat,string null_value = "null",bool borders = true)
        {
            String s = @"\pmatrix{";
            if (borders == false)
            {
                s = @"\matrix{";
            }
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
        public static String[][] StringArrTransponiert(string[][] mat)
        {
            String[][] ret = new string[mat[0].Length][];
            for(int c = 0; c < mat[0].Length; c++)
            {
                ret[c] = new String[mat.Length];
                for(int r = 0; r < mat.Length; r++)
                {
                    ret[c][r] = mat[r][c];
                }
            }
            return ret;
        }
        public static String[][] StringVecTransponiert(string[] mat)
        {
            String[][] ret = new string[mat.Length][];
            for (int c = 0; c < mat.Length; c++)
            {
                ret[c] = new String[1];
                ret[c][0] = mat[c];
            }
            return ret;
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
        public static string Ltx(string L)
        {
            return L.Replace(" ", "\\:").Replace(Environment.NewLine, "\\\\");
        }
    }
}
