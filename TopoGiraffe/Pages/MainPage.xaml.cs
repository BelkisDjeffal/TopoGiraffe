﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TopoGiraffe.Boites_de_dialogue;
using TopoGiraffe.Exceptions;
using TopoGiraffe.Noyau;
using TopoSurf.MessageBoxStyle;

namespace TopoGiraffe
{
    /// <summary>
    /// Logique d'interaction pour MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        //Liste de courbes representer en liste e liste de points (intersections detail)
        private readonly List<List<IntersectionDetail>> curves = new List<List<IntersectionDetail>>();


        // declaring variables

        private readonly List<Polyline> polylines = new List<Polyline>();
        private readonly List<CourbeNiveau> CourbesNiveau = new List<CourbeNiveau>();

        //PointCollection myPointCollection2 = new PointCollection();

        private CourbeNiveau courbeActuelle;
        private readonly List<Ellipse> cercles = new List<Ellipse>();
        private readonly PolyLineSegment polylinesegment = new PolyLineSegment();
        private bool btn2Clicked = false;
        private bool addLineClicked = false;
        private bool navClicked = false;
        private Polyline poly = new Polyline();
        private int LinePointscpt = 0;
#pragma warning disable CS0414 // Le champ 'MainPage.firstPoint' est assigné, mais sa valeur n'est jamais utilisée
        private Boolean firstPoint = true;
#pragma warning restore CS0414 // Le champ 'MainPage.firstPoint' est assigné, mais sa valeur n'est jamais utilisée
        private readonly List<int> Altitudes = new List<int>();
        private readonly List<List<ArtPoint>> PointsGlobal = new List<List<ArtPoint>>();
#pragma warning disable CS0414 // Le champ 'MainPage.NbCourbes' est assigné, mais sa valeur n'est jamais utilisée
        private readonly int NbCourbes = 0;
#pragma warning restore CS0414 // Le champ 'MainPage.NbCourbes' est assigné, mais sa valeur n'est jamais utilisée
        private Plan plan;
#pragma warning disable CS0169 // Le champ 'MainPage.scaleLine' n'est jamais utilisé
        private readonly Line scaleLine;
#pragma warning restore CS0169 // Le champ 'MainPage.scaleLine' n'est jamais utilisé
        private Boolean drawingScale = false;
        private Echelle mainScale;
        public static int exec = 0;



        public MainPage()
        {




            InitializeComponent();
            this.Title = "TopoGiraffe";
            this.DataContext = this;

            // code for first use guide

            //string text = File.ReadAllText("../../../assets/exec.txt");
            //if (text != String.Empty)
            //{
            //    exec = Convert.ToInt32(text);
            //}
            //if (exec == 0)
            //{
            //    exec++;

            //    File.WriteAllText("../../../assets/exec.txt", exec.ToString());


            //    HelpWindow();
            //}

            // ends here


            styleCourbeCmb.SelectedIndex = 0;


            //binding and adding data contexts









        }



        // code pour importer une carte 

        private void import_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "*.jpg,.png,.jpeg|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              " (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                imgPhoto.Source = new BitmapImage(new Uri(op.FileName));

            }


            imgPhoto.Opacity = .5;
            OpenInitialDialogBox();


        }











        private void border_KeyDown(object sender, KeyEventArgs e)
        {

        }


        // for a live preview of the line 

        private void mainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            double x = Mouse.GetPosition(mainCanvas).X;
            double y = Mouse.GetPosition(mainCanvas).Y;


            if (btn2Clicked == true && (courbeActuelle.polyline.Points.Count > 0))
            {



                Line newLine = new Line();



                newLine.Stroke = System.Windows.Media.Brushes.Black;
                newLine.X1 = courbeActuelle.polyline.Points[courbeActuelle.polyline.Points.Count - 1].X;
                newLine.Y1 = courbeActuelle.polyline.Points[courbeActuelle.polyline.Points.Count - 1].Y;

                newLine.X2 = x;
                newLine.Y2 = y;
                //newLine.HorizontalAlignment = HorizontalAlignment.Left;
                //newLine.VerticalAlignment = VerticalAlignment.Center;
                newLine.StrokeThickness = 3;
                mainCanvas.Children.Add(newLine);
                //Thread.Sleep(10);


                mainCanvas.Children.Remove(newLine);
                MouseMoveOnDraw(sender, e);

            }
            if (addLineClicked == true)
            {
                MouseMoveOnAddLine(sender, e);
            }
            if (navClicked == true)
            {






            }


        }

        // for the live preview of the curves -------------------------------------------

        private void MouseMoveOnDraw(object sender, MouseEventArgs e)
        {


            if (finalCtrlPoint == false && btn2Clicked == true)
            {

                if (courbeActuelle.polyline.Points.Count != 0) //to handle real-time drawing
                {
                    if (courbeActuelle.polyline.Points.Last().Equals(mousePos))
                    {
                        courbeActuelle.polyline.Points.RemoveAt(courbeActuelle.polyline.Points.Count - 1);
                    }

                    mousePos = new Point(e.GetPosition(this.mainCanvas).X, e.GetPosition(this.mainCanvas).Y);

                    courbeActuelle.polyline.Points.Add(mousePos);

                }
            }




        }



         //for the live preview of the Segment -------------------------------------------

        private void MouseMoveOnAddLine(object sender, MouseEventArgs e)
        {


            if (courbeActuelle.polyline.Points.Count != 0) //to handle real-time drawing
            {
                if (courbeActuelle.polyline.Points.Last().Equals(mousePos))
                {
                    courbeActuelle.polyline.Points.RemoveAt(courbeActuelle.polyline.Points.Count - 1);
                }

                mousePos = new Point(e.GetPosition(this.mainCanvas).X, e.GetPosition(this.mainCanvas).Y);
                //temporaryFigure = courbeActuelle;
                courbeActuelle.polyline.Points.Add(mousePos);
            }


        }
        List<IntersectionDetail> altit = new List<IntersectionDetail>();
        // button de dessin d'une Courbe de niveau ---------------------------------

        private void dessinerButton_Click(object sender, RoutedEventArgs e)
        {


            try
            {
                // gerer les booleens des autres buttons
                CourbeNiveau myCurve = DrawNewCurve();
                if (myCurve == null)
                {
                    return;
                }

                btn2Clicked = true;
                dragbool = false;
                navClicked = false;
                Cursor = Cursors.Arrow;


                CourbesNiveau.Add(myCurve);



                // styling


                courbeActuelle = myCurve;
                altit.Add(new IntersectionDetail(Convert.ToInt32(courbeActuelle.altitude) , false));

                AltSlider.Value = Convert.ToInt32(AltitudeString);

                courbeActuelle.polyline.Stroke = new SolidColorBrush(AltitudeToColor(Convert.ToInt32(AltitudeString)));
                myCurve.polyline.MouseMove += new MouseEventHandler(Path_MouseMove);
                myCurve.polyline.MouseLeftButtonUp += new MouseButtonEventHandler(Path_MouseLeftButtonUp);
                myCurve.polyline.MouseLeftButtonDown += new MouseButtonEventHandler(Path_MouseLeftButtonDown);

                mainCanvas.Children.Add(courbeActuelle.polyline);
                PointsGlobal.Add(new List<ArtPoint>());
                currentCurveCtrlPts = PointsGlobal[PointsGlobal.Count - 1];
                indexPoints++;
                finalCtrlPoint = false;
                // handling ctrl points
                RemoveCtrlPoints();
                ShownCtrlPoint = null;
            }
            catch (ErreurDeSaisieException)
            {

            }
            catch (ErreurAltitudeExcpetion)
            {

            }



            // (courbeActuelle).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(Polyline_MouseDown);



        }




        private void mainCanvas_MouseEnter(object sender, MouseEventArgs e)
        {

        }




        // to eliminate placeholders




        // button pour supprimer tout ---------------------------------------------------

        private void deleteAllButton_Click(object sender, RoutedEventArgs e)
        {
            // delteting the segment
            if (mainCanvas.Children.Count == 0)
            {
                MessageBox.Show("no polygone to delete");
            }
            else
            {
                // deleteting the curves
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Vous êtes sûr ?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {

                    CourbesNiveau.Clear();
                    mainCanvas.Children.Clear();

                    foreach (List<ArtPoint> ae in PointsGlobal)
                    {
                        ae.Clear();
                    }
                    PointsGlobal.Clear();
                    cercles.Clear();
                    IntersectionPoints.Clear();
                    PenteIntersectionPoints.Clear();
                    itm2.Clear();

                    indexPoints = -1;

                    nav.IsEnabled = true;
                    dessinerButton.IsEnabled = true;
                    add_line.IsEnabled = true;
                    dessinerPoint.IsEnabled = true;

                }

            }
        }
        public void prepareCanvas_Click(object sender, RoutedEventArgs e)
        {

            CourbesNiveau.Clear();
            mainCanvas.Children.Clear();
            foreach (List<ArtPoint> ae in PointsGlobal)
            {
                ae.Clear();
            }
            PointsGlobal.Clear();
            cercles.Clear();
            IntersectionPoints.Clear();
            PenteIntersectionPoints.Clear();
            indexPoints = -1;

            nav.IsEnabled = true;
            dessinerButton.IsEnabled = true;
            add_line.IsEnabled = true;
            dessinerPoint.IsEnabled = true;



        }

        // supprimer la derniere action-------------------------------------------------
#pragma warning disable CS0414 // Le champ 'MainPage.serializebool' est assigné, mais sa valeur n'est jamais utilisée
        private readonly bool serializebool = false;
#pragma warning restore CS0414 // Le champ 'MainPage.serializebool' est assigné, mais sa valeur n'est jamais utilisée


        private void deletePreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (mainCanvas.Children.Count == 0)
            {
                MessageBox.Show("no polygone to delete");
                return;
            }

            // suppression du segment
            navClicked = false;

            if (dessinerButton.IsEnabled == false)
            {
                mainCanvas.Children.Remove(CourbesNiveau[CourbesNiveau.Count - 1].polyline);
                CourbesNiveau.RemoveAt(CourbesNiveau.Count - 1);
                foreach (Ellipse el in cercles)
                {
                    mainCanvas.Children.Remove(el);
                }
                courbeActuelle = CourbesNiveau[CourbesNiveau.Count - 1];
                cercles.Clear();
                IntersectionPoints.Clear();
                nav.IsEnabled = true;
                dessinerButton.IsEnabled = true;
                add_line.IsEnabled = true;
                dessinerPoint.IsEnabled = true;


                return;
            }

            int index2 = CourbesNiveau.IndexOf(courbeActuelle);
            // gestion des points de ctrl
            try
            {
                if (ShownCtrlPoint != PointsGlobal[index2])
                {
                    RemoveCtrlPoints();
                    ShownCtrlPoint = PointsGlobal[index2];
                    DrawCtrlPoints(courbeActuelle);

                }
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("pas de courbe a supprimer");
            }

            if (mainCanvas.Children.Count == 0)
            {
                MessageBox.Show("no polygone to delete");
            }
            else
            {
                int index;
                List<ArtPoint> list;
                bool drawback = finalCtrlPoint;
                finalCtrlPoint = false;


                if (courbeActuelle.polyline.Points.Count > 0)
                {


                    if (courbeActuelle.polyline.Points[courbeActuelle.polyline.Points.Count - 1].Equals(courbeActuelle.polyline.Points[0]))
                    {

                        courbeActuelle.polyline.Points.RemoveAt(courbeActuelle.polyline.Points.Count - 1);

                    }
                    else
                    {


                        // removing circles
                        index = CourbesNiveau.IndexOf(courbeActuelle);
                        list = PointsGlobal[index];


                        if (courbeActuelle.polyline.Points.Count == 2 && list.Count == 2)
                        {
                            mainCanvas.Children.Remove(list[0].cercle);
                            mainCanvas.Children.Remove(list[1].cercle);
                            courbeActuelle.polyline.Points.Clear();

                            list.Clear();
                        }
                        else if (courbeActuelle.polyline.Points[courbeActuelle.polyline.Points.Count - 1].Equals(list[list.Count - 1].P))
                        {
                            mainCanvas.Children.Remove(list[list.Count - 1].cercle);
                            list.RemoveAt(list.Count - 1);
                            courbeActuelle.polyline.Points.RemoveAt(courbeActuelle.polyline.Points.Count - 1);
                        }



                    }

                }
                else
                {
                    index = CourbesNiveau.IndexOf(courbeActuelle);
                    list = PointsGlobal[index];
                    CourbesNiveau.Remove(courbeActuelle);
                    PointsGlobal.Remove(list);

                    if (CourbesNiveau.Count > 0)
                    {
                        courbeActuelle = CourbesNiveau[CourbesNiveau.Count - 1];

                    }
                    else
                    {
                        MessageBox.Show("no polygone to delete");
                    }

                }
            }
            nav.IsEnabled = true;
            dessinerButton.IsEnabled = true;
            btn2Clicked = true;
            dessinerPoint.IsEnabled = true;


        }






        // image visibilty with the display button
        private void display_Click(object sender, RoutedEventArgs e)
        {
            if (imgPhoto.Visibility == Visibility.Visible)
            {
                imgPhoto.Visibility = Visibility.Hidden;
            }
            else
            {
                imgPhoto.Visibility = Visibility.Visible;
            }
        }


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private bool Move = false;
        private int indexPoints = -1;

        // fonction de dessin principale -----------------------------------------------------------------------------------------

        private void mainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            double x = Mouse.GetPosition(mainCanvas).X;
            double y = Mouse.GetPosition(mainCanvas).Y;
            bool inter = false;

            object TestClicked = this.InputHitTest(e.GetPosition(this)) as FrameworkElement;//test the element we clicked on

            //  button de dessin cliqué 
            if (btn2Clicked == true)
            {
                firstPoint = true;
                Point lastPoint = new Point(x, y);

                // ajout des points d'articulation----------------------------------------------------------------------
                // fin de dessin de la courbe
                if (finalCtrlPoint == false)
                {


                    // gestion des erreurs de dessin

                    if (courbeActuelle.polyline.Points.Count > 2)
                    {
                        Line l = new Line();
                        l.X1 = courbeActuelle.polyline.Points[courbeActuelle.polyline.Points.Count - 2].X;
                        l.Y1 = courbeActuelle.polyline.Points[courbeActuelle.polyline.Points.Count - 2].Y;
                        l.X2 = lastPoint.X;
                        l.Y2 = lastPoint.Y;

                        inter = FindIntersection1(courbeActuelle.polyline, l);


                        if (inter == true)
                        {

                            new MssgBox("       Erreur de Dessin de la Courbe !\n veuillez dessiner votre segment à nouveau ").ShowDialog();

                        }

                    }
                    //------------------------------------------
                    // dessin de la courbe
                    if (inter == false)
                    {
                        courbeActuelle.polyline.Points.Add(lastPoint);
                        Ellipse circle = new Ellipse();
                        ArtPoint artPoint = new ArtPoint(circle, lastPoint);
                        indexPoints = CourbesNiveau.IndexOf(courbeActuelle);
                        ShownCtrlPoint = PointsGlobal[indexPoints];

                        PointsGlobal[indexPoints].Add(artPoint);
                        circle.Width = 10;
                        circle.Height = 10;
                        circle.Fill = Brushes.Purple;
                        circle.MouseMove += new System.Windows.Input.MouseEventHandler(Cercle_Mousemove);
                        circle.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(Ellipse_MouseLeftButtonUp);
                        circle.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(Ellipse_MouseLeftButtonDown);

                        Canvas.SetLeft(circle, lastPoint.X - (circle.Width / 2));
                        Canvas.SetTop(circle, lastPoint.Y - (circle.Height / 2));
                        mainCanvas.Children.Add(circle);
                        ShownCtrlPoint = PointsGlobal[indexPoints];

                    }

                }
                else
                // when click on the last (first) point 
                {
                    courbeActuelle.polyline.Points.RemoveAt(courbeActuelle.polyline.Points.Count - 1);
                    courbeActuelle.polyline.Points.Add(courbeActuelle.polyline.Points[0]);
                    btn2Clicked = false;
                    dragbool = true;
                    //PointsGlobal[indexPoints].Add(artPoint);

                }



            }
            // button de dessin de segment cliqué
            else if (addLineClicked == true)
            {
                LinePointscpt++;
                poly.Points.Add(new Point(x, y));



                // calcul des points d'intersection
                if (LinePointscpt == 2)
                {

                    line.X1 = poly.Points[0].X;
                    line.Y1 = poly.Points[0].Y;
                    line.X2 = poly.Points[1].X;
                    line.Y2 = poly.Points[1].Y;

                    foreach (CourbeNiveau courbe in CourbesNiveau)
                    {
                        FindIntersection(courbe, line);
                    }
                    // intersection des points altitude
                    if (pointsAltitude.Count > 0)
                    {
                        foreach (PointAltitude pointa in pointsAltitude)
                        {


                            FindIntersection(pointa, line);

                        }
                    }


                    distances();


                    curves.Add(IntersectionPoints);
                    curves.Add(Infos);
                    curves.Add(altit);

                    //    this.Serializee(curves);
                    List<Object> info = new List<Object>();

                    info.Add(Equidistance);
                    info.Add(AltitudeMax);
                    info.Add(AltitudeMin);
                    info.Add(mainScale);
                    //this.Serializee(info, "");




                    // dessin des cercles representant les points d'intersection
                    foreach (IntersectionDetail inters in IntersectionPoints)
                    {
                        Ellipse cercle = new Ellipse();
                        cercle.Width = 15;
                        cercle.Height = 15;
                        cercle.Fill = System.Windows.Media.Brushes.Red;
                        Canvas.SetLeft(cercle, inters.point.X - (cercle.Width / 2));
                        Canvas.SetTop(cercle, inters.point.Y - (cercle.Height / 2));
                        cercles.Add(cercle);
                        mainCanvas.Children.Add(cercle);
                    }

                    IntersectionPoints = IntersectionPoints.OrderBy(o => o.distance).ToList();

                    if (scaleLinePointsCount == 2)
                    {
                        MessageBox.Show(" Distance :" + Math.Round(mainScale.FindDistanceOnField(Outils.DistanceBtwTwoPoints(poly.Points[0], poly.Points[1])), 2) + " mètres");

                    }


                    addLineClicked = false;
                    double distance1 = Outils.DistanceBtwTwoPoints(new Point(line.X1, line.Y1), new Point(line.X2, line.Y2));// this can be optimized by using line.x, line.y
                    IntersectionPoints = IntersectionPoints.OrderBy(o => o.distance).ToList();
                    nav.IsEnabled = false;
                    dessinerButton.IsEnabled = false;
                    add_line.IsEnabled = false;
                    dessinerPoint.IsEnabled = false;
                    addLineClicked = false;




                }

            }
            // button d'echelle
            else if (drawingScale == true)

            {

                scaleLinePointsCount++;
                scalePolyline.Points.Add(new Point(x, y));

                if (scaleLinePointsCount == 2)
                {
                    mainScale.ScaleDistanceOnCanvas = Outils.DistanceBtwTwoPoints(scalePolyline.Points[0], scalePolyline.Points[1]);

                    string message = "Echelle sur plan " + Math.Round(mainScale.ScaleDistanceOnCanvas, 3) + "------>" + mainScale.ScaleDistanceOnField + " mètres";
                    MessageBox.Show(message);
                    mainCanvas.Children.Remove(scalePolyline);
                    drawingScale = false;
                }


            }
            // button de dessin de Points 
            else if (drawPointsClicked == true)
            {

                drawPointsClicked = false;


                if (pointAltitudeActuel != null)
                {
                    pointAltitudeActuel.point = new Point(x, y);
                    pointAltitudeActuel.DrawShape(mainCanvas);
                    pointsAltitude.Add(pointAltitudeActuel);

                }




            }
            //  button de selection
            else if (navClicked == true)
            {
                if (TestClicked is Polyline)
                {
                    foreach (CourbeNiveau courbe in CourbesNiveau)
                    {

                        if (courbe.polyline.Equals((Polyline)TestClicked))
                        {
                            courbeActuelle = courbe;
                        }
                    }


                }
                // verifier si la courbe clické n'est pas nulle
                if (TestClicked != null)
                {
                    if (TestClicked is Polyline || TestClicked is Ellipse)
                    {
                        int index = CourbesNiveau.IndexOf(courbeActuelle);
                        try
                        {
                            if (ShownCtrlPoint != PointsGlobal[index])
                            {
                                RemoveCtrlPoints();

                                ShownCtrlPoint = PointsGlobal[index];


                                //ShownCtrlPoint = PointsGlobal[index];
                                DrawCtrlPoints(courbeActuelle);


                            }
                        }
                        catch (Exception) { }
                        return;
                    }
                    else
                    {
                        RemoveCtrlPoints();
                        ShownCtrlPoint = null;
                    }


                }

            }
            else if (addTextClicked == true)
            {



                addTextClicked = false;

                texteBlockActuel.FontSize = 9;
                texteBlockActuel.Height = 10;
                texteBlockActuel.Width = 20;
                Canvas.SetLeft(texteBlockActuel, x - (texteBlockActuel.Width / 2));
                Canvas.SetTop(texteBlockActuel, y - (texteBlockActuel.Height / 2));
                mainCanvas.Children.Add(texteBlockActuel);



            }
        }

        private readonly List<object> Skew = new List<object>();
        private readonly List<PointAltitude> pointsAltitude = new List<PointAltitude>();
        private List<IntersectionDetail> IntersectionPoints = new List<IntersectionDetail>();

        // code d'intersection -------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------------------------


        private readonly Line line = new Line();
        // fonction qui retourne l'intersection entre une courbe et une ligne
        public void FindIntersection(CourbeNiveau p, Line line)
        {
            Line myLine = new Line();
            IntersectionDetail inter;

            for (int i = 0; i < p.polyline.Points.Count - 1; i++)
            {
                myLine.X1 = p.polyline.Points[i].X; myLine.Y1 = p.polyline.Points[i].Y;
                myLine.X2 = p.polyline.Points[i + 1].X; myLine.Y2 = p.polyline.Points[i + 1].Y;
                inter = intersectLines(myLine, line);

                inter.altitude = Convert.ToInt32(p.altitude);

                if (inter.intersect == true)
                {

                    IntersectionPoints.Add(inter);
                    PenteIntersectionPoints.Add(inter);

                }


            }


        }

        //surcharge de la foncton find intersection
        public void FindIntersection(PointAltitude p, Line line)
        {
            Line myLine = new Line();
            IntersectionDetail inter;

            for (int i = 0; i < p.triangle.Points.Count - 1; i++)
            {
                myLine.X1 = p.triangle.Points[i].X; myLine.Y1 = p.triangle.Points[i].Y;
                myLine.X2 = p.triangle.Points[i + 1].X; myLine.Y2 = p.triangle.Points[i + 1].Y;
                inter = intersectLines(myLine, line);

                inter.altitude = Convert.ToInt32(p.altitude);

                if (inter.intersect == true)
                {

                    IntersectionPoints.Add(inter);
                    PenteIntersectionPoints.Add(inter);

                }


            }


        }

        //une surcharge de la methode FindIntersection  
        public bool FindIntersection1(Polyline p, Line line)
        {
            Line myLine = new Line();
            IntersectionDetail inter;
            bool inters = false;

            for (int i = 0; i < p.Points.Count - 1; i++)
            {
                myLine.X1 = p.Points[i].X; myLine.Y1 = p.Points[i].Y;
                myLine.X2 = p.Points[i + 1].X; myLine.Y2 = p.Points[i + 1].Y;
                inter = intersectLines(myLine, line);

                if (inter.intersect == true && ((Math.Max(myLine.X1, myLine.X2) > inter.point.X) && (inter.point.X > Math.Min(myLine.X1, myLine.X2))
                && (Math.Max(myLine.Y1, myLine.Y2) > inter.point.Y) && (inter.point.Y > Math.Min(myLine.Y1, myLine.Y2))
                && (Math.Max(line.X1, line.X2) > inter.point.X) && (inter.point.X > Math.Min(line.X1, line.X2))
                && (Math.Max(line.Y1, line.Y2) > inter.point.Y) && (inter.point.Y > Math.Min(line.Y1, line.Y2))))
                {
                    inters = inter.intersect;
                }


            }
            return inters;

        }
        // intersection d'un segment avec une ligne 
        public IntersectionDetail intersectLines(Line line1, Line line2)
        {
            Equation equation1;
            Equation equation2;
            Boolean intersect = new Boolean();

            Point interscetionPoint = new Point();
            Point a1 = new Point(line1.X1, line1.Y1);
            Point b1 = new Point(line1.X2, line1.Y2);
            Point a2 = new Point(line2.X1, line2.Y1);
            Point b2 = new Point(line2.X2, line2.Y2);

            equation1 = GetSegmentEquation(a1, b1);
            equation2 = GetSegmentEquation(a2, b2);

            if (equation1.a == equation2.a)
            {
                intersect = false;
            }
            else
            {

                interscetionPoint.X = -(equation1.b - equation2.b) / (equation1.a - equation2.a);
                interscetionPoint.Y = (equation2.a * interscetionPoint.X) + equation2.b;

                if ((Math.Max(line1.X1, line1.X2) >= interscetionPoint.X) && (interscetionPoint.X >= Math.Min(line1.X1, line1.X2))
                && (Math.Max(line1.Y1, line1.Y2) >= interscetionPoint.Y) && (interscetionPoint.Y >= Math.Min(line1.Y1, line1.Y2))
                && (Math.Max(line2.X1, line2.X2) >= interscetionPoint.X) && (interscetionPoint.X >= Math.Min(line2.X1, line2.X2))
                && (Math.Max(line2.Y1, line2.Y2) >= interscetionPoint.Y) && (interscetionPoint.Y >= Math.Min(line2.Y1, line2.Y2)))
                {
                    intersect = true;
                }

            }

            return new IntersectionDetail(interscetionPoint, intersect);

        }

        //Calcule des distances
        private readonly List<double> distancesListe = new List<double>();

        public void distances()
        {

            foreach (IntersectionDetail intersectionDetail in IntersectionPoints)
            {
                double x1 = line.X1;//IntersectionPoints[1].point.X;
                double y1 = line.Y1;//IntersectionPoints[1].point.Y;

                double distance = Outils.DistanceBtwTwoPoints(new Point(x1, y1), intersectionDetail.point);// this can be optimized by using line.x, line.y


                //distancesListe.Add(distance);
                intersectionDetail.distance = distance;
            }
            //IntersectionPoints.Sort();
            IntersectionPoints = IntersectionPoints.OrderBy(o => o.distance).ToList();

        }


        //retourne une equation d''un segment a partir de deux points
        public Equation GetSegmentEquation(Point a, Point b)
        {
            Equation equation = new Equation();

            equation.a = (a.Y - b.Y) / (a.X - b.X);
            equation.b = -(equation.a) * (a.X) + a.Y;

            return equation;
        }

        //Boutton ajout d'une courbe
        private void add_line_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CourbesNiveau.Count == 0)
                {
                    throw new ErreurDeDessinDeSegment("Aucune courbe n'a été dessiné!");
                }
                poly = new Polyline();
                addLineClicked = true;
                add_line.IsEnabled = false;
                btn2Clicked = false;
                LinePointscpt = 0;
                poly.Stroke = Brushes.Indigo;
                poly.StrokeThickness = 5;
                poly.FillRule = FillRule.EvenOdd;
                CourbeNiveau polyC = new CourbeNiveau(poly, -1);
                CourbesNiveau.Add(polyC);
                courbeActuelle = polyC;
                mainCanvas.Children.Add(poly);
            }
            catch (ErreurDeDessinDeSegment)
            {

            }

        }




        //retourne une ligne avec style approprié
        public Polyline StyleCmbToRealStyle(Polyline pol, int index)
        {
            if (pol != null)
            {
                switch (index)
                {
                    case 0: //courbe simple
                        pol.StrokeThickness = 2;
                        pol.StrokeDashArray = null;

                        break;
                    case 1:// courbe intermediaire
                        pol.StrokeThickness = 2;
                        pol.StrokeDashArray.Add(1);
                        pol.StrokeDashArray.Add(3);
                        pol.StrokeDashArray.Add(1);
                        break;
                    case 2:// courbe maitresse
                        pol.StrokeThickness = 3.5;
                        pol.StrokeDashArray = null;

                        break;
                    default:
                        break;

                }

            }
            return pol;

        }




        private void mainCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                MessageBox.Show("cle retour clické");
            }
        }








        private readonly List<object> hitResultsList = new List<object>();


        //comportement du click droit sur le canvas
        private void mainCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {


            if (btn2Clicked == false)
            {
                return;
            }
            else
            {
                btn2Clicked = false;
                finalCtrlPoint = true;


                if (courbeActuelle.polyline != null && courbeActuelle.polyline.Points.Count > 0)
                {
                    courbeActuelle.polyline.Points.RemoveAt(courbeActuelle.polyline.Points.Count - 1);
                }
                else
                {
                    MessageBox.Show("vous n'avez pas de courbe");
                }



                if (CourbesNiveau.Contains(courbeActuelle) == false)
                {
                    CourbesNiveau.Add(courbeActuelle);
                }
                if (polylines.Count != 0)
                {
                    //int indexp = polylines.IndexOf(courbeActuelle);

                }
                RemoveCtrlPoints();
                ShownCtrlPoint = null;
                //curve is a list of points
                List<IntersectionDetail> curve = new List<IntersectionDetail>();
                for (int k = 0; k < courbeActuelle.polyline.Points.Count(); k++) { curve.Add(new IntersectionDetail(courbeActuelle.polyline.Points[k], false)); }
                curves.Add(curve);
            }
        }

        //boutton de navigation
        private void nav_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Arrow;
            navClicked = true;
            addLineClicked = false;
            btn2Clicked = false;
            finalCtrlPoint = false;
            // EditPolyline = courbeActuelle;


            //this.NavigationService.Refresh();
        }

        private int cptdebug = 0;
        private int index2 = 0;
        private FrameworkElement elDraggingEllipse;
        private Point ptMouseStart, ptElementStart;

        //------------------------------------------------------------------------------------------------------------------------------------------------
        // code to handle Modifying Control Points --------------------------------------------------------------------------------------------------------



        private CourbeNiveau Interpoly;
        private CourbeNiveau EditPolyline;
        private bool dragbool;
        //comportement du click droit sur le cercle
        public void Cercle_Mousemove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ArtPoint elDraggingEllip = new ArtPoint();
            if (((Ellipse)elDraggingEllipse) == null)
            {
                return;
            }

            if (e.LeftButton == MouseButtonState.Pressed && navClicked == true)
            {

                Point ptMouse = e.GetPosition(this.mainCanvas);
                //if (ptMouse.X > 0 && ptMouse.X < mainCanvas.Width && ptMouse.Y > 0 && ptMouse.Y < mainCanvas.Height)
                //{
                Move = true;
                if (Move == true)
                {

                    if (elDraggingEllipse == null)
                    {
                        elDraggingEllipse = ((Ellipse)elDraggingEllipse);
                    }
                    //change the position of the ellipse that represent the control point
                    Canvas.SetLeft(elDraggingEllipse, ptMouse.X - 10 / 2);
                    Canvas.SetTop(elDraggingEllipse, ptMouse.Y - 10 / 2);

                    int cpt3 = 0;
                    foreach (List<ArtPoint> ae in PointsGlobal)
                    {

                        foreach (ArtPoint a in ae)
                        {
                            if (a.cercle.Equals(elDraggingEllipse))
                            {
                                elDraggingEllip.cercle = a.cercle;
                                elDraggingEllip.P = a.P;

                                index2 = cpt3;
                            }
                        }

                        cpt3++;
                    }
                    EditPolyline = CourbesNiveau[index2];
                    //courbeActuelle = CourbesNiveau[index2]; 
                    //Polyline_Modify();


                    Interpoly = new CourbeNiveau(new Polyline(), courbeActuelle.altitude);
                    Interpoly.polyline.Stroke = new SolidColorBrush(AltitudeToColor(courbeActuelle.altitude));

                    StyleCmbToRealStyle(Interpoly.polyline, styleCourbeCmb.SelectedIndex);
                    Interpoly.polyline.StrokeThickness = ThickSlider.Value;
                    Interpoly.polyline.Points.Clear();
                    //Interpoly.FillRule = FillRule.EvenOdd;
                    //courbeActuelle = Interpoly;

                    //   mainCanvas.Children.Remove(EditPolyline);


                    int i = 0;
                    PointsGlobal[index2].Remove(elDraggingEllip);

                    foreach (Point point in EditPolyline.polyline.Points)
                    {
                        if (point.Equals(elDraggingEllip.P))
                        {
                            Interpoly.polyline.Points.Add(ptMouse);
                            elDraggingEllip = new ArtPoint((Ellipse)elDraggingEllipse, ptMouse);

                            PointsGlobal[index2].Add(elDraggingEllip);
                        }
                        else
                        {
                            if (dragbool == true && i == EditPolyline.polyline.Points.Count - 1)
                            {
                                Interpoly.polyline.Points.Add(EditPolyline.polyline.Points[0]);

                            }
                            else
                            {
                                Interpoly.polyline.Points.Add(EditPolyline.polyline.Points[i]);
                            }
                        }
                        i++;
                    }



                    //mainCanvas.Children.Remove(courbeActuelle);
                    mainCanvas.Children.Add(Interpoly.polyline);
                    mainCanvas.Children.Remove(EditPolyline.polyline);

                    EditPolyline = Interpoly;
                    CourbesNiveau[index2] = Interpoly;
                    courbeActuelle = Interpoly;







                }
                //}

            }
        }

        private void Ellipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (((Ellipse)elDraggingEllipse) == null)
            {
                return;
            }

            //if (((Ellipse)elDraggingEllipse) == null) return;
            //((Ellipse)elDraggingEllipse).Cursor = Cursors.Arrow; //change the cursor
            ((Ellipse)elDraggingEllipse).ReleaseMouseCapture();

            if (navClicked)
            {
                foreach (ArtPoint el in PointsGlobal[index2])
                {
                    mainCanvas.Children.Remove(el.cercle);

                    mainCanvas.Children.Add(el.cercle);

                }
            }

            cptdebug++;
            Ellipse elli = (Ellipse)elDraggingEllipse;
            elli.Fill = Brushes.Purple;

            ((Ellipse)elDraggingEllipse).Cursor = Cursors.Arrow;

        }

        private void Ellipse_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {


            elDraggingEllipse = (this).InputHitTest(e.GetPosition(this)) as FrameworkElement;
            if (elDraggingEllipse == null)
            {
                return;
            }

            if (elDraggingEllipse != null && elDraggingEllipse is Ellipse)
            {

                ((Ellipse)elDraggingEllipse).Cursor = Cursors.ScrollAll;

                Mouse.Capture((elDraggingEllipse));
            }
            if (btn2Clicked == true) // clicking on an ellipse while drawing
            {
                finalCtrlPoint = true;
                courbeActuelle.polyline.Points.RemoveAt(courbeActuelle.polyline.Points.Count - 1);
                courbeActuelle.polyline.Points.Add(courbeActuelle.polyline.Points[0]);
                RemoveCtrlPoints(courbeActuelle);
                ShownCtrlPoint = null;
                btn2Clicked = false;
                dragbool = true;
                btn2Clicked = false;
                //curve is a list of points
                List<IntersectionDetail> curve = new List<IntersectionDetail>();
                for (int k = 0; k < courbeActuelle.polyline.Points.Count(); k++) { curve.Add(new IntersectionDetail(courbeActuelle.polyline.Points[k], false)); }
                curves.Add(curve);



            }
            else if (navClicked == true)
            {
                int cpt4 = 0;
                foreach (List<ArtPoint> ae in PointsGlobal)
                {

                    foreach (ArtPoint a in ae)
                    {
                        if (a.cercle.Equals(elDraggingEllipse))
                        {
                            courbeActuelle = CourbesNiveau[cpt4];
                            ThickSlider.Value = courbeActuelle.polyline.StrokeThickness;
                        }
                    }

                    cpt4++;
                }
            }
            Ellipse elli = (Ellipse)elDraggingEllipse;
            elli.Fill = Brushes.Orange;
            AltSlider.Value = courbeActuelle.altitude;



        }

        private bool finalCtrlPoint = false;
        private List<ArtPoint> currentCurveCtrlPts;
        private Point mousePos;


        //------------------------------------------------------------------------------------------------------------------------------------------------
        // code to handle dragging of the poyline --------------------------------------------------------------------------------------------------------
        private bool isDragging, mvCtrl = true;
        private FrameworkElement elDragging;
#pragma warning disable CS0169 // Le champ 'MainPage.selectedPath' n'est jamais utilisé
        private readonly FrameworkElement selectedPath;
#pragma warning restore CS0169 // Le champ 'MainPage.selectedPath' n'est jamais utilisé
        private FrameworkElement selectedPolyline;
        private FrameworkElement selectedTriangle;
#pragma warning disable CS0169 // Le champ 'MainPage.minX' n'est jamais utilisé
#pragma warning disable CS0169 // Le champ 'MainPage.maxY' n'est jamais utilisé
#pragma warning disable CS0169 // Le champ 'MainPage.maxX' n'est jamais utilisé
#pragma warning disable CS0169 // Le champ 'MainPage.minY' n'est jamais utilisé
        private readonly double minX, minY, maxX, maxY;
#pragma warning restore CS0169 // Le champ 'MainPage.minY' n'est jamais utilisé
#pragma warning restore CS0169 // Le champ 'MainPage.maxX' n'est jamais utilisé
#pragma warning restore CS0169 // Le champ 'MainPage.maxY' n'est jamais utilisé
#pragma warning restore CS0169 // Le champ 'MainPage.minX' n'est jamais utilisé
#pragma warning disable CS0414 // Le champ 'MainPage.indexdrag' est assigné, mais sa valeur n'est jamais utilisée
        private readonly int indexdrag = 0;
#pragma warning restore CS0414 // Le champ 'MainPage.indexdrag' est assigné, mais sa valeur n'est jamais utilisée

        //comportement du click sur calcul de pente
        private void btn13_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IntersectionPoints.Count == 0)
                {
                    throw new ErreurDeDessinDeSegment("Aucun point d'intersection n'est trouvé!");
                }

                if (mainScale != null)
                {
                    pente = CalcPente(PenteIntersectionPoints, mainScale);
                    pente = System.Math.Round(pente, 3) * 100;
                    String penteText = " la pente est de   :" + pente.ToString() + " % ";
                    ProfileTopographique profile = new ProfileTopographique(IntersectionPoints, distancesListe, mainScale, penteText);
                    profile.Show();

                }
                else
                {
                    MessageBox.Show("Echelle non connue !");
                }
            }
            catch (ErreurDeDessinDeSegment)
            {

            }
        }

#pragma warning disable CS0649 // Le champ 'MainPage.DragPoints' n'est jamais assigné et aura toujours sa valeur par défaut null
        private readonly List<ArtPoint> DragPoints;
#pragma warning restore CS0649 // Le champ 'MainPage.DragPoints' n'est jamais assigné et aura toujours sa valeur par défaut null
        private Thickness margin;

        //comportement du click souris sur le canvas
        private void Path_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedPolyline = (this).InputHitTest(e.GetPosition(this)) as FrameworkElement;
            if (selectedPolyline == null)
            {
                return;
            }

            if (selectedPolyline != null && selectedPolyline is Polyline)
            {
                foreach (CourbeNiveau courbe in CourbesNiveau)
                {

                    if (courbe.polyline.Equals((Polyline)selectedPolyline))
                    {
                        courbeActuelle = courbe;

                    }


                }


                if (navClicked == true)
                {


                    AltSlider.Value = courbeActuelle.altitude;
                    ThickSlider.Value = courbeActuelle.polyline.StrokeThickness;
                    mvCtrl = true;
                    ptMouseStart = e.GetPosition(this);
                    elDragging = (this).InputHitTest(ptMouseStart) as FrameworkElement;
                    if (elDragging == null)
                    {
                        return;
                    }

                    if (elDragging != null && elDragging is Polyline)
                    {
                        ptElementStart = new Point(elDragging.Margin.Left, elDragging.Margin.Top);
                        margin = new Thickness(elDragging.Margin.Left, elDragging.Margin.Top, 0, 0);
                        elDragging.Cursor = Cursors.ScrollAll;
                        Mouse.Capture((elDragging));
                        isDragging = true;

                    }
                    int index = CourbesNiveau.IndexOf(courbeActuelle);

                    if (ShownCtrlPoint != PointsGlobal[index])
                    {
                        RemoveCtrlPoints();
                        //ShownCtrlPoint = PointsGlobal[index];

                        ShownCtrlPoint = PointsGlobal[index];

                        DrawCtrlPoints(courbeActuelle);



                    }

                }
                else
                {
                    return;
                }
            }
        }

#pragma warning disable CS0414 // Le champ 'MainPage.test' est assigné, mais sa valeur n'est jamais utilisée
        private bool test = false;
#pragma warning restore CS0414 // Le champ 'MainPage.test' est assigné, mais sa valeur n'est jamais utilisée


        //comportement du click gauche sur le canvas (Different modifications) 
        private void Path_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            Point pnt = e.GetPosition(this);
            if (elDragging == null)
            {
                return;
            }

            if (isDragging)
            {
                if (!mvCtrl)
                {
                    elDragging.Margin = margin;

                    foreach (ArtPoint ell in DragPoints)
                    {
                        ell.cercle.Margin = margin;
                    }

                }




            }

            isDragging = false;
            (elDragging).Cursor = Cursors.Arrow;
            (elDragging).ReleaseMouseCapture();
            elDragging = null;
            test = true;
        }

        private SolidColorBrush chosenBrush;
        private long colorVal;

        //colorisation d'une courbe
        private void cp_SelectedColorChanged_1(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (cp.SelectedColor.HasValue)
            {
                Color C = cp.SelectedColor.Value;
                byte Red = C.R;
                byte Green = C.G;
                byte Blue = C.B;
                colorVal = Convert.ToInt64(Blue * (Math.Pow(256, 0)) + Green * (Math.Pow(256, 1)) + Red * (Math.Pow(256, 2)));
                chosenBrush = new SolidColorBrush(Color.FromRgb(Red, Green, Blue));
                // update courbe actuelle 
                if (courbeActuelle != null)
                {
                    courbeActuelle.polyline.Stroke = chosenBrush;
                }
                else
                {
                    MessageBox.Show("Créer votre courbe d'abord");
                }

            }

        }


        private void styleCourbeCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (courbeActuelle != null)
            {

                courbeActuelle.polyline = StyleCmbToRealStyle(courbeActuelle.polyline, styleCourbeCmb.SelectedIndex);
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        //comportement du glissement de souris
        private void Path_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if ((elDragging) == null)
            {
                return;
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //Point ptMouse = e.GetPosition(this);
                //if (isDragging)
                //{

                //    if (elDragging == null)

                //        elDragging = (elDragging);

                //    double left = ptElementStart.X + ptMouse.X - ptMouseStart.X;
                //    double top = ptElementStart.Y + ptMouse.Y - ptMouseStart.Y;
                //    int index = CourbesNiveau.IndexOf(courbeActuelle);

                //    foreach (ArtPoint ell in PointsGlobal[index])
                //    {

                //        //ell.cercle.Margin = new Thickness(left, top, 0, 0);// modify the margin to move the curve
                //        Canvas.SetLeft(ell.cercle, (ell.P.X + left) - (ell.cercle.Width / 2));
                //        Canvas.SetTop(ell.cercle, (ell.P.Y + top ) -  (ell.cercle.Height / 2));

                //    }

                //    elDragging.Margin = new Thickness(left, top, 0, 0);// modify the margin to move the curve
                //    if (mvCtrl)
                //    {
                //        margin = elDragging.Margin;
                //    }


                //}
            }
        }

        private List<ArtPoint> ShownCtrlPoint;

        //dessin des points d'articulation d'une courbe de niveau

        public void DrawCtrlPoints(CourbeNiveau polyline)
        {
            if (polyline == null)
            {
                return;
            }

            if (ShownCtrlPoint == null)
            {
                return;
            }

            int index = CourbesNiveau.IndexOf(polyline);
            //ShownCtrlPoint = PointsGlobal[index];
            for (int i = 0; i < ShownCtrlPoint.Count; i++)
            {
                Ellipse circle = new Ellipse();
                ArtPoint artPoint = new ArtPoint(circle, ShownCtrlPoint[i].P);
                ShownCtrlPoint[i] = artPoint;
                circle.Width = 10;
                circle.Height = 10;
                circle.Fill = Brushes.Purple;
                circle.MouseMove += new MouseEventHandler(Cercle_Mousemove);
                circle.MouseLeftButtonUp += new MouseButtonEventHandler(Ellipse_MouseLeftButtonUp);
                circle.MouseLeftButtonDown += new MouseButtonEventHandler(Ellipse_MouseLeftButtonDown);

                Canvas.SetLeft(circle, artPoint.P.X - (circle.Width / 2));
                Canvas.SetTop(circle, artPoint.P.Y - (circle.Height / 2));
                mainCanvas.Children.Add(circle);

            }
        }

        //cacher les points d'articulation 
        public void RemoveCtrlPoints(CourbeNiveau polyline)
        {
            if (polyline == null)
            {
                return;
            }

            int index = CourbesNiveau.IndexOf(polyline);
            try
            {
                foreach (ArtPoint Ctrl in PointsGlobal[index])
                {

                    mainCanvas.Children.Remove(Ctrl.cercle);



                }
            }
            catch
            {
                MessageBox.Show("Erreur de dessin");
            }




        }

        //effacememnt des points d'articulation (surcharge) 
        public void RemoveCtrlPoints()
        {
            if (ShownCtrlPoint == null)
            {
                return;
            }

            foreach (ArtPoint Ctrl in ShownCtrlPoint)
            {

                mainCanvas.Children.Remove(Ctrl.cercle);


            }


        }

        private int indexAltitude = 0;
        private int Equidistance;
        private static float AltitudeMin;
        private static float AltitudeMax;
        private readonly List<IntersectionDetail> Infos = new List<IntersectionDetail>();
        public void UpdateTextBoxes(int equidistance, int altitudeMax, int altitudeMin, double echelleCanv, double echelleField)
        {


            // we gonan create a new plann here 
            plan = new Plan() { Equidistance = equidistance, MaxAltitude = altitudeMax, MinAltitude = altitudeMin };
            AltitudeMin = altitudeMin;
            AltitudeMax = altitudeMax;
            mainScale = new Echelle() { ScaleDistanceOnCanvas = echelleCanv, ScaleDistanceOnField = echelleField };
            equidistancePlan.DataContext = plan;
            altitudeMaxPlan.DataContext = plan;
            altMin.DataContext = plan;
            echelleOnFieldPlan.DataContext = mainScale;
            echelleOnCanvasPlan.DataContext = mainScale;
            AltSlider.Minimum = altitudeMin;
            AltSlider.Maximum = altitudeMax;
            AltSlider.SmallChange = equidistance;
            AltSlider.LargeChange = 2 * equidistance;
            AltSlider.TickFrequency = equidistance;
            Equidistance = equidistance;
        }

        public void OpenInitialDialogBox()
        {
            DataDialog dataDialog = new DataDialog();
            //dataDialog.Owner = this;
            dataDialog.ShowDialog();



            if (dataDialog.DialogResult == true)
            {

                try
                {
                    AltSlider.Minimum = Convert.ToInt32(dataDialog.MinTextBox.Text);
                    AltitudeMin = Convert.ToInt32(dataDialog.MinTextBox.Text);
                    AltitudeMax = Convert.ToInt32(dataDialog.MaxTextBox.Text);
                    AltSlider.Maximum = Convert.ToInt32(dataDialog.MaxTextBox.Text);
                    AltSlider.SmallChange =
                    ThickSlider.Value = 2;
                    Equidistance = Convert.ToInt32(dataDialog.EquidistanceTextBox.Text);
                    AltSlider.SmallChange = Equidistance;
                    AltSlider.LargeChange = 2 * Equidistance;
                    AltSlider.TickFrequency = Equidistance;

                    // serialization

                    Infos.Add(new IntersectionDetail(Equidistance, false));
                    Infos.Add(new IntersectionDetail(Convert.ToInt32(AltitudeMax), false));
                    Infos.Add(new IntersectionDetail(Convert.ToInt32(AltitudeMin), false));




                    if (AltitudeMin > AltitudeMax) { throw new ErreurDeSaisieException("Altitude min ne doit pas etre superieur a l'altitude max"); }
                }
                catch (Exception)
                {
                    dataDialog = new DataDialog();
                    //dataDialog.Owner = this;
                    dataDialog.ShowDialog();

                    AltSlider.Minimum = Convert.ToInt32(dataDialog.MinTextBox.Text);
                    AltitudeMin = Convert.ToInt32(dataDialog.MinTextBox.Text);
                    AltitudeMax = Convert.ToInt32(dataDialog.MaxTextBox.Text);
                    AltSlider.Maximum = Convert.ToInt32(dataDialog.MaxTextBox.Text);
                    ThickSlider.Value = 2;
                    Equidistance = Convert.ToInt32(dataDialog.EquidistanceTextBox.Text);


                }

                if (int.TryParse(dataDialog.EchelleOnCanvas, out int result1) && int.TryParse(dataDialog.EchelleOnField, out int result2))
                {
                    mainScale = new Echelle() { ScaleDistanceOnCanvas = result1, ScaleDistanceOnField = result2 };
                    if (mainScale != null)
                    {

                        Infos.Add(new IntersectionDetail(result1, false));
                        Infos.Add(new IntersectionDetail(result2, false));

                    }

                    plan = new Plan() { Equidistance = Convert.ToInt32(dataDialog.Equidistance), MaxAltitude = Convert.ToInt32(dataDialog.Max), MinAltitude = Convert.ToInt32(dataDialog.Min) };

                    dataDialog.EquidistanceTextBox.DataContext = plan;
                    dataDialog.MaxTextBox.DataContext = plan;
                    dataDialog.MinTextBox.DataContext = plan;
                    equidistancePlan.DataContext = plan;
                    altitudeMaxPlan.DataContext = plan;
                    altMin.DataContext = plan;


                    echelleOnCanvasPlan.DataContext = mainScale;
                    echelleOnFieldPlan.DataContext = mainScale;


                }
                else
                {
                    MessageBox.Show("Attention !\n Echelle non saisie une valeur par defaut est prise en compte, veuillez la saisir avec l'outil adequat situe sur la barre a gauche");

                }







            }

        }

        private String AltitudeString;


        //dessin d'une nouvelle courbe
        public CourbeNiveau DrawNewCurve()

        {
            CourbeNiveau Courbe = null;

            Window1 window1 = new Window1();

            window1.ShowDialog();
            // gerer les exceptions d'rreur d'altitude de courbe (selon l'equidistance)


            if (window1.DialogResult == true)
            {
                // taking the altitude from the dialog box

                if (int.TryParse(window1.Altitude.Text, out int result))
                {



                    if (CourbesNiveau.Count > 0)
                    {

                        if ((Math.Abs((result - CourbesNiveau[0].altitude)) % Equidistance) != 0)
                        {
                            throw new ErreurAltitudeExcpetion("L'altitude entrée ne correspend pas\n à l'equidistance");
                        }

                    }

                    Altitudes.Add(result);
                    indexAltitude++;

                    AltitudeString = window1.Altitude.Text;

                    // StyleCmbToRealStyle(courbeActuelle,Convert.ToInt32(Window1.Type.SelectedIndex));
                    Courbe = new CourbeNiveau(new Polyline(), result);

                    Courbe.polyline.StrokeThickness = 2;
                    Courbe.polyline.FillRule = FillRule.EvenOdd;

                    Courbe.polyline = StyleCmbToRealStyle(Courbe.polyline, window1.styleCourbeCmbInDialogBox.SelectedIndex); // styling it 


                }
                else
                {
                    MessageBox.Show("Erreur! \n votre altitude n'est pas un entier");
                }

            }



            return Courbe;

        }

        private Polyline scalePolyline;
        private int scaleLinePointsCount = 0;
        //comportement du boutton d'echelle
        private void scaleButton_Click(object sender, RoutedEventArgs e)
        {

            ScaleDialog scaleDialog = new ScaleDialog();
            scaleDialog.EchelleTextBoxOnCanvasScaleDB.DataContext = mainScale;
            scaleDialog.EchelleTextBoxOnFieldScaleDB.DataContext = mainScale;

            scaleDialog.ShowDialog();

            if (scaleDialog.DialogResult == true)
            {
                if ((bool)scaleDialog.ScaleDrawingRadioBtn.IsChecked)
                {
                    if (int.TryParse(scaleDialog.EchelleOnField, out int result))
                    {
                        // assign the first proprety of the scale scaleonField
                        if (mainScale == null)
                        {
                            mainScale = new Echelle() { ScaleDistanceOnField = result, ScaleDistanceOnCanvas = 0 };
                        }
                        else
                        {
                            mainScale.ScaleDistanceOnField = result;
                        }



                        mainScale = new Echelle(result);

                        drawingScale = true;
                        btn2Clicked = false;
                        addLineClicked = false;
                        navClicked = false;

                        // making the polyline
                        scalePolyline = new Polyline();

                        scalePolyline.Stroke = Brushes.Indigo;
                        scalePolyline.StrokeThickness = 3;
                        scalePolyline.FillRule = FillRule.EvenOdd;

                        mainCanvas.Children.Add(scalePolyline);
                    }
                    else
                    {
                        (new MssgBox("Entrée non valide , Vérifier l'échelle ne mètre !")).ShowDialog();
                    }

                }
                else
                {
                    if (int.TryParse(scaleDialog.EchelleOnCanvas, out int result1) && (int.TryParse(scaleDialog.EchelleOnField, out int result2)))
                    {
                        mainScale = new Echelle(result1, result2);

                    }
                    else
                    {
                        (new MssgBox("Entrée non valide , Vérifier !")).ShowDialog();

                    }


                }
                // else should be here 

            }







        }

        //-------------------------------glissier de changement d'altitude----------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------
        private void altitudeSliderValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {//change altitude
            changeSelectedCurveAltitude((float)AltSlider.Value);
        }

        //click ajout dans la glissiere d'altitude

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            if (courbeActuelle == null)
            {
                return;
            }
            AltSlider.Value += Equidistance;
            changeSelectedCurveAltitude((float)AltSlider.Value);

        }

        //click signe moins dans la glissiere d'altitude

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {

            if (courbeActuelle == null)
            {
                return;
            }
            AltSlider.Value -= Equidistance;
            changeSelectedCurveAltitude((float)AltSlider.Value);
        }

        //changement d'altitude pour une courbe selectionee

        private void changeSelectedCurveAltitude(float altit)
        {//the real change
            if (courbeActuelle == null)
            {
                return;
            }


            (courbeActuelle).polyline.Stroke = new SolidColorBrush(AltitudeToColor(altit));
            courbeActuelle.altitude = altit;

        }
        private void keyDownOnAltitude(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                try
                {
                    float altit = int.Parse(AltitudeBox.Text);
                    changeSelectedCurveAltitude(altit);
                }
                catch (System.FormatException)
                {
                    (new MssgBox("Donner une valeur numérique dans le champ Altitude")).ShowDialog();
                }
                catch (System.NullReferenceException)
                {
                }
            }
        }


        //Conversion d'une altitudee a une couleur donnee pour l'associer a une courbe 
        public static Color AltitudeToColor(float altit)
        {//implemeting the predefined altitude levels

            float range = (AltitudeMax - AltitudeMin) / 10;
            range = Convert.ToInt32(range);
            Color color = (Color)ColorConverter.ConvertFromString("#FFDFD991");

            if (altit < 0)
            {
                return Colors.Black;
            }

            if (altit <= AltitudeMin + range)
            {
                return (Color)ColorConverter.ConvertFromString("#6600CC");
            }

            if (altit <= AltitudeMin + 2 * range)
            {
                return (Color)ColorConverter.ConvertFromString("#0000CC");
            }

            if (altit <= AltitudeMin + 3 * range)
            {
                return (Color)ColorConverter.ConvertFromString("#0066CC");
            }

            if (altit <= AltitudeMin + 4 * range)
            {
                return (Color)ColorConverter.ConvertFromString("#00CCCC");
            }

            if (altit <= AltitudeMin + 5 * range)
            {
                return (Color)ColorConverter.ConvertFromString("#00CC66");
            }

            if (altit <= AltitudeMin + 6 * range)
            {
                return (Color)ColorConverter.ConvertFromString("#00CC00");
            }

            if (altit <= AltitudeMin + 7 * range)
            {
                return (Color)ColorConverter.ConvertFromString("#66CC00");
            }

            if (altit <= AltitudeMin + 8 * range)
            {
                return (Color)ColorConverter.ConvertFromString("#FFFF00");
            }

            if (altit <= AltitudeMin + 9 * range)
            {
                return (Color)ColorConverter.ConvertFromString("#FF8000");
            }

            if (altit <= AltitudeMin + 10 * range)
            {
                return (Color)ColorConverter.ConvertFromString("#FF0000");
            }

            return Colors.Black;




        }

        /* --------------------------------------------------------------------------- aide contextuel ------------------------------------------------*/
        /* -------------------------------------------------------------------------------------------------------------------------------------------*/



        //retardement de 5 secondes

        private async Task PutTaskDelay()
        {
            await Task.Delay(5000);
        }

        // code pour l'aide

        private async void Help_Click(object sender, RoutedEventArgs e)
        {

            popup_ud.IsOpen = true;
            await PutTaskDelay();
            popup_ud.IsOpen = false;
            popup_ue.IsOpen = true;
            popup_uh.PlacementTarget = import;
            popup_uh.Placement = PlacementMode.Bottom;
            Pops.PopupText.Text = "Importer une Carte";

            popup_uh.IsOpen = true;
            await PutTaskDelay();
            popup_uh.Visibility = Visibility.Collapsed;

            popup_ue.IsOpen = false;
            popup_uf.IsOpen = true;
            // button guide

            popup_uh.PlacementTarget = dessinerButton;
            popup_uh.Placement = PlacementMode.Right;
            Pops.PopupText.Text = "Dessiner une courbe";
            await PutTaskDelay();
            popup_uh.Visibility = Visibility.Collapsed;

            popup_uf.IsOpen = false;
            popup_ug.IsOpen = true;

            popup_uh.PlacementTarget = btn13;
            popup_uh.Placement = PlacementMode.Bottom;
            Pops.PopupText.Text = "Générer le Profil topographique";
            await PutTaskDelay();
            popup_ug.IsOpen = false;
            popup_uh.Visibility = Visibility.Collapsed;
            popup_uh.IsOpen = false;



        }
        public async void HelpWindow()
        {
            popup_ud.IsOpen = true;
            await PutTaskDelay();
            popup_ud.IsOpen = false;
            popup_ue.IsOpen = true;
            popup_uh.PlacementTarget = import;
            popup_uh.Placement = PlacementMode.Bottom;
            Pops.PopupText.Text = "Importer une Carte";

            popup_uh.IsOpen = true;
            await PutTaskDelay();
            popup_uh.Visibility = Visibility.Collapsed;

            popup_ue.IsOpen = false;
            popup_uf.IsOpen = true;
            // button guide

            popup_uh.PlacementTarget = dessinerButton;
            popup_uh.Placement = PlacementMode.Bottom;
            Pops.PopupText.Text = "Dessiner une courbe";
            await PutTaskDelay();
            popup_uh.Visibility = Visibility.Collapsed;

            popup_uf.IsOpen = false;
            popup_ug.IsOpen = true;

            popup_uh.PlacementTarget = btn13;
            popup_uh.Placement = PlacementMode.Bottom;
            Pops.PopupText.Text = "Générr le Profil topographique";
            await PutTaskDelay();
            popup_ug.IsOpen = false;
            popup_uh.Visibility = Visibility.Collapsed;
            popup_uh.IsOpen = false;

        }

        // --------------------------------------------------------------------------- popup fin -------------------------------------//
        // --------------------------------------------------------------------------------------------------------------------------//

        private void ThickSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {//change thickness
            if (courbeActuelle != null)
            {
                (courbeActuelle).polyline.StrokeThickness = ThickSlider.Value;
                ThickText.Text = ThickSlider.Value.ToString();
            }
        }


        //-------navigation vers la page de sauvegarde----
        //------------------------------------------------
        public void ShowSauvgardeWindow_Click(object sender, RoutedEventArgs e)
        {

            this.NavigationService.Navigate(new SauvgardePage(curves, this));

        }

        private List<IntersectionDetail> alts = new List<IntersectionDetail>();
        private List<List<IntersectionDetail>> itm2 = new List<List<IntersectionDetail>>();

        public void imprt_Click()
        {
            List<Polyline> curve = polylines;


            List<IntersectionDetail> alts = new List<IntersectionDetail>();




            itm2 = this.DeSerialize();
            String penteText = " la pente est de   :" + pente.ToString() + " % ";

            try
            {
                PenteIntersectionPoints = itm2[itm2.Count() - 1];
                IntersectionPoints = PenteIntersectionPoints;
            }
            catch (Exception)
            {
                MessageBox.Show("pas de projet importer");
            }
            if (mainScale != null)
            {
                CalcPente(PenteIntersectionPoints, mainScale);
            }
            distances();

            try
            {
                alts = itm2[itm2.Count() - 1];


                alts = alts.GetRange(0, itm2.Count() - 1);
                alts.Reverse();
                int h = 0;
                for (int i = 0; i < itm2.Count(); i++)
                {

                    Polyline li = new Polyline();
                    mainCanvas.Children.Add(li);

                    for (int j = 0; j < itm2[i].Count(); j++)
                    {

                        li.FillRule = FillRule.EvenOdd;
                        li.StrokeThickness = 4;
                        if (i < itm2.Count() - 1)
                        {
                            li.Stroke = new SolidColorBrush(AltitudeToColor(alts[i].altitude));
                        }
                        else
                        {
                            li.Stroke = Brushes.Black;
                        }

                        li.Visibility = System.Windows.Visibility.Visible;
                        Ellipse circle = new Ellipse();
                        circle.Width = 10;
                        circle.Height = 10;
                        if (i == itm2.Count() - 1)
                        {

                            circle.Fill = Brushes.Red;

                        }
                        else
                        {


                            circle.Fill = Brushes.Purple;
                        }



                        Canvas.SetLeft(circle, itm2[i][j].point.X - (circle.Width / 2));
                        Canvas.SetTop(circle, itm2[i][j].point.Y - (circle.Height / 2));
                        Point ps = new Point(itm2[i][j].point.X, itm2[i][j].point.Y);
                        li.Points.Add(ps);

                        CourbeNiveau Courbe = new CourbeNiveau(li, itm2[itm2.Count() - 1][i].altitude);



                        CourbesNiveau.Add(Courbe);
                        mainCanvas.Children.Add(circle);

                    }
                    li.FillRule = FillRule.EvenOdd;
                    li.Visibility = System.Windows.Visibility.Visible;
                    h++;





                    if (i == (itm2.Count() - 1))
                    {

                        li.Stroke = Brushes.Purple;
                        li.StrokeThickness = 7;

                    }
                    else
                    {


                        li.StrokeThickness = 2;
                    }



                }

            }
            catch (ArgumentNullException) { }

        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {

            List<Polyline> curve = polylines;

            itm2 = this.DeSerialize();
            PointsGlobal.Clear();
            mainCanvas.Children.Clear();
            for (int i = 0; i < itm2.Count() - 2; i++)
            {
                PointsGlobal.Add(new List<ArtPoint>());
                for (int j = 0; j < itm2[i].Count(); j++)
                {
                    Ellipse circle = new Ellipse();
                    ArtPoint artpt = new ArtPoint(circle, itm2[i][j].point);

                    PointsGlobal[i].Add(artpt);

                }
            }


            CourbesNiveau.Clear();

            String penteText = " la pente est de   :" + pente.ToString() + " % ";

            try
            {
                PenteIntersectionPoints = itm2[itm2.Count() - 3];
                IntersectionPoints = PenteIntersectionPoints;
                distances();
            }
            catch (Exception)
            {
                MessageBox.Show("pas de projet importé");
            }
            if (mainScale != null)
            {
                CalcPente(PenteIntersectionPoints, mainScale);
            }

            try
            {
                //alts = itm2[itm2.Count() - 2];


                //alts = alts.GetRange(0, alts.Count() - 2);
                //alts.Reverse();

                int h = 0;
                for (int i = 0; i < itm2.Count() - 1; i++)
                {



                    // infos du plan
                    if (i == itm2.Count() - 2)
                    {
                        UpdateTextBoxes(Convert.ToInt32(itm2[i][0].altitude), Convert.ToInt32(itm2[i][1].altitude), Convert.ToInt32(itm2[i][2].altitude), Convert.ToInt32(itm2[i][3].altitude), Convert.ToInt32(itm2[i][4].altitude));

                    }
                    else
                    {
                        Polyline li = new Polyline();
                        CourbeNiveau courbe = new CourbeNiveau(li);
                        CourbesNiveau.Add(courbe);

                        mainCanvas.Children.Add(li);
                        for (int j = 0; j < itm2[i].Count(); j++)
                        {

                            li.FillRule = FillRule.EvenOdd;
                            li.StrokeThickness = 4;
                            li.Visibility = System.Windows.Visibility.Visible;




                            List<ArtPoint> list = PointsGlobal[i];
                            ArtPoint artpt = list[j];
                            Ellipse circle = artpt.cercle;
                            // segment AB ----------------------------------
                            if (i == itm2.Count() - 3)
                            {


                                if (j == 0 || j == itm2[i].Count() - 1)
                                {

                                    li.Points.Add(artpt.P);
                                    artpt.cercle.Width = 15;
                                    artpt.cercle.Height = 15;
                                    artpt.cercle.Fill = Brushes.Red;
                                    cercles.Add(artpt.cercle);

                                    mainCanvas.Children.Add(artpt.cercle);

                                    Canvas.SetLeft(artpt.cercle, artpt.P.X - (artpt.cercle.Width / 2));
                                    Canvas.SetTop(artpt.cercle, artpt.P.Y - (artpt.cercle.Height / 2));
                                }
                                else
                                {
                                    artpt.cercle.Width = 15;
                                    artpt.cercle.Height = 15;
                                    artpt.cercle.Fill = Brushes.Red;
                                    cercles.Add(artpt.cercle);

                                    mainCanvas.Children.Add(artpt.cercle);

                                    Canvas.SetLeft(artpt.cercle, artpt.P.X - (artpt.cercle.Width / 2));
                                    Canvas.SetTop(artpt.cercle, artpt.P.Y - (artpt.cercle.Height / 2));
                                }



                                li.Stroke = Brushes.Purple;
                                li.StrokeThickness = 7;




                            }
                            // autres courbes----------------------------------------------------------------
                            else
                            {
                                // pts de ctrl
                                artpt.cercle.Width = 10;
                                artpt.cercle.Height = 10;
                                artpt.cercle.Fill = Brushes.Purple;
                                Point ps = new Point(artpt.P.X, artpt.P.Y);
                                li.Points.Add(ps);
                                circle.MouseMove += new System.Windows.Input.MouseEventHandler(Cercle_Mousemove);
                                circle.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(Ellipse_MouseLeftButtonUp);
                                circle.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(Ellipse_MouseLeftButtonDown);
                                //mainCanvas.Children.Add(artpt.cercle);

                                Canvas.SetLeft(artpt.cercle, artpt.P.X - (artpt.cercle.Width / 2));
                                Canvas.SetTop(artpt.cercle, artpt.P.Y - (artpt.cercle.Height / 2));

                                // Polyline 
                                li.FillRule = FillRule.EvenOdd;
                                li.Visibility = System.Windows.Visibility.Visible;
                                h++;
                                courbe.polyline.MouseMove += new MouseEventHandler(Path_MouseMove);
                                courbe.polyline.MouseLeftButtonUp += new MouseButtonEventHandler(Path_MouseLeftButtonUp);
                                courbe.polyline.MouseLeftButtonDown += new MouseButtonEventHandler(Path_MouseLeftButtonDown);


                                li.StrokeThickness = 2;

                                indexPoints = CourbesNiveau.IndexOf(courbe);
                                ShownCtrlPoint = PointsGlobal[indexPoints];

                            }
                        }




                    }







                }
                // gestion des altitudes ----------------------------
                for (int i = 0; i < itm2[itm2.Count - 1].Count; i++)
                {
                    List<IntersectionDetail> list = itm2[itm2.Count - 1];
                    CourbesNiveau[i].altitude = list[i].altitude;
                    CourbesNiveau[i].polyline.Stroke = new SolidColorBrush(AltitudeToColor(CourbesNiveau[i].altitude));

                }
                PointsGlobal.RemoveAt(PointsGlobal.Count - 1);
                courbeActuelle = CourbesNiveau[CourbesNiveau.Count - 1];
                nav.IsEnabled = false;
                dessinerButton.IsEnabled = false;
                add_line.IsEnabled = false;
                dessinerPoint.IsEnabled = false;
              



            }
            catch (ArgumentNullException) { }



        }


        //-----------methode de serialization--------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------

        public void Serializee(List<List<IntersectionDetail>> objet)
        {

            OpenFileDialog op = new OpenFileDialog();

            Stream s = File.Open("autosave.topo", FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(s, objet);
            s.Close();

        }

        //-----------surcharge methode de serialization--------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------
        public void Serializee(List<List<IntersectionDetail>> objet, string filename)
        {

            OpenFileDialog op = new OpenFileDialog();

            Stream s = File.Open(path: filename, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(s, objet);
            s.Close();
        }
        public void Serializee(List<object> objet)
        {

            OpenFileDialog op = new OpenFileDialog();

            Stream s = File.Open(path: "test.dat", FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(s, objet);

            s.Close();
        }


        //-----------methode de deserialization--------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------
        public List<List<IntersectionDetail>> DeSerialize()
        {
            List<List<IntersectionDetail>> objet = null;

            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a ";
            op.FileName = "Document"; // Default file name
            op.DefaultExt = ".topo"; // Default file extension
            op.Filter = "Text documents (.topo)|*.topo"; // Filter files by extension
            if (op.ShowDialog() == true)
            {
                string fiName = op.FileName;

            }
            try
            {
                Stream s = File.Open(path: op.FileName, FileMode.Open);

                BinaryFormatter bf = new BinaryFormatter();
                objet = (List<List<IntersectionDetail>>)bf.Deserialize(s);

                s.Close();
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("boite de dialogue fermee !!");
            }


            return objet;
        }

        private double pente;
        private List<IntersectionDetail> PenteIntersectionPoints = new List<IntersectionDetail>();



        //-----------affichage de pente--------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------

        private void Pente_Click(object sender, RoutedEventArgs e)
        {
            String pentetext;
            try
            {

                if (CourbesNiveau.Count == 0)
                {

                    throw new ErreurDeDessinDeSegment("Aucune courbes n'est dessiner!");

                }

                else
                {
                    try
                    {
                        Echelle echel = new Echelle(mainScale.ScaleDistanceOnCanvas, mainScale.ScaleDistanceOnField);
                    }
                    catch (NullReferenceException)
                    {
                        MessageBox.Show("echelle pas encore disponible ");
                    }


                    pente = System.Math.Round(CalcPente(PenteIntersectionPoints, mainScale), 3);




                    pentetext = (pente * 100).ToString() + " % ";
                    Pente pentebox = new Pente(pentetext);
                    pentebox.Show();
                    /*   MessageBox.Show(" la pente est de   :" + (pente * 100).ToString() + " % ");*/

                }
            }
            catch (ErreurDeDessinDeSegment)
            {

            }
        }
        //-------------------------------Click sur boutton de map------------------------------------
        //-------------------------------------------------------------------------------------------
        private void mapBut_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new MapPage(this));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {


        }
        //---ajout de ressource (style)---------
        //----------------------------------------
        public void AddResourceDictionary(string src)
        {
            Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(src, UriKind.Relative) });
        }
        public static String CurrentMode = "ResourceDictionnaries/LightTheme.xaml";

        //---------Boutton de changememnt de theme------------
        //----------------------------------------------------
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            Resources.Clear();

            Resources.MergedDictionaries.Clear();

            AddResourceDictionary("ResourceDictionnaries/DarkTheme.xaml");
            CurrentMode = "ResourceDictionnaries/DarkTheme.xaml";



        }


        //-----------------Boutton de changememnt de theme-----------------
        //-----------------------------------------------------------------
        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            Resources.Clear();
            Resources.MergedDictionaries.Clear();

            AddResourceDictionary("ResourceDictionnaries/LightTheme.xaml");
            CurrentMode = "ResourceDictionnaries/LightTheme.xaml";

        }

        //methode de calcul de pente
        //--------------------------
        public double CalcPente(List<IntersectionDetail> points, Echelle sc)
        {
            double sum = 0;

            Line l = new Line();

            for (int i = 0; i < points.Count() - 1; i++)
            {

                l.X1 = points[i + 1].point.X; l.Y1 = points[i + 1].point.Y;
                l.X2 = points[i].point.X; l.Y2 = points[i].point.Y;
                if (sc != null)
                {
                    sum += ((points[i + 1].altitude - points[i].altitude) / sc.FindDistanceOnField(l));
                }

            }
            return (Math.Round(sum / (points.Count()), 3));
        }



        //-------------------suppression d'une courbe------------------
        //-------------------------------------------------------------

        private void DeleteCurve_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (courbeActuelle == null)
                {
                    return;
                }

                mainCanvas.Children.Remove(courbeActuelle.polyline);
                int index = CourbesNiveau.IndexOf(courbeActuelle);

                try
                {
                    List<ArtPoint> list = PointsGlobal[index];


                    foreach (ArtPoint art in list)
                    {
                        mainCanvas.Children.Remove(art.cercle);

                    }
                    CourbesNiveau.Remove(courbeActuelle);
                    PointsGlobal.Remove(list);
                    list.Clear();


                }
                catch (ArgumentOutOfRangeException) { }
                if (CourbesNiveau.Count > 0)
                {
                    courbeActuelle = CourbesNiveau[CourbesNiveau.Count - 1];

                }
                else { courbeActuelle = null; }
            }
            catch (ArgumentNullException) { MessageBox.Show("pas de courbe selectionnée"); }

        }

        private PointAltitude pointAltitudeActuel = null;

        //--------------------------Creer une point et l'assigne a Point Altitude actuel-----------
        //-----------------------------------------------------------------------------------------
        public void MakeNewPoint()
        // this method creates a point and assigns it to PointAltitudeActuel
        {
            PointAltBox pointAltBox = new PointAltBox();
            pointAltBox.ShowDialog();


            if (pointAltBox.DialogResult == true)
            {
                if (int.TryParse(pointAltBox.Altitude.Text, out int result))

                {
                    pointAltitudeActuel = new PointAltitude(result, pointAltBox.typePointCmb.SelectedIndex);


                    pointAltitudeActuel.triangle.MouseLeftButtonDown += new MouseButtonEventHandler(Control_MouseLeftButtonDown2);
                    pointAltitudeActuel.triangle.MouseLeftButtonUp += new MouseButtonEventHandler(Control_MouseLeftButtonUp2);
                    pointAltitudeActuel.triangle.MouseMove += new MouseEventHandler(Control_MouseMove2);


                }
                else
                {
                    MessageBox.Show("Altitude Fausse !");
                }

            }


        }

        private Boolean drawPointsClicked = false;

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }


        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new SauvgardePage(curves, this));
        }

        public SerializationInfo BaseUri { get; private set; }

        private void drawPoint_Clicked(object sender, RoutedEventArgs e)
        // this method is when the trianngle button is clicked, it puts all other options to false, except the one for drawing the point
        {


            drawPointsClicked = true;
            addLineClicked = false;
            btn2Clicked = false;
            drawingScale = false;

            MakeNewPoint();


        }

        //comportement du click souris
        //-------------------------------
        private void Control_MouseLeftButtonDown2(object sender, MouseButtonEventArgs e)
        {
            selectedTriangle = (this).InputHitTest(e.GetPosition(this)) as FrameworkElement;
            if (selectedTriangle == null)
            {
                return;
            }

            if (selectedTriangle != null && selectedTriangle is Polygon)
            {

                if (navClicked == true)
                {


                    foreach (PointAltitude pointAltitude in pointsAltitude)
                    {

                        if (pointAltitude.triangle.Equals((Polygon)selectedTriangle))
                        {
                            pointAltitudeActuel = pointAltitude;

                            mainCanvas.Children.Remove(pointAltitudeActuel.altitudeTextBlock);

                        }


                    }

                    mvCtrl = true;
                    ptMouseStart = e.GetPosition(this);
                    elDragging = (this).InputHitTest(ptMouseStart) as FrameworkElement;
                    if (elDragging == null)
                    {
                        return;
                    }

                    if (elDragging != null && elDragging is Polygon)
                    {
                        ptElementStart = new Point(elDragging.Margin.Left, elDragging.Margin.Top);
                        margin = new Thickness(elDragging.Margin.Left, elDragging.Margin.Top, 0, 0);
                        elDragging.Cursor = Cursors.ScrollAll;
                        Mouse.Capture((elDragging));
                        isDragging = true;

                    }


                }
                else
                {
                    return;
                }
            }
        }

        //comportement du click souris
        //---------------------------
        private void Control_MouseLeftButtonUp2(object sender, MouseButtonEventArgs e)
        {
            Point pnt = e.GetPosition(this);
            if (elDragging == null)
            {
                return;
            }

            if (isDragging)
            {
                if (!mvCtrl)
                {
                    elDragging.Margin = margin;

                }

            }



            isDragging = false;
            (elDragging).Cursor = Cursors.Arrow;
            (elDragging).ReleaseMouseCapture();


            elDragging = null;
            test = true;
        }

        private void Control_MouseMove2(object sender, MouseEventArgs e)
        {
            if ((elDragging) == null)
            {
                return;
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point ptMouse = e.GetPosition(this);
                if (isDragging)
                {

                    if (elDragging == null)
                    {
#pragma warning disable CS1717 // Assignation effectuée à la même variable ; souhaitiez-vous assigner un autre élément ?
                        elDragging = (elDragging);
#pragma warning restore CS1717 // Assignation effectuée à la même variable ; souhaitiez-vous assigner un autre élément ?
                    }

                    double left = ptElementStart.X + ptMouse.X - ptMouseStart.X;
                    double top = ptElementStart.Y + ptMouse.Y - ptMouseStart.Y;



                    elDragging.Margin = new Thickness(left, top, 0, 0);// modify the margin to move the curve
                    if (mvCtrl)
                    {
                        margin = elDragging.Margin;
                    }


                }
            }
        }

        private TextBlock texteBlockActuel;
        private Boolean addTextClicked = false;


        //-------------------------methode d'ajout de text sur le canvas----
        //------------------------------------------------------------------

        public void AddNewText(object sender, RoutedEventArgs e)
        // this method creates a point and assigns it to PointAltitudeActuel
        {

            AddTextDialog addTextDialog = new AddTextDialog();// add text here
            addTextDialog.ShowDialog();


            if (addTextDialog.DialogResult == true)
            {
                if (addTextDialog.textTextBox.Text != "")
                {


                    addTextClicked = true;
                    drawPointsClicked = false;
                    addLineClicked = false;
                    btn2Clicked = false;
                    drawingScale = false;
                    texteBlockActuel = new TextBlock();
                    texteBlockActuel.Text = addTextDialog.textTextBox.Text;


                }
                else
                {
                    (new MssgBox("Pas d'altitude en entrée")).Show();
                }


            }


        }




    }

}
