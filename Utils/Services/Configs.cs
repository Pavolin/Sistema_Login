using System;
using System.IO;
using System.Xml;
using System.Reflection;

using Utils.Interfaces;
using Utils.Exceptions;

namespace Utils.Services
{
    public class Configs : IConfigs
    {
        #region Attributes
        /// <summary> 
        /// area das referencias ao atributos usados nessa classe
        /// </summary>
        private string _configPath;
        private XmlDocument _configs;
        #endregion

        #region Constructor Methods

        /// <summary> 
        /// area dos construtores da classe
        /// </summary>
        public Configs()
        {
            try
            {
                this._configPath = $"{Assembly.GetExecutingAssembly().Location}.config";
                _loadConfigs();
            }
            catch (Exception ex)
            {
                throw new ConfigsExceptions(ex.Message);
            }
        }

        public Configs(string path)
        {
            try
            {
                this._configPath = path;
                _loadConfigs();
            }
            catch (Exception ex)
            {
                throw new ConfigsExceptions(ex.Message);
            }
        }

        #endregion

        #region Methods
        /// <summary> 
        /// area dos metodos usados em outras classes
        /// </summary>

        private void _loadConfigs()
        {
            this._configs = new XmlDocument();
            using (StreamReader fr = new StreamReader(this._configPath))
                this._configs.LoadXml(fr.ReadToEnd());
        }

        public string GetParameter(string section, string key, bool allowNull = true, string defau = null)
        {
            if (string.IsNullOrEmpty(section)) throw new ConfigsExceptions("section can't be empty");
            if (string.IsNullOrEmpty(key)) throw new ConfigsExceptions("key can't be empty");
            try
            {
                string value = this._configs.SelectSingleNode("/configuration/" + section + "/add[@key='" + key + "']").Attributes["value"].Value.ToString();

                return value;
            }
            catch
            {
                if (!allowNull && string.IsNullOrEmpty(defau)) throw new ConfigsExceptions(string.Format("{0}.{1} can't be empty", section, key));
                else if (!allowNull) return defau;
                return string.Empty;
            }
        }

        public string GetAtribute(string section, string key, string attribute, bool allowNull = true, bool lower = true)
        {
            if (string.IsNullOrEmpty(section)) throw new ConfigsExceptions("section can't be empty");
            if (string.IsNullOrEmpty(key)) throw new ConfigsExceptions("key can't be empty");
            if (string.IsNullOrEmpty(attribute)) throw new ConfigsExceptions("attribute can't be empty");
            try
            {
                XmlAttribute xmlAttribute = this._configs.SelectSingleNode("/configuration/" + section + "/add[@key='" + key + "']").Attributes[attribute];
                if(xmlAttribute == null)
                    return string.Empty;
                string value = xmlAttribute.Value.ToString();
                if (lower)
                    value = value.ToLower();

                return value;
            }
            catch
            {
                if (!allowNull) throw new ConfigsExceptions(string.Format("{0}.{1}.{2} can't be empty", section, key, attribute));
                return string.Empty;
            }
        }

        #endregion
    }

    #region Configs
    /// <summary> 
    /// area dos metodos usados para configurar essa classe
    /// </summary>
    public static class ConfigsUI
    {

        #region Attributes
        static private XmlDocument _configs;
        #endregion

        #region Methods

        static internal void _changeGroup(string group)
        {
            XmlNodeList atributes = _configs.SelectNodes($"/configuration/{group}/add");
            Console.WriteLine($"--Group : {group}");
            for (int i = 0; i < atributes.Count; i++)
            {
                Console.WriteLine();
                string key = atributes[i].Attributes["key"].Value;
                string value = atributes[i].Attributes["value"].Value;
                string attr = "";
                try
                {
                    attr = atributes[i].Attributes["encrypted"].Value.ToLower();
                }
                catch
                { }
                Console.Write($"{key} (\"{value}\" press enter to keep):");
                string newValue = Console.ReadLine();
                if (newValue != String.Empty)
                {
                    atributes[i].Attributes["value"].Value = newValue;
                }
            }
        }

        static public void ChangeConfigsUI(string path)
        {
            try
            {
                Console.WriteLine($"Manage Config");
                _configs = new XmlDocument();
                using (StreamReader fr = new StreamReader(path))
                    _configs.LoadXml(fr.ReadToEnd());
                XmlNodeList XMLgroups = _configs.SelectNodes("/configuration/configSections/section");
                foreach (XmlNode XMLgroup in XMLgroups)
                {
                    _changeGroup(XMLgroup.Attributes["name"].Value);
                }
                _changeGroup("appSettings");
                if (File.Exists(path))
                    File.Delete(path);
                _configs.Save(path);
                Console.WriteLine("Succesffully to config !");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Issues in the configuration: {ex.Message}");
            }
            Console.WriteLine("Press some key...");
            Console.ReadKey();
        }
        #endregion
    }
    #endregion
}
