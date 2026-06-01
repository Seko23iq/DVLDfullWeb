// Get Person Info from server.
function getPersonIDfromURL()
{
    const para = new URLSearchParams(window.location.search);
    return para.get("id");
}
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
    loadPersonInfo();
});
async function loadPersonInfo()
{
    const personID = getPersonIDfromURL();
    if(!personID)
    {
        showError("No Person ID provided.");
        return;
    }
    try
    {
        const response = await fetch(`https://localhost:7223/api/People/PeopleInfo/${personID}`);

        if(!response.ok) throw new Error("Person not found!");

        const person = await response.json();
        renderPersonInfo(person);
        currentImagePath = person.imagePath; // ✅ احفظه

    }
    catch(error)
    {
        console.error("Error: ", error);
        showError("Failed to load person data.");
    }
}
function renderPersonInfo(person)
{
    setValue(".nationalNo", person.nationalNo);
    setValue(".firstName", person.firstName);
    setValue(".secondName", person.secondName);
    setValue(".thirdName", person.thirdName);
    setValue(".lastName", person.lastName);
    setValue(".dateOfBirth", formatDate(person.dateOfBirth));
    setValue(".email", person.email);
    setValue(".phone", person.phone);
    setValue(".address", person.address);

    setNationalitySelect(person.nationality);


    const img = document.querySelector("#imagePreview");
    if (img) {
        if (person.imagePath) {
            img.src = "https://localhost:7223" + person.imagePath;
            console.log(person.imagePath);
        } else {
            img.src = "/images/defaultImage.webp"; 
        }
        img.alt = person.firstName + ' ' + person.secondName + ' ' + person.thirdName + ' ' + person.lastName;
    }
    
    console.log(person);
    const gender = document.querySelector(".gender");
    gender.value = person.gender;
}
function setNationalitySelect(nationalityName)
{
    const select = document.getElementById("nationality");
    if (!select || !nationalityName) return;

    const option = [...select.options].find(
        o => o.text.toLowerCase() === nationalityName.toLowerCase()
    );

    select.value = option ? option.value : "";
}

function setValue(selector, value)
{
    const el = document.querySelector(selector);
    if(el) el.value = value;
}
function formatDate(dateStr)
{
    const date = new Date(dateStr);

    if (!isNaN(date.getTime())) {
        const formatted =
        date.getUTCFullYear() + "-" +
        String(date.getUTCMonth() + 1).padStart(2, "0") + "-" +
        String(date.getUTCDate()).padStart(2, "0");

    return formatted;
    }
}
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


// Send Person Info to server.
const personID = getPersonIDfromURL();
const API_URL = `https://localhost:7223/api/People/UpdatePerson/${personID}`;

// ── DOM refs ──────────────────────────────────
const btnSave      = document.getElementById("btnSave");
const imageInput   = document.getElementById("imageInput");
const imagePreview = document.getElementById("imagePreview");
const toast        = document.getElementById("toast");

// ── Image preview ─────────────────────────────
imageInput.addEventListener("change", function () {
    const file = this.files[0];
    if (!file) return;

    if (file.size > 5 * 1024 * 1024) {
        showToast("Image must be smaller than 5 MB.", "error");
        this.value = "";
        return;
    }

    const reader = new FileReader();
    reader.onload = e => imagePreview.src = e.target.result;
    reader.readAsDataURL(file);
});

// ── Validation rules ──────────────────────────
const rules = {
    nationalNo:  { required: true,  pattern: /^[a-zA-Z0-9]+$/,                       message: "Letters and numbers only." },
    gender:      { required: true,                                                     message: "Please select a gender." },
    firstName:   { required: true,  pattern: /^[a-zA-Z\u0600-\u06FF\s]+$/,            message: "Letters only." },
    secondName:  { required: true,  pattern: /^[a-zA-Z\u0600-\u06FF\s]+$/,            message: "Letters only." },
    thirdName:   { required: false, pattern: /^[a-zA-Z\u0600-\u06FF\s]*$/,            message: "Letters only." },
    lastName:    { required: true,  pattern: /^[a-zA-Z\u0600-\u06FF\s]+$/,            message: "Letters only." },
    dateOfBirth: { required: true,                                                     message: "Date of birth is required." },
    // nationality: { required: true,  pattern: /^[a-zA-Z\u0600-\u06FF\s]+$/,            message: "Letters only." },
    address:     { required: false },
    phone:       { required: false, pattern: /^\+?[0-9\s\-().]{7,20}$/,               message: "Invalid phone number." },
    email:       { required: false, pattern: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,            message: "Invalid email format." },
};

// ── Validate single field ─────────────────────
function validateField(id) {
    const rule  = rules[id];
    if (!rule) return true;

    const el    = document.getElementById(id);
    if (!el) return true;

    const errEl = document.getElementById(`err-${id}`);
    const val   = el.value.trim();

    let error = "";

    if (rule.required && val === "") {
        error = "This field is required.";
    } else if (rule.pattern && val !== "" && !rule.pattern.test(val)) {
        error = rule.message;
    }

    if (error) {
        el.classList.add("invalid");
        el.classList.remove("valid");
        if (errEl) errEl.textContent = error;
        return false;
    } else {
        el.classList.remove("invalid");
        if (val !== "") el.classList.add("valid");
        if (errEl) errEl.textContent = "";
        return true;
    }
}

// ── Live validation ───────────────────────────
Object.keys(rules).forEach(id => {
    const el = document.getElementById(id);
    if (!el) return;
    el.addEventListener("blur",  () => validateField(id));
    el.addEventListener("input", () => {
        if (el.classList.contains("invalid")) validateField(id);
    });
});

// ── Validate all ─────────────────────────────
function validateAll() {
    return Object.keys(rules)
        .map(id => validateField(id))
        .every(Boolean);
}

let currentImagePath = null;

// ── Build payload — يطابق JSON بالضبط ────────
function buildPayload() {
    return {
        nationalNo:  document.getElementById("nationalNo").value.trim(),
        firstName:   document.getElementById("firstName").value.trim(),
        secondName:  document.getElementById("secondName").value.trim(),
        thirdName:   document.getElementById("thirdName").value.trim()  || null,
        lastName:    document.getElementById("lastName").value.trim(),
        dateOfBirth: document.getElementById("dateOfBirth").value,
        gender:      document.getElementById("gender").value,
        address:     document.getElementById("address").value.trim()    || null,
        phone:       document.getElementById("phone").value.trim()      || null,
        email:       document.getElementById("email").value.trim()      || null,
        nationality: document.getElementById("nationality").value.trim(),
        imagePath:   currentImagePath, 

    };
}

// ── Reset button to original state ───────────
function resetBtn() {
    btnSave.disabled = false;
    btnSave.innerHTML = `
        <svg viewBox="0 0 20 20" fill="none" stroke="currentColor" stroke-width="1.8">
            <path d="M4 10.5l4.5 4.5 7.5-8"/>
        </svg>
        Save Person`;
}

// ── Save ──────────────────────────────────────
btnSave.addEventListener("click", async () => {
    if (!validateAll()) {
        showToast("Please fix the errors before saving.", "error");
        return;
    }

    btnSave.disabled = true;
    btnSave.innerHTML = `
        <svg viewBox="0 0 20 20" fill="none" stroke="currentColor"
             stroke-width="1.8" style="animation:spin .7s linear infinite;width:15px;height:15px">
            <circle cx="10" cy="10" r="7" stroke-dasharray="30" stroke-dashoffset="10"/>
        </svg>
        Saving…`;

    try {
        const payload = buildPayload();
        const file    = imageInput.files[0];

        console.log(payload);
        const formData = new FormData();

        Object.entries(payload).forEach(([k, v]) => {
            if (k === 'imagePath') {
                formData.append('imagePath', v ?? "");
            } else {
                formData.append(k, v ?? "");
            }
        });

        if (file) {
            formData.append("image", file);
        } 

        const response = await fetch(API_URL, {
            method: "PUT",
            body: formData,
        });

        if (!response.ok) {
            let errorMessage = `Server error (${response.status})`;
            try {
                const errBody = await response.json();
                console.error("Validation errors:", JSON.stringify(errBody, null, 2));
                errorMessage = errBody.message
                    || errBody.title
                    || JSON.stringify(errBody.errors ?? errBody)
                    || errorMessage;
            } catch {
                errorMessage = response.statusText || errorMessage;
            }
            throw new Error(errorMessage);
        }

        showToast("Person updated successfully!", "success");

        setTimeout(() => {
            window.location.href = "/Sections/People/people.html";
        }, 1400);

    } catch (error) {
        console.error("Save Error:", error);
        showToast(error.message || "Failed to update person.", "error");
        resetBtn();
    }
});

// ── Toast helper ──────────────────────────────
let toastTimer;
function showToast(message, type = "success") {
    clearTimeout(toastTimer);
    toast.textContent = message;
    toast.className   = `toast ${type} show`;
    toastTimer = setTimeout(() => {
        toast.classList.remove("show");
    }, 3000);
}

// ── Spinner keyframe (inject once) ───────────
const style = document.createElement("style");
style.textContent = `@keyframes spin { to { transform: rotate(360deg); } }`;
document.head.appendChild(style);


async function loadCountries() {
    try {
        const response = await fetch("https://localhost:7223/api/Country/AllCountries");
        const countries = await response.json();

        const select = document.getElementById("nationality");
        countries.forEach((country, index) => {
            const option = document.createElement("option");
            option.value = index + 1; // ← index يبدأ من 1
            option.textContent = country;
            select.appendChild(option);
        });

    } catch (error) {
        console.error("Failed to load countries:", error);
    }
}
// استدعها عند تحميل الصفحة
document.addEventListener("DOMContentLoaded", () => {
    loadCountries();
});

function getNationalityName() {
    const select = document.getElementById('nationality');
    if (!select) return "";
    const option = select.options[select.selectedIndex];
    return option ? option.text : "";
}