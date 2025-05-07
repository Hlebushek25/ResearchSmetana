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

        private static string dbpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Databases", "DB.db");

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

            List<MaterialCharacteristicValue> characteristics = LoadDB.GetMaterialCharacteristicsValues(materialId);
            List<EmpericalCoefValue> empericalCoefs = LoadDB.GetEmpericalCoefValues(materialId);
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

    public class LoadDB
    {
        private static string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Databases", "DB.db");
        private static string dbPathUser = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Databases", "Users.db");

        public static List<User> GetAllUsers()
        {
            var users = new List<User>();

            using (var connection = new SQLiteConnection($"Data Source={dbPathUser}"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                                        SELECT u.id_user, u.login, u.pass, r.name_role 
                                        FROM user u
                                        JOIN role r ON u.role = r.id_role";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            idUser = reader.GetInt32(0),
                            login = reader.GetString(1),
                            password = reader.GetString(2),
                            role = reader.GetString(3)
                        });
                    }
                }
            }
            return users;
        }
        public static List<Material> GetAllMaterials()
        {
            List<Material> users = new List<Material>();

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
                            idMaterial = reader.GetInt32(0),
                            nameMaterial = reader.GetString(1),
                        });
                    }
                }
            }

            return users;
        }
        public static List<MaterialCharacteristic> GetAllMaterialCharacteristics()
        {
            List<MaterialCharacteristic> characteristics = new List<MaterialCharacteristic>();
            using (var connection = new SQLiteConnection($"Data Source={dbPath}"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT id_characteristic, name_characteristic, unit FROM characteristic_material";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        characteristics.Add(new MaterialCharacteristic
                        {
                            id = reader.GetInt32(0),
                            name = reader.GetString(1),
                            unit = reader.GetString(2)
                        });
                    }
                }
            }

            return characteristics;
        }

        public static List<EmpericalCoef> GetAllEmpericalCoef()
        {
            List<EmpericalCoef> coefs = new List<EmpericalCoef>();
            using (var connection = new SQLiteConnection($"Data Source={dbPath}"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT id_empirical_coef, name_empirical_coef, unit FROM empirical_coef";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        coefs.Add(new EmpericalCoef
                        {
                            id = reader.GetInt32(0),
                            name = reader.GetString(1),
                            unit = reader.GetString(2)
                        });
                    }
                }
            }

            return coefs;
        }

        public static List<MaterialCharacteristicValue> GetMaterialCharacteristicsValues(int materialId)
        {
            List<MaterialCharacteristicValue> characteristics = new List<MaterialCharacteristicValue>();

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
                        characteristics.Add(new MaterialCharacteristicValue
                        {
                            id = reader.GetInt32(0),
                            idMaterial = reader.GetInt32(1),
                            idCharacteristic = reader.GetInt32(2),
                            value = reader.GetDouble(3)
                        });
                    }
                }
            }

            return characteristics;
        }

        public static List<EmpericalCoefValue> GetEmpericalCoefValues(int materialId)
        {
            List<EmpericalCoefValue> empCoef = new List<EmpericalCoefValue>();

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
                        empCoef.Add(new EmpericalCoefValue
                        {
                            id = reader.GetInt32(0),
                            idMaterial = reader.GetInt32(1),
                            idEmpericalCoef = reader.GetInt32(2),
                            value = reader.GetDouble(3)
                        });
                    }
                }
            }

            return empCoef;
        }
    }

    public class Material
    {
        public int idMaterial { get; set; }
        public string nameMaterial { get; set; }
    }

    public class MaterialCharacteristicValue
    {
        public int id { get; set; }
        public int idMaterial { get; set; }
        public int idCharacteristic { get; set; }
        public double value { get; set; }
    }
    public class EmpericalCoefValue
    {
        public int id { get; set; }
        public int idMaterial { get; set; }
        public int idEmpericalCoef { get; set; }
        public double value { get; set; }
    }
    public class User
    {
        public int idUser { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public string role { get; set; }
    }

    public class EmpericalCoef
    {
        public int id { get; set; }
        public string name { get; set; }
        public string unit { get; set; }
    }

    public class MaterialCharacteristic
    {
        public int id { get; set; }
        public string name { get; set; }
        public string unit { get; set; }
    }


}
