using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessbotIA2
{
    class Bot_IA_Management
    {
       public Dictionary<Piece, List<int[]>> allCoordonneesAuthorisees_blanches;
        public Bot_IA_Management()
        {
            allCoordonneesAuthorisees_blanches = new Dictionary<Piece, List<int[]>>();
        }
        public void Update(GameTime gameTime, List<Piece> p_blanches, List<Piece> p_noires, Dictionary<Piece, int[]> coordonneesJeu)
        {
            getAll_CoordonneesPossibles_blanches(p_blanches, coordonneesJeu);
        }

        private void getAll_CoordonneesPossibles_blanches(List<Piece> p_blanches, Dictionary<Piece, int[]> coordonneesJeu)
        {
            allCoordonneesAuthorisees_blanches.Clear();
            foreach (Piece p in p_blanches)
            {
                p.getCoordonneesAutorisees(coordonneesJeu);
               /* Console.WriteLine("piece " + p);*/
                Console.WriteLine("coords count :" + p.Name +"  "+ p.ListeCoordonneesAutorisees.Count);
                foreach (int[] i in p.ListeCoordonneesAutorisees)
                {
                    Console.WriteLine( p.Name + "  x: " + i[0]+" y: " + i[1]);
                }
                    //allCoordonneesAuthorisees_blanches.Add(p, p.ListeCoordonneesAutorisees);
                    //  Console.WriteLine("count " + allCoordonneesAuthorisees_blanches.Keys.Count);

                }
        }
    }
}
