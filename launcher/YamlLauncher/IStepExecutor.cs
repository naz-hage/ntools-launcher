#nullable enable

using System.Threading.Tasks;
using YamlLauncher.Models;

namespace YamlLauncher;

/// <summary>
/// Interface for executing a single step and capturing its output.
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
}
