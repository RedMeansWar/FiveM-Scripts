function showModal(modalId) {
    $(`#{modalId}`).modal('show');
}

function hideModal(modalId) {
    $(`#{modalId}`).modal('hide');
}

function closeAtmNui() {
    $.post('https://economy/closeAtmNui');
}



$(function() {
    window.addEventListener('message', function(event) {
        switch (event.data.type) {
            case "ECONOMY_DISPLAY_ATM": break;
        }
    })
});