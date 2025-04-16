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
        private double width = 0;
        private double height = 0;
        private double length = 0;
        private double density = 0;
        private double specificHeatCapacity = 0;
        private double meltingPoint = 0;
        private double lidSpeed = 0;
        private double lidTemperature = 0;
        private double viscAtZeroShearAndRefTemp = 0;
        private double viscThermCoeff = 0;
        private double ref_temp = 0;
        private double timeConstant = 0;
        private double viscAnomalyFactor = 0;
        private double heatTransferCoefficient = 0;


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
