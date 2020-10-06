using System;
using System.IO;
using Microsoft.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Saint.Seiya.Shared.Models; 
using Saint.Seiya.Shared.Models.Dto; 
using Saint.Seiya.Shared.Models.Exceptions;

namespace Saint.Seiya.Domain.Middleware
{ 
    public class MultipartRequestMiddleware
    {
        public MultipartRequestMiddleware()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contentType"></param>
        /// <param name="body"></param>
        /// <param name="notifications"></param>
        /// <returns></returns>
        public virtual async Task<MultipartResult<T>> GetMultipartContent<T>(string contentType, Stream body) where T : BaseDto, new()
        {
            var result = new MultipartResult<T>
            {
                UploadRequest = new UploadRequest()
                {
                    FileStream = new MemoryStream()
                }
            };

            var boundary = GetBoundary(MediaTypeHeaderValue.Parse(contentType), 70);
            var reader = new MultipartReader(boundary, body);

            var section = await reader.ReadNextSectionAsync();

            while (section != null)
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out ContentDispositionHeaderValue contentDisposition);


                if (hasContentDispositionHeader)
                {
                    if (HasFileContentDisposition(contentDisposition))
                    {
                        result.UploadRequest.OriginalFileName = contentDisposition.FileName.ToString();
                        result.UploadRequest.Extension = Path.GetExtension(contentDisposition.FileName.ToString());
                        result.UploadRequest.ContentType = section.ContentType;
                        result.UploadRequest.FileStream = section.Body;
                    }

                    if (HasFormDataContentDisposition(contentDisposition))
                    {
                        try
                        {
                            result.Document = await GetObjectFromSection<T>(section);
                        }
                        catch (JsonReaderException ex)
                        {
                            throw new SeiyaValidationException($"The given metadata object was not well formed: {ex.Message}");
                        } 
                    }
                }

                section = await reader.ReadNextSectionAsync();
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public bool IsMultipartContentType(string contentType)
        {
            return !string.IsNullOrEmpty(contentType)
                    && contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="lengthLimit"></param>
        /// <returns></returns>
        private string GetBoundary(MediaTypeHeaderValue contentType, int lengthLimit)
        {
            var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;
            if (string.IsNullOrWhiteSpace(boundary))
            {
                throw new InvalidDataException("Missing content-type boundary.");
            }

            if (boundary.Length > lengthLimit)
            {
                throw new InvalidDataException(
                    $"Multipart boundary length limit {lengthLimit} exceeded.");
            }
            return boundary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentDisposition"></param>
        /// <returns></returns>
        private bool HasFormDataContentDisposition(ContentDispositionHeaderValue contentDisposition)
        {
            return contentDisposition != null
                    && contentDisposition.DispositionType.Equals("form-data")
                    && string.IsNullOrEmpty(contentDisposition.FileName.Value)
                    && string.IsNullOrEmpty(contentDisposition.FileNameStar.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentDisposition"></param>
        /// <returns></returns>
        private bool HasFileContentDisposition(ContentDispositionHeaderValue contentDisposition)
        {
            return contentDisposition != null
                    && contentDisposition.DispositionType.Equals("form-data")
                    && (!string.IsNullOrEmpty(contentDisposition.FileName.Value)
                        || !string.IsNullOrEmpty(contentDisposition.FileNameStar.Value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section"></param>
        /// <returns></returns>
        private async Task<T> GetObjectFromSection<T>(MultipartSection section) where T : class
        {
            var encoding = GetEncoding(section);
            using (var streamReader = new StreamReader(
                section.Body,
                encoding,
                detectEncodingFromByteOrderMarks: true,
                bufferSize: 1024,
                leaveOpen: true))
            {
                var value = await streamReader.ReadToEndAsync();
                if (String.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                {
                    value = String.Empty;
                }

                try
                {
                    var deserializeValue = JsonConvert.DeserializeObject<T>(value);
                    return deserializeValue;
                }
                catch (JsonReaderException ex)
                {
                    throw new SeiyaValidationException($"The given metadata object was not well formed: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        private Encoding GetEncoding(MultipartSection section)
        {
            var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out MediaTypeHeaderValue mediaType);

            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }
            return mediaType.Encoding;
        }

       
    }
}
