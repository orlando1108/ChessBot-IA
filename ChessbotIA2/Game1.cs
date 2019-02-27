using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;

namespace ChessbotIA2
{
    
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Plateau plateau;
        List<Piece> listePieces_Blanches;
        List<Piece> listePieces_Noires;
        List<Piece> listePions;
        List<Piece> listePieces;
        Piece pion_b;
        Piece pion_n;
        Piece tour_b1;
        Piece tour_b2;
        Piece tour_n1;
        Piece tour_n2;
        Piece cavalier_b1;
        Piece cavalier_b2;
        Piece cavalier_n1;
        Piece cavalier_n2;
        Piece fou_b1;
        Piece fou_b2;
        Piece fou_n1;
        Piece fou_n2;
        Piece roi_b;
        Piece roi_n;
        Piece reine_b;
        Piece reine_n;
        KeyboardState state;
        KeyboardState oldState;
        Texture2D TexturePieces;
        int i;
        int j;
        bool playing;
        bool gameInitialisation;
        bool isBot;
        List<int[]> listeCoordonneesAutorisees;
        Dictionary<Piece, int[]> IndexesObjects_Map;
        List<Piece> liste_PiecesCapturees;
        Bot_IA_Management bot_IA;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 750;
            graphics.PreferredBackBufferWidth = 1400;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            plateau = new Plateau();
            bot_IA = new Bot_IA_Management();
            // listePions = new List<Piece>();

            i = 0;
            j = 0;
            playing = false;
            gameInitialisation = true;
            isBot = false;
            IndexesObjects_Map = new Dictionary<Piece, int[]>();
            liste_PiecesCapturees = new List<Piece>();

        // pion_b = new Pion();
            tour_b1 = new Piece(plateau.Position, "tour_b1", plateau.Dimension, plateau.CasesDimension, "tour", 'b','g');
            tour_b2 = new Piece(plateau.Position, "tour_b2", plateau.Dimension, plateau.CasesDimension, "tour", 'b', 'd');
            tour_n1 = new Piece(plateau.Position, "tour_n1", plateau.Dimension, plateau.CasesDimension, "tour", 'n', 'g');
            tour_n2 = new Piece(plateau.Position, "tour_n2", plateau.Dimension, plateau.CasesDimension, "tour", 'n', 'd');


            fou_b1 = new Piece(plateau.Position, "fou_b1", plateau.Dimension, plateau.CasesDimension, "fou", 'b', 'g');
            fou_b2 = new Piece(plateau.Position, "fou_b2", plateau.Dimension, plateau.CasesDimension, "fou", 'b', 'd');
            fou_n1 = new Piece(plateau.Position, "fou_n1", plateau.Dimension, plateau.CasesDimension, "fou", 'n', 'g');
            fou_n2 = new Piece(plateau.Position, "fou_n2", plateau.Dimension, plateau.CasesDimension, "fou", 'n', 'd');
           


            cavalier_b1 = new Piece(plateau.Position, "cavalier_b1", plateau.Dimension, plateau.CasesDimension, "cavalier", 'b', 'g');
            cavalier_b2 = new Piece(plateau.Position, "cavalier_b2", plateau.Dimension, plateau.CasesDimension, "cavalier", 'b', 'd');
            cavalier_n1 = new Piece(plateau.Position, "cavalier_n1", plateau.Dimension, plateau.CasesDimension, "cavalier", 'n', 'g');
            cavalier_n2 = new Piece(plateau.Position, "cavalier_n2", plateau.Dimension, plateau.CasesDimension, "cavalier", 'n', 'd');


            roi_b = new Piece(plateau.Position, "roi_b", plateau.Dimension, plateau.CasesDimension, "roi", 'b');
            roi_n = new Piece(plateau.Position, "roi_n", plateau.Dimension, plateau.CasesDimension, "roi", 'n');
            reine_b = new Piece(plateau.Position, "reine_b", plateau.Dimension, plateau.CasesDimension, "reine", 'b');
            reine_n = new Piece(plateau.Position, "reine_n", plateau.Dimension, plateau.CasesDimension, "reine", 'n');


            listePieces_Blanches = new List<Piece>() { tour_b1, fou_b1, cavalier_b1, tour_b2, fou_b2, cavalier_b2, roi_b, reine_b };
            listePieces_Noires = new List<Piece>() { tour_n1, fou_n1, cavalier_n1, tour_n2, fou_n2, cavalier_n2, roi_n, reine_n };
            listePieces = new List<Piece>() { tour_b1, fou_b1, cavalier_b1, tour_b2, fou_b2, cavalier_b2, roi_b, reine_b, tour_n1, fou_n1, cavalier_n1, tour_n2, fou_n2, cavalier_n2, roi_n, reine_n };

            

            for (int i = 0; i< plateau.Dimension;i++)
            {
                pion_b = new Piece(plateau.Position, "pion_b"+i, plateau.Dimension, plateau.CasesDimension, "pion", 'b','0',i);
              //  listePions.Add(pion_b);
                listePieces_Blanches.Add(pion_b);
                listePieces.Add(pion_b);
                pion_n = new Piece(plateau.Position, "pion_b"+i, plateau.Dimension, plateau.CasesDimension, "pion", 'n','0',i);
               // listePions.Add(pion_n);
                listePieces_Noires.Add(pion_n);
                listePieces.Add(pion_n);
            }
             foreach(Piece p in listePieces)
            {
                IndexesObjects_Map.Add(p, p.Coordonnees);
            }
        }
        
        protected override void Initialize()
        {
           
            base.Initialize();
        }

        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
           // rectangle = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
           // rectangle.SetData(new[] { Color.White });
            plateau.LoadContent(Content);
            TexturePieces = Content.Load<Texture2D>("Pieces");
            foreach(Piece p in listePieces)
            {
                p.LoadContent(Content,TexturePieces);
            }
        }
        
        protected override void UnloadContent()
        {
            //rectangle.Dispose();
            plateau.UnloadContent();
            foreach(Piece p in listePieces)
            {
                p.UnloadContent();
            }
        }

      
       
        protected override void Update(GameTime gameTime)
        {
            Vector2 coordonnee = new Vector2(0, 0);
            int casesPositions_X;
            int casesPositions_Y;

            plateau.Update(gameTime, listePieces_Blanches);
           

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (plateau.Initialisation)
            {
                for (i = 0; i <= plateau.Dimension - 1; i++)
                {
                    casesPositions_X = (int)plateau.Position.X + (80 * i);
                    casesPositions_Y = (int)plateau.Position.Y;

                    for (j = 0; j <= plateau.Dimension - 1; j++)
                    {
                        casesPositions_Y = (int)plateau.Position.Y + (80 * j);
                        plateau.IndexesPositions_Map.Add(new int[] { i, j }, new Vector2(casesPositions_X, casesPositions_Y));
                        
                    }
                }

                if (i > plateau.Dimension - 1 && j > plateau.Dimension - 1 && gameInitialisation)
                {
                    plateau.Initialisation = false;
                    gameInitialisation = false;
                    playing = true;
                    foreach (Piece p in listePieces)
                    {
                        p.Update(gameTime, plateau, ref listeCoordonneesAutorisees);
                        
                    }
                }
            }

           

            if (playing && !isBot)
            {
                oldState = state;
                state = Keyboard.GetState();
              
                if (oldState != state && state.IsKeyDown(Keys.Space))
                {
                    foreach (Piece p in listePieces_Blanches)
                    {
                        p.Update(gameTime, plateau, ref listeCoordonneesAutorisees, liste_PiecesCapturees, IndexesObjects_Map);

                        if (p.Moving)
                        {
                            IndexesObjects_Map[p] = p.Coordonnees;
                            Piece pieceMangee = p.liste_PiecesAmanger.Find(x => x.Active == false);
                            if (p.liste_PiecesAmanger.Count > 0 && pieceMangee != null)
                            {
                                pieceMangee.Update(gameTime, plateau, ref listeCoordonneesAutorisees);
                                p.liste_PiecesAmanger.Clear();
                            }
                            
                            p.Moving = false;
                            isBot = true;
                        }
                    }
                }
            }
            else
            {
                //chaud cacao !! 
                bot_IA.Update(gameTime, listePieces_Blanches, listePieces_Noires, IndexesObjects_Map);
                isBot = false;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            plateau.Draw(spriteBatch, listeCoordonneesAutorisees);
            
            foreach(Piece p in listePieces_Blanches)
            {
                p.Draw(spriteBatch);
            }
            foreach(Piece p in listePieces_Noires)
            {
                p.Draw(spriteBatch);
            }
            spriteBatch.End();
            

            base.Draw(gameTime);
        }
    }
}
