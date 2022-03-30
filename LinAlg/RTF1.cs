using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LinAlg.Utils;
namespace LinAlg
{
    class RTF1
    {
       struct Planet
        {
            public char Index;        // Abkürzung des Planeten
            public double r;          // Radius in km
            public double M;          // Masse in kg   
            public double my;         // μ in  km^3/s^2
            public double a;          // Große Halbachse in km (zur Sonne)
            public double v;          // in km/s
            public string name;       // name des Planeten
        }
        Dictionary<string,Planet> Planeten;
        static Planet sonne = new Planet { Index = 'H',r=6.69E5,M=1.989E30,my=1.3271E11,a=0,v=0,name="Sonne"};
        static Planet venus = new Planet { Index = 'V',r=6052,M=4.869E24,my=324860,a=108208777,v=35.020, name = "Venus" };
        static Planet erde = new Planet { Index = 'E',r=6378,M=5.9742E24,my=398599,a=149599366,v=29.784, name = "Erde" };
        static Planet mars = new Planet { Index = 'M',r=3397,M=6.4191E23,my=42828,a=227946314,v=24.129, name = "Mars" };
        static Planet jupiter = new Planet { Index = 'J',r=71398,M=1.8988E27,my=126687936,a=778344254,v=13.058, name = "Jupiter" };
        static Planet saturn = new Planet { Index = 'S',r=60000,M=5.686E26,my=37930320,a=1425945953,v=9.647, name = "Saturn" };
        static Planet neptun = new Planet { Index = 'N', r = 24764, M = 1.024E26, my = 6832128, a = 4.495E9, v = 5.43, name = "Neptun" };
        static Dictionary<string, Planet> P = new Dictionary<string, Planet>();
        static RTF1(){
            P.Add("H",sonne); P.Add("V",venus);P.Add("E",erde);P.Add("M",mars);P.Add("J",jupiter);P.Add("S",saturn); P.Add("N", neptun);
        }
        // Eingabe: Abkuerzung des Start/Ziel-Planeten sowie die jeweiligen Bahnhoehen
        public static String CalculateInterplanetaryMissionAsTex(string P_start, string P_ziel, double h_s, double h_z) {
            Planet P_s = P[P_start];
            Planet P_z = P[P_ziel];
            string Latex = Utils.Ltx("Berechnen eine Interplanetare Mission " + P_s.name + " mit h:" + h_s + " \\Rightarrow " + P_z.name + " mit h:" + h_z) + NL;
            // Hohmann Transfer
            Latex += Ltx("Berechnen als erstes einen Hohmann Transfer zwischen den Planeten");
            bool innerToOuter = P_z.a > P_s.a;
            Latex += Ltx("(vom " + (innerToOuter ? "inneren" : "ausseren") +" zum "+ (!innerToOuter ? "inneren" : "ausseren") + " Planeten) ") + NL;
            double r_p = (innerToOuter ? P_s.a : P_z.a);
            double r_a = (innerToOuter ? P_z.a : P_s.a);
            Latex += Ltx("Es gilt somit r_p = " + r_p + "km sowie r_a = " + r_a + "km") + NL;
            // berechnen epsilon
            double ε = (r_a - r_p) / (r_a + r_p);
            Latex += "\\epsilon_{hohmann} = \\frac{r_a-r_p}{r_a+r_p}=" + "\\frac{" + r_a + "-" + r_p + "}{" + r_a + "+" + r_p + "}=" + str(ε) + NL;
            // grosse Halbachse 
            double a = (r_a + r_p) / 2;
            Latex += Utils.Ltx("grosse Halbachse  ") + "a =\\frac{r_a+r_p}{2} =" + "\\frac{" + r_a + "+" + r_p + "}{2}=" + a + "km " + Utils.NL;
            // Bahnparameter
            double p = r_p * (1 + ε);
            Latex += Utils.Ltx("-Nutzen Bahngleichung zur Berechnung vom Bahnparameter") + NL + NL;
            Latex += "p=r_p*(1+\\epsilon)=" + r_p + "*(1+" + str(ε) + ")=" + str(p) + "km" + NL;
            // Apozentrums / Perizentrums Geschwindigkeit
            double v_ph = Math.Sqrt(P["H"].my * (2 / r_p + (ε * ε - 1) / p));
            Latex += Utils.Ltx("-Nutzen Vis-Viva-Integral zur Berechnung der Perizentrums und Apozentrums Geschwindigkeit") + NL + NL;
            Latex += "v_p = \\sqrt{\\mu_H(\\frac{2}{r_p}+\\frac{\\epsilon^2-1}{p})}=" + str(v_ph) + "km/s " + TAB;
            double v_ah = Math.Sqrt(P["H"].my * (2 / r_a + (ε * ε - 1) / p));
            Latex += "v_a = \\sqrt{\\mu_H(\\frac{2}{r_a}+\\frac{\\epsilon^2-1}{p})}=" + str(v_ah) + "km/s" + NL;

            double Tu = Math.PI * Math.Sqrt((a * a * a) / P["H"].my);
            Latex += "T_U= \\pi * \\sqrt{ \\frac{a^3}{ \\mu_h}} = \\pi * \\sqrt{ \\frac{" + str(a) + "^3}{ " + str(P["H"].my) + "}} ="+str(Tu)+"s =" + str(Tu/60/60/24/365) + "a" + NL;


            Latex += Utils.Ltx("-Berechnen den Antriebsbedarf fuer den Hohmann Transfer ") + Utils.NL;
            double deltav1h;
            double deltav2h;
            if (innerToOuter)
            {
                deltav1h = v_ph - P_s.v;
                Latex += Ltx("\\Delta v_1^h = v_p^h-v_" + P_start + "^h = " + str(v_ph) + "-" + str(P_s.v) + "=" + str(deltav1h) + "km/s") + NL;
                deltav2h = P_z.v - v_ah;
                Latex += Ltx("\\Delta v_2^h = v_" + P_ziel + "-v_a^h = " + str(P_z.v) + "-" + str(v_ah) + "=" + str(deltav2h) + "km/s") + NL;
            }
            else
            {
                deltav1h = P_s.v-v_ah;
                Latex += Ltx("\\Delta v_1^h = v_" + P_start + "^h - v_a^h =" + str(P_s.v) + "-" + str(v_ah) + "=" + str(deltav1h) + "km/s") + NL;
                deltav2h = v_ph-P_z.v;
                Latex += Ltx("\\Delta v_2^h = v_p^h -v_"+P_z.v+" =" + str(v_ph) + "-" + str(P_z.v) + "=" + str(deltav2h) + "km/s") + NL;
            }
            double deltavtoth = deltav1h+deltav2h;
            Latex += Ltx("\\Delta v_{tot}^h =\\Delta v_1^h + \\Delta v_2^h="+str(deltav1h)+"+"+str(deltav2h) + "="+str(deltavtoth)+ "km/s") + NL;
            Latex += Ltx("Berechnen nun die Flucht Hyperbel aus dem Gravitationsfeld des "+P_s.name) + NL;
            Latex += Ltx("Hierzu setzen wir nun ")+"v_{\\infty,Pl} = \\Delta v_1^h"+Ltx(" , da die Fluchtgeschwindigkeit = erste Geschwindigkeit (am Anfang des Fluges)")+ NL;
            Latex += Ltx("Berechnen die Kreisbahngeschwindigkeit und Perizentrumsgeschwindigkeit") + NL;
            double v_k = Math.Sqrt(P_s.my / (P_s.r + h_s));
            Latex += "v_k= \\sqrt{\\frac{\\mu_"+P_s.Index+"}{r_"+P_s.Index+"+"+ h_s + "km}}="+str(v_k)+"km/s "+TAB;
            double v_p = Math.Sqrt(2 * v_k * v_k + deltav1h * deltav1h);
            Latex += "v_p=\\sqrt{2*v_k^2 + (\\Delta v_1^h)^2}"+ "="+ str(v_p)+"km/s"+NL;
            double deltav1 = v_p - v_k;
            Latex += "\\Delta v_1 = v_p-v_k=" + str(v_p)+"-"+str(v_k)+"="+str(deltav1)+NL;
            Latex += Ltx("Berechnen nun das Einschwenken in den Orbit setzen analog v_{\\infty} = \\Delta v_2^h") + NL;
            double vbziel = Math.Sqrt(P_z.my/(P_z.r+h_z));
            string bziel = "v_{" + P_z.Index + "," + h_z + "}";
            Latex += Ltx(bziel +"=\\sqrt{\\frac{\\mu_"+P_ziel+"}{r_"+P_ziel+"+"+h_z+"}}=")+str(vbziel) + NL;
            double vpPl = Math.Sqrt(2 * vbziel * vbziel + deltav2h * deltav2h);
            Latex += "v_{p,Pl}=\\sqrt{2*"+bziel+"^2 + (\\Delta v_2^h)^2}="+str(vpPl)+" km/s " + NL;
            double deltav2 = vpPl - vbziel;
            Latex += "\\Delta v_2 = v_{p,Pl}-" + bziel +"="+ str(vpPl)+"-"+str(vbziel) +"="+ str(deltav2) + "km/s " +NL;
            double deltavtot = deltav1+deltav2;
            Latex += "\\Delta v_{total} = \\Delta v_1 + \\Delta v_2 =" + str(deltav1) + "+" + str(deltav2) + "=" + str(vbziel) + "=" + str(deltavtot) + "km/s "+NL;
            return Latex;
        }                                                                                                        
        
    }
}
