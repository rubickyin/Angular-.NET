namespace DocumentosAPI.Models;

public class Documento
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string NombreArchivo { get; set; } = string.Empty;
    public string RutaArchivo { get; set; } = string.Empty;
    public string? TipoArchivo { get; set; }
    public long? Tamanio { get; set; }
    public DateTime FechaSubida { get; set; } = DateTime.UtcNow;
    public DateTime? FechaActualizacion { get; set; }
}