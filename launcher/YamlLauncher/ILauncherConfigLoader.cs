#nullable enable

using System.IO;
using System.Threading.Tasks;
using YamlLauncher.Models;

namespace YamlLauncher;

/// <summary>
/// Defines the contract for loading YAML launcher configurations.
/// </summary>
public interface ILauncherConfigLoader
{
    /// <summary>
    /// Loads a launcher configuration from a YAML file.
    /// </summary>
    /// <param name="filePath">The path to the YAML file to load.</param>
    /// <returns>The parsed LauncherConfig.</returns>
    /// <exception cref="System.FileNotFoundException">Thrown when the file does not exist.</exception>
    /// <exception cref="System.UnauthorizedAccessException">Thrown when access to the file is denied.</exception>
    /// <exception cref="LauncherConfigException">Thrown when the YAML is invalid or validation fails.</exception>
    Task<LauncherConfig> LoadFromFileAsync(string filePath);

    /// <summary>
    /// Loads a launcher configuration from a YAML string.
    /// </summary>
    /// <param name="yamlContent">The YAML content as a string.</param>
    /// <returns>The parsed LauncherConfig.</returns>
    /// <exception cref="LauncherConfigException">Thrown when the YAML is invalid or validation fails.</exception>
    Task<LauncherConfig> LoadFromStringAsync(string yamlContent);

    /// <summary>
    /// Loads a launcher configuration from a stream.
    /// </summary>
    /// <param name="stream">The stream containing YAML content.</param>
    /// <returns>The parsed LauncherConfig.</returns>
    /// <exception cref="LauncherConfigException">Thrown when the YAML is invalid or validation fails.</exception>
    Task<LauncherConfig> LoadFromStreamAsync(Stream stream);
}
