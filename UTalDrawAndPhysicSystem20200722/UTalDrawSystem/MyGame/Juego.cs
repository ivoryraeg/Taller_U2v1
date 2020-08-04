﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTalDrawSystem.SistemaDibujado;
using UTalDrawSystem.SistemaFisico;
using UTalDrawSystem.SistemaGameObject;

namespace UTalDrawSystem.MyGame
{
    public class Juego : Escena
    { 
        public Camara camara { private set; get; }

        SpriteFont timer;
        SpriteFont vidasActuales;
        SpriteFont puntajeTotal;

        public Automovil auto;
        List<UTGameObject> listaMuros;
        List<Pelota> listaPelotas;
        List<Agujero> listaAgujeros;
        List<Coleccionable> listaMonedas;

        Random rnd;
        public int n_Choques { private set; get; }
        bool collision_on;
        bool ganoVidas;
        public double time { private set; get; }
        double timeSpawnPelotas;
        double timeSpawnAgujeros;
        double timeSpawnMoneda;
        int posYMoneda;
        int condicionalSpawnMonedas;
        
        

        public Juego(ContentManager content)
        {
            UTGameObjectsManager.Init();

            timer = content.Load<SpriteFont>("Titulo");
            vidasActuales = content.Load<SpriteFont>("Titulo");
            puntajeTotal = content.Load<SpriteFont>("Titulo");


            listaMuros = new List<UTGameObject>();
            listaPelotas = new List<Pelota>();
            listaAgujeros = new List<Agujero>();
            listaMonedas = new List<Coleccionable>();

            auto = new Automovil("Auto", new Vector2(450, Game1.INSTANCE.GraphicsDevice.Viewport.Height), 4, UTGameObject.FF_form.Rectangulo);

            rnd = new Random();
            time = 0;
            timeSpawnPelotas = 0;
            timeSpawnAgujeros = 0;
            timeSpawnMoneda = 0;
            condicionalSpawnMonedas = 0;
            n_Choques = 0;
            ganoVidas = false;

            camara = new Camara(new Vector2(0,0), .5f, 0);
            camara.HacerActiva();

          

        }




        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Game1.INSTANCE.ActiveScene == Game1.Scene.Game)
            {
                time += gameTime.ElapsedGameTime.TotalSeconds;
                timeSpawnPelotas += gameTime.ElapsedGameTime.TotalSeconds;
                timeSpawnAgujeros += gameTime.ElapsedGameTime.TotalSeconds;
                timeSpawnMoneda += gameTime.ElapsedGameTime.TotalSeconds;
            }

            camara.pos.X += 12;

            if (auto.objetoFisico.isColliding && !collision_on)
            {
                collision_on = true;
                n_Choques++;
            }
            else if (!auto.objetoFisico.isColliding)
            {
                collision_on = false;
            }
            
           

            if (camara.pos.X%300 == 0)
            {
                listaMuros.Add(new UTGameObject("Muro", new Vector2(camara.pos.X + 300 + Game1.INSTANCE.GraphicsDevice.Viewport.Width * 2, camara.pos.Y), 1, UTGameObject.FF_form.Rectangulo, true));
                listaMuros.Add(new UTGameObject("Muro", new Vector2(camara.pos.X + 300 + Game1.INSTANCE.GraphicsDevice.Viewport.Width * 2, camara.pos.Y + Game1.INSTANCE.GraphicsDevice.Viewport.Height * 2), 1, UTGameObject.FF_form.Rectangulo, true));

            }

            if (listaMuros.Count > 0)
            {
                if (listaMuros.First<UTGameObject>().objetoFisico.pos.X < camara.pos.X -200)
                {
                    listaMuros.First<UTGameObject>().Destroy();
                    listaMuros.Remove(listaMuros.First<UTGameObject>());
                }
            }

            if (timeSpawnAgujeros > 1f)
            {

                listaAgujeros.Add(new Agujero("Hoyo", new Vector2(camara.pos.X + 600 + Game1.INSTANCE.GraphicsDevice.Viewport.Width * 2, rnd.Next((int)camara.pos.Y + 200, (int)camara.pos.Y - 200 + Game1.INSTANCE.GraphicsDevice.Viewport.Height * 2)), 1, UTGameObject.FF_form.Circulo, false));

                timeSpawnAgujeros = 0;
            }
            if (listaAgujeros.Count > 0)
            {
                if (listaAgujeros.First<Agujero>().objetoFisico.pos.X < camara.pos.X - 200)
                {
                    listaAgujeros.First<Agujero>().Destroy();
                    listaAgujeros.Remove(listaAgujeros.First<Agujero>());
                }
            }

            if (timeSpawnPelotas > 0.3f)
            {
                listaPelotas.Add(new Pelota("obstaculo", new Vector2(camara.pos.X + 600 + Game1.INSTANCE.GraphicsDevice.Viewport.Width * 2, rnd.Next((int)camara.pos.Y + 200, (int)camara.pos.Y - 200 + Game1.INSTANCE.GraphicsDevice.Viewport.Height * 2)), 0.02f, UTGameObject.FF_form.Circulo, false));
                timeSpawnPelotas = 0;
            }
            if (listaPelotas.Count > 0)
            {
                if (listaPelotas.First<Pelota>().objetoFisico.pos.X < camara.pos.X - 200)
                {
                    listaPelotas.First<Pelota>().Destroy();
                    listaPelotas.Remove(listaPelotas.First<Pelota>());
                }
            }

            if (timeSpawnMoneda < 5f)
            {
                posYMoneda = rnd.Next((int)camara.pos.Y + 200, (int)camara.pos.Y - 200 + Game1.INSTANCE.GraphicsDevice.Viewport.Height * 2);
            }
            else
            {
                if (condicionalSpawnMonedas%5 == 0)
                {
                    listaMonedas.Add(new Coleccionable("moneda", new Vector2(camara.pos.X + 600 + Game1.INSTANCE.GraphicsDevice.Viewport.Width * 2, posYMoneda), 0.1f, UTGameObject.FF_form.Circulo, false));
                }
                condicionalSpawnMonedas++;
                if (listaMonedas.Count % 5 == 0)
                {
                    condicionalSpawnMonedas = 0;
                    timeSpawnMoneda = 0;
                }
            }          
                   
            if (listaMonedas.Count > 0)
            {
                if (listaMonedas.First<Coleccionable>().objetoFisico.pos.X < camara.pos.X - 200)
                {
                    listaMonedas.First<Coleccionable>().Destroy();
                    listaMonedas.Remove(listaMonedas.First<Coleccionable>());
                }
            }



            if (auto.objetoFisico.pos.X < camara.pos.X)
            {
                auto.objetoFisico.pos = auto.Respawn();
            }

            //Evita que salga por la parte dercha
            if (auto.objetoFisico.pos.X >= camara.pos.X - 30 + Game1.INSTANCE.GraphicsDevice.Viewport.Width*2)
            {
                auto.objetoFisico.pos = new Vector2(camara.pos.X - 30 + Game1.INSTANCE.GraphicsDevice.Viewport.Width * 2, auto.objetoFisico.pos.Y);
            }

            //Gana vidas cada 25 monedas recogidas (supuestamente)
            if(auto.puntaje > 0 && auto.puntaje%25 == 0)
            {
                if (!ganoVidas)
                {
                    ganoVidas = true;
                    auto.vidas++;
                }
                
            }

            else if(ganoVidas)
            {
                ganoVidas = false;
            }

            //Envia a la pantalla final si se acaban las vidas
            if (auto.vidas <= 0)
            {
                Game1.INSTANCE.ChangeScene(Game1.Scene.End);
            }

        }
        public void Draw (SpriteBatch SB)
        {
            Vector2 timerPos;
            Vector2 vidasPos;
            Vector2 puntajePos;

            timerPos = new Vector2(0,25);
            vidasPos = new Vector2((Game1.INSTANCE.GraphicsDevice.Viewport.Width) - 100,25);
            puntajePos = new Vector2(Game1.INSTANCE.GraphicsDevice.Viewport.Width / 2.5f, 25);

            SB.DrawString(timer, "Tiempo: " + Math.Round(time,2), timerPos, Color.Black);
            SB.DrawString(vidasActuales, "Vidas: " + Game1.INSTANCE.ventanaJuego.auto.vidas, vidasPos, Color.Black);
            SB.DrawString(puntajeTotal, "Monedas recogidas: " + Game1.INSTANCE.ventanaJuego.auto.puntaje, puntajePos, Color.Black);


        }

    }
}
