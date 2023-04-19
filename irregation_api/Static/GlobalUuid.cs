using irregation_api.Models.Response;

public static class GlobalUuid
{
    private static List<object> _devices = new List<object>();

    public static List<object> devices { get => _devices; }

    public static void setList(List<String> devicesList) {
        _devices.Clear();
        foreach (var device in devicesList) {
            _devices.Add(new { deviceUuid = device });
        }
    }


}
