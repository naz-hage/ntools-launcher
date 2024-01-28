[Next](#next)

## Version 1.2.0 - 28-jan-24

## Version 1.1.0 - 25-jan-24
- Add lock file to prevent file being changed before it is launched.
    - If file cannot be locked, then it is not launched. An error is returned.
    - After returnning from `launch`, the file is unlocked.
- Add option to check for digital signature of file.
    - If file is not signed, then it is not launched. An error is returned.
    - Digital signature check is only performed when the file is locked.

## Version 1.0.18 - jan-24
- Initial release Nuget package release.

## Next

TOCTOU (Time of Check to Time of Use) is a class of software bugs where a program's control flow can be altered by changing the system state between the time of check (TOC) and the time of use (TOU). This can lead to serious security vulnerabilities if an attacker can exploit this window of opportunity.

For example, consider a program that checks if a file exists and then opens it. An attacker could potentially replace the file between the time it is checked and the time it is opened, causing the program to open the wrong file.

To mitigate TOCTOU vulnerabilities, you should:

1. Minimize the time window between the check and use.
2. Avoid making security decisions based on the results of a previous check.
3. Use atomic operations whenever possible. These are operations that complete in a single step without the chance for the state to change in between.
4. Use file handles instead of file names when operating on files. Once a file is opened, the handle always refers to the same file, even if its name changes.

Here's an example of how to resolve a TOCTOU vulnerability in Python:

```python
# Vulnerable TOCTOU code
if os.path.exists(filename

):


    with open(filename, 'r') as f:
        data = f.read()

# Mitigated code
try:
    with open(filename, 'r') as f:
        data = f.read()
except FileNotFoundError:
    pass
```

In the mitigated code, we open the file directly without checking if it exists first. If the file doesn't exist, a `FileNotFoundError` is raised, which we can catch and handle appropriately. This eliminates the time window between the check and use, mitigating the TOCTOU vulnerability.