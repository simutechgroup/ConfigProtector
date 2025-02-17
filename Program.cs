using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace ConfigProtector
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: ConfigProtector.exe -e|-d <configFolder>");
                Console.WriteLine("Example: ConfigProtector.exe  -e \"W:\\inetpub\\wwwroot\\admin\"");
                return;
            }

            string operation = args[0].ToLower();
            string configFolder = args[1];

            string configFile = Path.Combine(configFolder, "web.config");
            if (!File.Exists(configFile))
            {
                Console.WriteLine("web.config not found at: " + configFile);
                return;
            }

            try
            {
                // Map the configuration file.
                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap
                {
                    ExeConfigFilename = configFile
                };

                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

                // List the sections (or section paths) to process.
                // Note: For nested sections (like "system.net/mailSettings/smtp"),
                // we will split on '/' and process the subsection from its group.
                List<string> sectionsToProcess = new List<string>
                {
                    "appSettings",
                    "connectionStrings",
                    "dbSettings",          // Assuming these custom sections are defined
                    "crmSettings",         // in <configSections>
                    "system.net/mailSettings/smtp" // nested section example
                };

                bool modified = false;

                foreach (string sec in sectionsToProcess)
                {
                    ConfigurationSection section = GetSectionByPath(config, sec);
                    if (section == null)
                    {
                        Console.WriteLine($"Section '{sec}' not found.");
                        continue;
                    }

                    if (ProcessSection(section, operation))
                    {
                        modified = true;
                    }
                }

                if (modified)
                {
                    // Save the configuration file if any changes were made.
                    config.Save(ConfigurationSaveMode.Full);
                    Console.WriteLine("Configuration updated successfully.");
                }
                else
                {
                    Console.WriteLine("No changes were necessary.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        /// <summary>
        /// Traverses the configuration to get a section using a path that may include nested section groups.
        /// For example, "system.net/mailSettings/smtp".
        /// </summary>
        static ConfigurationSection GetSectionByPath(Configuration config, string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            string[] parts = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return null;

            // If there's only one part, assume it's a top-level section.
            if (parts.Length == 1)
            {
                return config.GetSection(parts[0]);
            }

            // Traverse the section groups.
            ConfigurationSectionGroup group = config.SectionGroups[parts[0]];
            if (group == null)
                return null;

            for (int i = 1; i < parts.Length - 1; i++)
            {
                group = group.SectionGroups[parts[i]];
                if (group == null)
                    return null;
            }

            // The last part is the section name.
            return group.Sections[parts[parts.Length - 1]];
        }

        /// <summary>
        /// Encrypts or decrypts the specified configuration section.
        /// </summary>
        /// <param name="section">The configuration section.</param>
        /// <param name="operation">"-encrypt" or "-decrypt".</param>
        /// <returns>True if the section was modified; otherwise, false.</returns>
        static bool ProcessSection(ConfigurationSection section, string operation)
        {
            bool modified = false;
            string sectionName = section.SectionInformation.Name;

            if (operation == "-e")
            {
                if (!section.SectionInformation.IsProtected)
                {
                    Console.WriteLine("Encrypting section: " + sectionName);
                    // Protect the section using the DataProtectionConfigurationProvider.
                    section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                    section.SectionInformation.ForceSave = true;
                    modified = true;
                }
                else
                {
                    Console.WriteLine("Section already encrypted: " + sectionName);
                }
            }
            else if (operation == "-d")
            {
                if (section.SectionInformation.IsProtected)
                {
                    Console.WriteLine("Decrypting section: " + sectionName);
                    section.SectionInformation.UnprotectSection();
                    section.SectionInformation.ForceSave = true;
                    modified = true;
                }
                else
                {
                    Console.WriteLine("Section is not encrypted: " + sectionName);
                }
            }
            else
            {
                Console.WriteLine("Unknown operation. Use -e or -d.");
            }

            return modified;
        }
    }
}
