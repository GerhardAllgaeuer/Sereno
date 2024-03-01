#!/bin/bash
# https://dev.to/brunnerlivio/create-an-icon-web-font-for-your-design-system-1ei6

# Wechselt in das entsprechende Verzeichnis
cd D:/Projekte/Privat/Sereno/Sereno.Icons

# Temporäres Verzeichnis erstellen
rm -rf svg-path
mkdir svg-path

# Temporäre Dateien, die mit dem Script noch nachbearbeitet werden
cp svg/* svg-path/

# Alle Objekte im svg in einen Pfad umwandeln, der dann der Glyph wird
npx svgo --enable=convertShapeToPath svg-path/*.svg

# Icon-Font usw. generieren
npx icon-font-generator svg/*.svg -o font/ --name "sereno" --height 100

# svg-path ist nur temporär vorhanden, löschen
rm -rf svg-path


#!/bin/bash


# Fonts
SOURCE_DIR="/d/Projekte/Privat/Sereno/Sereno.Icons/font"
FONT_DEST_DIR="/d/Projekte/Privat/Sereno/sereno.client/src/assets/fonts"

# Kopiere alle Schriftarten-Dateien
cp "$SOURCE_DIR"/*.ttf "$FONT_DEST_DIR"
cp "$SOURCE_DIR"/*.otf "$FONT_DEST_DIR"
cp "$SOURCE_DIR"/*.eot "$FONT_DEST_DIR"
cp "$SOURCE_DIR"/*.woff "$FONT_DEST_DIR"
cp "$SOURCE_DIR"/*.woff2 "$FONT_DEST_DIR"
cp "$SOURCE_DIR"/*.svg "$FONT_DEST_DIR"


# css
SOURCE_FILE="/d/Projekte/Privat/Sereno/Sereno.Icons/font/sereno.css"
DEST_FILE="/d/Projekte/Privat/Sereno/sereno.client/src/assets/scss/paper-dashboard/_sereno.scss"
cp "$SOURCE_FILE" "$DEST_FILE"

# Pfade ersetzen
sed -i 's|url("\.\/|url("\.\.\/\.\.\/fonts\/|g' "$DEST_FILE"
#!/bin/bash

# SVG Teil rausnehmen
sed -i '/url("..\//fonts\/sereno.svg\?.*#sereno") format("svg")/d' "$DEST_FILE"
sed -i '/url("..\//fonts\/sereno.svg\?[^"]*#sereno") format("svg")/d' "/d/Projekte/Privat/Sereno/sereno.client/src/assets/scss/paper-dashboard/_sereno.scss"


#!/bin/bash

# Definiere den Pfad der Datei
FILE_PATH="/d/Projekte/Privat/Sereno/sereno.client/src/assets/scss/paper-dashboard/_sereno.scss"

# Entferne den spezifizierten Text aus der Datei
sed -i '/url("..\//fonts\/sereno.svg\?[^"]*#sereno") format("svg")/d' "$FILE_PATH"
