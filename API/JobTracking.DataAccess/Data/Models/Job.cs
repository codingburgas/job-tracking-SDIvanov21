using JobTracking.DataAccess.Data.Base;

namespace JobTracking.DataAccess.Data.Models;

public class Job : Entity
{
    public string Title { get; set; }
    
    public virtual List<Offer> Offers { get; set; }
}