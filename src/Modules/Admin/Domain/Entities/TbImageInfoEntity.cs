namespace Hello100Admin.Modules.Admin.Domain.Entities
{
    public class TbImageInfoEntity
    {
        public int ImgId { get; set; }

        public string ImgKey { get; set; } = default!;

        public string? Url { get; set; }

        public string DelYn { get; set; } = default!;

        public int RegDt { get; set; }
    }
}
