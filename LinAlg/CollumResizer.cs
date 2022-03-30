using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UniRechner
{
    // maybee refactor this
    class CollumResizer
    {
        TableLayoutPanel panel;
        int grabSize;
        public CollumResizer(ref TableLayoutPanel _panel,int grabSize)
        {
            panel = _panel;
            panel.MouseMove += panel_SelectBeamShowCursorIfApplicable;
            panel.MouseDown += panel_SelectBeamShowCursorIfApplicable;
           
            this.grabSize = grabSize;
        }    
        int movableIndex = -1;
        bool lastMoved = false;
        private void panel_SelectBeamShowCursorIfApplicable(object sender, MouseEventArgs e)
        {
            panel.FindForm().Cursor = Cursors.Default;


            List<int> beams = getBeams();
            if (!(lastMoved && e.Button == System.Windows.Forms.MouseButtons.Left))
            {
                movableIndex = beams.FindIndex(0,beams.Count-1,x => x < e.X + grabSize && x > e.X - grabSize && x!=0);
            }
            if(!(e.Button == System.Windows.Forms.MouseButtons.Left))
            {
                lastMoved = false;
            }
           // Console.WriteLine("movableindex:" + movableIndex);
            if (movableIndex != -1)
            {
               // panel.FindForm().Cursor = Cursors.VSplit;
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    // move this beam
                    int newSize;
                    if (movableIndex == 0)
                    {
                        newSize = e.X;
                    }
                    else
                    {
                        newSize = e.X - beams[movableIndex - 1];
                    }
                    panel.ColumnStyles[movableIndex].SizeType = SizeType.Absolute;
                    panel.ColumnStyles[movableIndex].Width = newSize;

                    panel.ColumnStyles[movableIndex+1].SizeType = SizeType.Absolute;
                    panel.ColumnStyles[movableIndex+1].Width = Math.Abs(panel.ColumnStyles[movableIndex + 1].Width-(newSize - beams[movableIndex]));
                    lastMoved = true;
                }
                else
                {
                    lastMoved = false;
                }
            }
            else
            {
                panel.FindForm().Cursor = Cursors.Default;
                lastMoved = false;
            }
        }
        private List<int> getBeams()
        {
            List<int> beams = new List<int>();
            int prev = 0;
            for(int i = 0;i<panel.ColumnStyles.Count;i++)
            {
                switch (panel.ColumnStyles[i].SizeType)
                {
                    case SizeType.Absolute:
                        beams.Add((int)panel.ColumnStyles[i].Width + prev);
                        prev += (int)panel.ColumnStyles[i].Width;
                        break;     
                    default:
                        throw new NotImplementedException("Sizetypes different than Absolute,Percent are unsupported");
                }
            }
            beams.Sort();
            return beams;
        }
    }
}
