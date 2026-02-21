#!/bin/bash
# Initializes the database schema from db/Objects SQL files.
# Runs in order: tables (FK checks off) → functions → views → procedures.
set -e

DB="${MYSQL_DATABASE}"
OBJECTS_DIR="/db-objects"

mysql_exec() {
    mysql -u root -p"${MYSQL_ROOT_PASSWORD}" "$DB" "$@"
}

echo "=== Initializing database schema ==="

# ── Tables (all in one session with FK checks disabled) ──────────────────────
if [ -d "$OBJECTS_DIR/tables" ] && ls "$OBJECTS_DIR/tables"/*.sql &>/dev/null; then
    echo "Loading tables..."
    {
        echo "SET FOREIGN_KEY_CHECKS=0;"
        for f in "$OBJECTS_DIR/tables"/*.sql; do
            [ -f "$f" ] || continue
            cat "$f"
            echo
        done
        echo "SET FOREIGN_KEY_CHECKS=1;"
    } | mysql_exec
    echo "  Tables done."
fi

# ── Functions ─────────────────────────────────────────────────────────────────
if [ -d "$OBJECTS_DIR/functions" ] && ls "$OBJECTS_DIR/functions"/*.sql &>/dev/null; then
    echo "Loading functions..."
    for f in "$OBJECTS_DIR/functions"/*.sql; do
        [ -f "$f" ] || continue
        mysql_exec < "$f"
    done
    echo "  Functions done."
fi

# ── Views ─────────────────────────────────────────────────────────────────────
if [ -d "$OBJECTS_DIR/views" ] && ls "$OBJECTS_DIR/views"/*.sql &>/dev/null; then
    echo "Loading views..."
    for f in "$OBJECTS_DIR/views"/*.sql; do
        [ -f "$f" ] || continue
        mysql_exec < "$f"
    done
    echo "  Views done."
fi

# ── Stored Procedures ─────────────────────────────────────────────────────────
if [ -d "$OBJECTS_DIR/procedures" ] && ls "$OBJECTS_DIR/procedures"/*.sql &>/dev/null; then
    echo "Loading procedures..."
    for f in "$OBJECTS_DIR/procedures"/*.sql; do
        [ -f "$f" ] || continue
        mysql_exec < "$f"
    done
    echo "  Procedures done."
fi

echo "=== Schema initialization complete ==="
