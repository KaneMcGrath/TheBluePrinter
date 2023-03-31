# The Blue Printer

The Blue Printer is a Windows desktop app for the game Factorio. It allows you to convert an image into a blueprint string of a "Belt Printer".  This will use requester chests to gather the items and assemble the image onto a belt.  This will then merge onto a bus that is already carrying items.
Each pixel of the image will be converted to an item that closely matches the color of the pixel.

## Usage

- Specify the path to your Factorio game folder (optional).
- Add an image under "Image source path". This can be an image from your computer or an icon from the game's path.
  - If you're using an icon from the game, you can format it so that the different resolutions in the image are cut out and you are left with a centered square image at the resolution you want.
  - If you're using an image from your computer, you can resize it. I recommend that you resize it to be very small (less than 32x32) because the resolution of the image will affect how large the image in the game will be, and larger images can be difficult to import or may crash the game.
- Change the color of the alpha channel of the image (optional). This affects what item will be used to fill in the empty space.  If you want your image to match the belt it will merge onto, choose a color that will match that item (I may make an item selector later on)
- Click "Generate Printer" to create the blueprint string.
- Click "Copy" to Copy the result blueprint string to your clipboard.
- Click "Generate Preview" to see a preview of the image.
- Adjust the resolution for icons to be used in the preview image using the slider.
  - If you are using a large source image, you should set this lower

### Known Issue

If you generate a large printer (about 50x50 or larger), the substation that connects all the combinators and front belts will not be connected to the rest of the printer. To fix this, you will need to connect a red wire to these two substations. See the included image for a demonstration.
I have no idea why this happens, you will have to reconnect it back manually

## Item Selection

The Item Selection feature allows you to choose which items from the game will be used by the printer when making the image.
Usage

- Click on an item in the "Allowed Items" or "Disallowed Items" column to select it.
- Use the "Move Selected" button to move items from one column to another.
- Enable "Click to move" to move an item immediately by clicking it.
- Only items in the "Allowed Items" column will be used.

## Convert Blueprint

The Convert Blueprint feature allows you to convert a blueprint into JSON format or convert a JSON-formatted blueprint into a blueprint string.
Usage

- To convert a blueprint to JSON format, paste the blueprint string in the left-hand side of the window and click "Convert to JSON".
- To convert a JSON-formatted blueprint to a blueprint string, paste the JSON string in the right-hand side of the window and click "Convert to Blueprint".
