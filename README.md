# PreviewProvider
Runtime Icon (previews) generation for Unity

![Screenshot_1](https://github.com/defaxer/PreviewProvider/assets/3694068/6cb26cbd-6a53-4001-93f8-eb10460d0dad)

## Preview generation in few simple steps:
    1. Create Canvas with RawImage
    2. Add new Script
    3. Ask PreviewProvider for object's preview with `PreviewProvider.RequestPreview(object obj, System.Action<Texture> callback)`
    4. When image is ready it'will be updated with provided callback
