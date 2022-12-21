namespace VRisingServerManagement.Classes;

public class RCONConfiguration
{
    public bool Enabled { get; set; }
    
    public string Password { get; set; }
    
    public string SecretPassword { get; set; }
    
    /// <summary>
    /// Defaults to the same port as TCP/UDP from server config
    /// </summary>
    public long Port { get; set; }
}