#!/bin/sh
# Generate code coverage report for .NET projects

dotnet tool install --global dotnet-coverage || true
dotnet tool install --global dotnet-reportgenerator-globaltool || true

export PATH="$PATH:$HOME/.dotnet/tools"

dotnet-coverage collect --output-format cobertura --output "coverage.cobertura.xml" \
  dotnet test --no-build

reportgenerator \
  -reports:coverage.cobertura.xml \
  -targetdir:coverage-report \
  -reporttypes:Html

echo "Open coverage-report/index.html in your browser to view the coverage report."
