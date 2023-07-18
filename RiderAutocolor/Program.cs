using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using System.Xml.XPath;
using Skybrud.Colors;

[assembly: InternalsVisibleTo("RiderAutocolor.Test")]

namespace RiderAutocolor
{
    internal class Program
    {
        /// <summary>
        /// This program modifies workspace.xml configuration file to have a different color for each project of the solution.
        /// </summary>
        /// <param name="sln">The path to the sln file you work on.</param>
        /// <param name="directory">The path to a directory where you want to process all the solutions.</param>
        /// <param name="lightness">Lightness of the color.</param>
        /// <param name="saturation">Saturation of the color.</param>
        static void Main(string sln = null, string directory = null, double lightness = 0.25, double saturation = 0.60)
        {
            if ((string.IsNullOrEmpty(sln) ^ string.IsNullOrEmpty(directory)) == false)
            {
                throw new Exception($"You should provide one of the arguments sln or directory.");
            }

            if (!string.IsNullOrEmpty(sln))
            {
                ProcessSolution(sln, lightness, saturation);
            }
            else
            {
                var solutions = Directory.GetFiles(directory, "*.sln", SearchOption.AllDirectories);
                foreach (var solution in solutions)
                {
                    ProcessSolution(solution, lightness, saturation);
                }
            }
        }

        private static void ProcessSolution(string sln, double lightness, double saturation)
        {
            Console.WriteLine(sln);
            var projectFileExtensions = new[] {"csproj", "fsproj"};

            var slnInfo = new FileInfo(sln);
            var configPath = $@"{slnInfo.Directory.FullName}\.idea\.idea.{Path.GetFileNameWithoutExtension(slnInfo.FullName)}\.idea\workspace.xml";

            if (File.Exists(configPath))
            {
                var projects =
                    projectFileExtensions.SelectMany(extension =>
                            Directory.GetFiles(slnInfo.Directory.FullName, $"*.{extension}", SearchOption.AllDirectories)
                        )
                        .OrderBy(f => f)
                        .ToList();
                var hueIndex = 0d;

                var xml = XElement.Load(configPath);
                var fileColors = GetNode(xml, "FileColors");
                var namedScopeManager = GetNode(xml, "NamedScopeManager");

                foreach (var project in projects)
                {
                    var fi = new FileInfo(project);

                    var projectPath = GetProjectPath(slnInfo.Directory.FullName, fi.Directory.FullName)
                        .Replace("\\", "/");

                    var pattern = $@"file[rider.module]:{projectPath}//*";
                    var scopeName = $@"Project:{Path.GetFileNameWithoutExtension(fi.FullName)}";

                    Console.WriteLine($"- {scopeName}");

                    // var color = new HslColor(hueIndex / 360.0, 0.8, 0.85);
                    var color = new HslColor(hueIndex / 360.0, saturation, lightness);

                    namedScopeManager.Add(
                        new XElement("scope",
                            new XAttribute("name", scopeName),
                            new XAttribute("pattern", pattern)
                        )
                    );

                    fileColors.Add(
                        new XElement("fileColor",
                            new XAttribute("scope", scopeName),
                            new XAttribute("color", color.ToHex())
                        )
                    );

                    hueIndex = (hueIndex + 50) % 360;
                }

                xml.Save(configPath);
            }

            Console.WriteLine();
        }

        internal static string GetProjectPath(string slnDirectory, string projectDirectory)
        {
            return projectDirectory.TrimEnd('\\').Substring(slnDirectory.TrimEnd('\\').Length + 1);
        }

        private static XElement GetNode(XElement xml, string name)
        {
            var node = xml.XPathSelectElement($@"//component[@name=""{name}""]");
            if (node != null)
            {
                node.RemoveNodes();
                return node;
            }

            node = new XElement("component", new XAttribute("name", name));
            xml.Add(node);
            return node;
        }
    }
}
