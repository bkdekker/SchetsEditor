using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace SchetsEditor
{
    public class Element
    {
        public ISchetsTool tool;
        public bool tekenen;
        public Point p1;
        public Point p2;
        public Brush kleur;
        public string tekst;
        public SchetsControl s;
        //public List<Elem>

        public Element Get()
        {
            return this;
        }
        public Element(ISchetsTool tool, Point p1, Point p2, Brush kleur, bool tekenen)
        {
            this.tekenen = tekenen;
            this.tool = tool;
            this.p1 = p1;
            this.p2 = p2;
            this.kleur = kleur;
            //this.
            
        }
        public Element(ISchetsTool tool, Point p1, Point p2, Brush kleur, string tekst, bool tekenen)
        {

            this.tool = tool;
            this.p1 = p1;
            this.p2 = p2;
            this.kleur = kleur;
            this.tekenen = tekenen;

            if (tekst != null)
            {
                this.tekst = tekst;
            }
        }
        public static Rectangle Punten2Rechthoek(Point p1, Point p2)
        {
            return new Rectangle(new Point(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y))
                                , new Size(Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y))
                                );
        }
        public static Pen MaakPen(Brush b, int dikte)
        {
            Pen pen = new Pen(b, dikte);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            return pen;
        }
        public void teken(Object obj, Graphics g, Point p1, Point p2)
        {
            if (tool.GetType() == typeof(PenTool))
            {
                g.DrawLine(MaakPen(kleur, 3), p1, p2);
            }
            else if (tool.GetType() == typeof(LijnTool))
            {
                g.DrawLine(MaakPen(kleur, 3), p1, p2);
            }
            else if (tool.GetType() == typeof(VolRechthoekTool))
            {
                g.FillRectangle(kleur, TweepuntTool.Punten2Rechthoek(p1, p2));
            }
            else if (tool.GetType() == typeof(RechthoekTool))
            {
                g.DrawRectangle(MaakPen(kleur, 3), TweepuntTool.Punten2Rechthoek(p1, p2));
            }
        }

    }
}
