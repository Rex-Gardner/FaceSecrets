using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Models.Tags.Exceptions;
using Models.Tags.Repositories;
using Model = Models.Tags;
using Client = ClientModels.Tags;
using Converter = ModelConverters.Tags;

namespace API.Controllers
{
    [Route("api/v1/tags")]
    public class TagsController : ControllerBase
    {
        private readonly ITagRepository repository;
        private const string Target = "Tag";

        public TagsController(ITagRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        
        /// <summary>
        /// Creates tag
        /// </summary>
        /// <param name="creationInfo">Tag creation info</param>
        /// <param name="cancellationToken"></param>
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> CreateTagAsync([FromBody]Client.TagCreationInfo creationInfo,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (creationInfo == null)
            {
                var error = ErrorResponsesService.BodyIsMissing(nameof(creationInfo));
                return BadRequest(error);
            }

            var modelCreationInfo = Converter.TagCreationInfoConverter.Convert(creationInfo);
            Model.Tag modelTag;

            try
            {
                modelTag =
                    await repository.CreateAsync(modelCreationInfo, cancellationToken).ConfigureAwait(false);
            }
            catch (TagDuplicationException ex)
            {
                var error = ErrorResponsesService.DuplicationError(Target, ex.Message);
                return BadRequest(error);
            }

            var clientTag = Converter.TagConverter.Convert(modelTag);
            return CreatedAtRoute("GetTagRoute", new { id = clientTag.Id }, clientTag);
        }

        /// <summary>
        /// Returns a list of tags by query
        /// </summary>
        /// <param name="searchInfo">Tag search info</param>
        /// <param name="cancellationToken"></param>
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> SearchTagsAsync([FromQuery]Client.TagSearchInfo searchInfo, 
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var modelSearchInfo = Converter.TagSearchInfoConverter.Convert(searchInfo ?? new Client.TagSearchInfo());
            var modelTagsList = await repository.SearchAsync(modelSearchInfo, cancellationToken).ConfigureAwait(false);
            var clientTagsList = modelTagsList.Select(Converter.TagConverter.Convert).ToImmutableList();

            return Ok(clientTagsList);
        }

        /// <summary>
        /// Returns a tag by id
        /// </summary>
        /// <param name="id">Tag id</param>
        /// <param name="cancellationToken"></param>
        [HttpGet]
        [Route("{id}", Name = "GetTagRoute")]
        public async Task<IActionResult> GetTagAsync([FromRoute]string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Model.Tag modelTag;

            try
            {
                modelTag = await repository.GetAsync(id, cancellationToken).ConfigureAwait(false);
            }
            catch (TagNotFoundException ex)
            {
                var error = ErrorResponsesService.NotFoundError(Target, ex.Message);
                return NotFound(error);
            }

            var clientTag = Converter.TagConverter.Convert(modelTag);
            return Ok(clientTag);
        }

        /// <summary>
        /// Updates tag info
        /// </summary>
        /// <param name="id">Tag id</param>
        /// <param name="patchInfo">Tag patch info</param>
        /// <param name="cancellationToken"></param>
        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> PatchTagAsync([FromRoute] string id,
            [FromBody] Client.TagPatchInfo patchInfo, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (patchInfo == null)
            {
                var error = ErrorResponsesService.BodyIsMissing(nameof(patchInfo));
                return BadRequest(error);
            }

            var modelPatchInfo = Converter.TagPatchInfoConverter.Convert(id, patchInfo);
            Model.Tag modelTag;

            try
            {
                modelTag = await repository.PatchAsync(modelPatchInfo, cancellationToken).ConfigureAwait(false);
            }
            catch (TagNotFoundException ex)
            {
                var error = ErrorResponsesService.NotFoundError(Target, ex.Message);
                return NotFound(error);
            }

            var clientTag = Converter.TagConverter.Convert(modelTag);
            return Ok(clientTag);
        }

        /// <summary>
        /// Deletes tag
        /// </summary>
        /// <param name="id">Tag id</param>
        /// <param name="cancellationToken"></param>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteTagAsync([FromRoute]string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await repository.RemoveAsync(id, cancellationToken).ConfigureAwait(false);
            }
            catch (TagNotFoundException ex)
            {
                var error = ErrorResponsesService.NotFoundError(Target, ex.Message);
                return NotFound(error);
            }

            return NoContent();
        }
    }
}