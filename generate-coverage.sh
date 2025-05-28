#!/bin/sh
# Generate code coverage report for .NET projects

# Install tools if not already installed
dotnet tool install --global dotnet-coverage --version 17.8.0 || true
dotnet tool install --global dotnet-reportgenerator-globaltool --version 5.2.4 || true

export PATH="$PATH:$HOME/.dotnet/tools"

# Restore all projects to ensure dependencies are present
dotnet restore

# Run tests with coverage from the solution root to ensure all projects are included
dotnet-coverage collect --output-format cobertura --output "coverage.cobertura.xml" \
  dotnet test --no-build /workspaces/codespaces-blank

# Generate HTML report
reportgenerator \
  -reports:coverage.cobertura.xml \
  -targetdir:coverage-report \
  -reporttypes:Html

echo "Open coverage-report/index.html in your browser to view the coverage report."
