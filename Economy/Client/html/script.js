var closestLoc = [];
var currentlyEditingName = null;
var editingBankAccountId = null;

function showModal(modalId) { $(`#{modalId}`).modal('show'); }
function hideModal(modalId) { $(`#${modalId}`).modal('hide'); }
function closeAtmNui() { $.post('https://economy/closeAtmNui');}

function setupAndDisplayCreateAccountModal() {
    $('')
}

$(function() {
    window.addEventListener('message', function(event) {
        switch (event.data.type) {
            case "ECONOMY_DISPLAY_ATM": break;
            case "ECONOMY_DISPLAY_TELLER": break;
            case "ECONOMY_PEEK_HUD": break;
            case "ECONOMY_CLOSE_NUI": break;
        }
    })
});