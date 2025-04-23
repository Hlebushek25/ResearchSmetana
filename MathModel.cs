using System;
using System.Collections.Generic;
using System.Data.SQLite;
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
        public double ref_temp { get; set; }                   //Температура приведения
        public double timeConstant { get; set; }               //Постоянная времени
        public double viscAnomalyFactor { get; set; }          //Показатель аномалии вязкости материала
        public double heatTransferCoefficient { get; set; }    //Коэффициент теплоотдачи от крышки канала к материалу
        public double step { get; set; }                       //Шаг


        public MathModel(double width, double height, double length, double density, double specificHeatCapacity, double meltingPoint, double lidSpeed, double lidTemperature,
                         double viscAtZeroShearAndRefTemp, double viscThermCoeff, double ref_temp, double timeConstant, double viscAnomalyFactor, double heatTransferCoefficient, double step)
        {
            this.width = width;
            this.height = height;
            this.length = length;
            this.density = density;
            this.specificHeatCapacity = specificHeatCapacity;
            this.meltingPoint = meltingPoint;
            this.lidSpeed = lidSpeed;
            this.lidTemperature = lidTemperature;
            this.viscAtZeroShearAndRefTemp = viscAtZeroShearAndRefTemp;
            this.viscThermCoeff = viscThermCoeff;
            this.ref_temp = ref_temp;
            this.timeConstant = timeConstant;
            this.viscAnomalyFactor = viscAnomalyFactor;
            this.heatTransferCoefficient = heatTransferCoefficient;
            this.step = step;
        }

        public double CalculatePerformance(/*double width, double height, double lidSpeed*/)
        {
            double performance = 3600 * density * (((height * width * lidSpeed) / 2) * ((0.125 * Math.Pow(height / width, 2)) - (0.625 * (height / width)) + 1));
            return Math.Round(performance);
        }

        public double[,] CalculateTemperature()
        {
            double Qch = (((height * width * lidSpeed) / 2) * ((0.125 * Math.Pow(height / width, 2)) - (0.625 * (height / width)) + 1));

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

            double qAlpha = width * heatTransferCoefficient * ((1/viscThermCoeff) - lidTemperature + ref_temp);

            int N = (int)Math.Round(length / step);
            
            double[,] combinedArray = new double[3, N+1];

            for (int i = 0; i <= N; i++)
            {
                combinedArray[0, i] = i * step;

                double temperature = ref_temp + ((1 / viscThermCoeff) * Math.Log((((viscThermCoeff * qGamma) + (width * heatTransferCoefficient)) / (viscThermCoeff * qAlpha)) * 
                       (1 - Math.Exp((-viscThermCoeff * qAlpha) / (density * specificHeatCapacity * Qch) * combinedArray[0, i])) + 
                       Math.Exp(viscThermCoeff * (meltingPoint - ref_temp - (qAlpha / (density * specificHeatCapacity * Qch) * combinedArray[0, i])))));
                combinedArray[1, i] = Math.Round(temperature, 2);

                double viscosity = viscAtZeroShearAndRefTemp * Math.Exp(-viscThermCoeff * (temperature - ref_temp)) * part7;
                combinedArray[2, i] = Math.Round(viscosity);
            }

            return combinedArray;
        }

        
    }

    public class LoadDB
    {
        private static string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Databases", "DB.db");
        public static List<Material> GetAllMaterials(string dbPath)
        {
            var users = new List<Material>();

            using (var connection = new SQLiteConnection($"Data Source={dbPath}"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT id_material, name_material FROM material";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new Material
                        {
                            IdMaterial = reader.GetInt32(0),
                            NameMaterial = reader.GetString(1),
                        });
                    }
                }
            }

            return users;
        }

        public static List<MaterialCharacteristic> GetMaterialCharacteristics(string dbPath, int materialId)
        {
            List<MaterialCharacteristic> characteristics = new List<MaterialCharacteristic>();

            using (var connection = new SQLiteConnection($"Data Source={dbPath}"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT id, id_material, id_characteristic, value_characteristic FROM value_characteristic_material WHERE id_material = @materialId";
                command.Parameters.AddWithValue("@materialId", materialId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        characteristics.Add(new MaterialCharacteristic
                        {
                            Id = reader.GetInt32(0),
                            IdMaterial = reader.GetInt32(1),
                            IdCharacteristic = reader.GetInt32(2),
                            ValueCharacteristic = reader.GetDouble(3)
                        });
                    }
                }
            }

            return characteristics;
        }

        public static List<MaterialEmpericalCoef> GetEmpericalCoef(string dbPath, int materialId)
        {
            List<MaterialEmpericalCoef> empCoef = new List<MaterialEmpericalCoef>();

            using (var connection = new SQLiteConnection($"Data Source={dbPath}"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT id, id_material, id_empirical_coef, value_empirical_coef FROM value_empirical_coef WHERE id_material = @materialId";
                command.Parameters.AddWithValue("@materialId", materialId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        empCoef.Add(new MaterialEmpericalCoef
                        {
                            Id = reader.GetInt32(0),
                            IdMaterial = reader.GetInt32(1),
                            IdEmpericalCoef = reader.GetInt32(2),
                            ValueEmpericalCoef = reader.GetDouble(3)
                        });
                    }
                }
            }

            return empCoef;
        }
    }

    public class Material
    {
        public int IdMaterial { get; set; }
        public string NameMaterial { get; set; }
    }

    public class MaterialCharacteristic
    {
        public int Id { get; set; }
        public int IdMaterial { get; set; }
        public int IdCharacteristic { get; set; }
        public double ValueCharacteristic { get; set; }
    }
    public class MaterialEmpericalCoef
    {
        public int Id { get; set; }
        public int IdMaterial { get; set; }
        public int IdEmpericalCoef { get; set; }
        public double ValueEmpericalCoef { get; set; }
    }


}
