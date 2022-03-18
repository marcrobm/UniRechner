using LinAlg;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UniRechner
{
    class TextHighlighter
    {
        RichTextBox box;
        bool userSelection = true;

        (string s, Color c)[] toHighlight = new (string s, Color c)[] {
            ("AND",Color.Green), ("OR",Color.Green), ("NOT",Color.Green),
            ("FORALL",Color.Green), ("EXISTS",Color.Green),
            ("->",Color.Green),
            ("<=",Color.Green), (">=",Color.Green),("<>",Color.Green),("!=",Color.Green),
            ("|",Color.Red),("{",Color.Red),("}",Color.Red)

        };
        Color errorBracketHighlight = Color.Red;
        Color BracketHighlight = Color.DarkSlateBlue;
        

        public TextHighlighter(ref RichTextBox box)
        {
            this.box = box;
            box.SelectionChanged += updateHighlighting;
            box.TextChanged += updateHighlighting;
        }
        private void updateHighlighting(object sender, EventArgs e)
        {
            if (userSelection)
            {
                userSelection = false;
                ClearHighlightsAndhighlightWords(ref box);
                highlightAssociatedBracketIfApplicable();
                userSelection = true;
            }
        }
        void highlightAssociatedBracketIfApplicable()
        {
            userSelection = false;
            int openingpos = -1;
            int closedpos = -1;
            if (box.SelectionStart < box.Text.Length && box.Text[box.SelectionStart] == ')')
            {
                openingpos = Utils.openingBraketPos(box.Text, box.SelectionStart);
                closedpos = box.SelectionStart;
            }
            else if (box.SelectionStart < box.Text.Length && box.Text[box.SelectionStart] == '(')
            {
                openingpos = box.SelectionStart;
                closedpos = Utils.closingBraketPos(box.Text, box.SelectionStart);
            }
            int t1 = box.SelectionStart;
            int t2 = box.SelectionLength;


            if (openingpos != -1 && closedpos != -1)
            {
                box.SelectionStart = openingpos;
                box.SelectionLength = 1;
                box.SelectionBackColor = BracketHighlight;

                box.SelectionStart = closedpos;
                box.SelectionLength = 1;
                box.SelectionBackColor = BracketHighlight;                       
            }
            else if(openingpos!=-1)
            {
                box.SelectionStart = openingpos;
                box.SelectionLength = 1;
                box.SelectionBackColor = errorBracketHighlight;
            }
            else if (closedpos != -1)
            {
                box.SelectionStart = closedpos;
                box.SelectionLength = 1;
                box.SelectionBackColor = errorBracketHighlight;
            }
            box.SelectionStart = t1;
            box.SelectionLength = t2;
            box.SelectionColor = Color.Black;
            box.SelectionBackColor = Color.White;
            userSelection = true;
        }

        // TODO implement some bracket parsing
        void ClearHighlightsAndhighlightWords(ref RichTextBox box)
        {
            userSelection = false;
            int prevs = box.SelectionStart;
            int prevl = box.SelectionLength;
            // remove previous highlights
            box.SelectionStart = 0;
            box.SelectAll();
            box.SelectionColor = Color.Black;
            box.SelectionBackColor = Color.White;
            // do new highlighting
            foreach (var item in toHighlight)
            {
                int startindex = 0;
                while (startindex < box.TextLength)
                {
                    int wordstartIndex = box.Find(item.s, startindex, RichTextBoxFinds.WholeWord);
                    if (wordstartIndex != -1)
                    {
                        box.SelectionStart = wordstartIndex;
                        box.SelectionLength = item.s.Length;
                        box.SelectionColor = item.c;
                    }
                    else
                    {
                        break;
                    }
                    startindex = wordstartIndex + item.s.Length;
                }
            }
            box.SelectionStart = prevs;
            box.SelectionLength = prevl;
            box.SelectionColor = Color.Black;
            userSelection = true;
        }

    }
}
