using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Models.Secrets.Exceptions;
using Models.Secrets.Repositories;
using Model = Models.Secrets;
using Client = ClientModels.Secrets;
using Converter = ModelConverters.Secrets;

namespace API.Controllers
{
    [Route("api/v1/secrets")]
    public class SecretsController : ControllerBase
    {
        private readonly ISecretRepository repository;
        private const string Target = "Secret";

        public SecretsController(ISecretRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        
        /// <summary>
        /// Creates secret
        /// </summary>
        /// <param name="creationInfo">Secret creation info</param>
        /// <param name="cancellationToken"></param>
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> CreateSecretAsync([FromBody]Client.SecretCreationInfo creationInfo,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (creationInfo == null)
            {
                var error = ErrorResponsesService.BodyIsMissing(nameof(creationInfo));
                return BadRequest(error);
            }

            const string blackTemplateWithColoredPoints = "1001582";
            var pointsUrl = await PhotolabService.PostMultipartFormData(creationInfo.ImageUrl, blackTemplateWithColoredPoints);
            var webClient = new WebClient();
            var imageBytes = await webClient.DownloadDataTaskAsync(pointsUrl);
            Stream stream = new MemoryStream(imageBytes);
            
            var description = PhysiognomyService.GetDescription(new Bitmap(stream));
            const string photoTemplateWithColoredPoints = "1001581";
            var url = await PhotolabService.PostMultipartFormData(creationInfo.ImageUrl, photoTemplateWithColoredPoints);
            
            var modelCreationInfo = new Model.SecretCreationInfo(url, description);
            var modelSecret =
                await repository.CreateAsync(modelCreationInfo, cancellationToken).ConfigureAwait(false);
            
            var clientSecret = Converter.SecretConverter.Convert(modelSecret);
            return CreatedAtRoute("GetSecretRoute", new { id = clientSecret.Id }, clientSecret);
        }
        
        /// <summary>
        /// Returns a secret by id
        /// </summary>
        /// <param name="id">Secret id</param>
        /// <param name="cancellationToken"></param>
        [HttpGet]
        [Route("{id}", Name = "GetSecretRoute")]
        public async Task<IActionResult> GetSecretAsync([FromRoute]string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!Guid.TryParse(id, out var guid) || guid.Equals(Guid.Empty))
            {
                var error = ErrorResponsesService.NotFoundError(Target, $"Secret with id '{id}' not found.");
                return NotFound(error);
            }
            
            Model.Secret modelSecret;
            
            try
            {
                modelSecret = await repository.GetAsync(guid, cancellationToken).ConfigureAwait(false);
            }
            catch (SecretNotFoundException ex)
            {
                var error = ErrorResponsesService.NotFoundError(Target, ex.Message);
                return NotFound(error);
            }

            var clientSecret = Converter.SecretConverter.Convert(modelSecret);
            return Ok(clientSecret);
        }
    }
}