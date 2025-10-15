using ApiClientes.Data;
using ApiClientes.Models;
using ApiClientes.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

//manejo automatico del mapeo de fechas en sql
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

//Config de inyección de dependencias de la DB
var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");
builder.Services.AddDbContext<ClienteContext>(options =>
    options.UseNpgsql(connectionString));

//Config de CORS
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

app.UseCors();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ClienteContext>();
    dbContext.Database.Migrate();
}


// Endpoints
app.MapGet("/clientes", async (ClienteContext db) =>
    await db.Clientes.ToListAsync());

app.MapGet("/clientes/{id}", async (int id, ClienteContext db) =>
    await db.Clientes.FindAsync(id)
        is Cliente cliente
        ? Results.Ok(cliente)
        : Results.NotFound());

app.MapGet("/clientes/search", async ([FromQuery] string? query, ClienteContext db) =>
{
    if (string.IsNullOrWhiteSpace(query))
    {
        return Results.Ok(await db.Clientes.ToListAsync());
    }

    var clientes = await db.Clientes
        .Where(c => 
            c.Nombres.ToLower().Contains(query.ToLower()) || 
            c.Apellidos.ToLower().Contains(query.ToLower()))
        .ToListAsync();

    return Results.Ok(clientes);
});


app.MapPost("/clientes", async (Cliente cliente, ClienteContext db) =>
{

     var validationErrors = ClienteValidator.Validate(cliente);
    if (validationErrors != null)
    {
        return Results.BadRequest(new { Errors = validationErrors });
    }
    
    if (await db.Clientes.AnyAsync(c => c.CUIT == cliente.CUIT))
    {
        return Results.Conflict(new { Errors = $"El CUIT {cliente.CUIT} ya está registrado." });
    }

    cliente.FechaRegistro = DateTime.UtcNow;
    db.Clientes.Add(cliente);
    await db.SaveChangesAsync();

    return Results.Created($"/clientes/{cliente.Id}", cliente);
});


app.MapPut("/clientes/{id}", async (int id, Cliente inputCliente, ClienteContext db) =>
{
    var validationErrors = ClienteValidator.Validate(inputCliente);
    if (validationErrors != null)
    {
        return Results.BadRequest(new { Errors = validationErrors });
    }
    
    var clienteExistente = await db.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

    if (clienteExistente is null) return Results.NotFound($"Cliente con ID {id} no encontrado para actualizar.");
    
    if (await db.Clientes.AnyAsync(c => c.CUIT == inputCliente.CUIT && c.Id != id))
    {
        return Results.Conflict(new { Errors = $"El CUIT {inputCliente.CUIT} ya está registrado por otro cliente." });
    }
    
    inputCliente.Id = id; 
    inputCliente.FechaRegistro = clienteExistente.FechaRegistro;
    db.Clientes.Update(inputCliente);
    await db.SaveChangesAsync();

    return Results.NoContent();
});


app.MapDelete("/clientes/{id}", async (int id, ClienteContext db) =>
{
    var cliente = await db.Clientes.FindAsync(id);
    
    if (cliente is null) return Results.NotFound();

    db.Clientes.Remove(cliente);
    await db.SaveChangesAsync();

    return Results.NoContent();
});


app.Run();