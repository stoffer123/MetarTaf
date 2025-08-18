window.playAmdAlert = function () {
    var audio = new Audio('/sounds/amdAlert.mp3');
    audio.play().catch(e => console.log("Autoplay prevented:", e));
};