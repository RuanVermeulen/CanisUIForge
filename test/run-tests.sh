#!/bin/bash
set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

echo "============================================"
echo "  CanisUIForge End-to-End Test Runner"
echo "============================================"
echo ""

# Step 1: Build the main solution
echo "[1/5] Building CanisUIForge solution..."
cd "$REPO_ROOT"
dotnet build CanisUIForge.sln --configuration Release --verbosity quiet
echo "  Main solution built successfully."
echo ""

# Step 2: Build the test API (ensures contracts DLL is available)
echo "[2/5] Building TestApi solution..."
cd "$SCRIPT_DIR/TestApi"
dotnet build TestApi.sln --configuration Release --verbosity quiet
echo "  TestApi built successfully."
echo ""

# Step 3: Build the integration tests
echo "[3/5] Building integration tests..."
cd "$SCRIPT_DIR/CanisUIForge.IntegrationTests"
dotnet build --configuration Release --verbosity quiet
echo "  Integration tests built successfully."
echo ""

# Step 4: Run integration tests
echo "[4/5] Running integration tests..."
cd "$SCRIPT_DIR/CanisUIForge.IntegrationTests"
dotnet test --configuration Release --verbosity normal --no-build
echo ""

# Step 5: Summary
echo "[5/5] Complete!"
echo "============================================"
echo "  All integration tests passed."
echo "============================================"
