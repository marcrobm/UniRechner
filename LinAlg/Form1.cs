﻿using LinAlg.Betriebssysteme;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using UniRechner;

namespace LinAlg
{
    public partial class Form1 : Form
    {
        private CollumResizer mainCresize;
        public Form1()
        {
            InitializeComponent();
            String[] files = Directory.GetFiles("Koerper");
            foreach (String f in files)
            {
                comboBox1.Items.Add(f.Split('.')[0].Split('\\')[1]);
            }
            textBox5.Text = File.ReadAllText("Koerper\\" + comboBox1.Text + ".js");

            mainCresize = new CollumResizer(ref tableLayoutPanel1, 15);

        }
        TextHighlighter rdbcodebox;
        private void Form1_Load(object sender, EventArgs e)
        {
            Calculate();
            UpdateRDBDRCTRC();
            rdbcodebox = new TextHighlighter(ref richTextBoxRDB1SET);
        }
        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int[] input = new int[gcd_textBox3.Text.Split(' ').Length];
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = int.Parse(gcd_textBox3.Text.Split(' ')[i]);
            }
            String log = "";
            Console.Out.WriteLine(gcd(input, out log));
            SetMainImageAndInfo(log);
        }
        static int gcd(int[] input, out String log)
        {
            String description = "";
            String temp = "";
            int t = 0;

            t = gcd(input[0], input[1], out temp);
            description += "gcd(" + input[0] + "," + input[1] + "):" + "\\\\" + temp;
            int val = t;
            if (input.Length > 2)
            {
                int[] s = new int[input.Length - 1];
                Array.Copy(input, 1, s, 0, input.Length - 1);
                s[0] = t;
                val = gcd(s, out temp);
                description += temp;
            }
            log = description;
            Console.Out.WriteLine(log);
            return (val);
        }
        static int gcd(int a, int b, out String log)
        {
            List<int> s = new List<int>();
            List<int> t = new List<int>();
            return gcd(a, b, out log, out s, out t);
        }
        static int gcd(int a, int b, out String log, out List<int> s, out List<int> t)
        {
            List<int> r = new List<int>();
            List<int> m = new List<int>();
            s = new List<int>();
            t = new List<int>();
            r.Add(a);
            r.Add(b);
            s.Add(1);
            s.Add(0);
            t.Add(0);
            t.Add(1);
            int i = 1;
            while (r[i] != 0)
            {
                r.Add(r[i - 1] % r[i]);
                m.Add((int)Math.Floor((decimal)(r[i - 1] / r[i])));
                i++;
            }
            for (int c = 2; c < m.Count + 2; c++)
            {
                int sval = s[c - 2] - m[c - 2] * s[c - 1];
                int tval = t[c - 2] - m[c - 2] * t[c - 1];
                s.Add(sval);
                t.Add(tval);
            }
            m.Add(0); m.Add(0);
            log = "";
            for (int d = 0; d < t.Count - 1; d++)
            {
                log += String.Format("r_{0}:" + r[d] + ",m_{1}:" + m[d] + ",s_{1}:" + s[d + 1] + ",t_{1}:" + t[d + 1], d, d + 1);
                log += "\\\\";
            }
            log += "Darstellung:" + r[i - 1] + "=" + t[t.Count - 2] + "*" + b + "+" + s[s.Count - 2] + "*" + a;
            log += "\\\\";
            return (r[i - 1]);
        }

        private void numtype_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Calculate();
        }




        Dictionary<String, Matrix> mats = new Dictionary<String, Matrix>();
        int ilatex = 0;
        static string oldmatchesID = "";//used to check if the new matches are the same as the old matches
        static string lastData = "";
        void CalculateSync()
        {
            KNG.K_Functions = textBox5.Text;
            try
            {
                mats = new Dictionary<String, Matrix>();
                // Read Inputs + Display them
                String Latex = "Inputs:\\\\";
                String inputText = textBox3.Text.Replace(" ", "");
                foreach (String s in Regex.Split(inputText, "\r\n|\r|\n").Where(s => s != String.Empty))
                {
                    Matrix M = Matrix.FromInput(s.Split('=')[1]);
                    mats.Add(s.Split('=')[0], M);
                    Latex += s.Split('=')[0] + "=" + M.ToString() + "\\\\";
                }
                Latex += "Outputs:\\\\";
                //new parsing code 
                // X=CMD(args)
                Regex Command = new Regex(@"(?<DST>\w+)\=(?<CMD>\w+)\((?<params>[a-zA-Z0-9_,]+)\)");

                // filter out comments
                var tempInp = textBox4.Text.Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
                tempInp.RemoveAll(x => x.ElementAt(0) == '%');
                string cmdsInput = String.Join(Environment.NewLine, tempInp.ToArray());

                var Matches = Command.Matches(cmdsInput);

                string newMatches = KNG.K_Functions;
                foreach (Match match in Matches)
                {
                    newMatches += match.Groups["DST"].ToString() + match.Groups["params"].ToString() + match.Groups["CMD"].ToString();
                }
                if (oldmatchesID == newMatches && lastData == inputText)
                {
                    //input did not realy change, no need to calculate anything
                    return;
                }
                lastData = inputText;
                oldmatchesID = newMatches;
                int outindex = 0;
                foreach (Match match in Matches)
                {
                    String dest;
                    if (match.Groups["DST"] != null)
                    {
                        dest = match.Groups["DST"].ToString();
                    }
                    else
                    {
                        dest = "out_" + outindex;
                    }
                    String paramn = match.Groups["params"].ToString();
                    Console.WriteLine("dest:" + dest + " paramn:" + paramn);
                    switch (match.Groups["CMD"].ToString())
                    {
                        case "mul":
                        case "Multiply":
                            Matrix mul = MulMat(paramn.Split(',').ToList());
                            Latex += dest + "=" + paramn.Replace(",", "*") + "=" + mul.ToString() + "\\\\";
                            mats.Add(dest, mul);
                            break;
                        case "g":
                        case "Gaus":
                            String gausRechenweg;
                            mats.Add(dest, Gauß.Elimination(mats[paramn].Clone(), out gausRechenweg));
                            Latex += dest + " =Gaus(" + paramn + ")=";
                            Latex += "\\\\";
                            Latex += gausRechenweg;
                            break;
                        case "pg":
                        case "partGaus":
                            String pgausRechenweg;
                            mats.Add(dest, Gauß.Elimination(mats[paramn].Clone(), out pgausRechenweg, true));
                            mats.Add(paramn + "_L", Gauß.L.Clone());
                            mats.Add(paramn + "_P", Gauß.P.Clone());
                            mats.Add(paramn + "_R", Gauß.R.Clone());
                            Latex += dest + " =Gaus(" + paramn + ")=";
                            Latex += pgausRechenweg + "\\\\";
                            Latex += paramn + "_L=" + mats[paramn + "_L"].ToString();
                            Latex += paramn + "_P=" + mats[paramn + "_P"].ToString();
                            Latex += "\\\\";
                            break;
                        case "rr":
                        case "RREF":
                            String rrefRechenweg;
                            mats.Add(dest, Gauß.rref(mats[paramn].Clone(), out rrefRechenweg));
                            Latex += dest + " =RREF(" + paramn + ")=";
                            Latex += "\\\\";
                            Latex += rrefRechenweg;
                            Latex += "\\\\";
                            break;
                        case "inv":
                        case "Inverse":
                            String inverseRechenweg;
                            mats.Add(dest, mats[paramn].Clone().Inverse(out inverseRechenweg));
                            Latex += dest + " =Inverse(" + paramn + ")=";
                            Latex += " \\\\ ";
                            Latex += inverseRechenweg;
                            break;
                        case "sg":
                        case "SGaus":
                            List<Matrix> gm3 = Gauß.symetrical(mats[paramn].Clone());
                            Latex += dest + "=SGaus(" + paramn + ")=";
                            foreach (Matrix m in gm3)
                            {
                                Latex += m.ToString() + "  ";
                            }
                            Latex += "\\\\";
                            mats.Add(dest, gm3[gm3.Count - 1]);
                            break;
                        case "1N":
                        case "1Norm":
                            Matrix dst = new Matrix(1, 1);
                            int used = 0;
                            dst.data[0][0] = mats[paramn].Norm1(out used);
                            Latex += dest + "=1Norm(" + paramn + ")=" + dst.ToString() + "Index:" + used + "\\\\";
                            mats.Add(dest, dst);
                            break;

                        case "IN":
                        case "InftyNorm":
                            Matrix dst2 = new Matrix(1, 1);
                            int used2 = 0;
                            dst2.data[0][0] = mats[paramn].NormInfty(out used2);
                            Latex += dest + "=InftyNorm(" + paramn + ")=" + dst2.ToString() + "Index:" + used2 + "\\\\";
                            mats.Add(dest, dst2);
                            break;
                        case "P":
                        case "Print":
                            Latex += paramn + "=" + mats[paramn].ToString() + "\\\\";
                            break;
                        case "T":
                        case "Transponiert":
                            mats.Add(dest, mats[paramn].Clone().Trans());
                            Latex += dest + " =Transponiert(" + paramn + ")=" + mats[paramn].Clone().Trans().ToString();
                            Latex += " \\\\ ";
                            break;
                        case "bI":
                        case "BackInsert":
                            string bilog;
                            mats.Add(dest, mats[paramn.Split(',')[0]].Clone().rueckwaertsEinsetzen(mats[paramn.Split(',')[1]], out bilog));
                            Latex += dest + " =BackInsert(" + paramn + ")=" + mats[dest].ToString() + "\\\\";
                            Latex += bilog;
                            break;
                        case "fI":
                        case "FrontInsert":
                            string bilog2;
                            mats.Add(dest, mats[paramn.Split(',')[0]].Clone().forwaertsEinsetzen(mats[paramn.Split(',')[1]], out bilog2));
                            Latex += dest + " =FrontInsert(" + paramn + ")=" + mats[dest].ToString() + "\\\\";
                            Latex += bilog2;
                            break;
                    }
                    ilatex++;
                    SetMainImageAndInfo(Latex, "");
                }
            }
            catch (Exception ec)
            {
                SetMainImageAndInfo(null, ec.Message + ec.StackTrace);
            }
        }
        Thread CalcThread;
        void Calculate()
        {
            if (CalcThread != null)
            {
                // abort older call to Do Calculation
                oldmatchesID = "";//calc again
                CalcThread.Abort();
            }
            CalcThread = new Thread(CalculateSync);
            CalcThread.Start();
        }

        Matrix MulMat(List<String> names)
        {
            Matrix M = mats[names[0]];
            for (int i = 1; i < names.Count; i++)
            {
                M *= mats[names[i]];
            }
            return M;
        }

        Image RenderLatex(String latex)
        {
            var parser = new WpfMath.TexFormulaParser();
            var formula = parser.Parse(latex);
            var renderer = formula.GetRenderer(WpfMath.TexStyle.Display, 30.0, "Arial");
            var bitmapSource = renderer.RenderToBitmap(0, 0);
            return (GetBitmap(bitmapSource));
        }
        // converts a Bitmap Source Object to a Bitmap, inspired by https://stackoverflow.com/questions/2284353/is-there-a-good-way-to-convert-between-bitmapsource-and-bitmap
        Bitmap GetBitmap(BitmapSource src)
        {
            Bitmap dst = new Bitmap(
              src.PixelWidth,
              src.PixelHeight,
              PixelFormat.Format32bppPArgb);
            BitmapData intermediate = dst.LockBits(
              new Rectangle(Point.Empty, dst.Size),
              ImageLockMode.WriteOnly,
              PixelFormat.Format32bppPArgb);
            src.CopyPixels(
              System.Windows.Int32Rect.Empty,
              intermediate.Scan0,
              intermediate.Height * intermediate.Stride,
              intermediate.Stride);
            dst.UnlockBits(intermediate);
            return dst;
        }
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox5.Text = File.ReadAllText("Koerper\\" + comboBox1.Text + ".js");
            Calculate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            File.WriteAllText("Koerper\\" + comboBox1.Text + ".js", textBox5.Text);
            Calculate();
        }
        private void textBoxfermat_TextChanged(object sender, EventArgs e)
        {
            String Latex = "Fermat \\: Faktorisierung \\\\";
            try
            {
                int c = 0;
                int N = int.Parse(textBoxfermat.Text);
                int x = (int)Math.Ceiling(Math.Sqrt(N));
                int r = x * x - N;
                Latex += "r=" + r + ",x=" + x + "\\\\";
                while (!(Math.Sqrt(r) % 1 <= 0.0000001) && c < 1000)
                {
                    r = r + 2 * x + 1;
                    x = x + 1;
                    Latex += "r=" + r + ",x=" + x + "\\\\";
                    c++;
                }
                int y = (int)Math.Round(Math.Sqrt((double)r));
                int a = x + y;
                int b = x - y;
                Latex += "\\\\floor(abs(sqrt(r))=" + y + "\\\\";
                Latex += "N=(" + x + "+" + y + ")" + "*" + "(" + x + " - " + y + ")" + " = " + a + "*" + b + "\\\\";
                SetMainImageAndInfo(Latex);
            }
            catch (Exception d)
            {

            }
        }

        List<int> SiebErastothenes(int maxnumber, out string Latex)
        {
            List<int> numberslist = new List<int>();
            List<int> primeslist = new List<int>();
            primeslist.Add(2);
            int activeprime = 3;
            //Numberslist mit den Zahlen initialisieren
            for (int i = 3; i <= maxnumber; i += 2)
            {
                numberslist.Add(i);
            }
            Console.WriteLine("numberslist done");
            Latex = "";
            while (numberslist.Count != 0)//nicht leere Menge
            {
                //Textausgabe
                //numberslist als string ausgeben
                Latex += "\\\\numbers:";
                foreach (int n in numberslist)
                {
                    Latex += (n + ",");
                }
                //primeslist als string ausgeben
                Latex += ("\\:primes:");
                foreach (int n in primeslist)
                {
                    Latex += (n + ",");
                }
                //alle vielfachen von activeprime entfernen
                numberslist.RemoveAll(x => x % activeprime == 0);
                //activeprime zu primes hinzufügen
                primeslist.Add(activeprime);
                if (numberslist.Count > 0)
                {
                    activeprime = numberslist.Min();
                }
                Latex += ("\\\\activeprime:" + activeprime);
            }
            Latex += ("\\\\primes:");
            foreach (int n in primeslist)
            {
                Latex += (n + ",");
            }
            return primeslist;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                textBoxerrinfo.Text = "";
                int N = int.Parse(textBox2.Text);
                if (N > 3)
                {
                    string Latex = "";
                    List<int> primes = SiebErastothenes(N, out Latex);
                    SetMainImageAndInfo(Latex);
                }
            }
            catch (Exception d)
            {
                textBoxerrinfo.Text = "Error";
            }
        }

        private void textBoxaud_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBoxaudout_Click(object sender, EventArgs e)
        {

        }

        class DataEntry
        {
            public int z;
            public int p;
            public int id;
            public double percentage;
            public DataEntry(int cz, int cp, int cid)
            {
                z = cz;
                p = cp;
                id = cid;
                percentage = 0;
            }
        }


        List<DataEntry> Greedy0(List<DataEntry> l, int Z)
        {
            List<DataEntry> input = l;
            input.Sort((x, y) => (x.z / x.p).CompareTo(y.z / y.p));
            for (int i = 0; i < input.Count; i++)
            {
                if (input.Sum(item => item.z * item.percentage) + input[i].z <= Z)
                {
                    input[i].percentage = 1;
                }
                else
                {
                    input[i].percentage = 0;
                }
            }
            input.Sort((x, y) => (x.id).CompareTo(y.id));
            return input;
        }
        List<DataEntry> Greedy(List<DataEntry> l, int Z)
        {
            List<DataEntry> input = l;
            input.Sort((x, y) => (x.z / x.p).CompareTo(y.z / y.p));
            for (int i = 0; i < input.Count; i++)
            {
                if (input.Sum(item => item.z * item.percentage) + input[i].z <= Z)
                {
                    input[i].percentage = 1;
                }
                else
                {
                    input[i].percentage = (Z - input.Sum(item => item.z * item.percentage)) / input[i].z;
                }
            }
            input.Sort((x, y) => (x.id).CompareTo(y.id));
            return input;
        }
        List<DataEntry> CloneDataentryList(List<DataEntry> inp, bool clonepercentage)
        {
            List<DataEntry> L = new List<DataEntry>();
            foreach (DataEntry D in inp)
            {
                L.Add(CloneDataEntry(D, clonepercentage));
            }
            return (L);
        }
        DataEntry CloneDataEntry(DataEntry d, bool clonepercentage)
        {
            DataEntry C = new DataEntry(d.z, d.p, d.id);
            if (clonepercentage)
            {
                C.percentage = d.percentage;
            }
            else
            {
                C.percentage = 0;
            }
            return (C);
        }


        private void Greedy_0_Click(object sender, EventArgs e)
        {
            try
            {
                List<DataEntry> inp = ParseEntrys(textBoxaud.Text);
                String eingabe_formatiert = DataentrylistToStr(inp);
                List<DataEntry> greedyresult = Greedy0(inp, int.Parse(textBoxZ.Text));
                String output = DataentrylistToStr(greedyresult);
                SetMainImageAndInfo("Input:\\\\" + eingabe_formatiert + "\\\\ Output: \\\\" + output);
            }
            catch (Exception err)
            {
                textBoxerrinfo.Text = "Error:" + err.Message;
            }
        }


        String DataentrylistToStr(List<DataEntry> L)
        {
            String s = "P: " + L.Sum(x => x.p * x.percentage) + "\\\\";
            s += "Z: " + L.Sum(x => x.z * x.percentage) + "\\\\";

            s += @"\pmatrix{";
            s += "i & z & p & \\% \\\\";
            for (int i = 0; i < L.Count; i++)
            {
                s += L[i].id + " & " + L[i].z + " & " + L[i].p + " & " + L[i].percentage;
                s += @"\\";
            }
            s = s.Substring(0, s.Length - 2); //remove last //
            s += "}";

            return (s);
        }
        List<DataEntry> ParseEntrys(String data)
        {
            String[] input = data.Split(';');
            List<DataEntry> inp = new List<DataEntry>();
            int l = 0;
            foreach (String p in input)
            {
                l++;
                inp.Add(new DataEntry(int.Parse(p.Split(',')[0]), int.Parse(p.Split(',')[1]), l));
            }
            return inp;
        }
        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void Fractional_Knapsack(object sender, EventArgs e)
        {
            try
            {
                List<DataEntry> inp = ParseEntrys(textBoxaud.Text);
                String eingabe_formatiert = DataentrylistToStr(inp);
                List<DataEntry> greedyresult = Greedy(inp, int.Parse(textBoxZ.Text));
                String output = DataentrylistToStr(greedyresult);
                SetMainImageAndInfo("Input:\\\\" + eingabe_formatiert + "\\\\ Output: \\\\" + output);
            }
            catch (Exception err)
            {
                textBoxerrinfo.Text = "Error:" + err.Message;
            }
        }

        private void Maximum_Knapsack_Click(object sender, EventArgs e)
        {
            List<DataEntry> inp = ParseEntrys(textBoxaud.Text);
            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add("Root", "Wurzel");
            treeView1.EndUpdate();
            int mp = 0;
            int max = MaxKnapsack(int.Parse(textBoxZ.Text), inp, "Root", "Root_", 0, 0, ref mp);
            textBoxerrinfo.Text = "Maximum is:" + max;
            treeView1.ExpandAll();
        }
        public void AddNode(string parent, string name, string text)
        {
            treeView1.BeginUpdate();

            TreeNode parentNode = treeView1.Nodes.Find(parent, true)[0];
            if (parentNode != null)
            {
                parentNode.Nodes.Add(name, text);
            }
            treeView1.EndUpdate();
        }

        int MaxKnapsack(int Z, List<DataEntry> inp, string nodeparent, string nodename, int maxArchievedP, int alreadyarchievedp, ref int maxlowerbound)
        {
            int upperbound = (int)Math.Floor(Greedy(CloneDataentryList(inp, false), Z).Sum(x => x.percentage * x.p));
            int lowerbound = (int)Math.Floor(Greedy0(CloneDataentryList(inp, false), Z).Sum(x => x.percentage * x.p));

            if (maxlowerbound < lowerbound + alreadyarchievedp)
            {
                maxlowerbound = lowerbound + alreadyarchievedp;
            }

            if (lowerbound > maxArchievedP)
            {
                maxArchievedP = lowerbound;
            }
            int U = (alreadyarchievedp + upperbound);
            int P = maxlowerbound;
            string nodeInfo = "U=" + U + " P=" + P + " Cp:" + maxArchievedP + " " + nodename;
            AddNode(nodeparent, nodename, nodeInfo);
            if (U <= P)
            {
                Console.WriteLine("bound");
                return (P);//do not go further this is useless, can not get a better solution
            }
            if (inp.Count == 0)
            {
                return P;//of course I can get no further points
            }

            //can I take the first Node?
            if (inp[0].z <= Z)
            {
                Console.WriteLine("Can Take " + nodename);
                var tempremoved = CloneDataentryList(inp, false); tempremoved.RemoveAt(0);
                int not_taken = MaxKnapsack(Z, tempremoved, nodename, nodename + "0", maxArchievedP, alreadyarchievedp, ref maxlowerbound);
                int taken = MaxKnapsack(Z - inp[0].z, tempremoved, nodename, nodename + "1", maxArchievedP, alreadyarchievedp + inp[0].p, ref maxlowerbound);
                Console.WriteLine((taken > not_taken) ? "taken" : "not taken");
                return Math.Max(not_taken, taken);
            }
            else
            {
                var tempremoved = CloneDataentryList(inp, false); tempremoved.RemoveAt(0);
                return (MaxKnapsack(Z, tempremoved, nodename, nodename + "0", maxArchievedP, alreadyarchievedp, ref maxlowerbound));
            }
        }
        private void Buttonhoersahl_Click(object sender, EventArgs e)
        {
            try
            {
                List<DataEntry> input = ParseEntrys(textBoxaud.Lines[0]);
                int[] w = new int[textBoxaud.Lines[1].Split(' ').Length];
                for (int i = 0; i < w.Length; i++)
                {
                    w[i] = int.Parse(textBoxaud.Lines[1].Split(' ')[i]);
                }
                textBoxerrinfo.Text = "G:" + HoersahlRecursive(input, input.Count - 1, w);
            }
            catch (Exception exx)
            {
                textBoxerrinfo.Text = "Error:" + exx.Message;
            }

        }
        int HoersahlRecursive(List<DataEntry> input, int i, int[] w)
        {
            if (i == 0)
            {
                return 0;
            }
            else
            {
                if (Predecessor(input, i) != -1)
                {
                    return (Math.Max(HoersahlRecursive(input, i - 1, w), HoersahlRecursive(input, Predecessor(input, i), w) + w[i]));
                }
                else
                {
                    return (HoersahlRecursive(input, i - 1, w));
                }
            }
        }
        int Predecessor(List<DataEntry> data, int i)
        {
            //z ist anfang, p ist ende
            int max = -1;
            int end = -1;
            for (int p = 0; p < data.Count; p++)
            {
                if ((data[p].p <= data[i].z) && data[p].p > end)
                {
                    end = data[p].p;
                    max = p;
                }
            }
            return (max);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            List<DataEntry> input = ParseEntrys(textBoxaud.Lines[0]);
            textBoxerrinfo.Text = "Pred:" + Predecessor(input, int.Parse(textBoxZ.Text));
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBoxHashfkt_TextChanged(object sender, EventArgs e)
        {
            CalcHashTable();
        }

        void CalcHashTable()
        {
            textBoxerrinfo.Text = "";
            try
            {
                //init variables
                String[] input = textBoxDatahash.Text.Split(' ');
                String script = textBoxHashfkt.Text;
                Jint.Engine js = new Jint.Engine();
                int mapsize = (int)js.Execute(script).GetValue("mapsize").AsNumber();
                int[] hashmap = new int[mapsize];
                for (int i = 0; i < hashmap.Length; i++) { hashmap[i] = int.MinValue; }
                string[] toInsertstr = textBoxDatahash.Text.Split(' ');
                String OutputLatex = "Hashing: \\\\";
                //add entrys to hashmap
                for (int i = 0; i < toInsertstr.Length; i++)
                {
                    int tomodify;
                    if (toInsertstr[i].Contains("~"))
                    {
                        tomodify = int.Parse(toInsertstr[i].Substring(1));
                        int iter = 0;
                        int hashvalue = (int)js.Execute(script).GetValue("Hash").Invoke(tomodify, iter).AsNumber();
                        OutputLatex += "Delete(" + tomodify + "," + iter + ")=" + hashvalue + "\\\\";
                        while (iter < hashmap.Length && hashmap[hashvalue] != tomodify && hashmap[hashvalue] != int.MinValue)
                        {
                            iter++;
                            hashvalue = (int)js.Execute(script).GetValue("Hash").Invoke(tomodify, iter).AsNumber();
                            OutputLatex += "Delete(" + tomodify + "," + iter + ")=" + hashvalue + "\\\\";
                        }
                        if (hashmap[hashvalue] == int.MinValue)
                        {
                            OutputLatex += "Item does not exist:" + tomodify + " \\\\";
                        }
                        else
                        {
                            OutputLatex += "Deleted:" + tomodify + " \\\\";
                            hashmap[tomodify] = int.MinValue;
                        }
                    }
                    else
                    {
                        tomodify = int.Parse(toInsertstr[i]);
                        int iter = 0;
                        int hashvalue = (int)js.Execute(script).GetValue("Hash").Invoke(tomodify, iter).AsNumber();
                        OutputLatex += "Hash(" + tomodify + "," + iter + ")=" + hashvalue + "\\\\";
                        while (hashmap[hashvalue] != int.MinValue && iter <= mapsize)
                        {
                            iter++;
                            hashvalue = (int)js.Execute(script).GetValue("Hash").Invoke(tomodify, iter).AsNumber();
                            OutputLatex += "Hash(" + tomodify + "," + iter + ")=" + hashvalue + "\\\\";
                        }
                        if (iter >= mapsize)
                        {
                            OutputLatex += "\\\\ CouldNotInsertFull? \\\\";
                        }
                        hashmap[hashvalue] = tomodify;
                    }
                }
                //output hashmap
                OutputLatex += HashmaptoStr(hashmap);
                Console.WriteLine("Latex:" + OutputLatex);
                SetMainImageAndInfo(OutputLatex);
            }
            catch (Exception err)
            {
                textBoxerrinfo.Text = "Error:" + err.Message;
            }
        }

        string HashmaptoStr(int[] values)
        {
            String s = @"\pmatrix{";
            for (int j = 0; j < values.Length; j++)
            {
                s += j.ToString() + " & ";
            }
            s = s.Substring(0, s.Length - 3);// last remove &
            s += @"\\";
            for (int j = 0; j < values.Length; j++)
            {
                s += values[j].ToString().Replace("-2147483648", " / ") + " & ";
            }
            s = s.Substring(0, s.Length - 3);// last remove &
            s += @"\\";
            s = s.Substring(0, s.Length - 2); //remove last //
            s += "}";

            return (s);
        }



        private void textBoxData_TextChanged(object sender, EventArgs e)
        {
            CalcHashTable();
        }



        private void textBox8_TextChanged(object sender, EventArgs e)
        {

            try
            {
                string toRender = "";
                int[] a, m;
                ChinesischerRestSatz.GetInput(textBoxChinRest.Lines, out a, out m);
                ChinesischerRestSatz.CalculateChinRestSatz(out toRender, m, a);
                SetMainImageAndInfo(toRender);
                textBoxerrinfo.Text = "OK";
            }
            catch (Exception ex)
            {
                textBoxerrinfo.Text = ex.Message;
            }
        }
        class ChinesischerRestSatz
        {
            public static void GetInput(string[] lines, out int[] a, out int[] m)
            {
                a = new int[lines.Length];
                m = new int[lines.Length];
                for (int i = 0; i < lines.Length; i++)
                {
                    a[i] = int.Parse(Regex.Replace(lines[i], @"\s+", "").Split('=')[1].Split(new string[] { "mod" }, StringSplitOptions.None)[0]);
                    m[i] = int.Parse(Regex.Replace(lines[i], @"\s+", "").Split('=')[1].Split(new string[] { "mod" }, StringSplitOptions.None)[1]);
                }
            }
            public static int CalculateChinRestSatz(out string Rechenweg, int[] m, int[] a)
            {
                Rechenweg = "Chinesischer\\:Restsatz \\\\ Input:\\\\";
                for (int i = 0; i < m.Length; i++)
                {
                    Rechenweg += "a_" + i + "=" + a[i] + "," + "m_" + i + "=" + m[i] + "\\\\";
                }
                int m_product = m.Aggregate(1, (d, b) => d * b);
                //calculate m~
                int[] M = new int[m.Length];
                for (int i = 0; i < m.Length; i++)
                {
                    Rechenweg += "M_" + i + "=" + m_product / m[i] + ",";
                    M[i] = m_product / m[i];
                }
                Rechenweg += "\\\\";
                //calculate e
                int[] E = new int[m.Length];
                int[] T = new int[m.Length];
                for (int i = 0; i < m.Length; i++)
                {
                    String log = "";
                    List<int> s;
                    List<int> t;
                    Rechenweg += String.Format("gcd(m_{0},M_{0})=gcd({1},{2})=" + gcd(m[i], M[i], out log, out s, out t) + "\\\\", i, m[i], M[i]);
                    Rechenweg += log + "\\\\";
                    T[i] = t[t.Count - 2];
                    Rechenweg += "t_i =" + T[i] + "\\\\";
                    int e = M[i] * T[i];
                    Rechenweg += " e_" + i + "=M[" + i + "]*T_i" + "=" + M[i] + "*" + T[i] + "=" + e + "\\\\";
                    E[i] = e;
                }
                Rechenweg += "\\\\  X=";
                //calculate X
                int X = 0;
                for (int i = 0; i < m.Length; i++)
                {
                    Rechenweg += a[i] + "*" + E[i] + "+";
                    X += E[i] * a[i];
                }
                Rechenweg = Rechenweg.Substring(0, Rechenweg.Length - 1);
                Rechenweg += "=" + X;
                Rechenweg += "\\\\";
                Rechenweg += "X:" + (X % (m_product)) + "\\:Eindeutig \\: bzgl \\: mod \\:" + (m_product);

                return X;
            }

        }

        private void buttonMillerR_Click(object sender, EventArgs e)
        {
            bool prime = false;
            SetMainImageAndInfo(doMillerRabin(int.Parse(textBoxMiller_n.Text), BigInteger.Parse(textBoxMiller_a.Text), out prime));
        }
        bool isPrimeRabin(int n)
        {
            bool prime = true;
            String Latex = "";
            for (int a = 3; a < 15; a++)
            {
                doMillerRabin(n, a, out prime);
                if (!prime)
                {
                    return false;
                }
            }
            return prime;
        }

        string doMillerRabin(int n, BigInteger a, out bool prime)
        {
            //long[] primes = { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97 };
            prime = false;
            String Latex = "Miller-Rabin\\: Primzahltest\\\\";
            int m = n - 1;
            BigInteger l = 0;
            while (m % 2 != 1)
            {
                ++l;
                m /= 2;
            }
            // n,m,l,a sind nun bestimmt
            Latex += String.Format("n={0},a={1},m={2},l={3}\\\\", n, a, m, l);
            bool terminate;
            List<BigInteger> x = new List<BigInteger>();
            x.Add(BigInteger.Pow(a, m) % n);
            if (x[0] == 1 || x[0] == n - 1)
            {
                Latex += "prime (line 4)\\\\";
                prime = true;
                terminate = true;
            }
            else
            {
                terminate = false;
            }
            int i = 1;
            Latex += String.Format(Utils.Ltx("Iteration:0, X_0={0} \\\\"), x[0]);
            while (terminate == false && i <= l - 1)
            {
                x.Add((x[i - 1] * x[i - 1]) % n);
                Latex += String.Format(Utils.Ltx("Iteration:{0}, X_{0}={1} \\\\"), i, x[i]);
                if (x[i] == n - 1)
                {
                    Latex += Utils.Ltx("prime (line 13) \\\\");
                    prime = true;
                    terminate = true;
                    break;
                }
                else if (x[i] == 1)
                {
                    prime = false;
                    Latex += Utils.Ltx("not prime (line 16) \\\\");
                    terminate = true;
                }
                ++i;
            }
            if (!terminate)
            {
                //prime = false;
                Latex += Utils.Ltx("not prime? (not terminated within loop) \\\\");
            }
            return Latex;
        }
        private void textBoxMiller_n_TextChanged(object sender, EventArgs e)
        {
            //doMillerRabin();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            button2_Click(sender, e);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SetMainImageAndInfo(Utils.Ltx(isPrimeRabin(int.Parse(textBoxMiller_n.Text)) ? "Is a prime" : "Is not a prime"));
        }
        private void button9_Click(object sender, EventArgs e)
        {
            Scheduling A = new Scheduling(textBoxBSProc.Text, textBoxBSGraph.Text);
            SetMainImageAndInfo(A.ScheduleFCFS(), "\\emptyset");
        }

        private void textBoxaudout_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
        string currentShownLatex = "";
        void SetMainImageAndInfo(string LatextoRender, string text = null)
        {
            if (text != null)
            {
                textBoxerrinfo.Invoke((MethodInvoker)delegate
                {
                    textBoxerrinfo.Text = text;
                });
            }
            if (LatextoRender == null)
            {
                return;
            }
            currentShownLatex = LatextoRender;
            LatexViewBox.Invoke((MethodInvoker)delegate
            {
                LatexViewBox.Image = RenderLatex(LatextoRender);
            });
        }

        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                TH1.AUTOMAT DFA = new TH1.AUTOMAT(textBoxth1.Text);
                SetMainImageAndInfo("Table-Filling\\\\" + TH1.TableFilling(DFA));
            }
            catch (Exception ex)
            {
                // textBoxerrinfo.Text = ex.Message;
            }
        }

        private void LatexViewBox_Click(object sender, EventArgs e)
        {
            try
            {
                MouseEventArgs me = (MouseEventArgs)e;
                if (me.Button == MouseButtons.Left)
                {
                    Clipboard.SetImage(LatexViewBox.Image);
                    textBoxerrinfo.Text = "copied Image";
                }
                else
                {
                    Clipboard.SetText(currentShownLatex);
                    textBoxerrinfo.Text = "copied Latex";
                }
            }
            catch (Exception ec)
            {
                textBoxerrinfo.Text = "Could not copy that";
            }
        }


        private void gcd_textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel8_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {
            Scheduling A = new Scheduling(textBoxBSProc.Text, textBoxBSGraph.Text);
            SetMainImageAndInfo(A.ScheduleSPN(), "\\emptyset");
        }
        List<int> getSpurenInput()
        {
            // Parse Input
            string[] spurenStr = textBoxBsSpuren.Text.Split(' ');
            List<int> spuren = new List<int>();
            for (int i = 0; i < spurenStr.Length; i++)
            {
                spuren.Add(int.Parse(spurenStr[i]));
            }
            return spuren;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            //SSTF
            int seekTime = int.Parse(textBoxBSSeek.Text);
            List<int> spuren = getSpurenInput();
            var latexTable = Utils.generate(3, spuren.Count + 1);
            latexTable[0][0] = "ZugriffsZeit";
            latexTable[1][0] = "Spur\t";
            latexTable[2][0] = "SuchZeit";
            int current = spuren[0];
            spuren.Remove(current);
            int time = 0;
            int Tableindex = 2;
            latexTable[0][1] = "0";
            latexTable[1][1] = "" + current;
            latexTable[2][1] = "0";
            while (spuren.Count > 0)
            {
                var temp = new List<int>(spuren);
                temp.Sort((A, B) => (Math.Abs(A - current) - Math.Abs(B - current)));
                int next = temp.ElementAt(0);
                latexTable[1][Tableindex] = "" + next;
                int to_travel = Math.Abs((current - next));
                time += seekTime * to_travel;
                latexTable[0][Tableindex] = "" + time;
                latexTable[2][Tableindex] = "" + seekTime * to_travel;
                current = next;
                spuren.Remove(current);
                Tableindex++;
            }
            SetMainImageAndInfo(Utils.StringArrToLatexMatrix(latexTable), Utils.StringArrToTextTable(latexTable));

        }

        private void button13_Click(object sender, EventArgs e)
        {
            //SSTF
            int seekTime = int.Parse(textBoxBSSeek.Text);
            List<int> spuren = getSpurenInput();
            var latexTable = Utils.generate(3, spuren.Count + 1);
            latexTable[0][0] = "ZugriffsZeit";
            latexTable[1][0] = "Spur\t";
            latexTable[2][0] = "SuchZeit";
            int current = spuren[0];
            spuren.Remove(current);
            int time = 0;
            int Tableindex = 2;
            latexTable[0][1] = "0";
            latexTable[1][1] = "" + current;
            latexTable[2][1] = "0";
            while (spuren.Count > 0)
            {
                int next = spuren.ElementAt(0);
                latexTable[1][Tableindex] = "" + next;
                int to_travel = Math.Abs((current - next));
                time += seekTime * to_travel;
                latexTable[0][Tableindex] = "" + time;
                latexTable[2][Tableindex] = "" + seekTime * to_travel;
                current = next;
                spuren.Remove(current);
                Tableindex++;
            }
            SetMainImageAndInfo(Utils.StringArrToLatexMatrix(latexTable), Utils.StringArrToTextTable(latexTable));
        }

        private void button14_Click(object sender, EventArgs e)
        {
            // Elevator
            int seekTime = int.Parse(textBoxBSSeek.Text);
            List<int> spuren = getSpurenInput();
            var latexTable = Utils.generate(3, spuren.Count + 1);
            latexTable[0][0] = "ZugriffsZeit";
            latexTable[1][0] = "Spur\t";
            latexTable[2][0] = "SuchZeit";
            int current = spuren[0];
            spuren.Remove(current);
            int time = 0;
            int Tableindex = 2;
            latexTable[0][1] = "0";
            latexTable[1][1] = "" + current;
            latexTable[2][1] = "0";
            int direction = 0;
            while (spuren.Count > 0)
            {
                int next;
                switch (direction)
                {
                    case 0:
                        var temp = new List<int>(spuren);
                        temp.Sort((A, B) => (Math.Abs(A - current) - Math.Abs(B - current)));
                        next = temp.ElementAt(0);
                        direction = Math.Sign(next - current);
                        break;
                    case 1:
                        var temp1 = new List<int>(spuren);
                        temp1.Sort((A, B) => (A - B));
                        temp1.RemoveAll(x => x < current);
                        if (temp1.Count == 0)
                        {
                            direction = -1;
                            continue;
                        }
                        next = temp1.ElementAt(0);
                        break;
                    case -1:
                        var temp2 = new List<int>(spuren);
                        temp2.Sort((A, B) => (B - A));
                        temp2.RemoveAll(x => x > current);
                        if (temp2.Count == 0)
                        {
                            direction = 1;
                            continue;
                        }
                        next = temp2.ElementAt(0);
                        break;
                    default:
                        throw new Exception("Programmer Error");
                }
                latexTable[1][Tableindex] = "" + next;
                int to_travel = Math.Abs((current - next));
                time += seekTime * to_travel;
                latexTable[0][Tableindex] = "" + time;
                latexTable[2][Tableindex] = "" + seekTime * to_travel;
                current = next;
                spuren.Remove(current);
                Tableindex++;
            }
            SetMainImageAndInfo(Utils.StringArrToLatexMatrix(latexTable), Utils.StringArrToTextTable(latexTable));

        }

        private void button8_Click(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 2;// TODO make this universal this changes the active Field "Koerper" to Reals
            string[] xvalues = textBoxNumX.Text.Split(',');
            string[] yvalues = textBoxNumF.Text.Split(',');
            // calculate Divided Differences
            Matrix CalcTable = new Matrix(xvalues.Length, xvalues.Length + 1);
            // init x values
            for (int i = 0; i < CalcTable.m; i++)
            {
                CalcTable.data[i][0] = new KNG(xvalues[i]);

            }
            // init y values
            for (int i = 0; i < CalcTable.m; i++)
            {
                CalcTable.data[i][1] = new KNG(yvalues[i]);
            }
            for (int i = 2; i < CalcTable.n; i++)
            {
                for (int c = 0; c < i - 2; c++)
                {
                    KNG P_unten = CalcTable.data[c][i - 1];
                    KNG P_oben = CalcTable.data[c][i - 2];
                    KNG X_unten = CalcTable.data[c][0];
                    KNG X_oben = CalcTable.data[c + i - 2][0];
                    CalcTable.data[c][i] = (P_unten - P_oben) / (X_unten - X_oben);
                }
            }

            SetMainImageAndInfo(CalcTable.ToString());
        }

        private void textBox8_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void button15_Click(object sender, EventArgs e)
        {
            Scheduling A = new Scheduling(textBoxBSProc.Text, textBoxBSGraph.Text);
            if (textBoxBSProc.Text.Contains("MINSWAP"))
            {
                SetMainImageAndInfo(A.ScheduleSRTF(true), "\\emptyset");
            }
            else
            {
                SetMainImageAndInfo(A.ScheduleSRTF(false), "\\emptyset");
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            try
            {
                Scheduling A = new Scheduling(textBoxBSProc.Text, textBoxBSGraph.Text);
                if (textBoxBSProc.Text.Contains("MINSWAP"))
                {
                    SetMainImageAndInfo(A.ScheduleRR(true), "\\emptyset");
                }
                else
                {
                    SetMainImageAndInfo(A.ScheduleRR(false), "\\emptyset");
                }
            }
            catch (Exception ex)
            {
                SetMainImageAndInfo("err", "Error:" + ex.Message);
            }
        }

        private void textBox8_TextChanged_2(object sender, EventArgs e)
        {
            // TODO implement
        }
        private void button17_Click(object sender, EventArgs e)
        {

        }

        void UpdateRDBDRCTRC()
        {
            try
            {
                string processed = richTextBoxRDB1SET.Text;

                Regex usedmacros = new Regex(@"(?<FUNC>\w+)\[(?<param>[a-z.A-Z,äüßöÄÜß_]+)\]");
                var matches = usedmacros.Matches(processed);
                while (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        string newText = "";
                        string func = match.Groups["FUNC"].ToString();
                        string param = match.Groups["param"].ToString();
                        string variable = param.Split(',')[0].Split('.')[0];
                        string attribute = param.Split(',')[0].Split('.')[1];
                        string table = param.Split(',')[1];
                        switch (comboBoxRDB.Text)
                        {
                            case "TRC":
                                switch (func)
                                {
                                    case "MAX":
                                        newText = table + "(" + variable + ") AND NOT EXISTS y(" + table + "(y) AND y." + attribute + " < " + variable + "." + attribute + ")";
                                        break;
                                    case "MIN":
                                        newText = table + "(" + variable + ") AND NOT EXISTS y(" + table + "(y) AND y." + attribute + " > " + variable + "." + attribute + ")";
                                        break;
                                }
                                break;
                            case "DRC":
                                switch (func)
                                {
                                    case "MAX":
                                        newText = "" + table + "(x1,x2, x3) AND NOT EXISTS y1, y2, " + param + "_alt (" + table + "(a2, b2, id2) AND " + param + "_alt < " + param + ")}";
                                        break;
                                    case "MIN":
                                        newText = "" + table + "(x1,x2, x3) AND NOT EXISTS y1, y2, " + param + "_alt (" + table + "(a2, b2, id2) AND " + param + "_alt > " + param + ")}";
                                        break;
                                }
                                break;
                        }
                        processed = processed.Remove(match.Index, match.Length).Insert(match.Index, newText);
                        richTextBoxRDB1SET.Text = processed;
                    }
                    matches = usedmacros.Matches(processed);
                }
            }
            catch (Exception e)
            {
                SetMainImageAndInfo(null, "Error parsing makros TRC requires syntax variable.attribute" + e.Message);
            }
            //   highlightWords(ref richTextBoxRDB1SET, TRCDRCKEYWORDS);
            // highlightAssociatedBracketIfApplicable(ref richTextBoxRDB1SET);
            //  userSelection = true;
        }



        private void richTextBoxRDB1SET_TextChanged(object sender, EventArgs e)
        {
            UpdateRDBDRCTRC();
        }

        private void button17_Click_1(object sender, EventArgs e)
        {
            try
            {
                string input = textBoxRTFMission.Text;
                Regex format = new Regex(@"(?<var>\w+)\=(?<value>\w+)");
                var Matches = format.Matches(input);
                string newMatches = KNG.K_Functions;
                Dictionary<string, string> value = new Dictionary<string, string>();
                foreach (Match match in Matches)
                {
                    value[match.Groups["var"].ToString()] = match.Groups["value"].ToString();
                }
                SetMainImageAndInfo(RTF1.CalculateInterplanetaryMissionAsTex(value["StartPlanet"], value["ZielPlanet"], double.Parse(value["StartHoehe"]), double.Parse(value["ZielHoehe"])));
            }catch(Exception ex)
            {
                SetMainImageAndInfo(null, ex.Message);
            }
            }

    }
}


