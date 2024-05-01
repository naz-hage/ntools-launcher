## Build and Test
The project is built and tested using the `NTools` Standardized build tools.  The `Nb` tool is used to build msbuild targets defined in the `nbuild.targets` file. To build and test

- Open a Developer command prompt for VS 2022
- Run the following command in directory of the solution to create a local build to deploy and test:
```bash
nb staging
```
- Run the following command in directory of the solution to list the available build targets:
```bash
nb -c targets
```
The [targets](./targets.md) file contains the list of available targets.