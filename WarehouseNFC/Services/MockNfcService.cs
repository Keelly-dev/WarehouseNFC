namespace WarehouseNFC.Services
{
    public class MockNfcService : INfcService
    {
        private readonly Random _random = new();
        private int _currentIndex = 0;
        private int _currentPositionVisits = 0;

        public void ResetIndex()
        {
            _currentIndex = 0;
            _currentPositionVisits++;
        }

        public Task<(int ItemId, int PositionId)> ReadTagAsync(int currentPositionId = 0)
        {
            if (currentPositionId > 0)
            {
                var positionItems = LocalDataService.ProductItems
                    .Where(i => i.PositionId == currentPositionId && i.Status == "На складе")
                    .ToList();

                // На первой позиции всегда пропускаем последний товар
                var availableCount = (_currentPositionVisits == 1 && positionItems.Count > 1)
                    ? positionItems.Count - 1
                    : positionItems.Count;

                if (positionItems.Count == 0 || _currentIndex >= availableCount)
                    return Task.FromResult((99999, currentPositionId));

                var item = positionItems[_currentIndex];
                _currentIndex++;

                var roll = _random.Next(100);

                if (roll < 90)
                    return Task.FromResult((item.Id, item.PositionId));
                else if (roll < 95)
                {
                    var otherPositions = LocalDataService.Positions
                        .Where(p => p.Id != currentPositionId)
                        .ToList();
                    var wrongPositionId = otherPositions.Count > 0
                        ? otherPositions[_random.Next(otherPositions.Count)].Id
                        : (currentPositionId == 1 ? 2 : currentPositionId - 1);
                    return Task.FromResult((item.Id, wrongPositionId));
                }
                else
                    return Task.FromResult((99999, currentPositionId));
            }

            var allItems = LocalDataService.ProductItems
                .Where(i => i.Status == "На складе")
                .ToList();

            if (allItems.Count == 0) return Task.FromResult((1, 1));

            var fallback = allItems[_currentIndex % allItems.Count];
            _currentIndex++;
            return Task.FromResult((fallback.Id, fallback.PositionId));
        }

        public Task WriteTagAsync(int itemId, int positionId)
        {
            return Task.CompletedTask;
        }
    }
}