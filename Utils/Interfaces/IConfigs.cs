namespace Utils.Interfaces
{
    public interface IConfigs
    {
        /// <summary> 
        /// funcoes usadas no services.config
        /// </summary>
        string GetParameter(string section, string key, bool allowNull = true, string defau = null);
    }
}
