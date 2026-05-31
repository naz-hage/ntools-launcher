#nullable enable

using System;
using System.IO;
using System.Threading.Tasks;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlLauncher.Models;

namespace YamlLauncher;

/// <summary>
/// Loads and parses YAML launcher configurations.
/// </summary>
public class YamlLauncherConfigLoader : ILauncherConfigLoader
{
    private readonly IDeserializer _deserializer;
    private readonly LauncherConfigValidator _validator;

    /// <summary>
    /// Initializes a new instance of the YamlLauncherConfigLoader class.
    /// </summary>
    public YamlLauncherConfigLoader()
    {
        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        _validator = new LauncherConfigValidator();
    }

    /// <summary>
    /// Initializes a new instance of the YamlLauncherConfigLoader class with a custom deserializer.
    /// </summary>
    /// <param name="deserializer">The YamlDotNet deserializer to use.</param>
    public YamlLauncherConfigLoader(IDeserializer deserializer)
    {
        _deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
        _validator = new LauncherConfigValidator();
    }

    /// <summary>
    /// Loads a launcher configuration from a YAML file.
    /// </summary>
    /// <param name="filePath">The path to the YAML file to load.</param>
    /// <returns>The parsed LauncherConfig.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when access to the file is denied.</exception>
    /// <exception cref="LauncherConfigException">Thrown when the YAML is invalid or validation fails.</exception>
    public async Task<LauncherConfig> LoadFromFileAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be null or whitespace.", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Configuration file not found: {filePath}");
        }

        try
        {
            using (var reader = new StreamReader(filePath))
            {
                var yamlContent = await reader.ReadToEndAsync();
                return await LoadFromStringAsync(yamlContent);
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new UnauthorizedAccessException($"Access denied reading file: {filePath}", ex);
        }
        catch (IOException ex)
        {
            throw new LauncherConfigException($"Error reading configuration file: {filePath}", ex);
        }
    }

    /// <summary>
    /// Loads a launcher configuration from a YAML string.
    /// </summary>
    /// <param name="yamlContent">The YAML content as a string.</param>
    /// <returns>The parsed LauncherConfig.</returns>
    /// <exception cref="LauncherConfigException">Thrown when the YAML is invalid or validation fails.</exception>
    public Task<LauncherConfig> LoadFromStringAsync(string yamlContent)
    {
        if (string.IsNullOrWhiteSpace(yamlContent))
        {
            throw new ArgumentException("YAML content cannot be null or whitespace.", nameof(yamlContent));
        }

        try
        {
            var config = _deserializer.Deserialize<LauncherConfig>(yamlContent);

            if (config == null)
            {
                throw new LauncherConfigException("Deserialized configuration is null. Invalid YAML structure.");
            }

            _validator.Validate(config);

            return Task.FromResult(config);
        }
        catch (YamlException ex)
        {
            // Extract line and column information from YamlException
            var lineInfo = ex.Start;
            var message = $"Invalid YAML format at line {lineInfo.Line}, column {lineInfo.Column}: {ex.Message}";
            throw new LauncherConfigException(message, ex)
            {
                LineNumber = (int)lineInfo.Line,
                ColumnNumber = (int)lineInfo.Column
            };
        }
        catch (LauncherConfigException)
        {
            throw; // Re-throw validation exceptions
        }
        catch (Exception ex)
        {
            throw new LauncherConfigException($"Error parsing YAML configuration: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Loads a launcher configuration from a stream.
    /// </summary>
    /// <param name="stream">The stream containing YAML content.</param>
    /// <returns>The parsed LauncherConfig.</returns>
    /// <exception cref="LauncherConfigException">Thrown when the YAML is invalid or validation fails.</exception>
    public async Task<LauncherConfig> LoadFromStreamAsync(Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        if (!stream.CanRead)
        {
            throw new ArgumentException("Stream must be readable.", nameof(stream));
        }

        try
        {
            using (var reader = new StreamReader(stream))
            {
                var yamlContent = await reader.ReadToEndAsync();
                return await LoadFromStringAsync(yamlContent);
            }
        }
        catch (LauncherConfigException)
        {
            throw; // Re-throw LauncherConfigException
        }
        catch (Exception ex)
        {
            throw new LauncherConfigException($"Error reading from stream: {ex.Message}", ex);
        }
    }
}
