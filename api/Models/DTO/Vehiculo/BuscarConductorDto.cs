namespace api.Models.DTO
{
    public class BuscarConductorResponse
    {
        public List<BuscarConductorDto> Data { get; set; }
    }

    public class BuscarConductorDto
    {
        public string id { get; set; }
        public CRMRelatedObject Conductor { get; set; }
    }
}
