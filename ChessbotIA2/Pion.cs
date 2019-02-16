using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ChessbotIA2
{
    class Pion
    {
        
       // int nbCoordonneesPossibles;
       /* public Pion(Vector2 initialPosition, int pasPosition, char type, char orientation, int startPosition) : base(initialPosition, pasPosition, type, orientation)
        {
          /*  if (Type == 'n')
            {
                Coordonnees = new int[] { startPosition, 1 };
            }
            else
            {
                Coordonnees = new int[] { startPosition, 6 };
           }*/
         //nbCoordonneesPossibles = 4;
       // }

     /*   public void getCoordonneesAutorisees()
        {
            ListeCoordonneesAutorisees = new List<int[]>() { };
            int[] Y_possibles = new int[] { Coordonnees[1]-1, Coordonnees[1]-2, Coordonnees[1]-1, Coordonnees[1]-1 };
            int[] X_possibles = new int[] { Coordonnees[0]-1, Coordonnees[0]+1, Coordonnees[0], Coordonnees[0] };

            foreach(int x in X_possibles)
            {
                if (x == Coordonnees[0])
                {
                    foreach (int y in Y_possibles)
                    {

                        if (y != Coordonnees[1])
                        {
                            int[] coordonneesPossible = new int[] { x, y };
                            ListeCoordonneesAutorisees.Add(coordonneesPossible);
                        }
                            

                    }
                }else
                {
                    foreach (int y in Y_possibles)
                    {

                        if (y == Coordonnees[1])
                        {
                            int[] coordonneesPossible = new int[] { x, y };
                            ListeCoordonneesAutorisees.Add(coordonneesPossible);
                        }
                    }
                }
                   
            }

            
          
        }
      /*  public override void Update(GameTime gameTime, Plateau plateau)
        {

            base.Update(gameTime, plateau);
        }

        /* public override void LoadContent(ContentManager content, string textureName)
         {
             base.LoadContent(content, textureName);
         }*/
    }
}
