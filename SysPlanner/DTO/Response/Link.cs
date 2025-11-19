namespace SysPlanner.DTO.Response
{
    public class Link
    {
        public string Href { get; set; } = string.Empty; // URL do recurso
        public string Rel { get; set; } = string.Empty;  // Relação (ex: "self", "update")
        public string Method { get; set; } = string.Empty; // Método HTTP (GET, POST...)
    }
}
