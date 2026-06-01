import {formatDate, showToast} from "./Global.js"

let currentPersonId = null;
const btnSearch = document.getElementById("btnSearch");




async function loadPersonInfo() {
    const personID = document.getElementById('personIdInput').value.trim();

    if (!personID) {
        showToast("No person ID provided.", "error");
        return;
    }

    // GET
    try {
        const response = await fetch(`https://localhost:7223/api/People/PeopleInfo/${personID}`);

        if (!response.ok) throw new Error("Person not found");

        const person = await response.json();

        searchPerson(person);

    } catch (error) {
        console.error("Error : " + error);
        showToast("Error occur while handling the request!", "error");
    }
}

btnSearch.addEventListener("click", () => {

  loadPersonInfo();
});

function searchPerson(person) {
    const card  = document.getElementById('personInfoCard');
    const notFound = document.getElementById('notFoundMsg');
    const form  = document.getElementById('addUserForm');

    // Reset
    card.classList.add('hidden');
    notFound.classList.add('hidden');
    form.classList.add('hidden');
    clearFormErrors();
    
    
    if (!person) {
      notFound.classList.remove('hidden');
      currentPersonId = null;
      return;
    }
    
    const fullName = [person.firstName, person.secondName, person.thirdName, person.lastName].filter(Boolean).join(" ");
    document.getElementById('infoId').textContent       = person.personID;
    document.getElementById('infoName').textContent     = fullName;
    document.getElementById('infoNational').textContent = person.nationalNo;
    document.getElementById('infoAddress').textContent = person.address;
    document.getElementById('infoGender').textContent   = person.gender;
    document.getElementById('infoEmail').textContent    = person.email;
    document.getElementById('infoDOB').textContent      = formatDate(person.dateOfBirth);
    document.getElementById('infoCountry').textContent  = person.nationality  || "—" ;

    const img = document.querySelector(".personImage input[type='image']");
    if (img && person.imagePath) {
        img.src = "https://localhost:7223" + person.imagePath;
        img.alt = fullName;
    } else 
    {
        img.src = "/Images/defaultImage.webp"
        img.alt = "There is no image.";
    }


    card.classList.remove('hidden');
    document.getElementById('formPersonId').value = person.personID;
    document.getElementById('formUsername').value = '';
    document.getElementById('formPassword').value = '';
    document.getElementById('formConfirm').value  = '';
    form.classList.remove('hidden');

    currentPersonId = person.personID;

    setTimeout(() => form.scrollIntoView({ behavior: 'smooth', block: 'start' }), 100);
}

document.getElementById('personIdInput').addEventListener('keydown', e => {
    if (e.key === 'Enter') loadPersonInfo();
});

function clearFormErrors() {
  ['formUsername','formPassword','formConfirm'].forEach(id => {
    document.getElementById(id)?.classList.remove('error');
  });
  ['usernameErr','passwordErr','confirmErr'].forEach(id => {
    document.getElementById(id)?.classList.add('hidden');
  });
}

function buildPayload ()
{
      let valid = true;

    const personID = document.getElementById('personIdInput').value.trim();
    const username = document.getElementById('formUsername').value.trim();
    const password = document.getElementById('formPassword').value;
    const confirm  = document.getElementById('formConfirm').value;
    let IsActive  = document.getElementById('formIsActive').value;

    if (!username) {
      document.getElementById('formUsername').classList.add('error');
      document.getElementById('usernameErr').classList.remove('hidden');
      valid = false;
    }

    if (password.length < 6) {
      document.getElementById('formPassword').classList.add('error');
      document.getElementById('passwordErr').classList.remove('hidden');
      valid = false;
    }

    if (password !== confirm) {
      document.getElementById('formConfirm').classList.add('error');
      document.getElementById('confirmErr').classList.remove('hidden');
      valid = false;
    }

    if(IsActive === "true")
      IsActive = 1;
    else
      IsActive = 0;

    if (!valid) return;
    return {
      PersonID : personID,
      username : username,
      password : password,
      IsActive : IsActive
    }
}

const btnSubmit = document.querySelector(".btn-submit");

btnSubmit.addEventListener("click", ()=> {
  submitForm()
});


async function submitForm() {
    clearFormErrors();

    try {
        const payload = buildPayload();
        const API_URL = "https://localhost:7223/api/Users/AddUser";

        const response = await fetch(API_URL, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload),
        });

        if (response.ok) {
            showToast("User added successfully!", "success");
            setTimeout(() => {
                window.location.href = "/Sections/Users/mainUser.html";
            }, 1400);
        } else {
            let msg = "User already exists!";
            try {
                const errBody = await response.json();
                msg = errBody.message ?? errBody.title ?? msg;
            } catch {  }

            showToast(msg, "error");
        }

    } catch (error) {
        console.error("Add User Error:", error);
        showToast(error.message ?? "Network error. Please try again.", "error");
    }
}
function resetAll() {
  document.getElementById('personIdInput').value = '';
  document.getElementById('personInfoCard').classList.add('hidden');
  document.getElementById('notFoundMsg').classList.add('hidden');
  document.getElementById('addUserForm').classList.add('hidden');
  clearFormErrors();
  currentPersonId = null;
  window.scrollTo({ top: 0, behavior: 'smooth' });
}
