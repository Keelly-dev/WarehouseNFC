# WarehouseNFC

Демонстрационное приложение складского учёта на .NET MAUI. Поддерживает сценарии приёмки, отгрузки и инвентаризации. Приложение использует локальные демоданные и имитирует чтение NFC.

Откройте `WarehouseNFC.slnx` в Visual Studio с .NET 10 SDK и рабочей нагрузкой .NET MAUI нужной платформы. Для локальной демонстрации доступны учётные записи `demo1`, `demo2` и `demo3` с паролем `warehouse-demo`.

Проект `WarehouseNFC.Data` содержит модели Entity Framework Core для SQL Server. Для работы с этим проектом передайте настройки через `DbContextOptions` или задайте переменную окружения `WAREHOUSE_NFC_CONNECTION_STRING`.
