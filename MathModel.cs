using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssleduemSmetanu
{
    internal class MathModel
    {
        public double width { get; set; }                      //Ширина
        public double height { get; set; }                     //Глубина
        public double length { get; set; }                     //Длина
        public double density { get; set; }                    //Плотность
        public double specificHeatCapacity { get; set; }       //Удельная теплоемкость
        public double meltingPoint { get; set; }               //Температура плавления
        public double lidSpeed { get; set; }                   //Скорость крышки
        public double lidTemperature { get; set; }             //Температура крышки
        public double viscAtZeroShearAndRefTemp { get; set; }  //Вязкость материала при нулевой скорости деформации сдвига и температуре приведения
        public double viscThermCoeff { get; set; }             //Температурный коэффициент вязкости материала
        public double castingTemp { get; set; }                //Температура приведения
        public double timeConstant { get; set; }               //Постоянная времени
        public double viscAnomalyFactor { get; set; }          //Показатель аномалии вязкости материала
        public double heatTransferCoefficient { get; set; }    //Коэффициент теплоотдачи от крышки канала к материалу
        public double step { get; set; }                       //Шаг

        public double QCH { get; set; }

        private static string dbpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Databases", "Smetana.db");

        public MathModel() 
        {
            width = 0.23;
            height = 0.015;       
            length = 7.5;        
            lidSpeed = 1.1;      
            lidTemperature = 180; 
            step= 0.005;         
        }

        public void LoadFromDatabase(int materialId)
        {
            density = 0;
            specificHeatCapacity = 0;
            meltingPoint = 0;
            viscAtZeroShearAndRefTemp = 0;
            viscThermCoeff = 0;
            castingTemp = 0;
            timeConstant = 0;
            viscAnomalyFactor = 0;
            heatTransferCoefficient = 0;

            List<MaterialCharacteristicValue> characteristics = InteractionDB.GetMaterialCharacteristicValues(materialId);
            List<EmpericalCoefValue> empericalCoefs = InteractionDB.GetEmpericalCoefValues(materialId);
            foreach (MaterialCharacteristicValue characteristic in characteristics)
            {
                switch (characteristic.idCharacteristic) 
                {
                    case 1: // Плотность
                        this.density = characteristic.value;
                        break;
                    case 2: // Удельная теплоемкость
                        this.specificHeatCapacity = characteristic.value;
                        break;
                    case 3: // Температура плавления
                        this.meltingPoint = characteristic.value;
                        break;
                }
            }
            foreach (EmpericalCoefValue empericalCoef in empericalCoefs)
            {
                switch (empericalCoef.idEmpericalCoef)
                {
                    case 1: // Вязкость материала при нулевой скорости деформации сдвига и температуре приведения
                        this.viscAtZeroShearAndRefTemp = empericalCoef.value;
                        break;
                    case 2: // Температурный коэффициент вязкости материала
                        this.viscThermCoeff = empericalCoef.value;
                        break;
                    case 3:// Температура приведения
                        this.castingTemp = empericalCoef.value;
                        break;
                    case 4: // Постоянная времени
                        this.timeConstant = empericalCoef.value;
                        break;
                    case 5: // Показатель аномалии вязкости материала
                        this.viscAnomalyFactor = empericalCoef.value;
                        break;
                    case 6: // Коэффициент теплоотдачи от крышки канала к материалу
                        this.heatTransferCoefficient = empericalCoef.value;
                        break;
                }
            }
        }

        public (double Result, long ElapsedTicks) CalculatePerformance()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            QCH = (((height * width * lidSpeed) / 2) * ((0.125 * Math.Pow(height / width, 2)) - (0.625 * (height / width)) + 1));
            double performance = 3600 * density * QCH;
            stopwatch.Stop();

            return (Math.Round(performance), stopwatch.ElapsedTicks);
        }

        public (double[,] Result, long ElapsedTicks) CalculateTemperature()
        {
            var stopwatch = Stopwatch.StartNew();
            //double Qch = (((height * width * lidSpeed) / 2) * ((0.125 * Math.Pow(height / width, 2)) - (0.625 * (height / width)) + 1));

            double gamma = lidSpeed / height;
            //double qGamma = height * width * viscAtZeroShearAndRefTemp * Math.Pow(gamma, 2) * (Math.Pow(1 + Math.Pow(timeConstant * gamma, 2), (viscAnomalyFactor - 1) / 2));
            double part1 = height * width * viscAtZeroShearAndRefTemp;
            double part2 = Math.Pow(gamma, 2);
            double part3 = timeConstant * gamma;
            double part4 = Math.Pow(part3, 2);
            double part5 = 1 + part4;
            double part6 = (viscAnomalyFactor - 1.0) / 2.0;
            double part7 = Math.Pow(part5, part6);
            double qGamma = part1 * part2 * part7;

            double qAlpha = width * heatTransferCoefficient * ((1/viscThermCoeff) - lidTemperature + castingTemp);

            int N = (int)Math.Round(length / step);
            
            double[,] combinedArray = new double[3, N+1];

            for (int i = 0; i <= N; i++)
            {
                combinedArray[0, i] = i * step;

                double temperature = castingTemp + ((1 / viscThermCoeff) * Math.Log((((viscThermCoeff * qGamma) + (width * heatTransferCoefficient)) / (viscThermCoeff * qAlpha)) * 
                       (1 - Math.Exp((-viscThermCoeff * qAlpha) / (density * specificHeatCapacity * QCH) * combinedArray[0, i])) + 
                       Math.Exp(viscThermCoeff * (meltingPoint - castingTemp - (qAlpha / (density * specificHeatCapacity * QCH) * combinedArray[0, i])))));
                combinedArray[1, i] = Math.Round(temperature, 2);

                double viscosity = viscAtZeroShearAndRefTemp * Math.Exp(-viscThermCoeff * (temperature - castingTemp)) * part7;
                combinedArray[2, i] = Math.Round(viscosity);
            }
            stopwatch.Stop();
            return (combinedArray, stopwatch.ElapsedTicks);
        }
    }
}
