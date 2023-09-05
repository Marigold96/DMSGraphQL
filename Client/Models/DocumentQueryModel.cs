namespace Client.Models
{
    public class DocumentQueryModel
    {
        public Guid? DocumentTypeId { get; set; }

        public string? Name { get; set; }

        public Guid? CreatorId { get; set; }

        public DateTime? CreationTime { get; set; }

        public decimal? Size { get; set; }

        public bool? SharedWithMe { get; set; }

        public bool? SharedByMe { get; set; }

        public string? OrderByField { get; set; }

        public string? OrderBy
        {
            get
            {
                if(OrderByField == null)
                {
                    return string.Empty;
                }
                string orderBy = " order: [{";
                switch(OrderByField)
                {
                    case "Najnovije":
                        orderBy += "creationTime: DESC}]";
                        break;
                    case "Najstarije":
                        orderBy += "creationTime: ASC}]";
                        break;
                    case "Po nazivu A-Z":
                        orderBy += "name: ASC}]";
                        break;
                    case "Po nazivu Z-A":
                        orderBy += "name: DESC}]";
                        break;
                    default:
                        throw new Exception($"Order by {OrderByField} not supported!");
                }
                return orderBy;
            }
        }
    }
}
