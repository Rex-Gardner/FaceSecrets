using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using API.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Secrets;
using Models.Secrets.Repositories;
using Converter = ModelConverters.Secrets;

namespace API.Controllers
{
    [Route("api/v1/images")]
    public class ImagesController : ControllerBase
    {
        private readonly IHostingEnvironment hostingEnvironment;
        private const string ImageDirectory = "/images/";
        private const string Target = "Image";
        private readonly ISecretRepository repository;

        public ImagesController(IHostingEnvironment hostingEnvironment, ISecretRepository repository)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        
        /// <summary>
        /// Uploads image
        /// </summary>
        /// <param name="imageFile">Image file</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Created image</response>
        /// <response code="400">Invalid image or image is empty</response>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UploadImageAsync(IFormFile imageFile, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (imageFile == null)
            {
                var error = ErrorResponsesService.BodyIsMissing(nameof(imageFile));
                return BadRequest(error);
            }

            var imageId = Guid.NewGuid();
            var imageName = imageId.ToString();
            var extension = Path.GetExtension(imageFile.FileName);
            var path = $"{ImageDirectory}{imageName}{extension}";
            var stream = imageFile.OpenReadStream();

            ClientModels.Secrets.Secret clientSecret;
            
            if (ImageValidationService.IsValidLength(imageFile.Length) && ImageValidationService.IsImage(stream))
            {
                var webDirectoryPath = $"{hostingEnvironment.WebRootPath}{ImageDirectory}";

                if (!Directory.Exists(webDirectoryPath))
                {
                    Directory.CreateDirectory(webDirectoryPath);
                }
                
                using (var fileStream = new FileStream($"{hostingEnvironment.WebRootPath}{path}", FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);
                    var creationInfo = new SecretCreationInfo(path, string.Empty);
                    var modelSecret = await repository.CreateAsync(creationInfo, cancellationToken).ConfigureAwait(false);
                    clientSecret = Converter.SecretConverter.Convert(modelSecret);
                }
            }
            else
            {
                var error = ErrorResponsesService.InvalidImageData(nameof(imageFile));
                return BadRequest(error);
            }

            return Ok(clientSecret);
        }
        
        /// <summary>
        /// Deletes image
        /// </summary>
        /// <param name="imageName">Image name</param>
        /// <param name="cancellationToken"></param>
        /// <response code="204">Image deleted</response>
        /// <response code="400">Image name is null</response>
        /// <response code="404">Image not found</response>
        [HttpDelete]
        [Route("{imageName}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteImageAsync([FromRoute]string imageName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (imageName == null)
            {
                var error = ErrorResponsesService.BodyIsMissing(nameof(imageName));
                return BadRequest(error);
            }

            var fileInfo = new FileInfo($"{hostingEnvironment.WebRootPath}{ImageDirectory}{imageName}");
            
            if (!fileInfo.Exists)
            {
                var error = ErrorResponsesService.NotFoundError(Target, "Image not found");
                return NotFound(error);
            }

            fileInfo.Delete();
            return NoContent();
        }
    }
}