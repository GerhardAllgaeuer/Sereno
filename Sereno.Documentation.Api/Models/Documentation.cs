namespace Sereno.Documentation.Api.Models
{
    public class Documentation
    {
        public string Id { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public string DocumentKey { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? HtmlContent { get; set; }
        public string? Author { get; set; }
        public DateTime? NextCheck { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
} 