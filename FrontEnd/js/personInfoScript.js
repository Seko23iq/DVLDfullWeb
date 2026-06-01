// ─── personInfoScript.js ─────────────────────────────────────────

// ── 1. استخراج الـ ID من URL ──────────────────────────────────────
function getPersonIDFromURL() {
    const params = new URLSearchParams(window.location.search);
    return params.get("id");
}
const personID = getPersonIDFromURL();

function getPersonNationalNoFromURL() {
    const params = new URLSearchParams(window.location.search);
    return params.get("NationalNo");
}
const NationalNo = getPersonNationalNoFromURL();

// ── 2. جلب بيانات الشخص من API ───────────────────────────────────
async function loadPersonInfoByPersonID() {
    if (!personID) {
        showError("No person ID provided.");
        return;
    }
    // GET
    try {
        const response = await fetch(`https://localhost:7223/api/People/PeopleInfo/${personID}`);

        if (!response.ok) throw new Error("Person not found");

        const person = await response.json();
        renderPersonInfo(person);

    } catch (error) {
        console.error("Error:", error);
        showError("Failed to load person data.");
    }
}
async function loadPersonInfoByNationalNo() {
    if (!NationalNo) {
        showError("No person with NationalNo provided.");
        return;
    }
    // GET
    try {
        const response = await fetch(`https://localhost:7223/api/People/PeopleInfoByNationalNo/${NationalNo}`);

        if (!response.ok) throw new Error("Person not found");

        const person = await response.json();
        renderPersonInfo(person);

    } catch (error) {
        console.error("Error:", error);
        showError("Failed to load person data.");
    }
}
// ── 3. عرض البيانات في الصفحة ────────────────────────────────────
function renderPersonInfo(person) {
    const fullName = [person.firstName, person.secondName, person.thirdName, person.lastName].filter(Boolean).join(" ");

    // النصوص
    setValue(".id",          person.personID);
    setValue(".Name",        fullName);
    setValue(".NationalNo",  person.nationalNo);
    setValue(".Gendor",      person.gender);
    setValue(".Email",       person.email       || "—");
    setValue(".Address",     person.address      || "—");
    setValue(".DateOfBirth", formatDate(person.dateOfBirth));
    setValue(".Country",     person.nationality  || "—");

    // الصورة
    const img = document.querySelector(".personImage input[type='image']");
    if (img) {
        if (person.imagePath) {
            img.src = "https://localhost:7223" + person.imagePath;
        } else {
            img.src = "/images/defaultImage.webp"; // صورة افتراضية
        }
        img.alt = fullName;
    }
    // Badge للجنس
    const genderEl = document.querySelector(".Gendor");
    if (genderEl) {
        const isMale = person.gender === "Male";
        genderEl.className = `Gendor badge ${isMale ? "badge-male" : "badge-female"}`;
    }
}
// ── 4. Helper: تعيين قيمة عنصر ───────────────────────────────────
function setValue(selector, value) {
    const el = document.querySelector(selector);
    if (el) el.textContent = value;
}
// ── 5. Helper: تنسيق التاريخ ─────────────────────────────────────
function formatDate(dateStr) {
    if (!dateStr) return "—";
    const date = new Date(dateStr);
    if (isNaN(date)) return dateStr; // إذا كان التاريخ مرسلاً مُنسَّقاً مسبقاً
    const day   = String(date.getDate()).padStart(2, "0");
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const year  = date.getFullYear();
    return `${day}/${month}/${year}`;
}
// ── 6. عرض رسالة خطأ ─────────────────────────────────────────────
function showError(message) {
    const main = document.querySelector("main");
    if (!main) return;

    main.innerHTML = `
        <div class="title-bar"><h1>Person Details</h1></div>
        <div style="
            text-align: center;
            padding: 48px;
            color: var(--muted);
            font-size: 15px;
        ">
            <svg width="40" height="40" viewBox="0 0 20 20" fill="none"
                stroke="#a32d2d" stroke-width="1.6" style="margin-bottom:12px">
                <circle cx="10" cy="10" r="7.5"/>
                <line x1="10" y1="6.5" x2="10" y2="10.5"/>
                <circle cx="10" cy="13" r=".6" fill="#a32d2d" stroke="none"/>
            </svg>
            <p style="color:#a32d2d; font-weight:600;">${message}</p>
            <a href="/Sections/People/people.html"
                style="display:inline-block; margin-top:16px; color:var(--accent);">
                ← Back to People
            </a>
        </div>`;
}
// ── 7. تشغيل عند تحميل الصفحة ────────────────────────────────────
window.addEventListener("load", () => {
    const userNameElement = document.getElementById("username");
    const storedName = localStorage.getItem("UserName");

    if(userNameElement)
    {
        if(storedName)
        {
            userNameElement.textContent = storedName;
        }
        else 
        {
            userNameElement.textContent = "Guest";
        }
    }

    if(personID)
    {
        loadPersonInfoByPersonID();
    }
    else if(NationalNo)
    {
        loadPersonInfoByNationalNo();
    }
});