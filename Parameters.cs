using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssleduemSmetanu
{
    public class Material
    {
        public int idMaterial { get; set; }
        public string nameMaterial { get; set; }
    }

    public class MaterialCharacteristicValue
    {
        public int id { get; set; }
        public int idMaterial { get; set; }
        public string nameMaterial { get; set; }
        public int idCharacteristic { get; set; }
        public string nameCharacteristic { get; set; }
        public double value { get; set; }
    }
    public class EmpericalCoefValue
    {
        public int id { get; set; }
        public int idMaterial { get; set; }
        public string nameMaterial { get; set; }
        public int idEmpericalCoef { get; set; }
        public string nameEmpericalCoef { get; set; }
        public double value { get; set; }
    }
    public class User
    {
        public int idUser { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public string role { get; set; }
    }

    public class Role
    {
        public int idRole { get; set; }
        public string nameRole { get; set; }
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
