namespace DocumentosAPI.DTOs;

// Lo que devolvemos al frontend
public class DocumentoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string NombreArchivo { get; set; } = string.Empty;
    public string? TipoArchivo { get; set; }
    public long? Tamanio { get; set; }
    public DateTime FechaSubida { get; set; }
}

// Lo que recibimos al CREAR
public class CrearDocumentoDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public IFormFile Archivo { get; set; } = null!;
}

// Lo que recibimos al EDITAR
public class ActualizarDocumentoDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public IFormFile? Archivo { get; set; } // opcional al editar
}