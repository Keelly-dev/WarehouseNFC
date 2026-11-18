namespace WarehouseNFC.Services
{
    public interface INfcService
    {
        Task<(int ItemId, int PositionId)> ReadTagAsync(int currentPositionId = 0);
        Task WriteTagAsync(int itemId, int positionId);
    }
}