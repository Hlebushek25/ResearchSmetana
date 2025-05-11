using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssleduemSmetanu
{
    internal class InteractionDB
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

        public static List<MaterialCharacteristicValue> GetAllMaterialCharacteristicsValues()
        {
            List<MaterialCharacteristicValue> characteristics = new List<MaterialCharacteristicValue>();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbPath}"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                                        SELECT 
                                            vcm.id,
                                            vcm.id_material,
                                            m.name_material,
                                            vcm.id_characteristic,
                                            ch.name_characteristic,
                                            vcm.value_characteristic
                                        FROM 
                                            value_characteristic_material vcm
                                        JOIN 
                                            material m ON vcm.id_material = m.id_material
                                        JOIN 
                                            characteristic_material ch ON vcm.id_characteristic = ch.id_characteristic";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        characteristics.Add(new MaterialCharacteristicValue
                        {
                            id = reader.GetInt32(0),
                            idMaterial = reader.GetInt32(1),
                            nameMaterial = reader.GetString(2),
                            idCharacteristic = reader.GetInt32(3),
                            nameCharacteristic = reader.GetString(4),
                            value = reader.GetDouble(5)
                        });
                    }
                }
            }
            return characteristics;
        }

        public static List<MaterialCharacteristicValue> GetMaterialCharacteristicValues(int materialId)
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

        public static List<EmpericalCoefValue> GetAllEmpericalCoefValues()
        {
            List<EmpericalCoefValue> coefs = new List<EmpericalCoefValue>();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbPath}"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                                        SELECT 
                                            vec.id,
                                            vec.id_material,
                                            m.name_material,
                                            vec.id_empirical_coef,
                                            ec.name_empirical_coef,
                                            vec.value_empirical_coef
                                        FROM 
                                            value_empirical_coef vec
                                        JOIN 
                                            material m ON vec.id_material = m.id_material
                                        JOIN 
                                            empirical_coef ec ON vec.id_empirical_coef = ec.id_empirical_coef";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        coefs.Add(new EmpericalCoefValue
                        {
                            id = reader.GetInt32(0),
                            idMaterial = reader.GetInt32(1),
                            nameMaterial = reader.GetString(2),
                            idEmpericalCoef = reader.GetInt32(3),
                            nameEmpericalCoef = reader.GetString(4),
                            value = reader.GetDouble(5)
                        });
                    }
                }
            }
            return coefs;
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
}
