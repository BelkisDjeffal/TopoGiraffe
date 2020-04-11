﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Shapes;

namespace TopoGiraffe.Noyau
{
    class Plan
    // cette classe va contenir toutes nos courbes, nos point et les données dessinées
    
    {
        private int equidistance;
        private double minAltitude;
        private double maxAltitude;
        private double echelle;
        private List<Polygon> courbes;// je vais remplacer polygon par courbeNiveau



        public Plan(int aEquidistance, double aMinAltitude, double aMaxAltitude, double aEchelle)
        {
            equidistance = aEquidistance;
            minAltitude = aMinAltitude;
            maxAltitude = aMaxAltitude;
            echelle = aEchelle;
        }



        public int Equidistance
        {
            get { return equidistance; }
            set { equidistance = value;  }
        }

        public double MinAltitude
        {
            get { return minAltitude;  }
            set { minAltitude = value;  }
        }

        public double MaxAltitude
        {
            get { return maxAltitude; }
            set { maxAltitude = value; }
        }

        public double Echelle
        {
            get { return echelle; }
            set { echelle = value; }
        }

        public void AjouterCourbe(Polygon courbe)
        {
            courbes.Add(courbe);
        }

        public void SupprimerCourbe(Polygon courbe)
        {
            courbes.Remove(courbe);
        }


    }
}