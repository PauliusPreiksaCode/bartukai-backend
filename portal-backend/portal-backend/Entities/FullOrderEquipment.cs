namespace portal_backend.Entities;

public class FullOrderEquipment
{
    public int FullOrderId { get; set; }
    public FullOrder FullOrder { get; set; }
    public int EquipmentId { get; set; }
    public Equipment Equipment { get; set; }
}