using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinAlg
{
    class RTF1
    {
       struct Planet
        {
            public char Index;         // Abkürzung des Planete
            public decimal r;          // Radius in km
            public decimal M;          // Masse in kg   
            public decimal my;         // μ in  km^3/s^2
            public decimal a;          // Große Halbachse in km (zur Sonne)
            public decimal v;          //in km/s
        }
        Planet erde = new Planet { Index = 'E',r=6378m,M=5.9742E24m,my=398599m,a=149599366m
        };
        Dictionary<char, Planet> planet;
    }
}
