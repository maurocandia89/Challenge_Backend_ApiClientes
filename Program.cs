using ApiClientes.Data;
using ApiClientes.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de la inyección de dependencias de la DB
var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");
builder.Services.AddDbContext<ClienteContext>(options =>
    options.UseNpgsql(connectionString));

// 2. Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

// Usar la política de CORS
app.UseCors();

// Middleware: Aplicar migraciones al iniciar (solo para desarrollo/prototipado simple)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ClienteContext>();
    dbContext.Database.Migrate();
}


// 3. Definición de Endpoints (ABM con EF Core)

// GET: /clientes
app.MapGet("/clientes", async (ClienteContext db) =>
    await db.Clientes.ToListAsync());

// GET: /clientes/{id}
app.MapGet("/clientes/{id}", async (int id, ClienteContext db) =>
    await db.Clientes.FindAsync(id)
        is Cliente cliente
        ? Results.Ok(cliente)
        : Results.NotFound());

// POST: /clientes
app.MapPost("/clientes", async (Cliente cliente, ClienteContext db) =>
{
    cliente.FechaRegistro = DateTime.Now; // Establecer la fecha de registro
    db.Clientes.Add(cliente);
    await db.SaveChangesAsync();

    return Results.Created($"/clientes/{cliente.Id}", cliente);
});

// PUT: /clientes/{id}
app.MapPut("/clientes/{id}", async (int id, Cliente inputCliente, ClienteContext db) =>
{
    var cliente = await db.Clientes.FindAsync(id);

    if (cliente is null) return Results.NotFound();
    
    // Actualizar solo los campos permitidos
    cliente.Nombres = inputCliente.Nombres;
    cliente.Apellidos = inputCliente.Apellidos;
    cliente.FechaNacimiento = inputCliente.FechaNacimiento;
    cliente.CUIT = inputCliente.CUIT;
    cliente.Domicilio = inputCliente.Domicilio;
    cliente.TelefonoCelular = inputCliente.TelefonoCelular;
    cliente.Email = inputCliente.Email;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

// DELETE: /clientes/{id}
app.MapDelete("/clientes/{id}", async (int id, ClienteContext db) =>
{
    var cliente = await db.Clientes.FindAsync(id);
    
    if (cliente is null) return Results.NotFound();

    db.Clientes.Remove(cliente);
    await db.SaveChangesAsync();

    return Results.NoContent();
});


app.Run();