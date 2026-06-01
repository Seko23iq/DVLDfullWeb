import {formatDate, showToast} from "./Global.js"


let currentPersonId = null;
let currentUserId   = null;

function clearFormErrors() {
    ['formUsername', 'formPassword', 'formConfirm'].forEach(id => {
        document.getElementById(id)?.classList.remove('error');
    });
    ['usernameErr', 'passwordErr', 'confirmErr'].forEach(id => {
        document.getElementById(id)?.classList.add('hidden');
    });
}

function searchPerson(person) 
{
    const card     = document.getElementById('personInfoCard');
    const notFound = document.getElementById('notFoundMsg');
    const form     = document.getElementById('addUserForm');

    card.classList.add('hidden');
    notFound.classList.add('hidden');
    form.classList.add('hidden');
    clearFormErrors();

    if (!person) {
        notFound.classList.remove('hidden');
        currentPersonId = null;
        return;
    }

    const fullName = [person.firstName, person.secondName, person.thirdName, person.lastName]
        .filter(Boolean)
        .join(' ');

    document.getElementById('infoId').textContent       = person.personID;
    document.getElementById('infoName').textContent     = fullName;
    document.getElementById('infoNational').textContent = person.nationalNo  || '—';
    document.getElementById('infoGender').textContent   = person.gender      || '—';
    document.getElementById('infoEmail').textContent    = person.email       || '—';
    document.getElementById('infoAddress').textContent  = person.address     || '—'; 
    document.getElementById('infoDOB').textContent      = formatDate(person.dateOfBirth);
    document.getElementById('infoCountry').textContent  = person.nationality || '—';

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

    form.classList.remove('hidden');

    currentPersonId = person.personID;

    setTimeout(() => form.scrollIntoView({ behavior: 'smooth', block: 'start' }), 100);
    }

async function loadPersonInfoInEditUser(personID) {
    if (!personID) {
        console.error('No person ID provided.');
        return;
    }

    try {
        const response = await fetch(`https://localhost:7223/api/People/PeopleInfo/${personID}`);
        if (!response.ok) throw new Error('Person not found');

        const person = await response.json();
        searchPerson(person);
        } catch (error) {
            console.error('Error loading person:', error);
            searchPerson(null); // عرض رسالة "not found"
        }
    }

    function getUserID() {
    const params = new URLSearchParams(window.location.search);
    return params.get('id');
    }

async function loadUserInfo() {
    const userID = getUserID();

    if (!userID) {
        showToast('لا يوجد مستخدم بهذا المعرّف.', 'error');
        return;
    }

    currentUserId = userID;

    try {
        const response = await fetch(`https://localhost:7223/api/Users/UserInfo/${userID}`);
        if (!response.ok) throw new Error('User not found!');

        const user = await response.json();
        const personID = user.person?.personID;

        await loadPersonInfoInEditUser(personID);

        renderUserInfo(user);

    } catch (error) {
        console.error('Error loading user:', error);
        showToast('فشل تحميل بيانات المستخدم.', 'error');
    }
}

function renderUserInfo(user) 
{
    setFieldValue('#formUsername', user.user?.userName);
    setFieldValue('#formIsActive', String(user.user?.isActive));

    document.getElementById('formPassword').value  = '';
    document.getElementById('formConfirm').value   = '';
}

function setFieldValue(selector, value) {
const el = document.querySelector(selector);
if (el && value !== undefined && value !== null) {
    el.value = value;
}
}

function validateForm() {
    let valid = true;
    const username = document.getElementById('formUsername').value.trim();
    const password = document.getElementById('formPassword').value;
    const confirm  = document.getElementById('formConfirm').value;
    if (!username) {
        document.getElementById('formUsername').classList.add('error');
        document.getElementById('usernameErr').classList.remove('hidden');
        valid = false;
    }
    if (password && password.length < 6) {
        document.getElementById('formPassword').classList.add('error');
        document.getElementById('passwordErr').classList.remove('hidden');
        valid = false;
    }
    if (password && password !== confirm) {
        document.getElementById('formConfirm').classList.add('error');
        document.getElementById('confirmErr').classList.remove('hidden');
        valid = false;
    }

    return valid;
}

const btnSubmit = document.querySelector(".btn-submit");

btnSubmit.addEventListener("click", () => {
    submitForm();
})

async function submitForm() {
    clearFormErrors();
    if (!validateForm()) return;

    const username = document.getElementById('formUsername').value.trim();
    const password = document.getElementById('formPassword').value;
    let isActive = document.getElementById('formIsActive').value;

    const body = { userName: username };
    if (password) body.password = password; 

    if(isActive === "true")
        isActive = 1;
    else 
        isActive = 0;

    body.isActive = isActive;

    try {
        const response = await fetch(`https://localhost:7223/api/Users/UpdateUser/${currentUserId}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body),
        });

        if (!response.ok) throw new Error('Update failed');

        showToast('تم تحديث المستخدم بنجاح!', 'success');
        setTimeout(() => {
            window.location.href = "/Sections/Users/mainUser.html";
        }, 1500);
    } catch (error) {
        console.error('Submit error:', error);
        showToast('فشل تحديث المستخدم.', 'error');
    }
}

function resetAll() {
    document.getElementById('personInfoCard').classList.add('hidden');
    document.getElementById('notFoundMsg').classList.add('hidden');
    document.getElementById('addUserForm').classList.add('hidden');
    clearFormErrors();
    currentPersonId = null;
}
window.addEventListener('load', loadUserInfo);