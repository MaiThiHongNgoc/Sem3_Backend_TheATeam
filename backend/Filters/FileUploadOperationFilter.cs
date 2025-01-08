using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace backend.Filters
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.RequestBody != null && operation.RequestBody.Content.ContainsKey("multipart/form-data"))
            {
                operation.RequestBody.Content["multipart/form-data"].Schema.Properties["imageFile"] = new OpenApiSchema
                {
                    Type = "string",
                    Format = "binary",
                    Description = "The image file to upload"
                };
            }
        }

    }

}