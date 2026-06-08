(function (window) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Utils = WIP.Utils || {};

    function isBase64Image(value) {
        if (!value) return false;
        return /^data:image\/(png|jpeg|jpg|gif|webp);base64,/i.test(String(value));
    }

    function imageToBase64(imageSrc, imageType, quality) {
        return new Promise(function (resolve) {
            if (!imageSrc) {
                resolve("");
                return;
            }

            var img = new Image();
            img.crossOrigin = "Anonymous";

            img.onload = function () {
                var canvas = document.createElement("canvas");
                canvas.width = img.width;
                canvas.height = img.height;

                var ctx = canvas.getContext("2d");
                ctx.drawImage(img, 0, 0);
                resolve(canvas.toDataURL(imageType || "image/jpeg", quality || 0.8));
            };

            img.onerror = function () {
                resolve("");
            };

            img.src = imageSrc;
        });
    }

    async function getImageByField(imageSrc) {
        if (!imageSrc) return "";
        if (isBase64Image(imageSrc)) return imageSrc;
        return await imageToBase64(imageSrc);
    }

    WIP.Utils.Image = {
        isBase64Image: isBase64Image,
        imageToBase64: imageToBase64,
        getImageByField: getImageByField
    };
})(window);
