const backgrounds = ["bg1.jpg", "bg2.jpg", "bg3.jpg"]
let lastBackground = "";
let allowedDepts = [];
let numOfCharacter;

function chooseBackground() {
  let index;

  do {
    index = Math.floor(Math.random() * backgrounds.length);
  } while (backgrounds[index] === lastBackground);

  lastBackground = backgrounds[index];
  let imageUrl = `nui://framework/html/imgs/${lastBackground}`

  let bgImage = new Image();
  bgImage.src = imageUrl;

  $('body').css('background-image', `url("${imageUrl}")`);
}

// Setups and modals
function setupAndDisplayErrorModal(error, title) {
  $('.modal').modal('hide');
  $('#errorMsg').text(error);
  $('.modal-title', $('#errorModal')).text(title);
  $('#errorModal').modal('show');
}

function setupAndDisplaySpawnModal(department) {
  switch(department) {
    case 'police':
      $('#spawnModalPolice').modal('show')
      break;

    case 'fire':
      $('#spawnModalFire').modal('show')
      break;

    case 'civ':
      $('#spawnModalCiv').modal('show')
      break;
  }
}

function hideSpawnModal(department) {
  switch(department) {
    case 'police':
      $('#spawnModalPolice').modal('hide');
      break;

    case 'fire':
      $('#spawnModalFire').modal('hide');
      break;

    case 'civ':
      $('#spawnModalCiv').modal('hide')
      break;
  }
}

function setupAndDisplayEditCharacterModal(firstName, lastName, dateOfBirth, gender, department, characterId) {
  const $editCharFirstName = $('#editCharFirstName');
  const $editCharLastName = $('#editCharLastName');
  const $editCharGender = $('#editCharGender');
  const $editCharDept = $('#editCharDept');
  const $editCharHeader = $('#editCharHeader');
  const $editCharDoB = $('#editCharDOB');
  const $editModalFooter = $('#editModalFooter');

  $editCharFirstName.val(firstName);
  $editCharLastName.val(lastName);
  $editCharHeader.text(`Editing: ${firstName} ${lastName} (${department})`);

  $editCharDoB.datepicker({
    format: 'mm/dd/yyyy',
    autoclose: true
  }).datepicker('setDate', new Date(dateOfBirth));

  $editCharDept.empty().append('<option selected disabled value="0">Select Department</option>');
  let deptOptions = allowedDepts.map(dept => `<option value="${dept}">${dept}</option>`).join('');
  $editCharDept.append(deptOptions).val(department.toUpperCase()).trigger('change');

  $editCharGender.empty().append('<option selected disabled>Select Gender</option>');
  const genderOptions = {
    'Male': 'Male',
    'Female': 'Female',
    'Other': 'Other'
  };
  const genderKeys = Object.keys(genderOptions);
  const genderOptionsHtml = genderKeys.map((key) => `<option value="${genderOptions[key]}">${key}</option>`).join('');
  $editCharGender.append(genderOptionsHtml).val(genderOptions[gender] || '').trigger('change');

  $('#editCharIdHandlerHidden').val(characterId);

  $editModalFooter.empty();
  $editModalFooter.append('<button type="button" class="btn btn-primary" onclick="validateAndEditCharacter()">Save Character</button>');
  $editModalFooter.append('<button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>');

  $('#editChar').modal('show');
}

function setupAndDisplayCreateCharacterModal() {
  const $createCharFirstName = $('#createCharFirstName');
  const $createCharLastName = $('#createCharLastName');
  const $createCharCash = $('#createCharCash');
  const $createCharBank = $('#createCharBank');
  const $createCharGender = $('#createCharGender');
  const $createCharDept = $('#createCharDept');

  $createCharFirstName.val('');
  $createCharLastName.val('');
  $createCharCash.val('');
  $createCharBank.val('');
  $('#datepicker').val('').datepicker('update');
  $createCharGender.val('').trigger('change');

  $createCharDept.empty();
  $createCharDept.append('<option selected disabled value="0">Select Department</option>');

  for (let i = 0; i < allowedDepts.length; i++) {
    $createCharDept.append(`<option value="${allowedDepts[i]}">${allowedDepts[i]}</option>`);
  }

  $createCharDept.trigger('change');

  $('#createChar').modal('show');
}

function setupAndDisplayDeleteModal(fn, ln, charid, dept) {
  $('#editChar').modal('hide');
  $('#confDeleteMessage').text(`Are you sure you wish to delete ${fn} ${ln} (${dept})? We won't be able to help you recover this character.`);
  $('#confDeleteModalFooter').empty();
  $('#confDeleteModalFooter').append('<button type="button" class="btn btn-danger mr-auto" onclick="deleteCharacter(' + charid + ')">Delete</button>');
  $('#confDeleteModalFooter').append('<button type="button" class="btn btn-warning" data-bs-dismiss="modal">Cancel</button>');
  $('#confDeleteModal').modal('show');
}

// Framework Functions
function playAsCharacter(fn, ln, cash, bank, gender, department, dob) {
  $.post('https://framework/selectCharacter', JSON.stringify({
    firstName: fn,
    lastName: ln,
    gender: gender,
    cash: cash.toString(),
    bank: bank.toString(),
    dob: dob,
    department: department
  }));
}

function createCharacter(fn, ln, cash, bank, gender, department, dob) {
  $('#createChar').modal('hide');

  $.post("https://framework/createCharacter", JSON.stringify({
    firstName: fn,
    lastName: ln,
    gender: gender,
    cash: cash,
    bank: bank,
    department: department,
    dob: dob
  }));
}

function editCharacter(fn, ln, gender, department, dob) {
  $('#editChar').modal('hide');

  $.post('https://framework/editCharacter', JSON.stringify({
    firstName: fn,
    lastName: ln,
    gender: gender,
    department: department,
    dob: dob,
    charId: $('#editCharIdHandlerHidden').val()
  }));
}

function deleteCharacter(charId) {
  $('#confDeleteModal').modal('hide');
  $.post("https://framework/deleteCharacter", JSON.stringify({
    characterId: charId.toString()
  }));
}

function validateAndCreateCharacter() {
  const emptyOrWhitespaceRegex = /^\s*$/;
  const alphabeticRegex = /^[A-Za-z]+$/;

  const firstNameInput = $('#createCharFirstName');
  const lastNameInput = $('#createCharLastName');
  const cashInput = $('#createCharCash');
  const bankInput = $('#createCharBank');
  const dobInput = $('#createCharDOB');
  const genderInput = $('#createCharGender');
  const departmentInput = $('#createCharDept');

  const firstName = firstNameInput.val().trim();
  const lastName = lastNameInput.val().trim();
  const cash = cashInput.val().trim();
  const bank = bankInput.val().trim();
  const dob = dobInput.val().trim();
  const gender = genderInput.val();
  const department = departmentInput.val();

  if (emptyOrWhitespaceRegex.test(firstName)) {
    setupAndDisplayErrorModal('Please enter a first name for this character!', 'Error!');
    return;
  } else if (!alphabeticRegex.test(firstName)) {
    setupAndDisplayErrorModal('First name should contain only alphabetic characters!', 'Error!');
    return;
  }

  if (emptyOrWhitespaceRegex.test(lastName)) {
    setupAndDisplayErrorModal('Please enter a last name for this character!', 'Error!');
    return;
  } else if (!alphabeticRegex.test(lastName)) {
    setupAndDisplayErrorModal('Last name should contain only alphabetic characters!', 'Error!');
    return;
  }

  const capitalizedFirstName = firstName.charAt(0).toUpperCase() + firstName.slice(1);
  const capitalizedLastName = lastName.charAt(0).toUpperCase() + lastName.slice(1);

  if (emptyOrWhitespaceRegex.test(cash)) {
    setupAndDisplayErrorModal('Please enter a starting cash value between 0 and 15000 for this character!', 'Error!');
    return;
  } else if (isNaN(Number(cash)) || Number(cash) < 0 || Number(cash) > 15000) {
    setupAndDisplayErrorModal('Starting cash value should be a number between 0 and 15000!', 'Error!');
    return;
  }

  if (emptyOrWhitespaceRegex.test(bank)) {
    setupAndDisplayErrorModal('Please enter a starting bank value between 0 and 15000 for this character!', 'Error!');
    return;
  } else if (isNaN(Number(bank)) || Number(bank) < 0 || Number(bank) > 15000) {
    setupAndDisplayErrorModal('Starting bank value should be a number between 0 and 15000!', 'Error!');
    return;
  }

  if (emptyOrWhitespaceRegex.test(dob) || isNaN(new Date(dob))) {
    setupAndDisplayErrorModal('Please use the datepicker to choose a valid DoB for this character!', 'Error!');
    return;
  }

  const currentDate = new Date();
  const selectedDate = new Date(dob);

  if (selectedDate > currentDate) {
    setupAndDisplayErrorModal('The selected date of birth is in the future!', 'Error!');
    return;
  }

  if (gender === null) {
    setupAndDisplayErrorModal('Please select a valid gender for this character!', 'Error!');
    return;
  }

  if (department === null) {
    setupAndDisplayErrorModal('Please select a valid department for this character!', 'Error!');
    return;
  }

  createCharacter(capitalizedFirstName, capitalizedLastName, cash, bank, gender, department, dob);
}

function validateAndEditCharacter() {
  const firstNameInput = $('#editCharFirstName');
  const lastNameInput = $('#editCharLastName');
  const dobInput = $('#editCharDOB');
  const genderInput = $('#editCharGender');
  const departmentInput = $('#editCharDept');

  const firstName = firstNameInput.val().trim();
  const lastName = lastNameInput.val().trim();
  const dob = dobInput.val().trim();
  const gender = genderInput.val();
  const department = departmentInput.val();

  const emptyOrWhitespaceRegex = /^\s*$/;
  const alphabeticRegex = /^[A-Za-z]+$/;

  if (emptyOrWhitespaceRegex.test(firstName)) {
    setupAndDisplayErrorModal('Please enter a first name for this character!', 'Error!');
    return;
  } else if (!alphabeticRegex.test(firstName)) {
    setupAndDisplayErrorModal('First name should contain only alphabetic characters!', 'Error!');
    return;
  }

  if (emptyOrWhitespaceRegex.test(lastName)) {
    setupAndDisplayErrorModal('Please enter a last name for this character!', 'Error!');
    return;
  } else if (!alphabeticRegex.test(lastName)) {
    setupAndDisplayErrorModal('Last name should contain only alphabetic characters!', 'Error!');
    return;
  }

  const capitalizedFirstName = firstName.charAt(0).toUpperCase() + firstName.slice(1);
  const capitalizedLastName = lastName.charAt(0).toUpperCase() + lastName.slice(1);

  if (emptyOrWhitespaceRegex.test(dob) || isNaN(new Date(dob))) {
    setupAndDisplayErrorModal('Please use the datepicker to choose a valid DoB for this character!', 'Error!');
    return;
  }

  const currentDate = new Date();
  const selectedDate = new Date(dob);

  if (selectedDate > currentDate) {
    setupAndDisplayErrorModal('The selected date of birth is in the future!', 'Error!');
    return;
  }

  if (emptyOrWhitespaceRegex.test(gender) || gender === null) {
    setupAndDisplayErrorModal('Please select a valid gender for this character!', 'Error!');
    return;
  }

  if (emptyOrWhitespaceRegex.test(department) || department === null) {
    setupAndDisplayErrorModal('Please select a valid department for this character!', 'Error!');
    return;
  }

  editCharacter(capitalizedFirstName, capitalizedLastName, gender, department, dob);
}

function setupCharacters(characters) {
  const $charList = $("#charList");
  $charList.empty();

  numOfCharacters = characters.length;

  characters.forEach(character => {
    let {
      Department,
      FirstName,
      LastName,
      CharacterId,
      DoB,
      Gender,
      Cash,
      Bank
    } = character;
    if (Department !== null && Department !== undefined) {
      if (Department == "CIV") Department = "Civ";

      const $listItem = $("<li>").addClass("characterItem");
      const $playButton = $("<button>")
        .addClass("btn btn-primary")
        .text(`Play As: ${FirstName} ${LastName} (${Department})`)
        .click(() => {
          playAsCharacter(FirstName, LastName, Cash, Bank, Gender, Department, DoB);
        });
      const $editButton = $("<button>")
        .addClass("btn btn-success")
        .text("Edit")
        .click(() => {
          setupEditCharacterModal(FirstName, LastName, DoB, Gender, Department, CharacterId);
      });

      $listItem.append($playButton, $editButton);
      $listItem.append(`<button type="button" class="btn btn-danger" onclick="setupAndDisplayDeleteModal('${FirstName}', '${LastName}', '${CharacterId}', '${Department}')"><i class="fa-solid fa-trash-can"></i></button>`);
      $charList.append($listItem);
    }
  });

  $("#createCharacterButton").remove();

  if (numOfCharacters < 99999999999999999999) {
    const createCharacterButton = $('<button>')
      .text('New Character')
      .attr('id', 'createCharacterButton')
      .addClass('create-btn-hover color-1')
      .click(function() {
        setupAndDisplayCreateCharacterModal();
      });

    const $createCharacterContainer = $('<div>')
      .addClass('text-center')
      .append(createCharacterButton);

    $("#mainBody").append($createCharacterContainer);
  }
}

// Misc Functions
function showModal(modalId) {
  $(`#${modalId}`).modal('show');
}

function hideModal(modalId) {
  $(`#${modalId}`).modal('hide');
}

function closeNui() {
  $.post('https://framework/closeFrameworkNui')
}

// NUI Handlers
$(function() {
  window.addEventListener('message', function(event) {
    if (event.data.type === 'SHOW_FRAMEWORK_NUI') {
      chooseBackground();

      allowedDepts = event.data.departments;
      $('#titleHeader').text(event.data.aop);
      
      $('body').fadeIn(300, function() {
        $('body').css('display', 'block');
      });

      setupCharacters(event.data.characters);
      $('#mainBody').css('display', 'block');

    } else if (event.data.type === 'CLOSE_FRAMEWORK_NUI') {
      $('body').fadeOut(300, function() {
        $('body').css('display', 'none');
      });

    } else if (event.data.type === 'FRAMEWORK_SUCCESS') {
      $('#successMsg').text(event.data.msg);
      $('#successModal').modal('show');

    } else if (event.data.type === 'FRAMEWORK_ERROR') {
      $('#errorMsg').text(event.data.msg);
      $('#errorModal').modal('show');

    } else if (event.data.type === 'FRAMEWORK_UPDATE_AOP') {
      $('#titleHeader').text(event.data.aop);

    }
  });

  $('.datepicker').datepicker({
    format: 'mm/dd/yyyy',
    autoclose: true
  });
});