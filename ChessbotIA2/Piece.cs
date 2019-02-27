using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessbotIA2
{
    class Piece
    {
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }
        public Rectangle Rec { get; set; }
        public char Couleur { get; set; }
        public char Orientation { get; set; }
        public int[] Coordonnees;
        public int[] CoordonneesInitiales;
        public int PasPosition { get; set; }
        public Vector2 InitialPosition { get; set; }
        public bool Active { get; set; }
        public bool Selected { get; set; }
        public bool Moving { get; set; }
        bool initialized;
        public bool Positioned { get; set; }
        //float speed;
        int perimetre;
        public Vector2 Direction { get; set; }
        public Vector2 Destination { get; set; }
        public List<int[]> ListeCoordonneesAutorisees { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        private bool firstSelection;


        List<int> Y_possibles_HV;
        List<int> X_possibles_HV;

        List<int> Y_possibles_D;
        List<int> X_possibles_D;


        List<int> Y_Haut_HV;
        List<int> Y_Bas_HV;
        List<int> Y_Gauche_HV;
        List<int> Y_Droite_HV;
        List<int> X_Bas_HV;
        List<int> X_Haut_HV;
        List<int> X_Gauche_HV;
        List<int> X_Droite_HV;


       /* List<int> Y_HautGauche_D;
        List<int> Y_HautDroite_D;
        List<int> Y_BasGauche_D;
        List<int> Y_BasDroite_D;
        List<int> X_HautGauche_D;
        List<int> X_HautDroite_D;
        List<int> X_BasGauche_D;
        List<int> X_BasDroites_D;*/
        
        public int numeroPion { get; set; }
        //List<int[]> coordonneesAutorisees_Supprimees;
        public List<Piece> liste_PiecesAmanger;
        private List<int[]> testCoords = new List<int[]>();
       


        public Piece(Vector2 initialPosition, string name, int perimetreJeu, int pasPosition, string type, char couleur, char orientation = '0', int startPosition = 0)
        {
            Position = new Vector2(0, 0);
            Direction = new Vector2(0, 0);
            CoordonneesInitiales = new int[2];
            Name = name;
            InitialPosition = initialPosition;
            PasPosition = pasPosition;
            //speed = 5f;
            Active = true;
            Type = type;
            Couleur = couleur;
            Orientation = orientation;
            Selected = false;
            Moving = false;
            initialized = false;
            Positioned = false;
            perimetre = perimetreJeu;
            numeroPion = startPosition;
            liste_PiecesAmanger = new List<Piece>();

            if (type == "pion")
            {
                firstSelection = true;
            }

            initializePosition(startPosition);


        }
        public virtual void UnloadContent()
        {
            Texture.Dispose();
        }

        public virtual void LoadContent(ContentManager content, Texture2D texture)
        {
            Texture = texture;
        }

        public virtual void Update(GameTime gameTime, Plateau plateau, ref List<int[]> coordAutorisees, List<Piece> piecesCapturees = null, Dictionary<Piece, int[]> coordonneesJeu = null)
        {

            if (!initialized)
            {
                Position = transformCoordonnees_ToPosition(Coordonnees);
                CoordonneesInitiales[0] = Coordonnees[0];
                CoordonneesInitiales[1] = Coordonnees[1];
                initialized = true;
            }
            else
            {
                if (Active)
                {
                    if (!Selected)
                    {
                        if (plateau.SelectedCase_Coordonnees[0] == Coordonnees[0] && plateau.SelectedCase_Coordonnees[1] == Coordonnees[1])
                        {
                            Selected = true;
                            coordAutorisees = new List<int[]>();
                            getCoordonneesAutorisees(coordonneesJeu);
                            if (coordAutorisees.Count < 1)
                            {
                                coordAutorisees = ListeCoordonneesAutorisees; // initial coordinates are replaced by new selected piece's authorized coordinates 
                            }
                        }
                    }
                    else
                    {
                        foreach (int[] coord in ListeCoordonneesAutorisees)
                        {
                            if (coord[0] == plateau.SelectedCase_Coordonnees[0] && coord[1] == plateau.SelectedCase_Coordonnees[1])
                            {
                                Moving = true;
                                ListeCoordonneesAutorisees.Clear();
                                break;
                            }
                        }
                        if (Moving)
                        {
                            Destination = transformCoordonnees_ToPosition(plateau.SelectedCase_Coordonnees);
                            if (Destination != Vector2.Zero)
                            {
                                Position = Destination;
                                Destination = Vector2.Zero;
                                Coordonnees[0] = plateau.SelectedCase_Coordonnees[0];
                                Coordonnees[1] = plateau.SelectedCase_Coordonnees[1];
                            }
                            if (liste_PiecesAmanger.Count > 0) //number of pieces which are on a green rectangle
                            {
                                Piece pieceAmanger = liste_PiecesAmanger.Find(x => x.Coordonnees[0] == plateau.SelectedCase_Coordonnees[0] && x.Coordonnees[1] == plateau.SelectedCase_Coordonnees[1]);
                                if (pieceAmanger != null)
                                {
                                    pieceAmanger.Active = false;
                                    piecesCapturees.Add(pieceAmanger);
                                    //before position translation, we have to adapt coordinates according their reduced dimensions 
                                    pieceAmanger.Coordonnees = new int[] { (plateau.Dimension * 2) + (piecesCapturees.Count - 1) , (plateau.Dimension * 2) - 2 };
                                    coordonneesJeu.Remove(pieceAmanger);
                                }

                            }
                            Selected = false;
                        }
                        else //finally player decide to not move his piece, unselect and clear coordinates
                        {
                            Selected = false;
                            ListeCoordonneesAutorisees.Clear();
                        }
                    }
                }
                else   // new position for captured / inactived piece
                {
                    Position = transformCoordonnees_ToPosition(Coordonnees);
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            int x_source = 300;
            int y_source = 400;
            int y_source_b = 400;
            int width_source = 300;
            int height_source = 400;

          
                if (Couleur == 'b')
                {
                    Rec = Active ? new Rectangle((int)Position.X + 3, (int)Position.Y + 5, 70, 80) : Rec = new Rectangle((int)Position.X, (int)Position.Y + 5, 40, 50);
                }
                else
                {
                    Rec = Active ? new Rectangle((int)Position.X + 3, (int)Position.Y, 70, 80) : new Rectangle((int)Position.X, (int)Position.Y, 40, 50);
                }
         
               /* if (Couleur == 'b')
                {
                    Rec = new Rectangle((int)Position.X, (int)Position.Y + 5, 30, 60);
                }
                else
                {
                    Rec =
                }
           */



            if (Selected)
            {

                if (Type == "pion")
                {
                    if (Couleur == 'n')
                    {
                        spriteBatch.Draw(Texture, Rec, new Rectangle(x_source * 5, 0, width_source, height_source), new Color(255, 128, 0, 130));
                    }
                    else
                    {
                        spriteBatch.Draw(Texture, Rec, new Rectangle(x_source * 5, y_source, width_source, height_source), new Color(255, 128, 0, 130));
                    }
                }
                if (Type == "tour")
                {
                    if (Couleur == 'n')
                    {
                        spriteBatch.Draw(Texture, Rec, new Rectangle(0, 0, width_source, height_source), new Color(255, 128, 0, 130));
                    }
                    else
                    {
                        spriteBatch.Draw(Texture, Rec, new Rectangle(0, y_source, width_source, height_source), new Color(255, 128, 0, 130));
                    }
                }


                if (Type == "fou")
                {
                    if (Couleur == 'n')
                    {
                        spriteBatch.Draw(Texture, Rec, new Rectangle(x_source, 0, width_source, height_source), new Color(255, 128, 0, 130));

                    }
                    else
                    {
                        spriteBatch.Draw(Texture, Rec, new Rectangle(x_source, y_source, width_source, height_source), new Color(255, 128, 0, 130));

                    }
                }
                if (Type == "cavalier")
                {
                    if (Couleur == 'n')
                    {
                        spriteBatch.Draw(Texture, Rec, new Rectangle(x_source * 4, 0, width_source, height_source), new Color(255, 128, 0, 130));

                    }
                    else
                    {
                        spriteBatch.Draw(Texture, Rec, new Rectangle(x_source * 4, y_source, width_source, height_source), new Color(255, 128, 0, 130));

                    }
                }
                if (Type == "roi")
                {
                    if (Couleur == 'n')
                    {
                        spriteBatch.Draw(Texture, Rec, new Rectangle(x_source * 3, 0, width_source, height_source), new Color(255, 128, 0, 130));
                    }
                    else
                    {
                        spriteBatch.Draw(Texture, Rec, new Rectangle(x_source * 3, y_source, width_source, height_source), new Color(255, 128, 0, 130));
                    }
                }
                if (Type == "reine")
                {
                    if (Couleur == 'n')
                    {
                        spriteBatch.Draw(Texture, Rec, new Rectangle(x_source * 2, 0, width_source, height_source), new Color(255, 128, 0, 130));
                    }
                    else
                    {
                        spriteBatch.Draw(Texture, Rec, new Rectangle(x_source * 2, y_source, width_source, height_source), new Color(255, 128, 0, 130));
                    }

                }
            }
            else
            {
                if (Type == "pion")
                {
                    if (Couleur == 'n')
                    {
                        spriteBatch.Draw(Texture, Rec, new Rectangle(x_source * 5, 0, width_source, height_source), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(Texture, Rec, new Rectangle(x_source * 5, y_source_b, width_source, height_source), Color.White);
                    }
                }
                if (Type == "tour")
                {
                    if (Couleur == 'n')
                    {
                        spriteBatch.Draw(Texture, Rec, new Rectangle(0, 0, width_source, height_source), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(Texture, Rec, new Rectangle(0, y_source, width_source, height_source), Color.White);

                    }
                }


                if (Type == "fou")
                {
                    if (Couleur == 'n')
                    {
                        spriteBatch.Draw(Texture, Rec, new Rectangle(x_source, 0, width_source, height_source), Color.White);

                    }
                    else
                    {
                        spriteBatch.Draw(Texture, Rec, new Rectangle(x_source, y_source, width_source, height_source), Color.White);

                    }
                }
                if (Type == "cavalier")
                {
                    if (Couleur == 'n')
                    {
                        spriteBatch.Draw(Texture, Rec, new Rectangle(x_source * 4, 0, width_source, height_source), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(Texture, Rec, new Rectangle(x_source * 4, y_source, width_source, height_source), Color.White);

                    }
                }
                if (Type == "roi")
                {
                    if (Couleur == 'n')
                    {
                        spriteBatch.Draw(Texture, Rec, new Rectangle(x_source * 3, 0, width_source, height_source), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(Texture, Rec, new Rectangle(x_source * 3, y_source, width_source, height_source), Color.White);
                    }
                }
                if (Type == "reine")
                {
                    if (Couleur == 'n')
                    {
                        spriteBatch.Draw(Texture, Rec, new Rectangle(x_source * 2, 0, width_source, height_source), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(Texture, Rec, new Rectangle(x_source * 2, y_source, width_source, height_source), Color.White);
                    }

                }



            }



        }

        public Vector2 transformCoordonnees_ToPosition(int[] coordonnees)
        {
            PasPosition = Active ? PasPosition : 40;
            Vector2 newPosition = new Vector2(InitialPosition.X + (coordonnees[0] * PasPosition), InitialPosition.Y + (coordonnees[1] * PasPosition));
            return newPosition;
        }

        public Vector2 getVectorDirection(Vector2 start, Vector2 end)
        {
            float distance = Vector2.Distance(start, end);
            Vector2 direction = Vector2.Normalize(end - start);
            return direction;
        }

        public void getCoordonneesAutorisees(Dictionary<Piece, int[]> coordonneesJeu)
        {
            ListeCoordonneesAutorisees = new List<int[]>() { };
            getCoordonnees_HorizontalesVerticales(coordonneesJeu);
            getCoordonnees_Diagonales(coordonneesJeu);

        }

        private void getCoordonnees_HorizontalesVerticales(Dictionary<Piece, int[]> coordonneesJeu)
        {
            Obtenir_XY_Possibles_HorizontlesVerticales();
            createCoordonneesPossibles_HorizontlesVerticales(coordonneesJeu);
        }

        private void getCoordonnees_Diagonales(Dictionary<Piece, int[]> coordonneesJeu)
        {
            Obtenir_XY_Possibles_Diagonales(coordonneesJeu);
            createCoordonneesPossibles_Diagonales(coordonneesJeu);
            //ListeCoordonneesAutorisees.Distinct().ToList();


        }

        #region test DRY
        /* private void algoCreationCoord_HV(ref int i , Dictionary<Piece, int[]> coordonneesJeu, string sensCoord)
         {
             foreach (int[] coord in coordonneesJeu.Values.Where(x => predicat_HV(x, sensCoord)))
             {
                 if (tri_coordPossibles_selonSens(sensCoord, X_possibles_HV[i], Y_possibles_HV[i]))
                 {
                     if (coord[0] == X_possibles_HV[i] && coord[1] == Y_possibles_HV[i])
                     {
                         if (getKeyByValue(coordonneesJeu, coord).Couleur == 'b')
                         {
                             if (X_possibles_HV[i] == ListeCoordonneesAutorisees.Last()[0])
                             {
                                 ListeCoordonneesAutorisees.RemoveAt(ListeCoordonneesAutorisees.Count - 1);
                                 i = X_possibles_HV.Count();
                                 break;
                             }
                             else
                             {
                                 i = X_possibles_HV.Count();
                                 break;
                             }

                         }
                         else
                         {
                             int[] coordonneePossible = new int[] { X_possibles_HV[i], Y_possibles_HV[i] };
                             ListeCoordonneesAutorisees.Add(coordonneePossible);
                             i = X_possibles_HV.Count();
                             break;
                         }
                     }
                     else
                     {
                         if (ListeCoordonneesAutorisees.Count == 0)
                         {
                             int[] coordonneesPossible = new int[] { X_possibles_HV[i], Y_possibles_HV[i] };
                             ListeCoordonneesAutorisees.Add(coordonneesPossible);
                         }
                         else
                         {
                             int[] coordonneesPossible = new int[] { X_possibles_HV[i], Y_possibles_HV[i] };
                             if (coordonneesPossible[0] != ListeCoordonneesAutorisees.Last()[0])
                             {
                                 ListeCoordonneesAutorisees.Add(coordonneesPossible);
                             }
                         }
                     }
                 }
             }
         }

         private bool predicat_HV(int[] coord, string sensCoord)
         {
             // int[] coordReturned = new int[2];
             bool resutl = false;
             if(sensCoord == "x-")
             {
                 if (coord[0] < Coordonnees[0])
                 {

                     resutl = true;
                 }
             }
             if (sensCoord == "x+")
             {
                 if (coord[0] > Coordonnees[0])
                 {

                     resutl = true;
                 }
             }
             if (sensCoord == "y-")
             {
                 if (coord[1] < Coordonnees[1])
                 {

                     resutl = true;
                 }
             }

             if (sensCoord == "y+")
             {
                 if (coord[1] > Coordonnees[1])
                 {

                     resutl = true;
                 }
             }

             return resutl;

         }

         private bool tri_coordPossibles_selonSens(string sens, int xPossible,int yPossible)
         {
             bool resutl = false;
             if(sens == "x-")
             {
                 if (xPossible < Coordonnees[0] && yPossible == Coordonnees[1])
                 {
                     resutl = true;
                 }else
                 {
                     resutl = false;
                 }
             }
             if (sens == "x+")
             {
                 if (xPossible > Coordonnees[0] && yPossible == Coordonnees[1])
                 {
                     resutl = true;
                 }
                 else
                 {
                     resutl = false;
                 }
             }
             if (sens == "y-")
             {
                 if (yPossible < Coordonnees[1] && xPossible == Coordonnees[0])
                 {
                     resutl = true;
                 }
                 else
                 {
                     resutl = false;
                 }
             }
             if (sens == "y+")
             {
                 if (yPossible > Coordonnees[1] && xPossible == Coordonnees[0])
                 {
                     resutl = true;
                 }
                 else
                 {
                     resutl = false;
                 }
             }
             return resutl;
         }*/
        #endregion

        private void createCoordonneesPossibles_HorizontlesVerticales(Dictionary<Piece, int[]> coordonneesJeu)
        {
            if (Type == "pion" || Type == "tour" || Type == "reine" || Type == "roi")
            {

                for (int i = 0; i < X_Gauche_HV.Count(); i++)
                {

                    
                    foreach (int[] coord in coordonneesJeu.Values.Where(x => x[0] < Coordonnees[0]))
                    {
                            if (coord[0] == X_Gauche_HV[i] && coord[1] == Y_Gauche_HV[i])
                            {
                                Piece pieceTrouvee = getPiece(coordonneesJeu, coord);
                                if (pieceTrouvee.Couleur == 'b')
                                {
                                    if (X_Gauche_HV[i] == ListeCoordonneesAutorisees.Last()[0])
                                    {
                                        ListeCoordonneesAutorisees.RemoveAt(ListeCoordonneesAutorisees.Count - 1);
                                        i = X_Gauche_HV.Count();
                                        break;
                                    }
                                    else
                                    {
                                        i = X_Gauche_HV.Count();
                                        break;
                                    }

                                }
                                else
                                {

                                    if (X_Gauche_HV[i] != ListeCoordonneesAutorisees.Last()[0])
                                    {
                                        int[] coordonneePossible = new int[] { X_Gauche_HV[i], Y_Gauche_HV[i] };
                                        ListeCoordonneesAutorisees.Add(coordonneePossible);
                                        liste_PiecesAmanger.Add(pieceTrouvee);
                                        i = X_Gauche_HV.Count();
                                        break;
                                    }
                                    else
                                    {
                                        int[] coordonneePossible = new int[] { X_Gauche_HV[i], Y_Gauche_HV[i] };
                                        ListeCoordonneesAutorisees.Add(coordonneePossible);
                                        liste_PiecesAmanger.Add(pieceTrouvee);
                                        i = X_Gauche_HV.Count();
                                        break;
                                    }

                                }
                            }
                            else
                            {
                                int[] coordonneePossible = new int[] { X_Gauche_HV[i], Y_Gauche_HV[i] };
                                if (ListeCoordonneesAutorisees.Count == 0)
                                {

                                    ListeCoordonneesAutorisees.Add(coordonneePossible);
                                }
                                else
                                {
                                    if (coordonneePossible[0] != ListeCoordonneesAutorisees.Last()[0])
                                    {
                                        ListeCoordonneesAutorisees.Add(coordonneePossible);
                                    }
                                }
                            }
                        
                    }
                    //algoCreationCoord_HV(ref i, coordonneesJeu, "x-");
                }

                for (int i = 0; i < X_Droite_HV.Count(); i++)
                {
                    
                    foreach (int[] coord in coordonneesJeu.Values.Where(x => x[0] > Coordonnees[0]))
                    {
                       
                            if (coord[0] == X_Droite_HV[i] && coord[1] == Y_Droite_HV[i])
                            {
                                Piece pieceTrouvee = getPiece(coordonneesJeu, coord);
                                if (pieceTrouvee.Couleur == 'b')
                                {
                                    if (X_Droite_HV[i] == ListeCoordonneesAutorisees.Last()[0])
                                    {
                                        ListeCoordonneesAutorisees.RemoveAt(ListeCoordonneesAutorisees.Count - 1);
                                        i = X_Droite_HV.Count();
                                        break;
                                    }
                                    else
                                    {
                                        i = X_Droite_HV.Count();
                                        break;
                                    }
                                }
                                else
                                {
                                    if (X_Droite_HV[i] != ListeCoordonneesAutorisees.Last()[0])
                                    {
                                        int[] coordonneePossible = new int[] { X_Droite_HV[i], Y_Droite_HV[i] };
                                        ListeCoordonneesAutorisees.Add(coordonneePossible);
                                        liste_PiecesAmanger.Add(pieceTrouvee);
                                        i = X_Droite_HV.Count();
                                        break;
                                    }
                                    else
                                    {
                                        int[] coordonneePossible = new int[] { X_Droite_HV[i], Y_Droite_HV[i] };
                                        ListeCoordonneesAutorisees.Add(coordonneePossible);
                                        liste_PiecesAmanger.Add(pieceTrouvee);
                                        i = X_Droite_HV.Count();
                                        break;
                                    }

                                }
                            }
                            else
                            {
                                if (ListeCoordonneesAutorisees.Count == 0)
                                {
                                    int[] coordonneesPossible = new int[] { X_Droite_HV[i], Y_Droite_HV[i] };
                                    ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                }
                                else
                                {
                                    int[] coordonneesPossible = new int[] { X_Droite_HV[i], Y_Droite_HV[i] };
                                    if (coordonneesPossible[0] != ListeCoordonneesAutorisees.Last()[0])
                                    {
                                        ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                    }
                                }
                            }
                        
                    }
                }


                for (int i = 0; i < Y_Haut_HV.Count(); i++)
                {

                    //dans toutes les coordonnees du jeu donbt x = piece.x et y != 
                    foreach (int[] coord in coordonneesJeu.Values.Where(x => x[1] < Coordonnees[1]))
                    {
                       
                            if (coord[1] == Y_Haut_HV[i] && coord[0] == X_Haut_HV[i])
                            {
                                Piece pieceTrouvee = getPiece(coordonneesJeu, coord);
                                if (pieceTrouvee.Couleur == 'b')
                                {
                                    if (Y_Haut_HV[i] == ListeCoordonneesAutorisees.Last()[1])
                                    {
                                        ListeCoordonneesAutorisees.RemoveAt(ListeCoordonneesAutorisees.Count - 1);
                                        i = Y_Haut_HV.Count();
                                        break;
                                    }
                                    else
                                    {
                                        i = Y_Haut_HV.Count();
                                        break;
                                    }
                                }
                                else
                                {
                                    if (Y_Haut_HV[i] != ListeCoordonneesAutorisees.Last()[1])
                                    {
                                        int[] coordonneePossible = new int[] { X_Haut_HV[i], Y_Haut_HV[i] };
                                        ListeCoordonneesAutorisees.Add(coordonneePossible);
                                        liste_PiecesAmanger.Add(pieceTrouvee);
                                        i = Y_Haut_HV.Count();
                                        break;
                                    }
                                    else
                                    {
                                        int[] coordonneePossible = new int[] { X_Haut_HV[i], Y_Haut_HV[i] };
                                        ListeCoordonneesAutorisees.Add(coordonneePossible);
                                        liste_PiecesAmanger.Add(pieceTrouvee);
                                        i = Y_Haut_HV.Count();
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (ListeCoordonneesAutorisees.Count == 0)
                                {
                                    int[] coordonneesPossible = new int[] { X_Haut_HV[i], Y_Haut_HV[i] };
                                    ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                }
                                else
                                {
                                    int[] coordonneesPossible = new int[] { X_Haut_HV[i], Y_Haut_HV[i] };
                                    if (coordonneesPossible[1] != ListeCoordonneesAutorisees.Last()[1])
                                    {
                                        ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                    }
                                }
                            }
                        }
                    
                }

                for (int i = 0; i < Y_Bas_HV.Count(); i++)
                {
                    //dans toutes les coordonnees du jeu donbt x = piece.x et y != 
                    foreach (int[] coord in coordonneesJeu.Values.Where(x => x[1] > Coordonnees[1]))
                    {
                            if (coord[1] == Y_Bas_HV[i] && coord[0] == X_Bas_HV[i])
                            {
                                Piece pieceTrouvee = getPiece(coordonneesJeu, coord);
                                if (pieceTrouvee.Couleur == 'b')
                                {
                                    if (Y_Bas_HV[i] == ListeCoordonneesAutorisees.Last()[1])
                                    {
                                        ListeCoordonneesAutorisees.RemoveAt(ListeCoordonneesAutorisees.Count - 1);
                                        i = Y_Bas_HV.Count();
                                        break;
                                    }
                                    else
                                    {
                                        i = Y_Bas_HV.Count();
                                        break;
                                    }
                                }
                                else
                                {
                                    if (Y_Bas_HV[i] != ListeCoordonneesAutorisees.Last()[1])
                                    {
                                        int[] coordonneePossible = new int[] { X_Bas_HV[i], Y_Bas_HV[i] };
                                        ListeCoordonneesAutorisees.Add(coordonneePossible);
                                        liste_PiecesAmanger.Add(pieceTrouvee);
                                        i = Y_Bas_HV.Count();
                                        break;
                                    }
                                    else
                                    {
                                        int[] coordonneePossible = new int[] { X_Bas_HV[i], Y_Bas_HV[i] };
                                        ListeCoordonneesAutorisees.Add(coordonneePossible);
                                        liste_PiecesAmanger.Add(pieceTrouvee);
                                        i = Y_Bas_HV.Count();
                                        break;
                                    }
                                }
                            }
                            else
                            {
                            int[] coordonneesPossible = new int[] { X_Bas_HV[i], Y_Bas_HV[i] };
                            if (ListeCoordonneesAutorisees.Count == 0)
                                {
                                    ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                }
                                else
                                {
                                   // int[] coordonneesPossible = new int[] { X_Bas_HV[i], Y_Bas_HV[i] };
                                    if (coordonneesPossible[1] != ListeCoordonneesAutorisees.Last()[1])
                                    {
                                        ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                    }
                                }

                            }
                        }
                    }
                
                

            }

        }
        private void createCoordonneesPossibles_Diagonales(Dictionary<Piece, int[]> coordonneesJeu)
        {
             if ( Type == "roi")
            {
                for (int i = 0; i < X_possibles_D.Count(); i++)
                {
                    foreach (int[] coord in coordonneesJeu.Values.Where(x => x[1] < Coordonnees[1] && x[0] < Coordonnees[0]))
                    {
                        if (X_possibles_D[i] < Coordonnees[0] && Y_possibles_D[i] < Coordonnees[1])
                        {
                            if (coord[0] == X_possibles_D[i] && coord[1] == Y_possibles_D[i])
                            {
                                Piece pieceTrouvee = getPiece(coordonneesJeu, coord);
                                if (pieceTrouvee.Couleur == 'b')
                                {
                                    if (X_possibles_D[i] == ListeCoordonneesAutorisees.Last()[0] && Y_possibles_D[i] == ListeCoordonneesAutorisees.Last()[1]) // crest toujours = !!!
                                    {
                                        if (Type == "cavalier")
                                        {
                                            ListeCoordonneesAutorisees.RemoveAt(ListeCoordonneesAutorisees.Count - 1);
                                            break;
                                        }
                                        else
                                        {
                                            ListeCoordonneesAutorisees.RemoveAt(ListeCoordonneesAutorisees.Count - 1);
                                            i = X_possibles_D.Count();
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (Type != "cavalier")
                                        {
                                            i = X_possibles_D.Count();
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    if (Type == "cavalier")
                                    {
                                        int[] coordonneesPossible = new int[] { X_possibles_D[i], Y_possibles_D[i] };
                                        Console.WriteLine("CONTAINS 4 " + ListeCoordonneesAutorisees.Contains(coordonneesPossible));
                                        ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                        liste_PiecesAmanger.Add(pieceTrouvee);
                                    }
                                    else
                                    {
                                        int[] coordonneesPossible = new int[] { X_possibles_D[i], Y_possibles_D[i] };
                                        ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                        liste_PiecesAmanger.Add(pieceTrouvee);
                                        i = X_possibles_D.Count();
                                        break;
                                    }

                                }
                            }
                            else
                            { // BUG LE X OU le Y DU LAST EST IDENTIQUE DONC LA PREMIERE COORD NE PASSE PAS. il faut verifier le last de chaque premier partie de diagonale si il n'y a qu'une seule coordonnée dedans.

                                int[] coordonneesPossible = new int[] { X_possibles_D[i], Y_possibles_D[i] };
                                if (ListeCoordonneesAutorisees.Count == 0)
                                {
                                    ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                }
                                else if (coordonneesPossible[0] != ListeCoordonneesAutorisees.Last()[0] && coordonneesPossible[1] != ListeCoordonneesAutorisees.Last()[1])
                                {
                                    ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                }
                            }
                        }
                    }
                }
                for (int i = 0; i < X_possibles_D.Count(); i++)
                {
                    foreach (int[] coord in coordonneesJeu.Values.Where(x => x[1] > Coordonnees[1] && x[0] < Coordonnees[0]))
                    {

                        if (X_possibles_D[i] < Coordonnees[0] && Y_possibles_D[i] > Coordonnees[1])
                        {

                            if (coord[0] == X_possibles_D[i] && coord[1] == Y_possibles_D[i])
                            {
                                Piece pieceTrouvee = getPiece(coordonneesJeu, coord);
                                if (pieceTrouvee.Couleur == 'b')
                                {
                                    if (X_possibles_D[i] == ListeCoordonneesAutorisees.Last()[0] && Y_possibles_D[i] == ListeCoordonneesAutorisees.Last()[1]) // crest toujours = !!!
                                    {
                                        if (Type == "cavalier")
                                        {
                                            ListeCoordonneesAutorisees.RemoveAt(ListeCoordonneesAutorisees.Count - 1);
                                            break;
                                        }
                                        else
                                        {
                                            ListeCoordonneesAutorisees.RemoveAt(ListeCoordonneesAutorisees.Count - 1);
                                            i = X_possibles_D.Count();
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (Type != "cavalier")
                                        {
                                            i = X_possibles_D.Count();
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    if (Type == "cavalier")
                                    {
                                        int[] coordonneesPossible = new int[] { X_possibles_D[i], Y_possibles_D[i] };
                                        Console.WriteLine("CONTAINS 3 " + ListeCoordonneesAutorisees.Contains(coordonneesPossible));
                                        ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                        liste_PiecesAmanger.Add(pieceTrouvee);
                                    }
                                    else
                                    {
                                        int[] coordonneesPossible = new int[] { X_possibles_D[i], Y_possibles_D[i] };
                                        ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                        liste_PiecesAmanger.Add(pieceTrouvee);
                                        i = X_possibles_D.Count();
                                        break;
                                    }
                                }
                            }
                            else
                            { // BUG LE X OU le Y DU LAST EST IDENTIQUE DONC LA PREMIERE COORD NE PASSE PAS. il faut verifier le last de chaque premier partie de diagonale si il n'y a qu'une seule coordonnée dedans.
                                  //int t = 0;
                                int[] coordonneesPossible = new int[] { X_possibles_D[i], Y_possibles_D[i] };
                                //ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                if (ListeCoordonneesAutorisees.Count == 0)
                                {
                                    ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                }
                                else
                                {
                                    if ((ListeCoordonneesAutorisees.Last()[1] < Coordonnees[1] && coordonneesPossible[0] == ListeCoordonneesAutorisees.Last()[0]) || (ListeCoordonneesAutorisees.Last()[0] == Coordonnees[0] && coordonneesPossible[1] == ListeCoordonneesAutorisees.Last()[1]))
                                    {
                                        ListeCoordonneesAutorisees.Add(coordonneesPossible);

                                    }
                                    else if (coordonneesPossible[0] != ListeCoordonneesAutorisees.Last()[0] && coordonneesPossible[1] != ListeCoordonneesAutorisees.Last()[1])
                                    {
                                        ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                    }
                                }

                            }
                        }
                    }
                }
                for (int i = 0; i < X_possibles_D.Count(); i++)
                {
                    foreach (int[] coord in coordonneesJeu.Values.Where(x => x[1] < Coordonnees[1] && x[0] > Coordonnees[0]))
                    {
                        if (X_possibles_D[i] > Coordonnees[0] && Y_possibles_D[i] < Coordonnees[1])
                        {
                            if (coord[0] == X_possibles_D[i] && coord[1] == Y_possibles_D[i])
                            {
                                Piece pieceTrouvee = getPiece(coordonneesJeu, coord);
                                if (pieceTrouvee.Couleur == 'b')
                                {
                                    if (X_possibles_D[i] == ListeCoordonneesAutorisees.Last()[0] && Y_possibles_D[i] == ListeCoordonneesAutorisees.Last()[1]) // crest toujours = !!!
                                    {
                                        if (Type == "cavalier")
                                        {
                                            ListeCoordonneesAutorisees.RemoveAt(ListeCoordonneesAutorisees.Count - 1);
                                            break;
                                        }
                                        else
                                        {
                                            ListeCoordonneesAutorisees.RemoveAt(ListeCoordonneesAutorisees.Count - 1);
                                            i = X_possibles_D.Count();
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (Type != "cavalier")
                                        {
                                            i = X_possibles_D.Count();
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    if (Type == "cavalier")
                                    {
                                        int[] coordonneesPossible = new int[] { X_possibles_D[i], Y_possibles_D[i] };
                                        Console.WriteLine("CONTAINS 1 " + ListeCoordonneesAutorisees.Contains(coordonneesPossible));
                                        ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                        liste_PiecesAmanger.Add(pieceTrouvee);
                                    }
                                    else
                                    {
                                        int[] coordonneesPossible = new int[] { X_possibles_D[i], Y_possibles_D[i] };
                                       // Console.WriteLine("CONTAINS 2 " + ListeCoordonneesAutorisees.Contains(coordonneesPossible));
                                        ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                        liste_PiecesAmanger.Add(pieceTrouvee);
                                        i = X_possibles_D.Count();
                                        break;
                                    }
                                }
                            }
                            else
                            { // BUG LE X OU le Y DU LAST EST IDENTIQUE DONC LA PREMIERE COORD NE PASSE PAS. il faut verifier le last de chaque premier partie de diagonale si il n'y a qu'une seule coordonnée dedans.
                                int[] coordonneesPossible = new int[] { X_possibles_D[i], Y_possibles_D[i] };
                                if (ListeCoordonneesAutorisees.Count == 0)
                                { // ICI  doublons !!! 
                                    
                                    ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                }
                                else
                                {
                                    if ((ListeCoordonneesAutorisees.Last()[0] < Coordonnees[0] && coordonneesPossible[1] == ListeCoordonneesAutorisees.Last()[1]))
                                    {
                                        
                                        ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                    }
                                    else if (coordonneesPossible[0] != ListeCoordonneesAutorisees.Last()[0] && coordonneesPossible[1] != ListeCoordonneesAutorisees.Last()[1])
                                    {
                                        
                                        ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                    }
                                }
                            }
                        }
                    }
                }
                for (int i = 0; i < X_possibles_D.Count(); i++)
                {
                    foreach (int[] coord in coordonneesJeu.Values.Where(x => x[1] > Coordonnees[1] && x[0] > Coordonnees[0]))
                    {
                        if (X_possibles_D[i] > Coordonnees[0] && Y_possibles_D[i] > Coordonnees[1])
                        {

                            if (coord[0] == X_possibles_D[i] && coord[1] == Y_possibles_D[i])
                            {
                                Piece pieceTrouvee = getPiece(coordonneesJeu, coord);
                                if (pieceTrouvee.Couleur == 'b')
                                {
                                    if (X_possibles_D[i] == ListeCoordonneesAutorisees.Last()[0] && Y_possibles_D[i] == ListeCoordonneesAutorisees.Last()[1]) // crest toujours = !!!
                                    {
                                        if (Type == "cavalier")
                                        {
                                            ListeCoordonneesAutorisees.RemoveAt(ListeCoordonneesAutorisees.Count - 1);
                                            break;
                                        }
                                        else
                                        {
                                            ListeCoordonneesAutorisees.RemoveAt(ListeCoordonneesAutorisees.Count - 1);
                                            i = X_possibles_D.Count();
                                            break;
                                        }

                                    }
                                    else
                                    {
                                        if (Type != "cavalier")
                                        {
                                            i = X_possibles_D.Count();
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    if (Type == "cavalier")
                                    {
                                        int[] coordonneesPossible = new int[] { X_possibles_D[i], Y_possibles_D[i] };
                                        Console.WriteLine("CONTAINS 2 " + ListeCoordonneesAutorisees.Contains(coordonneesPossible));
                                        ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                        liste_PiecesAmanger.Add(pieceTrouvee);
                                    }
                                    else
                                    {
                                        int[] coordonneesPossible = new int[] { X_possibles_D[i], Y_possibles_D[i] };
                                        ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                        liste_PiecesAmanger.Add(pieceTrouvee);
                                        i = X_possibles_D.Count();
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                // BUG LE X OU le Y DU LAST EST IDENTIQUE DONC LA PREMIERE COORD NE PASSE PAS. il faut verifier le last de chaque premier partie de diagonale si il n'y a qu'une seule coordonnée dedans.
                                int[] coordonneesPossible = new int[] { X_possibles_D[i], Y_possibles_D[i] };
                                if (ListeCoordonneesAutorisees.Count == 0)
                                {
                                    ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                }
                                else
                                {
                                    if ((ListeCoordonneesAutorisees.Last()[1] < Coordonnees[1] && coordonneesPossible[0] == ListeCoordonneesAutorisees.Last()[0]) || (ListeCoordonneesAutorisees.Last()[0] < Coordonnees[0] && coordonneesPossible[1] == ListeCoordonneesAutorisees.Last()[1]))
                                    {
                                        ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                    }
                                    else if (coordonneesPossible[0] != ListeCoordonneesAutorisees.Last()[0] && coordonneesPossible[1] != ListeCoordonneesAutorisees.Last()[1])
                                    {
                                        ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                    }
                                }


                            }
                        }
                    }
                }
            }
            if (Type == "pion")
            {
                for (int i = 0; i < X_possibles_D.Count(); i++)
                {
                    foreach (int[] coord in coordonneesJeu.Values.Where(x => x[1] < Coordonnees[1] && x[0] < Coordonnees[0]))
                    {
                        if (X_possibles_D[i] < Coordonnees[0] && Y_possibles_D[i] < Coordonnees[1])
                        {
                            if (coord[0] == X_possibles_D[i] && coord[1] == Y_possibles_D[i])
                            {
                                if (getPiece(coordonneesJeu, new int[] { X_possibles_D[i], Y_possibles_D[i] }).Couleur == 'n')
                                {
                                    if (X_possibles_D[i] == ListeCoordonneesAutorisees.Last()[0] && Y_possibles_D[i] == ListeCoordonneesAutorisees.Last()[1]) // crest toujours = !!!
                                    {
                                        ListeCoordonneesAutorisees.RemoveAt(ListeCoordonneesAutorisees.Count - 1);
                                        i = X_possibles_D.Count();
                                        break;
                                    }
                                    else
                                    {
                                        int[] coordonneesPossible = new int[] { X_possibles_D[i], Y_possibles_D[i] };
                                        ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                        i = X_possibles_D.Count();
                                        break;
                                    }
                                }
                                else
                                {
                                    i = X_possibles_D.Count();
                                    break;
                                }
                            }
                        }
                    }
                }
                for (int i = 0; i < X_possibles_D.Count(); i++)
                {
                    foreach (int[] coord in coordonneesJeu.Values.Where(x => x[1] < Coordonnees[1] && x[0] > Coordonnees[0]))
                    {
                        if (X_possibles_D[i] > Coordonnees[0] && Y_possibles_D[i] < Coordonnees[1])
                        {
                            if (coord[0] == X_possibles_D[i] && coord[1] == Y_possibles_D[i])
                            {
                                if (getPiece(coordonneesJeu, new int[] { X_possibles_D[i], Y_possibles_D[i] }).Couleur == 'n')
                                {
                                    int[] coordonneesPossible = new int[] { X_possibles_D[i], Y_possibles_D[i] };
                                    if (ListeCoordonneesAutorisees.Count == 0)
                                    {
                                        ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                    }
                                    else
                                    {
                                        if (X_possibles_D[i] == ListeCoordonneesAutorisees.Last()[0] && Y_possibles_D[i] == ListeCoordonneesAutorisees.Last()[1]) // crest toujours = !!!
                                        {
                                            ListeCoordonneesAutorisees.RemoveAt(ListeCoordonneesAutorisees.Count - 1);
                                            i = X_possibles_D.Count();
                                            break;
                                        }
                                        else
                                        {
                                            if (ListeCoordonneesAutorisees.Last()[0] < Coordonnees[0] && coordonneesPossible[1] == ListeCoordonneesAutorisees.Last()[1])
                                            {

                                                ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                            }
                                            else if (ListeCoordonneesAutorisees.Last()[0] == Coordonnees[0] && coordonneesPossible[1] == ListeCoordonneesAutorisees.Last()[1])
                                            {
                                                ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                            }
                                        }

                                    }
                                }
                                else
                                {
                                    i = X_possibles_D.Count();
                                    break;
                                }
                            }
                        }
                    }
                }
            }

        }
        private void Obtenir_XY_Possibles_HorizontlesVerticales()//si map alors param coordonnees
        {
            Y_possibles_HV = new List<int>();
            X_possibles_HV = new List<int>();
            Y_Haut_HV  = new List<int>();
            Y_Bas_HV = new List<int>();
            Y_Gauche_HV = new List<int>();
            Y_Droite_HV = new List<int>();
            X_Bas_HV = new List<int>();
            X_Haut_HV = new List<int>();
            X_Gauche_HV = new List<int>();
            X_Droite_HV = new List<int>();

            if (Type == "pion")
            {
                if (firstSelection)
                {
                    for (int i = 1; i < 3; i++)
                    {
                        Y_Haut_HV.Add(Coordonnees[1] - i);
                        X_Haut_HV.Add(Coordonnees[0]);
                    }
                    firstSelection = false;
                }
                else
                {
                    for (int i = 1; i < 2; i++)
                    {

                        Y_Haut_HV.Add(Coordonnees[1] - i);
                        X_Haut_HV.Add(Coordonnees[0]);
                    }
                }
            }
            if (Type == "tour" || Type == "reine")
            {

                //de 0 à X
                for (int i = 1; i <= Coordonnees[0]; i++)
                {
                    X_Gauche_HV.Add(Coordonnees[0] - i);
                    Y_Gauche_HV.Add(Coordonnees[1]);
                }
                //de X à la taille max du plateau
                for (int i = 1; i <= (perimetre - 1) - Coordonnees[0]; i++)
                {
                    X_Droite_HV.Add(Coordonnees[0] + i);
                    Y_Droite_HV.Add(Coordonnees[1]);
                }
                //de 0 à y
                for (int i = 1; i <= Coordonnees[1]; i++)
                {
                    //X_possibles_D.Add(Coordonnees[0] - i);
                    Y_Haut_HV.Add(Coordonnees[1] - i);
                    X_Haut_HV.Add(Coordonnees[0]);
                }
                //de Y à la taille max du plateau
                for (int i = 1; i <= (perimetre - 1) - Coordonnees[1]; i++)
                {
                    Y_Bas_HV.Add(Coordonnees[1] + i);
                    X_Bas_HV.Add(Coordonnees[0]);
                }
            }
            if (Type == "roi")
            {
               // for (int i = 1; i < 2; i++)
                //{
                    X_Gauche_HV.Add(Coordonnees[0] - 1);
                    X_Droite_HV.Add(Coordonnees[0] + 1);
                    X_Haut_HV.Add(Coordonnees[0]);
                    X_Bas_HV.Add(Coordonnees[0]);
                    Y_Gauche_HV.Add(Coordonnees[1]);
                    Y_Droite_HV.Add(Coordonnees[1]);
                    Y_Haut_HV.Add(Coordonnees[1] - 1);
                    Y_Bas_HV.Add(Coordonnees[1] + 1);
              //  }
            }
        }
        private void Obtenir_XY_Possibles_Diagonales(Dictionary<Piece,int[]> coordsJeu)
        {
            Y_possibles_D = new List<int>();
            X_possibles_D = new List<int>();
            int[] coordonneesPossible;
            Piece pieceTrouvee;

            //si le pion peut manger 
            if (Type == "pion")
            {
                for (int i = 1; i < 2; i++)
                {

                    Y_possibles_D.Add(Coordonnees[1] - i);
                    Y_possibles_D.Add(Coordonnees[1] - i);
                    X_possibles_D.Add(Coordonnees[0] - i);
                    X_possibles_D.Add(Coordonnees[0] + i);
                }
            }
            if (Type == "fou" || Type == "reine")
            {

                //de 0 à X
                for (int i = 1; i <= Coordonnees[0]; i++)
                {
                    if (!(Coordonnees[0] - i < 0 || Coordonnees[1] - i < 0))
                    {
                        coordonneesPossible = new int[] { Coordonnees[0] - i, Coordonnees[1] - i };
                        pieceTrouvee = getPiece(coordsJeu, coordonneesPossible);
                        if (pieceTrouvee != null)
                        {
                            if (pieceTrouvee.Couleur == 'b')
                            {
                                break;
                            }
                            else
                            {
                                ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                liste_PiecesAmanger.Add(pieceTrouvee);
                                break;
                            }
                        }
                        else
                        {
                            ListeCoordonneesAutorisees.Add(coordonneesPossible);
                        }
                    }
                }
            for (int i = 1; i <= Coordonnees[0]; i++)
                {
                    if (!(Coordonnees[0] - i < 0 || Coordonnees[1] + i > 7))
                    {
                        coordonneesPossible = new int[] { Coordonnees[0] - i, Coordonnees[1] + i };
                        pieceTrouvee = getPiece(coordsJeu, coordonneesPossible);
                        if (pieceTrouvee != null)
                        {
                            if (pieceTrouvee.Couleur == 'b')
                            {
                                break;
                            }
                            else
                            {
                                ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                liste_PiecesAmanger.Add(pieceTrouvee);
                                break;
                            }
                        }
                        else
                        {
                            ListeCoordonneesAutorisees.Add(coordonneesPossible);
                        }
                    }
                }


                for (int i = 1; i <= (perimetre - 1) - Coordonnees[0]; i++)
                {
                    if (!(Coordonnees[0] + i > 7 || Coordonnees[1] - i < 0))
                    {
                        coordonneesPossible = new int[] { Coordonnees[0] + i, Coordonnees[1] - i };
                        pieceTrouvee = getPiece(coordsJeu, coordonneesPossible);
                        if (pieceTrouvee != null)
                        {
                            if (pieceTrouvee.Couleur == 'b')
                            {
                                break;
                            }
                            else
                            {
                                ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                liste_PiecesAmanger.Add(pieceTrouvee);
                                break;
                            }
                        }
                        else
                        {
                            ListeCoordonneesAutorisees.Add(coordonneesPossible);
                        }
                    }
                }
                for (int i = 1; i <= (perimetre - 1) - Coordonnees[0]; i++)
                {
                        if (!(Coordonnees[0] + i > 7 || Coordonnees[1] + i > 7))
                    {
                        coordonneesPossible = new int[] { Coordonnees[0] + i, Coordonnees[1] + i };
                        pieceTrouvee = getPiece(coordsJeu, coordonneesPossible);
                        if (pieceTrouvee != null)
                        {
                            if (pieceTrouvee.Couleur == 'b')
                            {
                                break;
                            }
                            else
                            {
                                ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                liste_PiecesAmanger.Add(pieceTrouvee);
                                break;
                            }
                        }
                        else
                        {
                            ListeCoordonneesAutorisees.Add(coordonneesPossible);
                        }

                    }
                }

            }
            if (Type == "cavalier")
            {
                X_possibles_D.Add(Coordonnees[0] - 1);
                Y_possibles_D.Add(Coordonnees[1] - 2);
                X_possibles_D.Add(Coordonnees[0] - 2);
                Y_possibles_D.Add(Coordonnees[1] - 1);

                X_possibles_D.Add(Coordonnees[0] - 2);
                Y_possibles_D.Add(Coordonnees[1] + 1);
                X_possibles_D.Add(Coordonnees[0] - 1);
                Y_possibles_D.Add(Coordonnees[1] + 2);

                X_possibles_D.Add(Coordonnees[0] + 1);
                Y_possibles_D.Add(Coordonnees[1] - 2);
                X_possibles_D.Add(Coordonnees[0] + 2);
                Y_possibles_D.Add(Coordonnees[1] - 1);

                X_possibles_D.Add(Coordonnees[0] + 2);
                Y_possibles_D.Add(Coordonnees[1] + 1);
                X_possibles_D.Add(Coordonnees[0] + 1);
                Y_possibles_D.Add(Coordonnees[1] + 2);

                for(int i = 0; i < 8; i++)
                {
                    
                    
                    if (!(X_possibles_D[i] < 0 || X_possibles_D[i] > 7 ||  Y_possibles_D[i] < 0 || Y_possibles_D[i] > 7))
                    {
                        coordonneesPossible = new int[] { X_possibles_D[i], Y_possibles_D[i] };
                        pieceTrouvee = getPiece(coordsJeu, coordonneesPossible);
                        
                        if (pieceTrouvee != null)
                        {
                            if (pieceTrouvee.Couleur != 'b')
                            {
                                ListeCoordonneesAutorisees.Add(coordonneesPossible);
                                liste_PiecesAmanger.Add(pieceTrouvee);
                                
                            }
                        }
                        else
                        {
                            ListeCoordonneesAutorisees.Add(coordonneesPossible);
                        }
                    }
                }

            }
            if (Type == "roi")
            {
                for (int i = 1; i < 2; i++)
                {
                    X_possibles_D.Add(Coordonnees[0] - i);
                    X_possibles_D.Add(Coordonnees[0] - i);
                    X_possibles_D.Add(Coordonnees[0] + i);
                    X_possibles_D.Add(Coordonnees[0] + i);
                    Y_possibles_D.Add(Coordonnees[1] - i);
                    Y_possibles_D.Add(Coordonnees[1] + i);
                    Y_possibles_D.Add(Coordonnees[1] - i);
                    Y_possibles_D.Add(Coordonnees[1] + i);
                }
            }
        }
        /*private void verifierCoordonnees(Dictionary<Piece, int[]> coordonneesJeu)
        {
            coordonneesAutorisees_Supprimees = new List<int[]>();
            for (int i = ListeCoordonneesAutorisees.Count - 1; i >= 0; i--)
            {
                foreach (int[] coord in coordonneesJeu.Values)
                {
                    if (ListeCoordonneesAutorisees[i][0] == coord[0] && ListeCoordonneesAutorisees[i][1] == coord[1])
                    {
                        //  Trace.WriteLine(ListeCoordonneesAutorisees[i][0] + "      " + ListeCoordonneesAutorisees[i][1]);
                        coordonneesAutorisees_Supprimees.Add(ListeCoordonneesAutorisees[i]);
                        ListeCoordonneesAutorisees.RemoveAt(i);


                        break;
                    }
                }
            }
        }*/
        private List<int[]> getCoordonneesZone()
        {
            List<int[]> listeCoords = new List<int[]>();



            return listeCoords;
        }
        private Piece getPiece(Dictionary<Piece, int[]> coordonneesJeu, int[] coord)
        {
            var piece = coordonneesJeu.FirstOrDefault(x => x.Value[0] == coord[0] && x.Value[1] == coord[1]).Key;
            return piece;
        }

        /*  private void supprimerCoordonneesBlanches()
          {
              foreach(int[] coordAutorisee in ListeCoordonneesAutorisees)
              {

              }
              if (getKeyByValue(coordonneesJeu, coord).Couleur == 'b')
              {
                  if (Y_possibles_HV[i] == ListeCoordonneesAutorisees.Last()[1])
                  {
                      ListeCoordonneesAutorisees.RemoveAt(ListeCoordonneesAutorisees.Count - 1);
                      i = Y_possibles_HV.Count();
                      break;
                  }
                  else
                  {
                      i = Y_possibles_HV.Count();
                      break;
                  }
              }
              else
              {
                  int[] coordonneePossible = new int[] { X_possibles_HV[i], Y_possibles_HV[i] };
                  ListeCoordonneesAutorisees.Add(coordonneePossible);
                  i = Y_possibles_HV.Count();
                  break;
              }

          }*/

        private void initializePosition(int startPosition)
        {
            if (Type == "pion")
            {
                if (Couleur == 'n')
                {
                    Coordonnees = new int[] { startPosition, 1 };
                }
                else
                {
                    Coordonnees = new int[] { startPosition, 6 };
                }
            }
            if (Type == "tour")
            {
                if (Couleur == 'n')
                {
                    if (Orientation == 'g')
                    {
                        Coordonnees = new int[] { 0, 0 };
                    }
                    else
                    {
                        Coordonnees = new int[] { 7, 0 };
                    }
                }
                else
                {
                    if (Orientation == 'g')
                    {
                        Coordonnees = new int[] { 0, 7 };
                    }
                    else
                    {
                        Coordonnees = new int[] { 7, 7 };
                    }
                }
            }
            if (Type == "fou")
            {
                if (Couleur == 'n')
                {
                    if (Orientation == 'g')
                    {
                        Coordonnees = new int[] { 2, 0 };
                    }
                    else
                    {
                        Coordonnees = new int[] { 5, 0 };
                    }
                }
                else
                {
                    if (Orientation == 'g')
                    {
                        Coordonnees = new int[] { 2, 7 };
                    }
                    else
                    {
                        Coordonnees = new int[] { 6, 4 };
                    }
                }
            }
            if (Type == "cavalier")
            {
                if (Couleur == 'n')
                {
                    if (Orientation == 'g')
                    {
                        Coordonnees = new int[] { 1, 0 };
                    }
                    else
                    {
                        Coordonnees = new int[] { 6, 0 };
                    }
                }
                else
                {
                    if (Orientation == 'g')
                    {
                        Coordonnees = new int[] { 1, 7 };
                    }
                    else
                    {
                        Coordonnees = new int[] { 1, 4 };
                    }
                }
            }
            if (Type == "roi")
            {
                if (Couleur == 'n')
                {
                    Coordonnees = new int[] { 4, 0 };
                }
                else
                {
                    Coordonnees = new int[] { 4, 7 };
                }
            }
            if (Type == "reine")
            {
                if (Couleur == 'n')
                {
                    Coordonnees = new int[] { 3, 0 };
                }
                else
                {
                    Coordonnees = new int[] { 4, 4 };
                }
            }
        }

    }
}
