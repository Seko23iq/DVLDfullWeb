// ─── currentUserInfo.js ───────────────────────────────────────────

// ── 2. جلب بيانات المستخدم من API ────────────────────────────────
async function loadUserInfo() {
    const userID = localStorage.getItem("UserID");
    if (!userID) {
        showError("No user ID provided.");
        return;
    }
    try {
        const response = await fetch(`https://localhost:7223/api/Users/UserInfo/${userID}`);

        if (!response.ok) throw new Error("User not found");

        const data = await response.json();

        renderPersonInfo(data.person);
        renderUserInfo(data.user);

    } catch (error) {
        console.error("Error:", error);
        showError("Failed to load user data.");
    }
}

// ── 3. عرض بيانات الشخص ──────────────────────────────────────────
function renderPersonInfo(person) {
    if (!person) return;

    const fullName = [person.firstName, person.secondName, person.thirdName, person.lastName]
                     .filter(Boolean).join(" ");

    setValue(".id",          person.personID);
    setValue(".Name",        fullName);
    setValue(".NationalNo",  person.nationalNo);
    setValue(".Gendor",      person.gender);
    setValue(".Email",       person.email      || "—");
    setValue(".Address",     person.address     || "—");
    setValue(".DateOfBirth", formatDate(person.dateOfBirth));
    setValue(".Country",     person.nationality || "—");

    // الصورة
    const img = document.querySelector(".personImage input[type='image']");
    if (img && person.imagePath) {
        img.src = "https://localhost:7223" + person.imagePath;
        img.alt = fullName;
    }

    console.log(person);
    // Badge للجنس
    const genderEl = document.querySelector(".Gendor");
    if (genderEl) {
        const isMale = person.gender === "Male";
        genderEl.className = `Gendor badge ${isMale ? "badge-male" : "badge-female"}`;
    }
}

// ── 4. عرض بيانات المستخدم ───────────────────────────────────────
function renderUserInfo(user) {
    if (!user) return;

    setValue(".uid",   user.userID);
    setValue(".uname", user.userName);

    // IsActive badge
    const isActiveEl = document.querySelector(".isActive");
    if (isActiveEl) {
        const active = user.isActive === true || user.isActive === 1;
        isActiveEl.textContent = active ? "Active" : "Inactive";
        isActiveEl.className   = `isActive ${active ? "active-badge" : "inactive-badge"}`;
    }
}

// ── 5. Helper: تعيين قيمة عنصر ───────────────────────────────────
function setValue(selector, value) {
    const el = document.querySelector(selector);
    if (el) el.textContent = value ?? "—";
}

// ── 6. Helper: تنسيق التاريخ ─────────────────────────────────────
function formatDate(dateStr) {
    if (!dateStr) return "—";
    const date = new Date(dateStr);
    if (isNaN(date)) return dateStr;
    const day   = String(date.getDate()).padStart(2, "0");
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const year  = date.getFullYear();
    return `${day}/${month}/${year}`;
}

// ── 7. عرض رسالة خطأ ─────────────────────────────────────────────
function showError(message) {
    const main = document.querySelector("main");
    if (!main) return;

    main.innerHTML = `
        <div class="title-bar"><h1>User Details</h1></div>
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
            <a href="/Sections/Users/users.html"
                style="display:inline-block; margin-top:16px; color:var(--accent);">
                ← Back to Users
            </a>
        </div>`;
}