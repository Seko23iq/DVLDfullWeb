// ══════════════════════════════════════════════════════
//  editUser.js  —  النسخة المصحّحة
// ══════════════════════════════════════════════════════

// ── متغيرات عامة معرّفة بشكل صحيح ───────────────────
let currentPersonId = null;
let currentUserId   = null;

// ── 1. مسح أخطاء الفورم ──────────────────────────────
function clearFormErrors() {
    ['formUsername', 'formPassword', 'formConfirm'].forEach(id => {
        document.getElementById(id)?.classList.remove('error');
    });
    ['usernameErr', 'passwordErr', 'confirmErr'].forEach(id => {
        document.getElementById(id)?.classList.add('hidden');
    });
}

// ── 2. عرض بيانات الشخص في الكارد والفورم ────────────
function searchPerson(person) {
    const card     = document.getElementById('personInfoCard');
    const notFound = document.getElementById('notFoundMsg');
    const form     = document.getElementById('addUserForm');

    // إعادة تعيين
    card.classList.add('hidden');
    notFound.classList.add('hidden');
    form.classList.add('hidden');
    clearFormErrors();

    if (!person) {
        notFound.classList.remove('hidden');
        currentPersonId = null;
        return;
    }

    // ملء كارد المعلومات
    const fullName = [person.firstName, person.secondName, person.thirdName, person.lastName]
        .filter(Boolean)
        .join(' ');

    document.getElementById('infoId').textContent       = person.personID;
    document.getElementById('infoName').textContent     = fullName;
    document.getElementById('infoNational').textContent = person.nationalNo  || '—';
    document.getElementById('infoGender').textContent   = person.gender      || '—';
    document.getElementById('infoEmail').textContent    = person.email       || '—';
    document.getElementById('infoAddress').textContent  = person.address     || '—'; // ✅ تم إصلاحه
    document.getElementById('infoDOB').textContent      = formatDate(person.dateOfBirth);
    document.getElementById('infoCountry').textContent  = person.nationality || '—';

    const img = document.querySelector(".personImage input[type='image']");
    if (img && person.imagePath) {
        img.src = "https://localhost:7223" + person.imagePath;
        img.alt = fullName;
    }
    card.classList.remove('hidden');

    // ملء حقل Person ID في الفورم
    document.getElementById('formPersonId').value = person.personID;

    form.classList.remove('hidden');

    currentPersonId = person.personID;

    // تمرير سلس للفورم
    setTimeout(() => form.scrollIntoView({ behavior: 'smooth', block: 'start' }), 100);
    }

    // ── 3. جلب بيانات الشخص من API ───────────────────────
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

    // ── 4. قراءة ID المستخدم من URL ──────────────────────
    function getUserID() {
    const params = new URLSearchParams(window.location.search);
    return params.get('id');
    }

    // ── 5. تحميل بيانات المستخدم الحالي ──────────────────
    async function loadUserInfo() {
    const userID = localStorage.getItem("UserID");

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

        // ✅ await مضاف لضمان تحميل الفورم قبل ملء القيم
        await loadPersonInfoInEditUser(personID);

        // ✅ ملء حقل اسم المستخدم فقط — لا نعرض كلمة المرور أبداً
        renderUserInfo(user);

        const input = document.getElementById("formUsername");
        input.disabled = true;
        input.setAttribute("readonly", true);

    } catch (error) {
        console.error('Error loading user:', error);
        showToast('فشل تحميل بيانات المستخدم.', 'error');
    }
    }

    // ── 6. ملء بيانات المستخدم في الفورم ─────────────────
    function renderUserInfo(user) {
    setFieldValue('#formUsername', user.user?.userName);
    // ✅ كلمة المرور لا تُعرض — يجب على المستخدم إدخال كلمة مرور جديدة
    document.getElementById('formPassword').value  = '';
    document.getElementById('formConfirm').value   = '';
    }

    // ── 7. Helper: تعيين قيمة حقل ────────────────────────
    function setFieldValue(selector, value) {
    const el = document.querySelector(selector);
    if (el && value !== undefined && value !== null) {
        el.value = value;
    }
    }

    // ── 8. التحقق من صحة الفورم ──────────────────────────
    function validateForm() {
    let valid = true;

    // const username = document.getElementById('formUsername').value.trim();
    const password = document.getElementById('formPassword').value;
    const confirm  = document.getElementById('formConfirm').value;

    // Password — فقط إذا أدخل المستخدم قيمة جديدة
    if (password && password.length < 6) {
        document.getElementById('formPassword').classList.add('error');
        document.getElementById('passwordErr').classList.remove('hidden');
        valid = false;
    }

    // Confirm Password
    if (password && password !== confirm) {
        document.getElementById('formConfirm').classList.add('error');
        document.getElementById('confirmErr').classList.remove('hidden');
        valid = false;
    }

    return valid;
}

// ── 9. إرسال الفورم (تحديث المستخدم) ────────────────
async function submitForm() {
    clearFormErrors();
    if (!validateForm()) return;

    const username = document.getElementById('formUsername').value.trim();
    const password = document.getElementById('formPassword').value;

    const body = { userName: username };
    if (password) body.password = password; // إرسال كلمة المرور فقط إذا تم إدخالها

    // console.log(currentUserId);
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

// ── 10. إعادة تعيين الصفحة ───────────────────────────
function resetAll() {
    document.getElementById('personInfoCard').classList.add('hidden');
    document.getElementById('notFoundMsg').classList.add('hidden');
    document.getElementById('addUserForm').classList.add('hidden');
    clearFormErrors();
    currentPersonId = null;
}

// ── 11. عرض Toast ─────────────────────────────────────
function showToast(message, type = 'success') {
    const toast = document.getElementById('toast');
    if (!toast) return;

    toast.textContent = message;
    toast.className   = `toast toast--${type} show`;

    setTimeout(() => {
        toast.classList.remove('show');
    }, 3000);
}

// ── 12. Helper: تنسيق التاريخ ─────────────────────────
function formatDate(dateStr) {
    if (!dateStr) return '—';
    const date = new Date(dateStr);
    if (isNaN(date)) return dateStr;
    const day   = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year  = date.getFullYear();
    return `${day}/${month}/${year}`;
}

// ── التهيئة عند تحميل الصفحة ─────────────────────────
window.addEventListener('load', loadUserInfo);