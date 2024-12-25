using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace backend.Filters
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            foreach (var parameter in operation.Parameters.ToList())
            {
                // Check if the parameter is of type IFormFile
                if (parameter.Name == "imageFile" && parameter.Schema.Type == "string" && parameter.Schema.Format == "binary")
                {
                    parameter.Description = "File upload";
                    parameter.Schema.Type = "string";
                    parameter.Schema.Format = "binary";
                    parameter.Content = new Dictionary<string, OpenApiMediaType>
                {
                    {
                        "multipart/form-data", new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "string",
                                Format = "binary"
                            }
                        }
                    }
                };
                }
            }
        }
    }


}