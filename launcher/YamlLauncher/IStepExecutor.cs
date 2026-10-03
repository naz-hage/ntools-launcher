#nullable enable

using System.Threading.Tasks;
using YamlLauncher.Models;

namespace YamlLauncher;

/// <summary>
/// Interface for executing configured steps and capturing their output.
/// </summary>
public interface IStepExecutor
{
    /// <summary>
    /// Executes a single step asynchronously.
    /// </summary>
    /// <param name="config">The launcher configuration containing step definitions</param>
    /// <param name="stepIndex">The index of the step to execute (0-based)</param>
    /// <returns>Execution result with output, exit code, and execution time</returns>
    /// <exception cref="ArgumentException">Thrown when stepIndex is out of range</exception>
    /// <exception cref="OperationCanceledException">Thrown when execution is cancelled or times out</exception>
    Task<LaunchResult> LaunchAsync(LauncherConfig config, int stepIndex);

    /// <summary>
    /// Executes the uniquely named step asynchronously.
    /// </summary>
    /// <param name="config">The launcher configuration containing step definitions</param>
    /// <param name="stepName">The name of the step to execute</param>
    /// <returns>Execution result for the selected step</returns>
    Task<LaunchResult> LaunchAsync(LauncherConfig config, string stepName);

    /// <summary>
    /// Executes the selected steps asynchronously in the order requested.
    /// </summary>
    /// <param name="config">The launcher configuration containing step definitions</param>
    /// <param name="stepIndices">The zero-based indices of the steps to execute</param>
    /// <returns>Aggregated execution results for the selected steps</returns>
    Task<LaunchResult> LaunchAsync(LauncherConfig config, params int[] stepIndices);
}
