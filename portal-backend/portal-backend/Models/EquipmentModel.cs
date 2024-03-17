namespace portal_backend.Models;

public class EquipmentModel
{
    public int Id { get; set; }
    public int? InventoryNumber { get; set; }
    public string Name { get; set; }
    public string? Condition { get; set; }
    public string? Type { get; set; }
    public bool IsAvailable { get; set; }
}