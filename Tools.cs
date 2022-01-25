using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SchetsEditor
{
    public interface ISchetsTool
    {
        void MuisVast(SchetsControl s, Point p);
        void MuisDrag(SchetsControl s, Point p);
        void MuisLos(SchetsControl s, Point p);
        void Letter(SchetsControl s, char c);
    }

    public abstract class StartpuntTool : ISchetsTool
    {
        protected Point startpunt;
        protected Brush kwast;
        //nieuw
        protected int dikte_van_pen;

        public virtual void MuisVast(SchetsControl s, Point p)
        {   startpunt = p;
        }
        public virtual void MuisLos(SchetsControl s, Point p)
        {   kwast = new SolidBrush(s.PenKleur);
            //nieuw
            dikte_van_pen = s.Pendikte;
        }
        public abstract void MuisDrag(SchetsControl s, Point p);
        public abstract void Letter(SchetsControl s, char c);
    }

    public class TekstTool : StartpuntTool
    {
        public override string ToString() { return "tekst"; }

        public override void MuisDrag(SchetsControl s, Point p) { }

        public override void Letter(SchetsControl s, char c)
        {
            if (c >= 32)
            {
                Graphics gr = s.MaakBitmapGraphics();
                Font font = new Font("Tahoma", 40);
                string tekst = c.ToString();
                SizeF sz = 
                gr.MeasureString(tekst, font, this.startpunt, StringFormat.GenericTypographic);
                gr.DrawString   (tekst, font, kwast, 
                                              this.startpunt, StringFormat.GenericTypographic);
                // gr.DrawRectangle(Pens.Black, startpunt.X, startpunt.Y, sz.Width, sz.Height);
                
                TekstTool l = new TekstTool();
                Point test = new Point(startpunt.X + 2*(int)sz.Width, startpunt.Y + 2*(int)sz.Width);
                Element k = new Element(l, startpunt, test, kwast, true, tekst, dikte_van_pen);
                s.Schets.elements_list.Add(k);
                
                startpunt.X += (int)sz.Width;
                s.Invalidate();
                
            }
        }
    }

    public abstract class TweepuntTool : StartpuntTool
    {
        public static Rectangle Punten2Rechthoek(Point p1, Point p2)
        {   return new Rectangle( new Point(Math.Min(p1.X,p2.X), Math.Min(p1.Y,p2.Y))
                                , new Size (Math.Abs(p1.X-p2.X), Math.Abs(p1.Y-p2.Y))
                                );
        }
        public Pen MaakPen(Brush b, int dikte)
            //aangepast, hier stond eerst Brush b, Int dikte
        {   Pen pen = new Pen(b, dikte);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            return pen;
        }
        public override void MuisVast(SchetsControl s, Point p)
        {   base.MuisVast(s, p);
            kwast = Brushes.Gray;
        }
        public override void MuisDrag(SchetsControl s, Point p)
        {   s.Refresh();
            this.Bezig(s.CreateGraphics(), this.startpunt, p);
        }
        public override void MuisLos(SchetsControl s, Point p)
        {   base.MuisLos(s, p);
            this.Compleet(s, s.MaakBitmapGraphics(), this.startpunt, p);
            s.Invalidate();
        }
        public override void Letter(SchetsControl s, char c)
        {
        }
        public abstract void Bezig(Graphics g, Point p1, Point p2);
        
        public virtual void Compleet(SchetsControl s, Graphics g, Point p1, Point p2)
        {
            this.Bezig(g, p1, p2);
        }
    }

    public class RechthoekTool : TweepuntTool
    {
        public override string ToString() { return "kader"; }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {   
            // hier stond eerst
            g.DrawRectangle(MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(p1, p2)); 
        }
        public override void Compleet(SchetsControl s, Graphics g, Point p1, Point p2)
        {
          
            RechthoekTool l = new RechthoekTool();
            Element k = new Element(l, p1, p2, kwast, true, "rechthoek", dikte_van_pen);
            s.Schets.elements_list.Add(k);
            this.Bezig(g, p1, p2);
            

        }
    }
    
    public class VolRechthoekTool : RechthoekTool
    {
        //public bool volrechthoek;
        public override string ToString() { return "vlak"; }

        public override void Compleet(SchetsControl s, Graphics g, Point p1, Point p2)
        {
            VolRechthoekTool l = new VolRechthoekTool();
            Element k = new Element(l, p1, p2, kwast, true, "volrechthoek", dikte_van_pen);
            s.Schets.elements_list.Add(k);
            g.FillRectangle(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));

        }
    }

    public class LijnTool : TweepuntTool
    {
        public override string ToString() { return "lijn"; }

        public override void Bezig(Graphics g, Point p1, Point p2)
            //dit is aangepast
        {   g.DrawLine(MaakPen(this.kwast, dikte_van_pen), p1, p2);
        }
        public override void Compleet(SchetsControl s, Graphics g, Point p1, Point p2)
        {

            LijnTool l = new LijnTool();
            Element k = new Element(l, p1, p2, kwast, true, "LijnTool", dikte_van_pen);
            s.Schets.elements_list.Add(k);
            this.Bezig(g, p1, p2);
        }
    }

    public class PenTool : LijnTool
    {
        public override string ToString() { return "pen"; }

        public override void MuisDrag(SchetsControl s, Point p)
        {   this.MuisLos(s, p);
            this.MuisVast(s, p);
        }
        public override void Compleet(SchetsControl s, Graphics g, Point p1, Point p2)
        {
            PenTool l = new PenTool();
            Element k = new Element(l, p1, p2, kwast, true, "PenTool", dikte_van_pen);
            s.Schets.elements_list.Add(k);
            this.Bezig(g, p1, p2);
        }

    }
    
    public class GumTool : TweepuntTool
    {
        public override string ToString() { return "gum"; }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            //g.DrawLine(MaakPen(Brushes.White, 7), p1, p2);
        }
        public override void Compleet(SchetsControl s, Graphics g, Point p1, Point p2)
        {
            EventArgs ea = new EventArgs();
            s.Schoon(this, ea);
            for (int i = s.Schets.elements_list.Count - 1; i >= 0; i--)
            {
                Element k = s.Schets.elements_list[i];
                Rectangle test = new Rectangle();
                test = Punten2Rechthoek(k.p1, k.p2);
                if (test.Contains(p2))
                {
                    k.tekenen = false;
                    if (k.tekenen == false)
                    {
                        //s.Schets.vorige_elementen_list = s.Schets.elements_list; 
                        s.Schets.elements_list.RemoveAt(i);
                        
                    }
                    i = -1;

                }

            }
            foreach (Element elem in s.Schets.elements_list)
            {
                elem.teken(this, g, elem.p1, elem.p2);
            }
            s.Invalidate();


        
            
            //base.Compleet(s, g, p1, p2);
        }
        //

    }
    //nieuwe klasse
    public class Bring_forwardTool : TweepuntTool
    {
        public override string ToString() { return "Voorgrond"; }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            //g.DrawLine(MaakPen(Brushes.White, 7), p1, p2);
        }
        public override void Compleet(SchetsControl s, Graphics g, Point p1, Point p2)
        {
            EventArgs ea = new EventArgs();
            s.Schoon(this, ea);
            int tel = 0;
            for (int i = s.Schets.elements_list.Count - 1; i >= 0; i--)
            {
                Element k = s.Schets.elements_list[i];
                Rectangle test = new Rectangle();
                Element l;
                if (i  < s.Schets.elements_list.Count - 1)
                    l = s.Schets.elements_list[i + 1];
                else l = null;
                test = Punten2Rechthoek(k.p1, k.p2);
                if (test.Contains(p2))
                {
                    
                    if (tel > 0)
                    {
                        //s.Schets.vorige_elementen_list = s.Schets.elements_list;
                        s.Schets.elements_list.RemoveAt(i);
                        s.Schets.elements_list.Insert(i, l);
                        s.Schets.elements_list.RemoveAt(i+1);
                        s.Schets.elements_list.Insert(i+1, k);
                        
                        i = -1;
                    }
                    tel++;

                }

            }
            //ti.Insert(0, initialItem);
            foreach (Element elem in s.Schets.elements_list)
            {
                elem.teken(this, g, elem.p1, elem.p2);
            }
            s.Invalidate();




            //base.Compleet(s, g, p1, p2);
        }

    }



}
