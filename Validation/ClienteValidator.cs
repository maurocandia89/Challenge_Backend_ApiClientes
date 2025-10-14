using ApiClientes.Models;
using System.Text.RegularExpressions;

namespace ApiClientes.Validation;

    public static class ClienteValidator
    {
        public static string Validate(Cliente cliente)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(cliente.Nombres)) errors.Add("Nombres es obligatorio.");
            if (string.IsNullOrWhiteSpace(cliente.Apellidos)) errors.Add("Apellidos es obligatorio.");
            if (string.IsNullOrWhiteSpace(cliente.CUIT)) errors.Add("CUIT es obligatorio.");
            if (string.IsNullOrWhiteSpace(cliente.TelefonoCelular)) errors.Add("Teléfono Celular es obligatorio.");
            if (string.IsNullOrWhiteSpace(cliente.Email)) errors.Add("Email es obligatorio.");

            if (!Regex.IsMatch(cliente.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) errors.Add("Formato de Email inválido.");

            if (!Regex.IsMatch(cliente.CUIT, @"^\d{2}-\d{8}-\d{1}$")) errors.Add("Formato de CUIT inválido (debe ser XX-XXXXXXXX-X).");

            if (cliente.FechaNacimiento > DateOnly.FromDateTime(DateTime.Today)) errors.Add("Fecha de Nacimiento no puede ser futura.");

            return errors.Any() ? string.Join(" | ", errors) : null;
        }
    }
