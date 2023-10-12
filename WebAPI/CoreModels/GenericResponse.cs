using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels
{
    public class GenericResponse
    {
        public bool status { get; set; }
        public Object? data { get; set; }
        public Error? error { get; set; }
    }
    public class Error
    {
        public string? code { get; set; }
        public string? description { get; set; }
    }
}
