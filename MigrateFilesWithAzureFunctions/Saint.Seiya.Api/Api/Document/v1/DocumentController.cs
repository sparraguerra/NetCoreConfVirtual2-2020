using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saint.Seiya.Services.Abstract;
using Saint.Seiya.Shared.Models;
using Saint.Seiya.Shared.Models.Dto;
using Saint.Seiya.Shared.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Saint.Seiya.Api.Document.v1
{
    /// <summary>
    /// DocumentController
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService documentService;
        private readonly ILogger<DocumentController> logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="documentDomain"></param>
        /// <param name="logger"></param>
        public DocumentController(IDocumentService documentDomain, ILogger<DocumentController> logger)
        {
          
            this.documentService = documentDomain;
            
            this.logger = logger;
        }

        /// <summary>
        /// Retrieve a specific document.
        /// </summary>
        /// <returns>A DocumentResponse document.</returns>
        /// <response code="200">Returns the document.</response>
        /// <response code="400">If the getDocumentRequest is null or invalid.</response> 
        /// <response code="404">If the document is not found.</response> 
        [HttpGet]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DocumentResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public virtual async Task<IActionResult> GetById([FromQuery] GetDocumentRequest getDocumentRequest)
        {
            try
            {
                return Ok(await documentService.GetMetadataAsync(getDocumentRequest));
            }
            catch (SeiyaDataNotFoundException doc)
            {
              
                logger.LogError(doc, doc.Message);
                return NotFound(doc.Message);
            }
            catch (SeiyaBaseException ex)
            {
                logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);

            }
        }

        /// <summary>
        /// Retrieve a specific Uri with Sas of the file related with the document.
        /// </summary>
        /// <returns>A GetUriResponse item.</returns>
        /// <response code="200">Returns the Uri with the Id.</response>
        /// <response code="400">If the getDocumentRequest is null or invalid.</response> 
        /// <response code="404">If the document is not found.</response> 
        [HttpGet]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GetUriResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [Route("uri")]
        public async Task<IActionResult> GetUriSasById([FromQuery]GetDocumentRequest getDocumentRequest)
        {
            try
            {
                return Ok(await documentService.GetDocumentSasUriAsync(getDocumentRequest));
            }
            catch (SeiyaDataNotFoundException doc) 
            {
                logger.LogError(doc, doc.Message);
                
                return NotFound(doc.Message);
            }
            catch (SeiyaBaseException ex)
            {
                logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Get available processes
        /// </summary>
        /// <returns></returns>
        [HttpGet("availables")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<ProcessResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAvailableProcesses()
        {
            try
            {
                return Ok(await documentService.GetProcesses());
            }
            catch (SeiyaDataNotFoundException doc)
            {
                logger.LogError(doc, doc.Message);

                return NotFound(doc.Message);
            }
            catch (SeiyaBaseException ex)
            {
                logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Get traces processes
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("traces")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(TracesResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(500)]
        public IActionResult GetHistoricProcesses([FromQuery] TracesRequest request)
        {
            var result = documentService.GetTraces(request);

            return Ok(result);
        }



        /// <summary>
        /// Retrieve a specific file related with the document.
        /// </summary>
        /// <returns>A file.</returns>
        /// <response code="200">Returns the file related with the Id.</response>
        /// <response code="400">If the getDocumentRequest is null or invalid.</response> 
        /// <response code="404">If the document is not found.</response> 
        [HttpGet]
        [Consumes("application/json")]
        [Produces("application/octect-stream")]
        [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [Route("content")]
        public async Task<IActionResult> GetContentById([FromQuery]GetDocumentRequest getDocumentRequest)
        {
            try
            {
                var result = await documentService.GetDocumentAsync(getDocumentRequest);
                result.File.Seek(0, SeekOrigin.Begin);
                return File(result.File, result.ContentType, result.FileName);
            }
            catch (SeiyaDataNotFoundException doc)
            {
                logger.LogError(doc, doc.Message);
                return NotFound(doc.Message);
            }
            catch (SeiyaBaseException ex) {

                logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        } 

        /// <summary>
        /// Index a document with a file.
        /// </summary>
        /// <remarks>
        /// Is a method that receives two elements in the body request, one of them is a binary file and the other element is an IndexRequestWithBinary.
        /// </remarks>
        /// <returns>A DocumentResponse indexed.</returns>
        /// <response code="200">Returns the document indexed.</response>
        /// <response code="400">If the indexRequest is null or invalid.</response>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DocumentResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Route("indexbinary")]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [RequestSizeLimit(209715200)]
        public async Task<IActionResult> IndexBinary([FromForm] IndexDocumentWithBinaryrequest request)
        {
            try
            { 
                return Ok(await documentService.IndexDocumentWithBinaryAsync(request));
            }
            catch (SeiyaBaseException ex)
            {
                logger.LogError(ex, ex.Message);
                throw;
            }
        }



        /// <summary>
        /// Update Process Status.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>Boolean.</returns>
        /// <response code="200">Document processed.</response>
        /// <response code="400">Error Document.</response>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Route("updateProcess")]
        public async Task<IActionResult> UpdateProcess([FromBody] UpdateStatusRequest request)
        {
            try
            {
                return Ok(await documentService.UpdateProcess(request));
            }
            catch (SeiyaBaseException ex)
            {
                logger.LogError(ex, ex.Message);
                throw;
            }
        }



        /// <summary>
        /// Delete a specific document.
        /// </summary>
        /// <returns>A boolean response.</returns>
        /// <response code="200">Returns the boolean response of the deleting process.</response>
        /// <response code="400">If the deleteRequest is null or invalid.</response> 
        /// <response code="404">If the document is not found.</response> 
        [HttpDelete]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [Route("delete")]
        public async Task<IActionResult> Delete(DeleteRequest deleteRequest)
        {
            try
            {
                return Ok(await documentService.DeleteDocumentAsync(deleteRequest));
            }
            catch (SeiyaBaseException ex)
            {
                logger.LogError(ex, ex.Message);
               throw;
            }
        }

        /// <summary>
        /// Recover a deleted document.
        /// </summary>
        /// <returns>A DocumentResponse updated.</returns>
        /// <response code="200">Returns true if recovered document deleted.</response>
        /// <response code="400">If the updateRequest is null or invalid.</response> 
        /// <response code="404">If the document is not found.</response> 
        [HttpPut]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [Route("revertDelete")]
        public async Task<IActionResult> RevertDelete(RestoreRequest restoreRequest)
        {
            try
            {
                return Ok(await documentService.RestoreDocumentAsync(restoreRequest)); 
            }
            catch (SeiyaBaseException ex)
            {
                logger.LogError(ex, ex.Message);
                throw;
            }
        }


        /// <summary>
        /// Launch Specific Action
        /// </summary>
        /// <returns></returns>
        [HttpPost("launch")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> LaunchProcess([FromBody] LaunchProcessRequest request)
        {
            try
            {
                var result = await documentService.LaunchProcessAsync(request);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(223, ex.Message);
            }
        }
    }
}