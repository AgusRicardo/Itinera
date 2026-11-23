namespace Itinera.Domain.Propuestas;

public sealed record Estadia(Destino Destino, DateOnly FechaLlegada, DateOnly FechaPartida);
