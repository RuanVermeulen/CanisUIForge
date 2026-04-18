#!/bin/bash
set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

case "${1:-}" in
  up)
    echo "Starting SQL Server container..."
    cd "$SCRIPT_DIR"
    docker compose up -d
    echo "Waiting for SQL Server to be ready..."
    for i in $(seq 1 30); do
      if docker exec canisui-test-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "TestApi@Pass123" -C -Q "SELECT 1" &>/dev/null; then
        echo "SQL Server is ready."
        echo ""
        echo "Initializing database..."
        docker exec -i canisui-test-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "TestApi@Pass123" -C < "$SCRIPT_DIR/init-db.sql"
        echo "Database initialized."
        exit 0
      fi
      echo "  Waiting... ($i/30)"
      sleep 2
    done
    echo "ERROR: SQL Server did not become ready in time."
    exit 1
    ;;
  down)
    echo "Stopping SQL Server container..."
    cd "$SCRIPT_DIR"
    docker compose down -v
    echo "SQL Server stopped."
    ;;
  status)
    docker inspect -f '{{.State.Status}}' canisui-test-sqlserver 2>/dev/null || echo "Container not running."
    ;;
  *)
    echo "Usage: $0 {up|down|status}"
    echo ""
    echo "  up     - Start SQL Server and initialize the test database"
    echo "  down   - Stop SQL Server and remove volumes"
    echo "  status - Check container status"
    exit 1
    ;;
esac
