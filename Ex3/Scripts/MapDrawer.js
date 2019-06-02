
/**
 * Normalize lon by canvas size
 * @param {any} lon the lon
 * @param {any} canvas the canvas
 */
function normalizeLon(lon, canvas) {
    return ((lon + 180) / 360) * canvas.width
}

/**
 * Normalize lat by canvas size
 * @param {any} lat the lat
 * @param {any} canvas the canvas
 */
function normalizeLat(lat, canvas) {
    return ((lat + 90) / 180) * canvas.height
}

/**
 * Draw a circle in position
 * @param {any} x center x
 * @param {any} y center y
 * @param {any} context the canvas context
 */
function drawCircle(x, y, context) {
    context.beginPath();
    context.arc(x, y, 10, 0, 2 * Math.PI, false);
    context.fillStyle = "#8ED6FF";
    context.fill();
    context.lineWidth = 1;
    context.strokeStyle = "black";
    context.stroke();
}

/**
 * Draw a dot on canvas
 * @param {any} lat the lat
 * @param {any} lon the lon
 */
function drawDot(lat, lon) {
    // Get canvas
    var canvas = document.getElementById("mapCanvas");

    // Get sizes
    canvas.width = $(window).width()
    canvas.height = $(window).height()

    var context = canvas.getContext("2d");

    // Draw a circle
    context.beginPath();

    var xpos = normalizeLon(lon, canvas)
    var ypos = normalizeLat(lat, canvas)
    drawCircle(xpos, ypos, context)
}

/**
 * Draw a path on canvas
 * @param {any} lonArr the lon array
 * @param {any} latArr the lat array
 */
function drawPath(lonArr, latArr) {
    // Get canvas
    var canvas = document.getElementById("mapCanvas");

    // Set its sizes
    canvas.width = $(window).width()
    canvas.height = $(window).height()

    var context = canvas.getContext("2d");

    // Clear last path
    context.clearRect(0, 0, canvas.width, canvas.height);

    // Add the path to canvas
    context.beginPath();
    context.moveTo(normalizeLon(lonArr[0], canvas), normalizeLat(latArr[0], canvas));
    for (i = 1; i < lonArr.length; i++) {
        context.lineTo(normalizeLon(lonArr[i], canvas), normalizeLat(latArr[i], canvas));
        context.stroke();
    }

    // Draw a circle
    var xpos = normalizeLon(lonArr[lonArr.length - 1], canvas)
    var ypos = normalizeLat(latArr[latArr.length - 1], canvas)
    drawCircle(xpos, ypos, context)
}
