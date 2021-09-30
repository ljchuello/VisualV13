namespace VisualV13.Libreria
{
    public class oResultado
    {
        public string Mensaje { get; set; } = string.Empty;

        public bool Success { get; set; } = false;

        public dynamic Resultado { get; set; } = null;

        public oResultado Exitoso()
        {
            oResultado oResultado = new oResultado()
            {
                Success = true,
                Mensaje = string.Empty
            };
            return oResultado;
        }

        public oResultado Exitoso(dynamic obj)
        {
            oResultado oResultado = new oResultado()
            {
                Success = true,
                Mensaje = string.Empty,
                Resultado = obj
            };
            return oResultado;
        }

        public oResultado Fallido(string msj)
        {
            return new oResultado()
            {
                Success = false,
                Mensaje = msj
            };
        }
    }
}
