using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;


namespace AutosNoSql
{

    public class Conexion
    {


    }

    // Clase para representar un auto
    public class Auto
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Placa { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int Anio { get; set; }
        public string Color { get; set; }
        public decimal PrecioPorDia { get; set; }


        public byte[] Foto { get; set; }
        public bool Rentado { get; set; }
        
    }



}
