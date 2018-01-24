using System;
using System.Configuration;
using System.IO;

namespace Repelsteeltje.App
{
    public static class Config
    {
        /// <summary>Gets the <see cref="Format"/> to apply on the display of the names..</summary>
        public static string Format
        {
            get
            {
                var format = ConfigurationManager.AppSettings[nameof(Format)];
                return string.IsNullOrEmpty(format) ? "{0}" : format;
            }
        }

        /// <summary>Gets the <see cref="NameTypes"/>.</summary>
        public static NameTypes NameTypes
        {
            get
            {
                Enum.TryParse(ConfigurationManager.AppSettings[nameof(NameTypes)], true, out NameTypes tp);
                return tp;
            }
        }

        /// <summary>Gets the <see cref="Id"/>.</summary>
        public static string Id => ConfigurationManager.AppSettings[nameof(Id)];

        /// <summary>Gets the <see cref="Id"/>.</summary>
        public static DirectoryInfo FilesLocation => new DirectoryInfo(ConfigurationManager.AppSettings[nameof(FilesLocation)]);
    }
}
