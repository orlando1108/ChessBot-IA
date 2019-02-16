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
    class Plateau
    {
        public int Dimension { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Vector2 Position { get; set; }
        public int[] SelectedCase_Coordonnees { get; set; }
        public Vector2 CasesPosition { get; set; }
        public List<Vector2> CoordonneesList { get; set; }
        public List<int> IndexesList { get; set; }
        int[,] IndexesTab { get; set; }
        public Dictionary<int[], Vector2> IndexesPositions_Map { get; set; }
        public int CasesDimension { get; set; }
        //public bool Active { get; set; }
        public bool Initialisation { get; set; }
        Texture2D blanc;
        Texture2D noir;
        Texture2D vert;
        Color caseSelectedColor;
        bool released;
        SpriteFont x_spriteFont;
        SpriteFont y_spriteFont;
        // bool selectionActive;



        public Plateau()
        {
            Position = new Vector2(50, 50);
            CasesPosition = new Vector2(0, 0);
            SelectedCase_Coordonnees = new int[] { 0, 0 };
            IndexesPositions_Map = new Dictionary<int[], Vector2>();
            caseSelectedColor = new Color(0, 76, 153, 100);
            released = false;
            CasesDimension = 80;
            Dimension = 8;
            Width = CasesDimension * Dimension;
            Height = Width;
            //Active = true;
            Initialisation = true;
            
        }

        public void LoadContent(ContentManager content)
        {
            blanc = content.Load<Texture2D>("carreBlanc");
            noir = content.Load<Texture2D>("carreNoir");
            vert = content.Load<Texture2D>("carreVert");
            x_spriteFont = content.Load<SpriteFont>("X_spriteFont");
            y_spriteFont = content.Load<SpriteFont>("Y_spriteFont");
        }
        public void UnloadContent()
        {
            blanc.Dispose();
            noir.Dispose();
            vert.Dispose();
            
        }

        public void Update(GameTime gameTime, List<Piece> liste)
        {
            padSelector(liste);
        }

        public void Draw(SpriteBatch spriteBatch, List<int[]> ListeCoordonneesAutorisees )
        {
           /* int xIndex_spriteFont = 0;
            int yIndex_spriteFont = 0;*/
            foreach (int[] index in IndexesPositions_Map.Keys)
            { 
               
                
               
                
                    if ((index[1] % 2 == 0 && index[0] % 2 != 0) || (index[1] % 2 != 0 && index[0] % 2 == 0))
                    {
                            spriteBatch.Draw(noir, new Rectangle((int)IndexesPositions_Map[index].X, (int)IndexesPositions_Map[index].Y, CasesDimension, CasesDimension), new Color(147, 147, 147, 120));
                    }
                    else
                    {
                            spriteBatch.Draw(blanc, new Rectangle((int)IndexesPositions_Map[index].X, (int)IndexesPositions_Map[index].Y, CasesDimension, CasesDimension), Color.White);
                    }
                
                if (ListeCoordonneesAutorisees != null)
                {
                    foreach (int[] coordonneesPiece in ListeCoordonneesAutorisees)
                    {
                        if (index[0] == coordonneesPiece[0] && index[1] == coordonneesPiece[1])
                        {
                            spriteBatch.Draw(vert, new Rectangle((int)IndexesPositions_Map[index].X, (int)IndexesPositions_Map[index].Y, CasesDimension, CasesDimension), Color.White);
                        }
                    }

                }
                if (index[0] == SelectedCase_Coordonnees[0] && index[1] == SelectedCase_Coordonnees[1])
                {
                    spriteBatch.Draw(noir, new Rectangle((int)IndexesPositions_Map[index].X, (int)IndexesPositions_Map[index].Y, CasesDimension, CasesDimension), caseSelectedColor);
                }
            }

            for(int i = 0; i< Dimension; i++)
            {
                spriteBatch.DrawString(x_spriteFont, i.ToString(), new Vector2((Position.X+((CasesDimension/2)+(CasesDimension *i)-10)), 10), Color.White);

                if (i < 1)
                {
                    for (int j = 0; j < Dimension; j++)
                    {
                        spriteBatch.DrawString(x_spriteFont, j.ToString(), new Vector2(Position.X - 20, Position.Y + (CasesDimension * j)+20), Color.White);
                    }
                }
              
            }

        }

        private void padSelector(List<Piece> liste)
        {
            KeyboardState state = Keyboard.GetState();
            
            if(!released && state.IsKeyUp(Keys.Up) && state.IsKeyUp(Keys.Down) && state.IsKeyUp(Keys.Left) && state.IsKeyUp(Keys.Right))
            {
                released = true;

            }
            if (released && state.IsKeyDown(Keys.Down) && SelectedCase_Coordonnees[1] < Dimension -1)
            {
                SelectedCase_Coordonnees[1] +=1;
                
                released = false;
            }
            if (released && state.IsKeyDown(Keys.Up) && SelectedCase_Coordonnees[1] > 0)
            {
                SelectedCase_Coordonnees[1] -=1;
                released = false;
            }
            if (released && state.IsKeyDown(Keys.Right) && SelectedCase_Coordonnees[0] < Dimension-1)
            {
                SelectedCase_Coordonnees[0] += 1;
                released = false;
            }
            if (released && state.IsKeyDown(Keys.Left) && SelectedCase_Coordonnees[0] > 0)
            {
                SelectedCase_Coordonnees[0] -= 1;
                released = false;
            }
           
           
        }


    }
}
