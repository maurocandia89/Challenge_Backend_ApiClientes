namespace ApiClientes.Models;

    public class Cliente
    {
        public int Id { get; set; }
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public DateOnly FechaNacimiento { get; set; } // Uso DateOnly para solo la fecha
        public string CUIT { get; set; } = string.Empty;
        public string Domicilio { get; set; } = string.Empty;
        public string TelefonoCelular { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; } // Para auditoría
    }
