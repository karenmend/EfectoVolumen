﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace Reproductor
{
    class Delay : ISampleProvider

    {
        private ISampleProvider fuente;
        public int offsetMilisegundos { get; set; }

        private List<float> bufferDelay = new List<float>();
        private int cantidadMuestrasOffset;

        private int duracionBufferSegundos;
        private int tamañoBuffer;
        private int cantidadMuestrasTranscurridas = 0;
        private int cantidadMuestrasBorradas = 0;

        private bool activo;

        public WaveFormat WaveFormat
        {
            get
            {
                return fuente.WaveFormat;

            }
        }

        public Delay(ISampleProvider fuente)
        {

            this.fuente = fuente;
            offsetMilisegundos = 500;
            cantidadMuestrasOffset = (int)(((float)offsetMilisegundos / 1000.0f) * (float)fuente.WaveFormat.SampleRate);
            duracionBufferSegundos = 10;
            tamañoBuffer = fuente.WaveFormat.SampleRate * duracionBufferSegundos;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            //Leemos las muestras de las señal fuente
            var read = fuente.Read(buffer, offset, count);
            //Calcular los tiempos transcurridos
            float tiempoTranscurridoSgundos = (float)cantidadMuestrasTranscurridas / (float)fuente.WaveFormat.SampleRate;
            float milisegundosTranscurridos = tiempoTranscurridoSgundos * 1000.0f; 
            //Llenando el buffer
            for(int i = 0; i < read; i++)
            {
                bufferDelay.Add(buffer[i + offset]);
            }
            //Eliminar excedentes del buffer
            if (bufferDelay.Count > tamañoBuffer)
            {
                int diferencia = bufferDelay.Count - tamañoBuffer;
                bufferDelay.RemoveRange(0, diferencia);
                cantidadMuestrasBorradas += diferencia;
            }
            //Aplicar el efecto
            if(milisegundosTranscurridos > offsetMilisegundos)
            {
                for(int i = 0; i<read; i++)
                {
                    buffer[offset + i] += bufferDelay[cantidadMuestrasTranscurridas - cantidadMuestrasBorradas + i - cantidadMuestrasOffset];
                }
            }



            cantidadMuestrasTranscurridas += read;
            return read;
        }
    }
}