namespace portal_backend.Entities;

public class ServiceServiceCategory
{
    public int ServiceId { get; set; }
    public Service Service { get; set; }
    public int ServiceCategoryId { get; set; }
    public ServiceCategory ServiceCategory { get; set; }
}