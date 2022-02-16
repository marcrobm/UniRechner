using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinAlg
{   //modern
    public class KNG
    {
        public Jint.Native.JsValue val;
        //Helper die Koerper definieren
        public static Jint.Engine js = new Jint.Engine();
        public static String K_Functions;
        public static KNG mult_inv(KNG c1)
        {
            KNG A = new KNG();
            A.val = js.Execute(K_Functions).GetValue("mult_inv").Invoke(c1.val);
            return A;
        }
        public static KNG add_inv(KNG c1)
        {
            KNG A = new KNG();
            A.val = js.Execute(K_Functions).GetValue("add_inv").Invoke(c1.val);
            return A;
        }
        public static KNG abs(KNG c1)
        {
            KNG A = new KNG();
            A.val = js.Execute(K_Functions).GetValue("abs").Invoke(c1.val);
            return A;
        }
        public static bool operator <(KNG c1, KNG c2)
        {
           return js.Execute(K_Functions).GetValue("smaller").Invoke(c1.val, c2.val).AsBoolean();
        }
        public static bool operator >(KNG c1, KNG c2)
        {
            return js.Execute(K_Functions).GetValue("bigger").Invoke(c1.val, c2.val).AsBoolean();
        }
        public static KNG operator +(KNG c1,KNG c2)
        {
            KNG A = new KNG();
            A.val = js.Execute(K_Functions).GetValue("add").Invoke(c1.val, c2.val);
            return A;
        }
        public static KNG operator -(KNG c1, KNG c2)
        {
            KNG A = new KNG();
            A.val = js.Execute(K_Functions).GetValue("add").Invoke(c1.val, add_inv(c2).val);
            return A;
        }
        public static KNG operator *(KNG c1, KNG c2)
        {
            KNG A = new KNG();
            A.val = js.Execute(K_Functions).GetValue("multiply").Invoke(c1.val, c2.val);
            return A;
        }
        public static KNG operator /(KNG c1, KNG c2)
        {
            KNG A = new KNG();
            A.val = js.Execute(K_Functions).GetValue("multiply").Invoke(c1.val, mult_inv(c2).val);
            return A;
        }
        public override string ToString()
        {
            return(js.Execute(K_Functions).GetValue("tostr").Invoke(val).ToString());
        }
        public override bool Equals(object obj)
        {
            return ((KNG)obj).ToString().Equals(this.ToString());    
        }
        public KNG(String V)
        {
            val = js.Execute(K_Functions).GetValue("fromstr").Invoke(V);
        }
        public KNG()
        {
            val = js.Execute(K_Functions).GetValue("get0").Invoke();
        }
        public static KNG zero()
        {
            KNG A = new KNG();
            A.val = js.Execute(K_Functions).GetValue("get0").Invoke();
            return A;
        }
        public static KNG one()
        {
            KNG A = new KNG();
            A.val = js.Execute(K_Functions).GetValue("get1").Invoke();
            return A;
        }
        public KNG Clone()
        {
            KNG A = new KNG();
            A.val = val;
            return A;
        }

    }
}
