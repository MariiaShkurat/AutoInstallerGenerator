using AutoInstallerGeneratorWebAPI.DTO;
using AutoInstallerGeneratorWebAPI.Infrastructure;
using AutoInstallerGeneratorWebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace AutoInstallerGeneratorWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstallersController : ControllerBase
    {
        private readonly InstallerRepository _repo;
        public InstallersController(InstallerRepository repo) => _repo = repo;

        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] InstallerDocumentDTO dto)
        {
            if (dto.File is null || dto.File.Length == 0)
                return BadRequest("File missing");

            var doc = new InstallerDocument
            {
                ProjectName = dto.ProjectName,
                Culture = dto.Culture,
                FileName = dto.File.FileName,
                Log = dto.Log
            };

            await using var stream = dto.File.OpenReadStream();
            var id = await _repo.SaveAsync(stream, dto.File.FileName, doc);

            var url = Url.RouteUrl("DownloadInstaller",
                       new { id }, Request.Scheme, Request.Host.ToUriComponent());

            return Created(url!, new { id, url });
        }

        [HttpGet]
        public Task<List<InstallerDocument>> GetAll() => _repo.GetAllAsync();

        [HttpGet("{id}/file", Name = "DownloadInstaller")]
        public async Task<IActionResult> Download(string id)
        {
            var doc = await _repo.GetAsync(id);
            if (doc is null) return NotFound();

            Response.ContentType = "application/x-msdownload";
            Response.Headers.ContentDisposition = $"attachment; filename=\"{doc.FileName}\"";

            await _repo.DownloadAsync(doc.FileId, Response.Body);
            return new EmptyResult();
        }
    }
}
