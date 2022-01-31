using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinAlg
{
    class DatenflussAnalyse
    {
        public List<Block> Blocks;
        public DatenflussAnalyse(String Code)
        {
            String[] lines = Code.Split(new[] { "\r\n", "\r", "\n",Environment.NewLine},StringSplitOptions.RemoveEmptyEntries);
            
        }
        public class Block
        {
            String text;
        }
    }
}
