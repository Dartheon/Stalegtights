from krita import Krita, QColor, QImage

# Settings
LOW_ALPHA = 1      # Lower bound of semi-transparent range
HIGH_ALPHA = 254   # Upper bound of semi-transparent range
HIGHLIGHT_COLOR = QColor(0, 255, 0, 200)  # bright green overlay

app = Krita.instance()
doc = app.activeDocument()

if not doc:
    raise RuntimeError("No active document found.")

node = doc.activeNode()
if not node:
    raise RuntimeError("No active layer selected.")

# Get bounds of the layer (not whole doc!)
bounds = node.bounds()
w, h = bounds.width(), bounds.height()

# Create new paint layer above current layer
highlight_layer = doc.createNode("SemiTransparent_Pixels", "paintlayer")
parent = node.parentNode()
parent.addChildNode(highlight_layer, node)  # insert above current layer

# Extract raw pixel data (BGRA format)
data = node.pixelData(bounds.x(), bounds.y(), w, h)

# Make QImage to manipulate pixels
image = QImage(data, w, h, QImage.Format_ARGB32)

# Highlight QImage
highlight_img = QImage(w, h, QImage.Format_ARGB32)
highlight_img.fill(0)  # fully transparent

for y in range(h):
    for x in range(w):
        pixel = image.pixelColor(x, y)
        alpha = pixel.alpha()
        if LOW_ALPHA <= alpha <= HIGH_ALPHA:
            highlight_img.setPixelColor(x, y, HIGHLIGHT_COLOR)

# Write highlight pixels into the new layer
highlight_layer.setPixelData(
    highlight_img.bits().asstring(highlight_img.sizeInBytes()),
    bounds.x(), bounds.y(), w, h
)

doc.refreshProjection()
print("✅ Semi-transparent pixels highlighted above current layer!")
