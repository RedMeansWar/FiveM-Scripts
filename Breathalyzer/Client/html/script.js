function startBac() {
    $.post('https://bac/startTest');
}

function resetBac() {
    $.post('https://bac/resetTest');
}

function cancelNui() {
    $.post('https://bac/closeNui');
}

$(function () {
    window.addEventListener('message', function (event) {
        let type = event.data.type;

        switch (type) {
            case "DISPLAY_BREATHALYZER_NUI":
                $('#breathalyzer').fadeIn(300, function () {
                    $('#breathalyzer').css('display', 'block');
                });
                break;

            case "CLOSE_BREATHALYZER_NUI":
                $('#breathalyzer').fadeOut(300, function () {
                    $('#breathalyzer').css('display', 'none');
                });
                break;

            case "RESET_BREATHALYZER_NUI":
                $('#baclevel').text('0.00');
                break;

            case "UPDATE_BAC_LEVEL":
                $('#baclevel').text(event.data.level);
                break;
        }
    });
});

document.onkeyup = function (data) {
    if (data.key === 27) { // key 27 is the 'ESC' key.
        $.post('https://bac/closeNui');
        return;
    }
}