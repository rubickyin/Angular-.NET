using DocumentosAPI.Data;
using DocumentosAPI.DTOs;
using DocumentosAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocumentosAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly string _uploadsPath;

    public DocumentosController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _uploadsPath = Path.Combine(env.ContentRootPath, "Uploads");
        Directory.CreateDirectory(_uploadsPath); // crea la carpeta si no existe
    }

    // ─── GET TODOS ───────────────────────────────────────────
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DocumentoDto>>> GetAll()
    {
        var docs = await _context.Documentos
            .OrderByDescending(d => d.FechaSubida)
            .Select(d => new DocumentoDto
            {
                Id            = d.Id,
                Nombre        = d.Nombre,
                Descripcion   = d.Descripcion,
                NombreArchivo = d.NombreArchivo,
                TipoArchivo   = d.TipoArchivo,
                Tamanio       = d.Tamanio,
                FechaSubida   = d.FechaSubida
            })
            .ToListAsync();

        return Ok(docs);
    }

    // ─── GET POR ID ──────────────────────────────────────────
    [HttpGet("{id}")]
    public async Task<ActionResult<DocumentoDto>> GetById(int id)
    {
        var doc = await _context.Documentos.FindAsync(id);
        if (doc == null) return NotFound(new { mensaje = "Documento no encontrado" });

        return Ok(new DocumentoDto
        {
            Id            = doc.Id,
            Nombre        = doc.Nombre,
            Descripcion   = doc.Descripcion,
            NombreArchivo = doc.NombreArchivo,
            TipoArchivo   = doc.TipoArchivo,
            Tamanio       = doc.Tamanio,
            FechaSubida   = doc.FechaSubida
        });
    }

    // ─── DESCARGAR ARCHIVO ───────────────────────────────────
    [HttpGet("{id}/descargar")]
    public async Task<IActionResult> Descargar(int id)
    {
        var doc = await _context.Documentos.FindAsync(id);
        if (doc == null) return NotFound(new { mensaje = "Documento no encontrado" });

        var filePath = Path.Combine(_uploadsPath, doc.RutaArchivo);
        if (!System.IO.File.Exists(filePath))
            return NotFound(new { mensaje = "Archivo físico no encontrado en el servidor" });

        var memory = new MemoryStream();
        await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        await stream.CopyToAsync(memory);
        memory.Position = 0;

        return File(memory, doc.TipoArchivo ?? "application/octet-stream", doc.NombreArchivo);
    }

    // ─── CREAR ───────────────────────────────────────────────
    [HttpPost]
    public async Task<ActionResult<DocumentoDto>> Create([FromForm] CrearDocumentoDto dto)
    {
        if (dto.Archivo == null || dto.Archivo.Length == 0)
            return BadRequest(new { mensaje = "Debes adjuntar un archivo" });

        // Nombre único para evitar colisiones
        var nombreUnico  = $"{Guid.NewGuid()}_{Path.GetFileName(dto.Archivo.FileName)}";
        var rutaCompleta = Path.Combine(_uploadsPath, nombreUnico);

        await using var stream = new FileStream(rutaCompleta, FileMode.Create);
        await dto.Archivo.CopyToAsync(stream);

        var doc = new Documento
        {
            Nombre        = dto.Nombre,
            Descripcion   = dto.Descripcion,
            NombreArchivo = dto.Archivo.FileName,
            RutaArchivo   = nombreUnico,
            TipoArchivo   = dto.Archivo.ContentType,
            Tamanio       = dto.Archivo.Length,
            FechaSubida   = DateTime.UtcNow
        };

        _context.Documentos.Add(doc);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = doc.Id }, new DocumentoDto
        {
            Id            = doc.Id,
            Nombre        = doc.Nombre,
            Descripcion   = doc.Descripcion,
            NombreArchivo = doc.NombreArchivo,
            TipoArchivo   = doc.TipoArchivo,
            Tamanio       = doc.Tamanio,
            FechaSubida   = doc.FechaSubida
        });
    }

    // ─── ACTUALIZAR ──────────────────────────────────────────
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromForm] ActualizarDocumentoDto dto)
    {
        var doc = await _context.Documentos.FindAsync(id);
        if (doc == null) return NotFound(new { mensaje = "Documento no encontrado" });

        doc.Nombre              = dto.Nombre;
        doc.Descripcion         = dto.Descripcion;
        doc.FechaActualizacion  = DateTime.UtcNow;

        // Si manda nuevo archivo, reemplazamos el anterior
        if (dto.Archivo != null && dto.Archivo.Length > 0)
        {
            // Eliminar archivo anterior del disco
            var rutaAnterior = Path.Combine(_uploadsPath, doc.RutaArchivo);
            if (System.IO.File.Exists(rutaAnterior))
                System.IO.File.Delete(rutaAnterior);

            // Guardar nuevo archivo
            var nombreUnico  = $"{Guid.NewGuid()}_{Path.GetFileName(dto.Archivo.FileName)}";
            var rutaCompleta = Path.Combine(_uploadsPath, nombreUnico);

            await using var stream = new FileStream(rutaCompleta, FileMode.Create);
            await dto.Archivo.CopyToAsync(stream);

            doc.NombreArchivo = dto.Archivo.FileName;
            doc.RutaArchivo   = nombreUnico;
            doc.TipoArchivo   = dto.Archivo.ContentType;
            doc.Tamanio       = dto.Archivo.Length;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // ─── ELIMINAR ────────────────────────────────────────────
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var doc = await _context.Documentos.FindAsync(id);
        if (doc == null) return NotFound(new { mensaje = "Documento no encontrado" });

        // Eliminar archivo físico del disco
        var rutaArchivo = Path.Combine(_uploadsPath, doc.RutaArchivo);
        if (System.IO.File.Exists(rutaArchivo))
            System.IO.File.Delete(rutaArchivo);

        _context.Documentos.Remove(doc);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}