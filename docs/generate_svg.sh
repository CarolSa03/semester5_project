#!/bin/bash

if ! command -v plantuml >/dev/null 2>&1; then
  echo "Error: PlantUML not found. Install using: brew install plantuml"
  exit 1
fi

echo "Generating SVGs..."

for US_DIR in US-2.2.*; do
  [ -d "$US_DIR" ] || continue

  PUML_DIR="$US_DIR/puml"
  SVG_DIR="$US_DIR/svg"

  if [ ! -d "$PUML_DIR" ]; then
    continue
  fi

  mkdir -p "$SVG_DIR"

  echo ""
  echo "Processing: $PUML_DIR"

  (
    cd "$US_DIR" || exit

    for PUML_FILE in puml/*.puml; do
      [ -f "$PUML_FILE" ] || continue
      echo "  -> $(basename "$PUML_FILE")"
      plantuml -tsvg -o ../svg "$PUML_FILE"
    done
  )
done

echo ""
echo "SVG generation completed."
