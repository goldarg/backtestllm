namespace api.Models.DTO.User
{
    public class ApiResponse
    {
        public List<ResponseData>? data { get; set; }
    }

    public class ResponseData
    {
        public string? code { get; set; }
        public Details? details { get; set; }
        public string? message { get; set; }
        public string? status { get; set; }
    }

    public class Details
    {
        public string? id { get; set; }
    }
}